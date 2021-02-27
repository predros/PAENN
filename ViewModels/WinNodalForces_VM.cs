using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAENN.ViewModels
{
    public class WinNodalForces_VM : ObservableClass
    {
        private string text_force;
        public string Text_Force { get => text_force; set => PropertySet(ref text_force, "Text_Force", value); }

        private string text_moment;
        public string Text_Moment { get => text_moment; set => PropertySet(ref text_moment, "Text_Moment", value); }


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


        public WinNodalForces_VM()
        {
            Text_Force = "Força (" + Models.UnitsHolder.Force + ")";
            Text_Moment = "Momento (" + Models.UnitsHolder.Moment + ")";
        }

        public int Apply()
        {
            var fx = Helper.Functions.GetDouble(Entry_Fx);
            var fy = Helper.Functions.GetDouble(Entry_Fy);
            var fz = Helper.Functions.GetDouble(Entry_Fz);

            var mx = Helper.Functions.GetDouble(Entry_Mx);
            var my = Helper.Functions.GetDouble(Entry_My);
            var mz = Helper.Functions.GetDouble(Entry_Mz);

            if (!fx.HasValue || !fy.HasValue || !fz.HasValue || !mx.HasValue || !my.HasValue || !mz.HasValue)
                return -1;

            VarHolder.ApplyNodal = new Dictionary<string, double> { { "Fx", fx.GetValueOrDefault() }, { "Fy", fy.GetValueOrDefault() },
                                                                    { "Fz", fz.GetValueOrDefault() }, { "Mx", mx.GetValueOrDefault() },
                                                                    { "My", my.GetValueOrDefault() }, { "Mz", mz.GetValueOrDefault() } };

            return 0;
        }
    }
}
