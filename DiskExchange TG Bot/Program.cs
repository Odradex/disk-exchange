using System;
using Telegram.Bot;

namespace DiskExchange_TG_Bot
{
    class Program
    {
        private static ITelegramBotClient bot;
        static void Main(string[] args)
        {
            bot = new TelegramBotClient("1299381797:AAF58uk3gqiSt9pkILwJ8970UXo2t_0_brQ") { Timeout = TimeSpan.FromSeconds(10)};
            bot.OnMessage += Bot_OnMessage;

            bot.StartReceiving();
            Console.WriteLine($"Starting bot {bot.GetMeAsync().Result}...");
        }

        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var text = e.Message.Text;
            Console.WriteLine(e.Message.From.Username + ": " + text);
            bot.SendTextMessageAsync(e.Message.Chat, $"Вы написали: {text}");
            if (e.Message.Text == "/poll")
            {
                bot.SendPollAsync(
                    chatId: e.Message.Chat,
                    question: "This is the testing poll question.",
                    options: new[]
                    {
                        "Option 1",
                        "Option 2"
                    }
                );

            }
        }
    }
}
