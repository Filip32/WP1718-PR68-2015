using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class Korisnik
    {  
        public string KorisnickoIme {get; set;}
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public Enums.Pol Pol { get; set; }
        public string JMBG { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public Enums.Uloga Uloga { get; set; }
        public List<Voznja> Voznje { get; set; }
    }
}