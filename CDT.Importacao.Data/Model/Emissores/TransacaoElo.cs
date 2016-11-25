using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model.Emissores
{
    [Table("TransacoesElo")]
    public class TransacaoElo
    {
       
        [Column("Id_TransacaoElo")]
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_TransacaoElo { get; set; }

        [Column("TE")]
        public string TE { get; set; }

        [Column("FlagProblemaTratamento")]
        public bool FlagProblemaTratamento { get; set; }

        [Column("Cartao")]
        public string Cartao { get; set; }

        [Column("DataTransacao")]
        public DateTime DataTransacao { get; set; }

        [Column("DataProcessamento")]
        public DateTime DataProcessamento { get; set; }

        [Column("Valor")]
        public Decimal Valor { get; set; }

        [Column("CodigoMoeda")]
        public int CodigoMoeda { get; set; }

        [Column("CodigoMoedaOrigem")]
        public int CodigoMoedaOrigem { get; set; }

        [Column("NomeEstabelecimento")]
        public string NomeEstabelecimento { get; set; }

        [Column("CodigoMCC")]
        public int CodigoMCC { get; set; }

        [Column("FlagOriginal")]
        public bool FlagOriginal { get; set; }

        [Column("IdentificacaoTransacao")]
        public string IdentificacaoTransacao { get; set; }

        [Column("CodigoTransacao")]
        public string CodigoTransacao { get; set; }

        [Column("Id_Incoming")]
        public string Id_Incoming { get; set; }

        [Column("MensagemTexto")]
        public string MensagemTexto { get; set; }

        [Column("CodigoAutorizacao")]
        public string CodigoAutorizacao { get; set; }

        [Column("NumeroParcelas")]
        public string NumeroParcelas{get;set;}

        [Column("FlagJuros")]
        public bool FlagJuros { get; set; }

        [Column("ParcelaPedida")]
        public int ParcelaPedida  { get; set;}

        [Column("Id_TipoParcela")]
        public int Id_TipoParcela { get; set; }

        [Column("Id_CodigoChargeback")]
        public int Id_CodigoChargeback { get; set; }

        [Column("DescricaoChargeback")]
        public string DescricaoChargeback { get; set; }

        [Column("Id_Controle")]
        public string Id_Controle { get; set; }

        [Column("NomeArquivo")]
        public string NomeArquivo { get; set; }

        [Column("ValorOrigem")]
        public Decimal ValorOrigem { get; set; }

        [Column("CicloApresentacao")]
        public string CicloApresentacao { get; set; }

        [Column("Motivo2Reapresentacao")]
        public string Motivo2Reapresentacao { get; set; }

        [Column("FlagMatchProcessado")]
        public bool FlagMatchProcessado { get; set; }

        [Column("Id_Cartao")]
        public int Id_Cartao{ get; set; }

        [Column("AcquireReferenceNumber")]
        public string AcquireReferenceNumber { get; set; }

        [Column("IndicadorMeio")]
        public string IndicadorMeio { get; set; }

        [Column("IdTipoOperacao")]
        public int IdTipoOperacao { get; set; }

        [Column("FlagTransacaoInternacional")]
        public bool FlagTransacaoInternacional { get; set; }

        [Column("IdProduto")]
        public bool IdProduto { get; set; }




    }
}
