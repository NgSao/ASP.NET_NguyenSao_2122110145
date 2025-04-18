//Dùng swager để test API
//http://localhost:5037/swagger/index.html
//dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
//http://localhost:5037/api/brands 
using NguyenSao_2122110145.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using NguyenSao_2122110145.Mappings;
using NguyenSao_2122110145.Services;
using Octokit;

var builder = WebApplication.CreateBuilder(args);



// Đăng ký dịch vụ DbContext với MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection"))
    ));
builder.Services.AddScoped<IEmailService, EmailService>();

//Mappber
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.WebHost.UseUrls("http://192.168.1.111:5000", "http://0.0.0.0:5000");
// Đăng ký CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});



//Đăng ký IGitHubClient: Thêm đoạn mã sau vào Program.cs để cấu hình GitHub client:
builder.Services.AddSingleton<IGitHubClient>(new GitHubClient(new ProductHeaderValue("NguyenSao-API"))
{
    Credentials = new Credentials(builder.Configuration["GitHub:PersonalAccessToken"])
});

builder.Services.AddHttpClient();


// Đăng ký dịch vụ xác thực bằng JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))

    };

});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// Cấu hình Swagger có hỗ trợ Bearer Token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NguyenSao API", Version = "v1" });

    // Định nghĩa Bearer Token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token theo định dạng: Bearer {token}"
    });

    // Gắn Bearer vào tất cả các endpoint có [Authorize]
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
            Array.Empty<string>()
        }
    });
});




var app = builder.Build();

// Chỉ bật Swagger UI ở môi trường Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Thêm trước UseSwagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
