using System.Windows;


namespace PAENN.Views
{
    /// <summary>
    /// Lógica interna para PromptRename.xaml
    /// </summary>
    public partial class PromptRename : Window
    {
        public bool Result = false;
        static public string Text;


        public PromptRename()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PromptLoaded);
        }

        public static string Rename()
        {
            var win = new PromptRename();
            win.ShowDialog();

            if (win.Result == true)
                return win.TextBox_Name.Text;
            else
                return "";
        }
        
        private void PromptLoaded(object sender, RoutedEventArgs e)
        {
            TextBox_Name.Focus();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }
    }
}
