angular.module("ct").factory('ReimbursableSources', function ($http) {
    return {
        query: function (callback) {
            return $http.get('/api/reimbursablesource').then(function (result) {
                return result.data;
            }); 
        }
    }
});
