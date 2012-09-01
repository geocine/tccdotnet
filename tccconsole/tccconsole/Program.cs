using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace tccconsole
{
    class Program
    {
        static void Main(string[] args)
        {

            ConsoleColor.SetForeGroundColour(ConsoleColor.ForeGroundColour.Green, true);
            Console.WriteLine("tccconsole: Scanner and Lexical Analyer for C");
            ConsoleColor.SetForeGroundColour();
            Console.Write("Please input filename: ");
            string filename = Console.ReadLine();
            if (String.IsNullOrEmpty(filename))
                Environment.Exit(1);
            TestFromFile(filename);
            //TestFromFile("input.txt");
            Console.ReadKey(true);

        }

        public static void TestFromFile(string filename)
        {
            StreamWriter fstream = new StreamWriter("tokens.txt", false);

            StreamReader reader = null;
            reader = new StreamReader(filename);

            Scanner scanner = new Scanner();

            string input = reader.ReadToEnd();
            scanner.Init(input);
            Token tok = new Token();
            do
            {
                //tok = scanner.Scan(scanner.Tokens.ToArray());
                tok = scanner.Scan();
                //Here yay
                if (tok.Type != TokenType.WHITESPACE && tok.Type != TokenType.COMMENTLINE && tok.Type!= TokenType.COMMENTBLOCK)

                {
                    ConsoleColor.SetForeGroundColour(ConsoleColor.ForeGroundColour.Green, true);
                    if (tok.Type == TokenType.NULL)
                    {
                        Console.WriteLine("ERROR: Unknown Token found at Line: {2,3} \n", tok.Text.Trim(), "Unknown", tok.LinePos);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Token: {0} \n Type: {1,-12} Line: {2,3} \n", tok.Text.Trim(), tok.Type, tok.LinePos);
                        fstream.WriteLine("Token: {0} \r\n Type: {1,-12} Line: {2,3} \r\n", tok.Text.Trim(), tok.Type, tok.LinePos);
                    }

                    ConsoleColor.SetForeGroundColour();
                }
            } while (input.Length != tok.EndPos);
           
            //Closing the streamwriter fixes the bug
            fstream.Close();

        }

    }
}
