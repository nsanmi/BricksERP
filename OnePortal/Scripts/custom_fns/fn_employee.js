function get_employee(id) {
    $.ajax({
        url: baseUrl() + "employeeapi/GetEmployee/" + id,
        method: "GET",
        success: function (data) {

            $("#tb_subs").hide();
            
            $("#emp_name").text(data.emp_lastname + " " + data.emp_firstname);
            $("#emp_id").text(data.employee_id);
            $("#emp_job_title").text(data.emp_job_title);
            $("#emp_birthday").text(data.emp_birthday);
            $("#emp_location").text(data.emp_location);
            $("#emp_address").text(data.emp_work_email);
            $("#emp_status").text(data.emp_status);
            $("#emp_hm_telephone").text(data.emp_hm_telephone);
            $("#emp_address").text(data.emp_address);
            $("#emp_work_email").html("<a href='mailto:"+data.emp_work_email+"'>"+data.emp_work_email+"</a>");
            $("#joined_date").html(data.joined_date);
            if (data.emp_about != null)
                $("#emp_about").text(data.emp_about);
            else
                $("#emp_about").text("");
           // $("#emp_supervisors").text(data.emp_supervisors.join());

            if (data.emp_supervisors != null) {
                var emp_sup = [];
                for (var i = 0; i < data.emp_supervisors.length; i++) {
                    emp_sup[i] = data.emp_supervisors[i].person_name;
                }

                $("#emp_supervisors").text(emp_sup.join());
            }
            $("#contact_id").val(id);
            if (data.emp_subs != null) {
                $("#subs_count").text( "("+data.emp_subs.length + ")");
                $("#tb_subs").show();
                $("#tb_subs > tbody").empty();

                for (var i = 0; i < data.emp_subs.length; i++) {
                    var row = "<tr>";
                    row += "<td>" + data.emp_subs[i].person_name + "</td>";
                    row += "<td>" + data.emp_subs[i].person_jobtitle + "</td>";
                    row += "<td>" + data.emp_subs[i].person_location + "</td>";
                    row += "</tr>";

                    $("#tb_subs > tbody").append(row);
                }
             }

            if (data != null) {
                $("#contact-1").show();
            }
        }
    })
}