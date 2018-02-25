using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Leave.Models;
using Leave.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.CookiePolicy;

namespace Leave
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
            services.AddMvc(opts =>
            {
                opts.Filters.Add(typeof(AdalTokenAcquisitionExceptionFilter));
            });

            //services.AddDataProtection();

            services.Configure<AuthOptions>(Configuration.GetSection("AzureAd"));
            services.AddSingleton<ITokenCacheFactory, TokenCacheFactory>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSession();

            services.AddAuthentication(auth =>
            {
                auth.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                    .AddCookie()
                    .AddOpenIdConnect(opts =>
                    {
                        Configuration.GetSection("AzureAd").Bind(opts);

                        opts.Events = new OpenIdConnectEvents
                        {
                            OnAuthorizationCodeReceived = async ctx =>
                            {
                                HttpRequest request = ctx.HttpContext.Request;
                                string currentUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path);
                                var credential = new ClientCredential(ctx.Options.ClientId, ctx.Options.ClientSecret);

                                ITokenCacheFactory cacheFactory = ctx.HttpContext.RequestServices.GetRequiredService<ITokenCacheFactory>();
                                TokenCache cache = cacheFactory.CreateForUser(ctx.Principal);

                                var authContext = new AuthenticationContext(ctx.Options.Authority, cache);

                                string resource = "https://graph.microsoft.com";
                                AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
                                    ctx.ProtocolMessage.Code, new Uri(currentUri), credential, resource);

                                ctx.HandleCodeRedemption(result.AccessToken, result.IdToken);
                            }
                        };
                    });

            //var connection = Configuration["Production:SqliteConnectionString"];

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                /*app.Use(async (ctx, next) =>
                {
                    if (!ctx.Request.IsHttps)
                    {
                        //Insecure request, redirect to HTTPS side
                        /*
                        HttpRequest req = ctx.Request;
                        string url = "https://" + req.Host + req.Path + req.QueryString;
                        ctx.Response.Redirect(url, permanent: true);
                        */                
                   /* }
                    else
                    {
                        //Apply Strict Transport Security to all secure requests
                        //All requests done over secure channel for next year
                        ctx.Response.Headers["Strict-Transport-Security"] = "max-age=31536000";

                        await next();
                    }
                });*/

            }


            app.UseStaticFiles();

            app.UseSession();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
