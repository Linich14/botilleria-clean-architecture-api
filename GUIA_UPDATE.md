# 🔄 Guía de Uso del Endpoint UPDATE (PUT)

## ✅ Mejoras Implementadas

El endpoint `PUT /api/products/{id}` ahora está completamente funcional y maneja:

- ✅ **Campos básicos** (nombre, precio, stock, etc.)
- ✅ **Category** (crear nueva o reutilizar existente)
- ✅ **Brand** (crear nueva o reutilizar existente)
- ✅ **ProductType** (crear nuevo o reutilizar existente)
- ✅ **Origin** (con Country y Region, igual que POST)
- ✅ **Characteristics** (color, aroma, sabor, etc.)
- ✅ **Autenticación JWT** requerida

---

## 🚀 Cómo Usar en Swagger

### 1. Acceder a Swagger
Abre tu navegador en: `http://localhost:5280/swagger`

### 2. Autenticarse
1. Busca `POST /api/auth/login`
2. Click en "Try it out"
3. Body: `{"password": "secret123"}`
4. Copia el token
5. Click en **"Authorize"** (candado arriba)
6. Pega: `Bearer <tu-token>`

### 3. Usar PUT /api/products/{id}

Busca el endpoint `PUT /api/products/{id}` en Swagger.

---

## 📝 Ejemplos de Uso

### Ejemplo 1: Actualizar solo campos básicos

**Endpoint:** `PUT /api/products/1`

```json
{
  "name": "Vino Tinto Premium (Actualizado)",
  "description": "Descripción actualizada",
  "price": 29.99,
  "stock": 150,
  "isAvailable": true
}
```

**Resultado:** 
- ✅ Se actualizan los campos básicos
- ✅ Las relaciones existentes (Category, Brand, etc.) se mantienen
- ✅ Devuelve el producto actualizado completo

---

### Ejemplo 2: Cambiar la categoría a una existente (por nombre)

**Endpoint:** `PUT /api/products/1`

```json
{
  "name": "Vino Tinto Premium",
  "price": 29.99,
  "stock": 150,
  "isAvailable": true,
  "category": {
    "name": "Vinos"
  }
}
```

**Resultado:**
- Si "Vinos" existe, se reutiliza
- Si "Vinos" NO existe, se crea automáticamente
- El producto queda asociado a esa categoría

---

### Ejemplo 3: Cambiar la categoría a una existente (por ID)

**Endpoint:** `PUT /api/products/1`

```json
{
  "name": "Vino Tinto Premium",
  "price": 29.99,
  "stock": 150,
  "isAvailable": true,
  "category": {
    "id": 2
  }
}
```

**Resultado:**
- Se busca la categoría con ID=2
- Si existe, se asocia al producto
- Si no existe, el producto queda sin categoría

---

### Ejemplo 4: Remover la categoría

**Endpoint:** `PUT /api/products/1`

```json
{
  "name": "Vino Tinto Premium",
  "price": 29.99,
  "stock": 150,
  "isAvailable": true,
  "category": null
}
```

**Resultado:**
- ✅ Se elimina la asociación con la categoría
- El producto queda sin categoría (CategoryId = null)

---

### Ejemplo 5: Actualizar con Origin completo

**Endpoint:** `PUT /api/products/1`

```json
{
  "name": "Vino Carmenere Reserva",
  "description": "Vino chileno de alta calidad",
  "price": 35.50,
  "stock": 80,
  "isAvailable": true,
  "category": {
    "name": "Vinos",
    "subcategory": "Tinto"
  },
  "brand": {
    "name": "Concha y Toro"
  },
  "type": {
    "name": "Vino Tinto"
  },
  "origin": {
    "country": {
      "name": "Chile",
      "subcategory": "CL"
    },
    "region": {
      "name": "Valle de Colchagua"
    },
    "vineyard": "Viñedo Don Melchor"
  },
  "characteristics": {
    "color": "Rojo Rubí Intenso",
    "aroma": "Frutas rojas maduras con notas especiadas",
    "taste": "Cuerpo medio, taninos suaves",
    "servingTemperature": "16-18°C",
    "foodPairing": ["Carnes rojas a la parrilla", "Cordero", "Quesos maduros"]
  }
}
```

