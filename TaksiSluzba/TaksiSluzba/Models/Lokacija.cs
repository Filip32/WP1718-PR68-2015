﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public class Lokacija
    {
        public double XKordinata { get; set; }
        public double YKordinata { get; set; }
        public Adresa Adresa { get; set; }
    }
}