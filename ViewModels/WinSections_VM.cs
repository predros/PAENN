using System;
using PAENN.Models;
using Helper;
using System.Windows;

namespace PAENN.ViewModels
{
    /// <summary>
    /// ViewModel class used for WinSections class instances.
    /// </summary>
    public class WinSections_VM : ObservableClass
    {

        public int SecType;

        #region Listbox header labels
        public string ListText_Iz { get; set; } = "Inércia Z (" + UnitsHolder.Inertia + ")";
        public string ListText_Iy { get; set; } = "Inércia Y (" + UnitsHolder.Inertia + ")";
        public string ListText_Ix { get; set; } = "Momento polar (" + UnitsHolder.Inertia + ")";
        public string ListText_Area { get; set; } = "Área (" + UnitsHolder.Inertia + ")";
        public string ListText_Ysup { get; set; } = "Y superior (" + UnitsHolder.Height + ")";
        public string ListText_Yinf { get; set; } = "Y inferior (" + UnitsHolder.Height + ")";
        public string ListText_Zsup { get; set; } = "Z superior (" + UnitsHolder.Height + ")";
        public string ListText_Zinf { get; set; } = "Z inferior (" + UnitsHolder.Height + ")";
        #endregion


        #region Entry form labels
        public string Text_Iz { get; set; } = "Inércia Z (" + UnitsHolder.Inertia + "):";
        public string Text_Iy { get; set; } = "Inércia Y (" + UnitsHolder.Inertia + "):";
        public string Text_Ix { get; set; } = "Momento polar (" + UnitsHolder.Inertia + "):";
        public string Text_Area { get; set; } = "Área (" + UnitsHolder.Area + "):";
        public string Text_Ysup { get; set; } = "Y superior (" + UnitsHolder.Height + "):";
        public string Text_Yinf { get; set; } = "Y inferior (" + UnitsHolder.Height + "):";
        public string Text_Zsup { get; set; } = "Z superior (" + UnitsHolder.Height + "):";
        public string Text_Zinf { get; set; } = "Z superior (" + UnitsHolder.Height + "):";

        public string Text_Dext { get; set; } = "D (" + UnitsHolder.Height + "):";
        public string Text_Dint { get; set; } = "d (" + UnitsHolder.Height + "):";
        public string Text_Base { get; set; } = "Base (" + UnitsHolder.Height + "):";
        public string Text_Height { get; set; } = "Altura (" + UnitsHolder.Height + "):";
        #endregion


        #region Textbox-bound properties
        private string entry_name = "";
        public string Entry_Name { get => entry_name; set => PropertySet(ref entry_name, "Entry_Name", value); }

        private string entry_ix = "";
        public string Entry_Ix { get => entry_ix; set => PropertySet(ref entry_ix, "Entry_Ix", value); }

        private string entry_iy = "";
        public string Entry_Iy { get => entry_iy; set => PropertySet(ref entry_iy, "Entry_Iy", value); }

        private string entry_iz = "";
        public string Entry_Iz { get => entry_iz; set => PropertySet(ref entry_iz, "Entry_Iz", value); }

        private string entry_area = "";
        public string Entry_Area { get => entry_area; set => PropertySet(ref entry_area, "Entry_Area", value); }

        private string entry_ysup = "";
        public string Entry_Ysup { get => entry_ysup; set => PropertySet(ref entry_ysup, "Entry_Ysup", value); }

        private string entry_yinf = "";
        public string Entry_Yinf { get => entry_yinf; set => PropertySet(ref entry_yinf, "Entry_Yinf", value); }

        private string entry_zsup = "";
        public string Entry_Zsup { get => entry_zsup; set => PropertySet(ref entry_zsup, "Entry_Zsup", value); }

        private string entry_zinf = "";
        public string Entry_Zinf { get => entry_zinf; set => PropertySet(ref entry_zinf, "Entry_Zinf", value); }


        private string entry_dext = "";
        public string Entry_Dext { get => entry_dext; set => PropertySet(ref entry_dext, "Entry_Dext", value); }

        private string entry_dint = "";
        public string Entry_Dint { get => entry_dint; set => PropertySet(ref entry_dint, "Entry_Dint", value); }


        private string entry_base = "";
        public string Entry_Base { get => entry_base; set => PropertySet(ref entry_base, "Entry_Base", value); }

        private string entry_height = "";
        public string Entry_Height { get => entry_height; set => PropertySet(ref entry_height, "Entry_Height", value); }
        #endregion


        #region Boolean properties
        private bool rb_generic = true;
        public bool RB_Generic { get => rb_generic; set => PropertySet(ref rb_generic, "RB_Generic", value); }

