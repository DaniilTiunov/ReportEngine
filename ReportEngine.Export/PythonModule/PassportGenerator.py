from reportlab.lib.pagesizes import A4, landscape,portrait
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import ( PageTemplate, SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, NextPageTemplate, Frame)
from reportlab.lib.units import mm
from reportlab.lib import colors
from reportlab.lib.enums import TA_LEFT, TA_CENTER, TA_RIGHT, TA_JUSTIFY
import PdfHelper



portraitParams = {
    "startPointX" : 20 * mm,
    "startPointY": 20 * mm,
    "frameWidth":A4[0] - 40*mm,
    "frameHeight": A4[1] - 40*mm,
    "frameId": 'portraitFrame',
    "visibleBoundaries": 0
}

landscapeParams = {
    "startPointX" : 5 * mm,
    "startPointY": 5 * mm,
    "frameWidth":A4[1] - 10*mm,
    "frameHeight": A4[0] - 10*mm,
    "frameId": 'landscapeFrame',
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


def CreateSignatureTable(aboveLabel,underLabel):
    
    createdTable = Table(data = [[aboveLabel], [underLabel]])
    createdTable.setStyle(TableStyle(cmds = 
                                       PdfHelper.commonTableStyleCmd + 
                                       PdfHelper.visibleInnerBordersTableStyleCmd + 
                                       PdfHelper.invisibleOuterBordersTableStyleCmd + 
                                       PdfHelper.usualFontTableStyleCmd +
                                       PdfHelper.centerAlignTableStyleCmd +
                                       [("VALIGN", (0, 0), (0, 0), "BOTTOM")] +
                                       [("VALIGN", (-1, -1), (-1, -1), "TOP")] +
                                       [("BOTTOMPADDING", (0, 0), (0, 0), 0)] +
                                       [("TOPPADDING", (-1, -1), (-1, -1), 1)] + 
                                       [("FONTSIZE", (0, 0), (0, 0), 7)] + 
                                       [("FONTSIZE", (-1, -1), (-1, -1), 5 )] ))

    return createdTable



def fillTitlePage(stand,doc,project):
    
    frameWidth = landscapeParams['frameWidth'] * 0.99
    frameHeight = landscapeParams['frameHeight'] * 0.97
 
    styles = getSampleStyleSheet()

    usualStyle = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 7
    )

    titleStyle = ParagraphStyle(
        'Normal',
        parent = styles['Normal'],
        fontName ='Arial-Bold',
        encoding ='UTF-8',
        fontSize = 16,
        alignment = TA_CENTER,
        leading=20
    )

    eacImage =  PdfHelper.generateImageFromFile("EAC.jpg", frameWidth*0.06, frameWidth*0.06)
    logoImage = PdfHelper.generateImageFromFile("Etalon.jpg", frameWidth*0.28, frameHeight*0.25)
    mainTitle = Paragraph(text = "Стенд датчиков КИПиА" + "<br/>" +  stand["KKSCode"] + "<br/>" +"ПАСПОРТ", 
                          style = titleStyle)

    pageTable = Table(data = [ ["", eacImage ],
                               ["", logoImage],
                               ["", mainTitle] ], 
                      colWidths = frameWidth/ 2,  
                      rowHeights = [frameHeight * 0.1, frameHeight * 0.15, frameHeight * 0.75]  )

    pageTable.setStyle(TableStyle(cmds = 
                                  PdfHelper.commonTableStyleCmd +
                                  PdfHelper.invisibleOuterBordersTableStyleCmd + 
                                  #границы между левой и правой колонкой - пунктиром
                                  [('LINEBEFORE', (-1, 0), (-1, -1), 1, colors.black, None, (2, 2) )] + 
                                  #левую часть объединяем
                                  [('SPAN', (0, 0), (0, -1) )] +
                                  #выравниваем лого EAC
                                  [('LEFTPADDING', (-1, 0), (-1, 0), 20)] + 
                                  [('ALIGN', (-1, 0), (-1, 0), 'LEFT')] + 
                                  [("VALIGN", (-1, 0), (-1, 0), "CENTER")] + 
                                  #выравниваем лого Etalon
                                  [('ALIGN', (-1, 1), (-1, 1), 'CENTER')] + 
                                  [("VALIGN", (-1, 1), (-1, 1), "MIDDLE")] +
                                  #выравниваем заголовок
                                  [('ALIGN', (-1, -1), (-1, -1), 'CENTER')] + 
                                  [("VALIGN", (-1, -1), (-1, -1), "MIDDLE")] ))
    
    #собираем все объекты в массив и отдаем
    sheetElements = []
    sheetElements.append(pageTable)
           
    return sheetElements


