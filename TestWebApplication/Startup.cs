using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TestWebApplication.Conventions;
using TestWebApplication.Extensions;
using WeihanLi.AspNetCore.Authentication;
using WeihanLi.AspNetCore.Authentication.HeaderAuthentication;
using WeihanLi.AspNetCore.Authentication.QueryAuthentication;
using WeihanLi.Extensions;

namespace TestWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(HeaderAuthenticationDefaults.AuthenticationSchema)
                .AddHeader(HeaderAuthenticationDefaults.AuthenticationSchema, options => { options.AdditionalHeaderToClaims.Add("UserEmail", ClaimTypes.Email); })
                .AddQuery(QueryAuthenticationDefaults.AuthenticationSchema, options => { options.AdditionalQueryToClaims.Add("UserEmail", ClaimTypes.Email); })
                ;

            services.AddMvc(options =>
                {
                    options.Conventions.Add(new ApiControllerVersionConvention());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = ApiVersion.Default;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthCheck("/health", serviceProvider =>
                {
                    // 检查数据库访问是否正常
                    var configuration = serviceProvider.GetService<IConfiguration>();
                    var connString = configuration.GetConnectionString("Configurations");
                    using (var conn = new SqlConnection(connString))
                    {
                        conn.EnsureOpen();
                    }
                    return true;
                });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
