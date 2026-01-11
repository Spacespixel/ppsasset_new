////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// jQuery
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var resizeId;

function bindMobileDropdown() {
    // Remove conflicting jQuery handlers - let the vanilla JS in _Layout.cshtml handle dropdowns
    $(".dropdown-toggle").off("click.mobile");
    $('.dropdown-menu').removeAttr('style');
}

$(document).ready(function($) {
    "use strict";

    sliderHeight();

    if ($(".read-more").length > 0) {
        $(".read-more").each(function() {

            var collapseHeight = parseInt( $(this).attr("data-read-more-collapse-height"), 10);
            if( !collapseHeight ) collapseHeight = 200;

            var moreLink = $(this).attr("data-read-more-more-link");
            if( !moreLink ) moreLink = "Read more";

            var lessLink = $(this).attr("data-read-more-less-link");
            if( !lessLink ) lessLink = "Read less";

            $(this).readmore({
                speed: 500,
                collapsedHeight: collapseHeight,
                moreLink: '<div class="read-more-btn"><a href="#" class="btn btn-framed btn-primary btn-light-frame btn-rounded">' + moreLink + '</a></div>',
                lessLink: '<div class="read-more-btn"><a href="#" class="btn btn-framed btn-primary btn-light-frame btn-rounded">' + lessLink + '</a></div>'
            });
        });
    }

//  Smooth Scroll

    $('.main-nav a[href^="#"], a[href^="#"].scroll').not('a[href*="Index"]').on('click',function (e) {
        e.preventDefault();
        var target = this.hash,
            $target = $(target);
        if ($target.length) {
            $('html, body').stop().animate({
                'scrollTop': $target.offset().top
            }, 2000, 'swing', function () {
                window.location.hash = target;
            });
        }
    });

    function toggleMobileMenu(forceClose) {
        var shouldClose = forceClose === true;
        var $pageWrapper = $(".page-wrapper");
        var $navigation = $(".navigation");

        if (shouldClose) {
            $pageWrapper.removeClass("show-navigation");
            $navigation.removeClass("nav-open show-navigation");
        //    console.log("Mobile menu closed");
        } else {
            var isOpen = $pageWrapper.toggleClass("show-navigation").hasClass("show-navigation");
            $navigation.toggleClass("nav-open", isOpen);
            $navigation.toggleClass("show-navigation", isOpen);
            var navRect = $(".navigation-links").get(0) ? $(".navigation-links").get(0).getBoundingClientRect() : null;
       //     console.log("Mobile menu toggled", { isOpen: isOpen, navRect: navRect });
        }
    }

    // Primary hamburger handler
    $(document).on("click", ".nav-btn", function(e){
        e.preventDefault();
        e.stopPropagation();
        toggleMobileMenu();
    });

    // Close menu when clicking on navigation links (excluding dropdown toggles)
    $(document).on("click", ".navigation-links a:not(.dropdown-toggle)", function(e){
        // Don't prevent default - let the link work normally
        // Just close the mobile menu
        setTimeout(function() {
            toggleMobileMenu(true);
        }, 50);
    });

    // Close menu when tapping outside the navigation
    $(document).on("click", function(e) {
        if (!$(e.target).closest('.navigation, .nav-btn').length) {
            toggleMobileMenu(true);
        }
    });

    // Close menu with Escape key
    $(document).on("keyup", function(e) {
        if (e.key === "Escape") {
            toggleMobileMenu(true);
        }
    });

    // Prevent menu from closing immediately when interacting inside the panel
    $(document).on("click", ".navigation .navigation-links", function(e){
        e.stopPropagation();
    });

    // Dropdown handling removed - now handled by vanilla JS in _Layout.cshtml

    $(window).scroll(function () {
        if ($(window).scrollTop() > 1 ) {
            $(".navigation").addClass("show-background");
        } else {
            $(".navigation").removeClass("show-background");
        }
    });

//  Responsive Video Scaling

    if ($(".video").length > 0) {
        $(this).fitVids();
    }

//  Magnific Popup

    if ($('.image-popup').length > 0) {
        $('.image-popup').magnificPopup({
            type:'image',
            removalDelay: 300,
            mainClass: 'mfp-fade',
            overflowY: 'scroll'
        });
    }

    if ($('.video-popup').length > 0) {
        $('.video-popup').magnificPopup({
            type:'iframe',
            removalDelay: 300,
            mainClass: 'mfp-fade',
            overflowY: 'scroll',
            iframe: {
                markup: '<div class="mfp-iframe-scaler">'+
                    '<div class="mfp-close"></div>'+
                    '<iframe class="mfp-iframe" frameborder="0" allowfullscreen></iframe>'+
                    '</div>',
                patterns: {
                    youtube: {
                        index: 'youtube.com/',
                        id: 'v=',
                        src: '//www.youtube.com/embed/%id%?autoplay=1'
                    },
                    vimeo: {
                        index: 'vimeo.com/',
                        id: '/',
                        src: '//player.vimeo.com/video/%id%?autoplay=1'
                    },
                    gmaps: {
                        index: '//maps.google.',
                        src: '%id%&output=embed'
                    }
                },
                srcAction: 'iframe_src'
            }
        });
    }

    //  Scroll Reveal

    if ( $(window).width() > 768 && $("[data-scroll-reveal]").length ) {
        window.scrollReveal = new scrollReveal();
    }

    //bigGalleryWidth();

    if( $(".count-down").length ){
        var year = parseInt( $(".count-down").attr("data-countdown-year"), 10 );
        var month = parseInt( $(".count-down").attr("data-countdown-month"), 10 ) - 1;
        var day = parseInt( $(".count-down").attr("data-countdown-day"), 10 );
        $(".count-down").countdown({until: new Date(year, month, day), padZeroes: true});
    }

    $("[data-background-color-custom]").each(function() {
        $(this).css( "background-color", $(this).attr("data-background-color-custom") );
    });


    if( $("body").hasClass("links-hover-effect") ){
        $("a, button").each(function() {
            if( !$(this).hasClass("image-popup") && !$(this).hasClass("video-popup") && !$(this).hasClass("arrow-up") && !$(this).hasClass("image") && !$(this).hasClass("no-hover-effect") ){
                $(this).addClass("hover-effect");
                var htmlContent = $(this).html();
                $(this).text("");
                $(this).append("<span><div class='hover-element'>" + htmlContent + "</div><div class='hover-element'>" + htmlContent + "</div></span>");
            }
        });
    }

    if( $("body").hasClass("has-loading-screen") ){
        Pace.on("done", function() {
            $(".page-wrapper").addClass("loading-done");
            setTimeout(function() {
                $(".page-wrapper").addClass("hide-loading-screen");
            }, 500);
            $.each( $("#page-header .animate"), function (i) {
                var $this = $(this);
                setTimeout(function(){
                    $this.addClass("show");
                }, i * 150);
            });
        });
    }
    else {
        $.each( $("#page-header .animate"), function (i) {
            var $this = $(this);
            setTimeout(function(){
                $this.addClass("show");
            }, i * 150);
        });
    }

    $(".bg-transfer").each(function() {
        $(this).css("background-image", "url("+ $(this).find("img").attr("src") +")" );
    });

    $('.modal-body a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var element = $( $(this).attr("href") ).find(".one-item-carousel");
        if( !element.hasClass("owl-carousel") ){
            $(element).owlCarousel({
                autoheight: 1,
                loop: 0,
                margin: 0,
                items: 1,
                nav: 0,
                dots: 1,
                autoHeight: true,
                navText: []
            });
        }
    });

});

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// On Load
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

