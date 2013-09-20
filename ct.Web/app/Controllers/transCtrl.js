angular.module("ct")
    .controller("transCtrl", function transactionCtrl($scope, Transaction, Categories) {

        $scope.prefs = {
            advancedEdit:false
        };

        $scope.Categories = //["Clothes", "Eating Out", "Gifts", "Groceries", "Jacob's Income", "Miscellaneous", "New York 2013", "Recur.Expense Sav.", "Rent", "Transportation", "Utilities"];
            Categories.query({ StartDate: "6/20/13", EndDate: "7/1/13" });

        $scope.ReimbursableSources = ["Bonus", "Credit Card Points", "Cycling", "Emergency Fund", "Gifts", "House Fund", "Kelly's Bike", "New Car", "Recur.Exp.", "Russell", "School Fund", "Vacation"];

        $scope.transactions = Transaction.query({ StartDate: "6/20/13", EndDate: "7/1/13" });

        $scope.update = function (trans) {
            trans.$update();
        };
    });