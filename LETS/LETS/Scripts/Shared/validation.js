$.validator.setDefaults({
    highlight: function (element) {
        $(element).closest('.form-group').addClass('has-error').removeClass("success");
    },
    unhighlight: function (element) {
        $(element).closest('.form-group').addClass("success").removeClass('has-error');
    },
    errorPlacement: function (error, element) { }
});

$("input").on('blur', function () {
    $(this).valid();
    $("form").validate().element(this);
});

$("textarea").on('blur', function () {
    $(this).valid();
    $("form").validate().element(this);
});

$("select").on('blur', function () {
    $(this).valid();
    $("form").validate().element(this);
});