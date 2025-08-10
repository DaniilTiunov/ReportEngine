using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.Frame;
using System.Globalization;
using System.Windows.Data;

namespace ReportEngine.App.Convert
{
    public class FrameDetailCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3 || values[0] == null || values[1] == null)
                return 0;

            // Получаем Id компонента
            int id = (int)values[0];

            // Получаем модель
            FormedFrameModel model = values[1] as FormedFrameModel;
            if (model == null)
                return 0;

            // Определяем тип компонента
            var component = values[2];

            // Выбираем словарь в зависимости от типа компонента
            if (component is FrameDetail)
            {
                return model.FrameDetailCounts.TryGetValue(id, out int count) ? count : 1;
            }
            else if (component is FrameRoll)
            {
                return model.FrameRollCounts.TryGetValue(id, out int count) ? count : 1;
            }
            else if (component is PillarEqiup)
            {
                return model.PillarEqiupCounts.TryGetValue(id, out int count) ? count : 1;
            }

            return 1; // По умолчанию 1 комплектующая
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}