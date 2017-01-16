using CDT.Importacao.Data.Model.Emissores;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL
{
    public class ContextoEmissor :DbContext, IContext
    {

        public DbSet<Autorizacoes> Autorizacoes { get; set; }
        public DbSet<Cartoes> Cartoes{ get; set; }
        public DbSet<EventosExternosComprasNaoProcessados> EventosExternosComprasNaoProcessados { get; set; }
        public DbSet<TransacaoElo> TransacoesElo { get; set; }

        public ContextoEmissor(string connectionString):base(connectionString)
        {
            this.Configuration.AutoDetectChangesEnabled = false;
        }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbContext GetContext()
        {
            return this;
        }
    }
}
