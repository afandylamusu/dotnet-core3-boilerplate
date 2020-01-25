using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moonlay.Confluent.Kafka;
using Moonlay.Core.Models;
using Moonlay.WebApp.Clients;
using System;
using System.IO.Compression;

namespace Moonlay.WebApp
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
            services.AddHttpContextAccessor();

            services.AddScoped<ISignInService, SignInService>();

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

            services.AddResponseCaching();
            services.AddRazorPages();

            // Response Compression
            services.AddResponseCompression(options => {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });
            
            services.Configure<GzipCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Fastest;
            });

            ConfigureKafka(services);

            services.AddMetrics();
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseResponseCompression();

            app.UseRouting();
            app.UseResponseCaching();

            // Caches cacheable responses for up to 10 seconds.
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
