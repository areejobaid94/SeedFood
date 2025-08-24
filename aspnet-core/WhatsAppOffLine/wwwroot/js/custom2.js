

$(document).ready(function () {
    //"use strict";
    //document.documentElement.style.setProperty('--primary-color', '#389475');


    //$('#modal-view-allcatgories').modal('hide');

    var objItems = localStorage.getItem('itemInfo');



    if (objItems == null) {

        var newExistingItems = [];

        localStorage.setItem('itemInfo', JSON.stringify(newExistingItems));
    }
    FillTenantInfo();


    //$('select').select2();

    // ===========Tooltip============
   // $('[data-toggle="tooltip"]').tooltip()
    //defaultScript() 

    var tenantId = $('#TenantID').val();
    if (tenantId != 0) {
        
        //var idcat = $('.category-item').data('id');
        //$('#s_' + idcat).addClass('active');
        ////var lst = $('#s_' + idcat).data('subcategory');
        //subCategoryselection(idcat);
        var id = $('.Allsubcategory').data('id');
        var subCategory = $('.Allsubcategory').data('subcategory');
        if (id != null && id != undefined)
        filterAllSelection(id, subCategory);


        var isLoyalty = $('#IsLoyalty').val();
        if (isLoyalty =="True") {
            var idcat = $('.category-item')[1].id;
            var id = idcat.replace("cat_", "")
            $('#s_' + id).addClass('active');
            subCategoryselection(id);
        }
        else
        {
            var idcat = $('.category-item').data('id');
            $('#s_' + idcat).addClass('active');
            subCategoryselection(idcat);
        }
        var owl = $('.owl-carousel');
        var isDragging = false;

        owl.on('drag.owl.carousel', function () {
            //console.log('carousel drag');
            isDragging = true;
        });

        owl.on('dragged.owl.carousel', function () {
            //console.log('carousel drag ended');
        });

        $('.div-category-item').on('touchstart', function (event) {
            if (isDragging) {
                event.preventDefault();
                //var id = $('.div-category-item').data('id');
                //var subCategory = $('.div-category-item').data('subcategory');
                //filterAllSelection(id, subCategory);
                
                var id = $(this).data('id');
                if (id == 0) {
                    var button = document.querySelector('.categories-btn');
                    button.style.display = 'none';
                }
                else
                {
                    var button = document.querySelector('.categories-btn');
                    button.style.display = 'block';
                }
                var subCategory = $(this).data('subcategory');
                filterAllSelection(id, subCategory);
                filterSelection(id, subCategory);
                //console.log('Touch event ', idcat);
                isDragging = false;

                //var idcat = $(event.target).closest('.category-item').data('id');
                //alert(idcat + "  fffffff");
                //$('#s_' + idcat).addClass('active');
                //subCategoryselection(idcat);
            }
        });
        //  var p = 0;
    }
});

var scrollTimer = -1;

window.addEventListener('scroll', (event) => {
    //  scrollFunction();
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

$(window).scroll(function () {
    if ($(window).scrollTop() >= 300) {
        //$('.filter-slide').addClass('fixed-area');

    }
    else {
        //$('.filter-slide').removeClass('fixed-area');

    }
});


$(document).on("click", ".btn-filter", function (e) {
    var $target = $(this).data('target');
    $('.btn-filter').removeClass('active');
    $(this).toggleClass('active');
    if ($target != 'all') {
        $('.filterBox .jsFilter').css('display', 'none');
        $('.filterBox .jsFilter[data-type="' + $target + '"]').fadeIn('slow');

    } else {
        $('.filterBox .jsFilter').css('display', 'none').fadeIn('slow');
        setTimeout(function () {
            $('html, body').animate({
                scrollTop: $(".filterBox-connect").offset().top

            }, 2000);
        }, 500);
    }
});
$(document).on("click", ".btn-change-lang", function (e) {


    var url = "/Index1?&handler=SetLanguage";
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
        }
    });




});


