using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using isTakipSistemiMvc.Filters;
using isTakipSistemiMvc.Models;

namespace isTakipSistemiMvc.Controllers
{
    public class LoginController : Controller
    {
        isTakipDBEntities entity = new isTakipDBEntities();
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.mesaj = null;
            return View();
        }
        [HttpPost, ExcFilter]
        public ActionResult Index(string kullaniciAd, string parola)
        {
            //veritabanında kullanıcı adı ve parola kontrol ediliyor
            var personel = (from p in entity.personeller where p.personelKullanıcıAd == kullaniciAd && p.personelParola == parola && p.aktiflik == true select p).FirstOrDefault();



            if (personel != null)
            {
                var birim = (from b in entity.birimler where b.birimId == personel.personelBirimId select b).FirstOrDefault();
                Session["PersonelAdSoyad"] = personel.personelAdSoyad;
                Session["PersonelId"] = personel.personelId;
                Session["PersonelBirimId"] = personel.personelBirimId;
                Session["PersonelYetkiTurId"] = personel.personelYetkiTurId;
                if (birim == null)
                {
                    return RedirectToAction("Index", "SistemYoneticisi");
                }
                if (birim.aktiflik == true)
                {
                    if (personel.yeniPersonel == true)
                    {
                        return RedirectToAction("Index", "SifreKontrol");
                    }


                    switch (personel.personelYetkiTurId)
                    {
                        case 1:
                            return RedirectToAction("Index", "Yonetici");
                        case 2:
                            return RedirectToAction("Index", "Calisan");

                        default:
                            return View();
                    }


                }
                else
                {
                    ViewBag.mesaj = "Biriminiz Silindiği İçin Giriş Yapamazsınız.";
                    return View();
                }
            }
            else
            {
                ViewBag.mesaj = "kullanıcı adı veya parola yanlış.";
                return View();
            }
        }
    }
}