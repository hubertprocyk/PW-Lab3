using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using PW_Lab3.Models;
using Avalonia.Controls;
using PW_Lab3.Views;
using System.IO;
using System.Xml.Serialization;
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
                Filters = { new FileDialogFilter { Name = "XML", Extensions = { "xml" } } },
                AllowMultiple = false
            };

            var result = await dialog.ShowAsync(mainWindow);
            if (result is { Length: > 0 })
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<Pracownik>));
                await using var stream = new FileStream(result[0], FileMode.Open);
                if (serializer.Deserialize(stream) is ObservableCollection<Pracownik> nowiPracownicy)
                {
                    Pracownicy.Clear();
                    foreach (var pracownik in nowiPracownicy)
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
                Filters = { new FileDialogFilter { Name = "XML", Extensions = { "xml" } } },
                DefaultExtension = "xml",
                InitialFileName = "pracownicy.xml"
            };

            var result = await dialog.ShowAsync(mainWindow);
            if (!string.IsNullOrEmpty(result))
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<Pracownik>));
                await using var stream = new FileStream(result, FileMode.Create);
                serializer.Serialize(stream, Pracownicy);
            }
        });
    }

    private static Window? GetMainWindow()
    {
        return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    }
}