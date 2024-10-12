using EnvanterOtomasyonu.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EnvanterOtomasyonu.Controllers
{
    public class FizikselServerController : Controller
    {
        EnvanterOtoEntities7 en = new EnvanterOtoEntities7();

        public ActionResult FizikselServerlar()
        {
            var degerler = en.FizikselServer.ToList();
            return View(degerler);
        }

        [HttpPost]
        public async Task<ActionResult> KayitEkleGuncelle(FormCollection fc)
        {
            int fizikselsunucuID;
            if (!int.TryParse(fc["FizikselServerID"], out fizikselsunucuID))
            {
                fizikselsunucuID = 0;
            }

            string marka = fc["Marka"];
            string model = fc["Model"];
            string kullanimAmaci = fc["KullanımAmacı"];
            string ipAdresi = fc["IpAdresi"];

            FizikselServer fizikselServer;

            if (fizikselsunucuID == 0)
            {
                fizikselServer = new FizikselServer();
            }
            else
            {
                fizikselServer = await en.FizikselServer.FindAsync(fizikselsunucuID);
                if (fizikselServer == null)
                {
                    TempData["Message"] = "Güncelleme başarısız! Fiziksel sunucu bulunamadı.";
                    return RedirectToAction("FizikselServerlar");
                }
            }

            // Check for existing records with the same properties
            var existingServer = await en.FizikselServer
                .Where(s => s.IpAdresi == ipAdresi && s.Marka == marka && s.Model == model && s.KullanımAmacı == kullanimAmaci && s.FizikselServerID != fizikselsunucuID)
                .FirstOrDefaultAsync();

            if (existingServer != null)
            {
                TempData["Message"] = "Güncelleme başarısız! Aynı özelliklere sahip başka bir fiziksel sunucu mevcut.";
                return RedirectToAction("FizikselServerlar");
            }

            // Update fields
            fizikselServer.IpAdresi = ipAdresi;
            fizikselServer.Marka = marka;
            fizikselServer.Model = model;
            fizikselServer.KullanımAmacı = kullanimAmaci;

            if (fizikselsunucuID == 0)
            {
                en.FizikselServer.Add(fizikselServer);
            }

            await en.SaveChangesAsync();
            TempData["Message"] = "İşlem başarılı!";
            return RedirectToAction("FizikselServerlar");
        }

        public ActionResult Sil(int FizikselServerID)
        {
            FizikselServer fizikselServer = en.FizikselServer.Find(FizikselServerID);
            if (fizikselServer != null)
            {
                en.FizikselServer.Remove(fizikselServer);
                en.SaveChanges();
            }
            return RedirectToAction("FizikselServerlar");
        }
    }
}
