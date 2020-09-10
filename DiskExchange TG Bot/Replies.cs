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
                            new KeyboardButton("Контакты"),
                            new KeyboardButton("Назад")
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
                            InlineKeyboardButton.WithUrl("Попков Артем", "https://vk.com/mr666tema666")
                        }
                    });
            
                }
            }
            public static InlineKeyboardMarkup Disc
            {
                get
                {
                    return new InlineKeyboardMarkup(new[] {
                        new[]{
                            InlineKeyboardButton.WithCallbackData("Ввести название ")
                        }
                    });
                }
            }
        }
        public struct disc
        {
            
        }

    }
}
