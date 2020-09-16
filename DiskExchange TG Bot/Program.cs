// 1.8.1
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
        //static Disc currentdisc = new Disc(666);
        static Database db = new Database();

        static bool expectPhoto = false;
        static bool expextName = false;
        static bool expextPrice = false;
        static bool expextExchange = false;
        public static bool discExchangeable = false;

        static Logger log = new Logger();

        static int discMessageId;
        private static ITelegramBotClient bot;
        static void Main(string[] args)
        {
            Database db = new Database();
            db.NewDisc(25565, 712);
            
            bot = new TelegramBotClient("1299381797:AAF58uk3gqiSt9pkILwJ8970UXo2t_0_brQ") { Timeout = TimeSpan.FromSeconds(20)};
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Database db = new Database();

            Console.Write($"2/2: Starting @discExchangeBot... ".Pastel(Color.Yellow));
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
                    db.SetPlatform(message.From.Id, platform);
                    break;
                case "Xbox ⚪️":
                    platform = 1;
                    db.SetPlatform(message.From.Id, platform);
                    break;
                case "Switch ⚪️":
                    platform = 2;
                    db.SetPlatform(message.From.Id, platform);
                    break;
                case "Убрать обмен":
                    db.SetExchange(message.From.Id, " ");
                    discExchangeable = false;
                    break;
                case "Изменить название":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте название игры в следующем сообщении.", true);
                    discMessageId = message.MessageId;
                    expextName = true;
                    return;
                case "Указать цену":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте цену игры в следующем сообщении.", true);
                    discMessageId = message.MessageId;
                    expextPrice = true;
                    return;
                case "Обмен":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте названия желаемых игр в следующем сообщении.", true);
                    discMessageId = message.MessageId;
                    expextExchange = true;
                    return;
                case "Загрузить фото":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте фотогрфию в следующем сообщении.", true);
                    discMessageId = message.MessageId;
                    expectPhoto = true;
                    return;

            }
            try
            {
                await bot.EditMessageCaptionAsync(message.Chat.Id,
                message.MessageId,
                caption: db.GetMessage,
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
                        db.SetPhoto(message.From.Id, photo);

                        await bot.EditMessageMediaAsync(
                            chatId: message.Chat.Id,
                            messageId: discMessageId,
                            media: new Telegram.Bot.Types.InputMediaPhoto(photo));
                        await bot.EditMessageCaptionAsync(message.Chat.Id,
                            discMessageId,
                            caption: db.GetMessage,
                            replyMarkup: Replies.disc.diskKeyboard);

                        expectPhoto = false;
                    }
                    else if (expextName == true)
                    {
                        db.SetName(message.From.Id, message.Text);
                        await bot.EditMessageCaptionAsync(message.Chat.Id,
                            discMessageId,
                            caption: db.GetMessage,
                            replyMarkup: Replies.disc.diskKeyboard);
                        expextName = false;
                    }
                    else if (expextPrice == true)
                    {
                        db.SetPrice(message.From.Id, Convert.ToDouble(message.Text));
                        await bot.EditMessageCaptionAsync(message.Chat.Id,
                            discMessageId,
                            caption: db.GetMessage,
                            replyMarkup: Replies.disc.diskKeyboard);
                        expextPrice = false;
                    }
                    else if (expextExchange == true)
                    {
                        discExchangeable = true;
                        db.SetExchange(message.From.Id, message.Text);
                        await bot.EditMessageCaptionAsync(message.Chat.Id,
                            discMessageId,
                            caption: db.GetMessage,
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
                        replyMarkup: Replies.keyboards.main);
                    break;

                case "Назад 🔙":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.main);
                    break;

                case "Контакты 📱":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.contact);
                    break;

                case "Помощь ❓":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.help);
                    break;

                case "Добавить товар 💿":
                    await bot.SendPhotoAsync(message.Chat.Id, "AgACAgIAAxkBAAIGZF9aSti3CZNeKoW3AjRGDco3-45KAAL3rjEb0L7RSjbSrDV25SE0ECFzly4AAwEAAwIAA3gAA3CNAAIbBA", 
                        replyMarkup: Replies.disc.diskKeyboard,
                        caption: db.GetMessage);
                    break;
                case "Поиск 🔎":
                    break;
                case "Мой профиль 👤":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.profile);
                    break;
                case "Избранное 🌟":
                    break;
                //case "/poll":
                //    await bot.SendPollAsync(
                //        chatId: e.Message.Chat,
                //        question: "This is the testing poll question.",
                //        options: new[]{
                //        "Option 1",
                //        "Option 2"
                //        }
                //    );
                    //break;
                case "TEST":
                    break;
            }
            Console.WriteLine();
        }
    }
}