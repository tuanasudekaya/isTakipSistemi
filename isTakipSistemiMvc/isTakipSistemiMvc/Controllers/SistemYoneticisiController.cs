using isTakipSistemiMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using isTakipSistemiMvc.Filters;

namespace isTakipSistemiMvc.Controllers
{

    public class SistemYoneticisiController : Controller
    {
        isTakipDBEntities entity = new isTakipDBEntities();
        // GET: SistemYoneticisi

        [AuthFilter(3)]
        public ActionResult Index()
        {
            var birimler = (from b in entity.birimler where b.aktiflik == true select b).ToList();

            string LabelBirim = "[";

            foreach (var birim in birimler)
            {
                LabelBirim += "'" + birim.birimAd + "',";
            }

            LabelBirim += "]";

            ViewBag.LabelBirim = LabelBirim;

            List<int> islerToplam = new List<int>();

            foreach (var birim in birimler)
            {
                int toplam = 0;

                var personeller = (from p in entity.personeller where p.personelBirimId == birim.birimId && p.aktiflik == true select p).ToList();

                foreach (var personel in personeller)
                {
                    var isler = (from i in entity.isler where i.isPersonelId == personel.personelId select i).ToList();

                    toplam += isler.Count();
                }



                islerToplam.Add(toplam);
            }
            string dataIs = "[";

            foreach (var i in islerToplam)
            {
                dataIs += "'" + i + "',";
            }

            dataIs += "]";

            ViewBag.dataIs = dataIs;

            return View();
        }

        public ActionResult Birim()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 3)
            {
                var birimler = (from b in entity.birimler where b.aktiflik == true select b).ToList();

                return View(birimler);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult Olustur()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 3)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        [HttpPost, ActFilters("Yeni Birim Eklendi")]
        public ActionResult Olustur(string birimAd)
        {
            birimler yeniBirim = new birimler();
            string yeniAd = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(birimAd); //eklenen birimi bas harflerini büyük yapacak.
            yeniBirim.birimAd = yeniAd;
            yeniBirim.aktiflik = true;


            entity.birimler.Add(yeniBirim);
            entity.SaveChanges();
            TempData["bilgi"] = yeniBirim.birimAd;
            return RedirectToAction("Birim");
        }

        public ActionResult Guncelle(int id)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 3)
            {
                var birim = (from b in entity.birimler where b.birimId == id select b).FirstOrDefault();

                return View(birim);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost, ActFilters("Birim Güncellendi")]
        public ActionResult Guncelle(FormCollection fc)
        {
            int birimId = Convert.ToInt32(fc["birimId"]);
            string yeniAd = fc["birimAd"];

            var birim = (from b in entity.birimler where b.birimId == birimId select b).FirstOrDefault();

            birim.birimAd = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(yeniAd);
            entity.SaveChanges();

            TempData["bilgi"] = birim.birimAd;
            return RedirectToAction("Birim");
        }

        [ActFilters("Birim Silindi")]
        public ActionResult Sil(int id)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 3)
            {
                var birim = (from b in entity.birimler where b.birimId == id select b).FirstOrDefault();

                birim.aktiflik = false;//birim silmek için aktifliğin false olması gerekir.


                entity.SaveChanges();
                TempData["bilgi"] = birim.birimAd;

                return RedirectToAction("Birim");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [AuthFilter(3)]
        public ActionResult Loglar()
        {
            var loglar = (from l in entity.Loglar orderby l.tarih descending select l).ToList();
            return View(loglar);
        }


    } 
}
