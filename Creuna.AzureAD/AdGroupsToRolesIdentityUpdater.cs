using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EEN.Web.AzureAD
{
    public class AdGroupsToRolesIdentityUpdater : IIdentityUpdater
    {
        public AdGroupsToRolesIdentityUpdater(IAzureAdSecuritySettingsProvider azureAdSecuritySettingsProvider)
        {
            AzureAdSecuritySettingsProvider = azureAdSecuritySettingsProvider;
        }

        protected IAzureAdSecuritySettingsProvider AzureAdSecuritySettingsProvider { get; }

        public virtual void UpdateIdentity(ClaimsIdentity identity)
        {
            var groupsClaims =
                identity.Claims.Where(x => x.Type.Equals("groups", StringComparison.InvariantCultureIgnoreCase));

            var map = AzureAdSecuritySettingsProvider.GetSettings();

            var roles = new List<string>();

            foreach (var group in groupsClaims)
            {
                var mapped =
                    map.Groups.FirstOrDefault(
                        x => x.Uid.Equals(group.Value, StringComparison.InvariantCultureIgnoreCase));
                if (mapped != null)
                {
                    roles.AddRange(mapped.Roles.Where(role => !roles.Contains(role, StringComparer.InvariantCultureIgnoreCase)));
                }
            }

            var newRoles =
                roles.Where(role =>
                    !identity.Claims.Any(
                        c =>
                            c.Type.Equals(ClaimTypes.Role, StringComparison.InvariantCultureIgnoreCase) &&
                            c.Value.Equals(role, StringComparison.InvariantCultureIgnoreCase))).ToList();

            var newClaims = newRoles.Select(CreateClaimForRole);

            identity.AddClaims(newClaims);
        }

        protected virtual Claim CreateClaimForRole(string role)
        {
            var claim = new Claim(ClaimTypes.Role, role);
            return claim;
        }
    }
}