using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Across;

namespace CrosswordDebug
{
    class Program
    {
        static string AcrossV1 = @"<ACROSS PUZZLE>
<TITLE>
Irreverent
<AUTHOR>
Anonymous
<COPYRIGHT>
Public Domain
<SIZE>
8x3
<GRID>
ONE.TWO.
GRIDLINE
FOUR....
<ACROSS>
Un
Deux
Random word
Quatre
<DOWN>
On Gee Eff
En Ar Oh
E I U
Tee El
Fi
Off
The Letter E
Doctor
<NOTEPAD>
Banana
AAAAAAA";

        static void Main(string[] args)
        {
            Crossword c = new Crossword(AcrossV1);
            for (int j = 0; j < c.Rows; j++)
            {
                for (int i = 0; i < c.Columns; i++)
                {
                    Console.Write(c.Grid[i, j]);
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}
