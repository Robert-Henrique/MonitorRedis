app.service("redisService", function ($http) {

    this.obter = function () {
        return $http.get("api/Redis/Get");
    };
});