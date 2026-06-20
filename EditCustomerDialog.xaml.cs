using System.Windows;

namespace Rewards;

public partial class EditCustomerDialog : Window
{
    public string CustomerName { get; private set; } = "";
    public string CustomerPhone { get; private set; } = "";

    public EditCustomerDialog(string currentName, string currentPhone)
    {
        InitializeComponent();
        NameTextBox.Text = currentName;
        PhoneTextBox.Text = currentPhone;
        Loaded += (_, _) =>
        {
            NameTextBox.Focus();
            NameTextBox.SelectAll();
        };
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
