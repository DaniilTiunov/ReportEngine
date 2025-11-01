import json
from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont

# Регистрируем шрифт, чтобы корректно отображались кириллические символы
pdfmetrics.registerFont(UnicodeCIDFont("HeiseiMin-W3"))

def generate_report(input_json, output_pdf="report.pdf"):
    # Загружаем данные
    with open(input_json, "r", encoding="utf-8-sig") as f:
        data = json.load(f)

    doc = SimpleDocTemplate(output_pdf, pagesize=A4, rightMargin=40, leftMargin=40, topMargin=40, bottomMargin=40)

    styles = getSampleStyleSheet()
    styles.add(ParagraphStyle(name="Header", fontName="HeiseiMin-W3", fontSize=14, leading=18, alignment=1, spaceAfter=12))
    styles.add(ParagraphStyle(name="Body", fontName="HeiseiMin-W3", fontSize=10, leading=14))
    styles.add(ParagraphStyle(name="TableHeader", fontName="HeiseiMin-W3", fontSize=10, leading=12, alignment=1, spaceAfter=4))

    elements = []

    # Заголовок
    elements.append(Paragraph("ТЕХНОЛОГИЧЕСКАЯ КАРТА", styles["Header"]))
    elements.append(Paragraph(f"{data['название']}", styles["Header"]))
    elements.append(Paragraph(f"Размер стенда: {data['размер']}", styles["Body"]))
    elements.append(Paragraph(f"Номер по КД: {data['номер']}", styles["Body"]))
    elements.append(Spacer(1, 12))

    # Таблица материалов
    table_data = [["Наименование", "Ед.", "Норм.", "Факт."]]
    for mat in data["материалы"]:
        table_data.append([mat["наименование"], mat["ед"], mat["норм"], mat["факт"]])

    table = Table(table_data, colWidths=[280, 50, 60, 60])
    table.setStyle(TableStyle([
        ("FONTNAME", (0, 0), (-1, -1), "HeiseiMin-W3"),
        ("FONTSIZE", (0, 0), (-1, -1), 9),
        ("GRID", (0, 0), (-1, -1), 0.5, colors.black),
        ("BACKGROUND", (0, 0), (-1, 0), colors.lightgrey),
        ("ALIGN", (1, 1), (-1, -1), "CENTER"),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE")
    ]))

    elements.append(Paragraph("Основные материалы рамы стенда:", styles["Body"]))
    elements.append(table)
    elements.append(Spacer(1, 20))

    # Подписи
    elements.append(Paragraph("Изделие признано годным и передано на склад", styles["Body"]))
    elements.append(Spacer(1, 40))
    elements.append(Paragraph("ОТК (ФИО, подпись) _________________________", styles["Body"]))
    elements.append(Paragraph("Склад (ФИО, подпись) _________________________", styles["Body"]))

    # Генерация PDF
    doc.build(elements)
    print(f"✅ Отчёт успешно создан: {output_pdf}")


if __name__ == "__main__":
    import sys
    if len(sys.argv) < 2:
        print("Использование: python report_generator.py data.json [output.pdf]")
    else:
        input_file = sys.argv[1]
        output_file = sys.argv[2] if len(sys.argv) > 2 else "report.pdf"
        generate_report(input_file, output_file)
