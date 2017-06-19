using EPiServer.Framework.DataAnnotations;
using EPiServer.PlugIn;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Creuna.AzureAD.Configuration.Episerver.ContentModels
{
    [EditorHint("JsonString")]
    [PropertyDefinitionTypePlugIn(DisplayName = "Azure AD Security Settings", 
        Description = "Azure AD Security Settings")]
    public class AzureAdSecuritySettingsProperty : JsonProperty<AzureAdSecuritySettings>
    {
    }

    [EditorDescriptorRegistration(TargetType = typeof(AzureAdSecuritySettings), 
        UIHint = "JsonString")]
    public class JsonStringGenericEditorDescriptor : EditorDescriptor
    {
        public string Style { get; set; }

        public JsonStringGenericEditorDescriptor()
        {
            // this.ClientEditingClass = "epi/shell/widget/ValidationTextarea";
            this.ClientEditingClass = "creunaAzureAD/editors/JsonString";
            this.Style = "width:582px;";
        }

        protected override void SetEditorConfiguration(ExtendedMetadata metadata)
        {
            this.EditorConfiguration["style"] = (object)this.Style;
            this.EditorConfiguration["helptext"] = "It's needed to logout and login again for users to see the changes. It might be needed even to restart the webapp. ";
            base.SetEditorConfiguration(metadata);
        }
    }
}