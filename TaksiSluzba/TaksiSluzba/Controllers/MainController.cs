﻿using System;
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
        private static List<List<string>> blokirani = new List<List<string>>();
        private static List<Voznja> sveVoznje = new List<Voznja>();
        private static List<Voznja> slobodneVoznje = new List<Voznja>();
        
        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            ReadFromXML(Enums.Uloga.Dispecer);
            ReadFromXML(Enums.Uloga.Vozac);
            ReadFromXML(Enums.Uloga.Musterija);
            ReadFromXMLBlokirani();
            ReadFromXMLSlobodneVoznje();
            ReadFromXMLSveVoznje();

            var requestUri = Request.RequestUri;
            return Redirect(requestUri.AbsoluteUri + "Content/index.html");
        }

        [HttpGet,Route("api/main/loginuser")]
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
                if (korisnik.Blokiran)
                {
                    return Ok("Korisnik je blokiran.");
                }
                ulokovani.Add(korisnik.Id, korisnik.KorisnickoIme);
                return Ok(korisnik);
            }
            else if (korisniklist.Exists(i => i.KorisnickoIme == user.Username && i.Lozinka == user.Password))
            {
                korisnik = korisniklist.Find(i => i.KorisnickoIme == user.Username);
                if (korisnik.Blokiran)
                {
                    return Ok("Korisnik je blokiran.");
                }
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
            vozac.Automobil.Vozac = vozac.KorisnickoIme;
            vozaclist.Add(vozac);
            WriteToXMl(Enums.Uloga.Vozac);
            return Ok("Vozač je dodat.");
        }

        [Route("api/main/updateprofile")]
        public IHttpActionResult UpdateProfile(Vozac osoba)
        {
            if (adminlist.Exists(i => i.KorisnickoIme == osoba.KorisnickoIme && i.Id != osoba.Id))
            {
                return Ok("Korisničko ime već postoji u sistemu.");
            }
            else if (vozaclist.Exists(i => i.KorisnickoIme == osoba.KorisnickoIme && i.Id != osoba.Id))
            {
                return Ok("Korisničko ime već postoji u sistemu.");
            }
            else if (korisniklist.Exists(i => i.KorisnickoIme == osoba.KorisnickoIme && i.Id != osoba.Id))
            {
                return Ok("Korisničko ime već postoji u sistemu.");
            }
            else
            {
                if (osoba.Uloga == Enums.Uloga.Vozac)
                {
                    Vozac v = vozaclist.Find(i => i.Id == osoba.Id);
                    vozaclist.Remove(v);
                    vozaclist.Add(osoba);
                    ulokovani[osoba.Id] = osoba.KorisnickoIme;
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
                        ulokovani[k.Id] = k.KorisnickoIme;
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

        [HttpGet,Route("api/main/get5closest")]
        public IHttpActionResult Closest5([FromUri]Double x, [FromUri]Double y)
        {
            Dictionary<string, Double> racun = new Dictionary<string, Double>();
            foreach (Vozac v in vozaclist)
            {
                Double xx = Convert.ToDouble(v.Lokacija.XKordinata);
                Double yy = Convert.ToDouble(v.Lokacija.YKordinata);
                Double r = Math.Sqrt(Math.Pow((xx - yy),2) + Math.Pow((x - y),2));
                racun.Add(v.Id, r);
            }

            racun = racun.OrderBy(o => o.Value).ToDictionary(k => k.Key, k => k.Value);

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            int s = 0;
            foreach (string id in racun.Keys)
            {
                Vozac v = vozaclist.Find(D => D.Id == id);
                if (v.Slobodan)
                {
                    dictionary.Add(v.Id, v.KorisnickoIme);
                    s++;
                    if (s == 5) break;
                }
            }

            return Ok(dictionary);
        }

        [Route("api/main/adddriveto")]
        public IHttpActionResult AddDrive(Voznja voznja)
        {
            if (voznja.Vozac != null)
            {
                Vozac v = vozaclist.Find(i => i.Id == voznja.Vozac);
                if (v.Blokiran) return Ok("Ne možete da izvršite ovu operaciju blokirani ste.");
                vozaclist.Remove(v);
                voznja.Id = v.Voznje.Count + v.KorisnickoIme;
                voznja.StatusVoznje = Enums.StatusVoznje.Formirana;
                voznja.VremePorudjbine = DateTime.Now;
                voznja.Vozac = v.KorisnickoIme;
                v.Voznje.Add(voznja);
                vozaclist.Add(v);
                Korisnik dispecer = adminlist.Find(i => i.Id == voznja.Dispecer);
                voznja.Dispecer = dispecer.KorisnickoIme;
                adminlist.Remove(dispecer);
                dispecer.Voznje.Add(voznja);
                adminlist.Add(dispecer);
                sveVoznje.Add(voznja);
                WriteToXMl(Enums.Uloga.Dispecer);
                WriteToXMl(Enums.Uloga.Vozac);
                WriteToXMSveVoznje();
                return Ok(dispecer);
            }
            else
            {
                Korisnik musterija = korisniklist.Find(i => i.Id == voznja.Musterija);
                if (musterija.Blokiran) return Ok("Ne možete da izvršite ovu operaciju blokirani ste.");
                korisniklist.Remove(musterija);
                voznja.Id = musterija.Voznje.Count + musterija.KorisnickoIme;
                voznja.StatusVoznje = Enums.StatusVoznje.Kreirana;
                voznja.VremePorudjbine = DateTime.Now;
                voznja.Musterija = musterija.KorisnickoIme;
                musterija.Voznje.Add(voznja);
                korisniklist.Add(musterija);
                sveVoznje.Add(voznja);
                slobodneVoznje.Add(voznja);
                WriteToXMl(Enums.Uloga.Musterija);
                WriteToXMSveVoznje();
                WriteToXMSlobodneVoznje();
                return Ok(musterija);
            }
        }

        [HttpGet,Route("api/main/getblokirane")]
        public IHttpActionResult GetBlokirane()
        {
            return Ok(blokirani);
        }

        [HttpGet,Route("api/main/blokiranje")]
        public IHttpActionResult Blokiranje([FromUri]string Username)
        {
            if (vozaclist.Exists(i => i.KorisnickoIme == Username))
            {
                Vozac vozac = vozaclist.Find(i => i.KorisnickoIme == Username);
                if (!vozac.Blokiran)
                {
                    vozaclist.Remove(vozac);
                    vozac.Blokiran = true;
                    vozaclist.Add(vozac);
                    List<string> list = new List<string>();
                    list.Add(vozac.Id);
                    list.Add(vozac.KorisnickoIme);
                    list.Add(vozac.Uloga.ToString());
                    blokirani.Add(list);
                    WriteToXMlBlokirani();
                    WriteToXMl(Enums.Uloga.Vozac);
                    return Ok(blokirani);
                }
            }

            if (korisniklist.Exists(i => i.KorisnickoIme == Username))
            {
                Korisnik korisnik = korisniklist.Find(i => i.KorisnickoIme == Username);
                if (!korisnik.Blokiran)
                {
                    korisniklist.Remove(korisnik);
                    korisnik.Blokiran = true;
                    korisniklist.Add(korisnik);
                    List<string> list = new List<string>();
                    list.Add(korisnik.Id);
                    list.Add(korisnik.KorisnickoIme);
                    list.Add(korisnik.Uloga.ToString());
                    blokirani.Add(list);
                    WriteToXMlBlokirani();
                    WriteToXMl(Enums.Uloga.Musterija);
                    return Ok(blokirani);
                }
            }

            return Ok("Dato korisničo ime ne postoji ili nije moguće blokirati tu osobu.");
        }

        [HttpGet,Route("api/main/odblokiraj")]
        public IHttpActionResult Odblokiranje([FromUri]string Id)
        {
            List<string> list = new List<string>();
            foreach (List<string> l in blokirani)
            {
                if (l[0] == Id)
                    list = l;
            }

            string uloga = list[2];

            if (uloga == "Musterija")
            {
                Korisnik k = korisniklist.Find(i => i.Id == Id);
                korisniklist.Remove(k);
                k.Blokiran = false;
                korisniklist.Add(k);
                blokirani.Remove(list);
                WriteToXMl(Enums.Uloga.Musterija);
                WriteToXMlBlokirani();
            }
            else
            {
                Vozac v = vozaclist.Find(i => i.Id == Id);
                vozaclist.Remove(v);
                v.Blokiran = false;
                vozaclist.Add(v);
                blokirani.Remove(list);
                WriteToXMl(Enums.Uloga.Vozac);
                WriteToXMlBlokirani();
            }

            return Ok(blokirani);
        }

        private void WriteToXMl(Enums.Uloga uloga)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/"+ uloga.ToString() + ".xml");

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
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/" + uloga.ToString() + ".xml");
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

        private void WriteToXMlBlokirani()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Blokirani.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<List<string>>));

            using (TextWriter writer = new StreamWriter(path))
            {
                    serializer.Serialize(writer, blokirani);
            }
        }

        private void ReadFromXMLBlokirani()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Blokirani.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<List<string>>));

            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                        blokirani = (List<List<string>>)serializer.Deserialize(reader);
                }
            }
            catch { }
        }

        private void WriteToXMSveVoznje()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/SveVoznje.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Voznja>));

            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, sveVoznje);
            }
        }

        private void ReadFromXMLSveVoznje()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/SveVoznje.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Voznja>));

            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                    sveVoznje = (List<Voznja>)serializer.Deserialize(reader);
                }
            }
            catch { }
        }

        private void WriteToXMSlobodneVoznje()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/SlobodneVoznje.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Voznja>));

            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, slobodneVoznje);
            }
        }

        private void ReadFromXMLSlobodneVoznje()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/SlobodneVoznje.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Voznja>));

            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                    slobodneVoznje = (List<Voznja>)serializer.Deserialize(reader);
                }
            }
            catch { }
        }
    }
}
