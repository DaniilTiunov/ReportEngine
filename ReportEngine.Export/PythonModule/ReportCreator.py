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

    framesData = [["FrameSize", "DocDesignation", "Quantity"]]

    for frame in stand["Frames"]:

        frameArray = [frame["Width"], frame["DocName"], frame["Quantity"]]
        framesData.append(frameArray)

    #print(framesData)
    framesTable =  Table(framesData, colWidths=[50,50,50,50])

    return framesTable


def generate_empty_techcard(output_pdf="techcard_template.pdf"):
    data = openJsonFile()
    doc = SimpleDocTemplate(output_pdf, pagesize=A4)
    
    
    elements = []

    for stand in data["Stands"]:
      framesTable = fillStandList(stand,doc)
      elements.append(framesTable)

    
        
    

    pdfmetrics.registerFont(TTFont('Arial', 'Arial.ttf'))

    doc.build(elements)

    docCanvas = canvas.Canvas("zhopa.pdf")
    docCanvas.setFont("Arial",12)
    docCanvas.drawString(100,750, "Hello world!")
    docCanvas.save()

    


if __name__ == "__main__":
    generate_empty_techcard()
