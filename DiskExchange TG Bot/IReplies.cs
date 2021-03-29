using Telegram.Bot.Types.ReplyMarkups;

namespace DiskExchange_TG_Bot
{
    interface IReplies
    {
        struct keyboards
        {
            public static ReplyKeyboardMarkup main
            {
                get{
                    return new ReplyKeyboardMarkup(new[] {
                        new[] {
                            new KeyboardButton("Поиск 🔎"),
                            new KeyboardButton("Мой профиль 👤")
                        },
                        new[] {
                            new KeyboardButton("Избранное 🌟"),
                            new KeyboardButton("Помощь ❓"),
                        },
                    },
                        resizeKeyboard: true);
                }
            }
            public static ReplyKeyboardMarkup profile
            {
                get
                {
                    return new ReplyKeyboardMarkup(new[]
                                        {
                        new[]
                        {
                            new KeyboardButton("Добавить товар 💿"),
                            new KeyboardButton("Мои товары 💿")
                        },
                        new[]
                        {
                            new KeyboardButton("Отправить номер телефона 📲"){
                                RequestContact = true
                            }
                        },
                        new[]
                        {
                             new KeyboardButton("Назад ↩️")
                        }
                    },
                        resizeKeyboard: true);
                }
            }
            public static ReplyKeyboardMarkup help
            {
                get{
                    return new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Контакты 📱")
                        },
                        new[]
                        {
                             new KeyboardButton("Назад ↩️")
                        }
                    },
                        resizeKeyboard: true);
                }
            }
            public static ReplyKeyboardMarkup phone
            {
                get
                {
                    return new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Контакты 📱"){
                                RequestContact = true
                            }
                        }
                    },
                        resizeKeyboard: true);
                }
            }
            public static InlineKeyboardMarkup contact
            {
                get
                {
                    return new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Сиваков Даниил", "https://vk.com/blanker_bat"),
                            InlineKeyboardButton.WithUrl("Попков Артем", "https://vk.com/it_man_csharp"),
                        }
                    }); ;

                }
            }
            public static InlineKeyboardMarkup search
            {
                get
                {
                    return new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Поиск 🔎")
                        }
                    }); ;

                }
            }

        }
        static public InlineKeyboardMarkup discKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⭐️ В избранное ⭐️")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🛒 Связаться с продавцом 🛒")
                    }
                });
        }
        static public InlineKeyboardMarkup favKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("❌ Удалить из избранного ❌")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🛒 Связаться с продавцом 🛒")
                    }
                });
        }
        static public InlineKeyboardMarkup editKeyboard(string platform)
        {
            string uploadPhoto = "Загрузить фото";
            string editName = "Изменить название";
            string ps = $"PS4 {(platform == "PS4" ? "🔘" : "⚪️")}";
            string xbox = $"Xbox {(platform == "Xbox" ? "🔘" : "⚪️")}";
            string switchN = $"Switch {(platform == "Switch" ? "🔘" : "⚪️")}";
            string sell = "Указать цену";
            string exchange = "Обмен";
            return new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(uploadPhoto)
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(editName)
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(ps),
                        InlineKeyboardButton.WithCallbackData(xbox),
                        InlineKeyboardButton.WithCallbackData(switchN)
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(sell),
                        InlineKeyboardButton.WithCallbackData(exchange)
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("✅ Сохранить ✅"),
                        InlineKeyboardButton.WithCallbackData("❌ Удалить ❌")
                    }
                });
        }
    }
}
