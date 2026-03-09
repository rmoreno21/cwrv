// Infrastructure Layer - LoggerService
import { ILogger } from '../../domain/interfaces/IAuthService';

export class LoggerService implements ILogger {
    constructor(
        private readonly loggerInstance: any // Inyectar tu instancia de logger (winston, etc.)
    ) {}

    public info(message: string): void {
        this.loggerInstance.info(message);
        console.log(`[INFO] ${new Date().toISOString()}: ${message}`);
    }

    public warn(message: string): void {
        this.loggerInstance.warn(message);
        console.warn(`[WARN] ${new Date().toISOString()}: ${message}`);
    }

    public error(message: string, error?: Error): void {
        const errorInfo = error ? `\nError: ${error.message}\nStack: ${error.stack}` : '';
        const fullMessage = `${message}${errorInfo}`;
        
        this.loggerInstance.error(fullMessage);
        console.error(`[ERROR] ${new Date().toISOString()}: ${fullMessage}`);
    }
} 