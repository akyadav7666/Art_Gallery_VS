//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Art_Gallery.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Wishlist
    {
        public int WishList_Id { get; set; }
        public Nullable<int> Art_Id { get; set; }
        public Nullable<int> User_Id { get; set; }
    
        public virtual Tbl_Art Tbl_Art { get; set; }
        public virtual Tbl_User Tbl_User { get; set; }
    }
}
