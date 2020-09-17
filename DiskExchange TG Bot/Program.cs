// 1.9
using System;
using System.Drawing;
using Telegram.Bot;
using Pastel;
using System.Data.SQLite;
using Telegram.Bot.Exceptions;

namespace DiskExchange_TG_Bot
{
    class Program
    {
        public static byte platform = 0;
        //static Disc currentdisc = new Disc(666);
        static Logger log = new Logger();
        static Database db = new Database();
        struct expect
        {
            public static bool Photo = false;
            public static bool Name = false;
            public static bool Price = false;
            public static bool Exchange = false;
            public static bool Location = false;
        }
        
        public static bool discExchangeable = false;

        static int discMessageId;
        private static ITelegramBotClient bot;
        static void Main(string[] args)
        {
            bot = new TelegramBotClient("1299381797:AAF58uk3gqiSt9pkILwJ8970UXo2t_0_brQ") { Timeout = TimeSpan.FromSeconds(20)};
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
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
                    db.SetPlatform(platform, e.CallbackQuery.From.Id, true);
                    break;
                case "Xbox ⚪️":
                    platform = 1;
                    db.SetPlatform(platform, e.CallbackQuery.From.Id, true);
                    break;
                case "Switch ⚪️":
                    platform = 2;
                    db.SetPlatform(platform, e.CallbackQuery.From.Id, true);
                    break;
                case "Убрать обмен":
                    db.SetExchange("", e.CallbackQuery.From.Id, true);
                    discExchangeable = false;
                    break;
                case "Изменить название":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте название игры в следующем сообщении.", true);
                    discMessageId = message.MessageId;
                    expect.Name = true;
                    return;
                case "Указать цену":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте цену игры в следующем сообщении.", true);
                    discMessageId = message.MessageId;
                    expect.Price = true;
                    return;
                case "Обмен":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте названия желаемых игр в следующем сообщении.", true);
                    discMessageId = message.MessageId;
                    expect.Exchange = true;
                    return;
                case "Загрузить фото":
                    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                        "Отправьте фотогрфию в следующем сообщении.", true);
                    discMessageId = message.MessageId;
                    expect.Photo = true;
                    return;

            }
            try
            {
                await bot.EditMessageCaptionAsync(message.Chat.Id,
                message.MessageId,
                caption: db.GetCaption(e.CallbackQuery.From.Id, true),
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
                    try
                    {
                        if ((message.Photo != null) && (expect.Photo == true))
                        {
                            expect.Photo = false;
                            string photo = message.Photo[message.Photo.Length - 1].FileId;
                            db.SetPhoto(photo, message.From.Id, true);
                            await bot.EditMessageMediaAsync(
                                chatId: message.Chat.Id,
                                messageId: discMessageId,
                                media: new Telegram.Bot.Types.InputMediaPhoto(photo));
                            await bot.EditMessageCaptionAsync(message.Chat.Id,
                                discMessageId,
                                caption: db.GetCaption(message.From.Id, true),
                                replyMarkup: Replies.disc.diskKeyboard);
                        }
                        else if (expect.Name == true)
                        {
                            expect.Name = false;
                            db.SetName(text, message.From.Id, true);
                            await bot.EditMessageCaptionAsync(message.Chat.Id,
                                discMessageId,
                                caption: db.GetCaption(message.From.Id, true),
                                replyMarkup: Replies.disc.diskKeyboard);
                        }
                        else if (expect.Price == true)
                        {
                            expect.Price = false;
                            db.SetPrice(Convert.ToDouble(message.Text), message.From.Id, true);
                            await bot.EditMessageCaptionAsync(message.Chat.Id,
                                discMessageId,
                                caption: db.GetCaption(message.From.Id, true),
                                replyMarkup: Replies.disc.diskKeyboard);
                        }
                        else if (expect.Exchange == true)
                        {
                            expect.Exchange = false;
                            discExchangeable = true;
                            db.SetExchange(text, message.From.Id, true);
                            await bot.EditMessageCaptionAsync(message.Chat.Id,
                                discMessageId,
                                caption: db.GetCaption(message.From.Id, true),
                                replyMarkup: Replies.disc.diskKeyboard);
                        }
                        else if (expect.Location == true)
                        {
                            expect.Location = false;
                            db.NewUser(message.From.Id, text);
                            await bot.SendTextMessageAsync(message.From.Id, $"Профиль создан.",
                                replyMarkup: Replies.keyboards.main);
                            Console.WriteLine();
                            return;
                        }
                        else Console.Write(" - unprocessed message found. Deleted.".Pastel(Color.Gold));
                        Console.WriteLine();

                    }
                    catch (MessageIsNotModifiedException e1)
                    {
                        log.Error(e1.Message);
                        return;
                    }
                    
                    await bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    return;

                case "/start":
                    await bot.SendTextMessageAsync(message.From.Id,$"Привет {message.From.Username}, это бот по обмену дисками!");
                    await bot.SendTextMessageAsync(message.From.Id,$"Пожалуйста, введите ваш город проживания:");
                    expect.Location = true;
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
                    db.NewDisc(message.From.Id);
                    var temp = await bot.SendPhotoAsync(message.Chat.Id, "AgACAgIAAxkBAAIGZF9aSti3CZNeKoW3AjRGDco3-45KAAL3rjEb0L7RSjbSrDV25SE0ECFzly4AAwEAAwIAA3gAA3CNAAIbBA", 
                        caption: db.GetCaption(message.From.Id, true),
                        replyMarkup: Replies.disc.diskKeyboard);
                    db.SetEditMessageId(message.From.Id, temp.MessageId);
                    break;
                case "Поиск 🔎":
                    break;
                case "Мой профиль 👤":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.profile);
                    break;
                case "Избранное 🌟":
                    break;
                case "TEST":
                    break;
            }
            Console.WriteLine();
        }
    }
}