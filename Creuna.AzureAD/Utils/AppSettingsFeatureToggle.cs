using System;
using System.Configuration;
using FeatureToggle.Core;
using JetBrains.Annotations;

namespace Creuna.AzureAD.Utils
{
    public abstract class AppSettingsFeatureToggle : IFeatureToggle
    {
        [NotNull]
        protected abstract string AppKeyName { get; }
        public virtual bool Default => false;
        public virtual bool FeatureEnabled => IsFeatureEnabled(AppKeyName);
        
        public virtual bool IsFeatureEnabled([NotNull] string configKey)
        {
            if (configKey == null) throw new ArgumentNullException(nameof(configKey));

            var valueToParse = ConfigurationManager.AppSettings[configKey] ?? Default.ToString();

            try
            {
                return bool.Parse(valueToParse);
            }
            catch (Exception ex)
            {
                throw new ToggleConfigurationError(
                    $"The value '{(object) valueToParse}' cannot be converted to a boolean as defined in config key '{(object) configKey}'", ex);
            }
        }
    }
}