using System;
using Pastel;
using System.Drawing;
using Telegram.Bot.Args;
using System.Diagnostics;

namespace DiskExchange_TG_Bot
{
    class Logger
    {
        public Logger()
        {
            while (true)
            {
                Console.WriteLine("Welcome to the @DiskExchangeBot administrator terminal.\n" +
                    "\nPress 's' to start the bot." +
                    "\nPress 'd' to accsess the database." +
                    "\nPress 'esc' to close this window.");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.S:
                        Console.Clear();
                        return;
                    case ConsoleKey.D:
                        new Process { // X:\Programs\SQLite\DiskExchangeDB.db
                            StartInfo = new ProcessStartInfo(@"E:\DiskExchangeDB.db") {
                                UseShellExecute = true
                            }
                        }.Start();
                        break;
                    case ConsoleKey.Escape:
                        System.Environment.Exit(0);
                        break;
                        
                }
                Console.Clear();
            }
        }
        public void Message(Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            string text = e.Message.Text;
            if (message.Photo != null)
                text = "[Фотография]";
            Console.Write($"[{DateTime.Now}][{message.From.Username} - {message.From.Id}][MESSAGE]: ".Pastel(Color.DarkTurquoise) + text.Pastel(Color.Turquoise));
        }
        public void Query(Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var query = e.CallbackQuery;
            Console.WriteLine($"[{DateTime.Now}][{query.From.Username} - {query.From.Id}][ QUERY ]: ".Pastel(Color.DarkTurquoise) + query.Data.Pastel(Color.Turquoise));
        }

        internal void Error(string message)
        {
            Console.Write($"\n[{DateTime.Now}][ ERROR ]: {message}".Pastel(Color.Red));
        }
    }
}
