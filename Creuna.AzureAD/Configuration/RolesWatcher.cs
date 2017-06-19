using System.Collections.Generic;
using System.Linq;
using Creuna.AzureAD.Contracts;
using EPiServer.Security;

namespace Creuna.AzureAD.Configuration
{
    public class RolesWatcher : ICustomVirtualRolesWatcher
    {
        public RolesWatcher(IVirtualRoleRepository virtualRoleRepository)
        {
            VirtualRoleRepository = virtualRoleRepository;
        }

        protected IVirtualRoleRepository VirtualRoleRepository { get; }

        public virtual void Initialize(List<string> roles)
        {
            RolesChanged(new RolesChange
            {
                Added = roles.ToList()
            });
        }

        public virtual void RolesChanged(RolesChange change)
        {
            var repository = VirtualRoleRepository;

            foreach (var role in change.Removed)
            {
                repository.Unregister(role, true);
            }

            foreach (var role in change.Added)
            {
                repository.Register(role, new MappedRole(repository));
            }
        }
    }
}