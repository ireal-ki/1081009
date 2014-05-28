angular.module('api', [])

.factory('apiCaller', ['$http', function ($http) {
    var API_108_1009 = 'http://1081009.tourismthailand.org/api/';
    var data = {};
    return {
        apiList: getApiList(),
        call: call,
        category: getCategory,
        callJsonPost: callJsonPost
    };

    function call(args, params, callback, _self) {
        var argument = '';
        if (angular.isDefined(params)) {
            for (var i in params) {
                argument += '&' + params[i];
            }
        }

        var url = API_108_1009 + args + '?callback=JSON_CALLBACK' + argument;
        return $http.jsonp(url)
	    		.success(function (data) {
	    		    callback(data, _self);
	    		}
	    );
    }

    function callJsonPost(args, params, callback, _self) {
        var argument = '';
        if (angular.isDefined(params)) {
            for (var i in params) {
                if (argument == '') {
                    argument += params[i];
                } else {
                    argument += '&' + params[i];
                }
            }
        }

        var url = API_108_1009 + args + '?' + argument;
        return $http.post(url)
                .success(function (data) {
                    callback(data, _self);
                }
        );
    }

    function getCategory(catId) {
        var list;
        switch (catId) {
            case 1:
                list = [
                    {
                        group: 'travel%3A1',
                        title: 'ทั้งหมด',
                        type: -1
                    },
                    {
                        group: 'travel%3A1',
                        title: 'หมู่บ้าน, ชุมชน',
                        type: 1
                    },
                    {
                        group: 'travel%3A1',
                        title: 'ตลาด, ตลาดน้ำ',
                        type: 2
                    }
                ];
                break;
            case 2:
                list = [
                    {
                        group: 'travel%3A2',
                        title: 'ทั้งหมด',
                        type: -1
                    },
                    {
                        group: 'travel%3A2',
                        title: 'โครงการหลวง,  โครงการวิจัยและพัฒนา',
                        type: 3
                    },
                    {
                        group: 'travel%3A2',
                        title: 'สวนพฤกษศาสตร์',
                        type: 4
                    },
                    {
                        group: 'travel%3A2',
                        title: 'โรงเรียน, ฝึกอบรม',
                        type: 5
                    },
                    {
                        group: 'travel%3A2',
                        title: 'ท่องเที่ยวเชิงเกษตร',
                        type: 6
                    },
                    {
                        group: 'travel%3A2',
                        title: 'พิพิธภัณฑ์วิทยาศาสตร์',
                        type: 7
                    },
                    {
                        group: 'travel%3A2',
                        title: 'เที่ยวธรรมชาติ',
                        type: 8
                    }
                ];
                break;
            case 3:
                list = [
                    {
                        group: 'travel%3A3',
                        title: 'ทั้งหมด',
                        type: -1
                    },
                    {
                        group: 'travel%3A3',
                        title: 'แม่น้ำ, คลอง',
                        type: 9
                    },
                    {
                        group: 'travel%3A3',
                        title: 'ภูเขา, ป่า, น้ำตก, ถ้ำ',
                        type: 10
                    },
                    {
                        group: 'travel%3A3',
                        title: 'เขื่อน, ฝาย, ทะเลสาป',
                        type: 11
                    },
                    {
                        group: 'travel%3A3',
                        title: 'ทะเล, หาดทราย, อ่าว, เกาะ',
                        type: 12
                    },
                    {
                        group: 'travel%3A3',
                        title: 'ทุ่งดอกไม้',
                        type: 13
                    },
                    {
                        group: 'travel%3A3',
                        title: 'น้ำพุร้อน',
                        type: 14
                    }
                ];
                break;
            case 4:
                list = [
                    {
                        group: 'travel%3A4',
                        title: 'ทั้งหมด',
                        type: -1
                    },
                    {
                        group: 'travel%3A4',
                        title: 'โรงภาพยนตร์',
                        type: 15
                    },
                    {
                        group: 'travel%3A4',
                        title: 'สวนสัตว์, ฟาร์มปศุสัตว์',
                        type: 16
                    },
                    {
                        group: 'travel%3A4',
                        title: 'สวนสาธารณะ',
                        type: 17
                    },
                    {
                        group: 'travel%3A4',
                        title: 'สวนสนุก',
                        type: 18
                    },
                    {
                        group: 'travel%3A4',
                        title: 'สนามกีฬา / สถานออกกำลัง',
                        type: 19
                    },
                    {
                        group: 'travel%3A4',
                        title: 'ค่ายทหาร',
                        type: 20
                    },
                    {
                        group: 'travel%3A4',
                        title: 'ห้างสรรพสินค้า',
                        type: 27
                    }
                ];
                break;
            case 5:
                list = [
                    {
                        group: 'travel%3A5',
                        title: 'ทั้งหมด',
                        type: -1
                    },
                    {
                        group: 'travel%3A5',
                        title: 'โบราณสถาน',
                        type: 21
                    },
                    {
                        group: 'travel%3A5',
                        title: 'วัด, โบสถ์, สุเหร่า, อุทยานประวัติศาสตร์',
                        type: 22
                    },
                    {
                        group: 'travel%3A5',
                        title: 'หอศิลป์, สถาปัตยกรรม',
                        type: 23
                    },
                    {
                        group: 'travel%3A5',
                        title: 'อนุสาวรีย์',
                        type: 24
                    },
                    {
                        group: 'travel%3A5',
                        title: 'พระราชวัง',
                        type: 25
                    },
                    {
                        group: 'travel%3A5',
                        title: 'พิพิธภัณฑ์ประวัติศาสตร์และวัฒนธรรม',
                        type: 26
                    }
                ];
                break;
            case 6:
                break;
            case 7:
                list = [
                    {
                        group: 'activity%3A11',
                        title: 'ทั้งหมด',
                        type: -1
                    },
                    {
                        group: 'activity%3A11',
                        title: 'เรือคานู และคายัค',
                        type: 14
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ยิงปืน',
                        type: 15
                    },
                    {
                        group: 'activity%3A11',
                        title: 'เจ็ตสกี',
                        type: 16
                    },
                    {
                        group: 'activity%3A11',
                        title: 'เดินชมอุทยาน',
                        type: 17
                    },
                    {
                        group: 'activity%3A11',
                        title: 'เดินป่า',
                        type: 18
                    },
                    {
                        group: 'activity%3A11',
                        title: 'วินเซิร์ฟ',
                        type: 19
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ศึกษาธรรมชาติ',
                        type: 20
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ล่องแพ, ล่องแก่ง',
                        type: 21
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ขี่จักรยาน',
                        type: 22
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ดูดาว',
                        type: 23
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ดูนก',
                        type: 24
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ดูผีเสื้อ',
                        type: 25
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ตกปลา',
                        type: 26
                    },
                    {
                        group: 'activity%3A11',
                        title: 'ปีนเขา โรยตัว โดดหอ',
                        type: 27
                    },
                    {
                        group: 'activity%3A11',
                        title: 'รถออฟโรด',
                        type: 30
                    },
                    {
                        group: 'activity%3A11',
                        title: 'สำรวจถ้ำ',
                        type: 31
                    },
                    {
                        group: 'activity%3A11',
                        title: 'นั่งช้าง, ขี่ม้า, ขี่ล่อ',
                        type: 32
                    }
                ];
                break;
            case 8:
                list = [
                    {
                        group: 'activity%3A12',
                        title: 'ทั้งหมด',
                        type: -1
                    },
                    {
                        group: 'activity%3A12',
                        title: 'วัฒนธรรม',
                        type: 1
                    },
                    {
                        group: 'activity%3A12',
                        title: 'ทำเนียบสปา',
                        type: 2
                    },
                    {
                        group: 'activity%3A12',
                        title: 'โรงเรียนสอนทำอาหาร',
                        type: 3
                    },
                    {
                        group: 'activity%3A12',
                        title: 'ล่องแม่น้ำลำคลอง',
                        type: 4
                    },
                    {
                        group: 'activity%3A12',
                        title: 'ล่องเรือทานอาหาร',
                        type: 5
                    },
                    {
                        group: 'activity%3A12',
                        title: 'สนามกอล์ฟ',
                        type: 6
                    },
                    {
                        group: 'activity%3A12',
                        title: 'กิจกรรมการทำสมาธิ',
                        type: 7
                    },
                    {
                        group: 'activity%3A12',
                        title: 'กิจกรรมดำน้ำ',
                        type: 8
                    },
                    {
                        group: 'activity%3A12',
                        title: 'ท่องเที่ยวเชิงนิเวศ',
                        type: 9
                    },
                    {
                        group: 'activity%3A12',
                        title: 'มวยไทย',
                        type: 10
                    },
                    {
                        group: 'activity%3A12',
                        title: 'ชมทิวทัศน์ทางอากาศ, พารามอเตอร์',
                        type: 12
                    },
                    {
                        group: 'activity%3A12',
                        title: 'ท่องเที่ยวเขตทหาร',
                        type: 28
                    },
                    {
                        group: 'activity%3A12',
                        title: 'ร้านอาหาร',
                        type: 29
                    },
                    {
                        group: 'activity%3A12',
                        title: 'ร้านกาแฟ',
                        type: 36
                    }
                ];
                break;
            case 9:
                list = [
                    {
                        group: 'activity%3A13',
                        title: 'ทั้งหมด',
                        type: -1
                    },
                    {
                        group: 'activity%3A13',
                        title: 'โรงแรม',
                        type: 33
                    },
                    {
                        group: 'activity%3A13',
                        title: 'รีสอร์ท',
                        type: 34
                    },
                    {
                        group: 'activity%3A13',
                        title: 'โฮมสเตย์',
                        type: 35
                    }
                ];
                break;
        }
        return list;
    }

    function getApiList() {
        var apiList = {
            top10: {
                url: '/feed/top10',
                params: {
                    username: '',
                    offset: 0,
                    limit: 20
                }
            },
            latest_trip: {
                url: '/feed/latest_trip',
                params: {
                    device_id: '',
                    username: '',
                    keyword: '',
                    group: -1,
                    type: -1,
                    offset: 0,
                    limit: 20
                }
            },
            mostlike_trip: {
                url: '/feed/mostlike_trip',
                params: {
                    device_id: '',
                    username: '',
                    keyword: '',
                    group: -1,
                    type: -1,
                    offset: 0,
                    limit: 20
                }
            },
            topview_trip: {
                url: '/feed/topview_trip',
                params: {
                    device_id: '',
                    username: '',
                    keyword: '',
                    group: -1,
                    type: -1,
                    offset: 0,
                    limit: 20
                }
            },
            province_trip: {
                url: '/feed/province_trip',
                params: {
                    device_id: '',
                    username: '',
                    keyword: '',
                    group: -1,
                    type: -1,
                    offset: 0,
                    limit: 20
                }
            },
            travel_trip: {
                url: '/feed/travel_trip',
                params: {
                    device_id: '',
                    username: '',
                    keyword: '',
                    offset: 0,
                    limit: 20
                }
            },
            hotel_trip: {
                url: '/feed/hotel_trip',
                params: {
                    device_id: '',
                    username: '',
                    keyword: '',
                    offset: 0,
                    limit: 20
                }
            },
            restaurant_trip: {
                url: '/feed/restaurant_trip',
                params: {
                    device_id: '',
                    username: '',
                    keyword: '',
                    offset: 0,
                    limit: 20
                }
            },
            video: {
                url: '/feed/video',
                params: {
                    device_id: '',
                    username: '',
                    keyword: '',
                    offset: 0,
                    limit: 20
                }
            },
            search_nearby_amphur: {
                url: '/feed/search_nearby_amphur',
                params: {
                    device_id: '',
                    lat: 13.759394,
                    lng: 100.49469,
                    min_distance: 0,
                    max_distance: 300,
                    offset: 0,
                    limit: 20
                }
            },
            register: {
                url: '/register',
                params: {
                    username: '',
                    password: '',
                    first_name: '',
                    last_name: '',
                    fbid: '',
                    email: ''
                }
            },
            auth: {
                url: '/auth',
                params: {
                    username: '',
                    password: ''
                }
            },
            authFb: {
                url: '/auth/facebook',
                params: {
                    fbid: ''
                }
            },
            like: {
                url: '/feed/like',
                params: {
                    device_id: '',
                    username: '',
                    content_id: '',
                    content_type: 'trip'
                }
            },
            favorite_add: {
                url: '/feed/favorite',
                params: {
                    username: '',
                    content_id: '',
                    favorite: 1
                }
            },
            favorite_view: {
                url: '/feed/favorite_view',
                params: {
                    device_id: '',
                    username: '',
                    offset: 0,
                    limit: 20
                }
            },
            my_article_view: {
                url: '/feed/my_article_view',
                params: {
                    device_id: '',
                    username: ''
                }
            },
            my_article_add: {
                url: '/feed/my_article_add',
                params: {
                    username: '',
                    article_url: ''
                }
            },
            save_view: {
                url: '/feed/save_view',
                params: {
                    device_id: '',
                    username: '',
                    content_id: '',
                    content_type: 'trip'
                }
            },
            forgot: {
                url: '/forgot',
                params: {
                    email: ''
                }
            }
        };

        return apiList;
    }
}]);