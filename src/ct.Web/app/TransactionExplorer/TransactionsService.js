(
    function () {

        var app = angular.module("ct");
        app.factory("transactionsService", ["$resource","$http", transactionsService]);

        function transactionsService($resource,$http) {

            var svc = this;

            svc.searchParams = {};
            svc.transactions = null;
            svc.selectedTransaction = null;
            svc.query = function () {
                svc.api.query(svc.searchParams, function (data) {
                    svc.transactions = data;
                });
            }
            
            svc.api = $resource("/api/Transactions/:id",
                { id: '@ID' },
                { update: { method: "PUT" } }
            );

            if(svc.transactions==null)
            {
                svc.query();
            }

            $http.get("/api/Account")
                .success(function (data) {
                    svc.accounts = data;
                });

            $http.get("/api/TransactionType")
                .success(function (data) {
                    svc.transactionTypes = data;
                });

            return svc;

        }

    }()
);
