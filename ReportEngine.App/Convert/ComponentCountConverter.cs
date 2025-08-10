using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Shared.Config.DebugConsol;

namespace ReportEngine.App.Convert
{
    public class ComponentCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Детальная отладка
            DebugConsole.WriteLine("ComponentCountConverter вызван");
            DebugConsole.WriteLine($"Количество параметров: {values.Length}");
            
            if (values.Length < 3 || values[0] == null || values[1] == null || values[2] == null)
            {
                DebugConsole.WriteLine("Недостаточно параметров или null параметры");
                return 0;
            }

            DebugConsole.WriteLine($"Параметр 1 (Id): {values[0]} ({values[0]?.GetType().Name})");
            DebugConsole.WriteLine($"Параметр 2 (Model): {values[1]?.GetType().Name}");
            DebugConsole.WriteLine($"Параметр 3 (Component): {values[2]?.GetType().Name}");

            int id;
            if (values[0] is int intId)
                id = intId;
            else if (int.TryParse(values[0].ToString(), out int parsedId))
                id = parsedId;
            else
            {
                DebugConsole.WriteLine($"Id не является целым числом: {values[0]}");
                return 0;
            }

            var model = values[1] as FormedFrameModel;
            if (model == null)
            {
                DebugConsole.WriteLine("Не удалось получить модель FormedFrameModel");
                return 0;
            }

            if (model.FrameDetailCounts == null)
            {
                DebugConsole.WriteLine("FrameDetailCounts равен null");
                return 0;
            }

            var component = values[2];
            int count = 0;

            // Проверяем тип компонента и берем соответствующий словарь
            if (component is FrameDetail)
            {
                bool exists = model.FrameDetailCounts.TryGetValue(id, out count);
                DebugConsole.WriteLine($"FrameDetail (Id: {id}), Найден в словаре: {exists}, Количество: {count}");
            }
            else if (component is FrameRoll)
            {
                bool exists = model.FrameRollCounts.TryGetValue(id, out count);
                DebugConsole.WriteLine($"FrameRoll (Id: {id}), Найден в словаре: {exists}, Количество: {count}");
            }
            else if (component is PillarEqiup)
            {
                bool exists = model.PillarEqiupCounts.TryGetValue(id, out count);
                DebugConsole.WriteLine($"PillarEqiup (Id: {id}), Найден в словаре: {exists}, Количество: {count}");
            }
            else
            {
                DebugConsole.WriteLine($"Неизвестный тип компонента: {component?.GetType().Name}");
            }

            // Возвращаем количество, минимум 1
            return count > 0 ? count : 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}