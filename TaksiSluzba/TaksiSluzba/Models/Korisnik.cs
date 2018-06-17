using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class Korisnik
    {  
        public string Id { get; set; }
        public string KorisnickoIme {get; set;}
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public Enums.Pol Pol { get; set; }
        public string JMBG { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public Enums.Uloga Uloga { get; set; }
        public bool Slobodan { get; set; }
        public bool Blokiran { get; set; }
        public List<Voznja> Voznje { get; set; }

        public Korisnik()
        {
            Blokiran = false;
            Slobodan = true;
            Voznje = new List<Voznja>();
        }
    }
}