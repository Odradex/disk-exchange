using System;
using System.Data.SQLite;
using System.Drawing;
using Pastel;
namespace DiskExchange_TG_Bot
{

    class Database
    {
        static SQLiteConnection connection;
        SQLiteCommand cmd;
        SQLiteDataReader rdr;
        string[] platformNames = { "PS4", "Xbox", "Switch" };
        public struct discsArray
        {
            public int id;
            public string name;
            public string price;
            public int seller;
            public string platform;
            public string exchange;
            public string location;
        }
        public Database()
        {
            string path1 = @"URI=file:X:\Programs\SQLite\DiskExchangeDB.db";
            string path2 = @"URI=file:D:\DataBase\DiskExchangeDB.db";
            connection = new SQLiteConnection(path1);
            Console.Write("1/2: Connecting to Database... ".Pastel(Color.Yellow));
            Console.Beep();
            connection.Open();
            Console.WriteLine("[READY]");
        }
        #region AwaitInfo
        public void SetAwaitInfoType(int userId, int type) // Done
        {
            cmd = new SQLiteCommand($"UPDATE users SET awaitInfoType = {type} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public int GetAwaitInfoType(int userId) // Done
        {
            cmd = new SQLiteCommand($"SELECT awaitInfoType FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            if (rdr.HasRows == false)
                return 0;
            return rdr.GetInt32(0);
        }
        #endregion //DONE
        #region Platform
        public string GetOfferPlatform(int userId) //Done
        {
            userId = GetEditOfferId(userId);
            cmd = new SQLiteCommand($"SELECT platform FROM fullOffers WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return platformNames[rdr.GetInt32(0)-1];
        }
        public void SetPlatform(byte b, int offerId, bool getIdFromUser = false) // Done
        {
            b++;
            if (getIdFromUser)
                offerId = GetEditOfferId(offerId);
            cmd = new SQLiteCommand($"INSERT OR IGNORE INTO disks (platform, name) VALUES({b}, (SELECT name FROM fullOffers WHERE id = {offerId}))", connection);
            cmd.ExecuteNonQueryAsync();
            cmd = new SQLiteCommand($"UPDATE offers SET diskId = (SELECT id FROM disks WHERE name = (SELECT name FROM fullOffers WHERE id = {offerId}) AND platform = {b}) WHERE id = {offerId}", connection);
            cmd.ExecuteNonQueryAsync();
        }

        #endregion
        internal discsArray[] Search(string query) // Done
        {
            cmd = new SQLiteCommand($"SELECT * FROM fullOffers WHERE lower(name) LIKE '%{query}%'", connection);
            rdr = cmd.ExecuteReader();
            discsArray[] ret = new discsArray[0];
            while(rdr.Read())
            {
                Array.Resize(ref ret, ret.Length + 1);
                ret[ret.Length - 1].id = rdr.GetInt32(0);
                ret[ret.Length - 1].name = rdr.GetString(7);
                ret[ret.Length - 1].platform = platformNames[rdr.GetInt32(8) - 1];
                ret[ret.Length - 1].price = rdr.GetString(2);
                ret[ret.Length - 1].exchange = rdr.GetString(3);
                ret[ret.Length - 1].seller = rdr.GetInt32(4);
                ret[ret.Length - 1].location = rdr.GetString(5);
            }
            return ret;
        }
        #region Disc
        public int NewOffer(int Id) //Done
        {
            cmd = new SQLiteCommand($"INSERT INTO offers(sellerId, location, diskId) VALUES({Id}, (SELECT location FROM users_info WHERE id = {Id}), 0)", connection);
            cmd.ExecuteNonQueryAsync();
            cmd = new SQLiteCommand($"UPDATE users SET editOfferId = (SELECT last_insert_rowid() FROM offers) WHERE id = {Id}", connection);
            cmd.ExecuteNonQueryAsync();
            return 0;
        }
        public void DeleteOffer(int Id) // Done
        {
            cmd = new SQLiteCommand($"DELETE FROM offers WHERE id = (SELECT editOfferId FROM users WHERE id = {Id})", connection);
            cmd.ExecuteNonQuery();
        }
        public void DeleteOfferFromFav(int Id) // Done
        {
            cmd = new SQLiteCommand($"DELETE FROM favorites WHERE offer = {Id}", connection);
            cmd.ExecuteNonQuery();
        }
        public void SetSelectedOffer(int userId, int offerId) // Done
        {
            cmd = new SQLiteCommand($"UPDATE users SET selectedOfferId = {offerId} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public int GetSelectedOffer(int userId) // Done
        {
            cmd = new SQLiteCommand($"SELECT selectedOfferId FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        #endregion
        public void NewUser(int userId, string username) // Done
        {
            cmd = new SQLiteCommand($"INSERT INTO users(id) VALUES ({userId})", connection);
            cmd.ExecuteNonQueryAsync();
            cmd = new SQLiteCommand($"INSERT INTO users_info(id, location, phone) VALUES ({userId}, 'Минск', '{username}')", connection);
            cmd.ExecuteNonQueryAsync();
        }
        #region Phone
        public void SetUserPhone(int userId, string phone) //Done
        {
            cmd = new SQLiteCommand($"UPDATE users_info SET phone = '+{phone}' WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public string GetUserPhone(int userId) // Done
        {
            cmd = new SQLiteCommand($"SELECT phone FROM users_info WHERE id = (SELECT sellerId FROM offers WHERE id = (SELECT selectedOfferId FROM users WHERE id = {userId}))", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            string str = rdr.GetString(0);
            return str;
        }
        #endregion
        #region Photo
        public void SetPhoto(string fileId, int offerId, bool getIdFromUser = false) // Done
        {
            if (getIdFromUser)
                offerId = GetEditOfferId(offerId);
            cmd = new SQLiteCommand($"UPDATE offers SET photo = '{fileId}' WHERE id = {offerId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public bool UserHasFav(int Id) // Done
        {
            int counter = 0;
            cmd = new SQLiteCommand($"SELECT * FROM favorites WHERE user = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
                counter++;

            if (counter == 0)
                return false;
            return true;
        }
        public bool UserHasOffers(int Id) // Done
        {
            int counter = 0;
            cmd = new SQLiteCommand($"SELECT * FROM offers WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while(rdr.Read())
                counter++;

            if (counter == 0)
                return false;
            return true;
        }

        public string GetPhoto(int offerId, bool getIdFromUser = false) // Done
        {
            if (getIdFromUser)
                offerId = GetEditOfferId(offerId);
            cmd = new SQLiteCommand($"SELECT photo FROM offers WHERE id = {offerId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetString(0);
        }
        public string GetPhotoForList(int Id,int num) // Done
        {
            num -= 1;
            int counter = 0;
            cmd = new SQLiteCommand($"SELECT * FROM offers WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if(counter == num)
                    return rdr.GetString(1);
                counter++;
            }
            return "AgACAgIAAxkBAAIGZF9aSti3CZNeKoW3AjRGDco3-45KAAL3rjEb0L7RSjbSrDV25SE0ECFzly4AAwEAAwIAA3gAA3CNAAIbBA";
        }
        #endregion
        #region Exchange
        internal string GetExchange(int offerId, bool getIdFromUser = false) // Done
        {
            if (getIdFromUser)
                offerId = GetEditOfferId(offerId);
            cmd = new SQLiteCommand($"SELECT exchange FROM offers WHERE id = {offerId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetString(0);
        }
        public void SetExchange(string e, int offerId, bool getIdFromUser = false) // Done
        {
            if (getIdFromUser)
                offerId = GetEditOfferId(offerId);
            cmd = new SQLiteCommand($"UPDATE offers SET exchange = '{e}' WHERE id = {offerId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        #endregion
        public void SetPrice(string price, int offerId, bool getIdFromUser = false) // Done
        {
            if (getIdFromUser)
                offerId = GetEditOfferId(offerId);
            cmd = new SQLiteCommand($"UPDATE offers SET price = '{price}' WHERE id = {offerId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetName(string n, int offerId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                offerId = GetEditOfferId(offerId);
            cmd = new SQLiteCommand($"INSERT INTO disks (name, platform) VALUES('{n}', 1)", connection);
            cmd.ExecuteNonQueryAsync();
            cmd = new SQLiteCommand($"UPDATE offers SET diskId = (SELECT id FROM disks WHERE name = '{n}') WHERE id = {offerId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetEditMessageId(int userId, int messageId) // Done
        {
            cmd = new SQLiteCommand($"UPDATE users SET editMessageId = {messageId} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetEditOfferId(int userId, int offerId) // Done
        {
            cmd = new SQLiteCommand($"UPDATE users SET editofferId = {offerId} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public int GetEditMessageId(int userId) // Done
        {
            cmd = new SQLiteCommand($"SELECT editMessageId FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        public int GetEditOfferId(int userId) // Done
        {
            cmd = new SQLiteCommand($"SELECT editofferId FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        #region favorites
        public int GetOwnerId(int Id) // Done
        {
            cmd = new SQLiteCommand($"SELECT sellerId FROM offers WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        public string GetUserFav(int userId) // Done
        {
            SQLiteCommand cmd2;
            SQLiteDataReader rdr2;
            int a = 0;
            cmd = new SQLiteCommand($"SELECT * FROM favorites WHERE user = {userId}", connection);
            rdr = cmd.ExecuteReader();
            string temp = "";
            int count = 0;
            while (rdr.Read())
            {
                a = rdr.GetInt32(1);
                cmd2 = new SQLiteCommand($"SELECT * FROM fullOffers WHERE id = {a}", connection);
                rdr2 = cmd2.ExecuteReader();
                rdr2.Read();
                count++;
                temp += $"{count}: {rdr2.GetString(7)} | {platformNames[rdr2.GetInt32(8) - 1]} | {rdr2.GetString(2)} BYN\n";

            }
            return temp + "\n\nЧтобы выбрать товар, отправьте его номер в следующем сообщении.";
        }
        public int GetAmountOfFav(int userId) // Done
        {
            cmd = new SQLiteCommand($"SELECT * FROM favorites WHERE user = {userId}", connection);
            rdr = cmd.ExecuteReader();
            int count = 0;
            while (rdr.Read())
            {
                count++;
            }
            return count;
        }
        public int GetFavDisc(int userId, int num) // Done
        {
            cmd = new SQLiteCommand($"SELECT * FROM favorites WHERE user = {userId}", connection);
            rdr = cmd.ExecuteReader();
            int counter = 1;
            while(rdr.Read())
            {
                if (counter == num)
                    return rdr.GetInt32(1);
                counter++;
            }
            return -1;
        }
        public string GetSelectedFromFav(int Id) // Done
        {
            string str = "Диск под данным номером не найден...";
            str = GetCaption(Id, false);
            return str;
        }
        #endregion
        public string GetUserOffers(int userId) // Done
        {
            cmd = new SQLiteCommand($"SELECT * FROM fullOffers WHERE sellerId = {userId}",connection);
            rdr = cmd.ExecuteReader();
            string temp = "";
            int count = 0;
            while (rdr.Read())
            {
                count++;
                temp += $"{count}: {rdr.GetString(7)} | {platformNames[rdr.GetInt32(8)-1]} | {rdr.GetString(2)} BYN\n";
            }
            return temp + "\n\nЧтобы выбрать товар, отправьте его номер в следующем сообщении.";
        }
        public string GetSelectedFromListOffer(int Id,int num) // Done
        {
            num -= 1;
            int counter = 0;
            string str ="Диск под данным номером не найден...";
            cmd = new SQLiteCommand($"SELECT * FROM offers WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while(rdr.Read())
            {
                if(counter == num)
                {
                    cmd = new SQLiteCommand($"UPDATE users SET editOfferId = {rdr.GetInt32(0)} WHERE Id = {Id}", connection);
                    cmd.ExecuteReader();
                    str = GetCaption(rdr.GetInt32(0));
                }
                counter++;
            }
            return str;
        }
        internal void SetLocation(string text, int id) // Done
        {
            cmd = new SQLiteCommand($"UPDATE users_info SET location = '{text}' WHERE Id = {id}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public string GetCaption(int Id, bool getIdFromUser = false) // Done
        {
            if (getIdFromUser)
                Id = GetEditOfferId(Id);
            cmd = new SQLiteCommand($"SELECT * FROM fullOffers WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            string ret = $"💿Игра: {rdr.GetString(7)} | {platformNames[rdr.GetInt32(8)-1]}\n" +
                    $"💵Цена: {((rdr.GetString(2) != "")  ? rdr.GetString(2) + " BYN": "Не указана")}\n" + (rdr.GetString(3) != "" ?
                    $"🔄Обмен на: {rdr.GetString(3)}\n" : "") +
                    $"📍Расположение: {rdr.GetString(5)}";
            return ret;
        }
        public void AddSelectedOfferToFavorites(int userId) // Done
        {
            int offerId = GetSelectedOffer(userId);
            cmd = new SQLiteCommand($"INSERT OR REPLACE INTO favorites(key, offer, user) VALUES({(offerId ^ userId) + userId},{offerId},{userId})", connection);
            cmd.ExecuteNonQueryAsync();
        }
    }
}