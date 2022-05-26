using Microsoft.Web.Http.Routing;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;

namespace TemplateModularApp.Bootstrapper
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuration et services de l'API Web


            var constraintResolver = new DefaultInlineConstraintResolver() { ConstraintMap = { ["apiVersion"] = typeof(ApiVersionRouteConstraint) } };

            // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
            config.AddApiVersioning(options => options.ReportApiVersions = true);
            config.MapHttpAttributeRoutes(constraintResolver);

            // add the versioned IApiExplorer and capture the strongly-typed implementation (e.g. VersionedApiExplorer vs IApiExplorer)
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            var apiExplorer = config.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            config.EnableSwagger(
              "{apiVersion}/swagger",
              swagger =>
              {
                    // build a swagger document and endpoint for each discovered API version
                    swagger.MultipleApiVersions(
                      (apiDescription, version) => apiDescription.GetGroupName() == version,
                      info =>
                      {
                          foreach (var group in apiExplorer.ApiDescriptions)
                          {
                              var description = "A sample application with Swagger, Swashbuckle, and API versioning.";

                              if (group.IsDeprecated)
                              {
                                  description += " This API version has been deprecated.";
                              }

                              info.Version(group.Name, $"Sample API {group.ApiVersion}")
                                  .Contact(c => c.Name("Bill Mei").Email("bill.mei@somewhere.com"))
                                  .Description(description)
                                  .License(l => l.Name("MIT").Url("https://opensource.org/licenses/MIT"))
                                  .TermsOfService("Shareware");
                          }
                      });

                    // add a custom operation filter which sets default values
                    swagger.OperationFilter<SwaggerDefaultValues>();

                    // integrate xml comments
                    swagger.IncludeXmlComments(XmlCommentsFilePath);
              })
              .EnableSwaggerUi(swagger => swagger.EnableDiscoveryUrlSelector());
        }

        /// <summary>
        /// Get the root content path.
        /// </summary>
        /// <value>The root content path of the application.</value>
        public static string ContentRootPath
        {
            get
            {
                var app = AppDomain.CurrentDomain;

                if (string.IsNullOrEmpty(app.RelativeSearchPath))
                {
                    return app.BaseDirectory;
                }

                return app.RelativeSearchPath;
            }
        }

        static string XmlCommentsFilePath
        {
            get
            {
                var fileName = typeof(WebApiConfig).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(ContentRootPath, fileName);
            }
        }
    }

    /// <summary>
    /// Represents the Swagger/Swashbuckle operation filter used to provide default values.
    /// </summary>
    /// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
    /// Once they are fixed and published, this class can be removed.</remarks>
    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="schemaRegistry">The API schema registry.</param>
        /// <param name="apiDescription">The API description being filtered.</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            operation.deprecated |= apiDescription.IsDeprecated();

            if (operation.parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.name);

                // REF: https://github.com/domaindrivendev/Swashbuckle/issues/1101
                if (parameter.description == null)
                {
                    parameter.description = description.Documentation;
                }

                // REF: https://github.com/domaindrivendev/Swashbuckle/issues/1089
                // REF: https://github.com/domaindrivendev/Swashbuckle/pull/1090
                if (parameter.@default == null)
                {
                    parameter.@default = description.ParameterDescriptor?.DefaultValue;
                }
            }
        }
    }
}
