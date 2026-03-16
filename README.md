# Votify - Sistema de Votación

Proyecto desarrollado en .NET 8 con Clean Architecture.

## Tecnologías utilizadas
* **Arquitectura:** Clean Architecture (Core, UI, Web, Services, Persistence)
* **Backend / Host:** ASP.NET Core (API REST y Servidor de Blazor)
* **Frontend:** Blazor WebAssembly (`Votify.UI`)
* **Base de Datos:** PostgreSQL 17
* **ORM:** Entity Framework Core 8

## Configuración rápida

## 🗄️ Configuración de la Base de Datos (PostgreSQL)

Utilizamos **Entity Framework Core** estructurado en Clean Architecture. Para que la herramienta de migraciones no tenga conflictos con las rutas del proyecto Blazor, utilizamos el patrón `IDesignTimeDbContextFactory`.

### Pasos para levantar la base de datos en local:

1. **Configurar la ejecución de la API/Web: (Votify.Web)**
   Abre el archivo `Votify.Web/appsettings.json` y actualiza la cadena de conexión `DefaultConnection` con tus credenciales locales de PostgreSQL.

2. **Configurar la herramienta de Migraciones:**
   Abre el archivo `Votify.Persistence/Context/VotifyContextFactory.cs`. Actualiza la variable `connectionString` con tus credenciales reales (¡Recuerda no hacer commit de tu contraseña real en este archivo!).

3. **Ejecutar la creación de tablas:**
   Abre la **Consola del Administrador de Paquetes** en Visual Studio y ejecuta el siguiente comando para aplicar las migraciones a tu base de datos local:

   ```powershell
   Update-Database -Project Votify.Persistence -StartupProject Votify.Persistence
