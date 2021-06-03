var datatable;

$(document).ready(function () {
    var l = $.ajax( {
        "url": "/Admin/Bodegas/ObternerTodos"
    })
    console.log(l); //probando metodo ObtenerTodos



    loadDataTable();
  

});

function loadDataTable() {
   
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Admin/Bodegas/ObternerTodos"
        },
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        },
        "columns": [
            { "data": "nombre", "width": "20%" },
            { "data": "descripcion", "width": "40%" },
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
                            <a href="Admin/Bodegas/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a class="btn btn-danger text-white" style="cursor:pointer">
                                <i class="fas fa-trash"></i>
                            </a>
                        </div>
                        `;
                }, "width": "20%"
            }
        ]
    });
}