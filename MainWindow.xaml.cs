using Rewards.Models;
using Rewards.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rewards;

public partial class MainWindow : Window
{
    private List<Customer> _customers = new();
    private Customer? _selectedCustomer;

    public MainWindow()
    {
        InitializeComponent();
        _customers = DataService.Load();
        CustomerListBox.ItemsSource = _customers;
    }

    // ── Customer list ──────────────────────────────────────────────────

    private void CustomerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedCustomer = CustomerListBox.SelectedItem as Customer;
        bool hasSelection = _selectedCustomer != null;
        DetailPanel.IsEnabled = hasSelection;
        RemoveCustomerButton.IsEnabled = hasSelection;
        EditCustomerButton.IsEnabled = hasSelection;

        if (hasSelection)
            RefreshDetail(_selectedCustomer!);
        else
            ClearDetail();
    }

    private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddCustomerDialog { Owner = this };
        if (dialog.ShowDialog() != true) return;

        var customer = new Customer
        {
            Name = dialog.CustomerName,
            Phone = dialog.CustomerPhone
        };
        _customers.Add(customer);
        CustomerListBox.Items.Refresh();
        DataService.Save(_customers);
        CustomerListBox.SelectedItem = customer;
    }

    private void EditCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCustomer == null) return;

        var dialog = new EditCustomerDialog(_selectedCustomer.Name, _selectedCustomer.Phone) { Owner = this };
        if (dialog.ShowDialog() != true) return;

        _selectedCustomer.Name = dialog.CustomerName;
        _selectedCustomer.Phone = dialog.CustomerPhone;
        CustomerListBox.Items.Refresh();
        DataService.Save(_customers);
        RefreshDetail(_selectedCustomer);
    }

    private void RemoveCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCustomer == null) return;

        var result = MessageBox.Show(
            $"Remove \"{_selectedCustomer.Name}\" and all their purchases?",
            "Remove Customer", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        _customers.Remove(_selectedCustomer);
        CustomerListBox.Items.Refresh();
        DataService.Save(_customers);

        _selectedCustomer = null;
        DetailPanel.IsEnabled = false;
        RemoveCustomerButton.IsEnabled = false;
        EditCustomerButton.IsEnabled = false;
        ClearDetail();
    }

    // ── Purchases ──────────────────────────────────────────────────────

    private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCustomer == null) return;
        _selectedCustomer.Purchases.Add(new Purchase());
        DataService.Save(_customers);
        CustomerListBox.Items.Refresh();
        RefreshDetail(_selectedCustomer);
    }

    private void RemovePurchaseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCustomer == null) return;
        if (PurchaseListBox.SelectedItem is not PurchaseDisplayItem item) return;

        var result = MessageBox.Show(
            "Remove this purchase? This will update the reward count.",
            "Remove Purchase", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        _selectedCustomer.Purchases.Remove(item.Purchase);
        DataService.Save(_customers);
        CustomerListBox.Items.Refresh();
        RefreshDetail(_selectedCustomer);
    }

    private void PurchaseListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RemovePurchaseButton.IsEnabled = PurchaseListBox.SelectedItem != null;
    }

    private void ClaimRewardButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCustomer == null || _selectedCustomer.RewardsAvailable <= 0) return;

        var result = MessageBox.Show(
            $"Redeem 1 free dozen for {_selectedCustomer.Name}?",
            "Claim Reward", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        _selectedCustomer.Purchases.Add(new Purchase { IsFreeRedemption = true });
        DataService.Save(_customers);
        CustomerListBox.Items.Refresh();
        RefreshDetail(_selectedCustomer);
    }

    // ── Detail panel ───────────────────────────────────────────────────

    private void RefreshDetail(Customer c)
    {
        CustomerNameLabel.Text = c.Name;
        CustomerPhoneLabel.Text = string.IsNullOrWhiteSpace(c.Phone) ? "" : $"Phone: {c.Phone}";

        RefreshStampCard(c);

        RewardsEarnedLabel.Text    = c.RewardsEarned.ToString();
        RewardsClaimedLabel.Text   = c.RewardsClaimed.ToString();
        RewardsAvailableLabel.Text = c.RewardsAvailable.ToString();
        ClaimRewardButton.IsEnabled = c.RewardsAvailable > 0;

        PurchaseListBox.ItemsSource = null;
        PurchaseListBox.ItemsSource = c.Purchases
            .OrderByDescending(p => p.Date)
            .Select(p => new PurchaseDisplayItem(p))
            .ToList();

        RemovePurchaseButton.IsEnabled = false;
    }

    private void RefreshStampCard(Customer c)
    {
        StampCard.Items.Clear();
        int filled = c.StampProgress;

        for (int i = 0; i < 10; i++)
        {
            bool on = i < filled;
            var border = new Border
            {
                Width = 44,
                Height = 44,
                Margin = new Thickness(3),
                CornerRadius = new CornerRadius(22),
                Background = on
                    ? new SolidColorBrush(Color.FromRgb(255, 193, 7))
                    : new SolidColorBrush(Color.FromRgb(225, 225, 225)),
                BorderBrush = on
                    ? new SolidColorBrush(Color.FromRgb(180, 130, 0))
                    : new SolidColorBrush(Color.FromRgb(190, 190, 190)),
                BorderThickness = new Thickness(2)
            };

            border.Child = new TextBlock
            {
                Text = on ? "🥚" : "○",
                FontSize = on ? 20 : 16,
                Foreground = on
                    ? Brushes.White
                    : new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            StampCard.Items.Add(border);
        }

        int remaining = 10 - filled;
        StampProgressLabel.Text = $"{filled} / 10 stamps";
        StampHintLabel.Text = filled == 0
            ? "Buy 10 dozens to earn a free one!"
            : remaining == 0
                ? "Card complete — a free dozen is ready to claim!"
                : $"{remaining} more dozen{(remaining == 1 ? "" : "s")} until the next free reward";
    }

    private void ClearDetail()
    {
        CustomerNameLabel.Text = "";
        CustomerPhoneLabel.Text = "";
        StampCard.Items.Clear();
        StampProgressLabel.Text = "";
        StampHintLabel.Text = "";
        RewardsEarnedLabel.Text = "";
        RewardsClaimedLabel.Text = "";
        RewardsAvailableLabel.Text = "";
        ClaimRewardButton.IsEnabled = false;
        PurchaseListBox.ItemsSource = null;
    }
}

public record PurchaseDisplayItem(Purchase Purchase)
{
    public override string ToString() =>
        Purchase.IsFreeRedemption
            ? $"{Purchase.Date:MMM dd, yyyy}  —  FREE dozen redeemed"
            : $"{Purchase.Date:MMM dd, yyyy}  —  1 dozen purchased";
}
