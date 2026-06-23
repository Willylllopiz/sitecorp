using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SiteCorp.Api.Authentication;
using SiteCorp.Application.Authentication;
using SiteCorp.Application.HumanResources;
using SiteCorp.Domain.Common;
using SiteCorp.Infrastructure;
using SiteCorp.Infrastructure.Data;
using SiteCorp.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Auth:Jwt"));
builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection("Auth"));
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<IAccessTokenService, JwtAccessTokenService>();
builder.Services.AddScoped<HumanResourcesService>();
builder.Services.AddSiteCorpInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Auth:Jwt").Get<JwtOptions>() ?? new JwtOptions();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("SiteCorpClient", policy =>
    {
        var origins = builder.Configuration.GetSection("ClientOrigins").Get<string[]>()
            ?? ["http://localhost:5193", "https://localhost:7193"];

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    await app.Services.ApplySiteCorpMigrationsAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("SiteCorpClient");
app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api");

api.MapGet("/health", () => Results.Ok(new
{
    service = "SiteCorp.Api",
    status = "ok",
    checkedAt = DateTimeOffset.UtcNow
}));

var auth = api.MapGroup("/auth");

auth.MapPost("/login", async (LoginRequest request, AuthenticationService service, HttpContext httpContext, CancellationToken cancellationToken) =>
{
    try
    {
        var response = await service.LoginAsync(
            request,
            httpContext.Connection.RemoteIpAddress?.ToString(),
            httpContext.Request.Headers.UserAgent.ToString(),
            cancellationToken);

        return Results.Ok(response);
    }
    catch (AuthenticationException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

auth.MapPost("/refresh", async (RefreshTokenRequest request, AuthenticationService service, HttpContext httpContext, CancellationToken cancellationToken) =>
{
    try
    {
        var response = await service.RefreshAsync(
            request,
            httpContext.Connection.RemoteIpAddress?.ToString(),
            httpContext.Request.Headers.UserAgent.ToString(),
            cancellationToken);

        return Results.Ok(response);
    }
    catch (AuthenticationException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

auth.MapPost("/logout", async (LogoutRequest request, AuthenticationService service, HttpContext httpContext, CancellationToken cancellationToken) =>
{
    await service.LogoutAsync(request, httpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
    return Results.NoContent();
});

auth.MapGet("/me", async (ClaimsPrincipal user, AuthenticationService service, CancellationToken cancellationToken) =>
{
    var userIdValue = user.FindFirstValue("user_id") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

    if (!int.TryParse(userIdValue, out var userId))
    {
        return Results.Unauthorized();
    }

    try
    {
        return Results.Ok(await service.GetCurrentUserAsync(userId, cancellationToken));
    }
    catch (AuthenticationException)
    {
        return Results.Unauthorized();
    }
}).RequireAuthorization();

var protectedApi = api.MapGroup("").RequireAuthorization();

protectedApi.MapGet("/snapshot", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetSnapshotAsync(cancellationToken)));

protectedApi.MapGet("/dashboard", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetDashboardMetricsAsync(cancellationToken)));

protectedApi.MapGet("/departments", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetDepartmentsAsync(cancellationToken)));

protectedApi.MapGet("/employees", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetEmployeesAsync(cancellationToken)));

protectedApi.MapGet("/positions", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetPositionsAsync(cancellationToken)));

protectedApi.MapGet("/leave-requests", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetLeaveRequestsAsync(cancellationToken)));

protectedApi.MapGet("/organization/entities", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetOrganizationEntitiesAsync(cancellationToken)));

protectedApi.MapPost("/organization/business-groups", async (
    CreateBusinessGroupRequest request,
    HumanResourcesService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var created = await service.CreateBusinessGroupAsync(request, cancellationToken);
        return Results.Created($"/api/organization/entities/{created.Id}", created);
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

protectedApi.MapPost("/organization/companies", async (
    CreateCompanyRequest request,
    HumanResourcesService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var created = await service.CreateCompanyAsync(request, cancellationToken);
        return Results.Created($"/api/organization/entities/{created.Id}", created);
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

protectedApi.MapPost("/organization/business-units", async (
    CreateBusinessUnitRequest request,
    HumanResourcesService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var created = await service.CreateBusinessUnitAsync(request, cancellationToken);
        return Results.Created($"/api/organization/entities/{created.Id}", created);
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

protectedApi.MapGet("/hr/catalogs", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetCatalogsAsync(cancellationToken)));

protectedApi.MapGet("/people", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetPeopleAsync(cancellationToken)));

protectedApi.MapPost("/people", async (
    CreatePersonRequest request,
    HumanResourcesService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var created = await service.CreatePersonAsync(request, cancellationToken);
        return Results.Created($"/api/people/{created.Id}", created);
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

protectedApi.MapGet("/staffing/positions", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetStaffingPositionsAsync(cancellationToken)));

protectedApi.MapGet("/staffing/job-templates", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetJobTemplatesAsync(cancellationToken)));

protectedApi.MapGet("/staffing/job-template-positions", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetJobTemplatePositionsAsync(cancellationToken)));

protectedApi.MapPost("/staffing/positions", async (
    CreateStaffingPositionRequest request,
    HumanResourcesService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var created = await service.CreateStaffingPositionAsync(request, cancellationToken);
        return Results.Created($"/api/staffing/positions/{created.Id}", created);
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

protectedApi.MapPost("/staffing/job-templates", async (
    CreateJobTemplateRequest request,
    HumanResourcesService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var created = await service.CreateJobTemplateAsync(request, cancellationToken);
        return Results.Created($"/api/staffing/job-templates/{created.Id}", created);
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

protectedApi.MapPost("/staffing/job-templates/{jobTemplateId:guid}/positions", async (
    Guid jobTemplateId,
    AddJobTemplatePositionRequest request,
    HumanResourcesService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var created = await service.AddJobTemplatePositionAsync(jobTemplateId, request, cancellationToken);
        return Results.Created($"/api/staffing/job-template-positions/{created.Id}", created);
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

protectedApi.MapPost("/hiring", async (
    HirePersonRequest request,
    HumanResourcesService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        var created = await service.HirePersonAsync(request, cancellationToken);
        return Results.Created($"/api/hiring/{created.Id}", created);
    }
    catch (DomainException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

protectedApi.MapGet("/hiring", async (HumanResourcesService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetEmploymentsAsync(cancellationToken)));

app.Run();
