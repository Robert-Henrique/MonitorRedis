app.controller('redisController', function ($scope, $interval, redisService) {

    $scope.listas = new Array();
    $scope.requisicao = 0;
    $scope.hostName = {};
    $scope.horario = Date.now();

    obter();
    var stop = $interval(callAtInterval, 10000);

    function callAtInterval() {
        obter();
    };

    function obter() {
        var start = new Date().getTime();

        redisService.obter().then(function (response) {
            $scope.requisicao = new Date().getTime() - start;
            $scope.filas = response.data.listasDeIntegracoes;
            $scope.hostName = response.data.hostName;
            emitirAlerta(response.data.nivelDeIntensidade);
        });
    };

    function emitirAlerta(nivelDeIntensidade) {

        if (nivelDeIntensidade == 0)
            return;

        var alerta = "assets/audio/RedAlert.mp3";

        if (nivelDeIntensidade > 10)
            alerta = "assets/audio/Tornado.mp3";

        var audio = new Audio(alerta);
        audio.play();
    };

    var obterHorario = function () {
        $scope.horario = Date.now();
    };

    obterHorario();
    $interval(obterHorario, 1000);

    $scope.cancelarAlerta = function () {
        if (angular.isDefined(stop)) {
            $interval.cancel(stop);
            stop = undefined;
        }
    };
});