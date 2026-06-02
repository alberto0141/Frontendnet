using System.Globalization;
using System.Net.Http.Headers;
using frontendnet.Middlewares;
using frontendnet.Models.Validation;
using frontendnet.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("es-MX");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var apiBaseUrl = builder.Configuration["UrlWebAPI"];

if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiUri))
{
    throw new InvalidOperationException("UrlWebAPI no está configurada correctamente.");
}

var isInternalDockerApi =
    apiUri.Scheme == Uri.UriSchemeHttp &&
    string.Equals(apiUri.Host, "backend", StringComparison.OrdinalIgnoreCase);

if (!builder.Environment.IsDevelopment() &&
    apiUri.Scheme != Uri.UriSchemeHttps &&
    !isInternalDockerApi)
{
    throw new InvalidOperationException(
        "UrlWebAPI debe usar HTTPS en producción o usar el host interno backend dentro de Docker."
    );
}

var cookieSecurePolicy = builder.Environment.IsDevelopment()
    ? CookieSecurePolicy.SameAsRequest
    : CookieSecurePolicy.Always;

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "frontendnet.session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = cookieSecurePolicy;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = FileValidation.MaxFileSizeBytes;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "frontendnet.antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = cookieSecurePolicy;
    options.FormFieldName = "__RequestVerificationToken";
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "frontendnet.auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = cookieSecurePolicy;

        options.LoginPath = "/Auth";
        options.LogoutPath = "/Auth/Salir";
        options.AccessDeniedPath = "/Home/AccessDenied";

        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.ReturnUrlParameter = "returnUrl";
    });

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddTransient<EnviaBearerDelegatingHandler>();
builder.Services.AddTransient<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<AuthClientService>(ConfigureApiClient);

builder.Services.AddHttpClient<CategoriasClientService>(ConfigureApiClient)
    .AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
    .AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<ProductosClientService>(ConfigureApiClient)
    .AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
    .AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<PerfilClientService>(ConfigureApiClient)
    .AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
    .AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<RolesClientService>(ConfigureApiClient)
    .AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
    .AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<UsuariosClientService>(ConfigureApiClient)
    .AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
    .AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<ArchivosClientService>(ConfigureApiClient)
    .AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
    .AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<BitacoraClientService>(ConfigureApiClient)
    .AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
    .AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<PedidosClientService>(ConfigureApiClient)
    .AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
    .AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.Use(async (context, next) =>
{
    var apiOrigin = apiUri.GetLeftPart(UriPartial.Authority);

    context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
    context.Response.Headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

    context.Response.Headers.TryAdd(
        "Content-Security-Policy",
        $"default-src 'self'; " +
        $"script-src 'self'; " +
        $"style-src 'self' 'unsafe-inline'; " +
        $"img-src 'self' data: {apiOrigin}; " +
        $"font-src 'self'; " +
        $"connect-src 'self' {apiOrigin}; " +
        $"object-src 'none'; " +
        $"frame-ancestors 'none'; " +
        $"base-uri 'self'; " +
        $"form-action 'self';"
    );

    await next();
});

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

await app.RunAsync();

static void ConfigureApiClient(IServiceProvider services, HttpClient client)
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var configuredUrl = configuration["UrlWebAPI"];

    if (!Uri.TryCreate(configuredUrl, UriKind.Absolute, out var baseUri))
    {
        throw new InvalidOperationException("UrlWebAPI no está configurada correctamente.");
    }

    client.BaseAddress = baseUri;
    client.Timeout = TimeSpan.FromSeconds(30);

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json")
    );
}