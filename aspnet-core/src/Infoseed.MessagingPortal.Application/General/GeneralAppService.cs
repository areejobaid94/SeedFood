using Abp.Web.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Framework.Data;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.General.Dto;
using Infoseed.MessagingPortal.InfoSeedParser;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppMediaResult;

namespace Infoseed.MessagingPortal.General
{
    public class GeneralAppService : MessagingPortalAppServiceBase, IGeneralAppService
    {
        private readonly BlobServiceClient _client;
        private readonly BlobContainerClient _blobClient;
        private readonly string containerName = ConfigurationManager.AppSettings["BlobStorageSubscription"].ToString();
        public GeneralAppService()
        {
            var connectionString = AppSettingsCoreModel.BlobServiceConnectionStrings;
            _client = new BlobServiceClient(connectionString);
            _blobClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> FindFileByName(string fileName, string type)
        {

            string _baseUri = AppSettingsCoreModel.StorageServiceURL + containerName;
            var blobs = new List<BlobItem>();
            await foreach (var page in _blobClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, fileName + type).AsPages())
            {
                blobs.AddRange(page.Values);
            }
            if (blobs.Count > 0)
            {
                var returnVal = _baseUri + "/" + blobs[0].Name;
                return returnVal;
            }
            return _baseUri + "/" + fileName + type

;
        }

