using System;
using System.Collections.Generic;
using System.IO;

namespace WebLZW {
    class LZW {
        private string alphabet = "";

        private Dictionary<string, int> dictioanry;
        private List<string> list_dictioanry;
        public int progress { get; private set; }
        public int last_initial_size { get; private set; }
        public int last_compressed_size { get; private set; }

        public LZW() {
            for (int i = System.Char.MinValue; i < System.Char.MaxValue; i++) {
                alphabet += Convert.ToChar(i).ToString();
            }

            dictioanry = new Dictionary<string, int>();
            list_dictioanry = new List<string>();

            progress = 0;
            last_initial_size = 0;
            last_compressed_size = 0;

            for (int i = 0; i < alphabet.Length; i++) {
                if (dictioanry.ContainsKey(alphabet[i].ToString()))
                    continue;

                dictioanry.Add(alphabet[i].ToString(), dictioanry.Count);
            }
        }

        public void ResetDictionary() {
            dictioanry.Clear();
            list_dictioanry.Clear();

            for (int i = 0; i < alphabet.Length; i++) {
                if (dictioanry.ContainsKey(alphabet[i].ToString()))
                    continue;

                dictioanry.Add(alphabet[i].ToString(), dictioanry.Count);
            };
        }

        public void ResetSizes() {
            last_initial_size = 0;
            last_compressed_size = 0;
        }

        public void Compress(FileStream file, StreamWriter compressedFileWriter) {
            StreamReader reader = new StreamReader(file);

            string c = "";
            string s = "";

            int fileLength = (int)file.Length;

            last_initial_size += fileLength;

            for (int i = 0; i < fileLength; i++) {
                try {
                    var g = reader.Read();
                    if (g == -1)
                        break;
                    c = Convert.ToChar(g).ToString();

                    if (dictioanry.ContainsKey(s + c))
                        s += c;
                    else {
                        var sc = dictioanry[s];
                        if (s != "" && sc != -1)
                            compressedFileWriter.WriteLine(sc);
                        dictioanry.Add(s + c, dictioanry.Count);
                        s = c;
                    }

                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message + ' ' + c);
                }
            }
            compressedFileWriter.WriteLine(dictioanry[s]);
            compressedFileWriter.Close();
            file.Close();
        }

        public void Decompress(StreamReader reader, StreamWriter decompressedFileWriter) {
            foreach (string s in dictioanry.Keys)
                list_dictioanry.Add(s);

            progress = 0;

            int c = 0;
            int p = 0;
            string prev = "";

            p = Convert.ToInt32(reader.ReadLine());
            decompressedFileWriter.Write(list_dictioanry[p]);
            prev = list_dictioanry[p];

            for (int i = 0; ; i++) {
                var r = reader.ReadLine();

                var str = "";

                if (r == null || r == 'e'.ToString())
                    break;

                c = Convert.ToInt32(r);

                if (c >= list_dictioanry.Count)
                    str = list_dictioanry[p] + prev;
                else
                    str = list_dictioanry[c];

                decompressedFileWriter.Write(str);

                prev = str[0].ToString();

                list_dictioanry.Add(list_dictioanry[p] + prev);

                p = c;

                progress = i;
            }
            reader.Close();
            decompressedFileWriter.Close();
        }
        
    }
}