def fillBodyPage(stand,doc,project):

    frameWidth = landscapeParams['frameWidth'] * 0.99
    frameHeight = landscapeParams['frameHeight'] * 0.97
 
    styles = getSampleStyleSheet()

    usualStyle = ParagraphStyle(
        name = 'Normal',
        parent = styles['Normal'],
        fontName ='Arial',
        encoding ='UTF-8',
        fontSize = 8,
        firstLineIndent = 12,
        spaceAfter = 0.5
    )

    titleStyle = ParagraphStyle(
        name = 'Title',
        parent = styles['Normal'],
        fontName ='Arial-Bold',
        encoding ='UTF-8',
        fontSize = 8,
        spaceAfter = 6,
        alignment = TA_CENTER
    )

    noteStyle = ParagraphStyle(
        name = 'Note',
        parent = styles['Normal'],
        fontName ='Arial-Bold',
        encoding ='UTF-8',
        fontSize = 6,
        firstLineIndent = 12        
    )

    leftPartContent = [ Paragraph(text = "1. Основные сведения об изделии и и технические данные", 
                                  style = titleStyle) ]   
    leftPartContent.append(Paragraph(text = "1.1 Стенд датчиков КИПиА предназначены для установки на них манометрических и дифманометрических приборов, " +
                                            "контролирующих параметры технологических установок и участвующих в автоматизированном управлении технологическим процессом " +
                                            "на давление до 37 МПа (370 кгс/см2).", 
                                     style = usualStyle))
    leftPartContent.append(Paragraph(text = "1.2 Стенд датчиков имеет вид климатического исполнения и категорию УХЛ3.1 по ГОСТ-15150 и " + 
                                            "предназначен для эксплуатации в закрытых помещениях при температуре окружающего воздуха от плюс 5 до плюс 50 °С.",
                                     style = usualStyle))   
    leftPartContent.append(Paragraph(text = "1.3 Стенд датчиков соответствует требованиям ТУ 4200-012-45633145-2016 и комплекту конструкторской документации.",
                                     style = usualStyle))   
    leftPartContent.append(Paragraph(text = "1.4 Изготовитель: ЗАО «ЭТАЛОН-ПРИБОР», 454138, г. Челябинск, пр. Победы, 288" ,
                                     style = usualStyle))   
    leftPartContent.append(Paragraph(text = "1.5 Декларация соответствия требованиям ТР ТС 020/2011. Регистрационный номер ЕАЭС N RU Д-RU.PA01.B.94296/21. " +
                                     "Срок действия по 29.06.2024 включительно.",
                                     style = usualStyle))    
    leftPartContent.append(Paragraph(text = "1.6 Качество продукции обеспечено сертифицированной системой менеджмента качества, " + 
                                            "соответствующей требованиям ГОСТ ISO 9001-2011 (ISO 9001:2008).",
                                     style = usualStyle))    

    dimensions = (str(stand["Frames"][0]["Width"]), str(stand["Frames"][0]["Height"]), str(stand["Frames"][0]["Depth"])) if len(stand["Frames"])>0 else ("____","____","____")

    leftPartContent.append(Paragraph(text = "1.7 Основные параметры изделия: " + PdfHelper.newLineMark + 
                                            f"- импульсные линии в пределах стенда - {stand["MaterialLine"]} в соответствии с опросным листом;" + PdfHelper.newLineMark +   
                                            f"- {stand["Armature"]};" + PdfHelper.newLineMark +
                                            f"- габаритные размеры стенда (ШхВхГ), мм - {dimensions[0]}х{dimensions[1]}х{dimensions[2]}.",
                                     style = usualStyle))
    leftPartContent.append(Spacer(1,5))

    leftPartContent.append(Paragraph(text = "2. Комлектность", 
                                    style = titleStyle))
    leftPartContent.append(Paragraph(text = "2.1 В комплект поставки входят:" + PdfHelper.newLineMark +
                                            "- стенд датчиков в сборе 1 шт.;" + PdfHelper.newLineMark +
                                            "- паспорт изделия 1 шт.;" + PdfHelper.newLineMark +
                                            "- сертификаты качества на запорную арматуру и материалы импульсных линий - 1 компл. (на партию в один адрес).",
                                     style = usualStyle))
    leftPartContent.append(Paragraph(text = "Примечание - датчики, манометры, вентильные блоки в комплект поставки стенда не входят.", 
                                     style = noteStyle))
    leftPartContent.append(Spacer(1,3))

    leftPartContent.append(Paragraph(text = "3. Ресурсы, сроки службы, хранения и гарантии изготовителя", 
                                  style = titleStyle))
    leftPartContent.append(Paragraph(text = "3.1 Стенд датчиков в упаковке предприятия-изготовителя должен храниться в закрытых помещениях с естественной вентиляцией " + 
                                            "без искусственно регулируемых климатических условий, расположенных в макроклиматических районах с умеренным и холодным климатом, " + 
                                            "колебания температуры и влажности существенно меньше, чем на открытом воздухе.",
                                    style = usualStyle))
    leftPartContent.append(Paragraph(text = "3.2 Стенд датчиков в упаковке предприятия-изготовителя транспортируется любым видом крытого транспорта " +
                                            "с обеспечением защиты от механических повреждений. Условия транспортирования системы – С (средние) по ГОСТ-23170.",
                                    style = usualStyle))
    leftPartContent.append(Paragraph(text = "3.3 Изготовитель гарантирует соответствие изделия требованиям ТУ 4200-012-45633145-2016 " + 
                                            "при соблюдении потребителем условий транспортирования, хранения, монтажа и эксплуатации",
                                    style = usualStyle))
    leftPartContent.append(Paragraph(text = "3.4 Гарантийный срок эксплуатации изделия – 24 месяца со дня ввода в эксплуатацию, " + 
                                            "но не более 36 месяцев со дня отгрузки потребителю.",
                                    style = usualStyle))




    
    standInfoRowTable = Table(data = [[ CreateSignatureTable("Стенд датчиков", "Наименование изделия"),
                                        CreateSignatureTable(stand["KKSCode"], "Обозначение"),
                                        CreateSignatureTable(stand["SerialNumber"], "Заводской номер") ]] )
    standInfoRowTable.setStyle(TableStyle(cmds = PdfHelper.centerAlignTableStyleCmd))

    productionInfoRowTable = Table(data = [[ Paragraph(text = "изготовлен", style = usualStyle),
                                             CreateSignatureTable("         ", "Месяц, год"),
                                             Paragraph(text = "согласно", style = usualStyle),
                                             CreateSignatureTable("             ", "№ опросного листа, № спецификации, РД") ]] )
    productionInfoRowTable.setStyle(TableStyle(cmds = PdfHelper.centerAlignTableStyleCmd +
                                          [('VALIGN', (0, 0), (-1, -1), "TOP" )] ))

    productionSupervisorRowTable = Table(data = [[ CreateSignatureTable("Начальник производственного участка", "Должность"),
                                                   CreateSignatureTable("                   ", "Подпись"),
                                                   CreateSignatureTable(str(project["SeniorEngineer"]), "Расшифровка подписи") ]] )

    productionSupervisorRowTable.setStyle(TableStyle(cmds = PdfHelper.centerAlignTableStyleCmd ))

    acceptanceSupervisorRowTable = Table(data = [[ CreateSignatureTable("Представитель ОТК", "Должность"),
                                                CreateSignatureTable("                   ", "Подпись"),
                                                CreateSignatureTable(str(project["ResponsibleForAccept"]), "Расшифровка подписи") ]] )

    acceptanceSupervisorRowTable.setStyle(TableStyle(cmds = PdfHelper.centerAlignTableStyleCmd ))

    visualMeasuringSupervisorRowTable = Table(data = [[ CreateSignatureTable("Представитель ОТК", "Должность"),
                                                CreateSignatureTable("                   ", "Подпись"),
                                                CreateSignatureTable(str(project["SecondLevelSpecialist"]), "Расшифровка подписи") ]] )

    visualMeasuringSupervisorRowTable.setStyle(TableStyle(cmds = PdfHelper.centerAlignTableStyleCmd ))

    packagingSupervisorRowTable = Table(data = [[ CreateSignatureTable("Представитель ОСиЛ", "Должность"),
                                                  CreateSignatureTable("                   ", "Подпись"),
                                                  CreateSignatureTable(str(project["OSiL"]), "Расшифровка подписи") ]] )

    packagingSupervisorRowTable.setStyle(TableStyle(cmds = PdfHelper.centerAlignTableStyleCmd ))


    rightPartContent = [Paragraph(text = "4. Свидетельство об изготовлении", style = titleStyle) ]
    rightPartContent.append(standInfoRowTable)
    rightPartContent.append(productionInfoRowTable)
    rightPartContent.append(productionSupervisorRowTable)

    rightPartContent.append(Paragraph(text = "5. Свидетельство об приемке", style = titleStyle ))
    rightPartContent.append(standInfoRowTable)
    rightPartContent.append(Paragraph(text = "соответствует требованиям ТУ 4200-012-45633145-2016 и признан годным к эксплуатации.", style = usualStyle ))
    rightPartContent.append(acceptanceSupervisorRowTable)
    
    rightPartContent.append(Paragraph(text = "6. Свидетельство о визуальном и измерительном контроле", style = titleStyle ))
    rightPartContent.append(standInfoRowTable)
    rightPartContent.append(Paragraph(text = "контроль проведен в соответствии с ЭП-С.121.30.00, АКТ N 25-001", style = usualStyle ))
    rightPartContent.append(visualMeasuringSupervisorRowTable)

    rightPartContent.append(Paragraph(text = "7. Свидетельство об упаковывании", style = titleStyle))
    rightPartContent.append(standInfoRowTable)
    rightPartContent.append(Paragraph(text = "упакован с соответствии с требованиями ТУ 4200-012-45633145-2016 и ГОСТ 23170-78.", style = usualStyle ))
    rightPartContent.append(packagingSupervisorRowTable)



    pageTable = Table(data = [[leftPartContent, rightPartContent]], colWidths = frameWidth / 2, rowHeights = frameHeight)
    pageTable.setStyle(TableStyle(cmds = 
                                 PdfHelper.commonTableStyleCmd +
                                 PdfHelper.invisibleOuterBordersTableStyleCmd +           
                                 PdfHelper.centerAlignTableStyleCmd +
                                 #границы между левой и правой колонкой - пунктиром
                                 [('LINEBEFORE', (-1, 0), (-1, -1), 1, colors.black, None, (2, 2) )] + 
                                 [('VALIGN', (0, 0), (-1, -1), "TOP" )]   ))
    
    #собираем все объекты в массив и отдаем
    sheetElements = []
    sheetElements.append(pageTable)

    return sheetElements


def generateReport(jsonFilePath,outputFilePath):

    PdfHelper.registerFonts()

    doc = SimpleDocTemplate(outputFilePath, pagesize=A4)
    doc.addPageTemplates([landscapeTemplate, portraitTemplate])

    data = PdfHelper.openJsonFile(jsonFilePath)

    elements = []
    elements.append(NextPageTemplate('landscape'))

    for stand in data["Stands"]:    
        titlePage = fillTitlePage(stand,doc,data)
        elements.extend(titlePage)  
        elements.append(NextPageTemplate('landscape'))
        elements.append(PageBreak())
        conclusionSheet = fillBodyPage(stand,doc,data)
        elements.extend(conclusionSheet)
        elements.append(NextPageTemplate('landscape'))
        elements.append(PageBreak())

    doc.build(elements)

    
if __name__ == "__main__":
    generateReport()
