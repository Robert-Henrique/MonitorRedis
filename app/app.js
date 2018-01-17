var app = angular.module('app', ['ngRoute']);

app.config(function ($routeProvider, $locationProvider) {

    // remove o # da url
    //$locationProvider.html5Mode(true);
    $routeProvider
    .when('/', {
        templateUrl: 'views/Home/index.html'
    })
    .when('/Home', {
        templateUrl: 'views/Home/teste.html'
    })
    .otherwise({
        redirectTo: '/'
    });
});