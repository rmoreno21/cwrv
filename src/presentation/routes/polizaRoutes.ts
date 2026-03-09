// Presentation Layer - Routes
import { Router } from 'express';
import { DIContainer } from '../../infrastructure/ioc/DIContainer';
import { PolizaController } from '../controllers/PolizaController';

export const polizaRoutes = Router();

const container = DIContainer.getInstance();

// Middleware para autenticación (opcional)
const authMiddleware = (req: any, res: any, next: any) => {
    // Implementar validación de sesión si es necesario
    // Por ahora, simplemente continúa
    next();
};

// POST /api/polizas/transmitir
polizaRoutes.post('/transmitir', authMiddleware, async (req, res) => {
    const polizaController = container.resolve<PolizaController>('PolizaController');
    await polizaController.transmitirPoliza(req, res);
});

// Ejemplo de otras rutas que podrías necesitar
polizaRoutes.get('/healthcheck', (req, res) => {
    res.json({ 
        status: 'OK', 
        timestamp: new Date().toISOString(),
        service: 'Poliza Service'
    });
}); 