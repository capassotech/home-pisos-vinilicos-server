using Firebase.Database;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Interfaces;
using home_pisos_vinilicos.Application.Mapping;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Data;
using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Data.Repositories.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

// Servicios con interfaz
//builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ISecureDataService, SecureDataService>();

// Servicios sin interfaz
builder.Services.AddScoped<ProductService>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient();

// Repositorios
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISecureDataRepository, SecureDataRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();  // Registrar ILoginRepository y LoginRepository
builder.Services.AddScoped(typeof(IRepository<>), typeof(FirebaseRepository<>));

// Automapper
builder.Services.AddAutoMapper(typeof(Mapping));

// Firebase Client
builder.Services.AddScoped<FirebaseClient>(sp => new FirebaseClient("https://home-pisos-vinilicos-default-rtdb.firebaseio.com/"));

// Configure HttpClient
builder.Services.AddHttpClient("MyApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7223/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
