<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>ВЕДОМОСТЬ РАССТРОЙСТВА КРИВЫХ</title>
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
					margin-bottom:8px;
					border-collapse: collapse;
					}
					b {
					font-size: 12px;
					}
				</style>
			</head>
			<body>
				<div align="center" style="margin:30px 0px 0px">

					<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
						<xsl:value-of select="report/curve/@version" />
					</p>

					<b> ВЕДОМОСТЬ РАССТРОЙСТВА КРИВЫХ(ФПЦ-3.11)</b>
				</div>
				<div align="center">

					<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">


						<tr>
							<td align="left">
								<xsl:value-of select="report/@distance" />
							</td>
							<td align="left">
								Дорога:
								<xsl:value-of select="report/curve/@road" />
							</td>
						</tr>
						<tr>
							<td align="left">
								<xsl:value-of select="report/curve/@PC" />
							</td>
							<td align="left">
								Проверка:
								<xsl:value-of select="report/curve/@type" />
							</td>
							<td>
								<xsl:value-of select="report/curve/@period" />
							</td>
							<td align="right" style="font-style:italic;">
								<xsl:value-of select="report/curve/@wear" />
								многорадиусная кривая
							</td>
						</tr>
					</table>


				</div>
				<div align="center">
					<table class="main" style="font-size: 12px;       font-family:  'Times New Roman';">
						<thead>
							<tr>
								<td colspan="6" align="center" class="main">Кривая (кривизна/уровень)</td>
								<td colspan="4" align="center" class="main">Круговая кривая</td>
								<td style="border-right:0;" class="main">1-я</td>
								<td colspan="4" align="center" style="border-right:0; border-left:0;" class="main">Переходные</td>
								<td align="right" style="border-left:0;" class="main">2-я</td>
								<td colspan="4" align="center" class="main">Скорости</td>
								<td colspan="5" align="center" class="main">Степени расстройства</td>
							</tr>
							<tr>
								<td class="main"></td>
								<td colspan="2" align="center" style="border-right:0;" class="main">Начало</td>
								<td colspan="2" align="center" style="border-right:0; border-left:0;" class="main">Конец</td>
								<td style="border-left:0;" class="main"></td>
								<td colspan="4" class="main"></td>
								<td colspan="3" align="center" class="main">отвод</td>
								<td colspan="3" align="center" class="main">отвод</td>
								<td align="center" class="main">
									A
									<sub>нп</sub>
								</td>
								<td align="center" style="border-right:0;" class="main">
									V
									<sub>пз</sub>
								</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">
									V
									<sub>кр</sub>
								</td>
								<td align="center" style="border-left:0;" class="main">
									V
									<sub>из</sub>
								</td>
								<td rowspan="2" align="center" style="border-right:0; border-left:0;" class="main">
									P
									<sub>инт</sub>
								</td>
								<td rowspan="2" align="center" style="border-right:0; border-left:0;" class="main">
									P
									<sub>анп</sub>
								</td>
								<td rowspan="2" align="center" style="border-right:0; border-left:0;" class="main">
									P
									<sub>&#916;</sub>
								</td>
								<td rowspan="2" align="center" style="border-right:0; border-left:0;" class="main">
									P
									<sub>пл</sub>
								</td>
								<td rowspan="2" align="center" style="border-left:0;" class="main">
									P
									<sub>ур</sub>
								</td>

							</tr>

							<tr>
								<td class="main"></td>
								<td align="center" style="border-right:0;" class="main">км</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">м</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">км</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">м</td>
								<td align="center" style="border-left:0;" class="main">уг.</td>
								<td align="center" style="border-right:0;" class="main">дл.</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">мин.</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">макс.</td>
								<td align="center" style="border-left:0;" class="main">ср.</td>
								<td align="center" style="border-right:0;" class="main">дл.</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">макс.</td>
								<td align="center" style="border-left:0;" class="main">ср.</td>
								<td align="center" style="border-right:0;" class="main">дл.</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">макс.</td>
								<td align="center" style="border-left:0;" class="main">ср.</td>
								<td align="center" class="main">&#936;</td>
								<td align="center" style="border-right:0;" class="main">п/г</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">
									V
									<sub>пр</sub>
								</td>
								<td align="center" style="border-left:0;" class="main">
									V
									<sub>дп</sub>
								</td>
							</tr>
						</thead>
						<tr>
							<th align="right" colspan="30" style="font-size: 13px;">
								<xsl:value-of select="report/curve/@direction"/> &#160; Путь: <xsl:value-of select="report/curve/@track" />
							</th>
						</tr>
						<xsl:for-each select="report/curve">
							<tr>
								<td align="center" style="border-bottom:0;" class="main">
									<xsl:value-of select="@order" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="param_curve/@final_km" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@final_m" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@start_km" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@start_m" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@angle" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="param_circle_curve/@len" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@min" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@max" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@mid" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="transition/@len1" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="transition/@max1" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="transition/@mid1" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="transition/@len2" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="transition/@max2" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="transition/@mid2" />
								</td>
								<td align="center" style="border-bottom:0;" class="main">
									<xsl:value-of select="speed/@A" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="speed/@V1" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@V2" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@V3" />
								</td>
								<td rowspan="2" align="center" style="border-bottom:0; border-left:0; border-right:0;" class="main">
									<xsl:value-of select="disorders/@p5" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="disorders/@p1" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="disorders/@p2" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="disorders/@p3" />
								</td>
								<td align="center" style=" border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="disorders/@p4" />
								</td>

							</tr>
							<tr>
								<td align="center" style="border-top:0;" class="main">
									<xsl:value-of select="@side" />
								</td>
								<td style="border-top:0; border-right:0;" class="main"></td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@start_lvl" />
								</td>
								<td style="border-top:0; border-right:0; border-left:0;" class="main"></td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@final_lvl" />
								</td>
								<td style="border-top:0; border-left:0;" class="main"></td>
								<td align="center" style="border-top:0; border-right:0;" class="main">
									<xsl:value-of select="param_circle_curve/@len_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@min_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@max_lvl" />
								</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@mid_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0;" class="main">
									<xsl:value-of select="transition/@len1_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="transition/@max1_lvl" />
								</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">
									<xsl:value-of select="transition/@mid1_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0;" class="main">
									<xsl:value-of select="transition/@len2_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="transition/@max2_lvl" />
								</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">
									<xsl:value-of select="transition/@mid2_lvl" />
								</td>
								<td align="center" style="border-top:0;" class="main">
									<xsl:value-of select="speed/@psi" />
								</td>
								<td align="center" style="border-top:0; border-right:0;" class="main">
									<xsl:value-of select="speed/@pass" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@V4" />
								</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@V5" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0; border-top:0;" class="main">
									(
									<xsl:value-of select="disorders/@p1_2" />
									)
								</td>
								<td align="center" style="border-right:0; border-top:0; border-left:0;" class="main">
									(
									<xsl:value-of select="disorders/@p2_2" />
									)
								</td>
								<td align="center" style="border-right:0; border-top:0; border-left:0;" class="main">
									(
									<xsl:value-of select="disorders/@p3_2" />
									)
								</td>
								<td align="center" style=" border-top:0; border-left:0;" class="main">
									(
									<xsl:value-of select="disorders/@p4_2" />
									)
								</td>
							</tr>
						</xsl:for-each>
					</table>

					<table style="font-size: 12px; width: 90%;height: 5%; margin: auto;">

						<tr>
							<td align="left">
								Начальник &#160;&#160;<xsl:value-of select="report/curve/@PC" />
							</td>


							<td align="center">
								<xsl:value-of select="report/curve/@chief" />
							</td>
						</tr>

					</table>

				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>