$(document).ready(function () {
    loadDataTable();
    $('#tblData').on('click', '.btn-delete', function (e) {
        e.preventDefault();
        var productId = $(this).data('id');
        if (confirm("Are you sure you want to delete this product?")) {
            deleteProduct(productId);
        }
    });
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "20%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="btn-group" role="group">
                    <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2">
                        Edit
                    </a>
                    <a href="#" class="btn btn-danger mx-2 btn-delete" data-id="${data}">
                        Delete
                    </a>
                </div>`
                },
                "width": "20%"
            }
        ]
    });
}

function deleteProduct(productId) {
    $.ajax({
        url: '/admin/product/delete/' + productId,
        type: 'DELETE',
        success: function (response) {
            console.log(response);
            dataTable.ajax.reload();
        },
        error: function (error) {
            console.log(error);
        }
    });
}
