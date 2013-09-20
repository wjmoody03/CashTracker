angular.module("ct").
       factory("Categories", function ($resource) {
           return $resource("/api/category");
       });