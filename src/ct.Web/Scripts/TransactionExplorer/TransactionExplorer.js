angular.module("ct")
    .controller("explorerCtrl", explorerCtrl);

function explorerCtrl($scope, $resource, uiGridConstants,$location) {

    var api = $resource("/api/Transactions/:id",
            {id: '@ID'},
            { update: { method: "PUT" } }
        );

    $scope.transactions = api.query();

    var rowTemplate = '<div ' +
                        'ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.uid" ' +
                        'ui-grid-one-bind-id-grid="rowRenderIndex + \'-\' + col.uid + \'-cell\'" ' +
                        'class="ui-grid-cell" ' +
                        'ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader, flagged: row.entity.FlagForFollowUp, reimbursable: row.entity.ReimbursableSource }" ' +
                        'role="{{col.isRowHeader ? \'rowheader\' : \'gridcell\'}}" ' +
                        'ui-grid-cell> ' +
                      '</div>';
    $scope.selectAll = function () {
       // $scope.gridApi.selection.selectAllRows();
    };



    $scope.gridOptions = {
        data: "transactions",
        rowTemplate: rowTemplate,
        enableRowSelection: true,
        multiSelect: false,
        enableSelectAll: false,
        enableRowHeaderSelection: false,
        modifierKeysToMultiSelect: false,
        noUnselect: true,
        //onRegisterApi: function( gridApi ) {
        //    $scope.gridApi = gridApi;
        //},
        enableFullRowSelection: true,
        enableFiltering: true,
        enableGridMenu: true,
        showColumnFooter: true,
        columnDefs: [
            {
                name: 'Indicators', enableFiltering: false, displayName: '', field: 'ID', width: '5%', cellTemplate: '<div class="ui-grid-cell-contents" title="TOOLTIP">' +
                    '<i ng-show="row.entity.FlagForFollowUp" class="fa fa-flag text-danger"></i>' +
                    '<i title="{{row.entity.ReimbursableSource}}" ng-show="row.entity.ReimbursableSource" class="fa fa-reply text-success"></i>' +
                '</div>'
            },
            { name: 'TransactionDate', cellFilter: 'date', displayName: 'Date' },
            { name: 'Description' },
            { name: 'Category' },
            { name: 'Amount', cellFilter: 'currency', aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'currency' },
            { name: 'FlagForFollowUp', visible: false, displayName: 'Flagged' },
            { name: 'ReimbursableSource', visible: false, displayName: 'Reimbursable' }
        ]
    };

    $scope.gridOptions.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.gridApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            var msg = 'row selected ' + row.isSelected;
            $location.path('TransactionExplorer/' + row.entity.ID);
            console.log(row);
        });
    };
}