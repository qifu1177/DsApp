
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DS.Api;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.GetSection("LoggingFile");
var excutFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
var logPath = Path.Combine((new FileInfo(excutFile)).DirectoryName, config.Value);
builder.Logging.AddFile(logPath);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
var assemblies = AssemblyProvider.GetAssembliesFromProjects("DS");
builder.Host.ConfigureContainer<ContainerBuilder>(b => b.RegisterAssemblyModules<Autofac.Module>(assemblies));
//builder.Host.ConfigureContainer<ContainerBuilder>(b => b.RegisterModule(new DS.Api.Module(builder.Configuration)));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
