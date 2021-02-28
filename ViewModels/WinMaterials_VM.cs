using System;
using Helper;
using PAENN.Models;

namespace PAENN.ViewModels
{
    /// <summary>
    /// ViewModel used in WinMaterials class instances.
    /// </summary>
    public class WinMaterials_VM : ObservableClass
    {
        #region Listbox header labels
        public string ListText_Elasticity { get; set; } = "Elasticidade (" + UnitsHolder.Elasticity + ")";          // Elasticity (Young's) modulus
        public string ListText_Transversal { get; set; } = "Elast. transversal (" + UnitsHolder.Elasticity + ")";   // Transversal elasticity modulus
        public string ListText_Thermal { get; set; }  = "Coef. térmico (" + UnitsHolder.Thermal + ")";              // Thermal expansion coefficient
        #endregion

        #region Entry form labels
        public string Text_Elasticity { get; set; } = "Elasticidade (" + UnitsHolder.Elasticity + "):";             // Elasticity (Young's) modulus
        public string Text_Transversal { get; set; } = "Elast. transversal (" + UnitsHolder.Elasticity + "):";      // Transversal elasticity modulus
        public string Text_Thermal { get; set; } = "Coef. térmico (" + UnitsHolder.Thermal + "):";                  // Thermal expansion coefficient
        #endregion

        #region Textbox-bound properties
        private string entry_name = "";             // The material's name
        public string Entry_Name { get => entry_name; set => PropertySet(ref entry_name, "Entry_Name", value); }

        private string entry_elasticity = "";       // Elasticity (Young's) modulus
        public string Entry_Elasticity { get => entry_elasticity; set => PropertySet(ref entry_elasticity, "Entry_Elasticity", value); }

        private string entry_transversal = "";      // Transversal elasticity modulus
        public string Entry_Transversal { get => entry_transversal; set => PropertySet(ref entry_transversal, "Entry_Transversal", value); }

        private string entry_thermal = "";          // Thermal expansion coefficient
        public string Entry_Thermal { get => entry_thermal; set => PropertySet(ref entry_thermal, "Entry_Thermal", value); }
        #endregion



        /// <summary>
        /// Invoked when the listbox selection changes, used to fill the entry forms
        /// with the selected material's properties.
        /// </summary>
        /// <param name="n">The selected material's index.</param>
        public void SelectionChanged(int n)
        {
            Entry_Name = VarHolder.MaterialsList[n].Name;
            Entry_Elasticity = VarHolder.MaterialsList[n].Elasticity.ToString();
            Entry_Thermal = VarHolder.MaterialsList[n].Thermal.ToString();
            Entry_Transversal = VarHolder.MaterialsList[n].Transversal.ToString();
        }



        /// <summary>
        /// Deletes the material at the given index and clears the entry forms.
        /// </summary>
        /// <param name="n">The material's index.</param>
        public void DeleteMaterial(int n)
        {
            VarHolder.MaterialsList.RemoveAt(n);

            Entry_Name = "";
            Entry_Elasticity = "";
            Entry_Thermal = "";
            Entry_Transversal = "";
        }



        /// <summary>
        /// Replaces the material's name at the given index with the given name.
        /// </summary>
        /// <param name="n">The material's index.</param>
        /// <param name="name">The new name.</param>
        public void RenameMaterial(int n, string name)
        {
            VarHolder.MaterialsList[n].Name = name;
            Entry_Name = name;
        }



        /// <summary>
        /// Checks if there exists a material with the name currently in the "Name" textbox.
        /// </summary>
        /// <returns></returns>
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



        /// <summary>
        /// Adds a new material with the name and properties currently in the textboxes.
        /// </summary>
        /// <returns>Error code if fails, 0 otherwise.</returns>
        public int AddMaterial()
        {
            // Trims the textboxes, removing any whitespaces at the start/end.
            var name = Entry_Name.Trim();
            var elasticity = Entry_Elasticity.Trim();
            var thermal = Entry_Thermal.Trim();
            var transversal = Entry_Transversal.Trim();

            // If the name string is empty, returns an error.
            if (name == "")
                return -1;

            try 
            {
                // Converts the property strings into double values.
                var elast = Functions.GetDouble(elasticity, false, false);
                var alpha = Functions.GetDouble(thermal, false, false);
                var transv = Functions.GetDouble(transversal,false, false);

                // If any of them are non-positive, returns an error.
                if (!elast.HasValue || !alpha.HasValue || !transv.HasValue) return -2;


                // Creates a new material and adds to the list.
                var m = new Material(name, elast.GetValueOrDefault(), transv.GetValueOrDefault(), alpha.GetValueOrDefault());
                VarHolder.MaterialsList.Add(m);

                // Clears the entry-boxes and returns 0 (successful).
                Entry_Name = "";
                Entry_Elasticity = "";
                Entry_Thermal = "";
                Entry_Transversal = "";
                return 0;
            }
            // If not possible to convert any strings into double, returns an error.
            catch (FormatException) { return -3; }
        }



        /// <summary>
        /// Changes a material's properties according to the values in the textboxes.
        /// </summary>
        /// <param name="n">The material's index.</param>
        /// <returns>Error code if fails, 0 otherwise.</returns>
        public int EditMaterial(int n)
        {
            // Trims the strings, removing any whitespaces at the start/end.
            var elasticity = Entry_Elasticity.Trim();
            var thermal = Entry_Thermal.Trim();
            var transversal = Entry_Transversal.Trim();

            try
            {
                // Tries to convert the strings to double values.
                double? elast = Functions.GetDouble(elasticity, false, false);
                double? alpha = Functions.GetDouble(thermal, false, false);
                double? transv = Functions.GetDouble(transversal, false, false);

                // If the values are non-positive, returns an error.
                if (!elast.HasValue || !alpha.HasValue || !transv.HasValue) return -2;

                // Changes the selected material's properties.
                VarHolder.MaterialsList[n].Elasticity = elast.GetValueOrDefault();
                VarHolder.MaterialsList[n].Thermal = alpha.GetValueOrDefault();
                VarHolder.MaterialsList[n].Transversal = transv.GetValueOrDefault();

                // Clears the textboxes and returns 0 (successful).
                Entry_Name = "";
                Entry_Elasticity = "";
                Entry_Thermal = "";
                Entry_Transversal = "";
                return 0;
            }
            // If not possible to convert the values to double, returns an error.
            catch (FormatException) { return -3; }  
        }

        

        /// <summary>
        /// Applies the given material to every member in the structure.
        /// </summary>
        /// <param name="n">The material's index.</param>
        public void ApplyAll(int n)
        {
            var material = VarHolder.MaterialsList[n];

            foreach (Member member in VarHolder.MembersList)
                member.Material = material;
        }

    }
}
