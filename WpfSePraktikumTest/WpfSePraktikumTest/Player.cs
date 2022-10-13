using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfSePraktikumTest
{
    class HitSide
    {
        public HitTestResult? top;

        public HitTestResult? mid;
        public HitTestResult? buttom;
        public HitSide(HitTestResult? top, HitTestResult? mid, HitTestResult? buttom)
        {
            this.top = top;
            this.mid = mid;
            this.buttom = buttom;
        }
        public HitTestResult Ealuate()
        {
            bool hitSomething = false;
            HitTestResult? res = null;
            if (top != null)
            {
                hitSomething = true;
                res = top;
            }

            if (buttom != null)
            {
                hitSomething = true;
                res = buttom;
            }
            if (mid != null)
            {
                hitSomething = true;
                res = mid;
            }

            return res;
        }
    }
    public class Player
    {
        public Vector velocity = new Vector(0.0, 0.0);
        public double width = 50;
        public double height = 50;
        private double _x;
        private double _y;
        private Rectangle _rectangle;
        private Canvas _canvas;
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                Canvas.SetLeft(_rectangle, _x);
            }
        }
        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                Canvas.SetTop(_rectangle, _y);
            }
        }
        public Player(double x, double y, Canvas canv)
        {
            _canvas = canv;
            _rectangle = new Rectangle();
            _rectangle.Fill = new SolidColorBrush(Colors.Red);
            _rectangle.Width = width;
            _rectangle.Height = height;
            _canvas.Children.Add(_rectangle);
            Canvas.SetZIndex(_rectangle, 1);
            loop();

            X = x;
            Y = y;

        }
        public async void loop()
        {
            while (true)
            {
                velocity.Y += 0.2;
                collisiondetection();
                if (Math.Abs(velocity.X) <= 0.2) { velocity.X -= velocity.X; }
                else if (velocity.X > 0) velocity.X -= 0.2;
                else velocity.X += 0.2;
                //if (Math.Abs(velocity.Y) <= 0.2) { velocity.Y -= velocity.Y; }
                //else if (velocity.Y > 0) velocity.Y -= 0.2;
                //else velocity.Y += 0.2;

                X += velocity.X;
                Y += velocity.Y;


                await Task.Delay(20);
            }
        }
        public void Jump(double force)
        {
            velocity.Y = -force;
        }
        public void collisiondetection()
        {

            HitSide top = new HitSide(
                VisualTreeHelper.HitTest(_canvas, new Point(X + 1, Y)),
                VisualTreeHelper.HitTest(_canvas, new Point(X + width / 2, Y)),
                VisualTreeHelper.HitTest(_canvas, new Point(X + width - 1, Y))
                );
            HitSide right = new HitSide(
               VisualTreeHelper.HitTest(_canvas, new Point(X + width, Y - 1)),
               VisualTreeHelper.HitTest(_canvas, new Point(X + width, Y + height / 2)),
               VisualTreeHelper.HitTest(_canvas, new Point(X + width, Y + height - 1))
               );
            HitSide left = new HitSide(
               VisualTreeHelper.HitTest(_canvas, new Point(X, Y - 1)),
               VisualTreeHelper.HitTest(_canvas, new Point(X, Y + height / 2)),
               VisualTreeHelper.HitTest(_canvas, new Point(X, Y + height - 1))
               );
            HitSide buttom = new HitSide(
               VisualTreeHelper.HitTest(_canvas, new Point(X + 1, Y + height)),
               VisualTreeHelper.HitTest(_canvas, new Point(X + width / 2, Y + height)),
               VisualTreeHelper.HitTest(_canvas, new Point(X + width - 1, Y + height))
               );


            // Perform the hit test against a given portion of the visual object tree.

            HitTestResult resTop = top.Ealuate();
            HitTestResult resLeft = left.Ealuate();
            HitTestResult resRight = right.Ealuate();
            HitTestResult resButtom = buttom.Ealuate();
            if (resTop != null && resTop.VisualHit != _rectangle)
            {
                Rectangle? rect = resTop.VisualHit as Rectangle;

                if (rect != null && (string)rect.Tag == "wand")
                {
                    if (velocity.Y < 0) velocity.Y = 0;
                }

            }
            if (resLeft != null && resLeft.VisualHit != _rectangle)
            {
                Rectangle? rect = resLeft.VisualHit as Rectangle;
                if (rect != null && (string)rect.Tag == "wand")
                {
                    if (velocity.X < 0) velocity.X = 0;
                }
            }
            if (resRight != null && resRight.VisualHit != _rectangle)
            {
                Rectangle? rect = resRight.VisualHit as Rectangle;
                if (rect != null && (string)rect.Tag == "wand")
                {
                    if (velocity.X > 0) velocity.X = 0;
                }
            }
            if (resButtom != null && resButtom.VisualHit != _rectangle)
            {
                Rectangle? rect = resButtom.VisualHit as Rectangle;
                if (rect != null && (string)rect.Tag == "wand")
                {
                    if (velocity.Y > 0) velocity.Y = 0;
                }
            }
        }
    }

}

