using EPiServer.Core;

namespace Creuna.AzureAD.EpiserverTest.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
