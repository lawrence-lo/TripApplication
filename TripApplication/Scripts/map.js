var map = new ol.Map({
    target: 'map',
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        })
    ],
    view: new ol.View({
        center: ol.proj.fromLonLat([-79.39729357850779, 43.74048374533734]), //Toronto
        zoom: 1
    })
});

console.log("loaded");

// AJAX for coordinates
//goal: send a request which looks like this:
//GET: api/DestinationData/ListDestinationsInTrips/
var destinations;
var URL = "https://localhost:44399/api/DestinationData/ListDestinationsInTrips/";
var rq = new XMLHttpRequest();
rq.open("GET", URL, true);
rq.setRequestHeader("Content-Type", "application/json");
rq.onreadystatechange = function () {
    if (rq.readyState == 4 && rq.status == 200) {
        destinations = JSON.parse(this.responseText);
        //console.log(destinations);
        // add destinations to map
        // Reference: https://openstreetmap.be/en/projects/howto/openlayers.html
        for (i = 0; i < destinations.length; i++) {
            var layer = new ol.layer.Vector({
                source: new ol.source.Vector({
                    features: [
                        new ol.Feature({
                            geometry: new ol.geom.Point(ol.proj.fromLonLat([destinations[i]["DestinationLongitude"], destinations[i]["DestinationLatitude"]]))
                        })
                    ]
                })
            });
            map.addLayer(layer);
        }
    }
}
// Sending the request
rq.send();