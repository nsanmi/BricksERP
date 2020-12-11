$("#frmFile").submit(function () {
    $("#progress").show();
    var formData = new FormData($(this)[0]);
    

    $.ajax({
        url: baseUrl() + 'fileapi/postfile',
        type: 'POST',
        data: formData,
        async: true,
        success: function (data) {
            window.location.reload(true);
            $("#progress").hide();
        },
        cache: false,
        contentType: false,
        processData: false
    });


    return false;
});

$('#file').change(function () {
    $('#frmFile').submit();
});