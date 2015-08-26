angular.module("ct")
    .controller("explorerCtrl", explorerCtrl);

function explorerCtrl($scope, $resource, uiGridConstants) {

    var api = $resource("/api/Transactions/:id",
            {id: '@ID'},
            { update: { method: "PUT" } }
        );

    $scope.transactions = api.query();

    $scope.gridOptions = {
        data:"transactions",
        enableFiltering: true,
        enableGridMenu: true,
        showColumnFooter: true,
        columnDefs: [
            {name:'TransactionDate',cellFilter:'date'},
            { name: 'Description' },
            { name: 'Category' },
            { name: 'Amount', cellFilter: 'currency', aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter:'currency' }
        ]
    }

}