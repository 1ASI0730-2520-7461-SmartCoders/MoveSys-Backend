using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Cortex.Mediator.DependencyInjection;
using movesys_backend_.IAM.Infrastructure.Interfaces.ASP.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var allowFrontendCors = "AllowFrontend";
var frontendOrigin = builder.Configuration.GetValue<string>("Frontend:Origin") ?? "http://localhost:5173";

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "MoveSys API", 
        Version = "v1",
        Description = "API para la gestión del sistema MoveSys - Sistema de gestión de flota y entregas"
    });
    
    // Habilita comentarios XML para documentación mejorada
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Configuración para mostrar ejemplos personalizados
    c.UseOneOfForPolymorphism();
    c.UseAllOfToExtendReferenceSchemas();
});

// CORS for frontend - Configuración permisiva para desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowFrontendCors, policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // En desarrollo, permitir cualquier origen
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // En producción, usar orígenes específicos
            policy.WithOrigins(frontendOrigin, "http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .SetPreflightMaxAge(TimeSpan.FromSeconds(3600));
        }
    });
});

// Database (EF Core) - MySQL por defecto (usar Sqlite solo si se especifica)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var useSqlite = builder.Configuration.GetValue<bool>("Database:UseSqlite", false);
    if (useSqlite)
    {
        var sqliteCs = builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=movesys.db";
        options.UseSqlite(sqliteCs);
    }
    else
    {
        var mySqlCs = builder.Configuration.GetConnectionString("MySql") ?? "Server=127.0.0.1;Port=3306;Database=movesys;User=root;Password=;";
        // Usar una versión específica de MySQL en lugar de AutoDetect para evitar errores de conexión durante la configuración
        // FreeSQLDatabase generalmente usa MySQL 8.0
        // Usar MySQL 5.7 para compatibilidad con FreeSQLDatabase (no soporta datetime(6))
        var serverVersion = ServerVersion.Parse("5.7.40-mysql");
        options.UseMySql(mySqlCs, serverVersion, mySqlOptions =>
        {
            // Habilitar reintentos automáticos para errores transitorios
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
    }
});

// Configure Mediator (Cortex.Mediator)
builder.Services.AddCortexMediator(
    configuration: builder.Configuration,
    handlerAssemblyMarkerTypes: [typeof(Program)]);

// Register IAM Context Services
builder.AddIamContextServices();

// Register Shared and Bounded Context services (Repositories, UoW, Services)
builder.Services.AddScoped<movesys_backend_.Shared.Domain.Repositories.IUnitOfWork, movesys_backend_.Shared.Infrastructure.Persistence.EFC.Configuration.UnitOfWork>();
builder.Services.AddScoped<movesys_backend_.Fleet.Domain.Repositories.IVehicleRepository, movesys_backend_.Fleet.Infrastructure.Persistence.EFC.Repositories.VehicleRepository>();
builder.Services.AddScoped<movesys_backend_.Fleet.Application.Internal.CommandServices.IVehicleCommandService, movesys_backend_.Fleet.Application.Internal.CommandServices.VehicleCommandService>();
builder.Services.AddScoped<movesys_backend_.Fleet.Application.Internal.QueryServices.IVehicleQueryService, movesys_backend_.Fleet.Application.Internal.QueryServices.VehicleQueryService>();
builder.Services.AddScoped<movesys_backend_.Deliveries.Domain.Repositories.IDeliveryRepository, movesys_backend_.Deliveries.Infrastructure.Persistence.EFC.Repositories.DeliveryRepository>();
builder.Services.AddScoped<movesys_backend_.Deliveries.Application.Internal.CommandServices.IDeliveryCommandService, movesys_backend_.Deliveries.Application.Internal.CommandServices.DeliveryCommandService>();
builder.Services.AddScoped<movesys_backend_.Deliveries.Application.Internal.QueryServices.IDeliveryQueryService, movesys_backend_.Deliveries.Application.Internal.QueryServices.DeliveryQueryService>();
builder.Services.AddScoped<movesys_backend_.Conductores.Domain.Repositories.IConductorRepository, movesys_backend_.Conductores.Infrastructure.Persistence.EFC.Repositories.ConductorRepository>();
builder.Services.AddScoped<movesys_backend_.Conductores.Application.Internal.CommandServices.IConductorCommandService, movesys_backend_.Conductores.Application.Internal.CommandServices.ConductorCommandService>();
builder.Services.AddScoped<movesys_backend_.Conductores.Application.Internal.QueryServices.IConductorQueryService, movesys_backend_.Conductores.Application.Internal.QueryServices.ConductorQueryService>();
builder.Services.AddScoped<movesys_backend_.FuelConsumption.Domain.Repositories.IFuelEntryRepository, movesys_backend_.FuelConsumption.Infrastructure.Persistence.EFC.Repositories.FuelEntryRepository>();
builder.Services.AddScoped<movesys_backend_.FuelConsumption.Application.Internal.CommandServices.IFuelEntryCommandService, movesys_backend_.FuelConsumption.Application.Internal.CommandServices.FuelEntryCommandService>();
builder.Services.AddScoped<movesys_backend_.FuelConsumption.Application.Internal.QueryServices.IFuelEntryQueryService, movesys_backend_.FuelConsumption.Application.Internal.QueryServices.FuelEntryQueryService>();
builder.Services.AddScoped<movesys_backend_.Maintenance.Domain.Repositories.IMaintenanceRecordRepository, movesys_backend_.Maintenance.Infrastructure.Persistence.EFC.Repositories.MaintenanceRecordRepository>();
builder.Services.AddScoped<movesys_backend_.Maintenance.Application.Internal.CommandServices.IMaintenanceRecordCommandService, movesys_backend_.Maintenance.Application.Internal.CommandServices.MaintenanceRecordCommandService>();
builder.Services.AddScoped<movesys_backend_.Maintenance.Application.Internal.QueryServices.IMaintenanceRecordQueryService, movesys_backend_.Maintenance.Application.Internal.QueryServices.MaintenanceRecordQueryService>();
builder.Services.AddScoped<movesys_backend_.Reports.Application.Internal.QueryServices.IReportsQueryService, movesys_backend_.Reports.Application.Internal.QueryServices.ReportsQueryService>();

