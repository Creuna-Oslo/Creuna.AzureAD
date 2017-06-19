using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using EEN.Web.AzureAD.Configuration.Episerver.ContentModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Logging.Compatibility;
using EPiServer.Security;
using JetBrains.Annotations;

namespace EEN.Web.AzureAD.Configuration.Episerver
{
    public class AzureAdSecurityEpiserverProvider : IAzureAdSecuritySettingsProvider, ICustomVirtualRolesProvider
    {
        private int _settingsPageIdConfiguration = -1;

        private static ILog Log = LogManager.GetLogger(typeof (AzureAdSecurityEpiserverProvider));

        public AzureAdSecurityEpiserverProvider(IContentRepository contentRepository, 
            IAzureAdSecuritySettingsProvider providerFallback, 
            IContentEvents contentEvents, 
            IContentVersionRepository contentVersionRepository, ICustomVirtualRolesWatcher rolesWatcher)
        {
            ContentRepository = contentRepository;
            ProviderFallback = providerFallback;
            ContentEvents = contentEvents;
            ContentVersionRepository = contentVersionRepository;
            RolesWatcher = rolesWatcher;
        }

        protected IContentRepository ContentRepository { get; }
        protected IContentVersionRepository ContentVersionRepository { get; }
        protected IAzureAdSecuritySettingsProvider ProviderFallback { get; }
        protected ContentReference ConfigurationPageLink { get; private set; }
        protected IContentEvents ContentEvents { get; }
        protected ICustomVirtualRolesWatcher RolesWatcher { get; }

        protected virtual string ConfigurationKey => "AzureAD.SettingsPageId";

        protected virtual int SettingsPageIdConfiguration
        {
            get
            {
                if (_settingsPageIdConfiguration == -1)
                {
                    _settingsPageIdConfiguration = LoadSettingsPageId();
                }

                return _settingsPageIdConfiguration;
            }
        }

        protected virtual int LoadSettingsPageId()
        {
            var stringValue = ConfigurationManager.AppSettings[ConfigurationKey];
            int result;
            return int.TryParse(stringValue, out result) ? result : 0;
        }

        protected virtual LoaderOptions DefaultLoaderOptions => LanguageSelector.AutoDetect(enableMasterLanguageFallback: true);

        public virtual AzureAdSecuritySettings GetSettings()
        {
            if (ContentReference.IsNullOrEmpty(ConfigurationPageLink))
                throw new InvalidOperationException("Don't know where to get Azure AD security configuration");

            try
            {
                var settingsPage = ContentRepository.Get<AzureAdSecuritySettingsPage>(ConfigurationPageLink, DefaultLoaderOptions);
                var result = settingsPage.Settings ?? new AzureAdSecuritySettings();
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error reading  Azure AD security configuration. Check inner exception for details", ex);
            }
        }

        public virtual List<string> GetCustomRoles()
        {
            var settings = GetSettings();
            return settings.Roles;
        }

        protected virtual AzureAdSecuritySettingsPage TryGetSettingsPageForRoot(ContentReference root)
        {
            var result = ContentRepository.GetChildren<AzureAdSecuritySettingsPage>(root, DefaultLoaderOptions).FirstOrDefault();
            return result;
        }

        protected virtual ContentReference InitializeConfigurationPageLink()
        {
            if (SettingsPageIdConfiguration > 0)
            {
                return new ContentReference(SettingsPageIdConfiguration);
            }

            var settingsParent = DefaultSettingsParent;

            var defaultSettings = TryGetSettingsPageForRoot(settingsParent);
            if (defaultSettings != null)
            {
                return defaultSettings.ContentLink;
            }

            var newSettingsPage = ContentRepository.GetDefault<AzureAdSecuritySettingsPage>(settingsParent);
            newSettingsPage.Name = DefaultSettingsPageName;
            newSettingsPage.Settings = ProviderFallback.GetSettings();

            var result = ContentRepository.Save(newSettingsPage, SaveAction.Publish, AccessLevel.NoAccess);
            Inform($"Created Azure AD Settings page: #{result.ID}");
            return result;
        }

        public virtual string DefaultSettingsPageName => ConfigurationManager.AppSettings["AzureAD.DefaultSettingsPageName"].NullIfEmpty() ?? "Azure AD Security Settings";
        public virtual ContentReference DefaultSettingsParent { get; } = ContentReference.RootPage;


        protected virtual void Inform(string message)
        {
            if (Log.IsInfoEnabled)
                Log.Info(message);
            else
                Log.Error($"INFO {message}");
        }

        public virtual void Initialize()
        {
            ConfigurationPageLink = InitializeConfigurationPageLink();
            HandleConfigurationChanges(ConfigurationPageLink);
        }

        protected virtual void HandleConfigurationChanges(ContentReference configurationPageLink)
        {
            ContentEvents.PublishedContent += SettingsPublished;
            // ContentEvents.PublishedContent += SettingsPublishing;
        }

        //private void SettingsPublishing(object sender, ContentEventArgs e)
        //{
        //}

        protected virtual void SettingsPublished(object sender, ContentEventArgs e)
        {
            try
            {
                if (e.ContentLink.CompareToIgnoreWorkID(ConfigurationPageLink))
                {
                    var page = (AzureAdSecuritySettingsPage) e.Content;
                    var newRoles = page.Settings?.Roles ?? new List<string>();

                    var previousVersion = ContentVersionRepository.List(ConfigurationPageLink)
                        .Where(x => x.Status == VersionStatus.PreviouslyPublished)
                        .OrderByDescending(x => x.Saved)
                        .FirstOrDefault();

                    var previousPage = previousVersion != null ? ContentRepository.Get<AzureAdSecuritySettingsPage>(previousVersion.ContentLink, DefaultLoaderOptions) : null;

                    var diff = Diff(newRoles, previousPage?.Settings?.Roles ?? new List<string>());

                    if (!diff.IsEmpty)
                        RolesWatcher.RolesChanged(diff);
                }

            }
            catch (Exception ex)
            {
                Log.Error($"Error handling roles update (published page #{e.ContentLink.ID}): {ex}");
            }
        }

        public static List<string> SubstractSets([NotNull] List<string> left, [NotNull] List<string> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            var result = left.Where(x => !right.Contains(x, StringComparer.InvariantCultureIgnoreCase)).ToList();
            return result;
        }

        protected virtual RolesChange Diff(List<string> newRoles, List<string> oldRoles)
        {
            var result = new RolesChange
            {
                Added = SubstractSets(newRoles, oldRoles),
                Removed = SubstractSets(oldRoles, newRoles)
            };
            return result;
        }
    }

    internal static class StringExtensionss
    {
        public static string NullIfEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}