        public LocationsPinned GetNearbyLocationsPinned(int tenantId, float latitude, float longitude)
        {
            return getNearbyLocationsPinned(tenantId, latitude, longitude);
        }
        //ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
        public GetLocationInfoTowModel GetNearbyLocations(int tenantId, float latitude, float longitude, int ParentId)
        {
            return getNearbyLocations(tenantId, latitude, longitude, ParentId);
        }
        public ContactDto GetContactbyId(int id)
        {
            return getContactbyId(id);
        }
        public void UpdateContactInfo(ContactDto contactDto)
        {
            updateContactInfo(contactDto);
        }
        public ContactDto CreateContact(ContactDto contactDto)
        {
            return createContact(contactDto);
        }
        public List<CountryInfoModel> GetAllCountryInfo()
        {
            List<CountryInfoModel> countryInfo = new List<CountryInfoModel>
            {
                new CountryInfoModel("Afghanistan","","93", "AF",1),
                new CountryInfoModel("Albania","","355", "AL",1),
                new CountryInfoModel("Algeria","","213", "DZ",2),
                new CountryInfoModel("American Samoa","","1-684", "AS",1),
                new CountryInfoModel("Andorra","","376", "AD",1),
                new CountryInfoModel("Angola","","244", "AO",1),
                new CountryInfoModel("Anguilla","","1-264", "AI",1),
                new CountryInfoModel("Antarctica","","672", "AQ",1),
                new CountryInfoModel("Antigua and Barbuda","","1-268", "AG",1),
                new CountryInfoModel("Argentina","","54", "AR",1),
                new CountryInfoModel("Armenia","","374", "AM",1),
                new CountryInfoModel("Aruba","","297", "AW",1),
                new CountryInfoModel("Australia","","61", "AU",1),
                new CountryInfoModel("Austria","","43", "AT",(decimal)1.5),
                new CountryInfoModel("Azerbaijan","","994", "AZ",1),
                new CountryInfoModel("Bahamas","","1-242", "BS",1),
                new CountryInfoModel("Bahrain","BHD","973", "BH",1),
                new CountryInfoModel("Bangladesh","","880", "BD",1),
                new CountryInfoModel("Barbados","","1-246", "BB",1),
                new CountryInfoModel("Belarus","","375", "BY",(decimal)1.5),
                new CountryInfoModel("Belgium","","32", "BE",2),
                new CountryInfoModel("Belize","","501", "BZ",1),
                new CountryInfoModel("Benin","","229", "BJ",1),
                new CountryInfoModel("Bermuda","","1-441", "BM",1),
                new CountryInfoModel("Bhutan","","975", "BT",1),
                new CountryInfoModel("Bolivia","","591", "BO",1),
                new CountryInfoModel("Bosnia and Herzegovina","","387", "BA",1),
                new CountryInfoModel("Botswana","","267", "BW",1),
                new CountryInfoModel("Brazil","","55", "BR",1),
                new CountryInfoModel("British Indian Ocean Territory","","246", "IO",1),
                new CountryInfoModel("British Virgin Islands","","1-284", "VG",1),
                new CountryInfoModel("Brunei","","673", "BN",1),
                new CountryInfoModel("Bulgaria","","359", "BG",(decimal)1.5),
                new CountryInfoModel("Burkina Faso","","226", "BF",1),
                new CountryInfoModel("Burundi","","257", "BI",1),
                new CountryInfoModel("Cambodia","","855", "KH",1),
                new CountryInfoModel("Cameroon","","237", "CM",1),
                new CountryInfoModel("Canada","","1", "CA",(decimal)0.5),
                new CountryInfoModel("Cape Verde","","238", "CV",1),
                new CountryInfoModel("Cayman Islands","","1-345", "KY",1),
                new CountryInfoModel("Central African Republic","","236", "CF",1),
                new CountryInfoModel("Chad","","235", "TD",1),
                new CountryInfoModel("Chile","","56", "CL",1),
                new CountryInfoModel("China","","86", "CN",1),
                new CountryInfoModel("Christmas Island","","61", "CX",1),
                new CountryInfoModel("Cocos Islands","","61", "CC",1),
                new CountryInfoModel("Colombia","","57", "CO",(decimal)0.5),
                new CountryInfoModel("Comoros","","269", "KM",1),
                new CountryInfoModel("Cook Islands","","682", "CK",1),
                new CountryInfoModel("Costa Rica","","506", "CR",1),
                new CountryInfoModel("Croatia","","385", "HR",(decimal)1.5),
                new CountryInfoModel("Cuba","","53", "CU",1),
                new CountryInfoModel("Curacao","","599", "CW",1),
                new CountryInfoModel("Cyprus","","357", "CY",1),
                new CountryInfoModel("Czech Republic","","420", "CZ",(decimal)1.5),
                new CountryInfoModel("Democratic Republic of the Congo","","243", "CD",1),
                new CountryInfoModel("Denmark","","45", "DK",2),
                new CountryInfoModel("Djibouti","","253", "DJ",1),
                new CountryInfoModel("Dominica","","1-767", "DM",1),
                new CountryInfoModel("Dominican Republic","","1-809", "DO",1),
                new CountryInfoModel("East Timor","","670", "TL",1),
                new CountryInfoModel("Ecuador","","593", "EC",1),
                new CountryInfoModel("Egypt","EGP","20", "EG",(decimal)1.5),
                new CountryInfoModel("El Salvador","","503", "SV",1),
                new CountryInfoModel("Equatorial Guinea","","240", "GQ",1),
                new CountryInfoModel("Eritrea","","291", "ER",1),
                new CountryInfoModel("Estonia","","372", "EE",(decimal)1.5),
                new CountryInfoModel("Ethiopia","","251", "ET",1),
                new CountryInfoModel("Falkland Islands","","500", "FK",1),
                new CountryInfoModel("Faroe Islands","","298", "FO",1),
                new CountryInfoModel("Fiji","","679", "FJ",1),
                new CountryInfoModel("Finland","","358", "FI",1),
                new CountryInfoModel("France","","33", "FR",2),
                new CountryInfoModel("French Polynesia","","689", "PF",1),
                new CountryInfoModel("Gabon","","241", "GA",1),
                new CountryInfoModel("Gambia","","220", "GM",1),
                new CountryInfoModel("Georgia","","995", "GE",1),
                new CountryInfoModel("Germany","","49", "DE",2),
                new CountryInfoModel("Ghana","","233", "GH",1),
                new CountryInfoModel("Gibraltar","","350", "GI",1),
                new CountryInfoModel("Greece","","30", "GR",1),
                new CountryInfoModel("Greenland","","299", "GL",(decimal)0.5),
                new CountryInfoModel("Grenada","","1-473", "GD",1),
                new CountryInfoModel("Guam","","1-671", "GU",1),
                new CountryInfoModel("Guatemala","","502", "GT",1),
                new CountryInfoModel("Guernsey","","44-1481", "GG",1),
                new CountryInfoModel("Guinea","","224", "GN",1),
                new CountryInfoModel("Guinea-Bissau","","245", "GW",1),
                new CountryInfoModel("Guyana","","592", "GY",1),
                new CountryInfoModel("Haiti","","509", "HT",1),
                new CountryInfoModel("Honduras","","504", "HN",1),
                new CountryInfoModel("Hong Kong","","852", "HK",1),
                new CountryInfoModel("Hungary","","36", "HU",(decimal)1.5),
                new CountryInfoModel("Iceland","","354", "IS",1),
                new CountryInfoModel("India","","91", "IN",(decimal)0.5),
                new CountryInfoModel("Indonesia","","62", "ID",(decimal)0.5),
                new CountryInfoModel("Iran","","98", "IR",1),
                new CountryInfoModel("Iraq","IQD","964", "IQ",1),
                new CountryInfoModel("Ireland","","353", "IE",1),
                new CountryInfoModel("Isle of Man","","44-1624", "IM",1),
                new CountryInfoModel("Israel","","972", "IL",(decimal)0.5),
                new CountryInfoModel("Italy","","39", "IT",1),
                new CountryInfoModel("Ivory Coast","","225", "CI",1),
                new CountryInfoModel("Jamaica","","1-876", "JM",1),
                new CountryInfoModel("Japan","","81", "JP",1),
                new CountryInfoModel("Jersey","","44-1534", "JE",1),
                new CountryInfoModel("Jordan","JOD","962", "JO",1),
                new CountryInfoModel("Kazakhstan","","7", "KZ",1),
                new CountryInfoModel("Kenya","","254", "KE",1),
                new CountryInfoModel("Kiribati","","686", "KI",1),
                new CountryInfoModel("Kosovo","","383", "XK",1),
                new CountryInfoModel("Kuwait","KWD","965", "KW",1),
                new CountryInfoModel("Kyrgyzstan","","996", "KG",1),
                new CountryInfoModel("Laos","","856", "LA",1),
                new CountryInfoModel("Latvia","","371", "LV",1),
                new CountryInfoModel("Lebanon","","961", "LB",1),
                new CountryInfoModel("Lesotho","","266", "LS",1),
                new CountryInfoModel("Liberia","","231", "LR",1),
                new CountryInfoModel("Libya","","218", "LY",2),
                new CountryInfoModel("Liechtenstein","","423", "LI",1),
                new CountryInfoModel("Lithuania","","370", "LT",1),
                new CountryInfoModel("Luxembourg","","352", "LU",1),
                new CountryInfoModel("Macau","","853", "MO",1),
                new CountryInfoModel("Macedonia","","389", "MK",1),
                new CountryInfoModel("Madagascar","","261", "MG",1),
                new CountryInfoModel("Malawi","","265", "MW",1),
                new CountryInfoModel("Malaysia","","60", "MY",1),
                new CountryInfoModel("Maldives","","960", "MV",1),
                new CountryInfoModel("Mali","","223", "ML",1),
                new CountryInfoModel("Malta","","356", "MT",1),
                new CountryInfoModel("Marshall Islands","","692", "MH",1),
                new CountryInfoModel("Mauritania","","222", "MR",1),
                new CountryInfoModel("Mauritius","","230", "MU",1),
                new CountryInfoModel("Mayotte","","262", "YT",1),
                new CountryInfoModel("Mexico","","52", "MX",(decimal)0.5),
                new CountryInfoModel("Micronesia","","691", "FM",1),
                new CountryInfoModel("Moldova","","373", "MD",(decimal)1.5),
                new CountryInfoModel("Monaco","","377", "MC",1),
                new CountryInfoModel("Mongolia","","976", "MN",1),
                new CountryInfoModel("Montenegro","","382", "ME",1),
                new CountryInfoModel("Montserrat","","1-664", "MS",1),
                new CountryInfoModel("Morocco","","212", "MA",1),
                new CountryInfoModel("Mozambique","","258", "MZ",1),
                new CountryInfoModel("Myanmar","","95", "MM",1),
                new CountryInfoModel("Namibia","","264", "NA",1),
                new CountryInfoModel("Nauru","","674", "NR",1),
                new CountryInfoModel("Nepal","","977", "NP",1),
                new CountryInfoModel("Netherlands","","31", "NL",1),
                new CountryInfoModel("Netherlands Antilles","","599", "AN",1),
                new CountryInfoModel("New Caledonia","","687", "NC",1),
                new CountryInfoModel("New Zealand","","64", "NZ",1),
                new CountryInfoModel("Nicaragua","","505", "NI",1),
                new CountryInfoModel("Niger","","227", "NE",1),
                new CountryInfoModel("Nigeria","","234", "NG",1),
                new CountryInfoModel("Niue","","683", "NU",1),
                new CountryInfoModel("North Korea","","850", "KP",1),
                new CountryInfoModel("Northern Mariana Islands","","1-670", "MP",1),
                new CountryInfoModel("Norway","","47", "NO",1),
                new CountryInfoModel("Oman","","968", "OM",1),
                new CountryInfoModel("Pakistan","","92", "PK",1),
                new CountryInfoModel("Palau","","680", "PW",1),
                new CountryInfoModel("Palestine","","970", "PS",1),
                new CountryInfoModel("Panama","","507", "PA",1),
                new CountryInfoModel("Papua New Guinea","","675", "PG",1),
                new CountryInfoModel("Paraguay","","595", "PY",1),
                new CountryInfoModel("Peru","","51", "PE",1),
                new CountryInfoModel("Philippines","","63", "PH",1),
                new CountryInfoModel("Pitcairn","","64", "PN",1),
                new CountryInfoModel("Poland","","48", "PL",(decimal)1.5),
                new CountryInfoModel("Portugal","","351", "PT",1),
                new CountryInfoModel("Puerto Rico","","1-787", "PR",1),
                new CountryInfoModel("Puerto Rico","","1-939", "PR",1),
                new CountryInfoModel("Qatar","QAR","974", "QA",1),
                new CountryInfoModel("Republic of the Congo","","242", "CG",1),
                new CountryInfoModel("Reunion","","262", "RE",1),
                new CountryInfoModel("Romania","","40", "RO",(decimal)1.5),
                new CountryInfoModel("Russia","","7", "RU",1),
                new CountryInfoModel("Rwanda","","250", "RW",1),
                new CountryInfoModel("Saint Barthelemy","","590", "BL",1),
                new CountryInfoModel("Saint Helena","","290", "SH",1),
                new CountryInfoModel("Saint Kitts and Nevis","","1-869", "KN",1),
                new CountryInfoModel("Saint Lucia","","1-758", "LC",1),
                new CountryInfoModel("Saint Martin","","590", "MF",1),
                new CountryInfoModel("Saint Pierre and Miquelon","","508", "PM",(decimal)0.5),
                new CountryInfoModel("Saint Vincent and the Grenadines","","1-784", "VC",1),
                new CountryInfoModel("Samoa","","685", "WS",1),
                new CountryInfoModel("San Marino","","378", "SM",1),
                new CountryInfoModel("Sao Tome and Principe","","239", "ST",1),
                new CountryInfoModel("Saudi Arabia","SAR","966", "SA",(decimal)0.5),
                new CountryInfoModel("Senegal","","221", "SN",1),
                new CountryInfoModel("Serbia","","381", "RS",1),
                new CountryInfoModel("Seychelles","","248", "SC",1),
                new CountryInfoModel("Sierra Leone","","232", "SL",1),
                new CountryInfoModel("Singapore","","65", "SG",1),
                new CountryInfoModel("Sint Maarten","","1-721", "SX",1),
                new CountryInfoModel("Slovakia","","421", "SK",(decimal)1.5),
                new CountryInfoModel("Slovenia","","386", "SI",1),
                new CountryInfoModel("Solomon Islands","","677", "SB",1),
                new CountryInfoModel("Somalia","","252", "SO",1),
                new CountryInfoModel("South Africa","","27", "ZA",(decimal)0.5),
                new CountryInfoModel("South Korea","","82", "KR",1),
                new CountryInfoModel("South Sudan","","211", "SS",2),
                new CountryInfoModel("Spain","","34", "ES",1),
                new CountryInfoModel("Sri Lanka","","94", "LK",1),
                new CountryInfoModel("Sudan","","249", "SD",2),
                new CountryInfoModel("Suriname","","597", "SR",1),
                new CountryInfoModel("Svalbard and Jan Mayen","","47", "SJ",1),
                new CountryInfoModel("Swaziland","","268", "SZ",1),
                new CountryInfoModel("Sweden","","46", "SE",1),
                new CountryInfoModel("Switzerland","","41", "CH",1),
                new CountryInfoModel("Syria","SYP","963", "SY",1),
                new CountryInfoModel("Taiwan","","886", "TW",1),
                new CountryInfoModel("Tajikistan","","992", "TJ",1),
                new CountryInfoModel("Tanzania","","255", "TZ",1),
                new CountryInfoModel("Thailand","","66", "TH",1),
                new CountryInfoModel("Togo","","228", "TG",1),
                new CountryInfoModel("Tokelau","","690", "TK",1),
                new CountryInfoModel("Tonga","","676", "TO",1),
                new CountryInfoModel("Trinidad and Tobago","","1-868", "TT",1),
                new CountryInfoModel("Tunisia","","216", "TN",2),
                new CountryInfoModel("Turkey","TRY","90", "TR",(decimal)0.5),
                new CountryInfoModel("Turkmenistan","","993", "TM",1),
                new CountryInfoModel("Turks and Caicos Islands","","1-649", "TC",1),
                new CountryInfoModel("Tuvalu","","688", "TV",1),
                new CountryInfoModel("U.S. Virgin Islands","","1-340", "VI",1),
                new CountryInfoModel("Uganda","","256", "UG",1),
                new CountryInfoModel("Ukraine","","380", "UA",(decimal)1.5),
                new CountryInfoModel("United Arab Emirates","","971", "AE",(decimal)0.5),
                new CountryInfoModel("United Kingdom","","44", "GB",1),
                new CountryInfoModel("United States","","1", "US",(decimal)0.5),
                new CountryInfoModel("Uruguay","","598", "UY",1),
                new CountryInfoModel("Uzbekistan","","998", "UZ",1),
                new CountryInfoModel("Vanuatu","","678", "VU",1),
                new CountryInfoModel("Vatican","","379", "VA",1),
                new CountryInfoModel("Venezuela","","58", "VE",1),
                new CountryInfoModel("Vietnam","","84", "VN",1),
                new CountryInfoModel("Wallis and Futuna","","681", "WF",1),
                new CountryInfoModel("Western Sahara","","212", "EH",1),
                new CountryInfoModel("Yemen","","967", "YE",1),
                new CountryInfoModel("Zambia","","260", "ZM",1),
                new CountryInfoModel("Zimbabwe","","263", "ZW",1),
            };
            return countryInfo;

        }
        public CountryInfoModel GetCountryInfo(string countryISO)
        {
            List<CountryInfoModel> countryInfo = new List<CountryInfoModel>
            {
                new CountryInfoModel("Afghanistan","","93", "AF",1),
                new CountryInfoModel("Albania","","355", "AL",1),
                new CountryInfoModel("Algeria","","213", "DZ",2),
                new CountryInfoModel("American Samoa","","1-684", "AS",1),
                new CountryInfoModel("Andorra","","376", "AD",1),
                new CountryInfoModel("Angola","","244", "AO",1),
                new CountryInfoModel("Anguilla","","1-264", "AI",1),
                new CountryInfoModel("Antarctica","","672", "AQ",1),
                new CountryInfoModel("Antigua and Barbuda","","1-268", "AG",1),
                new CountryInfoModel("Argentina","","54", "AR",1),
                new CountryInfoModel("Armenia","","374", "AM",1),
                new CountryInfoModel("Aruba","","297", "AW",1),
                new CountryInfoModel("Australia","","61", "AU",1),
                new CountryInfoModel("Austria","","43", "AT",(decimal)1.5),
                new CountryInfoModel("Azerbaijan","","994", "AZ",1),
                new CountryInfoModel("Bahamas","","1-242", "BS",1),
                new CountryInfoModel("Bahrain","BHD","973", "BH",1),
                new CountryInfoModel("Bangladesh","","880", "BD",1),
                new CountryInfoModel("Barbados","","1-246", "BB",1),
                new CountryInfoModel("Belarus","","375", "BY",(decimal)1.5),
                new CountryInfoModel("Belgium","","32", "BE",2),
                new CountryInfoModel("Belize","","501", "BZ",1),
                new CountryInfoModel("Benin","","229", "BJ",1),
                new CountryInfoModel("Bermuda","","1-441", "BM",1),
                new CountryInfoModel("Bhutan","","975", "BT",1),
                new CountryInfoModel("Bolivia","","591", "BO",1),
                new CountryInfoModel("Bosnia and Herzegovina","","387", "BA",1),
                new CountryInfoModel("Botswana","","267", "BW",1),
                new CountryInfoModel("Brazil","","55", "BR",1),
                new CountryInfoModel("British Indian Ocean Territory","","246", "IO",1),
                new CountryInfoModel("British Virgin Islands","","1-284", "VG",1),
                new CountryInfoModel("Brunei","","673", "BN",1),
                new CountryInfoModel("Bulgaria","","359", "BG",(decimal)1.5),
                new CountryInfoModel("Burkina Faso","","226", "BF",1),
                new CountryInfoModel("Burundi","","257", "BI",1),
                new CountryInfoModel("Cambodia","","855", "KH",1),
                new CountryInfoModel("Cameroon","","237", "CM",1),
                new CountryInfoModel("Canada","","1", "CA",(decimal)0.5),
                new CountryInfoModel("Cape Verde","","238", "CV",1),
                new CountryInfoModel("Cayman Islands","","1-345", "KY",1),
                new CountryInfoModel("Central African Republic","","236", "CF",1),
                new CountryInfoModel("Chad","","235", "TD",1),
                new CountryInfoModel("Chile","","56", "CL",1),
                new CountryInfoModel("China","","86", "CN",1),
                new CountryInfoModel("Christmas Island","","61", "CX",1),
                new CountryInfoModel("Cocos Islands","","61", "CC",1),
                new CountryInfoModel("Colombia","","57", "CO",(decimal)0.5),
                new CountryInfoModel("Comoros","","269", "KM",1),
                new CountryInfoModel("Cook Islands","","682", "CK",1),
                new CountryInfoModel("Costa Rica","","506", "CR",1),
                new CountryInfoModel("Croatia","","385", "HR",(decimal)1.5),
                new CountryInfoModel("Cuba","","53", "CU",1),
                new CountryInfoModel("Curacao","","599", "CW",1),
                new CountryInfoModel("Cyprus","","357", "CY",1),
                new CountryInfoModel("Czech Republic","","420", "CZ",(decimal)1.5),
                new CountryInfoModel("Democratic Republic of the Congo","","243", "CD",1),
                new CountryInfoModel("Denmark","","45", "DK",2),
                new CountryInfoModel("Djibouti","","253", "DJ",1),
                new CountryInfoModel("Dominica","","1-767", "DM",1),
                new CountryInfoModel("Dominican Republic","","1-809", "DO",1),
                new CountryInfoModel("East Timor","","670", "TL",1),
                new CountryInfoModel("Ecuador","","593", "EC",1),
                new CountryInfoModel("Egypt","EGP","20", "EG",(decimal)1.5),
                new CountryInfoModel("El Salvador","","503", "SV",1),
                new CountryInfoModel("Equatorial Guinea","","240", "GQ",1),
                new CountryInfoModel("Eritrea","","291", "ER",1),
                new CountryInfoModel("Estonia","","372", "EE",(decimal)1.5),
                new CountryInfoModel("Ethiopia","","251", "ET",1),
                new CountryInfoModel("Falkland Islands","","500", "FK",1),
                new CountryInfoModel("Faroe Islands","","298", "FO",1),
                new CountryInfoModel("Fiji","","679", "FJ",1),
                new CountryInfoModel("Finland","","358", "FI",1),
                new CountryInfoModel("France","","33", "FR",2),
                new CountryInfoModel("French Polynesia","","689", "PF",1),
                new CountryInfoModel("Gabon","","241", "GA",1),
                new CountryInfoModel("Gambia","","220", "GM",1),
                new CountryInfoModel("Georgia","","995", "GE",1),
                new CountryInfoModel("Germany","","49", "DE",2),
                new CountryInfoModel("Ghana","","233", "GH",1),
                new CountryInfoModel("Gibraltar","","350", "GI",1),
                new CountryInfoModel("Greece","","30", "GR",1),
                new CountryInfoModel("Greenland","","299", "GL",(decimal)0.5),
                new CountryInfoModel("Grenada","","1-473", "GD",1),
                new CountryInfoModel("Guam","","1-671", "GU",1),
                new CountryInfoModel("Guatemala","","502", "GT",1),
                new CountryInfoModel("Guernsey","","44-1481", "GG",1),
                new CountryInfoModel("Guinea","","224", "GN",1),
                new CountryInfoModel("Guinea-Bissau","","245", "GW",1),
                new CountryInfoModel("Guyana","","592", "GY",1),
                new CountryInfoModel("Haiti","","509", "HT",1),
                new CountryInfoModel("Honduras","","504", "HN",1),
                new CountryInfoModel("Hong Kong","","852", "HK",1),
                new CountryInfoModel("Hungary","","36", "HU",(decimal)1.5),
                new CountryInfoModel("Iceland","","354", "IS",1),
                new CountryInfoModel("India","","91", "IN",(decimal)0.5),
                new CountryInfoModel("Indonesia","","62", "ID",(decimal)0.5),
                new CountryInfoModel("Iran","","98", "IR",1),
                new CountryInfoModel("Iraq","IQD","964", "IQ",1),
                new CountryInfoModel("Ireland","","353", "IE",1),
                new CountryInfoModel("Isle of Man","","44-1624", "IM",1),
                new CountryInfoModel("Israel","","972", "IL",(decimal)0.5),
                new CountryInfoModel("Italy","","39", "IT",1),
                new CountryInfoModel("Ivory Coast","","225", "CI",1),
                new CountryInfoModel("Jamaica","","1-876", "JM",1),
                new CountryInfoModel("Japan","","81", "JP",1),
                new CountryInfoModel("Jersey","","44-1534", "JE",1),
                new CountryInfoModel("Jordan","JOD","962", "JO",1),
                new CountryInfoModel("Kazakhstan","","7", "KZ",1),
                new CountryInfoModel("Kenya","","254", "KE",1),
                new CountryInfoModel("Kiribati","","686", "KI",1),
                new CountryInfoModel("Kosovo","","383", "XK",1),
                new CountryInfoModel("Kuwait","KWD","965", "KW",1),
                new CountryInfoModel("Kyrgyzstan","","996", "KG",1),
                new CountryInfoModel("Laos","","856", "LA",1),
                new CountryInfoModel("Latvia","","371", "LV",1),
                new CountryInfoModel("Lebanon","","961", "LB",1),
                new CountryInfoModel("Lesotho","","266", "LS",1),
                new CountryInfoModel("Liberia","","231", "LR",1),
                new CountryInfoModel("Libya","","218", "LY",2),
                new CountryInfoModel("Liechtenstein","","423", "LI",1),
                new CountryInfoModel("Lithuania","","370", "LT",1),
                new CountryInfoModel("Luxembourg","","352", "LU",1),
                new CountryInfoModel("Macau","","853", "MO",1),
                new CountryInfoModel("Macedonia","","389", "MK",1),
                new CountryInfoModel("Madagascar","","261", "MG",1),
                new CountryInfoModel("Malawi","","265", "MW",1),
                new CountryInfoModel("Malaysia","","60", "MY",1),
                new CountryInfoModel("Maldives","","960", "MV",1),
                new CountryInfoModel("Mali","","223", "ML",1),
                new CountryInfoModel("Malta","","356", "MT",1),
                new CountryInfoModel("Marshall Islands","","692", "MH",1),
                new CountryInfoModel("Mauritania","","222", "MR",1),
                new CountryInfoModel("Mauritius","","230", "MU",1),
                new CountryInfoModel("Mayotte","","262", "YT",1),
                new CountryInfoModel("Mexico","","52", "MX",(decimal)0.5),
                new CountryInfoModel("Micronesia","","691", "FM",1),
                new CountryInfoModel("Moldova","","373", "MD",(decimal)1.5),
                new CountryInfoModel("Monaco","","377", "MC",1),
                new CountryInfoModel("Mongolia","","976", "MN",1),
                new CountryInfoModel("Montenegro","","382", "ME",1),
                new CountryInfoModel("Montserrat","","1-664", "MS",1),
                new CountryInfoModel("Morocco","","212", "MA",1),
                new CountryInfoModel("Mozambique","","258", "MZ",1),
                new CountryInfoModel("Myanmar","","95", "MM",1),
                new CountryInfoModel("Namibia","","264", "NA",1),
                new CountryInfoModel("Nauru","","674", "NR",1),
                new CountryInfoModel("Nepal","","977", "NP",1),
                new CountryInfoModel("Netherlands","","31", "NL",1),
                new CountryInfoModel("Netherlands Antilles","","599", "AN",1),
                new CountryInfoModel("New Caledonia","","687", "NC",1),
                new CountryInfoModel("New Zealand","","64", "NZ",1),
                new CountryInfoModel("Nicaragua","","505", "NI",1),
                new CountryInfoModel("Niger","","227", "NE",1),
                new CountryInfoModel("Nigeria","","234", "NG",1),
                new CountryInfoModel("Niue","","683", "NU",1),
                new CountryInfoModel("North Korea","","850", "KP",1),
                new CountryInfoModel("Northern Mariana Islands","","1-670", "MP",1),
                new CountryInfoModel("Norway","","47", "NO",1),
                new CountryInfoModel("Oman","","968", "OM",1),
                new CountryInfoModel("Pakistan","","92", "PK",1),
                new CountryInfoModel("Palau","","680", "PW",1),
                new CountryInfoModel("Palestine","","970", "PS",1),
                new CountryInfoModel("Panama","","507", "PA",1),
                new CountryInfoModel("Papua New Guinea","","675", "PG",1),
                new CountryInfoModel("Paraguay","","595", "PY",1),
                new CountryInfoModel("Peru","","51", "PE",1),
                new CountryInfoModel("Philippines","","63", "PH",1),
                new CountryInfoModel("Pitcairn","","64", "PN",1),
                new CountryInfoModel("Poland","","48", "PL",(decimal)1.5),
                new CountryInfoModel("Portugal","","351", "PT",1),
                new CountryInfoModel("Puerto Rico","","1-787", "PR",1),
                new CountryInfoModel("Puerto Rico","","1-939", "PR",1),
                new CountryInfoModel("Qatar","QAR","974", "QA",1),
                new CountryInfoModel("Republic of the Congo","","242", "CG",1),
                new CountryInfoModel("Reunion","","262", "RE",1),
                new CountryInfoModel("Romania","","40", "RO",(decimal)1.5),
                new CountryInfoModel("Russia","","7", "RU",1),
                new CountryInfoModel("Rwanda","","250", "RW",1),
                new CountryInfoModel("Saint Barthelemy","","590", "BL",1),
                new CountryInfoModel("Saint Helena","","290", "SH",1),
                new CountryInfoModel("Saint Kitts and Nevis","","1-869", "KN",1),
                new CountryInfoModel("Saint Lucia","","1-758", "LC",1),
                new CountryInfoModel("Saint Martin","","590", "MF",1),
                new CountryInfoModel("Saint Pierre and Miquelon","","508", "PM",(decimal)0.5),
                new CountryInfoModel("Saint Vincent and the Grenadines","","1-784", "VC",1),
                new CountryInfoModel("Samoa","","685", "WS",1),
                new CountryInfoModel("San Marino","","378", "SM",1),
                new CountryInfoModel("Sao Tome and Principe","","239", "ST",1),
                new CountryInfoModel("Saudi Arabia","SAR","966", "SA",(decimal)0.5),
                new CountryInfoModel("Senegal","","221", "SN",1),
                new CountryInfoModel("Serbia","","381", "RS",1),
                new CountryInfoModel("Seychelles","","248", "SC",1),
                new CountryInfoModel("Sierra Leone","","232", "SL",1),
                new CountryInfoModel("Singapore","","65", "SG",1),
                new CountryInfoModel("Sint Maarten","","1-721", "SX",1),
                new CountryInfoModel("Slovakia","","421", "SK",(decimal)1.5),
                new CountryInfoModel("Slovenia","","386", "SI",1),
                new CountryInfoModel("Solomon Islands","","677", "SB",1),
                new CountryInfoModel("Somalia","","252", "SO",1),
                new CountryInfoModel("South Africa","","27", "ZA",(decimal)0.5),
                new CountryInfoModel("South Korea","","82", "KR",1),
                new CountryInfoModel("South Sudan","","211", "SS",2),
                new CountryInfoModel("Spain","","34", "ES",1),
                new CountryInfoModel("Sri Lanka","","94", "LK",1),
                new CountryInfoModel("Sudan","","249", "SD",2),
                new CountryInfoModel("Suriname","","597", "SR",1),
                new CountryInfoModel("Svalbard and Jan Mayen","","47", "SJ",1),
                new CountryInfoModel("Swaziland","","268", "SZ",1),
                new CountryInfoModel("Sweden","","46", "SE",1),
                new CountryInfoModel("Switzerland","","41", "CH",1),
                new CountryInfoModel("Syria","SYP","963", "SY",1),
                new CountryInfoModel("Taiwan","","886", "TW",1),
                new CountryInfoModel("Tajikistan","","992", "TJ",1),
                new CountryInfoModel("Tanzania","","255", "TZ",1),
                new CountryInfoModel("Thailand","","66", "TH",1),
                new CountryInfoModel("Togo","","228", "TG",1),
                new CountryInfoModel("Tokelau","","690", "TK",1),
                new CountryInfoModel("Tonga","","676", "TO",1),
                new CountryInfoModel("Trinidad and Tobago","","1-868", "TT",1),
                new CountryInfoModel("Tunisia","","216", "TN",2),
                new CountryInfoModel("Turkey","TRY","90", "TR",(decimal)0.5),
                new CountryInfoModel("Turkmenistan","","993", "TM",1),
                new CountryInfoModel("Turks and Caicos Islands","","1-649", "TC",1),
                new CountryInfoModel("Tuvalu","","688", "TV",1),
                new CountryInfoModel("U.S. Virgin Islands","","1-340", "VI",1),
                new CountryInfoModel("Uganda","","256", "UG",1),
                new CountryInfoModel("Ukraine","","380", "UA",(decimal)1.5),
                new CountryInfoModel("United Arab Emirates","","971", "AE",(decimal)0.5),
                new CountryInfoModel("United Kingdom","","44", "GB",1),
                new CountryInfoModel("United States","","1", "US",(decimal)0.5),
                new CountryInfoModel("Uruguay","","598", "UY",1),
                new CountryInfoModel("Uzbekistan","","998", "UZ",1),
                new CountryInfoModel("Vanuatu","","678", "VU",1),
                new CountryInfoModel("Vatican","","379", "VA",1),
                new CountryInfoModel("Venezuela","","58", "VE",1),
                new CountryInfoModel("Vietnam","","84", "VN",1),
                new CountryInfoModel("Wallis and Futuna","","681", "WF",1),
                new CountryInfoModel("Western Sahara","","212", "EH",1),
                new CountryInfoModel("Yemen","","967", "YE",1),
                new CountryInfoModel("Zambia","","260", "ZM",1),
                new CountryInfoModel("Zimbabwe","","263", "ZW",1),
            };
            CountryInfoModel result = countryInfo.Where(x => countryISO.ToUpper().StartsWith(x.Iso)).FirstOrDefault();
            return result;

        }

