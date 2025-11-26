from reportlab.lib.pagesizes import A4, landscape,portrait
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import (PageTemplate, SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, NextPageTemplate, Frame)
from reportlab.lib.units import mm
from reportlab.lib.enums import TA_LEFT, TA_CENTER, TA_RIGHT, TA_JUSTIFY
import PdfHelper
from reportlab.lib import colors


landscapeParams = {
    "startPointX" : 20 * mm,
    "startPointY": 20 * mm,
    "frameWidth":A4[1] - 40*mm,
    "frameHeight": A4[0] - 40*mm,
    "frameId": 'landscapeFrame',
    "visibleBoundaries": 0
}


portraitParams = {
    "startPointX" : 20 * mm,
    "startPointY": 20 * mm,
    "frameWidth":A4[0] - 40*mm,
    "frameHeight": A4[1] - 40*mm,
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



def fillStandPage(stand, doc, project):
    
    sheetWidth = A4[0]
    sheetHeight = A4[1]

    leftPartWidth = 0.58 * sheetWidth
    rightPartWidth = 0.38 * sheetWidth
    
    styles = getSampleStyleSheet()

    cyrillicStyle = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 7
    )


    #общие заголовки таблицы
    galvanizeStr = "Оцинковка" if project["IsGalvanized"] else "Покраска"
    standTechCardHeaderTable = Table(data = [[ "Технологическая карта", str(galvanizeStr), str(project["Description"]) ]],
                                   colWidths= leftPartWidth/3)
    standTechCardHeaderTable.setStyle(TableStyle(cmds =
                                                 PdfHelper.commonTableStyleCmd +
                                                 PdfHelper.centerAlignTableStyleCmd + 
                                                 PdfHelper.usualFontTableStyleCmd + 
                                                 PdfHelper.visibleOuterBordersTableStyleCmd +
                                                 PdfHelper.invisibleInnerBordersTableStyleCmd +
                                                 #Технологическая карта жирным
                                                 [('FONTNAME', (0, 0), (0, 0), "Arial-Bold")]
                                                 #У крайних ячеек выравнивание по краям
                                                 #[ ('ALIGN', (0, 0), (0, 0), 'LEFT')] +
                                                 #[ ('ALIGN', (-1, -1), (-1, -1), 'RIGHT')]
                                                 )) 
    

    
    

    standNameData = [[ "Стенд датчиков КИПиА " + str(stand["Designation"]) ]]
    standNameHeaderTable = Table(data = standNameData, colWidths = leftPartWidth)
    standNameHeaderTable.setStyle(TableStyle(cmds =
                                             PdfHelper.commonTableStyleCmd +
                                             PdfHelper.centerAlignTableStyleCmd + 
                                             PdfHelper.boldFontTableStyleCmd + 
                                             PdfHelper.visibleAllBordersTableStyleCmd ))  
    

    standsInfoData = [[ str(stand["KKSCode"]) , str(stand["SerialNumber"]) ]]
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
        frameArray = [frame["Width"], frame["DocName"], frame["Quantity"]]
        framesTableData.append(frameArray)

    framesTable = Table(data = framesTableData, colWidths = [leftPartWidth*0.15, leftPartWidth*0.75, leftPartWidth*0.1])
    framesTable.setStyle(TableStyle(cmds =
                                    PdfHelper.commonTableStyleCmd +
                                    PdfHelper.centerAlignTableStyleCmd + 
                                    PdfHelper.usualFontTableStyleCmd + 
                                    PdfHelper.visibleAllBordersTableStyleCmd + 
                                    #шапка жирным
                                    [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))


    columnsHeaderTitles = [["Наименование", "Единицы\n измерения", "Норм.","Факт."]]

    
    #таблица материалов рам
    framePartsHeaderTable = Table(data = [["Основные материалы рамы стенда"]], colWidths = leftPartWidth)
    framePartsHeaderTable.setStyle(TableStyle(cmds =
                                                PdfHelper.commonTableStyleCmd +
                                                PdfHelper.centerAlignTableStyleCmd + 
                                                PdfHelper.boldFontTableStyleCmd + 
                                                PdfHelper.visibleAllBordersTableStyleCmd ))

    framePartsRecords = columnsHeaderTitles.copy()

    for frameMaterial in stand["FrameParts"]:
        tableRecord = [frameMaterial["Name"], frameMaterial["Unit"], frameMaterial["Quantity"],""]
        framePartsRecords.append(tableRecord)

    framePartsTable = Table(data = framePartsRecords, colWidths = [leftPartWidth*0.65, leftPartWidth*0.15, leftPartWidth*0.1, leftPartWidth*0.1])
    framePartsTable.setStyle(TableStyle(cmds =
                                        PdfHelper.commonTableStyleCmd +
                                        PdfHelper.centerAlignTableStyleCmd + 
                                        PdfHelper.usualFontTableStyleCmd + 
                                        PdfHelper.visibleAllBordersTableStyleCmd + 
                                        #шапка жирным
                                        [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))


    #таблица монтажных частей
    mountPartsHeaderTable = Table(data = [["Комплект монтажных частей в зависимости от обвязок"]], colWidths = leftPartWidth)
    mountPartsHeaderTable.setStyle(TableStyle(cmds =
                                              PdfHelper.commonTableStyleCmd +
                                              PdfHelper.centerAlignTableStyleCmd + 
                                              PdfHelper.boldFontTableStyleCmd + 
                                              PdfHelper.visibleAllBordersTableStyleCmd ))


    mountPartsRecords = columnsHeaderTitles.copy()

    for mountPart in stand["MountParts"]:
        tableRecord = [mountPart["Name"], mountPart["Unit"], mountPart["Quantity"],""]
        mountPartsRecords.append(tableRecord)
    
    mountPartsTable = Table(data = mountPartsRecords, colWidths = [leftPartWidth*0.65, leftPartWidth*0.15, leftPartWidth*0.1, leftPartWidth*0.1])
    mountPartsTable.setStyle(TableStyle(cmds =
                                        PdfHelper.commonTableStyleCmd +
                                        PdfHelper.centerAlignTableStyleCmd + 
                                        PdfHelper.usualFontTableStyleCmd + 
                                        PdfHelper.visibleAllBordersTableStyleCmd + 
                                        #шапка жирным
                                        [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))

    #таблица дренажа
    drainagePartsHeaderTable = Table(data = [["Дренаж и/или продувка"]], colWidths = rightPartWidth)
    drainagePartsHeaderTable.setStyle(TableStyle(cmds =
                                             PdfHelper.commonTableStyleCmd +
                                             PdfHelper.centerAlignTableStyleCmd + 
                                             PdfHelper.boldFontTableStyleCmd + 
                                             PdfHelper.visibleAllBordersTableStyleCmd ))

    drainagePartsRecords = columnsHeaderTitles.copy()

    for drainagePart in stand["MountParts"]:
        tableRecord = [drainagePart["Name"], drainagePart["Unit"], drainagePart["Quantity"],""]
        drainagePartsRecords.append(tableRecord)
    
    drainagePartsTable = Table(data = drainagePartsRecords, colWidths = [rightPartWidth*0.65, rightPartWidth*0.15, rightPartWidth*0.1,rightPartWidth*0.1])
    drainagePartsTable.setStyle(TableStyle(cmds =
                                            PdfHelper.commonTableStyleCmd +
                                            PdfHelper.centerAlignTableStyleCmd + 
                                            PdfHelper.usualFontTableStyleCmd + 
                                            PdfHelper.visibleAllBordersTableStyleCmd + 
                                            #шапка жирным
                                            [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))


    #таблица электрическх компонентов
    electricPartsHeaderTable = Table(data = [["Электрические компоненты"]], colWidths = rightPartWidth)
    electricPartsHeaderTable.setStyle(TableStyle(cmds =
                                              PdfHelper.commonTableStyleCmd +
                                              PdfHelper.centerAlignTableStyleCmd + 
                                              PdfHelper.boldFontTableStyleCmd + 
                                              PdfHelper.visibleAllBordersTableStyleCmd ))

    electricPartsRecords = columnsHeaderTitles.copy()

    for electricPart in stand["MountParts"]:
        tableRecord = [electricPart["Name"], electricPart["Unit"], electricPart["Quantity"],""]
        electricPartsRecords.append(tableRecord)
    
    electricPartsTable = Table(data = electricPartsRecords, colWidths = [rightPartWidth*0.65, rightPartWidth*0.15, rightPartWidth*0.1,rightPartWidth*0.1])
    electricPartsTable.setStyle(TableStyle(cmds =
                                            PdfHelper.commonTableStyleCmd +
                                            PdfHelper.centerAlignTableStyleCmd + 
                                            PdfHelper.usualFontTableStyleCmd + 
                                            PdfHelper.visibleAllBordersTableStyleCmd ))


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
    if imageString is not None:  
        standBlueprint = PdfHelper.generateImageFromStr(imageString, rightPartWidth, sumHeight)  
    else:
        standBlueprint = Paragraph(text = "Ха-ха, пiймав на пiкчу", style = cyrillicStyle)


    blueprintTable = Table(data = [[standBlueprint]], colWidths = rightPartWidth, rowHeights = sumHeight)
    blueprintTable.setStyle(TableStyle(cmds = PdfHelper.commonTableStyleCmd +
                                              PdfHelper.centerAlignTableStyleCmd + 
                                              PdfHelper.usualFontTableStyleCmd + 
                                              PdfHelper.visibleAllBordersTableStyleCmd))

    leftPart = [standTechCardHeaderTable,
                standNameHeaderTable, 
                standInfoTable, 
                standSizeTable, 
                framesTable,
                framePartsHeaderTable, 
                framePartsTable, 
                mountPartsHeaderTable, 
                mountPartsTable]

    rightPart = [blueprintTable,
                 drainagePartsHeaderTable,
                 drainagePartsTable, 
                 electricPartsHeaderTable,  
                 #electricPartsTable
                 ]

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
    

    
        
    return sheetElements



def fillConclusionPage(stand,doc,project):

    sheetWidth = A4[1]
    sheetHeight = A4[0]

    styles = getSampleStyleSheet()

    cyrillicStyle = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 7
    )

    sheetElements = []

   

    #таблица с инфой о стенде и лого
    standTable = [["","Значение"]]
    standTable.append(["Наименование", "Стенд датчиков КИПиА"])
    standTable.append(["Обозначение по КД", str(stand["Designation"])])
    standTable.append(["Чертеж", ""])
    standTable.append(["Зав.номер", str(stand["SerialNumber"])])     
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
                                      colWidths = [sheetWidth*0.12,sheetWidth*0.21],
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
                                           hAlign ='LEFT')

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
    sheetElements.append(standInfoAlignmentTable)
    sheetElements.append(Spacer(1,10))
    sheetElements.append(orderNumberAlignmentTable)
    sheetElements.append(Spacer(1,10))
    sheetElements.append(doneStagesTable)
    sheetElements.append(Spacer(1,30))
    sheetElements.append(signatureAligmentTable)
    
    return sheetElements


def generateReport(jsonFilePath,outputFilePath):

    PdfHelper.registerFonts()

    data = PdfHelper.openJsonFile(jsonFilePath)
    doc = SimpleDocTemplate(outputFilePath, pagesize=A4)
    doc.addPageTemplates([portraitTemplate,landscapeTemplate])

    elements = []

    for stand in data["Stands"]:      
        standSheet = fillStandPage(stand,doc,data)
        elements.extend(standSheet)  
        elements.append(NextPageTemplate('landscape'))
        elements.append(PageBreak())
        conclusionSheet = fillConclusionPage(stand,doc,data)
        elements.extend(conclusionSheet)
        elements.append(NextPageTemplate('portrait'))
        elements.append(PageBreak())

    doc.build(elements)

    
if __name__ == "__main__":
    generateReport()
