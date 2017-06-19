using EPiServer.Shell;

namespace EEN.Web.AzureAD.Configuration.Episerver.ContentModels
{
    internal static class UIDescriptorExtensions
    {
        public static void DisableOnPageEditView(this UIDescriptor descriptor)
        {
            descriptor.DefaultView = CmsViewNames.AllPropertiesView;
            descriptor.AddDisabledView(CmsViewNames.OnPageEditView);
        }

        public static void DisablePreview(this UIDescriptor descriptor)
        {
            descriptor.AddDisabledView(CmsViewNames.PreviewView);
        }
    }
}