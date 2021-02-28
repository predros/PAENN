using System.Windows;
using PAENN.ViewModels;


namespace PAENN.Views
{
    /// <summary>
    /// "Add nodal forces" window internal logic.
    /// </summary>
    public partial class WinNodalForces : Window
    {
        WinNodalForces_VM VM = new WinNodalForces_VM();

        /// <summary>
        /// WinNodalForces class constructor.
        /// </summary>
        public WinNodalForces()
        {
            InitializeComponent();
            DataContext = VM;
        }



        /// <summary>
        /// Handles the Apply button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            var apply = VM.Apply();

            if (apply == -1)
                MessageBox.Show("Insira valores válidos.", "Erro");
            else
                VarHolder.ClickType = "NodalForces";
        }

        /// <summary>
        /// Handles the Closing event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VarHolder.ClickType = "Select";
        }

        /// <summary>
        /// Handles the Close buton click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
