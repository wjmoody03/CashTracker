angular.module("ct")
    .controller("explorerCtrl", ["$resource", "uiGridConstants", "$location", "$scope", "transactionsService", "$timeout","titleService", explorerCtrl]);

function explorerCtrl($resource, uiGridConstants, $location, $scope, transactionsService, $timeout,titleService) {
    var explorer = this;
    titleService.title="Transactions";
    explorer.svc = transactionsService;

    //refresh transactions if date params are different: 
    if ($location.search().StartDate) {
        explorer.svc.searchParams.startDate = new Date($location.search().StartDate);
        explorer.svc.query();
    }

    explorer.categoryFilter = $location.search().Category;
    explorer.reimbursableFilter = $location.search().ReimbursableSource;
}