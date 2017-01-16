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

        [Column("CodigoCredenciadora")]
        public string CodigoCredenciadora { get; set; }

        [Column("CodigoMoeda")]
        public Int16 CodigoMoeda { get; set; }

        [Column("CodigoMoedaOrigem")]
        public Int16 CodigoMoedaOrigem { get; set; }

        [Column("NomeEstabelecimento")]
        public string NomeEstabelecimento { get; set; }

        [Column("CodigoMCC")]
        public Int16 CodigoMCC { get; set; }

        [Column("FlagOriginal")]
        public bool FlagOriginal { get; set; }

        [Column("IdentificacaoTransacao")]
        public string IdentificacaoTransacao { get; set; }

        [Column("CodigoTransacao")]
        public string CodigoTransacao { get; set; }

        [Column("Id_Incoming")]
        public long Id_Incoming { get; set; }

        [Column("MensagemTexto")]
        public string MensagemTexto { get; set; }

        [Column("CodigoAutorizacao")]
        public string CodigoAutorizacao { get; set; }

        [Column("NumeroParcelas")]
        public int NumeroParcelas{get;set;}

        [Column("NumeroEstabelecimento")]
        public string NumeroEstabelecimento { get; set; }

        [Column("NSUOrigem")]
        public string NumeroLogicoEquipamento { get; set; }

        [Column("QtdDiasLiquidacao")]
        public string QtdDiasLiquidacao { get; set; }

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

        [Column("IdArquivo")]
        public int IdArquivo { get; set; }

        [Column("ValorOrigem")]
        public Decimal ValorOrigem { get; set; }

        [Column("ValorIntercambio")]
        public Decimal ValorIntercambio { get; set; }

        [Column("CicloApresentacao")]
        public int CicloApresentacao { get; set; }

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
        public int IdProduto { get; set; }




    }
}
