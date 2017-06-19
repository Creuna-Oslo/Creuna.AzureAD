namespace Creuna.AzureAD.Utils.FeatureToggles
{
    public class FeatureToggleForseJsonConfig : AppSettingsFeatureToggle
    {
        protected override string AppKeyName => "Creuna.AzureAD.ForceJsonConfig";
    }
}