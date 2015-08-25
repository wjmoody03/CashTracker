angular.module("ct")
    .controller("scrubberCtrl", scrubberCtrl);

function scrubberCtrl($scope, $http) {
    $scope.loadingTransactions = true;
    $scope.currentTransaction = {};
    $scope.history = [];

    $scope.setCategory = function (category) {
        $scope.fixing = true;
        $scope.currentTransaction.Category = category;
        $scope.history.unshift($scope.currentTransaction);
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
        $scope.currentTransaction.FlagForFollowUp = true;
        $scope.history.unshift($scope.currentTransaction);
        $http.post("/TransactionScrubber/FlagForFollowUp", { transactionID: $scope.currentTransaction.ID, Notes: $scope.currentTransaction.Notes })
            .success(function () {
                $scope.transactions.shift();
                $scope.currentTransaction = $scope.transactions[0];
                $scope.fixing = false;
                window.scrollTo(0, 0)
            });
    };

    $scope.undoCategory = function (trans) {
        $scope.fixing = true;
        $scope.transactions.unshift(trans);
        $scope.currentTransaction = $scope.transactions[0];
        var index = $scope.history.indexOf(trans);
        if (index > -1) {
            $scope.history.splice(index, 1);
        }
        $http.post("/TransactionScrubber/SetCategory", { category: "", transactionID: trans.currentTransaction.ID, Notes: trans.Notes })
            .success(function () {
                $scope.fixing = false;
                window.scrollTo(0, 0)
            });
    };
    $scope.undoFlag = function (trans) {
        $scope.fixing = true;
        $scope.transactions.unshift(trans);
        $scope.currentTransaction = $scope.transactions[0];
        var index = $scope.history.indexOf(trans);
        if (index > -1) {
            $scope.history.splice(index, 1);
        }
        $http.post("/TransactionScrubber/RemoveFlag", { transactionID: trans.currentTransaction.ID})
            .success(function () {
                $scope.fixing = false;
                window.scrollTo(0, 0)
            });
    };
    $scope.unSkip = function (trans) {
        $scope.transactions.unshift(trans);
        $scope.currentTransaction = $scope.transactions[0];
        var index = $scope.history.indexOf(trans);
        if (index > -1) {
            $scope.history.splice(index, 1);
        }
        window.scrollTo(0, 0)
    };

    $scope.skip = function () {
        $scope.history.unshift($scope.currentTransaction);
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