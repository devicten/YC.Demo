
if (!Number.prototype.currency) {
    Number.prototype.currency = function () {
        var parts = this.toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    };
}

if (!Date.prototype.ToYMD) {
    Date.prototype.ToYMD = function () {
        var mm = this.getMonth() + 1; // getMonth() is zero-based
        var dd = this.getDate();
        return (this.getFullYear() - 1911) + '年' + (mm > 9 ? '' : '0') + mm + '月' + (dd > 9 ? '' : '0') + dd + '日';
    };
}
if (!Date.prototype.ToYMDs) {
    Date.prototype.ToYMDs = function () {
        var mm = this.getMonth() + 1; // getMonth() is zero-based
        var dd = this.getDate();
        return (this.getFullYear()) + '-' + (mm > 9 ? '' : '0') + mm + '-' + (dd > 9 ? '' : '0') + dd;
    };
}

if (!Date.prototype.ToRecord) {
    Date.prototype.ToRecord = function () {
        var MM = this.getMonth() + 1; // getMonth() is zero-based
        var dd = this.getDate();
        var HH = this.getHours();
        var mm = this.getMinutes();
        var ss = this.getSeconds();
        return (this.getFullYear() - 1911) + '年' + (MM > 9 ? '' : '0') + MM + '月' + (dd > 9 ? '' : '0') + dd + '日' + ' ' + HH + '時' + mm + '分' + ss + '秒';
    }
}

if (!Date.prototype.DiffMinutes) {
    Date.prototype.DiffMinutes = function (PastTime) {
        var diffMs = (this - PastTime);
        return Math.round(((diffMs % 86400000) % 3600000) / 60000);
    }
}

if (!Array.prototype.GroupBy)
{
    Array.prototype.GroupBy = function (key) {
        return this.reduce((acc, obj) => {
            const property = obj[key];
            acc[property] = acc[property] || [];
            acc[property].push(obj);
            return acc;
        }, {});
    }
}

if (!String.prototype.FSOICDate) {
    String.prototype.FSOICDate = function () {
        var raw = this.toString();
        return raw.substr(0, 4) + '/' + raw.substr(4, 2) + '/' + raw.substr(6, 2);
    }
}

if (!String.prototype.FSOICYM) {
    String.prototype.FSOICYM = function () {
        var raw = this.toString();
        return raw.substr(0, 4) + '-' + raw.substr(4, 2);
    }
}

if (!String.prototype.FFloat) {
    String.prototype.FFloat = function () {
        var raw = this.toString();
        if (isNaN(Number(raw)) === true) {
            //'1,300.00'
            if (raw.indexOf('.00') > 0) {
                raw = raw.replace('.00', '');
            }
            //'1,300.50'
            else if (raw.indexOf('.') > 0) {
                raw = raw.substr(0, raw.length - 1);
            }
        } else {
            //'300.00'
            //'300.50'
            if (raw.indexOf('.') > 0) {
                raw = raw.substr(0, raw.length - 1);
            }
        }
        return raw;
    }
}

function SortByKey(_arr) {
    return Object.keys(_arr).sort().reduce(
        (obj, key) => {
            obj[key] = _arr[key];
            return obj;
        },
        {}
    );
}

if (!String.prototype.Pad) {
    String.prototype.Pad = function(n, width, z) {
        z = z || '0';
        n = n + '';
        return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
    }
}

if (SOIC === undefined) {
    var SOIC = { PAGE: {} }
}


SOIC.PAGE =
{
    DEBUG: 1,
    STATE: -1,
    DATA: -1,
    PANEL: {
        Main: {
            Show: function () {
                $("#PANEL_LOADING").hide();
                $("#PANEL_ERROR").hide();
                $("#PANEL_MAIN").show();
            },
            UpdateClass: function (newclass) {
                $("#PANEL_MAIN").removeAttr('class');
                $("#PANEL_MAIN").attr('class', newclass);
            }
        },
        Loading: {
            Show: function () {
                $("#PANEL_LOADING").show();
                $("#PANEL_ERROR").hide();
                $("#PANEL_MAIN").hide();
            },
        },
        Error: {
            Show: function (title, msg) {
                $("#PANEL_ERROR .err-title").html(title);
                $("#PANEL_ERROR .err-msg").html(msg);
                $("#PANEL_LOADING").hide();
                $("#PANEL_ERROR").show();
                $("#PANEL_MAIN").hide();
            }
        }
    },
    EVENT: {}
}
// jquery extend function
$.extend(
    {
        redirectPost: function (location, args) {
            var form = '';
            $.each(args, function (key, value) {
                value = value.split('"').join('\"')
                form += '<input type="hidden" name="' + key + '" value="' + value + '">';
            });
            $('<form action="' + location + '" method="POST" enctype="application/x-www-form-urlencoded">' + form + '</form>').appendTo($(document.body)).submit();
        }
    });
$(document).ajaxComplete(function (event, xhr, settings) {
    console.log('ajaxComplete:  START');
    console.log(xhr.responseJSON);
    if (xhr.responseJSON.code === 201)
        top.frames.lower_Top.SOIC.PAGE.A006.ClearSessionTimerAndShowLogin();
    else if (xhr.responseJSON.code === 200)
        top.frames.lower_Top.SOIC.PAGE.A006.ReflashSessionTimer(xhr.responseJSON.ValidTime);
    console.log('ajaxComplete:  END');
});

$(function () {
    SOIC.PAGE.PANEL.Main.Show();
    SOIC.PAGE.INIT();
});
