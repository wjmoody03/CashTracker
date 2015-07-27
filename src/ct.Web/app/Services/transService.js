angular.module("ct").
       factory("Transaction", function ($resource) {
           return $resource(
               "/api/transaction/:Id",
               {Id:"@TransactionID"},
               //{ StartDate: "1/1/13",EndDate:"1/5/13" },
               {
                   "update": { method: "PUT" }
               }
          );
       });