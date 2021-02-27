using System.ComponentModel;
using System.Windows;


namespace PAENN.Views
{
    /// <summary>
    /// Lógica interna para WinNewNode.xaml
    /// </summary>
    public partial class WinNewNode : Window
    {
        private ViewModels.WinNewNode_VM VM = new ViewModels.WinNewNode_VM();
        public WinNewNode()
        {
            InitializeComponent();
            ViewModels.VarHolder.ClickType = "NewNode";
            this.DataContext = VM;
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            int apply = VM.Apply();
            switch (apply)
            {
                case -1:
                    MessageBox.Show("Já existe um ponto nas coordenadas fornecidas.", "Erro");
                    break;
                case -2:
                    MessageBox.Show("Insira coordenadas válidas.", "Erro");
                    break;
                default:
                    var mainwindow = (MainWindow)Application.Current.Windows[0];
                    if (ViewModels.VarHolder.GridNormal == "None")
                        mainwindow.View_Redraw();
                    else
                        mainwindow.Canvas_Redraw();
                    break;
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ViewModels.VarHolder.ClickType = "Select";
        }
    }
}
