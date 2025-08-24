

$(document).ready(function () {
    "use strict";
    document.documentElement.style.setProperty('--primary-color', '#389475');


 

    FillTenantInfo();


    $('select').select2();

    //alert();
    // ===========Tooltip============
    $('[data-toggle="tooltip"]').tooltip()
    //defaultScript() 

    var tenantId = $('#TenantID').val();
    if (tenantId != 0) {
        var idcat = $('.category-item').data('id');
        $('#s_' + idcat).addClass('active');

        var lst = $('#s_' + idcat).data('subcategory');


        subCategoryselection(idcat);


        var p = 0;

    }




});

var scrollTimer = -1;

window.addEventListener('scroll', (event) => {
    scrollFunction();
    var e = $(".nn");

    var n;
    var nn;
    var t = 70;

    for (var i = 0; i < e.length; i++) {
        $(e[i]).offset().top - 150 < $(window).scrollTop() && (n = $(e[i]).attr("id"));


        $(".swiper-slide").removeClass("active");
        //  $("#s_" + n).length && ($("#s_" + n).addClass("active"));
        $("#s_" + n).addClass("active");

        scrollto(n);

    }

    if (scrollTimer != -1) 
        clearTimeout(scrollTimer);


 scrollTimer = window.setTimeout("pagenation()", 800);

    


});
//function ff() {
//    document.getElementById('md-productDetails').style.zoom = "100%";
//    document.body.style.zoom = "100%";
//}
//#region Languages

$(document).on("click", ".btn-change-lang", function (e) {


    var url = "/Index?&handler=SetLanguage";
    var lang = $(this).data('lang');

    var rout = $('#culture').data('rout');
    $.ajax({
        type: "POST",
        url: url,
        data: { culture: lang },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },

        success: function () {
            location.reload();
        },
        error: function () {
            alert("there was an error");
        }
    });




});


function SetLanguage() {



    //var url = "/Index?culture=" + x + "&returnUrl=" + rout + "&handler=SetLanguage";



}

//#end region



function goto(id) {
    x = document.getElementById(id);
    x.scrollIntoView({
        behavior: "smooth",
        block: "start",
        inline: "nearest"
    });


    console.log(id);
}

function scrollFunction() {

    var mybutton = document.getElementById("myBtn");
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        mybutton.style.display = "block";

    } else {
        mybutton.style.display = "none";
    }



}

$(document).on("click", ".myBtn", function () {
    document.body.scrollIntoView({ behavior: "smooth", block: "start", inline: "nearest" });
    //  document.documentElement.scrollIntoView({ behavior: "smooth", block: "start", inline: "nearest" });

});


$(document).on("click", ".flip-it", function () {
    $(this).toggleClass('fliped');

});


//#region pagenation  


function pagenation() {
    var Id = $('#hdSubCatId').val();
    var Idd = $('.div-subcategory-item.active').data('id');

    if (Idd != undefined) {

             scrollto2(Idd);

      }
    //var x1 = window.scrollY + 1000 ;
    //var heightdiv = document.getElementById("divSubItem");
    //if (heightdiv != null) {

    //    if (x1 == heightdiv.offsetHeight) {
    //        console.log(true)
          
    //        if (Idd != undefined) {

    //            scrollto2(Idd);

    //        }
    //    }
    //}
}


//#end region
//#region Advanced Menu Category and subCat and Items
function SubCateItem(newId, isNew, IsSort) {


   
    //var Id = $('#hdSubCatId').val();
    //debugger


    var isLoyaltyApplay = $('#IsLoyalty').val();
    if (isLoyaltyApplay == "True") {
        var loyaltyModel = localStorage.getItem('TenantInfo');

    }
    var search = $('#anythingSearch').val();
    if (search == undefined) {
        search = "";
    }


    if (isNew === true) {

        $('.div-subcategory-item').each(function () {
            $(this).removeClass('active');
        });
        $("#div-subcategory_" + newId).addClass('active');

    }


    var selectedsubcategory = "#div-subcategory_" + newId

    if (isNew === true) {
        $(selectedsubcategory).data('pagenumber', '0')
        $(selectedsubcategory).data('isAllGet', false)
    }
    
    var pageNumber = parseInt($(selectedsubcategory).data('pagenumber'));
    //if (id != Id) {
    //    $('#divSubItem').html('');
    //    var pageNumber = 0;

    //} else {

    //}

    if ($(selectedsubcategory).data('isAllGet') != "true") {


        var subCateoryname = $('#div-subcategory_' + newId).data('subname');


        var url = "/Index?tenantID=" + parseInt($('#TenantID').val()) + "&isLoyaltyApplay=" + isLoyaltyApplay + "&subCateoryname=" + subCateoryname + "&currencyCode=" + $('#CurrencyCode').val() + "&areaId=" + parseInt($('#AreaId').val()) + "&PageNumber=" + pageNumber + "&loyaltyModelJson=" + loyaltyModel + "&subCategoryid=" + newId + "&search=" + search + "&IsSort=" + IsSort + "&handler=SubCategoriesandItems";


        $.get(url, function (result) {


            if (isNew === true && pageNumber === 0) {
                $('#divSubItem').html('');
                //  document.body.scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                // $("#divSubItem").scrollTop() - $("#divTenantInfo").height();



            }

            if (result.statusCode == 0) {

                var xne = $('#div-subcategory_' + newId).next().data('id');
                if (xne != undefined) {
                    if (xne != newId && newId != 0) {
                        SubCateItem(xne, false);
                    } else {
                        $(selectedsubcategory).data('isAllGet', "true")
                    }

                } else {
                    $(selectedsubcategory).data('isAllGet', "true")
                }

            }
            else {
                // alert(result)
                if (result != null) {
                    $('.div-subcategory-item').each(function () {
                        $(this).removeClass('active');
                    });
                    $("#div-subcategory_" + newId).addClass('active');

                    $('#divSubItem').append(result);
                    pageNumber = parseInt($(selectedsubcategory).data('pagenumber')) + 1;
                    $(selectedsubcategory).data('pagenumber', pageNumber)
                    if (isNew === true) {
                        var height = $("#divTenantInfo").height();

                        if (window.scrollY >= height) {
                            //   alert("")
                            window.scrollTo({
                                top: height,
                          
                            });
                        }
                    }

                }
            }
        
        });


    }
    //scrollto2(id);

    //$(document).on("click", ".subcategory-item", function () {





}

