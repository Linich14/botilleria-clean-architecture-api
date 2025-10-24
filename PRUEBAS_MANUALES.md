# 📝 Pruebas Manuales - Botilleria API

## 🎯 Objetivo
Documentar las 3 pruebas manuales requeridas para validar el correcto funcionamiento de la API.

---

## 🧪 Prueba Manual 1: Autenticación JWT

### **Descripción**
Validar que el sistema de autenticación JWT funciona correctamente y retorna un token válido.

### **Endpoint**
```
POST http://localhost:5280/api/auth/login
```

### **Request Body**
```json
{
  "username": "admin",
  "password": "password"
}
```

### **Headers**
```
Content-Type: application/json
```

### **Respuesta Esperada**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-10-23T23:00:00Z"
}
```

### **Criterios de Éxito**
- ✅ Status Code: 200 OK
- ✅ El token no está vacío
- ✅ El token tiene formato JWT válido (tres partes separadas por punto)
- ✅ La expiración es una fecha futura

### **Pasos para Ejecutar (Thunder Client/Postman)**
1. Abrir Thunder Client en VS Code
2. Crear nueva request POST
3. Ingresar URL: `http://localhost:5280/api/auth/login`
4. En Body > JSON, pegar el JSON de credenciales
5. Click en "Send"
6. Copiar el token recibido para usarlo en las siguientes pruebas

### **Captura de Pantalla**
📸 [Guardar evidencia de la prueba exitosa]

---

## 🧪 Prueba Manual 2: Crear Producto con Autenticación

### **Descripción**
Validar que un usuario autenticado puede crear un producto nuevo en el sistema.

### **Endpoint**
```
POST http://localhost:5280/api/products
```

### **Request Body**
```json
{
  "name": "Vino Tinto Cabernet Sauvignon Reserva",
  "description": "Excelente vino tinto con 12 meses en barrica de roble francés. Notas de frutas rojas maduras, chocolate y vainilla.",
  "price": 15990,
  "discountPrice": 12990,
  "volume": 750,
  "unit": "ml",
  "alcoholContent": 13.5,
  "stock": 25,
  "isAvailable": true,
  "categoryId": 1,
  "productTypeId": 1,
  "brandId": 1,
  "originId": 1,
  "color": "Rojo rubí intenso",
  "aroma": "Frutas rojas, cassis, chocolate amargo, vainilla",
  "taste": "Taninos suaves, cuerpo medio, final persistente",
  "servingTemperature": "16-18°C",
  "foodPairing": [
    "Carne roja a la parrilla",
    "Cordero",
    "Pasta con salsa de tomate",
    "Quesos curados"
  ]
}
```

### **Headers**
```
Content-Type: application/json
Authorization: Bearer {TOKEN_DE_PRUEBA_1}
```

### **Respuesta Esperada**
```json
{
  "id": 5,
  "name": "Vino Tinto Cabernet Sauvignon Reserva",
  "description": "Excelente vino tinto con 12 meses en barrica...",
  "price": 15990,
  "discountPrice": 12990,
  "stock": 25,
  "isAvailable": true,
  "category": {
    "id": 1,
    "name": "Vinos Tintos"
  },
  "brand": {
    "id": 1,
    "name": "Concha y Toro"
  },
  "createdAt": "2025-10-23T20:30:00Z",
  "updatedAt": "2025-10-23T20:30:00Z"
}
```

### **Criterios de Éxito**
- ✅ Status Code: 201 Created
- ✅ El producto tiene un ID asignado
- ✅ Los datos coinciden con los enviados
- ✅ Las relaciones (category, brand) están incluidas
- ✅ createdAt y updatedAt tienen valores

### **Pasos para Ejecutar**
1. Copiar el token de la Prueba 1
2. Crear nueva request POST
3. Ingresar URL: `http://localhost:5280/api/products`
4. En Headers, agregar: `Authorization: Bearer {tu-token}`
5. En Body > JSON, pegar el JSON del producto
6. Click en "Send"
7. Verificar que el producto se creó correctamente

### **Validaciones Adicionales**
- ❌ Sin token: Debe retornar 401 Unauthorized
- ❌ Token expirado: Debe retornar 401 Unauthorized
- ❌ Datos inválidos: Debe retornar 400 Bad Request

### **Captura de Pantalla**
📸 [Guardar evidencia de la prueba exitosa]

---

## 🧪 Prueba Manual 3: Consulta GraphQL con Filtros

### **Descripción**
Validar que las consultas GraphQL funcionan correctamente con filtros y ordenamiento.

### **Endpoint**
```
POST http://localhost:5280/graphql
```

### **Request Body**
```json
{
  "query": "query GetFilteredProducts($where: ProductFilterInput, $order: [ProductSortInput!]) { products(where: $where, order: $order) { id name description price stock isAvailable category { id name } brand { id name } } }",
  "variables": {
    "where": {
      "price": {
        "gte": 5000,
        "lte": 20000
      },
      "stock": {
        "gt": 0
      },
      "isAvailable": {
        "eq": true
      }
    },
    "order": [
      {
        "price": "ASC"
      },
      {
        "name": "ASC"
      }
    ]
  }
}
```

