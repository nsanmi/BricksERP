function manage_duration() {

}

function toggle_duration(day_ctrl, time_ctrl,ctrl) {
    var value = $(ctrl).val();

    if (value == 1) {
        $(day_ctrl).show();
        $(time_ctrl).hide();
    } else {
        $(day_ctrl).hide();
        $(time_ctrl).show();
    }
}

function toggle_partial() {
    var value = $("#sel_partial_days").val();
    if (value == 1) {
        $("#rw_start").hide();
        $("#rw_end").hide();
        $("#rw_all").hide();
    }
    else if (value == 2) {
        $("#rw_all").show();
        $("#rw_start").hide();
        $("#rw_end").hide();
    } else if (value == 3) {
        $("#rw_all").hide();
        $("#rw_start").show();
        $("#rw_end").hide();
    } else if (value == 4) {
        $("#rw_start").hide();
        $("#rw_end").show();
        $("#rw_all").hide();
    }
    else if (value == 5) {
        $("#rw_all").hide();
        $("#rw_start").show();
        $("#rw_end").show();
    }
}