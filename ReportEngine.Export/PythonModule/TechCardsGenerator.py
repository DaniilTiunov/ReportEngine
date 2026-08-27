from reportlab.lib.pagesizes import A4, landscape,portrait
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import (PageTemplate, SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, NextPageTemplate, Frame)
from reportlab.lib.units import mm
from reportlab.lib.enums import TA_LEFT, TA_CENTER, TA_RIGHT, TA_JUSTIFY
import PdfHelper
from reportlab.lib import colors
import io



impulseTableInfo  = {}

class MySplittableTable(Table):


    def __init__(self, *args, **kwargs):

        # Сохраняем внешние данные
        self.standNumber = kwargs.pop('standNumber', None) 
        super().__init__(*args, **kwargs)



    def onSplit(self, R0):
        
        # Мы можем узнать количество строк в первой части
        rows_on_page = len(R0._cellvalues)            

        if self.standNumber is not None:
           print(f"onSplit called with standNumber: {self.standNumber }" )

           # Если стенда еще нет в словаре - инициализируем данные для него
           if self.standNumber not in impulseTableInfo:
              impulseTableInfo[self.standNumber] = {
                      "isPageSplitted": True,
                      "pagesCount" : 0,
                      "pages": []
                      
              }

           #высчитываем количество страниц на данный момент
           nowPageCount = len(impulseTableInfo[self.standNumber]["pages"])

           # Добавляем новую страницу
           impulseTableInfo[self.standNumber]["pages"].append(
               {
                   "pageNumber" : nowPageCount + 1,
                   "hasHeader"  : True,
                   "rowsCount"  : rows_on_page
                })

           #обновляем количество страниц
           impulseTableInfo[self.standNumber]["pagesCount"] = len(impulseTableInfo[self.standNumber]["pages"])


        # Вызываем родительский метод, если он необходим
        super().onSplit(R0) 






landscapeParams = {
    "startPointX" : 15 * mm,
    "startPointY": 15 * mm,
    "frameWidth":A4[1] - 30*mm,
    "frameHeight": A4[0] - 30*mm,
    "frameId": 'landscapeFrame',
    "visibleBoundaries": 0
}


portraitParams = {
    "startPointX" : 5 * mm,
    "startPointY": 5 * mm,
    "frameWidth":A4[0] - 10*mm,
    "frameHeight": A4[1] - 10*mm,
    "frameId": 'portraitFrame',
    "visibleBoundaries": 0
}

landscapeTemplate = PageTemplate(
        id='landscape', 
        pagesize=landscape(A4),
        frames= Frame(
            x1 = landscapeParams['startPointX'], y1 =  landscapeParams['startPointY'], 
            width = landscapeParams['frameWidth'], height = landscapeParams['frameHeight'],  
            id = landscapeParams['frameId'],
            showBoundary  = landscapeParams['visibleBoundaries']
    ))

portraitTemplate = PageTemplate(
        id = 'portrait', 
        pagesize = portrait(A4),
        frames= Frame(
            x1 = portraitParams['startPointX'], y1 =  portraitParams['startPointY'], 
            width = portraitParams['frameWidth'], height = portraitParams['frameHeight'],  
            id = portraitParams['frameId'],
            showBoundary  = portraitParams['visibleBoundaries']
    ))





















