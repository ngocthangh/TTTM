using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCoCaRo.GameClass
{
    class ChessBoard
    {
        private int _column;
        private int _row;
        
        public int Column
        {
            get
            {
                return _column;
            }

            set
            {
                _column = value;
            }
        }

        public int Row
        {
            get
            {
                return _row;
            }

            set
            {
                _row = value;
            }
        }
        public ChessBoard()
        {
            _column = GenericValue.BOARD_COL;
            _row = GenericValue.BOARD_ROW;
        }
        public void DrawBoard(Graphics g)
        {
            for (int i = 0; i <= GenericValue.BOARD_COL; i++)
            {
                g.DrawLine(GenericValue.PEN, i * GenericValue.CELL_WIDTH, 0, i * GenericValue.CELL_WIDTH, GenericValue.CELL_HEIGHT * GenericValue.BOARD_ROW);
            }
            for (int i = 0; i <= GenericValue.BOARD_ROW; i++)
            {
                g.DrawLine(GenericValue.PEN, 0, i * GenericValue.CELL_HEIGHT, GenericValue.CELL_WIDTH * GenericValue.BOARD_COL, i * GenericValue.CELL_HEIGHT);
            }
        }
        //public void DrawChess(Graphics g, Point pos, SolidBrush sb)
        //{
            
        //}
        //public void DeleteChess(Graphics g, Point pos, SolidBrush sb)
        //{
            
        //}
    }
}
