<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>

			<head>
				<title>ДФ-С1 - Ведомость уширении колеи в кривых за счет рельсов под нагрузкой</title>
				<style>
					.pages {
					page-break-before: always;
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

					table {
					border-collapse: collapse;
					}

					thead {
					background: #f5e8d0;
					/* Цвет фона заголовка */
					}

					td {
					padding-left: 5px;
					}

					.tr:nth-child(odd) {
					background-color: #EAF2D3;
					}

					.tr:hover {
					background-color: #E0E0FF;
					}

					.modal {
					display: none;
					position: fixed;
					z-index: 1;
					padding-top: 100px;
					left: 0;
					top: 0;
					width: 100%;
					height: 100%;
					overflow: auto;
					background-color: rgb(0, 0, 0);
					background-color: rgba(0, 0, 0, 0.4);
					}


					.modal-content {
					background-color: #fefefe;
					margin: auto;
					padding: 20px;
					border: 1px solid #888;
					width: 95%;
					}


					.close {
					width: 95%;
					margin: auto;
					padding-right: 50px;
					padding-top: 4px;
					text-align: right;
					color: #aaaaaa;
					float: right;
					font-size: 12px;
					font-weight: bold;

					}

					.close:hover,
					.close:focus {
					color: #000;
					text-decoration: none;
					cursor: pointer;
					}

					#mainImage {
					width: 100%;
					}

					.container {
					width: 100%;
					text-align: center;
					}
					#pageFooter:before {
					counter-increment: page;
					content:"Страница"  counter(page) "из" counter(page);
					left: 100%;
					top: 100%;
					white-space: nowrap;
					z-index: 20;
					-moz-border-radius: 5px;
					-moz-box-shadow: 0px 0px 4px #222;
					background-image: -moz-linear-gradient(top, #eeeeee, #cccccc);
					}
				</style>



				<style type="text/css" media="print">
					.dontprint {
					display: none;
					}

					@page {
					counter-increment: page;
					counter-reset: page 1;

					@top-right {
					content: "Page "counter(page) " of "counter(pages);
					}
					}
				</style>
				<script src="axios.min.js">//</script>
				<script src="/js/konva.min.js">//</script>
				<script src="/js/touch-emulator.js">//</script>
				<script src="/js/hammer-konva.js">//</script>
				<script src="getimage.js">//</script>


			</head>

			<body>
				<div id="myModal" class="modal">
					<span class="close">Закрыть</span>
					<div class="modal-content">
						<div id="container" class="container">
							<img id="mainImage" />
						</div>
					</div>
				</div>

				<xsl:for-each select="report/pages">
					<div align="right" class="pages">

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<H4 align = "center">
							Ведомость уширений колеи в кривых за счёт отжатий рельсов под нагрузкой
							(ДФ-С1)
						</H4>

					</div>

					<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
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
							</td>
							<td align="left">
								<xsl:value-of select="@periodDate" />
							</td>
						</tr>
						<!-- <tr>
                             <th  colspan="13"  align="center" valign="middle">Направление:<xsl:value-of select="@direction" /></th>
                             <th  colspan="3"  align="center" valign="middle">Путь: <xsl:value-of select="@track" /></th>
                            </tr>
                            <tr>
                                <th  colspan="13"  align="center" valign="middle">Участок: <xsl:value-of select="@road" /></th>
                                <th   colspan="2"  align="center"  valign="middle">ПЧ: <xsl:value-of select="@distance" /></th>
                                <th  colspan="3"  align="center"  valign="middle">Км: <xsl:value-of select="@km" /></th>
                            </tr> -->
					</table>



					<div align="center">




						<table class="main">
							<thead>
								<tr>
									<td class="main" align="center" valign="middle" rowspan="2">№</td>
									<td class="main" align="center" colspan="2">Начало кривой</td>
									<td class="main" align="center" colspan="2">Конец кривой</td>
									<td class="main" align="center">
										Vпз  <br />км/ч
									</td>
									<td class="main" align="center" colspan="2">Радиус,м</td>
									<td class="main" align="center" colspan="2">Ширина колеи</td>
									<td class="main" align="center" colspan="2">Износ</td>
									<td class="main" align="center" colspan="2">Макс. уширение</td>
									<td class="main" align="center" colspan="2">Тип</td>
									<td class="main" align="center" valign="middle" rowspan="2">
										Порог,<br /> мм
									</td>
								</tr>
								<tr class="tr">
									<td class="main" align="center">км</td>
									<td class="main" align="center">м</td>
									<td class="main" align="center">км</td>
									<td class="main" align="center">м</td>
									<td class="main" align="center">км/ч</td>
									<td class="main" align="center">сред.</td>
									<td class="main" align="center">мин, м</td>
									<td class="main" align="center">сред.</td>
									<td class="main" align="center">макс, мм</td>
									<td class="main" align="center">сред.</td>
									<td class="main" align="center">макс, мм</td>
									<td class="main" align="center">сред.</td>
									<td class="main" align="center">макс, мм</td>
									<td class="main" align="center">шпала</td>
									<td class="main" align="center">скрепление</td>
								</tr>
								<tr>
									<th class="main" align="right" colspan="17">
										<xsl:value-of select="@trackinfo" />
									</th>
								</tr>
							</thead>
							<xsl:for-each select="elements">

								<tr class="tr">
									<td class="main" align="center">
										<xsl:value-of select="@order" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@startkm" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@startm" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@finalkm" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@finalm" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@Vpz" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@radius" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@radiusmin" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@width" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@widthmax" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@wear" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@wearmax" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@broadening" />
									</td>
									<td class="main" align="center">
										<xsl:choose>
											<xsl:when test="@broadeningmax >= '4'">
												<xsl:value-of select="@broadeningmax" />
											</xsl:when>
											<xsl:otherwise>
												<b>
													<xsl:value-of select="@broadeningmax" />
												</b>
											</xsl:otherwise>
										</xsl:choose>


									</td>
									<td class="main" align="center">
										<xsl:value-of select="@brace" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@fastening" />
									</td>
									<td class="main" align="center">
										<xsl:value-of select="@limit" />
									</td>
								</tr>
							</xsl:for-each>
						</table>
						<table style="width:100%;height: 5%; font-size:10px" align="center" border="0" cellspacing="0" >
							<tr>
								<td>
									Начальник путеизмерителя
									<xsl:value-of select="@car" />
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