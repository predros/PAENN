using System;
using System.Collections.Generic;

namespace PAENN.ViewModels
{
    /// <summary>
    /// ViewModel class used for WinNodalForces class instances.
    /// </summary>
    public class WinNodalForces_VM : ObservableClass
    {
        #region Entry form labels
        public string Text_Force { get; set; } = "Força (" + Models.UnitsHolder.Force + ")";
        public string Text_Moment { get; set; } = "Momento (" + Models.UnitsHolder.Moment + ")";
        #endregion


        #region Textbox-bound properties
        private string entry_fx = "";
        public string Entry_Fx { get => entry_fx; set => PropertySet(ref entry_fx, "Entry_Fx", value); }

        private string entry_fy = "";
        public string Entry_Fy { get => entry_fy; set => PropertySet(ref entry_fy, "Entry_Fy", value); }

        private string entry_fz = "";
        public string Entry_Fz { get => entry_fz; set => PropertySet(ref entry_fz, "Entry_Fz", value); }

        private string entry_mx = "";
        public string Entry_Mx { get => entry_mx; set => PropertySet(ref entry_mx, "Entry_Mx", value); }

        private string entry_my = "";
        public string Entry_My { get => entry_my; set => PropertySet(ref entry_my, "Entry_My", value); }

        private string entry_mz = "";
        public string Entry_Mz { get => entry_mz; set => PropertySet(ref entry_mz, "Entry_Mz", value); }
        #endregion



        /// <summary>
        /// Converts the current values in the texboxes into a dictionary and stores in the VarHolder.
        /// </summary>
        /// <returns>Error code if fails, 0 if successful.</returns>
        public int Apply()
        {
            try
            {
                // Converts each string into double.
                var fx = Convert.ToDouble(Entry_Fx);
                var fy = Convert.ToDouble(Entry_Fy);
                var fz = Convert.ToDouble(Entry_Fz);

                var mx = Convert.ToDouble(Entry_Mx);
                var my = Convert.ToDouble(Entry_My);
                var mz = Convert.ToDouble(Entry_Mz);

                // Stores the values in the VarHolder and returns 0.
                VarHolder.ApplyNodal = new Dictionary<string, double> { { "Fx", fx }, { "Fy", fy }, { "Fz", fz },
                                                                        { "Mx", mx }, { "My", my }, { "Mz", mz } };
                return 0;
            }
            // If unable to convert the strings to double, returns an error.
            catch (FormatException) { return -1; }
        }
    }
}
