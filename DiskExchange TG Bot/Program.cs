using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using System.Reflection;
using Telegram.Bot.Args;
using System.Threading.Tasks;
using System.Threading;

namespace DiskExchange_TG_Bot
{
    class Program
    {
        private static ITelegramBotClient bot;
        static void Main(string[] args)
        {
            bot = new TelegramBotClient("1299381797:AAF58uk3gqiSt9pkILwJ8970UXo2t_0_brQ") { Timeout = TimeSpan.FromSeconds(10)};
            bot.OnMessage += Bot_OnMessage;
           // bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            bot.StartReceiving();
            Console.WriteLine($"Starting bot {bot.GetMeAsync().Result}...");
            Console.ReadKey();
        }

        //private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        //{
        //    string buttonText = e.CallbackQuery.Data;
        //    string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
        //    Console.WriteLine($"{name} нажал кнопку: '{buttonText}' ");
        //    switch (buttonText)
        //    {
        //        case "Контакты":
        //            var inlineKeyboard1 = new InlineKeyboardMarkup(new[]
        //              {
        //                new[]
        //                {
        //                    InlineKeyboardButton.WithUrl("Developer_1", "https://vk.com/blanker_bat"),
        //                    InlineKeyboardButton.WithUrl("Developer_1", "https://vk.com/mr666tema666")

        //                }
        //            });
        //            await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,"Контакты");
        //            //await bot.SendTextMessageAsync(e.CallbackQuery.Id, $"Контакты",
        //                //replyMarkup: inlineKeyboard1);
        //            break;
        //            //await bot.SendTextMessageAsync(message.From.Id, "Выберите пунк меню",
        //            //  replyMarkup: inlinekeyboard);
        //    }
        //}

        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var text = e.Message.Text;
            var message = e.Message;
            Console.WriteLine($"id {message.Chat.Id}");
            Console.WriteLine(e.Message.From.Username + ": " + text); 
            await bot.SendTextMessageAsync(e.Message.Chat, $"Вы написали: {text}");
            switch (message.Text)
            {
                case "/start":
                   await bot.SendTextMessageAsync(message.From.Id,$"Привет { message.From.Username}, это бот по обмену дисками!");
                    break;
                case "/dev":
                    var inlinekeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Developer_1", "https://vk.com/blanker_bat"),
                            InlineKeyboardButton.WithUrl("Developer_1", "https://vk.com/mr666tema666")
                            
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Пункт 1")
                        }
                    });
                    await bot.SendTextMessageAsync(message.From.Id, "Выберите пунк меню",
                        replyMarkup: inlinekeyboard);
                    break;
                case "/keyboard":
                    startKeyboard:
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Поиск"),
                            new KeyboardButton("Мой профиль")
                        },
                        new[]
                        {
                            
                            new KeyboardButton("Избранное"),
                            new KeyboardButton("Помощь")
                            
                        }

                    }
                    );
                   
                    await bot.SendTextMessageAsync(message.Chat.Id, "Сообщение",
                        replyMarkup: replyKeyboard);
                  
                    break;

                case "Помощь":
                    var replyKeyboard1 = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                             new KeyboardButton("Геолокация"){RequestLocation = true},
                             new KeyboardButton("Контакт"){RequestContact = true},
                             new KeyboardButton("Назад")



                        }
                    }
                    );
                    await bot.SendTextMessageAsync(message.Chat.Id, "Сообщение",
                       replyMarkup: replyKeyboard1);
                    break;
                case "Назад":
                    goto startKeyboard;
                    break;
                case "/poll":
                    {
                       await bot.SendPollAsync(
                        chatId: e.Message.Chat,
                        question: "This is the testing poll question.",
                        options: new[]
                        {
                        "Option 1",
                        "Option 2"
                        }
                    );
                    }
                    break;
            }

            
        }
    }
}
