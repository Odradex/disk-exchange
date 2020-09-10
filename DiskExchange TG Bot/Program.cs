//Версия 1.0
using System;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.ReplyMarkups;

namespace DiskExchange_TG_Bot
{
    class Program
    {
        private static ITelegramBotClient bot;
        private const string placeholderImageId = "AgACAgIAAxkBAAIEql9Y8CVaZ_9pDW7oj0yccav567XtAAIwrzEbEmfJSvG28vI5sia7GOc-li4AAwEAAwIAA3gAA7zAAQABGwQ";
        static void Main(string[] args)
        {
            bot = new TelegramBotClient("1299381797:AAF58uk3gqiSt9pkILwJ8970UXo2t_0_brQ") { Timeout = TimeSpan.FromSeconds(10)};
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            
            bot.StartReceiving();
            Console.WriteLine($"Starting bot {bot.GetMeAsync().Result}...");
            Console.ReadKey();
        }

        private static void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var data = e.CallbackQuery.Data;
            var message = e.CallbackQuery.Message;
            Console.WriteLine("query recived:" + data);
            if (data == "TEST")
                bot.EditMessageTextAsync(message.Chat.Id, message.MessageId, message.Text + "TEST", 
                    replyMarkup: Replies.keyboards.newDisc);
        }

        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var text = e.Message.Text;
            var message = e.Message;
            Console.WriteLine("name: " + e.Message.From.FirstName +" "+e.Message.From.LastName + ", user:" + e.Message.From.Username + ": " + text);
            

            switch (message.Text)
            {
                default:
                    await bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    //await bot.SendTextMessageAsync(e.Message.Chat, $"Вы написали: {text}");
                    break;

                case "/start":
                    await bot.SendTextMessageAsync(message.From.Id,$"Привет {message.From.Username}, это бот по обмену дисками!");
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    break;

                case "/dev":
                    var inlinekeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]{
                            InlineKeyboardButton.WithUrl("Сиваков Даниил", "https://vk.com/blanker_bat"),
                            InlineKeyboardButton.WithUrl("Попков Артем", "https://vk.com/mr666tema666")
                        },
                        new[]{
                            InlineKeyboardButton.WithCallbackData("Пункт 1")
                        }
                    });
                    await bot.SendTextMessageAsync(message.From.Id, "Выберите пункт меню",
                        replyMarkup: inlinekeyboard);
                    break;

                case "/keyboard":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.help);
                    break;

                case "Назад":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.main);
                    break;

                case "Помощь ❓":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.help);
                    break;

                case "Товар":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Создание нового товара...", 
                        replyMarkup: Replies.keyboards.newDisc);
                    break;

                case "/poll":
                    await bot.SendPollAsync(
                        chatId: e.Message.Chat,
                        question: "This is the testing poll question.",
                        options: new[]{
                        "Option 1",
                        "Option 2"
                        }
                    );
                    break;
                case "TEST":
                    break;
            }

            
        }
    }
}
