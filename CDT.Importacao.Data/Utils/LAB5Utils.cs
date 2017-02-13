using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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

            public static string AlterarInformacao(string linha, string novoValor, int posIni, int posFim)
            {
                int i = novoValor != string.Empty ? novoValor.Count() : 1;
              
                if (novoValor.Length != (posFim - posIni) + 1)
                    throw new Exception("Tamanho do novo valor difere o intervalo informado");
                string result="";
                try
                {
                    StringBuilder sb = new StringBuilder(linha);
                    sb.Remove(posIni - 1, (posFim - posIni) + 1);
                    sb.Insert(posIni - 1, novoValor);
                    result = sb.ToString();
                }
                catch (IndexOutOfRangeException)
                {
                    throw new Exception("Intervalo informado estava fora da linha.");
                }
                catch (Exception)
                {
                    return result;
                }

                return result;
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

            public static string[] Split(String splitter, string str)
            {
                return str.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
            }

            public static bool IsNumber(string str)
            {
                foreach (char c in str)
                    if (!Char.IsNumber(c))
                        return false;
                return true;
            }



            public static string OnlyNumbers(string valor)
            {
                string retorno = "";
                for(int i = 0; i < valor.Length; i++)
                {
                    if (Char.IsNumber(valor[i]))
                        retorno += valor[i];
                }
                return retorno;
            }
        }//FIM STRING UTIL



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
            /// <summary>
            /// Retorna data a partir de uma string no formato YYYYMMDD
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static DateTime RetornaData(string data)
            {
                if (data.Length == 8)
                {
                    return DateTime.ParseExact(data,"yyyyMMdd", CultureInfo.InvariantCulture);
                }
                else
                    throw new Exception("Campo data em formato incorreto");
            }

            public static string RetornaDataYYYYMMDD(DateTime data)
            {
                return data.ToString("yyyyMMdd");
            }

            public static string RetornaHoraHHMMSS(DateTime data)
            {
                return data.ToString("HHmmss");
            }
        }

        public class CriptografiaUtils
        {
            private static string key = "jesusaverdadeeavida";

            public static string TripleDESEncrypt(string toEncrypt, bool useHashing)
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                //System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
                // Get the key from config file

                
                //System.Windows.Forms.MessageBox.Show(key);
                //If hashing use get hashcode regards to your key
                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    //Always release the resources and flush data
                    //of the Cryptographic service provide. Best Practice

                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                //set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;
                //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
                tdes.Mode = CipherMode.ECB;
                //padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                //transform the specified region of bytes array to resultArray
                byte[] resultArray = cTransform.TransformFinalBlock
                        (toEncryptArray, 0, toEncryptArray.Length);
                //Release resources held by TripleDes Encryptor
                tdes.Clear();
                //Return the encrypted data into unreadable string format
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }


            public static string TripleDESDecrypt(string cipherString, bool useHashing)
            {
                byte[] keyArray;
                //get the byte code of the string

                byte[] toEncryptArray = Convert.FromBase64String(cipherString);


                if (useHashing)
                {
                    //if hashing was used get the hash code with regards to your key
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    //release any resource held by the MD5CryptoServiceProvider

                    hashmd5.Clear();
                }
                else
                {
                    //if hashing was not implemented get the byte code of the key
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);
                }

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                //set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;
                //mode of operation. there are other 4 modes.
                //We choose ECB(Electronic code Book)

                tdes.Mode = CipherMode.ECB;
                //padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock
                        (toEncryptArray, 0, toEncryptArray.Length);
                //Release resources held by TripleDes Encryptor
                tdes.Clear();
                //return the Clear decrypted TEXT
                return UTF8Encoding.UTF8.GetString(resultArray);
            }


            public static string HashMd5(string input)
            {
                MD5 md5Hash = MD5.Create();
                // Converter a String para array de bytes, que é como a biblioteca trabalha.
                byte[] data = md5Hash.ComputeHash(Encoding.ASCII.GetBytes(input));


                // Cria-se um StringBuilder para recompôr a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop para formatar cada byte como uma String em hexadecimal
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("X"));
                }

                return sBuilder.ToString();
            }


            public static byte[] GetMD5(string input)
            {
                MD5 md5Hash = MD5.Create();
                // Converter a String para array de bytes, que é como a biblioteca trabalha.
                byte[] data = md5Hash.ComputeHash(Encoding.ASCII.GetBytes(input));
                Array.Reverse(data);
                return data;
            }

        }//FIM CRIPTOGRAFIA UTILS


       


        public class ReflectionUtils
        {
            public static List<Type> GetTypes(string assemblyName,string namespaceName)
            {
                try
                {
                    Assembly asm = Assembly.Load(assemblyName);
                    var classes = asm.GetTypes().Where(p=>p.Namespace ==namespaceName).ToList();
                    return classes;
                }
                catch(Exception ex)
                {
                    return null;
                }
               
            }

            public static object TypeActivator(string assemblyname, string fullClassName)
            {
                Assembly asm = Assembly.Load(assemblyname);
                Type type = asm.GetType(fullClassName);

                Object imp = Activator.CreateInstance(type);
                if (imp != null)
                    return imp;
                return null;
            }

            public static string GetObjectDescription(Object entity)
            {
                string dados = "";

                foreach (PropertyInfo pi in entity.GetType().GetProperties())
                {
                    dados += pi.Name + ":" + Convert.ToString(pi.GetValue(entity, null)) + "|";
                }

                return dados;
            }
        }

        #region Utilitarios para Diretorios
        public class DirectoryUtils
        {
            public static DirectoryInfo CreateDirectory(string path)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if(!di.Exists)
                    di.Create();
                return di;
            }

        }

        #endregion


       
    }
}