**Resultado:**
- ✅ Actualiza todos los campos
- ✅ Crea o reutiliza Category, Brand, ProductType
- ✅ Crea o reutiliza Country y Region (guardando intermedio para FKs)
- ✅ Actualiza o crea Origin
- ✅ Actualiza o crea Characteristics
- ✅ Devuelve el producto completo actualizado

---

### Ejemplo 6: Actualizar solo el precio y stock

**Endpoint:** `PUT /api/products/1`

```json
{
  "name": "Vino Carmenere Reserva",
  "price": 32.99,
  "stock": 120,
  "isAvailable": true
}
```

**Nota:** Si no envías `category`, `brand`, `origin`, etc., esas relaciones **se eliminan** (se ponen en null). Si quieres mantenerlas, debes incluirlas en el body.

---

### Ejemplo 7: Remover el Origin

**Endpoint:** `PUT /api/products/1`

```json
{
  "name": "Producto sin origen",
  "price": 20.00,
  "stock": 50,
  "isAvailable": true,
  "origin": null
}
```

**Resultado:**
- ✅ Se elimina la asociación con Origin
- El producto queda sin origen (OriginId = null)

---

## 🔍 Comportamiento Detallado

### 1. Campos Básicos
- **Siempre se actualizan** con los valores del body
- `UpdatedAt` se establece automáticamente a `DateTime.UtcNow`

### 2. Category, Brand, ProductType
**Si envías `null`:** Se elimina la relación
**Si envías `{ "id": 5 }`:** Se busca por ID y se asocia
**Si envías `{ "name": "Nombre" }`:** 
  - Se busca por nombre
  - Si existe, se reutiliza
  - Si no existe, se crea nueva

### 3. Origin (Country + Region)
**Si envías `null`:** Se elimina Origin
**Si envías Origin completo:**
  - **Country:** Se busca por nombre o ID
    - Si no existe, se crea y guarda (para obtener ID)
    - `IsoCode` se genera automáticamente si no se proporciona
  - **Region:** Se busca por nombre + CountryId
    - Si no existe, se crea y guarda (para obtener ID)
  - **Origin:** Se actualiza si existe, o se crea nuevo

### 4. Characteristics
**Si envías `null`:** Se eliminan las características
**Si envías Characteristics:**
  - Si el producto ya tiene características, se actualizan
  - Si no tiene, se crean nuevas
  - `FoodPairing` se serializa como JSON

---

## ⚠️ Importante: Diferencia con PATCH

### PUT (Actualización Completa)
- Reemplaza **todos** los campos
- Si no envías una relación, se elimina
- **Idempotente:** Múltiples llamadas dan el mismo resultado

### PATCH (Actualización Parcial)
- Actualiza **solo** los campos especificados
- Las relaciones no enviadas se mantienen intactas
- Usa JSON Patch format (diferente sintaxis)

**Ejemplo PATCH:**
```json
[
  { "op": "replace", "path": "/price", "value": 25.99 },
  { "op": "replace", "path": "/stock", "value": 100 }
]
```

---

## 🎯 Casos de Uso Comunes

### Caso 1: Actualizar precio y stock manteniendo todo lo demás

**❌ Forma incorrecta con PUT:**
```json
{
  "price": 25.99,
  "stock": 100
}
```
Esto eliminaría todas las relaciones.

**✅ Forma correcta con PATCH:**
```json
[
  { "op": "replace", "path": "/price", "value": 25.99 },
  { "op": "replace", "path": "/stock", "value": 100 }
]
```

**✅ O con PUT (enviar todo):**
Primero haz GET del producto, modifica precio/stock, y envía todo de vuelta.

---

### Caso 2: Cambiar solo la marca

**Con PUT (debes enviar campos requeridos):**
```json
{
  "name": "Nombre del Producto",
  "price": 20.00,
  "stock": 50,
  "isAvailable": true,
  "brand": { "name": "Nueva Marca" }
}
```