$(document).on("click", ".div-category-item", function () {
    //  debugger;
    // alert('test');
    var id = $(this).data('id');
    var subCategory = $(this).data('subcategory');
    filterSelection(id, subCategory);

})

function filterSelection(c, sublist) {


    var objItems = JSON.stringify(sublist);



    var url = "/Index?&handler=SubList";
    if (objItems != null) {
        //  alert(objItems)
        spCartCount = JSON.parse(objItems).length;
    }
    document.getElementById("divSubcategorys").innerHTML = "";

    $.ajax({
        dataType: 'json',
        url: url,
        type: 'Post',
        data: { ItemsJson: objItems },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            //dealing with the data returned by the request


            document.getElementById("divSubcategorys").innerHTML = data;



            subCategoryselection(c);






        },
        error: function () {
            //     alert("AJAX Request Failed, Contact Support");

        }




    });









}
function subCategoryselection(c) {

    var idsub = $('.show').data('id');


    $('.div-category-item').removeClass('active');
    $('#s_' + c).addClass('active');






    SubCateItem(idsub, true);
}

function GoToNext(id) {
    var divs = document.getElementsByClassName("content");
    //divs now contain each and every div element on the pag
    var length = divs.length - 1;
    if (id != 0) {
        var nextDiv = divs[id];
    } else {

        var nextDiv = divs[length];
    }

    nextDiv.scrollIntoView({ behavior: "smooth", block: "start", })

}


function addToCartdirc(id) {

    $("#qty_" + id).removeClass("invisible");

}
$(document).on("click", ".minus1", function () {

    var $input = $(this).parent().find('input');


    if (parseInt($input.val()) <= 1) {

        $input.val(1);

    } else {
        $input.val(parseInt($input.val()) - 1);
    }
    $input.change();


    return false;
});


$(document).on("click", ".minus", function () {

    var $input = $(this).parent().find('input');

    if (parseInt($input.val()) <= 0) {
        $input.val(0);

    } else {
        $input.val(parseInt($input.val()) - 1);
    }
    $input.change();
    $("#carticon").addClass('btn2');



    return false;
});


$(document).on("click", ".plus", function () {

    var $input = $(this).parent().find('input');

    $input.val(parseInt($input.val()) + 1);
    $input.change();

    $("#carticon").addClass('btn2');
    $(".flip-it").removeClass('fliped');

    $('#carticon').on("animationend webkitAnimationEnd oAnimationEnd MSAnimationEnd", function () {
        $(this).removeClass("btn2");
    });

    return false;
});


$(document).on("keyup", ".hdItemQty", function () {
    
    //$(".error").css("display", "none");
    if (event.which !== 8 && event.which !== 0 && event.which < 48 || event.which > 57) {
        $(this).val(1);
    }
});

$(document).on("keyup", ".hdItemQtyAddtional", function () {
    //$(".error").css("display", "none");
    if (event.which !== 8 && event.which !== 0 && event.which < 48 || event.which > 57) {
        $(this).val(0);
    }
});
// plus

//end qty





// #region Header info

function FillTenantInfo() {
   // localStorage.setItem('TenantInfo', JSON.stringify(""));
     var model = $('#sec').data('model');
    

    if (model != undefined) {

        localStorage.setItem('TenantInfo', JSON.stringify(""));

        localStorage.setItem('TenantInfo', JSON.stringify(model));
    } else {



    }
 

    var objTenantInfo = localStorage.getItem('TenantInfo');

    var lang = localStorage.getItem('i18nextLng');
    var url = "/Index?lst=" + objTenantInfo + "&handler=TenantInfo";
    $.get(url, function (result) {
        $('#divTenantInfo').html('');
        $('#divTenantInfo').html(result);

        defaultScript();


        calculateTotal();

    });

}

function FillContactInfo() {

    var url = "/Index?contactId=" + $('#ContactId').val() + "&handler=ContactInfo";
    $.get(url, function (result) {
        $('#loyaltySection').html('');
        $('#loyaltySection').html(result);


    });

}





$(document).on("click", ".btn-order-detials", function () {

    var id = $(this).data('id');
    var status = $(this).data('status');
    var url = "/Orders?tenantId=" + $('#TenantID').val() + "&orderid=" + id + "&orderstatus=" + status + "&handler=OrderDetails";
    $.get(url, function (result) {

        $('#divOrderItems').html('');
        $('#divOrderItems').html(result);
        $('#md-OrderItems').modal('show');

   


    })
});

// #endregion


// #region fill product details

$(document).on("click", ".btn-product-detials", function () {
    var loyaltyModel = localStorage.getItem('TenantInfo');
    var id = $(this).data('id');
    var isLoyaltyApplay = $('#IsLoyalty').val();
  

    var url = "/Index?id=" + id + "&tenantId=" + $('#TenantID').val() + "&isLoyaltyApplay=" + isLoyaltyApplay + "&loyaltyModelJson=" + loyaltyModel+ "&subcatId=" + $('#hdSubCatId').val() + "&handler=ProuductDetails";
    $.get(url, function (result) {

        $('#divProductDetails').html('');
        $('#divProductDetails').html(result);
        $('#md-productDetails').modal('show');

        $('#errormsg').hide();
    
        getItemSelectItem();
    })
});


//#endregion



//#region cart


$(document).on('click', '[data-toggle="offcanvas"]', function () {
    $('#cartFooter').hide();
    $('body').removeClass('toggled');
    calculateTotal()
});

var sideBarOpen;

$(document).on('click', '.cart-sidebar', function () {
    // openCart();
    sideBarOpen = true;
});

$(document).on('click', '.toggled', function () {

    if (sideBarOpen) {
        //  openCart();
    } else {

        closeCart();

    }
    sideBarOpen = false;

});




//$(document).click(function (event) {
//    console.log(event);
//    var target = $(event.target);

