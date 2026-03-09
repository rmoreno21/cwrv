// Domain Entity - Abono
export class Abono {
    constructor(
        public readonly numeroPoliza: string,
        public readonly numeroImputacionMov: string | null,
        public readonly valorPesosAbono: number,
        public readonly fechaAbono: Date
    ) {}

    public estaRecaudado(): boolean {
        return this.numeroImputacionMov !== null && this.numeroImputacionMov !== undefined;
    }

    public tienePrimeraPrimaPagada(): boolean {
        return this.valorPesosAbono > 0;
    }

    public esMontoValido(): boolean {
        return this.valorPesosAbono >= 0;
    }
} 