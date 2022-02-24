<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>Ведомость величины передвижки рельсов</title>
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
					td {
					padding-left: 5px;
					}
					<!-- .demotable {
                    border-collapse: collapse;
                    counter-reset: schetchik -1;
                    }
                    .demotable tbody tr {
                    counter-increment: schetchik;
                    }
                    .demotable td,
                    .demotable tbody tr:before {
                    padding: .1em .5em;
                    border: 1px solid #E7D5C0;
                    }
                    .demotable tbody tr:before {
                    display: table-cell;
                    vertical-align: middle;
                    }
                    .demotable tbody tr:before,
                    .demotable b:after {
                    content: counter(schetchik);
                    color: #978777;
                    } -->
				</style>
			</head>
			<body>
				<div style = "page-break-before:always;">
					<xsl:for-each select="report/trip">


						<table width="100%" cellpadding="0" cellspacing="0"  class="demotable"  align="center" style="font-size: 14px; font-family: Times New Roman; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px;">Ведомость величины передвижки рельсов</p>
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
										<xsl:value-of select="@date_statement" />
									</b>

								</td>
								<!-- <td  align="left"> Проезд:                                    <b>
                                        <xsl:value-of select="@date_statement" />
                                    </b>
                                </td> -->


							</tr>
						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0"  class="demotable"  align="center" style="font-size: 14px; font-family: Times New Roman; width: 100%; margin: auto;">
							<thead>
								<tr>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="2">№ п/п</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="2">км</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="2">пк</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="2">метр</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="2">стык</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="2">
										Номинальный

										<br/> зазор, мм

									</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="1" colspan="4">левая нить</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="1" colspan="4">правая нить</td>
								</tr>
								<tr>
									<td class="border-head" style="padding:2.5px;" align="center"  width="5%" rowspan="1">зазор, мм</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="1">
										накопленый

										<br/> зазор,мм

									</td>
									<td class="border-head" style="padding:2.5px;" align="center"  rowspan="1">
										накопленый

										<br/> номинальный

										<br/> зазор,мм

									</td>
									<td class="border-head" style="padding:2.5px;" align="center"  width="5%" rowspan="1">
										передвижки

										<br/> рельсов, мм

									</td>
									<td class="border-head" style="padding:2.5px;" align="center"  width="5%" rowspan="1">зазор, мм</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="1">
										накопленый

										<br/> зазор,мм

									</td>
									<td class="border-head" style="padding:2.5px;"  align="center" rowspan="1">
										накопленый

										<br/> номинальный

										<br/> зазор,мм

									</td>
									<td class="border-head" style="padding:2.5px;" align="center" width="5%" rowspan="1">
										передвижки

										<br/> рельсов, мм

									</td>
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
													<xsl:value-of select="@km" />
												</td>
												<td align="center">
													<xsl:value-of select="@piket" />
												</td>
												<td align="center">
													<xsl:value-of select="@m" />
												</td>
												<td align="center">
													<xsl:value-of select="@styk" />
												</td>
												<td align="center">
													<xsl:value-of select="@nominalZazor" />
												</td>
												<td align="center">
													<xsl:value-of select="@zazorL" />
												</td>
												<td align="center">
													<xsl:value-of select="@napolZazorL" />
												</td>
												<td align="center">
													<xsl:value-of select="@napolNominalZazorL" />
												</td>
												<td align="center">
													<xsl:value-of select="@paredvRelsL" />
												</td>
												<td align="center">
													<xsl:value-of select="@zazorR" />
												</td>
												<td align="center">
													<xsl:value-of select="@napolZazorR" />
												</td>
												<td align="center">
													<xsl:value-of select="@napolNominalZazorR" />
												</td>
												<td align="center">
													<xsl:value-of select="@paredvRelsR" />
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
					</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>