        private bool rb_circular = false;
        public bool RB_Circular { get => rb_circular; set => PropertySet(ref rb_circular, "RB_Circular", value); }

        private bool rb_rectangular = false;
        public bool RB_Rectangular { get => rb_rectangular; set => PropertySet(ref rb_rectangular, "RB_Rectangular", value); }
        #endregion


        #region Visibility properties
        public Visibility Visible_Generic { get; set; }  = Visibility.Visible;
        public Visibility Visible_Dummy { get; set; } = Visibility.Visible;
        public Visibility Visible_Circular { get; set; } = Visibility.Hidden;
        public Visibility Visible_Rectangular { get; set; } = Visibility.Hidden;
        #endregion



        /// <summary>
        /// Invoked when listbox selection changes, replaces the textbox values
        /// with the parameters of the given section.
        /// </summary>
        /// <param name="n">The selected section's index.</param>
        public void SelectionChanged(int n)
        {
            // Locates the selected section in the SectionsList.
            var section = VarHolder.SectionsList[n];
            Entry_Name = section.Name;

            // Acts according to the section's type (Generic, Circular, Rectangular).
            switch (section.SecType)
            {
                case "Genérico":
                    RB_Generic = true;
                    RB_Circular = RB_Rectangular = false;
                    Entry_Ix = section.Ix.ToString();
                    Entry_Iy = section.Iy.ToString();
                    Entry_Iz = section.Iz.ToString();
                    Entry_Area = section.Area.ToString();
                    Entry_Ysup = section.Ysup.ToString();
                    Entry_Yinf = section.Yinf.ToString();
                    Entry_Zsup = section.Zsup.ToString();
                    Entry_Zinf = section.Zinf.ToString();
                    break;
                case "Circular":
                    RB_Circular = true;
                    RB_Generic = RB_Rectangular = false;
                    Entry_Dext = section.Parameters[0].ToString();
                    Entry_Dint = section.Parameters[1].ToString();
                    break;
                case "Retangular":
                    RB_Rectangular = true;
                    RB_Generic = RB_Circular = false;
                    Entry_Base = section.Parameters[0].ToString();
                    Entry_Height = section.Parameters[1].ToString();
                    break;
            }
            // Invokes the type changing method, hiding and showing entry forms accordingly.
            TypeChanged(section.SecType);
        }



        /// <summary>
        /// Shows and hides form entries according to the section type.
        /// </summary>
        /// <param name="type">The section type</param>
        public void TypeChanged(string type)
        {
            // Shows and collapses groups of entries.
            switch (type)
            {
                case "Genérico":
                    Visible_Generic = Visible_Dummy = Visibility.Visible;
                    Visible_Circular = Visible_Rectangular = Visibility.Hidden;
                    SecType = 0;
                    break;
                case "Circular":
                    Visible_Generic = Visible_Rectangular = Visible_Dummy = Visibility.Hidden;
                    Visible_Circular = Visibility.Visible;
                    SecType = 1;
                    break;
                case "Retangular":
                    Visible_Generic = Visible_Circular = Visible_Dummy = Visibility.Hidden;
                    Visible_Rectangular = Visibility.Visible;
                    SecType = 2;
                    break;
            }

            // Notifies the changes in visibility so that WPF can act accordingly.
            NotifyPropertyChanged("Visible_Generic");
            NotifyPropertyChanged("Visible_Dummy");
            NotifyPropertyChanged("Visible_Rectangular");
            NotifyPropertyChanged("Visible_Circular");
        }



        /// <summary>
        /// Clears the text in every textbox.
        /// </summary>
        public void ClearText()
        {
            Entry_Name = Entry_Area = "";
            Entry_Ix = Entry_Iy = "";
            Entry_Ysup = Entry_Yinf = "";
            Entry_Zsup = Entry_Zinf = "";
            Entry_Dext = Entry_Dint = "";
            Entry_Base = Entry_Height = "";
        }



        /// <summary>
        /// Deletes the section at the given index and clears the textboxes.
        /// </summary>
        /// <param name="n">The section's index.</param>
        public void DeleteSection(int n)
        {
            VarHolder.SectionsList.RemoveAt(n);
            ClearText();
        }



        /// <summary>
        /// Changes the name of the given section to the given string.
        /// </summary>
        /// <param name="n">The section's index.</param>
        /// <param name="name">The new name.</param>
        public void RenameSection(int n, string name)
        {
            VarHolder.SectionsList[n].Name = name;
            Entry_Name = name;
        }



