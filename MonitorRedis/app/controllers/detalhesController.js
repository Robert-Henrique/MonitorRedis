app.controller('detalhesController', function ($scope, $routeParams, $interval, redisService) {

    $scope.horario = Date.now();
    $scope.fila = new Object();
    $scope.requisicao = 0;

    var id = $routeParams.id;

    if (angular.isDefined(id) == false)
        $scope.voltar();

    var start = new Date().getTime();

    redisService.obterDetalhes(id).then(function (response) {
        $scope.requisicao = new Date().getTime() - start;
        $scope.fila = response.data;
    });

    var obterHorario = function () {
        $scope.horario = Date.now();
    };

    obterHorario();
    $interval(obterHorario, 1000);

    $scope.voltar = function () {
        var url = window.location.protocol + '//' + window.location.host + "#/";
        window.location = url;
    };
});