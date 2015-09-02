(
    function () {

        var app = angular.module("ct");
        app.factory("transactionsService", ["$resource", "$http","$filter", transactionsService]);

        function transactionsService($resource, $http,$filter) {

            var svc = this;            

            svc.searchParams = {
                startDate: new addMonths(new Date(), -2),
                endDate: new Date()
            };
            svc.transactions = null;
            svc.selectedTransaction = null;
            svc.query = function () {
                svc.api.query({ StartDate: $filter('date')(svc.searchParams.startDate, 'yyyy-MM-dd'), EndDate: $filter('date')(svc.searchParams.endDate, 'yyyy-MM-dd') }, function (data) {
                    svc.transactions = data;
                });
            }

            svc.api = $resource("/api/Transactions/:id",
                { id: '@ID' },
                { update: { method: "PUT" } }
            );

            if (svc.transactions == null) {
                svc.query();
            }

            $http.get("/api/Accounts")
                .success(function (data) {
                    svc.accounts = data;
                });

            $http.get("/api/TransactionType")
                .success(function (data) {
                    svc.transactionTypes = data;
                });

            $http.get("/api/Category")
                .success(function (data) {
                    svc.categories = data;
                });

            return svc;

        }

        function addMonths(date, inc) {
            var day = date.getDate();
            var month = date.getMonth();
            var year = date.getFullYear();

            //first set year
            if (inc > 0) {
                if (month + inc > 12) {
                    year++;
                }
            }
            else {
                if(month-inc<1) {
                    year--;
                }
            }
            month = month + inc;
            return new Date(year, month, day);
        };

    }()
);
