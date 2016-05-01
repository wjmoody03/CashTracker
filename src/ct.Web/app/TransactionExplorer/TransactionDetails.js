angular.module("ct")
    .controller("detailsCtrl", ["transactionsService", "$routeParams", "$location", "titleService", detailsCtrl]);

function detailsCtrl(transactionsService, $routeParams, $location, titleService) {
    var details = this;
    titleService.title = "Transaction Details";
    details.CreateMode = $routeParams.id == "Create";
    details.RedirectToParent = $routeParams.redirectToParent;

    if (details.CreateMode) {
        details.transaction = new transactionsService.api();
        if ($routeParams.parentTransactionID) {
            details.transaction.ParentTransactionID = $routeParams.parentTransactionID;
        }
        details.creating = true;
    }
    else {
        details.transaction = transactionsService.api.get({ id: $routeParams.id });
    }

    details.service = transactionsService;
    details.save = function () {
        if (details.creating) {
            details.transaction.$save(
                function () {
                    transactionsService.transactions.push(details.transaction);
                    details.redirect();
                }
            );
        }
        else {
            details.transaction.$update(function () {
                details.redirect();
            });
        }
    };

    details.redirect = function () {
        if (details.RedirectToParent) {
            $location.path("/TransactionExplorer/" + details.transaction.ParentTransactionID);
        }
        else {
            $location.path("/TransactionExplorer");
        }
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
            if (details.RedirectToParent) {
                $location.path("/TransactionExplorer/" + $routeParams.parentTransactionID);
            }
            else {
                $location.path("/TransactionExplorer");
            }
        });


    };
}