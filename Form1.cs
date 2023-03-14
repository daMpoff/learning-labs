using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace formTest
{
    public partial class Form1 : Form
    {
        private List<Ellipse> ellipses = new List<Ellipse>();
        public DashStyle dashStyle;
        private Pen pen = new Pen(Color.Black);
        private int penWidth;
        private bool inPaint;
        private int deltaX = 0;
        private int deltaY = 0;
        public Form1()
        {
            InitializeComponent();
            GenerateEllipses();
        }
        private void GenerateEllipses()
        {
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                int x = random.Next(pictureBox1.Width);
                int y = random.Next(pictureBox1.Height);
                int width = random.Next(50, 150);
                int height = random.Next(50, 150);
                ellipses.Add(new Ellipse(x, y, width, height));
            }
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            penWidth = trackBar1.Value;
            UpdatePen();
            pictureBox1.Invalidate();
        }
        private void UpdatePen()
        {
            pen = new Pen(pen.Color, penWidth);
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                foreach (Ellipse ellipse in ellipses)
                {
                    if (ellipse.Contains(e.Location))
                    {
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            ellipse.Color = colorDialog.Color;
                        }
                        break;
                    }
                }
                pictureBox1.Invalidate();
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Ellipse ellipse in ellipses)
            {
                if (ellipse.Contains(e.Location))
                {
                    inPaint = true;
                    deltaX = e.X - ellipse.X;
                    deltaY = e.Y - ellipse.Y;
                    break;
                }
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            inPaint = false;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (inPaint)
            {
                foreach (Ellipse ellipse in ellipses)
                {
                    if (ellipse.Contains(e.Location))
                    {
                        ellipse.X = e.X - deltaX;
                        ellipse.Y = e.Y - deltaY;
                        break;
                    }
                }
                pictureBox1.Invalidate();
            }
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Ellipse ellipse in ellipses)
            {
                Brush brush = new SolidBrush(ellipse.Color);
                Pen pen = new Pen(Color.Black, 3) { DashStyle = ellipse.DashStyle };
                e.Graphics.FillEllipse(brush, ellipse.X, ellipse.Y, ellipse.Width, ellipse.Height);
                e.Graphics.DrawEllipse(pen, ellipse.X, ellipse.Y, ellipse.Width, ellipse.Height);
            }
        }

        public class Ellipse
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public Color Color = Color.White;
            public DashStyle DashStyle = DashStyle.Solid;
            public Ellipse(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
            public bool Contains(Point point)
            {
                int a = Width / 2;
                int b = Height / 2;
                int h = X + a;
                int k = Y + b;
                int x = point.X;
                int y = point.Y;
                return (((x - h) * (x - h)) / (a * a)) + (((y - k) * (y - k)) / (b * b)) <= 1;
            }
        }
    }
}