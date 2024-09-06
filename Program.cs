using Firebase.Database;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Interfaces;
using home_pisos_vinilicos.Application.Mapping;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Data;
using home_pisos_vinilicos.Data.Repositories.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Servicios con interfaz
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

//Servicios sin interfaz
builder.Services.AddScoped<ProductService>();
builder.Services.AddSingleton<WeatherForecastService>();

// Repositorios
builder.Services.AddScoped<IProductRepository, ProductRepository>(); 

//Automapper
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
