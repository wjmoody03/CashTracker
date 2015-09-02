(
    function () {
        var app = angular.module("ct", ["ngResource", "ngRoute", "ui.grid", "ui.grid.resizeColumns", "ui.grid.grouping",
                                            'ui.grid.saveState', 'ui.grid.selection', 'ui.bootstrap', 'ngAnimate'])

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
            .when('/Accounts/:id', {
                templateUrl: '/app/AccountManager/AccountDetails.html',
                controller: 'accountDetailsCtrl',
                controllerAs: 'details'
            })
            .when('/Accounts', {
                templateUrl: '/app/AccountManager/Accounts.html',
                controller: 'accountsCtrl',
                controllerAs: 'accounts'
            })
            .when('/Scrubber', {
                templateUrl: '/app/TransactionScrubber/TransactionScrubber.html',
                controller: 'scrubberCtrl',
                controllerAs: 'scrubber'
            })
            .when('/Analysis', {
                templateUrl: '/app/Analysis/Analysis.html',
                controller: 'analysisCtrl',
                controllerAs: 'analysis'
            })
            .otherwise({
                templateUrl: '/app/Dashboard/Dashboard.html',
                controller: 'dashboardCtrl',
                controllerAs: 'dashboard'
            })
        });

    }()
);