// Register Command/Query Handlers (Manual registration since we're using a hybrid approach)
builder.Services.AddScoped<movesys_backend_.Conductores.Application.Internal.Handlers.CreateConductorCommandHandler>();
builder.Services.AddScoped<movesys_backend_.Conductores.Application.Internal.Handlers.UpdateConductorCommandHandler>();
builder.Services.AddScoped<movesys_backend_.Conductores.Application.Internal.Handlers.DeleteConductorCommandHandler>();
builder.Services.AddScoped<movesys_backend_.Conductores.Application.Internal.Handlers.UpdateConductorStatusCommandHandler>();
builder.Services.AddScoped<movesys_backend_.Conductores.Application.Internal.Handlers.GetAllConductoresQueryHandler>();
builder.Services.AddScoped<movesys_backend_.Conductores.Application.Internal.Handlers.GetConductorByIdQueryHandler>();

// Event Handlers are automatically registered by AddCortexMediator

var app = builder.Build();

// Ensure database is created (dev). Replace with Migrate() when migrations exist.
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Verificar si la base de datos existe
        var dbExists = ctx.Database.CanConnect();
        
        if (!dbExists)
        {
            // Base de datos no existe, crearla con todas las tablas
            logger.LogInformation("Creando base de datos y todas las tablas...");
            var created = ctx.Database.EnsureCreated();
            logger.LogInformation(created ? " Base de datos y tablas creadas" : "ℹ Base de datos ya existía");
        }
        else
        {
            // Base de datos existe, verificar tablas faltantes
            logger.LogInformation("Base de datos existe, verificando tablas...");
            
            var allTablesExist = true;
            var missingTables = new List<string>();
            
            // Verificar iam_users (IAM Module)
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM iam_users LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("iam_users");
            }
            
            // Verificar drivers (Conductores Module)
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM drivers LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("drivers");
            }
            
            // Verificar vehicles (Fleet Module)
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM vehicles LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("vehicles");
            }
            
            // Verificar deliveries (Deliveries Module)
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM deliveries LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("deliveries");
            }
            
            // Verificar fuel_entries (FuelConsumption Module)
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM fuel_entries LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("fuel_entries");
            }
            
            // Verificar maintenance_records (Maintenance Module)
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM maintenance_records LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("maintenance_records");
            }
            
            if (!allTablesExist)
            {
                logger.LogWarning($"⚠ Faltan las siguientes tablas: {string.Join(", ", missingTables)}");
                logger.LogInformation("Eliminando todas las tablas para recrearlas...");
                
                // En desarrollo, eliminar todas las tablas y recrearlas
                if (app.Environment.IsDevelopment())
                {
                    try
                    {
                        ctx.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");
                        // Eliminar tablas antiguas (si existen)
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS Deliveries;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS Vehicles;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS Conductores;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS FuelEntries;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS MaintenanceRecords;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS IamUsers;");
                        // Eliminar tablas nuevas (si existen)
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS iam_users;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS drivers;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS vehicles;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS deliveries;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS fuel_entries;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS maintenance_records;");
                        ctx.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");
                        
                        logger.LogInformation("Recreando todas las tablas...");
                        ctx.Database.EnsureCreated();
                        
                        logger.LogInformation(" Todas las tablas recreadas exitosamente");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($" Error al recrear tablas: {ex.Message}");
                        throw;
                    }
                }
            }
            else
            {
                logger.LogInformation(" Todas las tablas existen");
                
                // Todas las tablas (iam_users, drivers, vehicles, deliveries, fuel_entries, maintenance_records) 
                // se crean automáticamente con EnsureCreated()
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError($" Error al inicializar base de datos: {ex.Message}");
        logger.LogError($"Stack trace: {ex.StackTrace}");
        throw;
    }
}

// Configure the HTTP request pipeline.
// CORS debe estar al principio, antes de cualquier otro middleware
app.UseCors(allowFrontendCors);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HttpsRedirection puede causar problemas con CORS, comentarlo en desarrollo si es necesario
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();