using System.IO;
using System.Text.Json;
using Rewards.Models;

namespace Rewards.Services;

public static class DataService
{
    private static readonly string _folder =
        Path.Combine(
            Environment.GetEnvironmentVariable("OneDrive")
                ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EggRewards");

    private static readonly string _filePath = Path.Combine(_folder, "data.json");

    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public static List<Customer> Load()
    {
        if (!File.Exists(_filePath))
            return new List<Customer>();

        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Customer>>(json, _options) ?? new List<Customer>();
        }
        catch
        {
            return new List<Customer>();
        }
    }

    public static void Save(List<Customer> customers)
    {
        try
        {
            Directory.CreateDirectory(_folder);
            string json = JsonSerializer.Serialize(customers, _options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Failed to save data: {ex.Message}",
                "Save Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
        }
    }
}
