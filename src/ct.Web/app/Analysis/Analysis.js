angular.module("ct")
    .controller("analysisCtrl", ["$http", "$filter", "titleService", analysisCtrl]);

function analysisCtrl($http, $filter, titleService) {
    var analysis = this;
    titleService.title = "Analysis";

    //default chart visibility
    analysis.showSnapshotHistory = false;
    analysis.showIncomeExpenseHistory = true;
    analysis.showCategoryHistory = true;
    analysis.showUncreconciledHistory = false;

    //grid configs: 
    analysis.categoryGridOptions = {
        data: "analysis.categorySummary",
        enableFiltering: true,
        enableGridMenu: true,
        columnDefs: [
            { name: 'category', displayName: 'Category' },
            {
                name: 'AvgOver', enableFiltering: true, displayName: 'Avg Over Budget', cellTemplate:
                '<div class="ui-grid-cell-contents text-center" title="TOOLTIP">' +
                    '<i ng-show="row.entity.budgeted > row.entity.avg" class="fa fa-check text-success"></i>' +
                    '<i ng-show="row.entity.budgeted < row.entity.avg" class="fa fa-warning text-warning"></i>' +
                    '<span>{{row.entity.avg-row.entity.budgeted | currency}}</span>' + 
                '</div>'
            },
            { name: 'budgeted', displayName: 'Budgeted', cellFilter: 'currency' },
            { name: 'avg', displayName: 'Average', cellFilter: 'currency' },
            { name: 'min', displayName: 'Min', cellFilter: 'currency' },
            { name: 'max', displayName: 'Max', cellFilter: 'currency' },
            { name: 'total', displayName: 'Total', cellFilter: 'currency' }
        ]        
    };

    analysis.searchParams = {
        startDate: new addMonths(new Date(), -2),
        endDate: new Date(),
        toParams: function () {
            return { StartDate: $filter('date')(analysis.searchParams.startDate, 'yyyy-MM-dd'), EndDate: $filter('date')(analysis.searchParams.endDate, 'yyyy-MM-dd') };
        }
    };

    analysis.balanceHistory = function () {
        if (analysis.showBalanceHistory) {
            analysis.loadingBalanceHistory = true;
            $http.get("/Analysis/BalanceSnapshotHistory", { params: analysis.searchParams.toParams() })
                .success(function (data) {
                    analysis.loadingBalanceHistory = false;
                    $('#snapshotHistory').highcharts(data);
                })
        }
    }

    analysis.incomeVsExpenseHistory = function () {
        if (analysis.showIncomeExpenseHistory) {
            analysis.loadingIncomeVsExpenseHistory = true;
            $http.get("/Analysis/IncomeVsExpenseHistory", { params: analysis.searchParams.toParams() })
                .success(function (data) {
                    analysis.loadingIncomeVsExpenseHistory = false;
                    analysis.incomeVsExpenseSummary = data.summaryData;
                    $('#incomeVsExpenseHistory').highcharts(data);
                })
        }
    }

    analysis.categoryHistory = function () {
        if (analysis.showCategoryHistory) {
            analysis.loadingCategoryHistory = true;
            $http.get("/Analysis/CategoryHistory", { params: analysis.searchParams.toParams() })
                .success(function (data) {
                    analysis.loadingCategoryHistory = false;
                    $('#categoryHistory').highcharts(data);
                    analysis.categorySummary = data.summaryTable;
                })
        }
    }

    analysis.unreconciledHistory = function () {
        if (analysis.showUncreconciledHistory) {
            //analysis.loadingCategoryHistory = true;
            //$http.get("/Analysis/CategoryHistory", { params: analysis.searchParams.toParams() })
            //    .success(function (data) {
            //        analysis.loadingCategoryHistory = false;
            //        $('#categoryHistory').highcharts(data);
            //    })
        }
    }

    analysis.refresh = function () {
        analysis.balanceHistory();
        analysis.incomeVsExpenseHistory();
        analysis.categoryHistory();
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
        if (month - inc < 1) {
            year--;
        }
    }
    month = month + inc;
    return new Date(year, month, day);
};