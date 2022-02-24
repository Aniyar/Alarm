<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>ДФ-ПР2 - Ведомость характеристик ПР и НПК по участкам пути</title>
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
					<div id="pageFooter" align="right" style="page-break-before:always;" >

						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>

						<!-- <b>
                            <p align="center" style="color:black; font-size:14px">Ведомость характеристик ПР и НПК по участку пути с одинаковым типом верхнего строения (ДФ-ПР2)</p>
                        </b> -->
						<H4 align = "center">
							Ведомость характеристик ПР и НПК по участку пути<br />
							с одинаковым типом верхнего строения
						</H4>
						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;">

							<tr>
								<td align="left">
									ПЧ:<xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@car" />
								</td>
								<td align="left">
									Проверка:<xsl:value-of select="@chek"/>  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="@period" />
								</td>

							</tr>
						</table>

						<table class="main">
							<thead>
								<tr>
									<td class="main" align="center" colspan="2">Участок</td>
									<td class="main" align="center" colspan="2">Тип ВСП</td>
									<td class="main" align="center" valign="middle" rowspan="3">
										Рельс<br />  (л/пр)
									</td>
									<td class="main" align="center" colspan="5">ПР</td>
									<td class="main" align="center" colspan="5">НПК</td>
									<td class="main" align="center"  valign="middle" rowspan="3">V уст</td>
								</tr>
								<tr>
									<td class="main" align="center" valign="top" rowspan="1">Начало</td>
									<td class="main" align="center" valign="top" rowspan="1">Конец </td>
									<td class="main" align="center" valign="top" rowspan="2">Рельсы</td>
									<td class="main" align="center" valign="top" rowspan="2">Скрепление</td>
									<td class="main" align="center" valign="middle" rowspan="2">Сред.</td>
									<td class="main" align="center" valign="middle" rowspan="2">Мин.</td>
									<td class="main" align="center" valign="middle" rowspan="2">Макс.</td>
									<td class="main" align="center" colspan="2">Протяженность (м)</td>
									<td class="main" align="center" valign="middle" rowspan="2">Сред.</td>
									<td class="main" align="center" valign="middle" rowspan="2">Мин.</td>
									<td class="main" align="center" valign="middle" rowspan="2">Макс.</td>
									<td class="main" align="center" colspan="2">Протяженность (м)</td>
								</tr>
								<tr>

									<td class="main" align="center">  км.м</td>
									<td class="main" align="center">  км.м</td>
									<td class="main" align="center">Менее 1/60</td>
									<td class="main" align="center">Более 1/12</td>
									<td class="main" align="center">Менее 1/60</td>
									<td class="main" align="center">Более 1/12</td>
								</tr>
								<tr>
									<th  align="right" colspan="16"  style="font-size: 13px;">
										<xsl:value-of select="@track_info" />
									</th>
								</tr>

							</thead>
							<tbody id="this">

								<xsl:for-each select="tracks">

									<xsl:for-each select="elements">
										<tr id="this" class="tr">
											<td class="main" align="right">
												<xsl:value-of select="@start" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@final" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@rails" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@fastening" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@railside" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@PRmedium" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@PRmin" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@PRmax" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@PRlen160" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@PRlen120" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@NPKmedium" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@NPKmin" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@NPKmax" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@NPKlen160" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@NPKlen120" />
											</td>
											<td class="main" align="right">
												<xsl:value-of select="@speed" />
											</td>
										</tr>
									</xsl:for-each>
								</xsl:for-each>
							</tbody>
						</table>
						<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@car" />
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