var teamID = "";

$('.type-ahead').typeahead({
    hint: true,
    highlight: true,
    minLength: 1
},
{
    limit: 12,
    async: true,
    source: function (query, processSync, processAsync) {
        processSync();
        return $.ajax({
            url: "/Home/GetUserNames",
            type: 'GET',
            data: { username: query },
            dataType: 'json',
            success: function (json) {
                return processAsync(json);
            }
        });
    }
});

function AddUser() {
    var username = $("#Team_Username").val();
    if (username != null && username !== "") {
        $.ajax({
            type: "POST",
            url: "AddUser",
            data: { username: username },
            cache: false,
            success: function (data) {
                if (data != null && data !== "") {
                    var id = $(data).attr("id");
                    if (!$("#" + id).length) {
                        $(".added-team-members-list").append(data);
                        $("#Team_Username").val("");
                    }
                }
            }
        });
    }
}

function removeAddedTeamUser(id) {
    $(id).remove();
}

function CloseNewRequest() {
    resetForm($("#create_team"));
    $("#create_team div").removeClass("success").removeClass("has-error");
    $(".team-addition-chip").remove();
}

function resetForm($form) {
    $form.find("input:text, input:password, input:file, select, textarea").val("");
}

$("#create_team").on("submit", function (e) {
    var tempFormData = $("#create_team").serializeArray();

    var formData = new Array();

    formData.push({ name: tempFormData[0].name, value: tempFormData[0].value });

    formData.push({ name: "teamName", value: tempFormData[1].value });

    var teamMembers = [];

    $(".added-team-members-list").children(".team-addition-chip").each(function () {
        teamMembers.push($(this).children(".mdl-chip__text")[0].innerHTML);
    });

    if (teamMembers.length > 0) {
        $("#CreateNewTeamModal").modal("toggle");

        $("body").append("<div id = \"spinner_overlay\" class=\"modal-backdrop fade in\"></div>");

        $("body").addClass("modal-open");

        $("#spinner_overlay").html("<div class=\"loading\"><i class='fa fa-refresh fa-spin'></i></div>");

        if ($("#create_team").valid()) {
            formData.push({ name: "teamMembers", value: teamMembers });
            formData = jQuery.param(formData);
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "CreateTeamRequest",
                data: formData,
                success: function (partialView) {
                    $("#yourteams").prepend(partialView);
                    CloseNewRequest();
                    $("#spinner_overlay").remove();
                    $(".new_posted_request").show("slow");
                }
            });
        }
    } else {
        e.preventDefault();
    }
});

function ExpandMessageBox(id) {
    var formData = new Array();
    formData.push({ name: "teamId", value: id });
    $.ajax({
        type: "POST",
        url: "GetLatestMessages",
        data: formData,
        success: function (partialView) {
            $("#" + id.replace("-messagebox", "-message_canvas")).empty();
            $("#" + id.replace("-messagebox", "-message_canvas")).append(partialView);
            $("#" + id).toggle("slow");
            if ($("#" + id + "-expand-btn").hasClass("togglehide")) {
                $("#" + id + "-expand-btn").removeClass("togglehide");
                $("#" + id + "-collapse-btn").addClass("togglehide");
            } else {
                $("#" + id + "-expand-btn").addClass("togglehide");
                $("#" + id + "-collapse-btn").removeClass("togglehide");
            }
            var tempId = id.replace("-messagebox", "");
            $("#" + tempId + "-message_canvas").scrollTop($("#" + tempId + "-message_canvas")[0].scrollHeight);
            sendMessage(tempId);
        }
    });
}

$("#create_team").on("submit", function (e) {
    e.preventDefault();
});

$('.message_form').on('submit', function (e) {
    e.preventDefault();
});

function sendMessage(id) {
    try {
        event.preventDefault();
    } catch (err) {
        console.log(err);
    }

    teamID = id;
    var chat = $.connection.chatHub;

    chat.client.addNewMessageToPage = function (name, message) {
        if (name === $("#Authenticated-User").text()) {
            $("#" + teamID + "-message_canvas").append("<div class='col-xs-12 custom-message-padding'><div class='mdl-chip message-chip width-mobile-100 pull-right my-message'><div class='mdl-chip__text'><p class='mg-0'><strong>" + htmlEncode(name) + "</strong></p><p class='mg-0 custom_message_size'>" + htmlEncode(message) + "</p></div></div></div>");
        } else {
            $("#" + teamID + "-message_canvas").append("<div class='col-xs-12 custom-message-padding'><div class='mdl-chip message-chip width-mobile-100'><div class='mdl-chip__text'><strong><p class='mg-0 custom_message_size'>" + htmlEncode(name) + "</strong></p><p class='mg-0 custom_message_size'>" + htmlEncode(message) + "</p></div></div></div>");
        }
        $("#" + teamID + "-message_canvas").scrollTop($("#" + teamID + "-message_canvas")[0].scrollHeight);
    };

    $("#" + id + "-Team-Message").focus();

    // Start the connection.
    $.connection.hub.start().done(function () {
        var name = $("#Authenticated-User").text();
        var message = $("#" + teamID + "-Team-Message").val();
        if (name != null && message != null && name !== "" && message !== "") {
            // Call the Send method on the hub.
            chat.server.send(name, message, teamID);
            // Clear text box and reset focus for next comment.
            $("#" + teamID + "-Team-Message").val("").focus();
        }
    });

    function htmlEncode(value) {
        var encodedValue = $("<div />").text(value).html();
        return encodedValue;
    }
}

function DeleteTeam(id) {
    var formData = new Array();

    formData.push({ name: "teamId", value: id });
    $.ajax({
        type: "POST",
        url: "DeleteTeam",
        data: formData,
        success: function (partialView) {
            $("#" + id + "-team").hide("slow");
        }
    });
}

$('#yourteams').children('.my-team').each(function () {
    var tempId = this.children[0].id;
    var id = tempId.replace("-team", "");
});