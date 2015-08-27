angular.module("ct")
    .controller("explorerCtrl", ["$resource", "uiGridConstants", "$location", "$scope", "transactionsService", explorerCtrl]);

function explorerCtrl($resource, uiGridConstants, $location, $scope, transactionsService) {
    var explorer = this;
    explorer.svc = transactionsService;

    var rowTemplate = '<div ' +
                        'ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.uid" ' +
                        'ui-grid-one-bind-id-grid="rowRenderIndex + \'-\' + col.uid + \'-cell\'" ' +
                        'class="ui-grid-cell" ' +
                        'ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader, flagged: row.entity.FlagForFollowUp, reimbursable: row.entity.ReimbursableSource }" ' +
                        'role="{{col.isRowHeader ? \'rowheader\' : \'gridcell\'}}" ' +
                        'ui-grid-cell> ' +
                      '</div>';
    explorer.selectAll = function () {
        // explorer.gridApi.selection.selectAllRows();
    };


    explorer.monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    explorer.gridOptions = {
        data: "explorer.svc.transactions",
        rowTemplate: rowTemplate,
        enableRowSelection: true,
        multiSelect: false,
        enableSelectAll: false,
        enableRowHeaderSelection: false,
        modifierKeysToMultiSelect: false,
        noUnselect: true,
        //onRegisterApi: function( gridApi ) {
        //    explorer.gridApi = gridApi;
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
            { name: 'Month', visible: false },
            { name: 'TransactionDate', cellFilter: 'date', displayName: 'Date' },
            { name: 'Description' },
            { name: 'Category' },
            {
                name: 'Amount', cellFilter: 'currency', aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'currency',
                customTreeAggregationFinalizerFn: function (aggregation) {
                    aggregation.rendered = aggregation.value;
                }
            },
            { name: 'FlagForFollowUp', visible: false, displayName: 'Flagged' },
            { name: 'ReimbursableSource', visible: false, displayName: 'Reimbursable' },
            { name: 'Notes', visible: false, displayName: 'Notes' }
        ]
    };

    explorer.gridOptions.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        explorer.gridApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            explorer.svc.selectedTransaction = row.entity;
            $location.path('/TransactionExplorer/' + row.entity.ID);
        });
    };
}