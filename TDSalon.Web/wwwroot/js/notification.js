"use strict";

    var isNotificationLoaded = false;
    var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub")
        .build();

var brojPitanja = 0;
var notifikacijeKupac = 0;
var notifikacijeIdsKupac = [];
    connection.on("novaNotifikacija", (sadrzajId, sadrzaj, tipNotifikacije, notifikacijaId) => {
        console.log("Hello from nova notifikacija metode");
        if (tipNotifikacije === "PitanjeOdgovor") {
            brojPitanja++;
            var span = document.getElementById("pitanjaSpan");
            span.innerHTML = brojPitanja;
            span.removeAttribute("hidden");
            var div = document.getElementById("pitanjaNotifikacije");

            var notifikacija = document.createElement("a");
            notifikacija.className = "dropdown-item border-bottom";
            notifikacija.href = "/Pitanja/GetPitanje?pitanjeId=" + sadrzajId;
            notifikacija.innerHTML = sadrzaj;
            div.appendChild(notifikacija);
        }
        if (tipNotifikacije === "Odgovori") {
                if (notifikacijeIdsKupac.indexOf(notifikacijaId) === -1) {
                    notifikacijeKupac++;
                    var span = document.getElementById("notifikacijeSpan");
                    span.innerHTML = notifikacijeKupac;
                    span.removeAttribute("hidden");
                    var div = document.getElementById("notifikacijeDiv");
                    
                    var notifikacija = document.createElement("a");
                    notifikacija.className = "dropdown-item border-bottom";
                    notifikacija.href = "/Pitanja/PitanjaByKorisnik";
                    notifikacija.innerHTML = sadrzaj;
                    div.appendChild(notifikacija);
                    notifikacijeIdsKupac.push(notifikacijaId);
                }       
        }
        if (tipNotifikacije === "NarudzbeKupac") {
            if (notifikacijeIdsKupac.indexOf(notifikacijaId) === -1) {
                notifikacijeKupac++;
                var span = document.getElementById("notifikacijeSpan");
                span.innerHTML = notifikacijeKupac;
                span.removeAttribute("hidden");
                var div = document.getElementById("notifikacijeDiv");

                var notifikacija = document.createElement("a");
                notifikacija.className = "dropdown-item border-bottom";
                notifikacija.href = "/Pitanja/PitanjaByKorisnik";
                notifikacija.innerHTML = sadrzaj;
                div.appendChild(notifikacija);
                notifikacijeIdsKupac.push(notifikacijaId)
            }       
        }

    });
//async function start() {
//    try {
//        await connection.start().done(function () {
//            console.log("SignalR Connected.");
//            LoadNotification();
//        });

//    } catch (err) {
//        console.log(err);
//        setTimeout(start, 5000);
//    }
//};
connection.start().then(() => {
    //try some stuff here :)
    console.log("connection started");
    LoadNotification();
    LoadNotificationForKupac();
})
    .catch(function (err) {
        //failed to connect
        return console.error(err.toString());
    });
connection.onclose(async () => {
    console.log("signalR Disconnected.");
    await start();
});

async function LoadNotification() {
    $.ajax({
        url: "/Notifikacije/GetZaposlenikNotifikacije",
        type: 'GET'
    });
}
async function LoadNotificationForKupac() {
    $.ajax({
        url: "/Notifikacije/GetKupacNotifikacije",
        type: 'GET'
    });
}
// Start the connection.
//start();