function SetLanguage() {



    //var url = "/Index1?culture=" + x + "&returnUrl=" + rout + "&handler=SetLanguage";



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



//SideMenu Toggle
$(document).on("click", ".ToggleMenu", function () {
    $('#sidebars').toggleClass('toggle');
});
//SideMenu Toggle close on click anywhere
$(document).mouseup(function (e) {
    var container = $(".sidebar.toggle");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        container.toggleClass('toggle');
    }
    const myDiv = document.getElementById('enlarge');
    if (myDiv != null)
    {
        // Add a class to the element
        myDiv.classList.remove('enlarge-image');
        document.getElementById('modal-item-scrool').style.overflow = 'auto';
        
        $('#enlarge-icon').hide();
       // document.getElementById('offer-updet').style.display = "flex";
        //document.getElementById('offer-updet').classList.remove("disNone");
        //document.getElementById('offer-updet').classList.add("offer-updet");
        const icon = document.getElementById('enlarge-icon');
        if (icon != null) {
            icon.classList.remove('enlarge-icon-after');
        }
        const elements = document.getElementsByClassName('offer-updet');
        if (icon != null) {
            for (let i = 0; i < elements.length; i++) {
                elements[i].style.display = 'flex';
            }
        }
        
    }

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


        var url = "/Index1?tenantID=" + parseInt($('#TenantID').val()) + "&isLoyaltyApplay=" + isLoyaltyApplay + "&subCateoryname=" + subCateoryname + "&currencyCode=" + $('#CurrencyCode').val() + "&areaId=" + parseInt($('#AreaId').val()) + "&PageNumber=" + pageNumber + "&loyaltyModelJson=" + loyaltyModel + "&subCategoryid=" + newId + "&search=" + search + "&IsSort=" + IsSort + "&handler=SubCategoriesandItems";


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
                        //alert('1')
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
                    fillDefaultScript();

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
$(document).on("click", ".div-category-item-Loyalty", function () {
    var id = $(this).data('id');
    var subCategory = $(this).data('subcategory');
    filterSelection(id, subCategory);
    $('.div-category-item-Loyalty').addClass('active');
    document.getElementsByClassName("close")[3].click();
})

$(document).on("click", ".div-category-item", function () {
    //  debugger;
    var button = document.querySelector('.categories-btn');
    button.style.display = 'block';

    var id = $(this).data('id');
    var subCategory = $(this).data('subcategory');
    filterSelection(id, subCategory);

    document.getElementsByClassName("close")[3].click();
    //$('#modal-view-allcatgories').modal('hide');

})
/*$('.category-item').on('touchstart', function (event) {*/
$(document).on("click", ".Allsubcategory", function () {

    var id = $(this).data('id');
    var subCategory = $(this).data('subcategory');
    //console.log(id ," 111111");
    //console.log(subCategory , " 22222222222");

    filterAllSelection(id, subCategory);

    document.getElementsByClassName("close")[3].click();
})
$(document).on("click", ".div-All-Subcategory-item", function () {

    document.getElementsByClassName("close")[3].click();

})
$(document).on("click", ".hideAllSubCat", function () {
    var button = document.querySelector('.categories-btn');
    button.style.display = 'none';
})
function filterSelection(c, sublist) {

    var objItems = JSON.stringify(sublist);



    var url = "/Index1?&handler=SubList";
    if (objItems != null) {
        //  alert(objItems)
        spCartCount = JSON.parse(objItems).length;
    }

    $.ajax({
        dataType: 'json',
        url: url,
        type: 'Post',
        data: { ItemsJson: objItems },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            //dealing with the data returned by the request
            
            $('#divSubcategorys').html('');
            $('#divSubcategorys').append(data);
            
            
           subCategoryselection(c);


            fillDefaultScript();
            
            $('#modal-view-allcatgories').modal('hide');
            
        },
        error: function () {
            //     alert("AJAX Request Failed, Contact Support");

        }
    });
}
function filterAllSelection(c, sublist) {
    
    
    
    var objItems = JSON.stringify(sublist);
    
    var url = "/Index1?&handler=AllSubList";
    if (objItems != null) {
       spCartCount = JSON.parse(objItems).length;
    }
    
    $.ajax({
        dataType: 'json',
        url: url,
        type: 'Post',
        data: { ItemsJson: objItems },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            //dealing with the data returned by the request

            $('#divAllSubcategorys').html('');
            $('#divAllSubcategorys').append(data);
            
           // AllsubCategoryselection(c);

            fillDefaultScript();

            $('#modal-view-allcatgories').modal('hide');
        },
        error: function () {
            //     alert("AJAX Request Failed, Contact Support");
        }
    });
}

function ALLsubCategoryselection(c) {
    
    var idsub = $('.div-subcategory-item').data('id');
    $('.div-category-item').removeClass('active');
    $('.div-category-item-Loyalty').removeClass('active');

    $('#s_' + c).addClass('active');
        
    $('.btn-filter').removeClass('active');
    $($('.btn-filter')[0]).toggleClass('active');
    
    SubCateItem(idsub, true);
}




