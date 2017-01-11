$(document).ready(function () {
    initTypeAhead();
});

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

function SkillsSettings() {
    $("#user_skills_list_edit_button").slideUp();
    $(".user_skills_list").slideUp();
    $.ajax({
        type: "POST",
        url: "GetAddSkillsPartial",
        cache: false,
        success: function (data) {
            $("#user_skills_panel").prepend(data);
            initTypeAhead();
        }
    });
}

function AddSkill() {
    var skill = $("#Skill").val();
    if (skill != null && skill != "") {
        $.ajax({
            type: "POST",
            url: "AddSkill",
            data: { skill: skill },
            cache: false,
            success: function (data) {
                $("#skill_list").remove();
                $("#user_skills_panel").prepend(data);
                $("#Skill").val("");
            }
        });
    }
}

function removeSkill(button) {
    var skill = button.value;
    console.log(skill);
    if (skill != null && skill !== "") {
        $.ajax({
            type: "POST",
            url: "RemoveSkill",
            data: { skill: skill },
            cache: false,
            success: function (data) {
                console.log("success");
            }
        });
    }
}

function UploadDocument() {
    var form = document.getElementById('change_avatar');
    if ($('#change_avatar').valid()) {
        var formdata = new FormData(form); //FormData object
        var fileInput = document.getElementById('fileUpload');
        //Iterating through each files selected in fileInput
        for (i = 0; i < fileInput.files.length; i++) {
            //Appending each file to FormData object
            formdata.append(fileInput.files[i].name, fileInput.files[i]);
        }
        //Creating an XMLHttpRequest and sending
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'ChangeProfilePicture');
        if (xhr.upload) {
            xhr.upload.addEventListener('progress', progressHandlingFunction, false);
        }
        xhr.send(formdata);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200 && xhr.responseURL.indexOf("500") < 0) {
                location.reload();
            } else {
                $('.progress-bar').css('width', '0%').attr('aria-valuenow', 0);
            }
        }
    }
    return false;
}

function progressHandlingFunction(e) {
    if (e.lengthComputable) {
        if (e.lengthComputable) {
            var percentComplete = (e.loaded / e.total) * 100;
            $('.progress-bar').css('width', percentComplete + '%').attr('aria-valuenow', percentComplete);
        }
    }
}

function AddTag() {
    var tag = $("#UserTradingDetails_Request_Tag").val();
    var list = $(".tag").text();
    if (tag != null && tag !== "" && list.indexOf(tag) < 0) {
        $("#NewRequestTags").append("<div class=\"tag request-tag alert alert-info\" role=\"alert\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button><strong>" + tag + "</strong></div>");
    }
    $("#UserTradingDetails_Request_Tag").val("");
}