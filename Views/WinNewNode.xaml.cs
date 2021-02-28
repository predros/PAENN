using System.ComponentModel;
using System.Windows;
using PAENN.ViewModels;

namespace PAENN.Views
{
    /// <summary>
    /// "Add new node" window internal logic.
    /// </summary>
    public partial class WinNewNode : Window
    {
        private WinNewNode_VM VM = new WinNewNode_VM();


        /// <summary>
        /// WinNewNode class constructor.
        /// </summary>
        public WinNewNode()
        {
            InitializeComponent();
            VarHolder.ClickType = "NewNode";
            DataContext = VM;
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
                    MessageBox.Show("Já existe um ponto nas coordenadas fornecidas.", "Erro");
                    break;
                case -2:
                    MessageBox.Show("Insira coordenadas válidas.", "Erro");
                    break;
                default:
                    var mainwindow = (MainWindow)Application.Current.Windows[0];
                    if (VarHolder.GridNormal == "None")
                        mainwindow.View_Redraw();
                    else
                        mainwindow.Canvas_Redraw();
                    break;
            }
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
        /// Handles the Closing event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        public void Window_Closing(object sender, CancelEventArgs e)
        {
            VarHolder.ClickType = "Select";
        }
    }
}
