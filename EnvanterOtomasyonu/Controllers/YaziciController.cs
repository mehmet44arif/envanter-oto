using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EnvanterOtomasyonu.Models.Entity;



namespace EnvanterOtomasyonu.Models
{
    public class YaziciController : Controller
    {
        EnvanterOtoEntities7 en = new EnvanterOtoEntities7();
        public ActionResult Yazicilar()
        {
            var degerler = en.Yazicilar.ToList();

            return View(degerler);

        }
        [HttpPost]
        public async Task<ActionResult> KayitEkleGuncelle(FormCollection fc)
        {
            int yaziciID;
            if (!int.TryParse(fc["YaziciID"], out yaziciID))
            {
                yaziciID = 0; // Varsayılan olarak 0 atıyoruz
            }

            // Devamındaki kod...
            string ipAdresi = fc["IpAdresi"];
            string yaziciBirim = fc["YaziciBirim"];
            string yaziciMarkaModel = fc["YaziciMarkaModel"];

            Yazicilar yazici;

            if (yaziciID == 0)
            {
                // Yeni kayıt ekle
                yazici = new Yazicilar();
            }
            else
            {
                // Güncelleme yap
                yazici = await en.Yazicilar.FindAsync(yaziciID);
                if (yazici == null)
                {
                    TempData["Message"] = "Güncelleme başarısız! Yazıcı bulunamadı.";
                    return RedirectToAction("Yazicilar");
                }
            }

            // Mevcut kaydın kontrolü
            var existingYazici = await en.Yazicilar
                 .Where(y => y.IpAdresi == ipAdresi && y.YaziciBirim == yaziciBirim && y.YaziciMarkaModel == yaziciMarkaModel)
                 .FirstOrDefaultAsync();

            if (existingYazici != null && yaziciID == 0)
            {
                // Yeni kayıt eklerken aynı bilgilerin olduğunu kontrol et ve gerekirse kullanıcıya uyarı ver
                TempData["Message"] = "Bu yazıcı bilgileri zaten kullanımda, yeni bir kayıt eklenemiyor.";
                return RedirectToAction("Yazicilar");
            }

            // Alanları güncelle
            yazici.YaziciMarkaModel = yaziciMarkaModel;
            yazici.IpAdresi = ipAdresi;
            yazici.YaziciBirim = yaziciBirim;

            if (yaziciID == 0)
            {
                en.Yazicilar.Add(yazici);
            }

            await en.SaveChangesAsync();
            TempData["Message"] = "İşlem başarılı!";
            return RedirectToAction("Yazicilar");
        }



        public ActionResult Sil(int YaziciID)
        {
            Yazicilar yazici = en.Yazicilar.Find(YaziciID);
            if (yazici != null)
            {
                en.Yazicilar.Remove(yazici);
                en.SaveChanges();
            }
            return RedirectToAction("Yazicilar");
        }
       
        
       


    }
}