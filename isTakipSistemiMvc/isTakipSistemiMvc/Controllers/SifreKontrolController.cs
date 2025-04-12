using isTakipSistemiMvc.Filters;
using isTakipSistemiMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace isTakipSistemiMvc.Controllers
{
    public class SifreKontrolController : Controller
    {

        isTakipDBEntities entity = new isTakipDBEntities();
        // GET: SifreKontrol
        public ActionResult Index()
        {
            int personelId = Convert.ToInt32(Session["PersonelId"]);

            if (personelId == 0) return RedirectToAction("Index", "Login");

            var personel = (from p in entity.personeller where p.personelId == personelId select p).FirstOrDefault();

            ViewBag.mesaj = null;
            ViewBag.yetkiTurId = null;
            ViewBag.sitil = null;

            return View(personel);
        }
        [HttpPost,ActFilters("Parola Değiştirildi")]

        public ActionResult Index(int personelId, string eskiParola, string yeniParola, string yeniParolaKontrol)
        {
            var personel = (from p in entity.personeller where p.personelId == personelId select p).FirstOrDefault();

            if(eskiParola != personel.personelParola)
            {
                ViewBag.mesaj = "Eski Parolanızı Yanlış Girdiniz";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }
            if(yeniParola != yeniParolaKontrol)
            {
                ViewBag.mesaj = "Yeni Parola Ve Yeni Parola Tekrarı Eşleşmedi";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }
            if(yeniParola.Length<6 || yeniParola.Length > 15)
            {
                ViewBag.mesaj = "Yeni Parola En Az 6 Karakter En Çok 15 Karakter Olmalıdır";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }

            personel.personelParola = yeniParola;
            personel.yeniPersonel = false;
            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullanıcıAd;

            ViewBag.mesaj = "Parolanız Başarıyla Değiştirildi Anasayfaya Yönlendiriliyorsunuz";
            ViewBag.sitil = "alert alert-success";
            ViewBag.yetkiTurId = personel.personelYetkiTurId;

            return View(personel);

        }
    }
}