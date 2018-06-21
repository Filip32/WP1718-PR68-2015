using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class ZavrsitiVoznju
    {
        public string IdVozaca { get; set; }
        public string IdVoznje { get; set; }
        public string Opis { get; set; }
        public string Cena { get; set; }
        public Lokacija Destinacija { get; set; }
    }
}