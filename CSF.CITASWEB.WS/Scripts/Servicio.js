var _urlBase,
    _urlBaseControlador;
var $_alert, $_loading;
window.onload = function () {
    _urlBase = fnGetUrlBase('hdf_SE_UrlBase');
    var controller = document.getElementById('hdf_SE_Controller').value;
    _urlBaseControlador = _urlBase + controller;
    $_alert = Alert('_alert');
    $_alert.create();
    $_loading = Loading('_loading');
    $_loading.create();
    document.onkeydown = function (e) {
        e = e || event;
        var keyCode = ('which' in e) ? e.which : e.keyCode;
        if (e.altKey && String.fromCharCode(keyCode) === 'P') {
            var hdfToken = document.getElementById('hdf_SE_Token');
            var txtToken = document.getElementById('txtToken');
            txtToken.value = hdfToken.value;
        }
    };
};


var fnGetUrlBase = function (idUrlBase) {
    var txtUrlBase = document.getElementById(idUrlBase);
    var urlBase = location.protocol + "//" + location.host + (txtUrlBase ? txtUrlBase.value : '');
    return urlBase;
};
    var Alert = function (controlName) {
        var ctrl = document.getElementById(controlName);
        var create = function () {
            if (ctrl == undefined) {
                var div = document.createElement('DIV');
                div.id = controlName;
                div.className = 'jtse-alert';
                div.innerHTML = '<div class="alert move-right"><span></span></div>';
                document.body.appendChild(div);
                ctrl = document.getElementById(controlName);
            }
        };
        var show = function (message, type, time) {
            if (ctrl != undefined && message != undefined) {
                var msgType = '';
                switch (type) {
                    case 'S':
                        msgType = 'alert move-right success';
                        break;
                    case 'I':
                        msgType = 'alert move-right info';
                        break;
                    case 'D':
                        msgType = 'alert move-right default';
                        break;
                    case 'E':
                        msgType = 'alert move-right error';
                        break;
                    case 'W':
                    default:
                        msgType = 'alert move-right warning';
                        break;
                }
                ctrl.children[0].className = msgType;
                ctrl.children[0].innerHTML = message;
                ctrl.className = 'jtse-alert anix';
                setTimeout(function () {
                    ctrl.className = 'jtse-alert';
                }, (time != undefined ? time * 1000 : 3000));
            }
        };
        return {
            create: function () {
                create();
            },
            show: function (message, type, time) {
                show(message, type, time);
            }
        };
};

var Loading = function (controlName) {
    var ctrl = document.getElementById(controlName);
    var create = function () {
        if (ctrl == undefined) {
            var div = document.createElement('DIV');
            div.id = controlName;
            div.className = 'jtse-loading hide';
            var c = [];
            c.push('<div class="circle"></div>');
            c.push('<div class="circle-small"></div>');
            c.push('<div class="circle-big"></div>');
            c.push('<div class="circle-inner-inner"></div>');
            c.push('<div class="circle-inner"></div>');
            c.push('<div id="');
            c.push(controlName);
            c.push('_Texto" class="loading-text"></div>');
            div.innerHTML = c.join('');
            document.body.appendChild(div);
            ctrl = document.getElementById(controlName);
        }
    };
    var active = function (message) {
        if (ctrl != undefined) {
            ctrl.classList.remove('hide');
            document.body.style.cursor = 'none';
            document.body.style.pointerEvents = 'none';
            document.getElementById(controlName + '_Texto').innerHTML = message || '';
        }
    };
    var inactive = function () {
        if (ctrl != undefined) {
            ctrl.classList.add('hide');
            document.body.setAttribute('style', '');
            document.getElementById(controlName + '_Texto').innerHTML = '';
        }
    };
    return {
        create: function () {
            create();
        },
        active: function () {
            active();
        },
        inactive: function () {
            inactive();
        }
    };
};
