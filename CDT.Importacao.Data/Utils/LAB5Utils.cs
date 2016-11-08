using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB5
{
    public class LAB5Utils
    {
        /// <summary>
        /// Utilitários para manipulação de arquivos texto
        /// </summary>
        public class ArquivoUtils
        {
            /// <summary>
            /// Método para extrair a informação de uma linha utilizando a posição inicial e final.
            /// </summary>
            /// <param name="linha"></param>
            /// <param name="posIni"></param>
            /// <param name="posFim"></param>
            /// <returns></returns>
            public static string ExtrairInformacao(string linha, int posIni, int posFim)
            {
                try
                {
                    return linha.Substring(posIni - 1, (posFim - posIni) + 1);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new Exception("Intervalo informado estava fora da linha.");
                }
                catch (Exception)
                {
                    return "";
                }


            }
        }
        /// <summary>
        /// Utilitários para manipulação de strings
        /// </summary>
        public class StringUtils
        {
            public static void CopyTo(Stream src, Stream dest)
            {
                byte[] bytes = new byte[4096];

                int cnt;

                while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
                {
                    dest.Write(bytes, 0, cnt);
                }
            }

            public static byte[] Zip(string str)
            {
                var bytes = Encoding.UTF8.GetBytes(str);

                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        //msi.CopyTo(gs);
                        CopyTo(msi, gs);
                    }

                    return mso.ToArray();
                }
            }

            public static string Unzip(byte[] bytes)
            {
                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        //gs.CopyTo(mso);
                        CopyTo(gs, mso);
                    }

                    return Encoding.UTF8.GetString(mso.ToArray());
                }
            }


        }
        /// <summary>
        /// Utilitários para manipulação de listas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public  class ListUtils<T> where T:class
        {
            public static List<List<T>> Partition(int partitionSize, List<T> source)
            {
                return source.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / partitionSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
            }
        }
        /// <summary>
        /// Utilitários para manipulação de datas
        /// </summary>
        public class DataUtils
        {
            public static DateTime RetornaData(string data)
            {
                if (data.Length == 8)
                {
                    return DateTime.ParseExact(data,"yyyyMMdd", CultureInfo.InvariantCulture);
                }
                else
                    throw new Exception("Campo data em formato incorreto");
            }
        }
    }
}
