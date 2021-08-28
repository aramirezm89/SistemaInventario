var dataTable;

$(document).ready(() => {
    var prueba = $.ajax({
        "url": "/Admin/Orden/ObtenerOrdenLista"
    })
    var url = window.location.search;
    console.log(prueba);

    if (url.includes("pendiente")) {
        loadDataTable("ObtenerOrdenLista?estado=pendiente");
    } else {
        if (url.includes("aprobado")) {
            loadDataTable("ObtenerOrdenLista?estado=aprobado");
        } else {
            if (url.includes("completado")) {
                loadDataTable("ObtenerOrdenLista?estado=completado");
            } else {
                if (url.includes("rechazado")) {
                    loadDataTable("ObtenerOrdenLista?estado=rechazado");
                } else {
                    loadDataTable("ObtenerOrdenLista?estado=todas");
                }
            }
        }
    }
  
})

function loadDataTable(url) {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": `/Admin/Orden/${url}`
        },
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json",
            "decimal": ",",
            "thousands": "."
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "nombresCliente", "width": "15%" },
            { "data": "telefono", "width": "15" },
            { "data": "usuaioAplicacion.email", "width": "15%" },
            { "data": "estadoOrden", "width": "15%" },
            { "data": "totalOrden", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                           <div class="text-center">
                               <a href="/Admin/Orden/Detalle/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-list-ul"></i>
                                </a>
                            </div>
                        `;
                },"width":"5%"
            }

        ]
    })
}