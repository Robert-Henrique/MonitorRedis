angular.module("app").factory('$gritter', function () {
    return {
        success: function (msg, title) {
            return jQuery.gritter.add({ title: title || "Sucesso", text: msg, class_name: 'gritter-success', time: 3000 });
        },
        error: function (msg, title) {
            return jQuery.gritter.add({ title: title || "Erro", text: msg, class_name: 'gritter-error', time: 3000 });
        },
        warning: function (msg, title) {
            return jQuery.gritter.add({ title: title || "Atenção", text: msg, class_name: 'gritter-warning', time: 3000 });
        }
    }
});
