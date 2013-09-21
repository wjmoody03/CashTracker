         
angular.module("ct").factory('Categories', function($http) {
    return {
        query: function(callback) {
            //return ["Clothes", "Eating Out", "Gifts", "Groceries", "Jacob's Income", "Miscellaneous", "New York 2013", "Recur.Expense Sav.", "Rent", "Transportation", "Utilities"];
            return $http.get('/api/category').then(function (result) {
                return result.data;                
            });
        }
    }
});
