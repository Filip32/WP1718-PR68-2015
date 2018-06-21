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
            ReadFromXMLUlogovani();

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
                WriteToXMUlogovani();
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
                WriteToXMUlogovani();
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
                WriteToXMUlogovani();
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
                    
                    string staro = v.KorisnickoIme;
                    string novo = osoba.KorisnickoIme;

                    List<Voznja> vv = new List<Voznja>();
                    sveVoznje.ForEach((item) => { vv.Add(item); });
                    foreach (Voznja voznja in vv)
                    {
                        if (voznja.Vozac == staro)
                        {
                            sveVoznje.Remove(voznja);
                            voznja.Dispecer = novo;
                            sveVoznje.Add(voznja);
                        }
                    }
                    WriteToXMSveVoznje();

                    List<Vozac> vvv = new List<Vozac>();
                    vozaclist.ForEach((item) => { vvv.Add(item); });
                    foreach (Vozac korisnik in vvv)
                    {
                        vozaclist.Remove(korisnik);
                        List<Voznja> v1 = new List<Voznja>();
                        korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                        foreach (Voznja voznja in v1)
                        {
                            if (voznja.Vozac == staro)
                            {
                                korisnik.Voznje.Remove(voznja);
                                voznja.Dispecer = novo;
                                korisnik.Voznje.Add(voznja);
                            }
                        }
                        vozaclist.Add(korisnik);
                    }
                    WriteToXMl(Enums.Uloga.Vozac);

                    List<Korisnik> vvvv = new List<Korisnik>();
                    korisniklist.ForEach((item) => { vvvv.Add(item); });
                    foreach (Korisnik korisnik in vvvv)
                    {
                        korisniklist.Remove(korisnik);
                        List<Voznja> v1 = new List<Voznja>();
                        korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                        foreach (Voznja voznja in v1)
                        {
                            if (voznja.Vozac == staro)
                            {
                                korisnik.Voznje.Remove(voznja);
                                voznja.Dispecer = novo;
                                korisnik.Voznje.Add(voznja);
                            }
                        }
                        korisniklist.Add(korisnik);
                    }
                    WriteToXMl(Enums.Uloga.Musterija);

                    List<Korisnik> vvvvv = new List<Korisnik>();
                    adminlist.ForEach((item) => { vvvvv.Add(item); });
                    foreach (Korisnik korisnik in vvvvv)
                    {
                        adminlist.Remove(korisnik);
                        List<Voznja> v1 = new List<Voznja>();
                        korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                        foreach (Voznja voznja in v1)
                        {
                            if (voznja.Vozac == staro)
                            {
                                korisnik.Voznje.Remove(voznja);
                                voznja.Dispecer = novo;
                                korisnik.Voznje.Add(voznja);
                            }
                        }
                        adminlist.Add(korisnik);
                    }
                    WriteToXMl(Enums.Uloga.Dispecer);
                    //--------------------------------------------------
                    v = vozaclist.Find(i => i.Id == osoba.Id);
                    vozaclist.Remove(v);
                    osoba.Voznje = v.Voznje;
                    vozaclist.Add(osoba);
                    ulokovani[osoba.Id] = osoba.KorisnickoIme;
                    WriteToXMl(Enums.Uloga.Vozac);

                    return Ok(osoba);
                }
                else
                {
                    string ID = osoba.Id;
                    Korisnik k = new Korisnik() { Id = ID, Blokiran = osoba.Blokiran, KorisnickoIme = osoba.KorisnickoIme, Lozinka = osoba.Lozinka, Ime = osoba.Ime, Prezime = osoba.Prezime, Pol = osoba.Pol, Email = osoba.Email, JMBG = osoba.JMBG, Telefon = osoba.Telefon, Uloga = osoba.Uloga, Voznje = osoba.Voznje };

                    if (k.Uloga == Enums.Uloga.Dispecer)
                    {
                        Korisnik v = adminlist.Find(i => i.Id == ID);
                        //--------------------------------------------------
                        string staro = v.KorisnickoIme;
                        string novo = k.KorisnickoIme;

                        List<Voznja> vv = new List<Voznja>();
                        sveVoznje.ForEach((item) =>{vv.Add(item);});
                        foreach (Voznja voznja in vv)
                        {
                            if (voznja.Dispecer == staro)
                            {
                                sveVoznje.Remove(voznja);
                                voznja.Dispecer = novo;
                                sveVoznje.Add(voznja);
                            }
                        }
                        WriteToXMSveVoznje();

                        List<Vozac> vvv = new List<Vozac>();
                        vozaclist.ForEach((item) => { vvv.Add(item); });
                        foreach (Vozac korisnik in vvv)
                        {
                            vozaclist.Remove(korisnik);
                            List<Voznja> v1 = new List<Voznja>();
                            korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                            foreach (Voznja voznja in v1)
                            {
                                if (voznja.Dispecer == staro)
                                {
                                    korisnik.Voznje.Remove(voznja);
                                    voznja.Dispecer = novo;
                                    korisnik.Voznje.Add(voznja);
                                }
                            }
                            vozaclist.Add(korisnik);
                        }
                        WriteToXMl(Enums.Uloga.Vozac);

                        List<Korisnik> vvvv = new List<Korisnik>();
                        korisniklist.ForEach((item) => { vvvv.Add(item); });
                        foreach (Korisnik korisnik in vvvv)
                        {
                            korisniklist.Remove(korisnik);
                            List<Voznja> v1 = new List<Voznja>();
                            korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                            foreach (Voznja voznja in v1)
                            {
                                if (voznja.Dispecer == staro)
                                {
                                    korisnik.Voznje.Remove(voznja);
                                    voznja.Dispecer = novo;
                                    korisnik.Voznje.Add(voznja);
                                }
                            }
                            korisniklist.Add(korisnik);
                        }
                        WriteToXMl(Enums.Uloga.Musterija);

                        List<Korisnik> vvvvv = new List<Korisnik>();
                        adminlist.ForEach((item) => { vvvvv.Add(item); });
                        foreach (Korisnik korisnik in vvvvv)
                        {
                            adminlist.Remove(korisnik);
                            List<Voznja> v1 = new List<Voznja>();
                            korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                            foreach (Voznja voznja in v1)
                            {
                                if (voznja.Dispecer == staro)
                                {
                                    korisnik.Voznje.Remove(voznja);
                                    voznja.Dispecer = novo;
                                    korisnik.Voznje.Add(voznja);
                                }
                            }
                            adminlist.Add(korisnik);
                        }
                        WriteToXMl(Enums.Uloga.Dispecer);
                        //--------------------------------------------------
                        v = adminlist.Find(i => i.Id == ID);
                        adminlist.Remove(v);
                        k.Voznje = v.Voznje;
                        adminlist.Add(k);
                        ulokovani[k.Id] = k.KorisnickoIme;
                        WriteToXMl(Enums.Uloga.Dispecer);
                        return Ok(k);
                    }
                    else
                    {
                        Korisnik v = korisniklist.Find(i => i.Id == ID);

                        string staro = v.KorisnickoIme;
                        string novo = k.KorisnickoIme;

                        List<Voznja> vs = new List<Voznja>();
                        slobodneVoznje.ForEach((item) => { vs.Add(item); });
                        foreach (Voznja voznja in vs)
                        {
                            if (voznja.Musterija == staro)
                            {
                                slobodneVoznje.Remove(voznja);
                                voznja.Dispecer = novo;
                                slobodneVoznje.Add(voznja);
                            }
                        }
                        WriteToXMSlobodneVoznje();

                        List<Voznja> vv = new List<Voznja>();
                        sveVoznje.ForEach((item) => { vv.Add(item); });
                        foreach (Voznja voznja in vv)
                        {
                            if (voznja.Musterija == staro)
                            {
                                sveVoznje.Remove(voznja);
                                voznja.Dispecer = novo;
                                sveVoznje.Add(voznja);
                            }
                        }
                        WriteToXMSveVoznje();

                        List<Vozac> vvv = new List<Vozac>();
                        vozaclist.ForEach((item) => { vvv.Add(item); });
                        foreach (Vozac korisnik in vvv)
                        {
                            vozaclist.Remove(korisnik);
                            List<Voznja> v1 = new List<Voznja>();
                            korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                            foreach (Voznja voznja in v1)
                            {
                                if (voznja.Musterija == staro)
                                {
                                    korisnik.Voznje.Remove(voznja);
                                    voznja.Dispecer = novo;
                                    korisnik.Voznje.Add(voznja);
                                }
                            }
                            vozaclist.Add(korisnik);
                        }
                        WriteToXMl(Enums.Uloga.Vozac);

                        List<Korisnik> vvvv = new List<Korisnik>();
                        korisniklist.ForEach((item) => { vvvv.Add(item); });
                        foreach (Korisnik korisnik in vvvv)
                        {
                            korisniklist.Remove(korisnik);
                            List<Voznja> v1 = new List<Voznja>();
                            korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                            foreach (Voznja voznja in v1)
                            {
                                if (voznja.Musterija == staro)
                                {
                                    korisnik.Voznje.Remove(voznja);
                                    voznja.Dispecer = novo;
                                    korisnik.Voznje.Add(voznja);
                                }
                            }
                            korisniklist.Add(korisnik);
                        }
                        WriteToXMl(Enums.Uloga.Musterija);

                        List<Korisnik> vvvvv = new List<Korisnik>();
                        adminlist.ForEach((item) => { vvvvv.Add(item); });
                        foreach (Korisnik korisnik in vvvvv)
                        {
                            adminlist.Remove(korisnik);
                            List<Voznja> v1 = new List<Voznja>();
                            korisnik.Voznje.ForEach((item) => { v1.Add(item); });
                            foreach (Voznja voznja in v1)
                            {
                                if (voznja.Musterija == staro)
                                {
                                    korisnik.Voznje.Remove(voznja);
                                    voznja.Dispecer = novo;
                                    korisnik.Voznje.Add(voznja);
                                }
                            }
                            adminlist.Add(korisnik);
                        }
                        WriteToXMl(Enums.Uloga.Dispecer);
                        //--------------------------------------------------
                        v = korisniklist.Find(i => i.Id == ID);
                        korisniklist.Remove(v);
                        k.Voznje = v.Voznje;
                        korisniklist.Add(k);
                        WriteToXMl(Enums.Uloga.Musterija);
                        return Ok(k);
                    }
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
                    WriteToXMUlogovani();
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
                Double r = Math.Sqrt(Math.Pow((xx - x),2) + Math.Pow((yy - y),2));
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

            List<List<string>> list = new List<List<string>>();
            foreach (string p in dictionary.Keys)
            {
                List<string> k = new List<string>();
                k.Add(p);
                k.Add(dictionary[p]);
                list.Add(k);
            }

            return Ok(list);
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

        [HttpPut,Route("api/main/musterijaizmenavoznje")]
        public IHttpActionResult IzmenaVoznjeMusterija([FromBody]IzmenaVoznje izmenaVoznje)
        {
            Korisnik k = korisniklist.Find(i => i.Id == izmenaVoznje.Id);
            korisniklist.Remove(k);
            Voznja voznja = k.Voznje.Find(i => i.Id == izmenaVoznje.IdVoznje);
            k.Voznje.Remove(voznja);

            if (izmenaVoznje.Lokacija != null)
            {
                voznja.PolaznaTacka = izmenaVoznje.Lokacija;
                voznja.TipVozila = izmenaVoznje.TipVozila;
                if (izmenaVoznje.Opis != null)
                {
                    Voznja v = slobodneVoznje.Find(u => u.Id == izmenaVoznje.IdVoznje);
                    slobodneVoznje.Remove(v);
                    voznja.StatusVoznje = Enums.StatusVoznje.Otkazana;
                    voznja.Komentar = new Komentar();
                    voznja.Komentar.Opis = izmenaVoznje.Opis;
                    voznja.Komentar.OcenaVoznje = izmenaVoznje.Ocena.ToString();
                    voznja.Komentar.Korisnik = k.KorisnickoIme;
                    voznja.Komentar.DatumObjave = DateTime.Now;
                    voznja.Komentar.Voznja = voznja.Id;
                    WriteToXMSlobodneVoznje();
                }
                Voznja v1 = sveVoznje.Find(u => u.Id == izmenaVoznje.IdVoznje);
                sveVoznje.Remove(v1);
                sveVoznje.Add(voznja);
            }
            else
            {
                Vozac vozac = vozaclist.Find(i => i.KorisnickoIme == voznja.Vozac);
                vozaclist.Remove(vozac);
                vozac.Voznje.Remove(voznja);
                Korisnik dispecer = new Korisnik();
                if (voznja.Dispecer != null)
                {
                    dispecer = adminlist.Find(i => i.KorisnickoIme == voznja.Dispecer);
                    adminlist.Remove(dispecer);
                    dispecer.Voznje.Remove(voznja);
                }
                voznja.Komentar = new Komentar();
                voznja.Komentar.Opis = izmenaVoznje.Opis;
                voznja.Komentar.OcenaVoznje = izmenaVoznje.Ocena.ToString();
                voznja.Komentar.Korisnik = k.KorisnickoIme;
                voznja.Komentar.DatumObjave = DateTime.Now;
                voznja.Komentar.Voznja = voznja.Id;

                if (voznja.Dispecer != null)
                {
                    dispecer.Voznje.Add(voznja);
                    adminlist.Add(dispecer);
                    WriteToXMl(Enums.Uloga.Dispecer);
                }

                vozac.Voznje.Add(voznja);
                vozaclist.Add(vozac);
                WriteToXMl(Enums.Uloga.Vozac);
                Voznja v1 = sveVoznje.Find(u => u.Id == izmenaVoznje.IdVoznje);
                sveVoznje.Remove(v1);
                sveVoznje.Add(voznja);
            }

                k.Voznje.Add(voznja);
                korisniklist.Add(k);
                WriteToXMSveVoznje();
                WriteToXMl(Enums.Uloga.Musterija);
                return Ok(k);
        }

        [HttpPut, Route("api/main/dispecerizmenavoznje")]
        public IHttpActionResult IzmenaVoznjeDispecer([FromBody]IzmenaVoznje izmenaVoznje)
        {
            return Ok();
        }

        [HttpPut, Route("api/main/vozacizmenavoznje")]
        public IHttpActionResult IzmenaVoznjeVozac([FromBody]ZavrsitiVoznju izmenaVoznje)
        {
            Vozac vozac = vozaclist.Find(i => i.Id == izmenaVoznje.IdVozaca);
            Voznja voznja = vozac.Voznje.Find(i => i.Id == izmenaVoznje.IdVoznje);
            Korisnik musterija = new Korisnik();
            Korisnik admin = new Korisnik();

            if (voznja.Musterija != null)
            {
                musterija = korisniklist.Find(i => i.KorisnickoIme == voznja.Musterija);
                korisniklist.Remove(musterija);
                musterija.Voznje.Remove(voznja);
            }

            if (voznja.Dispecer != null)
            {
                admin = adminlist.Find(i => i.KorisnickoIme == voznja.Dispecer);
                adminlist.Remove(admin);
                admin.Voznje.Remove(voznja);
            }

            vozaclist.Remove(vozac);
            vozac.Voznje.Remove(voznja);
            sveVoznje.Remove(voznja);

            if (izmenaVoznje.Opis == null)
            {
                voznja.StatusVoznje = Enums.StatusVoznje.Uspesna;
                voznja.Odrediste = izmenaVoznje.Destinacija;
                voznja.Iznos = izmenaVoznje.Cena;
            }
            else
            {
                voznja.StatusVoznje = Enums.StatusVoznje.Neuspesna;
                voznja.Komentar = new Komentar();
                voznja.Komentar.Opis = izmenaVoznje.Opis;
                voznja.Komentar.DatumObjave = DateTime.Now;
                voznja.Komentar.Voznja = izmenaVoznje.IdVoznje;
                voznja.Komentar.OcenaVoznje = "0";
            }

            if (voznja.Musterija != null)
            {
                musterija.Voznje.Add(voznja);
                korisniklist.Add(musterija);
                WriteToXMl(Enums.Uloga.Musterija);
            }

            if (voznja.Dispecer != null)
            {
                admin.Voznje.Add(voznja);
                adminlist.Add(admin);
                WriteToXMl(Enums.Uloga.Dispecer);
            }

            sveVoznje.Add(voznja);
            WriteToXMSveVoznje();

            vozac.Voznje.Add(voznja);
            vozaclist.Add(vozac);
            WriteToXMl(Enums.Uloga.Vozac);
            return Ok(vozac);
        }

        [HttpPut, Route("api/main/preuzmivoznju")]
        public IHttpActionResult VozacPreuzmiVoznju([FromBody]IzmenaVoznje izmenaVoznje)
        {
            Voznja voznja = slobodneVoznje.Find(i => i.Id == izmenaVoznje.IdVoznje);
            slobodneVoznje.Remove(voznja);
            WriteToXMSlobodneVoznje();
            Korisnik korisnik = korisniklist.Find(i => i.KorisnickoIme == voznja.Musterija);
            korisnik.Voznje.Remove(voznja);
            sveVoznje.Remove(voznja);

            Vozac vozac = vozaclist.Find(i => i.Id == izmenaVoznje.Id);
            vozaclist.Remove(vozac);

            voznja.StatusVoznje = Enums.StatusVoznje.Prihvacena;
            voznja.Vozac = vozac.KorisnickoIme;

            korisnik.Voznje.Add(voznja);
            WriteToXMl(Enums.Uloga.Musterija);
            vozac.Voznje.Add(voznja);
            WriteToXMl(Enums.Uloga.Vozac);

            sveVoznje.Add(voznja);
            WriteToXMSveVoznje();

            return Ok(vozac);
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

        private void WriteToXMUlogovani()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Ulogovani.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<List<string>>));

            using (TextWriter writer = new StreamWriter(path))
            {
                List<List<string>> ll = new List<List<string>>();
                foreach (string l in ulokovani.Keys)
                {
                    List<string> o = new List<string>() {l,ulokovani[l] };
                    ll.Add(o);
                }
                serializer.Serialize(writer, ll);
            }
        }

        private void ReadFromXMLUlogovani()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Ulogovani.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<List<string>>));

            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                    List<List<string>> l = (List<List<string>>)serializer.Deserialize(reader);
                    foreach (List<string> ll in l)
                    {
                        ulokovani.Add(ll[0],ll[1]);
                    }
                }
            }
            catch { }
        }
    }
}
