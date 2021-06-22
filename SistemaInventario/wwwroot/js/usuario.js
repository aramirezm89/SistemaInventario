var datatable;

$(document).ready(function () {
    var l = $.ajax( {
        "url": "/Admin/Usuarios/ObtenerTodos"
    })
    console.log(l); //probando metodo ObtenerTodos



    loadDataTable();
  

});

function loadDataTable() {
   
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Admin/Usuarios/ObtenerTodos"
        },
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        },
        "columns": [
            { "data": "userName", "width": "10%" },
            { "data": "nombres", "width": "10%" },
            { "data": "apellidos", "width": "10%" },
            { "data": "email", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "role", "width": "10%"},
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var hoy = new Date().getTime();
                    var bloqueo = new Date(data.lockoutEnd).getTime();
                    if (bloqueo > hoy) {
                        //si condicion se cumple usuario esta bloqueado
                        return `
                        <div class="text-center">
                            <a onclick= BlockDesblock('${data.id}') class="btn btn-success text-white" style="cursor:pointer;width:135px;border-radius:5px">
                                <i class="fas fa-lock-open"  style="margin-right:5px" ></i>Desbloquear
                            </a>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="text-center">
                            <a onclick= BlockDesblock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer;width:135px;border-radius:5px">
                                <i class="fas fa-lock" style="margin-right:5px"></i>Bloquear
                            </a>
                        </div>
                        `;
                    }
                   
                }, "width": "20%"
             }
        ]
    });
}

function BlockDesblock(id){
   
            $.ajax({
                type : "POST",
                url: "/Admin/Usuarios/BlockDesblock",
                data: JSON.stringify(id),
                contentType : "application/json",
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