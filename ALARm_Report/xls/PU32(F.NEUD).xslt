<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<xsl:for-each select="report">
			<html>

				<head>
					<title>Ф.Неуд Километры с неудовлетворительной оценкой</title>

					<style>
						table.main,
						td.main {
						font-size: 12px;
						font-family: 'Times New Roman';
						border: 1.5px solid black;
						margin: auto;
						margin-bottom: 8px;
						border-collapse: collapse;
						padding: 5px;

						}

						tr {
						padding: 5px;
						}

						td {
						padding: 5px;
						text-align: center;
						vertical-align: middle;

						}

						td.vertical {}

						.main {
						text-align: left;
						vertical-align: middle;
						}

						.time-col {
						position: relative;
						writing-mode: vertical-rl;
						transform: rotate(180deg);
						}

						.col {
						position: relative;
						overflow: visible;
						}

						span {
						white-space: pre-wrap;
						display: inline-table;
						}

						.rotate {
						text-align: center;
						white-space: nowrap;
						vertical-align: middle;
						width: 1.5em;
						height: 8em;
						padding: 5px;
						}

						.rotate div {
						-moz-transform: rotate(-90.0deg);
						/* FF3.5+ */
						-o-transform: rotate(-90.0deg);
						/* Opera 10.5 */
						-webkit-transform: rotate(-90.0deg);
						/* Saf3.1+, Chrome */
						filter: progid:DXImageTransform.Microsoft.BasicImage(rotation=0.083);
						/* IE6,IE7 */
						-ms-filter: "progid:DXImageTransform.Microsoft.BasicImage(rotation=0.083)";
						/* IE8 */
						margin-left: -10em;
						margin-right: -10em;
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
				<div  id = "pageFooter" align="right" >
					<body>

						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>
						<H4 align = "center">Километры с неудовлетворительной оценкой</H4>
						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<td align="left">
									ПЧ: <xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога: <xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@car" />
								</td>
								<td align="left">
									Проверка:<xsl:value-of select="@triptype" />
								</td>
								<td align="left">
									<xsl:value-of select="@month" /> &#160; 2020
								</td>

							</tr>
						</table>

						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
							<tr>
								<td class="main" text-align="center" colspan="13">
									<!-- <pre style="font:inherit;heigth:20px;text-align:left" > -->
									Направление:
									<b>
										<xsl:value-of select="@name" />
										&#160;&#160;&#160;&#160;&#160;&#160;&#160; Код:
										<xsl:value-of select="@code" />
										&#160;&#160;&#160;&#160;&#160;&#160;&#160; Путь:
										<xsl:value-of select="@track" />
									</b>
									<!-- </pre> -->
								</td>
							</tr>

							<tr>
								<td rowspan="3">км</td>
								<td rowspan="3">
									V
									<sub>пз</sub>
								</td>
								<td colspan="3">Основные параметры</td>
								<td colspan="2">Дополнительные параметры</td>
								<td></td>
							</tr>
							<tr>

								<td colspan="2">Балловая оценка</td>
								<td rowspan="2">Примечания</td>
								<td rowspan="2">Балловая оценка</td>
								<td rowspan="2">Примечания (ограничение скорости, неисправность)</td>
								<td rowspan="2">
									Сумма баллов
									Осн. +доп
								</td>
							</tr>
							<tr>
								<td>
									ГРК;
									<br />
									сочетания
									<br />
									и др
								</td>
								<td>Кривые</td>
							</tr>
							<xsl:for-each select="bykilometer/section">
								<!-- <tr>
								<td></td>
								<td colspan="4">Код направления</td>
								<td>
									<xsl:value-of select="@code"/>
								</td>
								<td>
									Путь <xsl:value-of select="@track"/>
								</td>
								<td></td>
							</tr> -->
								<xsl:for-each select="pchu/pd/pdb/km">
									<xsl:if test="@c10 ='Н'">
										<tr>
											<td>
												<xsl:value-of select="@n" />
											</td>
											<td>
												<xsl:value-of select="@speed" />
											</td>
											<td>
												<xsl:value-of select="@mpoint" />
											</td>
											<td>
												<xsl:value-of select="@cpoint" />
											</td>
											<td>
												<xsl:value-of select="@c11" />
												<xsl:value-of select="@c13" />
											</td>
											<td>
												<xsl:value-of select="@apoint" />
											</td>
											<td>
												<xsl:value-of select="@c12" />
											</td>
											<td>
												<xsl:value-of select="@allsum" />
											</td>
										</tr>
									</xsl:if>
								</xsl:for-each>
							</xsl:for-each>
						</table>
						<table style="width:100%" align="center" border="0" cellspacing="0" cellpadding="5">
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

					</body>
				</div>
			</html>
		</xsl:for-each>

	</xsl:template>
</xsl:stylesheet>