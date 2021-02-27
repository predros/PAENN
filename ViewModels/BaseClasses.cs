using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace PAENN.ViewModels
{
    public class Node
        /// <summary>
        /// Basic node element, stores its coordinates and other properties.
        /// </summary>
    {
        public Point3D Coords;

        public Dictionary<string, bool> Restr = new Dictionary<string, bool>
        {
            {"Ux", false }, {"Uy", false}, {"Uz", false }, {"Rx", false}, {"Ry", false}, {"Rz", false}
        };

        public double SupportAngle;

        public Dictionary<string, double> SpringConstants = new Dictionary<string, double>
        {
            {"Ux", 0 }, {"Uy", 0 }, {"Uz", 0 }, {"Rx", 0 }, {"Ry", 0 }, {"Rz", 0 }
        };

        public Dictionary<string, double> PrescribedDispl = new Dictionary<string, double>
        {
            {"Ux", 0 }, {"Uy", 0 }, {"Uz", 0 }, {"Rx", 0 }, {"Ry", 0 }, {"Rz", 0 }
        };

        public Dictionary<string, List<double>> NodalForces = new Dictionary<string, List<double>>
        {
            {"Fx", new List<double>() }, {"Fy", new List<double>() }, {"Fz", new List<double>() },
            {"Mx", new List<double>() }, {"My", new List<double>() }, {"Mz", new List<double>() }
        };

        public List<double> ForcesTheta = new List<double>();
        public List<double> ForcesPhi = new List<double>();

        public Node(double X, double Y, double Z)
        {
            Coords = new Point3D(X,Y,Z);

            int ncases = VarHolder.LoadcasesList.Count;

            foreach(KeyValuePair<string, List<double>> entry in NodalForces)
            {
                for (int i = 0; i < ncases; i++)
                    entry.Value.Add(0);
            }

            for(int i=0; i < ncases; i++)
            {
                ForcesTheta.Add(0);
                ForcesPhi.Add(0);
            }
        }
    }

    public class Member 
    /// <summary>
    /// Member element, links two nodes and is responsible for the stiffness of the structure.
    /// </summary>
    {
        public Node StartNode;
        public Node EndNode;

        public Material Material;
        public Section Section;
        
        public double Length;
        public double Angle;

        public Dictionary<string, List<double>> MemberLoads = new Dictionary<string, List<double>>
        {
            {"Qx0", new List<double>() }, {"Qx1", new List<double>() },
            {"Qy0", new List<double>() }, {"Qy1", new List<double>() },
            {"Qz0", new List<double>() }, {"Qz1", new List<double>() }
        };

        public Dictionary<string, List<bool>> IsLoadLocal = new Dictionary<string, List<bool>>
        {
            {"x", new List<bool>() }, {"y", new List<bool>() }, {"z", new List<bool>() }
        };

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

            foreach(KeyValuePair<string, List<bool>> entry in IsLoadLocal)
            {
                for (int i = 0; i < ncases; i++)
                    entry.Value.Add(false);
            }
        }
    }

    public class Material : ObservableClass
    /// <summary>
    /// Material element, contains its properties.
    /// </summary>
    {
        private double elasticity;
        public double Elasticity { get => elasticity; set => PropertySet(ref elasticity, "Elasticity", value); }

        private double transversal;
        public double Transversal { get => transversal; set => PropertySet(ref transversal, "Transversal", value); }

        private double thermal;
        public double Thermal { get => thermal; set => PropertySet(ref thermal, "Thermal", value); }

        private string name;
        public string Name { get => name; set => PropertySet(ref name, "Name", value); }


        public Material(string name, double elasticity, double transversal, double thermal)
        {
            Name = name;
            Elasticity = elasticity;
            Transversal = transversal;
            Thermal = thermal;
        }
    }

    public class Section : ObservableClass
    /// <summary>
    /// Cross-section element, contains its properties and methods for calculations.
    /// </summary>
    {
        private string name;
        public string Name { get => name; set => PropertySet(ref name, "Name", value); }

        private double ix = 0;
        public double Ix { get => ix; set => PropertySet(ref ix, "Ix", value); }

        private double iy = 0;
        public double Iy { get => iy; set => PropertySet(ref iy, "Iy", value); }

        private double iz = 0;
        public double Iz { get => iz; set => PropertySet(ref iz, "Iz", value); }

        private double area = 0;
        public double Area { get => area; set => PropertySet(ref area, "Area", value); }

        private double ysup = 0;
        public double Ysup { get => ysup; set => PropertySet(ref ysup, "Ysup", value); }

        private double yinf = 0;
        public double Yinf { get => yinf; set => PropertySet(ref yinf, "Yinf", value); }

        private double zsup = 0;
        public double Zsup { get => zsup; set => PropertySet(ref zsup, "Zsup", value); }

        private double zinf = 0;
        public double Zinf { get => zinf; set => PropertySet(ref zinf, "Zinf", value); }

        private string sectype = "Genérico";
        public string SecType { get => sectype; set => PropertySet(ref sectype, "SecType", value); }

        public double[] Parameters { get; set; } = new double[] { 0, 0, 0, 0 };

        public Section(string name) 
        {
            Name = name;
        }

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

        public void Circle (double D, double d)
        {
            Iz = Iy = Math.PI * (Math.Pow(D,4) - Math.Pow(d,4)) / 64;
            Ix = 2 * Iz;

            Area = Math.PI * (D * D - d * d) / 4;
            Ysup = Yinf = Zsup = Zinf = D / 2;

            Parameters = new double[] { D, d };
            SecType = "Circular";
        }
    }


    public class ObservableClass : INotifyPropertyChanged
    {
        public void PropertySet(ref bool PrivateField, string PropName, bool value)
        {
            if (PrivateField != value)
            {
                PrivateField = value;
                this.NotifyPropertyChanged(PropName);
            }
        }

        public void PropertySet(ref int PrivateField, string PropName, int value)
        {
            if (PrivateField != value)
            {
                PrivateField = value;
                this.NotifyPropertyChanged(PropName);
            }
        }

        public void PropertySet(ref double PrivateField, string PropName, double value)
        {
            if (PrivateField != value)
            {
                PrivateField = value;
                this.NotifyPropertyChanged(PropName);
            }
        }

        public void PropertySet(ref string PrivateField, string PropName, string value)
        {
            if (PrivateField != value)
            {
                PrivateField = value;
                this.NotifyPropertyChanged(PropName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }

    }
}
