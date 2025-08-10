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
            // ��������� �������
            DebugConsole.WriteLine("ComponentCountConverter ������");
            DebugConsole.WriteLine($"���������� ����������: {values.Length}");
            
            if (values.Length < 3 || values[0] == null || values[1] == null || values[2] == null)
            {
                DebugConsole.WriteLine("������������ ���������� ��� null ���������");
                return 0;
            }

            DebugConsole.WriteLine($"�������� 1 (Id): {values[0]} ({values[0]?.GetType().Name})");
            DebugConsole.WriteLine($"�������� 2 (Model): {values[1]?.GetType().Name}");
            DebugConsole.WriteLine($"�������� 3 (Component): {values[2]?.GetType().Name}");

            int id;
            if (values[0] is int intId)
                id = intId;
            else if (int.TryParse(values[0].ToString(), out int parsedId))
                id = parsedId;
            else
            {
                DebugConsole.WriteLine($"Id �� �������� ����� ������: {values[0]}");
                return 0;
            }

            var model = values[1] as FormedFrameModel;
            if (model == null)
            {
                DebugConsole.WriteLine("�� ������� �������� ������ FormedFrameModel");
                return 0;
            }

            if (model.FrameDetailCounts == null)
            {
                DebugConsole.WriteLine("FrameDetailCounts ����� null");
                return 0;
            }

            var component = values[2];
            int count = 0;

            // ��������� ��� ���������� � ����� ��������������� �������
            if (component is FrameDetail)
            {
                bool exists = model.FrameDetailCounts.TryGetValue(id, out count);
                DebugConsole.WriteLine($"FrameDetail (Id: {id}), ������ � �������: {exists}, ����������: {count}");
            }
            else if (component is FrameRoll)
            {
                bool exists = model.FrameRollCounts.TryGetValue(id, out count);
                DebugConsole.WriteLine($"FrameRoll (Id: {id}), ������ � �������: {exists}, ����������: {count}");
            }
            else if (component is PillarEqiup)
            {
                bool exists = model.PillarEqiupCounts.TryGetValue(id, out count);
                DebugConsole.WriteLine($"PillarEqiup (Id: {id}), ������ � �������: {exists}, ����������: {count}");
            }
            else
            {
                DebugConsole.WriteLine($"����������� ��� ����������: {component?.GetType().Name}");
            }

            // ���������� ����������, ������� 1
            return count > 0 ? count : 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}