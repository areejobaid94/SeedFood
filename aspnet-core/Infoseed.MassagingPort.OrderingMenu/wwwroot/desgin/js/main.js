$(document).ready(function () {

//on scroll fixed filter 

$(window).scroll(function(){
    if ($(window).scrollTop() >= 300) {
        $('.filter-slide').addClass('fixed-area');
        
    }
    else {
        $('.filter-slide').removeClass('fixed-area');

    }
});

    //Call Script for inputs spinner + -
     $("input[type='number']").inputSpinner()
   
   
    
         //Call for global srearch
     $("#globalSearch").click(function() {
         $('.global-search').toggleClass('search-view');
         $('.toggle-search-icon').toggleClass('icon-search icon-close');
        });
     
 
       // Level One Catgories
     $('.slider').owlCarousel({
         rtl: true,
         margin:10,
         dots:false,
         nav:true,
         responsive:{
             0:{
                 items:3
             },
 
            600:{
                 items:4
             },
 
            1200:{
                 items:6
             }
         }
     });
 
     // Level Tow Catgories
     $('.filter-slide').owlCarousel({
         center: false,
         loop:false,
         rtl: true,
         margin:10,
         dots:false,
         nav:false,
         responsive:{
             0:{
                 items:3,
              
             },
 
            600:{
                 items:4
             },
 
            1200:{
                 items:10
             }
         }
     });
     
     // Items Slider Items Detials  
     $('.item-img-slide').owlCarousel({
         rtl: true,
         loop:true,
         margin:10,
         nav:false,
         dots:true,
         items:1
 
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
           setTimeout(function() { 
            $('html, body').animate({
             scrollTop: $(".filterBox-connect").offset().top
             
             }, 2000);
         }, 500);
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
     $("#Radio1").click(function() {
         $('#modal-item-detials .modal-body').animate({
             scrollTop: $("#Options2").offset().top
         }, 2000);
     });
     //End Example
 
 });
 
 