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
        private static Dictionary<string, string> ulokovani = new Dictionary<string, string>();

        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            ReadFromXML(Enums.Uloga.Dispecer);
            ReadFromXML(Enums.Uloga.Vozac);
            ReadFromXML(Enums.Uloga.Musterija);
            var requestUri = Request.RequestUri;
            return Redirect(requestUri.AbsoluteUri + "Content/index.html");
        }

        [HttpGet]
        [Route("api/main/loginuser")]
        public IHttpActionResult LoginUser([FromUri] LoginUserClass user)
        {
            if (ulokovani.ContainsKey(user.Username))
            {
                return Ok("Dati korisnik je već ulogovan.");
            }

            Korisnik korisnik;
            if (adminlist.Exists(i => i.KorisnickoIme == user.Username && i.Lozinka == user.Password))
            {
                korisnik = adminlist.Find(i => i.KorisnickoIme == user.Username);
                ulokovani.Add(korisnik.Id,korisnik.KorisnickoIme);
                return Ok(korisnik);
            }
            else if (vozaclist.Exists(i => i.KorisnickoIme == user.Username && i.Lozinka == user.Password))
            {
                korisnik = vozaclist.Find(i => i.KorisnickoIme == user.Username);
                ulokovani.Add(korisnik.Id, korisnik.KorisnickoIme);
                return Ok(korisnik);
            }
            else if (korisniklist.Exists(i => i.KorisnickoIme == user.Username && i.Lozinka == user.Password))
            {
                korisnik = korisniklist.Find(i => i.KorisnickoIme == user.Username);
                ulokovani.Add(korisnik.Id, korisnik.KorisnickoIme);
                return Ok(korisnik);
            }

            return Ok("Ne postoji traženi korisnik ili je pogrešna lozinka.");
        }

        [Route("api/main/adduser")]
        public IHttpActionResult AddUser(Korisnik korisnik)
        { 
            if (korisniklist.Exists(i => i.KorisnickoIme == korisnik.KorisnickoIme) ||
               vozaclist.Exists(i => i.KorisnickoIme == korisnik.KorisnickoIme) ||
               adminlist.Exists(i => i.KorisnickoIme == korisnik.KorisnickoIme))
            {
                return Ok("Korisničko ime je već zauzeto.");
            }

                korisnik.Uloga = Enums.Uloga.Musterija;
                korisnik.Id = (adminlist.Count + vozaclist.Count + korisniklist.Count + 1).ToString(); 
                korisniklist.Add(korisnik);
                WriteToXMl(Enums.Uloga.Musterija);

            return Ok("Uspešno ste se registrovali.");
        }

        [Route("api/main/adddriver")]
        public IHttpActionResult AddDriver(Vozac vozac)
        {
            if (vozaclist.Exists(i => i.KorisnickoIme == vozac.KorisnickoIme))
            {
                return Ok("Greška! Korisničko ime već postoji.");
            }
            else if (korisniklist.Exists(i => i.KorisnickoIme == vozac.KorisnickoIme))
            {
                return Ok("Greška! Korisničko ime već postoji.");
            }
            else if (adminlist.Exists(i => i.KorisnickoIme == vozac.KorisnickoIme))
            {
                return Ok("Greška! Korisničko ime već postoji.");
            }
            vozac.Uloga = Enums.Uloga.Vozac;
            vozac.Id = (adminlist.Count + vozaclist.Count + korisniklist.Count + 1).ToString();
            vozac.Automobil.BrTaksija = vozac.Id + "_" + vozac.KorisnickoIme;
            vozaclist.Add(vozac);
            WriteToXMl(Enums.Uloga.Vozac);
            return Ok("Vozač je dodat.");
        }
        
        [Route("api/main/updateprofile")]
        public IHttpActionResult UpdateProfile(Vozac osoba)
        {   
            if (osoba.Uloga == Enums.Uloga.Vozac)
            {
                Vozac v = vozaclist.Find(i => i.Id == osoba.Id);
                vozaclist.Remove(v);
                vozaclist.Add(osoba);
                WriteToXMl(Enums.Uloga.Vozac);

                return Ok("Uspešno ažuriran profil.");
            }
            else
            {
                string ID = osoba.Id;
                Korisnik k = new Korisnik() { Id = ID, KorisnickoIme = osoba.KorisnickoIme, Lozinka = osoba.Lozinka, Ime = osoba.Ime, Prezime = osoba.Prezime, Pol = osoba.Pol, Email = osoba.Email, JMBG = osoba.JMBG, Telefon = osoba.Telefon, Uloga = osoba.Uloga, Voznje = osoba.Voznje };
                if (k.Uloga == Enums.Uloga.Dispecer)
                {
                    Korisnik v = adminlist.Find(i => i.Id == ID);
                    adminlist.Remove(v);
                    adminlist.Add(k);
                    WriteToXMl(Enums.Uloga.Dispecer);
                }
                else
                {
                    Korisnik v = korisniklist.Find(i => i.Id == ID);
                    korisniklist.Remove(v);
                    korisniklist.Add(k);
                    WriteToXMl(Enums.Uloga.Musterija);
                }
                return Ok("Uspešno ažuriran profil.");
            }
        }

        [HttpDelete, Route("api/main/logoutuser")]
        public IHttpActionResult LogoutUser(LoginUserClass Username)
        {
            foreach (string key in ulokovani.Keys)
            {
                if (Username.Username == ulokovani[key])
                {
                    ulokovani.Remove(key);
                    break;
                }
            }

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
