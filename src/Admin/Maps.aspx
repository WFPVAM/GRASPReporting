<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Maps.aspx.cs" Inherits="Admin_Maps" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        body, html { height: 100%; min-height: 100%; }

        #map-canvas { margin-left: 15px; height: 700px; }
    </style>

    <script type="text/javascript"
        src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAOxs7GZioFnyECcG_2Uj1RXa1pSWSCQQ8&sensor=true&language=en">
    </script>

    <script type="text/javascript">

        var neighborhoods = [ <%= geoCoordinates %>];

        var markers = [];
        var iterator = 0;

        var map;

        function initialize() {
            var mapOptions = {
                zoom: 3,
                minZoom: 3
            };

            map = new google.maps.Map(document.getElementById("map-canvas"),
                mapOptions);

            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var pos = new google.maps.LatLng(position.coords.latitude,
                                                     position.coords.longitude);

                    var infowindow = new google.maps.InfoWindow({
                        map: map,
                        position: pos
                    });

                    map.setCenter(pos);
                }, function () {
                    handleNoGeolocation(true);
                });
            } else {
                // Browser doesn't support Geolocation
                handleNoGeolocation(false);
            }
            var elems = neighborhoods.length;
            for (var i = 0; i < 50; i++) {
                setTimeout(function () {
                    addMarker();
                }, i * 500);
            }
        }

        function addMarker() {
            var place = neighborhoods[iterator];
            if (place[0] != 0 && place[1] != 0) {
                var myLatLng = new google.maps.LatLng(place[0], place[1]);
                var URL = "ViewForm.aspx?id=" + place[2];
                var marker = new google.maps.Marker({
                    position: myLatLng,
                    map: map,
                    draggable: false,
                    //animation: google.maps.Animation.DROP,
                    url: URL
                });
                iterator++;

                markers.push(marker);
                google.maps.event.addListener(marker, 'click', function () {
                    window.open(this.url, '_blank');
                });
            }
        }



        function handleNoGeolocation(errorFlag) {
            if (errorFlag) {
                var content = 'Error: The Geolocation service failed.';
            } else {
                var content = 'Error: Your browser doesn\'t support geolocation.';
            }

            var options = {
                map: map,
                position: new google.maps.LatLng(60, 105),
                content: content
            };

            var infowindow = new google.maps.InfoWindow(options);
            map.setCenter(options.position);
        }
        google.maps.event.addDomListener(window, 'load', initialize);
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <div class="row">
        <div class="col-lg-12">
            <h1>Maps</h1>
            <ol class="breadcrumb">
                <li><i class="fa fa-home"></i><a href="Home_Page.aspx">Home</a></li>
                <li class="current"><i class="fa fa-check-square-o"></i>Maps</li>
            </ol>
        </div>
    </div>
    <div class="row">
        <div id="map-canvas" class="col-lg-11"></div>
    </div>
</asp:Content>

