using Infoseed.MessagingPortal.InfoSeedParser;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using InfoSeedParser.ConfigrationFile;
using InfoSeedParser.Helpers;
using InfoSeedParser.Interfaces;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InfoSeedParser.Parsers
{
    public class ContactExcelNewParser : IContactNewParser
    {
        public CampinToQueueDto ParseContactNew(ParseConfig parserOptions)
        {
            return parseContactNew(parserOptions);
        }

        private CampinToQueueDto parseContactNew(ParseConfig parserOptions)
        {
            try
            {
                MenuExcelDictionaryModel qnAExcelDictionaryModel = new MenuExcelDictionaryModel();

                var FileName = Path.GetFileNameWithoutExtension(parserOptions.FileName);
                var Data = ExcelReader.ParseExcelDictionaryByte(parserOptions.FileData);
                qnAExcelDictionaryModel.Data = Data;
                var result = ParseTheList(qnAExcelDictionaryModel, parserOptions.ContactConfig as ContactConfigurationExcelFile, FileName);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CampinToQueueDto ParseTheList(MenuExcelDictionaryModel contact, ContactConfigurationExcelFile confg, string FileName)
        {

            List<CampinToQueueDto> qnADTOs = new List<CampinToQueueDto>();

            try
            {
                var Skip = contact.Data.Skip(0);
                if (confg.SkipFirstRowInExcel)
                {
                    Skip = contact.Data.Skip(1);
                }

                foreach (var kvp in Skip)
                {
                    qnADTOs.Add(GetTheItem(kvp, confg, FileName));
                }

                List<ListContactToCampin> noRepet = FixList(qnADTOs);

                CampinToQueueDto parseContactModel = new CampinToQueueDto
                {
                    contacts = noRepet
                };

                return parseContactModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<ListContactToCampin> FixList(List<CampinToQueueDto> qnADTOs)
        {
            List<ListContactToCampin> Item = new List<ListContactToCampin>();

            try
            {
                foreach (var item in qnADTOs)
                {
                    var contacts = item.contacts.FirstOrDefault();
                    contacts.PhoneNumber = contacts.PhoneNumber.Trim();

                    //if ContactName is null I will make its value the same as the phone number
                    if (string.IsNullOrEmpty(contacts.ContactName))
                        contacts.ContactName = contacts.PhoneNumber;

                    if ((contacts.PhoneNumber.Length >= 10 && contacts.PhoneNumber.Length <= 15) &&
                         (long.TryParse(contacts.PhoneNumber, out _) && long.Parse(contacts.PhoneNumber) >= 0))
                    {
                        Item.Add(contacts);
                    }
                }
                Item = Item.DistinctBy(x => x.PhoneNumber).ToList();
                return Item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        CampinToQueueDto GetTheItem(KeyValuePair<int, Dictionary<string, string>> kvp, ContactConfigurationExcelFile confg, string FileName)
        {
            try
            {
                string contact_PhoneNumber_ColumnID;
                string contact_Name_ColumnID;
                GetValueFunchation(kvp, confg,
                    out contact_PhoneNumber_ColumnID,
                    out contact_Name_ColumnID,
                    out string contact_Var1_ColumnID,
                    out string contact_Var2_ColumnID,
                    out string contact_Var3_ColumnID,
                    out string contact_Var4_ColumnID,
                    out string contact_Var5_ColumnID
                );


                List<ListContactToCampin> Item = new List<ListContactToCampin>();

                TemplateVariablles vars = new TemplateVariablles();
                if (contact_Var1_ColumnID  != null && contact_Var2_ColumnID != null && contact_Var3_ColumnID != null && contact_Var4_ColumnID != null && contact_Var5_ColumnID != null)
                {
                    vars.VarOne = contact_Var1_ColumnID;
                    vars.VarTwo = contact_Var2_ColumnID;
                    vars.VarThree = contact_Var3_ColumnID;
                    vars.VarFour = contact_Var4_ColumnID;
                    vars.VarFive = contact_Var5_ColumnID;
                    //vars.VarSix = contact_Var5_ColumnID;
                    //vars.VarSeven = contact_Var5_ColumnID;
                    //vars.VarEight = contact_Var5_ColumnID;
                    //vars.VarNine = contact_Var5_ColumnID;
                    //vars.VarTen = contact_Var5_ColumnID;


                }
                else
                {
                    vars = null;
                }


                Item.Add(new ListContactToCampin
                {
                    PhoneNumber = contact_PhoneNumber_ColumnID,
                    ContactName = contact_Name_ColumnID,

                    templateVariables = vars

                }
                );

                CampinToQueueDto qnAModel = new CampinToQueueDto()
                {

                    contacts = Item,

                };


                return qnAModel;
            }
            catch (Exception)
            {
                return null;
            }



        }

        private static void GetValueFunchation(KeyValuePair<int, Dictionary<string, string>> kvp, ContactConfigurationExcelFile confg,
            out string contact_PhoneNumber_ColumnID,
            out string contact_Name_ColumnID,
            out string contact_Var1_ColumnID,
            out string contact_Var2_ColumnID,
            out string contact_Var3_ColumnID,
            out string contact_Var4_ColumnID,
            out string contact_Var5_ColumnID
            )
        {
            try
            {
                contact_PhoneNumber_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.contact_PhoneNumber_ColumnID.ToLower())).FirstOrDefault().Value;
                contact_Name_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.contact_Name_ColumnID.ToLower())).FirstOrDefault().Value;
                contact_Var1_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.contact_Var1_ColumnID.ToLower())).FirstOrDefault().Value;
                contact_Var2_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.contact_Var2_ColumnID.ToLower())).FirstOrDefault().Value;
                contact_Var3_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.contact_Var3_ColumnID.ToLower())).FirstOrDefault().Value;
                contact_Var4_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.contact_Var4_ColumnID.ToLower())).FirstOrDefault().Value;
                contact_Var5_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.contact_Var5_ColumnID.ToLower())).FirstOrDefault().Value;

                if (string.IsNullOrEmpty(contact_PhoneNumber_ColumnID))
                    contact_PhoneNumber_ColumnID = "";
                if (string.IsNullOrEmpty(contact_Name_ColumnID))
                    contact_Name_ColumnID = "";

                if (string.IsNullOrEmpty(contact_Var1_ColumnID))
                    contact_Var1_ColumnID = " ";
                if (string.IsNullOrEmpty(contact_Var2_ColumnID))
                    contact_Var2_ColumnID = " ";
                if (string.IsNullOrEmpty(contact_Var3_ColumnID))
                    contact_Var3_ColumnID = " ";
                if (string.IsNullOrEmpty(contact_Var4_ColumnID))
                    contact_Var4_ColumnID = " ";
                if (string.IsNullOrEmpty(contact_Var5_ColumnID))
                    contact_Var5_ColumnID = " ";

            }
            catch (Exception ex)
            {
                throw ex;

            }


        }


    }
}
