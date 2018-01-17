app.controller('homeController', function ($scope, $interval, homeService) {

    $scope.listas = new Array();
    $scope.requisicao = 0;

    obter();
    $interval(callAtInterval, 5000);

    function callAtInterval() {
        obter();
    };

    function obter() {
        var start = new Date().getTime();

        homeService.obter().then(function (response) {
            $scope.requisicao = new Date().getTime() - start;
            $scope.listas = response.data;
        });
    }
});