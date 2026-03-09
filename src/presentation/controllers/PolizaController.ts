// Presentation Layer - PolizaController
import { Request, Response } from 'express';
import { TransmitirPolizaUseCase, TransmitirPolizaRequest } from '../../application/usecases/TransmitirPolizaUseCase';
import { ILogger } from '../../domain/interfaces/IAuthService';

export class PolizaController {
    constructor(
        private readonly transmitirPolizaUseCase: TransmitirPolizaUseCase,
        private readonly logger: ILogger
    ) {}

    public async transmitirPoliza(req: Request, res: Response): Promise<void> {
        try {
            const { tokenUsuario, numeroSolicitud, numeroCorrelativo, numeroPoliza } = req.body;
            
            // Validar parámetros requeridos
            if (!tokenUsuario || !numeroSolicitud || !numeroCorrelativo || !numeroPoliza) {
                res.status(400).json({
                    estado: 'ERROR',
                    mensaje: 'Faltan parámetros requeridos',
                    titulo: 'Error de validación',
                    icono: 'error'
                });
                return;
            }

            // Construir el request para el caso de uso
            const useCaseRequest: TransmitirPolizaRequest = {
                tokenUsuario,
                numeroSolicitud,
                numeroCorrelativo: parseInt(numeroCorrelativo),
                numeroPoliza,
                sessionToken: req.session?.tokenUsuario || '',
                opcionesSistema: req.session?.opcionesSistema || []
            };

            // Ejecutar el caso de uso
            const respuesta = await this.transmitirPolizaUseCase.ejecutar(useCaseRequest);

            // Mapear respuesta del dominio a respuesta HTTP
            const httpStatusCode = this.mapearStatusCode(respuesta.estado);
            
            res.status(httpStatusCode).json({
                estado: respuesta.estado,
                mensaje: respuesta.mensaje,
                titulo: respuesta.titulo,
                icono: respuesta.icono,
                datos: respuesta.datos
            });

        } catch (error) {
            this.logger.error(`Error en TransmitirPoliza: ${error.message}`, error);
            
            res.status(500).json({
                estado: 'ERROR',
                mensaje: 'Error interno del servidor',
                titulo: 'Error',
                icono: 'error'
            });
        }
    }

    private mapearStatusCode(estado: string): number {
        switch (estado) {
            case 'OK':
                return 200;
            case 'VALIDACION':
                return 400;
            case 'ERROR':
                return 500;
            default:
                return 500;
        }
    }
} 