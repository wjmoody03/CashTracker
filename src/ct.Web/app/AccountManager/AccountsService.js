(
    function () {

        var app = angular.module("ct");
        app.factory("accountsService", ["$resource", "$http","$filter", accountsService]);

        function accountsService($resource, $http,$filter) {

            return $resource("/api/Accounts/:AccountID",
                { id: '@AccountID' },
                { update: { method: "PUT" } }
            );

        }

    }()
);
