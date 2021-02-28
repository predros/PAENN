using System;
using System.Windows;
using System.Windows.Data;
using PAENN.ViewModels;

namespace PAENN.Views
{
    /// <summary>
    /// "Add supports" window internal logic.
    /// </summary>
    public partial class WinSupports : Window
    {
        WinSupports_VM VM = new WinSupports_VM();


        /// <summary>
        /// WinSupports class constructor.
        /// </summary>
        public WinSupports()
        {
            InitializeComponent();
            DataContext = VM;
        }

        /// <summary>
        /// Handles the Apply button click.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event.</param>
        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            var apply = VM.SupportApply();

            if (apply == -1)
            {
                MessageBox.Show("As constantes de mola devem ser não-negativas.", "Erro");
                return;
            }

            VarHolder.ClickType = "Support";
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

    /// <summary>
    /// Converter using for inverting boolean values.
    /// </summary>
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
