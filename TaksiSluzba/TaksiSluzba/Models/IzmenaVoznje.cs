using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class IzmenaVoznje
    {
        public string Id { get; set; }
        public string IdVoznje { get; set; }
        public Lokacija Lokacija { get; set; }
        public Enums.TipVozila TipVozila { get; set; }
        public string Opis { get; set; }
        public int Ocena { get; set; }
    }
}