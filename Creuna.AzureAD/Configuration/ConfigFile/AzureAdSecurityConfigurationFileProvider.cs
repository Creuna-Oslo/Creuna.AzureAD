using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using Creuna.AzureAD.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Creuna.AzureAD.Configuration.ConfigFile
{
    public class AzureAdSecurityConfigurationFileProvider : IAzureAdSecuritySettingsProvider, ICustomVirtualRolesProvider
    {
        private AzureAdSecuritySettings _settings;

        protected virtual string ConfigFilePath => MakePath(ConfigurationManager.AppSettings["Creuna.AzureAD.JsonConfig"] ?? "~/configs/security.json");

        protected virtual string MakePath([NotNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (path.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase) ||
                path.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
            {
                return HostingEnvironment.MapPath(path);
            }

            return path;
        }

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