$(window).load(function(){
    if ( $(document).width() > 768) {
        var $equalHeight = $('.container');
        for( var i=0; i<$equalHeight.length; i++ ){
            equalHeight( $equalHeight );
        }
    }
    initializeOwl();
    centerVerticalNavigation();
});

$(window).resize(function(){
    clearTimeout(resizeId);
    resizeId = setTimeout(doneResizing, 250);

});

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Functions
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Do after resize

function doneResizing(){
    //bigGalleryWidth();
    sliderHeight();
    centerVerticalNavigation();
    // bindMobileDropdown removed - handled by vanilla JS
}

function centerVerticalNavigation(){
    if ( $(document).width() > 768) {
        $(".nav-btn-only .navigation-links").css("padding-top", ($(window).height()/2 - $(".navigation .container").height()) - ($(".nav-btn-only .navigation-links").height()/2) - 40 );
    }
}

function sliderHeight() {

    if( $(".hero-section").find(".container").height() > $(window).height() ){
        var paddingTop = $("nav.navigation").height();
        $(".hero-section .wrapper .hero-title").css("padding-top", paddingTop + "px");
        $(".hero-section").height( $(".hero-section").find(".container").height() + paddingTop );
        $(".hero-section .owl-stage-outer").height( $(".hero-section").children(".wrapper").height() + paddingTop );
      //  console.log("bigger");
    }
    else {
        $(".hero-section").height( $(window).height() );
        $(".hero-section .owl-stage-outer").height( $(window).height() );
    //    console.log("smaller");
    }



	//$(".hero-section").css( "min-height", $(window).height() + "px" );
	//$(".hero-section .owl-stage-outer").css( "min-height", $(window).height() + "px" );
}

