using System;
using System.Collections.Generic;
using System.ComponentModel;
using PAENN.Models;


namespace PAENN.ViewModels
{
    public class WinNewMember_VM : ObservableClass
    {
        public string Text_X1 { get; set; }
        public string Text_X2 { get; set; }


        public string Text_Y1 { get; set; }
        public string Text_Y2 { get; set; }

        public string Text_Z1 { get; set; }
        public string Text_Z2 { get; set; }


        private string entry_X1;
        public string Entry_X1 { get => entry_X1; set => PropertySet(ref entry_X1, "Entry_X1", value); }

        private string entry_X2;
        public string Entry_X2 { get => entry_X2; set => PropertySet(ref entry_X2, "Entry_X2", value); }

        private string entry_Y1;
        public string Entry_Y1 { get => entry_Y1; set => PropertySet(ref entry_Y1, "Entry_Y1", value); }

        private string entry_Y2;
        public string Entry_Y2 { get => entry_Y2; set => PropertySet(ref entry_Y2, "Entry_Y2", value); }

        private string entry_Z1;
        public string Entry_Z1 { get => entry_Z1; set => PropertySet(ref entry_Z1, "Entry_Z1", value); }

        private string entry_Z2;
        public string Entry_Z2 { get => entry_Z2; set => PropertySet(ref entry_Z2, "Entry_Z2", value); }


        private string var_Material;
        public string Var_Material { get => var_Material; set => PropertySet(ref var_Material, "Var_Material", value); }

        private string var_Section;
        public string Var_Section { get => var_Section; set => PropertySet(ref var_Section, "Var_Section", value); }

        public List<string> List_Materials { get; set; }
        public List<string> List_Sections { get; set; }


        public WinNewMember_VM()
        {
            var unit = PAENN.Models.UnitsHolder.Length;
            Text_X1 = "X inicial (" + unit + "):";
            Text_X2 = "X final (" + unit + "):";
            Text_Y1 = "Y inicial (" + unit + "):";
            Text_Y2 = "Y final (" + unit + "):";
            Text_Z1 = "Z inicial (" + unit + "):";
            Text_Z2 = "Z final (" + unit + "):";

            Entry_X1 = "";
            Entry_X2 = "";
            Entry_Y1 = "";
            Entry_Y2 = "";
            Entry_Z1 = "";
            Entry_Z2 = "";

            List_Materials = new List<string>();
            List_Sections = new List<string>();

            if (VarHolder.MaterialsList.Count != 0)
            {
                foreach (Material material in VarHolder.MaterialsList)
                    List_Materials.Add(material.Name);
            }

            if (VarHolder.SectionsList.Count != 0)
            {
                foreach (Section section in VarHolder.SectionsList)
                    List_Sections.Add(section.Name);
            }
        }

        public int Apply()
        {
            if (Entry_X1.Trim() == "")
                Entry_X1 = "0";
            if (Entry_X2.Trim() == "")
                Entry_X2 = "0";
            if (Entry_Y1.Trim() == "")
                Entry_Y1 = "0";
            if (Entry_Y2.Trim() == "")
                Entry_Y2 = "0";
            if (Entry_Z1.Trim() == "")
                Entry_Z1 = "0";
            if (Entry_Z2.Trim() == "")
                Entry_Z2 = "0";

            try
            {
                double X1 = Convert.ToDouble(Entry_X1);
                double X2 = Convert.ToDouble(Entry_X2);
                double Y1 = Convert.ToDouble(Entry_Y1);
                double Y2 = Convert.ToDouble(Entry_Y2);
                double Z1 = Convert.ToDouble(Entry_Z1);
                double Z2 = Convert.ToDouble(Entry_Z2);

                if (X1 == X2 && Y1 == Y2 && Z1 == Z2)
                    return -5;



                if (Var_Material.Trim() == "")
                    return -2;

                if (Var_Section.Trim() == "")
                    return -3;

                var MatIndex = List_Materials.IndexOf(Var_Material);
                var SecIndex = List_Sections.IndexOf(Var_Section);

                var Material = VarHolder.MaterialsList[MatIndex];
                var Section = VarHolder.SectionsList[SecIndex];

                int create = ActionHandler.NewMember(X1, Y1, Z1, X2, Y2, Z2, Material, Section);

                if (create != 0)
                {
                    return -4;
                }

                var X0 = X2;
                var Y0 = Y2;
                var Z0 = Z2;
                var XF = 2 * X2 - X1;
                var YF = 2 * Y2 - Y1;
                var ZF = 2 * Z2 - Z1;

                Entry_X1 = X0.ToString();
                Entry_Y1 = Y0.ToString();
                Entry_Z1 = Z0.ToString();

                Entry_X2 = XF.ToString();
                Entry_Y2 = YF.ToString();
                Entry_Z2 = ZF.ToString();
            }
            catch (FormatException)
            {
                return -1;
            }
            return 0;
        }
    }
}
