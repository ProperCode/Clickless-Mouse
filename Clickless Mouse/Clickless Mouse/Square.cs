using System.Drawing;
using System.Windows.Forms;

namespace Clickless_Mouse
{
    public partial class Square : Form
    {
        int side = 1;
        int line_width = 1;
        Color c1;
        Color c2;

        public Square(int Side, int Line_width, Color color1, Color color2)
        {
            InitializeComponent();

            BackColor = Color.White;
            TransparencyKey = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            side = Side;
            line_width = Line_width;
            c1 = color1;
            c2 = color2;

            //this solves blinking problem that sometimes happens when squares are regenerated
            this.PointToClient(new Point(0, 0));
            this.Location = new Point(side * -1, side * -1);
        }

        private void Crosshair_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.White, this.ClientRectangle);

            Pen p = new Pen(c1, line_width);
            g.DrawRectangle(p, (int)(line_width / 2), (int)(line_width / 2), side - line_width, side - line_width);
            p.Dispose();

            p = new Pen(c2, line_width);
            g.DrawRectangle(p, (int)(line_width / 2) + line_width, (int)(line_width / 2) + line_width,
                side - 3 * line_width, side - 3 * line_width);
            p.Dispose();

            //Horizontal
            //Pen p = new Pen(c1, 3);
            //g.DrawLine(p, 0, 0, side, 0);
            //p.Dispose();

            //p = new Pen(c1, 3);
            //g.DrawLine(p, 0, side, side, side);
            //p.Dispose();

            ////Vertical
            //p = new Pen(c1, 3);
            //g.DrawLine(p, 0, 0, 0, side);
            //p.Dispose();

            //p = new Pen(c1, 3);
            //g.DrawLine(p, side, 0, side, side);
            //p.Dispose();

            //Horizontal
            //p = new Pen(c2);
            //g.DrawLine(p, 1, 1, side - 2, 1);
            //p.Dispose();

            //p = new Pen(c2);
            //g.DrawLine(p, 1, side - 2, side - 2, side - 2);
            //p.Dispose();

            ////Vertical
            //p = new Pen(c2);
            //g.DrawLine(p, 1, 1, 1, side - 2);
            //p.Dispose();

            //p = new Pen(c2);
            //g.DrawLine(p, side - 2, 1, side - 2, side - 2);
            //p.Dispose();

            //p = new Pen(Color.Red, 1);
            //Rectangle myRectangle = new Rectangle(0, 0, 11, 11);
            //g.DrawEllipse(p, myRectangle);
        }
    }
}