function add_row(tb) {
    $('.iselect').select2("destroy");
    var $tableBody = $('#' + tb).find("tbody"),
        $trLast = $tableBody.find("tr:last"),
        $trNew = $trLast.clone();

 

    $trNew.find('input').each(function () {
       
        $(this).val("");

    });

    $trNew.find('span').each(function () {

        $(this).text("0");

    });


    //$trNew.find('select').each(function () {

    //    $(this).select2("destroy");

    //});


    $trNew.find('textarea').each(function () {

        $(this).val("");

    });

    $trLast.after($trNew);

    $('.iselect').select2({
        placeholder: 'Search for an item',

        ajax: {
            url: function (params) {
                return baseUrl() + "utils/GetItems/" + params.term;
            },
            dataType: 'json',
            quietMillis: 100,

            processResults: function (data) {
                return {
                    results: $.map(data, function (obj) {
                        return { id: obj.id, text: obj.name };
                    })
                };
            }

        }
    });

    $(".txtMult input").keyup(multInputs);
}

function add_row_n(tb) {
    $('.iselect').select2("destroy");
    var $tableBody = $('#' + tb).find("tbody"),
        $trLast = $tableBody.find("tr:last"),
        $trNew = $trLast.clone();

    $trNew.find('input').each(function () {

        $(this).val("");

    });

    $trNew.find('span').each(function () {

        $(this).text("0");

    });

    //$trNew.find('select').each(function () {

    //    $(this).select2("destroy");

    //});
    
    $trNew.find('textarea').each(function () {

        $(this).val("");

    });

    $trLast.after($trNew);

    $('.iselect').select2({
        placeholder: 'Search for an item',

        ajax: {
            url: function (params) {
                return baseUrl() + "utils/GetItems/" + params.term;
            },
            dataType: 'json',
            quietMillis: 100,

            processResults: function (data) {
                return {
                    results: $.map(data, function (obj) {
                        return { id: obj.id, text: obj.name };
                    })
                };
            }

        }
    });

    $(".txtMult_n input").keyup(multInputs_n);
}

function add_row_r(tb) {
    var $tableBody = $('#' + tb).find("tbody"),
        $trLast = $tableBody.find("tr:last"),
        $trNew = $trLast.clone();

    $trNew.find('input').each(function () {

        $(this).val("");

    });

    $trNew.find('span').each(function () {

        $(this).text("0");

    });

    //$trNew.find('select').each(function () {

    //    $(this).select2("destroy");

    //});

    $trNew.find('textarea').each(function () {

        $(this).val("");

    });

    $trLast.after($trNew);

    $(".txtMult_r input").keyup(multInputs_r);
}

function delete_row(row, tb) {
    var rows = $("#" + tb + " tbody tr").length;
    if (rows > 1) {
        $(row).parent().parent().remove();
    } else {
        clear_row(row);
    }

    multInputs();
}

function delete_row_n(row, tb) {
    var rows = $("#" + tb + " tbody tr").length;
    if (rows > 1) {
        $(row).parent().parent().remove();
    } else {
        clear_row(row);
    }

    multInputs_n();
}

function delete_row_r(row, tb) {
    var rows = $("#" + tb + " tbody tr").length;
    if (rows > 1) {
        $(row).parent().parent().remove();
    } else {
        clear_row(row);
    }

    multInputs_r();
}

function clear_row(row) {
    $(row).parent().parent().find("input").each(function () {
        $(this).val("");
    });
}


function multiply(val1, control, output_control) {
    var result = val1 * $(control).val();

    $("#" + output_control).val(result);
}

function multiply(val1,val2, control, output_control) {
    var result = val1 * val2* $(control).val();

    $("#" + output_control).val(result);
}


function generate_approval_token() {
    $("form").submit(function () {
        $(this).append('<input type="hidden" name="refresh_token" value="refresh" /> ');
    });

    $("form").submit();
}