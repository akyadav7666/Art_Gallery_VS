using Art_Gallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Art_Gallery.Models.Home
{
    public class Item
    {
        public Tbl_Art Art { get; set; }

        public Tbl_Wishlist wishlist { get; set; }
        public int Quantity { get; set; }
    }
}