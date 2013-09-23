angular.module("ct")
    .controller("transCtrl", function transactionCtrl($scope, Transaction, Categories, ReimbursableSources, $http, prefs, $filter) {

        $scope.prefs = prefs;

        Categories.query().then(function (data) {
            $scope.categoryList = data;
        });

        $scope.refreshData = function () {
            //console.log(prefs.startDate.format('YYYY-MM-DD'));
            console.log($filter('moment')(prefs.startDate, 'YYYY-MM-DD'));
            $scope.transactions = Transaction.query(
                { 
                    StartDate: moment(prefs.startDate).format('YYYY-MM-DD'),
                    EndDate: moment(prefs.endDate).format('YYYY-MM-DD')
                });
        }

        ReimbursableSources.query().then(function (data) {
            $scope.reimbursableSourceList = data;
        });        

        $scope.update = function (trans) {
            trans.$update();
        };

        $scope.refreshData();
    });