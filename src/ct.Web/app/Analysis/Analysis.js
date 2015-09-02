angular.module("ct")
    .controller("analysisCtrl", ["$http", "$filter", "titleService", analysisCtrl]);

function analysisCtrl($http, $filter, titleService) {
    var analysis = this;
    titleService.title = "Analysis";

    analysis.searchParams = {
        startDate: new addMonths(new Date(), -2),
        endDate: new Date()
    };

    analysis.balanceHistory = function () {
        $http.get("/Analysis/BalanceHistory", { params: analysis.searchParams })
            .success(function (data) {
                $('#container').highcharts({
                    chart: {
                        type: 'line'
                    },
                    title: {
                        text: 'Balance History'
                    },
                    xAxis: {
                        categories: ['Apples', 'Bananas', 'Oranges']
                    },
                    yAxis: {
                        title: {
                            text: 'Fruit eaten'
                        }
                    },
                    series: [{
                        name: 'Jane',
                        data: [1, 0, 4]
                    }, {
                        name: 'John',
                        data: [5, 7, 3]
                    }]
                });
            })
    }



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