angular.module("ct")
    .controller("detailsCtrl", ["transactionsService", detailsCtrl]);

function detailsCtrl(transactionsService) {
    var details = this;
    details.transaction = transactionsService.selectedTransaction;
}