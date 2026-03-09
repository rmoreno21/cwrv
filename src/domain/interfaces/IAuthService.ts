// Domain Interface - IAuthService
export interface IAuthService {
    validarTokenUsuario(token: string, sessionToken: string): boolean;
    validarPermiso(opcionesSistema: any, opcionRequerida: string): boolean;
    obtenerUsuarioActual(): string;
}

export interface ILogger {
    info(message: string): void;
    warn(message: string): void;
    error(message: string, error?: Error): void;
} 