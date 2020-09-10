using System;
using System.Collections.Generic;
using System.Text;

namespace DiskExchange_TG_Bot
{
    class Disc
    {
        string photoId { get { return photoId; }} //file_id for disk's photo
        string game; //Name of the game
        double price; //Game's price. If set to 0, price will not display in the message
        int discId; //Id of the disc for the database
        int userId; //User's id for the database
        string exchange; //Games that seller wants to exchange for. If set to null, will not display
        short platform; //Game platform (1-PS4 2-XONE 3-SWITCH)
        
    }
}
