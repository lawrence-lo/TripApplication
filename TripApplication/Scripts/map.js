const baseURL = "https://localhost:44399/";

// Create a map
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


// Make an ajax call to get all destinations
$.ajax({
    url: baseURL + 'api/DestinationData/ListDestinationsRelatedToTrips/', success: function (result) {
        var featuresArray = [];
        destinations = result;
        // add destinations to map
        // Reference: https://openstreetmap.be/en/projects/howto/openlayers.html
        for (i = 0; i < destinations.length; i++) {
            featuresArray.push(
                new ol.Feature({
                    geometry: new ol.geom.Point(ol.proj.fromLonLat([destinations[i]['DestinationLongitude'], destinations[i]['DestinationLatitude']])),
                    DestinationName: destinations[i]['DestinationName'],
                    DestinationID: destinations[i]['DestinationID']
                })
            );
        }
        var layer = new ol.layer.Vector({
            source: new ol.source.Vector({
                features: featuresArray
            })
        });
        map.addLayer(layer);
    }
});


// Initialize popup
/* References:
 * https://openlayers.org/en/latest/examples/popup.html
 * https://openstreetmap.be/en/projects/howto/openlayers.html
*/
var container = document.getElementById('popup');
var content = document.getElementById('popup-content');
var closer = document.getElementById('popup-closer');

var overlay = new ol.Overlay({
    element: container,
    autoPan: true,
    autoPanAnimation: {
        duration: 250
    }
});
map.addOverlay(overlay);

closer.onclick = function () {
    overlay.setPosition(undefined);
    closer.blur();
    return false;
};


// Open popup
map.on('singleclick', function (event) {
    if (map.hasFeatureAtPixel(event.pixel) === true) {
        let feature = map.getFeaturesAtPixel(event.pixel);
        // Make an ajax call to get trips for the destination
        $.ajax({
            url: baseURL + 'api/TripData/ListTripsForDestination/' + feature[0].A.DestinationID, success: function (result) {
                let tripNumber = '';
                if (result.length > 1) {
                    tripNumber = result.length + ' trips';
                } else {
                    tripNumber = result.length + ' trip';
                }
                content.innerHTML = '<div>' + feature[0].A.DestinationName + '</div><div><a href="' + baseURL + 'Destination/Details/' + feature[0].A.DestinationID + '">' + tripNumber + '</a></div>';
                overlay.setPosition(event.coordinate);
            }
        });
    } else {
        overlay.setPosition(undefined);
        closer.blur();
    }
});