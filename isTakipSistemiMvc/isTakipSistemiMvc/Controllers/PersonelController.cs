using isTakipSistemiMvc.Filters;
using isTakipSistemiMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace isTakipSistemiMvc.Controllers
{
    public class PersonelController : Controller
    {
        isTakipDBEntities entity = new isTakipDBEntities();

        [AuthFilter(3)]
        public ActionResult Index()
        {
            var personeller = (from p in entity.personeller where p.personelYetkiTurId != 3 && p.aktiflik==true select p).ToList();

            return View(personeller);
        }
        [AuthFilter(3)]
        public ActionResult Olustur()
        {
            BirimYetkiTurler by = birimYetkiTurlerDoldur();
            ViewBag.mesaj = null;
            return View(by);
        }

        [HttpPost, ActFilters("Yeni Personel Eklendi")]
        public ActionResult Olustur(FormCollection fc)
        {
            string personelKullaniciAd = fc["kullaniciAd"];
            var personel = (from p in entity.personeller where p.personelKullanıcıAd == personelKullaniciAd select p).FirstOrDefault();

            int birimId = Convert.ToInt32(fc["birim"]);
            if (Convert.ToInt32(fc["yetkiTur"]) == 1)
            {
                var birimYoneticiKontrol = (from p in entity.personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 1 select p).FirstOrDefault();

                if (birimYoneticiKontrol != null)
                {
                    BirimYetkiTurler by = birimYetkiTurlerDoldur();
                    ViewBag.mesaj = "Bu Birimin Sadece Bir Yöneticisi Olabilir";
                    TempData["bilgi"] = null;
                    return View(by);
                }
            }


            if (personel == null)
            {
                personeller yeniPersonel = new personeller();

                yeniPersonel.personelAdSoyad = fc["adSoyad"];
                yeniPersonel.personelKullanıcıAd = personelKullaniciAd;
                yeniPersonel.personelParola = fc["parola"];
                yeniPersonel.personelBirimId = Convert.ToInt32(fc["birim"]);
                yeniPersonel.personelYetkiTurId = Convert.ToInt32(fc["yetkiTur"]);
                yeniPersonel.yeniPersonel = true;
                yeniPersonel.aktiflik = true;

                entity.personeller.Add(yeniPersonel);
                entity.SaveChanges();

                TempData["bilgi"] = yeniPersonel.personelKullanıcıAd;
                return RedirectToAction("Index");
            }
            else
            {
                BirimYetkiTurler by = birimYetkiTurlerDoldur();
                ViewBag.mesaj = "Kullanıcı Adı Bulunmaktadır";
                TempData["bilgi"] = null;
                return View(by);
            }
        }

        [AuthFilter(3)]
        public ActionResult Guncelle(int id)
        {
            var personel = (from p in entity.personeller where p.personelId == id select p).FirstOrDefault();

            return View(personel);
        }

        [HttpPost,ActFilters("Personel Güncellendi")]
        public ActionResult Guncelle(int id, string adSoyad)
        {
            personeller personel = (from p in entity.personeller where p.personelId == id select p).FirstOrDefault();

            personel.personelAdSoyad = adSoyad;

            entity.SaveChanges();
            TempData["bilgi"] = personel.personelKullanıcıAd;
            return RedirectToAction("Index");
        }

        [AuthFilter(3)]
        public ActionResult Sil(int id)
        {
            personeller personel = (from p in entity.personeller where p.personelId == id select p).FirstOrDefault();
            return View(personel);
        }

        [HttpPost,ActFilters("Personel Silindi")]
        public ActionResult Sil(FormCollection fc)
        {
            int personelId = Convert.ToInt32(fc["personelId"]);
            var personel = (from p in entity.personeller where p.personelId == personelId select p).FirstOrDefault();
            personel.aktiflik = false;
            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullanıcıAd;
            return RedirectToAction("Index");
        }

        public BirimYetkiTurler birimYetkiTurlerDoldur()
        {
            BirimYetkiTurler by = new BirimYetkiTurler();

            by.birimlerList = (from b in entity.birimler where b.aktiflik == true select b).ToList();
            by.yetkiTurlerList = (from y in entity.yetkiTurler where y.yetkiTurId != 3 select y).ToList();

            return by;
        }
    }
}
