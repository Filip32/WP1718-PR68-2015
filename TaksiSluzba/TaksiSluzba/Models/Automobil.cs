using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class Automobil
    {
        public Vozac Vozac { get; set; }
        public string GodisteAutomobila { get; set; }
        public string RegistarskaTablica { get; set; }
        public string BrTaksija { get; set; }
        public Enums.TipVozila TipVozila { get; set; }
    }
}