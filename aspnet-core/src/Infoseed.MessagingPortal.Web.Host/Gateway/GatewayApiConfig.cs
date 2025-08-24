using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Infoseed.MessagingPortal.Web.Host
{
    public class GatewayApiConfig
    {
        public static readonly String WEBHOOKS_NOTIFICATION_FOLDER = "webhooks-notifications";

        public Boolean Debug { get; set; }

        public Boolean UseSsl { get; set; }
        public Boolean IgnoreSslErrors { get; set; }

        //proxy configuration
        public Boolean UseProxy { get; set; }
        public String ProxyHost { get; set; }
        public String ProxyUser { get; set; }
        public String ProxyPassword { get; set; }
        public String ProxyDomain { get; set; }

        //backing fields - avoid get/set stackoverflow 
        private string _version;
        private string _gatewayUrl;
        private string _gatewayURLCerfificate;
        private string _currency;
        private string _merchantId;
        private string _password;
        private string _userName;
        private string _certificateLocation;
        private String _certificatePassword;

        //environment variables configuration
        public String Version {
            get
            {
                if (String.IsNullOrEmpty(_version) || !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("GATEWAY_VERSION")))
                {
                    _version = Environment.GetEnvironmentVariable("GATEWAY_VERSION");
                }
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        public String GatewayUrl { 
            get
            {
                if (String.IsNullOrEmpty(_gatewayUrl))
                   {
                        _gatewayUrl = Environment.GetEnvironmentVariable("GATEWAY_BASE_URL");
                   }
                return _gatewayUrl;
            }
            set
            {
                _gatewayUrl = value;
            }
        }

        public String GatewayUrlCertificate
        {
            get
            {
                if (String.IsNullOrEmpty(_gatewayURLCerfificate))
                {
                    _gatewayURLCerfificate = Environment.GetEnvironmentVariable("GATEWAY_CERT_HOST_URL");
                }
                return _gatewayURLCerfificate;
            }
            set
            {
                _gatewayURLCerfificate = value;
            }
        }

        public String Currency {
            get
            {

                if (String.IsNullOrEmpty(_currency) || !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("GATEWAY_CURRENCY")))
                {
                    _currency = Environment.GetEnvironmentVariable("GATEWAY_CURRENCY");
                }

                return _currency;
            }
            set
            {
                _currency = value;
            }
        }

        public String MerchantId {
            get {
                if (String.IsNullOrEmpty(_merchantId))
                {
                    _merchantId = Environment.GetEnvironmentVariable("GATEWAY_MERCHANT_ID")?.Trim();
                }
                return _merchantId;
            }
            set {
                _merchantId = value;
            }
        }

        public String Password {
            get
            {
                if (String.IsNullOrEmpty(_password) && !this.AuthenticationByCertificate)
                {
                    _password = Environment.GetEnvironmentVariable("GATEWAY_API_PASSWORD")?.Trim();
                }
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public String Username {             
            get
            {
                if (String.IsNullOrEmpty(_userName) )
                {
                    _userName = "merchant." + MerchantId;   
                }
                return _userName;
            }
            set
            {
                _userName = value;
            } 
        }




        //certificate configuration
        public String CertificateLocation { 
            get
            {
                if (String.IsNullOrEmpty(_certificateLocation) )
                {
                    _certificateLocation = Environment.GetEnvironmentVariable("KEYSTORE_PATH");
                }
                return _certificateLocation;
            }
            set
            {
                _certificateLocation = value;
            } 
        
        }
        public String CertificatePassword { 
            get
            {
                if (String.IsNullOrEmpty(_certificatePassword) )
                {
                    _certificatePassword = Environment.GetEnvironmentVariable("KEYSTORE_PASSWORD");
                }
                return _certificatePassword;
            }
            set
            {
                _certificatePassword = value;
            } 
        }


        public Boolean AuthenticationByCertificate
        {
            get
            {
                return CertificateLocation != null && CertificatePassword != null;
            }
        }




        //webhooks
        public String WebhooksNotificationSecret { get; set; }


    }
}