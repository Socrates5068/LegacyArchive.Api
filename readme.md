# 1. Crear la migración
dotnet ef migrations add AddMediaFiles --project Infrastructure --startup-project LegacyArchive.Api

# 2. (Leer el archivo generado antes de seguir)

# 3. Aplicarla a la base de datos
dotnet ef database update --project Infrastructure --startup-project LegacyArchive.Api

# Si algo salió mal ANTES de aplicarla, deshacer la última:
dotnet ef migrations remove --project Infrastructure --startup-project LegacyArchive.Api

# Ver cuáles están aplicadas y cuáles pendientes:
dotnet ef migrations list --project Infrastructure --startup-project LegacyArchive.Api