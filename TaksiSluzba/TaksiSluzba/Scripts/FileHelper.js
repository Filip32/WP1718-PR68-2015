var menu = "<table><tr><td name=\"menu\" id=\"login\">Uloguj se</td><td>|</td>";
menu += "<td name=\"menu\" id=\"register\">Registracija</td></tr></table>";

var textlog = "<div class=\"Centar\" ><div id=\"error1\" class=\"Red\"></div>";
textlog += "Korisnicko ime: <br /> <input type=\"text\" id=\"username\" /> <br />";
textlog += "Lozinka: <br /> <input type=\"password\" id=\"password\" /> <br />";
textlog += "<input type=\"submit\" id=\"log_in\" value=\"Login\" /></div >";

var t1 = "<td name=\"menu\" id=\"home\">Pocetna strana</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"profil\">Profil</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"adddriver\">Dodaj vozaca</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"adddrive\">Dodaj voznju</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"dodrive\">Blokiranje</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"logout\">Izloguj se</td>";

var t2 = "<td name=\"menu\" id=\"home\">Pocetna strana</td><td>|</td>";
t2 += "<td name=\"menu\" id=\"profil\">Profil</td><td>|</td>";
t2 += "<td name=\"menu\" id=\"adddrive\">Dodaj voznju</td><td>|</td>";
t2 += "<td name=\"menu\" id=\"logout\">Izloguj se</td>";

var t3 = "<td name=\"menu\" id=\"home\">Pocetna strana</td><td>|</td>";
t3 += "<td name=\"menu\" id=\"profil\">Profil</td><td>|</td>";
t3 += "<td name=\"menu\" id=\"changeloc\">Izmena lokacija</td><td>|</td>";
t3 += "<td name=\"menu\" id=\"logout\">Izloguj se</td>";

var textreg = "<div class=\"Centar\"><div id= \"error1\" class=\"Red\" ></div >";
textreg += "Korisnicko ime: <br /> <input type=\"text\" id=\"username\" /> <br />";
textreg += "Lozinka: <br /> <input type=\"password\" id=\"password\" /> <br />";
textreg += "Ime:<br /> <input type=\"text\" id=\"name\" /> <br />";
textreg += "Prezime:<br /> <input type=\"text\" id=\"lastname\" /> <br />";
textreg += "Pol: <br /> <select id=\"pol\"><option value=\"Musko\">Musko</option><option value=\"Zensko\">Zensko</option></select><br />";
textreg += "JMBG:<br /> <input type=\"text\" id=\"jmbg\" /> <br />";
textreg += "Kontakt telefon:<br /> <input type=\"text\" id=\"tel\" /> <br />";
textreg += "Email:<br /> <input type=\"text\" id=\"email\" /> <br />";
textreg += "<input type=\"submit\" id=\"register_in\" value=\"Registracija\" /> </div >";

