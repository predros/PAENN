using System.Windows;
using System.Windows.Controls;

namespace PAENN.Views
{
    /// <summary>
    /// Lógica interna para WinMaterials.xaml
    /// </summary>
    public partial class WinMaterials : Window
    {
        private ViewModels.WinMaterials_VM VM = new ViewModels.WinMaterials_VM();
        public WinMaterials()
        {
            InitializeComponent();
            
            this.DataContext = VM;
            List_Materials.ItemsSource = ViewModels.VarHolder.MaterialsList;
        }

        private void List_Materials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = List_Materials.SelectedItem;
            if (item != null)
            {
                int n = List_Materials.Items.IndexOf(item);
                VM.SelectionChanged(n);
            }
        }

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

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Name.Text = "";
            TextBox_Elasticity.Text = "";
            TextBox_Thermal.Text = "";
        }

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModels.VarHolder.ClickType = "Select";
        }
    }
}
