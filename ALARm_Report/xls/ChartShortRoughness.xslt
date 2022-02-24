<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>ГД-КН2 - изменения индекса коротких неровностей на заданном участке</title>
				<style>
					.pages {
					page-break-before:always;
					width: 1000px;
					height: 760px;
					margin: auto;
					}
					.chart {
					margin: auto;
					width: 1000px;
					height: 690px;
					font-size: 12px;
					font-family: Arial;
					}

					#pageFooter:before {
					counter-increment: page;
					content:"Страница"  counter(page) "из" counter(page);
					left: 100%;
					top: 100%;
					font-size: 9px;
					white-space: nowrap;
					z-index: 20;
					-moz-border-radius: 5px;
					-moz-box-shadow: 0px 0px 4px #222;
					background-image: -moz-linear-gradient(top, #eeeeee, #cccccc);
					}
				</style>
			</head>
			<body>
				<xsl:for-each select="report/pages">
					<div align="right" id="pageFooter"  style="page-break-before:always; " class="pages">
						<table style="font-size: 12px; font-family: Arial; width: 100%;">
							<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
								<xsl:value-of select="@version" />
							</p>
							<tr>
								<th style="font-size: 20px;" align="center" colspan="3">Диаграмма изменения индекса коротких неровностей </th>
								<th style="font-size: 20px;" align="right">(ГД-КН2)</th>
							</tr>
							<tr>
								<td align="left">
									ПЧ: <xsl:value-of select="@distance" />
								</td>

								<td align="left">
									Дорога: <xsl:value-of select="@road" />
								</td>

							</tr>
							<tr>
								<td align="left">
									ДК <xsl:value-of select="@DKI" />
								</td>
								<td align="left">
									Проверка: <xsl:value-of select="@check" />
								</td>
								<td align="left">
									<xsl:value-of select="@periodDate" />
								</td>
								<!-- <td align="right">Км: <xsl:value-of select="@km" /></td> -->
							</tr>
							<tr/>
							<tr>
								<td style="font-size: 13px;" align="left" colspan="3">
									Направление:<xsl:value-of select="@Dname" />
								</td>
							</tr>
						</table>
						<svg class="chart" viewBox="0 40 1020 690">
							<text style="font-weight: bold;" x="930" y="678">Километры</text>
							<text y="48" x="17">3</text>
							<text y="148" x="10">2.5</text>
							<text y="248" x="17">2</text>
							<text y="348" x="10">1.5</text>
							<text y="448" x="17">1</text>
							<text y="548" x="10">0.5</text>
							<text y="648" x="17">0</text>
							<line stroke="black" fill="none" x1="25" x2="1000" y1="45" y2="45"/>
							<line stroke="black" fill="none" x1="25" x2="1000" y1="145" y2="145"/>
							<line stroke="black" fill="none" x1="25" x2="1000" y1="245" y2="245"/>
							<line stroke="black" fill="none" x1="25" x2="1000" y1="345" y2="345"/>
							<line stroke="black" fill="none" x1="25" x2="1000" y1="445" y2="445"/>
							<line stroke="black" fill="none" x1="25" x2="1000" y1="545" y2="545"/>
							<line stroke="black" fill="none" x1="25" x2="1000" y1="645" y2="645"/>
							<line stroke="black" fill="none" x1="30" x2="30" y1="45" y2="650"/>
							<line stroke="black" fill="none" x1="1000" x2="1000" y1="45" y2="650"/>
							<xsl:for-each select="lines">
								<line stroke="black" fill="none" x1="{@x1}" y1="{@y1}" x2="{@x2}" y2="{@y2}"/>
							</xsl:for-each>
							<g transform="translate(30,45)">
								<polyline stroke="red" fill="none" stroke-width="2" points="{@integral}"/>
								<polyline stroke="blue" fill="none" stroke-width="2" points="{@maxdeeprough}"/>
							</g>
							<g transform="translate(-10,660)">
								<xsl:for-each select="texts">
									<text x="{@x}" y="{@y}">
										<xsl:value-of select="@text" />
									</text>
								</xsl:for-each>
							</g>

							<rect stroke="black" fill="white" x="50" y="50" width="150" height="30"/>
							<text style="font-size: 6px;" x="80" y="60">Интегральный показатель</text>
							<text style="font-size: 6px;" x="80" y="75">Максимальная глубина неровностей(мм)</text>
							<line stroke="red" fill="none" x1="55" x2="75" y1="57.5" y2="57.5"/>
							<line stroke="blue" fill="none" x1="55" x2="75" y1="72.5" y2="72.5"/>
						</svg>
					</div>
					<table style="font-size: 15px; width: 80%;height: 5%;font-family: 'Times New Roman' ">

						<svg class="chart" style="width: 1200px;height: 400px;">

							<rect x="50" y="50" width="60" height="2" rx="1" ry="1" fill="red"></rect>
							<text x="125" y="55">Интегральный показатель</text>

							<rect x="50" y="90" width="60" height="2" rx="1" ry="1" fill="blue"></rect>
							<text x="125" y="95">Максимальная глубина неровностей (мм)</text>



						</svg>





					</table>
					<table style="font-size: 15px; width: 100%;height: 5%; ">
						<tr>
							<td align="left">
								Начальник &#160;&#160;<xsl:value-of select="@DKI" />
							</td>
							<td align="right">
								<xsl:value-of select="@chief" />
							</td>
						</tr>
					</table>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>