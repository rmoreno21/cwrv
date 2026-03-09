// Domain Value Object - Respuesta
export class Respuesta {
    constructor(
        public readonly estado: string,
        public readonly mensaje: string,
        public readonly titulo: string,
        public readonly icono: string,
        public readonly datos?: any
    ) {}

    public static exito(mensaje: string, datos?: any): Respuesta {
        return new Respuesta('OK', mensaje, 'Éxito', 'success', datos);
    }

    public static error(mensaje: string, titulo: string = 'Error'): Respuesta {
        return new Respuesta('ERROR', mensaje, titulo, 'error');
    }

    public static validacion(mensaje: string): Respuesta {
        return new Respuesta('VALIDACION', mensaje, 'Validación', 'warning');
    }

    public esExitoso(): boolean {
        return this.estado === 'OK';
    }

    public esError(): boolean {
        return this.estado === 'ERROR';
    }

    public esValidacion(): boolean {
        return this.estado === 'VALIDACION';
    }
} 