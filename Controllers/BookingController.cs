using booking_mgmt.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace booking_mgmt.Controllers
{
    public class BookingController : Controller
    {
        Dbhandle dbhandle = new Dbhandle();
        private List<SelectListItem> GetCategoryList()
        {
            var list = new List<SelectListItem>();
            var categories = dbhandle.GetCat();
            foreach (var item in categories)
            {
                list.Add(new SelectListItem
                {
                    Value = item.Cat_ID.ToString(),
                    Text = item.Cat_Type
                });
            }
            return list;
        }
        public JsonResult GetMoviesByCategory(int catId)
        {
            List<object> list = new List<object>();
            string query = "SELECT Movie_ID, Movie_name, Rate FROM Tbl_Movie WHERE Cat_ID = @Cat_ID";

            DataTable dt = dbhandle.ddlquery("SELECT Movie_ID, Movie_name, Rate FROM Tbl_Movie WHERE Cat_ID = " + catId);

            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new
                {
                    Value = dr["Movie_ID"].ToString(),
                    Text = dr["Movie_name"].ToString(),
                    Rate = dr["Rate"].ToString()
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        // GET: Booking
        public ActionResult Index(int? catId)
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Create", "User"); 
            }
            ViewBag.CatList = GetCategoryList();
            List<Booking> bookingList = dbhandle.GetBookings();
            return View(bookingList);
        }

        // GET: Booking/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Booking/Create
        public ActionResult Create()
        {
            int currentUserId = Session["uid"] != null ? Convert.ToInt32(Session["uid"]) : 1;

            Booking model = new Booking
            {
                User_ID = currentUserId,
                Categories = GetCategoryList(),
                Movies = new List<SelectListItem>()
            };

            return View(model);
        }

        // POST: Booking/Create
        [HttpPost]
        public ActionResult Create(Booking bkmodel)
        {
            try
            {
                int uid = Session["uid"] != null ? Convert.ToInt32(Session["uid"]) : 1;

                // Fetch accurate rate from DB and calculate total amount
                int rate = dbhandle.calRate(bkmodel.Movie_ID);
                int calculatedAmount = rate * bkmodel.No_of_tickets;

                if (dbhandle.addBooking(bkmodel, uid, calculatedAmount))
                {
                    return RedirectToAction("Index", "Movie");
                }
            }
            catch
            {
                ViewBag.category = GetCategoryList();
            }
            bkmodel.Categories = GetCategoryList();
            bkmodel.Movies = new List<SelectListItem>();
            return View(bkmodel);
        }

        // GET: Booking/Edit/5
        public ActionResult Edit(int id)
        {
            Booking bk = dbhandle.GetBookingByID(id);
            if (bk == null)
            {
                return HttpNotFound();
            }

            // Populate Category dropdown
            bk.Categories = GetCategoryList();

            // Populate Movies dropdown filtered by the current Category
            DataTable dt = dbhandle.ddlquery("SELECT Movie_ID, Movie_name FROM Tbl_Movie WHERE Cat_ID = " + bk.Cat_ID);
            List<SelectListItem> movieList = new List<SelectListItem>();
            foreach (DataRow dr in dt.Rows)
            {
                movieList.Add(new SelectListItem
                {
                    Value = dr["Movie_ID"].ToString(),
                    Text = dr["Movie_name"].ToString(),
                    Selected = (Convert.ToInt32(dr["Movie_ID"]) == bk.Movie_ID)
                });
            }
            bk.Movies = movieList;

            return View(bk);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Booking bkmodel)
        {
            try
            {
                int rate = dbhandle.calRate(bkmodel.Movie_ID);
                bkmodel.Amount = rate * bkmodel.No_of_tickets;
                bkmodel.Booking_ID = id;

                if (dbhandle.UpdateBooking(bkmodel))
                {
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
            bkmodel.Categories = GetCategoryList();
            bkmodel.Movies = new List<SelectListItem>();
            return View(bkmodel);
        }

        // GET: Booking/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Booking/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
