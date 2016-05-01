angular.module("ct").directive("transactionGrid", function(){
                return {
                    template: '<div ui-grid="explorer.gridOptions" class="grid" ui-grid-save-state ui-grid-resize-columns ui-grid-grouping ui-grid-selection></div>',
                    restrict: "E",
                    scope: {
                        transactions: "=",
                        categoryFilter: "=",
                        reimbursableFilter: "="
                    },
                    controller: ["$resource","uiGridConstants", "$location", "$scope", 
                            "transactionsService", "$timeout","titleService", transactionGridCtrl]
                }     
            });

function transactionGridCtrl($resource, uiGridConstants, $location, $scope,
    transactionsService, $timeout, titleService) {

    var rowTemplate = '<div ' +
                        'ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.uid" ' +
                        'ui-grid-one-bind-id-grid="rowRenderIndex + \'-\' + col.uid + \'-cell\'" ' +
                        'class="ui-grid-cell" ' +
                        'ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader, flagged: row.entity.FlagForFollowUp, reimbursable: row.entity.ReimbursableSource, split: row.entity.HasBeenSplit }" ' +
                        'role="{{col.isRowHeader ? \'rowheader\' : \'gridcell\'}}" ' +
                        'ui-grid-cell> ' +
                      '</div>';
    $scope.explorer = {};
    $scope.explorer.svc = transactionsService;
    $scope.explorer.gridOptions = {
        data: "transactions",
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
                    '<i title="This is a split transaction." ng-show="row.entity.ParentTransactionID" class="fa fa-code-fork text-primary"></i>' +
                    '<i title="This transaction has been split" ng-show="row.entity.HasBeenSplit" class="fa fa-tasks text-success"></i>' +
                    '<i title="{{row.entity.ReimbursableSource}}" ng-show="row.entity.ReimbursableSource" class="fa fa-reply text-success"></i>' +
                    '<i title="{{row.entity.Notes}}" ng-show="row.entity.Notes" class="fa fa-comment text-info"></i>' +
                '</div>'
            },
            { name: 'AccountName', displayName: 'Account', visible: false },
            { name: 'Month', visible: false },
            { name: 'TransactionTypeDescription', displayName: "Type", visible: false },
            { name: 'TransactionDate', cellFilter: 'date', displayName: 'Date' },
            { name: 'Description' },
            { name: 'Category', filter: { term: $scope.categoryFilter } },
            {
                name: 'Amount', cellFilter: 'currency', aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'currency',                
                customTreeAggregationFinalizerFn: function (aggregation) {
                    aggregation.rendered = aggregation.value;
                }
            },
            { name: 'FlagForFollowUp', visible: false, displayName: 'Flagged' },
            { name: 'ReimbursableSource', visible: $scope.reimbursableFilter!=undefined, displayName: 'Reimbursable', filter: { term: $scope.reimbursableFilter } },
            { name: 'Notes', visible: false, displayName: 'Notes' },
            { name: 'HasBeenSplit', visible: false, displayName: 'Has Been Split' }
        ]
    };

    $scope.explorer.gridOptions.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.explorer.gridApi = gridApi;

        $scope.explorer.restoreGridState();
        $timeout($scope.explorer.restoreGridState, 500); //in case it didn't work the first time

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            transactionsService.explorerState = $scope.explorer.gridApi.saveState.save();
            $scope.explorer.svc.selectedTransaction = row.entity;
            $location.path('/TransactionExplorer/' + row.entity.ID);
        });
    };

    $scope.explorer.restoreGridState = function () {
        if (transactionsService.explorerState) {
            $scope.explorer.gridApi.saveState.restore($scope.explorer, transactionsService.explorerState);
        }
    }

}