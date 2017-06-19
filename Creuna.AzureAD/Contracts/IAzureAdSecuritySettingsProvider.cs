using JetBrains.Annotations;

namespace Creuna.AzureAD.Contracts
{
    public interface IAzureAdSecuritySettingsProvider
    {
        [NotNull]
        AzureAdSecuritySettings GetSettings();
    }
}