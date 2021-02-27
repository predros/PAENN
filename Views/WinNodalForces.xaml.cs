using System.Windows;
using PAENN.ViewModels;


namespace PAENN.Views
{
    /// <summary>
    /// Lógica interna para WinNodalForces.xaml
    /// </summary>
    public partial class WinNodalForces : Window
    {
        WinNodalForces_VM VM = new WinNodalForces_VM();
        public WinNodalForces()
        {
            DataContext = VM;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var apply = VM.Apply();

            if (apply == -1)
                MessageBox.Show("Insira valores válidos.", "Erro");
            else
                VarHolder.ClickType = "NodalForces";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VarHolder.ClickType = "Select";
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
