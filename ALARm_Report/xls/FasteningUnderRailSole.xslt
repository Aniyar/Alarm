<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>ВЕДОМОСТЬ КЛЕММА СКРЕПЛЕНИЯ ЖБР-65 (ЖБР-65Ш) ПОД ПОДОШВОЙ РЕЛЬСА</title>
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
					<div class="pages">


						<table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">ВЕДОМОСТЬ КЛЕММА СКРЕПЛЕНИЯ ЖБР-65 (ЖБР-65Ш) ПОД ПОДОШВОЙ РЕЛЬСА</p>
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
						<table class="main">
							<tr>
								<td class="main" align="center">№ п/п</td>
								<td class="main" align="center">ПЧУ, ПД, ПДБ</td>
								<td class="main" align="center">Перегон, станция</td>
								<td class="main" align="center">км</td>
								<td class="main" align="center">пк</td>
								<td class="main" align="center">м</td>
								<td class="main" align="center">
									V<sub>уст</sub>
								</td>
								<td class="main" align="center">Отступление</td>
								<td class="main" align="center">Нить</td>
								<td class="main" align="center">Скрепление</td>
								<td class="main" align="center">Характеристика пути</td>
								<td class="main" align="center">Примечание</td>
							</tr>
							<xsl:for-each select="tracks">
								<tr>
									<td class="main" align="right" colspan="12">
										<xsl:value-of select="@trackinfo" />
									</td>
								</tr>
								<xsl:for-each select="elements">
									<tr>
										<td class="main" align="center">
											<xsl:value-of select="@n" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@pchu" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@station" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@km" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@piket" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@meter" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@vpz" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@digression" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@thread" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@fastening" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@tripplan" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@notice" />
										</td>
									</tr>
								</xsl:for-each>
							</xsl:for-each>
						</table>
						<!-- <div style="width: 90%;margin: auto;font-size: 9px;font-family: Arial;margin-top: 10px;" align="center"><xsl:value-of select="@info" /></div> -->
						<table style="font-size: 12px; width: 95%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@ps" />
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
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