using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infoseed.MessagingPortal.Sunshine.Models;

namespace Infoseed.MessagingPortal.Sunshine.Infrastructure
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
                await container.CreateIfNotExistsAsync();

                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob

                });
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(Guid.NewGuid() + attachment.Extension);
                cloudBlockBlob.Properties.ContentType = attachment.MimeType;
                using (MemoryStream ms = new MemoryStream(attachment.Content))
                {
                    await cloudBlockBlob.UploadFromStreamAsync(ms);
                    returnedURI = cloudBlockBlob.Uri.AbsoluteUri.ToString();
                }
               //await cloudBlockBlob.UploadFromStreamAsync(new MemoryStream(
               //     attachment.Content

               //     ));



                // Save attachment record in the database

                return returnedURI;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static CloudStorageAccount GetStorageAccountByURI()
        {
            //
            //zGavJ4898+nEMz+NafvWYGmmCQtaDyUmI62IrR3LvKg7QYaOyJJ6shZdsAPb3a8JoMgIvUQaFYFbjDxX7m2rmA==
            //ConfigurationManager.EnvironmentConfigurations.StorageSettings.SubscriptionStorageConnectionString
            return CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=infoseedstorage;AccountKey=zGavJ4898+nEMz+NafvWYGmmCQtaDyUmI62IrR3LvKg7QYaOyJJ6shZdsAPb3a8JoMgIvUQaFYFbjDxX7m2rmA==;EndpointSuffix=core.windows.net");
        }

    }
}
