angular.module("ct")
    .controller("transCtrl", function transactionCtrl($scope, Transaction, Categories, ReimbursableSources, $http) {

        $scope.prefs = {
            advancedEdit:false
        };

        Categories.query().then(function (data) {
            $scope.categoryList = data;
        });

        ReimbursableSources.query().then(function (data) {
            $scope.reimbursableSourceList = data;
        });

        $scope.transactions = Transaction.query({ StartDate: "6/20/13", EndDate: "7/1/13" });

        $scope.update = function (trans) {
            trans.$update();
        };
    });