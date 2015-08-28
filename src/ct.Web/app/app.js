(
    function () {
        var app = angular.module("ct", ["ngResource", "ngRoute", "ui.grid", "ui.grid.resizeColumns", "ui.grid.grouping", 'ui.grid.saveState', 'ui.grid.selection','ui.bootstrap'])

        app.filter('escape', function () {
            return window.encodeURIComponent;
        });

        app.config(function ($routeProvider, $locationProvider) {
            $routeProvider
             .when('/TransactionExplorer/:id', {
                 templateUrl: '/app/TransactionExplorer/TransactionDetails.html',
                 controller: 'detailsCtrl',
                 controllerAs: 'details'
             })
            .when('/TransactionExplorer', {
                templateUrl: '/app/TransactionExplorer/TransactionExplorer.html',
                controller: 'explorerCtrl',
                controllerAs: 'explorer'
            })
            .when('/Scrubber', {
                templateUrl: '/app/TransactionScrubber/TransactionScrubber.html',
                controller: 'scrubberCtrl',
                controllerAs: 'scrubber'
            })
            .otherwise({
                templateUrl: '/app/Dashboard/Index.html', 
                controller: 'dashboardCtrl'
            })
        });

    }()
);
