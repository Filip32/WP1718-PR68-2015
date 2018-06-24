var menu = "<table><tr><td name=\"menu\" id=\"login\">Uloguj se</td><td>|</td>";
menu += "<td name=\"menu\" id=\"register\">Registracija</td></tr></table>";

var textlog = "<div class=\"Centar\" ><div id=\"error1\" class=\"Red\"></div>";
textlog += "Korisničko ime: <br /> <input type=\"text\" id=\"username\" /> <br />";
textlog += "Lozinka: <br /> <input type=\"password\" id=\"password\" /> <br />";
textlog += "<input type=\"submit\" id=\"log_in\" value=\"Uloguj se\" /></div >";

var t1 = "<td name=\"menu\" id=\"home\">Početna strana</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"profil\">Profil</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"adddriver\">Dodaj vozača</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"adddrive\">Dodaj vožnju</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"dodrive\">Blokiranje</td><td>|</td>";
t1 += "<td name=\"menu\" id=\"logout\">Izloguj se</td>";

var t2 = "<td name=\"menu\" id=\"home\">Početna strana</td><td>|</td>";
t2 += "<td name=\"menu\" id=\"profil\">Profil</td><td>|</td>";
t2 += "<td name=\"menu\" id=\"adddrive\">Dodaj vožnju</td><td>|</td>";
t2 += "<td name=\"menu\" id=\"logout\">Izloguj se</td>";

var t3 = "<td name=\"menu\" id=\"home\">Početna strana</td><td>|</td>";
t3 += "<td name=\"menu\" id=\"profil\">Profil</td><td>|</td>";
t3 += "<td name=\"menu\" id=\"logout\">Izloguj se</td>";

var textreg = "<div class=\"Centar\"><div id= \"error1\" class=\"Red\" ></div >";
textreg += "Korisničko ime: <br /> <input type=\"text\" id=\"username\" /> <br />";
textreg += "Lozinka: <br /> <input type=\"password\" id=\"password\" /> <br />";
textreg += "Ime:<br /> <input type=\"text\" id=\"name\" /> <br />";
textreg += "Prezime:<br /> <input type=\"text\" id=\"lastname\" /> <br />";
textreg += "Pol: <br /> <select id=\"pol\"><option value=\"Musko\">Muško</option><option value=\"Zensko\">Žensko</option></select><br />";
textreg += "JMBG:<br /> <input type=\"text\" id=\"jmbg\" /> <br />";
textreg += "Kontakt telefon:<br /> <input type=\"text\" id=\"tel\" /> <br />";
textreg += "Email:<br /> <input type=\"text\" id=\"email\" /> <br />";
textreg += "<input type=\"submit\" id=\"register_in\" value=\"Registracija\" /> </div >";

var pocetnastr = "Pretraga po statusu:<br />";
pocetnastr += "<select id=\"status_v\"><option value=\"Kreirana\">Kreirana</option><option value=\"Formirana\">Formirana</option>";
pocetnastr += "<option value=\"Obradjena\">Obrađena</option><option value=\"Prihvacena\">Prihvaćena</option>";
pocetnastr += "<option value=\"Otkazana\">Otkazana</option><option value=\"Neuspesna\">Neuspešna</option>";
pocetnastr += "<option value=\"Uspesna\">Uspešna</option></select>";
pocetnastr += "&nbsp;&nbsp;<input type=\"submit\" id=\"po_statusu\" value=\"Pretraži\" /><br />";
pocetnastr += "<br />";
pocetnastr += "<input type=\"submit\" id=\"po_datumu\" value=\"Sortiranje po datumu\" />&nbsp;&nbsp;";
pocetnastr += "<input type=\"submit\" id=\"po_oceni\" value=\"Sortiranje po oceni\" /><br />";
pocetnastr += "<br />";
pocetnastr += "Pretraga po datumu:<br />";
pocetnastr += "Od:&nbsp; <input type=\"datetime-local\" id=\"oddatum\">&nbsp;&nbsp;&nbsp; Do:&nbsp;<input type=\"datetime-local\" id=\"dodatum\">&nbsp;&nbsp;<input type=\"submit\" id=\"po_datumu_oddo\" value=\"Pretraži\" /><br />";
pocetnastr += "<br />";
pocetnastr += "Pretraga po oceni:<br />";
pocetnastr += "Od: &nbsp;<select id=\"ocenaod\"><option value=\"0\">0</option><option value=\"1\">1</option><option value=\"2\">2</option><option value=\"3\">3</option><option value=\"4\">4</option><option value=\"5\">5</option><select>&nbsp;&nbsp;&nbsp; Do:&nbsp;<select id=\"ocenado\"><option value=\"0\">0</option><option value=\"1\">1</option><option value=\"2\">2</option><option value=\"3\">3</option><option value=\"4\">4</option><option value=\"5\">5</option><select>&nbsp;&nbsp;<input type=\"submit\" id=\"po_oceni_oddo\" value=\"Pretraži\" /><br />";
pocetnastr += "<br />";
pocetnastr += "Pretraga po ceni:<br />";
pocetnastr += "Od:&nbsp; <input type=\"text\" id=\"odcena\">&nbsp;&nbsp;&nbsp; Do:&nbsp;<input type=\"text\" id=\"docena\">&nbsp;&nbsp;<input type=\"submit\" id=\"po_ceni_oddo\" value=\"Pretraži\" /><br />";

