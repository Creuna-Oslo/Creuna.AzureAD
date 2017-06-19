using System.Collections.Generic;

namespace Creuna.AzureAD
{
    public class RolesChange
    {
        public List<string> Added { get; set; } = new List<string>();
        public List<string> Removed { get; set; } = new List<string>();

        public bool IsEmpty => Added?.Count + Removed?.Count == 0;
    }
}