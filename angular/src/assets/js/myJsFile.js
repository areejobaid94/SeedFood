





      /*<![CDATA[*/
      //  var username = $("#userName").val();
        var BaseUrl=null;//"https://localhost:44301";
        var FrontEndUrl=null;//"https://localhost:4200";
        var merchantId =null;// "TEST9800078400";
        var sessionId = null;//"SESSION0002682103447N0788632K71";
        var sessionVersion =null;// "45";
        var successIndicator =null;// '{"Id":"SESSION0002682103447N0788632K71","Version":"a80819db01","SuccessIndicator":"babfb1eaee57456d"}';
        var orderId =null;// "c6cd1ea0a8";
        var currency = null;//"JOD";
        var Total = null;//2;
        var BillingId=null;//
    /*]]>*/
        var resultIndicator = null;
      //  var getdata=null;


      var orderDescription="Ordered goods";
    

    function setCard(dataCard) {
    
       // this.getdata=data;
       this.merchantId = dataCard.merchantId;
       this.sessionId =dataCard.sessionId;
       this.sessionVersion = dataCard.sessionVersion;
       this.successIndicator = dataCard.successIndicator.successIndicator;
       this.orderId = dataCard.orderId;
       this.currency = dataCard.currency;
      // var resultIndicator = null;
       
    }
    function setBillingId(dataBillingId) {
        //alert(dataBillingId);
        this.BillingId=dataBillingId;
    }
    function setTotal(dataTotal) {
        this.Total=dataTotal;
    }

    function setorderDescription(dataDescription) {
        this.orderDescription=dataDescription;
    }
    
    function setBaseUrl(dataBaseUrl) {
        this.BaseUrl = dataBaseUrl;
       
    }
    function setFrontEndUrl(dataFrontEndUrl) {
        this.FrontEndUrl = dataFrontEndUrl;  
    }
    
    // This method preserves the current state of successIndicator and orderId, so they're not overwritten when we return to this page after redirect
    function beforeRedirect() {
 
        return {
            
            successIndicator: successIndicator,
            orderId: orderId
        };
    }
    
    // This method is specifically for the full payment page option. Because we leave this page and return to it, we need to preserve the
    // state of successIndicator and orderId using the beforeRedirect/afterRedirect option
    function afterRedirect(data) {
        // Compare with the resultIndicator saved in the completeCallback() method
        if(resultIndicator === "CANCELED"){
            return;
        }    
        else if (resultIndicator) {
            var result = (resultIndicator === data.successIndicator) ? "SUCCESS" : "ERROR";
            httpGet(this.BaseUrl+"/api/HostedCheckout/hostedCheckout/" + data.orderId + "/" + result+ "/" +this.BillingId);
           // window.location.assign(  this.BaseUrl+"/api/HostedCheckout/hostedCheckout/" + data.orderId + "/" + result);

            window.location.href = this.FrontEndUrl+"/app/main/billings/billings" ;
        }
        else {
            successIndicator = data.successIndicator;
            orderId = data.orderId;
            sessionId = data.sessionId;
            sessionVersion = data.sessionVersion;
            merchantId = data.merchantId;
    
           // window.location.assign(this.BaseUrl+"/api/HostedCheckout/hostedCheckout/" + data.orderId + "/" + data.successIndicator + "/" + data.sessionId);
            httpGet(this.BaseUrl+"/api/HostedCheckout/hostedCheckout/" + data.orderId + "/" + data.successIndicator + "/" + data.sessionId+ "/" +this.BillingId);
            window.location.href = this.FrontEndUrl+"/app/main/billings/billings" ;
        }
    
    }

    function httpGet(theUrl)
{
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open( "GET", theUrl, false ); // false for synchronous request
    xmlHttp.send( null );
    return xmlHttp.responseText;
}
    
    function errorCallback(error) {
        var message = JSON.stringify(error);
        $("#loading-bar-spinner").hide();
        var $errorAlert = $('#error-alert');
        $errorAlert.append("<p>" + message + "</p>");
        $errorAlert.show();
    }
    function cancelCallback() {
        resultIndicator = "CANCELED" ;
        window.location.href = this.FrontEndUrl+"/app/main/billings/billings"
        // Reload the page to generate a new session ID - the old one is out of date as soon as the lightbox is invoked
        //window.location.reload(true);
    }
    
    // This handles the response from Hosted Checkout and redirects to the appropriate endpoint
    function completeCallback(response) {
        // Save the resultIndicator
        resultIndicator = response;
        var result = (resultIndicator === successIndicator) ? "SUCCESS" : "ERROR";
       // window.location.href = this.BaseUrl+"/api/HostedCheckout/hostedCheckout/" + orderId + "/" + result;
        httpGet(this.BaseUrl+"/api/HostedCheckout/hostedCheckout/" + orderId + "/" + result+ "/" +this.BillingId);
        window.location.href = this.FrontEndUrl+"/app/main/billings/billings" ;
        //window.location.href = "/app/main/dashboard" 
    }
   

    function configureF() {
        Checkout.configure({
            merchant: merchantId,
            order: {
                amount: function() {
                    //Dynamic calculation of amount
                    return Total;
                },
                currency: currency,
                description: orderDescription
            },
            session: {
                id: sessionId
            },
            interaction: {
                merchant: {
                    name: 'Info-seed',
                    address: {
                        line1: '125 Mecca Street / Amman',
                        line2: '125 Mecca Street / Amman'
                    }
                }
            }
        });
    
   
    
    }

    function pay() {

        Checkout.showPaymentPage();
    }