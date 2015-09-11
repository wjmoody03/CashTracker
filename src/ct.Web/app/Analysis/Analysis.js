angular.module("ct")
    .controller("analysisCtrl", ["$http", "$filter", "titleService", analysisCtrl]);

function analysisCtrl($http, $filter, titleService) {
    var analysis = this;
    titleService.title = "Analysis";

    analysis.searchParams = {
        startDate: new addMonths(new Date(), -2),
        endDate: new Date(),
        toParams: function(){
            return { StartDate: $filter('date')(analysis.searchParams.startDate, 'yyyy-MM-dd'), EndDate: $filter('date')(analysis.searchParams.endDate, 'yyyy-MM-dd') };
        }
    };

    analysis.balanceHistory = function () {
        analysis.loadingBalanceHistory = true;
        $http.get("/Analysis/BalanceSnapshotHistory", { params: analysis.searchParams.toParams() })
            .success(function (data) {
                analysis.loadingBalanceHistory = false;
                $('#snapshotHistory').highcharts(data);
            })
    }

    analysis.incomeVsExpenseHistory = function () {
        analysis.loadingIncomeVsExpenseHistory= true;
        $http.get("/Analysis/IncomeVsExpenseHistory", { params: analysis.searchParams.toParams() })
            .success(function (data) {
                analysis.loadingIncomeVsExpenseHistory = false;
                $('#incomeVsExpenseHistory').highcharts(data);
            })
    }

    analysis.refresh = function () {
        analysis.balanceHistory();
        analysis.incomeVsExpenseHistory();
    }

    analysis.refresh();


};



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