var pocetnastr = "Pretraga po statusu:<br />";
pocetnastr += "<select id=\"status_v\"><option value=\"Kreirana\">Kreirana</option><option value=\"Formirana\">Formirana</option>";
pocetnastr += "<option value=\"Obradjena\">Obradjena</option><option value=\"Prihvacena\">Prihvacena</option>";
pocetnastr += "<option value=\"Otkazana\">Otkazana</option><option value=\"Neuspesna\">Neuspesna</option>";
pocetnastr += "<option value=\"Uspesna\">Uspesna</option></select>";
pocetnastr += "&nbsp;&nbsp;<input type=\"submit\" id=\"po_statusu\" value=\"Pretrazi\" /><br />";
pocetnastr += "<br />";
pocetnastr += "<input type=\"submit\" id=\"po_datumu\" value=\"Sortiranje po datumu\" />&nbsp;&nbsp;";
pocetnastr += "<input type=\"submit\" id=\"po_oceni\" value=\"Sortiranje po oceni\" /><br />";
pocetnastr += "<br />";
pocetnastr += "Pretraga po datumu(mm/dd/yyyy):<br />";
pocetnastr += "Od:&nbsp; <input type=\"text\" id=\"oddatum\">&nbsp;&nbsp;&nbsp; Do:&nbsp;<input type=\"text\" id=\"dodatum\">&nbsp;&nbsp;<input type=\"submit\" id=\"po_datumu_oddo\" value=\"Pretrazi\" /><br />";
pocetnastr += "<br />";
pocetnastr += "Pretraga po oceni:<br />";
pocetnastr += "Od: &nbsp;<input type=\"text\" id=\"odocena\">&nbsp;&nbsp;&nbsp; Do:&nbsp;<input type=\"text\" id=\"doocena\">&nbsp;&nbsp;<input type=\"submit\" id=\"po_oceni_oddo\" value=\"Pretrazi\" /><br />";
pocetnastr += "<br />";
pocetnastr += "Pretraga po ceni:<br />";
pocetnastr += "Od:&nbsp; <input type=\"text\" id=\"odcena\">&nbsp;&nbsp;&nbsp; Do:&nbsp;<input type=\"text\" id=\"docena\">&nbsp;&nbsp;<input type=\"submit\" id=\"po_ceni_oddo\" value=\"Pretrazi\" /><br />";

var addDriver = "<div><div id= \"error1\" class=\"Red\" ></div ><p style=\"font-weight:bold; font-size:x-large\">Dodaj vozaca<\p>";
addDriver += "<table><tr><td>Korisnicko ime:</td><td> <input type=\"text\" id=\"username\" /></td></tr>";
addDriver += "<tr><td>Lozinka: </td><td> <input type=\"password\" id=\"password\" /> </td></tr>";
addDriver += "<tr><td>Ime:</td><td> <input type=\"text\" id=\"name\" /> </td></tr>";
addDriver += "<tr><td>Prezime:</td><td> <input type=\"text\" id=\"lastname\" /> </td></tr>";
addDriver += "<tr><td>Pol:</td><td> <select id=\"pol\"><option value=\"Musko\">Musko</option><option value=\"Zensko\">Zensko</option></select></td></tr>";
addDriver += "<tr><td>JMBG:</td><td> <input type=\"text\" id=\"jmbg\" /> </td></tr>";
addDriver += "<tr><td>Kontakt telefon:</td><td> <input type=\"text\" id=\"tel\" /> </td></tr>";
addDriver += "<tr><td>Email:</td><td> <input type=\"text\" id=\"email\" /> </td></tr>";
addDriver += "<tr><td>Tip automobila: </td><td> <select id=\"cartipe\"><option value=\"Putnicko_Vozilo\">Putnicko vozilo</option><option value=\"Kombi_Vozilo\">Kombi vozilo</option></select></td></tr>";
addDriver += "<tr><td>Godiste automobila: </td><td> <input type=\"text\" id=\"godiste\" /> </td></tr>";
addDriver += "<tr><td>Broj tablica:</td><td><input type=\"text\" id=\"tablice\" /> <br /></td></tr>";
addDriver += "<tr><td>Lokacija:<br /></td><td><p id=\"kord\"></p></td></tr></table>";
addDriver += "<div id=\"map\" style=\"height:400px; text-align: center; width:50%\" class=\"map\"></div><script>";
addDriver += "var xx; var yy; var map = new ol.Map({layers: [new ol.layer.Tile({";
addDriver += "source: new ol.source.OSM()})],target: 'map',view: new ol.View({";
addDriver += "center: [2209717.3810248757,5660306.884676355],zoom: 19})});";
addDriver += "map.on('click', function (evt) {var coord = map.getCoordinateFromPixel(evt.pixel); coord = ol.proj.toLonLat(evt.coordinate);";
addDriver += " xx = coord[0];yy = coord[1]; $(\"#kord\").html(\"[\" + xx + \"  ,  \" + yy + \"]\<br />\");});";
addDriver += "<\/script>";
addDriver += "<br /><br /><input type=\"submit\" id=\"add_driver_button\" value=\"Dodaj Vozaca\" /></div>";
