using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using PW_Lab3.Models;
using Avalonia.Controls;
using PW_Lab3.Views;
using System.IO;
using System.Linq;
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
                Filters = { new FileDialogFilter { Name = "CSV", Extensions = { "csv" } } },
                AllowMultiple = false
            };

            var result = await dialog.ShowAsync(mainWindow);
            if (result is { Length: > 0 })
            {
                var lines = File.ReadAllLines(result[0]);
                foreach (var line in lines)
                {
                    var dane = line.Split(',');
                    if (dane.Length != 4) continue;

                    Pracownicy.Add(new Pracownik
                    {
                        Imie = dane[0],
                        Nazwisko = dane[1],
                        Wiek = int.TryParse(dane[2], out var wiek) ? wiek : 0,
                        Stanowisko = dane[3]
                    });
                }
            }
        });

        EksportujCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null) return;

            var dialog = new SaveFileDialog
            {
                Filters = { new FileDialogFilter { Name = "CSV", Extensions = { "csv" } } },
                DefaultExtension = "csv",
                InitialFileName = "pracownicy.csv"
            };

            var result = await dialog.ShowAsync(mainWindow);
            if (!string.IsNullOrEmpty(result))
            {
                var lines = Pracownicy.Select(p => $"{p.Imie},{p.Nazwisko},{p.Wiek},{p.Stanowisko}");
                File.WriteAllLines(result, lines);
            }
        });
    }

    private static Window? GetMainWindow()
    {
        return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    }
}