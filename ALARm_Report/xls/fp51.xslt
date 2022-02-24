<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>

			<head>
				<title>ФП-5.1 График изменения СССП</title>
				<style>
					<!-- @media print {
                    .addparam {
                        width: 760px;
                        margin: auto;
                    }
                } -->

					.addparam {
					width: 760px;
					margin: auto;
					}

					.pickets {
					transform: rotate(90deg);
					position: relative;
					top: -476;
					height: 11px;
					font-size: 10;
					width: 865;
					right: 262px;
					}

					.picket {
					float: left;
					margin-right: 65;
					}

					.bottom-labels {
					transform: rotate(90deg);
					top: -273;
					font-size: 9px;
					left: 422px;
					width: 20px;
					height: 550px;
					position: relative;
					margin-bottom: -550px;
					text-align: right;
					}

					.top-title {
					margin-right: 15px;
					border-color: black;
					border: solid;
					width: 574.1px;
					border-width: thin;
					padding-right: 10px;
					font-size: 11;
					margin-left: auto;
					}

					.top-c {
					/* margin-right: 85px; */
					border-color: black;
					/* width: 575px; */
					/* height: 12px; */
					border-width: thin;
					text-align: center;
					padding-right: 10px;
					margin-left: auto;
					margin-bottom: 20px;
					}

					.top-r {
					margin-top: 50px;
					border-color: black;
					width: 575px;
					height: 12px;
					border-width: thin;
					text-align: RIGHT;
					margin-left: auto;
					margin-bottom: 10px;
					}

					.label-name {
					margin-right: 15px;
					border-color: black;
					width: 575px;
					height: 12px;
					border-width: thin;
					text-align: center;
					padding-right: 10px;
					margin-left: auto;
					margin-bottom: 20px;
					font-size: 98%;
					}

					.main-chart {
					height: 300px;
					width: 720;
					/* margin-right: auto; */
					margin-left: auto;
					preserveAspectRatio: none;
					/* padding: 0px; */
					/* transform: scale(1.5) rotate(90deg); */
					/* position: absolute; */
					top: 400px;
					}

					.chart {
					margin-right: auto;
					margin-left: auto;
					width: 1000;
					height: 250;
					margin-bottom: 0.5px solid;
					}

					.right-title {
					position: relative;
					left: 373;
					top: 390px;
					transform: rotate(90deg);
					font-size: 12;
					font-family: Arial;
					}

					.button {
					background-color: #4CAF50;
					/* Green */
					border: none;
					color: white;
					padding: 8px 16px;
					text-align: center;
					text-decoration: none;
					display: inline-block;
					font-size: 12px;
					margin: 2px 3px;
					-webkit-transition-duration: 0.4s;
					/* Safari */
					transition-duration: 0.4s;
					cursor: pointer;
					background-color: white;
					color: black;
					border: 2px solid #4CAF50;
					position: fixed;
					right: 10px;
					top: 10px;
					}

					.button:hover {
					background-color: #4CAF50;
					color: white;
					}

					.nkm {
					font-size: 14;
					font-family: Arial;
					text-align: right;
					margin-right: 5px;
					}

					.text2 {
					position: absolute;
					right: 16px;
					top: 40px;
					background: #fff;
					padding: 0 10px;
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
					hr {
					border: none; /* Убираем границу */
					background-color: aqua; /* Цвет линии */
					color: aqua; /* Цвет линии для IE6-7 */
					height: 2px; /* Толщина линии */
					}
				</style>

				<style type="text/css" media="print">
					.dontprint {
					display: none;
					}

					#container {
					width: 600px;
					margin: 0 auto;
					}

					#center {
					width: 200px;
					height: 100px;
					float: left;
					background: rgb(255, 0, 0);
					}

				</style>
				<script type="text/javascript">

					function hidetitle() {
					var divs = document.querySelectorAll('.top-title');
					divs.forEach(function (div) {
					div.style.display = div.style.display == "none" ? "block" : "none";
					});
					}

				</script>
			</head>

			<body>
				<xsl:for-each select="report/addparam">
					<table style="font-size: 2 px;" align="left">
						<tr>
							<th>
								<xsl:value-of select="@ALARmReport" />
							</th>
						</tr>
					</table>

					<div class="addparam;" style="text-align: right;">

						<div id="pageFooter" class="top-c" style="text-align: right;page-break-before:always; margin:50px 0px 0px">

							<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
								<xsl:value-of select="@version" />
							</p>
							<H4 align = "center">График изменения СССП (ФП-5.1)</H4>

						</div>
						<table style="width:100%" align="center">
							<tr>
								<td align="left">
									ПЧ:                                    <xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:                                    <xsl:value-of select="@road" />
								</td>
								<td align="left">
									Направление:                                    <xsl:value-of select="@direction" />
								</td>


							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@ps" />
								</td>
								<td align="left">
									Проверка:                                    <xsl:value-of select="@check" />&#160;&#160;
									<xsl:value-of select="@periodDate"/>
								</td>
								<td align="left">
									Путь:                                    <xsl:value-of select="@track" />
								</td>
							</tr>
						</table>
						<!-- <div class="top-c">
                        <b>Оценка состояния пути по статическим характеристикам вертикальных неровностей</b>
                    </div> -->


						<div style="display: table; margin: 0 auto; text-align: centre;" class="main-chart">
							<svg class="chart" style="width: 1200px;height: 400px;">
								<xsl:attribute name="viewBox">
									<xsl:value-of select="@viewbox" />
								</xsl:attribute>

								<xsl:for-each select="graphics/CCCPspeedBYpiket">
									<rect width="10" stroke="black" stroke-width="0.5">
										<xsl:attribute name="height">
											<xsl:value-of select="@h" />
										</xsl:attribute>
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
										<xsl:attribute name="style">
											<xsl:value-of select="@style" />
										</xsl:attribute>
									</rect>
								</xsl:for-each>

								<xsl:for-each select="km/text">
									<text transform="rotate(-90)" style="font-size: 9px" font-weight="bold">
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
										<xsl:value-of select="." />
									</text>

									<rect width="1" stroke="black" stroke-width="0.5" height="10" x="{@y}" y="245" style="fill:black"></rect>

								</xsl:for-each>
								<xsl:for-each select="speedlimit/text">
									<text style="font-size: 9px" font-weight="bold">
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>

										<xsl:value-of select="." />
									</text>
								</xsl:for-each>


								<xsl:for-each select="MaxSpeedLimit">
									<line x1="4" x2="4" y2="-5" stroke="black" stroke-width="12" stroke-dasharray="1,9">
										<xsl:attribute name="y1">
											<xsl:value-of select="." />
										</xsl:attribute>
									</line>
								</xsl:for-each>
								<!-- <xsl:for-each select="MaxSpeedLimit">
                                <line transform="rotate(-90)" style="font-size: 9px" font-weight="bold" x1="4" x2="4" y2="-5" stroke="black" stroke-width="15" stroke-dasharray="1,9">
                                    <xsl:attribute name="y1">
                                        <xsl:value-of select="." />
                                    </xsl:attribute>
                                </line>
                            </xsl:for-each> -->

								<xsl:for-each select="cccpsred">
									<line x1="4" stroke-width="1" stroke="aqua" fill="aqua(Safe 16 Hex3)">
										<xsl:attribute name="y1">
											<xsl:value-of select="@v" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@v" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@dlina" />
										</xsl:attribute>
									</line>
									<text x="20" y="@v" class="small" style="border-top: 2px dashed black;color:transparent;" font-color="aqua(Safe 16 Hex3)" stroke="green">
										<xsl:attribute name="y">
											<xsl:value-of select="@v" />
										</xsl:attribute>
										<!-- CCCП сред  -->
										<!-- <xsl:value-of select="@cccp" /> -->
										<!-- <xsl:attribute name="style">
                                        <xsl:value-of select="@style" />
                                    </xsl:attribute> -->

									</text>

								</xsl:for-each>

								<xsl:for-each select="vpass">

									<polyline stroke-dasharray="4" x1="4" stroke-width="1" stroke="black" fill="none">
										<xsl:attribute name="points">
											<xsl:value-of select="@polylinePass" />
										</xsl:attribute>
									</polyline>
									<!--                                       
                                    <line stroke-dasharray="4" x1="4" stroke-width="1" stroke="black" fill="none">
                                        <xsl:attribute name="y1">
                                            <xsl:value-of select="@polylinePass" />
                                        </xsl:attribute>
                                        <xsl:attribute name="y2">
                                            <xsl:value-of select="@v" />
                                        </xsl:attribute>
                                        <xsl:attribute name="x2">
                                            <xsl:value-of select="@dlina" />
                                        </xsl:attribute>
                                    </line> -->
									<text x="20" y="" class="small" font-color="yellow" stroke="yellow">
										<xsl:attribute name="y">
											<xsl:value-of select="@v" />
										</xsl:attribute>
										<!-- Vпасс  -->
										<!-- &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160; -->
										<!-- <xsl:value-of select="@vpz" />  -->
									</text>

								</xsl:for-each>

								<xsl:for-each select="vgruz">
									<polyline stroke-dasharray="2" x1="5" stroke-width="1" stroke="black" fill="none">
										<xsl:attribute name="points">
											<xsl:value-of select="@polylineFreig" />
										</xsl:attribute>
									</polyline>
									<!-- <line stroke-dasharray="2" x1="5" stroke-width="1" stroke="black" fill="none">
                                        <xsl:attribute name="y1">
                                            <xsl:value-of select="@v" />
                                        </xsl:attribute>
                                        <xsl:attribute name="y2">
                                            <xsl:value-of select="@v" />
                                        </xsl:attribute>
                                        <xsl:attribute name="x2">
                                            <xsl:value-of select="@dlina" />
                                        </xsl:attribute>
                                    </line> -->
									<text x="20" class="small" font-color="red" stroke="red">
										<xsl:attribute name="y">
											<xsl:value-of select="@v" />
										</xsl:attribute>
										<!-- Vгруз &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160; &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160; <xsl:value-of select="@vgruz" /> -->
									</text>
								</xsl:for-each>


								<div style="display: table; margin: 0 auto; text-align: centre;" class="main-chart">
									<svg class="chart" style="width: 1200px;height: 100px;">

										<rect x="15" y="50" width="50" height="10" rx="5" ry="5" fill="lightgreen"></rect>
										<text x="80" y="60">СССП выше Vпасс.</text>

										<rect x="250" y="50" width="50" height="10" rx="5" ry="5" fill="red"></rect>
										<text x="315" y="60">СССП ниже Vпасс.</text>

										<rect x="480" y="55" width="50" height="2" rx="1" ry="1" fill="aqua"></rect>
										<text x="540" y="60">Ср. СССП</text>

										<!-- <rect x="670" y="50" width="50" height="1" rx="5" ry="5" fill="red"></rect> -->
										<line x1="650" y1="55" x2="730" y2="55" stroke="black"
				   stroke-dasharray="8"   stroke-width="5" fill="none"/>
										<text x="740" y="60">Vпасс </text>

										<line x1="830" y1="55" x2="890" y2="55" stroke="black"
				stroke-dasharray="4"   stroke-width="5" fill="none"/>
										<!-- <rect x="830" y="50" width="50" height="10" rx="5" ry="5" fill="red"></rect> -->
										<text x="900" y="60">Vгруз</text>

									</svg>
								</div>

							</svg>
						</div>


					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>