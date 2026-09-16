using booking_mgmt.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace booking_mgmt.Controllers
{
    public class MovieController : Controller
    {
        Dbhandle dbhandle=new Dbhandle();
        private List<SelectListItem> GetCategoryList()
        {
            String catstr = "SELECT * FROM Tbl_Movie_Category";
            DataTable dt = dbhandle.ddlquery(catstr);
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = Convert.ToString(dr.ItemArray[0]),
                    Text = Convert.ToString(dr.ItemArray[1])
                });
            }
            return list;
        }

        // GET: Movie
        public ActionResult Index()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Create", "User");
            }
            ViewBag.Uname = Session["User_name"];
            return View(dbhandle.GetMovies());
        }

        // GET: Movie/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Movie/Create
        public ActionResult Create()
        {
            ViewBag.category = GetCategoryList();
            return View();
        }

        // POST: Movie/Create
        [HttpPost]
        public ActionResult Create(Movie mov_model )
        {
            try
            {
                if (dbhandle.add_movie(mov_model))
                {
                    ViewBag.Message="Movie Inserted";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("About", "Home");
                }
            }
            catch
            {
                ViewBag.category = GetCategoryList();
                return View(mov_model);
            }
        }

        // GET: Movie/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Movie/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Movie/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Movie/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Movie mov_model)
        {
            try
            {
                if (dbhandle.del_movie(id))
                {
                    return RedirectToAction("Index", "Movie");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
