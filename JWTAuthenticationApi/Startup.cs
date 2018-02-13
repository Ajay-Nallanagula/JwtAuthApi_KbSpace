using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthenticationApi
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
            var jwtBearerAuthencticationScheme = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme); //SC : AuthenticationServiceCollectionExtensions.cs   Output is of type AuthenticationBuilder
            jwtBearerAuthencticationScheme.AddJwtBearer(JwtBearerAuthencticationOptions); //SC: JwtBearerExtensions.cs,JwtBearerHandler.cs -->HandleAuthenticateAsync()

            services.AddMvc();
        }

        // In particular, we specify which parameters must be taken into account in order to consider valid a JSON Web Token
        private void JwtBearerAuthencticationOptions(JwtBearerOptions jwtBearerOptions)
        {
            //Validity of a recieved JWT token iks tested based on the below mentioned properties, JWT has to fullfill the following attributes to qualify
            jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true, //validate the server that created that token
                ValidateAudience = true, //ensure that the recipient of the token is authorized to receive it
                ValidateLifetime = true, //check that the token is not expired and that the signing key of the issuer is valid 
                ValidateIssuerSigningKey = true, //verify that the key used to sign the incoming token is part of a list of trusted keys 
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
            };

            //jwtBearerOptions.Events = new JwtBearerEvents()
            //{
            //    OnAuthenticationFailed = (context) =>
            //    {
            //        System.Diagnostics.Debug.WriteLine($"OnAuthenticationFailed {context.Exception.Message}");
            //        return Task.FromException(context.Exception);
            //    },
            //    OnMessageReceived = (context) =>
            //    {
            //        System.Diagnostics.Debug.WriteLine($"OnMessageReceived {context.Response}");
            //        System.Diagnostics.Debug.WriteLine($"OnMessageReceived {context.Token}");
            //        return Task.CompletedTask;
            //    },
            //    OnTokenValidated = (context) =>
            //    {
            //        System.Diagnostics.Debug.WriteLine($"OnMessageReceived {context.Request.Headers}");
            //        return Task.CompletedTask;
            //    }
            //};
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
