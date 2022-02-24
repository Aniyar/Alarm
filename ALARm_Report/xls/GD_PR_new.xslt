<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>ГД_ПР</title>
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
					margin-top: 30;
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




								<!-- подукл пр. -->
								<!-- 1/60 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="160.7065" x2="160.7065" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="160.7065" x2="160.7065" y1="1000" y2="1002"></line>
								<!-- 1/30 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="186.67425" x2="186.67425" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="186.67425" x2="186.67425" y1="1000" y2="1002"></line>
								<!--  -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="204.24845" x2="204.24845" y1="{@minY}" y2="{@maxY}"></line>

								<!-- 1/20 -->
								<line stroke-width="0.3" fill="none" stroke-dasharray="3,3" stroke="black" x1="213.03555" x2="213.03555" y1="{@minY}" y2="{@maxY}"></line>
								<!-- <line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="213.03555" x2="213.03555" y1="{@minY}" y2="{@maxY}"></line> -->
								<line stroke-width="0,8" stroke="black" fill="none" x1="213.03555" x2="213.03555" y1="1000" y2="1002"></line>
								<!-- 1/18 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="221.82265" x2="221.82265" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="221.82265" x2="221.82265" y1="1000" y2="1002"></line>
								<!-- 1/16 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="230.60975" x2="230.60975" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="230.60975" x2="230.60975" y1="1000" y2="1002"></line>
								<!-- 1/12 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="265.22585" x2="265.22585" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="265.22585" x2="265.22585" y1="1000" y2="1002"></line>

								<!-- подукл пр. -->
								<!-- 1/60 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="306.48065" x2="306.48065" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="306.48065" x2="306.48065" y1="1000" y2="1002"></line>
								<!-- 1/30 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="332.4484" x2="332.4484" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="332.4484" x2="332.4484" y1="1000" y2="1002"></line>
								<!--  -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="350.0226" x2="350.0226" y1="{@minY}" y2="{@maxY}"></line>

								<!-- 1/20 -->
								<line stroke-width="0.3" fill="none" stroke-dasharray="3,3" stroke="black" x1="358.8097" x2="358.8097" y1="{@minY}" y2="{@maxY}"></line>
								<!-- <line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="358.8097" x2="358.8097" y1="{@minY}" y2="{@maxY}"></line> -->
								<line stroke-width="0,8" stroke="black" fill="none" x1="358.8097" x2="358.8097" y1="1000" y2="1002"></line>
								<!-- 1/18 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="367.5968" x2="367.5968" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="367.5968" x2="367.5968" y1="1000" y2="1002"></line>
								<!-- 1/16 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="376.3839" x2="376.3839" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="376.3839" x2="376.3839" y1="1000" y2="1002"></line>
								<!-- 1/12 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="411" x2="411" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="411" x2="411" y1="1000" y2="1002"></line>

								<!-- накл пк л. -->
								<!-- 1/60 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="452.2548" x2="452.2548" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="452.2548" x2="452.2548" y1="1000" y2="1002"></line>
								<!-- 1/30 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="478.22255" x2="478.22255" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="478.22255" x2="478.22255" y1="1000" y2="1002"></line>
								<!--  -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="495.79675" x2="495.79675" y1="{@minY}" y2="{@maxY}"></line>

								<!-- 1/20 -->
								<line stroke-width="0.3" fill="none" stroke-dasharray="3,3" stroke="black" x1="504.58385" x2="504.58385" y1="{@minY}" y2="{@maxY}"></line>
								<!-- <line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="504.58385" x2="504.58385" y1="{@minY}" y2="{@maxY}"></line> -->
								<line stroke-width="0,8" stroke="black" fill="none" x1="504.58385" x2="504.58385" y1="1000" y2="1002"></line>
								<!-- 1/18 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="513.37095" x2="513.37095" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="513.37095" x2="513.37095" y1="1000" y2="1002"></line>
								<!-- 1/16 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="522.15805" x2="522.15805" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="522.15805" x2="522.15805" y1="1000" y2="1002"></line>
								<!-- 1/12 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="556.77415" x2="556.77415" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="556.77415" x2="556.77415" y1="1000" y2="1002"></line>

								<!-- накл пк пр. -->
								<!-- 1/60 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="597.77415" x2="597.77415" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="597.77415" x2="597.77415" y1="1000" y2="1002"></line>
								<!-- 1/30 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="623.7419" x2="623.7419" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="623.7419" x2="623.7419" y1="1000" y2="1002"></line>
								<!--  -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="641.3161" x2="641.3161" y1="{@minY}" y2="{@maxY}"></line>

								<!-- 1/20 -->
								<line stroke-width="0.3" fill="none" stroke-dasharray="3,3" stroke="black" x1="650.1032" x2="650.1032" y1="{@minY}" y2="{@maxY}"></line>
								<!-- <line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="650.1032" x2="650.1032" y1="{@minY}" y2="{@maxY}"></line> -->
								<line stroke-width="0,8" stroke="black" fill="none" x1="650.1032" x2="650.1032" y1="1000" y2="1002"></line>
								<!-- 1/18 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="658.8903" x2="658.8903" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="658.8903" x2="658.8903" y1="1000" y2="1002"></line>
								<!-- 1/16 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="667.6774" x2="667.6774" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="667.6774" x2="667.6774" y1="1000" y2="1002"></line>
								<!-- 1/12 -->
								<line stroke-dasharray="0.5,5" stroke-width="0.5" stroke="gray" fill="none" x1="702.2935" x2="702.2935" y1="{@minY}" y2="{@maxY}"></line>
								<line stroke-width="0,8" stroke="black" fill="none" x1="702.2935" x2="702.2935" y1="1000" y2="1002"></line>


								<!-- Нер. г. пр.  0 -->
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-700.67123">1/12</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-665">1/16</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-656.520548">1/18</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-647.534246">1/20</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-622">1/30</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-596">1/60</text>

								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-554.5000">1/12</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-519.020548">1/16</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-510.534246">1/18</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-502">1/20</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-477.2000">1/30</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-451">1/60</text>

								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-409.50548">1/12</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-375">1/16</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-365">1/18</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-357.5000">1/20</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-332.7000">1/30</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-305">1/60</text>

								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-263.734246">1/12</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-229.30137">1/16</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-219.3712">1/18</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-211.8000">1/20</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-185.80137">1/30</text>
								<text x="1" style="transform: rotate(90deg);font-size: 7;text-align: right; white-space:pre" y="-159.0123">1/60</text>








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
									<polyline style="fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3">
										<xsl:attribute name="points">
											<xsl:value-of select="@points" />
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
									<rect width="136" height="10" style="stroke-width:0.5;fill:none;stroke: darkgray;">
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
										<xsl:value-of select = "@note1"/>
									</text>
									<text fill="dimgray" font-size="11px" x="0" style="white-space:pre">
										<xsl:attribute name = "y" >
											<xsl:value-of select = "@y2"/>
										</xsl:attribute>
										<xsl:value-of select = "@note2"/>
									</text>
									<polyline stroke-dasharray="0.25,1"  stroke="grey" stroke-width="1" fill="none">
										<xsl:attribute name="points">
											<xsl:value-of select="@points" />
										</xsl:attribute>
									</polyline>
									<rect width="175" height="21" style="stroke-width:0.5;fill:none;stroke: darkgray;" x="-2">
										<xsl:attribute name="y">
											<xsl:value-of select="@y3"/>
										</xsl:attribute>
									</rect>

								</xsl:for-each>



								<text x="0" font-size="8" y="31.5">
									<tspan x="4">м</tspan>
									<tspan x="18">| Отст</tspan>
									<tspan x="57">| Ст</tspan>
									<tspan x="72">| Откл</tspan>
									<tspan x="100">|Дл.</tspan>
									<tspan x="115">|Кол</tspan>
									<tspan x="130">|Огр.ск.</tspan>
									<tspan x="156">|</tspan>
									<tspan x="194">Подукл. л.</tspan>
									<tspan x="301">|</tspan>
									<tspan x="341">Подукл. пр.</tspan>
									<tspan x="447">|</tspan>
									<tspan x="485">Накл. ПК л.</tspan>
									<tspan x="593">|</tspan>
									<tspan x="630">Накл. ПК пр.</tspan>
									<tspan x="730">| Км</tspan>
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
