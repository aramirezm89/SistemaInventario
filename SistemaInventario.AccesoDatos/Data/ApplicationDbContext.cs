﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Modelos;


namespace SistemaInventario.AccesoDatos.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Marca> Marcas { get; set; }

        public DbSet<Producto> Producto { get; set; }

        public DbSet<UsuaioAplicacion> UsuarioAplicacion { get; set; }

        public DbSet<BodegaProducto> BodegaProducto { get; set; }

        public DbSet<Inventario> Inventario { get; set; }

        public DbSet<InventarioDetalle> InventarioDetalle { get; set; }

        public DbSet<Compañia> Compañia { get; set; }
    }
}
