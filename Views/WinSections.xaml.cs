using System.Windows;
using System.Windows.Controls;
using PAENN.ViewModels;

namespace PAENN.Views
{
    /// <summary>
    /// "Manage sections" window internal logic.
    /// </summary>
    public partial class WinSections : Window
    {
        public WinSections_VM VM = new WinSections_VM();


        /// <summary>
        /// WinSections class constructor.
        /// </summary>
        public WinSections()
        {
            InitializeComponent();
            DataContext = VM;
            List_Sections.ItemsSource = VarHolder.SectionsList;
        }



        /// <summary>
        /// Handles the sections list selection changes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void List_Sections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = List_Sections.SelectedItem;

            if (item == null) return;

            var n = List_Sections.Items.IndexOf(item);
            VM.SelectionChanged(n);
        }

        /// <summary>
        /// Handles the Delete button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            var item = List_Sections.SelectedItem;

            if (item == null)
            {
                MessageBox.Show("Selecione uma seção para apagar.", "Erro");
                return;
            }

            var Warning = MessageBox.Show("Tem certeza que deseja apagar a seção selecionada?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (Warning == MessageBoxResult.No) return;

            var n = List_Sections.Items.IndexOf(item);
            VM.DeleteSection(n);

        }

        /// <summary>
        /// Handles the Rename button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Rename_Click(object sender, RoutedEventArgs e)
        {
            var item = List_Sections.SelectedItem;

            if (item == null)
            {
                MessageBox.Show("Selecione uma seção para renomear.", "Erro");
                return;
            }
            string renamewin = PromptRename.Rename();
            if (renamewin.Trim() != "")
            {
                int n = List_Sections.Items.IndexOf(item);
                VM.RenameSection(n, renamewin);
            }
        }

        /// <summary>
        /// Handles the Radiobutton check/uncheck event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void RB_Checked(object sender, RoutedEventArgs e)
        {
            string sectype;
            if (VM.RB_Generic)
                sectype = "Genérico";

            else if (VM.RB_Circular)
                sectype = "Circular";

            else
                sectype = "Retangular";

            if (IsLoaded)
                VM.TypeChanged(sectype);
        }

        /// <summary>
        /// Handles the Add/Edit button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            string name = VM.Entry_Name;
            var warning = new MessageBoxResult();
 
            var exists = 0;
            var apply = 0;
            var n = -1;

            foreach (Section section in VarHolder.SectionsList)
            {
                if (section.Name == name)
                {
                    n = VarHolder.SectionsList.IndexOf(section);
                    exists = 1;
                    break;
                }
            }

            if (exists == 0)
                apply = VM.AddSection();

            else
            {
                warning = MessageBox.Show("Tem certeza que deseja alterar a seção selecionada?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (warning == MessageBoxResult.Yes)
                    apply = VM.EditSection(n);
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
            VM.ClearText();
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
