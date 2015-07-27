angular.module('ct')
    .factory('prefs', function () {
        return {
            search: "",
            startDate: moment(new Date()).subtract('months',1),
            endDate: moment(),
            advancedEdit: false
        };
    });