using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infoseed.MessagingPortal.Web.Models.Ctown
{
    public class StorageService
    {
        private readonly BlobServiceClient _client;
        private readonly BlobContainerClient _blobClient;
        private readonly string containerName = "ctownjo";
        public StorageService()
        {
            var connectionString =AppsettingsModel.StorageServiceConnectionStrings;
            _client = new BlobServiceClient(connectionString);
            _blobClient = new BlobContainerClient(connectionString, "ctownjo");
        }

        public async Task UploadFile( string filePath, string containerName = "ctownjo")
        {
            string fileName = Path.GetFileName(filePath);
            BlobContainerClient container = _client.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(fileName);

            using FileStream fileStream = File.OpenRead(filePath);
            await blob.UploadAsync(fileStream, true);
            fileStream.Close();
            //await blob.SetTagsAsync(tags);
        }

        public async Task UploadFile(byte[] byteArray, string fileName)
        {
            //string fileName = Path.GetFileName(filePath);
            BlobContainerClient container = _client.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(fileName);

            using (var stream = new MemoryStream(byteArray))
            {
                await blob.UploadAsync(stream, true);
            }

            //using FileStream fileStream = File.OpenRead(filebyt);
            //await blob.UploadAsync(fileStream, true);
            //fileStream.Close();
            //await blob.SetTagsAsync(tags);
        }

        public async Task<List<TaggedBlobItem>> FindCustomerFiles(string customerName, string containerName = "")
        {
            var foundItems = new List<TaggedBlobItem>();
            string searchExpression = $"\"customer\"='{customerName}'";
            if (!string.IsNullOrEmpty(containerName))
                searchExpression = $"@container = '{containerName}' AND \"customer\" = '{customerName}'";

            await foreach (var page in _client.FindBlobsByTagsAsync(searchExpression).AsPages())
            {
                foundItems.AddRange(page.Values);
            }
            return foundItems;
        }

        public async Task<string> FindFileByName(string fileName, string containerName = "ctownjo")
        {
            string _baseUri = AppsettingsModel.StorageServiceURL+ "ctownjo/";
            var blobs = new List<BlobItem>();
            await foreach (var page in _blobClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, fileName+".jpg").AsPages())
            {
                blobs.AddRange(page.Values);
            }
            if (blobs.Count > 0)
            {
                var returnVal = _baseUri + blobs[0].Name;
                return returnVal;
            }
            return string.Empty;
        }
    }
}
