using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Framework;
using EPiServer.ServiceLocation;

namespace Creuna.AzureAD.EpiserverTest
{
    //[InitializableModule]
    //[ModuleDependency(typeof(ServiceContainerInitialization))]
    //public class AdGroupsToRolesMapperInitializer : IConfigurableModule
    //{
    //    public void ConfigureContainer(ServiceConfigurationContext context)
    //    {
    //        var container = context.StructureMap();

    //        container.Configure(x => x.AddRegistry<AdGroupsToRolesMapperRegistry>());
    //    }

    //    public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
    //    { }

    //    public void Preload(string[] parameters)
    //    { }

    //    public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context)
    //    { }
    //}
}