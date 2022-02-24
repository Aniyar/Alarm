<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<style>                    td {                        padding-left: 5px;                    }                </style>
			</head>
			<body>
				<xsl:for-each select="report/trip">
					<div style = "page-break-before:always;">


						<table style="font-size: 14px; font-family: Arial; width: 90%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Ведомость состояния стыковых зазоров</p>
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
								<td align="left">
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
									:									<b>
										<xsl:value-of select="@date_statement" />
									</b>

								</td>
							</tr>
						</table>
						<table width="90%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<tr>
									<th class="border-head" style="padding:2.5px;" rowspan="1">№ п/п</th>
									<th class="border-head" style="padding:2.5px;" width="15%" rowspan="1">ПЧУ, ПД, ПДБ</th>
									<th class="border-head" style="padding:2.5px;" width="15%" rowspan="1">Перегон, станция</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">км</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">пк</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">метр</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Vпз</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Зазор правой нити</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Зазор левой нити</th>
									<th class="border-head" style="padding:2.5px;" width="5%" rowspan="1">T°</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Забег</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Vдоп</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Отступление</th>
									<th class="border-head" style="padding:2.5px;" width="10%" rowspan="1">Примечание</th>
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
													<xsl:value-of select="@Vdop" />
												</td>
												<td align="center">
													<xsl:value-of select="@Otst" />
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
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>