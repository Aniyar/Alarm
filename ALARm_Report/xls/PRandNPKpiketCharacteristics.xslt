<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>

			<head>
				<title>ДФ-ПР3 - Ведомость попикетных характеристик ПР и НПК</title>
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
				<xsl:for-each select="report/pages">
					<div align="right" style="page-break-before:always;">
						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>


						<H4 align="center">Ведомость попикетных характеристик   ПР и НПК (ДФ-ПР3) </H4>
						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;">
							<tr>
								<td align="left">
									ПЧ:
									<xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:
									<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@car" />
								</td>
								<td align="left">
									Проверка:
									<xsl:value-of select="@check" />
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									<xsl:value-of select="@period" />
								</td>

							</tr>
						</table>


						<table class="main">
							<thead>
								<tr>
									<td class="main" align="center">Км</td>
									<td class="main" align="center">Пикет</td>
									<td class="main" align="center">Рельс (л/пр)</td>
									<td class="main" align="center">Радиус, м</td>
									<td class="main" align="center">ПР сред.</td>
									<td class="main" align="center">ПР СКО</td>
									<td class="main" align="center">НПК сред.</td>
									<td class="main" align="center">НПК СКО</td>
									<td class="main" align="center">Vуст</td>
								</tr>

							</thead>
							<tbody id="this">
								<xsl:for-each select="tracks">
									<tr>
										<th align="right" colspan="9" style="border-bottom:0;" class="main">
											<xsl:value-of select="@trackinfo" />
										</th>
									</tr>
									<xsl:for-each select="elements">

										<tr>
											<td class="main" align="right">
												<xsl:value-of select="@km" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@piket" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@railside" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@radius" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@PRmedium" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@PRSKO" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@NPKmedium" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@NPKSKO" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@speed" />
											</td>
										</tr>


									</xsl:for-each>

								</xsl:for-each>
							</tbody>
						</table>
						<table style="font-size: 12px; width: 95%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@car" />
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