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
            public static ReplyKeyboardMarkup mainMenu
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

        }
       

    }
}
