console.log("localStorageHelper script loaded!");
window.localStorageHelper = {
    getItem: function (key) {
        return localStorage.getItem(key);
    },
    setItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    removeItem: function (key) {
        localStorage.removeItem(key);
    },
    getToken: function() {
        return localStorage.getItem('token');
    },
    setToken: function(token) {
        localStorage.setItem('token', token);
    },
    removeToken: function() {
        localStorage.removeItem('token');
    };
}

