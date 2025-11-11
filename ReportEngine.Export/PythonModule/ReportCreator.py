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

listWidth = 210 * cm
listHeight = 297 * cm

pdfmetrics.registerFont(TTFont('Arial','arial.ttf'))
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

def openJsonFile():
    
    script_dir = Path(__file__).parent
    file_path = os.path.join(script_dir, "TechnologicalCards_temp.json")
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as file:
            jsonData = json.load(file)           
    except Exception as e:
            print(f"Error: {e}")

    return jsonData


def generateImageFromFile():
    script_dir = Path(__file__).parent
    file_path = os.path.join(script_dir, "Etalon.jpg")
    return Image(file_path,width=100,height=50)
    

def generateImageFromStr(base64_string):
    imageData = base64.b64decode(base64_string)
    imageBuffer = io.BytesIO(imageData)
    return Image(imageBuffer)



def fillStandDataSheet(stand,doc,project):

    tableWidth = 500

    commonTableStyle = TableStyle([

        ('BACKGROUND', (0, 0), (-1, 0), colors.white),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
        ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
        ('FONTNAME', (0, 0), (-1, -1), "Arial"),
        ('FONTSIZE', (0, 0), (-1, 0), 12),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 12), 
        ('GRID', (0, 0), (-1, -1), 1, colors.black),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE")
    ])

    galvanizeStr = "Оцинковка" if project["IsGalvanized"] else "Покраска"
    standTechCardTitle = [["Технологическая карта" + "            " + str(galvanizeStr) + "           " + str(project["Description"])]]

    standNameTitle = [[ "Стенд датчиков КИПиА " + str(stand["Designation"]) ]]
    standInfoTitle = [[ str(stand["KKSCode"]), str(stand["SerialNumber"]) ]]
    frameSizeTitle = [["Размер стенда, мм          " + str(stand["Width"])]]
    framesData = [["Рама, мм", "Обозначение по КД", "Кол-во, шт", "","",""]]

    for frame in stand["Frames"]:
        frameArray = [frame["Width"], frame["DocName"], frame["Quantity"],"","",""]
        framesData.append(frameArray)


    columnsHeaderTitles = [["Наименование", "Единицы \n измерения", "Норм.","Факт.", ""]]

    
    #заполняем таблицы
    framePartsArray = []
    framePartsArray = columnsHeaderTitles.copy()

    for frameMaterial in stand["FrameParts"]:
        tableRecord = [frameMaterial["Name"], frameMaterial["Unit"], frameMaterial["Quantity"],"",""]
        framePartsArray.append(tableRecord)

    mountPartsArray = []
    mountPartsArray = columnsHeaderTitles.copy()

    for mountPart in stand["MountParts"]:
        tableRecord = [mountPart["Name"], mountPart["Unit"], mountPart["Quantity"],"",""]
        mountPartsArray.append(tableRecord)

    drainagePartsArray = []
    drainagePartsArray = columnsHeaderTitles.copy()

    for drainagePart in stand["DrainageParts"]:
        tableRecord = [drainagePart["Name"], drainagePart["Unit"], drainagePart["Quantity"],"",""]
        drainagePartsArray.append(tableRecord)

    electricPartsArray = []
    electricPartsArray = columnsHeaderTitles.copy()

    for electricPart in stand["ElectricParts"]:
        tableRecord = [electricPart["Name"], electricPart["Unit"], electricPart["Quantity"],"",""] 
        electricPartsArray.append(tableRecord)

    #создаем объекты
    standTechCardHeader = Table(data = standTechCardTitle, colWidths = tableWidth)
    standTechCardHeader.setStyle(commonTableStyle)

    standNameHeader = Table(data = standNameTitle, colWidths = tableWidth)
    standNameHeader.setStyle(commonTableStyle)

    standInfoHeader = Table(data = standInfoTitle, colWidths = [tableWidth * 0.5, tableWidth * 0.5])
    standInfoHeader.setStyle(commonTableStyle)

    frameSizeHeader = Table(data = frameSizeTitle, colWidths = tableWidth)
    frameSizeHeader.setStyle(commonTableStyle)
    
    framesTable = Table(data = framesData, colWidths=[tableWidth*0.15,tableWidth*0.25,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15])
    framesTable.setStyle(commonTableStyle)

    frameMaterialsHeader = Table(data = [["Основные материалы рамы стенда"]], colWidths = tableWidth)
    frameMaterialsHeader.setStyle(commonTableStyle)

    frameMaterialsData = Table(data = framePartsArray, colWidths = [tableWidth*0.4,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15])
    frameMaterialsData.setStyle(commonTableStyle)

    mountPartsHeader = Table(data = [["Комплект монтажных частей в зависимости от обвязок"]], colWidths = tableWidth)
    mountPartsHeader.setStyle(commonTableStyle)

    mountPartsData = Table(data = mountPartsArray, colWidths = [tableWidth*0.4,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15])
    mountPartsData.setStyle(commonTableStyle)

    drainagePartsHeader = Table(data = [["Дренаж и/или продувка"]], colWidths = tableWidth)
    drainagePartsHeader.setStyle(commonTableStyle)

    drainagePartsData = Table(data = drainagePartsArray, colWidths = [tableWidth*0.4,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15])
    drainagePartsData.setStyle(commonTableStyle)

    electricPartsHeader = Table(data = [["Электрические компоненты"]], colWidths = tableWidth)
    electricPartsHeader.setStyle(commonTableStyle)

    electricPartsData = Table(data = electricPartsArray, colWidths = [tableWidth*0.4,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15])
    electricPartsData.setStyle(commonTableStyle)


    #собираем все объекты в массив и отдаем
    sheetElements = []   
    sheetElements.append(standTechCardHeader)
    sheetElements.append(standNameHeader)
    sheetElements.append(standInfoHeader)
    sheetElements.append(frameSizeHeader)
    sheetElements.append(framesTable)
    sheetElements.append(frameMaterialsHeader)
    sheetElements.append(frameMaterialsData)
    sheetElements.append(mountPartsHeader)
    sheetElements.append(mountPartsData)
    sheetElements.append(drainagePartsHeader)
    sheetElements.append(drainagePartsData)
    sheetElements.append(electricPartsHeader)
    sheetElements.append(electricPartsData)

    imageString = stand["ImageData"]
    if imageString is not None: 
        standBlueprint = generateImageFromStr(imageString)
        standBlueprint.drawHeight = 200  # высота в пунктах
        standBlueprint.drawWidth = 200   # ширина в пунктах
        sheetElements.append(standBlueprint)
        
    return sheetElements



