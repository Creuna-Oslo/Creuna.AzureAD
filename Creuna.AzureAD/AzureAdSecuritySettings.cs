using System.Collections.Generic;
using Newtonsoft.Json;

namespace EEN.Web.AzureAD
{
    public class AzureAdSecuritySettings
    {
        public List<AdGroup> Groups { get; set; } = new List<AdGroup>();

        public List<string> Roles { get; set; } = new List<string>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}