using Invoices.Data.Models.Config;
using Invoices.Data.Models.Invoices;
using Microsoft.Azure.Cosmos;

namespace Invoices.Data.Services
{
    public interface IDataService
    {
        Task<InvoiceData> AddInvoice(InvoiceData invoice);
        Task<IEnumerable<InvoiceData>> GetAllInvoices();
        Task<InvoiceData> GetInvoiceDetails(string id);
    }

    public class DataService : IDataService
    {
        DbConfig _config;
        public DataService(DbConfig config)
        {
            _config = config;
        }

        public async Task<InvoiceData> AddInvoice(InvoiceData invoice)
        {
            var container = GetCosmosDbContainer();
            ItemResponse<InvoiceData> invoiceResponse = await container.CreateItemAsync<InvoiceData>(invoice, new PartitionKey(invoice.Id));
            return invoiceResponse.Resource;

        }
        public async Task<IEnumerable<InvoiceData>> GetAllInvoices()
        {
            var container = GetCosmosDbContainer();
            var sqlQueryText = "SELECT * FROM c";


            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            List<InvoiceData> invoices = new List<InvoiceData>();

            using (FeedIterator<InvoiceData> queryResultSetIterator = container.GetItemQueryIterator<InvoiceData>(queryDefinition)){


                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<InvoiceData> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (InvoiceData invoice in currentResultSet)
                    {
                        invoices.Add(invoice);
                       
                    }
                }
            }
            return invoices;


        }
        public async Task<InvoiceData> GetInvoiceDetails(string id)
        {
            var container = GetCosmosDbContainer();
            var sqlQueryText = "SELECT * FROM c";


            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            List<InvoiceData> invoices = new List<InvoiceData>();

            var item = await container.ReadItemAsync<InvoiceData>(id, new PartitionKey(id));

            return item.Resource;





        }
        #region private
        private Container GetCosmosDbContainer()
        {
            var cosmosClient = new CosmosClient(_config.ConnectionString);

            var database = cosmosClient.GetDatabase(_config.DatabaseId);
            return database.GetContainer(_config.ContainerId);
        }
        #endregion
    }
}