//    if (target === $('.cart-sidebar')) {
//        openCart();
//    } else {
//        if (sideBarOpen) {
//            closeCart();
//        }
//    }
//});



//$sidebar = $('.cart-sidebar');
//$("body").click(function (event) {
//    var a = event.target;
//    if (a === $sidebar) {
//        openCart();
//        //close the sidebar
//        //you may also want to test if it is actually open before calling the close function.
//    } else {
//        closeCart()
//    }
//});


$(document).on('click', '.acart-open', function () {
    var objItems = localStorage.getItem('itemInfo');

    var loyaltyModel = localStorage.getItem('TenantInfo');
    var x = $('#IsLoyalty').val()

    var spCartCount = 0;
    var url = "/Index?&handler=CartItem";
    if (objItems != null) {
        spCartCount = JSON.parse(objItems).length;
    }

    $("#spCartCount").html(spCartCount + " " + "item");
    $.ajax({
        dataType: 'json',
        url: url,
        type: 'Post',
        data: { isApplayLoyalty: x, ItemsJson: objItems, loyaltyModelJson: loyaltyModel},
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            //dealing with the data returned by the request
            $('body').toggleClass('toggled');
            $('#cartFooter').show();

            $('#divSidebarBody').html(data);
            $('#cartFooter').show();

         
            calculateTotal();

        },
        error: function () {
            //alert("AJAX Request Failed, Contact Support");

        }




    });


    // openCart()
});

function closeCart() {

    $('#cartFooter').hide();
    $('body').removeClass('toggled');


    calculateTotal();

}


$(document).on('click', '.remove-cart', function (element) {


    var id = $(this).data('cartid');
    objItems = localStorage.getItem('itemInfo');
    var currentdiv = $(this).closest('.div-selected-item');

    if (objItems !== 'undefined' && objItems !== null && objItems !== "") {
        var existingItems = [];
        var newExistingItems = [];
        existingItems = JSON.parse(objItems);

        existingItems.forEach(function (item, num) {

            if (item.CartItemId === id) {
                $(currentdiv).remove();
                $('.qty_' + item.Id).val('');
                $('.qty_' + item.Id).val(0);
            }
            else {
                newExistingItems.push(item);
            }
        });

        var spCartCount = newExistingItems.length;
        $(".spCartCount").html(spCartCount);

        localStorage.setItem('itemInfo', JSON.stringify(newExistingItems));
        calculateTotal()

    }
});
$(document).on("click", "#btnAddToCart", function (index, e) {


    index.preventDefault();
    if (isValidItem()) {

        let existingItems = []
        // localStorage.setItem('itemInfo', existingItems);

        objItems = localStorage.getItem('itemInfo');

        if (objItems !== 'undefined' && objItems !== null && objItems !== "") {
            existingItems = [];
            existingItems = JSON.parse(objItems);

            localStorage.setItem('itemInfo', JSON.stringify(existingItems));


        }

        let objItem = {
            Id: $('#hdId').val(),
            Name: $('#hdItemName').val(),
            NameEnglish: $('#hdItemNameEN').val(),
            NameArabic: $('#hdItemNameAR').val(),
            Price: $('#hdItemPrice').val(),
            ImageUrl: $('#hdItemImageUrl').val(),
            TenantId: $('#hdTenantId').val(),
            ContactId: $('#ContactId').val(),
            Discount: $('#hdItemDiscount').val(),
            Qty: $('#txtItemQty').val(),
            LoyaltyPoints: $('#LoyaltyPoints').val(),
            IsLoyalClick: $('#IsLoyalClick').val(),
            ItemNote: $('#ItemNotes').val(),
            lstOrderingCartItemSpecificationModel: Fillspecifcationchoices(),
            lstOrderingCartItemAdditionalModel: Filladditional(),
            CreateDateTime: new Date().toUTCString(),
            CartItemId: createUUID(),
            Total: 0,
            CridetPoint:0,
            TotalLoyaltyPoints: 0,
            TotalCreditPoints: 0


        };


        if (objItem.IsLoyalClick == "True") {
            objItem.TotalLoyaltyPoints = calculateTotalLoyaltyPoints(objItem);

            if (objItem.Qty != 0) {
                if (CheckOriginalLoyaltyPoints(objItem.TotalLoyaltyPoints)) {
                    existingItems.push(objItem);
                    // Save back to localStorage
                    localStorage.setItem('itemInfo', JSON.stringify(existingItems));
                    updatePrices();
                }
            } else {

                document.getElementById('txtItemQty').style.color = "red";


            }
        } else {
            objItem.Total = calculateTotalItem(objItem);
            var x = $('#IsLoyalty').val()

            if (x == "True") {
                objItem.TotalCreditPoints = calculateTotalCreditPoints(objItem)
            }

            if (objItem.Qty != 0) {
                existingItems.push(objItem);

                // Save back to localStorage
                localStorage.setItem('itemInfo', JSON.stringify(existingItems));
                updatePrices();

            } else {

                document.getElementById('txtItemQty').style.color = "red";


            }
        }



    }
});

$(document).on("click", ".btnAddToCart-withoutDetails", function (index, e) {


    var $input = $(this).parent().find('input');
    var num = parseInt($input.val());



    let existingItems = []
    // localStorage.setItem('itemInfo', existingItems);

    objItems = localStorage.getItem('itemInfo');

    if (objItems !== 'undefined' && objItems !== null && objItems !== "") {

        existingItems = JSON.parse(objItems);
        newExistingItems = [];
        existingItems.forEach(function (item, num) {

            if (item.Id !== $input.data('id')) {
                newExistingItems.push(item);
            }
        });
        existingItems = newExistingItems;
    }
    if (num > 0) {
        var total = num * parseFloat($input.data('price'));
        let objItem = {
            Id: $input.data('id'),
            Name: $input.data('name'),
            NameEnglish: $input.data('nameen'),
            NameArabic: $input.data('namear'),
            Price: $input.data('price'),
            TenantId: $('#TenantID').val(),
            ContactId: $('#ContactId').val(),
            Discount: $('#discount').val(),
            ImageUrl: $input.data('itemimageurl'),
            Qty: num,
            ItemNote:"",
            CreateDateTime: new Date().toUTCString(),
            CartItemId: createUUID(),
            Total: total
        };


        existingItems.push(objItem);
    }
    localStorage.setItem('itemInfo', JSON.stringify(existingItems));
    updatePrices();



});

