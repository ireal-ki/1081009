var main = angular.module('wp1081009', ['ngRoute', 'api']);
main.config(function ($routeProvider) {
    $routeProvider.
      when('/', {
          controller: [],
          templateUrl: ''
      }).
      otherwise({
          redirectTo: '/'
      });
});
var scopeNg = null;

function autoResize(id) {
    var newheight;
    var newwidth;

    if (document.getElementById) {
        newheight = document.getElementById(id).contentWindow.document.body.scrollHeight;
        newwidth = document.getElementById(id).contentWindow.document.body.scrollWidth;
    }

    document.getElementById(id).height = (newheight) + "px";
    document.getElementById(id).width = (newwidth) + "px";
}

function setUUID(id) {
    if (scopeNg != null) {
        scopeNg.uuid = id;
    }
};
function restoreFeed() {
    if (scopeNg != null) {
        scopeNg.restoreFeed();
    }
}
function onBackBtnPress(page) {
    if (scopeNg != null) {
        switch (page) {
            case 'top10':
                scopeNg.getTop10();
                break;
            case 'feed':
                scopeNg.getHomeFeed();
                break;
            case 'map':
                scopeNg.getMap();
                break;
            case 'userMenu':
                scopeNg.getUserMenu();
                break;
            case 'search':
                scopeNg.getSearch();
                break;
            case 'menuList':
                scopeNg.getMenu();
                break;
            case 'webview':
                scopeNg.restoreCurrentWebView();
                break;
            case 'viewPhoto':
                scopeNg.openViewPhoto();
                break;
            case 'fav':
                scopeNg.getFav();
                break;
        }
    }
};

function onAppBarBtnStoryClick() {
    if (scopeNg != null) {
        scopeNg.getMenu();
    }
};

function onAppBarBtnMapClick() {
    if (scopeNg != null) {
        scopeNg.getMap();
    }
};

function onAppBarBtnTop10Click() {
    if (scopeNg != null) {
        scopeNg.getTop10();
    }
};

function firstTimeLaunched() {
    if (scopeNg != null) {
        scopeNg.firstTimeLaunched();
    }
};

function SetUser(user_id, username, first_name, last_name, email) {
    if (scopeNg != null) {
        scopeNg.isLoggedIn = true;
        scopeNg.username = username;
        scopeNg.userInfo = {
            'user_id': user_id,
            'username': username,
            'first_name': first_name,
            'last_name': last_name,
            'email': email
        };
    }
};

function fbidReturn(fbid) {
    if (scopeNg != null) {
        scopeNg.fbid = fbid;
        $('main').hide();
        $('main.login').show();
        callFacebookLogin(scopeNg, scopeNg.apiCaller, fbid);
    }
}

function getRecentReturn(param) {
    if (scopeNg != null) {
        var list = param.split('^');
        var itemsList = [];
        for (var i = 0; i < list.length; i++) {
            var listItemParam = list[i].split('|');
            var item = {
                title: listItemParam[1],
                id: listItemParam[2],
                url: listItemParam[3],
                isFav: listItemParam[4] == 'true',
                isLike: listItemParam[5] == 'true',
                like: listItemParam[6],
                view: listItemParam[7]
            };
            itemsList.push(item);
        }

        scopeNg.openRecentView(itemsList);
    }
}

function callMapApi(lat, long) {
    if (scopeNg != null) {
        scopeNg.coords = {};
        scopeNg.coords.latitude = lat;
        scopeNg.coords.longitude = long;
        callMapData(scopeNg, scopeNg.apiCaller);
    }
}

