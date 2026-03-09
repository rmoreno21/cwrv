// Infrastructure Layer - DIContainer
import { IPolizaRepository } from '../../domain/interfaces/IPolizaRepository';
import { IAuthService, ILogger } from '../../domain/interfaces/IAuthService';
import { PolizaRepository } from '../repositories/PolizaRepository';
import { AuthService } from '../services/AuthService';
import { LoggerService } from '../services/LoggerService';
import { PolizaValidationService } from '../../domain/services/PolizaValidationService';
import { TransmitirPolizaUseCase } from '../../application/usecases/TransmitirPolizaUseCase';
import { PolizaController } from '../../presentation/controllers/PolizaController';

export class DIContainer {
    private static instance: DIContainer;
    private dependencies: Map<string, any> = new Map();

    private constructor() {}

    public static getInstance(): DIContainer {
        if (!DIContainer.instance) {
            DIContainer.instance = new DIContainer();
        }
        return DIContainer.instance;
    }

    public register<T>(key: string, factory: () => T): void {
        this.dependencies.set(key, factory);
    }

    public resolve<T>(key: string): T {
        const factory = this.dependencies.get(key);
        if (!factory) {
            throw new Error(`Dependencia no registrada: ${key}`);
        }
        return factory();
    }

    public configurar(dbConnection: any, sessionManager: any, loggerInstance: any): void {
        // Registrar servicios de infraestructura
        this.register<ILogger>('ILogger', () => new LoggerService(loggerInstance));
        this.register<IAuthService>('IAuthService', () => new AuthService(sessionManager));
        this.register<IPolizaRepository>('IPolizaRepository', () => 
            new PolizaRepository(this.resolve<ILogger>('ILogger'), dbConnection)
        );

        // Registrar servicios de dominio
        this.register<PolizaValidationService>('PolizaValidationService', () => 
            new PolizaValidationService()
        );

        // Registrar casos de uso
        this.register<TransmitirPolizaUseCase>('TransmitirPolizaUseCase', () => 
            new TransmitirPolizaUseCase(
                this.resolve<IPolizaRepository>('IPolizaRepository'),
                this.resolve<IAuthService>('IAuthService'),
                this.resolve<ILogger>('ILogger'),
                this.resolve<PolizaValidationService>('PolizaValidationService')
            )
        );

        // Registrar controladores
        this.register<PolizaController>('PolizaController', () => 
            new PolizaController(
                this.resolve<TransmitirPolizaUseCase>('TransmitirPolizaUseCase'),
                this.resolve<ILogger>('ILogger')
            )
        );
    }
} 