$(document).on("click", ".choices-selected", function (index, e) {

    getItemSelectItem();
});

$(document).on("click", ".btnaddtional", function (index, e) {

    getItemSelectItem();
});

$(document).on("click", ".choices-selected-cart", function (index, e) {
    

    objItems = localStorage.getItem('itemInfo');
    var currentdiv = $(this).closest('.div-selected-item');

    var $input = $(this).parent().find('input');
    var id = $input.data('cartid');

    if (parseInt($input.val()) == 0) {
        removeitem = "btnremove_" + id;
        $input.val(1);

        $('#' + removeitem).toggleClass('fliped');


        return
    }

    var qty = parseInt($input.val());
    var existingItems = JSON.parse(objItems);
    var newexistingItems = []

    existingItems.forEach(function (item, num) {

        if (item.CartItemId === id) {

            if (parseInt($input.val()) !== 0) {





                item.Qty = qty;

                if (item.IsLoyalClick == "True") {
                    item.TotalLoyaltyPoints = calculateTotalLoyaltyPoints(item);


                    $('#p_' + id).html(parseFloat(item.Total).toFixed(2));
                    $('#T_' + id).html(parseFloat(item.TotalLoyaltyPoints).toFixed(2));
                    newexistingItems.push(item);

                } else {
                    item.Total = calculateTotalItem(item);
                    calculateCreditPoints(item.Total, id);
                    var x = $('#IsLoyalty').val()

                    if (x == "True") {
                        item.TotalCreditPoints = calculateTotalCreditPoints(item);
                        $('#T_' + id).html(parseFloat(item.TotalLoyaltyPoints).toFixed(2));
                    }
                        $('#p_' + id).html(parseFloat(item.Total).toFixed(2));

                    newexistingItems.push(item);

                }
            }
            else {
                currentdiv.remove();

            }

        }
        else {
            newexistingItems.push(item);
        }

        $('.qty_' + item.Id).val('');
        $('.qty_' + item.Id).val(item.Qty);

    });
    localStorage.setItem('itemInfo', JSON.stringify(newexistingItems));
    calculateTotal();

});


function getItemSelectItem() {

    let objItem = {
        Id: $('#hdId').val(),
        Name: $('#hdItemName').val(),
        Price: $('#hdItemPrice').val(),
        ImageUrl: $('#hdItemImageUrl').val(),
        TenantId: $('#hdTenantId').val(),
        ContactId: $('#ContactId').val(),
        Discount: $('#hdItemDiscount').val(),
        Qty: $('#txtItemQty').val(),
        IsLoyalClick: $('#IsLoyalClick').val(),
        ItemNote: $('#ItemNotes').val(),
        lstOrderingCartItemSpecificationModel: Fillspecifcationchoices(),
        lstOrderingCartItemAdditionalModel: Filladditional(),
        CreateDateTime: new Date(),
        CartItemId: createUUID(),
        Total: 0,
        TotalLoyaltyPoints: 0,
        TotalCreditPoints: 0,
        btnId: $('#hdSubCatId').val(),
        LoyaltyPoints: $('#LoyaltyPoints').val(),


    };





    if (objItem.IsLoyalClick == "True") {

        objItem.TotalLoyaltyPoints = calculateTotalLoyaltyPoints(objItem);
    } else {
        objItem.Total = calculateTotalItem(objItem);
        var x = $('#IsLoyalty').val()

        if (x == "True") {
            objItem.TotalCreditPoints = calculateTotalCreditPoints(objItem);
        }
    }
    calculateTotal()
}
function calculateTotalItem(objItem) {


    var totalPrice = parseFloat(objItem.Price);

    if (objItem.lstOrderingCartItemSpecificationModel != 'undefined' && objItem.lstOrderingCartItemSpecificationModel != null && objItem.lstOrderingCartItemSpecificationModel.length > 0) {
        objItem.lstOrderingCartItemSpecificationModel.forEach(function (item) {

            if (item.lstOrderingCartItemSpecificationChoicesModel != null && item.lstOrderingCartItemSpecificationChoicesModel.length > 0) {
                item.lstOrderingCartItemSpecificationChoicesModel.forEach(function (item) {
                    if (item.Price != "") {
                        totalPrice += parseFloat(item.Price);
                    }
                });
            }

        });

    }

    if (objItem.lstOrderingCartItemAdditionalModel != 'undefined' && objItem.lstOrderingCartItemAdditionalModel != null && objItem.lstOrderingCartItemAdditionalModel.length > 0) {
        objItem.lstOrderingCartItemAdditionalModel.forEach(function (item) {
            if (item.Price != "") {
                totalPrice += parseFloat(item.Total);
            }
        });
    }
    totalPrice = parseFloat(totalPrice) * parseFloat(objItem.Qty);
    $('#stItemTotalPrice').html(totalPrice.toFixed(2));
    calculateCreditPoints(totalPrice, objItem.Id);

    return totalPrice;
}



function calculateCreditPoints(totalPrice, id) {



    $('.pointss_' + id).html();
    var x = $('#IsLoyalty').val()
    if (x == "True") {


        var url = "/Index?&handler=CalculatPointcredit";

        var loyaltyModel = localStorage.getItem('TenantInfo');



        $.ajax({
            dataType: 'json',
            url: url,
            type: 'Post',
            data: { tenantId: parseInt($('#TenantID').val()), price: totalPrice, loyaltyModelJson: loyaltyModel },
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function (data) {
                //dealing with the data returned by the request


                $('.pointss_' + id).html(data);
            },

            error: function () {
                alert("AJAX Request Failed, Contact Support");

            }
        });
    }
}




