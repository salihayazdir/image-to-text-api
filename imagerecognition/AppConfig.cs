using System;
namespace imagerecognition
{
    public class AppConfig : IAppConfig
    {
        public readonly string _visioncred = string.Empty;

        public IConfiguration Configuration { get; }
        public AppConfig(IConfiguration configuration)
        {
            Configuration = configuration;
            _visioncred = Configuration["VISION_CREDENTIALS"];
        }

        public string GetVisionCredentials()
        {
            return _visioncred;
        }
    }
    public interface IAppConfig
    {
        string GetVisionCredentials();
    }
}

