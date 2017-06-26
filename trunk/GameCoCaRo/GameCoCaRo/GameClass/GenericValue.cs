using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCoCaRo.GameClass
{
     static class GenericValue
    {
        public const int CELL_WIDTH = 25;
        public const int CELL_HEIGHT = 25;
        public const int BOARD_COL = 33;
        public const int BOARD_ROW = 25;
        public static Pen PEN = new Pen(Color.Black);
        public static SolidBrush sbWhite = new SolidBrush(Color.White);
        public static SolidBrush sbBlack = new SolidBrush(Color.Black);
        public static SolidBrush sbBack;
        public static SolidBrush getSbBack(Color backColor)
        {
            if(sbBack == null)
            {
                sbBack = new SolidBrush(backColor);
            }
            return sbBack;
        }
    }
}
