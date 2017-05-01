// Wait for auth to load
function load(code) {
  var callback = firebase.auth().onAuthStateChanged(function() {
    // De-subscribe after one call
    callback();
    code();
  });
}

angular.module('starter.controllers', [])

.controller(
  'AppCtrl',
  function($scope, $rootScope, $ionicModal, $timeout, $ionicPopup) {
    // With the new view caching in Ionic, Controllers are only
    // called
    // when they are recreated or on app start, instead of every
    // page change.
    // To listen for when this page is active (for example, to
    // refresh data),
    // listen for the $ionicView.enter event:
    //$scope.$on('$ionicView.enter', function(e) {
    //});

    var vm = $rootScope;

    // listen for server broadcasts in realtime
    var pusher = new Pusher('c367bd18dea7e8eaee28', {encrypted: true});
    var channel = pusher.subscribe('main');

    channel.bind('broadcast', function(data) {
      console.log('Broadcast received...');
      vm.refreshLocations();
    });

    vm.getLocations = function() {
      return new Promise(function(resolve, reject) {
        $.ajax({
          type: 'POST',
          url: 'https://web.ephemerant.com/api/location',
          data: JSON.stringify({type: 'get'}),
          success: function(response) {
            vm.locations = response.locations;
            vm.$apply();
            console.log(response);
            resolve(response);
          },
          error: function(error) { reject(error); },
          contentType: 'application/json'
        });
      });
    };

    vm.getTrucks = function() {
      return new Promise(function(resolve, reject) {
        $.ajax({
          type: 'POST',
          url: 'https://web.ephemerant.com/api/truck',
          data: JSON.stringify({type: 'get'}),
          success: function(response) {
            vm.trucks = response.trucks;
            Object.keys(vm.trucks).forEach(function(key) {
              var truck = vm.trucks[key];
              truck.menu = JSON.parse(truck.menu);
            });
            console.log(vm.trucks);
            vm.$apply();
            console.log(response);
            resolve(response);
          },
          error: function(error) { reject(error); },
          contentType: 'application/json'
        });
      });
    };

    vm.refreshLocations =
    function() { vm.getTrucks().then(vm.getLocations); }

    vm.refreshLocations();

    vm.getEvents = function() {
      console.log('ping1');
      // TODO: fix getEvents
      return new Promise(function(resolve, reject) {
      $.ajax({
        type: 'POST',
        url: 'https://web.ephemerant.com/api/schedule',
        data: JSON.stringify({
          type: 'get',
          data: {
            uid: 'all'
          }
        }),
        success: function (response) {
          console.log('ping2');
          var events = response.events;
          var eventlist = ["key","name","date","duration"];
          Object.keys(events).forEach(function(key) {
            var eventlist = events[key];
          });
        },
        error: function(error) { reject(error); },
        contentType: 'application/json'
        });
      });
    };

    load(function() {
      // Set our logged in state
      $scope.loggedIn = firebase.auth().currentUser !== null;
      $scope.$apply();
    });

    // Form data for the login modal
    $scope.loginData = {};

    // Create the login modal that we will use later
    $ionicModal.fromTemplateUrl('templates/login.html', {scope: $scope})
    .then(function(modal) { $scope.modal = modal; });

    // Triggered in the login modal to close it
    $scope.closeLogin = function() {
      // reset the form
      localStorage.lastEmail = $scope.loginData.email;
      $scope.loginData = {};
      // TODO: Add AJAX command to expire the broadcast
      $scope.modal.hide();
    };

    // Open the login modal
    $scope.login = function() {
      // Remember the email, because I'm tired of typing it for testing
      // :^)
      $scope.loginData.email = localStorage.lastEmail;

      $scope.modal.show();
    };

    $scope.logout = function() {
      firebase.auth().signOut().then(function() {
        $scope.loggedIn = false;
        $scope.$apply();
      });
    };

    // Perform the login action when the user submits the login
    // form
    $scope.doLogin = function() {
      $scope.loggingIn = true;

      console.log('Doing login', $scope.loginData);

      firebase.auth()
      .signInWithEmailAndPassword($scope.loginData.email,
        $scope.loginData.password)
        .then(function(result) {
          console.log(result);

          $scope.closeLogin();

          $scope.loggedIn = true;
          $scope.loggingIn = false;
          $scope.$apply();
        })
        .catch(function(error) {
          console.log(error);

          // Show the error
          $ionicPopup.alert({title: 'Error', template: error.message});

          $scope.loggingIn = false;
          $scope.$apply();
        });

        // Simulate a login delay. Remove this and replace with your
        // login
        // code if using a login system
        // $timeout(function() { $scope.closeLogin(); }, 100);
      };
    })

    .controller('socialNetworkCtrl',
    function($scope) {
      // TODO: Post this message to Facebook
      var msg = this.msg;
    })

    .controller('browseCtrl',
    function($scope, $rootScope) {
      // browse controller stub
    })

    .controller('listDetailCtrl',
    function($scope, $rootScope, $stateParams, $filter) {
      // listDetail controller stub
      var vm = $scope;
      var trucks = $rootScope.trucks;
      vm.id = $stateParams.uid;
      // var truck = $filter('filter')(trucks, {uid: vm.id});
    })

    .controller('aboutCtrl',
    function($scope, $rootScope) {
      // about Controller stub
    })

    .controller('eventListCtrl', function() {
      // calendarCtrl stub
    })

    .controller('SearchCtrl', function($scope, $rootScope, $cordovaGeolocation,
      $ionicModal) {
        var vm = $scope;

        $ionicModal.fromTemplateUrl('templates/broadcastModal.html',
        {scope: $scope})
        .then(function(modal) {
          vm.modal = modal;
          vm.modal.numberOfHours = 4;
        });

        vm.markers = [];

        vm.broadcast = function() {
          firebase.auth().currentUser.getToken(true).then(function(token) {
            // Send the data to the server
            $.ajax({
              type: 'POST',
              url: 'https://web.ephemerant.com/api/location',
              data: JSON.stringify({
                token: token,
                type: 'add',
                data: {lat: vm.lat, lon: vm.lon, hours: vm.modal.numberOfHours}
              }),
              success: function(response) {
                // Redirect
                console.log(response);
                $scope.modal.hide();
                // Refresh locations
                vm.refreshLocations();
              },
              contentType: 'application/json'
            });
          });
        };

        // Function to send a 'remove' command for a broadcast
        vm.broadcastStop = function(){
          firebase.auth().currentUser.getToken(true).then(function(token) {
            // Send the data to the server
            $.ajax({
              type: 'POST',
              url: 'https://web.ephemerant.com/api/location',
              data: JSON.stringify({
                token: token,
                type: 'remove',
                data: {lat: vm.lat, lon: vm.lon}
              }),
              success: function(response) {
                // Redirect
                console.log(response);
                $scope.modal.hide();
                // Refresh locations
                vm.refreshLocations();
              },
              contentType: 'application/json'
            });
          });
        };

        $scope.$on('$ionicView.enter', function(e) {  // refresh locations on load
          vm.refreshLocations();
        });

        // Rounds "n" to "d" decimal places
        function rounded(n, d) {
          return Math.round(n * Math.pow(10, d)) / Math.pow(10, d);
        }

        function formattedWindow(title, content) {
          return '<h4>' + title + '</h4><p>' + content + '</p>';
        }

        var posOptions = {timeout: 10000, enableHighAccuracy: false};

        $scope.$on('$ionicView.enter', function(event, data) {
          // resize, otherwise certain other views can screw with the map
          if (vm.map) google.maps.event.trigger(vm.map, 'resize');
        });

        $scope.$on("$ionicView.loaded", function(event, data) {
          // upon first being loaded, otherwise we will rely on "enter"
          $cordovaGeolocation.getCurrentPosition(posOptions)
          .then(
            function(position) {
              // position was successfully loaded
              loadMap(position, true);
            },
            function(err) {
              // unable to load position - fallback to default position
              loadMap(
                {coords: {latitude: 38.228732, longitude: -85.764771}},
                false);
              });
            });

            function loadMap(position, posLoaded) {
              vm.lat = rounded(position.coords.latitude, 7);
              vm.lon = rounded(position.coords.longitude, 7);

              var myLatlng = new google.maps.LatLng(vm.lat, vm.lon);

              var mapOptions = {
                center: myLatlng,
                zoom: 13,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                disableDefaultUI: true,
                styles: [{featureType: "poi", stylers: [{visibility: "off"}]}]
              };
              //var gm = google.maps;
              var map =
              new google.maps.Map(document.getElementById("map"), mapOptions);

              var spiderfiedColor = 'ffee22';


              // Create Spiderfier instance
              var oms = new OverlappingMarkerSpiderfier(map);

              // Global click listener for OverlappingMarkerSpiderfier
              var iw = new google.maps.InfoWindow();
              oms.addListener('click', function(marker, event) {
                iw.setContent(marker.desc);
                //iw.open(map, marker);
              });

              oms.addListener('spiderfy', function(markers) {
                iw.close();
              });

              // circular marker, can be used instead of food truck image
              // path

              var circle = {
                path: google.maps.SymbolPath.CIRCLE,
                fillColor: 'blue',
                fillOpacity: .6,
                scale: 6,
                strokeColor: 'white',
                strokeWeight: 2
              };

              // image from: http://www.flaticon.com/free-icon/van_205142

              var image = 'img/truck.png';

              if (posLoaded)
              new google.maps.Marker({
                position:
                new google.maps.LatLng(rounded(vm.lat, 6), rounded(vm.lon, 6)),
                map: map,
                icon: circle
              });

              // Update markers on location updates
              $rootScope.$watch('locations', function() {
                clearMarkers();

                if ($rootScope.locations == null) return;

                Object.keys($rootScope.locations)
                .forEach(function(key) {
                  var location = $rootScope.locations[key];

                  var marker = new google.maps.Marker({
                    position: new google.maps.LatLng(rounded(location.lat, 6),
                    rounded(location.lon, 6)),
                    map: map,
                    icon: image,
                    window: infowindow = new google.maps.InfoWindow({
                      content: formattedWindow($rootScope.trucks[key].name,
                        $rootScope.trucks[key].about)
                      })
                    });

                    vm.markers.push(marker);
                    oms.addMarker(marker);

                    marker.addListener('click', function() {
                      if (vm.lastMarker) {
                        vm.lastMarker.window.close();
                        // Allow marker toggling
                        if (vm.lastMarker == this) {
                          vm.lastMarker = undefined;
                          return;
                        }
                      }
                      this.window.open(vm.map, this);
                      vm.lastMarker = this;
                    });
                  });
                });

                function clearMarkers() {
                  vm.markers.forEach(function(marker) { marker.setMap(null); });

                  vm.markers = [];
                }

                map.addListener('click', function() {
                  if (vm.lastMarker) {
                    vm.lastMarker.window.close();
                    vm.lastMarker = undefined;
                  }
                });

                vm.map = map;
              }

            });
