﻿using isTakipSistemiMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace isTakipSistemiMvc.Controllers
{
    public class IsDurum
    {
        public string isBaslik { get; set; }
        public string isAciklama { get; set; }
        public DateTime? iletilenTarih { get; set; }
        public DateTime? yapilanTarih { get; set; }
        public string durumAd { get; set; }
        public string isYorum { get; set; }
    }

    public class CalisanController : Controller
    {
        isTakipDBEntities entity = new isTakipDBEntities();
        
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim = (from b in entity.birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;

                int personelId = Convert.ToInt32(Session["PersonelId"]);

                var isler = (from i in entity.isler where i.isPersonelId == personelId && i.isOkunma == false orderby i.iletilenTarih descending select i).ToList();

                ViewBag.isler = isler;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Index(int isId)
        {
            var tekIs=(from i in entity.isler where i.isId==isId select i).FirstOrDefault();

            tekIs.isOkunma = true;

            entity.SaveChanges();

            return RedirectToAction("Yap", "Calisan");

        }

        public ActionResult Yap()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from i in entity.isler where i.isPersonelId == personelId && i.isDurumId == 1 select i).ToList().OrderByDescending(i => i.iletilenTarih);
                ViewBag.isler = isler;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Yap(int isId, string isYorum)
        {
            var tekIs = (from i in entity.isler where i.isId == isId select i).FirstOrDefault();

            if (isYorum == "") isYorum = "Kullanıcı Yorum Yapmadı";

            tekIs.yapilanTarih = DateTime.Now;
            tekIs.isDurumId = 2;
            tekIs.isYorum = isYorum;

            entity.SaveChanges();

            return RedirectToAction("Index", "Calisan");
        }

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from i in entity.isler join d in entity.durumlar on i.isDurumId equals d.durumId where i.isPersonelId == personelId select i).ToList().OrderByDescending(i => i.iletilenTarih);

                IsDurumModel model = new IsDurumModel();

                List<IsDurum> list = new List<IsDurum>();

                foreach(var i in isler)
                {
                    IsDurum isDurum = new IsDurum();

                    isDurum.isBaslik = i.isBaslik;
                    isDurum.isAciklama = i.isAciklama;
                    isDurum.iletilenTarih = i.iletilenTarih;
                    isDurum.yapilanTarih = i.yapilanTarih;
                    isDurum.durumAd = i.durumlar.durumAd;
                    isDurum.isYorum = i.isYorum;


                    list.Add(isDurum);
                }
                model.isDurumlar = list;
                
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }

}