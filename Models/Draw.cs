using HelixToolkit.Wpf;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Helper;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PAENN.ViewModels;
using System.Windows.Shapes;

namespace Draw
{
    public static class DrawHelper
    {
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

        public static bool IsInViewport(HelixViewport3D viewport, Point3D P)
        {
            var q = Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, P);

            if (q.X > viewport.ActualWidth || q.X < 0)
                return false;
            else if (q.Y > viewport.ActualHeight || q.Y < 0)
                return false;

            else return true;
        }

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

        public static void DrawArrow(Point3D Start, Point3D End, Vector3D Normal, LinesVisual3D lines, double ArrowLength=0.3, double Angle=45, bool DoubleArrow = false, double unitsize = 0)
        {
            var i = Start - End;
            var length = ArrowLength * i.Length;
            i.Normalize();

            var j = Vector3D.CrossProduct(Normal, i);
            j.Normalize();

            var tip1 = End + length * (i + j);
            var tip2 = End + length * (i - j);

            lines.Points.Add(Start);
            lines.Points.Add(End);
            lines.Points.Add(End);
            lines.Points.Add(tip1);
            lines.Points.Add(End);
            lines.Points.Add(tip2);

            if (DoubleArrow)
            {
                tip1 = Start + length * (-i + Math.Tan(Angle * Math.PI/180) * j);
                tip2 = Start + length * (-i - Math.Tan(Angle * Math.PI / 180) * j);

                lines.Points.Add(Start);
                lines.Points.Add(tip1);
                lines.Points.Add(Start);
                lines.Points.Add(tip2);
            }

        }
    }

    public static class DrawAmbient
    {
        public static LinesVisual3D X_Axis = new LinesVisual3D()
        {
            Color = Colors.Red,
            Thickness = 1,
        };

        public static LinesVisual3D Y_Axis = new LinesVisual3D()
        {
            Color = Colors.Green,
            Thickness = 1,
        };

        public static LinesVisual3D Z_Axis = new LinesVisual3D()
        {
            Color = Colors.Blue,
            Thickness = 1,
        };

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

    public class NodeContainer
    {
        // 3D rendering parameters
        private HelixViewport3D viewport;
        public PointsVisual3D SelectedNodes = new PointsVisual3D() { Size = 10, Color = Colors.Red };
        public PointsVisual3D UnselectedNodes= new PointsVisual3D() { Size = 8, Color = Colors.Black };
        
        // 2D rendering parameters
        private Canvas canvas;
        public List<Ellipse> Nodes2D = new List<Ellipse>();
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
        ///  Adds a new node to the viewport or canvas.
        /// </summary>
        /// <param name="point">The point to be added.</param>
        /// <param name="type">Whether to add to the viewport (3D) or canvas (2D).</param>
        /// <param name="originX">X-coordinate of the origin, relative to the canvas' upper-left corner.</param>
        /// <param name="originY">Y-coordinate of the origin, relative to the canvas' upper-left corner.</param>
        public void AddNode(Point3D point, string type, double originX=0, double originY=0)
        {

            if (type == "viewport")
            {
                if (UnselectedNodes.Points.Contains(point) || SelectedNodes.Points.Contains(point))
                    return;

                UnselectedNodes.Points.Add(point);
                return;
            }


            var point2d = Functions.Point3DToCanvas(point, VarHolder.GridNormal);
            var p = new Point(point2d.X + originX, -point2d.Y + originY);


            if (NodesList.Contains(p))
                return;

            var scale = ((MatrixTransform)canvas.RenderTransform).Matrix.M11;

            var n = new Ellipse { Height = 6 / scale, Width = 6 / scale, Fill = Brushes.Black };
            Canvas.SetLeft(n, p.X - 3 / scale);
            Canvas.SetTop(n, p.Y - 3 / scale);

            Nodes2D.Add(n);
            NodesList.Add(p);
            canvas.Children.Add(n);

        }


        /// <summary>
        /// Changes a given node to be displayed as selected in the viewport or canvas.
        /// </summary>
        /// <param name="point">The point to be selected.</param>
        /// <param name="type">Whether to select it in the viewport (3D) .</param>
        /// <param name="originX">X-coordinate of the origin, relative to the canvas' upper-left corner.</param>
        /// <param name="originY">Y-coordinate of the origin, relative to the canvas' upper-left corner.</param>
        public void Select(Point3D point, string type, double originX=0, double originY=0)
        {
            if (type == "viewport")
            {
                if (!UnselectedNodes.Points.Contains(point))
                    return;

                UnselectedNodes.Points.Remove(point);
                SelectedNodes.Points.Add(point);
                return;
            }

            var point2d = Functions.Point3DToCanvas(point, VarHolder.GridNormal);
            point2d = new Point(point2d.X + originX, -point2d.Y + originY);

            if (!NodesList.Contains(point2d))
                return;

            for(int i=0; i<NodesList.Count; i++)
            {
                if(NodesList[i] == point2d)
                {
                    var N = (Ellipse)Nodes2D[i];
                    N.Fill = Brushes.Red;
                }
            }

        }


        /// <summary>
        /// Changes a given node to be displayed as not-selected in the viewport or canvas.
        /// </summary>
        /// <param name="point">The point to be unselected.</param>
        /// <param name="type">Whether to select it in the viewport (3D) or canvas (2D).</param>
        /// <param name="originX">X-coordinate of the origin, relative to the canvas' upper-left corner.</param>
        /// <param name="originY">Y-coordinate of the origin, relative to the canvas' upper-left corner.</param>
        public void Unselect(Point3D point, string type, double originX = 0, double originY = 0)
        {
            if (type == "viewport")
            {
                if (!SelectedNodes.Points.Contains(point))
                    return;

                UnselectedNodes.Points.Add(point);
                SelectedNodes.Points.Remove(point);
                return;
            }

            var point2d = Functions.Point3DToCanvas(point, VarHolder.GridNormal);
            point2d = new Point(point2d.X + originX, -point2d.Y + originY);

            if (!NodesList.Contains(point2d))
                return;

            for (int i = 0; i < NodesList.Count; i++)
            {
                if (NodesList[i] == point2d)
                {
                    var N = (Ellipse)Nodes2D[i];
                    N.Fill = Brushes.Black;
                }
            }

        }


        /// <summary>
        /// Check if a point is being clicked in the viewport or canvas.
        /// </summary>
        /// <param name="e">The mouse-click event.</param>
        /// <param name="point3d">The point to be checked.</param>
        /// <param name="type">Whether to check the viewport (3D) or canvas (2D).</param>
        /// <param name="originX">X-coordinate of the origin, relative to the canvas' upper-left corner.</param>
        /// <param name="originY">Y-coordinate of the origin, relative to the canvas' upper-left corner.</param>
        /// <returns></returns>
        public bool CheckIfClicked(MouseButtonEventArgs e, Point3D point3d, string type, double originX=0, double originY=0)
        {
            // Defaults to check the canvas
            var point2d = Functions.Point3DToCanvas(point3d, VarHolder.GridNormal);
            point2d = new Point(point2d.X + originX, -point2d.Y + originY);
            var click = e.GetPosition(canvas);

            // If it's the viewport, change the values accordingly
            if (type == "viewport")
            {
                point2d = Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, point3d);
                click = e.GetPosition(viewport);
            }

            // Returns the distance between the point and the cursor
            return (click - point2d).Length < 6;
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
                var N = (Ellipse)Nodes2D[i];
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
            if (type == "canvas")
            {
                NodesList.Clear();
                Nodes2D.Clear();
                return;
            }

            SelectedNodes.Points.Clear();
            UnselectedNodes.Points.Clear();
        }

    }

    public class MemberContainer
    {

        public LinesVisual3D SelectedMembers = new LinesVisual3D() { Thickness = 3, Color = Colors.DarkMagenta };

        public LinesVisual3D UnselectedMembers = new LinesVisual3D() { Thickness = 2, Color = Colors.SteelBlue };

        private HelixViewport3D viewport;


        public MemberContainer(HelixViewport3D viewport3D)
        {
            viewport = viewport3D;

            viewport.Children.Add(SelectedMembers);
            viewport.Children.Add(UnselectedMembers);
        }

        public void AddMember(Point3D P1, Point3D P2)
        {
            if (UnselectedMembers.Points.Contains(P1))
            {
                var i = UnselectedMembers.Points.IndexOf(P1);
                if (i != UnselectedMembers.Points.Count - 1)
                {
                    if (UnselectedMembers.Points[i + 1] == P2)
                        return;
                }

            }

            if (SelectedMembers.Points.Contains(P1))
            {
                var i = SelectedMembers.Points.IndexOf(P1);

                if (i != SelectedMembers.Points.Count - 1)
                {
                    if (SelectedMembers.Points[i + 1] == P2)
                        return;
                }
            }

            UnselectedMembers.Points.Add(P1);
            UnselectedMembers.Points.Add(P2);
        }

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

            if (d > 4)
                return false;

            if (click.X < xmin - 2 || click.Y < ymin - 2)
                return false;

            if (click.X > xmax + 2 || click.Y > xmax + 2)
                return false;

            return true;
        }

        public void Clear()
        {
            for (int i = SelectedMembers.Points.Count; i > 0; i--)
                SelectedMembers.Points.RemoveAt(i - 1);

            for (int i = UnselectedMembers.Points.Count; i > 0; i--)
                UnselectedMembers.Points.RemoveAt(i - 1);
        }
    }

    public class SupportContainer
    {
        private LinesVisual3D linesupports = new LinesVisual3D { Color = Colors.Black, Thickness = 1.5 };
        private Model3DGroup supports = new Model3DGroup();
        private ModelVisual3D supportsvisual = new ModelVisual3D();
        private HelixViewport3D viewport;

        public SupportContainer(HelixViewport3D viewport3D)
        {
            viewport = viewport3D;
            supportsvisual.Content = supports;
            viewport.Children.Add(supportsvisual);
            viewport.Children.Add(linesupports);
        }

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
                AddCart(P, 0, dist);

            else if (r["Ux"] && r["Uz"])
                AddCart(P, 1, dist);

            else if (r["Uy"] && r["Uz"])
                AddCart(P, 2, dist);

            else if (r["Ux"])
                AddSphere(P, 0, dist);

            else if (r["Uy"])
                AddSphere(P, 1, dist);

            else if (r["Uz"])
                AddSphere(P, 2, dist);

            if (r["Rx"] && r["Ry"] && r["Rz"])
            {
                AddFullRotFix(P, dist);
                return;
            }

            if (r["Rx"])
                AddRotationFixer(P, 0, dist);
            if (r["Ry"])
                AddRotationFixer(P, 1, dist);
            if (r["Rz"])
                AddRotationFixer(P, 2, dist);
        }

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

        private void AddSphere(Point3D P, int type, double dist)
        {
            var builder = new MeshBuilder();
            var color = Colors.Firebrick;

            switch (type)
            {
                case 0:
                    builder.AddSphere(new Point3D(P.X - dist / 2, P.Y, P.Z), dist / 2);
                    break;
                case 1:
                    builder.AddSphere(new Point3D(P.X, P.Y - dist / 2, P.Z), dist / 2);
                    break;
                case 2:
                    builder.AddSphere(new Point3D(P.X, P.Y, P.Z - dist / 2), dist / 2);
                    break;
            }
            var geometry = builder.ToMesh();
            supports.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(color) });
        }

        private void AddFixed(Point3D P, double dist)
        {
            var builder = new MeshBuilder();
            var color = Colors.DarkGreen;

            builder.AddPyramid(new Point3D(P.X, P.Y, P.Z - dist / 4), dist, dist / 2, true);
            var geometry = builder.ToMesh();
            supports.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(color) });
        }

        private void AddRotationFixer(Point3D P, int type, double dist)
        {
            var P1 = new Point3D();
            var P2 = new Point3D();
            var P3 = new Point3D();
            var P4 = new Point3D();

            switch (type)
            {
                case 0:
                    P1 = new Point3D(P.X, P.Y - dist / 2, P.Z - dist / 2);
                    P2 = new Point3D(P.X, P.Y + dist / 2, P.Z - dist / 2);
                    P3 = new Point3D(P.X, P.Y + dist / 2, P.Z + dist / 2);
                    P4 = new Point3D(P.X, P.Y - dist / 2, P.Z + dist / 2);
                    break;
                case 1:
                    P1 = new Point3D(P.X - dist / 2, P.Y, P.Z - dist / 2);
                    P2 = new Point3D(P.X + dist / 2, P.Y, P.Z - dist / 2);
                    P3 = new Point3D(P.X + dist / 2, P.Y, P.Z + dist / 2);
                    P4 = new Point3D(P.X - dist / 2, P.Y, P.Z + dist / 2);
                    break;
                case 2:
                    P1 = new Point3D(P.X - dist / 2, P.Y - dist / 2, P.Z);
                    P2 = new Point3D(P.X + dist / 2, P.Y - dist / 2, P.Z);
                    P3 = new Point3D(P.X + dist / 2, P.Y + dist / 2, P.Z);
                    P4 = new Point3D(P.X - dist / 2, P.Y + dist / 2, P.Z);
                    break;
            }
            DrawHelper.DrawRectangle(P1, P2, P3, P4, ref linesupports);
        }

        private void AddFullRotFix(Point3D P, double dist)
        {
            var rect = new Rect3D(new Point3D(P.X - dist / 2, P.Y - dist / 2, P.Z - dist / 2), new Size3D(dist, dist, dist));
            DrawHelper.DrawWireframeBox(rect, ref linesupports);
        }

        private void AddCart(Point3D P, int type, double dist)
        {
            var builder = new MeshBuilder();
            var builder2 = new MeshBuilder();

            switch (type)
            {
                case 0:     // Ux, Uy
                    builder2.AddCylinder(new Point3D(P.X - 0.25 * dist, P.Y - dist / 4, P.Z),
                                         new Point3D(P.X - 0.25 * dist, P.Y - dist / 3, P.Z), dist / 8);

                    builder2.AddCylinder(new Point3D(P.X - 0.25 * dist, P.Y + dist / 4, P.Z),
                                         new Point3D(P.X - 0.25 * dist, P.Y + dist / 3, P.Z), dist / 8);
                    builder.AddBox(new Rect3D(new Point3D(P.X - dist / 8, P.Y - dist / 3, P.Z - dist / 8),
                                   new Size3D(dist / 8, 2 * dist / 3, dist / 4)));
                    break;
                
                case 1:     // Ux, Uz
                    builder2.AddCylinder(new Point3D(P.X - 0.25 * dist, P.Y, P.Z - dist / 4),
                     new Point3D(P.X - 0.25 * dist, P.Y, P.Z - dist / 3), dist / 8);
                    builder2.AddCylinder(new Point3D(P.X - 0.25 * dist, P.Y, P.Z + dist / 4),
                                         new Point3D(P.X - 0.25 * dist, P.Y, P.Z + dist / 3), dist / 8);
                    builder.AddBox(new Rect3D(new Point3D(P.X - dist / 8, P.Y - dist / 8, P.Z - dist / 3),
                                   new Size3D(dist / 8, dist / 4, 2 * dist / 3)));
                    break;

                case 2:     // Uy, Uz
                    builder2.AddCylinder(new Point3D(P.X, P.Y - 0.25 * dist, P.Z - dist / 4),
                     new Point3D(P.X, P.Y - 0.25 * dist, P.Z - dist / 3), dist / 8);
                    builder2.AddCylinder(new Point3D(P.X, P.Y - 0.25 * dist, P.Z + dist / 4),
                                         new Point3D(P.X, P.Y - 0.25 * dist, P.Z + dist / 3), dist / 8);
                    builder.AddBox(new Rect3D(new Point3D(P.X - dist / 8, P.Y - dist / 8, P.Z - dist / 3),
                                   new Size3D(dist / 4, dist / 8, 2 * dist / 3)));
                    break;
            }
            var geometry = builder.ToMesh();
            var geometry2 = builder2.ToMesh();
            supports.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(Colors.DarkMagenta) });
            supports.Children.Add(new GeometryModel3D { Geometry = geometry2, Material = MaterialHelper.CreateMaterial(Colors.Black) });
        }

        private void AddEncastre(Point3D P, double dist)
        {
            var builder = new MeshBuilder();
            builder.AddCylinder(P, new Point3D(P.X, P.Y, P.Z - dist / 3), dist);
            var geometry = builder.ToMesh();
            supports.Children.Add(new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(Colors.SaddleBrown) });
        }

    }

    public class NodalForcesContainer
    {
        private LinesVisual3D lines = new LinesVisual3D { Color = Colors.DarkBlue, Thickness = 1.5 };
        private Model3DGroup forces = new Model3DGroup();
        private ModelVisual3D visual_forces = new ModelVisual3D();
        private BillboardTextGroupVisual3D textgroup = new BillboardTextGroupVisual3D();
        private HelixViewport3D viewport;

        public NodalForcesContainer(HelixViewport3D viewport3D)
        {
            viewport = viewport3D;
            visual_forces.Content = forces;
            viewport.Children.Add(visual_forces);
            viewport.Children.Add(lines);
            viewport.Children.Add(textgroup);
        }

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

        public void AddForce(Point3D point, Dictionary<string, List<double>> forces, int loadcase, double unitsize)
        {
            var funit = PAENN.Models.UnitsHolder.Force;

            if (forces["Fx"][loadcase] > 0)
                DrawForce(point, "Fx", GetArrowSize(forces["Fx"][loadcase], 0), forces["Fx"][loadcase].ToString() + " " + funit, false, unitsize);
            else if (forces["Fx"][loadcase] < 0)
                DrawForce(point, "Fx", GetArrowSize(forces["Fx"][loadcase], 0), Math.Abs(forces["Fx"][loadcase]).ToString() + " " + funit, true, unitsize);

            if (forces["Fy"][loadcase] > 0)
                DrawForce(point, "Fy", GetArrowSize(forces["Fy"][loadcase], 0), forces["Fy"][loadcase].ToString() + " " + funit, false, unitsize);
            else if (forces["Fy"][loadcase] < 0)
                DrawForce(point, "Fy", GetArrowSize(forces["Fy"][loadcase], 0), Math.Abs(forces["Fy"][loadcase]).ToString() + " " + funit, true, unitsize);

            if (forces["Fz"][loadcase] > 0)
                DrawForce(point, "Fz", GetArrowSize(forces["Fz"][loadcase], 0), forces["Fz"][loadcase].ToString() + " " + funit, false, unitsize);
            else if (forces["Fz"][loadcase] < 0)
                DrawForce(point, "Fz", GetArrowSize(forces["Fz"][loadcase], 0), Math.Abs(forces["Fz"][loadcase]).ToString() + " " + funit, true, unitsize);

            if (forces["Mx"][loadcase] > 0)
                DrawForce(point, "Mx", GetArrowSize(forces["Mx"][loadcase], 0), forces["Mx"][loadcase].ToString() + " " + funit, false, unitsize);
            else if (forces["Mx"][loadcase] < 0)
                DrawForce(point, "Mx", GetArrowSize(forces["Mx"][loadcase], 0), Math.Abs(forces["Mx"][loadcase]).ToString() + " " + funit, true, unitsize);

            if (forces["My"][loadcase] > 0)
                DrawForce(point, "My", GetArrowSize(forces["My"][loadcase], 0), forces["My"][loadcase].ToString() + " " + funit, false, unitsize);
            else if (forces["My"][loadcase] < 0)
                DrawForce(point, "My", GetArrowSize(forces["My"][loadcase], 0), Math.Abs(forces["My"][loadcase]).ToString() + " " + funit, true, unitsize);

            if (forces["Mz"][loadcase] > 0)
                DrawForce(point, "Mz", GetArrowSize(forces["Mz"][loadcase], 0), forces["Mz"][loadcase].ToString() + " " + funit, false, unitsize);
            else if (forces["Mz"][loadcase] < 0)
                DrawForce(point, "Mz", GetArrowSize(forces["Mz"][loadcase], 0), Math.Abs(forces["Mz"][loadcase]).ToString() + " " + funit, true, unitsize);
        }

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

    public class MemberLoadsContainer
    {
        public LinesVisual3D linesX = new LinesVisual3D() { Color = Colors.Red, Thickness = 2 };
        public LinesVisual3D linesY = new LinesVisual3D() { Color = Colors.Green, Thickness = 2 };
        public LinesVisual3D linesZ = new LinesVisual3D() { Color = Colors.Blue, Thickness = 2 };

        private HelixViewport3D viewport;
        private BillboardTextGroupVisual3D textgroup = new BillboardTextGroupVisual3D();

        public MemberLoadsContainer(HelixViewport3D viewport3D)
        {
            viewport = viewport3D;

            viewport.Children.Add(linesX);
            viewport.Children.Add(linesY);
            viewport.Children.Add(linesZ);
            viewport.Children.Add(textgroup);
        }

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


        public void AddLoad(Point3D start, Point3D end, Dictionary<string, List<double>> loads, int loadcase, double unitsize=0)
        {
            if (loads["Qx0"][loadcase] != 0 || loads["Qx1"][loadcase] != 0)
                DrawLoad(start, end, GetArrowSize(loads["Qx0"][loadcase]), GetArrowSize(loads["Qx1"][loadcase]),
                         "X", loads["Qx0"][loadcase], loads["Qx1"][loadcase], unitsize);

            if (loads["Qy0"][loadcase] != 0 || loads["Qy1"][loadcase] != 0)
                DrawLoad(start, end, GetArrowSize(loads["Qy0"][loadcase]), GetArrowSize(loads["Qy1"][loadcase]),
                         "Y", loads["Qy0"][loadcase], loads["Qy1"][loadcase], unitsize);

            if (loads["Qz0"][loadcase] != 0 || loads["Qz1"][loadcase] != 0)
                DrawLoad(start, end, GetArrowSize(loads["Qz0"][loadcase]), GetArrowSize(loads["Qz1"][loadcase]),
                         "Z", loads["Qz0"][loadcase], loads["Qz1"][loadcase], unitsize);
        }

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

    public class GridContainer
    {
        private LinesVisual3D lines = new LinesVisual3D { Thickness = 0.8 };
        private HelixViewport3D viewport;
        private double radius = 0;

        private Point3D upperleft = new Point3D();
        private Point3D lowerleft = new Point3D();
        private Point3D upperright = new Point3D();
        private Point3D lowerright = new Point3D();

        public GridContainer(HelixViewport3D viewport3D)
        {
            viewport = viewport3D;
            viewport.Children.Add(lines);
            radius = 20000;
        }

        public void Clear()
        {
            lines.Points.Clear();
        }

        public void DrawGrid(Point3D center, Vector3D dir1, Vector3D dir2, double dist1, double dist2, Color color)
        {
            lines.Color = color;
            var p1 = Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, new Point3D());
            var p2 = new Point(viewport.ActualWidth / 2, viewport.ActualHeight / 2);

            radius = Math.Max(3 * (p2 - p1).Length, 20000);
            int n1 = (int)(radius / dist1);
            int n2 = (int)(radius / dist2);

            for(int i=0; i<n1; i++)
            {
                lines.Points.Add(center + (i * dist1) * dir1 + (radius) * dir2);
                lines.Points.Add(center + (i * dist1) * dir1 - (radius) * dir2);

                lines.Points.Add(center - (i * dist1) * dir1 + (radius) * dir2);
                lines.Points.Add(center - (i * dist1) * dir1 - (radius) * dir2);
            }

            for(int j=0; j<n2; j++)
            {
                lines.Points.Add(center + (radius) * dir1 + (j * dist2) * dir2);
                lines.Points.Add(center - (radius) * dir1 + (j * dist2) * dir2);

                lines.Points.Add(center + (radius) * dir1 - (j * dist2) * dir2);
                lines.Points.Add(center - (radius) * dir1 - (j * dist2) * dir2);
            }

            upperleft = center - radius * dir1 + radius * dir2;
            lowerleft = center - radius * dir1 - radius * dir2;
            upperright = center + radius * dir1 + radius * dir2;
            lowerright = center + radius * dir1 - radius * dir2;
        }

        public bool NeedsRedraw(double tol)
        {
            var L1 = new List<Point>();
            var center = new Point(viewport.ActualWidth / 2, viewport.ActualHeight / 2);

            L1.Add(Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, upperleft));
            L1.Add(Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, lowerleft));
            L1.Add(Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, upperright));
            L1.Add(Viewport3DHelper.Point3DtoPoint2D(viewport.Viewport, lowerright));

            for(int i=0; i<4; i++)
            {
                if ((L1[i] - center).Length < tol) return true;
            }
            return false;
        }

    }
}
