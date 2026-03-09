// Application Layer - TransmitirPolizaUseCase
import { IPolizaRepository } from '../../domain/interfaces/IPolizaRepository';
import { IAuthService, ILogger } from '../../domain/interfaces/IAuthService';
import { PolizaValidationService } from '../../domain/services/PolizaValidationService';
import { Respuesta } from '../../domain/value-objects/Respuesta';

export interface TransmitirPolizaRequest {
    tokenUsuario: string;
    numeroSolicitud: string;
    numeroCorrelativo: number;
    numeroPoliza: string;
    sessionToken: string;
    opcionesSistema: any;
}

export class TransmitirPolizaUseCase {
    constructor(
        private readonly polizaRepository: IPolizaRepository,
        private readonly authService: IAuthService,
        private readonly logger: ILogger,
        private readonly validationService: PolizaValidationService
    ) {}

    public async ejecutar(request: TransmitirPolizaRequest): Promise<Respuesta> {
        try {
            // Validar token de usuario
            if (!this.authService.validarTokenUsuario(request.tokenUsuario, request.sessionToken)) {
                this.logger.error("Token de usuario no coincide con el de la sesión original");
                return Respuesta.error("Token de usuario inválido", "No autorizado");
            }

            // Validar permisos
            if (!this.authService.validarPermiso(request.opcionesSistema, 'CierrePolizaRVI')) {
                const usuario = this.authService.obtenerUsuarioActual();
                this.logger.error(`Usuario [${usuario}] ha intentado acceder a la opción [CierrePolizaRVI] que no corresponde a su rol.`);
                return Respuesta.error("No tiene permisos para realizar esta acción", "Acceso denegado");
            }

            const usuario = this.authService.obtenerUsuarioActual();

            // Obtener datos de abono y póliza
            const [abono, poliza] = await Promise.all([
                this.polizaRepository.obtenerAbonoPorPoliza(parseInt(request.numeroPoliza), usuario),
                this.polizaRepository.obtenerDatosPolizaRV(request.numeroSolicitud, request.numeroCorrelativo, usuario)
            ]);

            // Validar recaudación
            const errorValidacion = this.validationService.validarRecaudacionPoliza(poliza, abono);
            if (errorValidacion) {
                return errorValidacion;
            }

            // Transferir datos de la póliza
            const transferencia = await this.polizaRepository.transferirDatosPolizaRRVV(
                request.numeroSolicitud, 
                request.numeroCorrelativo, 
                usuario
            );

            if (!transferencia) {
                return Respuesta.error("Error al transferir los datos de la póliza");
            }

            // Actualizar cotización ganadora
            await this.polizaRepository.actualizarCotizacionGanadoraRVI(
                request.numeroSolicitud, 
                request.numeroCorrelativo, 
                usuario
            );

            this.logger.info(`Póliza ${request.numeroPoliza} transmitida exitosamente`);
            return Respuesta.exito("Póliza transmitida exitosamente");

        } catch (error) {
            this.logger.error(`Error al transmitir póliza: ${error.message}`, error);
            return Respuesta.error("Se ha producido un error al transmitir la póliza");
        }
    }
} 