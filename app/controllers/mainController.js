angular.module('app')
.constant('SAVE_SETTING', true)
    .controller('MainController', ['$scope', '$rootScope', '$http', '$timeout', '$location', '$interval', function ($scope, $rootScope, $http, $timeout, $location, $interval) {
        //some general variables
        $scope.ace = $scope.ace || {};
        $scope.ace.settings = $scope.ace.settings || {};

    }]);