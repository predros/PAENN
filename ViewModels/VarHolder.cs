using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PAENN.ViewModels
{
    public static class VarHolder
    {
        #region Item collections
        public static ObservableCollection<string> LoadcasesList = new ObservableCollection<string>();
        public static ObservableCollection<string> CombinationsList = new ObservableCollection<string>();
        
        public static ObservableCollection<Node> NodesList = new ObservableCollection<Node>();
        public static ObservableCollection<Member> MembersList = new ObservableCollection<Member>();
        public static ObservableCollection<Material> MaterialsList = new ObservableCollection<Material>();
        public static ObservableCollection<Section> SectionsList = new ObservableCollection<Section>();
        #endregion

        #region Appliable properties
        public static Dictionary<string, bool> ApplyRestr = new Dictionary<string, bool>();
        public static Dictionary<string, double> ApplySpring = new Dictionary<string, double>();
        public static Dictionary<string, double> ApplyPdispl = new Dictionary<string, double>();
        public static Dictionary<string, double> ApplyNodal = new Dictionary<string, double>();
        public static Dictionary<string, double> ApplyLoad = new Dictionary<string, double>();
        #endregion

        #region Max/min applied forces
        public static double MaxForce = 2;
        public static double MinForce = 1;

        public static double MaxMom = 2;
        public static double MinMom = 1;

        public static double MaxLoad = 2;
        public static double MinLoad = 1;
        #endregion

        #region Current properties
        public static int CurrentLoadcase = 0;
        public static int CurrentCombination = -1;

        public static string ClickType = "Select";

        public static Node StartNode;

        public static string GridNormal = "None";

        #endregion

        public static double GridH { get; set; } = 100;
        public static double GridV { get; set; } = 100;

        public static void MaxMinForces()
        {
            if (NodesList.Count == 0)
                return;

            foreach (Node node in NodesList)
            {
                var max = Math.Max(Math.Abs(node.NodalForces["Fx"][CurrentLoadcase]), Math.Abs(node.NodalForces["Fy"][CurrentLoadcase]));
                max = Math.Max(max, Math.Abs(node.NodalForces["Fz"][CurrentLoadcase]));

                var min = Math.Min(Math.Abs(node.NodalForces["Fx"][CurrentLoadcase]), Math.Abs(node.NodalForces["Fy"][CurrentLoadcase]));
                min = Math.Min(min, Math.Abs(node.NodalForces["Fz"][CurrentLoadcase]));

                MaxForce = Math.Max(max, MaxForce);
                MinForce = Math.Min(min, MinForce);

                max = Math.Max(Math.Abs(node.NodalForces["Mx"][CurrentLoadcase]), Math.Abs(node.NodalForces["My"][CurrentLoadcase]));
                max = Math.Max(max, Math.Abs(node.NodalForces["Mz"][CurrentLoadcase]));

                min = Math.Min(Math.Abs(node.NodalForces["Mx"][CurrentLoadcase]), Math.Abs(node.NodalForces["My"][CurrentLoadcase]));
                min = Math.Min(min, Math.Abs(node.NodalForces["Mz"][CurrentLoadcase]));

                MaxMom = Math.Max(max, MaxMom);
                MinMom = Math.Max(min, MinMom);

            }

            if (MinForce == 0)
                MinForce = MaxForce - 25;
            if (MinMom == 0)
                MinMom = MaxMom - 25;
        }

        public static void MaxMinLoads()
        {
            if (MembersList.Count == 0)
                return;

            double max = 0;
            double min = 100;

            foreach (Member member in MembersList)
            {
                var n = CurrentLoadcase;
                var max_x = Math.Max(Math.Abs(member.MemberLoads["Qx0"][n]), Math.Abs(member.MemberLoads["Qx1"][n]));
                var min_x = Math.Min(Math.Abs(member.MemberLoads["Qx0"][n]), Math.Abs(member.MemberLoads["Qx1"][n]));

                var max_y = Math.Max(Math.Abs(member.MemberLoads["Qy0"][n]), Math.Abs(member.MemberLoads["Qy1"][n]));
                var min_y = Math.Min(Math.Abs(member.MemberLoads["Qy0"][n]), Math.Abs(member.MemberLoads["Qy1"][n]));

                var max_z = Math.Max(Math.Abs(member.MemberLoads["Qz0"][n]), Math.Abs(member.MemberLoads["Qz1"][n]));
                var min_z = Math.Min(Math.Abs(member.MemberLoads["Qz0"][n]), Math.Abs(member.MemberLoads["Qz1"][n]));

                max = Math.Max(max, max_x);
                max = Math.Max(max, max_y);
                max = Math.Max(max, max_z);

                min = Math.Min(min, min_x);
                min = Math.Min(min, min_y);
                min = Math.Min(min, min_z);
            }

            if (min > max)
                min = 0;

            if (min == 0)
                min = max - 20;

            MaxLoad = max;
            MinLoad = min;
        }

    }
}
