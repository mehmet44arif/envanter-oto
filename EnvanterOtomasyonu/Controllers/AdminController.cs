using EnvanterOtomasyonu.Models.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnvanterOtomasyonu.Controllers
{
    
    public class AdminController : Controller
    {
        EnvanterOtoEntities7 en = new EnvanterOtoEntities7();

        public ActionResult TabloIslemleri(string tabloAdi)
        {
            ViewBag.Islem = string.IsNullOrEmpty(tabloAdi) ? "Ekle" : "Güncelle";
            ViewBag.TabloAdi = tabloAdi;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TabloIslemleri(string tabloAdi, string islem, string yeniSutun)
        {
            if (string.IsNullOrEmpty(tabloAdi))
            {
                ModelState.AddModelError("", "Tablo Adı gereklidir");
                ViewBag.Islem = islem;
                return View();
            }

            try
            {
                if (islem == "Ekle")
                {
                    string sorgu = $"CREATE TABLE {tabloAdi} (Id INT PRIMARY KEY IDENTITY, CreatedAt DATETIME DEFAULT GETDATE())";
                    SQLKomutuCalistir(sorgu);
                }
                else if (islem == "Güncelle")
                {
                    if (!string.IsNullOrEmpty(yeniSutun))
                    {
                        string sorgu = $"ALTER TABLE {tabloAdi} ADD {yeniSutun} NVARCHAR(50)";
                        SQLKomutuCalistir(sorgu);
                    }
                }
                else if (islem == "Sil")
                {
                    string sorgu = $"DROP TABLE {tabloAdi}";
                    SQLKomutuCalistir(sorgu);
                }

                return RedirectToAction("Yonetim");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "İşlem sırasında bir hata oluştu: " + ex.Message);
                return View();
            }
        }

        private void SQLKomutuCalistir(string sorgu)
        {
            try
            {
                using (var baglanti = new SqlConnection(en.Database.Connection.ConnectionString))
                {
                    baglanti.Open();
                    using (var komut = new SqlCommand(sorgu, baglanti))
                    {
                        komut.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Veritabanı işlemi sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public ActionResult Yonetim()
        {
            var tablolar = en.Database.SqlQuery<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'").ToList();
            return View(tablolar);
        }
    }
}