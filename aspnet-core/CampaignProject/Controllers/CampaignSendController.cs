using CampaignProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data;
using Framework.Data;
using Microsoft.AspNet.SignalR.Json;
using System.Linq;
using Infoseed.MessagingPortal.Web.Models;

namespace CampaignProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignSendController : ControllerBase
    {

        private readonly ILogger<CampaignSendController> _logger;

        public CampaignSendController(ILogger<CampaignSendController> logger)
        {

            _logger = logger;
        }

        [Route("SaveTheResponse")]
        [HttpPost]
        public async Task<IActionResult> SaveTheResponse(List<CampaignManager> Response)
        {
           await CreateCampaignManagers(Response);

            return Ok();
        }

        //[Route("SendCampaign")]
        //[HttpPost]
        //public async Task<IActionResult> SendCampaignAsync(CampaignModel campaign)
        //{

        //    const int requestsPerSecond =25;
        //    const int totalRequests = 2000; // Total number of requests to send
        //    string apiUrl = "https://infoseedbotapistg.azurewebsites.net/GetRandomString"; // Replace with your API endpoint

        //    // List to store all IDs
        //    List<CampaignManager> allIds = new List<CampaignManager>();

        //    // Prepare a list of tasks for the requests
        //    List<Task> requestTasks = new List<Task>();

        //    // Create an HttpClient instance
        //    using HttpClient client = new HttpClient();


        //    for (int i = 0; i < totalRequests; i++)
        //    {
        //        // Prepare the payload for the request (if applicable)
        //        PayloadModel payload = new PayloadModel()
        //        {
        //            to = $"recipient_number_{i}",
        //            message = $"Hello, this is message {i + 1}!"
        //        };

        //        // Add the request task with ID storage
        //        requestTasks.Add(SendWhatsAppMessageAsync(client, apiUrl, payload, allIds,27, 7824));

        //        // If we've reached the rate limit, wait for 1 second
        //        if ((i + 1) % requestsPerSecond == 0)
        //        {
        //            Console.WriteLine($"Sent {i + 1} requests. Waiting for 1 second...");
        //            await Task.Delay(1000); // Wait for 1 second
        //        }
        //    }




        //    // Wait for all tasks to complete
        //    await Task.WhenAll(requestTasks);
        //    Console.WriteLine("All requests have been processed.");

        //    // Example: Log or process the saved IDs
        //    Console.WriteLine("Saved IDs:");
        //    foreach (var id in allIds)
        //    {
        //        Console.WriteLine(id);
        //    }



        //    //create table : Id,teantId ,phoneNumber ,massageId,status(send,read,deliverd,faild) ,statusCode(200,1013,....),faildDetails

        //    // Respond based on the campaign status
        //    if (allIds.Count == totalRequests)
        //    {
        //        await CreateCampaignManagers(allIds);
        //        // All requests were processed successfully
        //        return Ok(new { Status = "Complete", SavedIds = allIds });
        //    }
        //    else
        //    {
        //        // Handle partial success
        //        return Ok(new { Status = "Partial", SavedIds = allIds });
        //    }
        //}

        #region privet

        private static async Task SendWhatsAppMessageAsync(HttpClient client, string apiUrl, PayloadModel payload, List<CampaignManager> allIds,int TenantId, int CampaignId)
        {
            try
            {
                // Convert payload to JSON (if needed for POST requests)

                var json = JsonConvert.SerializeObject(payload, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST request (or GET as per your API)
                HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));
              
                    // Read and process the response
                string responseContent = await response.Content.ReadAsStringAsync();
                SendResult templateResult = new SendResult();
                templateResult = JsonConvert.DeserializeObject<SendResult>(responseContent);
                var jsonResult = JsonConvert.SerializeObject(templateResult, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                CampaignManager campaign = new CampaignManager();


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    campaign.TenantId =TenantId;
                    campaign.PhoneNumber  =payload.to;
                    campaign.MassageId  =templateResult.result.messages.FirstOrDefault().id;
                    campaign.Status ="send";
                    campaign.StatusCode =200;
                    campaign.FaildDetails  ="";
                    campaign.DetailsJosn  =jsonResult;
                    campaign.CampaignId =CampaignId;

                }
                else
                {
                    campaign.TenantId =TenantId;
                    campaign.PhoneNumber  =payload.to;
                    campaign.MassageId  =templateResult.result.error.fbtrace_id;
                    campaign.Status ="falid";
                    campaign.StatusCode =templateResult.result.error.code;
                    campaign.FaildDetails  =templateResult.result.error.error_data.messaging_product;
                    campaign.DetailsJosn  =jsonResult;
                    campaign.CampaignId =CampaignId;

                }
             
                // Safely add the ID to the shared list
                lock (allIds)
                    {
                        allIds.Add(campaign);
                    }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private static async Task<int> CreateCampaignManagers(List<CampaignManager> campaignManagers)
        {
            try
            {


                if (campaignManagers.Count()>1000)
                {
                    int chunkSize = 5; // Define the size of each chunk
                    var chunks = SplitList(campaignManagers, chunkSize);

                    foreach (var chunk in chunks.Result)
                    {
                        await Task.Delay(2000);
                        string jsonChunk = JsonConvert.SerializeObject(chunk);

                        var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                           {
                               new System.Data.SqlClient.SqlParameter("@CampaignManagersJson", SqlDbType.NVarChar, -1)
                               {
                                   Value = jsonChunk
                               },
                             
                           };

                        SqlDataHelper.ExecuteNoneQuery("AddCampaignManagerBulk", sqlParameters.ToArray(), SettingsModel.ConnectionStrings);
                    }




                }
                else
                {


                    string jsonChunk = JsonConvert.SerializeObject(campaignManagers);

                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                           {
                               new System.Data.SqlClient.SqlParameter("@CampaignManagersJson", SqlDbType.NVarChar, -1)
                               {
                                   Value = jsonChunk
                               },
                              
                           };

                    SqlDataHelper.ExecuteNoneQuery("AddCampaignManagerBulk", sqlParameters.ToArray(), SettingsModel.ConnectionStrings);
                }


                return 1;
            }
            catch (Exception ex)
            {
                // Consider logging the exception instead of rethrowing
                return -1;
            }
        }
        private static async Task<List<List<T>>> SplitList<T>(List<T> list, int numberOfLists)
        {
            int chunkSize = (int)Math.Ceiling((double)list.Count / numberOfLists);

            return Enumerable.Range(0, numberOfLists)
                .Select(i => list.Skip(i * chunkSize).Take(chunkSize).ToList())
                .ToList();
        }
        #endregion
        private static string prepareMessageTemplateText(MessageTemplateModel objWhatsAppTemplateModel, out string type, out string mediaUrl)
        {
            try
            {
                string result = string.Empty;
                type = "text";
                mediaUrl = "";
                if (objWhatsAppTemplateModel.components != null)
                {
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type.Equals("HEADER"))
                        {


                            type = item.format.ToLower();

                            if (type=="document")
                            {


                                if (!string.IsNullOrEmpty(objWhatsAppTemplateModel.mediaLink))
                                {
                                    if (objWhatsAppTemplateModel.mediaLink.Contains(","))
                                    {

                                        var media = objWhatsAppTemplateModel.mediaLink.Split(",")[1];
                                        try
                                        {
                                            type = "application";
                                            result+=media+"\n\r";

                                            mediaUrl=media;
                                        }
                                        catch
                                        {

                                        }

                                    }
                                    else
                                    {
                                        try
                                        {
                                            type = "application";
                                            result+=objWhatsAppTemplateModel.mediaLink+"\n\r";

                                            mediaUrl=objWhatsAppTemplateModel.mediaLink;
                                        }
                                        catch
                                        {

                                        }

                                    }




                                }
                                else
                                {
                                    try
                                    {
                                        type = "application";
                                        result+=item.example.header_handle[0]+"\n\r";

                                        mediaUrl=item.example.header_handle[0];
                                    }
                                    catch
                                    {

                                    }


                                }



                            }

                        }
                        if (item.type.Equals("BUTTONS"))
                        {
                            for (int i = 0; i < item.buttons.Count; i++)
                            {
                                result = result + "\n\r" + (i + 1) + "-" + item.buttons[i].text;
                            }
                        }
                        result += item.text;

                    }

                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private static PostMessageTemplateModel prepareMessageTemplate(MessageTemplateModel objWhatsAppTemplateModel, ListContactCampaign contact, TemplateVariablles2 templateVariables)
        {
            try
            {
                PostMessageTemplateModel postMessageTemplateModel = new PostMessageTemplateModel();
                WhatsAppTemplateModel whatsAppTemplateModel = new WhatsAppTemplateModel();
                WhatsAppLanguageModel whatsAppLanguageModel = new WhatsAppLanguageModel();

                List<Component> components = new List<Component>();
                Component componentHeader = new Component();
                Parameter parameterHeader = new Parameter();

                Component componentBody = new Component();
                Parameter parameterBody1 = new Parameter();
                Parameter parameterBody2 = new Parameter();
                Parameter parameterBody3 = new Parameter();
                Parameter parameterBody4 = new Parameter();
                Parameter parameterBody5 = new Parameter();

                ImageTemplate image = new ImageTemplate();
                VideoTemplate video = new VideoTemplate();
                DocumentTemplate document = new DocumentTemplate();


                if (objWhatsAppTemplateModel.components != null)
                {
                    components = new List<Component>();
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (objWhatsAppTemplateModel.category=="AUTHENTICATION")
                        {
                            Component component = new Component();


                            if (item.type == "BODY")
                            {

                                component.type="body";
                                component.parameters=new List<Parameter>();
                                component.parameters.Add(new Parameter()
                                {
                                    type="text",
                                    text=contact.templateVariables.VarOne

                                });


                            }
                            else if (item.type == "BUTTONS")
                            {
                                component.type="button";
                                component.sub_type="url";
                                component.index=0;
                                component.parameters=new List<Parameter>();
                                component.parameters.Add(new Parameter()
                                {
                                    type="text",
                                    text=contact.templateVariables.VarOne

                                });


                            }



                            components.Add(component);


                        }
                        else
                        {
                            if (item.type == "HEADER")
                            {
                                componentHeader.type = item.type;
                                parameterHeader.type = item.format.ToLower();
                                if (item.format == "IMAGE")
                                {
                                    image.link = objWhatsAppTemplateModel.mediaLink;
                                    parameterHeader.image = image;
                                    componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };
                                }
                                if (item.format == "VIDEO")
                                {
                                    video.link = objWhatsAppTemplateModel.mediaLink;
                                    parameterHeader.video = video;
                                    componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };

                                }
                                if (item.format == "DOCUMENT")
                                {


                                    if (objWhatsAppTemplateModel.mediaLink.Contains(","))
                                    {
                                        try
                                        {
                                            document.filename=objWhatsAppTemplateModel.mediaLink.Split(",")[0];
                                            document.link =objWhatsAppTemplateModel.mediaLink.Split(",")[1];
                                        }
                                        catch
                                        {
                                            document.link = objWhatsAppTemplateModel.mediaLink;

                                        }

                                    }
                                    else
                                    {
                                        Uri uri = new Uri(objWhatsAppTemplateModel.mediaLink);

                                        // Extract the file name
                                        string fileName = System.IO.Path.GetFileName(uri.LocalPath);

                                        if (fileName.Length<20)
                                        {

                                            document.filename=fileName;
                                        }
                                        else
                                        {
                                            document.filename="fileName";
                                        }



                                        document.link = objWhatsAppTemplateModel.mediaLink;
                                    }

                                    parameterHeader.document = document;

                                    componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };

                                }
                                components.Add(componentHeader);

                            }
                            if (item.type == "BODY")
                            {
                                if (contact.templateVariables != null)
                                {
                                    if (objWhatsAppTemplateModel.VariableCount >= 1)
                                    {
                                        parameterBody1.type = "TEXT";
                                        parameterBody1.text = contact.templateVariables.VarOne;
                                        componentBody.parameters.Add(parameterBody1);
                                        if (objWhatsAppTemplateModel.VariableCount >= 2)
                                        {
                                            parameterBody2.type = "TEXT";
                                            parameterBody2.text = contact.templateVariables.VarTwo;
                                            componentBody.parameters.Add(parameterBody2);

                                            if (objWhatsAppTemplateModel.VariableCount >= 3)
                                            {
                                                parameterBody3.type = "TEXT";
                                                parameterBody3.text = contact.templateVariables.VarThree;
                                                componentBody.parameters.Add(parameterBody3);

                                                if (objWhatsAppTemplateModel.VariableCount >= 4)
                                                {
                                                    parameterBody4.type = "TEXT";
                                                    parameterBody4.text = contact.templateVariables.VarFour;
                                                    componentBody.parameters.Add(parameterBody4);

                                                    if (objWhatsAppTemplateModel.VariableCount >= 5)
                                                    {
                                                        parameterBody5.type = "TEXT";
                                                        parameterBody5.text = contact.templateVariables.VarFive;
                                                        componentBody.parameters.Add(parameterBody5);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                                else if (templateVariables != null)
                                {
                                    if (objWhatsAppTemplateModel.VariableCount >= 1)
                                    {
                                        componentBody.parameters = new List<Parameter>();

                                        parameterBody1.type = "TEXT";
                                        parameterBody1.text = templateVariables.VarOne;
                                        componentBody.parameters.Add(parameterBody1);

                                        if (objWhatsAppTemplateModel.VariableCount >= 2)
                                        {
                                            parameterBody2.type = "TEXT";
                                            parameterBody2.text = templateVariables.VarTwo;
                                            componentBody.parameters.Add(parameterBody2);

                                            if (objWhatsAppTemplateModel.VariableCount >= 3)
                                            {
                                                parameterBody3.type = "TEXT";
                                                parameterBody3.text = templateVariables.VarThree;
                                                componentBody.parameters.Add(parameterBody3);

                                                if (objWhatsAppTemplateModel.VariableCount >= 4)
                                                {
                                                    parameterBody4.type = "TEXT";
                                                    parameterBody4.text = templateVariables.VarFour;
                                                    componentBody.parameters.Add(parameterBody4);

                                                    if (objWhatsAppTemplateModel.VariableCount >= 5)
                                                    {
                                                        parameterBody5.type = "TEXT";
                                                        parameterBody5.text = templateVariables.VarFive;
                                                        componentBody.parameters.Add(parameterBody5);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }




                                componentBody.type = item.type;

                                components.Add(componentBody);
                            }
                        }



                    }

                }




                whatsAppLanguageModel.code = objWhatsAppTemplateModel.language;
                whatsAppTemplateModel.language = whatsAppLanguageModel;
                whatsAppTemplateModel.name = objWhatsAppTemplateModel.name;
                whatsAppTemplateModel.components = components;
                postMessageTemplateModel.template = whatsAppTemplateModel;
                postMessageTemplateModel.to = contact.PhoneNumber;
                //postMessageTemplateModel.to = "962786464718";
                postMessageTemplateModel.messaging_product = "whatsapp";
                postMessageTemplateModel.type = "template";
                return postMessageTemplateModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private static long UpdateCampaignStatus(long rowId)
        {
            try
            {
                var SP_Name = "[dbo].[CampaignUpdateStatus]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), SettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt64(OutputParameter.Value) : 0;
            }
            catch
            {
                return 0;
            }
        }
        private static string VariablesContact(ListContactCampaign contacts, string teaminboxmsg)
        {
            if (contacts.templateVariables!=null)
            {
                if (contacts.templateVariables.VarOne!=null)
                {
                    teaminboxmsg= teaminboxmsg.Replace("{{1}}", contacts.templateVariables.VarOne);

                }
                if (contacts.templateVariables.VarTwo!=null)
                {
                    teaminboxmsg= teaminboxmsg.Replace("{{2}}", contacts.templateVariables.VarTwo);

                }
                if (contacts.templateVariables.VarThree!=null)
                {
                    teaminboxmsg= teaminboxmsg.Replace("{{3}}", contacts.templateVariables.VarThree);

                }
                if (contacts.templateVariables.VarFour!=null)
                {
                    teaminboxmsg= teaminboxmsg.Replace("{{4}}", contacts.templateVariables.VarFour);

                }
                if (contacts.templateVariables.VarFive!=null)
                {
                    teaminboxmsg= teaminboxmsg.Replace("{{5}}", contacts.templateVariables.VarFive);

                }
            }

            return teaminboxmsg;
        }
      
    }
}
