using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace DiskExchange_TG_Bot
{
    interface Replies
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
                            new KeyboardButton("Геолокация") { RequestLocation = true },
                            new KeyboardButton("Контакт") { RequestContact = true },
                            new KeyboardButton("Назад")
                        }
                    },
                        resizeKeyboard: true);
                }
            }
            public static InlineKeyboardMarkup newDisc
            {
                get
                {
                    return new InlineKeyboardMarkup(new[]
                    {
                    new[]{
                        InlineKeyboardButton.WithCallbackData("TEST")
                    }
                }
                    );
                }
            }
        }
        

    }
}
