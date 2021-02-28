using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PAENN.ViewModels
{
    /// <summary>
    /// Static class using for holding global variables needed in multiple classes.
    /// </summary>
    public static class VarHolder
    {
        #region Item collections
        public static ObservableCollection<string> LoadcasesList = new ObservableCollection<string>();      // List of loadcase names
        public static ObservableCollection<string> CombinationsList = new ObservableCollection<string>();   // List of combination names
        
        public static ObservableCollection<Node> NodesList = new ObservableCollection<Node>();              // List of nodes
        public static ObservableCollection<Member> MembersList = new ObservableCollection<Member>();        // List of members
        public static ObservableCollection<Material> MaterialsList = new ObservableCollection<Material>();  // List of materials
        public static ObservableCollection<Section> SectionsList = new ObservableCollection<Section>();     // List of sections
        #endregion

        #region Appliable properties
        public static Dictionary<string, bool> ApplyRestr = new Dictionary<string, bool>();         // Stored DOF restriction values to be applied
        public static Dictionary<string, double> ApplySpring = new Dictionary<string, double>();    // Stored spring constants to be applied
        public static Dictionary<string, double> ApplyPdispl = new Dictionary<string, double>();    // Stored prescribed displacements to be applied
        public static Dictionary<string, double> ApplyNodal = new Dictionary<string, double>();     // Stored nodal forces to be applied
        public static Dictionary<string, double> ApplyLoad = new Dictionary<string, double>();      // Stored member loads to be applied
        #endregion

        #region Max/min applied forces
        // Global maximum/minimum nodal forces in current loadcase
        public static double MaxForce = 2;
        public static double MinForce = 1;

        // Global maximum/minimum nodal moments in current loadcase
        public static double MaxMom = 2;
        public static double MinMom = 1;

        // Global maximum/minimum distributed loads in current loadcase
        public static double MaxLoad = 2;
        public static double MinLoad = 1;
        #endregion

        #region Current properties
        // Current loadcase/combination indices
        public static int CurrentLoadcase = 0;
        public static int CurrentCombination = -1;

        // Action to be taken when clicking the viewport/canvas
        public static string ClickType = "Select";

        // Variable used to store the first node selected when creating members via clicking
        public static Node StartNode;
        #endregion

        #region Grid properties
        // Normal direction to the current canvas plane (None if using the viewport)
        public static string GridNormal = "None";

        // Horizontal and vertical grid spacing
        public static double GridH { get; set; } = 100;
        public static double GridV { get; set; } = 100;
        #endregion



        /// <summary>
        /// Finds the global maxima/minima for nodal forces/moments and stores them.
        /// </summary>
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



        /// <summary>
        /// Finds the global maximum/minimum for member loads and stores them.
        /// </summary>
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
