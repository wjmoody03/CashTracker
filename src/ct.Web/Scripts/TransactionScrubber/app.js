(
    function () {
        var app = angular.module("ct", [])
        app.filter('escape', function () {
            return window.encodeURIComponent;
        });
    }()
);
