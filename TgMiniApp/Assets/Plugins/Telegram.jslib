mergeInto(LibraryManager.library, {

    GetTelegramInitData: function () {
        if (typeof window !== "undefined" &&
            window.Telegram &&
            window.Telegram.WebApp) {

            var str = window.Telegram.WebApp.initData || "";

            var bufferSize = lengthBytesUTF8(str) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(str, buffer, bufferSize);

            return buffer;
        }

        var empty = "";
        var bufferSize = lengthBytesUTF8(empty) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(empty, buffer, bufferSize);

        return buffer;
    },

    GetTelegramUser: function () {
        if (typeof window !== "undefined" &&
            window.Telegram &&
            window.Telegram.WebApp &&
            window.Telegram.WebApp.initDataUnsafe &&
            window.Telegram.WebApp.initDataUnsafe.user) {

            var user = window.Telegram.WebApp.initDataUnsafe.user;
            var str = JSON.stringify(user);

            var bufferSize = lengthBytesUTF8(str) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(str, buffer, bufferSize);

            return buffer;
        }

        var empty = "{}";
        var bufferSize = lengthBytesUTF8(empty) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(empty, buffer, bufferSize);

        return buffer;
    },

    TelegramReady: function () {
        if (typeof window !== "undefined" &&
            window.Telegram &&
            window.Telegram.WebApp) {

            window.Telegram.WebApp.ready();
        }
    },

    TelegramExpand: function () {
        if (typeof window !== "undefined" &&
            window.Telegram &&
            window.Telegram.WebApp) {

            window.Telegram.WebApp.expand();
        }
    }

});