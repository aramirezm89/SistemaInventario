using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SistemaInventario.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name ="Numero de Serie")]
        public string NumeroSerie { get; set; }

        [Required]
        [MaxLength(60)]
        [Display(Name ="Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Range(1,9999999)]
        [Display(Name ="Precio")]
        public double Precio { get; set; }

        [Required]
        [Range(1,9999999)]
        [Display(Name ="Costo")]
        public double Costo { get; set; }

        public string ImagenUrl { get; set; }

        //foreign keys

        [Required]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        public int MarcaId { get; set; }

        [ForeignKey("MarcaId")]
        public Marca Marca { get; set; }

        //recursividad
        public int? PadreId { get; set; }
        public virtual Producto Padre { get; set; }
    }
}
