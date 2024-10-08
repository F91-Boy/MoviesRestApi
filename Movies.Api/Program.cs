using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth;
using Movies.Api.Endpoints;
using Movies.Api.Health;
using Movies.Api.Mapping;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


//builder.Services.AddControllers();

//授权策略
builder.Services.AddAuthorization(x=>
{
    //x.AddPolicy(AuthConstants.AdminUserPolicyName,p=>p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));
    //混合策略
    x.AddPolicy(AuthConstants.AdminUserPolicyName, p => p.AddRequirements(new AdminAuthRequirement(config["ApiKey"]!)));

    x.AddPolicy(AuthConstants.TrustedMemberPolicyName, p=>p.RequireAssertion(c=>
        c.User.HasClaim(m=>m is {Type:AuthConstants.AdminUserClaimName,Value:"true" })||
        c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" }))
    );
});

//认证配置
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme =JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme =JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Authience"],
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
    };
});

//添加版本控制
builder.Services.AddApiVersioning(x=>
{
    x.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
}).AddMvc().AddApiExplorer();

//添加swagger
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x=>x.OperationFilter<SwaggerDefaultValues>());

//builder.Services.AddEndpointsApiExplorer();

//添加业务服务
builder.Services.AddApplication();

//添加数据库
builder.Services.AddDatabase(config.GetConnectionString("SqlServer")!);

//添加健康检查
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

//响应缓存
//builder.Services.AddResponseCaching();
//输出缓存
builder.Services.AddOutputCache(x =>
{
    x.AddBasePolicy(c => c.Cache());
    x.AddPolicy("MovieCache", c =>
        c.Cache()
        .Expire(TimeSpan.FromMinutes(1))
        .SetVaryByQuery(["title", "yearOfRelease", "sortBy", "page", "pageSize"])
        .Tag("movies"));
});


builder.Services.AddScoped<ApiKeyAuthFilter>();

var app = builder.Build();

app.MapHealthChecks("_health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",description.GroupName);
        }
    });
}

app.UseHttpsRedirection();

//授权与认证
app.UseAuthentication();
app.UseAuthorization();


//app.UseCors();
//响应缓存
//app.UseResponseCaching();

//输出缓存,默认只缓存OK200,GET和HEAD请求
app.UseOutputCache();

app.UseMiddleware<ValidationMappingMiddleware>();

//app.MapControllers();
app.MapApiEndpoints();//启用最小api

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();  

app.Run();
