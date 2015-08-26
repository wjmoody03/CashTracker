(
    function () {
        var app = angular.module("ct", ["ngResource", "ngRoute", "ui.grid", "ui.grid.resizeColumns", "ui.grid.grouping", 'ui.grid.selection'])
        app.filter('escape', function () {
            return window.encodeURIComponent;
        });

        app.config(function ($routeProvider, $locationProvider) {
            $routeProvider
             .when('/TransactionExplorer/:id', {
                 templateUrl: '/Views/TransactionExplorer/TransactionDetails.html',
                 controller: 'transactionDetailsCtrl'
             })
            .otherwise({templateUrl:'asdf',controller:'f'})
            //.when('/Book/:bookId/ch/:chapterId', {
            //    templateUrl: 'chapter.html',
            //    controller: 'ChapterController'
            //});
        });

    }()
);
