﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        // user is currently locked out
                        return `
                        <div class="text-center">
                            <a class="btn btn-success text-white" onclick=ToggleLock("${data.id}") style="cursor: pointer; width: 100px">
                                <i class="fas fa-lock-open"></i> Unlock
                            </a>
                        </div>
                        `;
                    }
                    else {
                        return `
                        <div class="text-center">
                            <a class="btn btn-danger text-white" onclick=ToggleLock("${data.id}") style="cursor: pointer; width: 100px">
                                <i class="fas fa-lock"></i> Lock
                            </a>
                        </div>
                        `;
                    }

                    
                }, "width": "25%"
            }
        ]
    });
};

function ToggleLock(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/User/ToggleLock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    })
}