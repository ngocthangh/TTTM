using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCoCaRo.GameClass
{
    class ChessCell
    {
        private const int BLACK_TURN = 1;
        private const int WHITE_TURN = 2;
        private int _width;
        private int _height;
        private int _rowIdx;
        private int _colIdx;
        private int _belongTo;
        private Point _position;
        public int Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
            }
        }
        
        public int Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
            }
        }

        public int RowIdx
        {
            get
            {
                return _rowIdx;
            }

            set
            {
                _rowIdx = value;
            }
        }

        public int ColIdx
        {
            get
            {
                return _colIdx;
            }

            set
            {
                _colIdx = value;
            }
        }

        public int BelongTo
        {
            get
            {
                return _belongTo;
            }

            set
            {
                _belongTo = value;
            }
        }

        public Point Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }
        public ChessCell()
        {
            _width = GenericValue.CELL_WIDTH;
            _height = GenericValue.CELL_HEIGHT;
        }
        public ChessCell(int row, int col, int belong)
        {
            _rowIdx = row;
            _colIdx = col;
            _position = new Point(col * GenericValue.CELL_WIDTH, row * GenericValue.CELL_HEIGHT);
            _belongTo = belong;
        }
        public void Draw(Graphics g)
        {
            if (_belongTo == BLACK_TURN)
            {
                g.FillEllipse(GenericValue.sbBlack, _position.X + 2, _position.Y + 2, GenericValue.CELL_WIDTH - 4, GenericValue.CELL_HEIGHT - 4);
            }
            else if (_belongTo == WHITE_TURN)
            {
                g.FillEllipse(GenericValue.sbWhite, _position.X + 2, _position.Y + 2, GenericValue.CELL_WIDTH - 4, GenericValue.CELL_HEIGHT - 4);
            }
            
        }
        public void Delete(Graphics g, Color backColor)
        {
            Bitmap bitmap = new Bitmap(GenericValue.CELL_WIDTH - 2, GenericValue.CELL_HEIGHT - 2, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bitmap.MakeTransparent();
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new Rectangle(_position.X + 1, _position.Y + 1, GenericValue.CELL_WIDTH - 2, GenericValue.CELL_HEIGHT - 2));
            g.SetClip(path);
            g.Clear(Color.Transparent);
            //g.FillRectangle(GenericValue.getSbBack(backColor), _position.X + 1, _position.Y + 1, GenericValue.CELL_WIDTH - 2, GenericValue.CELL_HEIGHT - 2);
            g.ResetClip();
        }
    }
}
