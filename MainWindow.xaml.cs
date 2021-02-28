using System;
using System.Windows;
using System.Windows.Media;
using PAENN.ViewModels;
using HelixToolkit.Wpf;
using Draw;
using System.Windows.Media.Media3D;
using System.Windows.Input;


namespace PAENN
{
    /// <summary>
    /// Main window internal logic.
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindow_VM VM;           // Main window ViewModel
        Window window;              // Dummy instance used for every other window

        // Temporary 3D line container, used for drawing the temporary member when only one node is selected.
        LinesVisual3D templines = new LinesVisual3D() { Thickness = 2, Color = Colors.MediumBlue };

        Point origin = new Point();         // 2D canvas' origin point
        Point canvasfirstpont = new Point();            // 2D canvas' anchor for mouse deltas
        Matrix transfmatrix = new Matrix();             // 2D canvas' transformation matrix

        /// <summary>
        /// MainWindow class constructor.
        /// </summary>
        public MainWindow()
        {
            // Window and ViewModel initialization
            InitializeComponent();
            VM = new MainWindow_VM();
            DataContext = VM;
            CB_Case.ItemsSource = VarHolder.LoadcasesList;

            // Canvas and viewport initialization
            viewport.Children.Add(templines);
            canvasscroll.ScrollToVerticalOffset(origin.Y - 300);
            canvasscroll.ScrollToHorizontalOffset(origin.X - 300);
            canvasscroll.UpdateLayout();
            origin = new Point(canvas2d.Width / 2, canvas2d.Height / 2);
            View_Redraw();
        }