def fillConclusionDataSheet(stand,doc,project):

    tableWidth = 500

    commonTableStyle = TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.white),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
        ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
        ('FONTNAME', (0, 0), (-1, -1), "Arial"),
        ('FONTSIZE', (0, 0), (-1, 0), 12),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 12), 
        ('GRID', (0, 0), (-1, -1), 1, colors.black),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE")
    ])

    invisibleTableStyle = TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.white),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
        ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
        ('FONTNAME', (0, 0), (-1, -1), "Arial"),
        ('FONTSIZE', (0, 0), (-1, 0), 12),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 12), 
        ('GRID', (0, 0), (-1, -1), 1, colors.white),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE")
    ])

    styles = getSampleStyleSheet()

    cyrillic_style = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8'
    )

    sheetElements = []

    standTable = [["","Значение"]]
    standTable.append(["Наименование", "Стенд датчиков КИПиА"])
    standTable.append(["Обозначение по КД", str(stand["Designation"])])
    standTable.append(["Чертеж", "???"])
    standTable.append(["Зав.номер", str(stand["SerialNumber"])])     
    standInfoTable = Table(data = standTable, colWidths = [tableWidth*0.2,tableWidth*0.5])
    standInfoTable.setStyle(commonTableStyle)

    logoImage = generateImageFromFile()

    standInfoAlignmentTable = Table([[standInfoTable,logoImage]], colWidths = [tableWidth*0.8,tableWidth*0.2])
    standInfoAlignmentTable.setStyle(invisibleTableStyle)


    orderNumberLabel = Paragraph("№ заказа на производство", style = cyrillic_style)
    emptyCell = Table(data = [[""]], colWidths = 200)
    emptyCell.setStyle(commonTableStyle)

    orderNumberAlignmentTable = Table([[orderNumberLabel,emptyCell]], colWidths = [tableWidth*0.2,tableWidth*0.8])
    orderNumberAlignmentTable.setStyle(invisibleTableStyle)

    doneTable = [["№ п/п", 
                  "Наименование операции", 
                  "№ извещения о несоотвествии",
                  "Дата фактического выполнения операции",
                  "Ф.И.О. исполнителя",
                  "Подпись исполнителя", 
                  "№ протокола (ЛКП, ПСИ и т.д.)"]]


    doneTable.append(["1", "Сварочная","", "", "", "", ""])
    doneTable.append(["2", "Сборочная","", "", "", "", ""])
    doneTable.append(["3", "Подготовительно-окрасочная","", "", "", "", ""])
    doneTable.append(["4", "Сборочная (электрическая часть)","", "", "", "", ""])
    doneTable.append(["5", "Контрольная","", "", "", "", ""])
    doneTable.append(["", "","", "", "", "", ""])
    doneTableInfo = Table(data = doneTable, colWidths = [tableWidth*0.1,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15,tableWidth*0.15,tableWidth*0.1,tableWidth*0.1])
    doneTableInfo.setStyle(commonTableStyle)

    signatureTable = Table(data = [["",""]],colWidths = 200)
    signatureTable.setStyle(commonTableStyle)

    sheetElements.append(standInfoAlignmentTable)
    sheetElements.append(Spacer(1,10))
    sheetElements.append(orderNumberAlignmentTable)
    sheetElements.append(Spacer(1,10))
    sheetElements.append(doneTableInfo)
    sheetElements.append(Spacer(1,12))
    sheetElements.append(Paragraph("Изделие признано годным и передано на склад", style = cyrillic_style))
    sheetElements.append(Spacer(1,12))
    sheetElements.append(Paragraph("ОТК (ФИО, подпись)                      Склад (ФИО, подпись)", style = cyrillic_style))
    sheetElements.append(signatureTable)
    
    return sheetElements

def generate_empty_techcard():

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
    doc.addPageTemplates([landscapeTemplate,portraitTemplate])

    for stand in data["Stands"]:
        elements.append(NextPageTemplate('portrait'))
        standSheet = fillStandDataSheet(stand,doc,data)
        elements.extend(standSheet)       
        elements.append(PageBreak())
        elements.append(NextPageTemplate('landscape'))
        conclusionSheet = fillConclusionDataSheet(stand,doc,data)
        elements.extend(conclusionSheet)
        elements.append(PageBreak())

    doc.build(elements)

    

if __name__ == "__main__":
    generate_empty_techcard()
