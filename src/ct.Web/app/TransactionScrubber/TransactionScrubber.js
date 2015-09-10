angular.module("ct")
    .controller("scrubberCtrl", ["$http", "titleService", scrubberCtrl]);

function scrubberCtrl($http, titleService) {
    var scrubber = this;
    titleService.title = "Scrubber";
    scrubber.loadingTransactions = true;
    scrubber.currentTransaction = {};
    scrubber.history = [];
    scrubber.showReimbursable = false;
    scrubber.showingReimbursable = false;

    scrubber.setCategory = function (category) {
        scrubber.fixing = true;
        scrubber.currentTransaction.Category = category;
        scrubber.history.unshift(scrubber.currentTransaction);
        $http.post("/TransactionScrubber/SetCategory", { category: category, transactionID: scrubber.currentTransaction.ID, Notes: scrubber.currentTransaction.Notes })
            .success(function () {
                if (!scrubber.showReimbursable) {
                    scrubber.transactions.shift();
                    scrubber.currentTransaction = scrubber.transactions[0];
                    scrubber.fixing = false;
                    window.scrollTo(0, 0)
                }
                else {
                    scrubber.showingReimbursable = true;
                }
            });
    };

    scrubber.setReimbursableSource = function (reimbursableSource) {
        scrubber.fixing = true;
        scrubber.currentTransaction.ReimbursableSource = reimbursableSource;
        $http.post("/TransactionScrubber/SetReimbursableSource", { ReimbursableSource: reimbursableSource, transactionID: scrubber.currentTransaction.ID, Notes: scrubber.currentTransaction.Notes })
            .success(function () {
                scrubber.showingReimbursable = false;
                scrubber.transactions.shift();
                scrubber.currentTransaction = scrubber.transactions[0];
                scrubber.fixing = false;
                window.scrollTo(0, 0)
            });
    };

    scrubber.flag = function () {
        scrubber.fixing = true;
        scrubber.currentTransaction.FlagForFollowUp = true;
        scrubber.history.unshift(scrubber.currentTransaction);
        $http.post("/TransactionScrubber/FlagForFollowUp", { transactionID: scrubber.currentTransaction.ID, Notes: scrubber.currentTransaction.Notes })
            .success(function () {
                scrubber.transactions.shift();
                scrubber.currentTransaction = scrubber.transactions[0];
                scrubber.fixing = false;
                window.scrollTo(0, 0)
            });
    };

    scrubber.undoCategory = function (trans) {
        scrubber.fixing = true;
        scrubber.transactions.unshift(trans);
        scrubber.currentTransaction = scrubber.transactions[0];
        var index = scrubber.history.indexOf(trans);
        if (index > -1) {
            scrubber.history.splice(index, 1);
        }
        $http.post("/TransactionScrubber/SetCategory", { category: "", transactionID: trans.ID, Notes: trans.Notes })
            .success(function () {
                $http.post("/TransactionScrubber/SetReimbursableSource", { ReimbursableSource: "", transactionID: trans.ID, Notes: trans.Notes })
                    .success(function () {
                        scrubber.fixing = false;
                        window.scrollTo(0, 0)
                    });
            });
    };
    scrubber.undoFlag = function (trans) {
        scrubber.fixing = true;
        scrubber.transactions.unshift(trans);
        scrubber.currentTransaction = scrubber.transactions[0];
        var index = scrubber.history.indexOf(trans);
        if (index > -1) {
            scrubber.history.splice(index, 1);
        }
        $http.post("/TransactionScrubber/RemoveFlag", { transactionID: trans.ID })
            .success(function () {
                scrubber.fixing = false;
                window.scrollTo(0, 0)
            });
    };
    scrubber.unSkip = function (trans) {
        scrubber.transactions.unshift(trans);
        scrubber.currentTransaction = scrubber.transactions[0];
        var index = scrubber.history.indexOf(trans);
        if (index > -1) {
            scrubber.history.splice(index, 1);
        }
        window.scrollTo(0, 0)
    };

    scrubber.skip = function () {
        scrubber.history.unshift(scrubber.currentTransaction);
        scrubber.transactions.shift();
        scrubber.currentTransaction = scrubber.transactions[0];
    };

    $http.get("/TransactionScrubber/TransactionsNeedingAttention")
        .success(function (data) {
            scrubber.loadingTransactions = false;
            scrubber.transactions = data;
            scrubber.currentTransaction = data[0];
        });

    $http.get("/TransactionScrubber/ProbableCategories")
        .success(function (data) {
            scrubber.categories = data;
        })

    $http.get("/TransactionScrubber/ProbableReimbursableSources")
        .success(function (data) {
            scrubber.reimbursableSources = data;
    })

};