function calculateTotalCreditPoints(objItem) {



    var TotalCreditPoints = parseFloat(objItem.LoyaltyPoints);


    if (objItem.lstOrderingCartItemSpecificationModel != 'undefined' && objItem.lstOrderingCartItemSpecificationModel != null && objItem.lstOrderingCartItemSpecificationModel.length > 0) {
        objItem.lstOrderingCartItemSpecificationModel.forEach(function (item) {

            if (item.lstOrderingCartItemSpecificationChoicesModel != null && item.lstOrderingCartItemSpecificationChoicesModel.length > 0) {
                item.lstOrderingCartItemSpecificationChoicesModel.forEach(function (item) {
                    if (item.LoyaltyPoints != "") {
                        TotalCreditPoints += parseFloat(item.LoyaltyPoints);

                    }
                });
            }
        });

    }
    if (objItem.lstOrderingCartItemAdditionalModel != 'undefined' && objItem.lstOrderingCartItemAdditionalModel != null && objItem.lstOrderingCartItemAdditionalModel.length > 0) {
        objItem.lstOrderingCartItemAdditionalModel.forEach(function (item) {
            if (item.LoyaltyPoints != "") {
                TotalCreditPoints += parseFloat(item.TotalCreditPoints);
            }
        });
    }
    TotalCreditPoints = parseFloat(TotalCreditPoints) * parseFloat(objItem.Qty);


    return TotalCreditPoints;

}
function calculateTotalLoyaltyPoints(objItem) {


    var TotalLoyaltyPoints = parseFloat(objItem.LoyaltyPoints);


    if (objItem.lstOrderingCartItemSpecificationModel != 'undefined' && objItem.lstOrderingCartItemSpecificationModel != null && objItem.lstOrderingCartItemSpecificationModel.length > 0) {
        objItem.lstOrderingCartItemSpecificationModel.forEach(function (item) {

            if (item.lstOrderingCartItemSpecificationChoicesModel != null && item.lstOrderingCartItemSpecificationChoicesModel.length > 0) {
                item.lstOrderingCartItemSpecificationChoicesModel.forEach(function (item) {
                    if (item.LoyaltyPoints != "") {
                        TotalLoyaltyPoints += parseFloat(item.LoyaltyPoints);

                    }
                });
            }
        });

    }
    if (objItem.lstOrderingCartItemAdditionalModel != 'undefined' && objItem.lstOrderingCartItemAdditionalModel != null && objItem.lstOrderingCartItemAdditionalModel.length > 0) {
        objItem.lstOrderingCartItemAdditionalModel.forEach(function (item) {
            if (item.LoyaltyPoints != "") {
                TotalLoyaltyPoints += parseFloat(item.TotalLoyaltyPoints);
            }
        });
    }
    TotalLoyaltyPoints = parseFloat(TotalLoyaltyPoints) * parseFloat(objItem.Qty);

    $('#stItemTotalLoyaltyPoints').html(TotalLoyaltyPoints.toFixed(2));

    return TotalLoyaltyPoints;

}

function Fillspecifcationchoices() {
    var specifcationlist = [];

    $('.specifications-cart').each(function () {

        var uniqueid = $(this).data('uniqueid');
        var specificationName = $(this).data('specificationname');
        var specificationId = $(this).data('specificationid');
        var isrequired = $(this).data('isrequired');
        var maxselectnumber = $(this).data('maxselectnumber');
        var specifcationchoiceslist = [];


        $(".ch-checkbox_" + uniqueid + ":checkbox:checked").each(function () {
            var obj = {
                Id: $(this).data('id'),
                Name: $(this).data('name'),
                Price: $(this).data('price'),
                TotalCreditPoints: 0,
                LoyaltyPoints: $(this).data('loyaltypoints'),

            };

            specifcationchoiceslist.push(obj);
        });




        $(".ch-radio_" + uniqueid + ":radio:checked").each(function () {

            var obj = {
                Id: $(this).data('id'),
                Name: $(this).data('name'),
                Price: $(this).data('price'),
                 TotalCreditPoints: 0,
                LoyaltyPoints: $(this).data('loyaltypoints')
            };



            specifcationchoiceslist.push(obj);
        });





        var Obj = {
            SpecificationId: specificationId,
            SpecificationName: specificationName,
            lstOrderingCartItemSpecificationChoicesModel: specifcationchoiceslist,

        }
        if (Obj.lstOrderingCartItemSpecificationChoicesModel != null && Obj.lstOrderingCartItemSpecificationChoicesModel.length > 0) {
            specifcationlist.push(Obj);
        }
    })

    return specifcationlist
}

function Filladditional() {
    var additionallist = [];

    $('.ItemQtyAddtional-productdetails').each(function () {
        if (parseInt($(this).val()) > 0) {
            var totalPrice = 0;
            var TotalLoyaltyPoints = 0
            var TotalCreditPoints = 0
            var x = $("#IsLoyalClick").val();
            if (x == "True") {
                if ($(this).data('loyaltypoints') != "") {
                    TotalLoyaltyPoints = parseFloat($(this).data('loyaltypoints')) * parseInt($(this).val());
                }
            }
            else {
                if ($(this).data('price') != "") {
                    totalPrice = parseFloat($(this).data('price')) * parseInt($(this).val());
                }
            }



            var obj = {

                Id: $(this).data('itemadditionsid'),
                ItemAdditionsCategoryId: $(this).data('itemadditionscategoryid'),
                Name: $(this).data('name'),
                NameEnglish: $(this).data('nameen'),
                Price: $(this).data('price'),
                LoyaltyPoints: $(this).data('loyaltypoints'),
                Qty: $(this).val(),
                Total: totalPrice,
                TotalLoyaltyPoints: TotalLoyaltyPoints,
                TotalCreditPoints: TotalCreditPoints
            };
            additionallist.push(obj);
        }

    })

    return additionallist
}

