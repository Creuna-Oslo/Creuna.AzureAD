using EEN.Web.AzureAD.Configuration;
using EEN.Web.AzureAD.Configuration.ConfigFile;
using EEN.Web.AzureAD.Configuration.Episerver;
using StructureMap.Configuration.DSL;

namespace EEN.Web.AzureAD
{
    public class AdGroupsToRolesMapperRegistryBase : Registry
    {
        public AdGroupsToRolesMapperRegistryBase()
        {
            For<IIdentityUpdater>().Singleton().Use<AdGroupsToRolesIdentityUpdater>();
            //For<List<IIdentityUpdater>>().Use(ctx => new List<IIdentityUpdater>
            //{
            //    ctx.GetInstance<AdGroupsToRolesIdentityUpdater>()
            //});
            //For<IdentityUpdater>().Singleton().Use<IdentityUpdater>();
            //For<IIdentityUpdater>().Singleton().Use<IdentityUpdater>();
            For<ICustomVirtualRolesWatcher>().Singleton().Use<RolesWatcher>();
        }
    }

    public class AdGroupsToRolesMapperRegistry : AdGroupsToRolesMapperRegistryBase
    {
        public AdGroupsToRolesMapperRegistry()
        {
            For<AzureAdSecurityConfigurationFileProvider>().Singleton();
            For<IAzureAdSecuritySettingsProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityConfigurationFileProvider>());
            For<ICustomVirtualRolesProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityConfigurationFileProvider>());
        }
    }

    public class EpiserverAdGroupsToRolesMapperRegistry : AdGroupsToRolesMapperRegistryBase
    {
        public EpiserverAdGroupsToRolesMapperRegistry()
        {
            For<AzureAdSecurityConfigurationFileProvider>().Singleton();
            For<AzureAdSecurityEpiserverProvider>().Singleton().Use<AzureAdSecurityEpiserverProvider>()
                .Ctor<IAzureAdSecuritySettingsProvider>().Is<AzureAdSecurityConfigurationFileProvider>();
            For<IAzureAdSecuritySettingsProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityEpiserverProvider>());
            For<ICustomVirtualRolesProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityEpiserverProvider>());
        }
    }
}