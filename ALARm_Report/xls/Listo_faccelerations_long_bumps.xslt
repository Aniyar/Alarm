<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>Ведомость ускорений, вызванных длинными неровностями  </title>
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
					font-size: 12px;
					}
					thead {
					background: #f5e8d0;
					/* Цвет фона заголовка */
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


				</style>
				<style type="text/css" media="print">
					.dontprint {
					display: none;
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

					<div align="right" id="pageFooter"  style="page-break-before:always; margin:50px 0px 0px">


						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>


						<H4 align = "center">Ведомость ускорений, вызванных длинными неровностями  </H4>


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
									Проверка:<xsl:value-of select="@check" /> &#160;&#160;&#160;&#160;&#160;&#160; <xsl:value-of select="@periodDate" />
								</td>

							</tr>
						</table>
					</div>
					<div align="center">

						<table class="main">
							<thead>
								<tr>
									<td colspan="6"  class="main">
										Направление:   <xsl:value-of select="@direction"/><b>
											(<xsl:value-of select="@directioncode"/>)
										</b>
									</td>
									<td align="center" colspan="3" class="main">
										Путь:<b>
											<xsl:value-of select="@track"/>
										</b>
									</td>

								</tr>

								<tr>
									<td  align="center" rowspan="2" colspan="1" class="main">Км </td>
									<td  align="center"  rowspan="2"  class="main">м </td>
									<td   align="center" rowspan="2"  class="main">Vуст,км/ч </td>
									<td   align="center" rowspan="2"  class="main">
										Макс.неровность,<br/>мм
									</td>
									<td   align="center" rowspan="1" colspan="3"  class="main">Дополнительное ускорение </td>
									<td   align="center" rowspan="2"  class="main">Баллы </td>
									<td   align="center" rowspan="2"  class="main">Примечаие </td>
								</tr>

								<tr>
									<td align="center" colspan="1" class="main">Длина участка,м</td>
									<td align="center" colspan="1" class="main">
										В плане,м/c<sub>2</sub>
									</td>
									<td align="center" colspan="1" class="main">
										В профиле,м/c<sub>2</sub>
									</td>
								</tr>

							</thead>
							<tbody>
								<xsl:for-each select="tracks">
									<xsl:for-each select="note">

										<tr class="tr">
											<td align="center">
												<xsl:value-of select="@Km"/>
											</td>
											<td align="center">
												<xsl:value-of select="@m"/>
											</td>
											<td align="center">
												<xsl:value-of select="@Vust"/>
											</td>
											<td align="center">
												<xsl:value-of select="@Maxbump"/>
											</td>

											<td align="center" >
												<xsl:value-of select="@Sectionlength"/>
											</td>
											<td align="center" >
												<xsl:value-of select="@Plan"/>
											</td>


											<td align="center">
												<xsl:value-of select="@Profil"/>
											</td>
											<td align="center">
												<xsl:value-of select="@Ball"/>
											</td>
											<td align="center">
												<xsl:value-of select="@Wear"/>
											</td>



										</tr>

									</xsl:for-each>
								</xsl:for-each>
							</tbody>

						</table>
					</div>

					<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
						<tr>
							<td align="left">
								Начальник &#160;&#160;<xsl:value-of select="@ps" />
							</td>



							<td align="right">
								<xsl:value-of select="@chief" />
							</td>
						</tr>
					</table>

				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>