using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PatientCareManagement.API.JsonConverters;
using PatientCareManagement.Core.Interfaces;
using PatientCareManagement.Core.Services;
using PatientCareManagement.Data.Repositories;
using PatientCareManagement.Infrastructure.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Register custom JSON converter for the AttachmentType enum
        options.JsonSerializerOptions.Converters.Add(new AttachmentTypeJsonConverter());
        // Include enum values as strings in Swagger docs
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Patient Management API", Version = "v1" });
    
    // Configure Swagger to use enum names instead of integer values
    c.SchemaFilter<EnumSchemaFilter>();
});

// Register repositories (using in-memory implementation)
builder.Services.AddSingleton<IPatientRepository, PatientRepository>();
builder.Services.AddSingleton<IMedicalHistoryRepository, MedicalHistoryRepository>();
builder.Services.AddSingleton<IClinicalAttachmentRepository, ClinicalAttachmentRepository>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

// Register services
builder.Services.AddScoped<PatientService>();

// Set reasonable file upload size limit (50MB)
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800; // 50MB in bytes
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient Management API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Swagger EnumSchemaFilter to show enum values as strings in Swagger docs
public class EnumSchemaFilter : Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter
{
    public void Apply(OpenApiSchema schema, Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (var name in Enum.GetNames(context.Type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}