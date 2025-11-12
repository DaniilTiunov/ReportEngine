from reportlab.lib import colors
from reportlab.lib.pagesizes import A4, landscape,portrait
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import (KeepInFrame, PageTemplate, SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak,Image, NextPageTemplate, Frame)
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont
from reportlab.pdfgen import canvas
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.lib.units import cm,mm
import json
import base64
import io
import os
from datetime import datetime
from pathlib import Path


pdfmetrics.registerFont(TTFont('Arial','arial.ttf'))
pdfmetrics.registerFont(TTFont('Arial-Bold','arialbd.ttf'))
pdfmetrics.registerFont(UnicodeCIDFont('STSong-Light'))


landscapeTemplate = PageTemplate(
        id='landscape', 
        pagesize=landscape(A4),
        frames= Frame(
            20*mm, 20*mm,  # левый и нижний отступ
            A4[1] - 40*mm, A4[0] - 40*mm,  # меняем местами для альбомной
            id='landscape_frame'
    ))

portraitTemplate = PageTemplate(
        id = 'portrait', 
        pagesize = portrait(A4),
        frames = Frame(
            20*mm, 20*mm,  # левый и нижний отступ
            A4[0] - 40*mm, A4[1] - 40*mm,  # ширина и высота
            id='portrait_frame'
    ))

commonTableStyleCmd = [    
        ('BACKGROUND', (0, 0), (-1, 0), colors.white),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ('FONTSIZE', (0, 0), (-1, -1), 8)]

leftAlignTableStyleCmd = [ ('ALIGN', (0, 0), (-1, -1), 'LEFT')]
centerAlignTableStyleCmd = [ ('ALIGN', (0, 0), (-1, -1), 'CENTER')]

usualFontTableStyleCmd = [('FONTNAME', (0, 0), (-1, -1), "Arial")]
boldFontTableStyleCmd = [('FONTNAME', (0, 0), (-1, -1), "Arial-Bold")]

visibleAllBordersTableStyleCmd = [('GRID', (0, 0), (-1, -1), 1, colors.black)]
invisibleAllBordersTableStyleCmd = []

invisibleOuterBordersTableStyleCmd = []
visibleOuterBordersTableStyleCmd = [('BOX', (0, 0), (-1, -1),1, colors.black)]

invisibleInnerBordersTableStyleCmd = []
visibleInnerBordersTableStyleCmd = [('INNERGRID', (0, 0), (-1, -1),1, colors.black)]


def openJsonFile():
    
    script_dir = Path(__file__).parent
    file_path = os.path.join(script_dir, "TechnologicalCards_temp.json")
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as file:
            jsonData = json.load(file)           
    except Exception as e:
            print(f"Error: {e}")

    return jsonData


def generateLogo(width,height):
    script_dir = Path(__file__).parent
    file_path = os.path.join(script_dir, "Etalon.jpg")
    return Image(file_path,width,height)
    

def generateImageFromStr(base64_string,width, height):
    imageData = base64.b64decode(base64_string)
    imageBuffer = io.BytesIO(imageData)
    return Image(imageBuffer,width,height)



