app.controller('detalhesController', function ($scope, $routeParams, $interval, $gritter, redisService) {

    $scope.horario = Date.now();
    $scope.fila = new Object();
    $scope.requisicao = 0;

    var id = $routeParams.id;

    if (angular.isDefined(id) == false)
        $scope.voltar();

    var start = new Date().getTime();
    obterDetalhes(id);

    function obterDetalhes(id) {
        redisService.obterDetalhes(id).then(function (response) {
            $scope.requisicao = new Date().getTime() - start;
            $scope.fila = response.data;
            
            if ($scope.fila.tamanho == 0)
                $scope.voltar();
        });
    };

    var obterHorario = function () {
        $scope.horario = Date.now();
    };

    obterHorario();
    $interval(obterHorario, 1000);

    $scope.voltar = function () {
        var url = window.location.protocol + '//' + window.location.host + "#/";
        window.location = url;
    };

    $scope.excluir = function (item) {
        var msg = "<b>Você corrigiu este erro?</b></br>" +
            "<b>Essa integração foi realizada?</b></br></br>" +
            "<b style='font-style:italic'>Isso vale bolo hem!!!</b></br>";

        bootbox.confirm(msg, function (result) {
            if (result) {
                redisService.excluir(id, item.ErrorTimeStamp).then(function () {
                    $gritter.success('Erro excluído com sucesso!');
                    obterDetalhes(id);
                });
            }
        });
    };
});