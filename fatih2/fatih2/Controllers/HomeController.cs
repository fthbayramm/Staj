using fatih2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace fatih2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Tablo 1 verileri
            List<ProductionEntry> productionEntries = new List<ProductionEntry>
            {
                new ProductionEntry { KayitNo = 1, Baslangic = DateTime.Parse("23.05.2020 07:30"), Bitis = DateTime.Parse("23.05.2020 08:30"), ToplamSure = TimeSpan.FromHours(1), Statu = "URETIM", DurusNedeni = "" },
                new ProductionEntry { KayitNo = 2, Baslangic = DateTime.Parse("23.05.2020 08:30"), Bitis = DateTime.Parse("23.05.2020 12:00"), ToplamSure = TimeSpan.FromHours(3.5), Statu = "URETIM", DurusNedeni = "" },
                new ProductionEntry { KayitNo = 3, Baslangic = DateTime.Parse("23.05.2020 12:00"), Bitis = DateTime.Parse("23.05.2020 13:00"), ToplamSure = TimeSpan.FromHours(1), Statu = "URETIM", DurusNedeni = "" },
                new ProductionEntry { KayitNo = 4, Baslangic = DateTime.Parse("23.05.2020 13:00"), Bitis = DateTime.Parse("23.05.2020 13:45"), ToplamSure = TimeSpan.FromMinutes(45), Statu = "DURUS", DurusNedeni = "ARIZA" },
                new ProductionEntry { KayitNo = 5, Baslangic = DateTime.Parse("23.05.2020 13:45"), Bitis = DateTime.Parse("23.05.2020 17:30"), ToplamSure = TimeSpan.FromHours(3.75), Statu = "URETIM", DurusNedeni = "" }
            };

            // Tablo 2 verileri
            List<BreakTime> breakTimes = new List<BreakTime>
            {
                new BreakTime { Baslangic = TimeSpan.FromHours(10), Bitis = TimeSpan.FromHours(10).Add(TimeSpan.FromMinutes(15)), DurusNedeni = "Çay Molası" },
                new BreakTime { Baslangic = TimeSpan.FromHours(12), Bitis = TimeSpan.FromHours(12).Add(TimeSpan.FromMinutes(30)), DurusNedeni = "Yemek Molası" },
                new BreakTime { Baslangic = TimeSpan.FromHours(15), Bitis = TimeSpan.FromHours(15).Add(TimeSpan.FromMinutes(15)), DurusNedeni = "Çay Molası" }
            };

            // Tablo 3 verileri
            List<ProductionEntry> modifiedEntries = ModifyEntries(productionEntries, breakTimes);

            // Tablo 3 çıktısı
            ViewBag.Tablo3Output = modifiedEntries;

            return View();
        }
        private List<ProductionEntry> ModifyEntries(List<ProductionEntry> entries, List<BreakTime> breaks)
        {
            List<ProductionEntry> modifiedEntries = new List<ProductionEntry>();

            // Her bir Tablo 1 girişini kontrol et
            foreach (var entry in entries)
            {
                modifiedEntries.Add(entry);

                // Eğer girişin durumu "URETIM" ise
                if (entry.Statu == "URETIM")
                {
                    // Standart duruş saatlerine göre böl
                    foreach (var breakTime in breaks)
                    {
                        DateTime breakStart = entry.Baslangic.Date.Add(breakTime.Baslangic);
                        DateTime breakEnd = entry.Baslangic.Date.Add(breakTime.Bitis);

                        // Eğer üretim başlangıcı duruşun başlangıcı ile çakışıyorsa
                        if (entry.Baslangic < breakEnd && entry.Bitis > breakStart)
                        {
                            // Duruş nedeni, duruş süresi ve yeni üretim girişi ekle
                            modifiedEntries.Add(new ProductionEntry
                            {
                                KayitNo = entry.KayitNo,
                                Baslangic = breakEnd,
                                Bitis = entry.Bitis,
                                ToplamSure = entry.Bitis - breakEnd,
                                Statu = "DURUS",
                                DurusNedeni = breakTime.DurusNedeni
                            });

                            // Eski girişi güncelle
                            entry.Bitis = breakStart;
                        }
                    }
                }
            }

            return modifiedEntries;
        }
    }
}