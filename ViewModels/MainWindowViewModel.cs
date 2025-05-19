using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using PW_Lab3.Models;
using Avalonia.Controls;
using PW_Lab3.Views;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace PW_Lab3.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public ObservableCollection<Pracownik> Pracownicy { get; } = new();

    public ReactiveCommand<Unit, Unit> DodajCommand { get; }
    public ReactiveCommand<Unit, Unit> UsunCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportujCommand { get; }
    public ReactiveCommand<Unit, Unit> EksportujCommand { get; }

    private Pracownik? _wybranyPracownik;
    public Pracownik? WybranyPracownik
    {
        get => _wybranyPracownik;
        set => this.RaiseAndSetIfChanged(ref _wybranyPracownik, value);
    }

    [Obsolete("Obsolete")]
    public MainWindowViewModel()
    {
        DodajCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var addWindow = new AddEmployeeWindow();
            var mainWindow = GetMainWindow();
            if (mainWindow != null)
            {
                var result = await addWindow.ShowDialog<Pracownik?>(mainWindow);
                if (result != null)
                {
                    Pracownicy.Add(result);
                }
            }
        });

        UsunCommand = ReactiveCommand.Create(() =>
        {
            if (WybranyPracownik != null)
            {
                Pracownicy.Remove(WybranyPracownik);
            }
        });

        ImportujCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null) return;

            var dialog = new OpenFileDialog
            {
                Filters = { new FileDialogFilter { Name = "JSON", Extensions = { "json" } } }, 
                AllowMultiple = false
            };

            var result = await dialog.ShowAsync(mainWindow);
            if (result is { Length: > 0 })
            {
                var json = File.ReadAllText(result[0]);
                var pracownicy = JsonSerializer.Deserialize<ObservableCollection<Pracownik>>(json);
            
                if (pracownicy != null)
                {
                    Pracownicy.Clear();
                    foreach (var pracownik in pracownicy)
                    {
                        Pracownicy.Add(pracownik);
                    }
                }
            }
        });

        EksportujCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null) return;

            var dialog = new SaveFileDialog
            {
                Filters = { new FileDialogFilter { Name = "JSON", Extensions = { "json" } } }, 
                DefaultExtension = "json",
                InitialFileName = "pracownicy.json"
            };

            var result = await dialog.ShowAsync(mainWindow);
            if (!string.IsNullOrEmpty(result))
            {
                var options = new JsonSerializerOptions 
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var json = JsonSerializer.Serialize(Pracownicy, options);
                File.WriteAllText(result, json);
            }
        });
    }

    private static Window? GetMainWindow()
    {
        return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    }
}