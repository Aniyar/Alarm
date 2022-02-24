<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Графическая диаграмма СКО параметров ГРК</title>
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
					margin-right: auto;
					border-color: black;
					border: solid;
					width: 531.1px;
					border-width: thin;
					padding-right: 20px;
					font-size: 11;
					margin-left: 87px;
					margin-top: 50;
					}
					.top-c {
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
					font-size: 98%;
					}
					.main-chart {
					height: 940px;
					width: 585px;
					margin-right: auto;
					margin-left: auto;
					preserveAspectRatio: none;
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
					left: 144px;
					top: 507px;
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
					}

				</script>
			</head>
			<body>
				<!-- <button class="dontprint button" onclick="hidetitle();">Режим ленты</button> -->
				<xsl:for-each select="report/addparam">
					<div  class = "addparam">

						<div class= "top-title">
							<table style="font-size: 12px; font-family: Arial; margin: auto; width: 95%;">
								<tr>
									<th align="left">
										ПС № <xsl:value-of select="@PS" />
									</th>
									<th align="left" >
										Проезд: <xsl:value-of select="@tripdate" />
									</th>
									<th align="left">
										Направление: <xsl:value-of select="@direction" />
									</th>
								</tr>
								<tr>
									<th align="left" >
										Участок: <xsl:value-of select="@road" />
									</th>
									<th align="left" >
										Путь: <xsl:value-of select="@track" />
									</th>
									<th align="left" >
										ПЧ: <xsl:value-of select="@distance" />
									</th>
								</tr>
							</table>
						</div>
						<div class= "right-title">
							<xsl:value-of select="@right-title"/>
						</div>
						<div class = "main-chart">
							<svg class= "chart">
								<xsl:attribute name="viewBox">
									<xsl:value-of select="@viewbox" />
								</xsl:attribute>
								<xsl:for-each select="xgrid">
									<line stroke-width="0.5" stroke="black" fill="none">
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
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x00" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x00" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x000" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x000" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>

									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x00" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x000" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
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
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l1" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l1" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l2" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l2" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l1" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l2" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
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
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l4" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l4" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l5" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l5" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l4" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l5" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x3" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x3" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l7" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l7" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l8" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l8" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l7" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l8" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x4" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x4" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l10" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l10" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l11" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l11" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l10" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l11" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x41" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@x41" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l13" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l13" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l14" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l14" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@l13" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@l14" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@minYLine" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
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
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
									<line stroke-width="0.5" stroke="black" fill="none">
										<xsl:attribute name="x1">
											<xsl:value-of select="@x0" />
										</xsl:attribute>
										<xsl:attribute name="x2">
											<xsl:value-of select="@lineLow" />
										</xsl:attribute>
										<xsl:attribute name="y1">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
										<xsl:attribute name="y2">
											<xsl:value-of select="../@maxY" />
										</xsl:attribute>
									</line>
								</xsl:for-each>
								<xsl:for-each select="graphics/StraighteningLeft">
									<rect  stroke="black" style="fill: Gray" stroke-width="0.3" >
										<xsl:attribute name="height">
											<xsl:value-of select="@h" />
										</xsl:attribute>
										<xsl:attribute name="width">
											<xsl:value-of select="@w" />
										</xsl:attribute>
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
									</rect>
								</xsl:for-each>
								<xsl:for-each select="graphics/StraighteningRigth">
									<rect  stroke="black" style="fill: Gray" stroke-width="0.3" >
										<xsl:attribute name="height">
											<xsl:value-of select="@h" />
										</xsl:attribute>
										<xsl:attribute name="width">
											<xsl:value-of select="@w" />
										</xsl:attribute>
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
									</rect>
								</xsl:for-each>
								<xsl:for-each select="graphics/Level">
									<rect  stroke="black" style="fill: Gray" stroke-width="0.3" >
										<xsl:attribute name="height">
											<xsl:value-of select="@h" />
										</xsl:attribute>
										<xsl:attribute name="width">
											<xsl:value-of select="@w" />
										</xsl:attribute>
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
									</rect>
								</xsl:for-each>
								<xsl:for-each select="graphics/DrawdownLeft">
									<rect   stroke="black" style="fill: Gray" stroke-width="0.3" >
										<xsl:attribute name="height">
											<xsl:value-of select="@h" />
										</xsl:attribute>
										<xsl:attribute name="width">
											<xsl:value-of select="@w" />
										</xsl:attribute>
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
									</rect>
								</xsl:for-each>
								<xsl:for-each select="graphics/DrawdoownRigth">
									<rect   stroke="black" style="fill: Gray" stroke-width="0.3" >
										<xsl:attribute name="height">
											<xsl:value-of select="@h" />
										</xsl:attribute>
										<xsl:attribute name="width">
											<xsl:value-of select="@w" />
										</xsl:attribute>
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
									</rect>
								</xsl:for-each>
								<xsl:for-each select="graphics/TrackWidth">
									<rect  stroke="black" style="fill: Gray" stroke-width="0.3" >
										<xsl:attribute name="height">
											<xsl:value-of select="@h" />
										</xsl:attribute>
										<xsl:attribute name="width">
											<xsl:value-of select="@w" />
										</xsl:attribute>
										<xsl:attribute name="x">
											<xsl:value-of select="@x" />
										</xsl:attribute>
										<xsl:attribute name="y">
											<xsl:value-of select="@y" />
										</xsl:attribute>
									</rect>
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

									<rect stroke="black" height="0.1" width="5" x="3.5" y="{@x}"></rect>
									<rect stroke="black" height="0.1" width="5" x="119.5" y="{@x}"></rect>
									<rect stroke="black" height="0.1" width="5" x="224.5" y="{@x}"></rect>
									<rect stroke="black" height="0.1" width="5" x="312.9" y="{@x}"></rect>
									<rect stroke="black" height="0.1" width="5" x="401" y="{@x}"></rect>
									<rect stroke="black" height="0.1" width="5" x="489.5" y="{@x}"></rect>

								</xsl:for-each>

								<!-- нижняя линия -->
								<line stroke-width="0.5" stroke="black" fill="none" x1="-11" x2="575" y1="0" y2="0"></line>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-6" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-88" font-weight="bold">
									10
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-122" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-202" font-weight="bold">
									10
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-226" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-290" font-weight="bold">
									5
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-314" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-378" font-weight="bold">
									5
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-402" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-466" font-weight="bold">
									5
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-490" font-weight="bold">
									0
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="12" y="-554" font-weight="bold">
									5
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="40" y="-91.5" font-weight="bold">
									СКО_рихт л.
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="40" y="-206.9" font-weight="bold">
									СКО_рихт п.
								</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="40" y="-297" font-weight="bold">СКО_Ур</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="40" y="-385" font-weight="bold">СКО_прос л.</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="40" y="-473.5" font-weight="bold">СКО_прос п.</text>
								<text transform="rotate(90)" style="font-size: 10px;" x="40" y="-562" font-weight="bold">СКО_ШК</text>
							</svg>
						</div>
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>