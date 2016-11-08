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

        public void Importar(Arquivo arquivo)
        {
            StreamReader sr;
            int countLinha = 0;
            List<Registro> registros = arquivo.FK_Layout.Registros.ToList();
            List<Informacao> informacoes;
            if (Directory.Exists(arquivo.FK_Layout.DiretorioArquivo))
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

                }
                else
                    throw new IOException("Não existe arquivo com o nome informado");

            }
        }

        #region Métodos
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
            string tRegistro = TipoRegistroLinha(linha);
            RegistrarInformacaoBuffer(registroDAO.Buscar(tRegistro,arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
            linha = reader.ReadLine();
            while( TipoRegistroLinha(linha)!= Constantes.LiquidacaoInternacionalElo.TRAILER_GRUPO_CARTOES)
            {
                switch (TipoRegistroLinha(linha))
                {
                    case Constantes.LiquidacaoInternacionalElo.DETALHE_SEM_SDR:
                        RegistrarInformacaoBuffer(registroDAO.Buscar("0506", arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
                        linha = reader.ReadLine();
                        break;
                    case Constantes.LiquidacaoInternacionalElo.DETALHE_COM_SDR:
                        RegistrarInformacaoBuffer(registroDAO.Buscar("0506", arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
                        linha = reader.ReadLine();
                        tRegistro = TipoRegistroLinha(linha);
                        if (tRegistro.Equals(Constantes.LiquidacaoInternacionalElo.SDR))
                        {
                            RegistrarInformacaoBuffer(registroDAO.Buscar(tRegistro, arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
                            linha = reader.ReadLine();
                        }
                        else
                            throw new Exception("Registro inesperado. Confira a estrutura do arquivo.");
                        break;
                    case Constantes.LiquidacaoInternacionalElo.CONVERSAO_MOEDAS:
                        RegistrarInformacaoBuffer(registroDAO.Buscar(Constantes.LiquidacaoInternacionalElo.CONVERSAO_MOEDAS, arquivo.IdLayout).IdRegistro, arquivo.IdArquivo, linha);
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

        #endregion
    }
}
