function transactionCtrl($scope, $http){
    $http.get('/api/transaction?StartDate=7/1/13&EndDate=8/1/13').success(function (data) {
        $scope.transactions = data;
    });

    $scope.query = "";
}