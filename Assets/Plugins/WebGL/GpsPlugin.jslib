mergeInto(LibraryManager.library, {
  StartBrowserLocationWatch: function() {
    if (!navigator.geolocation) return;

    navigator.geolocation.watchPosition(
      function(pos) {
        var lat = pos.coords.latitude;
        var lon = pos.coords.longitude;
        SendMessage('GpsBridge', 'OnLocation', lat + ',' + lon);
      },
      function(err) {
        SendMessage('GpsBridge', 'OnLocationError', err.message);
      },
      { enableHighAccuracy: true, maximumAge: 5000, timeout: 30000 }
    );
  }
});
