using System.Windows;
using PAENN.ViewModels;
using System.Windows.Input;

namespace PAENN
{
    /// <summary>
    /// ViewModel class used for MainWindow class instances.
    /// </summary>
    class MainWindow_VM : ObservableClass
    {
        #region Drawing containers
        public Draw.NodeContainer nodeContainer;
        public Draw.MemberContainer memberContainer;
        public Draw.SupportContainer supportContainer;
        public Draw.NodalForcesContainer nodalContainer;
        public Draw.MemberLoadsContainer loadsContainer;
        #endregion

        #region Statusbar properties
        private bool snapgrid = false;
        public bool SnapGrid { get => snapgrid; set => PropertySet(ref snapgrid, "SnapGrid", value); }

        private bool gridenabled = false;
        public bool GridEnabled { get => gridenabled; set => PropertySet(ref gridenabled, "GridEnabled", value); }

        private string coordstext = "";
        public string CoordsText { get => coordstext; set => PropertySet(ref coordstext, "CoordsText", value); }
        #endregion

        /// <summary>
        /// MainWindow_VM class constructor.
        /// </summary>
        public MainWindow_VM()
        {
            // Finds the MainWindow, its viewport and canvas.
            var mainwindow = (MainWindow)Application.Current.Windows[0];
            var viewport = mainwindow.viewport;
            var canvas = mainwindow.canvas2d;

            // Initializes the drawing containers with the viewport and canvas.
            nodeContainer = new Draw.NodeContainer(viewport, canvas);
            memberContainer = new Draw.MemberContainer(viewport, canvas);
            supportContainer = new Draw.SupportContainer(viewport, canvas);
            nodalContainer = new Draw.NodalForcesContainer(viewport, canvas);
            loadsContainer = new Draw.MemberLoadsContainer(viewport, canvas);

            // Adjusts the viewport's movement gestures.
            viewport.PanGesture = new MouseGesture(MouseAction.RightClick);
            viewport.PanGesture2 = new MouseGesture(MouseAction.RightClick);
            viewport.RotateGesture = new MouseGesture(MouseAction.MiddleClick);
            viewport.RotateGesture2 = new MouseGesture(MouseAction.MiddleClick);



            // Adds a dummy material, section and loadcase to the VarHolder.
            var m = new ViewModels.Material("dababy", 20000, 7700, 1e-5);
            var s = new Section("bed");
            s.Rectangle(15, 15);
            VarHolder.LoadcasesList.Add("Caso 1");
            VarHolder.MaterialsList.Add(m);
            VarHolder.SectionsList.Add(s);
        }



        /// <summary>
        /// Applies the VarHolder's current support conditions to a given node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ApplySupport(Node node)
        {
            // If any of the VarHolder's dictionaries are empty, return.
            if (VarHolder.ApplyRestr.Count == 0 || VarHolder.ApplyPdispl.Count == 0 || VarHolder.ApplySpring.Count == 0)
                return;

            ActionHandler.ApplySupport(node, VarHolder.ApplyRestr, VarHolder.ApplySpring, VarHolder.ApplyPdispl);
        }



        /// <summary>
        /// Applies the VarHolder's current nodal forces to a given node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ApplyNodalForces(Node node)
        {
            // If the VarHolder's dictionary is empty, return.
            if (VarHolder.ApplyNodal.Count == 0)
                return;

            ActionHandler.ApplyNodalForces(node, VarHolder.ApplyNodal, VarHolder.CurrentLoadcase);
        }



        /// <summary>
        /// Applies the VarHolder's current distributed loads to a given member.
        /// </summary>
        /// <param name="member">The member.</param>
        public void ApplyMemberLoads(Member member)
        {
            // If the VarHolder's dictionary is empty, return.
            if (VarHolder.ApplyLoad.Count == 0)
                return;

            ActionHandler.ApplyMemberLoads(member, VarHolder.ApplyLoad, VarHolder.CurrentLoadcase);
        }



        /// <summary>
        /// Marks a given node as the "start node" when creating members by clicking.
        /// </summary>
        /// <param name="node">The node.</param>
        public void NewMemberStart(Node node)
        {
            VarHolder.StartNode = node;
            VarHolder.ClickType = "NewMember_End";
        }



        /// <summary>
        /// Creates a new member after clicking the second node.
        /// </summary>
        /// <param name="node">The second node.</param>
        /// <param name="window">The "New Member" window.</param>
        public void NewMemberEnd(Node node, Views.WinNewMember window)
        {
            // Variable initialization .
            var n1 = VarHolder.StartNode;
            var MatIndex = 0;
            var SecIndex = 0;

            // If the start and end nodes are the same, show an error message and return.
            if (n1 == node)
            {
                MessageBox.Show("Selecione pontos diferentes para início e término.", "Erro");
                return;
            }

            try
            {
                // Tries to find the current material and section in the VarHolder.
                MatIndex = window.VM.List_Materials.IndexOf(window.VM.Var_Material);
                SecIndex = window.VM.List_Sections.IndexOf(window.VM.Var_Section);
            }
            catch
            {
                // If unable to find, show an error message and return.
                MessageBox.Show("Selecione material e seção válidos.", "Erro");
                return;
            }
            var material = VarHolder.MaterialsList[MatIndex];
            var section = VarHolder.SectionsList[SecIndex];

            // Create a member at the given nodes.
            var apply = ActionHandler.NewMember(n1.Coords.X, n1.Coords.Y, n1.Coords.Z, node.Coords.X, node.Coords.Y, node.Coords.Z, material, section, 1);
           
            // If there is already a member with the same nodes, show an error message and return.
            if (apply == -1)
            {
                MessageBox.Show("Já existe uma barra nos pontos selecionados.", "Erro");
                return;
            }

            // Reset the member creating variable.
            VarHolder.ClickType = "NewMember_Start";
        }
    }

}
