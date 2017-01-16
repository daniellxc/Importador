using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDT.Importacao.Data.Model;
using System.IO;
using CDT.Importacao.Data.Utils;
using CDT.Importacao.Data.DAL.Classes;
using static LAB5.LAB5Utils;
using CDT.Importacao.Data.Model.Emissores;

namespace CDT.Importacao.Data.Business.Import
{
    public class ImportadorInternacionalElo : IImportador
    {
        InformacaoRegistroDAO infRegistroDAO = new InformacaoRegistroDAO();
        RegistroDAO registroDAO = new RegistroDAO();
        List<InformacaoRegistro> buffer = new List<InformacaoRegistro>();
        List<TransacaoElo> bufferElo = new List<TransacaoElo>();
        List<Registro> registrosArquivo = new List<Registro>();

        public bool Importar(Arquivo arquivo)
        {
            StreamReader sr;
            int countLinha = 0;
            List<Registro> registros = arquivo.FK_Layout.Registros.ToList();
            List<Informacao> informacoes;
            try
            {
                FileInfo fi = new DirectoryInfo(arquivo.FK_Layout.DiretorioArquivo).GetFiles().Where(f => f.Name.Equals(arquivo.NomeArquivo)).FirstOrDefault();
                informacoes = new List<Informacao>();
                if (fi != null)
                {
                    sr = new StreamReader(fi.OpenRead());
                    string linha = sr.ReadLine();
                    ImportarInformacaoRegisto(registros.Where(r => r.FK_TipoRegistro.NomeTipoRegistro.Equals("Header")).First(), arquivo.IdArquivo, StringUtil.Zip(linha), "");
                    linha = null;
                    string copia = "";
                    while ((linha = sr.ReadLine()) != null)
                    {

                        countLinha++;
                        if (!TipoRegistroLinha(linha).Equals("00"))
                            ProcessarRegistroDetalhe(registros, arquivo, ref sr, linha);

                        copia = linha;
                    }

                    PersistirLinhas();
                    ImportarInformacaoRegisto(registros.Where(r => r.FK_TipoRegistro.NomeTipoRegistro.Equals("Trailer")).First(), arquivo.IdArquivo, StringUtil.Zip(copia), "");
                    return true;
                }
                else
                    throw new IOException("Não existe arquivo com o nome informado");
            }
            catch (IOException iox)
            {
                throw new Exception("Erro ao tentar acessar o arquivo." + iox.Message);
            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao processar o arquivo." + ex.Message);
            }
               

            
        }

        public bool GerarTransacoesEmissor(Arquivo arquivo)
        {
            InformacaoRegistroDAO infregDAO = new InformacaoRegistroDAO();
            RegistroDAO regDAO = new RegistroDAO();
            List<InformacaoRegistro> informacoes = infregDAO.BuscarDetalhesComprimidosArquivo(arquivo.IdArquivo);
        

            int limit = informacoes.Count();
            try
            {
                for (int i = 0; i < limit; i++)

                {
                    InformacaoRegistro informacoesTransacao = informacoes[i];
                    if (informacoesTransacao.Chave != string.Empty)
                    {
                        TransacaoElo transacaoElo = new TransacaoElo();
                        transacaoElo.NomeArquivo = arquivo.NomeArquivo;
                        transacaoElo.Id_Incoming = informacoesTransacao.IdInformacaoRegistro;
                        transacaoElo.FlagTransacaoInternacional = true;
                        DecomporLinha(ref transacaoElo, StringUtil.Unzip(informacoesTransacao.Valor), arquivo.IdLayout);
                        InserirBufferElo(transacaoElo, arquivo.IdEmissor);
                        transacaoElo = null;
                    }
                }
                AtualizarBufferElo(arquivo.IdEmissor);
                return true;
            }catch(Exception ex)
            {
                throw new Exception("Erro ao gerar transações na base do emissor." + ex.Message);
            }
            
        }

        #region Métodos Apoio
        private void ImportarInformacaoRegisto(Registro registro, int idArquivo, byte[] linha, string chave)
        {
            new InformacaoRegistroDAO().Salvar(new InformacaoRegistro(registro.IdRegistro, idArquivo, chave, linha));
        }


        private string TipoRegistroLinha(string linha)
        {
            return ArquivoUtils.ExtrairInformacao(linha, 1, 2);
        }

        private string TipoRegistroTransacao(string linha)
        {
            return ArquivoUtils.ExtrairInformacao(linha, 4, 4);
        }

