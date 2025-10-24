# 🚀 GitHub Actions - Configuración CI/CD

## 📋 Descripción

Este proyecto usa GitHub Actions para automatizar el proceso de Integración Continua (CI) y Despliegue Continuo (CD).

## 🔧 Workflows Configurados

### **1. CI/CD Pipeline** (`.github/workflows/ci-cd.yml`)

Pipeline principal que se ejecuta en cada push o pull request a las ramas `main` y `develop`.

#### **Jobs Incluidos:**

#### 🏗️ **Build and Test**
- ✅ Checkout del código
- ✅ Configuración de .NET 8
- ✅ Restauración de dependencias
- ✅ Compilación del proyecto
- ✅ Ejecución de pruebas automatizadas
- ✅ Publicación de resultados de pruebas

#### 📊 **Code Quality Check**
- ✅ Verificación de formato de código
- ✅ Compilación con warnings como errores
- ✅ Validación de estándares de código

#### 🔒 **Security Scan**
- ✅ Escaneo de paquetes vulnerables
- ✅ Detección de paquetes deprecados
- ✅ Reporte de dependencias inseguras

#### 📋 **Summary**
- ✅ Resumen de ejecución del pipeline
- ✅ Estado de cada job
- ✅ Información del proyecto

## 🎯 Triggers

El workflow se ejecuta cuando:
- 📤 Se hace push a `main` o `develop`
- 🔀 Se crea un pull request hacia `main` o `develop`
- 🔘 Se ejecuta manualmente desde GitHub (workflow_dispatch)

## 📊 Visualización de Resultados

### **En GitHub:**
1. Ve a la pestaña **Actions** en tu repositorio
2. Selecciona el workflow **CI/CD Pipeline**
3. Verás el estado de cada ejecución

### **Badges (Insignias):**

Agrega esto a tu `README.md`:

```markdown
[![CI/CD Pipeline](https://github.com/Linich14/botilleria-clean-architecture-api/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/Linich14/botilleria-clean-architecture-api/actions/workflows/ci-cd.yml)
```

## 🔍 Detalles de Cada Job

### **Build and Test**
```yaml
- Restaura dependencias NuGet
- Compila en modo Release
- Ejecuta todas las pruebas
- Genera reportes en formato TRX
```

### **Code Quality**
```yaml
- Verifica el formato del código con dotnet format
- Compila con warnings como errores
- Garantiza calidad del código
```

### **Security Scan**
```yaml
- Escanea vulnerabilidades conocidas
- Lista paquetes deprecados
- Valida dependencias transitivas
```

## 🚨 Manejo de Errores

- ❌ **Build falla**: El pipeline se detiene
- ⚠️ **Tests fallan**: Se marca pero continúa (continue-on-error: true)
- 🔍 **Code quality falla**: Se marca pero continúa
- 🔒 **Security scan**: Informativo, no bloquea

## 📦 Requisitos

- ✅ .NET 8 SDK
- ✅ Proyecto .NET compilable
- ✅ Tests en xUnit (opcional, pero recomendado)

## 🎮 Ejecución Manual

1. Ve a **Actions** en GitHub
2. Selecciona **CI/CD Pipeline**
3. Click en **Run workflow**
4. Selecciona la rama
5. Click en **Run workflow** (botón verde)

## 🔧 Personalización

### **Cambiar versión de .NET:**
```yaml
env:
  DOTNET_VERSION: '8.0.x'  # Cambiar aquí
```

### **Cambiar configuración de build:**
```yaml
env:
  BUILD_CONFIGURATION: 'Release'  # O 'Debug'
```

### **Agregar más jobs:**
```yaml
jobs:
  mi-nuevo-job:
    name: Mi Nuevo Job
    runs-on: ubuntu-latest
    steps:
      - name: Mi paso
        run: echo "Hola mundo"
```

## 📈 Próximos Pasos

1. ✅ Agregar pruebas automatizadas al proyecto
2. ✅ Configurar deployment automático
3. ✅ Agregar análisis de cobertura de código
4. ✅ Integrar con SonarCloud para análisis de calidad
5. ✅ Agregar notificaciones (Slack, Discord, etc.)

## 🆘 Troubleshooting

### **El workflow no se ejecuta:**
- Verifica que el archivo esté en `.github/workflows/`
- Verifica que la rama esté en los triggers
- Revisa que el archivo YAML sea válido

### **Build falla:**
- Revisa los logs en la pestaña Actions
- Verifica que el proyecto compile localmente
- Asegúrate de que todas las dependencias estén en NuGet

### **Tests no se ejecutan:**
- Verifica que exista un proyecto de pruebas
- Asegúrate de que los tests usen xUnit, NUnit o MSTest
- Revisa que el comando `dotnet test` funcione localmente

## 📚 Referencias

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)
- [Test Reporter](https://github.com/dorny/test-reporter)

---

**Autor**: Jorge Soto  
**Proyecto**: Botilleria Clean Architecture API  
**Fecha**: Octubre 2025
