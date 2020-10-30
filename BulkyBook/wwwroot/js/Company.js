var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "street", "width": "15%" },
            { "data": "city", "width": "10%" },
            { "data": "county", "width": "10%" },
            { "data": "postcode", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "isAuthorised",
                "width": "5%",
                "render": function (data) {
                    if (data) {
                        return `<p>Yes</p>`
                    } else {
                        return `<p>No</p>`
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a class="btn btn-success text-white" href="/Admin/Company/Upsert/${data}" style="cursor: pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a class="btn btn-danger text-white" onclick=Delete("/Admin/Company/Delete/${data}") style="cursor: pointer">
                                <i class="fas fa-trash"></i>
                            </a>
                        </div>
                        `;
                }, "width": "15%"
            }
        ]
    });
};

function Delete(url) {
    swal({
        title: "Are you sure you want to delete this category?",
        text: "This action cannot be undone",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}