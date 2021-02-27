using System;
using System.ComponentModel;
using PAENN.Models;


namespace PAENN.ViewModels
{
    class WinNewNode_VM : ObservableClass
    {
       
        private string entry_XCoord;
        public string Entry_XCoord { get => entry_XCoord; set => PropertySet(ref entry_XCoord, "Entry_XCoord", value); }

        private string entry_YCoord;
        public string Entry_YCoord { get => entry_YCoord; set => PropertySet(ref entry_YCoord, "Entry_YCoord", value); }
        
        private string entry_ZCoord;
        public string Entry_ZCoord { get => entry_ZCoord; set => PropertySet(ref entry_ZCoord, "Entry_ZCoord", value); }


        public string Text_XCoord { get; set; }
        public string Text_YCoord { get; set; }

        public string Text_ZCoord { get; set; }


        public WinNewNode_VM()
        {
            var unit = Models.UnitsHolder.Length;
            Text_XCoord = "Coordenada X (" + unit + "):";
            Text_YCoord = "Coordenada Y (" + unit + "):";
            Text_ZCoord = "Coordenada Z (" + unit + "):";

            Entry_XCoord = "";
            Entry_YCoord = "";
            Entry_ZCoord = "";
        }


        public int Apply()
        {
            double X;
            double Y;
            double Z;

            if (Entry_XCoord.Trim() == "")
                Entry_XCoord = "0";
            if (Entry_YCoord.Trim() == "")
                Entry_YCoord = "0";
            if (Entry_ZCoord.Trim() == "")
                Entry_ZCoord = "0";

            try
            {
                X = Convert.ToDouble(Entry_XCoord);
                Y = Convert.ToDouble(Entry_YCoord);
                Z = Convert.ToDouble(Entry_ZCoord);

                int create = ActionHandler.NewNode(X, Y, Z, 1);

                return create;
            }
            catch (FormatException)
            {
                return -2;
            }
        }
    }
}
