using AutorizacaoAutenticacao.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.RateLimiting;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter(policyName: "ApiBlock", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(30);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";

        object responseContent;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            responseContent = new
            {
                Sucesso = false,
                Mensagem = $"Você excedeu o máximo de requests no momento. Tente novamente em {retryAfter.TotalSeconds} segundo(s)."
            };
        }
        else
        {
            responseContent = new
            {
                Sucesso = false,
                Mensagem = "Você excedeu o máximo de requests no momento. Tente novamente mais tarde."
            };
        }

        var jsonResult = new JsonResult(responseContent);
        await jsonResult.ExecuteResultAsync(new ActionContext
        {
            HttpContext = context.HttpContext
        });
    };

    // Global
    //options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    //    RateLimitPartition.GetFixedWindowLimiter(
    //        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
    //        factory: partition => new FixedWindowRateLimiterOptions
    //        {
    //            AutoReplenishment = true,
    //            PermitLimit = 10,
    //            QueueLimit = 0,
    //            Window = TimeSpan.FromMinutes(1)
    //        }));
});

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("Token").Value!))
    };
});

builder.Services.AddScoped<TokenService>();

var app = builder.Build();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
