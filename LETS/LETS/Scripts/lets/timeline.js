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

function openPost(post) {
    var postId = post.id;
    var postOwner = postId.substr(0, postId.indexOf("__"));
    var temp = postOwner.concat("__request__");
    var postDatabaseId = postId.substr(temp.length);
}