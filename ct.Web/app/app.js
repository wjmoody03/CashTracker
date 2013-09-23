var ct = angular.module("ct", ["ngResource", 'ui.bootstrap'])
    .config(['$routeProvider',function($routeProvider){
        $routeProvider
            .when('/transactions', { templateUrl: '/app/views/Transactions.html' })
            .when('/transaction/edit/:id', { templateUrl: '/app/views/Transaction.html' })
            .otherwise({ redirectTo: '/transactions' });
        ;
    }]);