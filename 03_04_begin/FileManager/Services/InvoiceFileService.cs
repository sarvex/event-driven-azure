using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Invoices.Data.Models;
using Invoices.Data.Models.Config;
using Invoices.Data.Models.Invoices;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace Invoices.Data.Services
{
    public interface IInvoiceFileService
    {
        Task ConvertFirstPageWordToImage(string id, MemoryStream file);
        Task ConvertInvoiceToAllFormats(string invoiceId);
        Task ConvertWordToHtml(string id, MemoryStream file);
        Task ConvertWordToPDF(string id, MemoryStream file);
        Task<string> CreateDownloadLink(string name);
        Task<MemoryStream> DownloadFile(string name);
        Task GenerateInvoice(InvoiceData invoice);
        IAsyncEnumerable<FileViewModel> GetFiles();
        Task<IEnumerable<FileViewModel>> GetFilesForInvoice(string id);
        Task UploadFile(Stream file, string name);
    }

    public class InvoiceFileService : IInvoiceFileService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private BlobContainerClient _blobContainer = null;
        public InvoiceFileService(StorageConfig storageConfig)
        {
            _connectionString = storageConfig.ConnectionString;
            _containerName = storageConfig.ContainerName;
        }
        public async Task<IEnumerable<FileViewModel>> GetFilesForInvoice(string id)
        {
            var container = await GetBlobContainer();
            var files = container.GetBlobs(prefix: id);
            return files.Select(f => new FileViewModel { Name = f.Name, Extension = Path.GetExtension(f.Name) }).ToList();

        }
        public async IAsyncEnumerable<FileViewModel> GetFiles()
        {
            var container = await GetBlobContainer();
            await foreach (BlobItem blobItem in container.GetBlobsAsync())
            {
                yield return new FileViewModel
                {
                    Name = blobItem.Name,

                };
            }
        }
        public async Task UploadFile(Stream file, string name)
        {
            var container = await GetBlobContainer();
            // Get a reference to a blob
            BlobClient blobClient = container.GetBlobClient(name);

            file.Position = 0;
            // Upload data from the local file
            await blobClient.UploadAsync(file, true);
        }
        public async Task<MemoryStream> DownloadFile(string name)
        {
            var container = await GetBlobContainer();
            BlobClient blobClient = container.GetBlobClient(name);
            var file = new MemoryStream();
            await blobClient.DownloadToAsync(file);
            return file;
        }
        public async Task<string> CreateDownloadLink(string name)
        {
            var container = await GetBlobContainer();
            BlobClient blobClient = container.GetBlobClient(name);
            var url = blobClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(20));
            return url.AbsoluteUri;
        }
        public async Task ConvertInvoiceToAllFormats(string invoiceId)
        {
            var container = await GetBlobContainer();
            BlobClient blobClient = container.GetBlobClient(invoiceId + ".docx");
            using (var file = new MemoryStream())
            {
                await blobClient.DownloadToAsync(file);
                
                await ConvertWordToPDF(invoiceId, file);
                await ConvertFirstPageWordToImage(invoiceId, file);
                await ConvertWordToHtml(invoiceId, file);


            }
        }


        public async Task GenerateInvoice(InvoiceData invoice)
        {
            using (var template = GetTemplateFile())
            using (Spire.Doc.Document document = new Spire.Doc.Document(template))
            {

                var regex = @"\{{(.*?)}}";

                var variableToReplace = document.FindPattern(new Regex(regex, RegexOptions.Multiline));
                //loop through all variables that have the format {{name}} and replace with data from the input
                while (variableToReplace != null)
                {
                    var propertyName = variableToReplace.SelectedText.ToString();
                    var prop = invoice.GetType().GetProperty(propertyName.Replace("{{", "").Replace("}}", ""));//remove brackets and get only the property name so we can get the value from the invoice object
                    var value = prop?.GetValue(invoice, null);

                    document.Replace(variableToReplace.SelectedText.ToString(), value?.ToString() ?? "", true, false);//adidtional processing might be needed to format the value properly. Ex:date formats

                    variableToReplace = document.FindPattern(new Regex(regex, RegexOptions.Multiline));

                }
                using (var docStream = new MemoryStream())
                {
                    //save document to stream
                    document.SaveToStream(docStream, Spire.Doc.FileFormat.Docx);
                    //upload stream to blob storage
                    await this.UploadFile(docStream, invoice.Id + ".docx");
                    document.Dispose();
                }
            }
        }
        public async Task ConvertWordToPDF(string id, MemoryStream file)
        {
            file.Position = 0;

            using (Spire.Doc.Document document = new Spire.Doc.Document(file))
            {
                using (var docStream = new MemoryStream())
                {
                    //save document to stream
                    document.SaveToStream(docStream, Spire.Doc.FileFormat.PDF);
                    //upload stream to blob storage
                    await this.UploadFile(docStream, id + ".pdf");
                    document.Dispose();
                }
            }
        }
        public async Task ConvertFirstPageWordToImage(string id, MemoryStream file)
        {

            file.Position = 0;
            using (Spire.Doc.Document document = new Spire.Doc.Document(file))
            {

                using (var docStream = new MemoryStream())
                {
                    document.SaveToImages(0, Spire.Doc.Documents.ImageType.Bitmap).Save(docStream, ImageFormat.Png);

                    //upload stream to blob storage
                    await this.UploadFile(docStream, id + ".png");
                }
            }
        }
        public async Task ConvertWordToHtml(string id, MemoryStream file)
        {
            file.Position = 0;

            using (Spire.Doc.Document document = new Spire.Doc.Document(file))
            {
                document.HtmlExportOptions.ImageEmbedded = true;
                using (var docStream = new MemoryStream())
                {
                    //save document to stream
                    document.SaveToStream(docStream, Spire.Doc.FileFormat.Html);
                    //upload stream to blob storage
                    await this.UploadFile(docStream, id + ".html");
                    document.Dispose();
                }
            }
        }

        #region private
        private Stream GetTemplateFile()
        {
            //get template from embeded resrouces for the demo. In real word application move the template to azure storage
            using Stream stream = this.GetType().Assembly.GetManifestResourceStream("Invoices.Data.Templates.invoice_template.docx");
            var output = new MemoryStream();
            stream.CopyTo(output);
            return output;
        }
        private async Task<BlobContainerClient> GetBlobContainer()
        {
            if (this._blobContainer != null) return this._blobContainer;
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            // Create the container and return a container client object
            this._blobContainer = blobServiceClient.GetBlobContainerClient(_containerName);
            //create container if it doesn't exist
            await _blobContainer.CreateIfNotExistsAsync();
            return _blobContainer;
        }
        #endregion
    }
}