        private void ProcessarRegistroDetalhe(List<Registro> registros, Arquivo arquivo, ref StreamReader reader, string linha)
        {
            switch (TipoRegistroLinha(linha))
            {
                case Constantes.LiquidacaoInternacionalElo.HEADER_GRUPO_CARTOES:
                    ProcessarGrupoCartoes(arquivo,ref reader, linha);
                    break;
                case Constantes.LiquidacaoInternacionalElo.HEADER_TAXAS:
                    ProcessarBloco(arquivo, ref reader, linha, Constantes.LiquidacaoInternacionalElo.TRAILER_TAXAS);
                    break;
                case Constantes.LiquidacaoInternacionalElo.HEADER_CORRECOES:
                    ProcessarBloco(arquivo, ref reader, linha, Constantes.LiquidacaoInternacionalElo.TRAILER_CORRECOES);
                    break;
                case Constantes.LiquidacaoInternacionalElo.HEADER_INTERCAMBIO:
                    ProcessarBloco(arquivo, ref reader, linha, Constantes.LiquidacaoInternacionalElo.TRAILER_INTERCAMBIO);
                    break;
                case Constantes.LiquidacaoInternacionalElo.HEADER_DESEMBOLSO:
                    ProcessarBloco(arquivo, ref reader, linha, Constantes.LiquidacaoInternacionalElo.TRAILER_DESEMBOLSO);
                    break;
            }
        }

