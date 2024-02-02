using Diplom_project_2024.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<HousesDBContext>();

builder.Services.AddDbContext<HousesDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HousesDb"));
    //options.UseSqlServer(builder.Configuration["HousesDB"]);
});

//builder.Services.AddAzureClients(clientBuilder =>
//{
//    clientBuilder.AddBlobServiceClient(builder.Configuration["HouseContainerBlob"], preferMsi: true);
//    clientBuilder.AddQueueServiceClient(builder.Configuration["HouseContainerQueue"], preferMsi: true);
//});

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
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["blob-string:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["blob-string:queue"], preferMsi: true);
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
