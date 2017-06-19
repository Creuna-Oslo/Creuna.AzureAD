using Creuna.AzureAD.Configuration;
using Creuna.AzureAD.Configuration.ConfigFile;
using Creuna.AzureAD.Configuration.Episerver;
using Creuna.AzureAD.Contracts;
using Creuna.AzureAD.Utils.FeatureToggles;
using StructureMap.Configuration.DSL;

namespace Creuna.AzureAD
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
            if (!new FeatureToggleDisableAzureAD().FeatureEnabled)
            {

                if (new FeatureToggleForseJsonConfig().FeatureEnabled)
                {
                    IncludeRegistry<JsonConfigAdGroupsToRolesMapperRegistry>();
                }
                else
                {
                    IncludeRegistry<EpiserverAdGroupsToRolesMapperRegistry>();
                }
            }
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

    public class JsonConfigAdGroupsToRolesMapperRegistry : AdGroupsToRolesMapperRegistryBase
    {
        public JsonConfigAdGroupsToRolesMapperRegistry()
        {
            For<AzureAdSecurityConfigurationFileProvider>().Singleton();
            For<IAzureAdSecuritySettingsProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityConfigurationFileProvider>());
            For<ICustomVirtualRolesProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityConfigurationFileProvider>());
        }
    }
}