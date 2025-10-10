# Botilleria Clean Architecture API

API REST y GraphQL para una botillería (tienda de vinos) implementada con Clean Architecture en .NET 8.

## 🏗️ Arquitectura del Proyecto

Este proyecto sigue los principios de **Clean Architecture** propuestos por Robert C. Martin, organizando el código en capas concéntricas donde las capas externas dependen de las internas, pero no al revés.

### Capas de la Arquitectura

#### 1. **Core/Domain** (Centro - Reglas de Negocio)
La capa más interna contiene las entidades de dominio y las reglas de negocio puras.

- **Entities/**: Modelos de datos del dominio (Product, Category, Brand, etc.)
- **Interfaces/**: Contratos que definen comportamientos (IRepository, IUnitOfWork)
- **Exceptions/**: Excepciones personalizadas del dominio
- **ValueObjects/**: Objetos de valor inmutables

#### 2. **Core/Application** (Casos de Uso)
Contiene la lógica de aplicación y casos de uso del negocio.

- **Services/**: Servicios de aplicación (ProductService, AuditService)
- **DTOs/**: Objetos de transferencia de datos
  - **Commands/**: Comandos para operaciones de escritura
  - **Queries/**: Consultas para operaciones de lectura
- **Interfaces/**: Interfaces de aplicación
- **Validators/**: Validadores para comandos y queries
- **Behaviors/**: Comportamientos de pipeline (logging, validation, etc.)

#### 3. **Infrastructure** (Adaptadores Externos)
Capa de infraestructura que implementa los contratos definidos en las capas internas.

- **Persistence/**: Implementación de acceso a datos
  - **ApplicationDbContext.cs**: Contexto de Entity Framework
  - **Repositories/**: Implementaciones de repositorios
- **Configuration/**: Configuración de servicios externos

#### 4. **Presentation** (Interfaz de Usuario)
Capa más externa que maneja las interfaces de usuario y APIs.

- **API/**
  - **Controllers/**: Controladores REST API
  - **GraphQL/**: Esquemas y resolvers de GraphQL
- **Middleware/**: Middleware personalizado

### Flujo de Datos

```
Presentation → Application → Domain ← Infrastructure
     ↓            ↓            ↓          ↓
  Controllers  Services    Entities  Repositories
  GraphQL      Commands    Value     DbContext
               Queries     Objects
               Validators
```

## 🚀 Tecnologías Utilizadas

- **.NET 8**: Framework principal
- **Entity Framework Core**: ORM para acceso a datos
- **SQLite**: Base de datos embebida
- **HotChocolate**: Servidor GraphQL
- **JWT Bearer**: Autenticación
- **MediatR**: Patrón Mediator para CQRS
- **FluentValidation**: Validación de datos
- **Newtonsoft.Json**: Serialización JSON
- **Swashbuckle**: Documentación OpenAPI/Swagger

## 📁 Estructura de Carpetas

```
botilleria-clean-architecture-api/
├── Core/
│   ├── Application/
│   │   ├── Behaviors/
│   │   ├── DTOs/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── Validators/
│   └── Domain/
│       ├── Entities/
│       ├── Exceptions/
│       ├── Interfaces/
│       └── ValueObjects/
├── Infrastructure/
│   ├── Configuration/
│   └── Persistence/
│       ├── ApplicationDbContext.cs
│       └── Repositories/
├── Presentation/
│   ├── API/
│   │   ├── Controllers/
│   │   └── GraphQL/
│   └── Middleware/
├── Dtos/
├── Migrations/
├── Models/
├── Properties/
└── wwwroot/
```

## 🔧 Configuración y Ejecución

### Prerrequisitos
- .NET 8 SDK
- SQLite (incluido en el proyecto)

### Instalación
1. Clonar el repositorio
2. Ejecutar migraciones de base de datos:
   ```bash
   dotnet ef database update
   ```
3. Ejecutar la aplicación:
   ```bash
   dotnet run
   ```

### Endpoints Principales

#### REST API
- `GET /api/products` - Listar productos
- `GET /api/products/{id}` - Obtener producto por ID
- `POST /api/products` - Crear producto (requiere autenticación)
- `PUT /api/products/{id}` - Actualizar producto (requiere autenticación)
- `DELETE /api/products/{id}` - Eliminar producto (requiere autenticación)

#### GraphQL
- `POST /graphql` - Endpoint GraphQL
- `GET /graphql` - Playground GraphQL

### Autenticación
La API utiliza JWT Bearer tokens para proteger operaciones de escritura. Para obtener un token:

```bash
curl -X POST "http://localhost:5280/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "password"}'
```

## 📊 Características

- ✅ **Clean Architecture**: Separación clara de responsabilidades
- ✅ **CQRS**: Separación de comandos y queries
- ✅ **Auditoría**: Logging automático de todas las operaciones
- ✅ **Validación**: Validación automática de entrada
- ✅ **GraphQL**: API flexible con GraphQL
- ✅ **JWT**: Autenticación segura
- ✅ **SQLite**: Base de datos embebida para desarrollo
- ✅ **Swagger**: Documentación interactiva de API

## 🔄 Ciclo de Desarrollo

1. **Domain**: Definir entidades y reglas de negocio
2. **Application**: Crear comandos/queries y servicios
3. **Infrastructure**: Implementar repositorios y configuración
4. **Presentation**: Crear controladores y endpoints
5. **Testing**: Probar todas las capas

## 📝 Notas de Desarrollo

- Las dependencias fluyen hacia adentro (Presentation → Domain)
- Las interfaces se definen en las capas internas
- La lógica de negocio no depende de frameworks externos
- Los adaptadores externos implementan contratos internos

---

**Proyecto desarrollado con Clean Architecture para mantener un código mantenible, testable y escalable.**