function bigGalleryWidth(){
    if ( $(document).width() < 768) {
        $(".big-gallery .slide .container").width( $(document).width() );
    }
}

function initializeOwl(){
    if( $(".owl-carousel").length ){
        $(".owl-carousel").each(function() {

            var items = parseInt( $(this).attr("data-owl-items"), 10);
            if( !items ) items = 1;

            var nav = parseInt( $(this).attr("data-owl-nav"), 10);
            if( !nav ) nav = 0;

            var dots = parseInt( $(this).attr("data-owl-dots"), 10);
            if( !dots ) dots = 0;

            var center = parseInt( $(this).attr("data-owl-center"), 10);
            if( !center ) center = 0;

            var loop = parseInt( $(this).attr("data-owl-loop"), 10);
            if( !loop ) loop = 0;

            var margin = parseInt( $(this).attr("data-owl-margin"), 10);
            if( !margin ) margin = 0;

            var autoWidth = parseInt( $(this).attr("data-owl-auto-width"), 10);
            if( !autoWidth ) autoWidth = 0;

            var navContainer = $(this).attr("data-owl-nav-container");
            if( !navContainer ) navContainer = 0;

            var autoplay = $(this).attr("data-owl-autoplay");
            if( !autoplay ) autoplay = 0;

            var fadeOut = $(this).attr("data-owl-fadeout");
            if( !fadeOut ) fadeOut = 0;
            else fadeOut = "fadeOut";

            var owlInstance = $(this).owlCarousel({
                navContainer: navContainer,
                animateOut: fadeOut,
                autoplaySpeed: 2000,
                autoplay: autoplay,
                autoheight: 1,
                autoWidth: autoWidth,
                items: items,
                center: center,
                loop: loop,
                margin: margin,
                nav: nav,
                dots: dots,
                autoHeight: true,
                navText: [],
                responsive: {
                    0 : {
                        items: 1,
                        autoWidth: false,
                        center: false
                    },
                    768 : {
                        autoWidth: autoWidth,
                        items: items,
                        center: center
                    }
                }
            });

            // Carousel text updates are now handled in Index.cshtml
            // This prevents conflicts and ensures proper timing with page lifecycle
        });

    }
}


