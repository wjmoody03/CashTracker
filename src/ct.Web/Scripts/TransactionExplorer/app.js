(
    function () {
        var app = angular.module("ct", ["ngResource", "ngRoute", "ui.grid", "ui.grid.resizeColumns", "ui.grid.grouping"])
        app.filter('escape', function () {
            return window.encodeURIComponent;
        });
    }()
);
