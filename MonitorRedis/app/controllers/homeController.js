﻿app.controller('homeController', function ($scope, $interval, homeService) {

    $scope.listas = new Array();
    $scope.requisicao = 0;
    $scope.hostName = {};
    $scope.horario = Date.now();

    obter();
    $interval(callAtInterval, 10000);

    function callAtInterval() {
        obter();
    };

    function obter() {
        var start = new Date().getTime();

        homeService.obter().then(function (response) {
            $scope.requisicao = new Date().getTime() - start;
            $scope.listas = response.data.listasDeIntegracoes;
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
});