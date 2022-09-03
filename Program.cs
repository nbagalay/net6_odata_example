using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using OData_webapi_netcore6.Models;
using OData_webapi_netcore6.Services;
using System.Reflection;

// -- ODATA EDM --
static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new();

    builder.EntitySet<Authors>("Authors")
        .EntityType.HasKey(k => k.Guid);

    builder.EntitySet<Books>("Books")
        .EntityType.HasKey(k => k.Guid);

    return builder.GetEdmModel();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddControllers()
    .AddOData(opt =>
    {
        opt.AddRouteComponents("odata", GetEdmModel());
        opt.Select().Filter().Expand().SetMaxTop(1000).Count().OrderBy().SkipToken();
    });

// -- DATABASE --
builder.Services.AddDbContext<OdataNet6TutorialContext>();

builder.Services.AddSingleton<DefaultSkipTokenHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("odata", new() { Title = "ODataTutorial", Version = "v1" });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Sets configuration so that it can be access at the controller later
// https://stackoverflow.com/questions/39231951/how-do-i-access-configuration-in-any-class-in-asp-net-core
//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/odata/swagger.json", "ODataTutorial v1");
        //c.RoutePrefix = String.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
