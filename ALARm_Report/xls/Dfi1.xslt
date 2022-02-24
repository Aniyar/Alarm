<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>(ДФ-И1) - Ведомость участков пути с износом,превышающим 3-ю и 4-ю степень дефектности </title>
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
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					}
				</style>
				<xsl:for-each select="report/trip">

					<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
						<xsl:value-of select="@version" />
					</p>


					<H4 align = "center">
						Ведомость участков пути с боковым износом <br /> требующих ограничение скорости
					</H4>


					<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">

						<tr>
							<td align="left">
								ПЧ:<xsl:value-of select="@pch" />
							</td>
							<td align="left">
								Дорога:<xsl:value-of select="@road" />
							</td>
						</tr>
						<tr>
							<td align="left">
								<xsl:value-of select="@ps" />
							</td>
							<td align="left">
								Проверка:<xsl:value-of select="@date_statement"/>  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="@period" />
							</td>

						</tr>
					</table>

					<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center" style="border-collapse: collapse;">
						<thead>

							<tr>
								<td  class="border-head" align="center" valign="middle" style="padding:2.5px;padding-left: 5px;" rowspan="2">
									<p>Кривая начало/конец(км,м)</p>
								</td>
								<td  class="border-head" align="center" valign="middle" style="padding:2.5px;padding-left: 5px;" colspan="2">
									<p>Величина износа(мм)</p>
								</td>
								<td  class="border-head" align="center" valign="middle" style="padding:2.5px;padding-left: 5px;" rowspan="2">
									<p>Радиус (м)</p>
								</td>
								<td  class="border-head" align="center" valign="middle" style="padding:2.5px;padding-left: 5px;" rowspan="2">
									<p>V уст,(км/м) </p>
								</td>
								<td  class="border-head" align="center" valign="middle" style="padding:2.5px;padding-left: 5px;" rowspan="2">
									<p>V доп(км/ч)</p>
								</td>
							</tr>

							<tr>
								<td  class="border-head" align="center" valign="middle" style="padding:2.5px;padding-left: 5px;">максимальная</td>
								<td  class="border-head" align="center" valign="middle" style="padding:2.5px;padding-left: 5px;">среднаяя</td>
							</tr>
						</thead>
						<tbody id="this">
							<xsl:for-each select="lev/Note">
								<tr>
									<td  class="main" align="center">
										<xsl:value-of select="@param" />
									</td>
									<td  class="main" align="center">
										<xsl:choose>
											<xsl:when test="@max >= '15'">
												<xsl:value-of select="@max" />
											</xsl:when>
											<xsl:otherwise>
												<b>
													<xsl:value-of select="@max" />
												</b>
											</xsl:otherwise>
										</xsl:choose>
									</td>
									<td  class="main" align="center">
										<xsl:value-of select="@sred" />
									</td>
									<td  class="main" align="center">
										<xsl:value-of select="@radius" />
									</td>
									<td  class="main" align="center">
										<xsl:value-of select="@Vust" />
									</td>
									<td  class="main" align="center">
										<xsl:value-of select="@Vogr" />
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
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
								&#160;&#160;&#160;&#160;&#160;
								<xsl:value-of select="@chief" />
							</td>
						</tr>
					</table>
				</xsl:for-each>
			</head>

		</html>
	</xsl:template>
</xsl:stylesheet>