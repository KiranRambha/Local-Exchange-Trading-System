$(".navbar-toggle").click(function () {
    if ($(".navbar-toggle").hasClass("active")) {
        $(".navbar-toggle").removeClass("active");
    } else {
        $(".navbar-toggle").addClass("active");
    }
});

$(".body-content").click(function () {
    if ($(".navbar-toggle").hasClass("active")) {
        $(".navbar-toggle").removeClass("active");
    }

    if ($(".navbar-collapse").hasClass("in")) {
        $(".navbar-collapse").removeClass("in");
    }
})

$(".footer").click(function () {
    if ($(".navbar-toggle").hasClass("active")) {
        $(".navbar-toggle").removeClass("active");
    }

    if ($(".navbar-collapse").hasClass("in")) {
        $(".navbar-collapse").removeClass("in");
    }
})

var substringMatcher = function (strs) {
    return function findMatches(q, cb) {
        var matches, substringRegex;

        // an array that will be populated with substring matches
        matches = [];

        // regex used to determine if a string contains the substring `q`
        substrRegex = new RegExp(q, 'i');

        // iterate through the pool of strings and for any string that
        // contains the substring `q`, add it to the `matches` array
        $.each(strs, function (i, str) {
            if (substrRegex.test(str)) {
                matches.push(str);
            }
        });

        cb(matches);
    };
};

function initTypeAhead() {
    $('input.type-ahead').each(function () {
        var selectList = $(this).prev();
        var selectOptions = $(selectList).find('option');
        var values = $.map(selectOptions, function (option) {
            return option.value;
        });
        $(this).typeahead({
            hint: true,
            highlight: true,
            minLength: 1
        },
        {
            name: 'text',
            source: substringMatcher(values)
        });
    });
}

function initUserNameTypeAhead() {
    $('input.type-ahead').each(function () {
        var selectList = $(this).prev();
        var selectOptions = $(selectList).find('option');
        var values = $.map(selectOptions, function (option) {
            return option.value;
        });
        $(this).typeahead({
            hint: true,
            highlight: true,
            minLength: 1
        },
        {
            name: 'text',
            source: substringMatcher(values)
        });
    });
}