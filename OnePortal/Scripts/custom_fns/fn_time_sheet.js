function add_row(tb) {
    var $tableBody = $('#'+tb).find("tbody"),
        $trLast = $tableBody.find("tr:last"),
        $trNew = $trLast.clone();

    var row_count = $trLast.index() + 1;
    $trNew.find('input,select').each(function () {
        var name = this.name;
        this.name = name.substring(0, name.length - 1) + (row_count + 1);


        var id = this.id;
        this.id = id.substring(0, name.length - 1) + (row_count + 1);
          
    });

    //reset the project selection
    $trNew.find('select').each(function () {
      
        this.selectedIndex = 0;

    });
    //disable activity dropdown
    $trNew.find('select')[1].disabled = true;

    $trNew.find('button').each(function () {
        
        if ($(this).hasClass("eligible")){
            $(this).removeClass("btn-warning");
        }
    
  
      });
   
 
    $trNew.find('input').each(function () {

        $(this).val("");
        $(this).attr("disabled","disabled");
    });

    $trLast.after($trNew);


}

function replace_name() {


    $(".pr").each(function (index) {
        $(this).attr("name", $(this).attr("class").split(' ')[2]);
    });

    $(".act").each(function (index) {
        $(this).attr("name", $(this).attr("class").split(' ')[2]);
    });
    
}

function clear_row(row) {
    $(row).parent().parent().find("input").each(function () {
        $(this).val("");
    });

    $trNew.find('button').each(function () {
        
        if ($(this).has("eligible")){
            $(this).removeClass("btn-warning");
        }
    
  
      });

    $(row).parent().parent().find("select").each(function () {
        this.selectedIndex = 0;
    });
    $(row).parent().parent().find("select")[1].disabled = true;
}

function delete_row(row,tb) {
    var rows=$("#" + tb + " tbody tr").length;
    if (rows > 1) {
        $(row).parent().parent().remove();
    } else {
        clear_row(row);
    }
}

function enable_activities(select) {
    var value = $(select).val();
    if (value == "") {
        $(select).closest('td').next().find('select')[0].selectedIndex = 0;
        $(select).closest('td').next().find('select')[0].disabled = true;
        //call on changes
        $(select).closest('td').next().find('select')[0].trigger("change");
        var controls = $(select).parent().parent().find(".eligible");
        for (var i = 0; i < controls.length; i++) {
            $(controls[i]).attr("disabled", "disabled");
            $(controls[i]).val("");
        }
    }
    else {
        var obj = $(select).closest('td').next().find('select')[0];
        //$(select).closest('td').next().find('select')[0].name = "";

        var control = $("[name='" + $(select).closest('td').next().find('select')[0].name + "']");
        control.empty();
        for (var i = 0; i < categories.length; i++) {
            if (categories[i].project_id == value) {
                control.append("<option value='" + categories[i].activity_id + "'>" + categories[i].name + "</option>");
                //$(select).closest('td').next().find('select')[0].append("<option value='" + categories[i].activity_id + "'>" + categories[i].name + "</option");
            }
        }


        $(select).closest('td').next().find('select')[0].disabled = false;




        if ($(select).val() == "") {
            var controls = $(select).parent().parent().find(".eligible");
            for (var i = 0; i < controls.length; i++) {
                $(controls[i]).attr("disabled", "disabled");
                $(controls[i]).val("");
            }
        }
        else {
            var controls = $(select).parent().parent().find(".eligible");
            for (var i = 0; i < controls.length; i++) {
                $(controls[i]).removeAttr("disabled");

            }
        }
    }
}


function enable_inputs(row) {
    if ($(row).val() == "") {
        var controls = $(row).parent().parent().find(".eligible");
        for (var i = 0; i < controls.length; i++) {
            $(controls[i]).attr("disabled", "disabled");
            $(controls[i]).val("");
        }
    }
    else {
        var controls = $(row).parent().parent().find(".eligible");
        for (var i = 0; i < controls.length; i++) {
            $(controls[i]).removeAttr("disabled");
            
        }
    }

}

function active_comment(id) {
    var ctrl = $(id).next();
    //active_comment_control = $(id).next()[0].id;

    var nxt=$(id).next()[0];

    active_comment_control = $(id).next()[0].id;

    $("#comment").val($("#" + active_comment_control).val());
   // $(id).addClass("btn-warning");
}

function add_comment() {
    var ctrl = $("#" + active_comment_control).prev();
    if ($("#comment").val() != "") {
        $("#" + active_comment_control).prev().addClass("btn-warning");
    }
    else {
        $("#" + active_comment_control).prev().removeClass("btn-warning");
    }
    $("#" + active_comment_control).val($("#comment").val());
    $('#myModal').modal('toggle');
}

function validate_entry() {
    var pattern = new RegExp("^023-[0-9]{0,7}$");

    $("input:text").change(function (e) {
        if (!pattern.test($(this).val())) {
            return false
        }
    })
}


function preview_timesheet_on_calendar(timesheet_id) {

    if (timesheet_id < 1)
        return;
    $("#id").val(timesheet_id);
    $("#loader").show();
    $.ajax({
        url: baseUrl() + 'TimesheetApi/GetTimesheetItems/'+timesheet_id,
        method: "GET",
        success: function (data) {


            var events = [];
            if (data.length > 0) {
                var timesheet_id = data[0].timesheet_id;
                for (var i = 0; i < data.length; i++) {
                    var item_title = data[i].item_title + ": " + data[i].duration + "hrs ";
                    if (data[i].comment != "") {
                        item_title += " " + data[i].comment + "";
                    }
                    events.push({ title: item_title, start: data[i].date });
                }

                get_timesheet_logs(timesheet_id);
                $('#calendar').fullCalendar('destroy');
                $('#calendar').fullCalendar({
                    header: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'month,agendaWeek,agendaDay'
                    },
                   
                    events: events,
                    weekMode:"variable"
                });
                $('#calendar').fullCalendar('gotoDate', data[0].date);

            }
            
            $("#loader").hide();
            

        }
    });
}


function get_timesheet_logs(timesheet_id) {
    $.ajax({
        url: baseUrl() + 'TimesheetApi/GetTimesheetLogs/'+timesheet_id,
        method: "GET",
        success: function (data) {
            $("#tb_logs tbody").empty();
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var row = "<tr>";
                    row += "<td>" + data[i].action + "</td>";
                    row += "<td>" + data[i].emp_name + "</td>";
                    row += "<td>" + data[i].date_time+ "</td>";
                    if (data[i].comment != null)
                        row += "<td>" + data[i].comment + "</td>";
                    else
                        row += "<td></td>";
                    row += "</tr>";

                    $("#tb_logs tbody").append(row);
                }
            }
        }
    })
}

function current_selections(year, month) {
    $("#year").val(year);
    $("#month").val(month);
}

function getCategories() {
    $.ajax({
        url: baseUrl() + 'TimesheetApi/GetCategories',
        method: "GET",
        success: function (data) {
            categories = [];
            for (var i = 0; i < data.length; i++) {
                categories.push({ project_id: data[i].project_id, activity_id: data[i].activity_id, name: data[i].name });
            }
        }
    });
}