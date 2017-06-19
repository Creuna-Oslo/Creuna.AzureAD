using EPiServer.Shell;

namespace Creuna.AzureAD.Configuration.Episerver.ContentModels
{
    [UIDescriptorRegistration]
    public class AzureAdSecuritySettingsPageUiDescriptor : UIDescriptor<AzureAdSecuritySettingsPage>
    {
        public AzureAdSecuritySettingsPageUiDescriptor()
        {
            this.DisableOnPageEditView();
            this.DisablePreview();
        }
    }
}