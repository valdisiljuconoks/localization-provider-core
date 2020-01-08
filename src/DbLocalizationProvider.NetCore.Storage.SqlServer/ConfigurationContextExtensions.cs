using DbLocalizationProvider.AspNetCore.Commands;
using DbLocalizationProvider.AspNetCore.Queries;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.NetCore.Storage.SqlServer
{
    public static class ConfigurationContextExtensions
    {
        public static ConfigurationContext UseSqlServer(this ConfigurationContext context, string connectionString)
        {
            Settings.DbContextConnectionString = connectionString;

            // must have handlers
            ConfigurationContext.Current.TypeFactory.ForQuery<SyncResources.Query>().SetHandler<ResourceSynchronizer>();

            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetResource.Query>().SetHandler<GetResourceHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslationHandler>();

            //ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResources.Command>().SetHandler<CreateNewResourcesHandler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<DeleteAllResources.Command>().SetHandler<DeleteAllResourcesHandler>();
            //ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResourceHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<RemoveTranslation.Command>().SetHandler<RemoveTranslationHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslationHandler>();

            return context;
        }
    }
}
