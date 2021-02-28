using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Media3D;
using Helper;

namespace PAENN.ViewModels
{
    /// <summary>
    /// Handles action history, used for undoing/redoing.
    /// </summary>
    public static class ActionHandler
    {
        // List of past states
        private static List<ObservableCollection<Node>> State_Nodes = new List<ObservableCollection<Node>>();
        private static List<ObservableCollection<Member>> State_Members = new List<ObservableCollection<Member>>();

        // List of redoable states
        private static List<ObservableCollection<Node>> Redo_Nodes = new List<ObservableCollection<Node>>();
        private static List<ObservableCollection<Member>> Redo_Members = new List<ObservableCollection<Member>>();



        /// <summary>
        /// Saves the current state of the structure.
        /// </summary>
        public static void SaveState()
        {
            State_Nodes.Add(VarHolder.NodesList);
            State_Members.Add(VarHolder.MembersList);

            Redo_Members.Clear();
            Redo_Nodes.Clear();
        }



        /// <summary>
        /// Undoes the current state of the structure.
        /// </summary>
        public static void Undo()
        {
            var n = State_Nodes.Count - 1;

            Redo_Nodes.Add(VarHolder.NodesList);
            Redo_Members.Add(VarHolder.MembersList);

            for(int i =0; i<n; i++)
            {
                Console.WriteLine(State_Nodes[i].Count);
            }

            if (State_Nodes.Count != 0)
            {
                VarHolder.NodesList.Clear();
                VarHolder.MembersList.Clear();

                foreach (Node node in State_Nodes.Last())
                    VarHolder.NodesList.Add(node);

                foreach (Member member in State_Members.Last())
                    VarHolder.MembersList.Add(member);

                State_Nodes.RemoveAt(n);
                State_Members.RemoveAt(n);
            }
        }



        /// <summary>
        /// Redoes the last undone state of the structure.
        /// </summary>
        public static void Redo()
        {
            var n = Redo_Nodes.Count - 1;

            if (n < 0) return;

            VarHolder.NodesList = new ObservableCollection<Node>(Redo_Nodes[n]);
            VarHolder.MembersList = new ObservableCollection<Member>(Redo_Members[n]);

            State_Nodes.Add(Redo_Nodes[n]);
            State_Members.Add(Redo_Members[n]);

            Redo_Nodes.RemoveAt(n);
            Redo_Members.RemoveAt(n);
        }



        /// <summary>
        /// Adds a new node to the structure.
        /// </summary>
        /// <param name="X">The node's X-coordinate.</param>
        /// <param name="Y">The node's Y-coordinate.</param>
        /// <param name="Z">The node's Z-coordinate.</param>
        /// <param name="save">Whether to save the current state before adding.</param>
        /// <returns></returns>
        public static int NewNode(double X, double Y, double Z, int save=0)
        {
            var tol = 1e-5;
            foreach(Node node in VarHolder.NodesList)
                {
                var d = (new Point3D(X, Y, Z) - node.Coords).Length;
                if (d < tol)
                    return -1;
                }


            if (save == 1)
                SaveState();

            var n = new Node(X, Y, Z);
            VarHolder.NodesList.Add(n);

            var tempmembers = new List<Member>();
            foreach (Member member in VarHolder.MembersList)
            {
                var n1 = member.StartNode;
                var n2 = member.EndNode;
                bool intersect = Functions.CheckPointIntersection(n1.Coords, n2.Coords, new Point3D(X, Y, Z));

                if (intersect)
                {
                    member.EndNode = n;
                    var newmember = new Member(n, n2, member.Material, member.Section);
                    tempmembers.Add(newmember);
                }
            }

            foreach (Member member in tempmembers)
                VarHolder.MembersList.Add(member);

            return 0;
        }



