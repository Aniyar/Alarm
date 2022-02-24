<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Ведомость шпал с полной засыпкой балласта</title>
				<style>
					.pages {
					page-break-before:always;
					}
					table.main {
					border-collapse: collapse;
					margin: auto;
					width: 100%;
					}
					table.main, td.main, th.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;

					}
				</style>
			</head>
			<body>
				<xsl:for-each select="report/pages">
					<div class="pages" align="center">

						<table style="font-size: 14px; font-family:  'Times New Roman'; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Ведомость шпал с полной засыпкой балласта</p>
									</b>
								</td>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@road" />
										ЖД
									</b>
								</td>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@period" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;


									<b>
										<xsl:value-of select="@type" />
									</b>

								</td>
								<td  align="left">
								</td>


							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@car" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@data" />
									</b>

								</td>
							</tr>
						</table>
					</div>
					<div align="center">
						<table class="main">
							<thead>
								<tr>
									<td class="main" align="center" rowspan="2">№ п/п</td>
									<td class="main" align="center" colspan="2">Начало</td>
									<td class="main" align="center" colspan="2">Конец</td>
									<td class="main" align="center" rowspan="2">Отступление</td>
									<td class="main" align="center" rowspan="2">Тип скреплений</td>
									<td class="main" align="center" rowspan="2">Протяженость, м</td>
								</tr>
								<tr>
									<td class="main" align="center">км</td>
									<td class="main" align="center">м</td>
									<td class="main" align="center">км</td>
									<td class="main" align="center">м</td>
								</tr>
							</thead>
							<xsl:for-each select="tracks">
								<tr>
									<th class="main" align="right" colspan="8">
										<xsl:value-of select="@trackinfo" />
									</th>
								</tr>
								<xsl:for-each select="elements">
									<tr>
										<td class="main" align="center">
											<xsl:value-of select="@n" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@startkm" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@startm" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@finalkm" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@finalm" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@digression" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@fastening" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@length" />
										</td>
									</tr>
								</xsl:for-each>
								<tr>
									<td class="main" align="right" colspan="8">
										<xsl:value-of select="@tracktotal" />
									</td>
								</tr>
							</xsl:for-each>
							<tr>
								<td class="main" align="right" colspan="8">
									<xsl:value-of select="@distancetotal" />
								</td>
							</tr>
						</table>
						<table style="font-size: 12px; width: 95%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@ps" />
								</td>


								<td align="center">
									<xsl:value-of select="@chief" />
								</td>
							</tr>
						</table>
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>