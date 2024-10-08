window.localStorageHelper = {
    getItem: function (key) {
        return localStorage.getItem(key);
    },
    setItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    removeItem: function (key) {
        localStorage.removeItem(key);
    }
};
/*
window.yourJavaScriptFunction = function () {
    return window.localStorageHelper.getItem("authToken");
};
*/
