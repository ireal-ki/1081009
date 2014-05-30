jQuery(document).ready(function($){

	// Slide Menu Left & Right

	$('main').on('click', function(){
		if($('main').hasClass('slideRightOut')) {
			$('main').removeClass('slideRightOut').addClass('slideIn');
			$('aside#search').removeClass('show');
			$('#search input[type=search]').blur();
			$('body').removeClass('sliderOpen');
			return false;
		} if ($('main').hasClass('slideLeftOut')) {
			$('main').removeClass('slideLeftOut').addClass('slideIn');
			$('aside#nav').removeClass('show');
			$('#search input[type=search]').blur();
			$('body').removeClass('sliderOpen');
			return false;
		}
		return true;
	});


	// End of Slide menu Left Right

	// Start Slide menu Top

	$('#btn-forget_password').on('click', function(){
		$('#forget_password').show();
		$('body').addClass('show-overlay');
		$('.close-overlay, #overlay').on('click', function(){
			$('body').removeClass('show-overlay');
		})
	})

	$('#btnShare').on('click', function () {

	    // will hide _contentWebBrowser
	    if (window.external)
	        window.external.notify("hide_contentWebBrowser");

		$('#share').show();
		$('body').addClass('show-overlay');
		$('.close-overlay, #overlay').on('click', function(){
		    $('body').removeClass('show-overlay');

		    // will hide _contentWebBrowser
		    if (window.external)
		        window.external.notify("show_contentWebBrowser");
		})
	})


	var navList = $('#nav-list');

	$('.header-list').on('click', function() {
		if($(this).hasClass('active')) {
			$(this).removeClass('active');
			navList.removeClass('slideTopIn');
		} else {
			$(this).addClass('active');
			navList.addClass('slideTopIn');
		}
		return false;
	})


	// Disable click before login
	$('.disable a').on('click touch', function() {
		return false;
	})

	// Clear Search
	$('.icon-clear').on('click touch',function(){
	   $(this).parent().find('input').val('');
	});

	// Append meta viewport in iframe

	$('#webviewFrame').contents().find("head").append('<meta name="viewport" content="width=1024">');

	// $('main.feed').find('#btnHome').addClass('active');


	// Grid
	$("article.grid").gridalicious({
		gutter: 8,
		selector: 'figure',
		width: 160,
		animate: true
	});
	$(window).setBreakpoints({
// use only largest available vs use all available
    distinct: true,
// array of widths in pixels where breakpoints
// should be triggered
    breakpoints: [
        320,
        480,
        768,
        1024
    ]
});

$(window).bind('enterBreakpoint768',function() {
    $("article.grid").gridalicious({
			gutter: 10,
			selector: 'figure',
			width: 240,
			animate: true
		});
});
$(window).bind('enterBreakpoint1024',function() {
    $("article.grid").gridalicious({
			gutter: 12,
			selector: 'figure',
			width: 240,
			animate: true
		});
});

// End of Grid

	var footer = $('footer');
	$('#btn-expand').on('click', function() {
		if(footer.hasClass('show')) {
			footer.removeClass('show');
		} else {
			footer.addClass('show');
		}
	})

	// Navigation
	$('#btnHome').on('click', function(){
		$('main').hide();
		$('main.feed').show(function(){
			$("article.grid").gridalicious({
				gutter: 8,
				selector: 'figure',
				width: 160,
				animate: true
			});
		});
	})

	$('#btn-top10').on('click', function(){
		$('main').hide();
		$('main.intro').show();
	})

	$('#btnSearch').on('click', function(){
		$('main').hide();
		$('main.search').show();
	})

	$('#btn-story').on('click', function(){
		$('main').hide();
		$('main.menulist').show();
	})

	var main = $('main');
	main.on('click', function(){
		if($(this).hasClass('first')) {
				$(this).removeClass('first');
		}
	})

});