<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>
			<head>
				<title>Графическая диаграмма «отклонения от паспортного положения в плане»</title>
				<style>
					.addparam {
					width: 760px;
					margin:auto;
					page-break-before:always;


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
					top: -1215;
					font-size: 9px;
					left: 377px;
					width: 20px;
					height: 550px;
					position: relative;
					margin-bottom: -550px;
					text-align: right;
					}
					.bottom-labels-0deg {
					top: -899;
					font-size: 9px;
					left: 63px;
					width: 20px;
					height: 550px;
					position: relative;
					margin-bottom: -550px;
					text-align: right;
					}
					.top-title {
					margin-right: 99px;
					border-color: black;
					width: 574.1px;
					border-width: thin;
					padding-right: 10px;
					font-size: 11;
					margin-left: auto;
					margin-top: 50;
					}
					.top-r {
					margin-top: 30px;
					<!-- margin-right: 15px; -->
					border-color: black;
					<!-- /* border: solid; */ -->
					width: 575px;
					height: 12px;
					border-width: thin;
					text-align: RIGHT;
					<!-- padding-right: 10px; -->
					<!-- font-size: 11;
                        font-family: Arial; -->
					margin-left: auto;
					margin-bottom: 10px;
					}
					.label-name {
					<!-- margin-top: 50px; -->
					margin-right: auto;
					border-color: black;
					<!-- /* border: solid; */ -->
					width: 575px;
					height: 12px;
					border-width: thin;
					text-align: center;
					padding-right: 10px;
					<!-- font-size: 11;
                        font-family: Arial; -->
					margin-left: auto;
					margin-bottom: 20px;
					font-size: 75%;
					margin-top: -45;
					}
					.main-chart {
					height: 940px;
					width: 585px;
					margin-right: auto;
					margin-left: auto;
					preserveAspectRatio: none;
					margin-top: -7px;
					}
					.chart {
					margin-right: 0px;
					margin-left: auto;
					width: 585px;
					height: 940px;
					margin-bottom: 0.5px solid;
					}
					.right-title {
					position: relative;
					left: 179px;
					top: 518px;
					transform: rotate(90deg);
					font-size: 9.3px;
					font-family: Arial;
					width: 1000px;
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
					.nkm {
					font-size: 14;
					font-family: Arial;
					text-align: right;
					margin-right: 5px;
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
					var divs = document.querySelectorAll('.top-title');
					divs.forEach(function(div) {
					div.style.display = div.style.display == "none" ? "block" : "none";
					});

					var divs = document.querySelectorAll('.top-c');
					divs.forEach(function(div) {
					div.style.display = div.style.display == "none" ? "block" : "none";
					});

					var divs = document.querySelectorAll('.top-r');
					divs.forEach(function(div) {
					div.style.display = div.style.display == "none" ? "block" : "none";
					});

					var divs = document.querySelectorAll('.label-name');
					divs.forEach(function(div) {
					div.style.display = div.style.display == "none" ? "block" : "none";
					});
					}

				</script>
			</head>
			<body>
				<!-- <button class="dontprint button" onclick="hidetitle();">Режим ленты</button> -->
				<xsl:for-each select="report/addparam">
					<div class="addparam">
						<tr>
							<td>
								<b>
									<p align="justify" style="color:black; font-size:14px">  &#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;Графическая диаграмма «отклонения от паспортного положения в плане </p>
								</b>
							</td>
						</tr>
						<div class="top-title">
							&#8195;&#8195;&#8195;Напр.: <xsl:value-of select="@directNaprav" />
							&#8195;&#8195;&#8195; Путь: <xsl:value-of select="@Put" />
							&#8195;&#8195;&#8195; Участок: <xsl:value-of select="@Uchastok" />
							&#8195;&#8195;&#8195;&#8195; Поездка: <xsl:value-of select="@TravelDate" />
							<b>

								<br />
								<br />
								&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;неровн_плана
								&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195; Кривизна
								&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;откл. от прямой
							</b>
						</div>
						<div class= "right-title">
							<xsl:value-of select="@right-title"/>
						</div>
						<div class="main-chart">
							<svg class="chart">
								<xsl:attribute name="viewBox">
									<xsl:value-of select="@viewbox" />
								</xsl:attribute>
								<xsl:for-each select="xgrid">


									<line stroke-width="1" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x0" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x0" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<!-- <line stroke-dasharray="3,3" stroke-width="0.3" stroke="black" fill="none">  0
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none"> -->
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x00" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x00" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="3,3" stroke-width="0.3" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x000" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x000" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x0000" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x0000" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>


									<line stroke-width="1" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x1" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x1" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l1" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l1" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="3,3" stroke-width="0.3" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l2" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l2" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l3" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l3" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>


									<line stroke-width="1" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x2" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x2" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l4" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l4" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l5" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l5" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l6" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l6" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="3,3" stroke-width="0.3" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l7" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l7" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l8" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l8" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l9" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l9" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>
									<line stroke-dasharray="0.5,3" stroke-width="0.5" stroke="gray" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l10" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l10" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>


									<line stroke-width="1" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x7" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x7" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>


									<!--гориз Полный сызык -->
									<line stroke-width="1" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x0" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@lineLow" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@minY" />
										</xsl:attribute>
									</line>
									<line stroke-width="1" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x0" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@lineLow" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@maxY1" />
										</xsl:attribute>
									</line>


								</xsl:for-each>
								<xsl:for-each select="lines/NerovPlana">
									<polyline style="fill:none;stroke:black;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3">
										<xsl:attribute name="points">
											<xsl:value-of select="." />
										</xsl:attribute>
									</polyline>
								</xsl:for-each>
								<xsl:for-each select="lines/Krivizna">
									<polyline style="fill:none;stroke:black;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3">
										<xsl:attribute name="points">
											<xsl:value-of select="." />
										</xsl:attribute>
									</polyline>
								</xsl:for-each>
								<xsl:for-each select="km/text">

									<text transform="rotate(90)" style="font-size: 10px" >
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
										<xsl:value-of select="."/>
									</text>

									<line stroke-dasharray="3,3" stroke-width="0.3" stroke="black" fill="none">
										<xsl:attribute name="x1">
											0
										</xsl:attribute>
										<xsl:attribute name="x2">
											611
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="@x" />
										</xsl:attribute>
									</line>
								</xsl:for-each>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-592" font-weight="bold">
									300
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-546" font-weight="bold">
									200
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-496" font-weight="bold">
									100
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-457" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-416" font-weight="bold">
									-100
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-376" font-weight="bold">
									-200
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-337" font-weight="bold">
									-300
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-289" font-weight="bold">
									50
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-237" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-187" font-weight="bold">
									-50
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-156" font-weight="bold">
									25
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-81" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="0" y="-6" font-weight="bold">
									-25
								</text>

							</svg>
						</div>
						<div class="bottom-labels">
							<xsl:for-each select="bl">
								<div>
									<xsl:attribute name="style">
										<xsl:value-of select="@style" />
									</xsl:attribute>
									<xsl:value-of select="." />
								</div>
							</xsl:for-each>
						</div>

					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>