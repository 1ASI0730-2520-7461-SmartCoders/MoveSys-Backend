using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Cortex.Mediator.DependencyInjection;

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
    c.SchemaFilter<movesys_backend_.Users.Infrastructure.Swagger.UserCreateExampleSchemaFilter>();
});

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowFrontendCors, policy =>
    {
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
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
        var serverVersion = ServerVersion.AutoDetect(mySqlCs);
        options.UseMySql(mySqlCs, serverVersion);
    }
});

// Configure Mediator (Cortex.Mediator)
builder.Services.AddCortexMediator(
    configuration: builder.Configuration,
    handlerAssemblyMarkerTypes: [typeof(Program)]);

// Register Shared and Bounded Context services (Repositories, UoW, Services)
builder.Services.AddScoped<movesys_backend_.Shared.Domain.Repositories.IUnitOfWork, movesys_backend_.Shared.Infrastructure.Persistence.EFC.Configuration.UnitOfWork>();
builder.Services.AddScoped<movesys_backend_.Fleet.Domain.Repositories.IVehicleRepository, movesys_backend_.Fleet.Infrastructure.Persistence.EFC.Repositories.VehicleRepository>();
builder.Services.AddScoped<movesys_backend_.Fleet.Application.Internal.CommandServices.IVehicleCommandService, movesys_backend_.Fleet.Application.Internal.CommandServices.VehicleCommandService>();
builder.Services.AddScoped<movesys_backend_.Fleet.Application.Internal.QueryServices.IVehicleQueryService, movesys_backend_.Fleet.Application.Internal.QueryServices.VehicleQueryService>();
builder.Services.AddScoped<movesys_backend_.Deliveries.Domain.Repositories.IDeliveryRepository, movesys_backend_.Deliveries.Infrastructure.Persistence.EFC.Repositories.DeliveryRepository>();
builder.Services.AddScoped<movesys_backend_.Deliveries.Application.Internal.CommandServices.IDeliveryCommandService, movesys_backend_.Deliveries.Application.Internal.CommandServices.DeliveryCommandService>();
builder.Services.AddScoped<movesys_backend_.Deliveries.Application.Internal.QueryServices.IDeliveryQueryService, movesys_backend_.Deliveries.Application.Internal.QueryServices.DeliveryQueryService>();
builder.Services.AddScoped<movesys_backend_.Users.Domain.Repositories.IUserRepository, movesys_backend_.Users.Infrastructure.Persistence.EFC.Repositories.UserRepository>();
builder.Services.AddScoped<movesys_backend_.Users.Application.Internal.CommandServices.IUserCommandService, movesys_backend_.Users.Application.Internal.CommandServices.UserCommandService>();
builder.Services.AddScoped<movesys_backend_.Users.Application.Internal.QueryServices.IUserQueryService, movesys_backend_.Users.Application.Internal.QueryServices.UserQueryService>();
builder.Services.AddScoped<movesys_backend_.FuelConsumption.Domain.Repositories.IFuelEntryRepository, movesys_backend_.FuelConsumption.Infrastructure.Persistence.EFC.Repositories.FuelEntryRepository>();
builder.Services.AddScoped<movesys_backend_.FuelConsumption.Application.Internal.CommandServices.IFuelEntryCommandService, movesys_backend_.FuelConsumption.Application.Internal.CommandServices.FuelEntryCommandService>();
builder.Services.AddScoped<movesys_backend_.FuelConsumption.Application.Internal.QueryServices.IFuelEntryQueryService, movesys_backend_.FuelConsumption.Application.Internal.QueryServices.FuelEntryQueryService>();
builder.Services.AddScoped<movesys_backend_.Maintenance.Domain.Repositories.IMaintenanceRecordRepository, movesys_backend_.Maintenance.Infrastructure.Persistence.EFC.Repositories.MaintenanceRecordRepository>();
builder.Services.AddScoped<movesys_backend_.Maintenance.Application.Internal.CommandServices.IMaintenanceRecordCommandService, movesys_backend_.Maintenance.Application.Internal.CommandServices.MaintenanceRecordCommandService>();
builder.Services.AddScoped<movesys_backend_.Maintenance.Application.Internal.QueryServices.IMaintenanceRecordQueryService, movesys_backend_.Maintenance.Application.Internal.QueryServices.MaintenanceRecordQueryService>();
builder.Services.AddScoped<movesys_backend_.Reports.Application.Internal.QueryServices.IReportsQueryService, movesys_backend_.Reports.Application.Internal.QueryServices.ReportsQueryService>();

