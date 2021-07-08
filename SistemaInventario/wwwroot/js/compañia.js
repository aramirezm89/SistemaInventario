var datatable;

$(document).ready(function () {
    var l = $.ajax( {
        "url": "/Admin/Compañia/ObternerTodos"
    })
    console.log(l); //probando metodo ObtenerTodos



    loadDataTable();
  

});

function loadDataTable() {
   
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Admin/Compañia/ObternerTodos"
        },
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        },
        "columns": [
            { "data": "nombre", "width": "15" },
            { "data": "descripcion", "width": "15" },
            { "data": "pais", "width": "15" },
            { "data": "ciudad", "width": "15" },
            { "data": "telefono", "width": "15" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Compañia/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer" title="Editar">
                                <i class="fas fa-edit"></i>
                            </a>
                        `;
                }, "width": "20%"
            }
        ]
    });
}

//function Delete(url){
//    swal({
//        title: "¿Esta seguro que desea Eliminar la Categoria?",
//        text: "Este registro no se podra recuperar.",
//        icon: "warning",
//        buttons: true,
//        dangerMode: true,
//        timer: 5000,
//        allowEscapeKey : true
//    }).then((result) => {
//        if (result) {
//            $.ajax({
//                type : "DELETE",
//                url: url,
//                success: function (data) {
//                    if (data.success) {
//                        toastr.success(data.message);
//                        datatable.ajax.reload();
//                    }
//                    else {
//                        toastr.error(data.message);
//                    }
//                }

//            });
//        }
//    })
//}

function validarEntrada() {
    if (document.getElementById("logo").value == "") {
        swal("Error", "Seleccione una Imagen", "error")
        return false;
    }
    return true;
}