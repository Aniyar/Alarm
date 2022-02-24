<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>Ведомость по эпюре на звене</title>
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
				<xsl:for-each select="report/trip">
					<div style = "page-break-before:always;">

						<table style="font-size: 14px; font-family: Times New Roman; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Ведомость по эпюре на звене</p>
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
									&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@trip_date" />
									</b>

								</td>
							</tr>
						</table>

						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<!-- <td  class="main" align="center">км</td>
                                    <td  class="main" align="center">метр</td>
                                    <td  class="main" align="center">№ звена</td>
                                    <td  class="main" align="center">Эпюра фактическая</td>
                                    <td  class="main" align="center">Эпюра оценочная</td>
                                    <td  class="main" align="center">Нить</td> -->
								<tr>
									<td class="border-head" style="padding:1.5px;" rowspan="1">№ п/п</td>
									<td class="border-head" style="padding:1.5px;" rowspan="1">км</td>
									<td class="border-head" style="padding:1.5px;" rowspan="1">пк</td>
									<td class="border-head" style="padding:1.5px;" rowspan="1">№ звена</td>
									<td class="border-head" style="padding:1.5px;" rowspan="1">Эпюра фактическая</td>
									<td class="border-head" style="padding:1.5px;" rowspan="1">Эпюра оценочная</td>
									<td class="border-head" style="padding:1.5px;" rowspan="1">Примечание</td>
								</tr>
							</thead>
							<xsl:for-each select="direction">
								<tr>
									<td align="right" colspan="10">
										<b>
											<xsl:value-of select="@name" />
										</b>
										<!-- начало записи-->
										<xsl:for-each select="Note">
											<tr>
												<td align="center">
													<xsl:value-of select="@iter" />
												</td>
												<td align="center">
													<xsl:value-of select="@km" />
												</td>
												<td align="center">
													<xsl:value-of select="@m" />
												</td>
												<td align="center">
													<xsl:value-of select="@n" />
												</td>
												<td align="center">
													<xsl:value-of select="@epure_fact" />
												</td>
												<td align="center">
													<xsl:value-of select="@epure_ocen" />
												</td>
												<td align="center">
													<xsl:value-of select="@primech" />
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