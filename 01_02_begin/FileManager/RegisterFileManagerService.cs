using Invoices.Data.Models.Config;
using Invoices.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Invoices.Data
{
    public static class Register
    {
        public static void RegisterInvoiceDataServices(this IServiceCollection services, StorageConfig storageConfig, DbConfig config)
        {
            services.AddSingleton<IInvoiceFileService>(p=> new InvoiceFileService(storageConfig));
            services.AddSingleton<IDataService>(p => new DataService(config));
        }
    }
}
