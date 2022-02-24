<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>ВЕДОМОСТЬ КРИВЫХ С БОКОВЫМ ИЗНОСОМ БОЛЕЕ ПОРОГОВОЙ ВЕЛИЧИНЫ</title>
				<style>
					table.main {
					border-collapse:
					collapse;
					margin:auto;
					width: 100%;
					}
					table.main,
					td.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;

					}
					b {
					font-size: 14px;
					}
				</style>
			</head>
			<body>
				<div align="center" style="margin:30px 0px 0px">

					<p align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
						<xsl:value-of select="report/@version" />
					</p>

					<H4 align="center">Ведомость кривых с боковым износом более пороговой величины (ФП-3.7) </H4>
				</div>
				<div align="center">
					<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse;">
						<!-- <tr>
                            <td align="left">ПЧ:                               <xsl:value-of select="report/@distance"/>
                            </td>
                            <td align="left" colspan="2">
                                <xsl:value-of select="report/@road"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">   <xsl:value-of select="report/@ps"/>
                            </td>
                              <td align="left"> Проверка:  <xsl:value-of select="report/@Check"/>
                            </td>
                            <td align="left">
                                <xsl:value-of select="report/@tripdate"/>
                            </td>
                            <td align="right" style="font-style:italic;">Пороговое значение износа:                                <xsl:value-of select="report/@wear"/>
 мм.</td>
                        </tr> -->

						<tr>
							<td>
								ПЧ:
								<xsl:value-of select="report/@distance" />
							</td>
							<td>
								Дорога:
								<xsl:value-of select="report/@road" />
							</td>

						</tr>
						<tr>
							<td>

								<xsl:value-of select="report/@ps" />
							</td>
							<td align="left">
								Проверка:<xsl:value-of select="report/@check" />
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								<xsl:value-of select="report/@tripdate" />
							</td>

							<td align="right" style="font-style:italic;">
								Пороговое значение износа:
								<xsl:value-of select="report/@wear" />
								мм.
							</td>
						</tr>
					</table>
					<table class="main">
						<thead>
							<tr>
								<td colspan="3" align="center" class="main">Кривая</td>
								<td colspan="3" align="center" class="main">Износ, мм</td>
								<td colspan="3" align="center" class="main">Длина, мм</td>
								<td rowspan="2" align="center" class="main">Vпз</td>
								<td rowspan="2" align="center" class="main">Vогр</td>
							</tr>
							<tr>
								<td align="center" class="main">Напр.</td>
								<td align="center" class="main">
									Начало - Конец,<br />км.м
								</td>
								<td align="center" class="main">
									Радиус,<br />м
								</td>
								<td align="center" class="main">Сред./Макс.</td>
								<td align="center" class="main">№ПК</td>
								<td align="center" class="main">Сред./Макс.</td>
								<td align="center" class="main">12-15мм</td>
								<td align="center" class="main">16-20мм</td>
								<td align="center" class="main">>20мм</td>
							</tr>
							<tr>
								<th align="right" colspan="11" style="font-size: 13px;">
									<xsl:value-of select="report/direction/@track_info" />
								</th>
							</tr>
						</thead>
						<tbody id="this">
							<xsl:for-each select="report/direction/curve">
								<xsl:for-each select="./PC">
									<xsl:if test="position() = 1">
										<tr>
											<td rowspan="{../@count_PC}" align="center" valign="top" class="main">
												<xsl:value-of select="../@side" />
											</td>
											<td rowspan="{../@count_PC}" align="center" valign="top" class="main">
												<xsl:value-of select="../@start_km" />
												.
												<xsl:value-of select="../@start_m" />
												-
												<xsl:value-of select="../@final_km" />
												.
												<xsl:value-of select="../@final_m" />
											</td>
											<td rowspan="{../@count_PC}" align="center" valign="top" class="main">
												<xsl:value-of select="../@radius" />
											</td>
											<td rowspan="{../@count_PC}" align="center" valign="top" class="main">
												<xsl:value-of select="../@wear_mid" />
												/
												<xsl:value-of select="../@wear_max" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@order" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@wear_mid" />
												/
												<xsl:value-of select="@wear_max" />
											</td>

											<td align="center" class="main">
												<xsl:value-of select="@len1215" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@len1620" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@len20" />
											</td>

											<td align="center" class="main">
												<xsl:value-of select="@Vpz" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@Vogr" />
											</td>

										</tr>
									</xsl:if>
									<xsl:if test="position() != 1 and position() != last()">
										<tr>
											<td align="center" class="main">
												<xsl:value-of select="@order" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@wear_mid" />
												/
												<xsl:value-of select="@wear_max" />
											</td>

											<td align="center" class="main">
												<xsl:value-of select="@len1215" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@len1620" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@len20" />
											</td>

											<td align="center" class="main">
												<xsl:value-of select="@Vpz" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@Vogr" />
											</td>
										</tr>
									</xsl:if>
									<xsl:if test="position() = last() and position() != 1">
										<tr>
											<td align="center" class="main">
												<xsl:value-of select="@order" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@wear_mid" />
												/
												<xsl:value-of select="@wear_max" />
											</td>

											<td align="center" class="main">
												<xsl:value-of select="@len1215" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@len1620" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@len20" />
											</td>

											<td align="center" class="main">
												<xsl:value-of select="@Vpz" />
											</td>
											<td align="center" class="main">
												<xsl:value-of select="@Vogr" />
											</td>
										</tr>
									</xsl:if>
								</xsl:for-each>
							</xsl:for-each>
						</tbody>
					</table>
					<table width="90%">
						<tr>
							<td align="left">
								Зам.СПС
								<xsl:value-of select="report/@ps" />
							</td>
							<td align="right">
								<xsl:value-of select="report/@chief" />
							</td>
						</tr>
					</table>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>