---

### Caso 3: Migrar producto a otra categoría

**Endpoint:** `PUT /api/products/1`

```json
{
  "name": "Whisky Escocés",
  "price": 89.99,
  "stock": 30,
  "isAvailable": true,
  "category": {
    "name": "Licores Premium"
  }
}
```

---

## 🧪 Prueba Rápida

### 1. Crear un producto (POST)
```bash
POST /api/products
{
  "name": "Test Product",
  "price": 10.00,
  "stock": 100,
  "isAvailable": true,
  "category": { "name": "Test" }
}
```
Resultado: Producto con ID=X

### 2. Actualizarlo (PUT)
```bash
PUT /api/products/X
{
  "name": "Test Product Updated",
  "price": 15.00,
  "stock": 150,
  "isAvailable": true,
  "category": { "name": "Test Updated" }
}
```
Resultado: ✅ Producto actualizado con nueva categoría

### 3. Verificar (GET)
```bash
GET /api/products/X
```
Resultado: Ver el producto con los cambios aplicados

---

## 📊 Respuesta del Endpoint

### Códigos HTTP:
- **200 OK:** Actualización exitosa, devuelve el producto actualizado completo
- **404 Not Found:** Producto con ese ID no existe
- **401 Unauthorized:** Token JWT inválido o no proporcionado
- **400 Bad Request:** Datos inválidos en el body

### Body de Respuesta (200 OK):
```json
{
  "id": 1,
  "name": "Vino Tinto Premium",
  "description": "Descripción actualizada",
  "price": 29.99,
  "stock": 150,
  "isAvailable": true,
  "category": {
    "id": 1,
    "name": "Vinos"
  },
  "brand": {
    "id": 2,
    "name": "Concha y Toro"
  },
  "origin": {
    "id": 3,
    "country": {
      "id": 1,
      "name": "Chile",
      "isoCode": "CL"
    },
    "region": {
      "id": 2,
      "name": "Valle de Colchagua"
    },
    "vineyard": "Viñedo Don Melchor"
  },
  "createdAt": "2025-10-01T10:00:00Z",
  "updatedAt": "2025-10-09T15:30:00Z"
}
```

---

## 🔐 Autenticación

**Todos los endpoints de modificación requieren JWT:**
- POST /api/products
- PUT /api/products/{id}
- PATCH /api/products/{id}
- DELETE /api/products/{id}

**Los endpoints de consulta NO requieren JWT:**
- GET /api/products
- GET /api/products/{id}

---

## 💡 Tips

1. **Mantener relaciones existentes:** Si solo quieres actualizar campos básicos, usa **PATCH** en lugar de PUT
2. **Crear entidades automáticamente:** Puedes enviar nombres en lugar de IDs, se crearán si no existen
3. **Validar antes:** Haz un GET primero para ver la estructura actual del producto
4. **Origin requiere Country Y Region:** No puedes crear Origin sin ambos
5. **IsoCode automático:** Si no envías `subcategory` para Country, se genera automáticamente

---

## 🐛 Troubleshooting

### Error: "FOREIGN KEY constraint failed"
- Verifica que estés enviando tanto Country como Region en Origin
- El sistema guarda Country primero, luego Region

### Error: 401 Unauthorized
- Obtén un nuevo token con `/api/auth/login`
- Verifica que incluiste "Bearer " antes del token en Authorization

### Error: 404 Not Found
- El producto con ese ID no existe
- Verifica el ID con GET /api/products

### Producto sin relaciones después de UPDATE
- Recuerda: PUT reemplaza todo
- Si no envías `category`, se elimina
- Usa PATCH si solo quieres cambiar algunos campos

---

## ✅ Resumen

| Acción | Método | Endpoint | Requiere JWT |
|--------|--------|----------|--------------|
| Actualización completa | PUT | `/api/products/{id}` | ✅ Sí |
| Actualización parcial | PATCH | `/api/products/{id}` | ✅ Sí |
| Ver producto | GET | `/api/products/{id}` | ❌ No |

**PUT está listo para usar en Swagger con todas las funcionalidades!** 🎉
