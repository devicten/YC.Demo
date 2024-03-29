﻿if (typeof SOIC !== 'object')
    throw new Error('Require SOIC library.');
if (typeof SOIC.PAGE !== 'object')
    throw new Error('Require SOIC library.');


SOIC.PAGE.EVENT.BTNModalCreate = function (obj) {
    SOIC.PAGE.STATE = 2;
    $("#MD_TITLE").text("Create New Sales");

    $("#ModalData .modal-body-1").show();
    $("#ModalData .modal-body-2").hide();

    $('#ddlStor').prop('disabled', false);
    $('#txtOrdNum').attr('disabled', false);
    $("#ddlPubInfo > option:selected").prop("selected", false);
    $("#ddlStor > option:selected").prop("selected", false);
    $("#txtOrdNum").val('');
    $("#txtOrdDate").val((new Date()).ToYMDs());
    $("#txtQty").val(1);
    $("#txtPayTerms").val('');
    
    $('#btnModalSubmit').text('Execute Create');
    $('#ModalData').modal('show');
    return false;
}

SOIC.PAGE.EVENT.BTNModalModify = function (obj) {
    SOIC.PAGE.STATE = 3;
    $("#MD_TITLE").text("Modify Sales");

    $("#ModalData .modal-body-1").show();
    $("#ModalData .modal-body-2").hide();

    var k = $(obj).attr('data-k');
    var row = SOIC.PAGE.DATA.listSales[k];

    $('#ddlStor').prop('disabled', 'disabled');
    $('#txtOrdNum').attr('disabled', 'disabled');
    $(`#ddlPubInfo > option[value="${row.pub_id}"]`).prop("selected", true);
    $(`#ddlStor > option[value="${row.stor_id}"]`).prop("selected", true);
    $("#txtOrdNum").val(row.ord_num);
    $("#txtOrdDate").val(row.ord_date);
    $("#txtQty").val(row.qty);
    $("#txtPayTerms").val(row.payterms);

    $('#btnModalSubmit').text('Execute Modify');
    $('#ModalData').modal('show');
    return false;
}

SOIC.PAGE.EVENT.BTNModalDelete = function (obj) {
    SOIC.PAGE.STATE = 4;
    $("#MD_TITLE").text("Delete Sales");
    $("#ModalData .modal-body-1").hide();
    $("#ModalData .modal-body-2").show();

    var k = $(obj).attr('data-k');
    var row = SOIC.PAGE.DATA.listSales[k];

    $("#ModalData .modal-message").text(`Are you sure to delete the order no. ${row.ord_num} ?`);

    $('#btnModalSubmit').text('Execute Delete');
    $('#ModalData').modal('show');
    return false;
}

SOIC.PAGE.EVENT.BTNModalSubmit = function (obj) {
    $('#ModalData').modal('hide');
    if (SOIC.PAGE.STATE === 2)
        return SOIC.PAGE.CreateOrModify();
    else if (SOIC.PAGE.STATE === 3)
        return SOIC.PAGE.CreateOrModify();
    else if (SOIC.PAGE.STATE === 4)
        return SOIC.PAGE.Delete();
    else
        return false;
}

