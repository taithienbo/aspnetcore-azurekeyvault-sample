using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AccessKeyVaultUsingCertificateAspNetCore
{
    public class Startup
    {
        private ILogger _logger; 

        public Startup(ILogger<Startup> logger, IHostingEnvironment env)
        {
            _logger = logger;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                builder.SetupKeyVault(_logger);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // expose the Configuraiton instance for using in other services. 
            services.Add(new ServiceDescriptor(typeof(IConfiguration),
                Configuration));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }

    public static class ConfigurationBuilderExtensions  
    {
        public static IConfigurationBuilder SetupKeyVault(this IConfigurationBuilder builder, 
            ILogger logger)
        {
            var configuration = builder.Build();
            var keyVaultURL = configuration["KeyVault:URL"];
            var appId = configuration["AzureAD:AppId"];
            // key vault access using certificate (private/public key) 
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates;
                logger.LogDebug("Num of certificates in store: " + certs.Count);
                var distinguishedName = new X500DistinguishedName(configuration["KeyVault:SubjectDistinguishedName"]);
                var certFound = certs.Find(X509FindType.FindBySubjectDistinguishedName, 
                    distinguishedName.Name, false).OfType<X509Certificate2>();
                if (!certFound.Any())
                {
                    logger.LogWarning("Unable to find the certificate to authenticate and access key vault");
                }
                else
                {
                    // found the certificate 
                    builder.AddAzureKeyVault(keyVaultURL, appId, certFound.Single());
                    store.Close();
                }
            }

            return builder; 
        }
    }
}