        /// <summary>
        /// Adds a new member to the structure.
        /// </summary>
        /// <param name="X1">The first node's X-coordinate.</param>
        /// <param name="Y1">The first node's Y-coordinate.</param>
        /// <param name="Z1">The first node's Z-coordinate.</param>
        /// <param name="X2">The second node's X-coordinate.</param>
        /// <param name="Y2">The second node's Y-coordinate.</param>
        /// <param name="Z2">The second node's Z-coordinate.</param>
        /// <param name="material">The member's material.</param>
        /// <param name="section">The member's cross-section.</param>
        /// <param name="save">Whether to save the current state before adding.</param>
        /// <returns></returns>
        public static int NewMember(double X1, double Y1, double Z1, double X2, double Y2, double Z2, Material material, Section section, int save=0)
        {
            var tol = 1e-5;
            Node n1 = new Node(0, 0, 0);    // Dummy values
            Node n2 = new Node(1, 1, 1);    // Dummy values
            int ret1 = 0;
            int ret2 = 0;

            foreach (Node node in VarHolder.NodesList)
            {
                var d = (new Point3D(X1, Y1, Z1) - node.Coords).Length;
                if (d < tol)
                {
                    n1 = node;
                    ret1 = 1;
                    break;
                }
            }
            
            foreach (Node node in VarHolder.NodesList)
            {
                var d = (new Point3D(X2, Y2, Z2) - node.Coords).Length;
                if (d < tol)
                {
                    n2 = node;
                    ret2 = 1;
                    break;
                }
            }

            if (ret1 == 0)
            {
                NewNode(X1, Y1, Z1);
                n1 = VarHolder.NodesList.LastOrDefault();
            }
                


            if (ret2 == 0)
            {
                NewNode(X2, Y2, Z2);
                n2 = VarHolder.NodesList.LastOrDefault();
            }
                


            foreach(Member member in VarHolder.MembersList)
            {
                if((member.StartNode == n1 && member.EndNode == n2) || (member.StartNode == n2 && member.EndNode == n1)) 
                    return -1;
            }

            if (save==1)
                SaveState();
            
            var m = new Member(n1, n2, material, section);
            VarHolder.MembersList.Add(m);
            return 0;
        }



        /// <summary>
        /// Applies support conditions to a given node.
        /// </summary>
        /// <param name="node">The node to be applied.</param>
        /// <param name="support">The support conditions to be applied.</param>
        /// <param name="spring">The spring constants to be applied.</param>
        /// <param name="pdispl">The prescribed displacements to be applied.</param>
        public static void ApplySupport(Node node, Dictionary<string, bool> support, Dictionary<string, double> spring, Dictionary<string, double> pdispl)
        {
            SaveState();

            node.Restr = new Dictionary<string, bool>(support);
            node.SpringConstants = new Dictionary<string, double>(spring);
            node.PrescribedDispl = new Dictionary<string, double>(pdispl);
        }



        /// <summary>
        /// Applies nodal forces to a given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="forces">The nodal forces.</param>
        /// <param name="loadcase">The loadcase being used.</param>
        public static void ApplyNodalForces(Node node, Dictionary<string, double> forces, int loadcase)
        {
            SaveState();

            node.NodalForces["Fx"][loadcase] = forces["Fx"];
            node.NodalForces["Fy"][loadcase] = forces["Fy"];
            node.NodalForces["Fz"][loadcase] = forces["Fz"];

            node.NodalForces["Mx"][loadcase] = forces["Mx"];
            node.NodalForces["My"][loadcase] = forces["My"];
            node.NodalForces["Mz"][loadcase] = forces["Mz"];
        }



        /// <summary>
        /// Applies distributed loads to a given member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="loads">The loads.</param>
        /// <param name="loadcase">The loadcase being applied to.</param>
        public static void ApplyMemberLoads(Member member, Dictionary<string, double> loads, int loadcase)
        {
            SaveState();

            member.MemberLoads["Qx0"][loadcase] = loads["X1"];
            member.MemberLoads["Qx1"][loadcase] = loads["X2"];

            member.MemberLoads["Qy0"][loadcase] = loads["Y1"];
            member.MemberLoads["Qy1"][loadcase] = loads["Y2"];

            member.MemberLoads["Qz0"][loadcase] = loads["Z1"];
            member.MemberLoads["Qz1"][loadcase] = loads["Z2"];
        }
    }
}
