<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>
					Ведомость кривых с несоответствием фактических и
					паспортных характеристик кривых
				</title>
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

					<div align="right" id="pageFooter" style="page-break-before:always; margin:50px 0px 0px">


						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>


						<H4 align = "center">
							Ведомость кривых с несоответствием фактических и                            <br/>
							паспортных характеристик кривых
						</H4>


					</div>
					<div align="center">
						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<td align="left">
									ПЧ:                                    <xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:                                    <xsl:value-of select="@road" />
								</td>

							</tr>
							<tr>

								<td align="left">
									<xsl:value-of select="@ps" />
								</td>

								<td align="left">
									Проверка:                                    <xsl:value-of select="@check" />
									&#160;&#160;&#160;&#160;&#160;&#160; <xsl:value-of select="@periodDate" />
								</td>

							</tr>
						</table>
					</div>
					<div align="center">

						<table class="main">
							<thead>
								<tr>
									<td align="center" rowspan="2" colspan="1" class="main">
										№                                        <br/>
										п/п
									</td>
									<th align="center" rowspan="1" colspan="5" class="main">Паспортные данные </th>
									<th align="center" rowspan="1" colspan="5" class="main">Фактические данные </th>
								</tr>

								<tr>
									<td align="center" colspan="1" class="main">
										Начало,                                        <br/>
										км.м
									</td>
									<td align="center" colspan="1" class="main">
										Конец,                                        <br/>
										км.м
									</td>
									<td align="center" colspan="1" class="main">
										Радиус,                                        <br/>
										м
									</td>
									<td align="center" colspan="1" class="main">
										Возвышение,                                        <br/>
										мм
									</td>
									<td align="center" colspan="1" class="main">
										Износ,                                        <br/>
										мм
									</td>

									<td align="center" colspan="1" class="main">
										Начало,                                        <br/>
										км.м
									</td>
									<td align="center" colspan="1" class="main">
										Конец,                                        <br/>
										км.м
									</td>
									<td align="center" colspan="1" class="main">
										Радиус,                                        <br/>
										м
									</td>
									<td align="center" colspan="1" class="main">
										Возвышение,                                        <br/>
										мм
									</td>
									<td align="center" colspan="1" class="main">
										Износ,                                        <br/>
										мм
									</td>


								</tr>
								<tr>
									<td colspan="7" class="main">
										Направление:                                        <xsl:value-of select="@direction"/>
										<b>
											(                                            <xsl:value-of select="@directioncode"/>
											)
										</b>
									</td>
									<td align="center" colspan="4" class="main">
										Путь:                                        <b>
											<xsl:value-of select="@track"/>
										</b>
									</td>

								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="tracks">
									<xsl:for-each select="note">
										<tr >

											<td colspan="1" class="main" align="center">
												<xsl:value-of select="@n"/>
											</td>

											<td colspan="1" class="main" align="center">
												<!-- <xsl:value-of select="@startkm"/> -->

												<xsl:choose>
													<xsl:when test="@razKMFinal > '1'">

														<xsl:value-of select="@startkm" />

													</xsl:when>
													<xsl:otherwise>
														<b>
															<xsl:value-of select="@startkm" />
														</b>
													</xsl:otherwise>
												</xsl:choose>


											</td>

											<td colspan="1" class="main" align="center">
												<!-- <xsl:value-of select="@finalkm"/> -->
												<xsl:choose>
													<xsl:when test="@razKMFinal > '1'">

														<xsl:value-of select="@finalkm" />

													</xsl:when>
													<xsl:otherwise>
														<b>
															<xsl:value-of select="@finalkm" />
														</b>
													</xsl:otherwise>
												</xsl:choose>


											</td>
											<td colspan="1" class="main" align="center">
												<xsl:value-of select="@Radius"/>
											</td>

											<td colspan="1" class="main" align="center">
												<!-- <xsl:value-of select="@Elevation"/> -->

												<xsl:choose>
													<xsl:when test="@razElevetion >= '18'">
														<b>
															<xsl:value-of select="@Elevation" />
														</b>
													</xsl:when>
													<xsl:otherwise>

														<xsl:value-of select="@Elevation" />

													</xsl:otherwise>
												</xsl:choose>



											</td>
											<td colspan="1" class="main" align="center">
												<!-- <xsl:value-of select="@Wear"/> -->

												<xsl:choose>
													<xsl:when test="@razWear >= '7'">
														<b>
															<xsl:value-of select="@Wear" />
														</b>
													</xsl:when>
													<xsl:otherwise>

														<xsl:value-of select="@Wear" />

													</xsl:otherwise>
												</xsl:choose>


											</td>

											<td colspan="1" class="main" align="center">
												<!-- <xsl:value-of select="@factstartkm"/> -->
												<xsl:choose>
													<xsl:when test="@razKMFinal > '1'">

														<xsl:value-of select="@factstartkm" />

													</xsl:when>
													<xsl:otherwise>
														<b>
															<xsl:value-of select="@factstartkm" />
														</b>
													</xsl:otherwise>
												</xsl:choose>

											</td>

											<td colspan="1" class="main" align="center">
												<!-- <xsl:value-of select="@factfinalkm"/> -->
												<xsl:choose>
													<xsl:when test="@razKMFinal > '1' ">

														<xsl:value-of select="@factfinalkm" />

													</xsl:when>
													<xsl:otherwise>
														<b>
															<xsl:value-of select="@factfinalkm" />
														</b>
													</xsl:otherwise>
												</xsl:choose>


											</td>
											<td colspan="1" class="main" align="center">
												<xsl:value-of select="@factRadius"/>
											</td>

											<td colspan="1" class="main" align="center">
												<!-- <xsl:value-of select="@factElevation"/> -->

												<xsl:choose>
													<xsl:when test="@razElevetion >= '18' ">
														<b>
															<xsl:value-of select="@factElevation" />
														</b>
													</xsl:when>
													<xsl:otherwise>

														<xsl:value-of select="@factElevation" />

													</xsl:otherwise>
												</xsl:choose>

											</td>
											<td colspan="1" class="main" align="center">
												<!-- <xsl:value-of select="@factWear"/> -->

												<xsl:choose>
													<xsl:when test="@razWear >= '7' ">
														<b>
															<xsl:value-of select="@factWear" />
														</b>
													</xsl:when>
													<xsl:otherwise>

														<xsl:value-of select="@factWear" />

													</xsl:otherwise>
												</xsl:choose>
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