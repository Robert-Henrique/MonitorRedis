app.service("redisService", function ($http) {

    this.obter = function () {
        return $http.get("api/Redis/ObterFilas/");
    };

    this.obterDetalhes = function (id) {
        return $http.get("api/Redis/ObterDetalhes/?id=" + id);
    };

    this.excluir = function (id, errorTimeStamp) {
        return $http.delete("api/Redis?filaId=" + id + "&errorTimeStamp=" + errorTimeStamp);
    };
});