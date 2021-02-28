using System;
using System.ComponentModel;


namespace PAENN.ViewModels
{
    /// <summary>
    /// ViewModel class used for WinNewNode class instances.
    /// </summary>
    class WinNewNode_VM : ObservableClass
    {
        #region Entry form labels
        public string Text_XCoord { get; set; } = "Coordenada X (" + Models.UnitsHolder.Length + "):";
        public string Text_YCoord { get; set; } = "Coordenada Y (" + Models.UnitsHolder.Length + "):";
        public string Text_ZCoord { get; set; } = "Coordenada Z (" + Models.UnitsHolder.Length + "):";
        #endregion


        #region Textbox-bound properties
        private string entry_XCoord = "";
        public string Entry_XCoord { get => entry_XCoord; set => PropertySet(ref entry_XCoord, "Entry_XCoord", value); }

        private string entry_YCoord = "";
        public string Entry_YCoord { get => entry_YCoord; set => PropertySet(ref entry_YCoord, "Entry_YCoord", value); }
        
        private string entry_ZCoord = "";
        public string Entry_ZCoord { get => entry_ZCoord; set => PropertySet(ref entry_ZCoord, "Entry_ZCoord", value); }
        #endregion


        /// <summary>
        /// Creates a new node, at the coordinates currently in the textboxes.
        /// </summary>
        /// <returns>Error code if fails, 0 if successful.</returns>
        public int Apply()
        {

            // Trims the textboxes. If they're empty, adds zeroes to them.
            if (Entry_XCoord.Trim() == "")
                Entry_XCoord = "0";
            if (Entry_YCoord.Trim() == "")
                Entry_YCoord = "0";
            if (Entry_ZCoord.Trim() == "")
                Entry_ZCoord = "0";

            try
            {
                // Converts each string to double
                var X = Convert.ToDouble(Entry_XCoord);
                var Y = Convert.ToDouble(Entry_YCoord);
                var Z = Convert.ToDouble(Entry_ZCoord);

                // Creates the node and returns the result from the ActionHandler
                int create = ActionHandler.NewNode(X, Y, Z, 1);
                return create;
            }
            // If unable to convert from string to double, returns an error.
            catch (FormatException) { return -2; }
        }
    }
}
