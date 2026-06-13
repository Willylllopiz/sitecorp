using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SiteCorp.Shared;
using SiteCorp.Web.Components;
using SiteCorp.Web.Clients;
using SiteCorp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/login";
        options.Cookie.Name = "SiteCorp.Auth";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();

var backendBaseUrl = builder.Configuration["BackendApi:BaseUrl"] ?? "http://localhost:5099/";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(backendBaseUrl) });
builder.Services.AddScoped<AuthenticationApiClient>();
builder.Services.AddScoped<HumanResourcesApiClient>();
builder.Services.AddScoped<HumanResourcesViewService>();
builder.Services.AddSingleton<SampleHumanResourcesData>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/sign-in", async (HttpContext httpContext, AuthenticationApiClient authClient, CancellationToken cancellationToken) =>
{
    var form = await httpContext.Request.ReadFormAsync(cancellationToken);
    var userName = form["userName"].ToString();
    var password = form["password"].ToString();

    try
    {
        var response = await authClient.LoginAsync(new LoginRequest(userName, password), cancellationToken);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, response.User.UserId.ToString()),
            new(ClaimTypes.Name, response.User.UserName),
            new(ClaimTypes.Email, response.User.Email),
            new("company_id", response.User.CompanyId.ToString()),
            new("full_name", response.User.FullName),
            new("access_token", response.AccessToken)
        };

        claims.AddRange(response.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(response.User.Permissions.Select(permission => new Claim("permission", permission)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        properties.StoreTokens(
        [
            new AuthenticationToken { Name = "access_token", Value = response.AccessToken },
            new AuthenticationToken { Name = "refresh_token", Value = response.RefreshToken },
            new AuthenticationToken { Name = "expires_at", Value = response.AccessTokenExpiresAt.ToString("O") }
        ]);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        return Results.Redirect("/");
    }
    catch
    {
        return Results.Redirect("/login?error=invalid");
    }
}).DisableAntiforgery();

app.MapGet("/logout", async (HttpContext httpContext, AuthenticationApiClient authClient, CancellationToken cancellationToken) =>
{
    var refreshToken = await httpContext.GetTokenAsync("refresh_token");

    if (!string.IsNullOrWhiteSpace(refreshToken))
    {
        await authClient.LogoutAsync(new LogoutRequest(refreshToken), cancellationToken);
    }

    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
