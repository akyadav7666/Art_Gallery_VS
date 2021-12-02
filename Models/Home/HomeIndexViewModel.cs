using Art_Gallery.Models;
using Art_Gallery.Repository;
using System;

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Art_Gallery.Models.Home
{
    public class HomeIndexViewModel
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        Art_GalleryEntities context = new Art_GalleryEntities();
        public IEnumerable<Tbl_Category> Tbl_Category { get; set; }
        public IEnumerable<Tbl_Art> Tbl_Art { get; set; }

        public IEnumerable<Tbl_Wishlist> Tbl_Wishlists { get; set; }
        public IEnumerable<Art_GalleryEntities> Art_GalleryEntities { get; set; }
        public IPagedList<Tbl_Art> ListOfProducts { get; set; }
        //public IEnumerable<tbl_Art> ListOfProducts { get; set; }

        
        public HomeIndexViewModel CreateModel(string search, int pageSize, int? page)
        {

            try
            {
                SqlParameter[] param = new SqlParameter[]{new SqlParameter("@search",search??(object)DBNull.Value)};
                IPagedList<Tbl_Art> data = context.Database.SqlQuery<Tbl_Art>("@search", param).ToList().ToPagedList(page ?? 1, pageSize);

                return new HomeIndexViewModel { ListOfProducts = data 
            };
                
            }
            catch(Exception ex)
            {
                if(ex is SqlException)
                {
                    
                }
                else
                {

                }

            }

            return new HomeIndexViewModel();
        }

        
    }
}