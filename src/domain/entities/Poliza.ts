// Domain Entity - Poliza
export class Poliza {
    constructor(
        public readonly numeroPoliza: string,
        public readonly numeroSolicitud: string,
        public readonly numeroCorrelativo: number,
        public readonly montoCompania: number,
        public readonly estado: string
    ) {}

    public validarMontoConAbono(montoAbono: number): boolean {
        return this.montoCompania === montoAbono;
    }

    public tieneMontoMenorAlAbono(montoAbono: number): boolean {
        return this.montoCompania > montoAbono;
    }

    public tieneMontoMayorAlAbono(montoAbono: number): boolean {
        return this.montoCompania < montoAbono;
    }
} 