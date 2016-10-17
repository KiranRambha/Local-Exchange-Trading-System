$(".navbar-toggle").click(function () {
    if ($(".navbar-toggle").hasClass("active")) {
        $(".navbar-toggle").removeClass("active");
    } else {
        $(".navbar-toggle").addClass("active");
    }
});

$(".body-content").click(function () {
    if ($(".navbar-toggle").hasClass("active")) {
        $(".navbar-toggle").removeClass("active");
    }

    if ($(".navbar-collapse").hasClass("in")) {
        $(".navbar-collapse").removeClass("in");
    }
})

$(".footer").click(function () {
    if ($(".navbar-toggle").hasClass("active")) {
        $(".navbar-toggle").removeClass("active");
    }

    if ($(".navbar-collapse").hasClass("in")) {
        $(".navbar-collapse").removeClass("in");
    }
})

$.validator.setDefaults({
    highlight: function (element) {
        $(element).closest('.form-group').addClass('has-error').removeClass("success");
    },
    unhighlight: function (element) {
        $(element).closest('.form-group').addClass("success").removeClass('has-error');
    },
    errorPlacement: function (error, element) { }
});