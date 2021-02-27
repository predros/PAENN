using PAENN.Models;
using System.Collections.Generic;

namespace PAENN.ViewModels
{
    public class WinSupports_VM : ObservableClass
    {
        #region Text labels
        public string Text_LinearSpring { get; set; }
        public string Text_RotateSpring { get; set; }
        public string Text_PrescrDispl { get; set; }
        public string Text_PrescrRot { get; set; }
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

        #region Spring constants
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

        #region Prescribed displacements
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


    public WinSupports_VM()
        {
            Text_LinearSpring = "Linear (" + UnitsHolder.Spring + ")";
            Text_RotateSpring = "Angular (" + UnitsHolder.TorsionSpring + ")";

            Text_PrescrDispl = "Deslocamento (" + UnitsHolder.Displacement + ")";
            Text_PrescrRot = "Rotação (" + UnitsHolder.Rotation + ")";
        }

    public int SupportApply()
        {
            var kdx = Helper.Functions.GetDouble(KUx, false);
            var kdy = Helper.Functions.GetDouble(KUy, false);
            var kdz = Helper.Functions.GetDouble(KUz, false);

            var krx = Helper.Functions.GetDouble(KRx, false);
            var kry = Helper.Functions.GetDouble(KRy, false);
            var krz = Helper.Functions.GetDouble(KRz, false);

            if (kdx == null && !RestrUx || kdy == null && !RestrUy || kdz == null && !RestrUz)
                return -1;

            if (krx == null && !RestrRx || kry == null && !RestrRy || krz == null && !RestrRz)
                return -1;

            var pdx = Helper.Functions.GetDouble(PUx);
            var pdy = Helper.Functions.GetDouble(PUy);
            var pdz = Helper.Functions.GetDouble(PUz);
            var prx = Helper.Functions.GetDouble(PRx);
            var pry = Helper.Functions.GetDouble(PRy);
            var prz = Helper.Functions.GetDouble(PRz);

            double K1 = (RestrUx) ? 0 : kdx.GetValueOrDefault(0);
            double K2 = (RestrRx) ? 0 : krx.GetValueOrDefault(0);
            double K3 = (RestrUy) ? 0 : kdy.GetValueOrDefault(0);
            double K4 = (RestrRy) ? 0 : kry.GetValueOrDefault(0);
            double K5 = (RestrUz) ? 0 : kdz.GetValueOrDefault(0);
            double K6 = (RestrRz) ? 0 : krz.GetValueOrDefault(0);

            double P1 = (RestrUx) ? pdx.GetValueOrDefault(0) : 0;
            double P2 = (RestrUx) ? prx.GetValueOrDefault(0) : 0;
            double P3 = (RestrUx) ? pdy.GetValueOrDefault(0) : 0;
            double P4 = (RestrUx) ? pry.GetValueOrDefault(0) : 0;
            double P5 = (RestrUx) ? pdz.GetValueOrDefault(0) : 0;
            double P6 = (RestrUx) ? prz.GetValueOrDefault(0) : 0;


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
