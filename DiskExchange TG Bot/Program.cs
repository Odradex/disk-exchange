// 1.6
using Pastel;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.InlineQueryResults;

namespace DiskExchange_TG_Bot
{
    internal class Program
    {
        private static Logger log = new Logger();
        private static Database db = new Database();
        private enum awaitInfoType : int
        {
            none = 0,
            photo = 1,
            name = 2,
            price = 3,
            exchange = 4,
            location = 5,
            discNumber = 6,
            searchResult = 7,
            favNumber = 8
        };

        private static ITelegramBotClient bot;

        private static void Main(string[] args)
        {
            bot = new TelegramBotClient("1299381797:AAF58uk3gqiSt9pkILwJ8970UXo2t_0_brQ") { Timeout = TimeSpan.FromSeconds(20) };
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            bot.OnInlineQuery += Bot_OnInlineQuery;
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

        private static async void Bot_OnInlineQuery(object sender, Telegram.Bot.Args.InlineQueryEventArgs e)
        {
            string query = e.InlineQuery.Query;
            if (query.Length < 3)
                return;

            Database.discsArray[] discs = db.Search(query);
            InlineQueryResultBase[] results = new InlineQueryResultArticle[discs.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var temp = new InlineQueryResultArticle(Convert.ToString(discs[i].id),
                    title: discs[i].name,
                    new InputTextMessageContent($"Товар {discs[i].id}: {discs[i].name}"));
                temp.Description = discs[i].platform + " | " + discs[i].price + " BYN";
                results[i] = temp;
            }
            if (results.Length == 0)
                return;
            Console.WriteLine(query);
            db.SetAwaitInfoType(e.InlineQuery.From.Id, (int)awaitInfoType.searchResult);
            try
            {
                await bot.AnswerInlineQueryAsync(e.InlineQuery.Id, results);
            }
            catch (Telegram.Bot.Exceptions.InvalidParameterException e5)
            {
                log.Error(e5.Message);
                return;
            }
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
                    case "⭐️ В избранное ⭐️":
                        db.AddSelectedDiscToFavorites(e.CallbackQuery.From.Id);
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

                    case "❌ Удалить ❌":
                        db.DeleteDisc(e.CallbackQuery.From.Id);
                        await bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, db.GetEditMessageId(e.CallbackQuery.From.Id));
                        await bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "✅ Товар удален!\n\nℹ️ Чтобы просмотреть список ваших товаров, выберите пункт \"Мои товары\".");
                        return;
                    case "❌ Удалить из избранного ❌":
                        db.DeleteDiscFromFav(db.GetEditDiscId(e.CallbackQuery.From.Id));
                        await bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, db.GetEditMessageId(e.CallbackQuery.From.Id));
                        await bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "✅ Товар удален из избранного!\n\nℹ️ Чтобы просмотреть список избранного, выберите пункт \"Избранное\".");
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
            //if (e.Message.From.Id == 652761067)
            //    return;
            try
            {
                if (message.Type == Telegram.Bot.Types.Enums.MessageType.Contact)
                    db.SetUserPhone(message.From.Id, message.Contact.PhoneNumber);
                switch (message.Text)
                {
                    default:
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
                                db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                db.SetLocation(text, message.From.Id);
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

                            case (int)awaitInfoType.discNumber:
                                db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                var temp1 = await bot.SendPhotoAsync(message.Chat.Id, db.GetPhotoForList(message.From.Id, Convert.ToInt32(message.Text)),
                                    caption: db.GetSelectedFromListDisk(message.From.Id, Convert.ToInt32(message.Text)),
                                    replyMarkup: Replies.editKeyboard(db.GetPlatform(message.From.Id)));
                                db.SetEditMessageId(message.From.Id, temp1.MessageId);
                                break;

                            case (int)awaitInfoType.searchResult:
                                db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                int discId = Convert.ToInt32(message.Text.Substring(5).Split(':')[0]);
                                db.SetSelectedDisc(message.From.Id, discId);
                                await bot.SendPhotoAsync(message.Chat.Id, db.GetPhoto(discId),
                                    caption: db.GetCaption(discId),
                                    replyMarkup: Replies.discKeyboard());
                                break;
                            case (int)awaitInfoType.favNumber:
                                if(Convert.ToInt32(message.Text) > db.GetAmountOfFav(message.From.Id) || Convert.ToInt32(message.Text) < 1)
                                {
                                    db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                    break;
                                }
                                db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                                int ownerId = db.GetOwnerId(db.GetSelectedDisc(message.From.Id));
                                var temp2 = await bot.SendPhotoAsync(message.Chat.Id, db.GetPhoto(db.GetFavDisc(message.From.Id,Convert.ToInt32(message.Text))),
                                    caption: db.GetSelectedFromFav(db.GetFavDisc(message.From.Id, Convert.ToInt32(message.Text))),
                                    replyMarkup: Replies.favKeyboard());
                                db.SetEditMessageId(message.From.Id, temp2.MessageId);
                                db.SetEditDiscId(message.From.Id, db.GetFavDisc(message.From.Id, Convert.ToInt32(message.Text)));
                                break;
                            default:
                                Console.WriteLine("Unprocessed message found. Deleted.".Pastel(Color.Gold));
                                break;
                        }
                        await bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                        return;

                    case "/start":
                        db.NewUser(message.From.Id);
                        await bot.SendTextMessageAsync(message.From.Id, $"Привет {message.From.Username}, это бот по обмену дисками!");
                        await bot.SendTextMessageAsync(message.From.Id, $"Пожалуйста, введите ваш город проживания:");
                        db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.location);
                        break;

                    case "/keyboard":
                        await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                            replyMarkup: Replies.keyboards.main);
                        break;

                    case "Назад ↩️":
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

                    case "Поиск 🔎":
                        await bot.SendTextMessageAsync(message.Chat.Id, "Чтобы начать поиск игр, нажмите на кнопку ниже:",
                            replyMarkup: Replies.keyboards.search);
                        break;

                    case "Мой профиль 👤":
                        await bot.SendTextMessageAsync(message.Chat.Id, "Выберите опцию из меню ниже:",
                            replyMarkup: Replies.keyboards.profile);
                        break;

                    case "Мои товары 💿":
                        if (db.UserHasDisk(message.From.Id))
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, db.GetUserDisks(message.From.Id));
                            db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.discNumber);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "У вас нет созданных дисков:",
                            replyMarkup: Replies.keyboards.profile);
                            db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                        }
                        break;

                    case "Добавить товар 💿":
                        if (e.Message.From.Username == null && db.GetUserPhone(message.From.Id) == "")
                        {
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "ℹ️ Не удалось получить ваш никнейм.\n\n" +
                                "Чтобы покупатель мог связатся с вами, добавьте свой номер телефона в настройках профиля.\n" +
                                "Вы также можете создать свой никнейм и повторить попытку.");
                            return;
                        }
                        db.NewDisc(message.From.Id);
                        var temp = await bot.SendPhotoAsync(message.Chat.Id, "AgACAgIAAxkBAAIGZF9aSti3CZNeKoW3AjRGDco3-45KAAL3rjEb0L7RSjbSrDV25SE0ECFzly4AAwEAAwIAA3gAA3CNAAIbBA",
                            caption: db.GetCaption(message.From.Id, true),
                            replyMarkup: Replies.editKeyboard(db.GetPlatform(message.From.Id)));
                        db.SetEditMessageId(message.From.Id, temp.MessageId);
                        break;

                    case "Избранное 🌟":
                        if (db.UserHasFav(message.From.Id))
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, db.GetUserFav(message.From.Id));
                            db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.favNumber);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "У вас избранных дисков:",
                            replyMarkup: Replies.keyboards.main);
                            db.SetAwaitInfoType(message.From.Id, (int)awaitInfoType.none);
                        }
                        break;
                }
            }
            catch (MessageIsNotModifiedException e1)
            {
                log.Error(e1.Message);
                return;
            }
            catch (FormatException e2)
            {
                log.Error(e2.Message);
                return;
            }
            catch (ApiRequestException e4)
            {
                log.Error(e4.Message);
                return;
            }
            catch (System.Net.Http.HttpRequestException e3)
            {
                log.Error(e3.Message);
                await bot.GetUpdatesAsync();
                return;
            }
        }
        private static async Task SetDiscCaptionAsync(long chat, int from)
        {
            await bot.EditMessageCaptionAsync(chat,
                db.GetEditMessageId(from),
                caption: db.GetCaption(from, true),
                replyMarkup: Replies.editKeyboard(db.GetPlatform(from)));
        }
    }
}