        private void ProcessarGrupoCartoes(Arquivo arquivo, ref StreamReader reader, string linha)
        {
            bool temConversaoMoeda = false;
            string tRegistro = TipoRegistroLinha(linha);
            RegistrarInformacaoBuffer(registroDAO.Buscar(tRegistro,arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
            linha = reader.ReadLine();
            while( TipoRegistroLinha(linha)!= Constantes.LiquidacaoInternacionalElo.TRAILER_GRUPO_CARTOES)
            {
                switch (TipoRegistroLinha(linha))
                {
                    case Constantes.LiquidacaoInternacionalElo.DETALHE_SEM_SDR:
                        temConversaoMoeda = TemConversaoMoeda(linha);
                        if (temConversaoMoeda)
                            linha += ComporLinha(reader.ReadLine());
                        RegistrarInformacaoBuffer(registroDAO.Buscar("0506", arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
                        linha = reader.ReadLine();
                        break;
                    case Constantes.LiquidacaoInternacionalElo.DETALHE_COM_SDR:
                        temConversaoMoeda = TemConversaoMoeda(linha); 
                        string linhaSDR = reader.ReadLine();
                        tRegistro = TipoRegistroLinha(linhaSDR);
                        if (tRegistro.Equals(Constantes.LiquidacaoInternacionalElo.SDR))
                        {
                            linha += ComporLinha(linhaSDR);
                        }
                        else
                            throw new Exception("Registro inesperado. Confira a estrutura do arquivo.");
                        if (temConversaoMoeda)
                            linha += ComporLinha(reader.ReadLine());
                        RegistrarInformacaoBuffer(registroDAO.Buscar("0506", arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
                        linha = reader.ReadLine();
                        break;
                    
                    default:
                        break;
                }
            }
            RegistrarInformacaoBuffer(registroDAO.Buscar(TipoRegistroLinha(linha), arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);

        }

        private void ProcessarBloco(Arquivo arquivo, ref StreamReader reader, string linha, string trailerBloco)
        {
            string chave = TipoRegistroLinha(linha);
            RegistrarInformacaoBuffer(registroDAO.Buscar(chave, arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
            linha = reader.ReadLine();
            while(TipoRegistroLinha(linha) != trailerBloco)
            {
                RegistrarInformacaoBuffer(registroDAO.Buscar(TipoRegistroLinha(linha), arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
                linha = reader.ReadLine();
            }

            RegistrarInformacaoBuffer(registroDAO.Buscar(TipoRegistroLinha(linha),arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
        }

        public void PersistirLinhas()
        {

            List<List<InformacaoRegistro>> partitions = ListUtils<InformacaoRegistro>.Partition(5000, buffer);
            List<InformacaoRegistro> infos;
            foreach (List<InformacaoRegistro> laux in partitions)
            {
                infos = new List<InformacaoRegistro>();
                laux.ForEach(l => infos.Add(new InformacaoRegistro(l.IdRegistro, l.IdArquivo, l.Chave, l.Valor)));
                new InformacaoRegistroDAO().Salvar(infos);
                infos = null;
            }
        }

        private void RegistrarInformacaoBuffer(int idRegistro, int idArquivo, string dados)
        {
            string chave = TipoRegistroLinha(dados).Equals(Constantes.LiquidacaoInternacionalElo.DETALHE_COM_SDR) || 
                           TipoRegistroLinha(dados).Equals(Constantes.LiquidacaoInternacionalElo.DETALHE_SEM_SDR) 
                           ? ArquivoUtils.ExtrairInformacao(dados, 32, 41) : ""; 

            buffer.Add( new InformacaoRegistro(idRegistro, idArquivo, chave, StringUtils.Zip(dados)));
        }


        private string ComporLinha(string linha)
        {
            return Constantes.SPLITTER_LINHA + linha;
        }

        public void  DecomporLinha(ref TransacaoElo transacaoElo, string linha, int idLayout)
        {
            string tRegistro = "";
            string[] registros = StringUtils.Split(Constantes.SPLITTER_LINHA, linha);
            foreach(string reg in registros)
            {
                tRegistro = TipoRegistroLinha(reg);
                if(tRegistro.Equals(Constantes.LiquidacaoInternacionalElo.DETALHE_COM_SDR) || tRegistro.Equals(Constantes.LiquidacaoInternacionalElo.DETALHE_SEM_SDR))
                {
                    InstanciarObjetoTransacao(ref transacaoElo, registroDAO.Buscar("0506", idLayout), reg);

                }
            }
        }

        private void InstanciarObjetoTransacao(ref TransacaoElo transacao, Registro registro, string linha)
        {
            List<Campo> campos = registro.Campos.Where(x => x.FlagRelevante == true).ToList();
            transacao.FlagTransacaoInternacional = true;
            switch (registro.ChaveRegistro)
            {
                case "0506":
                    transacao.AcquireReferenceNumber = ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NUMERO DE REFERENCIA DO ADQUIRENTE")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NUMERO DE REFERENCIA DO ADQUIRENTE")).PosFim);
                    transacao.DataProcessamento = DataUtils.RetornaData(ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA ORIGINAL DE POSTAGEM")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA ORIGINAL DE POSTAGEM")).PosFim));
                    transacao.DataTransacao = DataUtils.RetornaData(ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA ORIGINAL DA TRANSACAO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA ORIGINAL DA TRANSACAO")).PosFim));
                    transacao.Cartao = ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NUMERO CARTAO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NUMERO CARTAO")).PosFim);
                    transacao.Valor = Decimal.Parse(StringUtil.StringToMoney(ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("VALOR DA TRANSACAO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("VALOR DA TRANSACAO")).PosFim)));
                    transacao.ValorOrigem = Decimal.Parse(StringUtil.StringToMoney(ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("VALOR DA TRANSACAO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("VALOR DA TRANSACAO")).PosFim)));
                    transacao.NomeEstabelecimento = ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NOME DO EC OU DESCRICAO DO AJUSTE")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NOME DO EC OU DESCRICAO DO AJUSTE")).PosFim);
                    transacao.CodigoMCC = Int16.Parse(ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("MCC")).PosInicio, campos.Find(c => c.NomeCampo.Equals("MCC")).PosFim));
                    transacao.IdentificacaoTransacao = ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NUMERO DO ID DA TRANSACAO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NUMERO DO ID DA TRANSACAO")).PosFim);
                    transacao.CodigoTransacao = ArquivoUtils.ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CODIGO DA TRANSACAO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CODIGO DA TRANSACAO")).PosFim);
                    transacao.MensagemTexto = "TRANSACAO_INTERNACIONAL";
                    break;
                default:
                    break;
            }


        }

        private bool TemConversaoMoeda(string linha)
        {
            return ArquivoUtils.ExtrairInformacao(linha, 59, 59).Equals("Y");
        }


        private void InserirBufferElo(TransacaoElo transacao, int idEmissor)
        {
            if (bufferElo.Count < Constantes.BUFFER_LIMIT)
                bufferElo.Add(transacao);
            else
            {
                AtualizarBufferElo(idEmissor);
                bufferElo.Add(transacao);
            }

        }

        private void AtualizarBufferElo(int idEmissor)
        {
            if (bufferElo.Count > 0)
            {
                new TransacoesEloDAO(idEmissor).Salvar(bufferElo);
                bufferElo.Clear();
            }

        }

        #endregion
    }
}
