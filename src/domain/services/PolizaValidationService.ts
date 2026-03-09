// Domain Service - PolizaValidationService
import { Poliza } from '../entities/Poliza';
import { Abono } from '../entities/Abono';
import { Respuesta } from '../value-objects/Respuesta';

export class PolizaValidationService {
    public validarRecaudacionPoliza(poliza: Poliza, abono: Abono | null): Respuesta | null {
        if (!abono || !abono.estaRecaudado()) {
            return Respuesta.validacion(
                `La poliza ${poliza.numeroPoliza} a emitir no tiene el abono recaudado.`
            );
        }

        if (poliza.tieneMontoMayorAlAbono(abono.valorPesosAbono)) {
            return Respuesta.validacion(
                `La poliza ${poliza.numeroPoliza} a emitir tiene el monto recaudado mayor al que se debe cobrar.`
            );
        }

        if (poliza.tieneMontoMenorAlAbono(abono.valorPesosAbono)) {
            if (!abono.tienePrimeraPrimaPagada()) {
                return Respuesta.validacion(
                    `La poliza ${poliza.numeroPoliza} a emitir no tiene recaudada primera prima.`
                );
            }
            return Respuesta.validacion(
                `La poliza ${poliza.numeroPoliza} a emitir tiene el monto recaudado menor al que se debe cobrar.`
            );
        }

        return null; // No hay errores de validación
    }
} 