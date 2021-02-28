using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace PAENN.ViewModels
{
    /// <summary>
    /// Basic node element, stores its coordinates and other properties.
    /// </summary>
    public class Node
    {
        public Point3D Coords;      // The node's coordinates


        // Bool dictionary that stores each degree of freedom's restriction state (true = restricted, false = free).
        public Dictionary<string, bool> Restr = new Dictionary<string, bool>
        {
            {"Ux", false }, {"Uy", false}, {"Uz", false }, {"Rx", false}, {"Ry", false}, {"Rz", false}
        };

        // Double dictionary that stores each degree of freedom's spring constant (only effective if it's a free DOF).
        public Dictionary<string, double> SpringConstants = new Dictionary<string, double>
        {
            {"Ux", 0 }, {"Uy", 0 }, {"Uz", 0 }, {"Rx", 0 }, {"Ry", 0 }, {"Rz", 0 }
        };

        // Double dictionary that stores each degree of freedom's prescribed displacement/rotation (only effective if it's a fixed DOF)
        public Dictionary<string, double> PrescribedDispl = new Dictionary<string, double>
        {
            {"Ux", 0 }, {"Uy", 0 }, {"Uz", 0 }, {"Rx", 0 }, {"Ry", 0 }, {"Rz", 0 }
        };

        // Dictionary that contains each degree of freedom's list of nodal forces (ordered by loadcase)
        public Dictionary<string, List<double>> NodalForces = new Dictionary<string, List<double>>
        {
            {"Fx", new List<double>() }, {"Fy", new List<double>() }, {"Fz", new List<double>() },
            {"Mx", new List<double>() }, {"My", new List<double>() }, {"Mz", new List<double>() }
        };

        /// <summary>
        /// Node class constructor.
        /// </summary>
        /// <param name="X">The node's X coordinate.</param>
        /// <param name="Y">The node's Y coordinate.</param>
        /// <param name="Z">The node's Z coordinate.</param>
        public Node(double X, double Y, double Z)
        {
            Coords = new Point3D(X,Y,Z);

            int ncases = VarHolder.LoadcasesList.Count;

            foreach(KeyValuePair<string, List<double>> entry in NodalForces)
            {
                for (int i = 0; i < ncases; i++)
                    entry.Value.Add(0);
            }

        }
    }



    /// <summary>
    /// Member element, links two nodes and is responsible for the stiffness of the structure.
    /// </summary>
    public class Member 
    {
        public Node StartNode;          // The member's start node
        public Node EndNode;            // The member's end node

        public Material Material;       // The member's material
        public Section Section;         // The member's cross-section
        
        // Dictionary containing each direction's initial and final load lists, ordered by loadcase.
        public Dictionary<string, List<double>> MemberLoads = new Dictionary<string, List<double>>
        {
            {"Qx0", new List<double>() }, {"Qx1", new List<double>() },
            {"Qy0", new List<double>() }, {"Qy1", new List<double>() },
            {"Qz0", new List<double>() }, {"Qz1", new List<double>() }
        };


        /// <summary>
        /// Member class constructor.
        /// </summary>
        /// <param name="start">The start node.</param>
        /// <param name="end">The end node.</param>
        /// <param name="material">The member's material.</param>
        /// <param name="section">The member's cross-section.</param>
        public Member(Node start, Node end, Material material, Section section)
        {
            StartNode = start;
            EndNode = end;
            Material = material;
            Section = section;

            var ncases = VarHolder.LoadcasesList.Count;

            foreach(KeyValuePair<string, List<double>> entry in MemberLoads)
            {
                for (int i = 0; i < ncases; i++)
                    entry.Value.Add(0);
            }
        }
    }



    /// <summary>
    /// Material element, contains its physical properties.
    /// </summary>
    public class Material : ObservableClass
    {
        // The material's name.
        private string name;
        public string Name { get => name; set => PropertySet(ref name, "Name", value); }

        // Elasticity (E) modulus (or Young's modulus), represents the stress-strain ratio during the linear load phase.
        private double elasticity;
        public double Elasticity { get => elasticity; set => PropertySet(ref elasticity, "Elasticity", value); }


        // Transversal (G) modulus, represents the shear-distortion ratio during the linear load phase.
        private double transversal;
        public double Transversal { get => transversal; set => PropertySet(ref transversal, "Transversal", value); }


        // Thermal expansion coefficient (alpha), represents the strain-temperature change ratio.
        private double thermal;
        public double Thermal { get => thermal; set => PropertySet(ref thermal, "Thermal", value); }



        /// <summary>
        /// Material class constructor.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="elasticity">The elasticity (or Young's) modulus.</param>
        /// <param name="transversal">The transversal elasticity modulus.</param>
        /// <param name="thermal">The thermal expansion coefficient.</param>
        public Material(string name, double elasticity, double transversal, double thermal)
        {
            Name = name;
            Elasticity = elasticity;
            Transversal = transversal;
            Thermal = thermal;
        }
    }



    /// <summary>
    /// Cross-section element, contains its properties and methods for calculations.
    /// </summary>
    public class Section : ObservableClass
    {

        // The cross-section's name.
        private string name;
        public string Name { get => name; set => PropertySet(ref name, "Name", value); }


        // The cross-section's moment of inertia relative to the local X-axis, also known as the torsional (or polar) moment of inertia.
        private double ix = 0;
        public double Ix { get => ix; set => PropertySet(ref ix, "Ix", value); }


        // The cross-section's moment of inertia relative to the local Y-axis.
        private double iy = 0;
        public double Iy { get => iy; set => PropertySet(ref iy, "Iy", value); }


        // The cross-section's moment of inertia relative to the local Z-axis.
        private double iz = 0;
        public double Iz { get => iz; set => PropertySet(ref iz, "Iz", value); }


        // The cross-section's area.
        private double area = 0;
        public double Area { get => area; set => PropertySet(ref area, "Area", value); }


        // The cross-section's uppermost Y-coordinate (relative to its centroid).
        private double ysup = 0;
        public double Ysup { get => ysup; set => PropertySet(ref ysup, "Ysup", value); }

        // The cross-section's lowermost Y-coordinate (in absolute value, relative to its centroid).
        private double yinf = 0;
        public double Yinf { get => yinf; set => PropertySet(ref yinf, "Yinf", value); }


        // The cross-section's uppermost Z-coordinate (relative to its centroid). 
        private double zsup = 0;
        public double Zsup { get => zsup; set => PropertySet(ref zsup, "Zsup", value); }


        // The cross-section's lowermost Z-coordinate (in absolute value, relative to its centroid).
        private double zinf = 0;
        public double Zinf { get => zinf; set => PropertySet(ref zinf, "Zinf", value); }


        // The cross-section's type (currently supports generic, circular and rectangular).
        private string sectype = "Genérico";
        public string SecType { get => sectype; set => PropertySet(ref sectype, "SecType", value); }


        // The cross sections parameters, dependent on section type (diameter for circular, base and height for rectangular).
        public double[] Parameters { get; set; } = new double[] { 0, 0, 0, 0 };


        /// <summary>
        /// Section class constructor.
        /// </summary>
        /// <param name="name">The section's name.</param>
        public Section(string name) 
        {
            Name = name;
        }



        /// <summary>
        /// Manually inserts each cross-sectional property.
        /// </summary>
        /// <param name="Inx">The cross-section's moment of inertia relative to the local X-axis, also known as the torsional (or polar) moment of inertia.</param>
        /// <param name="Iny">The cross-section's moment of inertia relative to the local Y-axis.</param>
        /// <param name="Inz">The cross-section's moment of inertia relative to the local Z-axis.</param>
        /// <param name="A">The cross-section's area.</param>
        /// <param name="y_sup">The cross-section's uppermost Y-coordinate (relative to its centroid).</param>
        /// <param name="y_inf">The cross-section's lowermost Y-coordinate (in absolute value, relative to its centroid).</param>
        /// <param name="z_sup">The cross-section's uppermost Z-coordinate (relative to its centroid).</param>
        /// <param name="z_inf">The cross-section's lowermost Z-coordinate (in absolute value, relative to its centroid).</param>
        public void Generic(double Inx, double Iny, double Inz, double A, double y_sup, double y_inf, double z_sup, double z_inf)
        {
            Ix = Inx;
            Iy = Iny;
            Iz = Inz;
            Area = A;
            Ysup = y_sup;
            Yinf = y_inf;
            Zsup = z_sup;
            Zinf = z_inf;

            Parameters = new double[] { Ix, Iy, Iz, A, ysup, yinf };
            SecType = "Genérico";
        }



        /// <summary>
        /// Sets the cross-section to a rectangle, and calculates each property accordingly.
        /// </summary>
        /// <param name="B">The rectangle's base length.</param>
        /// <param name="H">The rectangle's height.</param>
        public void Rectangle(double B, double H)
        {
            Iz = B * H * H * H / 12;
            Iy = H * B * B * B / 12;
            Ix = B * H * (B * B + H * H) / 12;
            Area = B * H;
            Ysup = Yinf =  H / 2;
            Zsup = Zinf =  B / 2;

            Parameters = new double[] { B, H };
            SecType = "Retangular";
        }



        /// <summary>
        /// Sets the cross-section to a circle or ring, and calculates each property accordingly.
        /// </summary>
        /// <param name="D">The external diameter.</param>
        /// <param name="d">The internal diameter (if hollow).</param>
        public void Circle (double D, double d=0)
        {
            Iz = Iy = Math.PI * (Math.Pow(D,4) - Math.Pow(d,4)) / 64;
            Ix = 2 * Iz;

            Area = Math.PI * (D * D - d * d) / 4;
            Ysup = Yinf = Zsup = Zinf = D / 2;

            Parameters = new double[] { D, d };
            SecType = "Circular";
        }
    }



    /// <summary>
    /// Abstract superclass used for every class that needs to notify property changes.
    /// </summary>
    public abstract class ObservableClass : INotifyPropertyChanged
    {
        
        // Broadcasts the change in property so it can be picked up by the WPF controls.
        public event PropertyChangedEventHandler PropertyChanged;



        /// <summary>
        /// Broadcasts the change in a given property.
        /// </summary>
        /// <param name="Property">The property's name.</param>
        public void NotifyPropertyChanged(string Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }



        /// <summary>
        /// Notifies the change in a boolean property.
        /// </summary>
        /// <param name="PrivateField">The private field to be changed.</param>
        /// <param name="PropName">The property's name.</param>
        /// <param name="value">The value to be assigned.</param>
        public void PropertySet(ref bool PrivateField, string PropName, bool value)
        {
            if (PrivateField != value)
            {
                PrivateField = value;
                this.NotifyPropertyChanged(PropName);
            }
        }

        /// <summary>
        /// Notifies the change in an integer property.
        /// </summary>
        /// <param name="PrivateField">The private field to be changed.</param>
        /// <param name="PropName">The property's name.</param>
        /// <param name="value">The value to be assigned.</param>
        public void PropertySet(ref int PrivateField, string PropName, int value)
        {
            if (PrivateField != value)
            {
                PrivateField = value;
                this.NotifyPropertyChanged(PropName);
            }
        }

        /// <summary>
        /// Notifies the change in a double property.
        /// </summary>
        /// <param name="PrivateField">The private field to be changed.</param>
        /// <param name="PropName">The property's name.</param>
        /// <param name="value">The value to be assigned.</param>
        public void PropertySet(ref double PrivateField, string PropName, double value)
        {
            if (PrivateField != value)
            {
                PrivateField = value;
                this.NotifyPropertyChanged(PropName);
            }
        }

        /// <summary>
        /// Notifies the change in a string property.
        /// </summary>
        /// <param name="PrivateField">The private field to be changed.</param>
        /// <param name="PropName">The property's name.</param>
        /// <param name="value">The value to be assigned.</param>
        public void PropertySet(ref string PrivateField, string PropName, string value)
        {
            if (PrivateField != value)
            {
                PrivateField = value;
                this.NotifyPropertyChanged(PropName);
            }
        }

    }
}