function CheckOriginalLoyaltyPoints(Points) {
    var isloyal = $('#IsLoyalty').val();

    var result = true

    if (isloyal == "True") {

        // $('.stTotalPrice').html(parseFloat(totalPrice).toFixed(2));

        var OriginalPoints = $('#OriginalLoyaltyPoints').val();
        var op = parseFloat(OriginalPoints)
        var x = $('.stTotalLoyaltyPoints')[0].textContent
        var n = parseFloat(x);
        var total = n + parseFloat(Points);
        if (op >= total) {
            var o = op - total;

            if (o > 0) {


            }
            return result

        } else {
            document.getElementById('errormsg').removeAttribute("hidden");
            $('#errormsg').show();
            result = false
            return result
        }
    } else {
        return result
    }
}
function isValidItem() {
    //Audai Todo all validation
    var result = true;

    $('.specifications-cart').each(function () {

        var uniqueid = $(this).data('uniqueid');
        var specificationName = $(this).data('specificationname');
        var specificationId = $(this).data('specificationid');
        var isrequired = $(this).data('isrequired');
        var maxselectnumber = $(this).data('maxselectnumber');
        var ismulti = $(this).data('ismulti');
        var specifcationchoiceslist = [];


        var multiSelect = $(".ch-checkbox_" + uniqueid + ":checkbox:checked").length;
        var select = $(".ch-radio_" + uniqueid + ":radio:checked").length;


        $(".ch-radio_" + uniqueid + ":radio:checked")

        if (isrequired == "True") {

            if (ismulti == "True") {
                if (multiSelect > parseInt(maxselectnumber)) {
                    document.getElementById(this.id).scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                    document.getElementById('req_' + this.id).style.color = "red";

                    result = false;
                } else {
                    document.getElementById('req_' + this.id).style.color = "black";
                    if (multiSelect === 0) {
                        document.getElementById(this.id).scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                        document.getElementById('req_' + this.id).style.color = "red";

                        result = false
                    }
                }
            }
            else {
                document.getElementById('req_' + this.id).style.color = "black";

                if (select != 1) {

                    document.getElementById(this.id).scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                    document.getElementById('req_' + this.id).style.color = "red";

                    result = false;

                }

            }

        }
        //  $(".ch-radio_" + uniqueid + ":radio:checked").each(function () {

        //  });






    });
    return result;

}
function openCart() {
    $('#md-productDetails').modal('hide');
    //document.body.style.zoom = "100%";

    $('#cartFooter').hide();

    $('#cartFooter').show();


    checkValidItemDate();
    var objItems = localStorage.getItem('itemInfo');
    var url = "/Index?&handler=CartItem";
    var spCartCount = JSON.parse(objItems).length;
    $(".spCartCount").html(spCartCount);
    var loyaltyModel = localStorage.getItem('TenantInfo');
    var isLoyaltyApplay = $('#IsLoyalty').val();


    $.ajax({
        dataType: 'json',
        url: url,
        type: 'Post',
        data: { isApplayLoyalty: isLoyaltyApplay, ItemsJson: objItems, loyaltyModelJson: loyaltyModel },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            //dealing with the data returned by the request
            $('#cartFooter').hide();
            $('#cartFooter').show();

            $('body').toggleClass('toggled');
          //  $('#cartFooter').show();
            $('#cartFooter').show();

            $('#divSidebarBody').html(data);
            $('#errormsg2').hide();
            calculateTotal();

        },
        error: function () {
            alert("AJAX Request Failed, Contact Support");

        }
    });





    //Audai Todo all validation 

}
function updatePrices() {

    $('#md-productDetails').modal('hide');
 //   document.body.style.zoom = "100%";

    calculateTotal();

}
function createUUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
function calculateTotal() {

    checkValidItemDate()

    var objItems = localStorage.getItem('itemInfo');
    if (objItems !== undefined && objItems !== null && objItems !== "") {
        var items = JSON.parse(objItems);
        var totalPrice = 0;
        var TotalLoyaltyPoints = 0;
        var TotalCreditPoints = 0;

        items.forEach(function (item, num) {
            if (item.IsLoyalClick == "True") {

                TotalLoyaltyPoints += item.TotalLoyaltyPoints;


            } else {
                totalPrice += item.Total;
                $('#p_' + item.CartItemId).html(parseFloat(item.Total).toFixed(2));
                
                var x = $('#IsLoyalty').val()
                if (x == "True") {
                    TotalCreditPoints += item.LoyaltyPoints;
                }
            }
        });


        $(".spCartCount").html(items.length);
        $('.stTotalPrice').html('');
        $('.stTotalPrice').html(parseFloat(totalPrice).toFixed(2));
        var x = $('#IsLoyalty').val();

        if (x == "True") {

            calculateCreditPoints(totalPrice, 0)

            $('.stTotalLoyaltyPoints').html('');
            $('.stTotalLoyaltyPoints').html(parseFloat(TotalLoyaltyPoints).toFixed(2));

            var xr = $('#OriginalLoyaltyPoints').val();

            xr = parseFloat(xr);
            xr= xr - parseFloat(TotalLoyaltyPoints);
            $('.MyPoints').html('');
            $('.MyPoints').html(xr);
            var o = $('.MyPoints')[0].innerHTML;
            if (xr >= 0) {
                $('#errormsg2').hide();
        


                //  document.getElementById('errormsg2').setAttribute("hidden",);
            } else {
                $('#errormsg2').show();
              

            }

            if (x < 0) {
                document.getElementById('MyPoints').style.color = "red";


            } else {
                document.getElementById('MyPoints').style.color = "";
            }
        } else {
            $('#errormsg2').hide();

        }
    }


    else {
        $(".spCartCount").html(0);
    }

    $('.currencyCode').each(function () {
        $(this).html($('#CurrencyCode').val())
    });

}
function checkValidItemDate() {
    var objItems = localStorage.getItem('itemInfo');
    if (objItems !== undefined && objItems !== null && objItems !== "") {
        var items = JSON.parse(objItems);


        var existingItems = [];
        items.forEach(function (item, num) {

            var validDate = addHours(1, item.CreateDateTime)
            if (validDate > new Date().toUTCString() && $('#ContactId').val() == item.ContactId)
                existingItems.push(item);
        });

        // Save back to localStorage
        localStorage.setItem('itemInfo', JSON.stringify(existingItems));
    }
}

