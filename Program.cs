using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Galaxy_On_Fire2TextTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0].Contains(".txt"))
            {
                Rebuild(args[1], args[0]);
            }
            else
            {
                Extract(args[0]);
            }
        }
        public static void Extract(string gmsg)
        {
            var reader = new BinaryReader(File.OpenRead(gmsg));
            reader.BaseStream.Position += 8;
            int count = reader.ReadInt32();
            int[] ByteCount = new int[count];
            reader.BaseStream.Position += 4;
            int[] pointers = new int[count];
            int[] stringsLen = new int[count];
            string[] strings = new string[count];
            for (int i = 0; i < count; i++)
            {
                reader.BaseStream.Position += 8;
                pointers[i] = reader.ReadUInt16();
                pointers[i] = ((int)reader.BaseStream.Position + pointers[i]) - 0xA;
                reader.BaseStream.Position += 6;
            }
            for (int i = 0; i < count; i++)
            {
                reader.BaseStream.Position = pointers[i];
                strings[i] = Utils.ReadString(reader, Encoding.Unicode);
                stringsLen[i] = strings[i].Length;
                strings[i] = strings[i].Replace("\n","<lf>").Replace("\r","<br>");
                string myString = strings[i] + "{" + stringsLen[i].ToString() + "}" + "\n";
                File.AppendAllText(gmsg + ".txt", myString);
            }
        }
        public static void Rebuild(string game, string txt)
        {
            var reader = new BinaryReader(File.OpenRead(game));
            reader.BaseStream.Position += 8;
            int count = reader.ReadInt32();
            reader.BaseStream.Position += 4;
            string[] strings = File.ReadAllLines(txt);
            string[] newstrings = new string[count];
            int[] pointers = new int[count];
            int[] fileLen = new int[count];
            for (int i = 0; i < count; i++)
            {
                reader.BaseStream.Position += 8;
                pointers[i] = reader.ReadUInt16();
                pointers[i] = ((int)reader.BaseStream.Position + pointers[i]) - 0xA;
                reader.BaseStream.Position += 6;
            }
            reader.Close();

            var writer = new BinaryWriter(File.OpenWrite(game));
            for (int i = 0; i < count; i++)
            {
                Utils.ExtractTextAndNumber(strings[i], out newstrings[i], out fileLen[i]);
                newstrings[i] = newstrings[i].Replace("<lf>", "\n").Replace("<br>", "\r");
                writer.BaseStream.Position = pointers[i];
                if (newstrings[i].Length > fileLen[i])
                {
                    Console.WriteLine($"String {i + 1} is bigger original. It's don't be replaced.");
                }
                if (newstrings[i].Length < fileLen[i])
                {
                    Console.WriteLine($"String {i + 1} smaller original. It's been replaced on {writer.BaseStream.Position}");
                    int bytes = fileLen[i] - newstrings[i].Length;
                    writer.Write(Encoding.Unicode.GetBytes(newstrings[i]));
                    writer.Write(new byte[bytes]);
                }
                else
                {
                    Console.WriteLine($"String {i + 1} like an original. It's been replaced on {writer.BaseStream.Position}");
                    writer.Write(Encoding.Unicode.GetBytes(newstrings[i]));
                }
            }
        }
    }
}
