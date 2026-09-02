using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace ReportEngine.App.Converters;

public class HeaderToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string header = value?.ToString() ?? string.Empty;
            
        return header switch
        {
            "Проект" => PackIconKind.Folder,
            "Карточка проекта" => PackIconKind.CardOutline,
            "Редактирование стенда" => PackIconKind.Pencil,
            "Расчёт стенда" => PackIconKind.Calculator,
            "Формирование упаковки" => PackIconKind.PackageVariant,
            "Предпросмотр документов" => PackIconKind.FileDocumentOutline,
            "Расчёт проекта" => PackIconKind.Calculator,
            "Рассчитать проект" => PackIconKind.Play,
            "Пересчёт" => PackIconKind.Refresh,
            "Отчёты" => PackIconKind.FileMultiple,
            "Сводная ведомость" => PackIconKind.FileDocument,
            "Ведомость комплектующих" => PackIconKind.FormatListBulleted,
            "Шильдики и таблички" => PackIconKind.Tag,
            "Маркировка" => PackIconKind.Label,
            "Производство" => PackIconKind.Factory,
            "Фин. план" => PackIconKind.CurrencyUsd,
            "Отчёт тара" => PackIconKind.PackageVariant,
            "Паспорт" => PackIconKind.FileAccount,
            "Технологические карты" => PackIconKind.Card,
            "1C" => PackIconKind.Database,
            "Сводная ведомость (1C)" => PackIconKind.FileDocument,
            _ => PackIconKind.FolderOutline
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}