// Register Command/Query Handlers (Manual registration since we're using a hybrid approach)
builder.Services.AddScoped<movesys_backend_.Users.Application.Internal.Handlers.CreateUserCommandHandler>();
builder.Services.AddScoped<movesys_backend_.Users.Application.Internal.Handlers.UpdateUserCommandHandler>();
builder.Services.AddScoped<movesys_backend_.Users.Application.Internal.Handlers.DeleteUserCommandHandler>();
builder.Services.AddScoped<movesys_backend_.Users.Application.Internal.Handlers.UpdateUserStatusCommandHandler>();
builder.Services.AddScoped<movesys_backend_.Users.Application.Internal.Handlers.GetAllUsersQueryHandler>();
builder.Services.AddScoped<movesys_backend_.Users.Application.Internal.Handlers.GetUserByIdQueryHandler>();

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
            
            // Verificar Vehicles
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM Vehicles LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("Vehicles");
            }
            
            // Verificar Deliveries
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM Deliveries LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("Deliveries");
            }
            
            // Verificar FuelEntries
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM FuelEntries LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("FuelEntries");
            }
            
            // Verificar MaintenanceRecords
            try
            {
                ctx.Database.ExecuteSqlRaw("SELECT 1 FROM MaintenanceRecords LIMIT 1;");
            }
            catch
            {
                allTablesExist = false;
                missingTables.Add("MaintenanceRecords");
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
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS Deliveries;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS Vehicles;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS Users;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS FuelEntries;");
                        ctx.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS MaintenanceRecords;");
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
                
                // En desarrollo, eliminar columnas que no deberían existir en Users
                if (app.Environment.IsDevelopment())
                {
                    try
                    {
                        // Verificar si existe la columna Email
                        var emailColumnExists = false;
                        try
                        {
                            ctx.Database.ExecuteSqlRaw("SELECT Email FROM Users LIMIT 1;");
                            emailColumnExists = true;
                        }
                        catch
                        {
                            emailColumnExists = false;
                        }
                        
                        // Verificar si existe la columna Password
                        var passwordColumnExists = false;
                        try
                        {
                            ctx.Database.ExecuteSqlRaw("SELECT Password FROM Users LIMIT 1;");
                            passwordColumnExists = true;
                        }
                        catch
                        {
                            passwordColumnExists = false;
                        }
                        
                        // Eliminar columnas si existen
                        if (emailColumnExists || passwordColumnExists)
                        {
                            logger.LogInformation("Eliminando columnas Email y Password de la tabla Users...");
                            
                            if (emailColumnExists)
                            {
                                ctx.Database.ExecuteSqlRaw("ALTER TABLE Users DROP COLUMN Email;");
                                logger.LogInformation(" Columna Email eliminada");
                            }
                            
                            if (passwordColumnExists)
                            {
                                ctx.Database.ExecuteSqlRaw("ALTER TABLE Users DROP COLUMN Password;");
                                logger.LogInformation(" Columna Password eliminada");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning($"⚠️ No se pudieron eliminar las columnas Email/Password: {ex.Message}");
                        // No lanzamos excepción, solo registramos un warning
                    }
                }
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
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(allowFrontendCors);
app.UseAuthorization();
app.MapControllers();
app.Run();