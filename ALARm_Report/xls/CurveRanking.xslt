<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>ВЕДОМОСТЬ РАНЖИРОВАНИЯ КРИВЫХ ПО СТЕПЕНИ РАССТРОЙСТВА</title>
				<style>
					table.main {          border-collapse: collapse; margin:auto;
					width: 100%;          }          table.main, td.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					margin: auto;
					margin-bottom:8px;
					border-collapse: collapse;

					}          td.main {          text-align: center;          }          b {          font-size: 12px;          }
				</style>
			</head>
			<body>
				<div align="center" style="margin:30px 0px 0px">

					<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
						<xsl:value-of select="report/curve/@version" />
					</p>

					<b> ВЕДОМОСТЬ РАНЖИРОВАНИЯ КРИВЫХ ПО СТЕПЕНИ РАССТРОЙСТВА (ФП-3.4)</b>
				</div>
				<div align="center">
					<table style="font-size: 12px; font-family: 'Times New Roman';width: 110%; margin: auto;margin-bottom:8px;border-collapse: collapse">

						<tr>
							<td align="left">
								ПЧ:<xsl:value-of select="report/@distance" />
							</td>
							<td align="left">
								Дорога:<xsl:value-of select="report/curve/@road" />
							</td>
						</tr>
						<tr>
							<td align="left">
								<xsl:value-of select="report/curve/@PC" />
							</td>
							<td align="left">
								Проверка:<xsl:value-of select="report/curve/@type" />
							</td>
							<td>
								<xsl:value-of select="report/curve/@period" />
							</td>


						</tr>
						<xsl:if test="report/curve/@wear != '-1'">
							<tr>
								<td align="right" style="font-style:italic;">
									<xsl:value-of select="report/curve/@wear"/>многорадиусная кривая
								</td>
							</tr>
						</xsl:if>
					</table>
				</div>
				<div align="center">
					<table class="main" style="margin: auto; margin-bottom:8px;">
						<tr>
							<td class="main">№ п/п</td>
							<td class="main">Начало кривой (км, м)</td>
							<td class="main">Радиус (м)</td>
							<td class="main">Возвышение (мм)</td>
							<td class="main">
								V<sub>пз</sub>
							</td>
							<td class="main">
								<b>
									P<sub>инт</sub>
								</b>
							</td>
							<td class="main">
								P<sub>анп</sub>
							</td>
							<td class="main">
								P<sub>пл</sub>
							</td>
							<td class="main">
								P<sub>ур</sub>
							</td>
							<td class="main">
								P<sub>&#916;</sub>
							</td>
						</tr>

						<xsl:for-each select="report/curve">
							<tr>
								<th align="right" colspan="16" style="font-size: 13px;">
									<xsl:value-of select="@direction"/> Путь: <xsl:value-of select="@track"/>
								</th>
							</tr>
							<xsl:for-each select="param_curve">
								<tr>
									<td class="main">
										<xsl:number format="1 "/>
										<xsl:value-of select="."/>
										<xsl:text></xsl:text>
									</td>
									<td class="main">
										<xsl:value-of select="@start_km"/>.<xsl:value-of select="@start_m"/>
									</td>
									<td class="main">
										<xsl:value-of select="@mid"/>
									</td>
									<td class="main">
										<xsl:value-of select="@mid_lvl"/>
									</td>
									<td class="main">
										<xsl:value-of select="@V"/>
									</td>
									<td class="main">
										<b>
											<xsl:value-of select="@P1"/>
										</b>
									</td>
									<td class="main">
										<xsl:value-of select="@P2"/>
									</td>
									<td class="main">
										<xsl:value-of select="@P3"/>
									</td>
									<td class="main">
										<xsl:value-of select="@P4"/>
									</td>
									<td class="main">
										<xsl:value-of select="@P5"/>
									</td>

								</tr>
							</xsl:for-each>
						</xsl:for-each>
					</table>
					<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
						<tr>
							<td align="left">
								Начальник &#160;&#160;<xsl:value-of select="report/curve/@PC" />
							</td>
							<td align="right">
								<xsl:value-of select="report/curve/@chief" />
							</td>
						</tr>
					</table>

				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>