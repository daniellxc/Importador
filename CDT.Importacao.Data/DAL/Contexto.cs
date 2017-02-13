using CDT.Importacao.Data.DAL;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace CDT.Importacao.Data.Model
{
    public class Contexto : DbContext, IContext
    {

        #region DBSets
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Arquivo> Arquivos { get; set; }
        public DbSet<Campo> Campos { get; set; }
        public DbSet<ErroValidacaoArquivo> ErrosValidacaoArquivo { get; set; }
        public DbSet<ExecucaoAgendamento> ExecucoesAgendamento{ get; set; }
        public DbSet<Informacao> Informacao { get; set; }
        public DbSet<InformacaoRegistro> InformacoesRegistro { get; set; }
        public DbSet<Layout> Layouts { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Registro> Registros { get; set; }
        public DbSet<Subcampo> Subcampos { get; set; }
        public DbSet<TipoDado> TiposDado { get; set; }
        public DbSet<TipoRegistro> TiposRegistro { get; set; }
        public DbSet<TipoSubcampo> TiposSubcampo { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

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

        public DbContext GetContext()
        {
            
            return this;
            
        }
    }
}
