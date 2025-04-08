using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Services;  // Agregamos el espacio de nombres del servicio de correo
//using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar el DbContext para Identity y otros servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Configuración de autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Ruta de inicio de sesión
        options.LogoutPath = "/Account/Logout"; // Ruta de cierre de sesión
        options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta para acceso denegado (opcional)
    });// Agregar servicios de autorización si es necesario
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdministradorPolicy", policy =>
        policy.RequireRole("Administrador"));
    options.AddPolicy("InstructorPolicy", policy =>
        policy.RequireRole("Instructor"));
    options.AddPolicy("AlumnoPolicy", policy =>
        policy.RequireRole("Alumno"));
});

// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
/*// Configuración de Hangfire (para tareas programadas)
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();*/

// Agregar servicio de correo
builder.Services.AddTransient<EmailService>(); // Registro del servicio de correo
var app = builder.Build();
// Crear el servicio de proveedor de servicios y pasar al método SeedData.Initialize
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedData.Initialize(services, userManager, roleManager); // Inicializa los datos
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Activar autenticación y autorización
app.UseAuthentication();  // Añadir autenticación
app.UseAuthorization();   // Añadir autorización
/*
// Configurar Hangfire Dashboard (opcional)
app.UseHangfireDashboard("/hangfire"); // Ruta para ver el panel de Hangfire*/

// Rutas para los controladores
app.MapControllerRoute(
    name: "login",
    pattern: "{controller=Account}/{action=Login}");


app.MapControllerRoute(
    
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