function subCategoryselection(c) {

    var idsub = $('.div-subcategory-item').data('id');
    $('.div-category-item').removeClass('active');
    $('.div-category-item-Loyalty').removeClass('active');

    $('#s_' + c).addClass('active');

    $('.btn-filter').removeClass('active');
    $($('.btn-filter')[0]).toggleClass('active');

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
    var url = "/Index1?lst=" + objTenantInfo + "&handler=TenantInfo";
    $.get(url, function (result) {
        $('#divTenantInfo').html('');
        $('#divTenantInfo').html(result);

        //defaultScript();
        fillDefaultScript()

        calculateTotal();

    });

}

function FillContactInfo() {

    var url = "/Index1?contactId=" + $('#ContactId').val() + "&handler=ContactInfo";
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
    /*alert("nnnnn");*/
    
    var loyaltyModel = localStorage.getItem('TenantInfo');
    var id = $(this).data('id');
    var isLoyaltyApplay = $('#IsLoyalty').val();


    var url = "/Index1?id=" + id + "&tenantId=" + $('#TenantID').val() + "&isLoyaltyApplay=" + isLoyaltyApplay + "&loyaltyModelJson=" + loyaltyModel + "&subcatId=" + $('#hdSubCatId').val() + "&handler=ProuductDetails";
    $.get(url, function (result) {

        $('#divProductDetails').html('');
        $('#divProductDetails').html(result);
        $('#md-productDetails').modal('show');

        $('#errormsg').hide();
        $('#enlarge-icon').hide();
       // document.getElementById('offer-updet').style.display = "flex";
        //document.getElementById('offer-updet').classList.remove("disNone");
        //document.getElementById('offer-updet').classList.add("offer-updet");
        const icon = document.getElementById('enlarge-icon');
        if (icon != null) {
            icon.classList.remove('enlarge-icon-after');
        }


        fillDefaultScript()
        getItemSelectItem();



        $(".div-item-quantity").find("button").addClass('choices-selected');
        $(".div-addtionals").find("button").addClass('btnaddtional');
    })
});


///*#endregion*/
//$(document).on('click', '.close-Product', function () {
//    /*document.body.classList.remove('no-scroll');*/
//    /*$('#md-productDetails').toggleClass('toggle');*/
    
//    $('#md-productDetails').toggleClass('toggle');
//});


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

//$(document).on('click', '.toggled', function () {

//    if (sideBarOpen) {
//        //  openCart();
//    } else {

//        closeCart();

//    }
//    sideBarOpen = false;

//});




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
    var url = "/Index1?&handler=CartItem";
    if (objItems != null) {
        spCartCount = JSON.parse(objItems).length;
    }
    
    $("#spCartCount").html(spCartCount + " " + "item");
    $.ajax({
        dataType: 'json',
        url: url,
        type: 'Post',
        data: { isApplayLoyalty: x, ItemsJson: objItems, loyaltyModelJson: loyaltyModel },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            //dealing with the data returned by the request
            // $('body').toggleClass('toggled');
            //  $('#cartFooter').show();

            //  $('#cartFooter').show();


            $('#divSidebarBody').html('');
            $('#divSidebarBody').html(data);
            $('#modal-checkout').modal('show');
            fillDefaultScript()
            calculateTotal();
            $('.div-quantity-item-cart').find("button").addClass("choices-selected-cart");


        },
        error: function () {
            //alert("AJAX Request Failed, Contact Support");

        }




    });


    // openCart()
});

