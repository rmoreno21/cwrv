// Domain Interface - IPolizaRepository
import { Poliza } from '../entities/Poliza';
import { Abono } from '../entities/Abono';

export interface IPolizaRepository {
    obtenerDatosPolizaRV(numeroSolicitud: string, numeroCorrelativo: number, usuario: string): Promise<Poliza>;
    obtenerAbonoPorPoliza(numeroPoliza: number, usuario: string): Promise<Abono | null>;
    transferirDatosPolizaRRVV(numeroSolicitud: string, numeroCorrelativo: number, usuario: string): Promise<boolean>;
    actualizarCotizacionGanadoraRVI(numeroSolicitud: string, numeroCorrelativo: number, usuario: string): Promise<void>;
} 