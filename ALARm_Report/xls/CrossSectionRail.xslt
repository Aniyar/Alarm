<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>ДФ-з4 – график отклонений суммарной величины зазоров от нормативной величины</title>
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
					font-size: 14px;
					font-family: Arial;
					}
				</style>
			</head>
			<body>
				<xsl:for-each select="report/pages">
					<div align="center" class="pages">
						<b>
							<p align="left" style="color:black; font-size:12px">
								<xsl:value-of select="@version" />
							</p>
						</b>
						<table style="font-size: 14px; font-family: Arial; width: 90%;">
							<tr>
								<th style="font-size: 14px;" align="center" colspan="4">График отклонений суммарной величины зазоров от нормативной величины (Форма ДФ-з4))</th>
							</tr>

							<tr>
								<td align="left">
									ПЧ:<xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@car" />
								</td>
								<td align="left">
									Проверка:<xsl:value-of select="@check" />
								</td>
								<td>
									<xsl:value-of select="@period" />
								</td>
							</tr>
						</table>
						<svg class="chart" viewBox="0 0 1000 690">
							<text style="font-size: 14px; font-weight: bold;" x="345" y="40">График накопления зазоров на участке их разгонки</text>
							<text style="transform: rotate(270deg);" x="-150" y="10">Накопление зазоров, мм</text>
							<text x="950" y="668">№ стыков</text>
							<line stroke="black" fill="none" x1="60" x2="60" y1="30" y2="675"/>
							<line stroke="black" fill="none" x1="55" x2="1000" y1="670" y2="670"/>

							<xsl:for-each select="gapsline_right">
								<polyline stroke="black" fill="none" stroke-width="2" points="{@gapsline}"/>
								<polyline stroke="black" fill="none" stroke-width="2" points="{@nominalgapsline}"/>
							</xsl:for-each>

							<xsl:for-each select="nominalgaps">
								<polyline stroke="black" fill="black" points="{@mark}"/>
							</xsl:for-each>

							<xsl:for-each select="texts">
								<text style="font-size: 6px;" x="{@x}" y="{@y}">
									<xsl:value-of select="@text" />
								</text>
							</xsl:for-each>

							<g>
								<line stroke="black" fill="none" x1="90" x2="90" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="120" x2="120" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="150" x2="150" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="180" x2="180" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="210" x2="210" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="240" x2="240" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="270" x2="270" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="300" x2="300" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="330" x2="330" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="360" x2="360" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="390" x2="390" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="420" x2="420" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="450" x2="450" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="480" x2="480" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="510" x2="510" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="540" x2="540" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="570" x2="570" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="600" x2="600" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="630" x2="630" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="660" x2="660" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="690" x2="690" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="720" x2="720" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="750" x2="750" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="780" x2="780" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="810" x2="810" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="840" x2="840" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="870" x2="870" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="900" x2="900" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="930" x2="930" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="960" x2="960" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="990" x2="990" y1="670" y2="675"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="630" y2="630"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="590" y2="590"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="550" y2="550"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="510" y2="510"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="470" y2="470"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="430" y2="430"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="390" y2="390"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="350" y2="350"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="310" y2="310"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="270" y2="270"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="230" y2="230"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="190" y2="190"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="150" y2="150"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="110" y2="110"/>
								<line stroke="black" fill="none" x1="55" x2="1000" y1="70" y2="70"/>
							</g>

							<rect stroke="black" fill="white" x="50" y="50" width="150" height="30"/>
							<rect stroke="black" fill="white" x="50" y="50" width="150" height="30"/>
							<text style="font-size: 6px;" x="0" y="0">накопление измеренных зазоров</text>
							<text style="font-size: 6px;" x="0" y="0">накопление номинальных зазоров</text>
							<line stroke="red" fill="none" x1="55" x2="75" y1="57.5" y2="57.5"/>
							<line stroke="blue" fill="none" x1="55" x2="75" y1="72.5" y2="72.5"/>
						</svg>
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>