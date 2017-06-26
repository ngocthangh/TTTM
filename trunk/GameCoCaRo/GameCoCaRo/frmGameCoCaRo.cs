using GameCoCaRo.GameClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCoCaRo
{
    public partial class frmGameCoCaRo : Form
    {
        private const int MODE_PVP = 1;
        private const int MODE_PVC = 2;
        private MainGame game;
        private Graphics g;
        public frmGameCoCaRo()
        {
            InitializeComponent();
            game = new MainGame();
            g = pnlChessBoard.CreateGraphics();
        }

        private void frmGameCoCaRo_Load(object sender, EventArgs e)
        {
            lblChuChay.Text = "Chào mừng các bạn đến với chương trình\ncờ caro của chúng tôi, Chúc các bạn\nchơi vui vẻ.";
        }

        private void pnlChessBoard_Paint(object sender, PaintEventArgs e)
        {
            game.DrawChessBoard(g);
            game.redrawChessArr(g);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Do you want to exit?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
                this.Dispose();
            }
        }

        private void initGame()
        {
            btnUndo.Enabled = true;
            btnRedo.Enabled = true;
        }
        private void endGame()
        {
            btnUndo.Enabled = false;
            btnRedo.Enabled = false;
        }
        private void pnlChessBoard_MouseClick(object sender, MouseEventArgs e)
        {
            game.goChess(e.X, e.Y, g);
            if (game.checkWinner())
            {
                endGame();
                return;
            }
            if (game.Mode() == MODE_PVC)
            {
                game.runComputer(g);
            }
            if (game.checkWinner())
            {
                endGame();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblChuChay.Location = new Point(lblChuChay.Location.X, lblChuChay.Location.Y - 1);
        }

        private void clearBackground(Graphics g)
        {
            Bitmap bitmap = new Bitmap(pnlChessBoard.Width, pnlChessBoard.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bitmap.MakeTransparent();
            pnlChessBoard.BackgroundImage = bitmap;
        }

        private void btnPlayerVSPlayer_Click(object sender, EventArgs e)
        {
            initGame();
            clearBackground(g);
            game.startNewPlayerVsPlayer();
            game.DrawChessBoard(g);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            game.undo(g, pnlChessBoard.BackColor);
            pnlChessBoard.Invalidate();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            game.redo(g);
        }

        private void btnPlayerVSCom_Click(object sender, EventArgs e)
        {
            initGame();
            clearBackground(g);
            game.startNewPlayerVsCom(g);
            game.DrawChessBoard(g);
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            if (game.Mode() == MODE_PVC)
            {
                btnPlayerVSCom.PerformClick();
            }
            else
            {
                btnPlayerVSPlayer.PerformClick();
            }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Menu_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
