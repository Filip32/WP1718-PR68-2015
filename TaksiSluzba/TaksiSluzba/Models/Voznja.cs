using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class Voznja
    {
        public DateTime VremePorudjbine { get; set; }
        public Lokacija PolaznaTacka { get; set; }
        public Enums.TipVozila TipVozila { get; set; }
        public Korisnik Musterija { get; set; }
        public Lokacija Odrediste { get; set; }
        public Korisnik Dispecer { get; set; }
        public Vozac Vozac { get; set; }
        public Double Iznos { get; set; }
        public Komentar Komentar { get; set; }
        public Enums.StatusVoznje StatusVoznje { get; set; }
    }
}