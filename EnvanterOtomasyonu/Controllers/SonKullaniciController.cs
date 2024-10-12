using EnvanterOtomasyonu.Models.Entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EnvanterOtomasyonu.Controllers
{
    public class SonKullaniciController : Controller
    {
        EnvanterOtoEntities7 en = new EnvanterOtoEntities7();

        public ActionResult Index()
        {
            var degerler = en.KullanıcıBilgisayar.ToList();
            return View(degerler);
        }

        [HttpPost]
        public async Task<ActionResult> KayitEkleGuncelle(FormCollection fc)
        {
            int sonKullaniciID;
            if (!int.TryParse(fc["SonKullaniciID"], out sonKullaniciID))
            {
                sonKullaniciID = 0; 
            }

            // Formdan verileri al
            string kullaniciAdi = fc["KullaniciAdi"];
            string birim = fc["Birim"];
            string marka = fc["Marka"];
            string seriNo = fc["SeriNo"];
            bool sapKullaniciMi = fc["Sapkullanicimi"] == "on";

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(birim) || string.IsNullOrEmpty(marka) || string.IsNullOrEmpty(seriNo))
            {
                TempData["Message"] = "Formdaki gerekli alanlar doldurulmamış.";
                return RedirectToAction("Index");
            }

            KullanıcıBilgisayar kullaniciBilgisayar;

            if (sonKullaniciID == 0)
            {
                // Yeni kayıt ekle
                kullaniciBilgisayar = new KullanıcıBilgisayar
                {
                    KullaniciAdi = kullaniciAdi,
                    Birim = birim,
                    Marka = marka,
                    SeriNo = seriNo,
                    Sapkullanicimi = sapKullaniciMi
                };

                // Aynı verilerin olup olmadığını kontrol et
                var existingKullanici = await en.KullanıcıBilgisayar
                    .Where(k => k.KullaniciAdi == kullaniciAdi && k.Birim == birim && k.Marka == marka && k.SeriNo == seriNo)
                    .FirstOrDefaultAsync();

                if (existingKullanici != null)
                {
                    TempData["Message"] = "Bu kayıt zaten mevcut!";
                    return RedirectToAction("Index"); // Hata mesajını göstermek için Index'e yönlendir
                }

                en.KullanıcıBilgisayar.Add(kullaniciBilgisayar);
            }
            else
            {
                // Güncelleme yap
                kullaniciBilgisayar = await en.KullanıcıBilgisayar.FindAsync(sonKullaniciID);
                if (kullaniciBilgisayar == null)
                {
                    TempData["Message"] = "Güncelleme başarısız! Kayıt bulunamadı.";
                    return RedirectToAction("Index");
                }

                kullaniciBilgisayar.KullaniciAdi = kullaniciAdi;
                kullaniciBilgisayar.Birim = birim;
                kullaniciBilgisayar.Marka = marka;
                kullaniciBilgisayar.SeriNo = seriNo;
                kullaniciBilgisayar.Sapkullanicimi = sapKullaniciMi;
            }

            try
            {
                await en.SaveChangesAsync();
                TempData["Message"] = "İşlem başarılı!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Kayıt işlemi sırasında bir hata oluştu: " + ex.InnerException?.Message; // Daha ayrıntılı hata mesajı
            }

            return RedirectToAction("Index");
        }

        public ActionResult Sil(int? sonKullaniciID)
        {
            KullanıcıBilgisayar sonkullanici = en.KullanıcıBilgisayar.Find(sonKullaniciID);
            if (sonkullanici != null)
            {
                en.KullanıcıBilgisayar.Remove(sonkullanici);
                en.SaveChanges();
            }
           

            return RedirectToAction("Index");
        }
    }
}