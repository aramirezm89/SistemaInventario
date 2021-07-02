using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaInventario.Modelos
{
    public class Inventario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string UsuarioAplicacionId { get; set; }

        [ForeignKey("UsuarioAplicacionId")]
        public UsuaioAplicacion UsuarioAplicacion { get; set; }

        [Required]
        [Display(Name = "Fecha Inicial")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:DD-mm-yyyy HH:mm}")]
        public DateTime FechaInicial { get; set; }

        [Display(Name = "FechaFinal")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:DD-mm-yyyy HH:mm}")]
        public DateTime FechaFinal { get; set; }

        [Required]
        [Display(Name = "Bodega")]
        public int BodegaId { get; set; }

        [ForeignKey("BodegaId")]
        public Bodega Bodega { get; set; }

        public bool Estado { get; set; }




    }
}