        /// <summary>
        /// Adds a new section with the name and properties currently in the textoxes.
        /// </summary>
        /// <returns>Error code if fails, zero if successful.</returns>
        public int AddSection()
        {
            // Trims the section name, removing any whitespaces at the start/end.
            var name = Entry_Name.Trim();

            // If the trimmed name is empty, returns an error.
            if (name == "")
                return -1;

            // Creates a new section with the given name and an array for its parameters.
            var s = new Section(name);
            var paramstring = new string[] { };


            // Act according to the selected section type.
            switch (SecType)
            {
                // If the section is Generic, the parameters are the property values themselves.
                case 0:
                    paramstring = new string[] { Entry_Ix, Entry_Iy, Entry_Iz, Entry_Area, Entry_Ysup, Entry_Yinf, Entry_Zsup, Entry_Zinf };
                    break;
                // If the section is Circular, the parameters are the diameters.
                case 1:
                    paramstring = new string[] { Entry_Dext, Entry_Dint};
                    break;
                // If the section is Rectangular, the parameters are the side lengths.
                case 2:
                    paramstring = new string[] { Entry_Base, Entry_Height};
                    break;
            }

            // Creates a double array to store the converted values.
            var parameters = new double[paramstring.Length];

            try
            {
                for (int i = 0; i < paramstring.Length; i++)
                {
                    // Convert each value from string to double
                    var parameter = Functions.GetDouble(paramstring[i], false);

                    // If negative, returns an error.
                    if (!parameter.HasValue)
                        return -3;

                    // If zero and not the circle's internal diameter, returns an error.
                    if (parameters[i] == 0 && (i != 1 || SecType != 1))
                        return -2;

                    // Stores the converted value in the double array.
                    parameters[i] = parameter.GetValueOrDefault();
                }
            }
            // If unable to convert from string to double, returns an error.
            catch (FormatException) { return -3;  }

            // Applies the correct section method, according to the section type.
            switch (SecType)
            {
                case 0:
                    s.Generic(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7]);
                    break;
                case 1:
                    s.Circle(parameters[0], parameters[1]);
                    break;
                case 2:
                    s.Rectangle(parameters[0], parameters[1]);
                    break;
            }

            // Adds the section to the VarHolder, clears the textboxes and returns 0.
            VarHolder.SectionsList.Add(s);
            ClearText();
            return 0;
        }



        /// <summary>
        /// Changes the selected section's properties according to the values in the textboxes.
        /// </summary>
        /// <param name="n">The section's index.</param>
        /// <returns>Error code if fails, zero if successful.</returns>
        public int EditSection(int n)
        {
            // Creates a string array to store the properties to be assigned.
            var paramstring = new string[] { };

            // Acts according to section type.
            switch (SecType)
            {
                // If the section is Generic, store the final properties.
                case 0:
                    paramstring = new string[] { Entry_Ix, Entry_Iy, Entry_Iz, Entry_Area, Entry_Ysup, Entry_Yinf, Entry_Zsup, Entry_Zinf };
                    break;
                // If the section is Circular, store the diameters.
                case 1:
                    paramstring = new string[] { Entry_Dext, Entry_Dint};
                    break;
                // If the section is Rectangular, store the side lengths.
                case 2:
                    paramstring = new string[] { Entry_Base, Entry_Height };
                    break;
            }

            // Creates a double array to store the converted values.
            var parameters = new double[paramstring.Length];

            try
            {
                for (int i = 0; i < paramstring.Length; i++)
                {
                    // Converts each value from string to double.
                    var parameter = Functions.GetDouble(paramstring[i], false);

                    // If the value is negative, returns an error.
                    if (!parameter.HasValue)
                        return -3;

                    // If the value is zero and not the internal diameter, returns an error.
                    if (parameter == 0 && (i != 1 || SecType != 1))
                        return -2;

                    // Stores the value in the double array.
                    parameters[i] = parameter.GetValueOrDefault();
                }
            }
            // If unable to convert from string to double, returns an error.
            catch (FormatException) { return -3; }


            // Applies section methods according to the section type.
            switch (SecType)
            {
                case 0:
                    VarHolder.SectionsList[n].Generic(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4],
                                                      parameters[5], parameters[6], parameters[7]);
                    break;
                case 1:
                    VarHolder.SectionsList[n].Circle(parameters[0], parameters[1]);
                    break;
                case 2:
                    VarHolder.SectionsList[n].Rectangle(parameters[0], parameters[1]);
                    break;
            }

            // Clears the textboxes and returns zero.
            ClearText();
            return 0;
        }
    }
}

