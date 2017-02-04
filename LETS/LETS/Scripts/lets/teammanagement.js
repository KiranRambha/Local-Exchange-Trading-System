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
                // in this example, json is simply an array of strings
                return processAsync(json);
            }
        });
    }
});