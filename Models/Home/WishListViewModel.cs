using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Art_Gallery.Models.Home
{
    public class WishListViewModel 
    {


        Art_GalleryEntities _art = new Art_GalleryEntities();
        public IEnumerable<Tbl_Wishlist> whishlists1;

        public IEnumerable<Tbl_Wishlist> Getwishlists()
        {
            return whishlists1;
        }

        public void Setwhishlists(IEnumerable<Tbl_Wishlist> value)
        {
            whishlists1 = value;
        }

        public virtual Tbl_Art Tbl_Art { get; set; }
        public IEnumerable<Tbl_Wishlist> Tbl_Wishlists { get; set; }
        public IEnumerable<Art_GalleryEntities> Art_GalleryEntities { get; set; }

        public IEnumerable<Tbl_Art> arts { get; set; }

        public IEnumerable<Tbl_User> users { get; set; }
        public int WhishList_Id { get; set; }
        public Nullable<int> Art_Id { get; set; }
        public Nullable<int> User_Id { get; set; }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(p => p.Name, p => p.GetGetMethod().Invoke(this, null)).GetEnumerator();
        }

        
    }



}
