<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Ведомость сварных стыков с дефектами по поверхности катания</title>
				<style>
					.pages {
					page-break-after:always;

					}
					td {
					padding-left: 5px;
					}
					table.main, td.main, th.main {

					border-collapse: collapse;
					border: 1.5px solid black;
					font-size: 12px;
					font-family: 'Times New Roman';
					}

					table.main {
					width: 100%;
					margin: auto;
					}
					b {
					font-size: 12px;
					}
				</style>
			</head>
			<body>
				<xsl:for-each select="report/pages">
					<div style="page-break-before:always;">



						<table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Ведомость сварных стыков с дефектами по поверхности катания рельса                                          </p>
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


					<!-- <table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;">
                     
                         
                            <tr>
                                <td align="left">Наименование железной дороги                                    <b>
                                        <xsl:value-of select="@direction" />
                                    </b>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">Отчетный период                                    <b>
                                        <xsl:value-of select="@period" />
                                    </b>, тип проверки                                    <b>
                                        <xsl:value-of select="@type" />
                                    </b>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">наименование и номер единицы                                    <b>
                                        <xsl:value-of select="@car" />
                                    </b>, данные по проезду                                    <b>
                                        <xsl:value-of select="@data" />
                                    </b>
                                </td>
                            </tr>
                        </table> -->


					<div class="pages">
						<table class="main">
							<tr>
								<th class="main" align="center">№ п/п</th>
								<th class="main" align="center">ПЧУ, ПД, ПДБ</th>
								<th class="main" align="center">Перегон, станция</th>
								<th class="main" align="center">км</th>
								<th class="main" align="center">пк</th>
								<th class="main" align="center">м</th>
								<th class="main" align="center">Vпз</th>
								<th class="main" align="center">Отступление</th>
								<th class="main" align="center">Величина, мм</th>
								<th class="main" align="center">Примечание</th>
							</tr>
							<xsl:for-each select="tracks">
								<tr>
									<th class="main" align="right" colspan="10">
										<xsl:value-of select="@trackinfo" />
									</th>
								</tr>
								<xsl:for-each select="elements">
									<tr>
										<td class="main"></td>
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
											<xsl:value-of select="@speed" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@digression" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@amount" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@notice" />
										</td>
									</tr>
								</xsl:for-each>
							</xsl:for-each>
						</table>

						<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@ps" />
								</td>



								<td align="right">
									<xsl:value-of select="@chief" />
								</td>
							</tr>
						</table>
						<!-- <div style="width: 90%;margin: auto;font-size: 9px;font-family: Arial;margin-top: 10px;" align="center">
                            <xsl:value-of select="@info" />
                        </div> -->
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>