def fillStandDataSheet(stand,doc,project):
    
    sheetWidth = A4[0]
    sheetHeight = A4[1]

    styles = getSampleStyleSheet()

    cyrillic_style = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 8
    )


    #общие заголовки таблицы
    galvanizeStr = "Оцинковка" if project["IsGalvanized"] else "Покраска"
    standTechCardHeaderTable = Table(data = [[ "Технологическая карта", str(galvanizeStr), str(project["Description"]) ]],
                                   colWidths= [sheetWidth*0.2])
    standTechCardHeaderTable.setStyle(TableStyle(cmds =
                                                commonTableStyleCmd +
                                                centerAlignTableStyleCmd + 
                                                usualFontTableStyleCmd + 
                                                visibleOuterBordersTableStyleCmd +
                                                invisibleInnerBordersTableStyleCmd +
                                                #Технологическая карта жирным
                                                [('FONTNAME', (0, 0), (0, 0), "Arial-Bold")] ))   

    
    

    standNameData = [[ "Стенд датчиков КИПиА " + str(stand["Designation"]) ]]
    standNameHeaderTable = Table(data = standNameData, colWidths = sheetWidth * 0.6)
    standNameHeaderTable.setStyle(TableStyle(cmds =
                                             commonTableStyleCmd +
                                             centerAlignTableStyleCmd + 
                                             boldFontTableStyleCmd + 
                                             visibleAllBordersTableStyleCmd ))  
    

    standsInfoData = [[ str(stand["KKSCode"]) , str(stand["SerialNumber"]) ]]
    standInfoTable = Table(data = standsInfoData, colWidths = sheetWidth*0.3)
    standInfoTable.setStyle(TableStyle(cmds =
                                       commonTableStyleCmd +
                                       centerAlignTableStyleCmd + 
                                       boldFontTableStyleCmd + 
                                       visibleAllBordersTableStyleCmd ))

    standSizeData = [[ "Размер стенда, мм ", str(stand["Width"]) ]]
    standSizeTable = Table(data = standSizeData, colWidths = [sheetWidth*0.4, sheetWidth * 0.2])
    standSizeTable.setStyle(TableStyle(cmds =
                                       commonTableStyleCmd +
                                       centerAlignTableStyleCmd + 
                                       boldFontTableStyleCmd + 
                                       visibleAllBordersTableStyleCmd  ))

    
    #таблица рам
    framesTableHeaderData = [["Рама, мм", "Обозначение по КД", "Кол-во,\n шт", "","",""]]
    framesTableData = framesTableHeaderData.copy()

    for frame in stand["Frames"]:
        frameArray = [frame["Width"], frame["DocName"], frame["Quantity"],"","",""]
        framesTableData.append(frameArray)

    framesTable = Table(data = framesTableData, colWidths = [sheetWidth*0.1, sheetWidth*0.2, sheetWidth*0.075,sheetWidth*0.075,sheetWidth*0.075,sheetWidth*0.075])
    framesTable.setStyle(TableStyle(cmds =
                                    commonTableStyleCmd +
                                    centerAlignTableStyleCmd + 
                                    usualFontTableStyleCmd + 
                                    visibleAllBordersTableStyleCmd + 
                                    #шапка жирным
                                    [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))


    columnsHeaderTitles = [["Наименование", "Единицы\n измерения", "Норм.","Факт.", ""]]

    
    #таблица материалов рам
    framePartsHeaderTable = Table(data = [["Основные материалы рамы стенда"]], colWidths = sheetWidth * 0.6)
    framePartsHeaderTable.setStyle(TableStyle(cmds =
                                                commonTableStyleCmd +
                                                centerAlignTableStyleCmd + 
                                                boldFontTableStyleCmd + 
                                                visibleAllBordersTableStyleCmd ))

    framePartsRecords = columnsHeaderTitles.copy()

    for frameMaterial in stand["FrameParts"]:
        tableRecord = [frameMaterial["Name"], frameMaterial["Unit"], frameMaterial["Quantity"],"",""]
        framePartsRecords.append(tableRecord)

    framePartsTable = Table(data = framePartsRecords, colWidths = [sheetWidth*0.3, sheetWidth*0.075, sheetWidth*0.075,sheetWidth*0.075,sheetWidth*0.075])
    framePartsTable.setStyle(TableStyle(cmds =
                                        commonTableStyleCmd +
                                        centerAlignTableStyleCmd + 
                                        usualFontTableStyleCmd + 
                                        visibleAllBordersTableStyleCmd + 
                                        #шапка жирным
                                        [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))


    #таблица монтажных частей
    mountPartsHeaderTable = Table(data = [["Комплект монтажных частей в зависимости от обвязок"]], colWidths = sheetWidth * 0.6)
    mountPartsHeaderTable.setStyle(TableStyle(cmds =
                                              commonTableStyleCmd +
                                              centerAlignTableStyleCmd + 
                                              boldFontTableStyleCmd + 
                                              visibleAllBordersTableStyleCmd ))


    mountPartsRecords = columnsHeaderTitles.copy()

    for mountPart in stand["MountParts"]:
        tableRecord = [mountPart["Name"], mountPart["Unit"], mountPart["Quantity"],"",""]
        mountPartsRecords.append(tableRecord)
    
    mountPartsTable = Table(data = mountPartsRecords, colWidths = [sheetWidth*0.3, sheetWidth*0.075, sheetWidth*0.075,sheetWidth*0.075,sheetWidth*0.075])
    mountPartsTable.setStyle(TableStyle(cmds =
                                        commonTableStyleCmd +
                                        centerAlignTableStyleCmd + 
                                        usualFontTableStyleCmd + 
                                        visibleAllBordersTableStyleCmd + 
                                        #шапка жирным
                                        [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))

    #таблица дренажа
    drainagePartsHeaderTable = Table(data = [["Дренаж и/или продувка"]], colWidths = sheetWidth * 0.4)
    drainagePartsHeaderTable.setStyle(TableStyle(cmds =
                                              commonTableStyleCmd +
                                              centerAlignTableStyleCmd + 
                                              boldFontTableStyleCmd + 
                                              visibleAllBordersTableStyleCmd ))

    drainagePartsRecords = columnsHeaderTitles.copy()
    drainagePartsRecords[0].pop()

    for mountPart in stand["MountParts"]:
        tableRecord = [mountPart["Name"], mountPart["Unit"], mountPart["Quantity"],""]
        drainagePartsRecords.append(tableRecord)
    
    drainagePartsTable = Table(data = drainagePartsRecords, colWidths = [sheetWidth*0.18, sheetWidth*0.086, sheetWidth*0.066,sheetWidth*0.066])
    drainagePartsTable.setStyle(TableStyle(cmds =
                                            commonTableStyleCmd +
                                            centerAlignTableStyleCmd + 
                                            usualFontTableStyleCmd + 
                                            visibleAllBordersTableStyleCmd + 
                                            #шапка жирным
                                            [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))


    #таблица электрическх компонентов
    electricPartsHeaderTable = Table(data = [["Электрические компоненты"]], colWidths = sheetWidth * 0.4)
    electricPartsHeaderTable.setStyle(TableStyle(cmds =
                                              commonTableStyleCmd +
                                              centerAlignTableStyleCmd + 
                                              boldFontTableStyleCmd + 
                                              visibleAllBordersTableStyleCmd ))

    electricPartsRecords = columnsHeaderTitles.copy()
    electricPartsRecords[0].pop()

    for mountPart in stand["MountParts"]:
        tableRecord = [mountPart["Name"], mountPart["Unit"], mountPart["Quantity"],""]
        electricPartsRecords.append(tableRecord)
    
    electricPartsTable = Table(data = electricPartsRecords, colWidths = [sheetWidth*0.18, sheetWidth*0.086, sheetWidth*0.066,sheetWidth*0.066])
    electricPartsTable.setStyle(TableStyle(cmds =
                                            commonTableStyleCmd +
                                            centerAlignTableStyleCmd + 
                                            usualFontTableStyleCmd + 
                                            visibleAllBordersTableStyleCmd + 
                                            #шапка жирным
                                            [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] ))


    #чертеж стенда
    imageString = stand["ImageData"]
    if imageString is not None: 
        standBlueprint = generateImageFromStr(imageString, sheetWidth*0.4, 200)  
    else:
        standBlueprint = Paragraph(text = "Ха-ха, пiймав на пикчу",style = cyrillic_style)

    leftPart = [standTechCardHeaderTable,
                standNameHeaderTable, 
                standInfoTable, 
                standSizeTable, 
                framesTable,
                framePartsHeaderTable, 
                framePartsTable, 
                mountPartsHeaderTable, 
                mountPartsTable]

    rightPart = [standBlueprint,
                 drainagePartsHeaderTable,
                 drainagePartsTable, 
                 #electricPartsHeaderTable,  
                 #electricPartsTable
                 ]

    sheetTable = Table(data = [[ leftPart, rightPart ]], colWidths = [sheetWidth * 0.6 , sheetWidth * 0.4])

    sheetTable.setStyle(TableStyle(cmds = 
                         commonTableStyleCmd +
                         centerAlignTableStyleCmd + 
                         boldFontTableStyleCmd))




    #собираем все объекты в массив и отдаем
    sheetElements = []   
    sheetElements.append(sheetTable)
    

    
        
    return sheetElements



def fillConclusionDataSheet(stand,doc,project):

    sheetWidth = A4[1]
    sheetHeight = A4[0]

    commonTableStyleCmd = [    
        ('BACKGROUND', (0, 0), (-1, 0), colors.white),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ('FONTSIZE', (0, 0), (-1, -1), 8)]

    leftAlignTableStyleCmd = [ ('ALIGN', (0, 0), (-1, -1), 'LEFT')]
    centerAlignTableStyleCmd = [ ('ALIGN', (0, 0), (-1, -1), 'CENTER')]
    usualFontTableStyleCmd = [('FONTNAME', (0, 0), (-1, -1), "Arial")]
    boldFontTableStyleCmd = [('FONTNAME', (0, 0), (-1, -1), "Arial-Bold")]
    visibleBordersTableStyleCmd = [('GRID', (0, 0), (-1, -1), 1, colors.black)]
    invisibleBordersTableStyleCmd = [('GRID', (0, 0), (-1, -1), 1, colors.white)]

    styles = getSampleStyleSheet()

    cyrillic_style = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 8
    )

    sheetElements = []



    #таблица с инфой о стенде и лого
    standTable = [["","Значение"]]
    standTable.append(["Наименование", "Стенд датчиков КИПиА"])
    standTable.append(["Обозначение по КД", str(stand["Designation"])])
    standTable.append(["Чертеж", "???"])
    standTable.append(["Зав.номер", str(stand["SerialNumber"])])     
    standInfoTable = Table(data = standTable, colWidths = [sheetWidth*0.2,sheetWidth*0.3])

    standInfoTable.setStyle(TableStyle(cmds = 
                                       commonTableStyleCmd +
                                       centerAlignTableStyleCmd + 
                                       visibleBordersTableStyleCmd +    
                                       #жирный шрифт для шапки 
                                       [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold"), 
                                        ('FONTNAME', (0, 0), (0, -1), "Arial-Bold")] +      
                                       #обычный шрифт для данных таблицы
                                       [('FONTNAME', (1, 1), (-1, -1), "Arial")] ))


    #магические размеры убрать!!!
    logoImage = generateLogo(sheetWidth * 0.18,sheetHeight * 0.15)


    standInfoAlignmentTable = Table(data = [[standInfoTable,logoImage]], 
                                    colWidths = [sheetWidth*0.52,sheetWidth*0.2])

    standInfoAlignmentTable.setStyle(TableStyle(cmds = 
                                                commonTableStyleCmd +
                                                centerAlignTableStyleCmd + 
                                                invisibleBordersTableStyleCmd ))

    #графа № заказа на производство
    orderNumberLabel = Paragraph("№ заказа на производство", style = cyrillic_style)
    emptyCell = Table(data = [[""]], colWidths = sheetWidth * 0.3)
    emptyCell.setStyle(TableStyle(cmds =
                                  commonTableStyleCmd +  
                                  visibleBordersTableStyleCmd))

    orderNumberAlignmentTable = Table(data = [[orderNumberLabel, emptyCell]], 
                                      colWidths = [sheetWidth*0.15,sheetWidth*0.75])

    orderNumberAlignmentTable.setStyle(TableStyle(cmds = 
                                                  commonTableStyleCmd +
                                                  leftAlignTableStyleCmd + 
                                                  invisibleBordersTableStyleCmd))

    #таблица исполнения этапов
    doneTable = [["№ п/п", 
                  "Наименование\n операции", 
                  "№ извещения о\n несоотвествии",
                  "Дата фактического\n выполнения\n операции",
                  "Ф.И.О. исполнителя",
                  "Подпись\n исполнителя", 
                  "№ протокола (ЛКП, ПСИ и т.д.)"]]


    doneTable.append(["1", "Сварочная","", "", "", "", ""])
    doneTable.append(["2", "Сборочная","", "", "", "", ""])
    doneTable.append(["3", "Подготовительно-окрасочная","", "", "", "", ""])
    doneTable.append(["4", "Сборочная (электрическая часть)","", "", "", "", ""])
    doneTable.append(["5", "Контрольная","", "", "", "", ""])
    doneTable.append(["", "","", "", "", "", ""])
    doneStagesTable = Table(data = doneTable,
                          colWidths =[sheetWidth*0.05,sheetWidth*0.2, sheetWidth*0.1, sheetWidth*0.125,sheetWidth*0.125, sheetWidth*0.1, sheetWidth*0.15],
                          rowHeights=[sheetHeight*0.08,sheetHeight*0.05,sheetHeight*0.05,sheetHeight*0.05,sheetHeight*0.05,sheetHeight*0.05,sheetHeight*0.05])



    doneStagesTable.setStyle(TableStyle(cmds = 
                                      commonTableStyleCmd +
                                      centerAlignTableStyleCmd + 
                                      visibleBordersTableStyleCmd +
                                      #жирный шрифт для шапки      
                                      [('FONTNAME', (0, 0), (-1, 0), "Arial-Bold")] + 
                                      #обычный шрифт для тела
                                      [('FONTNAME', (0, 1), (-1, -1), "Arial")] ))


    #графа подписей
    productReadyLabel = Paragraph(text = "Изделие признано годным и передано на склад", 
                                  style = cyrillic_style)
   

    signatureLabels = Table(data = [["ОТК (ФИО, подпись)","Склад (ФИО, подпись)"]],
                            colWidths = sheetWidth*0.2)

    signatureLabels.setStyle(TableStyle(cmds = 
                                        commonTableStyleCmd +
                                        centerAlignTableStyleCmd + 
                                        invisibleBordersTableStyleCmd +
                                        usualFontTableStyleCmd))

    signatureTable = Table(data = [["",""]],
                           colWidths = sheetWidth*0.2)
    signatureTable.setStyle(TableStyle(cmds = 
                                        commonTableStyleCmd +
                                        centerAlignTableStyleCmd + 
                                        visibleBordersTableStyleCmd ))

    allSignatureTable = [signatureLabels, signatureTable]

    signatureAligmentTable = Table(data = [[productReadyLabel, allSignatureTable]], 
                                   colWidths = [sheetWidth*0.25,sheetWidth*0.75])

    signatureAligmentTable.setStyle(TableStyle(cmds = 
                                               commonTableStyleCmd +
                                               leftAlignTableStyleCmd + 
                                               invisibleBordersTableStyleCmd +
                                               [("VALIGN", (0, 0), (-1, -1), "BOTTOM")] ))


    #собираем все элементы листа
    sheetElements.append(standInfoAlignmentTable)
    sheetElements.append(Spacer(1,10))
    sheetElements.append(orderNumberAlignmentTable)
    sheetElements.append(Spacer(1,10))
    sheetElements.append(doneStagesTable)
    sheetElements.append(Spacer(1,10))
    sheetElements.append(signatureAligmentTable)
    
    return sheetElements


def generateTechcard():

    outputDir = "C:/Work/Тестовая папка для отчётов/"

    now = datetime.now()
    outputFileName = "Технологические карты___"
    outputFileName += "{}-{}-{}___{}-{}-{}".format(now.day,now.month,now.year,now.hour,now.minute,now.second)
    outputFileName += ".pdf"
    
    output_pdf = outputDir + outputFileName

    data = openJsonFile()
    doc = SimpleDocTemplate(output_pdf, pagesize=A4)

    elements = []

    #добавляем стили страницы
    doc.addPageTemplates([portraitTemplate,landscapeTemplate])

    for stand in data["Stands"]:      
        standSheet = fillStandDataSheet(stand,doc,data)
        elements.extend(standSheet)  
        elements.append(NextPageTemplate('landscape'))
        elements.append(PageBreak())
        conclusionSheet = fillConclusionDataSheet(stand,doc,data)
        elements.extend(conclusionSheet)
        elements.append(NextPageTemplate('portrait'))
        elements.append(PageBreak())

    doc.build(elements)

    
if __name__ == "__main__":
    generateTechcard()
