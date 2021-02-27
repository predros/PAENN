using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PAENN.ViewModels;

namespace PAENN.Views
{
    /// <summary>
    /// Lógica interna para WinSupports.xaml
    /// </summary>
    public partial class WinSupports : Window
    {
        WinSupports_VM VM = new WinSupports_VM();
        public WinSupports()
        {
            InitializeComponent();
            DataContext = VM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var apply = VM.SupportApply();

            if (apply == -1)
            {
                MessageBox.Show("As constantes de mola devem ser não-negativas.", "Erro");
                return;
            }

            VarHolder.ClickType = "Support";
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

    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }


        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
