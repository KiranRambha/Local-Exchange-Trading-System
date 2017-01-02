var app = angular.module("letsApp", []);

app.controller('letsController', function ($scope) {
    $(":file").filestyle({ buttonText: "Browse", buttonBefore: true, iconName: "fa fa-lg fa-folder-open", buttonName: "btn-default custom-browse-btn", placeholder: "No file selected" });
});