<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title> Ведомость участков пути с ослабленными промежуточными скреплениями   </title>
				<style>
					td {
					padding-left: 5px;
					}
					table.main, td.main, th.main {

					border-collapse: collapse;
					border: 1.5px solid black;
					font-size: 12px;
					font-family: 'Times New Roman';
					}

					table.main {
					width: 100%;
					margin: auto;
					}
					b {
					font-size: 18px;
					}
					thead {
					background: #f5e8d0;
					/* Цвет фона заголовка */
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

				<xsl:for-each select="report/trip">

					<div   style="page-break-before:always; margin:50px 0px 0px">


						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>


						<H4 align = "center">Ведомость участков пути с ослабленными промежуточными скреплениями </H4>


					</div>
					<div align="center">
						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<td align="left">
									ПЧ: <xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога: <xsl:value-of select="@road" />
								</td>

							</tr>
							<tr>

								<td align="left">
									<xsl:value-of select="@ps" />
								</td>

								<td align="left">
									Проверка:<xsl:value-of select="@check" />
								</td>

								<td align="left">
									<xsl:value-of select="@periodDate" />
								</td>
								<td>Пороговое значение РШК – 5мм</td>
							</tr>
						</table>
					</div>
					<div align="center">

						<table class="main">
							<thead>
								<tr>
									<td  align="center" rowspan="1" colspan="2" class="main">  Начало отжатия  </td>
									<td  align="center"  rowspan="2" colspan="1" class="main">
										Длина отжатия,<br/>м
									</td>
									<td   align="center" rowspan="2" colspan="1" class="main">
										Макс. РШК,<br/>мм
									</td>
									<td   align="center" rowspan="2" colspan="1" class="main">
										Макс. РШК,<br/>мм
									</td>
									<td   align="center" rowspan="2" colspan="1" class="main">Радиус, </td>
									<td   align="center" rowspan="2" colspan="1" class="main">
										Vпз<br/>
									</td>
									<td   align="center" rowspan="2" colspan="1" class="main">
										Промежуточное скрепление<br/>
									</td>
								</tr>

								<tr>
									<td align="center" colspan="1" class="main">Км</td>
									<td align="center" colspan="1" class="main">м</td>


								</tr>
								<tr>
									<td  class="main" colspan="11">
										Направление:   <xsl:value-of select="@direction"/> Путь:  <xsl:value-of select="@track"/>
									</td>

								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="note">
									<tr class="tr">

										<td align="center">
											<xsl:value-of select="@km"/>
										</td>
										<td align="center">
											<xsl:value-of select="@m"/>
										</td>
										<td align="center">
											<xsl:value-of select="@found_date"/>
										</td>

										<td align="center" >
											<xsl:value-of select="@deviation"/>
										</td>
										<td align="center" >
											<xsl:value-of select="@count"/>
										</td>
									</tr>
								</xsl:for-each>
							</tbody>

						</table>
					</div>

					<table style="font-size: 12px; width: 80%;height: 5%; margin: auto;">
						<tr>
							<td align="left">
								Начальник &#160;&#160;<xsl:value-of select="@ps" />
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
								&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;


								<xsl:value-of select="@chief" />
							</td>
						</tr>
					</table>

				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>