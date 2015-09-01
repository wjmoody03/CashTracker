(
function () {

    var app = angular.module("ct");
    app.factory("titleService", titleService);

    function titleService() {

        var svc = this;
        svc.title = "Home";
        return svc;

    }
}
)();