function appCtrl($rootScope, $scope, apiCaller, $sce) {
    $scope.toplist = [];
    $scope.cover = {};
    $scope.feedContent = [];
    $scope.coords = null;
    $scope.mapContent = null;
    $scope.mapObject = null;
    $scope.isShowHome = true;
    $scope.isShowFeed = false;
    $scope.isShowMap = false;
    $scope.isShowSearch = false;
    $scope.searchSub = [];
    $scope.isLoggedIn = false;
    $scope.username = '';
    $scope.viewPhoto = {};
    $scope.webview = {};
    $scope.uuid = '';
    $scope.userInfo = {};
    $scope.alertMsg = '';
    $scope.feedLanding = '';
    $scope.favoriteCount = 0;
    $scope.myArticleCount = 0;
    $scope.apiCaller = apiCaller;

    $scope.firstTimeLaunched = function () {
        $('main.intro').addClass('first');
        $('main.view-photo').addClass('first');
        $('main.search').addClass('first');
    };
    $scope.hideAll = function () {
        $scope.isShowHome = false;
        $scope.isShowFeed = false;
        $scope.isShowMap = false;
        $scope.isShowSearch = false;
    };
    $scope.getTop10 = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.isShowHome = true;

        if ($scope.toplist == null)
            $scope.toplist = [];

        if ($scope.cover == null)
            $scope.cover = {};

        $('main').hide();
        $('main.intro').show();

        callWindowsPhoneNotify('top10');
        callTop10($scope, apiCaller);
    };

    $scope.restoreFeed = function () {

        // log
        callWindowsPhoneNotify("log|restoreFeed");

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);

        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: false
            });
        });

        callWindowsPhoneNotify('feed');
        //callHomeFeed($scope, apiCaller);
    };

    $scope.getHomeFeed = function () {

        // log
        callWindowsPhoneNotify("log|getHomeFeed");

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);
        $scope.feedContent = [];
        $scope.feedLanding = 'โพสต์ล่าสุด';
        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: true
            });
        });

        callWindowsPhoneNotify('feed');
        callHomeFeed($scope, apiCaller);
    };
    $scope.getMap = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $('main').hide();
        $('main.map').show();
        $scope.isShowMap = true;
        $scope.mapContent = null;

        callWindowsPhoneNotify('map');
        callMap($scope, apiCaller);
    };
    $scope.getUserMenu = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.appBarVisible(false);

        $('main').hide();
        if ($scope.isLoggedIn) {
            $('main.menu').show();
        } else {
            $('main.login').show();
        }

        callWindowsPhoneNotify('userMenu');
    };
    $scope.getSearch = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $('main').hide();
        $('main.search').show();
        $scope.isShowSearch = true;

        callWindowsPhoneNotify('search');
    };
    $scope.getSearchSub = function (group, name) {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        var list = apiCaller.category(group);
        $scope.feedLanding = name;
        if (group == 6) {
            $scope.getSearchFeedTrip('activity%3A7', -1);
        } else {
            $('main').hide();
            $('main.search2').show();
            $scope.searchSub = list;
        }
    };
    $scope.getSearchFeedTrip = function (group, type, title) {

        // log
        callWindowsPhoneNotify("log|getSearchFeedTrip" + group, type, title);

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);
        $scope.feedContent = [];
        $scope.feedLanding = title;
        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: true
            });
        });

        callWindowsPhoneNotify('feed');
        callFeedSubActivityTrip($scope, apiCaller, group, type);
    };
    $scope.getMenu = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $('main').hide();
        $('main.menulist').show();

        callWindowsPhoneNotify('menuList');
    };
    $scope.getFeedTrip = function (feedName, name) {

        // log
        callWindowsPhoneNotify("log|getFeedTrip" + feedName, name);

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);
        $scope.feedContent = [];
        $scope.feedLanding = name;
        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: true
            });
        });

        callWindowsPhoneNotify('feed');
        callFeedTrip($scope, apiCaller, feedName);
    };
    $scope.regist = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $('main').hide();
        $('main.regist').show();
    };
    $scope.registNewMember = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        var name = $('#regist-name').val();
        var surname = $('#regist-surname').val();
        var email = $('#regist-email').val();
        var username = $('#regist-username').val();
        var password = $('#regist-password').val();
        var repassword = $('#regist-confirm').val();

        if (password != repassword) {
            $('#alert5').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('#alert5').hide();
                $('body').removeClass('show-overlay');
            })
        } else if (email == '') {
            $('#alert3').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('#alert3').hide();
                $('body').removeClass('show-overlay');
            })
        } else {
            var paramObj = {
                username: username,
                password: password,
                first_name: name,
                last_name: surname,
                email: encodeURIComponent(email)
            };
            callRegist($scope, apiCaller, paramObj);
        }
    };
    $scope.login = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        var username = $('#login-username').val();
        var password = $('#login-password').val();

        callLogin($scope, apiCaller, username, password);
    };
    $scope.openViewPhoto = function (list) {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        if (list != null) {
            $scope.viewPhoto = list;
        }

        $('main').hide();
        $('main.view-photo').show();

        callWindowsPhoneNotify('viewPhoto');
    };
    $scope.getFeedSearch = function (keyword) {

        // log
        callWindowsPhoneNotify("log|getFeedSearch:" + keyword);

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);

        // already cache
        var isCached = ($scope.feedContent.length > 0 && $scope.feedLanding == keyword);
        if (!isCached) 
        {
            $scope.feedContent = [];
            $scope.feedLanding = keyword;
        }

        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: false
            });
        });

        callWindowsPhoneNotify('feed');

        if (!isCached) 
            callFeedSearch($scope, apiCaller, keyword);
    };
    $scope.openWebView = function (item) {

        //$scope.hideAll();

        if (item != null) {
            $scope.webview = item;
            item.view++;
        }

        $('main').hide();
        $('main.webview').show();
        $scope.appBarVisible(false);

        callWindowsPhoneNotify("recentAdd" + "|" + item.title + '|' + item.id + "|" + item.url + "|" + item.isFav + "|" + item.isLike + "|" + item.like + "|" + item.view);
        callSaveView($scope, apiCaller, item.id);
        callWindowsPhoneNotify('webview' + "|" + item.url);
    };
    $scope.restoreCurrentWebView = function (item) {

        item = $scope.webview;

        $('main').hide();
        $('main.webview').show();
        //$scope.appBarVisible(false);

        //callWindowsPhoneNotify("recentAdd" + "|" + item.title + '|' + item.id + "|" + item.url + "|" + item.isFav + "|" + item.isLike + "|" + item.like + "|" + item.view);
        callSaveView($scope, apiCaller, item.id);
        callWindowsPhoneNotify('webview' + "|" + item.url);
    };
    $scope.openRecentView = function (itemList) {

        //callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.recentList = itemList;
        $('main').hide();
        $('main.recent').show();
    }
    $scope.trustSrc = function (src) {
        return $sce.trustAsResourceUrl(src);
    };
    $scope.callIE = function (url) {
        var param = 'ieOpen|' + url;

        callWindowsPhoneNotify(param);
    };
    $scope.setIsLike = function (item, event) {
        if (event) {
            event.stopPropagation();
        }
        if (!item.isLike) {
            item.isLike = true;
            item.like++;
            callIslike($scope, apiCaller, item.id);
        }
    };
    $scope.setIsFav = function (item) {
        if ($scope.username != '') {
            item.isFav = true;
            callIsFav($scope, apiCaller, item.id);
        } else {

            callWindowsPhoneNotify('hide_contentWebBrowser');

            $('main').hide();
            $('main.login').show();
        }
    };
    $scope.getSetting = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $('main').hide();
        $('main.setting').show();
    };
    $scope.getTerm = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $('main').hide();
        $('main.term').show();
    };
    $scope.accept = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $('main').hide();
        $('main.menu').show();
    };
    $scope.logout = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.isLoggedIn = false;
        $scope.username = '';
        $scope.userInfo = {};
        callWindowsPhoneNotify('logoutUser');
        $('#logout').show();
        $('body').addClass('show-overlay');
    };
    $scope.logoutComplete = function () {
        $scope.getTop10();
        $('#logout').hide();
        $('body').removeClass('show-overlay');
    };
    $scope.getFav = function () {

        // log
        callWindowsPhoneNotify("log|getFav");

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);
        $scope.feedContent = [];
        $scope.feedLanding = 'สถานที่โปรด';
        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: true
            });
        });
        callWindowsPhoneNotify('fav');
        callFavView($scope, apiCaller);
    };
    $scope.getMyStory = function () {

        // log
        callWindowsPhoneNotify("log|getMyStory");

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);
        $scope.feedContent = [];
        $scope.feedLanding = 'เรื่องราวท่องเที่ยวของฉัน';
        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: true
            });
        });
        callArticalView($scope, apiCaller);
    };
    $scope.addArticle = function () {
        $('#add-article').show();
        $('body').addClass('show-overlay');
        $('.close-overlay').on('click', function () {
            $('#add-article').hide();
            $('body').removeClass('show-overlay');
        })
    };
    $scope.addArticalSubmit = function () {
        var url = $('#input-url').val();
        callArticalAdd($scope, apiCaller, url);
        $('#add-article').hide();
        $('body').removeClass('show-overlay');
    };
    $scope.userSetting = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $('#setting-name').val($scope.userInfo.first_name);
        $('#setting-surname').val($scope.userInfo.last_name);
        $('#setting-email').val($scope.userInfo.email);
        $('#setting-username').val($scope.userInfo.username);

        $('main').hide();
        $('main.user-setting').show();
    };
    $scope.saveProfileChange = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        $('main').hide();
        $('main.menu').show();
    };
    $scope.searchKeyword = function () {

        // log
        callWindowsPhoneNotify("log|searchKeyword");

        callWindowsPhoneNotify('hide_contentWebBrowser');

        var group = '-1';
        var keyword = $('#searchbox').val();
        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);
        $scope.feedContent = [];
        $scope.feedLanding = keyword;
        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: true
            });
        });

        callWindowsPhoneNotify('feed');
        callSearchKeyword($scope, apiCaller, group, keyword);
    };
    $scope.searchKeyword2 = function (searchSub) {

        // log
        callWindowsPhoneNotify("log|searchKeyword2 : " + searchSub);

        var group = '-1';
        if (searchSub) {
            var item = searchSub[0];
            group = item.group;
        }
        var keyword = $('#searchbox2').val();
        $scope.hideAll();
        $scope.isShowFeed = true;
        $scope.appBarVisible(true);
        $scope.feedContent = [];
        $scope.feedLanding = keyword;
        $('main').hide();
        $('main.feed').show(function () {
            $("article.grid").gridalicious({
                gutter: 8,
                selector: 'figure',
                width: 160,
                animate: true
            });
        });

        callWindowsPhoneNotify('feed');
        callSearchKeyword($scope, apiCaller, group, keyword);
    };
    $scope.sendForgotPassword = function () {
        var email = $('#forgotPwdEmail').val();
        if (email != '') {
            $('body').removeClass('show-overlay');
            $('.wp-popup').hide();

            callForgot($scope, apiCaller, email);
        }
    };
    $scope.getHistory = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        callWindowsPhoneNotify('getRecent');
    };
    $scope.fbLogin = function () {

        callWindowsPhoneNotify('hide_contentWebBrowser');

        callWindowsPhoneNotify('fbLogin');
    };

    $scope.appBarVisible = function (newBool) {
        callWindowsPhoneNotify('AppBar|' + newBool);
    };

    $scope.$watch('coords', function (data) {
        if ($scope.mapObject != null) {
            //callMapData($scope, apiCaller);
        }
    }, true).bind(this);

    $scope.$watch('mapContent', function (dataList) {
        /*if ($scope.mapObject != null) {
            var mapPushPinList = [];
            for (var i = 0; i < dataList.length; i++) {
                var data = dataList[i];
                var lat = parseFloat(data.lat);
                var lng = parseFloat(data.lng);
                var imageMarker = new nokia.maps.map.Marker([lat, lng], {
                    icon: data.image_url,
                    anchor: new nokia.maps.util.Point(0, 0)
                });
                mapPushPinList.push(imageMarker);
            }
            $scope.mapObject.objects.addAll(mapPushPinList);
        }*/
        if (dataList != null) {
            var dataString = JSON.stringify(dataList);
            callWindowsPhoneNotify('MapData|' + dataString);
        }
    }, true).bind(this);

    scopeNg = $scope;
    callWindowsPhoneNotify('checkSaveLogin');
    callWindowsPhoneNotify('uuid');
    callWindowsPhoneNotify('top10');
    callTop10($scope, apiCaller);
};

