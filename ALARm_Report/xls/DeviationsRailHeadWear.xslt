<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<style>          td {              padding-left: 5px;          }      </style>
			</head>
			<body>
				<xsl:for-each select="report/trip">
					<div style = "page-break-before:always;">
						<b>
							<p align="center" style="color:black; font-size:15px">Отступления износа головки рельса</p>
						</b>
						<table style="width:90%" align="center">
							<tr>
								<td>
									ПЧ:                                <xsl:value-of select="@distance" />
								</td>
								<td>
									Дорога:                                <xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td>
									ПС:                                <xsl:value-of select="@ps" />
								</td>
								<td>
									Проверка:                                <xsl:value-of select="@check" />
								</td>
								<td>
									<xsl:value-of select="@periodDate" />
								</td>
							</tr>
						</table>
						<table width="90%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<tr>
									<th class="border-head" style="padding:2.5px;" rowspan="1">ПЧУ</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">ПД</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">ПДБ</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Км</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Метр</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Отступление</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Откл, мм</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Длина, м</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Уст.ск.</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Огр.ск.</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Прим</th>
								</tr>
							</thead>
							<xsl:for-each select="direction">
								<tr>
									<td align="center" style="padding:2.5px;" colspan="13">
										<b>
											Направление:                                        <xsl:value-of select="@name" />
										</b>
										<xsl:for-each select="track">
											<tr>
												<td align="left" style="padding:2.5px;" colspan="13">
													<b>
														&#8195;&#8195;&#8195;&#8195; Путь:                                                    <xsl:value-of select="@name" />
													</b>
													<!-- начало записи-->
													<xsl:for-each select="PCHU">
														<tr>
															<td align="center" valign="top" style="padding:2.5px;">
																<xsl:attribute name="rowspan">
																	<xsl:value-of select="@recordCount" />
																</xsl:attribute>
																<xsl:value-of select="@number" />
															</td>
															<xsl:for-each select="PD">
																<td align="center" valign="top" style="padding:2.5px;">
																	<xsl:attribute name="rowspan">
																		<xsl:value-of select="@recordCount" />
																	</xsl:attribute>
																	<xsl:value-of select="@number" />
																</td>
																<xsl:for-each select="PDB">
																	<td align="center" valign="top" style="padding:2.5px;">
																		<xsl:attribute name="rowspan">
																			<xsl:value-of select="@recordCount" />
																		</xsl:attribute>
																		<xsl:value-of select="@number" />
																	</td>
																	<xsl:for-each select="NOTE">
																		<td>
																			<xsl:value-of select="@km" />
																		</td>
																		<td>
																			<xsl:value-of select="@meter" />
																		</td>
																		<td>
																			<xsl:value-of select="@digression" />
																		</td>
																		<td>
																			<xsl:value-of select="@value" />
																		</td>
																		<td>
																			<xsl:value-of select="@length" />
																		</td>
																		<td>
																			<xsl:value-of select="@setSpeed" />
																		</td>
																		<td>
																			<xsl:value-of select="@limitedSpeed" />
																		</td>
																		<td>
																			<xsl:value-of select="@primech" />
																		</td>
																		<tr />
																	</xsl:for-each>
																</xsl:for-each>
															</xsl:for-each>
														</tr>
													</xsl:for-each>
												</td>
											</tr>
											<tr>
												<th align="left" style="padding:2.5px;" colspan="13">
													Всего по пути                                                <xsl:value-of select="@name" />                                                -                                                <xsl:value-of select="@recordCount" />                                                шт.
												</th>
											</tr>
										</xsl:for-each>
									</td>
								</tr>
							</xsl:for-each>
						</table>
						<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
							<tr>
								<td style="width:10%">
									Итого по ПЧ:                                <xsl:value-of select="@totalCount" />
								</td>
							</tr>
						</table>
						<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
							<tr>
								<td>
									Начальник путеизмерителя:                                <xsl:value-of select="@ps" />
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