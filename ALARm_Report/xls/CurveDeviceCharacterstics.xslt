<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>ВЕДОМОСТЬ ХАРАКТЕРИСТИК УСТРОЙСТВА КРИВЫХ УЧАСТКОВ ПУТИ</title>
				<style>
					table.main {
					border-collapse: collapse;
					margin:auto;
					width: 100%;
					}
					table.main, td.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					margin: auto;
					margin-bottom:2px;
					margin-top:2px;
					}
					b {
					font-size: 12px;
					}
				</style>
			</head>
			<body>
				<div style="page-break-before:always;">
					<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
						<xsl:value-of select="report/@version" />
					</p>
					<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
						<td align = "center">
							<b>ВЕДОМОСТЬ ХАРАКТЕРИСТИК УСТРОЙСТВА КРИВЫХ УЧАСТКОВ ПУТИ </b>
						</td>
						<td align  ="right">
							<b>(ФП-3.1)</b>
						</td>
					</table>

					<div align="center">
						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<td>
									ПЧ: <xsl:value-of select="report/@distance" />
								</td>
								<td>
									Дорога: <xsl:value-of select="report/@way" />
								</td>
							</tr>
							<tr>
								<td>
									<xsl:value-of select="report/curve/@PC" />
								</td>
								<td>
									Проверка: <xsl:value-of select="report/curve/@check" />
								</td>
								<td>
									<xsl:value-of select="report/curve/@Period" />
								</td>
							</tr>
						</table>
					</div>
					<div align="center">
						<table class="main">
							<thead>
								<tr>
									<td colspan="7" rowspan="1" align="center" class="main">Кривая (кривизна/уровень)</td>
									<td colspan="4" rowspan="2" align="center" class="main">Круговая кривая</td>
									<td rowspan="1" style="border-right:0;" class="main">1-я</td>
									<td rowspan="1" colspan="4" align="center" style="border-right:0; border-left:0;" class="main">Переходные</td>
									<td rowspan="1" align="right" style="border-left:0;" class="main">2-я</td>
									<td rowspan="3" colspan="1" align="center" class="main">Тип</td>
									<td rowspan="3" colspan="1" align="center" class="main">
										А <sub>нп</sub> <br/> ср
									</td>
									<td rowspan="3" colspan="1" align="center" class="main">
										А <sub>r </sub> <br/> max
									</td>
									<td rowspan="3" colspan="1" align="center" class="main">&#936;</td>
									<td rowspan="3" colspan="1" align="center" class="main">
										V<sub>пз</sub>
									</td>
									<td rowspan="3" colspan="1" align="center" class="main">
										V<sub>кр</sub>
									</td>
									<td rowspan="3" colspan="1" align="center" class="main">
										V<sub>пр</sub>
									</td>
									<td rowspan="3" colspan="1" align="center" class="main">
										V<sub>из</sub>
									</td>
									<td rowspan="3" colspan="1" align="center" class="main">
										V<sub>огр</sub>
									</td>
								</tr>
								<tr>
									<td style="width:2%;" colspan="1" rowspan="2" class="main">№</td>
									<td style="width:2%;" colspan="1" rowspan="2" class="main">
										№ <br/> э.к.
									</td>
									<td rowspan="1" colspan="2" align="center" style="border-right:0;" class="main">Начало</td>
									<td rowspan="1" colspan="2" align="center" class="main">Конец</td>
									<td rowspan="1" colspan="1" align="center" class="main">угол</td>
									<td colspan="3" align="center" class="main">отвод</td>
									<td colspan="3" align="center" class="main">отвод</td>
								</tr>
								<tr>
									<td align="center" class="main">км</td>
									<td align="center" class="main">м</td>
									<td align="center" class="main">км</td>
									<td align="center" class="main">м</td>
									<td align="center" class="main">град</td>
									<td align="center" class="main">дл.</td>
									<td align="center" class="main">min.</td>
									<td align="center" class="main">max.</td>
									<td align="center" class="main">ср.</td>
									<td align="center" class="main">дл.</td>
									<td align="center" class="main">макс.</td>
									<td align="center" class="main">ср.</td>
									<td align="center" class="main">дл.</td>
									<td align="center" class="main">max.</td>
									<td align="center" class="main">ср.</td>
								</tr>
								<tr>
									<td align="left" colspan="26">
										Направление: <xsl:value-of select="report/trip/@direction" />
										&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160; Путь: <b>
											<xsl:value-of select="report/trip/@track" />
										</b>
									</td>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="report/curve">
									<tr>
										<td style="border-bottom:0;" align="center" class="main">
											<xsl:value-of select="@side"/>
											<br/>
										</td>
										<td style="border-bottom:0;" align="center" class="main">
											<xsl:value-of select="@ID_DB"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_curve/@start_km"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_curve/@start_m"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_curve/@final_km"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_curve/@final_m"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_curve/@angle"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_circle_curve/@len"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_circle_curve/@min"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_circle_curve/@max"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_circle_curve/@mid"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@len1"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@max1"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@mid1"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@len2"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@max2"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@mid2"/>
										</td>
										<td align="center" class="main">
											Пасс.
										</td>

										<td align="center" class="main">
											<xsl:value-of select="speed/@avgPASSA"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@PASSAMAX"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@psi"/>
										</td>

										<td align="center" class="main">
											<xsl:value-of select="speed/@V1"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@V2"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@V3"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@V4"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@V5"/>
										</td>
									</tr>
									<tr>
										<td align="center" style="border-top:0;" class="main">
											<xsl:value-of select="@order"/>
										</td>
										<td style="border-top:0;" class="main"></td>
										<td class="main"></td>


										<td align="center" class="main">
											<xsl:value-of select="param_curve/@start_lvl"/>
										</td>
										<td class="main"></td>
										<td align="center" class="main">
											<xsl:value-of select="param_curve/@final_lvl"/>
										</td>
										<td class="main"></td>
										<td align="center" class="main">
											<xsl:value-of select="param_circle_curve/@len_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_circle_curve/@min_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_circle_curve/@max_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="param_circle_curve/@mid_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@len1_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@max1_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@mid1_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@len2_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@max2_lvl"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="transition/@mid2_lvl"/>
										</td>
										<td align="center" class="main">
											Груз.
										</td>

										<td align="center" class="main">
											<xsl:value-of select="speed/@avgfreightA"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@freightAMAX"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@PsiFreigh"/>
										</td>

										<td align="center" class="main">
											<xsl:value-of select="speed/@V1Freigh"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@V2Freigh"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@V3Freigh"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@V4Freigh"/>
										</td>
										<td align="center" class="main">
											<xsl:value-of select="speed/@V5Freigh"/>
										</td>
									</tr>
								</xsl:for-each>
							</tbody>
						</table>
						<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
							<tr>
								<td>
									Начальник путеизмерителя: <xsl:value-of select="report/trip/@ps" />
								</td>
								<td>
									<xsl:value-of select="report/trip/@chief" />
								</td>
							</tr>
						</table>
					</div>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>