Quickstart
==========

1. Make sure your OWIN startup class is partial and calls ConfigureAuth(app) (from Startup.Auth.cs file) on startup
2. Register an app in Azure Active Directory => App Registrations and put proper settings to the web config
	- Creuna.AzureAD.ClientId
	- Creuna.AzureAD.Tenant
	- Creuna.AzureAD.AADInstance
	- Creuna.AzureAD.PostLogoutRedirectUri


NOTE: the following 2 steps are not needed if you have an Azure Premium subscription and manage roles in the Azure Portal
3. update your app manifest to have 
	...
	"groupMembershipClaims": "SecurityGroup",
	...
4. Make sure ~/configs/security.json has mapping that suits your needs

5. Add the following to your IoC configuration:
	if you use StructureMap Registry:	
	- this.IncludeRegistry<AdGroupsToRolesMapperRegistry>();

	otherwise it can be done as an initialization module:

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