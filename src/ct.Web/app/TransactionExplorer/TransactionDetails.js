angular.module("ct")
    .controller("detailsCtrl", ["$resource", "uiGridConstants", "$location", detailsCtrl]);

function detailsCtrl($resource, uiGridConstants,$location) {
    var details = this;
    details.message = "heyo";
}