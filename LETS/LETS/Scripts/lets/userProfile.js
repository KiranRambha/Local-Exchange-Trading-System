﻿function AccountSettings() {
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
            initSkillsTypeAhead();
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
    var form = document.getElementById("change_avatar");
    if ($("#change_avatar").valid()) {
        var formdata = new FormData(form); //FormData object
        var fileInput = document.getElementById("fileUpload");
        //Iterating through each files selected in fileInput
        for (var i = 0; i < fileInput.files.length; i++) {
            //Appending each file to FormData object
            formdata.append(fileInput.files[i].name, fileInput.files[i]);
        }
        //Creating an XMLHttpRequest and sending
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "ChangeProfilePicture");
        if (xhr.upload) {
            xhr.upload.addEventListener("progress", progressHandlingFunction, false);
        }
        xhr.send(formdata);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200 && xhr.responseURL.indexOf("500") < 0) {
                location.reload();
            } else {
                $(".progress-bar").css("width", "0%").attr("aria-valuenow", 0);
            }
        }
    }
    return false;
}

function progressHandlingFunction(e) {
    if (e.lengthComputable) {
        if (e.lengthComputable) {
            var percentComplete = (e.loaded / e.total) * 100;
            $(".progress-bar").css("width", percentComplete + "%").attr("aria-valuenow", percentComplete);
        }
    }
}

function AddTag() {
    var tag = $("#UserTradingDetails_Request_Tag").val();
    var tags = [];

    $(".tag").children("strong").each(function () {
        tags.push(this.innerHTML);
    });

    if (tag != null && tag !== "" && jQuery.inArray(tag, tags) === -1) {
        $("#NewRequestTags").append("<div class=\"tag request-tag alert alert-info\" role=\"alert\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button><strong>" + tag + "</strong></div>");
        tags.push(tag);
    }

    console.log(tags);

    $("#UserTradingDetails_Request_Tag").val("");
}

$("#create_request").on("submit", function (e) {
    var tempFormData = $("#create_request").serializeArray();

    var formData = new Array();

    formData.push({ name: tempFormData[0].name, value: tempFormData[0].value });

    formData.push({ name: "title", value: tempFormData[1].value });

    formData.push({ name: "description", value: tempFormData[2].value });

    formData.push({ name: "budget", value: tempFormData[3].value });

    formData.push({ name: "tag", value: tempFormData[4].value });

    var requestTags = [];

    $(".tag").children("strong").each(function () {
        requestTags.push(this.innerHTML);
    });

    if (requestTags.length > 0) {
        $("#CreateNewRequestModal").modal("toggle");

        $("body").append("<div id = \"spinner_overlay\" class=\"modal-backdrop fade in\"></div>");

        $("body").addClass("modal-open");

        $("#spinner_overlay").html("<div class=\"loading\"><i class='fa fa-refresh fa-spin'></i></div>");

        if ($("#create_request").valid()) {
            formData.push({ name: "tags", value: requestTags });
            formData = jQuery.param(formData);
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "PostRequest",
                data: formData,
                success: function (partialView) {
                    $(".personal_requests").prepend(partialView);
                    resetForm($("#create_request"));
                    $("#create_request div").removeClass("success").removeClass("has-error");
                    $(".tag").remove();
                    $("#spinner_overlay").remove();
                    $(".new_posted_request").show("slow");
                }
            });
        }
    } else {
        e.preventDefault();
    }
});

function resetForm($form) {
    $('#UserTradingDetails_Request_Budget').val(null);
    $form.find("input:text, input:password, input:file, select, textarea").val("");
    $form.find("input:radio, input:checkbox").removeAttr("checked").removeAttr("selected");
}

function CloseNewRequest() {
    resetForm($("#create_request"));
    $("#create_request div").removeClass("success").removeClass("has-error");
    $(".tag").remove();
}

function UserRequestExpand(username, postId) {
    $("#ExpandedRequest").remove();
    $(".bid-accept-reject-modal").remove();
    $("body").append("<div id = \"spinner_overlay\" class=\"modal-backdrop fade in\"></div>");
    $("body").addClass("modal-open");
    $("#spinner_overlay").html("<div class=\"loading\"><i class=\"fa fa-refresh fa-spin\" aria-hidden=\"true\"></i></div>");
    $.ajax({
        type: "POST",
        url: "ExpandPost",
        data: { username: username, postId: postId },
        cache: false,
        success: function (partialView) {
            $(".container.body-content").append(partialView);
            $('[data-toggle="tooltip"]').tooltip();
            $("#ExpandedRequest").modal("toggle");
            $("#spinner_overlay").remove();
        }
    });
}

function BidAcceptRejectModal(username) {
    if ($("#" + username).length !== 0) {
        $("#ExpandedRequest").modal("hide");
        setTimeout(function () {
            $("#" + username).modal("show");
        }, 450);
    }
}

function ShowExpandedRequest(username) {
    $("#" + username).modal("hide");
    setTimeout(function () {
        $("#ExpandedRequest").modal("show");
    }, 450);
}

function AcceptUserBid(postid, username) {
    $.ajax({
        type: "POST",
        url: "AcceptUserBid",
        data: { postId: postid, userName: username },
        cache: false,
        success: function (value) {
            $('.select-bidder').prop('disabled', true);
            $('.select-bidder').addClass("bid-chip");
            $('#' + username + '-bid-chip').prop('disabled', false);
            ShowExpandedRequest(username);
            setTimeout(function () {
                $('.bid-accept-reject-modal').remove();
            }, 450);
        }
    });
}


function initSkillsTypeAhead() {
    $('#Skill.type-ahead').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    },
    {
        limit: 14,
        async: true,
        source: function (query, processSync, processAsync) {
            processSync();
            return $.ajax({
                url: "/Account/GetUserSkills",
                type: 'GET',
                data: { skill: query },
                dataType: 'json',
                success: function (json) {
                    return processAsync(json);
                }
            });
        }
    });
}