using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal
{
    public class AzureBlobProvider
    {

        public async Task<string> Save(AttachmentContent attachment)
        {
            try
            {


                // Save attachment record in the storage account,
                string returnedURI = string.Empty;
                CloudStorageAccount cloudStorageAccount = GetStorageAccountByURI();
                // Create the blob client.
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = cloudBlobClient.GetContainerReference(attachment.SubscriptionID.ToString());
             //   await container.CreateIfNotExistsAsync();

                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob

                });
                if (string.IsNullOrEmpty(attachment.fileName))
                {

                    attachment.fileName=Guid.NewGuid()+ attachment.Extension;

                }
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(attachment.fileName + attachment.Extension);
                cloudBlockBlob.Properties.ContentType = attachment.MimeType;
                using (MemoryStream ms = new MemoryStream(attachment.Content))
                {
                    await cloudBlockBlob.UploadFromStreamAsync(ms);
                    returnedURI = cloudBlockBlob.Uri.AbsoluteUri.ToString();
                }
               
                // Save attachment record in the database

                return returnedURI;
            }
            catch (Exception )
            {
                throw;
            }
        }


        public async Task<List<string>> SaveListAttachment(List<AttachmentContent> attachment)
        {
            try
            {
                List<string> returnedURI = new List<string>();

                foreach (var item in attachment)
                {
                    // Save attachment record in the storage account,

                    CloudStorageAccount cloudStorageAccount = GetStorageAccountByURI();
                    // Create the blob client.
                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    // Retrieve a reference to a container.
                    CloudBlobContainer container = cloudBlobClient.GetContainerReference(item.SubscriptionID.ToString());
                    await container.CreateIfNotExistsAsync();

                    await container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob

                    });
                    CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(Guid.NewGuid() + item.Extension);
                    cloudBlockBlob.Properties.ContentType = item.MimeType;
                    using (MemoryStream ms = new MemoryStream(item.Content))
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(ms);
                        returnedURI.Add(cloudBlockBlob.Uri.AbsoluteUri.ToString());
                    }


                }



                // Save attachment record in the database

                return returnedURI;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static CloudStorageAccount GetStorageAccountByURI()
        {
            var key = AppSettingsModel.BlobServiceConnectionStrings;
            //
            //zGavJ4898+nEMz+NafvWYGmmCQtaDyUmI62IrR3LvKg7QYaOyJJ6shZdsAPb3a8JoMgIvUQaFYFbjDxX7m2rmA==
            //ConfigurationManager.EnvironmentConfigurations.StorageSettings.SubscriptionStorageConnectionString
            return CloudStorageAccount.Parse(key);
           // return CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=infoseedstorage;AccountKey=zGavJ4898+nEMz+NafvWYGmmCQtaDyUmI62IrR3LvKg7QYaOyJJ6shZdsAPb3a8JoMgIvUQaFYFbjDxX7m2rmA==;EndpointSuffix=core.windows.net");
        }

    }
}
