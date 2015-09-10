angular.module("ct")
    .controller("manualImportCtrl", ['$scope', '$http', '$upload', manualImportCtrl]);


function manualImportCtrl ($scope, $http, $upload) {
  
    $scope.file = null;
    $scope.result = null;

    $scope.onFileSelect = function ($files) {
        //$files: an array of files selected, each file has name, size, and type.
        if ($files.length > 1) {
            toastr.error("Please select a single file!");
            return;
        }
        $scope.file = $files[0];
    };

    $scope.go = function(){

        if ($scope.file==null) {
            alert("Please select a file!");
            return;
        }

        $scope.uploading = true;

        $scope.upload = $upload.upload({
            url: "/api/ManualImport/Upload",
            //data: { importID: $scope.queryStringImportID() == undefined ? $scope.import.ID : $scope.queryStringImportID() },
            file: $scope.file
        }).progress(function (evt) {
            console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        }).success(function (data, status, headers, config) {
            $scope.result = data;
            $scope.uploading = false;
        })
        .error(function (err) {
            console.log(err);
            $scope.uploading = false;
            alert("Error occurred while uploading file!");
        });
        //.then(success, error, progress); 
        //.xhr(function(xhr){xhr.upload.addEventListener(...)})// access and attach any event listener to XMLHttpRequest.
    };

};