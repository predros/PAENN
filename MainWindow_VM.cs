using System.Windows;
using PAENN.ViewModels;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace PAENN
{
    class MainWindow_VM : ObservableClass
    {
        public Draw.NodeContainer nodeContainer;
        public Draw.MemberContainer memberContainer;
        public Draw.SupportContainer supportContainer;
        public Draw.NodalForcesContainer nodalContainer;
        public Draw.MemberLoadsContainer loadsContainer;
        public Draw.GridContainer gridContainer;

        public System.Windows.Media.Media3D.Point3D CamStart = new System.Windows.Media.Media3D.Point3D();

        MouseGesture Middle { get; set; } = new MouseGesture(MouseAction.MiddleClick);
        MouseGesture Right { get; set; } = new MouseGesture(MouseAction.RightClick);

        private bool snapgrid = false;
        public bool SnapGrid { get => snapgrid; set => PropertySet(ref snapgrid, "SnapGrid", value); }

        private bool gridenabled = false;
        public bool GridEnabled { get => gridenabled; set => PropertySet(ref gridenabled, "GridEnabled", value); }

        private string coordstext = "";
        public string CoordsText { get => coordstext; set => PropertySet(ref coordstext, "CoordsText", value); }

        public MainWindow_VM()
        {
            var mainwindow = Application.Current.Windows[0] as MainWindow;
            var viewport = mainwindow.viewport;
            var canvas = mainwindow.canvas2d;

            nodeContainer = new Draw.NodeContainer(viewport, canvas);
            memberContainer = new Draw.MemberContainer(viewport);
            supportContainer = new Draw.SupportContainer(viewport);
            nodalContainer = new Draw.NodalForcesContainer(viewport);
            loadsContainer = new Draw.MemberLoadsContainer(viewport);
            gridContainer = new Draw.GridContainer(viewport);

            #region Viewport config
            viewport.PanGesture = Right;
            viewport.PanGesture2 = Right;
            viewport.RotateGesture = Middle;
            viewport.RotateGesture2 = Middle;
            #endregion

            VarHolder.LoadcasesList.Add("Caso 1");

            var m = new ViewModels.Material("dababy", 20000, 7700, 1e-5);
            var s = new Section("bed");

            s.Rectangle(15, 15);

            VarHolder.MaterialsList.Add(m);
            VarHolder.SectionsList.Add(s);
        }

        public void ApplySupport(Node node)
        {
            if (VarHolder.ApplyRestr.Count == 0 || VarHolder.ApplyPdispl.Count == 0 || VarHolder.ApplySpring.Count == 0)
                return;

            ActionHandler.ApplySupport(node, VarHolder.ApplyRestr, VarHolder.ApplySpring, VarHolder.ApplyPdispl);
        }

        public void ApplyNodalForces(Node node)
        {
            if (VarHolder.ApplyNodal.Count == 0)
                return;

            ActionHandler.ApplyNodalForces(node, VarHolder.ApplyNodal, VarHolder.CurrentLoadcase);
        }

        public void ApplyMemberLoads(Member member)
        {
            if (VarHolder.ApplyLoad.Count == 0)
                return;

            ActionHandler.ApplyMemberLoads(member, VarHolder.ApplyLoad, VarHolder.CurrentLoadcase);
        }

        public void NewMemberStart(Node node)
        {
            VarHolder.StartNode = node;
            VarHolder.ClickType = "NewMember_End";
        }

        public void NewMemberEnd(Node node, Views.WinNewMember window)
        {
            var n1 = VarHolder.StartNode;
            var MatIndex = 0;
            var SecIndex = 0;
            try
            {
                MatIndex = window.VM.List_Materials.IndexOf(window.VM.Var_Material);
                SecIndex = window.VM.List_Sections.IndexOf(window.VM.Var_Section);
            }
            catch
            {
                MessageBox.Show("Selecione material e seção válidos.", "Erro");
                return;
            }
            var material = VarHolder.MaterialsList[MatIndex];
            var section = VarHolder.SectionsList[SecIndex];

            var apply = ActionHandler.NewMember(n1.Coords.X, n1.Coords.Y, n1.Coords.Z, node.Coords.X, node.Coords.Y, node.Coords.Z, material, section, 1);
            if (apply == -1)
            {
                MessageBox.Show("Já existe uma barra nos pontos selecionados.", "Erro");
                return;
            }

            VarHolder.ClickType = "NewMember_Start";
        }
    }

}
