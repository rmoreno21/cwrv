using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Interseguro.CWRV.Dominio.Entidades;
using System.Data.Entity.Infrastructure;


namespace Interseguro.CWRV.Infraestructura.Datos.Nucleo
{
    public class CWRVDataContext : DbContext
    {
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<Asistencia> Asistencia { get; set; }

        //public DbSet<Category> Categories { get; set; }
        //public DbSet<Expense> Expenses { get; set; }
        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }


        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Curso>().ToTable("Curso");
            modelBuilder.Entity<Asistencia>().ToTable("Asistencia");
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
        }

        //public void GuardarCambios()
        //{
        //    SaveChanges();
        //}

        //public IDbSet<TEntidad> CrearSet<TEntidad>() where TEntidad : class
        //{
        //    return Set<TEntidad>();
        //}

    }
}
