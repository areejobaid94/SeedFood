$(document).ready(function () {

    //SideMenu Toggle
    $(".ToggleMenu").click(function() {
       $('#sidebars').toggleClass('toggle');
      
      });
      //SideMenu Toggle close on click anywhere
      $(document).mouseup(function(e) 
        {
      var container = $(".sidebar.toggle");
      if (!container.is(e.target) && container.has(e.target).length === 0) 
      {
          container.toggleClass('toggle');
      }
      
    });
    
    $("#globalSearch").click(function() {
        $('.global-search').toggleClass('search-view');
        $('.toggle-search-icon').toggleClass('icon-search icon-close');
       });
    

    $("input[type='number']").inputSpinner()

    
    $('.slider').owlCarousel({
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


    $('.filter-slide').owlCarousel({
        center: false,
        loop:false,
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

    $('.item-img-slide').owlCarousel({
        loop:true,
        margin:10,
        nav:false,
        dots:true,
        items:1

    })
    

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
});

