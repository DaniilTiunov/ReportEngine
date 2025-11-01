from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak
)
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont

# Поддержка кириллицы
pdfmetrics.registerFont(UnicodeCIDFont("HeiseiMin-W3"))

def generate_empty_techcard(output_pdf="techcard_template.pdf"):
    doc = SimpleDocTemplate(output_pdf, pagesize=A4,
                            rightMargin=30, leftMargin=30,
                            topMargin=30, bottomMargin=30)

    styles = getSampleStyleSheet()
    styles.add(ParagraphStyle(name="Header", fontName="HeiseiMin-W3",
                              fontSize=14, leading=18, alignment=1, spaceAfter=12))
    styles.add(ParagraphStyle(name="SubHeader", fontName="HeiseiMin-W3",
                              fontSize=11, leading=14, spaceAfter=6))
    styles.add(ParagraphStyle(name="Body", fontName="HeiseiMin-W3",
                              fontSize=9, leading=12))
    styles.add(ParagraphStyle(name="Tiny", fontName="HeiseiMin-W3",
                              fontSize=8, leading=10))

    elements = []

    # === Заголовок ===
    elements.append(Paragraph("ТЕХНОЛОГИЧЕСКАЯ КАРТА", styles["Header"]))
    elements.append(Paragraph("Стенд датчиков КИПиА МОМ-02-02", styles["SubHeader"]))
    elements.append(Paragraph("Размер стенда, мм: ________", styles["Body"]))
    elements.append(Paragraph("Рама, мм. Обозначение по КД: ________", styles["Body"]))
    elements.append(Spacer(1, 10))

    # === Таблица 1. Основные материалы рамы стенда ===
    elements.append(Paragraph("Основные материалы рамы стенда", styles["SubHeader"]))
    table1_data = [["Наименование", "Единица измерения", "Норм.", "Факт."]] + [
        ["", "", "", ""] for _ in range(10)
    ]
    table1 = Table(table1_data, colWidths=[250, 100, 60, 60])
    table1.setStyle(TableStyle([
        ("FONTNAME", (0, 0), (-1, -1), "HeiseiMin-W3"),
        ("FONTSIZE", (0, 0), (-1, -1), 8),
        ("GRID", (0, 0), (-1, -1), 0.5, colors.black),
        ("BACKGROUND", (0, 0), (-1, 0), colors.lightgrey),
        ("ALIGN", (2, 1), (-1, -1), "CENTER")
    ]))
    elements.append(table1)
    elements.append(Spacer(1, 10))

    # === Таблица 2. Комплект монтажных частей ===
    elements.append(Paragraph("Комплект монтажных частей (Дренаж и/или продувка)", styles["SubHeader"]))
    table2_data = [["Наименование", "Ед. изм.", "Норм.", "Факт."]] + [
        ["", "", "", ""] for _ in range(8)
    ]
    table2 = Table(table2_data, colWidths=[250, 100, 60, 60])
    table2.setStyle(TableStyle([
        ("FONTNAME", (0, 0), (-1, -1), "HeiseiMin-W3"),
        ("FONTSIZE", (0, 0), (-1, -1), 8),
        ("GRID", (0, 0), (-1, -1), 0.5, colors.black),
        ("BACKGROUND", (0, 0), (-1, 0), colors.lightgrey),
        ("ALIGN", (2, 1), (-1, -1), "CENTER")
    ]))
    elements.append(table2)
    elements.append(Spacer(1, 10))

    # === Таблица 3. Электрические компоненты ===
    elements.append(Paragraph("Электрические компоненты", styles["SubHeader"]))
    table3_data = [["Наименование", "Ед. изм.", "Норм.", "Факт."]] + [
        ["", "", "", ""] for _ in range(6)
    ]
    table3 = Table(table3_data, colWidths=[250, 100, 60, 60])
    table3.setStyle(TableStyle([
        ("FONTNAME", (0, 0), (-1, -1), "HeiseiMin-W3"),
        ("FONTSIZE", (0, 0), (-1, -1), 8),
        ("GRID", (0, 0), (-1, -1), 0.5, colors.black),
        ("BACKGROUND", (0, 0), (-1, 0), colors.lightgrey),
        ("ALIGN", (2, 1), (-1, -1), "CENTER")
    ]))
    elements.append(table3)
    elements.append(PageBreak())

    # === Таблица 4. Таблица соединений ===
    elements.append(Paragraph("Таблица соединений", styles["SubHeader"]))
    table4_data = [["№", "Цепь", "Маркировка", "Коробка", "Клеммы", "Примечание"]] + [
        ["", "", "", "", "", ""] for _ in range(6)
    ]
    table4 = Table(table4_data, colWidths=[25, 80, 100, 80, 80, 80])
    table4.setStyle(TableStyle([
        ("FONTNAME", (0, 0), (-1, -1), "HeiseiMin-W3"),
        ("FONTSIZE", (0, 0), (-1, -1), 8),
        ("GRID", (0, 0), (-1, -1), 0.5, colors.black),
        ("BACKGROUND", (0, 0), (-1, 0), colors.lightgrey),
        ("ALIGN", (0, 0), (-1, -1), "CENTER"),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE")
    ]))
    elements.append(table4)
    elements.append(Spacer(1, 20))

    # === Блок подписей ===
    elements.append(Paragraph("Изделие признано годным и передано на склад", styles["Body"]))
    elements.append(Spacer(1, 30))
    elements.append(Paragraph("ОТК (ФИО, подпись) _________________________", styles["Body"]))
    elements.append(Paragraph("Склад (ФИО, подпись) _________________________", styles["Body"]))
    elements.append(Spacer(1, 10))
    elements.append(Paragraph("Дата фактического выполнения операции: ____________", styles["Tiny"]))
    elements.append(Paragraph("Ф.И.О. исполнителя: ____________________________", styles["Tiny"]))
    elements.append(Paragraph("Подпись исполнителя: ____________________________", styles["Tiny"]))

    # === Генерация PDF ===
    doc.build(elements)
    print(f"✅ Пустая технологическая карта создана: {output_pdf}")


if __name__ == "__main__":
    generate_empty_techcard()
