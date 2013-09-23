angular.module("ct")
    .controller("transCtrl", function transactionCtrl($scope, Transaction, Categories, ReimbursableSources, $http, prefs, $routeParams, $location) {

        $scope.prefs = prefs;

        Categories.query().then(function (data) {
            $scope.categoryList = data;
        });

        $scope.refreshData = function () {

            if ($routeParams.id) {
                $scope.currentTransaction = Transaction.get({ id: $routeParams.id });
            } else {
                $scope.currentTransaction = new Transaction();
                $scope.transactions = Transaction.query(
                    {
                        StartDate: moment(prefs.startDate).format('YYYY-MM-DD'),
                        EndDate: moment(prefs.endDate).format('YYYY-MM-DD')
                    });
            }

        }

        ReimbursableSources.query().then(function (data) {
            $scope.reimbursableSourceList = data;
        });        

        $scope.updateAndReturn = function () {
            $scope.currentTransaction.$update();
            $location.path("/");
        };

        $scope.update = function (trans) {
            trans.$update();
        };

        $scope.refreshData();
    });