<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>ФП-4.3.Ведомость элементов продольного профиля пути</title>
				<style>
					.pages {
					page-break-after:always;
					}
					table.main {
					border-collapse: collapse; margin:auto;
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
				<xsl:for-each select="report/pages">
					<div class="pages">

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>
						<H4 align = "center">Ведомость элементов продольного профиля пути (ФП-4.3)</H4>
						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;">
							<!-- <tr>
               <td align="center" colspan="4">Ведомость элементов продольного профиля пути (ФП-4.3)</td>
              </tr> -->
							<tr>
								<td>
									ПЧ:
									<xsl:value-of select="@distance" />
								</td>
								<td>
									Дорога:
									<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td>

									<xsl:value-of select="@car" />
								</td>
								<td>
									Проверка:
									<xsl:value-of select="@trip_info" />
								</td>
								<td>
									<xsl:value-of select="@period" />
								</td>
							</tr>

						</table>
						<table class="main">
							<thead>
								<tr>
									<td class="main" align="center" colspan="3">Точки перелома профиля</td>
									<td class="main" align="center" colspan="2">Элементы профиля</td>
									<td class="main" align="center" rowspan="2">
										Разность смежных уклонов,<br/> &#8240;
									</td>
								</tr>
								<tr>
									<td class="main" align="center">км</td>
									<td class="main" align="center">м</td>
									<td class="main" align="center">Отметка, см</td>
									<td class="main" align="center">Длина, м</td>
									<td class="main" align="center">Уклон, &#8240;</td>
								</tr>
							</thead>
							<tbody>
								<tr>
									<th class="main" align="right" colspan="8">
										<xsl:value-of select="@tripinfo" />
									</th>
								</tr>
								<xsl:for-each select="elements">
									<tr>
										<td class="main" align="center">
											<xsl:value-of select="@km" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@meter" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@mark" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@length" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@slope" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@slopedifference" />
										</td>
									</tr>
								</xsl:for-each>
							</tbody>
						</table>
						<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник                                    <xsl:value-of select="@car" />
								</td>
								<td align="right">
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