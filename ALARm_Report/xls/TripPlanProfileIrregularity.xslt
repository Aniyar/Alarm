<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>

			<head>
				<title>Ведомость характеристик сверхнормативных длинных неровностей в плане и профиле (ДФ-ДН)</title>
				<style>
					.pages {
					page-break-after: always;
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
					font-family: Arial;
					border: 1.5px solid black;
					}
				</style>
			</head>

			<body>
				<xsl:for-each select="report/pages">
					<div  class="pages">


						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<H4 align = "center">
							Ведомость характеристик сверхнормативных <br/>длинных неровностей в плане и профиле (ДФ-ДН)
						</H4>


						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>

								<th align="left">
									ПЧ:
									<xsl:value-of select="@distance" />
								</th>
								<th align="left">
									Дорога:
									<xsl:value-of select="@road" />
								</th>

							</tr>
							<tr>
								<th align="left">
									<xsl:value-of select="@car" />
								</th>
								<th align="left">
									Поверка:
									<xsl:value-of select="@trip_info" />
								</th>
								<th align="left" >
									<xsl:value-of select="@period" />
								</th>
							</tr>

						</table>
						<table class="main">
							<thead>
								<tr>
									<th class="main" align="center">Путь</th>
									<th class="main" align="center">Местоположение (км, пк)</th>
									<th class="main" align="center">Величина неровности (мм)</th>
									<th class="main" align="center">Крутизна отвода (мм/м)</th>
									<th class="main" align="center">Примечание</th>
								</tr>
								<tr>
									<td class="main" align="center" colspan="5">Сверхнормативные неровности в плане</td>
								</tr>
							</thead>
							<tbody>

								<xsl:for-each select="plan">
									<xsl:for-each select="elements">
										<xsl:if test="position() = 1">
											<tr>
												<td class="main" valign="top" align="center" rowspan="{../@count}">
													<xsl:value-of select="@track" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@km" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@irregularity" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@tapvalue" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@notice" />
												</td>
											</tr>
										</xsl:if>
										<xsl:if test="position() != 1">
											<tr>
												<td class="main" align="center">
													<xsl:value-of select="@km" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@irregularity" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@tapvalue" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@notice" />
												</td>
											</tr>
										</xsl:if>
									</xsl:for-each>
								</xsl:for-each>
								<tr>
									<td class="main" align="center" colspan="5">Сверхнормативные неровности в профиле</td>
								</tr>
								<xsl:for-each select="profile">
									<xsl:for-each select="elements">
										<xsl:if test="position() = 1">
											<tr>
												<td class="main" valign="top" align="center" rowspan="{../@count}">
													<xsl:value-of select="@track" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@km" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@irregularity" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@tapvalue" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@notice" />
												</td>
											</tr>
										</xsl:if>
										<xsl:if test="position() != 1">
											<tr>
												<td class="main" align="center">
													<xsl:value-of select="@km" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@irregularity" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@tapvalue" />
												</td>
												<td class="main" align="center">
													<xsl:value-of select="@notice" />
												</td>
											</tr>
										</xsl:if>
									</xsl:for-each>
								</xsl:for-each>
							</tbody>
						</table>
						<table style="font-size: 12px; width: 90%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@distance" />
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
									&#160;&#160;&#160;&#160;&#160;
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