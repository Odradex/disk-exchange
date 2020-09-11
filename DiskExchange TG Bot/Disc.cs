using System;
using System.Collections.Generic;
using System.Text;

namespace DiskExchange_TG_Bot
{
    class Disc
    {
        private string[] platformNames = {"PS4","Xbox One","Swtich"};
        int userId; //User id
        public string photoId; //file_id for disk photo

        public string message //Text for the disk message. Generates automaticly, read-only
        {
            get
            {
                return
                    $"💿Игра:{name} | {platformNames[platform]}\n" +
                    $"💵Цена: {((price > 0) ? Convert.ToString(price) : "Не указана")}\n" + (exchange != "" ?
                    $"🔄Обмен на: {exchange}\n" : "") +
                    $"📍Расположение:{location}";
            }
        }

        string name; //Name of the game
        double price; //Game price. If set to 0, price will not display in the message
        byte platform; //Game platform (0-PS4 1-XONE 2-SWITCH)
        string exchange; //Games that seller wants to exchange for. If set to null, will not display
        string location; //Seller city

        public Disc(int user)
        {
            photoId = "AgACAgIAAxkBAAIGZF9aSti3CZNeKoW3AjRGDco3-45KAAL3rjEb0L7RSjbSrDV25SE0ECFzly4AAwEAAwIAA3gAA3CNAAIbBA";
            exchange = "";
            location = "Минск";
            userId = user;
        }

        public void SetPhoto(string fileId) { photoId = fileId; }
        public void SetPrice(int p) { price = p; }
        public void SetExchange(string e) { exchange = e; }
        public void SetPlatform(byte b) { platform = b; }
        public void SetName(string n) { name = n; }
        
    }
}