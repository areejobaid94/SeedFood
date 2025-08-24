using InfoSeedAzureFunction.Entities;
using InfoSeedAzureFunction.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace InfoSeedAzureFunction.DAL
{
    public class ConversationRepository
    {
        public static void LogConversationMasurements(ConservationMeasurement entity, TextWriter log)
        {
            try
            {
                if (!string.IsNullOrEmpty(entity.ConversationId))
                {
                    decimal rate = 0;
                    CoutryTelCodeModel countryCode = getRate(entity.PhoneNumber);
                    rate = countryCode.Rate;

                    // int AddHour = int.Parse(ConfigurationManager.AppSettings["AddHour"]);
                    // DateTime dateTime = DateTime.UtcNow.AddHours(AddHour);

                    var sqlParameters = new List<SqlParameter> {
                        new SqlParameter("@TenantId",entity.TenantId),
                        new SqlParameter("@CommunicationInitiated",(int)entity.CommunicationInitiated),
                        new SqlParameter("@ConversationId",entity.ConversationId),
                        new SqlParameter("@PhoneNumber",entity.PhoneNumber),
                        new SqlParameter("@MessageId",entity.MessageId),
                        new SqlParameter("@MessageStatusId",entity.MessageStatusId),

                        new SqlParameter("@expiration_timestamp",entity.expiration_timestamp),
                        new SqlParameter("@creation_timestamp",entity.creation_timestamp),
                        new SqlParameter("@RateUsage",1),
                    };

                    SqlParameter OutsqlParameter = new SqlParameter();
                    OutsqlParameter.ParameterName = "@IsBundleActive";
                    OutsqlParameter.SqlDbType = System.Data.SqlDbType.Bit;
                    OutsqlParameter.Direction = System.Data.ParameterDirection.Output;
                    sqlParameters.Add(OutsqlParameter);

                    SqlDataHelper.ExecuteNoneQuery(Constants.SP_ConservationMeasurementUpdate,sqlParameters.ToArray(), Constants.ConnectionString);
                    try
                    {
                        if (OutsqlParameter.Value != DBNull.Value && !(bool)OutsqlParameter.Value)
                        {
                            UpdateConversationBundle(entity.TenantId);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception, e.g., log it for troubleshooting
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                }
                else
                {
                    var sqlParameters = new List<SqlParameter> {
                        new SqlParameter("@MessageId",entity.MessageId),
                        new SqlParameter("@MessageStatusId",entity.MessageStatusId),
                        new SqlParameter("@TenantId",entity.TenantId),
                    };
                    SqlDataHelper.ExecuteNoneQuery(Constants.SP_ContactsCampaignMessageUpdate,sqlParameters.ToArray(), Constants.ConnectionString);
                }

            }
            catch (SqlException sqlex)
            {
                log.Write("SQL ERROR WHILE  UPDATING ConservationMeasurement: " + sqlex.StackTrace);

                throw sqlex;
            }
            catch (Exception ex)
            {
                log.Write("UNEXPECTED EXCEPTION WHILE UPDATING ConservationMeasurement: " + ex.StackTrace);
                throw ex;
            }
        }


        private static CoutryTelCodeModel getRate(string phone)
        {
            List<CoutryTelCodeModel> TelCodes = new List<CoutryTelCodeModel>
                   {
                       new CoutryTelCodeModel("93", "AF",1),
                       new CoutryTelCodeModel("355", "AL",1),
                       new CoutryTelCodeModel("213", "DZ",2),
                       new CoutryTelCodeModel("1-684", "AS",1),
                       new CoutryTelCodeModel("376", "AD",1),
                       new CoutryTelCodeModel("244", "AO",1),
                       new CoutryTelCodeModel("1-264", "AI",1),
                       new CoutryTelCodeModel("672", "AQ",1),
                       new CoutryTelCodeModel("1-268", "AG",1),
                       new CoutryTelCodeModel("54", "AR",1),
                       new CoutryTelCodeModel("374", "AM",1),
                       new CoutryTelCodeModel("297", "AW",1),
                       new CoutryTelCodeModel("61", "AU",1),
                       new CoutryTelCodeModel("43", "AT",(decimal)1.5),
                       new CoutryTelCodeModel("994", "AZ",1),
                       new CoutryTelCodeModel("1-242", "BS",1),
                       new CoutryTelCodeModel("973", "BH",1),
                       new CoutryTelCodeModel("880", "BD",1),
                       new CoutryTelCodeModel("1-246", "BB",1),
                       new CoutryTelCodeModel("375", "BY",(decimal)1.5),
                       new CoutryTelCodeModel("32", "BE",2),
                       new CoutryTelCodeModel("501", "BZ",1),
                       new CoutryTelCodeModel("229", "BJ",1),
                       new CoutryTelCodeModel("1-441", "BM",1),
                       new CoutryTelCodeModel("975", "BT",1),
                       new CoutryTelCodeModel("591", "BO",1),
                       new CoutryTelCodeModel("387", "BA",1),
                       new CoutryTelCodeModel("267", "BW",1),
                       new CoutryTelCodeModel("55", "BR",1),
                       new CoutryTelCodeModel("246", "IO",1),
                       new CoutryTelCodeModel("1-284", "VG",1),
                       new CoutryTelCodeModel("673", "BN",1),
                       new CoutryTelCodeModel("359", "BG",(decimal)1.5),
                       new CoutryTelCodeModel("226", "BF",1),
                       new CoutryTelCodeModel("257", "BI",1),
                       new CoutryTelCodeModel("855", "KH",1),
                       new CoutryTelCodeModel("237", "CM",1),
                       new CoutryTelCodeModel("1", "CA",(decimal)0.5),
                       new CoutryTelCodeModel("238", "CV",1),
                       new CoutryTelCodeModel("1-345", "KY",1),
                       new CoutryTelCodeModel("236", "CF",1),
                       new CoutryTelCodeModel("235", "TD",1),
                       new CoutryTelCodeModel("56", "CL",1),
                       new CoutryTelCodeModel("86", "CN",1),
                       new CoutryTelCodeModel("61", "CX",1),
                       new CoutryTelCodeModel("61", "CC",1),
                       new CoutryTelCodeModel("57", "CO",(decimal)0.5),
                       new CoutryTelCodeModel("269", "KM",1),
                       new CoutryTelCodeModel("682", "CK",1),
                       new CoutryTelCodeModel("506", "CR",1),
                       new CoutryTelCodeModel("385", "HR",(decimal)1.5),
                       new CoutryTelCodeModel("53", "CU",1),
                       new CoutryTelCodeModel("599", "CW",1),
                       new CoutryTelCodeModel("357", "CY",1),
                       new CoutryTelCodeModel("420", "CZ",(decimal)1.5),
                       new CoutryTelCodeModel("243", "CD",1),
                       new CoutryTelCodeModel("45", "DK",2),
                       new CoutryTelCodeModel("253", "DJ",1),
                       new CoutryTelCodeModel("1-767", "DM",1),
                       new CoutryTelCodeModel("1-809", "DO",1),
                       new CoutryTelCodeModel("1-829", "DO",1),
                       new CoutryTelCodeModel("1-849", "DO",1),
                       new CoutryTelCodeModel("670", "TL",1),
                       new CoutryTelCodeModel("593", "EC",1),
                       new CoutryTelCodeModel("20", "EG",(decimal)1.5),
                       new CoutryTelCodeModel("503", "SV",1),
                       new CoutryTelCodeModel("240", "GQ",1),
                       new CoutryTelCodeModel("291", "ER",1),
                       new CoutryTelCodeModel("372", "EE",(decimal)1.5),
                       new CoutryTelCodeModel("251", "ET",1),
                       new CoutryTelCodeModel("500", "FK",1),
                       new CoutryTelCodeModel("298", "FO",1),
                       new CoutryTelCodeModel("679", "FJ",1),
                       new CoutryTelCodeModel("358", "FI",1),
                       new CoutryTelCodeModel("33", "FR",2),
                       new CoutryTelCodeModel("689", "PF",1),
                       new CoutryTelCodeModel("241", "GA",1),
                       new CoutryTelCodeModel("220", "GM",1),
                       new CoutryTelCodeModel("995", "GE",1),
                       new CoutryTelCodeModel("49", "DE",2),
                       new CoutryTelCodeModel("233", "GH",1),
                       new CoutryTelCodeModel("350", "GI",1),
                       new CoutryTelCodeModel("30", "GR",1),
                       new CoutryTelCodeModel("299", "GL",(decimal)0.5),
                       new CoutryTelCodeModel("1-473", "GD",1),
                       new CoutryTelCodeModel("1-671", "GU",1),
                       new CoutryTelCodeModel("502", "GT",1),
                       new CoutryTelCodeModel("44-1481", "GG",1),
                       new CoutryTelCodeModel("224", "GN",1),
                       new CoutryTelCodeModel("245", "GW",1),
                       new CoutryTelCodeModel("592", "GY",1),
                       new CoutryTelCodeModel("509", "HT",1),
                       new CoutryTelCodeModel("504", "HN",1),
                       new CoutryTelCodeModel("852", "HK",1),
                       new CoutryTelCodeModel("36", "HU",(decimal)1.5),
                       new CoutryTelCodeModel("354", "IS",1),
                       new CoutryTelCodeModel("91", "IN",(decimal)0.5),
                       new CoutryTelCodeModel("62", "ID",(decimal)0.5),
                       new CoutryTelCodeModel("98", "IR",1),
                       new CoutryTelCodeModel("964", "IQ",1),
                       new CoutryTelCodeModel("353", "IE",1),
                       new CoutryTelCodeModel("44-1624", "IM",1),
                       new CoutryTelCodeModel("972", "IL",(decimal)0.5),
                       new CoutryTelCodeModel("39", "IT",1),
                       new CoutryTelCodeModel("225", "CI",1),
                       new CoutryTelCodeModel("1-876", "JM",1),
                       new CoutryTelCodeModel("81", "JP",1),
                       new CoutryTelCodeModel("44-1534", "JE",1),
                       new CoutryTelCodeModel("962", "JO",1),
                       new CoutryTelCodeModel("7", "KZ",1),
                       new CoutryTelCodeModel("254", "KE",1),
                       new CoutryTelCodeModel("686", "KI",1),
                       new CoutryTelCodeModel("383", "XK",1),
                       new CoutryTelCodeModel("965", "KW",1),
                       new CoutryTelCodeModel("996", "KG",1),
                       new CoutryTelCodeModel("856", "LA",1),
                       new CoutryTelCodeModel("371", "LV",1),
                       new CoutryTelCodeModel("961", "LB",1),
                       new CoutryTelCodeModel("266", "LS",1),
                       new CoutryTelCodeModel("231", "LR",1),
                       new CoutryTelCodeModel("218", "LY",2),
                       new CoutryTelCodeModel("423", "LI",1),
                       new CoutryTelCodeModel("370", "LT",1),
                       new CoutryTelCodeModel("352", "LU",1),
                       new CoutryTelCodeModel("853", "MO",1),
                       new CoutryTelCodeModel("389", "MK",1),
                       new CoutryTelCodeModel("261", "MG",1),
                       new CoutryTelCodeModel("265", "MW",1),
                       new CoutryTelCodeModel("60", "MY",1),
                       new CoutryTelCodeModel("960", "MV",1),
                       new CoutryTelCodeModel("223", "ML",1),
                       new CoutryTelCodeModel("356", "MT",1),
                       new CoutryTelCodeModel("692", "MH",1),
                       new CoutryTelCodeModel("222", "MR",1),
                       new CoutryTelCodeModel("230", "MU",1),
                       new CoutryTelCodeModel("262", "YT",1),
                       new CoutryTelCodeModel("52", "MX",(decimal)0.5),
                       new CoutryTelCodeModel("691", "FM",1),
                       new CoutryTelCodeModel("373", "MD",(decimal)1.5),
                       new CoutryTelCodeModel("377", "MC",1),
                       new CoutryTelCodeModel("976", "MN",1),
                       new CoutryTelCodeModel("382", "ME",1),
                       new CoutryTelCodeModel("1-664", "MS",1),
                       new CoutryTelCodeModel("212", "MA",1),
                       new CoutryTelCodeModel("258", "MZ",1),
                       new CoutryTelCodeModel("95", "MM",1),
                       new CoutryTelCodeModel("264", "NA",1),
                       new CoutryTelCodeModel("674", "NR",1),
                       new CoutryTelCodeModel("977", "NP",1),
                       new CoutryTelCodeModel("31", "NL",1),
                       new CoutryTelCodeModel("599", "AN",1),
                       new CoutryTelCodeModel("687", "NC",1),
                       new CoutryTelCodeModel("64", "NZ",1),
                       new CoutryTelCodeModel("505", "NI",1),
                       new CoutryTelCodeModel("227", "NE",1),
                       new CoutryTelCodeModel("234", "NG",1),
                       new CoutryTelCodeModel("683", "NU",1),
                       new CoutryTelCodeModel("850", "KP",1),
                       new CoutryTelCodeModel("1-670", "MP",1),
                       new CoutryTelCodeModel("47", "NO",1),
                       new CoutryTelCodeModel("968", "OM",1),
                       new CoutryTelCodeModel("92", "PK",1),
                       new CoutryTelCodeModel("680", "PW",1),
                       new CoutryTelCodeModel("970", "PS",1),
                       new CoutryTelCodeModel("507", "PA",1),
                       new CoutryTelCodeModel("675", "PG",1),
                       new CoutryTelCodeModel("595", "PY",1),
                       new CoutryTelCodeModel("51", "PE",1),
                       new CoutryTelCodeModel("63", "PH",1),
                       new CoutryTelCodeModel("64", "PN",1),
                       new CoutryTelCodeModel("48", "PL",(decimal)1.5),
                       new CoutryTelCodeModel("351", "PT",1),
                       new CoutryTelCodeModel("1-787", "PR",1),
                       new CoutryTelCodeModel("1-939", "PR",1),
                       new CoutryTelCodeModel("974", "QA",1),
                       new CoutryTelCodeModel("242", "CG",1),
                       new CoutryTelCodeModel("262", "RE",1),
                       new CoutryTelCodeModel("40", "RO",(decimal)1.5),
                       new CoutryTelCodeModel("7", "RU",1),
                       new CoutryTelCodeModel("250", "RW",1),
                       new CoutryTelCodeModel("590", "BL",1),
                       new CoutryTelCodeModel("290", "SH",1),
                       new CoutryTelCodeModel("1-869", "KN",1),
                       new CoutryTelCodeModel("1-758", "LC",1),
                       new CoutryTelCodeModel("590", "MF",1),
                       new CoutryTelCodeModel("508", "PM",(decimal)0.5),
                       new CoutryTelCodeModel("1-784", "VC",1),
                       new CoutryTelCodeModel("685", "WS",1),
                       new CoutryTelCodeModel("378", "SM",1),
                       new CoutryTelCodeModel("239", "ST",1),
                       new CoutryTelCodeModel("966", "SA",(decimal)0.5),
                       new CoutryTelCodeModel("221", "SN",1),
                       new CoutryTelCodeModel("381", "RS",1),
                       new CoutryTelCodeModel("248", "SC",1),
                       new CoutryTelCodeModel("232", "SL",1),
                       new CoutryTelCodeModel("65", "SG",1),
                       new CoutryTelCodeModel("1-721", "SX",1),
                       new CoutryTelCodeModel("421", "SK",(decimal)1.5),
                       new CoutryTelCodeModel("386", "SI",1),
                       new CoutryTelCodeModel("677", "SB",1),
                       new CoutryTelCodeModel("252", "SO",1),
                       new CoutryTelCodeModel("27", "ZA",(decimal)0.5),
                       new CoutryTelCodeModel("82", "KR",1),
                       new CoutryTelCodeModel("211", "SS",2),
                       new CoutryTelCodeModel("34", "ES",1),
                       new CoutryTelCodeModel("94", "LK",1),
                       new CoutryTelCodeModel("249", "SD",2),
                       new CoutryTelCodeModel("597", "SR",1),
                       new CoutryTelCodeModel("47", "SJ",1),
                       new CoutryTelCodeModel("268", "SZ",1),
                       new CoutryTelCodeModel("46", "SE",1),
                       new CoutryTelCodeModel("41", "CH",1),
                       new CoutryTelCodeModel("963", "SY",1),
                       new CoutryTelCodeModel("886", "TW",1),
                       new CoutryTelCodeModel("992", "TJ",1),
                       new CoutryTelCodeModel("255", "TZ",1),
                       new CoutryTelCodeModel("66", "TH",1),
                       new CoutryTelCodeModel("228", "TG",1),
                       new CoutryTelCodeModel("690", "TK",1),
                       new CoutryTelCodeModel("676", "TO",1),
                       new CoutryTelCodeModel("1-868", "TT",1),
                       new CoutryTelCodeModel("216", "TN",2),
                       new CoutryTelCodeModel("90", "TR",(decimal)0.5),
                       new CoutryTelCodeModel("993", "TM",1),
                       new CoutryTelCodeModel("1-649", "TC",1),
                       new CoutryTelCodeModel("688", "TV",1),
                       new CoutryTelCodeModel("1-340", "VI",1),
                       new CoutryTelCodeModel("256", "UG",1),
                       new CoutryTelCodeModel("380", "UA",(decimal)1.5),
                       new CoutryTelCodeModel("971", "AE",(decimal)0.5),
                       new CoutryTelCodeModel("44", "GB",1),
                       new CoutryTelCodeModel("1", "US",(decimal)0.5),
                       new CoutryTelCodeModel("598", "UY",1),
                       new CoutryTelCodeModel("998", "UZ",1),
                       new CoutryTelCodeModel("678", "VU",1),
                       new CoutryTelCodeModel("379", "VA",1),
                       new CoutryTelCodeModel("58", "VE",1),
                       new CoutryTelCodeModel("84", "VN",1),
                       new CoutryTelCodeModel("681", "WF",1),
                       new CoutryTelCodeModel("212", "EH",1),
                       new CoutryTelCodeModel("967", "YE",1),
                       new CoutryTelCodeModel("260", "ZM",1),
                       new CoutryTelCodeModel("263", "ZW",1),
            };


            CoutryTelCodeModel result = TelCodes.Where(x => phone.StartsWith(x.Pfx)).FirstOrDefault();
            return result;

        }

        //public static bool CheckConversationBundleValidate(ConservationMeasurement entity, TextWriter log)
        //{
        //    try
        //    {

        //        int AddHour =int.Parse( ConfigurationManager.AppSettings["AddHour"]);
        //        DateTime dateTime = DateTime.UtcNow.AddHours(AddHour);
        //        var sqlParameters = new List<SqlParameter> {
        //            new SqlParameter("@TenantId",entity.TenantId),
        //            new SqlParameter("@DateTime",dateTime),

        //        };



        //        SqlDataHelper.ExecuteNoneQuery(
        //                   Constants.SP_ConversationBundleValidateGet,
        //                   sqlParameters.ToArray(), ConfigurationManager.ConnectionStrings["InfoseedDB"].ConnectionString);
        //        SqlParameter OutsqlParameter = new SqlParameter();
        //        OutsqlParameter.ParameterName = "@Result";
        //        OutsqlParameter.SqlDbType = System.Data.SqlDbType.Bit;
        //        OutsqlParameter.Direction = System.Data.ParameterDirection.Output;
        //        return (bool)OutsqlParameter.Value; ;
        //    }

        //    catch (SqlException sqlex)
        //    {
        //        log.Write("SQL ERROR WHILE  Check Conversation Bundle Validate: " + sqlex.StackTrace);

        //        throw sqlex;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Write("UNEXPECTED EXCEPTION WHILE Check Conversation Bundle Validate: " + ex.StackTrace);
        //        throw ex;
        //    }
        //}


        public static async void UpdateConversationBundle(int tenantId)
        {

            var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType ==ContainerItemTypes.Tenant && a.TenantId == tenantId);
            tenant.IsBundleActive = false;

            try
            {
                var url = Constant.EngineAPIURL + "api/WhatsAppAPI/DeleteCache?phoneNumberId=" + tenant.D360Key+ "&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges";
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(url))
                    {
                        var resultrespons = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {

                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            var result = itemsCollection.UpdateItemAsync(tenant._self, tenant);
        }


    }
}
