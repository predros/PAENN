using PAENN.Models;
using System.Collections.Generic;

namespace PAENN.ViewModels
{
    /// <summary>
    /// ViewModel class used for WinSupports class instances.
    /// </summary>
    public class WinSupports_VM : ObservableClass
    {
        #region Entry form labels
        public string Text_LinearSpring { get; set; } = "Linear (" + UnitsHolder.Spring + ")";
        public string Text_RotateSpring { get; set; } = "Angular (" + UnitsHolder.TorsionSpring + ")";
        public string Text_PrescrDispl { get; set; } = "Deslocamento (" + UnitsHolder.Displacement + ")";
        public string Text_PrescrRot { get; set; } = "Rotação (" + UnitsHolder.Rotation + ")";
        #endregion


        #region Nodal restriction properties
        private bool restrux;
        public bool RestrUx { get => restrux; set => PropertySet(ref restrux, "RestrUx", value); }

        private bool restrrx;
        public bool RestrRx { get => restrrx; set => PropertySet(ref restrrx, "RestrRx", value); }

        private bool restruy;
        public bool RestrUy { get => restruy; set => PropertySet(ref restruy, "RestrUy", value); }

        private bool restrry;
        public bool RestrRy { get => restrry; set => PropertySet(ref restrry, "RestrRy", value); }

        private bool restruz;
        public bool RestrUz { get => restruz; set => PropertySet(ref restruz, "RestrUz", value); }

        private bool restrrz;
        public bool RestrRz { get => restrrz; set => PropertySet(ref restrrz, "RestrRz", value); }
        #endregion


        #region Spring constant properties
        private string kux = "";
        public string KUx { get => kux; set => PropertySet(ref kux, "KUx", value); }

        private string krx = "";
        public string KRx { get => krx; set => PropertySet(ref krx, "KRx", value); }

        private string kuy = "";
        public string KUy { get => kuy; set => PropertySet(ref kuy, "KUy", value); }

        private string kry = "";
        public string KRy { get => kry; set => PropertySet(ref kry, "KRy", value); }

        private string kuz = "";
        public string KUz { get => kuz; set => PropertySet(ref kuz, "KUz", value); }

        private string krz = "";
        public string KRz { get => krz; set => PropertySet(ref krz, "KRz", value); }
        #endregion


        #region Prescribed displacement properties
        private string pux = "";
        public string PUx { get => pux; set => PropertySet(ref pux, "PUx", value); }

        private string prx = "";
        public string PRx { get => prx; set => PropertySet(ref prx, "PRx", value); }

        private string puy = "";
        public string PUy { get => puy; set => PropertySet(ref puy, "PUy", value); }
       
        private string pry = "";
        public string PRy { get => pry; set => PropertySet(ref pry, "PRy", value); }

        private string puz = "";
        public string PUz { get => puz; set => PropertySet(ref puz, "PUz", value); }

        private string prz = "";
        public string PRz { get => prz; set => PropertySet(ref prz, "PRz", value); }
        #endregion



    /// <summary>
    /// Converts the current support conditions to dictionaries and stores in the VarHolder.
    /// </summary>
    /// <returns>Error codes if fails, zero if successful.</returns>
    public int SupportApply()
        {
            // Converts each spring constant from string to double.
            var kdx = Helper.Functions.GetDouble(KUx, false);
            var kdy = Helper.Functions.GetDouble(KUy, false);
            var kdz = Helper.Functions.GetDouble(KUz, false);

            var krx = Helper.Functions.GetDouble(KRx, false);
            var kry = Helper.Functions.GetDouble(KRy, false);
            var krz = Helper.Functions.GetDouble(KRz, false);

            // If any valid spring constant is negative, returns an error.
            if (!kdx.HasValue && !RestrUx || !kdy.HasValue && !RestrUy || !kdz.HasValue && !RestrUz ||
                !krx.HasValue && !RestrRx || !kry.HasValue && !RestrRy || !krz.HasValue && !RestrRz)
                return -1;

            // Converts each prescribed displacement from string to double
            var pdx = Helper.Functions.GetDouble(PUx);
            var pdy = Helper.Functions.GetDouble(PUy);
            var pdz = Helper.Functions.GetDouble(PUz);
            var prx = Helper.Functions.GetDouble(PRx);
            var pry = Helper.Functions.GetDouble(PRy);
            var prz = Helper.Functions.GetDouble(PRz);


            // Checks if each DOF is free, assigning its spring constant if true and zero if false.
            double K1 = (RestrUx) ? 0 : kdx.GetValueOrDefault(0);
            double K2 = (RestrRx) ? 0 : krx.GetValueOrDefault(0);
            double K3 = (RestrUy) ? 0 : kdy.GetValueOrDefault(0);
            double K4 = (RestrRy) ? 0 : kry.GetValueOrDefault(0);
            double K5 = (RestrUz) ? 0 : kdz.GetValueOrDefault(0);
            double K6 = (RestrRz) ? 0 : krz.GetValueOrDefault(0);

            // Checks if each DOF is free, assigning its prescribed displacement if false and zero if true.
            double P1 = (RestrUx) ? pdx.GetValueOrDefault(0) : 0;
            double P2 = (RestrUx) ? prx.GetValueOrDefault(0) : 0;
            double P3 = (RestrUx) ? pdy.GetValueOrDefault(0) : 0;
            double P4 = (RestrUx) ? pry.GetValueOrDefault(0) : 0;
            double P5 = (RestrUx) ? pdz.GetValueOrDefault(0) : 0;
            double P6 = (RestrUx) ? prz.GetValueOrDefault(0) : 0;


            // Stores the values in the VarHolder appliable dictionaries and returns zero.
            VarHolder.ApplyRestr = new Dictionary<string, bool> { { "Ux", RestrUx }, { "Rx", RestrRx }, { "Uy", RestrUy },
                                                                  { "Ry", RestrRy }, { "Uz", RestrUz }, { "Rz", RestrRz} };

            VarHolder.ApplySpring = new Dictionary<string, double>{ { "Ux", K1 }, { "Rx", K2 }, { "Uy", K3 },
                                                                    { "Ry", K4 }, { "Uz", K5 }, { "Rz", K6} };
            
            VarHolder.ApplyPdispl = new Dictionary<string, double>{ { "Ux", P1 }, { "Rx", P2 }, { "Uy", P3 },
                                                                    { "Ry", P4 }, { "Uz", P5 }, { "Rz", P6} };
            return 0;
        }

    }
}
