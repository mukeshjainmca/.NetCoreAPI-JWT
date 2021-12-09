using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITemplate.Configuration
{
    public static class Config
    {
        private static IConfiguration configuration;

        public static string GetConnectionString(string name)
        {
            var section = configuration.GetConnectionString(name);
            return section;
        }

        static Config()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true);
                configuration = builder.Build();
            }
            catch (Exception ex)
            {
                {
                    throw;
                }
            }
        }
    }
}
