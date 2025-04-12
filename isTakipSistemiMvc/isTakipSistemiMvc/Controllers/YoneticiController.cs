﻿using isTakipSistemiMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace isTakipSistemiMvc.Controllers
{
    public class YoneticiController : Controller
    {
        isTakipDBEntities entity = new isTakipDBEntities();
            // GET: Yonetici
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim = (from b in entity.birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;

                var personeller = from p in entity.personeller
                                  join i in entity.isler on p.personelId equals i.isPersonelId into isler
                                  where p.personelBirimId == birimId && p.personelYetkiTurId != 1
                                  select new
                                  {
                                      personelAd = p.personelAdSoyad,
                                      isler = isler
                                  };//sorgu sonucunda personelin adı ve işler bilgisi geldi.
                List<ToplamIs> list = new List<ToplamIs>();
                foreach (var personel in personeller)
                {
                    ToplamIs toplamIs = new ToplamIs();
                    toplamIs.personelAdSoyad = personel.personelAd;
                    if (personel.isler.Count() == 0)
                    {
                        toplamIs.toplamIs = 0;
                    }
                    else
                    {
                        int toplam = 0;
                        foreach(var item in personel.isler)
                        {
                            if(item.yapilanTarih != null)
                            {
                                toplam++;
                            }
                        }
                        toplamIs.toplamIs = toplam;
                    }

                    list.Add(toplamIs);
                }
                IEnumerable<ToplamIs> siraliListe = new List<ToplamIs>();
                siraliListe = list.OrderByDescending(i => i.toplamIs);
                
                
                return View(siraliListe);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Ata() {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from p in entity.personeller where p.personelBirimId ==birimId && p.personelYetkiTurId == 2 select p).ToList();

                ViewBag.personeller = calisanlar;
               
                var birim = (from b in entity.birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {

            string isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            int secilenPersonelId =Convert.ToInt32(formCollection["selectPer"]);

            isler yeniIs = new isler();

            yeniIs.isBaslik = isBaslik;
            yeniIs.isAciklama = isAciklama;
            yeniIs.isPersonelId = secilenPersonelId;
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isDurumId = 1;
            yeniIs.isOkunma = false;//baslangıçta okunmadıgını belirttim

            entity.isler.Add(yeniIs);
            entity.SaveChanges();

            return RedirectToAction("Takip", "Yonetici");
        }

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from p in entity.personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2 && p.aktiflik==true select p).ToList();

                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Takip(int selectPer)
        {
            //selectPer içerisinde personel id tutulacak.
            var secilenPersonel = (from p in entity.personeller where p.personelId == selectPer select p).FirstOrDefault();
            //1 tane veri döneceği için firstordefault kullandık.
            //seçilen personel gelmiş oldu.

            TempData["secilen"] = secilenPersonel;
            return RedirectToAction("Listele", "Yonetici");
        }

        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                personeller secilenPersonel = (personeller)TempData["secilen"];
                try
                {
                    var isler = (from i in entity.isler where i.isPersonelId == secilenPersonel.personelId select i).ToList().OrderByDescending(i => i.iletilenTarih);

                    ViewBag.isler = isler; //işler bilgisini viewbage aktardık.
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count(); //iş sayısı listele viewinda bulunur.

                    return View();
                }
                catch (Exception)
                {
                    return RedirectToAction("Takip", "Yonetici");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }



        public ActionResult AyinElemani()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int simdikiYil = DateTime.Now.Year;//şimdiki yılı verir.

                List<int> yillar = new List<int>();

                for(int i= simdikiYil; i>=2024; i--)
                {
                    yillar.Add(i);
                }

                ViewBag.yillar = yillar;
                ViewBag.ayinElemani = null;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult AyinElemani(int aylar, int yillar)
        {

            int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

            var personeller = entity.personeller.Where(p => p.personelBirimId == birimId).Where(p => p.personelYetkiTurId != 1);

            DateTime baslangicTarih = Convert.ToDateTime("01-" + aylar + "-" + yillar);
            DateTime bitisTarih = Convert.ToDateTime("31-" + aylar + "-" + yillar);

            var isler = entity.isler.Where(i => i.yapilanTarih >= baslangicTarih).Where(i => i.yapilanTarih <= bitisTarih);
            //personeller ve işleri gruplayalım.
            var groupJoin = personeller.GroupJoin(isler, p => p.personelId, i => i.isPersonelId, (p, group) => new
            {
                sonucIsler = group,
                personelAd = p.personelAdSoyad
            });

            List<ToplamIs> list = new List<ToplamIs>();


            foreach(var personel in groupJoin)
            {
                ToplamIs toplamIs = new ToplamIs();
                toplamIs.personelAdSoyad = personel.personelAd;

                if (personel.sonucIsler.Count() == 0)
                {
                    toplamIs.toplamIs = 0;
                }
                else
                {
                    int toplam = 0;
                    foreach(var item in personel.sonucIsler)
                    {
                        if (item.yapilanTarih != null)
                        {
                            toplam++;
                        }
                    }
                    toplamIs.toplamIs = toplam;
                }
                list.Add(toplamIs);
            }

            IEnumerable<ToplamIs> siraliListe = new List<ToplamIs>();
            siraliListe = list.OrderByDescending(i => i.toplamIs);

            ViewBag.ayinElemani = siraliListe.FirstOrDefault();
            int simdikiYil = DateTime.Now.Year;

            List<int> sonucYillar = new List<int>();

            for (int i = simdikiYil; i >= 2023; i--)
            {
                sonucYillar.Add(i);
            }

            ViewBag.yillar = sonucYillar;
            return View();
        }

    }


    }
