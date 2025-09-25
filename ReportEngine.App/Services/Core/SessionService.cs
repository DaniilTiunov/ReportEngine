using System.ComponentModel;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Core;

public static class SessionService
{
    private static User? _currentUser;

    public static User? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }
    }

    public static event PropertyChangedEventHandler? PropertyChanged;

    private static void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
}