function simpleMap(latitude, longitude, markerImage, mapTheme, projectData){
    var element = "map";
    if ( mapTheme == "light" ){
        var mapStyles = [{"featureType":"water","elementType":"geometry.fill","stylers":[{"color":"#d3d3d3"}]},{"featureType":"transit","stylers":[{"color":"#808080"},{"visibility":"off"}]},{"featureType":"road.highway","elementType":"geometry.stroke","stylers":[{"visibility":"on"},{"color":"#b3b3b3"}]},{"featureType":"road.highway","elementType":"geometry.fill","stylers":[{"color":"#ffffff"}]},{"featureType":"road.local","elementType":"geometry.fill","stylers":[{"visibility":"on"},{"color":"#ffffff"},{"weight":1.8}]},{"featureType":"road.local","elementType":"geometry.stroke","stylers":[{"color":"#d7d7d7"}]},{"featureType":"poi","elementType":"geometry.fill","stylers":[{"visibility":"on"},{"color":"#ebebeb"}]},{"featureType":"administrative","elementType":"geometry","stylers":[{"color":"#a7a7a7"}]},{"featureType":"road.arterial","elementType":"geometry.fill","stylers":[{"color":"#ffffff"}]},{"featureType":"road.arterial","elementType":"geometry.fill","stylers":[{"color":"#ffffff"}]},{"featureType":"landscape","elementType":"geometry.fill","stylers":[{"visibility":"on"},{"color":"#efefef"}]},{"featureType":"road","elementType":"labels.text.fill","stylers":[{"color":"#696969"}]},{"featureType":"administrative","elementType":"labels.text.fill","stylers":[{"visibility":"on"},{"color":"#737373"}]},{"featureType":"poi","elementType":"labels.icon","stylers":[{"visibility":"off"}]},{"featureType":"poi","elementType":"labels","stylers":[{"visibility":"off"}]},{"featureType":"road.arterial","elementType":"geometry.stroke","stylers":[{"color":"#d6d6d6"}]},{"featureType":"road","elementType":"labels.icon","stylers":[{"visibility":"off"}]},{},{"featureType":"poi","elementType":"geometry.fill","stylers":[{"color":"#dadada"}]}];
    }
    else if ( mapTheme == "dark" ){
        mapStyles = [{"featureType":"all","elementType":"labels.text.fill","stylers":[{"saturation":36},{"color":"#000000"},{"lightness":40}]},{"featureType":"all","elementType":"labels.text.stroke","stylers":[{"visibility":"on"},{"color":"#000000"},{"lightness":16}]},{"featureType":"all","elementType":"labels.icon","stylers":[{"visibility":"off"}]},{"featureType":"administrative","elementType":"geometry.fill","stylers":[{"color":"#000000"},{"lightness":20}]},{"featureType":"administrative","elementType":"geometry.stroke","stylers":[{"color":"#000000"},{"lightness":17},{"weight":1.2}]},{"featureType":"landscape","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":20}]},{"featureType":"poi","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":21}]},{"featureType":"road.highway","elementType":"geometry.fill","stylers":[{"color":"#000000"},{"lightness":17}]},{"featureType":"road.highway","elementType":"geometry.stroke","stylers":[{"color":"#000000"},{"lightness":29},{"weight":0.2}]},{"featureType":"road.arterial","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":18}]},{"featureType":"road.local","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":16}]},{"featureType":"transit","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":19}]},{"featureType":"water","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":17}]}]
    }
    var mapCenter = new google.maps.LatLng(latitude,longitude);
    var mapOptions = {
        zoom: 13,
        center: mapCenter,
        disableDefaultUI: true,
        scrollwheel: false,
        styles: mapStyles
    };
    var mapElement = document.getElementById(element);
    var map = new google.maps.Map(mapElement, mapOptions);
    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(latitude,longitude),
        map: map,
        icon: markerImage
    });

    // Create custom InfoWindow with project details
    if (projectData) {
        var infoWindowContent = createProjectInfoWindow(projectData);
        var infoWindow = new google.maps.InfoWindow({
            content: infoWindowContent,
            maxWidth: 350
        });

        // Open info window by default
        infoWindow.open(map, marker);

        // Close when clicking the marker again
        marker.addListener('click', function() {
            infoWindow.close();
        });
    }
}

function createProjectInfoWindow(projectData) {
    var html = '<div class="gmap-info-window" style="font-family: Sarabun, sans-serif;">';

    if (projectData.image) {
        html += '<div class="gmap-info-image" style="width: 100%; height: auto; margin-bottom: 10px; border-radius: 4px; overflow: hidden;">';
        html += '<img src="' + projectData.image + '" style="width: 100%; height: auto; object-fit: cover;" />';
        html += '</div>';
    }

    html += '<div class="gmap-info-content" style="padding: 0;">';

    if (projectData.name) {
        html += '<h3 style="margin: 0 0 8px 0; font-size: 16px; font-weight: 600; color: #2c3e50; line-height: 1.3;">' + projectData.name + '</h3>';
    }

    if (projectData.address) {
        html += '<p style="margin: 0 0 10px 0; font-size: 13px; color: #555; line-height: 1.4;">' + projectData.address + '</p>';
    }

    if (projectData.rating) {
        var stars = '';
        for (var i = 0; i < 5; i++) {
            stars += '<span style="color: ' + (i < projectData.rating ? '#FFC107' : '#ddd') + ';">★</span>';
        }
        html += '<div style="margin-bottom: 8px;">';
        html += '<span style="font-size: 13px;">' + stars + ' </span>';
        html += '<span style="font-size: 12px; color: #666;">' + projectData.rating + '/5 (' + (projectData.reviews || 0) + ' รีวิว)</span>';
        html += '</div>';
    }

    if (projectData.description) {
        html += '<p style="margin: 8px 0; font-size: 13px; color: #666; line-height: 1.4;">' + projectData.description + '</p>';
    }

    html += '<a href="' + (projectData.url || '#') + '" style="display: inline-block; margin-top: 8px; padding: 6px 12px; background-color: #FF6B35; color: white; text-decoration: none; border-radius: 3px; font-size: 12px; font-weight: 500;">ดูแผนที่ยาวได้โหลย</a>';

    html += '</div></div>';

    return html;
}

