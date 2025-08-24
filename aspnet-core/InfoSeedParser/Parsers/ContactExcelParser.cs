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
    public class ContactExcelParser : IContactParser
    {
        public ContactParserModel ParseContact(ParseConfig parserOptions)
        {
            MenuExcelDictionaryModel qnAExcelDictionaryModel = new MenuExcelDictionaryModel();

            var FileName = Path.GetFileNameWithoutExtension(parserOptions.FileName);
            var Data = ExcelReader.ParseExcelDictionaryByte(parserOptions.FileData);
            qnAExcelDictionaryModel.Data = Data;
            var result = ParseTheList(qnAExcelDictionaryModel, parserOptions.ContactConfig as ContactConfigurationExcelFile, FileName);
            return result;
        }
        private ContactParserModel ParseTheList(MenuExcelDictionaryModel contact, ContactConfigurationExcelFile confg, string FileName)
        {

            List<ContactParserModel> qnADTOs = new List<ContactParserModel>();

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

                List<WhatsAppContactsDto> noRepet = FixList(qnADTOs);

                ContactParserModel parseContactModel = new ContactParserModel
                {
                    Contacts = noRepet
                };

                return parseContactModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<WhatsAppContactsDto> FixList(List<ContactParserModel> qnADTOs)
        {
            List<WhatsAppContactsDto> Item = new List<WhatsAppContactsDto>();

            try
            {
                foreach (var item in qnADTOs)
                {
                    var contacts = item.Contacts.FirstOrDefault();
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

        ContactParserModel GetTheItem(KeyValuePair<int, Dictionary<string, string>> kvp, ContactConfigurationExcelFile confg, string FileName)
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


                List<WhatsAppContactsDto> Item = new List<WhatsAppContactsDto>();

                TemplateVariables vars = new TemplateVariables();
                if (contact_Var1_ColumnID  != null && contact_Var2_ColumnID != null && contact_Var3_ColumnID != null && contact_Var4_ColumnID != null && contact_Var5_ColumnID != null)
                {
                    vars.VarOne = contact_Var1_ColumnID;
                    vars.VarTwo = contact_Var2_ColumnID;
                    vars.VarThree = contact_Var3_ColumnID;
                    vars.VarFour = contact_Var4_ColumnID;
                    vars.VarFive = contact_Var5_ColumnID;
                }
                else
                {
                    vars = null;
                }
                

                Item.Add(new WhatsAppContactsDto
                {
                    PhoneNumber = contact_PhoneNumber_ColumnID,
                    ContactName = contact_Name_ColumnID,
                    
                    templateVariables = vars

                }
                );

                ContactParserModel qnAModel = new ContactParserModel()
                {

                    Contacts = Item,

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
