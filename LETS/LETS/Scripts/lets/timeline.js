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
    resetForm($("#expanded_request"));
    $("#expanded_request div").removeClass("success").removeClass("has-error");
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
            $.validator.unobtrusive.parse("#ExpandedRequest");
            $("#ExpandedRequest").modal("toggle");
            $("#spinner_overlay").remove();
        }
    });
}

function submitBit() {
    var tempFormData = $("#user_bid_form").serializeArray();
    event.preventDefault();
    if ($('#user_bid_form').valid()) {
        var postId = $("#user_bid_form").attr('class');
        var postOwner = postId.substr(0, postId.indexOf("__"));
        var temp = postOwner.concat("__request__");
        var postDatabaseId = parseInt(postId.substr(temp.length));
        var bid = parseInt(tempFormData[1].value);
        var formData = new Array();
        formData.push({ name: tempFormData[0].name, value: tempFormData[0].value });
        formData.push({ name: "username", value: postOwner });
        formData.push({ name: "postId", value: postDatabaseId });
        formData.push({ name: "bid", value:  bid });
        $.ajax({
            type: "POST",
            url: "PostUserBid",
            data: formData,
            success: function (partialViewResult) {
                $("#no_bids").remove();
                var id = $(partialViewResult).attr('id');
                $("#"+id).slideUp("slow");
                $("#"+id).remove();
                $('.bids_holder').append(partialViewResult);
                $('.new_bid_chip').slideDown("slow");
                $('#Request_Bid').val(null);
                $("#user_bid_form div").removeClass("success").removeClass("has-error");
            }
        });
    }
}


function resetForm($form) {
    $form.find("input:text, input:password, input:file, select, textarea").val("");
    $form.find("input:radio, input:checkbox").removeAttr("checked").removeAttr("selected");
}

function searchPosts() {
    var tempFormData = $("#SearchInputForm").serializeArray();
    try {
        event.preventDefault();
    } catch (err) {
        console.log(err);
    }
    var searchInput = $("#searchinput").val();
    console.log(searchInput);
    var formData = new Array();
    formData.push({ name: tempFormData[0].name, value: tempFormData[0].value });
    formData.push({ name: "searchInput", value: searchInput });
    $.ajax({
        type: "POST",
        url: "SearchPosts",
        data: formData,
        success: function (partialViewResult) {
            console.log(partialViewResult);
            $(".timeline__posts").slideUp("slow");
            $(".timeline__posts").remove();
            $(".timeline__post__list").append(partialViewResult);
        }
    });
}

$('.search-post-input').bind('blur', function () {
    searchPosts();
});