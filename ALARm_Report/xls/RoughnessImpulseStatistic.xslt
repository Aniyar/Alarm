<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>(Ф.ДП2) – Статистические показатели импульсных неровностей</title>
				<style>
					.pages {
					page-break-before:always;
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
					height: 1%;
					width: 95%;
					font-size:10px;
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
					<!-- <div align="center" class="pages"> -->
					<div align="right" id="pageFooter">

						<p  align="left" style="color:black;width: 110%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>
						<H4 align = "center">Статистические показатели импульсных неровностей (ф.ДП2)</H4>
						<!-- <b> <p align="center" style="color:black; font-size:14px">Статистические показатели импульсных неровностей (ф.ДП2)</p></b> -->

						<table  style="font-size: 12px; width: 110%;border-collapse: collapse; margin:auto;margin-bottom:8px; ">

							<tr>
								<td align="left">
									ПЧ:<xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:<xsl:value-of select="@way" />
								</td>
							</tr>
							<tr >
								<td align="left">
									<xsl:value-of select="@DKI" />
								</td>
								<td align="left">
									Проверка:<xsl:value-of select="@check" /> &#160;&#160;&#160;&#160;&#160;&#160; <xsl:value-of select="@periodDate" />
								</td>
							</tr>

						</table>
					</div>
					<div>
						<table class="main">
							<thead>
								<tr>
									<td class="main" align="center" valign="middle" rowspan="3">Км</td>
									<td class="main" align="center" colspan="8">Количество ИН с глубиной, мм</td>
								</tr>
								<tr>
									<td class="main" align="center" colspan="4">Правая нить</td>
									<td class="main" align="center" colspan="4">Левая нить</td>
								</tr>
								<tr>
									<td class="main" align="center">до 1.0</td>
									<td class="main" align="center">от 1.0 до 2.0</td>
									<td class="main" align="center">от 2.0 до 3.0</td>
									<td class="main" align="center">более 3.0</td>
									<td class="main" align="center">до 1.0</td>
									<td class="main" align="center">от 1.0 до 2.0</td>
									<td class="main" align="center">от 2.0 до 3.0</td>
									<td class="main" align="center">более 3.0</td>
								</tr>

							</thead>
							<xsl:for-each select="trips">
								<tr>
									<th class="main" align="right" colspan="9">
										<xsl:value-of select="@tripinfo" />
									</th>
								</tr>
								<xsl:for-each select="elements">
									<tr>
										<td align="right">
											<xsl:value-of select="@km" />
										</td>
										<td align="right">
											<xsl:value-of select="@rightbefore1" />
										</td>
										<td align="right">
											<xsl:value-of select="@rightbefore2" />
										</td>
										<td align="right">
											<xsl:value-of select="@rightbefore3" />
										</td>
										<td align="right">
											<xsl:value-of select="@rightafter3" />
										</td>
										<td align="right">
											<xsl:value-of select="@leftbefore1" />
										</td>
										<td align="right">
											<xsl:value-of select="@leftbefore2" />
										</td>
										<td align="right">
											<xsl:value-of select="@leftbefore3" />
										</td>
										<td align="right">
											<xsl:value-of select="@leftafter3" />
										</td>
									</tr>
								</xsl:for-each>
								<tr class="main">
									<td class="main" align="center" valign="middle"  border="1">Итого      </td>
									<xsl:for-each select="elements">
										<td class="main" align="right">
											<xsl:value-of select="@rightbefore1" />
										</td>
										<td class="main" align="right">
											<xsl:value-of select="@rightbefore2" />
										</td>
										<td  class="main" align="right">
											<xsl:value-of select="@rightbefore3" />
										</td>
										<td  class="main" align="right">
											<xsl:value-of select="@rightafter3" />
										</td>
										<td class="main" align="right">
											<xsl:value-of select="@leftbefore1" />
										</td>
										<td class="main" align="right">
											<xsl:value-of select="@leftbefore2" />
										</td>
										<td class="main" align="right">
											<xsl:value-of select="@leftbefore3" />
										</td>
										<td class="main" align="right">
											<xsl:value-of select="@leftafter3" />
										</td>

									</xsl:for-each>

								</tr>

							</xsl:for-each>
						</table>

						<table style="font-size: 12px; width: 80%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник <xsl:value-of select="@car" />
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
