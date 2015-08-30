angular.module("ct")
    .controller("dashboardCtrl", ["$http", "$filter", dashboardCtrl]);

function dashboardCtrl($http, $filter) {
    var dashboard = this;

    dashboard.getBalanceSnapshot = function () {
        dashboard.loadingSnapshot = true;
        $http.get("/Dashboard/SnapshotHistory")
            .success(function (result) {
                dashboard.loadingSnapshot = false;
                dashboard.snapshot = result[0];
            });
    }

    dashboard.getFlagged = function () {
        dashboard.loadingFlagged = true;
        $http.get("/Dashboard/FlaggedTransactions")
            .success(function (result) {
                dashboard.loadingFlagged = false;
                dashboard.flagged = result;
            });
    };

    dashboard.getReimbursables = function () {
        dashboard.loadingReimbursables = true;
        $http.get("/Dashboard/ReimbursableBalances")
            .success(function (result) {
                dashboard.loadingReimbursables = false;
                dashboard.reimbursables = result;
            });
    };

    dashboard.getReimbursables();
    dashboard.getFlagged();
    dashboard.getBalanceSnapshot();

}