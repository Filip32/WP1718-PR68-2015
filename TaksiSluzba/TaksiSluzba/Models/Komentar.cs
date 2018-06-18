using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class Komentar
    {
        public string Opis { get; set; }
        public DateTime DatumObjave { get; set; }
        public string Korisnik { get; set; }
        public Voznja Voznja { get; set; }
        public string OcenaVoznje { get; set; }
    }
}