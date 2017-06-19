using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using Creuna.AzureAD.Contracts;
using Newtonsoft.Json;

namespace Creuna.AzureAD.Configuration.ConfigFile
{
    public class AzureAdSecurityConfigurationFileProvider : IAzureAdSecuritySettingsProvider, ICustomVirtualRolesProvider
    {
        private AzureAdSecuritySettings _settings;

        protected virtual string ConfigFilePath => HostingEnvironment.MapPath("~/configs/security.json");

        protected virtual AzureAdSecuritySettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = LoadSettings();
                }
                return _settings;
            }
        }

        protected virtual AzureAdSecuritySettings LoadSettings()
        {
            if (!File.Exists(ConfigFilePath))
                return new AzureAdSecuritySettings();
            var json = File.ReadAllText(ConfigFilePath);
            var settings = JsonConvert.DeserializeObject<AzureAdSecuritySettings>(json);
            return settings;
        }

        public virtual AzureAdSecuritySettings GetSettings()
        {
            return Settings;
        }

        public virtual List<string> GetCustomRoles()
        {
            return Settings.Roles;
        }

        public virtual void Initialize()
        {
        }
    }
}