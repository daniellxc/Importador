using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB5
{
    public class LAB5Utils
    {
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


        public class DataUtils
        {
            public static DateTime RetornaData(string data)
            {
                if (data.Length == 8)
                {
                    return DateTime.Parse(data);
                }
                else
                    throw new Exception("Campo data em formato incorreto");
            }
        }
    }
}
