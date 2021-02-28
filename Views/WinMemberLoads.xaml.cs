using System.Windows;
using PAENN.ViewModels;

namespace PAENN.Views
{
    /// <summary>
    /// "Add member loads" window internal logic.
    /// </summary>
    public partial class WinMemberLoads : Window
    {
        WinMemberLoads_VM VM = new WinMemberLoads_VM();


        /// <summary>
        /// WinMemberLoads class constructor.
        /// </summary>
        public WinMemberLoads()
        {
            InitializeComponent();
            DataContext = VM;
        }



        /// <summary>
        /// Handles the Closing event.
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VarHolder.ClickType = "Select";
        }

        /// <summary>
        /// Handles the Close button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
                VarHolder.ClickType = "MemberLoads";
        }
    }
}
