using BudgetPlanner.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BudgetPlanner
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
        }

        public void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Wait for the edit to complete before saving
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var editedTransaction = e.Row.Item as Models.DatabaseTransaction;
                if(editedTransaction != null)
                {
                    viewModel.OnTransactionEdited(editedTransaction);
                }
            }
        }

        //Marks whole amount when editing
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }
    }
}