<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Графическая диаграмма A4</title>
				<style>

					.addparam {
					page-break-before: always;
					margin-top: auto;
					margin-left: auto;
					margin-right: auto;
					width: 820;

					}

					.pickets {
					transform: rotate(90deg);
					position: relative;
					top: -433;
					height: 11px;
					font-size: 10;
					width: 865;
					left: 286px;
					}
					.picket {
					float: left;
					margin-right: 89;
					}
					.bottom-labels {
					transform: rotate(90deg);
					top: -268;
					font-size: 9px;
					left: 422px;
					width: 20px;
					height: 550px;
					position: relative;
					margin-bottom: -550px;
					text-align: right;
					}
					.top-title {
					margin-top: 15px;
					margin-right: 15px;
					width: 600px;
					height: 24px;
					border-width: thin;
					padding-right: 3px;
					text-align: right;
					font-size: 11;
					font-family: Arial;
					margin-left: auto;
					margin-bottom: 20px;
					border-right: solid;
					border-bottom: solid;
					border-left: solid;
					border-width: thin;
					border-color: dimgray;
					}
					.main-chart {

					margin-left: auto;
					position: relative;

					}
					.chart {
					margin-right: 0px;
					margin-left: auto;
					margin-bottom: 0.5px solid;
					margin-top: 25;
					}
					.right-title {
					position: relative;
					left: 250px;
					top: 505px;
					transform: rotate(90deg);
					font-size: 9.5;
					font-family: Arial;
					width:1000px;
					}
					.inform-title {
					display: block;
					font-family: monospace;
					font-size: 9.5;
					white-space: pre;
					margin: 1em 0px;
					color: darkgray;
					border-color: darkgray;


					}
					.button {
					background-color: #4CAF50; /* Green */
					border: none;
					color: white;
					padding: 8px 16px;
					text-align: center;
					text-decoration: none;
					display: inline-block;
					font-size: 12px;
					margin: 2px 3px;
					-webkit-transition-duration: 0.4s; /* Safari */
					transition-duration: 0.4s;
					cursor: pointer;
					background-color: white;
					color: darkgray;
					border: 2px solid #4CAF50;
					position: fixed;
					right: 10px;
					top:10px;
					}
					.button:hover {
					background-color: #4CAF50;
					color: white;
					}
					.nkm2 {
					font-size: 14;
					font-family: Arial;
					text-align: right;
					margin-right: 5px;
					}
					.nkm1 {
					font-size: 14;
					font-family: Arial;
					float: left;
					margin-left: -80;
					padding-right: 55;
					}


				</style>
				<style type="text/css" media="print">
					.dontprint
					{
					display: none;
					}

				</style>

			</head>
			<body>
				<!-- <button class="dontprint button" onclick="hidetitle();">Режим ленты</button> -->

				<xsl:for-each select="report/addparam">
					<div  class = "addparam">

						<div class = "main-chart">
							<svg class= "chart">

								<xsl:attribute name="viewBox">
									<xsl:value-of select="@viewbox" />
								</xsl:attribute>

								<text x="765" style="font-size: 12;" text-anchor="end">
									<xsl:attribute name="y">
										<xsl:value-of select="@topx1" />
									</xsl:attribute>
									<xsl:value-of select="@top-title"/>
								</text>
								<text x="765" style="font-size: 12;" text-anchor="end">
									<xsl:attribute name="y">
										<xsl:value-of select="@topx2" />
									</xsl:attribute>
									<xsl:value-of select="@speedlimit"/>
								</text>
								<rect fill="none" stroke="black" x="217" width="550" height="35">
									<xsl:attribute name="y">
										<xsl:value-of select="@topr" />
									</xsl:attribute>
								</rect>


								<text transform="rotate(90)" font-size="11" y="-770">
									<xsl:attribute name="x">
										<xsl:value-of select="@topx" />
									</xsl:attribute>
									<xsl:value-of select="@right-title"/>
								</text>

								<text fill="black" x="0" font-size="10">
									<xsl:attribute name="y">
										<xsl:value-of select="@topf" />
									</xsl:attribute>
									<xsl:value-of select="@fragment" />
								</text>
								<xsl:for-each select="xgrid/x">
									<line stroke-width="0.3" fill="none">
										<xsl:attribute name="stroke-dasharray">
											<xsl:value-of select="@dasharray"/>
										</xsl:attribute>
										<xsl:attribute name="stroke">
											<xsl:value-of select="@stroke"/>
										</xsl:attribute>
										<xsl:attribute name="x1">
											<xsl:value-of select="." />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="." />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../../@maxY" />
										</xsl:attribute>

									</line>
									<text fill="dimgray" transform="rotate(90)" text-anchor="end" font-size="7">
										<xsl:attribute name="y">
											-<xsl:value-of select="@y" />
										</xsl:attribute>
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:value-of select="@label" />
									</text>
								</xsl:for-each>

								<xsl:for-each select="polyline">
									<polyline >
										<xsl:attribute name="points">
											<xsl:value-of select="@points" />
										</xsl:attribute>
										<xsl:attribute name="style">
											<xsl:value-of select="@style" />
										</xsl:attribute>
									</polyline>
								</xsl:for-each>

								<xsl:for-each select="lines/line">
									<polyline style="fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3">
										<xsl:attribute name="points">
											<xsl:value-of select="." />
										</xsl:attribute>
									</polyline>
								</xsl:for-each>
								<xsl:for-each select="artcons/line">
									<polyline style="fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3">
										<xsl:attribute name="points">
											<xsl:value-of select="." />
										</xsl:attribute>
									</polyline>
								</xsl:for-each>

								<xsl:for-each select="impulsleft/imp">
									<line stroke-width="0.5" stroke="darkgray" fill="none" marker-end="url(#b-circle)">
										<xsl:attribute name="x1">
											<xsl:value-of select="../@x1" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x2" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="@y" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@y"/>
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="darkgray" fill="none" marker-end="url(#b-circle)">
										<xsl:attribute name="x1">
											<xsl:value-of select="../@x3" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x4" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="@y" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@y"/>
										</xsl:attribute>
									</line>
								</xsl:for-each>
								<xsl:for-each select="impulsright/imp">
									<line stroke-width="0.5" stroke="darkgray" fill="none" marker-end="url(#b-circle)">
										<xsl:attribute name="x1">
											<xsl:value-of select="../@x1" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x2" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="@y" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@y"/>
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="darkgray" fill="none" marker-end="url(#b-circle)">
										<xsl:attribute name="x1">
											<xsl:value-of select="../@x3" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x4" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="@y" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@y"/>
										</xsl:attribute>
									</line>
								</xsl:for-each>

								<xsl:for-each select = "digressions/m">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/otst">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/step">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/otkl">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/len">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/count">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/ogrsk">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/mark">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/R">
									<text fill="dimgray" font-size="11px" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>
								<xsl:for-each select = "digressions/rect">
									<rect width="160" height="10" style="stroke-width:0.5;fill:none;stroke: darkgray;">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@top"/>
										</xsl:attribute>
										<xsl:attribute name = "x" >
											<xsl:value-of select = "@x"/>
										</xsl:attribute>
									</rect>
								</xsl:for-each>
								<xsl:for-each select = "digressions/line">
									<line stroke-dasharray="0.25,0.25"  stroke="grey" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="@y1" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@y2"/>
										</xsl:attribute>
										<xsl:attribute name="stroke-width">
											<xsl:value-of select="@w" />
										</xsl:attribute>
									</line>
								</xsl:for-each>
								<xsl:for-each select = "speedline">
									<text fill="dimgray" font-size="11px" x="0" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@y1"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note1"/>
									</text>
									<text fill="dimgray" font-size="11px" x="0" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@y2"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note2"/>
									</text>
									<polyline stroke-dasharray="0.25,1"  stroke="grey" stroke-width="1" fill="none">
										<xsl:attribute name="points">
											<xsl:value-of select="@points" />
										</xsl:attribute>
									</polyline>
									<rect width="190" height="21" style="stroke-width:0.5;fill:none;stroke: darkgray;" x="-2">
										<xsl:attribute name="y">
											<xsl:value-of select="@y3"/>
										</xsl:attribute>
									</rect>
								</xsl:for-each>

								<xsl:for-each select = "AnpNote">
									<text fill="dimgray" font-size="9px" x="110" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@y1"/>
										</xsl:attribute>
										<xsl:attribute name = "font-weight" >
											<xsl:value-of select = "@fw"/>
										</xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
								</xsl:for-each>

								<text x="0" font-size="10">
									<xsl:attribute name="y">
										<xsl:value-of select="@pre"/>
									</xsl:attribute><tspan x="5">м</tspan> <tspan x="18">|Отст   </tspan><tspan x="55"> |Ст</tspan>|Откл|Дл.| Балл|  Огр.ск <tspan x="185">|</tspan>
									<tspan x="235">Уровень</tspan><tspan x="320">|</tspan><tspan x="340">Рихтовка л.</tspan><tspan x="412">|</tspan><tspan x="435">Рихтовка пр.</tspan><tspan x="508">|</tspan><tspan x="545">Шаблон</tspan><tspan x="618">|</tspan><tspan x="630">Пр. л. </tspan><tspan x="675">|</tspan><tspan x="688">Пр. пр. </tspan><tspan x="730">| Км</tspan>
								</text>
								<rect x="0" height="12" width="770" style="fill: none;stroke: black;">
									<xsl:attribute name="y">
										<xsl:value-of select="@prer"/>
									</xsl:attribute>
								</rect>
							</svg>
						</div>

					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>