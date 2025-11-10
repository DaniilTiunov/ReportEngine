from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import (SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, frames, tables)
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont
from reportlab.pdfgen import canvas
from reportlab.pdfbase.ttfonts import TTFont
import json
import os
from datetime import datetime
from pathlib import Path



pdfmetrics.registerFont(TTFont('Arial','arial.ttf'))
pdfmetrics.registerFont(UnicodeCIDFont('STSong-Light'))





def openJsonFile():
    
    script_dir = Path(__file__).parent
    file_path = os.path.join(script_dir, "TechnologicalCards_temp.json")
    print(file_path)
    print("File exist",os.path.exists(file_path))

    try:
        with open(file_path, 'r', encoding='utf-8-sig') as file:
            jsonData = json.load(file)           
    except Exception as e:
            print(f"Error: {e}")

    return jsonData


def fillStandList(stand,doc,project):

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
    standTable = []   
    standTable.append(standTechCardHeader)
    standTable.append(standNameHeader)
    standTable.append(standInfoHeader)
    standTable.append(frameSizeHeader)
    standTable.append(framesTable)
    standTable.append(frameMaterialsHeader)
    standTable.append(frameMaterialsData)
    standTable.append(mountPartsHeader)
    standTable.append(mountPartsData)
    standTable.append(drainagePartsHeader)
    standTable.append(drainagePartsData)
    standTable.append(electricPartsHeader)
    standTable.append(electricPartsData)

    return standTable


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

    for stand in data["Stands"]:
        standTable = fillStandList(stand,doc,data)
        elements.extend(standTable)
        elements.append(PageBreak())

    styles = getSampleStyleSheet()

    usual_text_style = ParagraphStyle(
        'UsualCyrillicStyle',
        parent=styles['Normal'],
        fontName='Arial',
        encoding='UTF-8'
    )

    bold_text_style = ParagraphStyle(
        'BoldCyrillicStyle',
        parent=styles['Normal'],
        fontName='Arial', 
        encoding='UTF-8'
    )

    #print(elements)
      
    doc.build(elements)

    


if __name__ == "__main__":
    generate_empty_techcard()
