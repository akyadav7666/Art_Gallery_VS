using System;
using System.Collections.Generic;
using Art_Gallery.Models;

namespace Art_Gallery.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<Tbl_Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }

          }
}