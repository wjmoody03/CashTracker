(
    function () {

        var app = angular.module("ct");
        app.factory("reminderService", ["$resource", remindersService]);

        function remindersService($resource) {

            return $resource("/api/reminder/:id",
                { id: '@ReminderID' },
                { update: { method: "PUT" } }
            );

        }

    }()
);
