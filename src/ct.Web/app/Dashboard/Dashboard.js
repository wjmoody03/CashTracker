angular.module("ct")
    .controller("dashboardCtrl", ["$http","$filter", dashboardCtrl]);

function dashboardCtrl($http,$filter) {
    var dashboard = this;

    dashboard.getBalanceSnapshot = function () {
        dashboard.loadingSnapshot = true;
        $http.get("/Dashboard/SnapshotHistory")
            .success(function (result) {
                dashboard.loadingSnapshot = false;
                dashboard.snapshot = result[0];
            });
    }

    dashboard.getBalanceSnapshot();

}