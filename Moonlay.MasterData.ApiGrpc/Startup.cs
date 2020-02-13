using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moonlay.Confluent.Kafka;
using Moonlay.Core.Models;
using Moonlay.MasterData.ApiGrpc.Db;
using Moonlay.MasterData.ApiGrpc.Services;
using Moonlay.MasterData.Domain.Customers;
using Moonlay.MasterData.Domain.Customers.Consumers;
using Moonlay.MasterData.Domain.DataSets;
using System.IO.Compression;

namespace Moonlay.MasterData.ApiGrpc
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyDbContext>(options => options.UseNpgsql(Configuration.GetSection("ConnectionStrings:Connection").Value));
            services.AddDbContext<MyDbTrailContext>(options => options.UseNpgsql(Configuration.GetSection("ConnectionStrings:ConnectionTrail").Value));

            services.AddScoped<IDbxConnection>(c => new MyConnection(new Npgsql.NpgsqlConnection(Configuration.GetSection("ConnectionStrings:Connection").Value)));

            services.AddScoped<IDbContext, MyDbContext>();
            services.AddScoped<IDbTrailContext, MyDbTrailContext>();

            services.AddScoped<IDataSetRepository, DataSetRepository>();
            services.AddScoped<IDataSetUseCase, DataSetUseCase>();


            services.AddScoped<ISignInService, SignInService>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerUseCase, CustomerUseCase>();

            ConfigureKafka(services);

            services.AddGrpc();
            services.AddHttpContextAccessor();
            services.AddMetrics();

            // Response Compression
            services.AddResponseCompression(options => {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Fastest;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ManageDataSetRpc>();
                endpoints.MapGrpcService<ManageOrganizationRpc>();
                endpoints.MapGrpcService<ManageCustomerRpc>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

        private void ConfigureKafka(IServiceCollection services)
        {
            //services.AddHostedService<MasterDataHostConsumers>();

            //services.AddSingleton(c => new SchemaRegistryConfig
            //{
            //    Url = Configuration.GetSection("Kafka:SchemaRegistryUrl").Value,
            //    // Note: you can specify more than one schema registry url using the
            //    // schema.registry.url property for redundancy (comma separated list). 
            //    // The property name is not plural to follow the convention set by
            //    // the Java implementation.
            //    // optional schema registry client properties:
            //    RequestTimeoutMs = 5000,
            //    MaxCachedSchemas = 10
            //});

            //services.AddSingleton<ISchemaRegistryClient>(c => new CachedSchemaRegistryClient(c.GetRequiredService<SchemaRegistryConfig>()));

            //services.AddSingleton(c => new ConsumerConfig
            //{
            //    GroupId = "mdm-consumer-group",
            //    BootstrapServers = Configuration.GetSection("Kafka:BootstrapServers").Value,
            //    // Note: The AutoOffsetReset property determines the start offset in the event
            //    // there are not yet any committed offsets for the consumer group for the
            //    // topic/partitions of interest. By default, offsets are committed
            //    // automatically, so in this example, consumption will only start from the
            //    // earliest message in the topic 'my-topic' the first time you run the program.
            //    AutoOffsetReset = AutoOffsetReset.Earliest
            //});

            services.AddScoped<IConsumerRegister, ConsumerRegister>();
            services.AddScoped<INewCustomerConsumer, NewCustomerConsumer>();
            services.AddScoped<IUpdateCustomerConsumer, UpdateCustomerConsumer>();
            services.AddKafkaConsumer();
        }

    }
}
