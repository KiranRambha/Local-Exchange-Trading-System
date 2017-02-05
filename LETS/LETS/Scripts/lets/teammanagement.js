$('#Username.type-ahead').typeahead({
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
    var username = $("#Username").val();
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
                        $("#Username").val("");
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

    formData.push({ name: "TeamName", value: tempFormData[1].value });

    formData.push({ name: "TeamDescription", value: tempFormData[2].value });

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
                    $(".personal_requests").prepend(partialView);
                    CloseNewRequest();
                    $("#spinner_overlay").remove();
                    //$(".new_posted_request").show("slow");
                }
            });
        }
    } else {
        e.preventDefault();
    }
});