<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>(ДФ-з1) -Ведомость величин стыковых зазоров и температур рельсов </title>
				<style>
					td {
					padding-left: 5px;
					}
				</style>
			</head>

			<body>
				<xsl:for-each select="report/trip">
					<div style="page-break-before:always;">
						<b>
							<p align="center" style="color:black; font-size:14px">
								Ведомость величин стыковых зазоров и
								температур рельсов (Форма ДФ-з1)
							</p>
						</b>
						<table style="width:90%" align="center">
							<tr>
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

									<xsl:value-of select="@nput	" />
								</td>
							</tr>
							<tr>
								<td>
									Участок:  <xsl:value-of select="@uchastok" />
								</td>
								<td>
									ПЧ:

									<xsl:value-of select="@npch" />
								</td>
								<td>
									Км:

									<xsl:value-of select="@km" />
								</td>
							</tr>
						</table>
						<table width="90%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center"
							style="font-size: 12px;border-collapse: collapse;">
							<thead>
								<tr>
									<th class="border-head" style="padding:2.5px;" rowspan="2">км</th>
									<th class="border-head" style="padding:2.5px;" rowspan="2">м</th>
									<th class="border-head" style="padding:2.5px;" rowspan="2">Параметр</th>
									<th class="border-head" style="padding:2.5px;" rowspan="2">Величина</th>
									<th class="border-head" style="padding:2.5px;" rowspan="2">
										V<sub>пз</sub>
									</th>
									<th class="border-head" style="padding:2.5px;" rowspan="2">
										V<sub>огр</sub> (зазоры)
									</th>
								</tr>
							</thead>
							<xsl:for-each select="Note">
								<tr>
									<td align="center">
										<xsl:value-of select="@km" />
									</td>
									<td align="center">
										<xsl:value-of select="@m" />
									</td>
									<td align="center">
										<xsl:value-of select="@param" />
									</td>
									<td align="center">

										<xsl:choose>
											<xsl:when test="@VdopZazor != '' ">
												<b>
													<xsl:value-of select="@velichina" />
												</b>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="@velichina" />
											</xsl:otherwise>
										</xsl:choose>
									</td>
									<td align="center">
										<xsl:value-of select="@Vpz" />
									</td>
									<td align="center">
										<xsl:value-of select="@VdopZazor" />
									</td>
								</tr>
							</xsl:for-each>
						</table>
					</div>
				</xsl:for-each>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>