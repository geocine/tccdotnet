

using System;
using System.Runtime.InteropServices;

namespace tccconsole
{


    public class ConsoleColor
    {
        

        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;
        const int STD_ERROR_HANDLE = -12;

        [DllImportAttribute("Kernel32.dll")]
        private static extern IntPtr GetStdHandle
        (
            int nStdHandle 

        );

        [DllImportAttribute("Kernel32.dll")]
        private static extern bool SetConsoleTextAttribute
        (
            IntPtr hConsoleOutput, 

            int wAttributes    

        );

        

        [Flags]
        public enum ForeGroundColour
        {
            Black = 0x0000,
            Blue = 0x0001,
            Green = 0x0002,
            Cyan = 0x0003,
            Red = 0x0004,
            Magenta = 0x0005,
            Yellow = 0x0006,
            Grey = 0x0007,
            White = 0x0008
        }

        

        

        private ConsoleColor()
        {
        }

        public static bool SetForeGroundColour()
        {
            

            return SetForeGroundColour(ForeGroundColour.Grey);
        }

        public static bool SetForeGroundColour(
            ForeGroundColour foreGroundColour)
        {
            

            return SetForeGroundColour(foreGroundColour, true);
        }

        public static bool SetForeGroundColour(
            ForeGroundColour foreGroundColour,
            bool brightColours)
        {
            

            IntPtr nConsole = GetStdHandle(STD_OUTPUT_HANDLE);
            int colourMap;

            

            if (brightColours)
                colourMap = (int)foreGroundColour |
                    (int)ForeGroundColour.White;
            else
                colourMap = (int)foreGroundColour;

            return SetConsoleTextAttribute(nConsole, colourMap);
        }
    }

}
