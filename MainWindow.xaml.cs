using System;
using System.Windows;
using System.Windows.Media;
using PAENN.ViewModels;
using HelixToolkit.Wpf;
using Draw;
using System.Windows.Media.Media3D;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PAENN
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindow_VM VM;
        Window window;
        LinesVisual3D templines = new LinesVisual3D() { Thickness = 2, Color = Colors.MediumBlue };


        Point origin = new Point();         // 2D canvas' origin point
        Point canvasfirstpont = new Point();            // 2D canvas' anchor for mouse deltas
        Matrix transfmatrix = new Matrix();             // 2D canvas' transformation matrix

        public MainWindow()
        {
            InitializeComponent();
            VM = new MainWindow_VM();
            DataContext = VM;
            viewport.Children.Add(templines);

            origin = new Point(canvas2d.Width / 2, canvas2d.Height / 2);

            CB_Case.ItemsSource = VarHolder.LoadcasesList;
            canvasscroll.ScrollToVerticalOffset(origin.Y - 300);
            canvasscroll.ScrollToHorizontalOffset(origin.X  - 300);

            canvasscroll.UpdateLayout();
            View_Redraw();
        }


        #region Button command handling
        private void Button_NewNode_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinNewNode();
            window.Owner = this;
            window.Show();
        }

        private void Button_Materials_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinMaterials();
            window.Owner = this;
            window.Show();
        }

        private void Button_Sections_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinSections();
            window.Owner = this;
            window.Show();
        }

        private void Button_NewMember_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinNewMember();
            window.Owner = this;
            window.Show();
        }

        private void Button_Supports_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinSupports();
            window.Owner = this;
            window.Show();
        }

        private void Button_Nodal_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinNodalForces();
            window.Owner = this;
            window.Show();
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinMemberLoads();
            window.Owner = this;
            window.Show();
        }

        private void CloseWindows()
        {
            foreach (Window item in App.Current.Windows)
            {
                if (item != this)
                    item.Close();
            }
        }

        private void Button_Undo_Click(object sender, RoutedEventArgs e)
        {
            ActionHandler.Undo();
            View_Redraw();
        }

        private void Button_Redo_Click(object sender, RoutedEventArgs e)
        {
            ActionHandler.Redo();
            View_Redraw();
        }
        #endregion


        #region 3D viewport event handling
        public void View_Redraw()
        {
            VM.nodeContainer.Clear("viewport");
            VM.memberContainer.Clear();
            VM.supportContainer.Clear();
            VM.nodalContainer.Clear();
            VM.loadsContainer.Clear();

            var perp = viewport.Camera.LookDirection.FindAnyPerpendicular();
            var origin = new Point3D();

            var test = (DrawHelper.CorrectRelativeSize(viewport, origin, origin + 1 * perp, 1) - origin).Length;
            DrawAmbient.DrawAmbientLight(viewport);
            DrawAmbient.DrawAxes(viewport, test);
            templines.Points.Clear();

            foreach (Member member in VarHolder.MembersList)
            {
                var p1 = member.StartNode.Coords;
                var p2 = member.EndNode.Coords;

                var vdir = (p2 - p1) / (p2 - p1).Length;
                var pm = p1 + 0.5 * (p2 - p1).Length * vdir;
                var unitsize = (DrawHelper.CorrectRelativeSize(viewport, pm, pm + 1 * perp, 1) - pm).Length;

                VM.memberContainer.AddMember(p1, p2);
                VM.loadsContainer.AddLoad(p1, p2, member.MemberLoads, VarHolder.CurrentLoadcase, unitsize);
            }

            foreach (Node node in VarHolder.NodesList)
            {
                var coords = node.Coords;
                VM.nodeContainer.AddNode(coords, "viewport");
                var unitsize = (DrawHelper.CorrectRelativeSize(viewport, coords, coords + 1 * perp, 1) - coords).Length;

                VM.supportContainer.AddSupport(coords, node.Restr, unitsize);
                VM.nodalContainer.AddForce(coords, node.NodalForces, VarHolder.CurrentLoadcase, unitsize);
            }
        }

        private void viewport_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VarHolder.MaxMinForces();
            VarHolder.MaxMinLoads();
            bool found = false;
            foreach(Node node in VarHolder.NodesList)
            {
                if (VM.nodeContainer.CheckIfClicked(e, node.Coords, "viewport") && !found)
                {
                    switch (VarHolder.ClickType)
                    {
                        case "Select":
                            VM.nodeContainer.Select(node.Coords, "viewport");
                            break;
                        case "Support":
                            VM.ApplySupport(node);
                            View_Redraw();
                            break;
                        case "NewNode":
                            break;
                        case "NodalForces":
                            VM.ApplyNodalForces(node);
                            View_Redraw();
                            break;
                        case "NewMember_Start":
                            VM.NewMemberStart(node);
                            VM.nodeContainer.Select(node.Coords, "viewport");
                            break;
                        case "NewMember_End":
                            VM.NewMemberEnd(node, (Views.WinNewMember)window);
                            View_Redraw();
                            break;
                    }
                    found = true;
                }

                else
                    VM.nodeContainer.Unselect(node.Coords, "viewport");
            }

            foreach (Member member in VarHolder.MembersList)
            {
                var p1 = member.StartNode.Coords;
                var p2 = member.EndNode.Coords;

                if (VM.memberContainer.CheckIfClicked(e, p1, p2) && !found)
                {
                    switch (VarHolder.ClickType)
                    {
                        case "Select":
                            VM.memberContainer.Select(p1, p2);
                            break;
                        case "Support":
                            break;
                        case "NodalForces":
                            break;
                        case "MemberLoads":
                            VM.ApplyMemberLoads(member);
                            View_Redraw();
                            break;
                        case "NewMember_Start":
                            break;
                        case "NewMember_End":
                            break;
                    }
                    found = true;
                }
                   
                else
                    VM.memberContainer.Unselect(p1, p2);
            }

            if (!found)
            {
                var mousepos = viewport.CursorPosition.GetValueOrDefault();
                switch (VarHolder.ClickType)
                {
                    case "NewNode":
                        ActionHandler.NewNode(mousepos.X, mousepos.Y, mousepos.Z, 1);
                        break;
                }

            }
            View_Redraw();
        }

        private void viewport_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousepos = viewport.CursorPosition.GetValueOrDefault();

            var unit = Models.UnitsHolder.Length;

            var string1 = "X = " + String.Format("{0:0.00}", mousepos.X) + " " + unit;
            var string2 = " Y = " + String.Format("{0:0.00}", mousepos.Y) + " " + unit;
            var string3 = " Z = " + String.Format("{0:0.00}", mousepos.Z) + " " + unit;

            switch (VarHolder.GridNormal)
            {
                case "None":
                    break;
                case "X":
                    string1 = "";
                    break;
                case "Y":
                    string2 = "";
                    break;
                case "Z":
                    string3 = "";
                    break;
            }

            VM.CoordsText = string1 + string2 + string3;

            if (VarHolder.ClickType == "NewMember_End")
            {
                templines.Points.Clear();
                templines.Points.Add(VarHolder.StartNode.Coords);
                templines.Points.Add(mousepos);
            }
        }

        private void CB_Camera_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var sel = CB_Camera.SelectedIndex;

            if (viewport == null) return;

            if (sel == 0)
            {
                viewport.Visibility = Visibility.Visible;
                canvasscroll.Visibility = Visibility.Hidden;

                VarHolder.GridNormal = "None";
                View_Redraw();
            }

            else
            {
                viewport.Visibility = Visibility.Hidden;
                canvasscroll.Visibility = Visibility.Visible;

                switch (sel)
                {
                    case 1:
                        VarHolder.GridNormal = "Y";
                        break;
                    case 2:
                        VarHolder.GridNormal = "Z";
                        break;
                    case 3:
                        VarHolder.GridNormal = "X";
                        break;
                }
                Canvas_Redraw();
            }
        }

        private void viewport_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
            if (CB_Camera.SelectedIndex == 0)
                return;
            else
            {
                
                var dist = (viewport.Camera.Position - VM.CamStart).Length;
                var delta = e.Delta * 1 / (1 + Math.Exp(dist));
                viewport.CameraController.Zoom(delta);
            }
        }
        #endregion


        #region 2d canvas event handling
        private void canvas2d_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var transf = (MatrixTransform)canvas2d.RenderTransform;
            var pos = e.GetPosition(canvas2d);

            var f = e.Delta > 0 ? 1.02 : 1 / 1.02;

            var matrix = transf.Matrix;
            matrix.ScaleAt(f, f, pos.X, pos.Y);
            transf.Matrix = matrix;
            transfmatrix = matrix;

            VM.nodeContainer.Rescale();
            e.Handled = true;
        }

        private void canvas2d_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvasfirstpont = e.GetPosition(this);
        }

        public void Canvas_Redraw()
        {
            VM.nodeContainer.Clear("canvas");

            foreach(Node node in VarHolder.NodesList)
            {
                VM.nodeContainer.AddNode(node.Coords, "canvas", origin.X, origin.Y);
            }
           
        }

        private void canvas2d_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvas2d_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                var delta = e.GetPosition(this) - canvasfirstpont;
                canvasscroll.ScrollToVerticalOffset(canvasscroll.VerticalOffset - delta.Y);
                canvasscroll.ScrollToHorizontalOffset(canvasscroll.HorizontalOffset - delta.X);

                canvasfirstpont = e.GetPosition(this);
            }

            var mousepos = e.GetPosition(canvas2d);
            var unit = Models.UnitsHolder.Length;

            var string1 = "";
            var string2 = "";
            var string3 = "";

            switch (VarHolder.GridNormal)
            {
                case "X":
                    string1 = "";
                    string2 = " Y = " + String.Format("{0:0.00}", mousepos.X - origin.X) + " " + unit;
                    string3 = " Z = " + String.Format("{0:0.00}", mousepos.Y - origin.Y) + " " + unit;
                    break;
                case "Y":
                    string1 = "X = " + String.Format("{0:0.00}", mousepos.X - origin.X) + " " + unit;
                    string2 = "";
                    string3 = " Z = " + String.Format("{0:0.00}", mousepos.Y - origin.Y) + " " + unit;
                    break;
                case "Z":
                    string1 = "X = " + String.Format("{0:0.00}", mousepos.X - origin.X) + " " + unit;
                    string2 = " Y = " + String.Format("{0:0.00}", mousepos.Y - origin.Y) + " " + unit;
                    string3 = "";
                    break;
            }

            VM.CoordsText = string1 + string2 + string3;
        }
        #endregion

        private void canvas2d_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (Node node in VarHolder.NodesList)
            {
                var clicked = VM.nodeContainer.CheckIfClicked(e, node.Coords, "canvas", origin.X, origin.Y);
                if (clicked)
                {
                    VM.nodeContainer.Select(node.Coords, "canvas", origin.X, origin.Y);
                    Console.WriteLine("yo");
                }
                else
                    VM.nodeContainer.Unselect(node.Coords, "canvas", origin.X, origin.Y);
            }
        }
    }
}
