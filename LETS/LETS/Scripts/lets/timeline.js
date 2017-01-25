function gotoNextPage() {
    var actualPageNumber = parseInt($(".current_page_number").text());
    $(".current_page_number").text(actualPageNumber + 1);
    $(".previous").removeClass("disabled");
}

function gotoPreviousPage() {
    var actualPageNumber = parseInt($(".current_page_number").text());

    if (actualPageNumber - 1 >= 1) {
        $(".current_page_number").text(actualPageNumber - 1);
    }

    if (actualPageNumber - 1 === 1) {
        $(".previous").addClass("disabled");
    }
}

function CloseNewRequest() {
    resetForm($("#create_request"));
    $("#create_request div").removeClass("success").removeClass("has-error");
    $(".tag").remove();
}

function openPost(post) {
    var postId = post.id;
    var postOwner = postId.substr(0, postId.indexOf("__"));
    var temp = postOwner.concat("__request__");
    var postDatabaseId = postId.substr(temp.length);
    $("#ExpandedRequest").remove();
    $("body").append("<div id = \"spinner_overlay\" class=\"modal-backdrop fade in\"></div>");
    $("body").addClass("modal-open");
    $("#spinner_overlay").html("<div class=\"loading\"><i class=\"fa fa-refresh fa-spin\" aria-hidden=\"true\"></i></div>");
    $.ajax({
        type: "POST",
        url: "ExpandPost",
        data: { username: postOwner, postId: postDatabaseId },
        cache: false,
        success: function (partialView) {
            $(".container.body-content").append(partialView);
            $("#ExpandedRequest").modal("toggle");
            $("#spinner_overlay").remove();
        }
    });
}