# CWRV Póliza Service

## Descripción

Este proyecto es la migración de la funcionalidad `TransmitirPoliza` desde C# .NET a Node.js con TypeScript, implementando una arquitectura limpia que sigue los principios SOLID y patrones de diseño modernos.

## Arquitectura

### Capas de la Aplicación

```
src/
├── domain/                 # Capa de Dominio
│   ├── entities/          # Entidades del dominio
│   ├── value-objects/     # Objetos de valor
│   ├── interfaces/        # Interfaces del dominio
│   └── services/          # Servicios del dominio
├── application/           # Capa de Aplicación
│   └── usecases/         # Casos de uso
├── infrastructure/        # Capa de Infraestructura
│   ├── repositories/     # Implementaciones de repositorios
│   ├── services/         # Servicios de infraestructura
│   └── ioc/              # Contenedor de dependencias
└── presentation/          # Capa de Presentación
    ├── controllers/      # Controladores HTTP
    └── routes/           # Rutas de Express
```

### Principios SOLID Aplicados

1. **Single Responsibility Principle (SRP)**
   - Cada clase tiene una única razón para cambiar
   - `PolizaValidationService` solo se encarga de validaciones
   - `PolizaRepository` solo maneja el acceso a datos

2. **Open/Closed Principle (OCP)**
   - Las interfaces están abiertas para extensión
   - Las implementaciones están cerradas para modificación

3. **Liskov Substitution Principle (LSP)**
   - Todas las implementaciones respetan sus contratos de interfaces
   - `PolizaRepository` puede ser sustituido por cualquier implementación de `IPolizaRepository`

4. **Interface Segregation Principle (ISP)**
   - Interfaces específicas y cohesivas
   - `IAuthService` e `ILogger` son interfaces separadas

5. **Dependency Inversion Principle (DIP)**
   - Las capas superiores no dependen de las inferiores
   - Todo se inyecta a través de interfaces

### Patrones de Diseño Implementados

1. **Repository Pattern**
   - Abstrae el acceso a datos
   - `IPolizaRepository` define el contrato

2. **Dependency Injection**
   - Contenedor IoC personalizado
   - Todas las dependencias se inyectan

3. **Use Case Pattern**
   - Cada caso de uso encapsula una operación de negocio
   - `TransmitirPolizaUseCase` maneja toda la lógica de transmisión

4. **Builder Pattern**
   - Construcción fluida de respuestas
   - `Respuesta.exito()`, `Respuesta.error()`, etc.

5. **Strategy Pattern** (implícito)
   - Diferentes implementaciones pueden ser intercambiadas
   - Validaciones pueden ser extendidas fácilmente

## Instalación

```bash
# Instalar dependencias
npm install

# Compilar TypeScript
npm run build

# Ejecutar en desarrollo
npm run dev

# Ejecutar en producción
npm start
```

## Configuración

### Variables de Entorno

Crear un archivo `.env` en la raíz del proyecto:

```env
PORT=3000
DB_HOST=localhost
DB_PORT=3306
DB_USER=usuario
DB_PASSWORD=contraseña
DB_NAME=cwrv_db
```

### Base de Datos

Configurar la conexión a tu base de datos en `src/app.ts`:

```typescript
const dbConnection = {
    // Tu conexión real (mysql2, pg, etc.)
    execute: async (query: string, params: any[]) => {
        // Implementar conexión real
        return await pool.execute(query, params);
    }
};
```

## Uso

### Endpoint Principal

```http
POST /api/polizas/transmitir
Content-Type: application/json

{
    "tokenUsuario": "token-del-usuario",
    "numeroSolicitud": "12345",
    "numeroCorrelativo": 1,
    "numeroPoliza": "POL-12345"
}
```

### Respuesta Exitosa

```json
{
    "estado": "OK",
    "mensaje": "Póliza transmitida exitosamente",
    "titulo": "Éxito",
    "icono": "success"
}
```

### Respuesta de Error

```json
{
    "estado": "ERROR",
    "mensaje": "Descripción del error",
    "titulo": "Error",
    "icono": "error"
}
```

## Testing

```bash
# Ejecutar tests
npm test

# Ejecutar tests en modo watch
npm run test:watch
```

## Linting

```bash
# Ejecutar linter
npm run lint

# Corregir errores automáticamente
npm run lint:fix
```

## Migración desde C#

### Comparación de Funcionalidades

| Funcionalidad C# | Equivalente Node.js/TypeScript |
|------------------|-------------------------------|
| `[WebMethod]` | Endpoint REST con Express |
| `NDC.Push()` | Logging con Winston |
| `LocalizadorProxy.ObtenerServicio()` | Dependency Injection Container |
| `Utilitarios.ValidarPermiso()` | `AuthService.validarPermiso()` |
| `servicioCotizador.ObtenerAbonoPorPoliza()` | `PolizaRepository.obtenerAbonoPorPoliza()` |
| `HttpContext.Current.Session` | Express Session |

### Beneficios de la Migración

1. **Mejor Testabilidad**: Inyección de dependencias facilita unit testing
2. **Mantenibilidad**: Separación clara de responsabilidades
3. **Escalabilidad**: Arquitectura preparada para microservicios
4. **Flexibilidad**: Fácil intercambio de implementaciones
5. **Documentación**: Código autodocumentado con TypeScript

## Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crea un Pull Request

## Licencia

MIT License - ver el archivo LICENSE para más detalles. 