function closeCart() {
    
    $('#cartFooter').hide();
    //$('body').removeClass('toggled');
    

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
        if (newExistingItems.length <= 0) {
            var url = "/Index1?&handler=UpdateReminderMessage";
            $.ajax({
                url: url,
                type: 'Post',
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (html, status, Data) {

                //    alert("ddf");
                },
                error: function () {
                    alert("AJAX Request Failed, Contact Support");

                }
            });
        }
        var spCartCount = newExistingItems.length;
        $(".spCartCount").html(spCartCount);

        localStorage.setItem('itemInfo', JSON.stringify(newExistingItems));
        calculateTotal()

    }
});
$(document).on("click", "#btnAddToCart", function (index, e) {

    //document.getElementById('offer-updet').classList.remove("disNone");
    //document.getElementById('offer-updet').classList.add("offer-updet");
    //document.getElementById('offer-updet').style.display = "flex";
    index.preventDefault();
    if (isValidItem($('#txtItemQty').val())) {

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
            /*Point: $('#hdItemPoint').val(),*/
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
            CridetPoint: 0,
            TotalLoyaltyPoints: 0,
            TotalCreditPoints: 0,
            IsLoyal: $('#IsLoyal').val(),
            /*TotalOrderNotComblet: $('#TotalOrderNotComblet').val(),*/
            LoyaltycreditPoints: $('#LoyaltycreditPoints').val(),
        };


        if (objItem.IsLoyalClick == "True") {
            objItem.Total = calculateTotalItemForloyaltyItem(objItem);
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
            //var url = "/Index1?&handler=CreateMenuReminderMessage";
            //var objItems = localStorage.getItem('itemInfo');
            //var objItems = JSON.stringify(sublist);
            //if (objitem.total >= objitem.totalordernotcomblet) {
            //    var url = "/index1?handler=CreateMenuReminderMessage";

            //}
           
          
        }

        var url = "/Index1?&handler=CreateReminderMessage";
        $.ajax({
            url: url,
            type: 'Post',
            data: { contactId: parseInt(objItem.ContactId) },
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function (html, status, Data) {


            },
            error: function () {
                //     alert("AJAX Request Failed, Contact Support");

            }
        });
        //document.getElementById("btnAddToCart").disabled = true;
        //document.getElementById("btnAddToCart").style.cursor = 'no-drop';
        //document.getElementById("btnAddToCart").style.opacity = .65; 
        
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
            ItemNote: "",
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
        isValidItem($('#txtItemQty').val())
        //$('#md-productDetails .modal-body').animate({
        //    scrollTop: $("#Options2").offset().top
        //}, 2000);

    getItemSelectItem();
});

$(document).on("click", ".btnaddtional", function (index, e) {
    
    getItemSelectItem();
});

$(document).on("click", ".enlarge-Click", function () {

    const myDiv = document.getElementById('enlarge');
    //alert("ss");
    // Add a class to the element
    myDiv.classList.add('enlarge-image');
    document.getElementById('modal-item-scrool').style.overflow = 'hidden';
    document.getElementById('enlarge-icon').removeAttribute("hidden");
    //document.getElementById('offer-updet').classList.add("disNone");
    //document.getElementById('offer-updet').classList.remove("offer-updet");
    const elements = document.getElementsByClassName('offer-updet');
    for (let i = 0; i < elements.length; i++) {
        elements[i].style.display = 'none';
    }
    $('#enlarge-icon').show();


    const icon = document.getElementById('enlarge-icon');
    icon.classList.add('enlarge-icon-after');



    
});

$(document).on("click", ".choices-selected-cart", function (index, e) {
    
    objItems = localStorage.getItem('itemInfo');
    var currentdiv = $(this).closest('.div-selected-item');

    var $input = $(this).parent().find('input');


    var id = $(this).closest('.div-quantity-item-cart').data('cartid');
    
     var removeitem = "btnremove_" + id;
    $('#' + removeitem).css("display", "none")
    if (parseInt($input.val()) == 0) {
        $input.val(1);
        $('#' + removeitem).css("display", "block")

     //  $('#' + removeitem).toggleClass('fliped');


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
                    item.Total = calculateTotalItemForloyaltyItem(item);
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
        LoyaltycreditPoints: $('#LoyaltycreditPoints').val(),
        IsLoyal: $('#IsLoyal').val(),
        

    };
                                                                                            



    if (objItem.IsLoyalClick == "True") {
        
        objItem.Total = calculateTotalItemForloyaltyItem(objItem);
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
function calculateTotalItemForloyaltyItem(objItem) {

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
    return totalPrice;
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


        var url = "/Index1?&handler=CalculatPointcredit";

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
                var CridetPointElements = document.querySelectorAll('.CridetPoint');
                var CridetPoints = 0;
                CridetPointElements.forEach(function (element) {
                    CridetPoints += parseInt(element.innerText);
                });

                $('.stTotalCreditPointsForBasket').html('');
                $('.stTotalCreditPointsForBasket').html(parseFloat(CridetPoints).toFixed(2));
            },

            error: function () {
                alert("AJAX Request Failed, Contact Support");

            }
        });

    }
}




