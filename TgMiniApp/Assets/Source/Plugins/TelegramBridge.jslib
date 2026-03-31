mergeInto(LibraryManager.library, {
    InitTelegram: function () {
        try {
            if (!window.Telegram || !window.Telegram.WebApp)
            {
                console.log("Telegram WAPI Not found.");
                SendMessage("RestApiDemo", "OnTelegramError", "Telegram WAPI Not found.");
                return;
            }

            const tg = window.Telegram.WebApp;

            tg.ready();
            tg.expand();

            const initData = tg.initData || "";
            const user = tg.initDataUnsafe && tg.initDataUnsafe.user ? tg.initDataUnsafe.user : null;

            SendMessage("RestApiDemo", "SetTelegramInitData", initData);
            if (user){
                const displayName = user.username || 
                [user.first_name, user.last_name].filter(Boolean).join(" ") ||
                "TelegramUser";

                SendMessage("RestApiDemo", "SetTelegramUserName", displayName);
            }
        } catch (e)
        {
                SendMessage("RestApiDemo", "OnTelegramError", e.toString());
        }
    }
})