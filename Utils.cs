using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LWord
{
    public static class Utils
    {
        public static int DICTIONARY_SIZE = 10;

        public static void HighlightText(this RichTextBox myRtb, string word, Color color)
        {

            if (word == string.Empty)
                return;

            int s_start = myRtb.SelectionStart, startIndex = 0, index;

            while ((index = myRtb.Text.IndexOf(word, startIndex)) != -1)
            {
                myRtb.Select(index, word.Length);
                myRtb.SelectionColor = color;

                startIndex = index + word.Length;
            }

            myRtb.SelectionStart = s_start;
            myRtb.SelectionLength = 0;
            myRtb.SelectionColor = Color.Black;
        }

        public static int HashFunction(string s, int size)
        {
            int total = 0;
            char[] c;

            // c = s.ToLower().Normalize(NormalizationForm.FormD).ToCharArray();

            c = s.ToLower().ToCharArray();

            for (int k = 0; k <= c.GetUpperBound(0); k++)
                total += (int)c[k];

            return total % size;
        }

        public static Node[] ReadDictionary()
        {
            
            Node[] dictionary = new Node[DICTIONARY_SIZE];

            string file = "dictionary.txt";

            // Verifica se o arquivo não existe
            if (!File.Exists(file))
            {
                Debug.WriteLine(String.Format("Arquivo {0} não existe", file));
                return dictionary;
            }

            string[] lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                int hashCode = HashFunction(line, DICTIONARY_SIZE);
                if (dictionary[hashCode] == null)
                {
                    Node newNode = new Node(line, null, null);
                    dictionary[hashCode] = newNode;
                }

                else
                {
                    dictionary[hashCode].insertOrdenate(line);
                }
                
            }

            return dictionary;
        }

        public static string NormalizeString(string text)
        {
            foreach (var chr in new string[] { "(", ")", "!", "@", "#", "[", "]", "?" })
            {
                text = text.Replace(chr, "");
            }

            return text;
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }


        public static void CheckWord(string word, Node[] dictionary, RichTextBox richTextBox)
        {
            // Ignorar textos que forem digitos
            if (!int.TryParse(word, out _))
            {
                string normalizedWord = NormalizeString(RemoveDiacritics(word));
                int hashCode = HashFunction(normalizedWord, DICTIONARY_SIZE);

                if (dictionary[hashCode] == null)
                {
                    HighlightText(richTextBox, NormalizeString(word), Color.Red);
                }

                else
                {
                    Node node = dictionary[hashCode].find(normalizedWord);

                    if (node.getElement() == null)
                    {
                        HighlightText(richTextBox, NormalizeString(word), Color.Red);
                    }
                }
            }
        } 


    }
}
