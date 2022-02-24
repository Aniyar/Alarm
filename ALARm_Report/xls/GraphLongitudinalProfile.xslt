<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>

			<head>
				<title>ДФ-4.1 - График продольного профиля главного пути с высотными отметками</title>
				<style>
					.pages {

					page-break-after: always;
					}
					.main-container {
					width: 100%;
					overflow-x: auto;
					}
					table.main, td.main, th.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					}

					.divpage {
					float: center;
					page-break-after: always;
					margin: auto;
					height: 20cm;
					width: 26.5cm;
					}

					.main_svg {
					width: 100%;
					height: 18cm;
					font-size: 7px;
					font-family: 'Times New Roman';
					}
				</style>
				<script>
					function ready(fn) {
					if (document.readyState != 'loading') {
					fn();
					} else if (document.addEventListener) {
					document.addEventListener('DOMContentLoaded', fn);
					} else {
					document.attachEvent('onreadystatechange', function () {
					if (document.readyState != 'loading')
					fn();
					});
					}
					}

					// test
					window.ready(function () {
					// var containerWidth = document.getElementsByClassName("divpage").length * 25;
					// alert(containerWidth + 'cm'); document.getElementById('main-container').style.width = containerWidth + 'cm';

					})
				</script>
			</head>

			<body>
				<svg width="25cm" height="20" style="overflow:visible" viewbox="40 0 945 756">

					<!-- <div class="main-container" id="main-container"> -->
					<div >
						<xsl:for-each select="report/pages">
							<div class="divpage">
								<div>

									<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
										<xsl:value-of select="@version" />
									</p>
									<H4 align = "center" style="margin: 1;">График продольного профиля главного пути с высотными отметками (ДФ-4.1)</H4>
									<table style="width: 100%; height: 1cm; font-size: 12px; font-family: 'Times New Roman'; margin: auto;">
										<!-- <tr>
                                        <th align="center" colspan="4"></th>
                                    </tr> -->

										<tr>
											<td align="left">
												ПЧ:
												<xsl:value-of select="@distance" />
											</td>
											<td align="left">
												Дорога:
												<xsl:value-of select="@road" />
											</td>
											<td align="left">

											</td>

											<td align="left">
												Направление:
												<xsl:value-of select="@direction" />
											</td>


										</tr>
										<tr>
											<td align="left">
												<xsl:value-of select="@car" />
											</td>


											<td align="left">
												<xsl:value-of select="@trip_info" />
											</td>
											<td align="left">
												<xsl:value-of select="@period" />
											</td>

											<td align="left">
												Путь:
												<xsl:value-of select="@track" />
											</td>
										</tr>
										<!-- <tr>
                                        <td align="left">ПЧ:
                                            <xsl:value-of select="@distance" />
                                        </td>
                                        <td align="left">Дорога:
                                            <xsl:value-of select="@road" />
                                        </td>

                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <xsl:value-of select="@car" />
                                        </td>
                                        <td align="left">Поверка:
                                            <xsl:value-of select="@trip_info" />
                                        </td>
                                        <td align="left">
                                            <xsl:value-of select="@period" />
                                        </td>
                                    </tr> -->
									</table>


								</div>
								<div>
									<div class="main_svg">
										<!--Обычные элементы-->

										<!--График-->
										<svg width="25cm" height="9.45cm">
											<!--Масштаб-->
											<rect x="0.05cm" y="7.8cm" height="0.5cm" width="0.1cm" stroke="black" fill="black" />
											<rect x="0.05cm" y="8.3cm" height="0.5cm" width="0.1cm" stroke="black" fill="none" />
											<rect x="0.35cm" y="8.85cm" height="0.1cm" width="0.5cm" stroke="black" fill="black" />
											<rect x="0.85cm" y="8.85cm" height="0.1cm" width="0.5cm" stroke="black" fill="none" />
											<text style="transform: rotate(90deg);" x="7.85cm" y="-0.2cm">1см:100см</text>
											<text x="0.3cm" y="8.8cm">1см:20000см</text>

											<!--Графики-->

											<!-- Оригинал -->
											<!-- <polyline stroke="black" fill="none" stroke-width="1.5px" points="{@graph}" />  -->

											<!-- С применением линеара и экспоненты -->
											<polyline stroke="black" fill="none" stroke-width="2px" points="{@new_original}" />

											<!-- Линеара жогарыдагы графка -->
											<polyline stroke="red" fill="none" stroke-width="1px" points="{@straight_graph}" />

											<!-- Линеара Reper -->
											<polyline stroke="red" fill="none" stroke-width="0.4px" points="{@linearReper}" />

											<!-- Линеара Reper By Orig -->
											<polyline stroke="red" fill="none" stroke-width="1px" points="{@linearReperByOrig}" />

											<!-- Конечный линеар -->
											<polyline stroke="green" fill="none" stroke-width="1.2px" points="{@ORIGINminusLBOplusLR}" />

											<polyline stroke="black" fill="none" stroke-width="1px" points="{@deviation_graph}" />

											<!--Граница-->
											<line stroke="black" fill="none" x1="0" x2="0" y1="0" y2="12cm" />



											<line stroke="black" fill="none" x1="0" x2="25cm" y1="0" y2="0" />
											<line stroke="black" fill="none" x1="25cm" x2="25cm" y1="0" y2="12cm" />
											<line stroke="black" fill="none" x1="0" x2="25cm" y1="12cm" y2="12cm" />
											<line stroke="black" fill="none" x1="0" x2="25cm" y1="11cm" y2="11cm" />
											<line stroke="black" fill="none" x1="0" x2="25cm" y1="10cm" y2="10cm" />
											<rect x="0" y="25cm" y1="10cm" y2="10cm" style="fill: #A8A8A8">
											</rect>

											<!-- Реперные точки -->
											<xsl:for-each select="RefPoint">
												<!-- <rect x="{@xr}" y="{@yr}" height="25" width="2" stroke="blue" fill="aqua"></rect> -->
												<!-- <image href="C:\SNTFI\mapsmark.png" x="{@xr}" y="{@yr}" width="10" height="10"></image> -->

												<!-- <circle cx="{@xr}" cy="{@yr}" r="0.5" stroke="black" stroke-width="0.5" fill="red"></circle> -->
											</xsl:for-each>


											<!-- KM numbers -->
											<xsl:for-each select="Kms">
												<text x="{@x}" y="9.4cm" font-weight="bold">
													<xsl:value-of select="@txt" />
												</text>

												<line stroke="black" stroke-opacity="0.8" stroke-dasharray="5" fill="none" x1="{@x}" x2="{@x}" y1="0" y2="9cm" />
											</xsl:for-each>

											<!-- Pikets numbers -->
											<xsl:for-each select="Pikets">
												<text x="{@x}" y="9.2cm">
													<xsl:value-of select="@txt" />
												</text>

												<line stroke="black" stroke-opacity="0.2" stroke-dasharray="1" fill="none" x1="{@x}" x2="{@x}" y1="0" y2="9cm" />
											</xsl:for-each>

											<!-- Девияция-->
											<text font-family="Airal" font-size="10" x="0" y="9.95cm" inline-size="200">10.0</text>
											<text font-family="Airal" font-size="10" x="0" y="10.5cm" inline-size="200">0</text>
											<text font-family="Airal" font-size="10" x="-0" y="11.23cm" inline-size="200">-10.0</text>

											<line stroke="black" fill="none" x1="0" x2="25cm" y1="9cm" y2="9cm" />

										</svg>


										<!--Таблица-->





										<svg width="25cm" height="4cm">
											<!--Граница-->
											<line stroke="black" fill="none" x1="0" x2="0" y1="0" y2="4cm" />
											<line stroke="black" fill="none" x1="0" x2="25cm" y1="0" y2="0" />
											<line stroke="black" fill="none" x1="25cm" x2="25cm" y1="0" y2="4cm" />
											<line stroke="black" fill="none" x1="0" x2="25cm" y1="4cm" y2="4cm" />
											<line stroke="black" fill="none" x1="0" x2="25cm" y1="1cm" y2="1cm" />
											<line stroke="black" fill="none" x1="0" x2="25cm" y1="2cm" y2="2cm" />
											<line stroke="black" fill="none" x1="0" x2="25cm" y1="3cm" y2="3cm" />


											<xsl:for-each select="semicircle_line">
												<circle cx="{@xr}" cy="{@yr}" r="0.5" />
											</xsl:for-each>


											<!--Спрямление-->
											<xsl:for-each select="straights">
												<line stroke="black" fill="none" x1="{@x1}cm" x2="{@x2}cm" y1="{@y1}cm" y2="{@y2}cm" />
											</xsl:for-each>
											<xsl:for-each select="straights_paths">
												<path stroke="black" fill="none" d="{@d}" />
											</xsl:for-each>
											<xsl:for-each select="straights_value">
												<text x="{@x}cm" y="{@y}cm" transform="rotate({@rotate})" style="font-size: 6px;">
													<xsl:value-of select="@text" />
												</text>
											</xsl:for-each>
											<!--По пикетные-->
											<xsl:for-each select="pickets">
												<line stroke="black" fill="none" x1="{@x1}cm" x2="{@x2}cm" y1="{@y1}cm" y2="{@y2}cm" />
											</xsl:for-each>
											<xsl:for-each select="pickets_value">
												<!-- По пикетно занчение -->
												<text x="{@x}cm" y="{@y}cm" transform="rotate(270)" style="font-size: 6px;">
													<xsl:value-of select="@text" />
												</text>
												<!-- Линия пикета -->
												<line stroke="black" fill="none" x1="{@y}cm" x2="{@y}cm" y1="3cm" y2="4cm" />
												<!-- Линия между пикетами -->
												<line stroke="black" fill="none" x1="{@y}cm" x2="{@y_prev}cm" y1="{@x1}cm" y2="{@x2}cm" />

												<text x="{@yR}cm" y="3.2cm" style="font-size: 4.5px;">
													<xsl:value-of select="@razn" />
												</text>

												<!-- <text x="{@yP}cm" y="3.95cm" style="font-size: 4.5px;">100</text> -->
											</xsl:for-each>
										</svg>

										<svg height="1.5cm" width="1cm" style="overflow:visible" viewbox="937 0 38 57">
											<text x="-1.0cm" y="1.85cm">Станции</text>
											<text x="-1.0cm" y="2.35cm">Мосты</text>
											<text x="-1.0cm" y="2.85cm">Стрелки</text>
										</svg>

										<!-- Станции Стрелки Мосты -->
										<svg width="25cm" height="1.5cm">

											<line stroke="black" fill="none" x1="0cm" x2="25cm" y1="0.4cm" y2="0.4cm"></line>

											<line stroke="black" fill="none" x1="0cm" x2="25cm" y1="0.9cm" y2="0.9cm"></line>

											<line stroke="black" fill="none" x1="0cm" x2="25cm" y1="1.4cm" y2="1.4cm"></line>

											<line stroke="black" fill="none" x1="0cm" x2="0cm" y1="0cm" y2="1.4cm"></line>
											<!-- <line stroke="black" fill="none" x1="0.8cm" x2="0.8cm" y1="0cm" y2="1.4cm"></line> -->
											<line stroke="black" fill="none" x1="25cm" x2="25cm" y1="0cm" y2="1.4cm"></line>

											<xsl:for-each select="stations">

												<line stroke="black" fill="none" x1="{@start}cm" x2="{@start}cm" y1="0cm" y2="0.4cm"></line>

												<text x="{@startStationCoord}cm" y="{@y}cm">
													<xsl:value-of select="@startStation" />
												</text>

												<text x="{@x}cm" y="{@y}cm">
													<xsl:value-of select="@text" />
												</text>

												<text x="{@finalStationCoord}cm" y="{@y}cm">
													<xsl:value-of select="@finalStation" />
												</text>

												<line stroke="black" fill="none" x1="{@final}cm" x2="{@final}cm" y1="0cm" y2="0.4cm"></line>

											</xsl:for-each>

											<xsl:for-each select="switches">
												<rect x="{@x1}cm" y="0.9cm" height="0.5cm" width="0.1cm" fill="red" stroke="black" style="stroke-width: 0.3;"></rect>
											</xsl:for-each>

											<xsl:for-each select="bridges">
												<!-- <rect x="{@x1}cm" y="0.4cm" height="0.04cm" width="0.1cm" fill="white" stroke="blue" style="stroke-width: 0.5;"></rect>
                                            <rect x="{@x1}cm" y="0.86cm" height="0.04cm" width="0.1cm" fill="white" stroke="blue" style="stroke-width: 0.5;"></rect> -->

												<text x="{@x2}cm" y="0.63cm">
													<xsl:value-of select="@koord" />
												</text>

												<text x="{@x3}cm" y="0.8cm">
													<xsl:value-of select="@len" />
												</text>

											</xsl:for-each>

										</svg>

										<!--Кривые-->
										<svg width="25cm" height="1cm">
											<xsl:for-each select="curves">
												<line stroke="black" fill="none" x1="{@x1}cm" x2="{@x2}cm" y1="{@y1}cm" y2="{@y2}cm" />
											</xsl:for-each>

											<xsl:for-each select="curvesBox">
												<line stroke="black" fill="none" x1="{@x1}cm" x2="{@x2}cm" y1="{@y1}cm" y2="{@y2}cm" />
											</xsl:for-each>

											<xsl:for-each select="curves_texts">
												<text x="{@x}cm" y="{@y}cm" transform="rotate({@rotate})">
													<xsl:value-of select="@text" />
												</text>
											</xsl:for-each>
										</svg>
									</div>

								</div>
							</div>
						</xsl:for-each>
					</div>
				</svg>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>