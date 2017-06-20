using System.Threading;
using Creuna.AzureAD.Contracts;
using Creuna.AzureAD.Utils.FeatureToggles;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace Creuna.AzureAD
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CustomVirtualRolesInitializer : IInitializableModule
    {

        protected virtual T Locate<T>(InitializationEngine context)
        {
            return context.Locate.Advanced.GetInstance<T>();
        }

        public virtual void Initialize(InitializationEngine context)
        {
            if (!new FeatureToggleDisableAzureAD().FeatureEnabled)
            {
                var customVirtualRolesProvider = Locate<ICustomVirtualRolesProvider>(context);
                customVirtualRolesProvider.Initialize();
                var rolesWatcher = Locate<ICustomVirtualRolesWatcher>(context);
                rolesWatcher.Initialize(customVirtualRolesProvider.GetCustomRoles());
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}