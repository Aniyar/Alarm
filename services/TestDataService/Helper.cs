using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TestDataService
{
    public static class Helper
    {

        public static string ConnectionString()
        {
            try
            {
                var configBuilder = new ConfigurationBuilder();
                var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "appsettings.json");
                configBuilder.AddJsonFile(path, false);
                var root = configBuilder.Build();
                var appSetting = root.GetSection("ConnectionStrings:DefaultConnection");
                return appSetting.Value;
            }
            catch
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
            }
        }

       
    }
}
