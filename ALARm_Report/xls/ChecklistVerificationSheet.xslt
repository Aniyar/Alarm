<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>
					Ведомость результатов измерений ширины колеи и уровня на
					контрольных участках
				</title>
				<style>
					td {
					padding-left: 5px;

					}

					td.info {
					text-align: center;
					font-weight: bold;
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
				<xsl:for-each select="report/trip">
					<div id="pageFooter" align="right" style="page-break-before:always;">

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<b>
							<p align="center" style="color:black; font-size:14px">
								Ведомость результатов измерений ширины колеи и уровня на<br/>
								контрольных участках
							</p>
						</b>
						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<td align="left">
									ПЧ:
									<xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:
									<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@ps" />
								</td>
								<td align="left">
									Проверка:
									<xsl:value-of select="@check" />  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;  <xsl:value-of select="@periodDate" />
								</td>
								<!-- <td align="left">
                               
                            </td> -->
							</tr>
						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center"
							style="font-size: 14px;border-collapse: collapse;">
							<thead>
								<tr>

									<td align="center" ROWSPAN="3">КМ</td>
									<td align="center" ROWSPAN="3">Дата</td>
									<td align="center" colspan="4">Параметр Уровень</td>
									<td align="center" colspan="4">Параметр Шаблон</td>
								</tr>
								<tr>
									<td align="center" colspan="2">измеренные</td>
									<td align="center" colspan="2">установленные</td>
									<td align="center" colspan="2">измеренные</td>
									<td  align="center" colspan="2">установленные</td>
								</tr>
								<tr>
									<td align="center">МО</td>
									<td align="center">СКО</td>
									<td align="center">МО</td>
									<td align="center">СКО</td>
									<td align="center">МО</td>
									<td align="center">СКО</td>
									<td align="center">МО</td>
									<td align="center">СКО</td>
								</tr>
								<xsl:for-each select="lev">
									<tr>
										<td align="left" colspan="11">
											Направление:
											<xsl:value-of select="@dirname" /><b>
												(<xsl:value-of select="@dircode" />)
											</b>   &#160;&#160;&#160;
											&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Путь:<b>
												<xsl:value-of select="@dirtrack" />
											</b>
										</td>
									</tr>
									<tr />
									<xsl:for-each select="note">


										<tr>

											<td align="center">
												<xsl:value-of select="@km" />
											</td>
											<td align="center">
												<xsl:value-of select="@mes" />


											</td>
											<td align="center">
												<xsl:choose>
													<xsl:when test="@ur1 - @ur3 > 1">
														<th>
															<u>
																<b>
																	<xsl:value-of select="ur1" />
																</b>
															</u>
														</th>
													</xsl:when>
													<xsl:otherwise>
														<xsl:value-of select="@ur1" />
													</xsl:otherwise>
												</xsl:choose>



												<!-- <xsl:value-of select="@ur1" /> -->
											</td>
											<td align="center">
												<xsl:value-of select="@ur2" />
											</td>
											<td align="center">
												<xsl:value-of select="@ur3" />
											</td>
											<td align="center">
												<xsl:value-of select="@ur4" />
											</td>
											<td align="center">
												<xsl:value-of select="@sh1" />
											</td>
											<td align="center">
												<xsl:value-of select="@sh2" />
											</td>
											<td align="center">
												<xsl:value-of select="@sh3" />
											</td>
											<td align="center">
												<xsl:value-of select="@sh4" />
											</td>
											<tr />
										</tr>
									</xsl:for-each>
								</xsl:for-each>

							</thead>
						</table>
						<table width="100%;" border="0" cellpadding="0" cellspacing="0" class="demotable" align="center" style="font-size: 12px;border-collapse: collapse;height: 5%;">
							<td align="left">	Всего 1 проезд КУ. Параметры Шаблон в норме 1 проезда. Параметры Уровень в норме 0 проезда. </td>

						</table>
						<table width="100%;" border="0" cellpadding="0" cellspacing="0" class="demotable" align="center" style="font-size: 12px;border-collapse: collapse;height: 5%;">
							<tr>
								<td  align="left">
									Зам. начальника  <xsl:value-of select="@ps" />
								</td>
								<td>
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