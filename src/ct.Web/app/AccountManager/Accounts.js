angular.module("ct")
    .controller("accountsCtrl", ["$resource", "uiGridConstants", "$location", "$scope", "accountsService", accountsCtrl]);

function accountsCtrl($resource, uiGridConstants, $location, $scope, accountsService) {
    var accounts = this;
    accounts.svc = accountsService;
    accounts.accounts = accountsService.query();

    accounts.gridOptions = {
        data: "accounts.accounts",
        enableRowSelection: true,
        multiSelect: false,
        enableSelectAll: false,
        enableRowHeaderSelection: false,
        modifierKeysToMultiSelect: false,
        noUnselect: true,
        enableFullRowSelection: true,
        enableFiltering: true,
        enableGridMenu: true,
        
        columnDefs: [
            { name: 'AccountName', displayName:'Name'},
            { name: 'AccountType', displayName: 'Type' }            
        ]
    };

    accounts.gridOptions.onRegisterApi = function (gridApi) {
        accounts.gridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            accountsService.selectedAccount = row.entity;
            $location.path('/Accounts/' + row.entity.AccountID);
        });
    };

}