def fillStandPage(stand, project, tableSplittingInfo = None):
    
    #инициализируем стили листа
    styles = getSampleStyleSheet()

    tableContentStyle = ParagraphStyle(
        'TableContent',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 6,
        wordWrap = 'LTR',
        alignment = TA_CENTER,
        leading = 7
    )


    cyrillicStyle = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 6,
        wordWrap = 'LTR'
    )


    testExecution = tableSplittingInfo is None

    #вписываем в рамку
    sheetWidth = portraitParams['frameWidth'] * 0.99
    sheetHeight = portraitParams['frameHeight'] * 0.99

    leftPartWidth = 0.55 * sheetWidth
    rightPartWidth = 0.45 * sheetWidth
    
    



    #общие заголовки таблицы
    galvanizeStr = "Оцинковка" if project["IsGalvanized"] else "Покраска"

    standTechCardHeaderTable = Table(data = [[ "Технологическая карта", PdfHelper.to_str(galvanizeStr), PdfHelper.to_str(project["RequestProduction"]) ]],
                                   colWidths= leftPartWidth/3)
    standTechCardHeaderTable.setStyle(TableStyle(cmds =
                                                 PdfHelper.commonTableStyleCmd +
                                                 PdfHelper.centerAlignTableStyleCmd + 
                                                 PdfHelper.usualFontTableStyleCmd + 
                                                 PdfHelper.visibleOuterBordersTableStyleCmd +
                                                 PdfHelper.invisibleInnerBordersTableStyleCmd +
                                                 #Технологическая карта жирным
                                                 [('FONTNAME', (0, 0), (0, 0), "Arial-Bold")]
                                                 )) 

   
    standNameData = [[ "Стенд датчиков КИПиА " + PdfHelper.to_str(stand["Designation"]) ]]
    standNameHeaderTable = Table(data = standNameData, colWidths = leftPartWidth)
    standNameHeaderTable.setStyle(TableStyle(cmds =
                                             PdfHelper.commonTableStyleCmd +
                                             PdfHelper.centerAlignTableStyleCmd + 
                                             PdfHelper.boldFontTableStyleCmd + 
                                             PdfHelper.visibleAllBordersTableStyleCmd ))  
    

    standsInfoData = [[ PdfHelper.to_str(stand["KKSCode"]) , PdfHelper.to_str(stand["SerialNumber"]) ]]
    standInfoTable = Table(data = standsInfoData, colWidths = leftPartWidth/2)
    standInfoTable.setStyle(TableStyle(cmds =
                                       PdfHelper.commonTableStyleCmd +
                                       PdfHelper.centerAlignTableStyleCmd + 
                                       PdfHelper.boldFontTableStyleCmd + 
                                       PdfHelper.visibleAllBordersTableStyleCmd ))

    standSizeData = [[ "Размер стенда, мм ", str(stand["Width"]) ]]
    standSizeTable = Table(data = standSizeData, colWidths = [leftPartWidth*0.8, leftPartWidth * 0.2])
    standSizeTable.setStyle(TableStyle(cmds =
                                       PdfHelper.commonTableStyleCmd +
                                       PdfHelper.centerAlignTableStyleCmd + 
                                       PdfHelper.boldFontTableStyleCmd + 
                                       PdfHelper.visibleAllBordersTableStyleCmd  ))

    
    #таблица рам
    framesTableHeaderData = [["Рама, мм", "Обозначение по КД", "Кол-во,\n шт"]]
    framesTableData = framesTableHeaderData.copy()

    for frame in stand["Frames"]:
        frameArray = [
            Paragraph(PdfHelper.to_str(frame["Width"]), tableContentStyle), 
            Paragraph(PdfHelper.to_str(frame["DocName"]), tableContentStyle), 
            Paragraph(PdfHelper.to_str(frame["Quantity"]), tableContentStyle)
        ]
        framesTableData.append(frameArray)

    framesTable = Table(data = framesTableData, colWidths = [leftPartWidth*0.15, leftPartWidth*0.75, leftPartWidth*0.1])
    framesTable.setStyle(TableStyle(cmds =
                                    PdfHelper.commonTableStyleCmd +
                                    PdfHelper.centerAlignTableStyleCmd + 
                                    PdfHelper.usualFontTableStyleCmd + 
                                    PdfHelper.visibleAllBordersTableStyleCmd + 
                                    #шапка жирным
                                    [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))


    columnsHeaderTitles = [["Наименование", "Ед. изм.", "Норм.","Факт."]]

    
    #таблица материалов рам
    framePartsHeaderTable = Table(data = [["Основные материалы рамы стенда"]], colWidths = leftPartWidth)
    framePartsHeaderTable.setStyle(TableStyle(cmds =
                                                PdfHelper.commonTableStyleCmd +
                                                PdfHelper.centerAlignTableStyleCmd + 
                                                PdfHelper.boldFontTableStyleCmd + 
                                                PdfHelper.visibleAllBordersTableStyleCmd ))

    framePartsRecords = columnsHeaderTitles.copy()

    for frameMaterial in stand["FrameParts"]:
        tableRecord = [
            Paragraph(PdfHelper.to_str(frameMaterial["Name"]), tableContentStyle), 
            Paragraph(PdfHelper.to_str(frameMaterial["Unit"]), tableContentStyle), 
            Paragraph(PdfHelper.to_str(frameMaterial["Quantity"]), tableContentStyle),
            Paragraph("", tableContentStyle)  # Пустая ячейка для "Факт"
        ]
        framePartsRecords.append(tableRecord)

    framePartsTable = Table(data = framePartsRecords, colWidths = [leftPartWidth*0.68, leftPartWidth*0.12, leftPartWidth*0.1, leftPartWidth*0.1])
    framePartsTable.setStyle(TableStyle(cmds =
                                        PdfHelper.commonTableStyleCmd +
                                        PdfHelper.centerAlignTableStyleCmd + 
                                        PdfHelper.usualFontTableStyleCmd + 
                                        PdfHelper.visibleAllBordersTableStyleCmd + 
                                        #шапка жирным
                                        [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] +
                                        PdfHelper.firstColumnLeftTableStyleCmd ))


    #таблица монтажных частей

    #подсчет кол-ва строк в левой части листа для выравнивания
    leftPartElementsCount = 0

    mountPartsHeaderTable = Table(data = [["Комплект монтажных частей в зависимости от обвязок"]], colWidths = leftPartWidth)
    mountPartsHeaderTable.setStyle(TableStyle(cmds =
                                              PdfHelper.commonTableStyleCmd +
                                              PdfHelper.centerAlignTableStyleCmd + 
                                              PdfHelper.boldFontTableStyleCmd + 
                                              PdfHelper.visibleAllBordersTableStyleCmd +
                                              PdfHelper.firstColumnLeftTableStyleCmd ))
    leftPartElementsCount+=1



    mountPartsRecords = columnsHeaderTitles.copy()
    leftPartElementsCount+=1

    for mountPart in stand["MountParts"]:
        tableRecord = [
           Paragraph(PdfHelper.to_str(mountPart["Name"]), tableContentStyle), 
           Paragraph(PdfHelper.to_str(mountPart["Unit"]), tableContentStyle), 
           Paragraph(PdfHelper.to_str(mountPart["Quantity"]), tableContentStyle), 
           Paragraph("", tableContentStyle)
           ]
        mountPartsRecords.append(tableRecord)
        leftPartElementsCount+=1
    
    


    #таблица дренажа

    rightPartElementsCount=0

    drainagePartsHeaderTable = Table(data = [["Дренаж и/или продувка"]], colWidths = rightPartWidth)
    drainagePartsHeaderTable.setStyle(TableStyle(cmds =
                                             PdfHelper.commonTableStyleCmd +
                                             PdfHelper.centerAlignTableStyleCmd + 
                                             PdfHelper.boldFontTableStyleCmd + 
                                             PdfHelper.visibleAllBordersTableStyleCmd ))
    rightPartElementsCount+=1

    drainagePartsRecords = columnsHeaderTitles.copy()
    rightPartElementsCount+=1

    for drainagePart in stand["DrainageParts"]:
        tableRecord = [
            Paragraph(PdfHelper.to_str(drainagePart["Name"]), tableContentStyle),
            Paragraph(PdfHelper.to_str(drainagePart["Unit"]), tableContentStyle), 
            Paragraph(PdfHelper.to_str(drainagePart["Quantity"]), tableContentStyle),
            Paragraph("", tableContentStyle)
            ]
        drainagePartsRecords.append(tableRecord)
        rightPartElementsCount+=1
    
    drainagePartsTable = Table(data = drainagePartsRecords, colWidths = [rightPartWidth*0.68, rightPartWidth*0.12, rightPartWidth*0.1,rightPartWidth*0.1])
    drainagePartsTable.setStyle(TableStyle(cmds =
                                            PdfHelper.commonTableStyleCmd +
                                            PdfHelper.centerAlignTableStyleCmd + 
                                            PdfHelper.usualFontTableStyleCmd + 
                                            PdfHelper.visibleAllBordersTableStyleCmd + 
                                            #шапка жирным
                                            [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] +
                                            PdfHelper.firstColumnLeftTableStyleCmd ))


    #таблица электрических компонентов
    electricPartsHeaderTable = Table(data = [["Электрические компоненты"]], colWidths = rightPartWidth)
    electricPartsHeaderTable.setStyle(TableStyle(cmds =
                                              PdfHelper.commonTableStyleCmd +
                                              PdfHelper.centerAlignTableStyleCmd + 
                                              PdfHelper.boldFontTableStyleCmd + 
                                              PdfHelper.visibleAllBordersTableStyleCmd ))
    rightPartElementsCount+=1

    electricPartsRecords = columnsHeaderTitles.copy()
    rightPartElementsCount+=1

    for electricPart in stand["ElectricParts"]:
        tableRecord = [
            Paragraph(PdfHelper.to_str(electricPart["Name"]), tableContentStyle),
            Paragraph(PdfHelper.to_str(electricPart["Unit"]), tableContentStyle), 
            Paragraph(PdfHelper.to_str(electricPart["Quantity"]), tableContentStyle),
            Paragraph("", tableContentStyle)
            ]
        electricPartsRecords.append(tableRecord)
        rightPartElementsCount+=1
    
    


    #чертеж стенда
    blueprintLeftElements = [standTechCardHeaderTable,
                standNameHeaderTable, 
                standInfoTable, 
                standSizeTable, 
                framesTable,
                framePartsHeaderTable, 
                framePartsTable ]

    sumHeight = 0.0
    for element in blueprintLeftElements:
        (_,elementHeight) = element.wrap(0,0)
        sumHeight += elementHeight


    imageString = stand["ImageData"]
    blueprintOriginalWidth,blueprintOriginalHeight = 0,0


    imageExist = imageString is not None

    if imageExist:     
         blueprintOriginalWidth,blueprintOriginalHeight = PdfHelper.getImageOriginalSizes(imageString)
         newWidth, newHeight , _ = PdfHelper.scaleImageToFit(blueprintOriginalWidth,blueprintOriginalHeight,rightPartWidth,sumHeight)
         standBlueprint = PdfHelper.generateImageFromStr(imageString, newWidth, newHeight)  
    else:
        standBlueprint = Paragraph(text = "Изображение отсутствует", style = cyrillicStyle)
        
    

    blueprintTable = Table(data = [[standBlueprint]], colWidths = rightPartWidth, rowHeights = sumHeight)
    blueprintTable.setStyle(TableStyle(cmds = PdfHelper.commonTableStyleCmd +
                                              PdfHelper.centerAlignTableStyleCmd + 
                                              PdfHelper.usualFontTableStyleCmd + 
                                              PdfHelper.visibleAllBordersTableStyleCmd))

    # #выравнивание таблиц по кол-вам строк
    # rowsOffset = leftPartElementsCount - rightPartElementsCount

    # targetObject = mountPartsRecords if rowsOffset < 0 else electricPartsRecords

    # for _ in range(abs(rowsOffset-1)):

    #     emptyRow = [
    #         Paragraph("",tableContentStyle),
    #         Paragraph("",tableContentStyle),
    #         Paragraph("",tableContentStyle),
    #         Paragraph("",tableContentStyle),
    #         ]

    #     targetObject.append(emptyRow)


    mountPartsTable = Table(data = mountPartsRecords, colWidths = [leftPartWidth*0.68, leftPartWidth*0.12, leftPartWidth*0.1, leftPartWidth*0.1])
    mountPartsTable.setStyle(TableStyle(cmds =
                                        PdfHelper.commonTableStyleCmd +
                                        PdfHelper.centerAlignTableStyleCmd + 
                                        PdfHelper.usualFontTableStyleCmd + 
                                        PdfHelper.visibleAllBordersTableStyleCmd + 
                                        #шапка жирным
                                        [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] +
                                        PdfHelper.firstColumnLeftTableStyleCmd))

    electricPartsTable = Table(data = electricPartsRecords, colWidths = [rightPartWidth*0.68, rightPartWidth*0.12, rightPartWidth*0.1,rightPartWidth*0.1])
    electricPartsTable.setStyle(TableStyle(cmds =
                                            PdfHelper.commonTableStyleCmd +
                                            PdfHelper.centerAlignTableStyleCmd + 
                                            PdfHelper.usualFontTableStyleCmd + 
                                            PdfHelper.visibleAllBordersTableStyleCmd+
                                            #шапка жирным
                                            [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] +
                                            PdfHelper.firstColumnLeftTableStyleCmd))

    leftPart = [ standTechCardHeaderTable,
                standNameHeaderTable, 
                standInfoTable, 
                standSizeTable, 
                framesTable,
                framePartsHeaderTable, 
                framePartsTable, 
                mountPartsHeaderTable, 
                mountPartsTable ]


    rightPart = [ blueprintTable,
                 drainagePartsHeaderTable,
                 drainagePartsTable, 
                 electricPartsHeaderTable,  
                 electricPartsTable ]



    #пытаемся выровнять левую и правую части таблицы дополнительным отступом
    leftPartHeight = PdfHelper.get_column_height(leftPart, leftPartWidth)
    rightPartHeight = PdfHelper.get_column_height(rightPart, rightPartWidth)

    partsHeightOffset = leftPartHeight - rightPartHeight

    if abs(partsHeightOffset) > 0:

        targetObject = leftPart if partsHeightOffset < 0 else rightPart
        targetWidht = leftPartWidth if partsHeightOffset < 0 else rightPartWidth

        spacerTable = Table([[""]], rowHeights = [abs(partsHeightOffset)], colWidths=[targetWidht])
        spacerTable.setStyle(TableStyle(cmds =PdfHelper.visibleAllBordersTableStyleCmd))

        targetObject.append(spacerTable)


    sheetTable = Table(data = [[ leftPart, rightPart ]], colWidths = [leftPartWidth , rightPartWidth])

    sheetTable.setStyle(TableStyle(cmds = 
                         PdfHelper.commonTableStyleCmd +
                         PdfHelper.centerAlignTableStyleCmd +
                         PdfHelper.boldFontTableStyleCmd + 
                         #выравнивание по верху
                         [('VALIGN', (0, 0), (-1, -1), "TOP")] ))


    #собираем все объекты в массив и отдаем
    sheetElements = []   
    sheetElements.append(sheetTable) 



    if not testExecution:
        ProcessSplitInfo(stand)
    
    impulseLineTable = CreateImpulseLinesTable(stand,project,tableSplittingInfo)
    sheetElements.append(impulseLineTable)
         


    return sheetElements





#генерация таблицы импульсных линий
def CreateImpulseLinesTable(stand, project, tableSplittingInfo = None):

    #вписываем в рамку
    sheetWidth = portraitParams['frameWidth'] * 0.99
    sheetHeight = portraitParams['frameHeight'] * 0.99

    #инициализируем стили листа
    styles = getSampleStyleSheet()

    tableContentStyle = ParagraphStyle(
        'TableContent',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 6,
        wordWrap = 'LTR',
        alignment = TA_CENTER,
        leading = 7
    )


    #если данных по разделению таблиц нет - тестовый проход
    testExecution = tableSplittingInfo is None

    #вытаскиваем параметр - с электрикой или без
    includeElectric = project["ReportSettings"]["TechCardIncludeElectric"]

    
    #формируем заголовок шапки
    impulseLinesHeaderData = [ 
        ["№\nимп.линии", "Наименование импульсной линии\n и код KKS", "Таблица соединений","","","","Примечание"],
        ["","","Цепь","Маркировка","Коробка","Клеммы",""] ]
        

    impulseLineTableData = impulseLinesHeaderData.copy()
    
    #если проход чистовой - обрабатываем инфу о стенде
    if not testExecution:
        ProcessSplitInfo(stand);


 
    standImpulseLines = stand["ImpulseLines"]

    #вытаскиваем и подготавливаем данные для вставки каждой импульсной линии
    for i, impulseLine in enumerate(standImpulseLines,1):

        #подготавливаем данные
        impulseLineNumber = str(i)

        impulseLineDescAndKKS = [impulseLine["Name"],impulseLine["CodeKKS"]]
        impulseLineDescAndKKS= "<br/>".join(impulseLineDescAndKKS)

        wires = []
        for wire in impulseLine["Wires"]:

            #в зависимости от параметра вставляем электрику или нет
            if includeElectric:
                wires.append([ wire["Circuit"], wire["Mark"], wire["ElectricBox"], wire["Terminal"] ])                
            else:
                wires.append( ["","","",""] )

        impulseLineNote = impulseLine["Annotation"]

        #формируем строки таблицы
        for j, wire in enumerate(wires):

            if j == 0:
                rowArray = [impulseLineNumber, impulseLineDescAndKKS]
            else:
                rowArray = ["", ""]

            rowArray.extend(wire)

            if j == 0:
                rowArray.extend([impulseLineNote])
            else:
                rowArray.extend([""])

            impulseLineTableData.append(rowArray)

       



    #если проход чистовой - убираем лишние данные 
    if not testExecution:

       #вытаскиваем стенд из словаря
       standNN = stand["Number"]
       standTableData = tableSplittingInfo.get(standNN);

       if standTableData is not None:
            
        standPages = standTableData["pages"]

        boxColumnIndex = 4
        annotationColumnIndex = 6

        #проходим по всем страницам
        for p, pageInfo in enumerate(standPages):    

            middleRecordFirstRowIndex = pageInfo["middleRecordFirstRowIndex"]

        #проходимся по строкам таблицы
        #стираем все данные по коробке и примечанию
        for i, _ in enumerate(impulseLineTableData):

            #шапку не трогаем
            if i < 2:
                continue

            #среднюю запись не трогаем
            if i == middleRecordFirstRowIndex:
                continue

            impulseLineTableData[i][boxColumnIndex] = ""
            impulseLineTableData[i][annotationColumnIndex] = ""

            
    #оформляем таблицу   
    impulseLineTableColumnSizes = [sheetWidth * 0.075,
                                    sheetWidth * 0.275,
                                    sheetWidth * 0.1,
                                    sheetWidth * 0.15,
                                    sheetWidth * 0.15,
                                    sheetWidth * 0.1,
                                    sheetWidth * 0.15]

    allTableColumnCount = 6

    #проходимся по строкам и столбцам таблицы 
    #заводим все данные в Paragraph
    for i, _ in enumerate(impulseLineTableData):

        #шапку не трогаем
        if i < 2:
            continue

        for j in range(allTableColumnCount+1):

            impulseLineTableData[i][j] = Paragraph(impulseLineTableData[i][j], tableContentStyle)





    impulseLineTable = MySplittableTable(data = impulseLineTableData, 
                                            colWidths = impulseLineTableColumnSizes, 
                                            standNumber = stand["Number"], #передаем номер стенда
                                            repeatRows=2, #повторяем шапку на каждой странице
                                            splitByRow=1)  #разрешаем разделять по строкам



    impulseLineTableStyleCmds = PdfHelper.commonTableStyleCmd.copy()

    impulseLineTableStyleCmds.extend(PdfHelper.centerAlignTableStyleCmd +
                                            PdfHelper.visibleAllBordersTableStyleCmd + 
                                            PdfHelper.usualFontTableStyleCmd)
    #оформляем шапку
    impulseLineTableStyleCmds.extend([ ('FONTNAME', (0, 0), (-1, 1), "Arial-Bold"),
                                       ('SPAN', (0, 0), (0,1) ),
                                       ('SPAN', (1, 0), (1, 1) ),
                                       ('SPAN', (-1, 0), (-1, 1) ),
                                       ('SPAN', (-1, 0), (-1, 1) ),
                                       ('SPAN', (2, 0), (5, 0) )
                                        ])
                                      
    headerRows = 2
    recordsStartRow = 2
    rowsPerRecord = 3

    currentRow = recordsStartRow
    recordEndRow = 0

 
    #оформляем импульсные линии
    for impulseLineRecord in range(len(standImpulseLines)):

        recordEndRow = currentRow + rowsPerRecord - 1

        impulseLineTableStyleCmds.extend([
            ('SPAN', (0, currentRow), (0, recordEndRow)),  #номер имп линии
            ('SPAN', (1, currentRow), (1, recordEndRow)),   #наименование имп линии  
            ('SPAN', (4, currentRow), (4, recordEndRow)),   #коробка
            ('SPAN', (-1, currentRow), (-1, recordEndRow)) #примечание 
            ])      
                                        

        currentRow +=rowsPerRecord



    #если генерация окончательная -  отрисовываем все по правильному
    if not testExecution:
            
        #вытаскиваем стенд из словаря
        standNN = stand["Number"]
        standTableData = tableSplittingInfo.get(standNN);

        print("---------------------")
        print(f"standNN: {standNN}")
        print("---------------------")

        
        if standTableData is not None:
            
            standPages = standTableData["pages"]

            for i, pageInfo in enumerate(standPages):

                startDataRowIndex = pageInfo["startDataRowIndex"]
                endDataRowIndex = pageInfo["endDataRowIndex"]

                #применяем оформление к одной странице
                impulseLineTableStyleCmds.extend([

                    # Убираем сетку внутри блоков 
                    ('INNERGRID', (4, startDataRowIndex), (4, endDataRowIndex), 2, colors.white),   #коробка
                    ('INNERGRID', (-1, startDataRowIndex), (-1, endDataRowIndex), 2, colors.white),   #примечание

                    # прорисовываем вертикальные границы заново 
                    ('LINEBEFORE', (4, startDataRowIndex), (4, endDataRowIndex), 1, colors.black), #коробка
                    ('LINEAFTER', (4, startDataRowIndex), (4, endDataRowIndex), 1, colors.black),   #коробка

                    ('LINEBEFORE', (-1, startDataRowIndex), (-1, endDataRowIndex), 1, colors.black), #примечание
                    ('LINEAFTER', (-1, startDataRowIndex), (-1, endDataRowIndex), 1, colors.black)  #примечание
 
                ])


    impulseLineTable.setStyle(TableStyle(cmds= impulseLineTableStyleCmds ))

    return impulseLineTable




def ProcessSplitInfo(stand):

    headerRows = 2
    recordsStartRow = 2
    rowsPerRecord = 3


    impulseLineStartRecordIndex = 1

    #вытаскиваем кол-во импульсных линий
    impulseLineCount = len(stand["ImpulseLines"])

    #вытаскиваем стенд из словаря
    standNN = stand["Number"]
    standData = impulseTableInfo.get(standNN);

    standDataExist = standData is not None

    #если его там нет - инициализируем
    if not standDataExist:
        impulseTableInfo[standNN] = {
                      "isPageSplitted": False,
                      "pagesCount" : 1,
                      "pages": []                      
              }

        # Добавляем единственную страницу
        impulseTableInfo[standNN]["pages"].append(
            {
                "pageNumber" : 1,
                "hasHeader"  : True,
                "rowsCount"  : (rowsPerRecord * impulseLineCount) + headerRows
            })



   
    standPages = impulseTableInfo[standNN]["pages"]
    standPagesCount = impulseTableInfo[standNN]["pagesCount"]
    pagesSplitted = impulseTableInfo[standNN]["isPageSplitted"]
  


    tableDataStartRow = 0
    tableDataEndRow = 0

    # Сумма записей на предыдущих страницах
    rowsProceed = 0
    recordsProcessed = 0  

    #обрабатываем каждую страницу
    for i, page in enumerate(standPages):

        pageRowsCount =  impulseTableInfo[standNN]["pages"][i]["rowsCount"]
        pageHasHeader = impulseTableInfo[standNN]["pages"][i]["hasHeader"]

        
        if (standPagesCount > 1 and i > 0):
            tableDataStartRow = tableDataEndRow + 1
        else:
            tableDataStartRow = 0

        tableDataRowsCount = pageRowsCount

        #если есть заголовок - перескакиваем через него
        if pageHasHeader:
            tableDataStartRow += headerRows
            tableDataRowsCount -= headerRows
    
        #высчитываем начальную, конечную и среднюю строки
        tableDataEndRow = (tableDataStartRow + tableDataRowsCount) - 1
        tableDataMiddleRow = (tableDataStartRow + tableDataEndRow) // 2

        impulseTableInfo[standNN]["pages"][i]["startDataRowIndex"] = tableDataStartRow
        impulseTableInfo[standNN]["pages"][i]["endDataRowIndex"] = tableDataEndRow
        impulseTableInfo[standNN]["pages"][i]["middleDataRowIndex"] = tableDataMiddleRow



        #высчитываем количество записей
        tableRecordsCount = tableDataRowsCount // rowsPerRecord
            

        if (standPagesCount > 1 and i > 0):
            tableStartRecord = tableEndRecord + 1
        else:
            tableStartRecord = impulseLineStartRecordIndex


        #высчитываем начальную, конечную и среднюю записи
        
        tableEndRecord = (tableStartRecord + tableRecordsCount) - 1
        tableMiddleRecord = (tableStartRecord + tableEndRecord) // 2

        impulseTableInfo[standNN]["pages"][i]["recordsCount"] = tableRecordsCount
        impulseTableInfo[standNN]["pages"][i]["startRecordIndex"] = tableStartRecord
        impulseTableInfo[standNN]["pages"][i]["endRecordIndex"] = tableEndRecord
        impulseTableInfo[standNN]["pages"][i]["middleRecordIndex"] = tableMiddleRecord


        if (standPagesCount > 1 and i > 0):
            startRecordFirstRowIndex = tableDataEndRow + 1
        else:
            startRecordFirstRowIndex = impulseLineStartRecordIndex

        #высчитываем первую строчку начальной, конечной и средней записи
        startRecordFirstRowIndex = tableDataStartRow
        endRecordFirstRowIndex = tableDataEndRow - rowsPerRecord + 1

        middleRecordOnPage = tableMiddleRecord - recordsProcessed
        middleRecordFirstRowIndex = startRecordFirstRowIndex + ((middleRecordOnPage - 1) * rowsPerRecord)

        impulseTableInfo[standNN]["pages"][i]["startRecordFirstRowIndex"] = startRecordFirstRowIndex
        impulseTableInfo[standNN]["pages"][i]["endRecordFirstRowIndex"] = endRecordFirstRowIndex
        impulseTableInfo[standNN]["pages"][i]["middleRecordFirstRowIndex"] = middleRecordFirstRowIndex

        recordsProcessed += tableRecordsCount


            


def fillConclusionPage(stand,project):

    #вписываем в рамку
    sheetWidth = landscapeParams['frameWidth'] * 0.99
    sheetHeight = landscapeParams['frameHeight'] * 0.99

    #инициализируем стили листа
    styles = getSampleStyleSheet()

    cyrillicStyle = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 7
    )

    #таблица с инфой о стенде и лого
    standTable = [["","Значение"]]
    standTable.append(["Наименование", "Стенд датчиков КИПиА"])
    standTable.append(["Обозначение по КД", PdfHelper.to_str(stand["Designation"])])
    standTable.append(["Чертеж", ""])
    standTable.append(["Зав.номер", PdfHelper.to_str(stand["SerialNumber"])])     
    standInfoTable = Table(data = standTable, colWidths = [sheetWidth*0.2,sheetWidth*0.3])

    standInfoTable.setStyle(TableStyle(cmds = 
                                       PdfHelper.commonTableStyleCmd +
                                       PdfHelper.centerAlignTableStyleCmd + 
                                       PdfHelper.visibleAllBordersTableStyleCmd +    
                                       #жирный шрифт для шапки 
                                       [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold"), 
                                        ('FONTNAME', (0, 0), (0, -1), "Arial-Bold")] +      
                                       #обычный шрифт для данных таблицы
                                       [('FONTNAME', (1, 1), (-1, -1), "Arial")] ))

    logoImage =  PdfHelper.generateImageFromFile("Etalon.jpg",sheetWidth * 0.18,sheetHeight * 0.15)


    standInfoAlignmentTable = Table(data = [[standInfoTable,logoImage]], 
                                    colWidths = [sheetWidth*0.52,sheetWidth*0.2])

    standInfoAlignmentTable.setStyle(TableStyle(cmds = 
                                                 PdfHelper.commonTableStyleCmd +
                                                 PdfHelper.centerAlignTableStyleCmd + 
                                                 PdfHelper.invisibleAllBordersTableStyleCmd ))

    #графа № заказа на производство
    orderNumberAlignmentTable = Table(data = [["№ заказа на производство",""]], 
                                      colWidths = [sheetWidth*0.15,sheetWidth*0.25],
                                      hAlign = 'LEFT')

    orderNumberAlignmentTable.setStyle(TableStyle(cmds = 
                                                   PdfHelper.commonTableStyleCmd +
                                                   PdfHelper.leftAlignTableStyleCmd + 
                                                   PdfHelper.usualFontTableStyleCmd + 
                                                   [('GRID', (-1, 0), (-1, 0), 1, colors.black)] ))

    #таблица исполнения этапов
    doneTable = [["№ п/п", 
                  "Наименование\n операции", 
                  "№ извещения о\n несоотвествии",
                  "Дата фактического\n выполнения\n операции",
                  "Ф.И.О. исполнителя",
                  "Подпись\n исполнителя", 
                  "№ протокола (ЛКП, ПСИ и т.д.)"]]


    doneTable.append(["1", "СВАРОЧНАЯ","", "", "", "", ""])
    doneTable.append(["2", "СБОРОЧНАЯ (АРМАТУРА)","", "", "", "", ""])
    doneTable.append(["3", "ПОДГОТОВИТЕЛЬНО-ОКРАСОЧНАЯ","", "", "", "", ""])
    doneTable.append(["4", "СБОРОЧНАЯ (ЭЛЕКТРИЧЕСКАЯ ЧАСТЬ)","", "", "", "", ""])
    doneTable.append(["5", "КОНТРОЛЬНАЯ","", "", "", "", ""])
    doneTable.append(["", "","", "", "", "", ""])
    doneStagesTable = Table(data = doneTable,
                          colWidths =[sheetWidth*0.05,sheetWidth*0.2, sheetWidth*0.1, sheetWidth*0.125,sheetWidth*0.125, sheetWidth*0.1, sheetWidth*0.15],
                          rowHeights=[sheetHeight*0.08,sheetHeight*0.05,sheetHeight*0.05,sheetHeight*0.05,sheetHeight*0.05,sheetHeight*0.05,sheetHeight*0.05])



    doneStagesTable.setStyle(TableStyle(cmds = 
                                       PdfHelper.commonTableStyleCmd +
                                       PdfHelper.centerAlignTableStyleCmd + 
                                       PdfHelper.visibleAllBordersTableStyleCmd +
                                      #жирный шрифт для шапки      
                                      [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] + 
                                      #обычный шрифт для тела
                                      [('FONTNAME', (0, 1), (-1, -1), "Arial")] ))



    #подписи
    productReadyLabel = "Изделие признано годным" + "\n" + "и передано на склад"
    controlSignatureLabel = "ОТК (ФИО, подпись)"
    storeSignatureLabel = "Склад (ФИО, подпись)"


    signatureAligmentTable = Table(data = [["",controlSignatureLabel, storeSignatureLabel],
                                           [productReadyLabel, "", ""]],
                                           colWidths = [sheetWidth * 0.15, sheetWidth * 0.15, sheetWidth * 0.15],
                                           hAlign ='LEFT' )

    signatureAligmentTable.setStyle(TableStyle(cmds = 
                                                PdfHelper.commonTableStyleCmd +
                                                PdfHelper.invisibleAllBordersTableStyleCmd +
                                                PdfHelper.boldFontTableStyleCmd +
                                                PdfHelper.centerAlignTableStyleCmd +
                                                #label к подписям
                                               [("VALIGN", (0, 0), (-1, 0), "BOTTOM")] + 
                                               #label годности изделия
                                               [("VALIGN", (0, -1), (0, -1), "MIDDLE")] + 
                                               #видимые боксы для подписей
                                               [('GRID', (1, -1), (-1, -1), 1, colors.black)]))
    #собираем все элементы листа
    sheetElements = []

    sheetElements.append(standInfoAlignmentTable)
    sheetElements.append(Spacer(1,20))
    sheetElements.append(orderNumberAlignmentTable)
    sheetElements.append(Spacer(1,20))
    sheetElements.append(doneStagesTable)
    sheetElements.append(Spacer(1,100))
    sheetElements.append(signatureAligmentTable)
    
    return sheetElements






