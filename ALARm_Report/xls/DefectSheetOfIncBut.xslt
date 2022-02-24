<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>Ведомость дефектных изолирующих стыковых зазоров</title>
				<style>
					td {
					padding-left: 5px;
					}
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
				<xsl:for-each select="report/trip">
					<div style = "page-break-before:always;">

						<table style="font-size: 14px; font-family: Times New Roman; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Ведомость дефектных изорирующих стыковых зазоров</p>
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
										<xsl:value-of select="@periodDate" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;

									<b>
										<xsl:value-of select="@check" />
									</b>

								</td>
								<td  align="left">
								</td>


							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@ps" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@date_statement" />
									</b>

								</td>

							</tr>
						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<tr>
									<td class="border-head" style="padding:2.5px;" rowspan="1">№ п/п</td>
									<td class="border-head" style="padding:2.5px;" width="15%" rowspan="1">ПЧУ, ПД, ПДБ</td>
									<td class="border-head" style="padding:2.5px;" width="15%" rowspan="1">Перегон, станция</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">км</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">пк</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">метр</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">Vпз</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">Зазор правой нити, мм</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">Зазор левой нити, мм</td>
									<td class="border-head" style="padding:2.5px;" width="5%" rowspan="1">T°</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">Забег, мм</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">План пути</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">Vдоп</td>
									<td class="border-head" style="padding:2.5px;" width="10%" rowspan="1">Примечание</td>
								</tr>
							</thead>
							<xsl:for-each select="direction">
								<tr>
									<td align="right" style="padding:2.5px;" colspan="14">
										<b>
											<xsl:value-of select="@name" />
										</b>
										<!-- начало записи-->
										<xsl:for-each select="Note">
											<tr>
												<td align="center">
													<xsl:value-of select="@n" />
												</td>
												<td align="center">
													<xsl:value-of select="@PPP" />
												</td>
												<td align="center">
													<xsl:value-of select="@PeregonStancia" />
												</td>
												<td align="center">
													<xsl:value-of select="@km" />
												</td>
												<td align="center">
													<xsl:value-of select="@piket" />
												</td>
												<td align="center">
													<xsl:value-of select="@m" />
												</td>
												<td align="center">
													<xsl:value-of select="@Vpz" />
												</td>
												<td align="center">
													<xsl:value-of select="@ZazorR" />
												</td>
												<td align="center">
													<xsl:value-of select="@ZazorL" />
												</td>
												<td align="center">
													<xsl:value-of select="@T" />
												</td>
												<td align="center">
													<xsl:value-of select="@Zabeg" />
												</td>
												<td align="center">
													<xsl:value-of select="@PlanPuti" />
												</td>
												<td align="center">
													<xsl:value-of select="@Vdop" />
												</td>
												<td align="center">
													<xsl:value-of select="@Primech" />
												</td>
											</tr>
										</xsl:for-each>
									</td>
								</tr>
							</xsl:for-each>
						</table>
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

						<!-- <table style="width:80%" align="center" border="0" cellspacing="0" cellpadding="5">
                            <tr>
                                <td style="width:10%"></td>
                            </tr>
                            <tr>
                                <td align="left">ТС стр.48</td>
                                <td style="width:70%" align="left">3.1.11. Зазор в стыке, соседнем с изолирующим, должен быть не менее 3мм, а при низких температурах не превышать 18 мм при диаметре отверстий в рельсах 36 мм.</td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="width:70%" align="left">3.1.12. Рельсовые стыки обеих рельсовых нитей располагаются по наугольнику. Забег стыка по одной рельсовой нити относительно стыка другой нити должны быть на прямых не более 80 мм, на кривых – 80 мм плюс половина стандартного укорочения рельса (в данной кривой). Забег одного изолирующего стыка относительно другого допускается: на прямых – не более 50 мм; на кривых – 50 мм плюс половина стандартного укорочения рельса. Превышение указанных величин устраняется в плановом порядке в летний период. </td>
                            </tr>
                        </table> -->
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>