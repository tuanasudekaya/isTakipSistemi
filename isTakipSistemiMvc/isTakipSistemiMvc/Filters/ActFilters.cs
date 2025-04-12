using isTakipSistemiMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace isTakipSistemiMvc.Filters
{
    public class ActFilters : FilterAttribute, IActionFilter
    {
        isTakipDBEntities entity = new isTakipDBEntities();

        protected string aciklama;
        public ActFilters(string actAciklama)
        {
            this.aciklama = actAciklama;
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

            if (filterContext.Controller.TempData["bilgi"] != null)
            {
                Loglar log = new Loglar();

                log.logAciklama = this.aciklama + "(" + filterContext.Controller.TempData["bilgi"] + ")";
                log.actionAd = filterContext.ActionDescriptor.ActionName;
                log.controllerAd = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                log.tarih = DateTime.Now;
                log.personelId = Convert.ToInt32(filterContext.HttpContext.Session["personelId"]);

                entity.Loglar.Add(log);
                entity.SaveChanges();
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
        }
    }
}