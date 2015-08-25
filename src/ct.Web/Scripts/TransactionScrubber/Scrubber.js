angular.module("ct")
    .controller("scrubberCtrl", scrubberCtrl);

function scrubberCtrl($scope, $http) {
    $scope.loadingTransactions = true;
    $scope.currentTransaction = {};

    $scope.setCategory = function (category) {
        $scope.fixing = true;
        $http.post("/TransactionScrubber/SetCategory", { category: category, transactionID: $scope.currentTransaction.ID, Notes: $scope.currentTransaction.Notes })
            .success(function () {
                $scope.transactions.shift();
                $scope.currentTransaction = $scope.transactions[0];
                $scope.fixing = false;
                window.scrollTo(0, 0)
            });
    };
    $scope.flag = function () {
        $scope.fixing = true;
        $http.post("/TransactionScrubber/FlagForFollowUp", { transactionID: $scope.currentTransaction.ID, Notes: $scope.currentTransaction.Notes })
            .success(function () {
                $scope.transactions.shift();
                $scope.currentTransaction = $scope.transactions[0];
                $scope.fixing = false;
                window.scrollTo(0, 0)
            });
    };

    $scope.skip = function () {
        $scope.transactions.shift();
        $scope.currentTransaction = $scope.transactions[0];
    };

    $http.get("/TransactionScrubber/TransactionsNeedingAttention")
        .success(function (data) {
            $scope.loadingTransactions = false;
            $scope.transactions = data;
            $scope.currentTransaction = data[0];
        });

    $http.get("/TransactionScrubber/ProbableCategories")
        .success(function (data) {
            $scope.categories = data;
        })

};