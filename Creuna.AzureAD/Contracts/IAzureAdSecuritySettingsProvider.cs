using JetBrains.Annotations;

namespace EEN.Web.AzureAD
{
    public interface IAzureAdSecuritySettingsProvider
    {
        [NotNull]
        AzureAdSecuritySettings GetSettings();
    }
}