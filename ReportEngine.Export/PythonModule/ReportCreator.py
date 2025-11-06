from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import (SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, tables)
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont
from reportlab.pdfgen import canvas
from reportlab.pdfbase.ttfonts import TTFont
import json
import os
from datetime import datetime
from pathlib import Path

pdfmetrics.registerFont(UnicodeCIDFont("HeiseiMin-W3"))

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


def fillStandList(stand,doc):

    framesData = [["Рама, мм", "Обозначение по КД", "Кол-во, шт"]]

    for frame in stand["Frames"]:

        frameArray = [frame["Width"], frame["DocName"], frame["Quantity"]]
        framesData.append(frameArray)

    framesTable =  Table(framesData, colWidths=[150,250,150])

    framesTable.setStyle(TableStyle([

        ('BACKGROUND', (0, 0), (-1, 0), colors.white),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
        ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
        ('FONTNAME', (0, 0), (-1, 0), "HeiseiMin-W3"),
        ('FONTSIZE', (0, 0), (-1, 0), 12),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 12), 
        ('GRID', (0, 0), (-1, -1), 1, colors.black),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE")
    ]))

    return framesTable


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
        framesTable = fillStandList(stand,doc)
        elements.append(framesTable)
        elements.append(Spacer(1, 20))

    elements.append(Spacer(1, 100))
    elements.append(Paragraph("Изделие признано говном и выброшено на парашу"))
    elements.append(Spacer(1, 40))
    elements.append(Paragraph("ОТК (ФИО, подпись) _________________________"))
    elements.append(Paragraph("Склад (ФИО, подпись) _________________________"))


    print(elements)
      
    doc.build(elements)

    


if __name__ == "__main__":
    generate_empty_techcard()
