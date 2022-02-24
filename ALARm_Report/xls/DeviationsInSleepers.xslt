<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>

			<head>
				<title>ВЕДОМОСТЬ ОГРАНИЧЕНИЙ СКОРОСТИ В СОДЕРЖАНИИ ШПАЛ </title>
				<style>
					td {
					padding-left: 5px;
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
				</style>


			</head>

			<body>

				<xsl:for-each select="report/pages">

					<div style="page-break-before:always;">
						<table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">ВЕДОМОСТЬ ОГРАНИЧЕНИЙ СКОРОСТИ В СОДЕРЖАНИИ ШПАЛ</p>
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
										<xsl:value-of select="@period" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;

									<b>
										<xsl:value-of select="@type" />
									</b>

								</td>
								<td  align="left">
								</td>


							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@car" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@data" />
									</b>

								</td>
							</tr>
						</table>

						<!-- <div style="page-break-before:always;"  align="center"> -->

						<table class="main">
							<thead>
								<tr>
									<th class="main" align="center">№ п/п</th>
									<th class="main" align="center">ПЧУ, ПД, ПДБ</th>
									<th class="main" align="center">Перегон, станция</th>
									<th class="main" align="center">км</th>
									<th class="main" align="center">пк</th>
									<th class="main" align="center">м</th>
									<th class="main" align="center">Vуст</th>
									<th class="main" align="center">Отступление</th>
									<th class="main" align="center">Величина</th>
									<th class="main" align="center">Тип рельса</th>
									<th class="main" align="center">Промежуточное скрепление</th>
									<th class="main" align="center">Класс пути</th>
									<th class="main" align="center">Характеристика пути</th>
									<th class="main" align="center">Шаблон, мм</th>
									<th class="main" align="center">Vдоп</th>
									<th class="main" align="center">Примечание</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="tracks">
									<tr>
										<th class="main" align="right" colspan="16">
											<xsl:value-of select="@trackinfo" />
										</th>
									</tr>
									<xsl:for-each select="elements">
										<tr>
											<td class="main" align="center">
												<xsl:value-of select="@n" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@pchu" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@station" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@km" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@piket" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@meter" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@speed" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@digression" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@size" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@railtype" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@fastening" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@trackclass" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@trackplan" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@template" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@addspeed" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@notice" />
											</td>
										</tr>
									</xsl:for-each>
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
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
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