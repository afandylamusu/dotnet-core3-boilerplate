using Confluent.Kafka;
using Confluent.SchemaRegistry;
using GraphQL;
using GraphQL.Server;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Moonlay.Confluent.Kafka;
using Moonlay.Core.Models;
using Moonlay.MasterData.OpenApi.Clients;
using Moonlay.MasterData.OpenApi.GraphQLTypes;

namespace Moonlay.MasterData.OpenApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureRestFullServices(services);

            ConfigureKafka(services);

            services.AddScoped<ISignInService, SignInService>();

            services.AddHttpClient();

            services.AddScoped<IManageDataSetClient>(c => {
                if (Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    var httpClientHandler = new System.Net.Http.HttpClientHandler();

                    // Return `true` to allow certificates that are untrusted/invalid
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        System.Net.Http.HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                    return new ManageDataSetClient(Grpc.Net.Client.GrpcChannel.ForAddress(Configuration.GetSection("Grpc:ServerUrl").Value, new Grpc.Net.Client.GrpcChannelOptions { HttpClient = new System.Net.Http.HttpClient(httpClientHandler) }));
                }
                else
                    return new ManageDataSetClient(Grpc.Net.Client.GrpcChannel.ForAddress(Configuration.GetSection("Grpc:ServerUrl").Value));
            });

            services.AddMetrics();

            services.AddHttpContextAccessor();
        }

        private void ConfigureKafka(IServiceCollection services)
        {
            services.AddSingleton(c => new SchemaRegistryConfig
            {
                Url = Configuration.GetSection("Kafka:SchemaRegistryUrl").Value,
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                // optional schema registry client properties:
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            });

            services.AddSingleton<ISchemaRegistryClient>(c => new CachedSchemaRegistryClient(c.GetRequiredService<SchemaRegistryConfig>()));
            services.AddSingleton(c => new ProducerConfig() { BootstrapServers = Configuration.GetSection("Kafka:BootstrapServers").Value });

            services.AddScoped<IKafkaProducer, KafkaProducer>();
        }

        private void ConfigureRestFullServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API",
                    Description = "Moonlay Web API BoilerPlate",
                    Contact = new OpenApiContact()
                    {
                        Name = "Afandy Lamusu",
                        Email = "afandy.lamusu@moonlay.com",
                        // Url = new Uri("www.dotnetdetail.net", UriKind.RelativeOrAbsolute)
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Moonlay",
                        // Url = new Uri("www.dotnetdetail.net", UriKind.RelativeOrAbsolute)
                    },
                });
            });
        }

        private void ConfigureGraphQL(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            // services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            // services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddSingleton<CustomerType>();
            // services.AddSingleton<StarWarsMutation>();
            // services.AddSingleton<HumanType>();
            // services.AddSingleton<HumanInputType>();
            // services.AddSingleton<DroidType>();
            // services.AddSingleton<CharacterInterface>();
            // services.AddSingleton<EpisodeEnum>();

            services.AddSingleton<DQuery>();
            services.AddSingleton<DMutation>();
            services.AddSingleton<ISchema, DSchema>();

            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
            })
            .AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseRouting();
            // app.UseRequestLocalization();
            // app.UseCors();

            app.UseAuthentication();
            // app.UseSession();

            app.UseSwagger(c=> {
                c.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api-docs/v1/swagger.json", "Test API V1");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
