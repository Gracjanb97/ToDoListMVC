$(document).ready(function () {
    $.ajax({
        url: '/ToDoes/BuildToDoTable',
        success: function (result) {
            $('#ToDoTable').html(result);
        }
    });
});