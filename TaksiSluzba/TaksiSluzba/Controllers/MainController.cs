using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using TaksiSluzba.Models;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace TaksiSluzba.Controllers
{
    //[RoutePrefix("api/main")]
    public class MainController : ApiController
    {
        private static List<Korisnik> adminlist = new List<Korisnik>();
        private static List<Vozac> vozaclist = new List<Vozac>();
        private static List<Korisnik> korisniklist = new List<Korisnik>();
        private static Dictionary<string, Enums.Uloga> ulokovani = new Dictionary<string, Enums.Uloga>();

        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            ReadFromXML(Enums.Uloga.Dispecer);
            ReadFromXML(Enums.Uloga.Vozac);
            ReadFromXML(Enums.Uloga.Musterija);
            var requestUri = Request.RequestUri;
            return Redirect(requestUri.AbsoluteUri + "Content/index.html");
        }

        [HttpPost]
        [Route("api/main/loginuser")]
        public IHttpActionResult LoginUser(LoginUserClass user)
        {
            if (ulokovani.ContainsKey(user.Username))
            {
                return Ok("Dati korisnik je vec ulogovan.");
            }

            Korisnik korisnik;
            if (adminlist.Exists(i => i.KorisnickoIme == user.Username && i.Lozinka == user.Password))
            {
                korisnik = adminlist.Find(i => i.KorisnickoIme == user.Username);
                ulokovani.Add(korisnik.KorisnickoIme, korisnik.Uloga);
                return Ok(korisnik);
            }
            else if (vozaclist.Exists(i => i.KorisnickoIme == user.Username && i.Lozinka == user.Password))
            {
                korisnik = vozaclist.Find(i => i.KorisnickoIme == user.Username);
                ulokovani.Add(korisnik.KorisnickoIme, korisnik.Uloga);
                return Ok(korisnik);
            }
            else if (korisniklist.Exists(i => i.KorisnickoIme == user.Username && i.Lozinka == user.Password))
            {
                korisnik = korisniklist.Find(i => i.KorisnickoIme == user.Username);
                ulokovani.Add(korisnik.KorisnickoIme, korisnik.Uloga);
                return Ok(korisnik);
            }

            return Ok("Ne postoji trazeni korisnik ili je pogresna lozinka.");
        }


        [HttpPost]
        [Route("api/main/adduser")]
        public IHttpActionResult AddUser(Korisnik korisnik)
        { 
            if (korisniklist.Exists(i => i.KorisnickoIme == korisnik.KorisnickoIme) ||
               vozaclist.Exists(i => i.KorisnickoIme == korisnik.KorisnickoIme) ||
               adminlist.Exists(i => i.KorisnickoIme == korisnik.KorisnickoIme))
            {
                return Ok("Korisnicko ime je vec zauzeto.");
            }

                korisnik.Uloga = Enums.Uloga.Musterija;
                korisniklist.Add(korisnik);
                WriteToXMl(Enums.Uloga.Musterija);

            return Ok("Uspesno ste se registrovali.");
        }

        [HttpPost]
        [Route("api/main/adddriver")]
        public IHttpActionResult AddDriver(Vozac vozac)
        {
            if (vozaclist.Exists(i => i.KorisnickoIme == vozac.KorisnickoIme))
            {
                return Ok("Greska! Korisnicko ime vec postoji.");
            }
            else if (korisniklist.Exists(i => i.KorisnickoIme == vozac.KorisnickoIme))
            {
                return Ok("Greska! Korisnicko ime vec postoji.");
            }
            else if (adminlist.Exists(i => i.KorisnickoIme == vozac.KorisnickoIme))
            {
                return Ok("Greska! Korisnicko ime vec postoji.");
            }
            vozac.Uloga = Enums.Uloga.Vozac;
            vozaclist.Add(vozac);
            WriteToXMl(Enums.Uloga.Vozac);
            return Ok("Vozac je dodat.");
        }

        [HttpPost]
        [Route("api/main/updateprofile")]
        public IHttpActionResult UpdateProfile(Vozac osoba)
        {
            
            if (osoba.Uloga == Enums.Uloga.Vozac)
            {
                Vozac v = vozaclist.Find(i => i.KorisnickoIme == osoba.KorisnickoIme);
                vozaclist.Remove(v);
                vozaclist.Add(osoba);
                WriteToXMl(Enums.Uloga.Vozac);

                return Ok("Uspesno azuriran profil.");
            }
            else
            {
                string name = osoba.KorisnickoIme;
                Korisnik k = new Korisnik() { KorisnickoIme = osoba.KorisnickoIme, Lozinka = osoba.Lozinka, Ime = osoba.Ime, Prezime = osoba.Prezime, Pol = osoba.Pol, Email = osoba.Email, JMBG = osoba.JMBG, Telefon = osoba.Telefon, Uloga = osoba.Uloga, Voznje = osoba.Voznje };
                if (k.Uloga == Enums.Uloga.Dispecer)
                {
                    Korisnik v = adminlist.Find(i => i.KorisnickoIme == name);
                    adminlist.Remove(v);
                    adminlist.Add(k);
                    WriteToXMl(Enums.Uloga.Dispecer);
                }
                else
                {
                    Korisnik v = korisniklist.Find(i => i.KorisnickoIme == name);
                    korisniklist.Remove(v);
                    korisniklist.Add(k);
                    WriteToXMl(Enums.Uloga.Musterija);
                }
                return Ok("Uspesno azuriran profil.");
            }
        }

        [HttpPost]
        [Route("api/main/logoutuser")]
        public IHttpActionResult LogoutUser(LoginUserClass Username)
        {
            ulokovani.Remove(Username.Username);
            return Ok();
        }

        private void WriteToXMl(Enums.Uloga uloga)
        {
            string path = @"C:\Users\filip\Desktop\Projects\Web\Projekat\WP1718-PR68-2015\TaksiSluzba\" + uloga.ToString() + ".xml";
            XmlSerializer serializer;
            if (uloga == Enums.Uloga.Vozac)
                serializer = new XmlSerializer(typeof(List<Vozac>));
            else
                serializer = new XmlSerializer(typeof(List<Korisnik>));

            using (TextWriter writer = new StreamWriter(path))
            {
                if (uloga == Enums.Uloga.Vozac)
                    serializer.Serialize(writer, vozaclist);
                else if(uloga == Enums.Uloga.Musterija)
                    serializer.Serialize(writer, korisniklist);
                else
                    serializer.Serialize(writer, adminlist);
            }
        }

        private void ReadFromXML(Enums.Uloga uloga)
        {
            string path = @"C:\Users\filip\Desktop\Projects\Web\Projekat\WP1718-PR68-2015\TaksiSluzba\" + uloga.ToString() + ".xml";
            XmlSerializer serializer;
            if (uloga == Enums.Uloga.Vozac)
                    serializer = new XmlSerializer(typeof(List<Vozac>));
            else
                    serializer = new XmlSerializer(typeof(List<Korisnik>));

            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                    if (uloga == Enums.Uloga.Dispecer)
                        adminlist = (List<Korisnik>)serializer.Deserialize(reader);
                    else if (uloga == Enums.Uloga.Musterija)
                        korisniklist = (List<Korisnik>)serializer.Deserialize(reader);
                    else if (uloga == Enums.Uloga.Vozac)
                        vozaclist = (List<Vozac>)serializer.Deserialize(reader);
                }
            }
            catch { }
        }

    }
}
