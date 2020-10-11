using System;
using System.Data.SQLite;
using System.Diagnostics.Tracing;
using System.Drawing;
using Pastel;
using Telegram.Bot.Requests;

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
            connection = new SQLiteConnection(path2);
            Console.Write("1/2: Connecting to Database... ".Pastel(Color.Yellow));
            Console.Beep();
            connection.Open();
            Console.WriteLine("[READY]");
        }
        #region AwaitInfo
        public void SetAwaitInfoType(int userId, int type)
        {
            cmd = new SQLiteCommand($"UPDATE users SET awaitInfoType = {type} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public int GetAwaitInfoType(int userId)
        {
            cmd = new SQLiteCommand($"SELECT awaitInfoType FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            if (rdr.HasRows == false)
                return 0;
            return rdr.GetInt32(0);
        }
        #endregion
        #region Platform
        public string GetPlatform(int userId)
        {
            cmd = new SQLiteCommand($"SELECT platform FROM disks WHERE id = (SELECT editDiscId FROM users WHERE id = {userId})", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetString(0);
        }
        public void SetPlatform(byte b, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET platform = '{platformNames[b]}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }

        #endregion
        internal discsArray[] Search(string query)
        {
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE name LIKE '%{query}%'", connection);
            rdr = cmd.ExecuteReader();
            discsArray[] ret = new discsArray[0];
            while(rdr.Read())
            {
                Array.Resize(ref ret, ret.Length + 1);
                ret[ret.Length - 1].id = rdr.GetInt32(0);
                ret[ret.Length - 1].name = rdr.GetString(1);
                ret[ret.Length - 1].platform = rdr.GetString(2);
                ret[ret.Length - 1].price = rdr.GetString(4);
                ret[ret.Length - 1].exchange = rdr.GetString(5);
                ret[ret.Length - 1].seller = rdr.GetInt32(6);
                ret[ret.Length - 1].location = rdr.GetString(7);
            }
            return ret;
        }
        #region Disc
        public int NewDisc(int Id)
        {
            cmd = new SQLiteCommand($"INSERT INTO disks(sellerId, location) VALUES({Id}, (SELECT location FROM users WHERE id = {Id}))", connection);
            cmd.ExecuteNonQueryAsync();
            cmd = new SQLiteCommand($"UPDATE users SET editDiscId = (SELECT last_insert_rowid() FROM disks) WHERE id = {Id}", connection);
            cmd.ExecuteNonQueryAsync();
            return 0;
        }
        public void DeleteDisc(int Id)
        {
            cmd = new SQLiteCommand($"DELETE FROM disks WHERE id = (SELECT editDiscId FROM users WHERE id = {Id})", connection);
            cmd.ExecuteNonQuery();
        }
        public void DeleteDiscFromFav(int Id)
        {
            cmd = new SQLiteCommand($"DELETE FROM favorites WHERE disc = {Id}", connection);
            cmd.ExecuteNonQuery();
        }
        private int GetDiscId(int Id)
        {
            cmd = new SQLiteCommand($"SELECT editDiscId FROM users WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        public void SetSelectedDisc(int userId, int discId)
        {
            cmd = new SQLiteCommand($"UPDATE users SET selectedDiscId = {discId} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public int GetSelectedDisc(int userId)
        {
            cmd = new SQLiteCommand($"SELECT selectedDiscId FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        #endregion
        public void NewUser(int userId)
        {
            cmd = new SQLiteCommand($"INSERT INTO users(id) VALUES ({userId})", connection);
            cmd.ExecuteNonQueryAsync();
        }
        #region Phone
        public void SetUserPhone(int userId, string phone)
        {
            cmd = new SQLiteCommand($"UPDATE users SET phone = '{phone}' WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public string GetUserPhone(int userId)
        {
            cmd = new SQLiteCommand($"SELECT phone FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            string str = rdr.GetString(0);
            return str;
        }
        #endregion
        #region Photo
        public void SetPhoto(string fileId, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET photo = '{fileId}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public bool UserHasFav(int Id)
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
        public bool UserHasDisk(int Id)
        {
            int counter = 0;
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while(rdr.Read())
                counter++;

            if (counter == 0)
                return false;
            return true;
        }

        public string GetPhoto(int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"SELECT photo FROM disks WHERE id = {discId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetString(0);
        }
        public string GetPhotoForList(int Id,int num)
        {
            num -= 1;
            int counter = 0;
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if(counter == num)
                    return rdr.GetString(3);
                counter++;
            }
            return "AgACAgIAAxkBAAIGZF9aSti3CZNeKoW3AjRGDco3-45KAAL3rjEb0L7RSjbSrDV25SE0ECFzly4AAwEAAwIAA3gAA3CNAAIbBA";
        }
        #endregion
        #region Exchange
        internal string GetExchange(int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"SELECT exchange FROM disks WHERE id = {discId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetString(0);
        }
        public void SetExchange(string e, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET exchange = '{e}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        #endregion
        public void SetPrice(string price, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET price = '{price}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetName(string n, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET name = '{n}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetEditMessageId(int userId, int messageId)
        {
            cmd = new SQLiteCommand($"UPDATE users SET editMessageId = {messageId} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetEditDiscId(int userId, int discId)
        {
            cmd = new SQLiteCommand($"UPDATE users SET editDiscId = {discId} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public int GetEditMessageId(int userId)
        {
            cmd = new SQLiteCommand($"SELECT editMessageId FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        public int GetEditDiscId(int userId)
        {
            cmd = new SQLiteCommand($"SELECT editDiscId FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        public int GetOwnerId(int Id)
        {
            cmd = new SQLiteCommand($"SELECT sellerId FROM disks WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        public string GetUserFav(int userId)
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
                cmd2 = new SQLiteCommand($"SELECT * FROM disks WHERE id = {a}", connection);
                rdr2 = cmd2.ExecuteReader();
                rdr2.Read();
                count++;
                temp += $"{count}: {rdr2.GetString(1)} | {rdr2.GetString(2)} | {rdr2.GetString(4)} BYN\n";
    
            }
            return temp + "\n\nЧтобы выбрать товар, отправьте его номер в следующем сообщении.";
        }
        public int GetAmountOfFav(int userId)
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
        public string GetUserDisks(int userId)
        {
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE sellerId = {userId}",connection);
            rdr = cmd.ExecuteReader();
            string temp = "";
            int count = 0;
            while (rdr.Read())
            {
                count++;
                temp += $"{count}: {rdr.GetString(1)} | {rdr.GetString(2)} | {rdr.GetString(4)} BYN\n";
            }
            return temp + "\n\nЧтобы выбрать товар, отправьте его номер в следующем сообщении.";
        }
        public int GetFavDisc(int userId, int num)
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
        public string GetSelectedFromFav(int Id)
        {
            string str = "Диск под данным номером не найден...";
            str = GetCaption(Id,false);
            return str;
        }
        public string GetSelectedFromListDisk(int Id,int num)
        {
            num -= 1;
            int counter = 0;
            string str ="Диск под данным номером не найден...";
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while(rdr.Read())
            {
                if(counter == num)
                {
                    cmd = new SQLiteCommand($"UPDATE users SET editDiscId = {rdr.GetInt32(0)} WHERE Id = {Id}", connection);
                    cmd.ExecuteReader();
                    str = GetCaption(rdr.GetInt32(0));
                }
                counter++;
            }
            return str;
        }

        internal void SetLocation(string text, int id)
        {
            cmd = new SQLiteCommand($"UPDATE users SET location = '{text}' WHERE Id = {id}", connection);
            cmd.ExecuteNonQueryAsync();
        }

        public string GetCaption(int Id, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                Id = GetDiscId(Id);
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            return $"💿Игра: {rdr.GetString(1)} | {rdr.GetString(2)}\n" +
                    $"💵Цена: {((rdr.GetString(4) != "")  ? rdr.GetString(4) + " BYN": "Не указана")}\n" + (rdr.GetString(5) != "" ?
                    $"🔄Обмен на: {rdr.GetString(5)}\n" : "") +
                    $"📍Расположение: {rdr.GetString(7)}";
        }
        public void AddSelectedDiscToFavorites(int userId)
        {
            int discId = GetSelectedDisc(userId);
            cmd = new SQLiteCommand($"INSERT OR REPLACE INTO favorites(key, disc, user) VALUES({(discId ^ userId) + userId},{discId},{userId})", connection);
            cmd.ExecuteNonQueryAsync();
        }

    }
}