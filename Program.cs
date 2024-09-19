using CurrieTechnologies.Razor.SweetAlert2;
using Firebase.Database;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Interfaces;
using home_pisos_vinilicos.Application.Mapping;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.Services.Firebase;
using home_pisos_vinilicos.Data;
using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Data.Repositories.IRepository;


var builder = WebApplication.CreateBuilder(args);



FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase.json")
});



//builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>((sp, httpClient) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    httpClient.BaseAddress = new Uri(configuration["Authentication: TokenUri"]!);

});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

// Servicios con interfaz
//builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ISecureDataService, SecureDataService>();

// Servicios sin interfaz
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<SubCategoryService>();


builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Repositorios
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

builder.Services.AddScoped<ISecureDataRepository, SecureDataRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(FirebaseRepository<>));

// Automapper
builder.Services.AddAutoMapper(typeof(Mapping));

//sweet alert
builder.Services.AddSweetAlert2();

// Firebase Client
builder.Services.AddScoped<FirebaseClient>(sp => new FirebaseClient("https://home-pisos-vinilicos-default-rtdb.firebaseio.com/"));

// Configure HttpClient
builder.Services.AddHttpClient("MyApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7223/");
});

var app = builder.Build();
// Middleware to check Firebase token
//app.UseMiddleware<FirebaseSessionMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
//app.UseAuthorization(); // Ensure authorization is enabled
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();