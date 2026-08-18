import json
import base64
import io
import os
from pathlib import Path
from datetime import datetime
from reportlab.platypus import Image as ReportLabImage
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.lib import colors
from PIL import Image as PILImage

#стили
commonTableStyleCmd = [    
        ('BACKGROUND', (0, 0), (-1, 0), colors.white),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ('FONTSIZE', (0, 0), (-1, -1), 7)]

leftAlignTableStyleCmd = [ ('ALIGN', (0, 0), (-1, -1), 'LEFT')]
centerAlignTableStyleCmd = [ ('ALIGN', (0, 0), (-1, -1), 'CENTER')]
firstColumnLeftTableStyleCmd = [('ALIGN', (0, 1), (0, -1), 'LEFT')]

usualFontTableStyleCmd = [('FONTNAME', (0, 0), (-1, -1), "Arial")]
boldFontTableStyleCmd = [('FONTNAME', (0, 0), (-1, -1), "Arial-Bold")]

visibleAllBordersTableStyleCmd = [('GRID', (0, 0), (-1, -1), 1, colors.black)]
invisibleAllBordersTableStyleCmd = []

invisibleOuterBordersTableStyleCmd = []
visibleOuterBordersTableStyleCmd = [('BOX', (0, 0), (-1, -1), 1, colors.black)]

invisibleInnerBordersTableStyleCmd = []
visibleInnerBordersTableStyleCmd = [('INNERGRID', (0, 0), (-1, -1), 1, colors.black)]



newLineMark = "<br/>"

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
    return ReportLabImage(filePath, width, height)
    

def generateImageFromStr(base64string, width, height):
    imageData = base64.b64decode(base64string)
    imageBuffer = io.BytesIO(imageData)
    return ReportLabImage(imageBuffer, width, height), 


def getImageOriginalSizes(base64string):
    image_data = base64.b64decode(base64string)
    image_buffer = io.BytesIO(image_data)
    with PILImage.open(image_buffer) as img:
        w,h = img.size
        return w,h 

def scaleImageToFit(originalWidth,originalHeight,targetWidth,targetHeight):
    widthScaleCoef = targetWidth / originalWidth
    heightScaleCoef = targetHeight / originalHeight

    resultScaleCoef = min(widthScaleCoef,heightScaleCoef)

    newWidht = originalWidth * resultScaleCoef
    newHeight = originalHeight * resultScaleCoef

    return newWidht, newHeight, resultScaleCoef




def generateReportName(reportName):
    now = datetime.now()
    resultFileName = f"{reportName}___{now.strftime('%d-%m-%Y___%H-%M-%S')}.pdf"
    return resultFileName

def registerFonts():
    pdfmetrics.registerFont(TTFont('Arial','arial.ttf'))
    pdfmetrics.registerFont(TTFont('Arial-Bold','arialbd.ttf'))
    pdfmetrics.registerFont(UnicodeCIDFont('STSong-Light'))


def to_str(value):
    return str(value) if value is not None else ""



def calculate_element_height(element, width):
    #Вычисляет реальную высоту элемента
    try:
        w, h = element.wrap(width, 0)
        return h
    except:
        # Если элемент не поддерживает wrap
        return 0


def get_column_height(elements, column_width):
    #Вычисляет суммарную высоту колонки
    total = 0
    for element in elements:
        if isinstance(element, list):
            # Рекурсивно для вложенных списков
            for sub_element in element:
                total += calculate_element_height(sub_element, column_width)
        else:
            total += calculate_element_height(element, column_width)
    return total
