﻿angular.module("ct")
    .controller("detailsCtrl", ["transactionsService","$routeParams","$location", detailsCtrl]);

function detailsCtrl(transactionsService,$routeParams,$location) {
    var details = this;
    if (transactionsService.selectedTransaction == null) {
        if ($routeParams.id == "Create") {
            transactionsService.selectedTransaction = new transactionsService.api();
            details.creating = true;
        }
        else {
            //probably means page was refreshed on the details... 
            transactionsService.selectedTransaction = transactionsService.api.get({ id: $routeParams.id });
        }
    }

    details.service = transactionsService;
    details.transaction = transactionsService.selectedTransaction;

    details.save = function () {
        if (details.creating) {
            details.transaction.$save(
                function () {
                    transactionsService.transactions.push(details.transaction);
                }
            );
        }
        else {
            details.transaction.$update();
        }
        $location.path("/TransactionExplorer");
    };

    details.cancel = function () {
        //reload transaction from the server in case they made any changes
        var index = transactionsService.transactions.indexOf(details.transaction);
        transactionsService.api.get({ id: $routeParams.id }, function (data) {
            //gotta be a better way to do this...
            angular.forEach(transactionsService.transactions, function (item, ix) {
                if (item.ID == data.ID) {
                    transactionsService.transactions[ix] = data;
                    console.log('match found');
                }
            })
            $location.path("/TransactionExplorer");
        });
    }

    details.delete = function () {
        details.transaction.$delete(function () {
            var index = transactionsService.transactions.indexOf(details.transaction);
            if (index > -1) {
                transactionsService.transactions.splice(index, 1);
            }
        });
        $location.path("/TransactionExplorer");
    };
}