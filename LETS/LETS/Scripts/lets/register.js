$("#Account_UserName").blur(function () {
    var userName = $("#Account_UserName").val();
    if (userName != null && userName !== "") {
        $.ajax({
            type: "POST",
            url: "CheckUserName",
            data: { userName: userName },
            cache: false,
            success: function (data) {
                if (data === "False") {
                    $("#Account_UserName").closest(".form-group").addClass("has-error").removeClass("success");
                    $('#Account_UserName_Error_Block label span[data-valmsg-for="Account.UserName"]')
                        .addClass("field-validation-error")
                        .removeClass("field-validation-valid")
                        .text("Sorry, This username is not available.");
                }
            }
        });
    }
});