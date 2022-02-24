<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>(ДФ-зЗ)Ведомость сверхнормативных стыковых зазоров, требующих ограничения скорости движения</title>
				<style>
					td {
					padding-left: 5px;
					text-align: center;
					}

					td.info {
					text-align: center;
					font-weight: bold;
					}
				</style>
			</head>

			<body>
				<xsl:for-each select="report/trip">
					<div style="page-break-before:always;">
						<b>

							<p align="center" style="color:black; font-size:14px">
								Ведомость сверхнормативных стыковых
								зазоров, требующих ограничения скорости движения(ДФ-зЗ)
							</p>
						</b>
						<table style="width:90%" align="center">
							<tr>
								<td>
									ПС:

									<xsl:value-of select="@ps" />
								</td>

								<td>
									Направление:

									<xsl:value-of select="@direction" />
								</td>
								<td>
									Поездка:

									<xsl:value-of select="@date_statement" />
								</td>

								<td>
									Путь:

									<xsl:value-of select="@track	" />
								</td>
							</tr>
							<tr>
								<td>
									Участок:  <xsl:value-of select="@road" />
								</td>
								<td>
									ПЧ:

									<xsl:value-of select="@distance" />
								</td>
								<td>
									Км:

									<xsl:value-of select="@km" />
								</td>
							</tr>


						</table>
						<table width="90%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center"
							style="font-size: 10px;border-collapse: collapse;">
							<tr>
								<th colspan="6">
									Левая нить
								</th>
								<th colspan="6">
									Правая нить
								</th>
							</tr>
							<tr>
								<th colspan="4">
									Зазоры
								</th>
								<th colspan="2">
									Скорость
								</th>
								<th colspan="4">
									Зазоры
								</th>
								<th colspan="2">
									Скорость
								</th>
							</tr>
							<tr>
								<th class="border-head" style="padding:2.5px;" rowspan="2">км</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">м</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">Величина, мм</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">Т, °С</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">
									V
									<sub>пз</sub>
								</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">
									V
									<sub>дп</sub>
								</th>

								<th class="border-head" style="padding:2.5px;" rowspan="2">км</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">м</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">Величина, мм</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">Т, °С</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">
									V
									<sub>пз</sub>
								</th>
								<th class="border-head" style="padding:2.5px;" rowspan="2">
									V
									<sub>дп</sub>
								</th>
							</tr>
							<tr />
							<xsl:for-each select="lev">
								<xsl:for-each select="Note">
									<tr>
										<td align="center">
											<xsl:value-of select="@lkm" />
										</td>
										<td align="center">
											<xsl:value-of select="@lm" />
										</td>
										<td align="center">
											<xsl:value-of select="@lvelich" />
										</td>
										<td align="center">
											<xsl:value-of select="@lt" />
										</td>
										<td align="center">
											<xsl:value-of select="@lvpz" />
										</td>
										<td align="center">
											<xsl:value-of select="@lvdp" />
										</td>

										<td align="center">
											<xsl:value-of select="@rkm" />
										</td>

										<td align="center">
											<xsl:value-of select="@rm" />
										</td>
										<td align="center">
											<xsl:value-of select="@rvelich" />
										</td>
										<td align="center">
											<xsl:value-of select="@rt" />
										</td>
										<td align="center">
											<xsl:value-of select="@rvpz" />
										</td>
										<td align="center">
											<xsl:value-of select="@rvdp" />
										</td>
									</tr>
								</xsl:for-each>
								<tr>
									<td class="info" colspan="6">
										Величина стыкового зазора,ш
									</td>
									<th colspan="6">
										Допустимая скорость,км/ч
									</th>
								</tr>
								<tr>
									<td class="info" colspan="6">
										<xsl:text>Более 24 мм до 26 мм:</xsl:text>
										<xsl:value-of select="./@boleeFirst" />
										ш
									</td>
									<th colspan="6">
										100
									</th>
								</tr>
								<tr>
									<td class="info" colspan="6">
										<xsl:text>Более 26 мм до 30 мм:</xsl:text>
										<xsl:value-of select="./@boleeSecond" />
										ш
									</td>
									<th colspan="6">
										60
									</th>
								</tr>
								<tr>
									<td class="info" colspan="6">
										<xsl:text>Более 30 мм до 35 мм:</xsl:text>
										<xsl:value-of select="./@boleeTherd" />
										ш
									</td>
									<th colspan="6">
										25
									</th>
								</tr>
								<tr>
									<td class="info" colspan="6">
										<xsl:text>Более 35 мм:</xsl:text>
										<xsl:value-of select="./@boleeFourth" />
										ш
									</td>
									<th colspan="6">
										Движение закрываеться
									</th>
								</tr>
							</xsl:for-each>
						</table>
						<!-- <table width="90%" border="0" cellpadding="0" cellspacing="0"  class="demotable"  align="center" style="font-size: 10px;border-collapse: collapse;">
                            <tr>
                                <td style="text-align:justify;">
                                    &#160;&#160;&#160;&#160;Приложение: кадры подвагонной видеосъемки (пакет)
                                </td>
                            </tr>
                        </table> -->

					</div>
				</xsl:for-each>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>