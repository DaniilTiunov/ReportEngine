import json
import base64
import io
import os
from pathlib import Path
from datetime import datetime
from reportlab.platypus import Image
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.lib import colors

#стили
commonTableStyleCmd = [    
        ('BACKGROUND', (0, 0), (-1, 0), colors.white),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ('FONTSIZE', (0, 0), (-1, -1), 7)]

leftAlignTableStyleCmd = [ ('ALIGN', (0, 0), (-1, -1), 'LEFT')]
centerAlignTableStyleCmd = [ ('ALIGN', (0, 0), (-1, -1), 'CENTER')]

usualFontTableStyleCmd = [('FONTNAME', (0, 0), (-1, -1), "Arial")]
boldFontTableStyleCmd = [('FONTNAME', (0, 0), (-1, -1), "Arial-Bold")]

visibleAllBordersTableStyleCmd = [('GRID', (0, 0), (-1, -1), 1, colors.black)]
invisibleAllBordersTableStyleCmd = []

invisibleOuterBordersTableStyleCmd = []
visibleOuterBordersTableStyleCmd = [('BOX', (0, 0), (-1, -1), 1, colors.black)]

invisibleInnerBordersTableStyleCmd = []
visibleInnerBordersTableStyleCmd = [('INNERGRID', (0, 0), (-1, -1), 1, colors.black)]




#функции
def openJsonFile(filePath):
    
    try:
        with open(filePath, 'r', encoding='utf-8-sig') as file:
            jsonData = json.load(file)           
    except Exception as e:
            print(f"Error: {e}")

    return jsonData


def generateImageFromFile(fileName, width, height):
    scriptDir = Path(__file__).parent
    filePath = os.path.join(scriptDir, fileName)
    return Image(filePath, width, height)
    

def generateImageFromStr(base64string, width, height):
    imageData = base64.b64decode(base64string)
    imageBuffer = io.BytesIO(imageData)
    return Image(imageBuffer, width, height)

def generateReportName(reportName):
    now = datetime.now()
    resultFileName = f"{reportName}___{now.strftime('%d-%m-%Y___%H-%M-%S')}.pdf"
    return resultFileName

def registerFonts():
    pdfmetrics.registerFont(TTFont('Arial','arial.ttf'))
    pdfmetrics.registerFont(TTFont('Arial-Bold','arialbd.ttf'))
    pdfmetrics.registerFont(UnicodeCIDFont('STSong-Light'))