var callMap = function ($scope, apiCaller) {

    callWindowsPhoneNotify('hide_contentWebBrowser');

    if ($scope.mapObject == null) {
        document.scope = $scope;
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.src = 'http://js.api.here.com/se/2.5.4/jsl.js?blank=true';

        script.onload = function () {
            nokia.Features.load(
				nokia.Features.getFeaturesFromMatrix(["all"]),
				function () {
				    nokia.Settings.set("app_id", "PFPhx8cGVkVGp2jRqrLc");
				    nokia.Settings.set("app_code", "cu-wfooTSmf3sCq2L48MZA");
				    nokia.Settings.set('serviceMode', 'cit');

				    document.scope.mapObject = new nokia.maps.map.Display(document.getElementById('map-canvas'), {
				        center: [52.51, 13.4],
				        zoomLevel: 10,
				        components: [new nokia.maps.map.component.Behavior()]
				    });

				    if (nokia.maps.positioning.Manager) {
				        var positioning = new nokia.maps.positioning.Manager();
				        document.scope.mapObject.addListener("displayready", function () {
				            positioning.getCurrentPosition(
					            function (position) {
					                document.scope.coords = position.coords;
					                var marker = new nokia.maps.map.StandardMarker(document.scope.coords);
					                var accuracyCircle = new nokia.maps.map.Circle(document.scope.coords, document.scope.coords.accuracy);

					                document.scope.mapObject.objects.addAll([accuracyCircle, marker]);
					                document.scope.mapObject.zoomTo(accuracyCircle.getBoundingBox(), false, "default");
					            },
					            function (error) {
					                var errorMsg = "Location could not be determined: ";
					                if (error.code == 1) errorMsg += "PERMISSION_DENIED";
					                else if (error.code == 2) errorMsg += "POSITION_UNAVAILABLE";
					                else if (error.code == 3) errorMsg += "TIMEOUT";
					                else errorMsg += "UNKNOWN_ERROR";

					                alert(errorMsg);
					            }
				        	);
				        });
				    }
				},
				function (error) {
				    alert("Map could not be loaded, an error occurred. " + error);
				},
				document,
				false
			);
        };
        document.body.appendChild(script);
    } else {

    }
};

