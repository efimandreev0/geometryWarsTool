using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_On_Fire2TextTool
{
    internal static class Utils
    {
        public static void ExtractTextAndNumber(string line, out string text, out int number)
        {
            int openingBracketIndex = line.IndexOf('{');
            int closingBracketIndex = line.IndexOf('}');

            if (openingBracketIndex != -1 && closingBracketIndex != -1 && closingBracketIndex > openingBracketIndex)
            {
                text = line.Substring(0, openingBracketIndex);
                string numberString = line.Substring(openingBracketIndex + 1, closingBracketIndex - openingBracketIndex - 1);

                if (int.TryParse(numberString, out number))
                {
                    return; // успешно извлекли текст и число
                }
            }

            text = null; // не удалось извлечь текст
            number = 0; // не удалось извлечь число
        }

        public static string ReadString(BinaryReader binaryReader, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            List<byte> data = new List<byte>();

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                data.Add(binaryReader.ReadByte());

                string partialString = encoding.GetString(data.ToArray(), 0, data.Count);

                if (partialString.Length > 0 && partialString.Last() == '\0')
                    return encoding.GetString(data.SkipLast(encoding.GetByteCount("\0")).ToArray());
            }
            throw new InvalidDataException("Hit end of stream while reading null-terminated string.");
        }
        private static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");

            Queue<TSource> queue = new Queue<TSource>();

            foreach (TSource item in source)
            {
                queue.Enqueue(item);

                if (queue.Count > count) yield return queue.Dequeue();
            }
        }
    }
}
