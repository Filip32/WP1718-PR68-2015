using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class ZaPretrage
    {
        public string IdKorisnika { get; set; }
        public string Status { get; set; }
        public bool Dodatni { get; set; }
        public string Stavka1 { get; set; }
        public string Stavka2 { get; set; }
        public List<Voznja> Pretraga { get; set; } = null;
    }
}