var callMapData = function ($scope, apiCaller) {
    var apiList = apiCaller.apiList;
    var url = apiList.search_nearby_amphur.url;
    var apiParams = apiList.search_nearby_amphur.params;
    var params = [];
    apiParams.lat = $scope.coords.latitude;
    apiParams.lng = $scope.coords.longitude;

    for (var param in apiParams) {
        params.push(param + '=' + apiParams[param])
    }

    apiCaller.call(url, params, function (response, self) {
        self.mapContent = response;
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callTop10 = function ($scope, apiCaller) {

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');

    // already load cover?
    if (self.cover == null)
    {
        // not yet, will fecth top10

        //$scope.showFeed = false;
        var apiList = apiCaller.apiList;
        var url = apiList.top10.url;
        var apiParams = apiList.top10.params;
        var params = [];

        for (var param in apiParams) {
            if (param == 'username') {
                params.push(param + '=' + $scope.username);
            } else {
                params.push(param + '=' + apiParams[param]);
            }
        }

        // call api
        apiCaller.call(url, params, function (response, self) {
            for (var i = 0; i < response.data.length; i++) {
                if (response.data[i].iscover == 'true') {
                    self.cover = response.data[i];
                } else {
                    self.toplist.push(response.data[i]);
                }
            }
            callWindowsPhoneNotify('responseNavigated');
            callWindowsPhoneNotify('top10Loaded');
        }, $scope);
    } else {
        // use cache, no api call just notify then
        callWindowsPhoneNotify('responseNavigated');
        callWindowsPhoneNotify('top10Loaded');
    }
};

var callHomeFeed = function ($scope, apiCaller) {

    // log
    callWindowsPhoneNotify("log|callHomeFeed");

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.latest_trip.url;
    var apiParams = apiList.latest_trip.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else if (param == 'device_id') {
            params.push(param + '=' + $scope.uuid);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        for (var i = 0; i < response.length; i++) {
            self.feedContent.push(response[i]);
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callFavView = function ($scope, apiCaller) {

    // log
    callWindowsPhoneNotify("log|callFavView");

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.favorite_view.url;
    var apiParams = apiList.favorite_view.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        if (response.length == 0) {
            self.alertMsg = 'ไม่มีเรื่องราวท่องเที่ยวของฉัน';
        } else {
            for (var i = 0; i < response.length; i++) {
                self.feedContent.push(response[i]);
            }
        }
        if (self.alertMsg != '') {
            $('#alert6').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('#alert6').hide();
                $('body').removeClass('show-overlay');
                self.alertMsg = '';
            })
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callArticalView = function ($scope, apiCaller) {

    // log
    callWindowsPhoneNotify("log|callArticalView");

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.my_article_view.url;
    var apiParams = apiList.my_article_view.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else if (param == 'device_id') {
            params.push(param + '=' + $scope.uuid);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {

        if (response.length == 0) {
            self.alertMsg = 'ไม่มีเรื่องราวท่องเที่ยวของฉัน';
        } else {
            for (var i = 0; i < response.length; i++) {
                self.feedContent.push(response[i]);
            }
        }
        if (self.alertMsg != '') {
            $('#alert6').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('body').removeClass('show-overlay');
                $('#alert6').hide();
                self.alertMsg = '';
            })
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callArticalAdd = function ($scope, apiCaller, article_url) {

    // log
    callWindowsPhoneNotify("log|callArticalAdd");

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var apiURL = apiList.my_article_add.url;
    var apiParams = apiList.my_article_add.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else {
            params.push(param + '=' + article_url);
        }
    }

    apiCaller.call(apiURL, params, function (response, self) {

        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callFeedSearch = function ($scope, apiCaller, keyword) {

    // log
    callWindowsPhoneNotify("log|callFeedSearch");

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.latest_trip.url;
    var apiParams = apiList.latest_trip.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'keyword') {
            params.push(param + '=' + keyword)
        } else if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else if (param == 'device_id') {
            params.push(param + '=' + $scope.uuid);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        for (var i = 0; i < response.length; i++) {
            self.feedContent.push(response[i]);
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callSearchKeyword = function ($scope, apiCaller, group, keyword) {

    // log
    callWindowsPhoneNotify("log|callSearchKeyword");

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.latest_trip.url;
    var apiParams = apiList.latest_trip.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'keyword') {
            params.push(param + '=' + keyword)
        } else if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else if (param == 'device_id') {
            params.push(param + '=' + $scope.uuid);
        } else if (param == 'group') {
            params.push(param + '=' + group);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        for (var i = 0; i < response.length; i++) {
            self.feedContent.push(response[i]);
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callFeedTrip = function ($scope, apiCaller, apiName) {

    // log
    callWindowsPhoneNotify("log|callFeedTrip");

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList[apiName].url;
    var apiParams = apiList[apiName].params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else if (param == 'device_id') {
            params.push(param + '=' + $scope.uuid);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        for (var i = 0; i < response.length; i++) {
            self.feedContent.push(response[i]);
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
}

var callRegist = function ($scope, apiCaller, registParams) {

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.register.url;
    var apiParams = apiList.register.params;
    var params = [];

    for (var param in apiParams) {
        if (registParams[param] != null) {
            params.push(param + '=' + registParams[param]);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        if (response.result == 'success') {
            self.isLoggedIn = true;
            self.username = response.data.username;
            self.userInfo = response.data;
            callWindowsPhoneNotify('saveUser|' + self.userInfo.user_id + '|' + self.userInfo.username + '|' + self.userInfo.first_name + '|' + self.userInfo.last_name + '|' + self.userInfo.email);
            $('main').hide();
            $('main.menu').show();
            self.alertMsg = "ลงทะเบียนสำเร็จ"
            $('#alert6').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('#alert6').hide();
                $('body').removeClass('show-overlay');
                self.alertMsg = '';
            })
        } else if (response.result == 'exist_username') {
            self.alertMsg = "ชื่อผู้ใช้ซ้ำ"
        } else if (response.result == 'exist_email') {
            self.alertMsg = "อีเมล์ซ้ำ"
        } else if (response.error == 'invalid_request') {
            self.alertMsg = "ข้อมูลไม่ครบ"
        } else {
            self.alertMsg = "ไม่สามารถทำรายการได้"
        }
        if (self.alertMsg != '') {
            $('.wp-popup').hide();
            $('#alert6').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('#alert6').hide();
                $('body').removeClass('show-overlay');
                self.alertMsg = '';
            })
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callForgot = function ($scope, apiCaller, email) {

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.forgot.url;
    var apiParams = apiList.forgot.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'email') {
            params.push(param + '=' + email);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        if (response.result == 'success') {

        } else if (response.result == 'exist_username') {
            self.alertMsg = "ชื่อผู้ใช้ซ้ำ"
        } else if (response.result == 'exist_email') {
            self.alertMsg = "อีเมล์ซ้ำ"
        } else if (response.error == 'invalid_request') {
            self.alertMsg = "ข้อมูลไม่ครบ"
        } else {
            self.alertMsg = "ไม่สามารถทำรายการได้"
        }
        if (self.alertMsg != '') {
            $('.wp-popup').hide();
            $('#alert6').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('#alert6').hide();
                $('body').removeClass('show-overlay');
                self.alertMsg = '';
            })
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callLogin = function ($scope, apiCaller, username, password) {

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.auth.url;
    var apiParams = apiList.auth.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'username') {
            params.push(param + '=' + username);
        } else if (param == 'password') {
            params.push(param + '=' + password);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        if (response.result == 'success') {
            self.isLoggedIn = true;
            self.userInfo = response.data;
            self.username = response.data.username;
            self.favoriteCount = response.favorite_count;
            self.myArticleCount = response.my_article_count;
            callWindowsPhoneNotify('saveUser|' + self.userInfo.user_id + '|' + self.userInfo.username + '|' + self.userInfo.first_name + '|' + self.userInfo.last_name + '|' + self.userInfo.email);
            $('main').hide();
            $('main.menu').show();
        } else if (response.result == 'wrong_password') {
            self.alertMsg = 'password ไม่ถูกต้อง';
        } else if (response.result == 'user_not_found') {
            self.alertMsg = 'ไม่พบข้อมูล';
        } else {
            self.alertMsg = 'ไม่สามารถทำรายการได้';
        }
        if (self.alertMsg != '') {
            $('.wp-popup').hide();
            $('#alert6').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('#alert6').hide();
                $('body').removeClass('show-overlay');
                self.alertMsg = '';
            })
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callFeedSubActivityTrip = function ($scope, apiCaller, group, type) {

    // log
    callWindowsPhoneNotify("log|callFeedSubActivityTrip");

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.latest_trip.url;
    var apiParams = apiList.latest_trip.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'group') {
            params.push(param + '=' + group);
        } else if (param == 'type') {
            params.push(param + '=' + type);
        } else if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else if (param == 'device_id') {
            params.push(param + '=' + $scope.uuid);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        for (var i = 0; i < response.length; i++) {
            self.feedContent.push(response[i]);
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callIsFav = function ($scope, apiCaller, itemId) {
    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.favorite_add.url;
    var apiParams = apiList.favorite_add.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'content_id') {
            params.push(param + '=' + itemId);
        } else if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callIslike = function ($scope, apiCaller, itemId) {
    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.like.url;
    var apiParams = apiList.like.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'content_id') {
            params.push(param + '=' + itemId);
        } else if (param == 'device_id') {
            params.push(param + '=' + $scope.uuid);
        } else if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callSaveView = function ($scope, apiCaller, itemId) {

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.save_view.url;
    var apiParams = apiList.save_view.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'content_id') {
            params.push(param + '=' + itemId);
        } else if (param == 'device_id') {
            params.push(param + '=' + $scope.uuid);
        } else if (param == 'username') {
            params.push(param + '=' + $scope.username);
        } else {
            params.push(param + '=' + apiParams[param]);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callFacebookLogin = function ($scope, apiCaller, fbid) {

    callWindowsPhoneNotify('hide_contentWebBrowser');

    callWindowsPhoneNotify('responseNavigating');
    var apiList = apiCaller.apiList;
    var url = apiList.authFb.url;
    var apiParams = apiList.authFb.params;
    var params = [];

    for (var param in apiParams) {
        if (param == 'fbid') {
            params.push(param + '=' + fbid);
        }
    }

    apiCaller.call(url, params, function (response, self) {
        if (response.result == 'success') {
            self.isLoggedIn = true;
            self.userInfo = response.data;
            self.username = response.data.username;
            self.favoriteCount = response.favorite_count;
            self.myArticleCount = response.my_article_count;
            callWindowsPhoneNotify('saveUser|' + self.userInfo.user_id + '|' + self.userInfo.username + '|' + self.userInfo.first_name + '|' + self.userInfo.last_name + '|' + self.userInfo.email);
            $('main').hide();
            $('main.menu').show();
        } else if (response.result == 'user_not_found') {
            self.alertMsg = 'ไม่พบข้อมูล';
        } else {
            self.alertMsg = 'ไม่สามารถทำรายการได้';
        }
        if (self.alertMsg != '') {
            $('.wp-popup').hide();
            $('#alert6').show();
            $('body').addClass('show-overlay');
            $('.btnPopupOk').on('click', function () {
                $('#alert6').hide();
                $('body').removeClass('show-overlay');
                self.alertMsg = '';
            })
        }
        callWindowsPhoneNotify('responseNavigated');
    }, $scope);
};

var callWindowsPhoneNotify = function (arg) {
    if (window.external) {
        window.external.notify(arg);
    }
};

main.directive("contentGrid", function () {
    return function (scope, element, attrs) {
        scope.$watch("feedContent", function (values) {
            if (values.length > 0) {
                $("article.grid").gridalicious({
                    gutter: 8,
                    selector: 'figure',
                    width: 160,
                    animate: true
                });
            }
        }, true);
    };
});

appCtrl.$inject = ['$rootScope', '$scope', 'apiCaller', '$sce'];