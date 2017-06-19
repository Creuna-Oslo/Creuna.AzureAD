using EPiServer.Shell;

namespace EEN.Web.AzureAD.Configuration.Episerver.ContentModels
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