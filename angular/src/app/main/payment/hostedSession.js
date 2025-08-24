

if (self === top) {
    var antiClickjack = document.getElementById("antiClickjack");
    if (antiClickjack) antiClickjack.parentNode.removeChild(antiClickjack);
} else {
    top.location = self.location;
}

PaymentSession.setFocus('card.number');

PaymentSession.setFocusStyle(["card.number","card.securityCode"], {
    borderColor: 'red',
    borderWidth: '3px'
});

PaymentSession.configure({
    fields: {
        // ATTACH HOSTED FIELDS TO YOUR PAYMENT PAGE FOR A CREDIT CARD
        card: {
            number: "#card-number",
            securityCode: "#security-code",
            expiryMonth: "#expiry-month",
            expiryYear: "#expiry-year"
        }
    },
    //SPECIFY YOUR MITIGATION OPTION HERE
    frameEmbeddingMitigation: ["javascript"],
    callbacks: {
        initialized: function (response) {
            // HANDLE INITIALIZATION RESPONSE
        },
        formSessionUpdate: function (response) {
            // HANDLE RESPONSE FOR UPDATE SESSION
            if (response.status) {
                if ("ok" == response.status) {

                    // Submit fields
                    var data = {
                        apiOperation: DotNetSample.operation(),
                        sessionId: response.session.id,
                        transactionId: $('#transaction-id').val(),
                        orderId: $('#order-id').val(),
                        orderAmount: $('#order-amount').val(),
                        orderCurrency: $('#order-currency').val(),
                        orderDescription: $('#order-description').val(),
                        secureIdResponseUrl: DotNetSample.secureIdResponseUrl()
                    };

                    var xhr = new XMLHttpRequest();
                    xhr.open('POST', DotNetSample.endpoint(), true);
                    xhr.setRequestHeader('Content-Type', 'application/json');
                    xhr.onreadystatechange = function() {
                        if (xhr.readyState == XMLHttpRequest.DONE) {
                            document.documentElement.innerHTML =this.response;
                        }
                    };
                    xhr.send(JSON.stringify(data));

                } else if ("fields_in_error" == response.status) {


                    if (response.errors.cardNumber) {
                        handleError("Card number missing or invalid.");
                    }
                    if (response.errors.expiryYear) {
                        handleError("Expiry year missing or invalid.");
                    }
                    if (response.errors.expiryMonth) {
                        handleError("Expiry month missing or invalid.");
                    }
                    if (response.errors.securityCode) {
                        handleError("Security code invalid.");
                    }
                } else if ("request_timeout" == response.status) {
                    handleError("Session update failed with request timeout: " + response.errors.message);
                } else if ("system_error" == response.status) {
                    handleError("Session update failed with system error: " + response.errors.message)
                }
            } else {
                handleError("Session update failed: " + response);
            }
        }
    }
});

function pay() {
    alert("I am an alert box!");
    $("#loading-bar-spinner").show();
    // UPDATE THE SESSION WITH THE INPUT FROM HOSTED FIELDS
    PaymentSession.updateSessionFromForm('card');
}

function test() {
    alert("I am an alert box!");
}

function handleError(message) {
    $("#loading-bar-spinner").hide();
    var $errorAlert = $('#error-alert');
    $errorAlert.append("<p>" + message + "</p>");
    $errorAlert.show();
}