function calculateTotalCreditPoints(objItem) {


    if (objItem.LoyaltycreditPoints == null) {
        var TotalCreditPoints = parseFloat(0);
    }
    else {
        var TotalCreditPoints = parseFloat(objItem.LoyaltycreditPoints);
    }

    if (objItem.lstOrderingCartItemSpecificationModel != 'undefined' && objItem.lstOrderingCartItemSpecificationModel != null && objItem.lstOrderingCartItemSpecificationModel.length > 0) {
        objItem.lstOrderingCartItemSpecificationModel.forEach(function (item) {
            
                if (item.lstOrderingCartItemSpecificationChoicesModel != null && item.lstOrderingCartItemSpecificationChoicesModel.length > 0) {
                    item.lstOrderingCartItemSpecificationChoicesModel.forEach(function (item) {
                        if (item.LoyaltyPoints != "") {
                            if (parseFloat(item.Id) == 1195 || parseFloat(item.Id) == 1193 || parseFloat(item.Id) == 1192 || parseFloat(item.Id) == 1194) {
                            }
                            else { 
                                TotalCreditPoints += parseFloat(item.LoyaltyPoints);
                            }
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
    //Here if the points are negative
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
                MaxSelectNumber: $(this).data('MaxSelectNumber'),
                IsMultipleSelection: $(this).data('IsMultipleSelection'),

            };
            //if (isrequired && specifcationchoiceslist.length+1 <= maxselectnumber) {
            //    specifcationchoiceslist.push(obj);
            //}
            specifcationchoiceslist.push(obj);
        });




        $(".ch-radio_" + uniqueid + ":radio:checked").each(function () {

            var obj = {
                Id: $(this).data('id'),
                Name: $(this).data('name'),
                Price: $(this).data('price'),
                TotalCreditPoints: 0,
                LoyaltyPoints: $(this).data('loyaltypoints'),
                MaxSelectNumber: $(this).data('MaxSelectNumber'),
                IsMultipleSelection: $(this).data('IsMultipleSelection'),
            };


   

            specifcationchoiceslist.push(obj);
        });


        
        /*alert(specifcationchoiceslist.length);*/
        




        var Obj = {
            SpecificationId: specificationId,
            SpecificationName: specificationName,
            MaxSelectNumber: maxselectnumber,
            IsMultipleSelection: isrequired,
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
        if (parseInt($(this).val()) > 0 && $(this).data('itemadditionsid') !== undefined) {
            
            var totalPrice = 0;
            var TotalLoyaltyPoints = 0
            var TotalCreditPoints = 0
            var x = $("#IsLoyalClick").val();
            if (x == "True") {
                if ($(this).data('loyaltypoints') != "") {
                    if ($(this).data('price') != "") {
                        totalPrice = parseFloat($(this).data('price')) * parseInt($(this).val());
                    }
                    TotalLoyaltyPoints = parseFloat($(this).data('loyaltypoints')) * parseInt($(this).val());
                }
            }
            else {
                if ($(this).data('price') != "") {
                    totalPrice = parseFloat($(this).data('price')) * parseInt($(this).val());
                    var x = $('#IsLoyal').val();
                    if (x == "True") {
                        if ($(this).data('loyaltypoints') != "") {
                            TotalCreditPoints = parseFloat($(this).data('loyaltypoints')) * parseInt($(this).val());
                        }
                    }
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
        var op = parseFloat(OriginalPoints);
        if ($('.stTotalLoyaltyPointsForBasket')[0].value > 0 && $('.stTotalLoyaltyPointsForBasket')[0].value != null) {
            var x = $('.stTotalLoyaltyPointsForBasket')[0].textContent;
        }
        else {
            x = $('.stTotalLoyaltyPoints')[0].textContent;
        }
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
function isValidItem(qte) {

   /* alert(qte)*/
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
        
        var $smallrequired = $(this).find('.small-required')
        
        $smallrequired.removeClass('required');
       /* alert($(".ch-checkbox_" + uniqueid + ":checkbox:checked").length)*/
        var multiSelect = $(".ch-checkbox_" + uniqueid + ":checkbox:checked").length ;
        var select = $(".ch-radio_" + uniqueid + ":radio:checked").length;
       /* alert(multiSelect)*/

        $(".ch-radio_" + uniqueid + ":radio:checked")
        
        if (isrequired == "True") {

            if (ismulti == "True") {

                if (multiSelect > parseInt(maxselectnumber)) {
                    
                    document.getElementById(this.id).scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                    // document.getElementById('req_' + this.id).style.color = "red";
                    $smallrequired.addClass('required');
                    result = false;
                } else {
                    //   document.getElementById('req_' + this.id).style.color = "black";
                    
                    if (multiSelect === 0) {
                        document.getElementById(this.id).scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                        //  document.getElementById('req_' + this.id).style.color = "red";
                        $smallrequired.addClass('required');

                        result = false
                    }
                }
            }
            else {
                //  document.getElementById('req_' + this.id).style.color = "black";
                
                if (select != 1) {
  
                    document.getElementById(this.id).scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                    //  document.getElementById('req_' + this.id).style.color = "red";
                    $smallrequired.addClass('required');

                    result = false;
                   //alert(uniqueid + "uniqueid");
                   // alert(this.id + "ihdsvcv");
                }
              
            }


        }
        else {
            if (ismulti == "True") {
                
                if (multiSelect > parseInt(maxselectnumber)) {


                    document.getElementById(this.id).scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                    // document.getElementById('req_' + this.id).style.color = "red";
                    $smallrequired.addClass('required');
                    result = false;
                }
            }
            else {
                //  document.getElementById('req_' + this.id).style.color = "black";

                if (select != 1 && select!=0) {
                    document.getElementById(this.id).scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
                    //  document.getElementById('req_' + this.id).style.color = "red";
                    $smallrequired.addClass('required');

                    result = false;

                }

            }
        }
        


        //  $(".ch-radio_" + uniqueid + ":radio:checked").each(function () {

        //  });




        
        return result;
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
    var url = "/Index1?&handler=CartItem";
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

           // $('body').toggleClass('toggled');
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

                //var x = $('#IsLoyalty').val()
                //if (x == "True") {
                //    //TotalCreditPoints += item.LoyaltyPoints;
                //    TotalCreditPoints += item.TotalCreditPoints;
                //}
                if (item.IsLoyal =="True") {
                    //TotalCreditPoints += item.LoyaltyPoints;
                    TotalCreditPoints += item.TotalCreditPoints;

                  
                }
            }
        });


        $(".spCartCount").html(items.length);
        if (items.length > 0) {
            $('#spCartTextBasket').show();
        }
        else {
            $('#spCartTextBasket').hide();
        }
        $('.stTotalPrice').html('');
        $('.stTotalPrice').html(parseFloat(totalPrice).toFixed(2));
        var x = $('#IsLoyalty').val();

        if (x == "True") {
            $('.stTotalCreditPoints').html('');
            $('.stTotalCreditPoints').html(parseFloat(TotalCreditPoints).toFixed(2));



            var points = $('#OriginalLoyaltyPoints').val();
            $('.InvoiceTotalPoints').html('');
            $('.InvoiceTotalPoints').html(parseFloat(points).toFixed(2));

            calculateCreditPoints(totalPrice, 0)

            var CridetPointElements = document.querySelectorAll('.CridetPoint');
            var CridetPoints = 0;
            CridetPointElements.forEach(function (element) {
                CridetPoints += parseInt(element.innerText);
            });

            $('.stTotalCreditPointsForBasket').html('');
            $('.stTotalCreditPointsForBasket').html(parseFloat(CridetPoints).toFixed(2));

            $('.stTotalLoyaltyPoints').html('');
            $('.stTotalLoyaltyPoints').html(parseFloat(TotalLoyaltyPoints).toFixed(2));

            var loyaltyPointsElements = document.querySelectorAll('.TotalLoyaltyPoints');
            var point = 0;
            loyaltyPointsElements.forEach(function (element) {
                point += parseInt(element.innerText);
            });

            $('.stTotalLoyaltyPointsForBasket').html('');
            $('.stTotalLoyaltyPointsForBasket').html(parseFloat(point).toFixed(2));

            var xr = $('#OriginalLoyaltyPoints').val();

            xr = parseFloat(xr);
            if (point <= 0 || point == null) {
                var TotalLoyaltyPoint = document.getElementById("TotalLoyaltyPoints");
                var TotalLoyaltyPointsForBasket = document.getElementById("TotalLoyaltyPointsForBasket");
                if (TotalLoyaltyPointsForBasket != null) {
                    TotalLoyaltyPointsForBasket.disabled = true;
                    TotalLoyaltyPointsForBasket.style.opacity = 0;
                }
                if (TotalLoyaltyPoint != null) {
                    TotalLoyaltyPoint.disabled = false;
                    TotalLoyaltyPoint.style.opacity = 1;
                }

                xr = xr - parseFloat(TotalLoyaltyPoints);
            }
            else {
                var TotalLoyaltyPoint = document.getElementById("TotalLoyaltyPoints");
                var TotalLoyaltyPointsForBasket = document.getElementById("TotalLoyaltyPointsForBasket");
                if (TotalLoyaltyPointsForBasket != null) {
                    TotalLoyaltyPointsForBasket.disabled = false;
                    TotalLoyaltyPointsForBasket.style.opacity = 1;
                }

                if (TotalLoyaltyPoint != null) {
                    TotalLoyaltyPoint.disabled = true;
                    TotalLoyaltyPoint.style.opacity = 0;
                }

                xr = xr - parseFloat(point);
            }
            if (xr < 0) {
                xr = 0;
            }
            $('.MyPoints').html('');
            $('.MyPoints').html(parseFloat(xr).toFixed(2));
            var o = $('.MyPoints')[0].innerHTML;
            if (xr > 0) {
                $('#errormsg2').hide();
                var btnSubmitOrder = document.getElementById("btnSubmitOrder");
                if (btnSubmitOrder != null) {
                    btnSubmitOrder.disabled = false;
                    btnSubmitOrder.style.cursor = 'auto';
                    btnSubmitOrder.style.opacity = 1;
                }
                //  document.getElementById('errormsg2').setAttribute("hidden",);
            } else {
                $('#errormsg2').show();

                document.getElementById("btnSubmitOrder").disabled = true;
                document.getElementById("btnSubmitOrder").style.cursor = 'no-drop';
                document.getElementById("btnSubmitOrder").style.opacity = .65; 
            }

            if (x < 0) {
                document.getElementById('MyPoints').style.color = "red";


            } else {
                document.getElementById('MyPoints').style.color = "";
            }
        } else {
            $('#errormsg2').hide();
            var btnSubmitOrder = document.getElementById("btnSubmitOrder");
            if (btnSubmitOrder != null) {
                btnSubmitOrder.disabled = false;
                btnSubmitOrder.style.cursor = 'auto';
                btnSubmitOrder.style.opacity = 1;
            }
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
    //alert('4')
    
    SubCateItem(id, true, x);

}


$(document).on('click', '.input-group-btn', function () {
    search();
});

//Call for global srearch
$(document).on('click', '#globalSearch', function () {
    $('.global-search').toggleClass('search-view');
    $('.toggle-search-icon').toggleClass('icon-search icon-close');
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
        //if (value.length >= 3 ) {
        //    search();
        //} else {
        //    if (value.length == 0 ) {
        //        search();
        //    }
        //}
        if (event.keyCode === 13 || event.key === "Enter") {
            search();
        }
        else {
            if (value.length == 0)
                search();
            }
        //$(document).on('click', '#AllSearch', function () {
        //    search();
        //});
        
        //$("#myDIV ").filter(function () {
        //    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        //});
    }



});

//#endregion


//#region Order
$(document).on('click', '#btnSubmitOrder', function () {



    //   document.getElementById("sucsess").style.display = "none";
    //document.getElementById("btnSubmitOrder").style.display = "none";
    var objItems = localStorage.getItem('itemInfo');
    
    if (objItems !== undefined && objItems !== null && objItems !== "" && objItems !== "[]") {

        
        var note = $("#txtOrderNotes").val();

        //alert("s");
        if (CheckOriginalLoyaltyPoints(0)) {
            //   document.getElementById("loading").style.visibility = "visible";
            //alert("saddd");

            var url = "/Index1?&handler=SubmitOrder";

            $('#btnSubmitOrder').addClass('disabled');
            $('#btnSubmitOrder').addClass('availablity');
            var isLoyalty = $('#IsLoyalty').val();

            $.ajax({
                dataType: 'json',
                url: url,
                type: 'Post',
                data: { ItemsJson: objItems, isLoyaltyApplay: isLoyalty, orderNote: note },
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (result) {

                    //  document.getElementById("notes").value = "";
                    emptycart();
                    //  document.getElementById("sucsess").style.display = "";
                    //  closeCart();
                    $('#modal-checkout').modal('hide');
                    $('#modal-order-success').modal('show');

                    //$('#btnSubmitOrder').removeClass('disabled');
                    //$('#btnSubmitOrder').removeClass('availablity');
                    //$('#loading').removeClass('availablity');


                },
                error: function () {

                    $('#modal-checkout').modal('hide');
                    $('#modal-order-erorr').modal('show');
                    // document.getElementById("faild").style.display = "";
                    //closeCart();
                    //$('#btnSubmitOrder').removeClass('disabled');
                    //$('#btnSubmitOrder').removeClass('availablity');
                    //$('#loading').removeClass('availablity');
                }
            });
            document.getElementById("btnSubmitOrder").disabled = true;
            document.getElementById("btnSubmitOrder").style.cursor = 'no-drop'; 
            document.getElementById("btnSubmitOrder").style.opacity = .65; 

        } else {
            $('#errormsg2').show();
            $('#loading').removeClass('availablity');


        }
    }
    else {
        $('#modal-checkout').modal('hide');
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


//var swiper = new Swiper(".mySwiper", {

//    slidesPerView: swiperlenth,
//    freeMode: true,
//    preventClicks: true,
//    a11y: false,
//    spaceBetween: 00,


//});



//#endregion  

//$(document).on("click", "#btnnav", function () {


//    var $main_nav = $('#main-nav');
//    var $toggle = $('#toggle');



//    var defaultOptions = {
//        disableAt: false,
//        customToggle: $toggle,
//        levelSpacing: 40,

//        levelTitles: true,
//        levelTitleAsBack: true,
//        pushContent: '#container',
//        insertClose: 0
//    };
//    // call our plugin
//    var Nav = $main_nav.hcOffcanvasNav(defaultOptions);

//    if (Nav.isOpen()) {

//        Nav.close();
//    }
//    else {
//        Nav.open();
//    }



//});









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

function trimText(selector, maxlength) {
    $(selector).each(function () {
        if ($(this).text().length > maxlength) {
            $(this).text($(this).text().substr(0, maxlength) + ' ...');
        }
    });
}

function fillDefaultScript() {
    var rtl = false;
   // var lang = $(this).data('lang');
    var lang = $("html").attr("lang");
    if (lang == "ar") {
        rtl = true;
    }
    
    //Call Script for inputs spinner + -
    $("input[type='number']").inputSpinner()


    //SideMenu Toggle
    //$(".ToggleMenu").click(function () {
    //    $('#sidebars').toggleClass('toggle');

    //});
    //SideMenu Toggle close on click anywhere
    $(document).mouseup(function (e) {
        var container = $(".sidebar.toggle");
        if (!container.is(e.target) && container.has(e.target).length === 0) {
          //  container.toggleClass('toggle');
        }

    });
    //Call for global srearch
    //$("#globalSearch").click(function () {
    //    $('.global-search').toggleClass('search-view');
    //    $('.toggle-search-icon').toggleClass('icon-search icon-close');
    //});


    // Level One Catgories
    $('.slider').owlCarousel({
        rtl: rtl,
        margin: 10,
        dots: false,
        nav: true,
        //slideSpeed: 200,
        autoWidth: true,
        responsive: {
            0: {
                items: 3
            },

            600: {
                items: 4
            },

            1200: {
                items: 6
            }
        }
    });

    // Level Tow Catgories
    $('.filter-slide').owlCarousel({
        center: false,
        loop: false,
        rtl: rtl,
        margin: 0,
        dots: false,
        nav: false,
        autoWidth: true,
        //slideSpeed: 200,
        responsive: {
            0: {
                items:3,

            },

            600: {
                items: 4
            },

            1200: {
                items: 4
            }
        }
        // margin: 10,
        //dots: false,
        //nav: false,
        //responsive: {
        //    0: {
        //        items: 3,
        //    },
        //    600: {
        //        items: 4
        //    },
        //    1200: {
        //        items: 10
        //    }
        //}
    });

    // Items Slider Items Detials  
    $('.item-img-slide').owlCarousel({
        rtl: rtl,
        loop: true,
        margin: 10,
        nav: false,
        dots: true,
        items: 1

    })
    
    // Filter of Level Tow Catgories
    $('.btn-filter').on('click', function () {
        var $target = $(this).data('target');
        $('.btn-filter').removeClass('active');
        $(this).toggleClass('active');
        if ($target != 'all') {
            $('.filterBox .jsFilter').css('display', 'none');
            $('.filterBox .jsFilter[data-type="' + $target + '"]').fadeIn('slow');
        } else {
            $('.filterBox .jsFilter').css('display', 'none').fadeIn('slow');
        }
    });
    


    function trimText(selector, maxlength) {
        $(selector).each(function () {
            if ($(this).text().length > maxlength) {
                $(this).text($(this).text().substr(0, maxlength) + ' ...');
            }
        });
    }
    trimText('.trim-text-10', 10);
    trimText('.trim-text-15', 15);
    trimText('.trim-text-20', 20);
    trimText('.trim-text-40', 40);
    trimText('.trim-text-50', 50);
    trimText('.trim-text-85', 85);
    trimText('.trim-text-100', 100);
    trimText('.trim-text-200', 200);
    trimText('.trim-text-250', 250);


    //This only for demo
    //Example of on modal #modal-item-detials scroll down for next options once filled the first one
    $("#Radio1").click(function () {
        $('#modal-item-detials .modal-body').animate({
            scrollTop: $("#Options2").offset().top
        }, 2000);
    });
    //End Example

}
