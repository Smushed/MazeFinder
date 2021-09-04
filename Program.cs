using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MazeFinder
{
    class Program
    {
        static int R; //Number of rows in the maze
        static int C; //Number of columns in the maze

        static void Main(string[] args)
        {
            int x;
            Console.WriteLine("Enter a value 1-8 for which maze to solve:");
            if (int.TryParse(Console.ReadLine(), out x))
            {
                string fileName = $".\\tests\\test{x}.txt";
                List<string> mazeStrings;
                try
                {
                    mazeStrings = File.ReadAllLines(fileName).ToList();
                }
                catch (FileNotFoundException exception)
                {
                    throw new FileNotFoundException("Test file for the number you selected doesn't exist.", exception);
                }

                int[,] maze = ReadLines(mazeStrings);

                var solver = new MazeSolver();
                solver.FindPath(maze);
            }
            else 
            { 
                Console.WriteLine("Please start the program again and input a valid integer!");
            };
        }

        static int[,] ReadLines(List<string> mazeStrings)
        {
            R = mazeStrings.Count;
            C = mazeStrings[0].Length;

            var maze = new int[R,C];
            for (var i = 0; i < mazeStrings.Count; i++)
            {
                for(var ii = 0; ii < mazeStrings[i].Length; ii++)
                {
                    maze[i, ii] = (int)Char.GetNumericValue(mazeStrings[i][ii]);
                }
            }
            
            return maze;
        }

        class MazeSolver
        {
            public void FindPath(int[,] maze)
            {
                var solution = new int[R, C];

                //Need to find the entry point for the maze
                int startCol = SearchForEntry(maze);

                var path = new StringBuilder();
                //Feeding 0 to start at the top row to start moving from there
                if (Navigate(maze, 0, startCol, solution, path) == true) //Returning true or false to see if a soltion was found
                {
                    Console.WriteLine("Solved");
                    Console.WriteLine(path);
                }
                else
                {
                    Console.WriteLine("No Solution Found");
                }
            }

            int SearchForEntry(int[,] maze)
            {
                for (int i = 0; i < C; i++)
                {
                    if (maze[0, i] == 1) return i; 
                }
                return 0;
            }

            bool Navigate(int[,]maze, int x, int y, int[,] solution, StringBuilder path)
            {
                // If X is at the last row and the value for the maze is 1 then we have found the path
                if (x == R - 1 && maze[x, y] == 1)
                {
                    path.Append($"({x},{y})");
                    solution[x, y] = 1;
                    return true;
                }

                if (CanMove(maze, x, y) == true)
                {
                    // If we backtracked exit
                    if (solution[x, y] == 1) return false;

                    // Mark x, y as part of solution path
                    solution[x, y] = 1;
                    path.Append($"({x},{y}) ");

                    //Move in cardinal directions looking for the next place to go
                    if (Navigate(maze, x, y + 1, solution, path)) return true;
                    if (Navigate(maze, x, y - 1, solution, path)) return true;
                    if (Navigate(maze, x + 1, y, solution, path)) return true;
                    if (Navigate(maze, x - 1, y, solution, path)) return true;

                    // If none of the above movements works then we've hit a dead end and unmark x, y  as part of the solution
                    solution[x, y] = 0;
                    path.Remove(path.Length-6, 6); //Each part of the solution is 6 characters. Remove them from the path string if we went the wrong way
                    return false;
                }
                return false;
            }

            bool CanMove(int[,] maze, int x, int y)
            {
                //Checks if x & y are in bounds and that the next step is open (is 1)
                return (x >= 0 && x < R && 
                        y >= 0 && y < C && 
                        maze[x, y] == 1);
            }
        }

    }
}
