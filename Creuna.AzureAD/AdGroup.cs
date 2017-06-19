using System.Collections.Generic;

namespace EEN.Web.AzureAD
{
    public class AdGroup
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}