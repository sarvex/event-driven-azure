
using Invoices.Data;
using Invoices.Data.Models.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//register services

var dbConfig = builder.Configuration.GetSection("DbConfig").Get<DbConfig>();
var storageConfig = new StorageConfig
{
    ConnectionString = builder.Configuration.GetConnectionString("StorageConnectionString"),
    ContainerName = "invoices"
};

builder.Services.RegisterInvoiceDataServices(storageConfig, dbConfig);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
