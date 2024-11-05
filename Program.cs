using CurrieTechnologies.Razor.SweetAlert2;
using Firebase.Database;
using home_pisos_vinilicos.Application.Interfaces;
using home_pisos_vinilicos.Application.Mapping;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.Services.Firebase;
using home_pisos_vinilicos.Data;
using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Data.Repositories.IRepository;

var builder = WebApplication.CreateBuilder(args);

FirebaseInitializer.InitializeFirebase();

builder.Services.AddScoped<FirebaseClient>(sp =>
    new FirebaseClient("https://home-pisos-vinilicos-default-rtdb.firebaseio.com/"));

builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>((sp, httpClient) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    httpClient.BaseAddress = new Uri(configuration["Authentication: TokenUri"]!);

});

builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60); // Aumenta el timeout
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);  // Intervalo para mantener la conexión activa
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

builder.Services.AddScoped<ISecureDataService, SecureDataService>();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<FAQService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<SocialNetworkService>();

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFAQRepository, FAQRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ISocialNetworkRepository, SocialNetworkRepository>();

builder.Services.AddScoped<ISecureDataRepository, SecureDataRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(FirebaseRepository<>));
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAutoMapper(typeof(Mapping));

builder.Services.AddSweetAlert2();

builder.Services.AddScoped<FirebaseClient>(sp => new FirebaseClient("https://home-pisos-vinilicos-default-rtdb.firebaseio.com/"));

builder.Services.AddScoped(sp =>
{
    var tokenService = sp.GetRequiredService<ITokenService>();
    var handler = new AuthMessageHandler(tokenService)
    {
        InnerHandler = new HttpClientHandler()
    };
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7223/") // Cambia por la URL de tu API
    };
});

var app = builder.Build();
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