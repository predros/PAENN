using System;
using Helper;
using PAENN.Models;

namespace PAENN.ViewModels
{
    public class WinMaterials_VM : ObservableClass
    {

        public string ListText_Elasticity { get; set; } = "Elasticidade (" + UnitsHolder.Elasticity + ")";

        public string ListText_Transversal { get; set; } = "Elast. transversal (" + UnitsHolder.Elasticity + ")";
        public string ListText_Thermal { get; set; }  = "Coef. térmico (" + UnitsHolder.Thermal + ")";

        public string Text_Elasticity { get; set; } = "Elasticidade (" + UnitsHolder.Elasticity + "):";
        public string Text_Transversal { get; set; } = "Elast. transversal (" + UnitsHolder.Elasticity + "):";
        public string Text_Thermal { get; set; } = "Coef. térmico (" + UnitsHolder.Thermal + "):";



        private string entry_name = "";
        public string Entry_Name { get => entry_name; set => PropertySet(ref entry_name, "Entry_Name", value); }

        private string entry_elasticity = "";
        public string Entry_Elasticity { get => entry_elasticity; set => PropertySet(ref entry_elasticity, "Entry_Elasticity", value); }

        private string entry_transversal = "";
        public string Entry_Transversal { get => entry_transversal; set => PropertySet(ref entry_transversal, "Entry_Transversal", value); }

        private string entry_thermal = "";
        public string Entry_Thermal { get => entry_thermal; set => PropertySet(ref entry_thermal, "Entry_Thermal", value); }

        public void SelectionChanged(int n)
        {
            Entry_Name = VarHolder.MaterialsList[n].Name;
            Entry_Elasticity = VarHolder.MaterialsList[n].Elasticity.ToString();
            Entry_Thermal = VarHolder.MaterialsList[n].Thermal.ToString();
            Entry_Transversal = VarHolder.MaterialsList[n].Transversal.ToString();
        }

        public void DeleteMaterial(int n)
        {
            VarHolder.MaterialsList.RemoveAt(n);

            Entry_Name = "";
            Entry_Elasticity = "";
            Entry_Thermal = "";
            Entry_Transversal = "";
        }

        public void RenameMaterial(int n, string name)
        {
            VarHolder.MaterialsList[n].Name = name;
            Entry_Name = name;
        }

        public int CheckIfExists()
        {
            var name = Entry_Name.Trim();
            var n = -1;

            foreach (Material material in VarHolder.MaterialsList)
            {
                if (material.Name == name)
                {
                    n = VarHolder.MaterialsList.IndexOf(material);
                    break;
                }
            }

            return n;
        }

        public int AddMaterial()
        {
            var name = Entry_Name.Trim();
            var elasticity = Entry_Elasticity.Trim();
            var thermal = Entry_Thermal.Trim();
            var transversal = Entry_Transversal.Trim();

            if (name == "")
                return -1;

            try 
            {
                double elast = (double)Functions.GetDouble(elasticity);
                double alpha = (double)Functions.GetDouble(thermal);
                double transv = (double)Functions.GetDouble(transversal);

                if (elast <= 0 || alpha <= 0 || transv <= 0) return -2;

                var m = new Material(name, elast, transv, alpha);
                VarHolder.MaterialsList.Add(m);

                Entry_Name = "";
                Entry_Elasticity = "";
                Entry_Thermal = "";
                Entry_Transversal = "";

                return 0;
            }
            catch (FormatException) { return -3; }
        }

        public int EditMaterial(int n)
        {
            var elasticity = Entry_Elasticity.Trim();
            var thermal = Entry_Thermal.Trim();
            var transversal = Entry_Transversal.Trim();

            try
            {
                double? elast = Functions.GetDouble(elasticity, false, false);
                double? alpha = Functions.GetDouble(thermal, false, false);
                double? transv = Functions.GetDouble(transversal, false, false);

                if (elast == null || alpha == null || transv == null) return -2;

                VarHolder.MaterialsList[n].Elasticity = elast.GetValueOrDefault();
                VarHolder.MaterialsList[n].Thermal = alpha.GetValueOrDefault();
                VarHolder.MaterialsList[n].Transversal = transv.GetValueOrDefault();

                Entry_Name = "";
                Entry_Elasticity = "";
                Entry_Thermal = "";
                Entry_Transversal = "";
            }
            catch (FormatException) { return -3; }

            return 0;
        }

        public void ApplyAll(int n)
        {
            var material = VarHolder.MaterialsList[n];

            foreach (Member member in VarHolder.MembersList)
                member.Material = material;
        }

    }
}
