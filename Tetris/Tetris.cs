﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris
{
    class Tetris
    {
        
        const char BorderCharacter = (char)219;
        static int Score = 0;
        static int Level = 1;

       const int TetrisWidth = 10;
       const int TetrisHeigh = 16;
       const int InfoPanelWidth = 10;
       const int GameWidth = TetrisWidth + InfoPanelWidth + 3;
       const int GameHeight = TetrisHeigh + 2;
        #region Figyres
        static bool[][,] Figyres = new bool[7][,]
            {
                new bool[,]// I
                {
                    {true,true,true,true }
                },
                 new bool[,]// L
                {
                    {true,true,true},
                    {false,false,true }
                },
                new bool[,]// J
                {
                    {true,true,true},
                    {true,false,false}
                },
                new bool[,]// O
                {
                    {true,true},
                    {true,true}
                },
                new bool[,]// S
                {
                    {false,true,true},
                    {true,true,false}
                },
                new bool[,]// T
                {
                    {true,true,true },
                    {false,true,false}
                },
                 new bool[,]// Z
                {
                    {true,true,false },
                    {false,true,true}
                },
            };
        #endregion
        static bool[,] currentFigure;
        static int currentFigureRow = 0;
        static int currentFigureCol = 4;
        static bool[,] nextFigure;
        static Random random = new Random();

        static bool[,] gameState = new bool[TetrisHeigh, TetrisWidth];

        static int[] ScorePerLines = { 10, 30, 50, 80 };
        static int[] speedPerLevel = { 800, 700, 600, 500, 400, 300, 200, 250, 100, 50 };

        static void Main(string[] args)
        {
            



            Console.OutputEncoding = Encoding.GetEncoding(1252);
            Console.CursorVisible = false;
            Console.Title = "Tetris";
            Console.WindowWidth = GameWidth;
            Console.BufferWidth = GameWidth;
            Console.WindowHeight = GameHeight + 1;
            Console.BufferHeight = GameHeight + 1;

            StartNewGame();
            PrintBorders();


            Task.Run(() =>
            {
                while (true)
                {
                    PlaySound();
                }
            });

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        if (currentFigureCol > 1)
                        {
                            currentFigureCol--;

                        }
                    }
                    else if (key.Key == ConsoleKey.RightArrow)
                    {
                        if (currentFigureCol + currentFigure.GetLength(1) - 1 < TetrisWidth)
                        {
                            currentFigureCol++;

                        }
                    }
                }

                if (CollisionDetection())
                {
                    PlaceCurrentFigure();
                    int removedLines = CheckForFullLines();

                    if (removedLines > 0)
                    {
                        Score += ScorePerLines[removedLines - 1] * Level;
                    }
                    Level = Score / 1000 + 1;

                    currentFigure = nextFigure;
                    nextFigure = Figyres[random.Next(0, Figyres.Length)];
                    currentFigureRow = 1;
                    currentFigureCol = 4;
                }
                else
                {
                    currentFigureRow++;

                }
                PrintInfoPanel();

                PrintGameField();

                PrintBorders();

                PrintFigure(currentFigure, currentFigureRow, currentFigureCol);



                Thread.Sleep(speedPerLevel[Level - 1]);
            }
        }

        static int CheckForFullLines()
        {

            int linesRemoved = 0;

            for (int row = 0; row < gameState.GetLength(0); row++)
            {
                bool isFullLine = true;
                for (int col = 0; col < gameState.GetLength(1); col++)
                {
                    if (gameState[row, col] == false)
                    {
                        isFullLine = false;
                        break;
                    }
                }
                if (isFullLine)
                {
                    for (int nextLine = row - 1; nextLine >= 0; nextLine--)
                    {
                        if (row < 0)
                        {
                            continue;
                        }

                        for (int colFromNextLine = 0; colFromNextLine < gameState.GetLength(1); colFromNextLine++)
                        {
                            gameState[nextLine + 1, colFromNextLine] = gameState[nextLine, colFromNextLine];
                        }
                    }

                    for (int colLastLine = 0; colLastLine < gameState.GetLength(1); colLastLine++)
                    {
                        gameState[0, colLastLine] = false;
                    }
                    linesRemoved++;
                }
            }
            return linesRemoved;
        }

        static void PlaceCurrentFigure()
        {
            for (int figRow = 0; figRow < currentFigure.GetLength(0); figRow++)
            {
                for (int figCol = 0; figCol < currentFigure.GetLength(1); figCol++)
                {
                    var row = currentFigureRow - 1 + figRow;
                    var col = currentFigureCol - 1 + figCol;

                    if (currentFigure[figRow, figCol])
                    {
                        gameState[row, col] = true;

                    }
                }
            }
        }

        static bool CollisionDetection()
        {
            var currentFigureLowestRow = currentFigureRow + currentFigure.GetLength(0);

            if (currentFigureLowestRow > TetrisHeigh)
            {
                return true;
            }

            for (int figRow = 0; figRow < currentFigure.GetLength(0); figRow++)
            {
                for (int figCol = 0; figCol < currentFigure.GetLength(1); figCol++)
                {
                    var row = currentFigureRow + figRow;
                    var col = currentFigureCol - 1 + figCol;

                    if (row < 0)
                    {
                        continue;
                    }

                    if (gameState[row, col] == true && currentFigure[figRow, figCol] == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static void PrintGameField()
        {
            for (int row = 1; row <= TetrisHeigh; row++)
            {
                for (int col = 1; col <= TetrisWidth; col++)
                {
                    if (gameState[row - 1, col - 1] == true)
                    {
                        Print(row, col, '*');
                    }
                    else
                    {
                        Print(row, col, ' ');
                    }
                }
            }
        }

        static void PrintFigure(bool[,] figure, int row, int col)
        {
            for (int x = 0; x < figure.GetLength(0); x++)
            {
                for (int y = 0; y < figure.GetLength(1); y++)
                {
                    if (figure[x, y] == true)
                    {
                        Print(row + x, col + y, '*');
                    }
                }
            }
        }

        static void StartNewGame()
        {
            currentFigure = Figyres[random.Next(0, Figyres.Length)];
            nextFigure = Figyres[random.Next(0, Figyres.Length)];
        }

        static void PrintInfoPanel()
        {
            Print(1, TetrisWidth + 4, "Next:");
            for (int i = 2; i <= 5; i++)
            {
                Print(i, TetrisWidth + 2, "          ");
            }
            PrintFigure(nextFigure, 2, TetrisWidth + 5);

            Print(6, TetrisWidth + 4, "Score:");
            int scoreStartPosition = InfoPanelWidth / 2 - (Score.ToString().Length - 1) / 2;
            scoreStartPosition += TetrisWidth + 2;
            Print(7, scoreStartPosition - 1, Score);


            Print(9, TetrisWidth + 4, "Level:");
            Print(10, TetrisWidth + 6, Level);
            Print(12, TetrisWidth + 3, "Controls:");
            Print(13, TetrisWidth + 2, "    ^    ");
            Print(14, TetrisWidth + 2, "   < >   ");
            Print(15, TetrisWidth + 2, "    V  ");
            Print(16, TetrisWidth + 2, " space:");
        }

        static void PrintBorders()
        {
            for (int col = 0; col < GameWidth; col++)
            {
                Print(0, col, BorderCharacter);
                Print(GameHeight - 1, col, BorderCharacter);
            }

            for (int row = 0; row < GameHeight; row++)
            {
                Print(row, 0, BorderCharacter);
                Print(row, TetrisWidth + 1, BorderCharacter);
                Print(row, TetrisWidth + 1 + InfoPanelWidth + 1, BorderCharacter);

            }
        }

        static void Print(int row, int col, object data)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(col, row);
            Console.Write(data);
        }

        static void PlaySound()
        {

            Console.Beep(658, 125);
            Console.Beep(1320, 500);
            Console.Beep(990, 250);
            Console.Beep(1056, 250);
            Console.Beep(1188, 250);
            Console.Beep(1320, 125);
            Console.Beep(1188, 125);
            Console.Beep(1056, 250);
            Console.Beep(990, 250);
            Console.Beep(880, 500);
            Console.Beep(880, 250);
            Console.Beep(1056, 250);
            Console.Beep(1320, 500);
            Console.Beep(1188, 250);
            Console.Beep(1056, 250);
            Console.Beep(990, 750);
            Console.Beep(1056, 250);
            Console.Beep(1188, 500);
            Console.Beep(1320, 500);
            Console.Beep(1056, 500);
            Console.Beep(880, 500);
            Console.Beep(880, 500);
            Thread.Sleep(250);
            Console.Beep(1188, 500);
            Console.Beep(1408, 250);
            Console.Beep(1760, 500);
            Console.Beep(1584, 250);
            Console.Beep(1408, 250);
            Console.Beep(1320, 750);
            Console.Beep(1056, 250);
            Console.Beep(1320, 500);
            Console.Beep(1188, 250);
            Console.Beep(1056, 250);
            Console.Beep(990, 500);
            Console.Beep(990, 250);
            Console.Beep(1056, 250);
            Console.Beep(1188, 500);
            Console.Beep(1320, 500);
            Console.Beep(1056, 500);
            Console.Beep(880, 500);
            Console.Beep(880, 500);
            Thread.Sleep(500);
            Console.Beep(1320, 500);
            Console.Beep(990, 250);
            Console.Beep(1056, 250);
            Console.Beep(1188, 250);
            Console.Beep(1320, 125);
            Console.Beep(1188, 125);
            Console.Beep(1056, 250);
            Console.Beep(990, 250);
            Console.Beep(880, 500);
            Console.Beep(880, 250);
            Console.Beep(1056, 250);
            Console.Beep(1320, 500);
            Console.Beep(1188, 250);
            Console.Beep(1056, 250);
            Console.Beep(990, 750);
            Console.Beep(1056, 250);
            Console.Beep(1188, 500);
            Console.Beep(1320, 500);
            Console.Beep(1056, 500);
            Console.Beep(880, 500);
            Console.Beep(880, 500);
            Thread.Sleep(250);
            Console.Beep(1188, 500);
            Console.Beep(1408, 250);
            Console.Beep(1760, 500);
            Console.Beep(1584, 250);
            Console.Beep(1408, 250);
            Console.Beep(1320, 750);
            Console.Beep(1056, 250);
            Console.Beep(1320, 500);
            Console.Beep(1188, 250);
            Console.Beep(1056, 250);
            Console.Beep(990, 500);
            Console.Beep(990, 250);
            Console.Beep(1056, 250);
            Console.Beep(1188, 500);
            Console.Beep(1320, 500);
            Console.Beep(1056, 500);
            Console.Beep(880, 500);
            Console.Beep(880, 500);
            Thread.Sleep(500);
            Console.Beep(660, 1000);
            Console.Beep(528, 1000);
            Console.Beep(594, 1000);
            Console.Beep(495, 1000);
            Console.Beep(528, 1000);
            Console.Beep(440, 1000);
            Console.Beep(419, 1000);
            Console.Beep(495, 1000);
            Console.Beep(660, 1000);
            Console.Beep(528, 1000);
            Console.Beep(594, 1000);
            Console.Beep(495, 1000);
            Console.Beep(528, 500);
            Console.Beep(660, 500);
            Console.Beep(880, 1000);
            Console.Beep(838, 2000);
            Console.Beep(660, 1000);
            Console.Beep(528, 1000);
            Console.Beep(594, 1000);
            Console.Beep(495, 1000);
            Console.Beep(528, 1000);
            Console.Beep(440, 1000);
            Console.Beep(419, 1000);
            Console.Beep(495, 1000);
            Console.Beep(660, 1000);
            Console.Beep(528, 1000);
            Console.Beep(594, 1000);
            Console.Beep(495, 1000);
            Console.Beep(528, 500);
            Console.Beep(660, 500);
            Console.Beep(880, 1000);
            Console.Beep(838, 2000);
        }
    }
}
