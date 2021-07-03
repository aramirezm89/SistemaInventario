var datatable;
var datatableHistorial;

$(document).ready(function () {
    var l = $.ajax( {
        "url": "/Inventario/Inventario/ObtenerTodos"
    })
    console.log(l); //probando metodo ObtenerTodos



    loadDataTable();
});



function loadDataTable() {
   
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Inventario/Inventario/ObtenerTodos"
        },
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        },
        "columns": [
            { "data": "bodega.nombre", "width": "20%" },
            { "data": "producto.descripcion", "width": "30%" },
            {
                "data": function ProductoCosto(data) {
                    const formatterPeso = new Intl.NumberFormat('es-CO', {
                        style: 'currency',
                        currency: 'COP',
                        minimumFractionDigits: 2
                    })
                    return formatterPeso.format(data.producto.costo)
            }, "width": "10%", "classname": "text-end"},
            { "data": "cantidad", "width": "10%", "classname": "text-end"}
        ]
    });
}
