using Diplom_project_2024.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Diplom_project_2024.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

//DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
//{
//    Diagnostics =
//    {
//        LoggedHeaderNames = { "x-ms-request-id" },
//        LoggedQueryParameters = { "api-version" },
//        IsLoggingContentEnabled = true
//    }
//};
//
var keyVaultEndpoint = new Uri("https://diplomproject2024vault.vault.azure.net/");
//builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential(options));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());




// Add services to the container.

builder.Services.AddControllers();

//Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddIdentityApiEndpoints<User>().AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<HousesDBContext>();
builder.Services.AddDbContext<HousesDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HousesDb"));
    //options.UseSqlServer(builder.Configuration["HousesDB"]);
});

//builder.Services.AddAzureClients(clientBuilder =>
//{
//    //clientBuilder.AddBlobServiceClient(builder.Configuration["HouseContainerBlob"], preferMsi: true);
//    //clientBuilder.AddQueueServiceClient(builder.Configuration["HouseContainerQueue"], preferMsi: true); 

//    clientBuilder
//    .AddBlobServiceClient(builder.Configuration["blob-string--blob"], preferMsi: true);
//    clientBuilder
//    .AddQueueServiceClient(builder.Configuration["blob-string--queue"], preferMsi: true);
//});
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["blob-string"]!, preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["blob-string"]!, preferMsi: true);
});

//builder.Services.AddAuthentication().AddGoogle(options =>
//{
//    //IConfigurationSection googlesection = configuration.GetSection("Authentication:Google");
//    //options.ClientId = googlesection.GetValue<string>("ClientId");
//    //options.ClientSecret = googlesection["ClientSecret"];
//    options.ClientId = configuration.GetValue<string>("GoogleClientId");
//    options.ClientSecret = configuration["GoogleClientSecret"];
//    options.ClaimActions.MapJsonKey("picture", "picture", "url");
//    options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization(); //authorization

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
