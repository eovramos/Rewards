using System.Windows;

namespace Rewards;

public partial class AddCustomerDialog : Window
{
    public string CustomerName { get; private set; } = "";
    public string CustomerPhone { get; private set; } = "";

    public AddCustomerDialog()
    {
        InitializeComponent();
        Loaded += (_, _) => NameTextBox.Focus();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("Please enter a customer name.", "Name Required",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            NameTextBox.Focus();
            return;
        }
        CustomerName = NameTextBox.Text.Trim();
        CustomerPhone = PhoneTextBox.Text.Trim();
        DialogResult = true;
    }
}
