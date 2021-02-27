using System.Windows;


namespace PAENN.Views
{
    /// <summary>
    /// Lógica interna para WinMemberLoads.xaml
    /// </summary>
    public partial class WinMemberLoads : Window
    {
        ViewModels.WinMemberLoads_VM VM = new ViewModels.WinMemberLoads_VM();
        public WinMemberLoads()
        {
            InitializeComponent();
            DataContext = VM;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModels.VarHolder.ClickType = "Select";
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            var apply = VM.Apply();

            if (apply == -1)
                MessageBox.Show("Insira valores válidos.", "Erro");
            else
                ViewModels.VarHolder.ClickType = "MemberLoads";
        }
    }
}
