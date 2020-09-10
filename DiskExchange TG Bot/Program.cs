// 1.3
using System;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.ReplyMarkups;

namespace DiskExchange_TG_Bot
{
    class Program
    {
        private static ITelegramBotClient bot;
        private const string placeholderImageId = "AgACAgIAAxkBAAIGZF9aSti3CZNeKoW3AjRGDco3-45KAAL3rjEb0L7RSjbSrDV25SE0ECFzly4AAwEAAwIAA3gAA3CNAAIbBA";
        static void Main(string[] args)
        {
            bot = new TelegramBotClient("1299381797:AAF58uk3gqiSt9pkILwJ8970UXo2t_0_brQ") { Timeout = TimeSpan.FromSeconds(60)};
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
        }

        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var text = e.Message.Text;
            var message = e.Message;
            Console.WriteLine($"Recived message from user {message.From.Username} ({message.From.Id}): " + text);

            Disc currentDisk = new Disc(message.From.Id);
            await bot.SendTextMessageAsync(message.From.Id, currentDisk.message);

            switch (message.Text)
            {
                default:
                    await bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    break;

                case "/start":
                    await bot.SendTextMessageAsync(message.From.Id,$"Привет {message.From.Username}, это бот по обмену дисками!");
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    break;

                case "/keyboard":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.help);
                    break;

                case "Назад":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.main);
                    break;

                case "Контакты":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.contact);
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