### **Headers**
```
Content-Type: application/json
```

### **Respuesta Esperada**
```json
{
  "data": {
    "products": [
      {
        "id": 1,
        "name": "Producto A",
        "description": "Descripción del producto A",
        "price": 8990,
        "stock": 15,
        "isAvailable": true,
        "category": {
          "id": 1,
          "name": "Vinos Tintos"
        },
        "brand": {
          "id": 1,
          "name": "Concha y Toro"
        }
      },
      {
        "id": 2,
        "name": "Producto B",
        "description": "Descripción del producto B",
        "price": 12990,
        "stock": 8,
        "isAvailable": true,
        "category": {
          "id": 2,
          "name": "Vinos Blancos"
        },
        "brand": {
          "id": 2,
          "name": "Santa Rita"
        }
      }
    ]
  }
}
```

### **Criterios de Éxito**
- ✅ Status Code: 200 OK
- ✅ Los productos están filtrados por precio (entre 5000 y 20000)
- ✅ Solo productos con stock > 0
- ✅ Solo productos disponibles (isAvailable: true)
- ✅ Los productos están ordenados por precio ASC
- ✅ Las relaciones (category, brand) están cargadas

### **Pasos para Ejecutar (GraphQL Playground)**
1. Abrir navegador en: `http://localhost:5280/graphql`
2. En el panel izquierdo, pegar la query
3. En el panel "Query Variables", pegar las variables
4. Click en el botón de ejecutar (▶️)
5. Verificar los resultados en el panel derecho

### **Pasos para Ejecutar (Thunder Client)**
1. Crear nueva request POST
2. Ingresar URL: `http://localhost:5280/graphql`
3. En Headers: `Content-Type: application/json`
4. En Body > JSON, pegar el JSON completo (query + variables)
5. Click en "Send"
6. Verificar los resultados

### **Validaciones Adicionales**

#### **Query Simple (sin filtros):**
```graphql
{
  products {
    id
    name
    price
  }
}
```

#### **Query con filtro de categoría:**
```graphql
{
  products(where: { category: { name: { contains: "Vino" } } }) {
    id
    name
    category { name }
  }
}
```

#### **Query con ordenamiento:**
```graphql
{
  products(order: { price: DESC }) {
    id
    name
    price
  }
}
```

### **Captura de Pantalla**
📸 [Guardar evidencia de la prueba exitosa]

---

## 📊 Resumen de Pruebas Manuales

| # | Prueba | Endpoint | Método | Autenticación | Estado |
|---|--------|----------|--------|---------------|---------|
| 1 | Login JWT | `/api/auth/login` | POST | No | ⬜ |
| 2 | Crear Producto | `/api/products` | POST | Sí (JWT) | ⬜ |
| 3 | GraphQL Filtros | `/graphql` | POST | No | ⬜ |

### **Checklist de Validación**
- [ ] Prueba 1 ejecutada y exitosa
- [ ] Token JWT obtenido y copiado
- [ ] Prueba 2 ejecutada con token válido
- [ ] Producto creado correctamente
- [ ] Prueba 3 ejecutada en GraphQL Playground
- [ ] Filtros aplicados correctamente
- [ ] Ordenamiento funciona
- [ ] Capturas de pantalla guardadas
- [ ] Evidencias documentadas

---

## 🔧 Configuración del Entorno

### **Pre-requisitos**
1. Aplicación corriendo en `http://localhost:5280`
2. Base de datos SQLite inicializada
3. Datos de prueba cargados (categorías, marcas, etc.)

### **Comandos de Inicio**
```bash
# Restaurar la base de datos
dotnet ef database update

# Ejecutar la aplicación
dotnet run
```

### **Herramientas Recomendadas**
- 🔵 Thunder Client (Extension de VS Code)
- 🟠 Postman Desktop
- 🟣 GraphQL Playground (http://localhost:5280/graphql)
- 🔴 Insomnia

---

## 📸 Evidencias

### **Formato de Evidencias**
Para cada prueba, incluir:
1. Captura del request (headers, body, URL)
2. Captura de la respuesta (status code, body)
3. Timestamp de ejecución
4. Nombre del evaluador

### **Plantilla de Reporte**
```markdown
## Prueba: [Nombre]
- **Fecha**: [DD/MM/YYYY HH:mm]
- **Evaluador**: [Nombre]
- **Resultado**: ✅ EXITOSA / ❌ FALLIDA
- **Observaciones**: [Comentarios adicionales]
- **Evidencia**: [Link a captura de pantalla]
```

---

**Documento creado por**: Jorge Soto  
**Proyecto**: Botilleria Clean Architecture API  
**Fecha**: Octubre 2025  
**Versión**: 1.0