var map = "<div id=\"map\" style=\"height:400px; text-align: center; width:50%\" class=\"map\"></div><script>";
map += "var xx; var yy; var ulica_broj; var grad;";
map += "function reverseGeocode(coords) {";
map += "fetch('http://nominatim.openstreetmap.org/reverse?format=json&lon=' + coords[0] + '&lat=' + coords[1])";
map += ".then(function(response) {return response.json();}).then(function(json) {var add = json.address;console.log(add);";
map += "ulica_broj = add.road; if(add.house_number != null){ulica_broj += \" \"+add.house_number;}grad = add.city + \" \" + add.postcode; $(\"#kord\").html(\"[\" + ulica_broj +\" , \"+ grad + \"]\");});}";
map += "var map = new ol.Map({layers: [new ol.layer.Tile({";
map += "source: new ol.source.OSM()})],target: 'map',view: new ol.View({";
map += "center: [2209717.3810248757,5660306.884676355],zoom: 19})});";
map += "map.on('dblclick', function (evt) {var coord = ol.proj.toLonLat(evt.coordinate); reverseGeocode(coord);";
map += " xx = coord[0]; yy = coord[1];});";
map += "<\/script>";


var addDriver = "<div><div id= \"error1\" class=\"Red\" ></div ><p style=\"font-weight:bold; font-size:x-large\">Dodaj vozača<\p>";
addDriver += "<table><tr><td>Korisničko ime:</td><td> <input type=\"text\" id=\"username\" /></td></tr>";
addDriver += "<tr><td>Lozinka: </td><td> <input type=\"password\" id=\"password\" /> </td></tr>";
addDriver += "<tr><td>Ime:</td><td> <input type=\"text\" id=\"name\" /> </td></tr>";
addDriver += "<tr><td>Prezime:</td><td> <input type=\"text\" id=\"lastname\" /> </td></tr>";
addDriver += "<tr><td>Pol:</td><td> <select id=\"pol\"><option value=\"Musko\">Muško</option><option value=\"Zensko\">Žensko</option></select></td></tr>";
addDriver += "<tr><td>JMBG:</td><td> <input type=\"text\" id=\"jmbg\" /> </td></tr>";
addDriver += "<tr><td>Kontakt telefon:</td><td> <input type=\"text\" id=\"tel\" /> </td></tr>";
addDriver += "<tr><td>Email:</td><td> <input type=\"text\" id=\"email\" /> </td></tr>";
addDriver += "<tr><td>Tip automobila: </td><td> <select id=\"cartipe\"><option value=\"Putnicko_Vozilo\">Putničko vozilo</option><option value=\"Kombi_Vozilo\">Kombi vozilo</option></select></td></tr>";
addDriver += "<tr><td>Godište automobila: </td><td> <input type=\"text\" id=\"godiste\" /> </td></tr>";
addDriver += "<tr><td>Broj tablica:</td><td><input type=\"text\" id=\"tablice\" /> <br /></td></tr>";
addDriver += "<tr><td>Lokacija:<br /></td><td><p id=\"kord\"></p></td></tr></table>";

addDriver += map;

addDriver += "<br /><br /><input type=\"submit\" id=\"add_driver_button\" value=\"Dodaj Vozača\" /></div>";
