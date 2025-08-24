using InfoSeedAzureFunction.Model;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static InfoSeedAzureFunction.Model.CreateContactMg;

namespace InfoSeedAzureFunction
{
    public static class MgMotorIntegrationFunction
    {

        //[FunctionName("MgMotorIntegrationFunction")]
        //public static void Run([QueueTrigger("mgcontacts-sync", Connection = "AzureWebJobsStorage")] string message)
        //{


        //    // log.LogInformation($"C# Queue trigger function processed: {message}");
        //    CreateContactMg obj = JsonConvert.DeserializeObject<CreateContactMg>(message);
        //    Sync(obj).Wait();
        //}
        public static async Task Sync(CreateContactMg model)
        {
            try
            {

                if (model.vid == "001100")
                {//post ERP MG


                    string url = "http://mg-api.ddns.net:86/XactaMobileApp/api/ServiceAppointment/CreateAppointment";
                    var client = new RestClient(url + $"?vin={model.createAppointmentMGModel.vin}&date={model.createAppointmentMGModel.date}&time={model.createAppointmentMGModel.time}&maintenance={model.createAppointmentMGModel.maintenance}&note={model.createAppointmentMGModel.note}&result=");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    IRestResponse response = client.Execute(request);

                }
                else if (model.vid == null || model.vid == "")
                {
                    HttpPost(Constant.MgMotorbaseUrl + "contacts/v1/contact", model); //create

                }
                else
                {

                    var con = GetContact(model.ContactId);

                    var result = HttpGet(Constant.MgMotorbaseUrl + "contacts/v1/search/query?q=" + con.PhoneNumber);
                    var contactMGId = JsonConvert.DeserializeObject<ContactsMg>(result).contacts.FirstOrDefault();


                    CreateContactMg createContactMg = new CreateContactMg();

                    var Interestedname = GetAssetLevelTwoName(model.levelTwoId);

                    ContactsMg contactsMg = new ContactsMg();

                    var liststring = "";
                    try
                    {
                        liststring = contactMGId.properties.car__cloned__1.value;

                        if (!liststring.ToLower().Contains(Interestedname.ToLower()))
                        {


                            liststring = liststring + ";" + Interestedname;
                        }
                    }
                    catch
                    {
                        liststring = Interestedname;
                    }


                    Property1 property1 = new Property1 { property = "car__cloned__1", value = liststring };
                    List<Property1> properties = new List<Property1>();

                    properties.Add(property1);

                    createContactMg.properties = properties.ToArray();
                    createContactMg.vid = contactMGId.vid.ToString();


                    HttpPost(Constant.MgMotorbaseUrl + "contacts/v1/contact/vid/" + createContactMg.vid, createContactMg); //update
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string HttpPost(string url, object body, Dictionary<string, string> queryString = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (queryString != null)
                {
                    StringBuilder b = QueryStringBuilder(url, queryString);
                    url = b.ToString();
                }




                var Jso = JsonConvert.SerializeObject(body, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });



                HttpContent content = new StringContent(Jso, Encoding.UTF8, "application/json");



                client.DefaultRequestHeaders.Add($"Authorization", "Bearer " + Constant.MgMotorKey);
                var result = client.PostAsync(url, content).Result;
                string resStr = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return resStr;
                }
                else
                {
                    throw new Exception(resStr);
                }
            }
        }

        private static StringBuilder QueryStringBuilder(string url, Dictionary<string, string> queryString)
        {
            StringBuilder b = new StringBuilder(url);
            var first = queryString.First();
            b = b.Append($"?{first.Key}={first.Value}");
            queryString.Remove(first.Key);
            foreach (var item in queryString)
            {
                b = b.Append($"{item.Key}={item.Value}");
            }

            return b;
        }

        private static Contact GetContact(int id)
        {

            try
            {


                Contact contact = new Contact();



                string connString = Constants.ConnectionString;
                string query = "select * from [dbo].[Contacts] where Id=" + id;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);


                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    try
                    {
                        contact = new Contact
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
                            PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),

                            Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
                            EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
                            Website = dataSet.Tables[0].Rows[i]["Website"].ToString(),

                            DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
                            //ContactDisplayName = dataSet.Tables[0].Rows[i]["ContactDisplayName"].ToString(),
                            TotalOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
                            TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
                            DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
                            loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"]),
                            StreetName = dataSet.Tables[0].Rows[i]["StreetName"].ToString(),
                            BuildingNumber = dataSet.Tables[0].Rows[i]["BuildingNumber"].ToString(),
                            FloorNo = dataSet.Tables[0].Rows[i]["FloorNo"].ToString(),
                            ApartmentNumber = dataSet.Tables[0].Rows[i]["ApartmentNumber"].ToString(),
                            TenantId = dataSet.Tables[0].Rows[i]["TenantId"] != null ? Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]) : 0,

                        };
                    }
                    catch
                    {
                        contact = new Contact
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
                            PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),

                            Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
                            EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
                            Website = dataSet.Tables[0].Rows[i]["Website"].ToString(),

                            DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
                            TotalOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
                            TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
                            DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
                            loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"]),
                            TenantId = dataSet.Tables[0].Rows[i]["TenantId"] != null ? Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]) : 0,
                        };
                    }

                }

                conn.Close();
                da.Dispose();

                return contact;

            }
            catch
            {
                return null;

            }

        }
        private static string GetAssetLevelTwoName(int? Id)
        {

            try
            {
                string connString = Constants.ConnectionString;
                string query = "select * from [dbo].[AssetLevelTwo] where Id=" + Id;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                string contact = "";

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    contact = dataSet.Tables[0].Rows[i]["LevelTwoNameEn"].ToString();
                }

                conn.Close();
                da.Dispose();

                return contact;

            }
            catch (Exception ex)
            {
                return "";

            }

        }


        public static string HttpGet(string url, Dictionary<string, string> queryString = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (queryString != null)
                {
                    StringBuilder b = QueryStringBuilder(url, queryString);
                    url = b.ToString();
                }
                client.DefaultRequestHeaders.Add($"Authorization", "Bearer " + Constant.MgMotorKey);

                var result = client.GetAsync(url).Result;
                string resStr = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return resStr;
                }
                else
                {
                    throw new Exception(resStr);
                }
            }
        }
    }
}
