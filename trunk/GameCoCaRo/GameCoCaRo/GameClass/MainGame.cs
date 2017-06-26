using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCoCaRo.GameClass
{
    enum Result
    {
        NORESULT,
        FAIR,
        PLAYER1_WIN,
        PLAYER2_WIN,
        COM_WIN
    }
    class MainGame
    {
        private const int NO_TURN = 0;
        private const int BLACK_TURN = 1;
        private const int WHITE_TURN = 2;
        private const int MODE_PVP = 1;
        private const int MODE_PVC = 2;
        private ChessBoard _cBoard;
        private ChessCell[,] _ChessArray;
        private Stack<ChessCell> _stkGoneChess;
        private Stack<ChessCell> _stkUndoChess;
        private int _turn;
        private bool _ready;
        private Result _result;
        private int _mode;
        public int Mode() { return _mode; }
        public MainGame()
        {
            _cBoard = new ChessBoard();
            _ChessArray = new ChessCell[_cBoard.Row, _cBoard.Column];
            initChessArray();
            _stkGoneChess = new Stack<ChessCell>();
            _stkUndoChess = new Stack<ChessCell>();
            _turn = BLACK_TURN;
            _ready = false;
            _result = Result.NORESULT;
            _mode = MODE_PVP;
        }
        public void DrawChessBoard(Graphics g)
        {
            _cBoard.DrawBoard(g);
        }
        public void initChessArray()
        {
            for (int i = 0; i < _cBoard.Row; i++)
            {
                for (int j = 0; j < _cBoard.Column; j++)
                {
                    _ChessArray[i, j] = new ChessCell(i, j, 0);
                }
            }
        }
        //public void drawChessCell(ChessCell chess, Graphics g)
        //{
        //    if (chess.BelongTo == BLACK_TURN)
        //    {
        //        _cBoard.DrawChess(g, chess.Position, GenericValue.sbBlack);
        //        _turn = WHITE_TURN;
        //    }
        //    else if (chess.BelongTo == WHITE_TURN)
        //    {
        //        _cBoard.DrawChess(g, chess.Position, GenericValue.sbWhite);
        //        _turn = BLACK_TURN;
        //    }
        //}
        private void changeTurn()
        {
            if (_turn == BLACK_TURN)
                _turn = WHITE_TURN;
            else
                _turn = BLACK_TURN;
        }
        public bool goChess(int mouseX, int mouseY, Graphics g)
        {
            if (!_ready)
            {
                return false;
            }
            if(mouseX % GenericValue.CELL_WIDTH == 0 || mouseY % GenericValue.CELL_HEIGHT == 0)
            {
                return false;
            }
            int col = mouseX / GenericValue.CELL_WIDTH;
            int row = mouseY / GenericValue.CELL_HEIGHT;
            if (_ChessArray[row, col].BelongTo != 0)
            {
                return false;
            }
            _ChessArray[row, col].BelongTo = _turn;
            _ChessArray[row, col].Draw(g);
            ChessCell cell = _ChessArray[row, col];
            _stkUndoChess.Clear();
            _stkGoneChess.Push(new ChessCell(cell.RowIdx, cell.ColIdx, cell.BelongTo));
            changeTurn();
            return true;
        }
        public void redrawChessArr(Graphics g)
        {
            foreach(ChessCell chess in _ChessArray)
            {
                chess.Draw(g);
            }
        }
        
        public void undo(Graphics g, Color backColor)
        {
            if(_stkGoneChess.Count > 0)
            {
                ChessCell undoCell = _stkGoneChess.Pop();
                _stkUndoChess.Push(undoCell);
                _ChessArray[undoCell.RowIdx, undoCell.ColIdx].BelongTo = NO_TURN;
                _turn = undoCell.BelongTo;
            }
        }

        public void redo(Graphics g)
        {
            if(_stkUndoChess.Count > 0)
            {
                ChessCell redoCell = _stkUndoChess.Pop();
                _stkGoneChess.Push(redoCell);
                _ChessArray[redoCell.RowIdx, redoCell.ColIdx].BelongTo = redoCell.BelongTo;
                redoCell.Draw(g);
                changeTurn();
            }
        }

        public void startNewPlayerVsPlayer()
        {
            _mode = MODE_PVP;
            _ready = true;
            initChessArray();
            _stkGoneChess.Clear();
            _stkUndoChess.Clear();
            _turn = BLACK_TURN;
        }
        public void startNewPlayerVsCom(Graphics g)
        {
            _mode = MODE_PVC;
            _ready = true;
            initChessArray();
            _stkGoneChess.Clear();
            _stkUndoChess.Clear();
            _turn = WHITE_TURN;
            //runComputer(g);
        }

        public void endGame()
        {
            switch (_result)
            {
                case Result.FAIR:
                    MessageBox.Show("Hòa cờ!");
                    break;
                case Result.PLAYER1_WIN:
                    MessageBox.Show("Người chơi 1 thắng!");
                    break;
                case Result.PLAYER2_WIN:
                    MessageBox.Show("Người chơi 2 thắng!");
                    break;
                case Result.COM_WIN:
                    MessageBox.Show("Máy thắng!");
                    break;
                default:
                    MessageBox.Show("Lỗi!");
                    break;
            }
            _ready = false;
        }
        public bool checkWinner()
        {
            if(_stkGoneChess.Count == _cBoard.Column * _cBoard.Row)
            {
                _result = Result.FAIR;
                endGame();
                return true;
            }
            foreach  (ChessCell cell in _stkGoneChess)
            {
                if (checkVertical(cell) || checkHorizontal(cell) || checkFirstDiagonal(cell) || checkSecondDiagonal(cell))
                {
                    _result = cell.BelongTo == BLACK_TURN ? Result.PLAYER1_WIN : Result.PLAYER2_WIN;
                    endGame();
                    return true;
                }
            }
            return false;
        }
        public bool checkVertical(ChessCell cell)
        {
            if (cell.RowIdx > _cBoard.Row - 5)
                return false;
            for (int i = 1; i < 5; i++)
            {
                if (_ChessArray[cell.RowIdx + i, cell.ColIdx].BelongTo != cell.BelongTo)
                    return false;
            }
            if (cell.RowIdx == 0 || cell.RowIdx + 5 == _cBoard.Row)
                return true;
            if (_ChessArray[cell.RowIdx - 1, cell.ColIdx].BelongTo == 0 || _ChessArray[cell.RowIdx + 5, cell.ColIdx].BelongTo == 0)
                return true;

            return false;
        }

        public bool checkHorizontal(ChessCell cell)
        {
            if (cell.ColIdx > _cBoard.Column - 5)
                return false;
            for (int i = 1; i < 5; i++)
            {
                if (_ChessArray[cell.RowIdx, cell.ColIdx + i].BelongTo != cell.BelongTo)
                    return false;
            }
            if (cell.ColIdx == 0 || cell.ColIdx + 5 == _cBoard.Column)
                return true;
            if (_ChessArray[cell.RowIdx, cell.ColIdx - 1].BelongTo == 0 || _ChessArray[cell.RowIdx, cell.ColIdx + 5].BelongTo == 0)
                return true;
            return false;
        }

        public bool checkFirstDiagonal(ChessCell cell)
        {
            if (cell.RowIdx > _cBoard.Row - 5 || cell.ColIdx > _cBoard.Column - 5)
                return false;
            for (int i = 1; i < 5; i++)
            {
                if (_ChessArray[cell.RowIdx + i, cell.ColIdx + i].BelongTo != cell.BelongTo)
                    return false;
            }
            if (cell.RowIdx == 0 || cell.RowIdx + 5 == _cBoard.Row || cell.ColIdx == 0 || cell.ColIdx + 5 == _cBoard.Column)
                return true;
            if (_ChessArray[cell.RowIdx -1, cell.ColIdx - 1].BelongTo == 0 || _ChessArray[cell.RowIdx + 5, cell.ColIdx + 5].BelongTo == 0)
                return true;
            return false;
        }

        public bool checkSecondDiagonal(ChessCell cell)
        {
            if (cell.RowIdx < 4 || cell.ColIdx > _cBoard.Column - 5)
                return false;
            for (int i = 1; i < 5; i++)
            {
                if (_ChessArray[cell.RowIdx - i, cell.ColIdx + i].BelongTo != cell.BelongTo)
                    return false;
            }
            if (cell.RowIdx == _cBoard.Row - 1 || cell.RowIdx == 4 || cell.ColIdx == 0 || cell.ColIdx + 5 == _cBoard.Column)
                return true;
            if (_ChessArray[cell.RowIdx + 1, cell.ColIdx - 1].BelongTo == 0 || _ChessArray[cell.RowIdx - 5, cell.ColIdx + 5].BelongTo == 0)
                return true;
            return false;
        }

        #region AI_Computer
        public long[] Defend_Score = new long[7] { 0, 3, 27, 99, 729, 6561, 59049 };//Mảng điểm chặn
        public long[] Attack_Score = new long[7] { 0, 6, 54, 162, 1458, 13112, 118008 };//Mảng điểm tấn công
        //public long[] AScore = new long[5] { 0, 3, 24, 243, 2197 };
        //public long[] Attack_Score = new long[5] { 0, 1, 9, 81, 729 };
        public void goAChess(ChessCell chess, Graphics g)
        {
            chess.Draw(g);
            _ChessArray[chess.RowIdx, chess.ColIdx].BelongTo = chess.BelongTo;
            _stkGoneChess.Push(chess);
            changeTurn();
        }
        public void runComputer(Graphics g)
        {
            ChessCell chess = null;
            if (_stkGoneChess.Count == 0)
                chess = new ChessCell(_cBoard.Column / 2, _cBoard.Row / 2, BLACK_TURN);
            else
                chess = AIFindCell();
            if (chess != null)
                chess.BelongTo = BLACK_TURN; 
                goAChess(chess, g);
        }
        public ChessCell AIFindCell()
        {
            ChessCell ChessCellTemp = new ChessCell();
            long iScore = 0;
            for (int i = 0; i < _cBoard.Row; i++)
            {
                for (int j = 0; j < _cBoard.Column; j++)
                {
                    long iScoreAttack = 0;
                    long iScoreDefend = 0;
                    if (_ChessArray[i, j].BelongTo == 0)
                    {
                        iScoreAttack += Score_Attack_Duyet_Doc(i, j);
                        iScoreAttack += Score_Attack_Duyet_Ngang(i, j);
                        iScoreAttack += Score_Attack_Duyet_Cheo_Nguoc(i, j);
                        iScoreAttack += Score_Attack_Duyet_Cheo_Xuoi(i, j);

                        iScoreDefend += Score_Defend_Duyet_Doc(i, j);
                        iScoreDefend += Score_Defend_Duyet_Ngang(i, j);
                        iScoreDefend += Score_Defend_Duyet_Cheo_Nguoc(i, j);
                        iScoreDefend += Score_Defend_Duyet_Cheo_Xuoi(i, j);

                        if (iScoreDefend <= iScoreAttack)
                        {
                            if (iScore <= iScoreAttack)
                            {
                                iScore = iScoreAttack;
                                ChessCellTemp = _ChessArray[i, j];
                            }
                        }
                        else
                        {
                            if (iScore <= iScoreDefend)
                            {
                                iScore = iScoreDefend;
                                ChessCellTemp = _ChessArray[i, j];
                            }
                        }
                    }
                }
            }
            return ChessCellTemp;
        }
        #region Attack
        private long Score_Attack_Duyet_Doc(long Dong, long Cot)
        {
            long iScoreTempDoc = 0;
            long iScoreAttack = 0;
            int iSoQuanTa = 0;
            int iSoQuanDich = 0;
            for (int iDem = 1; iDem < 6 && Dong + iDem < _cBoard.Row; iDem++)
            {
                if (_ChessArray[Dong + iDem, Cot].BelongTo == 1)
                    iSoQuanTa++;
                if (_ChessArray[Dong + iDem, Cot].BelongTo == 2)
                {
                    iSoQuanDich++;
                    iScoreTempDoc -= 9;
                    break;
                }
                if (_ChessArray[Dong + iDem, Cot].BelongTo == 0)
                    break;
            }
            //iScoreAttack += Attack_Score[iSoQuanTa];
            //iSoQuanTa = 0;

            for (int iDem = 1; iDem < 6 && Dong - iDem >= 0; iDem++)
            {
                if (_ChessArray[Dong - iDem, Cot].BelongTo == 1)
                    iSoQuanTa++;
                if (_ChessArray[Dong - iDem, Cot].BelongTo == 2)
                {
                    iScoreTempDoc -= 9;
                    iSoQuanDich++;
                    break;
                }
                if (_ChessArray[Dong - iDem, Cot].BelongTo == 0)
                    break;
            }
            if (iSoQuanDich == 2)
                return 0;
            iScoreAttack += Attack_Score[iSoQuanTa];
            iScoreAttack -= Attack_Score[iSoQuanDich];
            iScoreTempDoc += iScoreAttack;

            return iScoreTempDoc;
        }
        private long Score_Attack_Duyet_Ngang(long Dong, long Cot)
        {
            long iScoreTempNgang = 0;
            long iScoreAttack = 0;
            int iSoQuanTa = 0;
            int iSoQuanDich = 0;
            for (int iDem = 1; iDem < 6 && Cot + iDem < _cBoard.Column; iDem++)
            {
                if (_ChessArray[Dong, Cot + iDem].BelongTo == 1)
                    iSoQuanTa++;
                if (_ChessArray[Dong, Cot + iDem].BelongTo == 2)
                {
                    iSoQuanDich++;
                    iScoreTempNgang -= 9;
                    break;
                }
                if (_ChessArray[Dong, Cot + iDem].BelongTo == 0)
                    break;
            }
            //iScoreAttack += Attack_Score[iSoQuanTa];
            //iSoQuanTa = 0;

            for (int iDem = 1; iDem < 6 && Cot - iDem >= 0; iDem++)
            {
                if (_ChessArray[Dong, Cot - iDem].BelongTo == 1)
                    iSoQuanTa++;
                if (_ChessArray[Dong, Cot - iDem].BelongTo == 2)
                {
                    iScoreTempNgang -= 9;
                    iSoQuanDich++;
                    break;
                }
                if (_ChessArray[Dong, Cot - iDem].BelongTo == 0)
                    break;
            }
            if (iSoQuanDich == 2)
                return 0;
            iScoreAttack += Attack_Score[iSoQuanTa];
            iScoreAttack -= Attack_Score[iSoQuanDich];
            iScoreTempNgang += iScoreAttack;

            return iScoreTempNgang;
        }
        private long Score_Attack_Duyet_Cheo_Nguoc(long Dong, long Cot)
        {
            long iScoreTempCheoNguoc = 0;
            long iScoreAttack = 0;
            int iSoQuanTa = 0;
            int iSoQuanDich = 0;
            for (int iDem = 1; iDem < 6 && Cot + iDem < _cBoard.Column && Dong + iDem < _cBoard.Row; iDem++)
            {
                if (_ChessArray[Dong + iDem, Cot + iDem].BelongTo == 1)
                    iSoQuanTa++;
                if (_ChessArray[Dong + iDem, Cot + iDem].BelongTo == 2)
                {
                    iScoreTempCheoNguoc -= 9;
                    iSoQuanDich++;
                    break;
                }
                if (_ChessArray[Dong + iDem, Cot + iDem].BelongTo == 0)
                    break;
            }
            //iScoreAttack += Attack_Score[iSoQuanTa];
            //iSoQuanTa = 0;

            for (int iDem = 1; iDem < 6 && Cot - iDem >= 0 && Dong - iDem >= 0; iDem++)
            {
                if (_ChessArray[Dong - iDem, Cot - iDem].BelongTo == 1)
                    iSoQuanTa++;
                if (_ChessArray[Dong - iDem, Cot - iDem].BelongTo == 2)
                {
                    iScoreTempCheoNguoc -= 9;
                    iSoQuanDich++;
                    break;
                }
                if (_ChessArray[Dong - iDem, Cot - iDem].BelongTo == 0)
                    break;
            }
            if (iSoQuanDich == 2)
                return 0;
            iScoreAttack += Attack_Score[iSoQuanTa];
            iScoreAttack -= Attack_Score[iSoQuanDich];
            iScoreTempCheoNguoc += iScoreAttack;

            return iScoreTempCheoNguoc;
        }
        private long Score_Attack_Duyet_Cheo_Xuoi(long Dong, long Cot)
        {
            long iScoreTempCheoXuoi = 0;
            long iScoreAttack = 0;
            int iSoQuanTa = 0;
            int iSoQuanDich = 0;
            for (int iDem = 1; iDem < 6 && Cot - iDem >= 0 && Dong + iDem < _cBoard.Row; iDem++)
            {
                if (_ChessArray[Dong + iDem, Cot - iDem].BelongTo == 1)
                    iSoQuanTa++;
                if (_ChessArray[Dong + iDem, Cot - iDem].BelongTo == 2)
                {
                    iScoreTempCheoXuoi -= 9;
                    iSoQuanDich++;
                    break;
                }
                if (_ChessArray[Dong + iDem, Cot - iDem].BelongTo == 0)
                    break;
            }
            //iScoreAttack += Attack_Score[iSoQuanTa];
            //iSoQuanTa = 0;

            for (int iDem = 1; iDem < 6 && Cot + iDem < _cBoard.Column && Dong - iDem >= 0; iDem++)
            {
                if (_ChessArray[Dong - iDem, Cot + iDem].BelongTo == 1)
                    iSoQuanTa++;
                if (_ChessArray[Dong - iDem, Cot + iDem].BelongTo == 2)
                {
                    iScoreTempCheoXuoi -= 9;
                    iSoQuanDich++;
                    break;
                }
                if (_ChessArray[Dong - iDem, Cot + iDem].BelongTo == 0)
                    break;
            }
            if (iSoQuanDich == 2)
                return 0;
            iScoreAttack += Attack_Score[iSoQuanTa];
            iScoreAttack -= Attack_Score[iSoQuanDich];
            iScoreTempCheoXuoi += iScoreAttack;

            return iScoreTempCheoXuoi;
        }
        #endregion
        
        // Tính điểm phòng ngự
        #region Defense

        private long Score_Defend_Duyet_Doc(long Dong, long Cot)
        {
            long iScoreTempDoc = 0;
            long iScoreDefend = 0;
            int iSoQuanDich = 0;
            int iSoQuanTa = 0;
            for (int iDem = 1; iDem < 6 && Dong + iDem < _cBoard.Row; iDem++)
            {
                if (_ChessArray[Dong + iDem, Cot].BelongTo == 1)
                {
                    iSoQuanTa++;
                    break;
                }
                if (_ChessArray[Dong + iDem, Cot].BelongTo == 2)
                    iSoQuanDich++;
                if (_ChessArray[Dong + iDem, Cot].BelongTo == 0)
                    break;
            }
            //iScoreDefend += Defend_Score[iSoQuanDich];
            //iSoQuanDich = 0;

            for (int iDem = 1; iDem < 6 && Dong - iDem >= 0; iDem++)
            {
                if (_ChessArray[Dong - iDem, Cot].BelongTo == 1)
                {
                    iSoQuanTa++;
                    break;
                }
                if (_ChessArray[Dong - iDem, Cot].BelongTo == 2)
                    iSoQuanDich++;
                if (_ChessArray[Dong - iDem, Cot].BelongTo == 0)
                    break;
            }
            if (iSoQuanTa == 2)
                return 0;

            iScoreDefend += Defend_Score[iSoQuanDich];
            if (iSoQuanDich > 0)
                iScoreDefend -= Attack_Score[iSoQuanTa] * 2;
            iScoreTempDoc += iScoreDefend;

            return iScoreTempDoc;
        }
        private long Score_Defend_Duyet_Ngang(long Dong, long Cot)
        {
            long iScoreTempNgang = 0;
            long iScoreDefend = 0;
            int iSoQuanDich = 0;
            int iSoQuanTa = 0;
            for (int iDem = 1; iDem < 6 && Cot + iDem < _cBoard.Column; iDem++)
            {
                if (_ChessArray[Dong, Cot + iDem].BelongTo == 1)
                {
                    iSoQuanTa++;
                    break;
                }
                if (_ChessArray[Dong, Cot + iDem].BelongTo == 2)
                    iSoQuanDich++;
                if (_ChessArray[Dong, Cot + iDem].BelongTo == 0)
                    break;
            }
            //iScoreDefend += Defend_Score[iSoQuanDich];
            //iSoQuanDich = 0;

            for (int iDem = 1; iDem < 6 && Cot - iDem >= 0; iDem++)
            {
                if (_ChessArray[Dong, Cot - iDem].BelongTo == 1)
                {
                    iSoQuanTa++;
                    break;
                }
                if (_ChessArray[Dong, Cot - iDem].BelongTo == 0)
                    break;
                if (_ChessArray[Dong, Cot - iDem].BelongTo == 2)
                    iSoQuanDich++;
            }
            if (iSoQuanTa == 2)
                return 0;
            iScoreDefend += Defend_Score[iSoQuanDich];
            if (iSoQuanDich > 0)
                iScoreDefend -= Attack_Score[iSoQuanTa] * 2;
            iScoreTempNgang += iScoreDefend;

            return iScoreTempNgang;
        }
        private long Score_Defend_Duyet_Cheo_Nguoc(long Dong, long Cot)
        {
            long iScoreTempCheoNguoc = 0;
            long iScoreDefend = 0;
            int iSoQuanDich = 0;
            int iSoQuanTa = 0;
            for (int iDem = 1; iDem < 6 && Cot + iDem < _cBoard.Column && Dong + iDem < _cBoard.Row; iDem++)
            {
                if (_ChessArray[Dong + iDem, Cot + iDem].BelongTo == 1)
                {
                    iSoQuanTa++;
                    break;
                }
                if (_ChessArray[Dong + iDem, Cot + iDem].BelongTo == 0)
                    break;
                if (_ChessArray[Dong + iDem, Cot + iDem].BelongTo == 2)
                    iSoQuanDich++;
            }
            //iScoreDefend += Defend_Score[iSoQuanDich];
            //iSoQuanDich = 0;

            for (int iDem = 1; iDem < 6 && Cot - iDem >= 0 && Dong - iDem >= 0; iDem++)
            {
                if (_ChessArray[Dong - iDem, Cot - iDem].BelongTo == 1)
                {
                    iSoQuanTa++;
                    break;
                }

                if (_ChessArray[Dong - iDem, Cot - iDem].BelongTo == 0)
                    break;
                if (_ChessArray[Dong - iDem, Cot - iDem].BelongTo == 2)
                    iSoQuanDich++;
            }
            if (iSoQuanTa == 2)
                return 0;
            iScoreDefend += Defend_Score[iSoQuanDich];
            if (iSoQuanDich > 0)
                iScoreDefend -= Attack_Score[iSoQuanTa] * 2;
            iScoreTempCheoNguoc += iScoreDefend;

            return iScoreTempCheoNguoc;
        }

        private long Score_Defend_Duyet_Cheo_Xuoi(long Dong, long Cot)
        {
            long iScoreTempCheoXuoi = 0;
            long iScoreDefend = 0;
            int iSoQuanDich = 0;
            int iSoQuanTa = 0;
            for (int iDem = 1; iDem < 6 && Cot - iDem >= 0 && Dong + iDem < _cBoard.Row; iDem++)
            {
                if (_ChessArray[Dong + iDem, Cot - iDem].BelongTo == 1)
                {
                    iSoQuanTa++;
                    break;
                }
                if (_ChessArray[Dong + iDem, Cot - iDem].BelongTo == 0)
                    break;
                if (_ChessArray[Dong + iDem, Cot - iDem].BelongTo == 2)
                    iSoQuanDich++;
            }
            //iScoreDefend += Defend_Score[iSoQuanDich];
            //iSoQuanDich = 0;

            for (int iDem = 1; iDem < 6 && Cot + iDem < _cBoard.Column && Dong - iDem >= 0; iDem++)
            {
                if (_ChessArray[Dong - iDem, Cot + iDem].BelongTo == 1)
                {
                    iSoQuanTa++;
                    break;
                }
                if (_ChessArray[Dong - iDem, Cot + iDem].BelongTo == 0)
                    break;
                if (_ChessArray[Dong - iDem, Cot + iDem].BelongTo == 2)
                    iSoQuanDich++;
            }
            if (iSoQuanTa == 2)
            {
                return 0;
            }
            iScoreDefend += Defend_Score[iSoQuanDich];
            if (iSoQuanDich > 0)
                iScoreDefend -= Attack_Score[iSoQuanTa] * 2;
            iScoreTempCheoXuoi += iScoreDefend;

            return iScoreTempCheoXuoi;
        }
        #endregion
        #endregion
    }
}
