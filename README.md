# Votify - Sistema de Votación

Proyecto desarrollado en .NET 8 con Clean Architecture.

## Tecnologías utilizadas
* **Backend / API:** ASP.NET Core 8
* **Frontend:** Blazor Web App (Server + WebAssembly)
* **UI Framework:** Radzen Blazor (Componentes Responsive y Gráficas)
* **Base de Datos:** PostgreSQL 17
* **ORM:** Entity Framework Core 8 (Patrón Table-Per-Hierarchy aplicado)

## Configuración rápida

## Configuración de la Base de Datos (PostgreSQL)

Utilizamos **Entity Framework Core** estructurado en Clean Architecture. Para que la herramienta de migraciones no tenga conflictos con las rutas del proyecto Blazor, utilizamos el patrón `IDesignTimeDbContextFactory`.

### Pasos para levantar la base de datos en local:

El proyecto utiliza Entity Framework Core. Sigue estos pasos para generar las tablas en tu entorno local:

1. **Configura tus credenciales:** Abre el archivo `Votify.Web/appsettings.json/appsetings.development.json` y actualiza la cadena de conexión `DefaultConnection` con tu usuario y contraseña local de PostgreSQL.
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=Votify;Username=postgres;Password=TU_PASSWORD"
   }

3. **Ejecutar la creación de tablas:**
   Abre la **Consola del Administrador de Paquetes** en Visual Studio y ejecuta el siguiente comando para aplicar las migraciones a tu base de datos local:

   ```powershell
   Update-Database -Project Votify.Persistence -StartupProject Votify.Persistence