SOIC.PAGE.Delete = function () {
    if (SOIC.PAGE.STATE === 1) {
        return;
    }
    SOIC.PAGE.STATE = 1;
    SOIC.PAGE.PANEL.Loading.Show();

    var deldata = {
        stor_id: $("#ddlStor > option:selected").val(),
        ord_num: $("#txtOrdNum").val()
    }

    fetch(_API_SALES_, {
        method: 'DELETE',
        body: JSON.stringify(deldata),
        headers: {
            'Accept': 'application/json; charset=utf-8',
            'Content-Type': 'application/json;charset=UTF-8'
        }
    })
        .then(res => res.json())
        .then(response => {
            SOIC.PAGE.STATE = 0;
            if (response.code !== 200) {
                SOIC.PAGE.PANEL.Error.Show(response.message);
                return;
            }
            SOIC.PAGE.Read();
        })
        .catch(error => {
            SOIC.PAGE.STATE = 0;
            SOIC.PAGE.PANEL.Error.Show('Action Sales Delete Error', error);
        });
    return false;
}
SOIC.PAGE.CreateOrModify = function () {
    if (SOIC.PAGE.STATE === 1) {
        return;
    }
    var STATE = SOIC.PAGE.STATE;
    SOIC.PAGE.STATE = 1;
    SOIC.PAGE.PANEL.Loading.Show();

    var putdata = {
        title_id: $("#ddlPubInfo > option:selected").val(),
        stor_id: $("#ddlStor > option:selected").val(),
        ord_num: $("#txtOrdNum").val(),
        ord_date: $("#txtOrdDate").val(),
        qty: $("#txtQty").val(),
        payterms: $("#txtPayTerms").val()
    }

    fetch(_API_SALES_, {
        method: STATE === 2 ? 'PUT' : 'PATCH',
        body: JSON.stringify(putdata),
        headers: {
            'Accept': 'application/json; charset=utf-8',
            'Content-Type': 'application/json;charset=UTF-8'
        }
    })
        .then(res => res.json())
        .then(response => {
            SOIC.PAGE.STATE = 0;
            if (response.code !== 200) {
                SOIC.PAGE.PANEL.Error.Show(response.message);
                return;
            }
            SOIC.PAGE.Read();
        })
        .catch(error => {
            SOIC.PAGE.STATE = 0;
            SOIC.PAGE.PANEL.Error.Show('Action Sales CreateOrModify Error', error);
        });
    return false;
}
SOIC.PAGE.Read = function () {
    if (SOIC.PAGE.STATE !== 0) {
        return;
    }
    SOIC.PAGE.STATE = 1;
    SOIC.PAGE.PANEL.Loading.Show();

    fetch(_API_SALES_, {
        method: 'POST',
        //body: JSON.stringify({ userName, password }),
        headers: {
            'Accept': 'application/json; charset=utf-8',
            'Content-Type': 'application/json;charset=UTF-8'
        }
    })
    .then(res => res.json())
    .then(response => {
        SOIC.PAGE.STATE = 0;
        if (response.code !== 200) {
            SOIC.PAGE.PANEL.Error.Show(response.message);
            return;
        }
        SOIC.PAGE.DATA = response.result;

        var html = '';
        html += `<tr>
            <th scope="col">#</th>
            <th scope="col">Logo</th>
            <th scope="col">Store</th>
            <th scope="col">Address</th>
            <th scope="col">No.</th>
            <th scope="col">Date</th>
            <th scope="col">Qty.</th>
            <th scope="col">Terms</th>
            <td scope="col"> ** </td>
        </tr>`;
        $("#tbData > thead").html(html);
        html = '';
        for (var k = 0; k < SOIC.PAGE.DATA.listSales.length; k++) {
            var row = SOIC.PAGE.DATA.listSales[k];
            html += `<tr>
            <th scope="row">${k+1}</th>
            <td><img src="data:image/png;base64, ${row.logo}" alt="${row.pr_info}" /></td>
            <td>${row.stor_name}</td>
            <td>${row.stor_address}</td>
            <td>${row.ord_num}</td>
            <td>${row.ord_date}</td>
            <td>${row.qty}</td>
            <td>${row.payterms}</td>
            <td>
                <div class="btn btn-warning" data-k="${k}" onclick="return SOIC.PAGE.EVENT.BTNModalModify(this);">Edit</div>
                <div class="btn btn-danger"  data-k="${k}" onclick="return SOIC.PAGE.EVENT.BTNModalDelete(this);">Delete</div>
            </td>
        </tr>`;
        }
        
        $("#tbData > tbody").html(html);


        html = '';
        for (var k = 0; k < SOIC.PAGE.DATA.listTitles.length; k++) {
            var row = SOIC.PAGE.DATA.listTitles[k];
            html += `<option value="${row.title_id}">${row.title}</option>`;
        }
        $("#ddlPubInfo").html(html);

        html = '';
        for (var k = 0; k < SOIC.PAGE.DATA.listStores.length; k++) {
            var row = SOIC.PAGE.DATA.listStores[k];
            html += `<option value="${row.stor_id}">${row.stor_name}</option>`;
        }
        $("#ddlStor").html(html);

        SOIC.PAGE.PANEL.Main.Show();
    })
    .catch(error => {
        SOIC.PAGE.STATE = 0;
        SOIC.PAGE.PANEL.Error.Show('Action Sales Error', error);
    });
    return false;
}

SOIC.PAGE.INIT = function () {
    $("body").removeAttr('class');
    SOIC.PAGE.PANEL.Main.UpdateClass('');
    SOIC.PAGE.STATE = 0;
    SOIC.PAGE.Read();
}