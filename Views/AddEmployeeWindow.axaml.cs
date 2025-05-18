using System;
using Avalonia.Controls;
using PW_Lab3.ViewModels;

namespace PW_Lab3.Views;

public partial class AddEmployeeWindow : Window {
    public AddEmployeeWindow() {
        InitializeComponent();
        var vm = new AddEmployeeViewModel();
        this.DataContext = vm;

        vm.ZatwierdzCommand.Subscribe(new Action<object>(Close));
        vm.AnulujCommand.Subscribe(_ => Close(null));
    }
}