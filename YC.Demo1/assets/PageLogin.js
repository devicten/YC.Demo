if (typeof SOIC !== 'object')
    throw new Error('Require SOIC library.');
if (typeof SOIC.PAGE !== 'object')
    throw new Error('Require SOIC library.');



SOIC.PAGE.EVENT.BTNLogin_OnClick = function () {
    var userName = $('#data_account').val();
    var password = $('#data_password').val();

    if (SOIC.PAGE.STATE === 1) {
        return;
    }
    SOIC.PAGE.STATE = 1;
    SOIC.PAGE.PANEL.Loading.Show();
    fetch(_API_LOGIN_, {
        method: 'POST',
        body: JSON.stringify({ userName, password }), 
        headers: {
            'Accept': 'application/json; charset=utf-8',
            'Content-Type': 'application/json;charset=UTF-8'
        }
    })
        .then(res => res.json())
        .then(response => {
            SOIC.PAGE.STATE = 0;
            console.log(response);
            SOIC.PAGE.PANEL.Main.Show();
            $.redirectPost(_URL_LoginWebApp_, { token: response.result.token });
        })
        .catch(error => {
            SOIC.PAGE.STATE = 0;
            SOIC.PAGE.PANEL.Error.Show('Action Login Error', error);
        });
    return false;
}

SOIC.PAGE.INIT = function () {
    $("body").removeAttr('class');
    $("body").attr('class', 'd-flex align-items-center py-4 bg-body-tertiary');
    SOIC.PAGE.PANEL.Main.UpdateClass('form-signin w-100 m-auto');
}