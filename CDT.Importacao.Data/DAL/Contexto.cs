using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace CDT.Importacao.Data.Model
{
    public class Contexto:DbContext
    {

        #region DBSets
        public DbSet<Arquivo> Arquivos { get; set; }
        public DbSet<Campo> Campos { get; set; }
        public DbSet<Informacao> Informacao { get; set; }
        public DbSet<InformacaoRegistro> InformacoesRegistro { get; set; }
        public DbSet<Layout> Layouts { get; set; }
        public DbSet<Registro> Registros { get; set; }
        public DbSet<TipoDado> TiposDado { get; set; }
        public DbSet<TipoRegistro> TiposRegistro { get; set; }
        #endregion 


        public Contexto():base("Importacao")
        {
            this.Configuration.AutoDetectChangesEnabled = false;    
        }

      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Informacao>().HasKey(e => e.idInformacao);
            base.OnModelCreating(modelBuilder);


        }


    
    }
}
