using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application;
using Movies.Application.Database;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//��Ȩ����
builder.Services.AddAuthorization(x=>
{
    x.AddPolicy(AuthConstants.AdminUserPolicyName,p=>p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));
    x.AddPolicy(AuthConstants.TrustedMemberPolicyName, p=>p.RequireAssertion(c=>
        c.User.HasClaim(m=>m is {Type:AuthConstants.AdminUserClaimName,Value:"true" })||
        c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" }))
    );
});

//��֤����
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


//���ҵ�����
builder.Services.AddApplication();

//������ݿ�
builder.Services.AddDatabase(config.GetConnectionString("SqlServer")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//��Ȩ����֤
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();  

app.Run();
