using System.Windows;
using PAENN.ViewModels;


namespace PAENN.Views
{
    /// <summary>
    /// "Add new member" window internal logic
    /// </summary>
    public partial class WinNewMember : Window
    {

        public WinNewMember_VM VM = new WinNewMember_VM();


        /// <summary>
        /// WinNewMember class constructor.
        /// </summary>
        public WinNewMember()
        {
            InitializeComponent();

            DataContext = VM;
            VarHolder.ClickType = "NewMember_Start";
        }



        /// <summary>
        /// Handles the Apply button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
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
        /// Handles the Close button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
