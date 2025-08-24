using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Infoseed.MessagingPortal.Localization
{
    public static class MessagingPortalLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    MessagingPortalConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MessagingPortalLocalizationConfigurer).GetAssembly(),
                        "Infoseed.MessagingPortal.Localization.MessagingPortal"
                    )
                )
            );
        }
    }
}