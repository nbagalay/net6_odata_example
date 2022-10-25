using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using OData_webapi_netcore6.Models;
using OData_webapi_netcore6.Security;
using OData_webapi_netcore6.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Xml.Linq;

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
    // ER Diagram generation with use of "oasis-tcs/odata - openapi" from GitHub
    c.SwaggerDoc("odata", new() { 
        Title = "ODataTutorial", 
        Description = "NET 6 Odata Sample Created by Nick Bagalay" +
        "<br>" +
        "![ER Diagram](https://yuml.me/diagram/class/[Authors{bg:orange}],[Authors]-*>[Books{bg:orange}],[Books{bg:orange}],[Books]-0..1>[Authors{bg:orange}],[Authors{bg:dodgerblue}]++-*>[Authors],[Books{bg:dodgerblue}]++-*>[Books])\n" +
        "<br><b>Legend</b><br>" +
        "![Legend](https://yuml.me/diagram/plain;dir:TB;scale:75/class/[External.Type{bg:whitesmoke}],[ComplexType],[EntityType{bg:orange}],[EntitySet/Singleton/Operation{bg:dodgerblue}])",
        Version = "v1" });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// ****** A U T H O R I Z A T I O N *******
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
     {
         c.Authority = $"https://{builder.Configuration["Security:Domain"]}";
         c.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
         {
             ValidAudience = builder.Configuration["Security:Audience"],
             ValidIssuer = $"{builder.Configuration["Security:Domain"]}"
         };

         //-----ADDITION: Obtaining the Account details
         c.Events = new JwtBearerEvents
         {
             OnTokenValidated = context =>
             {
                 // Grab the raw value of the token, and store it as a claim so we can retrieve it again later in the request pipeline
                 // Have a look at the UsersController.UserInformation() method to see how to retrieve it and use it to retrieve the
                 // user's information from the /auth0userinfo endpoint
                 if (context.SecurityToken is JwtSecurityToken token)
                 {
                     if (context.Principal.Identity is ClaimsIdentity identity)
                     {
                         identity.AddClaim(new Claim("access_token", token.RawData));

                         // Account Details to Claims
                         var accountKey = token.Claims.FirstOrDefault(f => f.Type == "sub").Value;
                         identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, accountKey));
                     }
                 }

                 return Task.FromResult(0);
             }
         };
     });

builder.Services.AddAuthorization(o =>
{
    /* --SPECIAL NOTE--
     * Depending on your Identity Provider (IP), 'SCOPES' tend to be a string (ex: 'read:authors write:authors'. Hopefully the IP
     * will have an array. Auth0 has one called 'permissions' that breaks out the scopes into an array. C# needs this into an array. If
     * your IP doesn't have an array, you can use the line below to add the Scopes then uncomment the 
     * 'AddSingleton<IAuthorizationHandler, HasScopeHandler>()' to break the scopes out.
     */

    //o.AddPolicy("read:authors", policy => policy.Requirements.Add(new HasScopeRequirement("read:authors", $"https://{builder.Configuration["Security:Domain"]}")));

    o.AddPolicy("read:authors", p => p.
        RequireAuthenticatedUser().
        RequireClaim("permissions", "read:authors"));    

    o.AddPolicy("write:authors", p => p.
        RequireAuthenticatedUser().
        RequireClaim("permissions", "write:authors"));

    o.AddPolicy("read:books", p => p.
        RequireAuthenticatedUser().
        RequireClaim("permissions", "read:books"));

    o.AddPolicy("write:books", p => p.
        RequireAuthenticatedUser().
        RequireClaim("permissions", "write:books"));
});

//builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();


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

// Security - Both below are needed
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
