using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class Lokacija
    {
        public string XKordinata { get; set; }
        public string YKordinata { get; set; }
        public Adresa Adresa { get; set; }
    }
}