function addHours(numOfHours, dateutc) {


    var date = new Date(dateutc)
    const dateCopy = new Date(date.getTime());

    dateCopy.setTime(dateCopy.getTime() + numOfHours * 60 * 60 * 1000);
    // dateCopy.setTime(dateCopy.getTime() + numOfHours  * 60 * 1000);

    return dateCopy.toUTCString();
}
//#endregion



//#region Search

function search() {
    var id = $('.div-subcategory-item.active').data('id');

    SubCateItem(id, true);
}

function filtering(x) {
    var id = $('.div-subcategory-item.active').data('id');

    SubCateItem(id, true, x);

}


$(document).on('click', '.input-group-btn', function () {
    search();
});


$(document).on("keyup", "#anythingSearch", function () {
    var value = $(this).val().toLowerCase();
    var TenantId = parseInt($('#TenantID').val())
    if (TenantId === 34) {
        if (value.length >= 3) {
            search();

            //   document.getElementById("sec").style.display = "none";

        } else {
            if (value.length == 0) {

                search();

            }
            // document.getElementById("sec").style.display = "block";


        }


    } else {


        $("#myDIV ").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });

    }



});

//#endregion


//#region Order
$(document).on('click', '#btnSubmitOrder', function () {


    document.getElementById("sucsess").style.display = "none";
    document.getElementById("faild").style.display = "none";
    var objItems = localStorage.getItem('itemInfo');


    if (objItems !== undefined && objItems !== null && objItems !== "") {
      
        var note = "";
       

        if (CheckOriginalLoyaltyPoints(0)) {
            document.getElementById("loading").style.visibility = "visible";

            var url = "/Index?&handler=SubmitOrder";

            $('#btnSubmitOrder').addClass('disabled');
            $('#btnSubmitOrder').addClass('availablity');
            var isLoyalty = $('#IsLoyalty').val();

            $.ajax({
                dataType: 'json',
                url: url,
                type: 'Post',
                data: { ItemsJson: objItems, isLoyaltyApplay: isLoyalty, orderNote:note},
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (result) {

                  //  document.getElementById("notes").value = "";
                    emptycart();
                    document.getElementById("sucsess").style.display = "";
                    closeCart();

                    $('#btnSubmitOrder').removeClass('disabled');
                    $('#btnSubmitOrder').removeClass('availablity');
                    $('#loading').removeClass('availablity');


                },
                error: function () {

                    document.getElementById("faild").style.display = "";
                    closeCart();
                    $('#btnSubmitOrder').removeClass('disabled');
                    $('#btnSubmitOrder').removeClass('availablity');
                    $('#loading').removeClass('availablity');
                }
            });
        } else {
            $('#errormsg2').show();
            $('#loading').removeClass('availablity');


        }
    } 




});


function emptycart() {
    var newExistingItemss = [];
    localStorage.setItem('itemInfo', JSON.stringify(newExistingItemss));
    $(".spCartCount").html(0);
}
//#endregion













//#region Swipers


function scrollto2(n) {


    SubCateItem(n);

}



function scrollto(n) {
    var r = swiper.$("#s_" + n).index();
    var m = r - 1;

    if (r > 0) {
        swiper.slideTo(m);
    }
}

var swiperlenth = 3.6
if (window.screen.availWidth > 460 && window.screen.availWidth <= 800) {
    swiperlenth = 5.6
}
else if (window.screen.availWidth > 800) {
    swiperlenth = 7.5

}


var swiper = new Swiper(".mySwiper", {

    slidesPerView: swiperlenth,
    freeMode: true,
    preventClicks: true,
    a11y: false,
    spaceBetween: 00,


});



//#end region  

$(document).on("click", "#btnnav", function () {


    var $main_nav = $('#main-nav');
    var $toggle = $('#toggle');



    var defaultOptions = {
        disableAt: false,
        customToggle: $toggle,
        levelSpacing: 40,

        levelTitles: true,
        levelTitleAsBack: true,
        pushContent: '#container',
        insertClose: 0
    };
    // call our plugin
    var Nav = $main_nav.hcOffcanvasNav(defaultOptions);

    if (Nav.isOpen()) {

        Nav.close();
    }
    else {
        Nav.open();
    }



});









//#region DefualtScript


function defaultScript() {


    var objowlcarousel = $(".owl-carousel-featured");
    if (objowlcarousel.length > 0) {
        objowlcarousel.owlCarousel({
            responsive: {
                0: {
                    items: 2,
                },
                600: {
                    items: 2,
                    nav: false
                },
                1000: {
                    items: 5,
                },
                1200: {
                    items: 5,
                },
            },
            lazyLoad: true,
            pagination: false,
            loop: true,
            dots: false,
            navigation: true,
            stopOnHover: true,
            nav: true,
            navigationText: ["<i class='mdi mdi-chevron-left'></i>", "<i class='mdi mdi-chevron-right'></i>"]
        });
    }

    // ===========Category Owl Carousel============
    var objowlcarousel = $(".owl-carousel-category");
    if (objowlcarousel.length > 0) {
        objowlcarousel.owlCarousel({
            responsive: {
                0: {
                    items: 3,
                },
                600: {
                    items: 5,
                    nav: false
                },
                1000: {
                    items: 8,
                },
                1200: {
                    items: 8,
                },
            },
            items: 8,
            lazyLoad: true,
            pagination: false,
            loop: true,
            dots: false,

            navigation: true,
            stopOnHover: true,
            nav: true,
            navigationText: ["<i class='mdi mdi-chevron-left'></i>", "<i class='mdi mdi-chevron-right'></i>"]
        });
    }





    // ===========Slider============
    var mainslider = $(".owl-carousel-slider");
    if (mainslider.length > 0) {
        mainslider.owlCarousel({
            items: 1,
            dots: false,
            lazyLoad: true,
            pagination: true,

            loop: true,
            singleItem: true,
            navigation: true,
            stopOnHover: true,
            nav: true,
            navigationText: ["<i class='mdi mdi-chevron-left'></i>", "<i class='mdi mdi-chevron-right'></i>"]
        });
    }

    // ===========Single Items Slider============   
    var sync1 = $("#sync1");
    var sync2 = $("#sync2");
    sync1.owlCarousel({
        singleItem: true,
        items: 1,
        slideSpeed: 1000,
        pagination: false,
        navigation: true,
        autoPlay: 2500,
        dots: false,
        nav: true,
        navigationText: ["<i class='mdi mdi-chevron-left'></i>", "<i class='mdi mdi-chevron-right'></i>"],
        afterAction: syncPosition,
        responsiveRefreshRate: 200,
    });
    sync2.owlCarousel({
        items: 5,
        navigation: true,
        dots: false,
        pagination: false,
        nav: true,
        navigationText: ["<i class='mdi mdi-chevron-left'></i>", "<i class='mdi mdi-chevron-right'></i>"],
        responsiveRefreshRate: 100,
        afterInit: function (el) {
            el.find(".owl-item").eq(0).addClass("synced");
        }
    });

    function syncPosition(el) {
        var current = this.currentItem;
        $("#sync2")
            .find(".owl-item")
            .removeClass("synced")
            .eq(current)
            .addClass("synced")
        if ($("#sync2").data("owlCarousel") !== undefined) {
            center(current)
        }
    }
    $("#sync2").on("click", ".owl-item", function (e) {
        e.preventDefault();
        var number = $(this).data("owlItem");
        sync1.trigger("owl.goTo", number);
    });

    function center(number) {
        var sync2visible = sync2.data("owlCarousel").owl.visibleItems;
        var num = number;
        var found = false;
        for (var i in sync2visible) {
            if (num === sync2visible[i]) {
                var found = true;
            }
        }
        if (found === false) {
            if (num > sync2visible[sync2visible.length - 1]) {
                sync2.trigger("owl.goTo", num - sync2visible.length + 2)
            } else {
                if (num - 1 === -1) {
                    num = 0;
                }
                sync2.trigger("owl.goTo", num);
            }
        } else if (num === sync2visible[sync2visible.length - 1]) {
            sync2.trigger("owl.goTo", sync2visible[1])
        } else if (num === sync2visible[0]) {
            sync2.trigger("owl.goTo", num - 1)
        }
    }



}








