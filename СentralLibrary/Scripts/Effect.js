$("document").ready(function () {
    $(".card").mouseover(function () {
        var i = $(this).index();
        $(".border-effect").eq(i).animate({ width: "100%" }, 200);
        $(this).find("img").css({
            '-moz-transform': 'scale(1.1)',
            '-webkit-transform': 'scale(1.1)',
            'transform': 'scale(1.1)'
        })
    })

    $(".card").mouseleave(function () {
        var i = $(this).index();
        $(".border-effect").eq(i).animate({ width: "20%" }, 200);
        $(this).find("img").css({
            '-moz-transform': 'scale(1.0)',
            '-webkit-transform': 'scale(1.0)',
            'transform': 'scale(1.0)'
        })
    })
})