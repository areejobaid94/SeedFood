using DocumentFormat.OpenXml.Spreadsheet;
using Infoseed.MessagingPortal.InfoSeedParser;
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
    public class MembersGroupParser : IMembersGroupParser
    {

        #region public
        public MembersGroup ParseMembers(ParseConfig parserOptions)
        {
            return parseMembers(parserOptions);
        }

        #endregion

        #region private

        private MembersGroup parseMembers(ParseConfig parserOptions)
        {
            try
            {
                MenuExcelDictionaryModel qnAExcelDictionaryModel = new MenuExcelDictionaryModel();

                var FileName = Path.GetFileNameWithoutExtension(parserOptions.FileName);
                var Data = ExcelReader.ParseExcelDictionaryByte(parserOptions.FileData);
                qnAExcelDictionaryModel.Data = Data;
                var result = ParseTheList(qnAExcelDictionaryModel, parserOptions.MembersConfig as MembersConfigrationExcelFile, FileName);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private MembersGroup ParseTheList(MenuExcelDictionaryModel contact, MembersConfigrationExcelFile confg, string FileName)
        {

            List<MembersGroup> qnADTOs = new List<MembersGroup>();

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

                (List<MembersList> noRepet, long duplicateCount) = FixList(qnADTOs);

                MembersGroup parseMemberModel = new MembersGroup
                {
                    members = noRepet,
                    duplicateCount = duplicateCount

                };

                return parseMemberModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        MembersGroup GetTheItem(KeyValuePair<int, Dictionary<string, string>> kvp, MembersConfigrationExcelFile confg, string FileName)
        {
            try
            {
                string members_PhoneNumber_ColumnID;
                string members_Name_ColumnID;
                string members_Var1_ColumnID;
                string members_Var2_ColumnID;
                string members_Var3_ColumnID;
                string members_Var4_ColumnID;
                string members_Var5_ColumnID;
            GetValueFunchation(kvp, confg,
                    out members_PhoneNumber_ColumnID,
                    out members_Name_ColumnID,
                    out members_Var1_ColumnID,
                    out members_Var2_ColumnID,
                    out members_Var3_ColumnID,
                    out members_Var4_ColumnID,
                    out members_Var5_ColumnID
                );

                List<MembersList> Item = new List<MembersList>();
                // Create dictionary to store variable mappings
                var variablesDict = new Dictionary<string, string>
                        {
                            { "var1", members_Var1_ColumnID },
                            { "var2", members_Var2_ColumnID },
                            { "var3", members_Var3_ColumnID },
                            { "var4", members_Var4_ColumnID },
                            { "var5", members_Var5_ColumnID }
                        };


                Item.Add(new MembersList
                {
                    phoneNumber = members_PhoneNumber_ColumnID,
                    displayName = members_Name_ColumnID,
                    variables = variablesDict
                }
                );

                MembersGroup qnAModel = new MembersGroup()
                {
                    members = Item,
                };


                return qnAModel;
            }
            catch (Exception ex)
            {
                 throw ex;
            }
        }

        private static void GetValueFunchation(KeyValuePair<int, Dictionary<string, string>> kvp, MembersConfigrationExcelFile confg,out string members_PhoneNumber_ColumnID,out string members_Name_ColumnID,
                                                out string members_Var1_ColumnID, out string members_Var2_ColumnID, out string members_Var3_ColumnID, out string members_Var4_ColumnID , out string members_Var5_ColumnID)
        {
            try
            {
                members_PhoneNumber_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.members_PhoneNumber_ColumnID.ToLower())).FirstOrDefault().Value;
                members_Name_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.members_Name_ColumnID.ToLower())).FirstOrDefault().Value;
                members_Var1_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.members_Var1_ColumnID.ToLower())).FirstOrDefault().Value;
                members_Var2_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.members_Var2_ColumnID.ToLower())).FirstOrDefault().Value;
                members_Var3_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.members_Var3_ColumnID.ToLower())).FirstOrDefault().Value;
                members_Var4_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.members_Var4_ColumnID.ToLower())).FirstOrDefault().Value;
                members_Var5_ColumnID = kvp.Value.ToList().Where(x => x.Key.ToLower().Contains(confg.members_Var5_ColumnID.ToLower())).FirstOrDefault().Value;


                if (string.IsNullOrEmpty(members_PhoneNumber_ColumnID))
                    members_PhoneNumber_ColumnID = "";
                if (string.IsNullOrEmpty(members_Name_ColumnID))
                    members_Name_ColumnID = "";
                if (string.IsNullOrEmpty(members_Var1_ColumnID))
                    members_Var1_ColumnID = "";
                if (string.IsNullOrEmpty(members_Var2_ColumnID))
                    members_Var2_ColumnID = "";
                if (string.IsNullOrEmpty(members_Var3_ColumnID))
                    members_Var3_ColumnID = "";
                if (string.IsNullOrEmpty(members_Var4_ColumnID))
                    members_Var4_ColumnID = "";
                if (string.IsNullOrEmpty(members_Var5_ColumnID))
                    members_Var5_ColumnID = "";

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        private (List<MembersList> List, long DuplicateCount) FixList(List<MembersGroup> qnADTOs)
        {
            try
            {
                List<MembersList> Item = new List<MembersList>(qnADTOs.Count);
                HashSet<string> seenPhoneNumbers = new HashSet<string>();
                long Id = 1;
                long duplicateCounter = 0;

                foreach (var group in qnADTOs)
                {
                    var member = group.members.FirstOrDefault();
                    if (member == null || string.IsNullOrWhiteSpace(member.phoneNumber))
                        continue;

                    string cleanedNumber = member.phoneNumber.Trim();

                    if (string.IsNullOrWhiteSpace(member.displayName))
                        member.displayName = cleanedNumber;

                    member.phoneNumber = cleanedNumber;

                    if (cleanedNumber.Length == 0 || !IsValidPhoneNumber(cleanedNumber))
                    {
                        member.isFailed = true;
                    }
                    else if (!seenPhoneNumbers.Add(cleanedNumber))
                    {
                        member.isFailed = true; // duplicate
                        duplicateCounter++;
                    }
                    else
                    {
                        member.isFailed = false;
                    }

                    member.Id = Id++;
                    Item.Add(member);
                }

                return (Item, duplicateCounter);

            }
            catch(Exception ex)
            {

                throw ex;
            }
           
        }

        private bool IsValidPhoneNumber(string number)
        {
            if (number.StartsWith("+"))
                number = number.Substring(1);

            return number.All(char.IsDigit) && long.TryParse(number, out _);
        }


        #endregion
    }
}
