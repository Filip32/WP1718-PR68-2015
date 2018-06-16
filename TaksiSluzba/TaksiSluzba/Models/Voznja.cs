using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class Voznja
    {
        public string Id { get; set; }
        public DateTime VremePorudjbine { get; set; }
        public Lokacija PolaznaTacka { get; set; }
        public Enums.TipVozila TipVozila { get; set; }
        public string Musterija { get; set; } //Korisnik
        public Lokacija Odrediste { get; set; }
        public string Dispecer { get; set; } //Dispcer
        public string Vozac { get; set; } //Vozac
        public string Iznos { get; set; }
        public Komentar Komentar { get; set; }
        public Enums.StatusVoznje StatusVoznje { get; set; }
    }
}