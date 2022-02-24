<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>(Форма ДФ-ПР1)Ведомость распределения величин подуклонки рельсов </title>
				<style>

					.pages {
					page-break-before:always;
					}
					table.main {
					border-collapse: collapse;
					margin: auto;
					width: 100%;
					}
					table.main,
					td.main,
					th.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
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
					<div class="pages" id = "pageFooter" align="right" style="page-break-before:always;">

						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<H4 align = "center">Ведомость распределения величин подуклонки рельсов (ДФ-ПР1)</H4>

						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">

							<tr>
								<td align="left">
									ПЧ:                                    <xsl:value-of select="@pch" />
								</td>
								<td align="left">
									Дорога:                                    <xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@ps" />
								</td>
								<td align="left">
									Проверка:                                    <xsl:value-of select="@type"/>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="@date_statement" />
								</td>

							</tr>

						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center" style="border-collapse: collapse;">

							<thead>
								<tr >
									<td class="border-head" align="center" valign="middle" style="padding:2.5px;padding-left: 5px;" rowspan="1">км</td>
									<td class="border-head" align="center" valign="middle" style="padding:2.5px;" rowspan="1">
										<b>менее 1/60</b>
									</td>
									<td class="border-head" align="center" valign="middle" style="padding:2.5px;" rowspan="1">От 1/60 до 1/30</td>
									<td class="border-head" align="center" valign="middle" style="padding:2.5px;" rowspan="1">От 1/30 до 1/15</td>
									<td class="border-head" align="center" valign="middle" style="padding:2.5px;" rowspan="1">От 1/15 до 1/12 </td>
									<td class="border-head" align="center" valign="middle" style="padding:2.5px;" rowspan="1">
										<b>более 1/12</b>
									</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1">V ycт</td>
								</tr>
							</thead>
							<tbody id="this">


								<xsl:for-each select="./data/Pch">
									<tr>
										<th align="right" colspan="9" style="border-bottom:0;" class="main">
											<xsl:value-of select="@trackinfo"/>
										</th>
									</tr>
									<xsl:for-each select="Put">
										<xsl:for-each select="Prop">
											<tr class="main">
												<td class="main" align="center">
													<xsl:value-of select="@kM_a" />
												</td>
												<td class="main" align="center">
													<b>
														<xsl:value-of select="@v1" />
													</b>
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@v2" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@v3" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@v4" />
												</td>
												<td class="main" align="center">
													<b>
														<xsl:value-of select="@v5" />
													</b>
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@v" />
												</td>
											</tr>
										</xsl:for-each>

									</xsl:for-each>
								</xsl:for-each>
							</tbody>
						</table>
						<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@ps" />
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