#основной генератор отчета
def generateReport(jsonFilePath,outputFilePath):

    #сначала делаем тестовый проход, чтобы узнать как поделились таблицы
    generateTestReport(jsonFilePath)
    

    print(impulseTableInfo)

    print("CLEAN EXECUTION")

    PdfHelper.registerFonts()

    data = PdfHelper.openJsonFile(jsonFilePath)
    doc = SimpleDocTemplate(outputFilePath, pagesize=A4)
    doc.addPageTemplates([portraitTemplate,landscapeTemplate])

    elements = []

    for stand in data["Stands"]:      
        standSheet = fillStandPage(stand,data, tableSplittingInfo = impulseTableInfo) # теперь передаем инфу в заполнитель
        elements.extend(standSheet)  
        elements.append(NextPageTemplate('landscape'))
        elements.append(PageBreak())
        conclusionSheet = fillConclusionPage(stand,data)
        elements.extend(conclusionSheet)
        elements.append(NextPageTemplate('portrait'))
        elements.append(PageBreak())



    print(f"final object: {impulseTableInfo}")
    doc.build(elements)

    



#тестовый проход, чтобы получить информацию о таблицах
def generateTestReport(jsonFilePath):
 
    print("TEST EXECUTION")

    #принудительно чистим
    global splitInfo
    impulseTableInfo ={}


    #временный буфер в памяти для тестового проекта
    buffer = io.BytesIO()


    PdfHelper.registerFonts()

    data = PdfHelper.openJsonFile(jsonFilePath)
    doc = SimpleDocTemplate(buffer, pagesize=A4)  
    doc.addPageTemplates([portraitTemplate,landscapeTemplate])

    elements = []

    for stand in data["Stands"]:      
        standSheet = fillStandPage(stand,data)
        elements.extend(standSheet)  
        elements.append(NextPageTemplate('landscape'))
        elements.append(PageBreak())
        conclusionSheet = fillConclusionPage(stand,data)
        elements.extend(conclusionSheet)
        elements.append(NextPageTemplate('portrait'))
        elements.append(PageBreak())

    doc.build(elements)

    #чистим буфер
    buffer.close()
    
if __name__ == "__main__":
    generateReport()
