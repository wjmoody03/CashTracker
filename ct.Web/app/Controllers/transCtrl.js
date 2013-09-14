angular.module("ct")
    .controller("transCtrl", function transactionCtrl($scope,Transaction) {
        $scope.transactions = Transaction.query({ StartDate: "6/1/13", EndDate: "7/1/13" });

        $scope.update = function (trans) {
            trans.$update();
        };
    });