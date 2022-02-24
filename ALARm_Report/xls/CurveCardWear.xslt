<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>КАРТОЧКА КРИВОЙ</title>
				<style>
					table.main, td.main {
					border-collapse: collapse;
					margin: auto;
					width: 680px;
					border: 1px solid black;
					font-size: 9px;
					font-family: Arial;
					}
					h1.main {
					page-break-before: always;
					}
					b {
					font-size: 14px;
					font-family: Arial;
					}
					.curve_chart {
					width: 620px;
					top: 0;
					right: 0;
					bottom: 0;
					left: 0;
					margin: auto;
					height: 126px;
					}
					svg {
					border: 1px solid;
					height: 125px;
					width: 620px;
					preserveAspectRatio: none;					}					.zerotext {						top: -25px;						left: -32px;						position: relative;						text-align: right;						width: 30px;					}					.titletext {						top: -146px;						left: 2px;						position: relative;						text-align: left;						font-size: 9px;                        font-family: Arial;					}					.xaxis {						vector-effect: non-scaling-stroke;						stroke: rgb(128, 128, 128);						stroke-width: 1;						stroke-dasharray: 3,3;					}					.yaxis {						vector-effect: non-scaling-stroke;						stroke: red;						stroke-width: 1;						stroke-dasharray: 3,3;					}					.rectangles {						vector-effect: non-scaling-stroke;						fill: green;						stroke: green;						stroke-width: 1;					}
				</style>
			</head>

			<body>
				<xsl:for-each select="report/curve">
					<xsl:choose>
						<xsl:when test="@ismulti">
							<div align="center" style="page-break-before:always; margin:50px 20px 0px">
								<b> Многорадиусная кривая с боковым износом  наружного рельса более 4-й (3-й) степени дефектности (ФП-3.3.И)</b>
							</div>
						</xsl:when>
						<xsl:otherwise>
							<div align="center" style="page-break-before:always; margin:50px 0px 0px">

								<p  align="left" style="color:black;width: 680px;height: 1%;font-size:7px;">
									<xsl:value-of select="@version" />
								</p>

								<!-- <b>Кривая с боковым износом  наружного рельса более 4-й (3-й) степени дефектности (ФП-3.2.И)</b> -->
								<b>Карточка кривой (ФП-3.2И)</b>
							</div>
						</xsl:otherwise>
					</xsl:choose>
					<div align="center">
						<table width="680px" style="font-size: 9px;font-family: Arial;margin-top:10px">
							<tr>
								<td align="left">
									ПЧ:                                    <xsl:value-of select="../@distance" />
								</td>

								<td align="left">
									Дорога:                                    <xsl:value-of select="@road" />
								</td>

								<td align="left">
									Направление:                                    <xsl:value-of select="@direction" />
								</td>
								<td align="left">
									Путь:                                    <xsl:value-of select="@track" />
								</td>


							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@PC" />
								</td>
								<td align="left">
									Проверка:                                    <xsl:value-of select="@type" />
								</td>
								<td align="left">
									<xsl:value-of select="@period" />
								</td>
							</tr>
						</table>
					</div>
					<div align="center">
						<table class="main">
							<tr>
								<td colspan="2" align="right" style="border-bottom:0" class="main">
									Кривых:                                    <xsl:value-of select="count(../*)" />
								</td>
								<td colspan="8" align="center" style="border-bottom:0;" class="main">Характеристики кривой</td>
								<td style="border-right:0; border-bottom:0;" class="main">1-я</td>
								<td colspan="4" align="center" style="border-left:0; border-right:0; border-bottom:0;" class="main">Переходные</td>
								<td align="right" style="border-left:0; border-bottom:0;" class="main">2-я</td>
							</tr>
							<tr>
								<td colspan="2" align="right" style="border-top:0; border-bottom:0;" class="main">
									<xsl:value-of select="@side" />
									,                                    <xsl:value-of select="@order" />
								</td>
								<td colspan="2" align="center" style="border-top:0; border-right:0; border-bottom:0;" class="main">Начало</td>
								<td colspan="2" align="center">Конец</td>
								<td colspan="4" />
								<td colspan="3" align="center" style="border-top:0; border-right:0; border-bottom:0;" class="main">отвод</td>
								<td colspan="3" align="center" style="border-top:0; border-bottom:0; border-left:0;" class="main">отвод</td>
							</tr>
							<tr>
								<td colspan="2" style="border-top:0;" class="main" />
								<td align="center" style="border-top:0; border-right:0;" class="main">км</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">м</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">км</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">м</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">дл.</td>
								<td colspan="3" align="center" style="border-top:0; border-left:0;" class="main">уг.</td>
								<td align="center" style="border-top:0; border-right:0;" class="main">макс.</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">ср.</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">дл.</td>
								<td align="center" style="border-top:0; border-right:0;" class="main">макс.</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">ср.</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">дл.</td>
							</tr>
							<tr>
								<td colspan="2" align="right" style="border-bottom:0;" class="main">план</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="param_curve/@start_km" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@start_m" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@final_km" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@final_m" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@len" />
								</td>
								<td colspan="3" align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@angle" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_max1" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_mid1" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_len1" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_max2" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_mid2" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_len2" />
								</td>
							</tr>
							<tr>
								<td colspan="2" align="right" style="border-top:0;" class="main">уровень</td>
								<td style="border-top:0; border-right:0;" class="main" />
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@start_lvl" />
								</td>
								<td style="border-top:0; border-right:0; border-left:0;" class="main" />
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@final_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="param_curve/@len_lvl" />
								</td>
								<td colspan="3" style="border-top:0; border-left:0;" class="main" />
								<td align="center" style="border-top:0; border-right:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_max1_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_mid1_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_len1_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_max2_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_mid2_lvl" />
								</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">
									<xsl:value-of select="withdrawal/@tap_len2_lvl" />
								</td>
							</tr>
							<tr>
								<td colspan="2" align="right" style="border-bottom:0;" class="main">
									<!--пр.<xsl:value-of select="param_circle_curve/@pr" />-->
								</td>
								<td colspan="8" align="center" style="border-bottom:0;" class="main">Характеристики круговой кривой</td>
								<td colspan="2" align="center" class="main">
									A<sub>нп</sub>
								</td>
								<td align="center" class="main">Ψ</td>
								<td align="center" style="border-right:0;" class="main">Скор.</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">пасс.</td>
								<td align="center" style="border-left:0;" class="main">груз.</td>
							</tr>
							<tr>
								<td colspan="2" align="right" style="border-top:0; border-bottom:0;" class="main">
									<!--сл.<xsl:value-of select="param_circle_curve/@sl" />-->
								</td>
								<td colspan="2" align="center" style="border-top:0; border-right:0; border-bottom:0;" class="main">Начало</td>
								<td colspan="2" align="center">Конец</td>
								<td style="border-top:0; border-bottom:0; border-left:0;" class="main" />
								<td colspan="3" align="center" style="border-top:0; border-bottom:0;" class="main">Рад./Уров./Шаб.</td>
								<td colspan="2" align="center" style="border-bottom:0;" class="main">
									<xsl:value-of select="computing/@a1" />
									/                                    <xsl:value-of select="computing/@a2" />
								</td>
								<td align="center" style="border-bottom:0;" class="main">
									<xsl:value-of select="computing/@psi1" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									V<sub>пз</sub>
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@pass1" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@frei1" />
								</td>
							</tr>
							<tr>
								<td colspan="2" style="border-top:0;" class="main" />
								<td align="center" style="border-top:0; border-right:0;" class="main">км</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">м</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">км</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">м</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">дл.</td>
								<td align="center" style="border-top:0; border-right:0;" class="main">мин.</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">макс.</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">ср.</td>
								<td colspan="2" align="center" style="border-top:0; border-bottom:0;" class="main">
									<xsl:value-of select="computing/@a3" />
									/                                    <xsl:value-of select="computing/@a4" />
								</td>
								<td style="border-top:0; border-bottom:0;" class="main" />
								<td align="center" style="border-top:0; border-right:0; border-bottom:0;" class="main">
									V<sub>кр</sub>
								</td>
								<td align="center">
									<xsl:value-of select="speed/@pass2" />
								</td>
								<td align="center" style="border-top:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@frei2" />
								</td>
							</tr>
							<tr>
								<td colspan="2" align="right" style="border-bottom:0;" class="main">план</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="param_circle_curve/@start_km" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@start_m" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@final_km" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@final_m" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@len" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="param_circle_curve/@rad_min" />
								</td>
								<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@rad_max" />
								</td>
								<td align="center" style="border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@rad_mid" />
								</td>
								<td colspan="2" align="center" style="border-top:0;" class="main">
									<xsl:value-of select="computing/@a5" />
								</td>
								<td align="center" style="border-top:0;" class="main">
									<xsl:value-of select="computing/@psi2" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-bottom:0;" class="main">
									V<sub>пр</sub>
								</td>
								<td align="center">
									<xsl:value-of select="speed/@pass3" />
								</td>
								<td align="center" style="border-top:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@frei3" />
								</td>
							</tr>
							<tr>
								<td colspan="2" align="right" style="border-top:0; border-bottom:0;" class="main">уровень</td>
								<td style="border-top:0; border-right:0; border-bottom:0;" class="main" />
								<td align="center">
									<xsl:value-of select="param_circle_curve/@start_lvl" />
								</td>
								<td />
								<td align="center">
									<xsl:value-of select="param_circle_curve/@final_lvl" />
								</td>
								<td align="center" style="border-top:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@len_lvl" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-bottom:0;" class="main">
									<xsl:value-of select="param_circle_curve/@lvl_min" />
								</td>
								<td align="center">
									<xsl:value-of select="param_circle_curve/@lvl_max" />
								</td>
								<td align="center" style="border-top:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@lvl_mid" />
								</td>
								<td colspan="3" align="center" class="main">
									Р                                    <sub>растр</sub>=                                    <xsl:value-of select="computing/@P" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-bottom:0;" class="main">
									V<sub>из</sub>
								</td>
								<td align="center">
									<xsl:value-of select="speed/@pass4" />
								</td>
								<td align="center" style="border-top:0; border-bottom:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@frei4" />
								</td>
							</tr>
							<tr>
								<td colspan="2" align="right" style="border-top:0; border-right:0;" class="main">шаблон</td>
								<td colspan="5" style="border-top:0;" class="main" />
								<td align="center" style="border-top:0; border-right:0;" class="main">
									<xsl:value-of select="param_circle_curve/@gauge_min" />
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@gauge_max" />
								</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">
									<xsl:value-of select="param_circle_curve/@gauge_mid" />
								</td>
								<td colspan="3" align="center" style="border-bottom:0;" class="main">
									<b>V</b>
									<sub>+03</sub>
									<b>
										=                                        <xsl:value-of select="computing/@V1" />
									</b>
								</td>
								<td align="center" style="border-top:0; border-right:0;" class="main">
									V<sub>сопр</sub>
								</td>
								<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@pass5" />
								</td>
								<td align="center" style="border-top:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@frei5" />
								</td>
							</tr>
							<tr>
								<td colspan="2" align="right" class="main">Боковой износ</td>
								<td colspan="2" align="center" class="main">
									&gt;6мм:<xsl:value-of select="side_wear/@mm6" />
								</td>
								<td colspan="2" align="center" class="main">
									&gt;10мм:<xsl:value-of select="side_wear/@mm10" />
								</td>
								<td colspan="2" align="center" class="main">
									&gt;15мм:<xsl:value-of select="side_wear/@mm15" />
								</td>
								<td align="center" style="border-right:0;" class="main">
									<xsl:value-of select="side_wear/@max" />
								</td>
								<td align="center" style="border-left:0;" class="main">
									<xsl:value-of select="side_wear/@mid" />
								</td>
								<td colspan="3" align="center" style="border-top:0;" class="main">
									V<sub>-03</sub>=                                    <xsl:value-of select="computing/@V2" />
								</td>
								<td align="center" style="border-right:0;" class="main">
									V<sub>дп</sub>
								</td>
								<td align="center" style="border-right:0; border-left:0;" class="main">
									<xsl:value-of select="speed/@pass6" />
								</td>
								<td align="center" style="border-left:0;" class="main">
									<xsl:value-of select="speed/@frei6" />
								</td>
							</tr>
							<xsl:if test="@ismulti">
								<tr>
									<td colspan="2" rowspan="2" class="main"/>
									<td colspan="8" align="center" style="border-bottom:0;" class="main">Характеристики элементарных кривых</td>
									<td colspan="3" style="border-bottom:0;" class="main"/>
									<td colspan="3" rowspan="2" class="main"/>
								</tr>
								<tr>
									<td colspan="2" align="center" style="border-top:0; border-right:0;" class="main">Начало</td>
									<td colspan="2" align="center" style="border-left:0; border-top:0; border-right:0;" class="main">Конец</td>
									<td align="center" style="border-left:0; border-top:0;" class="main">дл.</td>
									<td align="center" style="border-right:0; border-top:0;" class="main">ср.рад./уров.</td>
									<td align="center" style="border-left:0; border-top:0; border-right:0;" class="main">ср.отв.</td>
									<td align="center" style="border-top:0; border-left:0;" class="main">дл.</td>
									<td colspan="2" align="center" style="border-top:0;" class="main">
										A<sub>нп</sub>
									</td>
									<td align="center" style="border-top:0;" class="main">Ψ</td>
								</tr>
								<xsl:for-each select="multicurves">
									<tr>
										<td rowspan="2" valign="top" align="center" class="main">
											<xsl:value-of select="@order"/>
										</td>
										<td align="right" style="border-bottom=0;" class="main">план</td>
										<td align="center" style="border-right:0; border-bottom:0;" class="main">
											<xsl:value-of select="@start_km" />
										</td>
										<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@start_m" />
										</td>
										<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@final_km" />
										</td>
										<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@final_m" />
										</td>
										<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@len" />
										</td>
										<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@radius" />
										</td>
										<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@midtap" />
										</td>
										<td align="center" style="border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@len2" />
										</td>
										<td colspan="2" align="center" style="border-right:0; border-bottom:0;" class="main">
											<xsl:value-of select="@anp" />
										</td>
										<td align="center" style="border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@psi" />
										</td>
										<td align="center" style="border-right:0; border-bottom:0;" class="main">
											V<sub>пз</sub>
										</td>
										<td align="center" style="border-right:0; border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@pass1" />
										</td>
										<td align="center" style="border-bottom:0; border-left:0;" class="main">
											<xsl:value-of select="@frei1" />
										</td>
									</tr>
									<tr>
										<td align="right" style="border-top:0;" class="main">уров.</td>
										<td style="border-top:0; border-right:0;" class="main" />
										<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
											<xsl:value-of select="@start_lvl" />
										</td>
										<td style="border-top:0; border-right:0; border-left:0;" class="main" />
										<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
											<xsl:value-of select="@final_lvl" />
										</td>
										<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
											<xsl:value-of select="@len_lvl" />
										</td>
										<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
											<xsl:value-of select="@lvl" />
										</td>
										<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
											<xsl:value-of select="@midtap_lvl" />
										</td>
										<td align="center" style="border-top:0; border-left:0;" class="main">
											<xsl:value-of select="@len2_lvl" />
										</td>
										<td colspan="2" align="center" style="border-top:0; border-right:0;" class="main">
											<xsl:value-of select="@anp2" />
										</td>
										<td style="border-top:0; border-left:0;" class="main"/>
										<td align="center" style="border-top:0; border-right:0;" class="main">
											V<sub>дп</sub>
										</td>
										<td align="center" style="border-top:0; border-right:0; border-left:0;" class="main">
											<xsl:value-of select="@pass2" />
										</td>
										<td align="center" style="border-top:0; border-left:0;" class="main">
											<xsl:value-of select="@frei2" />
										</td>
									</tr>
									<tr></tr>
								</xsl:for-each>
							</xsl:if>
						</table>
					</div>
					<br />
					<div class="curve_chart">
						<svg width="620px" preserveAspectRatio="none" vector-effect="non-scaling-stroke">
							<xsl:attribute name="viewBox">
								<xsl:value-of select="@viewbox" />
							</xsl:attribute>
							<xsl:for-each select="xaxis/line">
								<line y1="5" class="xaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@x1" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@x1" />
									</xsl:attribute>
									<xsl:attribute name="y2">
										<xsl:value-of select="@y2" />
									</xsl:attribute>
								</line>
							</xsl:for-each>
							<xsl:for-each select="xaxis/xparam">
								<line y1="0" y2="0" class="xaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
								<line y1="-6" y2="-6" class="yaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
								<line y1="-10" y2="-10" class="yaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
								<line y1="-15" y2="-15" class="yaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
							</xsl:for-each>
							<xsl:for-each select="xaxis/rectangle">
								<rect class="rectangles">
									<xsl:attribute name="x">
										<xsl:value-of select="@x"/>
									</xsl:attribute>
									<xsl:attribute name="y">
										<xsl:value-of select="@y"/>
									</xsl:attribute>
									<xsl:attribute name="width">
										<xsl:value-of select="@width"/>
									</xsl:attribute>
									<xsl:attribute name="height">
										<xsl:value-of select="@height"/>
									</xsl:attribute>
								</rect>
							</xsl:for-each>
							<polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:black;stroke-width:1">
								<xsl:attribute name="points">
									<xsl:value-of select="@radius" />
								</xsl:attribute>
							</polyline>
							<polyline style="vector-effect:non-scaling-stroke;fill:none; stroke-linejoin:round;stroke:green;stroke-width:1">
								<xsl:attribute name="points">
									<xsl:value-of select="@radius-average" />
								</xsl:attribute>
							</polyline>
							<polyline style="vector-effect:non-scaling-stroke;fill:none; stroke-linejoin:round;stroke:red;stroke-width:1">
								<xsl:attribute name="points">
									<xsl:value-of select="@gauge" />
								</xsl:attribute>
							</polyline>
						</svg>
						<div class="zerotext">0</div>
						<div class="titletext">
							Радиус (м) -                            <xsl:value-of select="@radius-length"/>
						</div>
						<xsl:for-each select="marks/mark">
							<div>
								<xsl:attribute name="style">
									top: <xsl:value-of select="@topValue"/>
									px;left: -32px;position:relative;width: 30px;text-align: right;color: red;
								</xsl:attribute>
								<xsl:value-of select="@sign"/>
							</div>
						</xsl:for-each>
					</div>
					<div class="curve_chart">
						<svg preserveAspectRatio="none" vector-effect="non-scaling-stroke">
							<xsl:attribute name="viewBox">
								<xsl:value-of select="@viewbox-level"/>
							</xsl:attribute>
							<xsl:for-each select="xaxis/line">
								<line class="xaxis" y1="5">
									<xsl:attribute name="x1">
										<xsl:value-of select="@x1" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@x1" />
									</xsl:attribute>
									<xsl:attribute name="y2">
										<xsl:value-of select="@y2-level" />
									</xsl:attribute>
								</line>
							</xsl:for-each>
							<xsl:for-each select="xaxis/xparam">
								<line y1="0" y2="0" class="xaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
								<line y1="-50" y2="-50" class="yaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
								<line y1="-100" y2="-100" class="yaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
							</xsl:for-each>
							<xsl:for-each select="xaxis/rectangle_lvl">
								<rect class="rectangles">
									<xsl:attribute name="x">
										<xsl:value-of select="@x"/>
									</xsl:attribute>
									<xsl:attribute name="y">
										<xsl:value-of select="@y"/>
									</xsl:attribute>
									<xsl:attribute name="width">
										<xsl:value-of select="@width"/>
									</xsl:attribute>
									<xsl:attribute name="height">
										<xsl:value-of select="@height"/>
									</xsl:attribute>
								</rect>
							</xsl:for-each>
							<polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:black;stroke-width:1">
								<xsl:attribute name="points">
									<xsl:value-of select="@level" />
								</xsl:attribute>
							</polyline>
							<polyline style="vector-effect:non-scaling-stroke;fill:none; stroke-linejoin:round;stroke:green;stroke-width:1">
								<xsl:attribute name="points">
									<xsl:value-of select="@level-average" />
								</xsl:attribute>
							</polyline>
						</svg>
						<div class="zerotext">0</div>
						<div class="titletext">Уровень (мм)</div>
						<xsl:for-each select="marks/markLvl">
							<div>
								<xsl:attribute name="style">
									top: <xsl:value-of select="@topValue"/>
									px;left: -32px;position:relative;width: 30px;text-align: right;color: red;
								</xsl:attribute>
								<xsl:value-of select="@sign"/>
							</div>
						</xsl:for-each>
					</div>
					<div class="curve_chart">
						<svg preserveAspectRatio="none" vector-effect="non-scaling-stroke">
							<xsl:attribute name="viewBox">
								<xsl:value-of select="@boost-level" />
							</xsl:attribute>
							<polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:blue;stroke-width:1">
								<xsl:attribute name="points">
									<xsl:value-of select="@freightboost" />
								</xsl:attribute>
							</polyline>
							<polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:red;stroke-width:1">
								<xsl:attribute name="points">
									<xsl:value-of select="@passboost" />
								</xsl:attribute>
							</polyline>
							<xsl:for-each select="xaxis/line">
								<line y1="5" class="xaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@x1" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@x1" />
									</xsl:attribute>
									<xsl:attribute name="y2">
										<xsl:value-of select="@y2" />
									</xsl:attribute>
								</line>
							</xsl:for-each>
							<xsl:for-each select="xaxis/xparam">
								<line y1="0" y2="0" class="xaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
								<line y1="-0.3" y2="-0.3" class="xaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
								<line y1="-0.7" y2="-0.7" class="xaxis">
									<xsl:attribute name="x1">
										<xsl:value-of select="@minX" />
									</xsl:attribute>
									<xsl:attribute name="x2">
										<xsl:value-of select="@maxX" />
									</xsl:attribute>
								</line>
							</xsl:for-each>
						</svg>
						<div class="zerotext">0</div>
						<div class="titletext">Анп (м/с/с)</div>
						<div style="top: -138.364px;left: -32px;position:relative;width: 30px;text-align: right;">0,7</div>
						<div style="top: -110.18px;left: -32px;position:relative;width: 30px;text-align: right;">0,3</div>
					</div>
					<div style="margin:auto;width:620px;position:relative;">
						<xsl:for-each select="xaxis/labels/label">
							<div>
								<xsl:attribute name="style">
									<xsl:value-of select="@style"/>
								</xsl:attribute>
								<xsl:value-of select="@value" />
							</div>
						</xsl:for-each>
					</div>
					<br/>
					<div style="margin:auto;width:620px;position:relative;">
						<xsl:for-each select="xaxis/kmlabels/label">
							<div>
								<xsl:attribute name="style">
									<xsl:value-of select="@style"/>
								</xsl:attribute>
								<b>
									<xsl:value-of select="@value" />
									км
								</b>
							</div>
						</xsl:for-each>
					</div>
					<div align="center">
						<table width="620px" style="margin-top:15px;font-size: 9px;font-family: Arial;">
							<tr>
								<th align="left">
									Порог износа - более                                    <xsl:value-of select="../@wear" />
								</th>
							</tr>
						</table>
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>