        public TenantsModel GetTenantById(int tenantId)
        {
            return getTenantById(tenantId);
        }

        public List<BotTemplatesModel> GetBotTemplates()
        {
            return getBotTemplates();
        }

        [DontWrapResult]
        [HttpPost]
        public Task<WhatsAppHeaderUrl> GetInfoSeedUrlFile([FromForm] UploadFileModel file)
        {
            return getInfoSeedUrlFile(file);
        }


        #region Contact Private Methods
        private void updateContactInfo(ContactDto contactDto)
        {
            try
            {
                //var contact = getContactbyId()
                var SP_Name = Constants.Contacts.SP_ContactInfoUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactInfoJson",JsonConvert.SerializeObject(contactDto))

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ContactDto createContact(ContactDto contactDto)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactInfoJson",JsonConvert.SerializeObject(contactDto))
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@ContactId";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);
                contactDto   = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.MapContact, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return contactDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        private ContactDto getContactbyId(int id)
        {
            try
            {
                ContactDto contactDto = new ContactDto();
                var SP_Name = Constants.Contacts.SP_ContactbyIdGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
               new System.Data.SqlClient.SqlParameter("@Id",id)
                 };

                contactDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.MapContact, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return contactDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Tenant Private Methods

        private TenantsModel getTenantById(int tenantId)
        {
            try
            {
                TenantsModel tenant = new TenantsModel();
                var SP_Name = Constants.Tenant.SP_TenantByIdGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                };

                tenant = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTenant, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return tenant;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<WhatsAppHeaderUrl> getInfoSeedUrlFile([FromForm] UploadFileModel model)
        {
            WhatsAppHeaderUrl whatsAppHeaderUrl = new WhatsAppHeaderUrl();
            
            if (model.FormFile != null)
            {
                if (model.FormFile.Length > 0)
                {
                    var formFile = model.FormFile;
                    long ContentLength = formFile.Length;
                    byte[] fileData = null;
                    using (var ms = new MemoryStream())
                    {
                        formFile.CopyTo(ms);
                        fileData = ms.ToArray();
                    }

                    AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                    AttachmentContent attachmentContent = new AttachmentContent()
                    {
                        Content = fileData,
                        Extension = Path.GetExtension(formFile.FileName),
                        MimeType = formFile.ContentType,
                        fileName=formFile.FileName.Replace(Path.GetExtension(formFile.FileName), "")

                    };

                    whatsAppHeaderUrl.InfoSeedUrl = await azureBlobProvider.Save(attachmentContent);

                }



            }

            return whatsAppHeaderUrl;
        }

        #endregion


        #region Location Private Methods

        private LocationsPinned getNearbyLocationsPinned(int tenantId, float latitude, float longitude)
        {
            try
            {
                var SP_Name = Constants.LocationsPinned.SP_LocationsPinnedNearbyGet;

                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@TenantId",tenantId)
                ,new SqlParameter("@Latitude",latitude)
                ,new SqlParameter("@Longitude",longitude)
                };


                LocationsPinned locationsPinned =
                    SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertToLocationsPinnedDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                return locationsPinned;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private GetLocationInfoTowModel getNearbyLocations(int tenantId, float latitude, float longitude, int ParentId)
        {
            try
            {
                var SP_Name = Constants.LocationsPinned.SP_LocationsNearbyGet;

                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@TenantId",tenantId)
                ,new SqlParameter("@Latitude",latitude)
                ,new SqlParameter("@Longitude",longitude)
                ,new SqlParameter("@ParentId",ParentId)
                };


                GetLocationInfoTowModel locationsPinned =
                    SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertToLocationsP, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                return locationsPinned;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Bot Private Methods
        private List<BotTemplatesModel> getBotTemplates()
        {
            try
            {
                List<BotTemplatesModel> templates = new List<BotTemplatesModel>();
                var SP_Name = Constants.Bot.SP_BotTemplatesGet;

                var sqlParameters = new List<SqlParameter>
                {
                };

                templates = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBotTemplates, AppSettingsModel.ConnectionStrings).ToList();

                return templates;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}
