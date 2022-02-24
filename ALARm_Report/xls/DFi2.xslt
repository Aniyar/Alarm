<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>ДФ-И2 Ведомость кривых с наибольшей интенсивностью бокового износа рельсов</title>
				<style>
					td {
					padding-left: 5px;
					}

					.pages {
					page-break-after: always;
					}

					table.main {
					border-collapse: collapse;
					margin: auto;
					width: 100%;
					}

					table.main,
					td.main,
					th.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					}
				</style>
			</head>

			<body>
				<xsl:for-each select="report/trip">
					<div style="page-break-before:always;">

						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<b>
							<p align="center" style="color:black; font-size:14px">
								Ведомость кривых с наибольшей интенсивностью<br /> бокового износа рельсов(Форма ДФ-И2)
							</p>
						</b>
						<table style="font-size: 12px; font-family: 'Times New Roman';width: 110%; margin: auto;margin-bottom:8px;border-collapse: collapse">


							<tr>
								<td align="left">
									ПЧ:<xsl:value-of select="@pch" />
								</td>
								<td align="left">
									Дорога:<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@ps" />
								</td>
								<td align="left">
									Проверка:<xsl:value-of select="@date_statement"/>  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="@period1" />
								</td>
								<td align="left">Порог роста износа -  0 мм/мес</td>
							</tr>



						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
							<thead>
								<tr>
									<td align="center" valign="middle" rowspan="2">
										№ <br /> п/п
									</td>
									<td align="center" valign="middle" rowspan="2">№ крв</td>
									<td align="center" valign="middle" rowspan="1" colspan="1">Кривая</td>

									<td align="center" valign="middle" rowspan="2">Vпз</td>
									<td align="center" valign="middle" rowspan="1" colspan="2">
										Боковой износ<br />

										<xsl:value-of select="@date_statement" />  &#160;&#160;<xsl:value-of select="@period1" />
									</td>
									<td   align="center" valign="middle" rowspan="1" colspan="2">
										Боковой износ <br />

										<xsl:value-of select="@date_statement" />    &#160;&#160;<xsl:value-of select="@period2" />
									</td>
									<td   align="center" valign="middle" rowspan="2">
										Интенсивность<br /> (мм/мес)
									</td>
								</tr>
								<tr>
									<td  align="center" valign="middle" rowspan="1">
										Начало - Конец <br/> (км.м)
									</td>
									<td   align="center" valign="middle" rowspan="1">сред.</td>
									<td   align="center" valign="middle" rowspan="1">макс.</td>
									<td   align="center" valign="middle" rowspan="1">сред.</td>
									<td   align="center" valign="middle" rowspan="1">макс.</td>
								</tr>
								<tr>
									<th  align="right" colspan="11"  style="font-size: 13px;padding:2.5px;">
										<xsl:value-of select="@trackinfo" />
									</th>
								</tr>
							</thead>
							<tbody id="this">
								<xsl:for-each select="lev">

									<xsl:for-each select="Note">
										<tr>
											<td  class="main" align="center">
												<xsl:value-of select="@iter" />
											</td>
											<td  class="main" align="center">
												<xsl:value-of select="@id" />
											</td>
											<td  class="main" align="center">
												<xsl:value-of select="@start_km" /> - <xsl:value-of select="@final_km" />
											</td>

											<td  class="main" align="center">
												<xsl:value-of select="@Vust" />
											</td>
											<td  class="main" align="center">
												<xsl:value-of select="@sred1" />
											</td>
											<td  class="main" align="center">
												<xsl:value-of select="@max1" />
											</td>
											<td  class="main" align="center">
												<xsl:value-of select="@sred2" />
											</td>
											<td  class="main" align="center">
												<xsl:value-of select="@max2" />
											</td>
											<td  class="main" align="center">
												<xsl:value-of select="@intensv" />
											</td>
										</tr>
									</xsl:for-each>
								</xsl:for-each>
							</tbody>
						</table>
						<table style="font-size: 12px; width: 90%;height: 5%; margin: auto;">
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