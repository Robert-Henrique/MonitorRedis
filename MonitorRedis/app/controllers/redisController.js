app.controller('redisController', function ($scope, $interval, $gritter, redisService) {

    $scope.requisicao = 0;
    $scope.hostName = {};
    $scope.IP = {};
    $scope.horario = Date.now();
    $scope.pauseAudio = false;

    obter();
    var stop = $interval(callAtInterval, 10000);

    function callAtInterval() {
        obter();
    };

    function obter() {
        var start = new Date().getTime();

        redisService.obter().then(function (response) {
            $scope.requisicao = new Date().getTime() - start;
            $scope.filasDeIntegracoes = response.data.filasDeIntegracoes;
            $scope.filasDeIntegracoesComErros = response.data.filasDeIntegracoesComErros;
            $scope.hostName = response.data.hostName;
            $scope.IP = response.data.IP;
            emitirAlerta(response.data.nivelDeIntensidade);
            exibirMensagem(response.data.nivelDeIntensidade);
        });
    };

    function emitirAlerta(nivelDeIntensidade) {

        if (nivelDeIntensidade == 0)
            return;

        var alerta = "assets/audio/RedAlert.mp3";

        if (nivelDeIntensidade > 10)
            alerta = "assets/audio/Tornado.mp3";

        var audio = new Audio(alerta);
        if ($scope.pauseAudio == false)
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

    $scope.controlarAudio = function () {
        if ($scope.pauseAudio) 
            $scope.pauseAudio = false;
        else
            $scope.pauseAudio = true;
    };

    $scope.color = function (tamanho) {
        if (tamanho == 0)
            return 'white';
        else if (tamanho < 10)
            return 'yellow';
        else
            return 'red';
    };

    function exibirMensagem(quantidadeDeErros) {
        if (quantidadeDeErros == 0)
            $gritter.success('NENHUM ERRO DE INTEGRAÇÃO!');
        else if (quantidadeDeErros < 10)
            $gritter.warning(quantidadeDeErros + ' ERRO(S) DE INTEGRAÇÃO!');
        else
            $gritter.error(quantidadeDeErros + ' ERROS DE INTEGRAÇÃO!');
    }
});