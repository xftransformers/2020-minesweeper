using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFMinesweeper_V1
{
    //A Console-based version of minesweeper
    //Copyright(C) 2020     Simeon K

    //This program is free software: you can redistribute it and/or modify
    //it under the terms of the GNU General Public License as published by
    //the Free Software Foundation, either version 3 of the License, or
    //(at your option) any later version.

    //This program is distributed in the hope that it will be useful,
    //but WITHOUT ANY WARRANTY; without even the implied warranty of
    //MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    //GNU General Public License for more details.

    //You can contact the author and get a copy of the orginial code from:
    //https://github.com/xftransformers

    //You should have received a copy of the GNU General Public License
    //along with this program.If not, see<https://www.gnu.org/licenses/>.
    public class Cell
    {
        public bool mine;
        public string state;
        public bool marked;

        public Cell()
        {
            mine = false;
            state = "";
            marked = false;
        }
    }
    
    class Program
    {
        #region constants and variables
        const int mines = 10;
        const int x = 8;
        const int y = 8;

        

        static Cell[,] Board = new Cell[x, y];
        static bool[,] visible = new bool[x, y];
        
        #endregion
        static void Main(string[] args)
        {
            #region generate the board
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    Board[i, j] = new Cell();
                }
            }
            #endregion
            Generate();
            Display(visible, Board);
            bool end = false;
            while (!end)
            {
                int[] guesses = GameLoop();
                Console.Clear();
                Display(visible, Board);
                end = CheckWin(guesses); 
            }
            Console.Read();
        }

        static string CheckSurroundings(int locx, int locy,Cell[,] table)
        {
            int adjacencies = 0;
            // [x-1,y-1],[x-1,y],[x-1,y+1],[x,y-1],[x,y+1],[x+1,y-1],[x+1,y],[x+1,y+1]
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    try
                    {
                        if (table[locx + j, locy + i].mine)
                        {
                            adjacencies++;
                        }
                    }catch{}
                }
              }
            string outcome = adjacencies.ToString();
            if (adjacencies == 0)
            {
                outcome = "-";
            }
            return outcome;
        }

        static void Display(bool[,] visible, Cell[,] Board)
        {
            Console.Write(" |");
            for (int i = 0; i < x; i++)
            {
                Console.Write(i);
            }
            Console.WriteLine(Environment.NewLine + "-+--------");
            for (int j = 0; j < y; j++)
            {
                Console.Write(j + "|");
                for (int i = 0; i < x; i++)
                {
                    if (visible[i, j])
                    {
                        Console.Write(Board[i, j].state);
                    }
                    else if (Board[i,j].marked)
                    {
                        Console.Write("F");
                    }
                    else
                    {
                        Console.Write("#");
                    }
                }
                Console.WriteLine();
            }
        }

        static void Fill(int locx, int locy)
        {
            Debug.WriteLine("Filling " + locx + "," + locy);
            try
            {
                if (Board[locx, locy].state != "X" && !visible[locx, locy])
                {
                    visible[locx, locy] = true;
                    if (Board[locx,locy].state == "-")
                    {
                        Fill(locx + 1, locy);
                        Fill(locx - 1, locy);
                        Fill(locx, locy + 1);
                        Fill(locx, locy - 1);
                        Fill(locx + 1, locy + 1);
                        Fill(locx + 1, locy - 1);
                        Fill(locx - 1, locy + 1);
                        Fill(locx - 1, locy - 1);
                    }
                    
                    
                }
            }
            catch
            {
            }
            
            return;
        }

        static void Generate()
        {
            Random random = new Random();
            int minesToPlace = mines;
            int tempx = 0;
            int tempy = 0;
            while (minesToPlace > 0)
            {
                minesToPlace -= 1;
                tempx = random.Next(x);
                tempy = random.Next(y);
                Board[tempx, tempy].mine = true;

            }


            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    if (Board[i, j].mine)
                    {
                        Board[i, j].state = "X";
                    }
                    else
                    {
                        Board[i, j].state = CheckSurroundings(i, j, Board);
                    }
                    visible[i, j] = false;
                    Debug.Write(Board[i, j].state);
                }
                Debug.WriteLine("");
            }
        }

        static int[] GameLoop()
        {
            int guessx, guessy;
            Console.WriteLine();
            Console.WriteLine("Mark Mines or Clear Land? (M/C)");
            string option = Console.ReadLine();
        LoopStart:
            try
            {
                Console.Write("X: ");
                guessx = int.Parse(Console.ReadLine());
                Console.Write("Y: ");
                guessy = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                goto LoopStart;
            }
            if (option == "M" || option == "m")
            {
                Board[guessx, guessy].marked = true;
            }
            else
            {
                if (Board[guessx, guessy].state == "-")
                {
                    Fill(guessx, guessy);
                }
                else
                {
                    visible[guessx, guessy] = true;
                }
            }
            int[] guesses = new int[2];
            guesses[0] = guessx;
            guesses[1] = guessy;
            return guesses;
        }

        static bool CheckWin(int[] guesses)
        {
            int guessx = guesses[0];
            int guessy = guesses[1];
            int visibleCells = 0;
            foreach (bool cell in visible)
            {
                if (cell)
                {
                    visibleCells++;
                }
            }

            if (Board[guessx, guessy].mine && visible[guessx, guessy])
            {
                
                Console.WriteLine("YOU LOSE");
                return true;
            }
            else if (visibleCells == (x * y) - mines)
            {
                
                Console.WriteLine("YOU WIN!");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
