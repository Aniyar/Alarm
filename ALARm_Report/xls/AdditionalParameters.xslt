<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Графическая диаграмма коротких неровностей рельсов</title>
				<style>

					.addparam {
					width: 760px;
					margin:auto;
					page-break-before:always;
					height:1000px;

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
					margin-top: 50px;
					margin-right: 15px;
					border-color: black;
					border: solid;
					width: 742px;
					height: 24px;
					border-width: thin;
					padding-right: 3px;
					text-align: right;
					font-size: 11;
					font-family: Arial;
					margin-left:auto;
					margin-bottom:20px;
					}
					.main-chart {
					height: 958px;

					margin-right: 15px;
					margin-left: auto;
					position: relative;
					}
					.chart {
					margin-right: 0px;
					margin-left: auto;
					width: 770px;
					height: 958px;
					margin-bottom: 0.5px solid;
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
					font-size: 9px;
					border: solid 0.5px;
					width: 745px;
					margin-top: 10px;

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
					color: black;
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
				<script type="text/javascript">

					function hidetitle()
					{
					var divs = document.querySelectorAll('.top-title, .bottom-labels, .right-title, .inform-title');
					divs.forEach(function(div) {
					div.style.display = div.style.display == "none" ? "block" : "none";
					});
					var divs = document.querySelectorAll('.addparam');
					divs.forEach(function(div) {
					div.style.height = div.style.height == "951px" ? "1000px" : "951px";
					});
					}

				</script>
			</head>
			<body>
				<button class="dontprint button" onclick="hidetitle();">Режим ленты</button>

				<xsl:for-each select="report/addparam">
					<div  class = "addparam">
						<div class= "top-title">
							<xsl:value-of select="@top-title"/>
							<br/>
							<div style="text-align:right">
								<xsl:value-of select="@ind"/>
							</div>
						</div>
						<div class= "right-title">
							<xsl:value-of select="@right-title"/>
						</div>
						<div class = "main-chart">
							<svg class= "chart">

								<xsl:attribute name="viewBox">
									<xsl:value-of select="@viewbox" />
								</xsl:attribute>
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


								</xsl:for-each>
								<xsl:for-each select="polyline">
									<polyline style="fill:none;stroke:black;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3">
										<xsl:attribute name="points">
											<xsl:value-of select="@points" />
										</xsl:attribute>
									</polyline>
								</xsl:for-each>
								<xsl:for-each select="lines/line">
									<polyline style="fill:none;stroke:black;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3">
										<xsl:attribute name="points">
											<xsl:value-of select="." />
										</xsl:attribute>
									</polyline>
								</xsl:for-each>
								<xsl:for-each select="artcons/line">
									<polyline style="fill:none;stroke:black;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3">
										<xsl:attribute name="points">
											<xsl:value-of select="." />
										</xsl:attribute>
									</polyline>
								</xsl:for-each>

								<xsl:for-each select="impulsleft/imp">
									<line stroke-width="0.5" stroke="black" fill="none" marker-end="url(#b-circle)">
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
									<line stroke-width="0.5" stroke="black" fill="none" marker-end="url(#b-circle)">
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
									<line stroke-width="0.5" stroke="black" fill="none" marker-end="url(#b-circle)">
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
									<line stroke-width="0.5" stroke="black" fill="none" marker-end="url(#b-circle)">
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

								<!-- <xsl:for-each select = "digressions/dig">
									<text font-size="10px" style="white-space:pre">
										<xsl:attribute name = "y" ><xsl:value-of select = "@top"/></xsl:attribute>
										<xsl:attribute name = "x" ><xsl:value-of select = "@x"/></xsl:attribute>
										<xsl:value-of select = "@note"/>
									</text>
                                </xsl:for-each> -->
								<xsl:for-each select = "digressions/m">
									<text font-size="11px" style="white-space:pre">
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
									<text font-size="11px" style="white-space:pre">
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
								<xsl:for-each select = "digressions/znach">
									<text font-size="11px" style="white-space:pre">
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
								<xsl:for-each select = "digressions/dlina">
									<text font-size="11px" style="white-space:pre">
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

								<text x="1001" y="-540" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">     0</text>
								<text x="1001" y="-465" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">     0</text>
								<text x="1001" y="-400" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">0.45</text>
								<text x="1001" y="-389" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">     0</text>
								<text x="1001" y="-324" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">0.45</text>
								<text x="1001" y="-313" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">     0</text>
								<text x="1001" y="-252" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">  0.6</text>
								<text x="1001" y="-238" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">     0</text>
								<text x="1001" y="-176" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">  0.6</text>
								<text x="1001" y="-162" style="transform: rotate(90deg);font-size: 8;text-align: right; white-space:pre">     0</text>
								<text x="-10" y="1010" style="font-size: 12;text-align: right; white-space:pre">
									<xsl:value-of select = "@fragment"/>
								</text>

								<xsl:for-each select = "speedline">
									<text font-size="11px" x="-20" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@y1"/>
										</xsl:attribute>
										<xsl:value-of select = "@note1"/>
									</text>
									<text font-size="11px" x="-20" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@y2"/>
										</xsl:attribute>
										<xsl:value-of select = "@note2"/>
									</text>
									<line x1="114" x2="730" stroke-dasharray="0.25,1"  stroke="grey" stroke-width="1" fill="none">
										<xsl:attribute name="y1">
											<xsl:value-of select="@y1"/>
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@y1"/>
										</xsl:attribute>
									</line>
									<rect width="136" height="21" style="stroke-width:0.5;fill:none;stroke: black;" x="-22">
										<xsl:attribute name="y">
											<xsl:value-of select="@y3"/>
										</xsl:attribute>
									</rect>

								</xsl:for-each>
							</svg>
						</div>
						<pre class="inform-title">  м   |  Дп   |Знач  |Дл    |Объект|    ДВ пр.   |     ДВ л.   |    СВ пр.    |     СВ л.   |     КВ пр.   |    КВ л.  |    ИН пр.  |   ИН л.  | Км</pre>
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>