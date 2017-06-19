using System.Collections.Generic;
using JetBrains.Annotations;

namespace EEN.Web.AzureAD
{
    public interface ICustomVirtualRolesProvider
    {
        [NotNull]
        List<string> GetCustomRoles();

        void Initialize();
    }
}