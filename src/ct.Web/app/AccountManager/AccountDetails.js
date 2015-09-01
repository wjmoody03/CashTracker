angular.module("ct")
    .controller("accountDetailsCtrl", ["accountsService", "$routeParams", "$location","$http","titleService", accountDetailsCtrl]);

function accountDetailsCtrl(accountsService,$routeParams,$location,$http,titleService) {
    var details = this;
    titleService.title = "Account Details";

    if ($routeParams.id == "Create") {
            accountsService.selectedaccount = new accountsService(); 
            details.creating = true;
    }
    else if (accountsService.selectedaccount == null) {
            //probably means page was refreshed on the details... 
            accountsService.selectedaccount = accountsService.get({ id: $routeParams.id });
    }

    details.service = accountsService;
    details.account = accountsService.selectedaccount;

    details.save = function () {
        if (details.creating) {
            details.account.$save(
                { UpdateSensitive: details.updateSensitive },
                function () {
                    accountsService.accounts.push(details.account);
                }
            );
        }
        else {
            details.account.$update({ UpdateSensitive: details.updateSensitive });
        }
        $location.path("/Accounts");
    };

    details.cancel = function () {
        //reload account from the server in case they made any changes
        accountsService.get({ id: $routeParams.id }, function (data) {
            //gotta be a better way to do this...
            angular.forEach(accountsService.accounts, function (item, ix) {
                if (item.AccountID == data.AccountID) {
                    accountsService.accounts[ix] = data;
                    console.log('match found');
                }
            })
            $location.path("/Accounts");
        });
    }

    details.delete = function () {
        details.account.$delete(function () {
            var index = accountsService.accounts.indexOf(details.account);
            if (index > -1) {
                accountsService.accounts.splice(index, 1);
            }
        });
        $location.path("/Accounts");
    };

    details.downloadTransactions = function () {
        details.downloadingTransactions = true;
        $http.post("/Utility/DownloadNewTransactions", { AccountID: details.account.AccountID } )
            .success(function (result) {
                details.downloadResult = result;
                details.downloadingTransactions = false;
            })
    };
}