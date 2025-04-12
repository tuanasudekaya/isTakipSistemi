using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace isTakipSistemiMvc.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            Session.Abandon();//Çıkış yapabilmek için bütün sessionları boşaltır.
            return RedirectToAction("Index", "Login");
        }
    }
}