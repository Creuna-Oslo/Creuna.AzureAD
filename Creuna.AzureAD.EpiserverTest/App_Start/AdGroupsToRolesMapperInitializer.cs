using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Creuna.AzureAD.Configuration;
using Creuna.AzureAD.Configuration.ConfigFile;
using Creuna.AzureAD.Configuration.Episerver;
using Creuna.AzureAD.Contracts;
using Creuna.AzureAD.Utils.FeatureToggles;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using StructureMap;

namespace Creuna.AzureAD.EpiserverTest
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class AdGroupsToRolesMapperInitializer : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            var container = context.StructureMap();

            container.Configure(x => x.AddRegistry<AdGroupsToRolesMapperRegistry>());
        }

        public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
        { }

        public void Preload(string[] parameters)
        { }

        public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context)
        { }
    }

    public class AdGroupsToRolesMapperRegistry : Registry
    {
        public AdGroupsToRolesMapperRegistry()
        {
            if (!new FeatureToggleDisableAzureAD().FeatureEnabled)
            {

                if (new FeatureToggleForseJsonConfig().FeatureEnabled)
                {
                    JsonConfigAdGroupsToRolesMapper();
                }
                else
                {
                    EpiserverAdGroupsToRolesMapper();
                }
            }
        }

        private void JsonConfigAdGroupsToRolesMapper()
        {
            CommonComponents();

            For<AzureAdSecurityConfigurationFileProvider>().Singleton();
            For<IAzureAdSecuritySettingsProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityConfigurationFileProvider>());
            For<ICustomVirtualRolesProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityConfigurationFileProvider>());
        }

        private void EpiserverAdGroupsToRolesMapper()
        {
            CommonComponents();

            For<AzureAdSecurityConfigurationFileProvider>().Singleton();
            For<AzureAdSecurityEpiserverProvider>().Singleton().Use<AzureAdSecurityEpiserverProvider>()
                .Ctor<IAzureAdSecuritySettingsProvider>().Is<AzureAdSecurityConfigurationFileProvider>();
            For<IAzureAdSecuritySettingsProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityEpiserverProvider>());
            For<ICustomVirtualRolesProvider>().Singleton().Use(ctx => ctx.GetInstance<AzureAdSecurityEpiserverProvider>());
        }

        private void CommonComponents()
        {
            For<IIdentityUpdater>().Singleton().Use<AdGroupsToRolesIdentityUpdater>();
            For<ICustomVirtualRolesWatcher>().Singleton().Use<RolesWatcher>();
        }
    }
}