////#region reOrder
//function reOrderConfirm() {
  
//    document.getElementById("orderDetailss").style.opacity = .3;

//    $('#md-reOrderConfirm').modal('show');

//}
//function reOrder(itemlst) {

//    //confirm msg for emptycart


//    $('#md-reOrderConfirm').modal('hide');

//    var LoyaltyModel = localStorage.getItem('TenantInfo');

//    var lstItems = JSON.stringify(itemlst);
//    var url = "/Orders?&handler=ReOrder";

//    $.ajax({
//        dataType: 'json',
//        url: url,
//        type: 'Post',
//        data: { tenantId: $('#TenantID').val(), lstItems: lstItems, contactid:  $('#ContactId').val() },
//        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
//        success: function (data) {
//            //dealing with the data returned by the request
    
//            let existingItems = [];
  

//            data.forEach(function (item, num) {
//                //tem.ContactId = item.contactId,
//                //    item.CreateDateTime = new Date().toUTCString(),
//                //    item.CartItemId = createUUID()
                  
//                 let obj = {
//                     Id: item.id.toString(),
//                        Name: item.name,
//                        NameEnglish: item.nameEnglish,
//                     NameArabic: item.nameArabic,
//                     Price: item.price.toString(),
//                     Discount: "",
//                     IsLoyalClick: "False",
//                     LoyaltyPoints:0,
//                        ImageUrl: item.imageUrl,
//                     TenantId: item.tenantId.toString(),
//                     ContactId: item.contactId.toString(),
//                     Qty: item.qty.toString(),
//                     lstOrderingCartItemSpecificationModel: FillListSpec(item.lstOrderingCartItemSpecificationModel),
//                     lstOrderingCartItemAdditionalModel: FillListAddtional( item.lstOrderingCartItemAdditionalModel),
//                        CreateDateTime: new Date().toUTCString(),
//                        CartItemId: createUUID(),
//                     Total: item.total,
//                     ItemNote: item.itemNote,
//                    TotalCreditPoints:0,
//                       TotalLoyaltyPoints:0
                      
//                };
//                existingItems.push(obj);

//            });

 

             
//            //});
//            localStorage.setItem('itemInfo', JSON.stringify(existingItems));
            

//            calculateTotal();

        
//            $('#md-OrderItems').modal('hide');
//            openCart();
            
//        },
//        error: function () {
//            //     alert("AJAX Request Failed, Contact Support");

//        }
//    });

//}

//function FillListSpec(lstOrderingCartItemSpecificationModel) {
//    var specifcationlist = [];

//    if (lstOrderingCartItemSpecificationModel != 'undefined' && lstOrderingCartItemSpecificationModel != null && lstOrderingCartItemSpecificationModel.length > 0) {

//        lstOrderingCartItemSpecificationModel.forEach(function (spec) {
//            var specifcationchoiceslist = [];

//            spec.lstOrderingCartItemSpecificationChoicesModel.forEach(function (choice) {



//                var obj = {
//                    Id: choice.id,
//                    Name: choice.name,
//                    Price: choice.price,
//                    LoyaltyPoints: 0

//                };
//                specifcationchoiceslist.push(obj);

//            });

//            var obj2 = {
//                SpecificationId: spec.specificationId,
//                SpecificationName: spec.specificationName,
//                lstOrderingCartItemSpecificationChoicesModel: specifcationchoiceslist,

//            }
//            specifcationlist.push(obj2);



//        });


//    }


//    return specifcationlist;
        
        
//}


//function FillListAddtional(lstOrderingCartItemAdditionalModel) {
//    var additionallist = [];

//    lstOrderingCartItemAdditionalModel.forEach(function (addt) {


//        var obj = {

//            Id: addt.id,
//            ItemAdditionsCategoryId: addt.itemAdditionsCategoryId,
//            Name: addt.name,
//            NameEnglish: addt.name,
//            Price: addt.price,
//            LoyaltyPoints: 0,
//            Qty: addt.qty.toString(),
//            Total: addt.total,
//            TotalLoyaltyPoints: 0,
//            TotalCreditPoints: 0,
//        };
//        additionallist.push(obj);

//    });

//    return additionallist;

//}

////#end region

