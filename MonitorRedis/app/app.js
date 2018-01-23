var app = angular.module('app', ['ngRoute']);

app.config(function ($routeProvider, $locationProvider) {

    // remove o # da url
    //$locationProvider.html5Mode(true);

    $routeProvider
    .when('/', {
        templateUrl: 'views/Redis/index.html'
    })
    .when('/redis/detalhes/:id', {
        templateUrl: 'views/Redis/detalhes.html'
    })
    .otherwise({
        redirectTo: '/'
    });
});