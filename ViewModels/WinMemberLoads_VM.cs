using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAENN.ViewModels
{
    /// <summary>
    /// ViewModel used for WinMemberLoads class instances.
    /// </summary>
    class WinMemberLoads_VM : ObservableClass
    {
        #region Entry-form labels
        public string Text_Start { get; set; } = "Carga inicial\n   (" + Models.UnitsHolder.Load + ")";
        public string Text_End { get; set;  } = "Carga final\n   (" + Models.UnitsHolder.Load + ")";
        #endregion

        #region Boolean properties
        // Whether the X-direction load is variable (linear) or constant.
        private bool isvariable_x = false;
        public bool IsVariable_X { get => isvariable_x; set => PropertySet(ref isvariable_x, "IsVariable_X", value); }

        // Whether the Y-direction load is variable (linear) or constant.
        private bool isvariable_y = false;
        public bool IsVariable_Y { get => isvariable_y; set => PropertySet(ref isvariable_y, "IsVariable_Y", value); }

        // Whether the Z-direction load is variable (linear) or constant.
        private bool isvariable_z = false;
        public bool IsVariable_Z { get => isvariable_z; set => PropertySet(ref isvariable_z, "IsVariable_Z", value); }
        #endregion

        #region Textbox-bound properties

        // Start load values
        private string loadstart_x = "";
        public string LoadStart_X { get => loadstart_x; set => PropertySet(ref loadstart_x, "LoadStart_X", value); }

        private string loadstart_y = "";
        public string LoadStart_Y { get => loadstart_y; set => PropertySet(ref loadstart_y, "LoadStart_Y", value); }

        private string loadstart_z = "";
        public string LoadStart_Z { get => loadstart_z; set => PropertySet(ref loadstart_z, "LoadStart_Z", value); }


        // End load values
        private string loadend_x = "";
        public string LoadEnd_X { get => loadend_x; set => PropertySet(ref loadend_x, "LoadEnd_X", value); }

        private string loadend_y = "";
        public string LoadEnd_Y { get => loadend_y; set => PropertySet(ref loadend_y, "LoadEnd_Y", value); }

        private string loadend_z = "";
        public string LoadEnd_Z { get => loadend_z; set => PropertySet(ref loadend_z, "LoadEnd_Z", value); }
        #endregion
    
    

        /// <summary>
        /// Extracts the values in the textboxes and 
        /// </summary>
        /// <returns>Error code if fails, 0 otherwise.</returns>
        public int Apply()
        {
            try
            {
                // Converts each starting value string into double
                var x1 = Helper.Functions.GetDouble(LoadStart_X);
                var y1 = Helper.Functions.GetDouble(LoadStart_Y);
                var z1 = Helper.Functions.GetDouble(LoadStart_Z);
                
                // If the loads are variable, converts the endvalues into double (otherwise, repeats the starting values)
                var x2 = IsVariable_X ? Helper.Functions.GetDouble(LoadEnd_X) : x1;
                var y2 = IsVariable_Y ? Helper.Functions.GetDouble(LoadEnd_Y) : y1;
                var z2 = IsVariable_Z ? Helper.Functions.GetDouble(LoadEnd_Z) : z1;


                // If any of the values are null, returns an error
                if (!x1.HasValue || !y1.HasValue || !z2.HasValue)
                    return -1;
                if (IsVariable_X && !x2.HasValue || IsVariable_Y && !y2.HasValue || IsVariable_Z && !z2.HasValue)
                    return -1;

                // Creates a new dictionary comprised of the values, and stores it in the VarHolder
                VarHolder.ApplyLoad = new Dictionary<string, double>
                {
                    {"X1", x1.GetValueOrDefault() }, {"X2", x2.GetValueOrDefault() },
                    {"Y1", y1.GetValueOrDefault() }, {"Y2", y2.GetValueOrDefault() },
                    {"Z1", z1.GetValueOrDefault() }, {"Z2", z2.GetValueOrDefault() }
                };
                return 0;
            }
            // If unable to convert the values from string to double, returns an error
            catch (FormatException) { return -1; }



        }
    }
}
