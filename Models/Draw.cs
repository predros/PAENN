using HelixToolkit.Wpf;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Helper;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Draw
{

    /// <summary>
    /// Static class comprised of several functions useful when drawing on the 3D Viewport.
    /// </summary>
    public static class DrawHelper
    {

        /// <summary>
        /// Given two points, corrects the second one's position (along the same vector) so that
        /// their apparent distance (as seen through the viewport's camera) is equal to that supplied.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="P1">The first (fixed) point.</param>
        /// <param name="P2">The second (moving) point.</param>
        /// <param name="distance">The desired apparent distance.</param>
        /// <returns>The adjusted point, along the same line as P1-P2.</returns>
        public static Point3D CorrectRelativeSize(HelixViewport3D viewport, Point3D P1, Point3D P2, double distance)
        {
            var T = Viewport3DHelper.GetTotalTransform(viewport.Viewport);

            var q1 = T.Transform(P1);
            var q2 = T.Transform(P2);

            var q1q2 = (Vector3D)(q2 - q1);
            q1q2.Normalize();

            var q2p = new System.Windows.Point(q1.X + q1q2.X * distance, q1.Y + q1q2.Y * distance);

            var Pnear = new Point3D();
            var Pfar = new Point3D();

            Viewport3DHelper.Point2DtoPoint3D(viewport.Viewport, q2p, out Pnear, out Pfar);

            var NearFar = (Vector3D)(Pfar - Pnear);
            NearFar.Normalize();

            var e = (Vector3D)(P2 - P1);
            var f = (Vector3D)(Pnear - Pfar);
            var g = (Vector3D)(Pnear - P1);

            var h = Vector3D.CrossProduct(f, g);
            var k = Vector3D.CrossProduct(f, e);

            var sign = Math.Sign(Vector3D.DotProduct(h, k));

            var P = P1 + sign * (h.Length / k.Length) * e;

            return P;
        }


        /// <summary>
        /// Checks whether a given point in a viewport is currently in view or not.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="P">The point.</param>
        /// <returns>True if the point is in view, false otherwise.</returns>
        public static bool IsInViewport(HelixViewport3D viewport, Point3D P)
        {
            var q = Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, P);

            if (q.X > viewport.ActualWidth || q.X < 0)
                return false;
            else if (q.Y > viewport.ActualHeight || q.Y < 0)
                return false;

            else return true;
        }

        /// <summary>
        /// Draws a 2D (unfilled) four-sided polygon in a 3D viewport.
        /// </summary>
        /// <param name="P1">The "upper-right" corner.</param>
        /// <param name="P2">The "upper-left" corner.</param>
        /// <param name="P3">The "lower-left" corner.</param>
        /// <param name="P4">The "lower-right" corner.</param>
        /// <param name="linesVisual">The LinesVisual3D used for drawing the polygon.</param>
        public static void DrawRectangle(Point3D P1, Point3D P2, Point3D P3, Point3D P4, ref LinesVisual3D linesVisual)
        {
            linesVisual.Points.Add(P1);
            linesVisual.Points.Add(P2);

            linesVisual.Points.Add(P2);
            linesVisual.Points.Add(P3);

            linesVisual.Points.Add(P3);
            linesVisual.Points.Add(P4);

            linesVisual.Points.Add(P4);
            linesVisual.Points.Add(P1);
        }

        /// <summary>
        /// Draws a wireframe rectangular prism in a 3D viewport.
        /// </summary>
        /// <param name="rectangle">The bounding prism.</param>
        /// <param name="linesVisual">The LinesVisual3D used for drawing.</param>
        public static void DrawWireframeBox(Rect3D rectangle, ref LinesVisual3D linesVisual)
        {
            var i = new Vector3D(1, 0, 0);
            var j = new Vector3D(0, 1, 0);
            var k = new Vector3D(0, 0, 1);

            var P1 = rectangle.Location;
            var P2 = P1 + rectangle.SizeY * j;
            var P3 = P1 + rectangle.SizeZ * k;
            var P4 = P2 + rectangle.SizeZ * k;

            var P5 = P1 + rectangle.SizeX * i;
            var P6 = P5 + rectangle.SizeY * j;
            var P7 = P5 + rectangle.SizeZ * k;
            var P8 = P6 + rectangle.SizeZ * k;

            linesVisual.Points.Add(P1);
            linesVisual.Points.Add(P2);

            linesVisual.Points.Add(P1);
            linesVisual.Points.Add(P3);

            linesVisual.Points.Add(P2);
            linesVisual.Points.Add(P4);

            linesVisual.Points.Add(P3);
            linesVisual.Points.Add(P4);

            linesVisual.Points.Add(P1);
            linesVisual.Points.Add(P5);

            linesVisual.Points.Add(P2);
            linesVisual.Points.Add(P6);

            linesVisual.Points.Add(P3);
            linesVisual.Points.Add(P7);

            linesVisual.Points.Add(P4);
            linesVisual.Points.Add(P8);

            linesVisual.Points.Add(P5);
            linesVisual.Points.Add(P6);

            linesVisual.Points.Add(P6);
            linesVisual.Points.Add(P8);

            linesVisual.Points.Add(P7);
            linesVisual.Points.Add(P8);

            linesVisual.Points.Add(P7);
            linesVisual.Points.Add(P5);
        }


        /// <summary>
        /// Draws an arc in 3D space.
        /// </summary>
        /// <param name="Center">Center point of the arc.</param>
        /// <param name="StartDir">3D direction between the center and the starting point.</param>
        /// <param name="EndDir">3D direction between the center and the ending point.</param>
        /// <param name="Radius">Radius of the arc.</param>
        /// <param name="linesVisual">LinesVisual3D used for drawing the arc.</param>
        /// <param name="ArrowStart">Whether to draw an arrow at the start tip.</param>
        /// <param name="ArrowEnd">Whether to draw an arrow at the end tip.</param>
        /// <param name="ArrowSize">Ratio between the arrow length and the arc radius.</param>
        /// <param name="steps">Number of line segments used to approximate the arc curvature.</param>
        public static void DrawArc(Point3D Center, Vector3D StartDir, Vector3D EndDir, double Radius, 
                                   LinesVisual3D linesVisual, bool ArrowStart=false, bool ArrowEnd = false, double ArrowSize = 0.2, double steps = 15)
        {
            var angle = Vector3D.AngleBetween(StartDir, EndDir) * Math.PI / 180;

            StartDir.Normalize();
            EndDir.Normalize();

            var Start = Radius * StartDir;
            var End = Radius * EndDir;

            var P = Center + Start;
            var arrowhead = new Point3D();
            var arrowtail = new Point3D();
            var arrowwing1 = new Point3D();
            var arrowwing2 = new Point3D();
            var arrowdir = new Vector3D();

            double t = 0;

            for(int i = 0; i < steps; i++)
            {
                t = i * angle / steps;
                linesVisual.Points.Add(P);
                P = Center + (Math.Sin(angle - t) * Start + Math.Sin(t) * End) / Math.Sin(angle);
                linesVisual.Points.Add(P);
            }

            linesVisual.Points.Add(P);
            linesVisual.Points.Add(Center + End);

            if (ArrowStart)
            {
                arrowhead = Center + Start;
                arrowtail = Center + (Math.Sin(angle - angle / steps) * Start + Math.Sin(angle / steps) * End) / Math.Sin(angle);

                arrowdir = arrowtail - arrowhead;
                arrowdir.Normalize();

                arrowwing1 = arrowhead + ArrowSize * Radius * (arrowdir + StartDir);
                arrowwing2 = arrowhead + ArrowSize * Radius * (arrowdir -  StartDir);

                linesVisual.Points.Add(arrowhead);
                linesVisual.Points.Add(arrowwing1);

                linesVisual.Points.Add(arrowhead);
                linesVisual.Points.Add(arrowwing2);
            }

            if (ArrowEnd)
            {
                arrowhead = Center + End;
                arrowtail = Center + (Math.Sin(angle - t) * Start + Math.Sin(t) * End) / Math.Sin(angle);
                arrowdir = arrowtail - arrowhead;
                arrowdir.Normalize();

                arrowwing1 = arrowhead + ArrowSize * Radius * (arrowdir + EndDir);
                arrowwing2 = arrowhead + ArrowSize * Radius * (arrowdir - EndDir);

                linesVisual.Points.Add(arrowhead);
                linesVisual.Points.Add(arrowwing1);

                linesVisual.Points.Add(arrowhead);
                linesVisual.Points.Add(arrowwing2);
            }
        }


        /// <summary>
        /// Draws a 2D arrow in 3D space.
        /// </summary>
        /// <param name="Start">Starting point of the arrow.</param>
        /// <param name="End">Ending point of the arrow.</param>
        /// <param name="Normal">Vector normal to the plane containing the arrow.</param>
        /// <param name="linesVisual">LinesVisual3D element used for drawing the arrow.</param>
        /// <param name="ArrowLength">Ratio between the arrow-tip length and the full length.</param>
        /// <param name="Angle">Angle between the arrow's wing and body.</param>
        /// <param name="DoubleArrow">Whether to draw tips both on start and end.</param>
        public static void DrawArrow(Point3D Start, Point3D End, Vector3D Normal, LinesVisual3D linesVisual, double ArrowLength=0.3, double Angle=45, bool DoubleArrow = false)
        {
            var i = Start - End;
            var length = ArrowLength * i.Length;
            i.Normalize();

            var j = Vector3D.CrossProduct(Normal, i);
            j.Normalize();

            var tip1 = End + length * (i + j);
            var tip2 = End + length * (i - j);

            linesVisual.Points.Add(Start);
            linesVisual.Points.Add(End);
            linesVisual.Points.Add(End);
            linesVisual.Points.Add(tip1);
            linesVisual.Points.Add(End);
            linesVisual.Points.Add(tip2);

            if (DoubleArrow)
            {
                tip1 = Start + length * (-i + Math.Tan(Angle * Math.PI/180) * j);
                tip2 = Start + length * (-i - Math.Tan(Angle * Math.PI / 180) * j);

                linesVisual.Points.Add(Start);
                linesVisual.Points.Add(tip1);
                linesVisual.Points.Add(Start);
                linesVisual.Points.Add(tip2);
            }

        }
    }



    /// <summary>
    /// Controls the rendering of axes (both 3D and 2D), lighting (3D) and gridlines (2D).
    /// </summary>
    public static class DrawAmbient
    {
        public static LinesVisual3D X_Axis = new LinesVisual3D{ Color = Colors.Red, Thickness = 1 };
        public static LinesVisual3D Y_Axis = new LinesVisual3D { Color = Colors.Green, Thickness = 1 };
        public static LinesVisual3D Z_Axis = new LinesVisual3D{ Color = Colors.Blue, Thickness = 1};


        /// <summary>
        /// Draws the X-Y-Z axes in the 3D viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="unitsize">How long must a line be in order to be 1 unit length on screen.</param>
        public static void DrawAxes(HelixViewport3D viewport, double unitsize=2)
        {
            if (Double.IsNaN(unitsize) || Double.IsInfinity(unitsize) || unitsize == 0)
                unitsize = 2;

            X_Axis.Points.Clear();
            X_Axis.Points.Add(new Point3D(0, 0, 0));
            X_Axis.Points.Add(new Point3D(10 * unitsize, 0, 0));

            Y_Axis.Points.Clear();
            Y_Axis.Points.Add(new Point3D(0, 0, 0));
            Y_Axis.Points.Add(new Point3D(0, 10 * unitsize, 0));

            Z_Axis.Points.Clear();
            Z_Axis.Points.Add(new Point3D(0, 0, 0));
            Z_Axis.Points.Add(new Point3D(0, 0, 10 * unitsize));

            if (!viewport.Children.Contains(X_Axis))
                viewport.Children.Add(X_Axis);

            if (!viewport.Children.Contains(Y_Axis))
                viewport.Children.Add(Y_Axis);

            if (!viewport.Children.Contains(Z_Axis))
                viewport.Children.Add(Z_Axis);
        }

        /// <summary>
        /// Adds ambient lighting to the 3D viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        public static void DrawAmbientLight(HelixViewport3D viewport)
        {
            foreach(ModelVisual3D child in viewport.Children)
            {
                if (child.Content != null && child.Content.GetType() == typeof(AmbientLight))
                    return;
            }
            viewport.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });
        }
    }



    /// <summary>
    /// Contains and controls the rendering of nodes both on 2D and 3D.
    /// </summary>
    public class NodeContainer
    {
        // 3D rendering parameters
        private HelixViewport3D viewport;
        public PointsVisual3D SelectedNodes = new PointsVisual3D() { Size = 10, Color = Colors.Red };
        public PointsVisual3D UnselectedNodes= new PointsVisual3D() { Size = 8, Color = Colors.Black };
        
        // 2D rendering parameters
        private Canvas canvas;
        public List<Ellipse> EllipsesList = new List<Ellipse>();
        private List<Point> NodesList = new List<Point>();


        
        /// <summary>
        /// Node Container class constructor
        /// </summary>
        /// <param name="viewport3D">The viewport used for the 3D view.</param>
        /// <param name="cnv">The canvas used for the 2D views.</param>
        public NodeContainer(HelixViewport3D viewport3D, Canvas cnv)
        {
            viewport = viewport3D;
            canvas = cnv;

            viewport.Children.Add(SelectedNodes);
            viewport.Children.Add(UnselectedNodes);
        }



        /// <summary>
        ///  Adds a new node to the viewport.
        /// </summary>
        /// <param name="point">The point to be added.</param>
        public void AddNode(Point3D point)
        {

            if (UnselectedNodes.Points.Contains(point) || SelectedNodes.Points.Contains(point))
                return;

            UnselectedNodes.Points.Add(point);
        }

        /// <summary>
        ///  Adds a new node to the canvas.
        /// </summary>
        /// <param name="point">The point to be added.</param>
        public void AddNode(Point point)
        {
            if (NodesList.Contains(point))
                return;

            var scale = ((MatrixTransform)canvas.RenderTransform).Matrix.M11;

            var n = new Ellipse { Height = 6 / scale, Width = 6 / scale, Fill = Brushes.Black };
            Canvas.SetLeft(n, point.X - 3 / scale);
            Canvas.SetTop(n, point.Y - 3 / scale);

            EllipsesList.Add(n);
            NodesList.Add(point);
            canvas.Children.Add(n);
        }



        /// <summary>
        /// Changes a given node to be displayed as selected in the viewport.
        /// </summary>
        /// <param name="point">The point to be selected.</param>
        public void Select(Point3D point)
        {

            if (!UnselectedNodes.Points.Contains(point))
                return;

            UnselectedNodes.Points.Remove(point);
            SelectedNodes.Points.Add(point);
        }

        /// <summary>
        /// Changes a given node to be displayed as selected in the canvas.
        /// </summary>
        /// <param name="point">The point to be selected.</param>
        public void Select(Point point)
        {
            if (!NodesList.Contains(point))
                return;

            var i = NodesList.IndexOf(point);
            EllipsesList[i].Fill = Brushes.Red;
        }



        /// <summary>
        /// Changes a given node to be displayed as not selected in the viewport.
        /// </summary>
        /// <param name="point">The point to be unselected.</param>
        public void Unselect(Point3D point)
        {

            if (!SelectedNodes.Points.Contains(point))
                return;

            SelectedNodes.Points.Remove(point);
            UnselectedNodes.Points.Add(point);
        }

        /// <summary>
        /// Changes a given node to be displayed as not selected in the canvas.
        /// </summary>
        /// <param name="point">The point to be unselected.</param>
        public void Unselect(Point point)
        {
            if (!NodesList.Contains(point))
                return;

            var i = NodesList.IndexOf(point);
            EllipsesList[i].Fill = Brushes.Black;
        }



        /// <summary>
        /// Check if a point is being clicked in the viewport.
        /// </summary>
        /// <param name="e">The mouse-click event.</param>
        /// <param name="point3d">The point to be checked.</param>
        /// <returns></returns>
        public bool CheckIfClicked(MouseButtonEventArgs e, Point3D point3d)
        {
            var point2d = Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, point3d);
            var click = e.GetPosition(viewport);
            return (click - point2d).Length < 6;
        }

        /// <summary>
        /// Check if a point is being clicked in the canvas.
        /// </summary>
        /// <param name="e">The mouse-click event.</param>
        /// <param name="point">The point to be checked.</param>
        /// <returns></returns>
        public bool CheckIfClicked(MouseButtonEventArgs e, Point point)
        {
            var click = e.GetPosition(canvas);
            return (click - point).Length < 6;
        }



        /// <summary>
        /// Rescales every node in the canvas, in order to keep their radius constant when zooming in or out.
        /// </summary>
        public void Rescale()
        {
            var scale = ((MatrixTransform)canvas.RenderTransform).Matrix.M11;
            Console.WriteLine(scale);
            for (int i = 0; i < NodesList.Count; i++)
            {
                var N = EllipsesList[i];
                N.Width = 6 / scale;
                N.Height = 6 / scale;
                Canvas.SetLeft(N, NodesList[i].X - 3 / scale);
                Canvas.SetTop(N, NodesList[i].Y - 3 / scale);
            }
        }



        /// <summary>
        /// Clears every node from the viewport or canvas.
        /// </summary>
        /// <param name="type">Whether to clear the viewport (3D) or canvas (2D).</param>
        public void Clear(string type)
        {
            switch (type)
            {
                case "canvas":
                    foreach (Ellipse ellipse in EllipsesList)
                        canvas.Children.Remove(ellipse);

                    NodesList.Clear();
                    EllipsesList.Clear();
                    break;
                case "viewport":
                    SelectedNodes.Points.Clear();
                    UnselectedNodes.Points.Clear();
                    break;
            }
        }

    }



    /// <summary>
    /// Contains and controls the rendering of members both in 2D and 3D.
    /// </summary>
    public class MemberContainer
    {
        // 3D rendering parameters
        private HelixViewport3D viewport;
        public LinesVisual3D SelectedMembers = new LinesVisual3D() { Thickness = 3, Color = Colors.DarkMagenta };
        public LinesVisual3D UnselectedMembers = new LinesVisual3D() { Thickness = 2, Color = Colors.SteelBlue };

        private Canvas canvas;
        private List<Line> LinesList = new List<Line>();
        private List<Point> StartList = new List<Point>();
        private List<Point> EndList = new List<Point>();

        /// <summary>
        /// Member container class constructor.
        /// </summary>
        /// <param name="viewport3D">The viewport.</param>
        /// <param name="cnv">The canvas.</param>
        public MemberContainer(HelixViewport3D viewport3D, Canvas cnv)
        {
            viewport = viewport3D;
            canvas = cnv;

            viewport.Children.Add(SelectedMembers);
            viewport.Children.Add(UnselectedMembers);
        }



        /// <summary>
        /// Adds a member to the 3D viewport.
        /// </summary>
        /// <param name="P1">The member's starting point.</param>
        /// <param name="P2">The member's ending point.</param>
        public void AddMember(Point3D p1, Point3D p2)
        {
            if (UnselectedMembers.Points.Contains(p1))
            {
                var i = UnselectedMembers.Points.IndexOf(p1);
                if (i != UnselectedMembers.Points.Count - 1 && UnselectedMembers.Points[i + 1] == p2)
                    return;
            }

            if (SelectedMembers.Points.Contains(p1))
            {
                var i = SelectedMembers.Points.IndexOf(p1);

                if (i != SelectedMembers.Points.Count - 1 && SelectedMembers.Points[i + 1] == p2)
                    return;
            }
            UnselectedMembers.Points.Add(p1);
            UnselectedMembers.Points.Add(p2);
        }

        /// <summary>
        /// Adds a member to the 2D canvas.
        /// </summary>
        /// <param name="P1">The member's starting point.</param>
        /// <param name="P2">The member's ending point.</param>
        public void AddMember(Point p1, Point p2)
        {
            for(int i=0; i<StartList.Count; i++)
            {
                if (StartList[i] == p1 && EndList[i] == p2 || EndList[i] == p1 && StartList[i] == p2)
                    return;
            }
            var l = new Line { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, StrokeThickness = 2.5, Stroke = Brushes.LightSteelBlue };
            
            canvas.Children.Add(l);
            LinesList.Add(l);
            StartList.Add(p1);
            EndList.Add(p2);
        }



        /// <summary>
        /// Changes a member to be displayed as selected in the 3D viewport.
        /// </summary>
        /// <param name="p1">The member's starting point.</param>
        /// <param name="p2">The member's ending point.</param>
        public void Select(Point3D p1, Point3D p2)
        {
            var index = FindPointPair(p1, p2, UnselectedMembers.Points);

            if (index == null)
                return;                

            UnselectedMembers.Points.RemoveAt((int)index + 1);
            UnselectedMembers.Points.RemoveAt((int)index);

            SelectedMembers.Points.Add(p1);
            SelectedMembers.Points.Add(p2);
        }

        /// <summary>
        /// Changes a member to be displayed as selected in the 2D canvas.
        /// </summary>
        /// <param name="p1">The member's starting point.</param>
        /// <param name="p2">The member's ending point.</param>
        public void Select(Point p1, Point p2)
        {
            foreach (Line line in LinesList)
            {
                var q1 = new Point(line.X1, line.Y1);
                var q2 = new Point(line.X2, line.Y2);

                if (p1 == q1 && p2 == q2)
                    line.Stroke = Brushes.DarkMagenta;
            }
        }



        /// <summary>
        /// Changes a member to be displayed as not selected in the 3D viewport.
        /// </summary>
        /// <param name="p1">The member's starting point.</param>
        /// <param name="p2">The member's ending point.</param>
        public void Unselect(Point3D p1, Point3D p2)
        {
            var index = FindPointPair(p1, p2, SelectedMembers.Points);

            if (index == null)
                return;

            SelectedMembers.Points.RemoveAt((int)index + 1);
            SelectedMembers.Points.RemoveAt((int)index);

            UnselectedMembers.Points.Add(p1);
            UnselectedMembers.Points.Add(p2);
        }

        /// <summary>
        /// Changes a member to be displayed as not selected in the 2D canvas.
        /// </summary>
        /// <param name="p1">The member's starting point.</param>
        /// <param name="p2">The member's ending point.</param>
        public void Unselect(Point p1, Point p2)
        {
            foreach (Line line in LinesList)
            {
                var q1 = new Point(line.X1, line.Y1);
                var q2 = new Point(line.X2, line.Y2);

                if (p1 == q1 && p2 == q2)
                    line.Stroke = Brushes.LightSteelBlue;
            }
        }



        /// <summary>
        /// Checks if a given pair of points form are contained within a list in successive order.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="list">The list to be checked.</param>
        /// <returns>If the pair of points is found, return the first one's index. Otherwise, returns null.</returns>
        public int? FindPointPair(Point3D p1, Point3D p2, Point3DCollection list)
        {
            for(int i = list.Count - 1; i > 0; i--)
            {
                if (list[i] == p2)
                {
                    if (list[i - 1] == p1)
                        return i - 1;
                }
            }
            return null;
        }



        /// <summary>
        /// Checks if a given member in the 3D Viewport is being clicked.
        /// </summary>
        /// <param name="e">The click event.</param>
        /// <param name="p1">The member's starting point.</param>
        /// <param name="p2">The member's ending point.</param>
        /// <returns>Whether or not the member is being clicked.</returns>
        public bool CheckIfClicked(MouseButtonEventArgs e, Point3D p1, Point3D p2)
        {
            var p1_2d = Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, p1);
            var p2_2d = Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, p2);
            var click = e.GetPosition(viewport);

            var xmax = Math.Max(p1_2d.X, p2_2d.X);
            var xmin = Math.Min(p1_2d.X, p2_2d.X);
            var ymax = Math.Max(p1_2d.Y, p2_2d.Y);
            var ymin = Math.Min(p1_2d.Y, p2_2d.Y);
            var d = Functions.DistancePointLine(p1_2d, p2_2d, click);

            if (d > 4 || click.X < xmin - 2 || click.Y < ymin - 2 || click.X > xmax + 2 || click.Y > xmax + 2)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Checks if a given member in the 2D Canvas is being clicked.
        /// </summary>
        /// <param name="e">The click event.</param>
        /// <param name="p1">The member's starting point.</param>
        /// <param name="p2">The member's ending point.</param>
        /// <returns>Whether or not the member is being clicked.</returns>
        public bool CheckIfClicked(MouseButtonEventArgs e, Point p1, Point p2)
        {
            var click = e.GetPosition(canvas);

            var xmax = Math.Max(p1.X, p2.X);
            var xmin = Math.Min(p1.X, p2.X);
            var ymax = Math.Max(p1.Y, p2.Y);
            var ymin = Math.Min(p1.Y, p2.Y);

            var d = Functions.DistancePointLine(p1, p2, click);
           
            if (d > 4 || click.X < xmin - 2 || click.Y < ymin - 2 || click.X > xmax + 2 || click.Y > xmax + 2)
                return false;
            else
                return true;
        }



        /// <summary>
        /// Rescales every member so that the line thickness is kept constant when zooming in/out.
        /// </summary>
        public void Rescale()
        {
            var scale = ((MatrixTransform)canvas.RenderTransform).Matrix.M11;

            foreach (Line line in LinesList)
                line.StrokeThickness = 2.5 / scale;
        }



        /// <summary>
        /// Clears the members being rendered in the 2D Canvas or 3D Viewport.
        /// </summary>
        /// <param name="type">Whether to clear the Canvas or the Viewport.</param>
        public void Clear(string type)
        {
            switch (type)
            {
                case "viewport":
                    SelectedMembers.Points.Clear();
                    UnselectedMembers.Points.Clear();
                    break;
                case "canvas":
                    foreach (Line line in LinesList)
                        canvas.Children.Remove(line);
                    LinesList.Clear();
                    StartList.Clear();
                    EndList.Clear();
                    break;
            }
        }
    }



    /// <summary>
    /// Contains and controls the rendering of support elements both in 2D and 3D.
    /// </summary>
    public class SupportContainer
    {
        // 3D rendering parameters
        private HelixViewport3D viewport;
        private LinesVisual3D linesupports = new LinesVisual3D { Color = Colors.Black, Thickness = 1.5 };
        private Model3DGroup supports = new Model3DGroup();
        private ModelVisual3D supportsvisual = new ModelVisual3D();

        // 2D rendering parameters
        private Canvas canvas;



        /// <summary>
        /// Support Container class constructor.
        /// </summary>
        /// <param name="viewport3D">The viewport.</param>
        /// <param name="cnv">The canvas.</param>
        public SupportContainer(HelixViewport3D viewport3D, Canvas cnv)
        {
            viewport = viewport3D;
            supportsvisual.Content = supports;
            viewport.Children.Add(supportsvisual);
            viewport.Children.Add(linesupports);

            canvas = cnv;
        }



        /// <summary>
        /// Adds a support element to a given point, based upon the applied restrictions.
        /// </summary>
        /// <param name="P">The point.</param>
        /// <param name="r">Bool dictionary containing the states of every degree of freedom.</param>
        /// <param name="unitsize">Real length needed, given the current camera position, to result in an apparent (screen units) unitary length.</param>
        public void AddSupport(Point3D P, Dictionary<string, bool> r, double unitsize=0)
        {
            var dist = 10 * unitsize;
            if (dist ==0)
            {
                var p2 = DrawHelper.CorrectRelativeSize(viewport, P, new Point3D(P.X, P.Y - 10, P.Z), 10);
                dist = (P - p2).Length;
            }

            if (!r["Ux"] && !r["Rx"] && !r["Uy"] && !r["Ry"] && !r["Uz"] && !r["Rz"])
                return;

            if (r["Ux"] && r["Rx"] && r["Uy"] && r["Ry"] && r["Uz"] && r["Rz"])
            {
                AddEncastre(P, dist);
                return;
            }

            if (r["Ux"] && r["Uy"] && r["Uz"])
                AddFixed(P, dist);

            else if (r["Ux"] && r["Uy"])
                AddCart(P, "Z", dist);

            else if (r["Ux"] && r["Uz"])
                AddCart(P, "Y", dist);

            else if (r["Uy"] && r["Uz"])
                AddCart(P, "X", dist);

            else if (r["Ux"])
                AddSphere(P, "X", dist);

            else if (r["Uy"])
                AddSphere(P, "Y", dist);

            else if (r["Uz"])
                AddSphere(P, "Z", dist);

            if (r["Rx"] && r["Ry"] && r["Rz"])
            {
                AddFullRotFix(P, dist);
                return;
            }

            if (r["Rx"])
                AddRotationFixer(P, "X", dist);
            if (r["Ry"])
                AddRotationFixer(P, "Y", dist);
            if (r["Rz"])
                AddRotationFixer(P, "Z", dist);
        }



        /// <summary>
        /// Clears the support elements contained in the viewport.
        /// </summary>
        public void Clear()
        {
            viewport.Children.Remove(supportsvisual);
            viewport.Children.Remove(linesupports);

            supports = new Model3DGroup();
            supportsvisual = new ModelVisual3D();
            linesupports = new LinesVisual3D();

            supportsvisual.Content = supports;

            viewport.Children.Add(supportsvisual);
            viewport.Children.Add(linesupports);
        }



        /// <summary>
        /// Draws a sphere (single-directional displacement support) at the given point.
        /// </summary>
        /// <param name="P">The point.</param>
        /// <param name="direction">The restricted direction.</param>
        /// <param name="diameter">The sphere's diameter.</param>
        private void AddSphere(Point3D P, string direction, double diameter)
        {
            var builder = new MeshBuilder();
            var color = Colors.Firebrick;

            switch (direction)
            {
                case "X":
                    builder.AddSphere(new Point3D(P.X - diameter / 2, P.Y, P.Z), diameter / 2);
                    break;
                case "Y":
                    builder.AddSphere(new Point3D(P.X, P.Y - diameter / 2, P.Z), diameter / 2);
                    break;
                case "Z":
                    builder.AddSphere(new Point3D(P.X, P.Y, P.Z - diameter / 2), diameter / 2);
                    break;
            }
            var geometry = builder.ToMesh();
            supports.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(color) });
        }



        /// <summary>
        /// Adds a pyramid (three-directional displacement support) at the given point.
        /// </summary>
        /// <param name="P">The point.</param>
        /// <param name="length">The pyramid's side length.</param>
        private void AddFixed(Point3D P, double length)
        {
            var builder = new MeshBuilder();
            var color = Colors.DarkGreen;

            builder.AddPyramid(new Point3D(P.X, P.Y, P.Z - length / 4), length, length / 2, true);
            var geometry = builder.ToMesh();
            supports.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(color) });
        }



        /// <summary>
        /// Draws a two-dimensional rectangle (single-directional rotation support) at the given point.
        /// </summary>
        /// <param name="P">The point</param>
        /// <param name="type">The restricted direction.</param>
        /// <param name="length">The rectangle's side length.</param>
        private void AddRotationFixer(Point3D P, string type, double length)
        {
            var P1 = new Point3D();
            var P2 = new Point3D();
            var P3 = new Point3D();
            var P4 = new Point3D();

            switch (type)
            {
                case "X":
                    P1 = new Point3D(P.X, P.Y - length / 2, P.Z - length / 2);
                    P2 = new Point3D(P.X, P.Y + length / 2, P.Z - length / 2);
                    P3 = new Point3D(P.X, P.Y + length / 2, P.Z + length / 2);
                    P4 = new Point3D(P.X, P.Y - length / 2, P.Z + length / 2);
                    break;
                case "Y":
                    P1 = new Point3D(P.X - length / 2, P.Y, P.Z - length / 2);
                    P2 = new Point3D(P.X + length / 2, P.Y, P.Z - length / 2);
                    P3 = new Point3D(P.X + length / 2, P.Y, P.Z + length / 2);
                    P4 = new Point3D(P.X - length / 2, P.Y, P.Z + length / 2);
                    break;
                case "Z":
                    P1 = new Point3D(P.X - length / 2, P.Y - length / 2, P.Z);
                    P2 = new Point3D(P.X + length / 2, P.Y - length / 2, P.Z);
                    P3 = new Point3D(P.X + length / 2, P.Y + length / 2, P.Z);
                    P4 = new Point3D(P.X - length / 2, P.Y + length / 2, P.Z);
                    break;
            }
            DrawHelper.DrawRectangle(P1, P2, P3, P4, ref linesupports);
        }



        /// <summary>
        /// Draws a 3D wireframe box (three-directional rotation support)
        /// </summary>
        /// <param name="P">The point.</param>
        /// <param name="length">The box's side length.</param>
        private void AddFullRotFix(Point3D P, double length)
        {
            var rect = new Rect3D(new Point3D(P.X - length / 2, P.Y - length / 2, P.Z - length / 2), new Size3D(length, length, length));
            DrawHelper.DrawWireframeBox(rect, ref linesupports);
        }



        /// <summary>
        /// Draws a little cart (two-direction displacement support) at the given point.
        /// </summary>
        /// <param name="P">The point.</param>
        /// <param name="type">The direction normal to the restricted plane (a.k.a. the unrestricted direction)</param>
        /// <param name="length">The cart's length.</param>
        private void AddCart(Point3D P, string type, double length)
        {
            var builder = new MeshBuilder();
            var builder2 = new MeshBuilder();

            switch (type)
            {
                case "Z":     // Ux, Uy
                    builder2.AddCylinder(new Point3D(P.X - 0.25 * length, P.Y - length / 4, P.Z),
                                         new Point3D(P.X - 0.25 * length, P.Y - length / 3, P.Z), length / 8);

                    builder2.AddCylinder(new Point3D(P.X - 0.25 * length, P.Y + length / 4, P.Z),
                                         new Point3D(P.X - 0.25 * length, P.Y + length / 3, P.Z), length / 8);
                    builder.AddBox(new Rect3D(new Point3D(P.X - length / 8, P.Y - length / 3, P.Z - length / 8),
                                   new Size3D(length / 8, 2 * length / 3, length / 4)));
                    break;
                
                case "Y":     // Ux, Uz
                    builder2.AddCylinder(new Point3D(P.X - 0.25 * length, P.Y, P.Z - length / 4),
                     new Point3D(P.X - 0.25 * length, P.Y, P.Z - length / 3), length / 8);
                    builder2.AddCylinder(new Point3D(P.X - 0.25 * length, P.Y, P.Z + length / 4),
                                         new Point3D(P.X - 0.25 * length, P.Y, P.Z + length / 3), length / 8);
                    builder.AddBox(new Rect3D(new Point3D(P.X - length / 8, P.Y - length / 8, P.Z - length / 3),
                                   new Size3D(length / 8, length / 4, 2 * length / 3)));
                    break;

                case "X":     // Uy, Uz
                    builder2.AddCylinder(new Point3D(P.X, P.Y - 0.25 * length, P.Z - length / 4),
                     new Point3D(P.X, P.Y - 0.25 * length, P.Z - length / 3), length / 8);
                    builder2.AddCylinder(new Point3D(P.X, P.Y - 0.25 * length, P.Z + length / 4),
                                         new Point3D(P.X, P.Y - 0.25 * length, P.Z + length / 3), length / 8);
                    builder.AddBox(new Rect3D(new Point3D(P.X - length / 8, P.Y - length / 8, P.Z - length / 3),
                                   new Size3D(length / 4, length / 8, 2 * length / 3)));
                    break;
            }
            var geometry = builder.ToMesh();
            var geometry2 = builder2.ToMesh();
            supports.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(Colors.DarkMagenta) });
            supports.Children.Add(new GeometryModel3D { Geometry = geometry2, Material = MaterialHelper.CreateMaterial(Colors.Black) });
        }



        /// <summary>
        /// Adds a short cylinder/encastre (three-directional displacement and rotation support) at the given point.
        /// </summary>
        /// <param name="P">The point.</param>
        /// <param name="diameter">The cylinder's diameter.</param>
        private void AddEncastre(Point3D P, double diameter)
        {
            var builder = new MeshBuilder();
            builder.AddCylinder(P, new Point3D(P.X, P.Y, P.Z - diameter / 3), diameter);
            var geometry = builder.ToMesh();
            supports.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(Colors.SaddleBrown) });
        }

    }



    /// <summary>
    /// Contains and controls the rendering of nodal forces symbols both in 2D and 3D.
    /// </summary>
    public class NodalForcesContainer
    {
        // 3D rendering parameters
        private HelixViewport3D viewport;
        private LinesVisual3D lines = new LinesVisual3D { Color = Colors.DarkBlue, Thickness = 1.5 };
        private Model3DGroup forces = new Model3DGroup();
        private ModelVisual3D visual_forces = new ModelVisual3D();
        private BillboardTextGroupVisual3D textgroup = new BillboardTextGroupVisual3D();

        // 2D rendering parameters
        private Canvas canvas;



        /// <summary>
        /// Nodal Forces Container class constructor.
        /// </summary>
        /// <param name="viewport3D">The viewport.</param>
        /// <param name="cnv">The canvas.</param>
        public NodalForcesContainer(HelixViewport3D viewport3D, Canvas cnv)
        {
            viewport = viewport3D;
            visual_forces.Content = forces;
            viewport.Children.Add(visual_forces);
            viewport.Children.Add(lines);
            viewport.Children.Add(textgroup);

            canvas = cnv;
        }



        /// <summary>
        /// Clears every nodal force being rendered in the viewport.
        /// </summary>
        public void Clear()
        {
            if (viewport.Children.Contains(lines))
                viewport.Children.Remove(lines);

            if (viewport.Children.Contains(textgroup))
                viewport.Children.Remove(textgroup);

            textgroup = new BillboardTextGroupVisual3D();
            forces = new Model3DGroup();
            lines = new LinesVisual3D();

            visual_forces.Content = forces;

            viewport.Children.Add(lines);
            viewport.Children.Add(textgroup);

        }



        /// <summary>
        /// Adds to the given point the forces in the given dictionary/loadcase.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="forces">The dictionary containing the nodal forces.</param>
        /// <param name="loadcase">The loadcase number to be considered.</param>
        /// <param name="unitsize">The real length needed around the point in order to create an apparent unitary length in the viewport.</param>
        public void AddForce(Point3D point, Dictionary<string, List<double>> forces, int loadcase, double unitsize)
        {
            var funit = PAENN.Models.UnitsHolder.Force;
            var Fx = forces["Fx"][loadcase];
            var Fy = forces["Fy"][loadcase];
            var Fz = forces["Fz"][loadcase];
            var Mx = forces["Mx"][loadcase];
            var My = forces["My"][loadcase];
            var Mz = forces["Mz"][loadcase];


            if (Fx > 0)
                DrawForce(point, "Fx", GetArrowSize(Fx, 0), Fx.ToString() + " " + funit, false, unitsize);
            else if (Fx < 0)
                DrawForce(point, "Fx", GetArrowSize(Fx, 0), Math.Abs(Fx).ToString() + " " + funit, true, unitsize);


            if (Fy > 0)
                DrawForce(point, "Fy", GetArrowSize(Fy, 0), Fy.ToString() + " " + funit, false, unitsize);
            else if (Fy < 0)
                DrawForce(point, "Fy", GetArrowSize(Fy, 0), Math.Abs(Fy).ToString() + " " + funit, true, unitsize);


            if (Fz > 0)
                DrawForce(point, "Fz", GetArrowSize(Fz, 0), Fz.ToString() + " " + funit, false, unitsize);
            else if (Fz < 0)
                DrawForce(point, "Fz", GetArrowSize(Fz, 0), Math.Abs(Fz).ToString() + " " + funit, true, unitsize);


            if (Mx > 0)
                DrawForce(point, "Mx", GetArrowSize(Mx, 0), Mx.ToString() + " " + funit, false, unitsize);
            else if (Mx < 0)
                DrawForce(point, "Mx", GetArrowSize(Mx, 0), Math.Abs(Mx).ToString() + " " + funit, true, unitsize);


            if (My > 0)
                DrawForce(point, "My", GetArrowSize(My, 0), My.ToString() + " " + funit, false, unitsize);
            else if (My < 0)
                DrawForce(point, "My", GetArrowSize(My, 0), Math.Abs(My).ToString() + " " + funit, true, unitsize);


            if (Mz > 0)
                DrawForce(point, "Mz", GetArrowSize(Mz, 0), Mz.ToString() + " " + funit, false, unitsize);
            else if (Mz < 0)
                DrawForce(point, "Mz", GetArrowSize(Mz, 0), Math.Abs(Mz).ToString() + " " + funit, true, unitsize);
        }



        /// <summary>
        /// Interpolates the nodal forces' value, given the global maximum and minimum.
        /// </summary>
        /// <param name="force">The value of the nodal force.</param>
        /// <param name="type">Whether it's a force (0) or moment (1).</param>
        /// <returns>The arrow length.</returns>
        private double GetArrowSize(double force, int type)
        {
            var size = 0.0;
            var max = 0.0;
            var min = 0.0;

            if (type == 0)
            {
                max = PAENN.ViewModels.VarHolder.MaxForce;
                min = PAENN.ViewModels.VarHolder.MinForce;
            }

            else
            {
                max = PAENN.ViewModels.VarHolder.MaxMom;
                min = PAENN.ViewModels.VarHolder.MinMom;
            }

            size = 15 + (Math.Abs(force) - min) * 25 / (max - min);
            return size;
        }



        /// <summary>
        /// Draws a given force at a given point in 3D space.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="type">The type/direction of the force.</param>
        /// <param name="length">The arrow length.</param>
        /// <param name="value">The value to be placed in the text.</param>
        /// <param name="IsNegative">Whether to invert the arrow or not.</param>
        /// <param name="unitsize">The real length needed around the point in order to create an apparent unitary length in the viewport.</param>
        private void DrawForce(Point3D point, string type, double length, string value="", bool IsNegative=false, double unitsize=0)
        {
            var builder = new MeshBuilder();

            var i = new Vector3D(1, 0, 0);
            var j = new Vector3D(0, 1, 0);
            var k = new Vector3D(0, 0, 1);
            var unit = unitsize;

            if (unit == 0)
            {
                var q = DrawHelper.CorrectRelativeSize(viewport, point, point - i, 1);
                unit = (point - q).Length;
            }

            var p2 = point - length * unit * i;
            var color = Colors.Crimson;
            var inverse = false;
            var textpos = p2 + unit * (8 * j + 8 * k);

            switch (type)
            {
                case "Fx":
                    break;
                case "Fy":
                    p2 = point - length * unit * j;
                    textpos = p2 + unit * (8 * i + 8 * k);
                    break;
                case "Fz":
                    p2 = point - length * unit * k;
                    textpos = p2 + unit * (8 * i + 8 * j);
                    break;
                case "Mx":
                    color = Colors.DarkOrange;
                    p2 = point + length * unit * i;
                    var center = point + 0.5 * length * unit * i;
                    textpos = p2 + unit * (8 * j + 8 * k);
                    DrawHelper.DrawArc(center, 0.5 * j - k, - j + k, 5 * unit, lines, IsNegative, !IsNegative, 0.2);
                    inverse = true;
                    break;
                case "My":
                    color = Colors.DarkOrange;
                    p2 = point + length * unit * j;
                    center = point + 0.5 * length * unit * j;
                    textpos = p2 + unit * (8 * i + 8 * k);
                    DrawHelper.DrawArc(center, 0.5 * i - k, -i + k, 5 * unit, lines, IsNegative, !IsNegative, 0.2);
                    inverse = true;
                    break;
                case "Mz":
                    //q = DrawHelper.CorrectRelativeSize(viewport, point, point + k, 1);
                    //unit = (point - q).Length;
                    color = Colors.DarkOrange;
                    p2 = point + length * unit * k;
                    center = point + 0.5 * length * unit * k;
                    textpos = p2 + unit * (8 * j + 8 * i);
                    DrawHelper.DrawArc(center, 0.5 * j - i, -j + i, 5 * unit, lines, IsNegative, !IsNegative, 0.2);
                    inverse = true;
                    break;
            }
            if (IsNegative && !inverse || !IsNegative && inverse)
                builder.AddArrow(point, p2, 2 * unit, 10);
            else
                builder.AddArrow(p2, point, 2 * unit, 10) ;

            var geometry = builder.ToMesh();
            forces.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(color) });

            if (value == "") return;

            var text = new BillboardTextVisual3D() { Text = value, Position = textpos, FontSize = 15 };
            textgroup.Children.Add(text);
        }
    }



    /// <summary>
    /// Contains and controls the rendering of member load symbols both in 2D and 3D.
    /// </summary>
    public class MemberLoadsContainer
    {
        // 3D rendering parameters
        private HelixViewport3D viewport;
        public LinesVisual3D linesX = new LinesVisual3D() { Color = Colors.Red, Thickness = 2 };
        public LinesVisual3D linesY = new LinesVisual3D() { Color = Colors.Green, Thickness = 2 };
        public LinesVisual3D linesZ = new LinesVisual3D() { Color = Colors.Blue, Thickness = 2 };
        private BillboardTextGroupVisual3D textgroup = new BillboardTextGroupVisual3D();

        // 2D rendering parameters
        private Canvas canvas;


        /// <summary>
        /// Member Loads Container class constructor.
        /// </summary>
        /// <param name="viewport3D">The viewport.</param>
        /// <param name="cnv">The canvas.</param>
        public MemberLoadsContainer(HelixViewport3D viewport3D, Canvas cnv)
        {
            viewport = viewport3D;
            canvas = cnv;

            viewport.Children.Add(linesX);
            viewport.Children.Add(linesY);
            viewport.Children.Add(linesZ);
            viewport.Children.Add(textgroup);
        }



        /// <summary>
        /// Clears every member load from the viewport.
        /// </summary>
        public void Clear()
        {
            if (viewport.Children.Contains(linesX))
                viewport.Children.Remove(linesX);

            if (viewport.Children.Contains(linesY))
                viewport.Children.Remove(linesY);

            if (viewport.Children.Contains(linesZ))
                viewport.Children.Remove(linesZ);

            if (viewport.Children.Contains(textgroup))
                viewport.Children.Remove(textgroup);

            textgroup = new BillboardTextGroupVisual3D();
            linesX = new LinesVisual3D();
            linesY = new LinesVisual3D();
            linesZ = new LinesVisual3D();

            viewport.Children.Add(linesX);
            viewport.Children.Add(linesY);
            viewport.Children.Add(linesZ);
            viewport.Children.Add(textgroup);
        }



        /// <summary>
        /// Adds the given loads to the member defined by the given start and end points.
        /// </summary>
        /// <param name="start">The member's start point.</param>
        /// <param name="end">The member's end point.</param>
        /// <param name="loads">Dictionary containing every load applied to the member.</param>
        /// <param name="loadcase">Loadcase index to be considered.</param>
        /// <param name="unitsize">The real length needed around the point in order to create an apparent unitary length in the viewport.</param>
        public void AddLoad(Point3D start, Point3D end, Dictionary<string, List<double>> loads, int loadcase, double unitsize=0)
        {
            var Qx0 = loads["Qx0"][loadcase];
            var Qy0 = loads["Qy0"][loadcase];
            var Qz0 = loads["Qz0"][loadcase];
            var Qx1 = loads["Qx1"][loadcase];
            var Qy1 = loads["Qy1"][loadcase];
            var Qz1 = loads["Qz1"][loadcase];


            if (Qx0 != 0 || Qx1 != 0)
                DrawLoad(start, end, GetArrowSize(Qx0), GetArrowSize(Qx1), "X", Qx0, Qx1, unitsize);


            if (Qy0 != 0 || loads["Qy1"][loadcase] != 0)
                DrawLoad(start, end, GetArrowSize(Qy0), GetArrowSize(Qy1), "Y", Qy0, Qy1, unitsize);


            if (Qz0 != 0 || Qz1 != 0)
                DrawLoad(start, end, GetArrowSize(Qz0), GetArrowSize(Qz1), "Z", Qz0, Qz1, unitsize);
        }



        /// <summary>
        /// Interpolates the given load value between the global maximum and minimum in order to find the correct arrow length.
        /// </summary>
        /// <param name="force">The load value.</param>
        /// <returns>The arrow length.</returns>
        private double GetArrowSize(double force)
        {

            var max = PAENN.ViewModels.VarHolder.MaxLoad;
            var min = PAENN.ViewModels.VarHolder.MinLoad;
            double size = 10;

            if (force > 0)
                size = 10 + (force - min) * 20 / (max - min);
            else if (force < 0)
                size = -10 + (force + min) * (-20) / (min - max);
            else
                size = 0;
            return size;
        }



        /// <summary>
        /// Draws the distributed load arrows along the member's axis.
        /// </summary>
        /// <param name="start">The member's start point.</param>
        /// <param name="end">The member's end point.</param>
        /// <param name="heightstart">The load's start height.</param>
        /// <param name="heightend">The load's end height.</param>
        /// <param name="direction">The load's direction.</param>
        /// <param name="startvalue">The load's start value (to be used in labeling).</param>
        /// <param name="endvalue">The load's end value (to be used in labeling).</param>
        /// <param name="unitsize">The real length needed around the point in order to create an apparent unitary length in the viewport.</param>
        public void DrawLoad(Point3D start, Point3D end, double heightstart, double heightend, string direction, double startvalue=0, double endvalue= 0, double unitsize = 0)
        {
            var vdir = end - start;
            vdir.Normalize();

            var i = new Vector3D(1, 0, 0);
            var j = new Vector3D(0, 1, 0);
            var k = new Vector3D(0, 0, 1);

            var p2 = DrawHelper.CorrectRelativeSize(viewport, start, end, Math.Max(Math.Abs(heightend), Math.Abs(heightstart))/2);
            var nsteps = (int)((end - start).Length / (p2 - start).Length);
            var dir = i;
            var lines = linesX;


            var unit = new Point3D();

            switch (direction)
            {
                case "X":
                    unit = DrawHelper.CorrectRelativeSize(viewport, start, start + i, 1);
                    break;
                case "Y":
                    unit = DrawHelper.CorrectRelativeSize(viewport, start, start + j, 1);
                    dir = j;
                    lines = linesY;
                    break;
                case "Z":
                    unit = DrawHelper.CorrectRelativeSize(viewport, start, start + k, 1);
                    lines = linesZ;
                    dir = k; 
                    break;
            }
            var u = unitsize;

            if (u == 0)
                u = (unit - start).Length;

            double height = 0;
            double hstep = (heightend - heightstart) / nsteps;
            double length = 0;
            double lstep = (end - start).Length / nsteps;
            Point3D pos = new Point3D();

            var normal = Vector3D.CrossProduct((Vector3D)unit, vdir);

            for (int n = 0; n < nsteps; n++)
                {
                    length = n * lstep;
                    pos = start + length * vdir;
                    height = heightstart + (pos - start).Length * (heightend - heightstart) / (end - start).Length;

                    DrawHelper.DrawArrow(pos + height * u * dir, pos, normal, linesX, 0.1, 30);
                }

            DrawHelper.DrawArrow(end + heightend * u * dir, end, normal, linesX, 0.1, 30);
            linesX.Points.Add(end + heightend * u * dir);
            linesX.Points.Add(start + heightstart * u * dir);

            var lunit = PAENN.Models.UnitsHolder.Load;

            if (startvalue == endvalue)
            {
                var textpos = start + 0.5 * (end - start).Length * vdir + (heightstart + Math.Sign(heightstart) * 5) * u * dir;
                var text = new BillboardTextVisual3D { Text = startvalue.ToString() + " " + lunit, Position = textpos };
                textgroup.Children.Add(text);
            }
            else
            {
                var textpos = start + (heightstart + Math.Sign(heightstart) * 5) * u * dir;
                var textpos2 = end + (heightend + Math.Sign(heightend) * 5) * u * dir;
                var text = new BillboardTextVisual3D { Text = Math.Abs(startvalue).ToString() + " " + lunit, Position = textpos };
                var text2 = new BillboardTextVisual3D { Text = Math.Abs(endvalue).ToString() + " " + lunit, Position = textpos2 };

                if (startvalue != 0)
                    textgroup.Children.Add(text);
                if (endvalue != 0)
                    textgroup.Children.Add(text2);
            }
        }
    }

}
