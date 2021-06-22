var datatable;

$(document).ready(function () {
    var l = $.ajax( {
        "url": "/Admin/Categorias/ObtenerTodos"
    })
    console.log(l); //probando metodo ObtenerTodos



    loadDataTable();
  

});

function loadDataTable() {
   
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Admin/Categorias/ObtenerTodos"
        },
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        },
        "columns": [
            { "data": "nombre", "width": "20%" },
            {
                "data": "estado",
                "render": function (data) {
                    if (data == true) {
                        return "Activo";
                    }
                    else {
                        return "Inactivo";
                    }
                }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Categorias/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a onclick= Delete("/Admin/Categorias/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                <i class="fas fa-trash"></i>
                            </a>
                        </div>
                        `;
                }, "width": "20%"
            }
        ]
    });
}

function Delete(url){
    swal({
        title: "¿Esta seguro que desea Eliminar la Categoria?",
        text: "Este registro no se podra recuperar.",
        icon: "warning",
        buttons: true,
        dangerMode: true,
        timer: 5000,
        allowEscapeKey : true
    }).then((result) => {
        if (result) {
            $.ajax({
                type : "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }

            });
        }
    })
}