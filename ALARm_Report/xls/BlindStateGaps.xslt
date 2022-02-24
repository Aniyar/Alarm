<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>Два и более слепых стыковых зазоров подряд</title>
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
					<div style="page-break-before:always;">



						<table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Два и более слепых стыковых зазоров подряд</p>
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
										<xsl:value-of select="@type" />
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
									<td class="border-head" style="padding:2.5px;" rowspan="1">Нить</td>
									<td class="border-head" style="padding:2.5px;" width="5%" rowspan="1">T°</td>
									<!--<td class="border-head" style="padding:2.5px;" rowspan="1">Отступление</td> -->
									<td class="border-head" style="padding:2.5px;" width="10%" rowspan="1">Примечание</td>
								</tr>
							</thead>
							<xsl:for-each select="direction">
								<tr>
									<td align="right" style="padding:2.5px;" colspan="10">
										<b>
											<xsl:value-of select="@name" />
										</b>
										<!-- начало записи-->
									</td>
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
												<xsl:value-of select="@nit" />
											</td>
											<td align="center">
												<xsl:value-of select="@T" />
											</td>
											<!-- <td align="center">
                                    <xsl:value-of select="@Otst" />
                                </td> -->
											<td align="center">
												<xsl:value-of select="@Primech" />
											</td>
										</tr>
									</xsl:for-each>

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
					</div>
				</xsl:for-each>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>