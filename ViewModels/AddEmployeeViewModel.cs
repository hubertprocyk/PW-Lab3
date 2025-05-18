using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using PW_Lab3.Models;

namespace PW_Lab3.ViewModels;

public class AddEmployeeViewModel : ReactiveObject
{
    public ObservableCollection<string> DostepneStanowiska { get; }

    private string _imie;
    public string Imie
    {
        get => _imie;
        set => this.RaiseAndSetIfChanged(ref _imie, value);
    }

    private string _nazwisko;
    public string Nazwisko
    {
        get => _nazwisko;
        set => this.RaiseAndSetIfChanged(ref _nazwisko, value);
    }

    private int _wiek;
    public int Wiek
    {
        get => _wiek;
        set => this.RaiseAndSetIfChanged(ref _wiek, value);
    }

    private string _stanowisko;
    public string Stanowisko
    {
        get => _stanowisko;
        set => this.RaiseAndSetIfChanged(ref _stanowisko, value);
    }

    public ReactiveCommand<Unit, Pracownik> ZatwierdzCommand { get; }
    public ReactiveCommand<Unit, Unit> AnulujCommand { get; }

    private AddEmployeeViewModel(string imie, string nazwisko, string stanowisko)
    {
        _imie = imie;
        _nazwisko = nazwisko;
        _stanowisko = stanowisko;
        DostepneStanowiska = [
            "Programista",
            "Tester",
            "Menadżer",
            "HR"
        ];

        ZatwierdzCommand = ReactiveCommand.Create(() => new Pracownik
        {
            Imie = Imie,
            Nazwisko = Nazwisko,
            Wiek = Wiek,
            Stanowisko = Stanowisko
        });

        AnulujCommand = ReactiveCommand.Create(() => { });
    }

    public AddEmployeeViewModel() : this(string.Empty, string.Empty, string.Empty)
    {
    }
}