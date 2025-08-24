
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewFunctionApp
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

                if (string.IsNullOrEmpty(attachment.AttacmentName))
                    attachment.AttacmentName = Guid.NewGuid().ToString();


                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(attachment.AttacmentName + attachment.Extension);
                cloudBlockBlob.Properties.ContentType = attachment.MimeType;
                using (MemoryStream ms = new MemoryStream(attachment.Content))
                {
                    await cloudBlockBlob.UploadFromStreamAsync(ms);
                    returnedURI = cloudBlockBlob.Uri.AbsoluteUri.ToString();
                }



                // Save attachment record in the database

                return returnedURI;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string> geturlAsync(string Id)
        {
            try
            {
                string apiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxIiwianRpIjoiZDc5Mzk0MGQ2MTg0MTVmYzYxNTNmN2Y1YzNlYjZlNDE3YWU5NTFlNmU4NWRjYmEzN2ZiZWRmMzE1YmI2YzYwYzcxMjc1NTIyZGViMzM1YmEiLCJpYXQiOjE2ODU0NTU2MDkuNjg5NTYsIm5iZiI6MTY4NTQ1NTYwOS42ODk1NjIsImV4cCI6NDg0MTEyOTIwOS42ODEyNywic3ViIjoiMjI3NjMzMDEiLCJzY29wZXMiOlsidXNlci5yZWFkIiwidXNlci53cml0ZSIsInRhc2sucmVhZCIsInRhc2sud3JpdGUiLCJ3ZWJob29rLnJlYWQiLCJ3ZWJob29rLndyaXRlIiwicHJlc2V0LnJlYWQiLCJwcmVzZXQud3JpdGUiXX0.RxDf3jNPnUNkdqHMQKoznIDYneWxJkxLmLVzjZ_M3PdbELS4Ry6tCXJ__svDUiWpexV0ZHmw8woOhrYQcz9UlwLPiM3_9Z8gsMTBamijRBeGYo0MqSoh0eqYslz7Tlh7Cf-19FFz2k-FoOggWiKGLOhE10eSFhV5I8l3omdnvdpvfH-AHKViC0qxpGPeJuYoiVKZ1_O9_BS7qqYM3ERjW3IHgnWrxv_BzFdtQGJV-98s0bUYEuZepj6SYU9CaqD8lkjutNfNIcdVWCwKVKaGnBR5qdHUdS3JDMXt131FLIZrrfxi3uLyHtogv2Ggfd6zrKFqiasswrmWKdOfO_N6MqHYFp6ySoTPxBLI2Mu98TkyP80-6MzkJ3Nldu86tLhEtSwOyU7-8sUpgvEVILP6dXwnkpG9_8LXWEMs8uMdqA-eCEvk4Tg2hRRBgfup_ntFLm_TyObybMLXt86msfblS3SvaNCaIm909gy88SY0Pe_PEibfZksnT7P4LwxUwyR_K-_lNa0rMEhAzQ68Kb1rqr3g2I2iNUTnj1KEs60BsjWwjqjyc-vOR32h7mM1QrH2VoNmFhl5Uz-ltl5hcLr6GrW0s64Ye7agD6gCk99bUhMZiUwr8TKpBihobe_4sFcysRHiPP5X_7M1LFIrJpbV7HbMewvuptqjCT1A0XnZOwY";

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.cloudconvert.com/v2/tasks/"+Id+"?include=payload");
                request.Headers.Add("Authorization", "Bearer "+apiKey);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var Model = JsonConvert.DeserializeObject<GetUrlConvert>(await response.Content.ReadAsStringAsync());

                return Model.data.result.files.FirstOrDefault().inline_url;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        private async Task<string> createJopAsync(string url, string name)
        {
            try
            {

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.cloudconvert.com/v2/jobs");
                request.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxIiwianRpIjoiNWI3NjRjZDA5OGUxNjAyMzhhYWY4YjMzYzYwZDdkYWQ1ZDcyOTQ2ZTIxYzE5NzY1ODg3YjQwMzg2ZGIxMGQ0ODI5Yjk2OTU3NWUxNmY1NGUiLCJpYXQiOjE2ODU1MTU0NTIuMzE1NDkyLCJuYmYiOjE2ODU1MTU0NTIuMzE1NDkzLCJleHAiOjQ4NDExODkwNTIuMzA3MjE1LCJzdWIiOiIyMjc2MzMwMSIsInNjb3BlcyI6WyJ0YXNrLnJlYWQiLCJ0YXNrLndyaXRlIiwicHJlc2V0LnJlYWQiLCJwcmVzZXQud3JpdGUiLCJ3ZWJob29rLnJlYWQiLCJ3ZWJob29rLndyaXRlIiwidXNlci5yZWFkIiwidXNlci53cml0ZSJdfQ.dJe7R1syWhtEm7whOwyyznee_d9YqCozTwRD-HOFILIYOcZ3vWJ3057s9shfp9rhD9rxHwycDvQa-CVvdMerDkYAg23sIezLR7wI8jjku-lL7paeIRGj8vsUax_nj7z6wfOxCzyVQB_ageUCSTa9ygC4eZk-KKDPKh3OOlg3bROPkhsrGZ45tOL2fcQPp0emCHLJp-10EYG4RtMKifowACU9dyyslBN6eShGua-E7Jfy8YjVJ-LxMIg0NZeaN0WzsyOTUFGMhqoDVfXcrDl4AI2kR8erLDFnQsA_add9DMXaLBfPZUXpb2-LpXls8tUd8J3uwIzHXZ_dE4Ew1W2CcqXe1Gugi67ZwcRf0mAO46NG3E90VZkExdDQsYYl-QPFcTPSk2ADISBf6H3AmY1vB1HTfXly9hmUa_oTzZxIDP36FNQrrLoslFQf1u_2FFwa_1mswhWERWmvKqs_XI9EvCS42_ncj9aEYbY-wo65nNSczhNPrvEOzbI-e35hJwScT29iZuEKKtcg_AHlSbasDqXHwWI3E_wIYX__kHVtPn0VwCiCyYTmwets8Cnlrpopdc_G6m-mCTRQ8Bisq4NJ_L-xzNUmZCxdUqOeGQROslzDjAWX9dkBHO5YGXFzAMYXTlQ4exiz6yLftF_ue4PW71U0_uidit38YW3CQJ0D-78");

                JobsSendModel jobsSendModel = new JobsSendModel()
                {
                    tag="webinterface",
                    tasks=new JobsSendModel.Tasks
                    {
                        convertmp3=new JobsSendModel.ConvertMp3
                        {
                            engine="ffmpeg",
                            input= new string[] { "import-1" },
                            input_format="wav",
                            operation="convert",
                            output_format="mp3"


                        },
                        export= new JobsSendModel.Export
                        {
                            archive_multiple_files=true,
                            inline_additional=true,
                            input="convert-mp3",
                            operation="export/url"

                        },
                        import1=new JobsSendModel.Import1
                        {
                            operation="import/url",
                            filename=name+".mp3",
                            url=url,
                        }

                    }

                };


                var constra = JsonConvert.SerializeObject(jobsSendModel);

                constra=constra.Replace("convertmp3", "convert-mp3");
                constra=constra.Replace("\"import1\":", "\"import-1\":");

                var content = new StringContent(constra, null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var Model = JsonConvert.DeserializeObject<ConvertJopModel>(await response.Content.ReadAsStringAsync());
                return Model.data.tasks.Where(x => x.operation=="export/url").FirstOrDefault().id;


            }
            catch (Exception ex)
            {
                return "";
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
                throw;
            }
        }

        public static CloudStorageAccount GetStorageAccountByURI()
        {
            // var key = "DefaultEndpointsProtocol=https;AccountName=infoseedstoragestg;AccountKey=sc089/Ku+IUBAbCwGnlumuK72RultGBqL/TwHS36SJHlCx3uC9dtEKjJJJHPRiRrAMwrIs2FyP6Z+ASt8j6gWg==;EndpointSuffix=core.windows.net";// System.Environment.GetEnvironmentVariable("Values:AzureWebJobsStorage");     //AppSettingsModel.BlobServiceConnectionStrings;
            //
            //zGavJ4898+nEMz+NafvWYGmmCQtaDyUmI62IrR3LvKg7QYaOyJJ6shZdsAPb3a8JoMgIvUQaFYFbjDxX7m2rmA==
            //ConfigurationManager.EnvironmentConfigurations.StorageSettings.SubscriptionStorageConnectionString
            return CloudStorageAccount.Parse(Constant.AzureStorageAccount);
            // return CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=infoseedstorage;AccountKey=zGavJ4898+nEMz+NafvWYGmmCQtaDyUmI62IrR3LvKg7QYaOyJJ6shZdsAPb3a8JoMgIvUQaFYFbjDxX7m2rmA==;EndpointSuffix=core.windows.net");
        }

    }
}
