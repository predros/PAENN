using System.Windows;
using System.Windows.Controls;
using PAENN.ViewModels;

namespace PAENN.Views
{
    /// <summary>
    /// "Manage materials" window internal logic.
    /// </summary>
    public partial class WinMaterials : Window
    {
        private WinMaterials_VM VM = new WinMaterials_VM();


        /// <summary>
        /// WinMaterials class constructor.
        /// </summary>
        public WinMaterials()
        {
            InitializeComponent();
            
            DataContext = VM;
            List_Materials.ItemsSource = ViewModels.VarHolder.MaterialsList;
        }



        /// <summary>
        /// Handles the change in the materials list selection.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void List_Materials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = List_Materials.SelectedItem;
            if (item != null)
            {
                int n = List_Materials.Items.IndexOf(item);
                VM.SelectionChanged(n);
            }
        }

        /// <summary>
        /// Handles the Delete button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            var item = List_Materials.SelectedItem;

            if (item == null)
                MessageBox.Show("Selecione um material para apagar.", "Erro");
            else
            {
                var Warning = MessageBox.Show("Tem certeza que deseja apagar o material selecionado?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (Warning == MessageBoxResult.Yes)
                {
                    var n = List_Materials.Items.IndexOf(item);
                    VM.DeleteMaterial(n);
                }
            }
        }

        /// <summary>
        /// Handles the Rename button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Rename_Click(object sender, RoutedEventArgs e)
        {
            var item = List_Materials.SelectedItem;

            if (item == null)
                MessageBox.Show("Selecione um material para renomear.", "Erro");

            else
            {
                string renamewin = PromptRename.Rename();

                if (renamewin.Trim() != "")
                {
                    int n = List_Materials.Items.IndexOf(item);

                    VM.RenameMaterial(n, renamewin);
                }
            }
        }

        /// <summary>
        /// Handles the Add/Edit button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            var warning = new MessageBoxResult();
            var apply = 0;

            int n = VM.CheckIfExists();

            if (n < 0)
                apply = VM.AddMaterial();

            else
            {
                warning = MessageBox.Show("Tem certeza que deseja alterar o material selecionado?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (warning == MessageBoxResult.Yes)
                {
                    apply = VM.EditMaterial(n);
                }
                else
                    apply = 0;
            }
            

            switch (apply)
            {
                case 0:
                    break;
                case -1:
                    MessageBox.Show("Insira um nome válido", "Erro");
                    break;
                case -2:
                    MessageBox.Show("Insira parâmetros positivos", "Erro");
                    break;
                case -3:
                    MessageBox.Show("Insira parâmetros válidos", "Erro");
                    break;
            }
        }

        /// <summary>
        /// Handles the Clear button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            VM.Entry_Name = "";
            VM.Entry_Elasticity = "";
            VM.Entry_Transversal = "";
            VM.Entry_Thermal = "";
        }

        /// <summary>
        /// Handles the Apply to All button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_ApplyAll_Click(object sender, RoutedEventArgs e)
        {
            var item = List_Materials.SelectedItem;

            if (item == null)
                MessageBox.Show("Selecione um material para aplicar.", "Erro");
            else
            {
                int n = List_Materials.Items.IndexOf(item);
                VM.ApplyAll(n);
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
    }
}
