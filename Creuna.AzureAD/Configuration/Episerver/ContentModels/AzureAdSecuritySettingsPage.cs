using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace Creuna.AzureAD.Configuration.Episerver.ContentModels
{
    [ContentType(DisplayName = "Azure AD Security settings",
        Description = "Azure AD Security settings",
        GUID = "{C7D312B0-00A5-4BCF-B074-B1E61B1904BE}",
        GroupName = "System",
        Order = 10000,
        AvailableInEditMode = true)]
    public class AzureAdSecuritySettingsPage : PageData
    {
        [Display(Order = 10, Name = "Settings")]
        [CultureSpecific(false)]
        [BackingType(typeof(AzureAdSecuritySettingsProperty))]
        public virtual AzureAdSecuritySettings Settings { get; set; }
    }
}