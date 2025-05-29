using CoWorking.Controllers;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.Data;
using Coworking.Configs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>(); // necesario para que pueda mandar los emails
builder.Services.AddHttpClient(); // necesario para obtener y adjuntar el codigo qr al email

// Configuración de autenticación y validación de JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
        };
    });

// autorizacion para que solo si el token tiene el claim Admin pueda efectuar la accion (revisar la data del token jwt.io)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

string connectionString = builder.Configuration.GetConnectionString("coworking");
builder.Services.AddDbContext<CoworkingDBContext>(options =>
    options.UseSqlServer(connectionString));

// Configuración de servicios
builder.Services.AddScoped<IAuthService, AuthService>();





builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();

builder.Services.AddScoped<IRolesRepository, RolesRepository>();

builder.Services.AddScoped<ICaracteristicasSalaRepository, CaracteristicasSalaRepository>();

builder.Services.AddScoped<ISedesRepository, SedesRepository>();

builder.Services.AddScoped<ITiposPuestosTrabajoRepository, TiposPuestosTrabajoRepository>();

builder.Services.AddScoped<ITramosHorariosRepository, TramosHorariosRepository>();


builder.Services.AddScoped<ITramosHorariosRepository, TramosHorariosRepository>();
builder.Services.AddScoped<ILineasRepository, LineasRepository>();


builder.Services.AddScoped<ITiposSalasRepository, TiposSalasRepository>();

builder.Services.AddScoped<ISalasRepository, SalasRepository>();   

builder.Services.AddScoped<IZonasTrabajoRepository, ZonasTrabajoRepository>();   

    

builder.Services.AddScoped<IDisponibilidadesRepository, DisponibilidadesRepository>();
builder.Services.AddScoped<DisponibilidadesService>();

// Add services
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<ITiposSalasService, TiposSalasService>();
builder.Services.AddScoped<ISedesService, SedesService>();
builder.Services.AddScoped<ITiposPuestosTrabajoService, TiposPuestosTrabajoService>();
builder.Services.AddScoped<ISalasService, SalasService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<ITramosHorariosService, TramosHorariosService>();
builder.Services.AddScoped<IReservasService, ReservasService>();
builder.Services.AddScoped<ILineasService, LineasService>();
builder.Services.AddScoped<IDisponibilidadesService, DisponibilidadesService>();
builder.Services.AddScoped<ICaracteristicasSalaService, CaracteristicasSalaService>();
builder.Services.AddScoped<IZonasTrabajoService, ZonasTrabajoService>();
builder.Services.AddScoped<IReservasRepository, ReservasRepository>();

builder.Services.AddScoped<IPuestosTrabajoService, PuestosTrabajoService>(); 
builder.Services.AddScoped<IPuestosTrabajoRepository, PuestosTrabajoRepository>(); 
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEstadisticasRepository, EstadisticasRepository>();
builder.Services.AddScoped<IEstadisticasService, EstadisticasService>();



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // para que no salga el   "$id": "1",  "$values": [

    });

    
// Configuración de CORS para permitir cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });

    // Seguridad en Swagger para Bearer token
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplicar CORS a toda la API
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();