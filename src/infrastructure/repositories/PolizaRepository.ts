// Infrastructure Layer - PolizaRepository
import { IPolizaRepository } from '../../domain/interfaces/IPolizaRepository';
import { Poliza } from '../../domain/entities/Poliza';
import { Abono } from '../../domain/entities/Abono';
import { ILogger } from '../../domain/interfaces/IAuthService';

export class PolizaRepository implements IPolizaRepository {
    constructor(
        private readonly logger: ILogger,
        private readonly dbConnection: any // Inyectar tu conexión de base de datos
    ) {}

    public async obtenerDatosPolizaRV(numeroSolicitud: string, numeroCorrelativo: number, usuario: string): Promise<Poliza> {
        try {
            // Ejemplo de implementación con tu base de datos
            const query = `
                SELECT numero_poliza, numero_solicitud, numero_correlativo, val_mto_cia, estado
                FROM polizas_rv 
                WHERE numero_solicitud = ? AND numero_correlativo = ?
            `;
            
            const result = await this.dbConnection.execute(query, [numeroSolicitud, numeroCorrelativo]);
            
            if (!result || result.length === 0) {
                throw new Error(`No se encontró la póliza para solicitud ${numeroSolicitud} y correlativo ${numeroCorrelativo}`);
            }

            const row = result[0];
            return new Poliza(
                row.numero_poliza,
                row.numero_solicitud,
                row.numero_correlativo,
                row.val_mto_cia,
                row.estado
            );

        } catch (error) {
            this.logger.error(`Error al obtener datos de póliza RV: ${error.message}`, error);
            throw error;
        }
    }

    public async obtenerAbonoPorPoliza(numeroPoliza: number, usuario: string): Promise<Abono | null> {
        try {
            const query = `
                SELECT numero_poliza, num_imputacion_mov, val_pesos_abono, fecha_abono
                FROM abonos_poliza 
                WHERE numero_poliza = ?
            `;
            
            const result = await this.dbConnection.execute(query, [numeroPoliza]);
            
            if (!result || result.length === 0) {
                return null;
            }

            const row = result[0];
            return new Abono(
                row.numero_poliza.toString(),
                row.num_imputacion_mov,
                row.val_pesos_abono,
                new Date(row.fecha_abono)
            );

        } catch (error) {
            this.logger.error(`Error al obtener abono por póliza: ${error.message}`, error);
            throw error;
        }
    }

    public async transferirDatosPolizaRRVV(numeroSolicitud: string, numeroCorrelativo: number, usuario: string): Promise<boolean> {
        try {
            // Implementar la lógica de transferencia específica de tu sistema
            const query = `
                CALL sp_transferir_datos_poliza_rrvv(?, ?, ?)
            `;
            
            await this.dbConnection.execute(query, [numeroSolicitud, numeroCorrelativo, usuario]);
            return true;

        } catch (error) {
            this.logger.error(`Error al transferir datos de póliza RRVV: ${error.message}`, error);
            throw error;
        }
    }

    public async actualizarCotizacionGanadoraRVI(numeroSolicitud: string, numeroCorrelativo: number, usuario: string): Promise<void> {
        try {
            const query = `
                UPDATE cotizaciones_rvi 
                SET estado = 'GANADORA', fecha_actualizacion = NOW(), usuario_actualizacion = ?
                WHERE numero_solicitud = ? AND numero_correlativo = ?
            `;
            
            await this.dbConnection.execute(query, [usuario, numeroSolicitud, numeroCorrelativo]);

        } catch (error) {
            this.logger.error(`Error al actualizar cotización ganadora RVI: ${error.message}`, error);
            throw error;
        }
    }
} 