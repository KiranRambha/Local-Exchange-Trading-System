var changeProfile = function (element) {
    if ($(element).hasClass("account_overview_toggle") && !$(element).hasClass("active")) {
        $(element).addClass("active");
        $(".account_settings_toggle").removeClass("active");
        $("#account_overview_view").addClass("show").removeClass("hide");
        $("#account_settings_view").removeClass("show").addClass("hide");
    }

    if ($(element).hasClass("account_settings_toggle") && !$(element).hasClass("active")) {
        $(element).addClass("active");
        $(".account_overview_toggle").removeClass("active");
        $("#account_overview_view").addClass("hide").removeClass("show");
        $("#account_settings_view").addClass("show").removeClass("hide");
    }
};

function AccountSettings() {
    $("#account_settings_edit_button").slideUp();
    $(".user_details_table").slideUp();
    $.ajax({
        type: "POST",
        url: "GetAccountSettingsPartial",
        cache: false,
        success: function (data) {
            $("#user_panel").prepend(data);
            $("#account_settings").valid();
        }
    });
}

function CancelAccountSettingsEdit() {
    $("#account_settings").slideUp();
    $("#account_settings_edit_button").slideDown();
    $(".user_details_table").slideDown();
    $("#account_settings").remove();
}