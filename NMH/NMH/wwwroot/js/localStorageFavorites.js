window.favorites = {
    get: function () {
        return JSON.parse(localStorage.getItem("favorites") || "[]");
    },
    set: function (value) {
        localStorage.setItem("favorites", JSON.stringify(value));
    }
};