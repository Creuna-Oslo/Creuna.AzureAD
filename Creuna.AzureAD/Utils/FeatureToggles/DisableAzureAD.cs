namespace Creuna.AzureAD.Utils.FeatureToggles
{
    public class FeatureToggleDisableAzureAD : AppSettingsFeatureToggle
    {
        protected override string AppKeyName => "Creuna.AzureAD.Disabled";
    }
}