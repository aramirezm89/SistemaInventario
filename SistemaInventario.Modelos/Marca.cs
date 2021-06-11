﻿using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Modelos
{
    public class Marca
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Nombre Marca")]
        public string Nombre { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}
