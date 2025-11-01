from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, tables
)
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.cidfonts import UnicodeCIDFont

# Поддержка кириллицы
pdfmetrics.registerFont(UnicodeCIDFont("HeiseiMin-W3"))

def generate_empty_techcard(output_pdf="techcard_template.pdf"):
    doc = SimpleDocTemplate(output_pdf, pagesize=A4)


if __name__ == "__main__":
    generate_empty_techcard()
