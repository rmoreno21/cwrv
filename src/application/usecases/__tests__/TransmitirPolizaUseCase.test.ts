// Test - TransmitirPolizaUseCase
import { TransmitirPolizaUseCase, TransmitirPolizaRequest } from '../TransmitirPolizaUseCase';
import { IPolizaRepository } from '../../../domain/interfaces/IPolizaRepository';
import { IAuthService, ILogger } from '../../../domain/interfaces/IAuthService';
import { PolizaValidationService } from '../../../domain/services/PolizaValidationService';
import { Poliza } from '../../../domain/entities/Poliza';
import { Abono } from '../../../domain/entities/Abono';
import { Respuesta } from '../../../domain/value-objects/Respuesta';

describe('TransmitirPolizaUseCase', () => {
    let useCase: TransmitirPolizaUseCase;
    let mockPolizaRepository: jest.Mocked<IPolizaRepository>;
    let mockAuthService: jest.Mocked<IAuthService>;
    let mockLogger: jest.Mocked<ILogger>;
    let mockValidationService: jest.Mocked<PolizaValidationService>;

    beforeEach(() => {
        // Configurar mocks
        mockPolizaRepository = {
            obtenerDatosPolizaRV: jest.fn(),
            obtenerAbonoPorPoliza: jest.fn(),
            transferirDatosPolizaRRVV: jest.fn(),
            actualizarCotizacionGanadoraRVI: jest.fn()
        };

        mockAuthService = {
            validarTokenUsuario: jest.fn(),
            validarPermiso: jest.fn(),
            obtenerUsuarioActual: jest.fn()
        };

        mockLogger = {
            info: jest.fn(),
            warn: jest.fn(),
            error: jest.fn()
        };

        mockValidationService = {
            validarRecaudacionPoliza: jest.fn()
        } as any;

        useCase = new TransmitirPolizaUseCase(
            mockPolizaRepository,
            mockAuthService,
            mockLogger,
            mockValidationService
        );
    });

    describe('Cuando se transmite una póliza exitosamente', () => {
        it('debería devolver una respuesta exitosa', async () => {
            // Arrange
            const request: TransmitirPolizaRequest = {
                tokenUsuario: 'token-valido',
                numeroSolicitud: '12345',
                numeroCorrelativo: 1,
                numeroPoliza: 'POL-12345',
                sessionToken: 'token-valido',
                opcionesSistema: ['CierrePolizaRVI']
            };

            const mockPoliza = new Poliza('POL-12345', '12345', 1, 1000, 'ACTIVA');
            const mockAbono = new Abono('POL-12345', 'IMP-001', 1000, new Date());

            mockAuthService.validarTokenUsuario.mockReturnValue(true);
            mockAuthService.validarPermiso.mockReturnValue(true);
            mockAuthService.obtenerUsuarioActual.mockReturnValue('usuario-test');
            mockPolizaRepository.obtenerAbonoPorPoliza.mockResolvedValue(mockAbono);
            mockPolizaRepository.obtenerDatosPolizaRV.mockResolvedValue(mockPoliza);
            mockValidationService.validarRecaudacionPoliza.mockReturnValue(null);
            mockPolizaRepository.transferirDatosPolizaRRVV.mockResolvedValue(true);
            mockPolizaRepository.actualizarCotizacionGanadoraRVI.mockResolvedValue();

            // Act
            const resultado = await useCase.ejecutar(request);

            // Assert
            expect(resultado.esExitoso()).toBe(true);
            expect(resultado.mensaje).toBe('Póliza transmitida exitosamente');
            expect(mockPolizaRepository.obtenerAbonoPorPoliza).toHaveBeenCalledWith(12345, 'usuario-test');
            expect(mockPolizaRepository.obtenerDatosPolizaRV).toHaveBeenCalledWith('12345', 1, 'usuario-test');
            expect(mockPolizaRepository.transferirDatosPolizaRRVV).toHaveBeenCalledWith('12345', 1, 'usuario-test');
            expect(mockPolizaRepository.actualizarCotizacionGanadoraRVI).toHaveBeenCalledWith('12345', 1, 'usuario-test');
        });
    });

    describe('Cuando el token de usuario es inválido', () => {
        it('debería devolver un error de autorización', async () => {
            // Arrange
            const request: TransmitirPolizaRequest = {
                tokenUsuario: 'token-invalido',
                numeroSolicitud: '12345',
                numeroCorrelativo: 1,
                numeroPoliza: 'POL-12345',
                sessionToken: 'token-valido',
                opcionesSistema: ['CierrePolizaRVI']
            };

            mockAuthService.validarTokenUsuario.mockReturnValue(false);

            // Act
            const resultado = await useCase.ejecutar(request);

            // Assert
            expect(resultado.esError()).toBe(true);
            expect(resultado.mensaje).toBe('Token de usuario inválido');
            expect(resultado.titulo).toBe('No autorizado');
        });
    });

    describe('Cuando el usuario no tiene permisos', () => {
        it('debería devolver un error de acceso denegado', async () => {
            // Arrange
            const request: TransmitirPolizaRequest = {
                tokenUsuario: 'token-valido',
                numeroSolicitud: '12345',
                numeroCorrelativo: 1,
                numeroPoliza: 'POL-12345',
                sessionToken: 'token-valido',
                opcionesSistema: []
            };

            mockAuthService.validarTokenUsuario.mockReturnValue(true);
            mockAuthService.validarPermiso.mockReturnValue(false);
            mockAuthService.obtenerUsuarioActual.mockReturnValue('usuario-test');

            // Act
            const resultado = await useCase.ejecutar(request);

            // Assert
            expect(resultado.esError()).toBe(true);
            expect(resultado.mensaje).toBe('No tiene permisos para realizar esta acción');
            expect(resultado.titulo).toBe('Acceso denegado');
        });
    });

    describe('Cuando hay un error de validación de póliza', () => {
        it('debería devolver el error de validación', async () => {
            // Arrange
            const request: TransmitirPolizaRequest = {
                tokenUsuario: 'token-valido',
                numeroSolicitud: '12345',
                numeroCorrelativo: 1,
                numeroPoliza: 'POL-12345',
                sessionToken: 'token-valido',
                opcionesSistema: ['CierrePolizaRVI']
            };

            const mockPoliza = new Poliza('POL-12345', '12345', 1, 1000, 'ACTIVA');
            const mockAbono = null;
            const errorValidacion = Respuesta.validacion('La póliza no tiene abono recaudado');

            mockAuthService.validarTokenUsuario.mockReturnValue(true);
            mockAuthService.validarPermiso.mockReturnValue(true);
            mockAuthService.obtenerUsuarioActual.mockReturnValue('usuario-test');
            mockPolizaRepository.obtenerAbonoPorPoliza.mockResolvedValue(mockAbono);
            mockPolizaRepository.obtenerDatosPolizaRV.mockResolvedValue(mockPoliza);
            mockValidationService.validarRecaudacionPoliza.mockReturnValue(errorValidacion);

            // Act
            const resultado = await useCase.ejecutar(request);

            // Assert
            expect(resultado.esValidacion()).toBe(true);
            expect(resultado.mensaje).toBe('La póliza no tiene abono recaudado');
        });
    });

    describe('Cuando ocurre un error inesperado', () => {
        it('debería devolver un error genérico', async () => {
            // Arrange
            const request: TransmitirPolizaRequest = {
                tokenUsuario: 'token-valido',
                numeroSolicitud: '12345',
                numeroCorrelativo: 1,
                numeroPoliza: 'POL-12345',
                sessionToken: 'token-valido',
                opcionesSistema: ['CierrePolizaRVI']
            };

            mockAuthService.validarTokenUsuario.mockReturnValue(true);
            mockAuthService.validarPermiso.mockReturnValue(true);
            mockAuthService.obtenerUsuarioActual.mockReturnValue('usuario-test');
            mockPolizaRepository.obtenerAbonoPorPoliza.mockRejectedValue(new Error('Error de base de datos'));

            // Act
            const resultado = await useCase.ejecutar(request);

            // Assert
            expect(resultado.esError()).toBe(true);
            expect(resultado.mensaje).toBe('Se ha producido un error al transmitir la póliza');
            expect(mockLogger.error).toHaveBeenCalled();
        });
    });
}); 