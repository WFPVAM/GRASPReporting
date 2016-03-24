//function Ctrl($scope) {
//    //$scope.surname = 'guest';
//    //$scope.cognomeD = /^\s*\w*\s*$/;
//    //$scope.age = 18;
//    //$scope.nFigli = 1;
//}// JavaScript source code

var app = angular.module('angularjs-starter', []);

app.controller('MainCtrl', function ($scope) {


    $scope.master = {};

    $scope.update = function (currForm) {
        $scope.master = angular.copy(currForm);
    };

    $scope.reset = function () {
        $scope.currForm = angular.copy($scope.master);
    };

    $scope.isUnchanged = function (currForm) {
        return angular.equals(currForm, $scope.master);
    };

    //$scope.reset();

    //$scope.currForm.items = [];
    //$scope.addNew = function () {
    //    $scope.currForm.items.push({ name: '', macchina: '' });
    //};

    //$scope.currForm.items2 = [];
    //$scope.addNew2 = function () {
    //    $scope.currForm.items2.push({ prodotto: '', tipo: '' });
    //};
});