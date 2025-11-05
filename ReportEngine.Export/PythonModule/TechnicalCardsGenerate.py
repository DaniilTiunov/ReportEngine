import json
from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont

try:
    pdfmetrics.registerFont(UnicodeCIDFont("HeiseiMin-W3"))
except Exception:
    pass

def generate_report(output_pdf="technical_cards_report.pdf", pages: int = 1):
    doc = SimpleDocTemplate(output_pdf, pagesize=A4, rightMargin=40, leftMargin=40, topMargin=40, bottomMargin=40)

    styles = getSampleStyleSheet()
    body_style = ParagraphStyle(
        "Body",
        parent=styles["Normal"],
        fontName="HeiseiMin-W3",
        fontSize=10,
        leading=12,
    )

    story = []
    for i in range(pages):
        story.append(Paragraph(" ", body_style))
        if i != pages - 1:
            story.append(Spacer(1, 12))

    doc.build(story)
    return output_pdf


if __name__ == "__main__":
    output_file = "technical_cards_report.pdf"
    generate_report(output_file)