function equalHeight(container){
    var currentTallest = 0,
        currentRowStart = 0,
        rowDivs = new Array(),
        $el,
        topPosition = 0;

    $(container).find(".equal-height").each(function() {
        $el = $(this);
        $($el).height('auto');
        topPostion = $el.position().top;
        if (currentRowStart != topPostion) {
            for (currentDiv = 0 ; currentDiv < rowDivs.length ; currentDiv++) {
                rowDivs[currentDiv].height(currentTallest);
            }
            rowDivs.length = 0; // empty the array
            currentRowStart = topPostion;
            currentTallest = $el.height();
            rowDivs.push($el);
        } else {
            rowDivs.push($el);
            currentTallest = (currentTallest < $el.height()) ? ($el.height()) : (currentTallest);
        }
        for (currentDiv = 0 ; currentDiv < rowDivs.length ; currentDiv++) {
            rowDivs[currentDiv].height(currentTallest);
        }
    });
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LAZY LOADING ANIMATIONS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Initialize Intersection Observer for lazy loading animations
function initLazyLoading() {
    // Add class to body to enable animations
    document.body.classList.add('lazy-animations-enabled');
    
    // Check if Intersection Observer is supported
    if ('IntersectionObserver' in window) {
        
        const observerOptions = {
            root: null,
            rootMargin: '50px 0px -50px 0px', // Trigger 50px before entering viewport
            threshold: [0, 0.1, 0.25, 0.5, 0.75, 1.0] // Multiple thresholds for smooth animations
        };

        // Create intersection observer
        const lazyObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                const element = entry.target;
                
                // Check if element is intersecting and visible enough
                if (entry.isIntersecting && entry.intersectionRatio > 0.1) {
                    
                    // Add loaded class to trigger animation
                    element.classList.add('loaded');
                    
                    // Stop observing this element
                    lazyObserver.unobserve(element);
                    
                    // Dispatch custom event for additional tracking
                    element.dispatchEvent(new CustomEvent('lazyLoaded', {
                        detail: { element: element, ratio: entry.intersectionRatio }
                    }));
                }
            });
        }, observerOptions);

        // Observe all elements with lazy loading classes
        const lazyElements = document.querySelectorAll(`
            .lazy-load, 
            .lazy-fade, 
            .lazy-slide-left, 
            .lazy-slide-right, 
            .lazy-scale, 
            .lazy-stagger, 
            .lazy-text-reveal, 
            .section-animate
        `);

        lazyElements.forEach(element => {
            // Only observe elements that don't already have the 'loaded' class
            if (!element.classList.contains('loaded')) {
                lazyObserver.observe(element);
            }
        });

        // Add special handling for stagger animations
        const staggerGroups = document.querySelectorAll('.lazy-stagger-group');
        staggerGroups.forEach(group => {
            const staggerElements = group.querySelectorAll('.lazy-stagger');
            if (staggerElements.length > 0) {
                // Create a separate observer for stagger groups
                const staggerObserver = new IntersectionObserver((entries) => {
                    entries.forEach(entry => {
                        if (entry.isIntersecting && entry.intersectionRatio > 0.1) {
                            // Trigger stagger animation with delay
                            staggerElements.forEach((element, index) => {
                                setTimeout(() => {
                                    element.classList.add('loaded');
                                }, index * 100); // 100ms delay between each element
                            });
                            staggerObserver.unobserve(entry.target);
                        }
                    });
                }, observerOptions);
                
                staggerObserver.observe(group);
            }
        });

        console.log(`Lazy loading initialized: ${lazyElements.length} elements`);

    } else {
        // Fallback for browsers without Intersection Observer
        console.warn('Intersection Observer not supported, using fallback');
        
        // Add loaded class to all elements immediately
        const fallbackElements = document.querySelectorAll(`
            .lazy-load, 
            .lazy-fade, 
            .lazy-slide-left, 
            .lazy-slide-right, 
            .lazy-scale, 
            .lazy-stagger, 
            .lazy-text-reveal, 
            .section-animate
        `);
        
        fallbackElements.forEach(element => {
            element.classList.add('loaded');
        });
    }
}

// Utility function to manually trigger animations for dynamically added content
function triggerLazyAnimation(selector) {
    const elements = document.querySelectorAll(selector);
    elements.forEach(element => {
        element.classList.add('loaded');
    });
}

// Initialize lazy loading when document is ready
document.addEventListener('DOMContentLoaded', function() {
    initLazyLoading();
});

// Re-initialize if content is dynamically added (for AJAX content)
$(document).on('contentUpdated', function() {
    setTimeout(initLazyLoading, 100);
});
