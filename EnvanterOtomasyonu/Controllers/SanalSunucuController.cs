using EnvanterOtomasyonu.Models.Entity;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace EnvanterOtomasyonu.Controllers
{
    public class SanalSunucuController : Controller
    {
         EnvanterOtoEntities7 en =new EnvanterOtoEntities7();
        public ActionResult SanalSunucular()
        {
          
            var degerler=en.SanalSunucu.ToList();
            return View(degerler);
        }

        [HttpPost]
        public async Task<ActionResult> KayitEkleGuncelle(FormCollection fc)
        {
            int sanalsunucuID;
            if (!int.TryParse(fc["SanalSunucuID"] ,out sanalsunucuID ))
            {
                sanalsunucuID = 0;
            }

            string sunucuAdi = fc["SunucuAdi"];
            string kullanimAmaci = fc["KullanimAmaci"];
            string ipAdresi = fc["IpAdresi"];

            SanalSunucu sanalsunucu;
            if (sanalsunucuID == 0)
            {
                // Yeni kayıt ekle
                sanalsunucu = new SanalSunucu();
            }
            else
            {
                // Güncelleme yap
                sanalsunucu = await en.SanalSunucu.FindAsync(sanalsunucuID);
                if (sanalsunucu == null)
                {
                   
                    return RedirectToAction("SanalSunucular");
                }
            }
            var existingSunucu=await en.SanalSunucu
            .Where(s => s.IpAdresi == ipAdresi && s.SunucuAdi == sunucuAdi && s.KullanımAmacı == kullanimAmaci)
                 .FirstOrDefaultAsync();

            sanalsunucu.SunucuAdi = sunucuAdi;
            sanalsunucu.KullanımAmacı = kullanimAmaci;
            sanalsunucu.IpAdresi = ipAdresi;

            if (sanalsunucuID==0)
            {
                en.SanalSunucu.Add(sanalsunucu);
            }

            await en.SaveChangesAsync();
            TempData["Message"] = "İşlem başarılı!";
            return RedirectToAction("SanalSunucular");



        }
        public ActionResult Sil(int SanalSunucuID)
        {
            SanalSunucu sanalSunucu = en.SanalSunucu.Find(SanalSunucuID);
            if (sanalSunucu != null)
            {
                en.SanalSunucu.Remove(sanalSunucu);
                en.SaveChanges();
         
            
            }
            return RedirectToAction("SanalSunucular");
        }   
    }
}