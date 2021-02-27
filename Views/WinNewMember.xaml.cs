using System.Windows;


namespace PAENN.Views
{
    /// <summary>
    /// Lógica interna para WinNewMember.xaml
    /// </summary>
    public partial class WinNewMember : Window
    {
        public ViewModels.WinNewMember_VM VM = new ViewModels.WinNewMember_VM();
        public WinNewMember()
        {
            DataContext = VM;
            ViewModels.VarHolder.ClickType = "NewMember_Start";
            InitializeComponent();
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            int apply = VM.Apply();
            switch (apply)
            {
                case -1:
                    MessageBox.Show("Insira coordenadas válidas.", "Erro");
                    break;
                case -2:
                    MessageBox.Show("Selecione um material válido.", "Erro");
                    break;
                case -3:
                    MessageBox.Show("Selecione uma seção válida.", "Erro");
                    break;
                case -4:
                    MessageBox.Show("Já existe uma barra na posição fornecida.", "Erro" );
                    break;
                case -5:
                    MessageBox.Show("Os pontos inicial e final devem ser diferentes.", "Erro");
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModels.VarHolder.ClickType = "Select";
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
