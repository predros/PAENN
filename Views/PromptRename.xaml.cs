using System.Windows;


namespace PAENN.Views
{
    /// <summary>
    /// Prompt window using for renaming objects.
    /// </summary>
    public partial class PromptRename : Window
    {
        public bool Result = false;
        static public string Text;


        /// <summary>
        /// PromptRename class constructor.
        /// </summary>
        public PromptRename()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PromptLoaded);
        }



        /// <summary>
        /// Creates an instance of the prompt window and returns the textbox string.
        /// </summary>
        /// <returns>The new name.</returns>
        public static string Rename()
        {
            var win = new PromptRename();
            win.ShowDialog();

            if (win.Result == true)
                return win.TextBox_Name.Text;
            else
                return "";
        }
        


        /// <summary>
        /// Handles the prompt window loading.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void PromptLoaded(object sender, RoutedEventArgs e)
        {
            TextBox_Name.Focus();
        }

        /// <summary>
        /// Handles the clicking of the Cancel button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the clicking of the Ok button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }
    }
}
