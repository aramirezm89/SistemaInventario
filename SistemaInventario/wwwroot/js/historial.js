
var datatableHistorial;
$(document).ready(function () {

    var dato = $.ajax({

        "url": "/Inventario/Inventario/ObtenerHistorial"
    })
    console.log(dato);
    loadDatatableHistorial();
})




function loadDatatableHistorial() {

    datatableHistorial = $('#tblHistorialInv').DataTable({
        "ajax": {
            "url": "/Inventario/Inventario/ObtenerHistorial"
        },
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        },
        "columns": [
            {
                "data": "fechaInicial", "width": "15%",
                "render": function (data) {
                    var d = new Date(data);
                    return d.toLocaleDateString();
                }
            },
            {
                "data": "fechaFinal", "width": "15%",
                "render": function (data) {
                    var d = new Date(data);
                    return d.toLocaleDateString();
                }
            },
            { "data": "bodega.nombre", "width": "15%" },
            {
                "data": function nombreUsuario(data) {
                    return data.usuarioAplicacion.nombres + "," + data.usuarioAplicacion.apellidos;
                }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Inventario/Inventario/DetalleHistorial/${data}" class="btn btn-success text-white" style="cursor:pointer">
                              Detalle
                            </a>
                        </div>
                        `;
                }, "width": "10%"
            }
        ]
    })
}