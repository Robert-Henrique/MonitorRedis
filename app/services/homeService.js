app.service("homeService", function ($http) {

    this.obter = function () {
        return $http.get("api/Home/Get");
    };
});