angular.module("ct")
    .controller("titleCtrl", ["titleService", titleCtrl]);

function titleCtrl(titleService) {
    var title = this;
    title.svc = titleService;
}
