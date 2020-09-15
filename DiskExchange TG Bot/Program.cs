// 1.7
using System;
using System.Drawing;
using Telegram.Bot;
using Pastel;
using System.Data.SQLite;

namespace DiskExchange_TG_Bot
{
    class Program
    {
        public static byte platform = 0;
        static Disc currentDisk = new Disc(666);

        static bool expectPhoto = false;
        static bool expextName = false;
        static bool expextPrice = false;
        static bool expextExchange = false;
        public static bool diskExchangeable = false;

        static Logger log = new Logger();

        static int diskMessageId;
        private static ITelegramBotClient bot;
        static void Main(string[] args)
        {
            bot = new TelegramBotClient("1299381797:AAF58uk3gqiSt9pkILwJ8970UXo2t_0_brQ") { Timeout = TimeSpan.FromSeconds(20)};
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Database db = new Database();

            Console.Write($"2/2: Starting @DiskExchangeBot... ".Pastel(Color.Yellow));
            Console.Beep();

            try
            {
                bot.StartReceiving();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nStartup failed! Error message: " + e.Message.Pastel(Color.Red));
                Console.ReadKey();
                return;
            }
            Console.WriteLine("[READY]\n");

            Console.WriteLine(">ALL SYSTEMS READY\n>Welcome, admin\n");
            Console.CursorVisible = false;
            Console.Beep();
            Console.ReadLine();
        }

        private static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var data = e.CallbackQuery.Data;
            var message = e.CallbackQuery.Message;

            log.Query(e);

            switch (data)
            {
                default:
                    return;
              
                case "PS4 ⚪️":
                    platform = 0;
                    currentDisk.SetPlatform(0);
                    break;
                case "Xbox ⚪️":
                    platform = 1;
                    currentDisk.SetPlatform(1);
                    break;
                case "Switch ⚪️":
                    platform = 2;
                    currentDisk.SetPlatform(2);
                    break;
                case "Убрать обмен":
                    currentDisk.SetExchange("");
                    diskExchangeable = false;
                    break;
                case "Изменить название":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте название игры в следующем сообщении.", true);
                    diskMessageId = message.MessageId;
                    expextName = true;
                    return;
                case "Указать цену":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте цену игры в следующем сообщении.", true);
                    diskMessageId = message.MessageId;
                    expextPrice = true;
                    return;
                case "Обмен":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте названия желаемых игр в следующем сообщении.", true);
                    diskMessageId = message.MessageId;
                    expextExchange = true;
                    return;
                case "Загрузить фото":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте фотогрфию в следующем сообщении.", true);
                    diskMessageId = message.MessageId;
                    expectPhoto = true;
                    return;

            }
            try
            {
                await bot.EditMessageCaptionAsync(message.Chat.Id,
                message.MessageId,
                caption: currentDisk.message,
                replyMarkup: Replies.disc.diskKeyboard);
            }
            catch (Telegram.Bot.Exceptions.MessageIsNotModifiedException)
            { 
                return;
            }
        }
        
        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var text = e.Message.Text;
            var message = e.Message;
            log.Message(e);

            
            switch (message.Text)
            {
                default:
                    if ((message.Photo != null) && (expectPhoto == true))
                    {
                        string photo = message.Photo[message.Photo.Length - 1].FileId;
                        currentDisk.SetPhoto(photo);

                        await bot.EditMessageMediaAsync(
                            chatId: message.Chat.Id,
                            messageId: diskMessageId,
                            media: new Telegram.Bot.Types.InputMediaPhoto(photo));
                        await bot.EditMessageCaptionAsync(message.Chat.Id,
                            diskMessageId,
                            caption: currentDisk.message,
                            replyMarkup: Replies.disc.diskKeyboard);

                        expectPhoto = false;
                    }
                    else if (expextName == true)
                    {
                        currentDisk.SetName(message.Text);
                        await bot.EditMessageCaptionAsync(message.Chat.Id,
                            diskMessageId,
                            caption: currentDisk.message,
                            replyMarkup: Replies.disc.diskKeyboard);
                        expextName = false;
                    }
                    else if (expextPrice == true)
                    {
                        currentDisk.SetPrice(Convert.ToDouble(message.Text));
                        await bot.EditMessageCaptionAsync(message.Chat.Id,
                            diskMessageId,
                            caption: currentDisk.message,
                            replyMarkup: Replies.disc.diskKeyboard);
                        expextPrice = false;
                    }
                    else if (expextExchange == true)
                    {
                        diskExchangeable = true;
                        currentDisk.SetExchange(message.Text);
                        await bot.EditMessageCaptionAsync(message.Chat.Id,
                            diskMessageId,
                            caption: currentDisk.message,
                            replyMarkup: Replies.disc.diskKeyboard);
                        expextExchange = false;
                    }
                    else Console.Write(" - unprocessed message found. Deleted.".Pastel(Color.Gold));
                    Console.WriteLine();

                    await bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    return;

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
                    await bot.SendPhotoAsync(message.Chat.Id, currentDisk.photoId, 
                        replyMarkup: Replies.disc.diskKeyboard,
                        caption: currentDisk.message);
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
            Console.WriteLine();
        }
    }
}