# Votify - Sistema de Votación

Proyecto desarrollado en .NET 8 con Clean Architecture.

## Tecnologías utilizadas
* **Backend / API:** ASP.NET Core 8
* **Frontend:** Blazor Web App (Server + WebAssembly)
* **UI Framework:** Radzen Blazor (Componentes Responsive y Gráficas)
* **Base de Datos:** PostgreSQL 17
* **ORM:** Entity Framework Core 8 (Patrón Table-Per-Hierarchy aplicado)
* **IA Local:** Ollama (llama3.2) para generación de hojas de ruta
* **PDF:** QuestPDF para generación de certificados y reportes
* **Tests:** xUnit + Moq

## Requisitos previos

Antes de empezar, asegúrate de tener instalado en tu equipo:

| Software | Versión | Descarga |
|---|---|---|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| PostgreSQL | 15+ | https://www.postgresql.org/download/ |
| Ollama (opcional) | Última | https://ollama.com/download |
| Visual Studio 2022 / VS Code | - | https://visualstudio.microsoft.com/ |

## Configuración rápida

### 1. Clonar el repositorio

```bash
git clone <url-del-repo>
cd Votify
```

### 2. Configurar la Base de Datos (PostgreSQL)

1. Instala PostgreSQL y recuerda la contraseña que estableciste para el usuario `postgres`.

2. Abre el archivo `Votify.Client/Votify.Web/appsettings.Development.json` y actualiza la cadena de conexión con tu contraseña:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=VotifyDB;Username=postgres;Password=TU_PASSWORD"
}
```

3. Abre la **Consola del Administrador de Paquetes** en Visual Studio y ejecuta:

```powershell
Update-Database -Project Votify.Persistence -StartupProject Votify.Web
```

> **Nota:** La base de datos se crea y pobla automáticamente con datos de ejemplo al ejecutar la aplicación por primera vez (`DbInitializer` en `Program.cs`).

### 3. Configurar Ollama (IA Local - Opcional)

La funcionalidad de **Hoja de Ruta de Mejora con IA** funciona por defecto con un proveedor mock. Para usar IA real sin costes:

1. Descarga e instala Ollama desde https://ollama.com/download
2. Abre una terminal y ejecuta:
   ```bash
   ollama pull llama3.2
   ```
3. Verifica que está disponible:
   ```bash
   ollama list
   ```
4. Ollama se ejecuta automáticamente en segundo plano en `http://localhost:11434`. No necesitas ejecutar `ollama serve` manualmente.

> **Importante:** Si Ollama no está instalado, la aplicación usará `MockIAProvider` que devuelve respuestas de ejemplo. Para activar Ollama, asegúrate de que en `Program.cs` la línea de registro de IA esté así:
> ```csharp
> builder.Services.AddHttpClient<IIAProvider, OllamaProvider>(client =>
> {
>     client.BaseAddress = new Uri("http://localhost:11434");
>     client.Timeout = TimeSpan.FromMinutes(2);
> });
> ```

### 4. Ejecutar la aplicación

Desde Visual Studio:
- Establece `Votify.Web` como proyecto de inicio
- Pulsa `F5` o el botón de ejecutar

Desde terminal:
```bash
cd Votify.Client/Votify.Web
dotnet run
```

La aplicación estará disponible en:
- **HTTPS:** https://localhost:7xxx
- **HTTP:** http://localhost:5xxx

### 5. Ejecutar las pruebas unitarias

```bash
dotnet test Votify.Tests
```

Se ejecutan 44 tests que cubren:
- Patrón Observer (notificaciones de votación)
- Servicio de Análisis de Mejora con IA
- Casos edge (respuestas inválidas, filtros, ordenamiento)

## Funcionalidades principales

### Tipos de votación
- **Votación Popular:** Un participante, un voto
- **Votación por Puntuación:** Escalas numéricas configurables
- **Votación Multicriterio:** Múltiples criterios con pesos
- **Votación de Experto:** Jueces con comentarios cualitativos

### Hoja de Ruta de Mejora con IA
Los participantes pueden generar una hoja de ruta personalizada basada en los comentarios de los jueces:

1. Inicia sesión como participante
2. Ve al Dashboard
3. Pulsa "Hoja de Ruta IA" en cualquier proyecto
4. La IA analiza los comentarios y genera sugerencias priorizadas
5. Descarga el resultado como PDF

### Notificaciones en tiempo real
- Notificaciones de apertura/cierre de votaciones
- Recordatorios automáticos
- Actualizaciones en tiempo real vía Observer Pattern

### Certificados
- Generación automática de certificados de premio en PDF
- Accesible desde el panel de resultados

## Cuentas de ejemplo

La base de datos se pobla automáticamente con estas credenciales:

| Rol | Email | Contraseña |
|---|---|---|
| Organizador | maria.garcia@votify.com | Admin123! |
| Juez | alejandro.fernandez@votify.com | Juez123! |
| Participante | novatech@votify.com | Part123! |

## Arquitectura

```
Votify/
├── Votify.Core/          # Entidades, interfaces, enums, factories
├── Votify.Persistence/   # Contexto EF Core, repositorios, UnitOfWork, DbInitializer
├── Votify.Services/      # Lógica de negocio, implementaciones de servicios
│   └── Implementations/
│       ├── IA/           # Proveedores de IA (Mock, Ollama)
│       ├── Analysis/     # Análisis de mejora
│       ├── Pdf/          # Generación de PDFs
│       ├── Voting/       # Servicios de votación
│       ├── Management/   # Gestión de entidades
│       ├── Notifications/# Notificaciones y observers
│       └── Observers/    # Patrón Observer
├── Votify.Client/
│   ├── Votify.UI/        # Componentes Blazor reutilizables
│   └── Votify.Web/       # Aplicación web principal, Program.cs, appsettings
└── Votify.Tests/         # Pruebas unitarias (xUnit + Moq)
```

## Patrones de diseño utilizados

- **Clean Architecture:** Separación en capas (Core → Persistence → Services → Web)
- **Repository + Unit of Work:** Acceso a datos abstracto y transaccional
- **Observer Pattern:** Notificaciones de estado de votación en tiempo real
- **Factory Pattern:** Creación de eventos con diferentes configuraciones
- **Strategy Pattern:** Diferentes tipos de votación con comportamiento polimórfico
- **Dependency Injection:** Inversión de control en toda la aplicación
