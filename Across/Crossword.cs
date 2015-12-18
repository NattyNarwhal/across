using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: handle malformed
// TODO: smarter clue handling, so UI doesn't have to

namespace Across
{
    /// <summary>
    /// Representation of an Across Lite crossword.
    /// 
    /// Format documentation:
    /// http://www.litsoft.com/across/alite/man/AcrossTextFormat.pdf
    /// </summary>
    public class Crossword
    {
        /// <summary>
        /// The title of the crossword.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The author/editor of the puzzle.
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// The puzzle copyright notice.
        /// Consuming software will prepend a © for you.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Miscellenous comments.
        /// </summary>
        public string Notepad { get; set; }

        /// <summary>
        /// The list of clues, going down
        /// </summary>
        public List<String> Across { get; set; }
        /// <summary>
        /// The list of clues, going up.
        /// </summary>
        public List<String> Down { get; set; }

        /// <summary>
        /// The crossword puzzle itself. '.' characters will be used as
        /// black spaces.
        /// </summary>
        public char[,] Grid { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }

        public Crossword()
        {
            Title = String.Empty;
            Author = String.Empty;
            Copyright = String.Empty;
            Notepad = String.Empty;
            Across = new List<string>();
            Down = new List<string>();
            Columns = 0;
            Rows = 0;
        }

        // TODO: seperate into a parser/serializer class?
        /// <summary>
        /// Constructor for the Crossword class.
        /// </summary>
        /// <param name="acrossText">
        /// An Across Lite v1 file, in TEXT format.
        /// </param>
        public Crossword(string acrossText) : this()
        {
            StringReader sr = new StringReader(acrossText);

            StringWriter nw = new StringWriter(); // for Notepad

            SectionType state = SectionType.Magic;
            
            bool readingContent = false;
            int gridReadPos = 0;

            while (true)
            {
                string line = sr.ReadLine();
                if (line == null)
                    break;

                if (IsSectionHeader(line))
                {
                    state = GetParseState(line);
                    if (state != SectionType.Magic)
                        readingContent = true;
                    continue;
                }
                if (readingContent)
                {
                    switch (state)
                    {
                        case SectionType.Title:
                            Title = line;
                            break;
                        case SectionType.Author:
                            Author = line;
                            break;
                        case SectionType.Copyright:
                            Copyright = line;
                            break;
                        case SectionType.Size:
                            string[] sz = line.Split(new char[] { 'x' });
                            int c, r = 0; // can't use out/ref on props
                            if (int.TryParse(sz[0], out c) &
                                int.TryParse(sz[1], out r))
                            {
                                Columns = c;
                                Rows = r;
                                Grid = new char[Columns, Rows];
                            }
                            else
                            {
                                throw new Exception("Size is invalid.");
                            }
                            break;
                        case SectionType.Grid:
                            for (int i = 0; i < Columns; i++)
                            {
                                Grid[i, gridReadPos] = line[i];
                            }
                            gridReadPos++;
                            break;
                        case SectionType.Across:
                            Across.Add(line);
                            break;
                        case SectionType.Down:
                            Down.Add(line);
                            break;
                        case SectionType.Notepad:
                            nw.WriteLine(line);
                            break;
                        default: break;
                    }
                }
            }

            Notepad = nw.GetStringBuilder().ToString();
        }

        private enum SectionType
        {
            Magic, Title, Author, Copyright,
            Size, Grid, Across, Down, Notepad
        }

        private bool IsSectionHeader(string line)
        {
            return !String.IsNullOrWhiteSpace(line) &
                line.StartsWith("<") & line.EndsWith(">") &
                !String.IsNullOrWhiteSpace(line.Trim(new char[] { '<', '>' }));
        }

        private SectionType GetParseState(string line)
        {
            switch (line.Trim(new char[] { '<', '>' }))
            {
                case "ACROSS PUZZLE":
                    return SectionType.Magic;
                case "TITLE":
                    return SectionType.Title;
                case "AUTHOR":
                    return SectionType.Author;
                case "COPYRIGHT":
                    return SectionType.Copyright;
                case "SIZE":
                    return SectionType.Size;
                case "GRID":
                    return SectionType.Grid;
                case "ACROSS":
                    return SectionType.Across;
                case "DOWN":
                    return SectionType.Down;
                case "NOTEPAD":
                    return SectionType.Notepad;
                default:
                    throw new ArgumentException("Not a section header.");
            }
        }



        public byte[] GetBinaryAcrossLiteFile()
        {
            throw new NotImplementedException();
        }
    }
}