        /// <summary>
        /// Closes every other window in the application.
        /// </summary>
        private void CloseWindows()
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item != this)
                    item.Close();
            }
        }


        /// <summary>
        /// Handles the Camera type combobox change.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void CB_Camera_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Gets the Combobox selection.
            var sel = CB_Camera.SelectedIndex;

            // If either the viewport or canvas are not loaded, return.
            if (viewport == null || canvas2d == null) return;

            // Is "3D view" is selected, hide the canvas and show the viewport.
            if (sel == 0)
            {
                viewport.Visibility = Visibility.Visible;
                canvasscroll.Visibility = Visibility.Hidden;

                VarHolder.GridNormal = "None";
                View_Redraw();
            }

            // Otherwise, hide the viewport and show the canvas.
            else
            {
                viewport.Visibility = Visibility.Hidden;
                canvasscroll.Visibility = Visibility.Visible;

                // Change the normal vector to the canvas plane, depending on the option selected
                switch (sel)
                {
                    case 1:     // X-Z plane
                        VarHolder.GridNormal = "Y";
                        break;
                    case 2:     // X-Y plane
                        VarHolder.GridNormal = "Z";
                        break;
                    case 3:     // Y-Z plane
                        VarHolder.GridNormal = "X";
                        break;
                }
                Canvas_Redraw();
            }
        }



        #region Button command handling
        /// <summary>
        /// Handles the Add New Node button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_NewNode_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinNewNode();
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Handles the Manage Materials button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Materials_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinMaterials();
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Handles the Manage Sections button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Sections_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinSections();
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Handles the Add New Member button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_NewMember_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinNewMember();
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Handles the Add Supports button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Supports_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinSupports();
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Handles the Add Nodal Forces button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Nodal_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinNodalForces();
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Handles the Add Member Loads button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            CloseWindows();
            window = new Views.WinMemberLoads();
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Handles the Undo button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Undo_Click(object sender, RoutedEventArgs e)
        {
            ActionHandler.Undo();
            View_Redraw();
        }

        /// <summary>
        /// Handles the Redo button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Redo_Click(object sender, RoutedEventArgs e)
        {
            ActionHandler.Redo();
            View_Redraw();
        }
        #endregion



        #region 3D viewport event handling
        /// <summary>
        /// Clears the 3D viewport and redraws everything.
        /// </summary>
        public void View_Redraw()
        {
            // Clears the viewport containers.
            VM.nodeContainer.Clear("viewport");
            VM.memberContainer.Clear("viewport");
            VM.supportContainer.Clear();
            VM.nodalContainer.Clear();
            VM.loadsContainer.Clear();
            templines.Points.Clear();

            // Draws the ambient lights.
            DrawAmbient.DrawAmbientLight(viewport);

            // Draws the ordinate axes at the origin.
            var perp = viewport.Camera.LookDirection.FindAnyPerpendicular();
            var origin = new Point3D();
            var unit = (DrawHelper.CorrectRelativeSize(viewport, origin, origin + 1 * perp, 1) - origin).Length;
            DrawAmbient.DrawAxes(viewport, unit);


            // For each member, draws the memberline and any existing loads.
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

            // For each node, draws the node point, any existing supports and forces.
            foreach (Node node in VarHolder.NodesList)
            {
                var coords = node.Coords;
                VM.nodeContainer.AddNode(coords);
                var unitsize = (DrawHelper.CorrectRelativeSize(viewport, coords, coords + 1 * perp, 1) - coords).Length;

                VM.supportContainer.AddSupport(coords, node.Restr, unitsize);
                VM.nodalContainer.AddForce(coords, node.NodalForces, VarHolder.CurrentLoadcase, unitsize);
            }
        }



        /// <summary>
        /// Handles the viewport's LMB clicking.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Finds and stores the current maxima and minima of loads/forces.
            VarHolder.MaxMinForces();
            VarHolder.MaxMinLoads();

            // Indicates whether a clicked object has been fount
            bool found = false;

            // Iterate through every node in the structure
            foreach(Node node in VarHolder.NodesList)
            {
                // If the node is being clicked and nothing else was found before it...
                if (VM.nodeContainer.CheckIfClicked(e, node.Coords) && !found)
                {
                    switch (VarHolder.ClickType)
                    {
                        case "Select":          // Select the node
                            VM.nodeContainer.Select(node.Coords);
                            break;
                        case "Support":         // Apply the current support conditions
                            VM.ApplySupport(node);
                            View_Redraw();
                            break;
                        case "NodalForces":     // Apply the current nodal forces
                            VM.ApplyNodalForces(node);
                            View_Redraw();
                            break;
                        case "NewMember_Start": // Select the node as the new member's starting point
                            VM.NewMemberStart(node);
                            VM.nodeContainer.Select(node.Coords);
                            break;
                        case "NewMember_End":   // Create a member between the last clicked node and the current one
                            VM.NewMemberEnd(node, (Views.WinNewMember)window);
                            View_Redraw();
                            break;
                    }
                    found = true;       // Signal that something clickable has been found
                }

                // Otherwise, signal the node as not-selected
                else
                    VM.nodeContainer.Unselect(node.Coords);
            }

            // iterate through every member in the structure 
            foreach (Member member in VarHolder.MembersList)
            {
                var p1 = member.StartNode.Coords;
                var p2 = member.EndNode.Coords;

                // If the member is being clicked and nothing else was found before it...
                if (VM.memberContainer.CheckIfClicked(e, p1, p2) && !found)
                {
                    switch (VarHolder.ClickType)
                    {
                        case "Select":              // Select the node
                            VM.memberContainer.Select(p1, p2);
                            break;
                        case "MemberLoads":         // Apply the current distributed loads
                            VM.ApplyMemberLoads(member);
                            View_Redraw();
                            break;
                    }
                    found = true;        // Signal that something clickable has been found
                }
                
                // Otherwise, signal the member as not-selected
                else
                    VM.memberContainer.Unselect(p1, p2);
            }

            // If no node or member was clicked on...
            if (!found)
            {
                // Get the mouse position in the viewport
                var mousepos = viewport.CursorPosition.GetValueOrDefault();
                switch (VarHolder.ClickType)
                {
                    case "NewNode":     // Creates a new node at the mouse position
                        ActionHandler.NewNode(mousepos.X, mousepos.Y, mousepos.Z, 1);
                        break;
                }

            }
            View_Redraw();
        }



        /// <summary>
        /// Handles the viewport's mouse movement event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void viewport_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Gets the current mouse position (converted to a 3D point) in the viewport.
            var mousepos = viewport.CursorPosition.GetValueOrDefault();

            // Updates the statusbar's coordinates text.
            var unit = Models.UnitsHolder.Length;
            var string1 = "X = " + String.Format("{0:0.00}", mousepos.X) + " " + unit;
            var string2 = " Y = " + String.Format("{0:0.00}", mousepos.Y) + " " + unit;
            var string3 = " Z = " + String.Format("{0:0.00}", mousepos.Z) + " " + unit;
            VM.CoordsText = string1 + string2 + string3;

            // If adding a member and already selected a start node, draw a temp
            // node between that node and the cursor.
            if (VarHolder.ClickType == "NewMember_End")
            {
                templines.Points.Clear();
                templines.Points.Add(VarHolder.StartNode.Coords);
                templines.Points.Add(mousepos);
            }
        }
        #endregion



        #region 2D canvas event handling
        /// <summary>
        /// Handles the canvas' mouse wheel scroll event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void canvas2d_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // Gets the canvas' current transform matrix
            var transf = (MatrixTransform)canvas2d.RenderTransform;
            var matrix = transf.Matrix;

            // Gets the current cursor position
            var pos = e.GetPosition(canvas2d);

            // Scales the canvas matrix based on the scroll direction, centered at the cursor position
            var f = e.Delta > 0 ? 1.02 : 1 / 1.02;
            matrix.ScaleAt(f, f, pos.X, pos.Y);
            transf.Matrix = transfmatrix = matrix;

            // Rescales everything so they keep their constant apparent size.
            VM.nodeContainer.Rescale();
            VM.memberContainer.Rescale();

            // Signal that the event has been handled.
            e.Handled = true;
        }



        /// <summary>
        /// Handles the canvas' mouse button press event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void canvas2d_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Stores the click point (relative to the window), used for drag deltas.
            canvasfirstpont = e.GetPosition(this);
        }



        /// <summary>
        /// Clears the canvas and redraws everything.
        /// </summary>
        public void Canvas_Redraw()
        {
            // Clears the canvas containers.
            VM.nodeContainer.Clear("canvas");
            VM.memberContainer.Clear("canvas");

            // For each member, draw the memberline.
            foreach(Member member in VarHolder.MembersList)
            {
                var p1 = Helper.Functions.Point3DToCanvas(member.StartNode.Coords, VarHolder.GridNormal, origin);
                var p2 = Helper.Functions.Point3DToCanvas(member.EndNode.Coords, VarHolder.GridNormal, origin);

                VM.memberContainer.AddMember(p1, p2);
            }

            // For each node, draw the node circle.
            foreach(Node node in VarHolder.NodesList)
            {
                var p = Helper.Functions.Point3DToCanvas(node.Coords, VarHolder.GridNormal, origin);
                VM.nodeContainer.AddNode(p);
            }
           
        }



        /// <summary>
        /// Handles the canvas' mouse movement event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void canvas2d_MouseMove(object sender, MouseEventArgs e)
        {
            // If the right button is pressed while moving, scroll according to the
            // drag delta (coordinates are taken relative to the window so as not to
            // be affected by the canvas scaling).
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                var delta = e.GetPosition(this) - canvasfirstpont;
                canvasscroll.ScrollToVerticalOffset(canvasscroll.VerticalOffset - delta.Y);
                canvasscroll.ScrollToHorizontalOffset(canvasscroll.HorizontalOffset - delta.X);

                canvasfirstpont = e.GetPosition(this);
            }

            // Get the current mouse position in the canvas.
            var mousepos = e.GetPosition(canvas2d);


            // Initialize the variables used for the statusbar coordinates text.
            var unit = Models.UnitsHolder.Length;
            var string1 = "";
            var string2 = "";
            var string3 = "";

            // Depending on the current canvas plane, ommits one of the coordinates.
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
            
            // Update the statusbar coordinates text.
            VM.CoordsText = string1 + string2 + string3;
        }



        /// <summary>
        /// Handles the canvas' LMB release event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void canvas2d_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Used for checking whether something clickable has been found.
            bool found = false;

            // Iterate through each node.
            foreach (Node node in VarHolder.NodesList)
            {
                // Converts the node's 3D coordinates to the canvas coordinates system.
                var p = Helper.Functions.Point3DToCanvas(node.Coords, VarHolder.GridNormal, origin);

                // If the node is being clicked and nothing else was found before it...
                if (!found && VM.nodeContainer.CheckIfClicked(e, p))
                {
                    // Select the node.
                    VM.nodeContainer.Select(p);
                    found = true;
                }

                // Otherwise, flag it as not-selected.
                else
                    VM.nodeContainer.Unselect(p);
            }

            // Iterate through each member.
            foreach (Member member in VarHolder.MembersList)
            {
                var p1 = Helper.Functions.Point3DToCanvas(member.StartNode.Coords, VarHolder.GridNormal, origin);
                var p2 = Helper.Functions.Point3DToCanvas(member.EndNode.Coords, VarHolder.GridNormal, origin);

                // If the member is being clicked and nothing else was found before it...
                if (!found && VM.memberContainer.CheckIfClicked(e, p1, p2))
                {
                    VM.memberContainer.Select(p1, p2);
                    found = true;
                }

                // Otherwise, flag it as not-selected.
                else
                    VM.memberContainer.Unselect(p1, p2);
            }
        }
        #endregion
    }
}
