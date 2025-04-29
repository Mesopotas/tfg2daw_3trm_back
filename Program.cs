using CoWorking.Controllers;
using CoWorking.Repositories;
using CoWorking.Service;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("coworking");

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

// Configuración de servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>(provider =>
        new UsuariosRepository(connectionString));

builder.Services.AddScoped<ITipoSalasRepository, TipoSalasRepository>(provider =>
    new TipoSalasRepository(connectionString));

builder.Services.AddScoped<ISedesRepository, SedesRepository>(provider =>
    new SedesRepository(connectionString));

builder.Services.AddScoped<ISalasRepository, SalasRepository>(provider =>
    new SalasRepository(connectionString));  

builder.Services.AddScoped<IRolesRepository, RolesRepository>(provider =>
    new RolesRepository(connectionString));    

builder.Services.AddScoped<ITramosHorariosRepository, TramosHorariosRepository>(provider =>
    new TramosHorariosRepository(connectionString)); 

builder.Services.AddScoped<IDetallesReservasRepository, DetallesReservasRepository>(provider =>
    new DetallesReservasRepository(connectionString));        

builder.Services.AddScoped<IReservasRepository, ReservasRepository>(provider =>
    new ReservasRepository(connectionString));  

builder.Services.AddScoped<ILineasRepository, LineasRepository>(provider =>
    new LineasRepository(connectionString));        
builder.Services.AddScoped<IDisponibilidadesRepository, DisponibilidadesRepository>(provider =>
    new DisponibilidadesRepository(connectionString)); 

// Add services
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<ITipoSalasService, TipoSalasService>();
builder.Services.AddScoped<ISedesService, SedesService>();
builder.Services.AddScoped<ISalasService, SalasService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<ITramosHorariosService, TramosHorariosService>();
builder.Services.AddScoped<IDetallesReservasService, DetallesReservasService>();
builder.Services.AddScoped<IReservasService, ReservasService>();
builder.Services.AddScoped<ILineasService, LineasService>();
builder.Services.AddScoped<IDisponibilidadesService, DisponibilidadesService>();


// Configuración de controladores
builder.Services.AddControllers();

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