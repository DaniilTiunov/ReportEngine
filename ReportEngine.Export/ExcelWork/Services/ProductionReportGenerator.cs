using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Export.ExcelWork.Services
{
    public class ProductionReportGenerator : IReportGenerator
    {
        ReportType IReportGenerator.Type => ReportType.ProductionReport;

        private readonly IProjectInfoRepository _projectInfoRepository;

        public ProductionReportGenerator(IProjectInfoRepository projectInfoRepository)
        {
            _projectInfoRepository = projectInfoRepository;
        }

        public async Task GenerateAsync(int projectId)
        {

            var project = await _projectInfoRepository.GetByIdAsync(projectId);

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("MainSheet");

                CreateCommonHeader(ws,project);
                int endRow = FillStandsTable(ws, project);
                FillComponentsTable(ws, project, endRow);


                ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Cells().Style.Alignment.WrapText = true;
                ws.Columns().AdjustToContents();

                var savePath = SettingsManager.GetReportDirectory();

                var fileName = "Тара___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".xlsx";
                var fullSavePath = Path.Combine(savePath, fileName);

                Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
                wb.SaveAs(fullSavePath);
            }
        }



   




        //создание общего заголовка
        private void CreateCommonHeader(IXLWorksheet ws, ProjectInfo project)
        {

            var headerRange = ws.Range("A1:D2").Merge();



            ws.Cell("A1").Value = $"{project.Company}";
            ws.Cell("B1").Value = $"{project.OrderCustomer}";
            ws.Cell("C1").Value = $"{project.Description}";

            var projectStringArea = ws.Range("A2:D2").Merge();
            projectStringArea.Value = "Проект целиком";


            ws.Columns().AdjustToContents();
            headerRange.Style.Font.SetBold();


            // headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            //headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);


        }

        private int FillStandsTable(IXLWorksheet ws, ProjectInfo project)
        {

            const string dbErrorString = "Не найдена информация в БД";



            //формируем шапку

            int activeRow = 3;

            ws.Cell($"A{activeRow}").Value = "Наименование";

            var kksCodeStringArea = ws.Range($"B{activeRow}:C{activeRow}").Merge();
            kksCodeStringArea.Value = "Код KKS";

            ws.Cell($"D{activeRow}").Value = "Серийный номер";

            activeRow++;



            //выводим стенды

            var standsRecords = project.Stands.
                Select(stand => new
                {
                    standName = stand.Design,
                    standKKS = stand.KKSCode,
                    standSN = stand.SerialNumber
                });


            foreach (var standRecord in standsRecords)
            {
                ws.Cell($"A{activeRow}").Value = standRecord.standName ?? dbErrorString;

                var standKksArea = ws.Range($"B{activeRow}:C{activeRow}").Merge();
                standKksArea.Value = standRecord.standKKS ?? dbErrorString;

                ws.Cell($"D{activeRow}").Value = standRecord.standSN ?? dbErrorString;

                activeRow++;
            }




            //формируем общее кол-во стендов

            var standsQuantityStringArea = ws.Range($"A{activeRow}:C{activeRow}").Merge();
            standsQuantityStringArea.Value = "Количество стендов по отчету";

            ws.Cell($"D{activeRow}").Value = standsRecords.Count();



            var floorTableArea = ws.Range($"A{activeRow}:D{activeRow}").Merge();

            floorTableArea.Style.Font.SetBold();
    
            activeRow++;




            return activeRow;

        }



    
        //создание сводной ведомости
        private void FillComponentsTable(IXLWorksheet ws, ProjectInfo project, int startRow)
        {
            const string dbErrorString = "Ошибка получения данных из БД";

            int activeRow = startRow;














            //Формирование списка труб

            var pipesList = project.Stands
                .SelectMany(stand => stand.ObvyazkiInStand)
                .Select(obv => new
                {
                    name = obv.MaterialLine,
                    units = obv.MaterialLineMeasure,
                    length = obv.MaterialLineCount
                })
                .GroupBy(pipe => pipe.name)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().units ?? dbErrorString,
                    quantity: group.Sum(pipe => pipe.length).ToString() ?? dbErrorString
                ))
                .ToList();

            //Формирование списка арматуры

            var armaturesList = project.Stands
                .SelectMany(stand => stand.ObvyazkiInStand)
                .Select(obv => new
                {
                    name = obv.Armature,
                    units = obv.ArmatureMeasure,
                    quantity = obv.ArmatureCount
                })
                .GroupBy(arm => arm.name)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().units ?? dbErrorString,
                    quantity: group.Sum(arm => arm.quantity).ToString() ?? dbErrorString
                ))
                .ToList();



            //Формирование списка тройников и КМЧ

            var treeList = project.Stands
                .SelectMany(stand => stand.ObvyazkiInStand)
                .Select(obv => new
                {
                    name = obv.TreeSocket,
                    units = obv.TreeSocketMaterialMeasure,
                    quantity = obv.TreeSocketCount
                })
                .GroupBy(item => item.name)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().units ?? dbErrorString,
                    quantity: group.Sum(item => item.quantity).ToString() ?? dbErrorString
                ))
                .ToList();



            var kmchList = project.Stands
                .SelectMany(stand => stand.ObvyazkiInStand)
                .Select(obv => new
                {
                    name = obv.KMCH,
                    units = obv.KMCHMeasure,
                    quantity = obv.KMCHCount
                })
                .GroupBy(item => item.name)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().units ?? dbErrorString,
                    quantity: group.Sum(item => item.quantity).ToString() ?? dbErrorString
                ))
                .ToList();


            //формирование дренажа

            var drainageParts = project.Stands
                .SelectMany(stand => stand.StandDrainages)
                .SelectMany(drainage => drainage.Drainage.Purposes)
                .GroupBy(purpose => purpose.Material)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().Measure ?? dbErrorString,
                    quantity: group.Sum(groupElement => groupElement.Quantity).ToString() ?? dbErrorString
                ))
                .ToList();


            //Формирование списка рамных комплектующих

            var framesList = project.Stands
                .SelectMany(stand => stand.StandFrames)
                .SelectMany(fr => fr.Frame.Components)
                .Select(comp => new
                {
                    name = comp.ComponentType,
                    unit = comp.Measure,
                    quantity = comp.Count
                })
                .GroupBy(frameComp => frameComp.name)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().unit ?? dbErrorString,
                    quantity: group.Sum(frameComp => frameComp.quantity).ToString() ?? dbErrorString
                ))
                .ToList();




            //сомнительная хрень, хз что брать за источник информации
            //формирование списка кронштейнов

            var sensorsHolders = project.Stands
                .SelectMany(stand => stand.StandAdditionalEquips)
                .SelectMany(equip => equip.AdditionalEquip.Purposes)
                .Where(purpose => purpose.Purpose.Contains("Кронштейн"))
                .GroupBy(purpose => purpose.Material)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().Measure ?? dbErrorString,
                    quantity: group.Sum(groupElement => groupElement.Quantity).ToString() ?? dbErrorString
                ))
                .ToList();


            //формирование списка электрических комплектующих

            var electricalParts = project.Stands
                .SelectMany(stand => stand.StandElectricalComponent)
                .SelectMany(equip => equip.ElectricalComponent.Purposes)
                .GroupBy(purpose => purpose.Material)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().Measure ?? dbErrorString,
                    quantity: group.Sum(item => item.Quantity).ToString() ?? dbErrorString
                ))
                .ToList();



            //формирование списка дополнительного оборудования

            var additionalParts = project.Stands
                .SelectMany(stand => stand.StandAdditionalEquips)
                .SelectMany(equip => equip.AdditionalEquip.Purposes)
                .GroupBy(purpose => purpose.Material)
                .Select(group => (
                    name: group.Key ?? dbErrorString,
                    unit: group.First().Measure ?? dbErrorString,
                    quantity: group.Sum(item => item.Quantity).ToString() ?? dbErrorString
                ))
                .Except(sensorsHolders);


            var othersParts = additionalParts
                .Where(part => part.name.Contains("Шильдик") || part.name.Contains("Табличка"))
                .ToList();

            var supplies = additionalParts
                .Except(othersParts)
                .ToList();


            var activeRow = 4;



            activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
            activeRow = FillSubtableData(activeRow, pipesList, ws);

            activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
            activeRow = FillSubtableData(activeRow, armaturesList, ws);

            activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
            activeRow = FillSubtableData(activeRow, treeList, ws);
            activeRow = FillSubtableData(activeRow, kmchList, ws);

            activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
            activeRow = FillSubtableData(activeRow, drainageParts, ws);

            activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
            activeRow = FillSubtableData(activeRow, framesList, ws);

            activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
            activeRow = FillSubtableData(activeRow, sensorsHolders, ws);

            activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
            activeRow = FillSubtableData(activeRow, electricalParts, ws);

            activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
            activeRow = FillSubtableData(activeRow, othersParts, ws);

            activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
            activeRow = FillSubtableData(activeRow, supplies, ws);


        }

   


        #region Вспомогательные 

        //создает подзаголовок для подтаблицы и возвращает следующую строку
        private int CreateSubheaderOnWorksheet(int row, string title, IXLWorksheet ws)
        {
            var subHeaderRange = ws.Range($"B{row}:D{row}");
            subHeaderRange.Merge();
            subHeaderRange.Value = title;
            subHeaderRange.Style.Font.SetFontSize(10);
            subHeaderRange.Style.Font.SetBold();

            row++;
            return row;
        }

        //Заполняет подтаблицу и возвращает следующую строку
        private int FillSubtableData(int startRow, List<(string name, string unit, string quantity)> items, IXLWorksheet ws)
        {
            var currentRow = startRow;
            foreach (var item in items)
            {
                ws.Cell($"B{currentRow}").Value = item.name;
                ws.Cell($"C{currentRow}").Value = item.unit;
                ws.Cell($"D{currentRow}").Value = item.quantity;
                currentRow++;
            }

            return currentRow;
        }

        #endregion


    }
}

