using booking_mgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace booking_mgmt.Controllers
{
    public class UserController : Controller
    {
        Dbhandle dbhandle = new Dbhandle();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(User umodel)
        {
            try
            {
                if (dbhandle.u_login(umodel)!=0)
                {
                    //ViewBag.Message = "User Loged in";
                    Session["uid"]=dbhandle.u_login(umodel);
                    Session["User_name"] = umodel.User_Name;
                    return RedirectToAction("Index", "Movie");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return View(umodel);
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit()
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(User umodel)
        {
            try
            {
                if (Session["uid"] != null)
            {
                int id =(int) Session["uid"];
                if (dbhandle.editUser(id,umodel))
                {
                    return RedirectToAction("Index", "Movie");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to update profile. Please try again.");
                    
                }
            }
        }
            catch
            {
                return View();
    }
            return View(umodel);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
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
