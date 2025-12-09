using _2HerenciaSimpleIES;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// esta carpeta siempre la creo con este contenido 

namespace IGraficaIES.Context
{
    class MyDbContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // en Catalog pongo el nombre de la base de datos
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=IGraficaIES_Lucia;" +
                "Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;" +
                "Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
        public DbSet<ProfesorFuncionario> ProfesoresFuncionarios { get; set; }
        public DbSet<ProfesorExtendido> ProfesoresExtendidos { get; set; }
    }
}

// Crear base de datos:
// Entramos en Explorador de objetos de SQL Server
// SQL Server > (localdb) > Base de datos > Clic derecho sobre Base de datos > nueva base de datos > nombre


// Migración 
// Herramientas > Administrador de paquetes NuGet > Consola del administrador de paquetes
// Add-migration crear_bd
// update-database
