using System.Data.SQLite;

namespace DiskExchange_TG_Bot
{
    interface ISQLiteTable
    {
        static SQLiteConnection connection;
        public static SQLiteCommand cmd;
        static SQLiteDataReader rdr;
    }
    class Disk
    {
        string name;
        int platform;
    }
    class Offer
    {
        int id;
        string photo;
        string price;
        string exchange;
        int sellerId;
        string location;
        int disk;
    }
    enum InfoType : int
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
    class User : ISQLiteTable
    {
        int id;
        string location;
        string phone;
        int editMessageId;
        int editDiscId;
        int selectedDiscId;
        InfoType awaitInfoType;

        public User(int id, SQLiteConnection connection)
        {
            ISQLiteTable.cmd = new SQLiteCommand($"SELECT * FROM users WHERE user = {id}", connection);
            rdr = cmd.ExecuteReader();
            int counter = 1;
            while (rdr.Read())
            {
                if (counter == num)
                    return rdr.GetInt32(1);
                counter++;
            }
            return -1;
        }
    }
    class Favorites
    {
        int key;
        int offerID;
        int userID;
    }
    class DatabaseV2
    {

        SQLiteConnection connection = new SQLiteConnection("DiskExchangeDB.db");
    }
}
