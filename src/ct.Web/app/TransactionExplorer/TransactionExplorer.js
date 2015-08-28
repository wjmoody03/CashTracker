angular.module("ct")
    .controller("explorerCtrl", ["$resource", "uiGridConstants", "$location", "$scope", "transactionsService", "$timeout", explorerCtrl]);

function explorerCtrl($resource, uiGridConstants, $location, $scope, transactionsService, $timeout) {
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
        enableFullRowSelection: true,
        enableFiltering: true,
        enableGridMenu: true,
        showColumnFooter: true,
        //state save prefs:
        saveFocus: true,
        saveScroll: true,
        saveSelection: false,
        saveGroupingExpandedStates: true,
        saveWidths: true,
        saveOrder: true,
        saveVisible: true,
        saveSort: true,
        saveFilter: true,
        savePinning: true,
        saveGrouping: true,
        saveTreeView: true,

        columnDefs: [
            {
                name: 'Indicators', enableFiltering: false, displayName: '', field: 'ID', width: '5%', cellTemplate: '<div class="ui-grid-cell-contents text-center" title="TOOLTIP">' +
                    '<i ng-show="row.entity.FlagForFollowUp" class="fa fa-flag text-danger"></i>' +
                    '<i title="{{row.entity.ReimbursableSource}}" ng-show="row.entity.ReimbursableSource" class="fa fa-reply text-success"></i>' +
                    '<i title="{{row.entity.Notes}}" ng-show="row.entity.Notes" class="fa fa-comment text-info"></i>' +
                '</div>'
            },
            { name: 'AccountName', displayName: 'Account', visible: false },
            { name: 'Month', visible: false },
            { name: 'TransactionTypeDescription', displayName: "Type", visible: false },
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

        explorer.restoreGridState();
        $timeout(explorer.restoreGridState, 500); //in case it didn't work the first time

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            transactionsService.explorerState = explorer.gridApi.saveState.save();
            explorer.svc.selectedTransaction = row.entity;
            $location.path('/TransactionExplorer/' + row.entity.ID);
        });
    };

    explorer.restoreGridState = function () {
        if (transactionsService.explorerState) {
            explorer.gridApi.saveState.restore(explorer, transactionsService.explorerState);
        }
    }
}