// 1.13
using System;
using System.Drawing;
using Telegram.Bot;
using Pastel;
using System.Data.SQLite;
using Telegram.Bot.Exceptions;
using System.Threading.Tasks;

namespace DiskExchange_TG_Bot
{
    class Program
    {
        static Logger log = new Logger();
        static Database db = new Database();
        
        enum awaitInfoType : int
        { 
            none = 0,
            photo = 1,
            name = 2,
            price = 3,
            exchange = 4,
            location = 5,
            discNumber = 6
        };

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
            Console.ReadLine();
        }

        private static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var data = e.CallbackQuery.Data;
            var message = e.CallbackQuery.Message;

            log.Query(e);

            try
            {
                switch (data)
                {
                    default:
                        return;
              
                    case "PS4 ⚪️":
                        db.SetPlatform(0, e.CallbackQuery.From.Id, true);
                        break;
                    case "Xbox ⚪️":
                        db.SetPlatform(1, e.CallbackQuery.From.Id, true);
                        break;
                    case "Switch ⚪️":
                        db.SetPlatform(2, e.CallbackQuery.From.Id, true);
                        break;
                    case "Изменить название":
                        await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                            "Отправьте название игры в следующем сообщении.", true);
                        db.SetAwaitInfoType(e.CallbackQuery.From.Id, (int)awaitInfoType.name);
                        return;
                    case "Указать цену":
                        await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                            "Отправьте цену игры в следующем сообщении.", true);
                        db.SetAwaitInfoType(e.CallbackQuery.From.Id, (int)awaitInfoType.price);
                        return;
                    case "Обмен":
                        if (db.GetExchange(e.CallbackQuery.From.Id, true) != "")
                        {
                            db.SetExchange("", e.CallbackQuery.From.Id, true);
                            break;
                        }
                        else
                        {
                            await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                            "Отправьте названия желаемых игр в следующем сообщении.", true);
                            db.SetAwaitInfoType(e.CallbackQuery.From.Id, (int)awaitInfoType.exchange);
                            return;
                        }
                    case "Загрузить фото":
                        await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id,
                            "Отправьте фотогрфию в следующем сообщении.", true);
                        db.SetAwaitInfoType(e.CallbackQuery.From.Id, (int)awaitInfoType.photo);
                        return;
                    case "✅ Сохранить ✅":
                        await bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, db.GetEditMessageId(e.CallbackQuery.From.Id));
                        await bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "✅ Товар добавлен!\n\nℹ️ Чтобы просмотреть список ваших товаров, выберите пункт \"Мои товары\".");
                        return;
                }
                await bot.EditMessageCaptionAsync(message.Chat.Id,
                message.MessageId,
                caption: db.GetCaption(e.CallbackQuery.From.Id, true),
                replyMarkup: Replies.editKeyboard(db.GetPlatform(e.CallbackQuery.From.Id)));
            }
            catch (MessageIsNotModifiedException e3)
            {
                log.Error(e3.Message);
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
                        switch (db.GetAwaitInfoType(message.From.Id))
                        {
                            case 0:
                                break;
                            case (int)awaitInfoType.name:
                                db.SetName(text, message.From.Id, true);
                                db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                await SetDiscCaptionAsync(message.Chat.Id, message.From.Id);
                                break;
                            case (int)awaitInfoType.price:
                                db.SetPrice(message.Text, message.From.Id, true);
                                db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                await SetDiscCaptionAsync(message.Chat.Id, message.From.Id);
                                break;
                            case (int)awaitInfoType.exchange:
                                db.SetExchange(text, message.From.Id, true);
                                db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                await SetDiscCaptionAsync(message.Chat.Id, message.From.Id);
                                break;
                            case (int)awaitInfoType.location:
                                db.NewUser(message.From.Id, text);
                                db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                await bot.SendTextMessageAsync(message.From.Id, $"Профиль создан.",
                                    replyMarkup: Replies.keyboards.main);
                                Console.WriteLine();
                                return;
                            case (int)awaitInfoType.photo:
                                if (message.Photo == null)
                                    break;
                                string photo = message.Photo[message.Photo.Length - 1].FileId;
                                db.SetPhoto(photo, message.From.Id, true);
                                await bot.EditMessageMediaAsync(
                                    chatId: message.Chat.Id,
                                    messageId: db.GetEditMessageId(message.From.Id),
                                    media: new Telegram.Bot.Types.InputMediaPhoto(photo));
                                await SetDiscCaptionAsync(message.Chat.Id, message.From.Id);
                                break;
                            default:
                                Console.Write(" - unprocessed message found. Deleted.".Pastel(Color.Gold));
                                break;
                        }
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
                    db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.location);
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
                        replyMarkup: Replies.editKeyboard(db.GetPlatform(message.From.Id)));
                    db.SetEditMessageId(message.From.Id, temp.MessageId);
                    break;
                case "Поиск 🔎":
                    break;
                case "Мой профиль 👤":
                    await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                        replyMarkup: Replies.keyboards.profile);
                    break;
                case "Мои товары 💿":
                    await bot.SendTextMessageAsync(message.Chat.Id, db.GetUserDisks(message.From.Id));
                    db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.discNumber);
                    break;
                case "Избранное 🌟":
                    break;
                
            }
            Console.WriteLine();
        }
         static async Task SetDiscCaptionAsync(long chat, int from)
         {
            await bot.EditMessageCaptionAsync(chat,
                db.GetEditMessageId(from),
                caption: db.GetCaption(from, true),
                replyMarkup: Replies.editKeyboard(db.GetPlatform(from)));
         }
    }
}