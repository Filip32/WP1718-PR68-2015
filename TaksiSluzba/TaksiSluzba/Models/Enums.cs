using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaksiSluzba.Models
{
    public static class Enums
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Pol : int
        {
            Musko = 0,
            Zensko = 1
        };

        public enum Uloga : int
        {
            Musterija = 0,
            Dispecer = 1,
            Vozac = 2
        };

        public enum StatusVoznje : int
        {
            Kreirana = 0,
            Formirana = 1,
            Obradjena = 2,
            Prihvacena = 3,
            Otkazana = 4,
            Neuspesna = 5,
            Uspesna = 6
        };

        public enum TipVozila : int
        {
            Putnicko_Vozilo = 0,
            Kombi_Vozilo = 1
        }
    }
}