using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace InfoSeedParser.Helpers
{
    public class ExcelReader
    {

        public static Dictionary<int, Dictionary<string, string>> ParseExcelDictionary(string fileName)
        {
            var isUrl = checkWebsite(fileName);
            var theFile = fileName;
            if (isUrl)
            {
                //Create Attachement Document 
                theFile = CreateAttachementDocument(fileName);
            }
            //Lets open the existing excel file and read through its content . Open the excel using openxml sdk
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(theFile, false))
            {
                //create the object for workbook part  
                WorkbookPart workbookPart = doc.WorkbookPart;
                Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();
                StringBuilder excelResult = new StringBuilder();
                var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
           
                Dictionary<int, Dictionary<string, string>> myDic = new Dictionary<int, Dictionary<string, string>>();



                //using for each loop to get the sheet from the sheetcollection  
                foreach (Sheet thesheet in thesheetcollection)
                {
                    Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;
                    SheetData thesheetdata = (SheetData)theWorksheet.GetFirstChild<SheetData>();
                    int rowNum = 0;
                    foreach (Row thecurrentrow in thesheetdata)
                    {
                        int numCal = thecurrentrow.ChildElements.Count();
                        rowNum++;

                        Dictionary<string, string> ListRow = new Dictionary<string, string>();
                        for (int i = 0; i < numCal; i++)
                        {
                            var theCell = thecurrentrow.ChildElements.ElementAtOrDefault(i) as Cell;
                            var value = theCell.InnerText;

                            if (theCell.DataType != null)
                            {
                                switch (theCell.DataType.Value)
                                {
                                    case CellValues.SharedString:
                                        if (stringTable != null)
                                        {
                                            value = stringTable.SharedStringTable.
                                             ElementAt(int.Parse(value)).InnerText;
                                            ListRow.Add(theCell.CellReference.InnerText, value);
                                        }
                                        break;

                                    case CellValues.Boolean:
                                        switch (value)
                                        {
                                            case "0":
                                                value = "FALSE";
                                                break;
                                            default:
                                                value = "TRUE";
                                                break;
                                        }
                                        break;
                                }

                            }



                            if (theCell.DataType == null && value != "")
                            {
                                ListRow.Add(theCell.CellReference.InnerText, value);
                            }

                        }
                        if (ListRow.Count() != 0)
                        {
                            myDic.Add(rowNum, ListRow);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return myDic;
            }

        }

        private static string CreateAttachementDocument(string Url)
        {
            var request = WebRequest.CreateHttp(Url);
            var response = request.GetResponse();

            var fileName = Guid.NewGuid().ToString("N");

            using (var fs = new FileStream(fileName + ".xlsx", FileMode.Create, FileAccess.Write, FileShare.None))
            // using (var fs = new FileStream("D:\\" + fileName + ".xlsx", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var stream = response.GetResponseStream())
                {
                    stream.CopyTo(fs);
                }
            }

            return fileName + ".xlsx";
        }

        /// <summary>
        /// This method will check a url to see that it does not return server or protocol errors
        /// </summary>
        /// <param name="url">The path to check</param>
        /// <returns></returns>
        public static bool checkWebsite(string URL)
        {


            //Set URL in the string.

            if (!URL.Contains("http://"))
            {
                URL = "http://" + URL;
            }
            try
            {
                var request = WebRequest.Create(URL) as HttpWebRequest;
                request.Method = "HEAD";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }



        }


        public static Dictionary<int, Dictionary<string, string>> ParseExcelDictionaryByte(byte[] fileData)
        {
           

            using (var stream = new MemoryStream(fileData))
            {

                //Lets open the existing excel file and read through its content . Open the excel using openxml sdk
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
                {
                    //create the object for workbook part  
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();
                    StringBuilder excelResult = new StringBuilder();
                    var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                    Dictionary<int, Dictionary<string, string>> myDic = new Dictionary<int, Dictionary<string, string>>();



                    //using for each loop to get the sheet from the sheetcollection  
                    foreach (Sheet thesheet in thesheetcollection)
                    {
                        Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;
                        SheetData thesheetdata = (SheetData)theWorksheet.GetFirstChild<SheetData>();
                        int rowNum = 0;
                        foreach (Row thecurrentrow in thesheetdata)
                        {
                            int numCal = thecurrentrow.ChildElements.Count();
                            rowNum++;

                            Dictionary<string, string> ListRow = new Dictionary<string, string>();
                            for (int i = 0; i < numCal; i++)
                            {
                                var theCell = thecurrentrow.ChildElements.ElementAtOrDefault(i) as Cell;
                                var value = theCell.InnerText;

                                if (theCell.DataType != null)
                                {
                                    switch (theCell.DataType.Value)
                                    {
                                        case CellValues.SharedString:
                                            if (stringTable != null)
                                            {
                                                value = stringTable.SharedStringTable.
                                                 ElementAt(int.Parse(value)).InnerText;
                                                ListRow.Add(theCell.CellReference.InnerText, value);
                                            }
                                            break;

                                        case CellValues.Boolean:
                                            switch (value)
                                            {
                                                case "0":
                                                    value = "FALSE";
                                                    break;
                                                default:
                                                    value = "TRUE";
                                                    break;
                                            }
                                            break;
                                    }

                                }



                                if (theCell.DataType == null && value != "")
                                {
                                    ListRow.Add(theCell.CellReference.InnerText, value);
                                }

                            }
                            if (ListRow.Count() != 0)
                            {
                                myDic.Add(rowNum, ListRow);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    return myDic;
                }
            }
        }

    }


}
