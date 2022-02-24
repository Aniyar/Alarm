<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title> Отступления 2 степени, близкие к 3</title>
				<style>
					td {
					padding-left: 5px;
					padding: 2px;

					}

					.border {
					border: solid 1px #000;
					}

					.border-head {
					border-bottom: solid 1px #000;
					border-left: 1px solid black
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
					table.main, td.main, th.main {

					border-collapse: collapse;
					border: 1.5px solid black;
					font-size: 12px;
					font-family: 'Times New Roman';
					}

					table.main {
					width: 100%;
					margin: auto;
					}
					b {
					font-size: 12px;
					}
				</style>
				<style type="text/css" media="print">
					@media print {
					h1 {page-break-before: always;}
					}
				</style>
			</head>

			<body>
				<xsl:for-each select="report/trip">
					<div id="pageFooter" align="right" style="page-break-before:always;">

						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 12px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>

						<b>
							<p align="center" style="color:black; font-size:15px">Ведомость отступлений 2 степени, близких к 3</p>
						</b>
						<table style="width:100%;font-size: 12px;" align="center">
							<tr>
								<td align="left">
									ПЧ:									<xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:									<xsl:value-of select="@road" />
								</td>





							</tr>
							<tr>

								<td align="left">
									<xsl:value-of select="@ps" />
								</td>

								<td align="left">
									Проверка:
									<xsl:value-of select="@check" />
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;  <xsl:value-of select="@periodDate" />
								</td>

							</tr>
						</table>
						<div align="center">
							<table width="100%" border="0" cellpadding="5" cellspacing="0" class="border" align="center" style="font-size: 12px; border-collapse: collapse;">
								<thead>
									<tr>
										<td align="center" class="main">ПЧУ</td>
										<td align="center" class="main">ПД</td>
										<td align="center" class="main">ПДБ</td>

										<td align="center" class="main" width="20">Км</td>
										<td align="center" class="main" width="20">м</td>
										<td align="center" class="main">
											Дата											<br/>
											обнаружения
										</td>

										<td align="center" class="main">Отступления</td>
										<td align="center" class="main">
											Отклонение,											<br/>
											мм
										</td>
										<td align="center" class="main">Длина, м</td>
										<td align="center" class="main">Кол.</td>
										<td align="center" class="main" width="45">Примечание</td>
									</tr>
								</thead>

								<xsl:for-each select="direction">
									<tr>
										<td align="left" colspan="11">
											Направление:
											<xsl:value-of select="@direction" />
											<b>
												(												<xsl:value-of select="@directioncode" />
												)
											</b>  &#160;&#160;&#160;
											&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Путь: <b>
												<xsl:value-of select="@track" />
											</b>
										</td>
									</tr>
									<tr>
										<td align="center" colspan="11">

											<xsl:for-each select="track">
												<tr>
													<td align="left" colspan="11">

														<!-- начало записи-->
														<xsl:for-each select="PCHU">
															<tr>
																<td class="main" text-align="center" align="center" valign="top">
																	<xsl:attribute name="rowspan">
																		<xsl:value-of select="@recordCount" />
																	</xsl:attribute>
																	<xsl:value-of select="@number" />
																</td>
																<xsl:for-each select="PD">
																	<td class="main" text-align="center" align="center" valign="top">
																		<xsl:attribute name="rowspan">
																			<xsl:value-of select="@recordCount" />
																		</xsl:attribute>
																		<xsl:value-of select="@number" />
																	</td>
																	<xsl:for-each select="PDB">
																		<td class="main" text-align="center" align="center" valign="top">
																			<xsl:attribute name="rowspan">
																				<xsl:value-of select="@recordCount" />
																			</xsl:attribute>
																			<xsl:value-of select="@number" />
																		</td>
																		<xsl:for-each select="NOTE">

																			<td class="main" text-align="center" align="center" valign="top">
																				<xsl:value-of select=" @km" />
																			</td>
																			<td class="main" text-align="center" align="center" valign="top">
																				<xsl:value-of select="@meter" />
																			</td>
																			<td class="main" text-align="center" align="center" valign="top">
																				<xsl:value-of select="@founddate" />
																			</td>
																			<td class="main" text-align="center" align="center" valign="top">
																				<xsl:value-of select="@digression" />
																			</td>
																			<td class="main" text-align="center" align="center" valign="top">
																				<xsl:value-of select="@value" />
																			</td>
																			<td class="main" text-align="center" align="center" valign="top">
																				<xsl:value-of select="@length" />
																			</td>
																			<td class="main" text-align="center" align="center" valign="top">

																				<xsl:value-of select="@count" />
																			</td>
																			<td class="main" text-align="center" align="center" valign="top">

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
													<th align="left" colspan="10" class="main">
														Итого по пути													<xsl:value-of select="@name"/>
													</th>
													<th align="center" colspan="1" class="main">
														<xsl:value-of select="@totalCount" />
													</th>

												</tr>
												<tr>
													<th align="left" colspan="10" class="main">Итого по ПЧ                     </th>
													<th align="center" colspan="1" class="main">
														<xsl:value-of select="@countDistance"/>
													</th>
												</tr>



											</xsl:for-each>
										</td>
									</tr>
								</xsl:for-each>

							</table>

							<table width="100%" style="font-size: 12px;border-collapse: collapse;height: 5%;">
								<tr>
									<td colspan="2">В том числе:</td>
								</tr>
								<xsl:for-each select="total">
									<tr>
										<td width="10%"></td>
										<td>
											<xsl:value-of select="@totalinfo" />
										</td>
									</tr>
								</xsl:for-each>
							</table>


							<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
								<tr>
									<td>
										Начальник путеизмерителя:
										<xsl:value-of select="@ps" />
									</td>
									<td>
										<xsl:value-of select="@chief" />
									</td>
								</tr>
							</table>
						</div>
					</div>
				</xsl:for-each>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>