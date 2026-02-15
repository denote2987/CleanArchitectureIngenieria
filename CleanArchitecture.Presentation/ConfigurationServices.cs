using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Presentation
{
    public static class ConfigurationServices
    {
        public static void AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Build connection string from environment variables or appsettings
            var connectionString = BuildConnectionString(configuration);
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        private static string BuildConnectionString(IConfiguration configuration)
        {
            // Priority order:
            // 1. Environment variable DB_CONNECTION_STRING
            // 2. Individual environment variables (DB_SERVER, DB_DATABASE, etc.)
            // 3. DatabaseSettings from appsettings
            
            var envConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                return envConnectionString;
            }

            // Build from individual environment variables or appsettings
            var server = Environment.GetEnvironmentVariable("DB_SERVER") 
                ?? configuration["DatabaseSettings:Server"] 
                ?? throw new InvalidOperationException("Database server not configured. Set DB_SERVER environment variable or DatabaseSettings:Server in appsettings.");
            
            var database = Environment.GetEnvironmentVariable("DB_DATABASE") 
                ?? configuration["DatabaseSettings:Database"] 
                ?? throw new InvalidOperationException("Database name not configured. Set DB_DATABASE environment variable or DatabaseSettings:Database in appsettings.");
            
            var useIntegratedSecurity = bool.TryParse(
                Environment.GetEnvironmentVariable("DB_USE_INTEGRATED_SECURITY") 
                ?? configuration["DatabaseSettings:UseIntegratedSecurity"],
                out var result) && result;
            
            var userId = Environment.GetEnvironmentVariable("DB_USER_ID") 
                ?? configuration["DatabaseSettings:UserId"];
            
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") 
                ?? configuration["DatabaseSettings:Password"];
            
            var connectTimeout = int.TryParse(
                Environment.GetEnvironmentVariable("DB_CONNECT_TIMEOUT") 
                ?? configuration["DatabaseSettings:ConnectTimeout"],
                out var timeout) ? timeout : 30;

            // Build connection string manually
            var connectionStringParts = new List<string>
            {
                $"Data Source={server}",
                $"Initial Catalog={database}",
                $"Connect Timeout={connectTimeout}",
                "Encrypt=true",
                "Trust Server Certificate=false"
            };

            if (useIntegratedSecurity)
            {
                connectionStringParts.Add("Integrated Security=True");
            }
            else
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException("Database credentials not configured. Set DB_USER_ID and DB_PASSWORD environment variables.");
                }
                connectionStringParts.Add($"User Id={userId}");
                connectionStringParts.Add($"Password={password}");
            }

            return string.Join(";", connectionStringParts);
        }
    }
}
