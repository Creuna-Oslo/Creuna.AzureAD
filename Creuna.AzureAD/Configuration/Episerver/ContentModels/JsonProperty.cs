using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Creuna.AzureAD.Configuration.Episerver.ContentModels
{
    public class JsonProperty<TValue> : TypedStringBasedProperty<TValue> 
        where TValue : class
    {
        protected override string SerializeObject([CanBeNull] TValue value)
        {
            if (value == null)
                return null;
            return JsonConvert.SerializeObject(value);
        }

        protected override TValue DeserializeObject([CanBeNull] string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            return JsonConvert.DeserializeObject<TValue>(json);
        }
    }
}