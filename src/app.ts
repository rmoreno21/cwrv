// Application Entry Point
import express from 'express';
import { DIContainer } from './infrastructure/ioc/DIContainer';
import { polizaRoutes } from './presentation/routes/polizaRoutes';

const app = express();
const port = process.env.PORT || 3000;

// Configurar middleware
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Configurar CORS (opcional)
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept, Authorization');
    next();
});

// Configurar contenedor de dependencias
const container = DIContainer.getInstance();

// Aquí deberías configurar tus conexiones reales
const dbConnection = {
    // Tu conexión a base de datos (mysql2, pg, etc.)
    execute: async (query: string, params: any[]) => {
        // Implementar conexión real
        console.log('Ejecutando query:', query, 'con parámetros:', params);
        return [];
    }
};

const sessionManager = {
    getCurrentUser: () => 'current-user'
};

const loggerInstance = {
    info: (message: string) => console.log(message),
    warn: (message: string) => console.warn(message),
    error: (message: string) => console.error(message)
};

container.configurar(dbConnection, sessionManager, loggerInstance);

// Configurar rutas
app.use('/api/polizas', polizaRoutes);

// Ruta raíz
app.get('/', (req, res) => {
    res.json({
        message: 'API de Pólizas CWRV',
        version: '1.0.0',
        endpoints: {
            transmitir: 'POST /api/polizas/transmitir',
            health: 'GET /api/polizas/healthcheck'
        }
    });
});

// Middleware de manejo de errores global
app.use((err: Error, req: express.Request, res: express.Response, next: express.NextFunction) => {
    console.error('Error no manejado:', err);
    res.status(500).json({
        estado: 'ERROR',
        mensaje: 'Error interno del servidor',
        titulo: 'Error',
        icono: 'error'
    });
});

// Iniciar servidor
app.listen(port, () => {
    console.log(`Servidor ejecutándose en puerto ${port}`);
    console.log(`Endpoints disponibles:`);
    console.log(`- GET  http://localhost:${port}/`);
    console.log(`- POST http://localhost:${port}/api/polizas/transmitir`);
    console.log(`- GET  http://localhost:${port}/api/polizas/healthcheck`);
});

export default app; 