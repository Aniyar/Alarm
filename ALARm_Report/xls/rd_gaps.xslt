<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>Ведомость состояния стыковых зазоров</title>
				<style>
					table {
					border-collapse: collapse;
					}

					thead {
					background: #f5e8d0;
					/* Цвет фона заголовка */
					}

					td {
					padding-left: 5px;
					}

					.tr:nth-child(odd) {
					background-color: #EAF2D3;
					}

					.tr:hover {
					background-color: #E0E0FF;
					}

					.modal {
					display: none;
					position: fixed;
					z-index: 1;
					padding-top: 100px;
					left: 0;
					top: 0;
					width: 100%;
					height: 100%;
					overflow: auto;
					background-color: rgb(0,0,0);
					background-color: rgba(0,0,0,0.4);
					}


					.modal-content {
					background-color: #fefefe;
					margin: auto;
					padding: 20px;
					border: 1px solid #888;
					width: 95%;
					}


					.close {
					width: 95%;
					margin: auto;
					padding-right: 50px;
					padding-top: 4px;
					text-align: right;
					color: #aaaaaa;
					float: right;
					font-size: 12px;
					font-weight: bold;

					}

					.close:hover,
					.close:focus {
					color: #000;
					text-decoration: none;
					cursor: pointer;
					}
					#mainImage{
					width:100%;
					}
					.container{
					width:100%;
					text-align:center;
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
				<style>
					button {
					display: inline-block;
					padding: 0px 20px;
					font-size: 12px;
					cursor: pointer;
					text-align: center;
					text-decoration: none;
					outline: none;
					color: #fff;
					background-color: #4CAF50;
					border: none;
					border-radius: 10px;
					}

					button:hover {
					background-color: #3e8e90
					}

					button:active {
					background-color: #3e8e10;
					box-shadow: 0 1px #666;
					transform: translateY(1px);
					}
				</style>
				<style>
					.table .hover td:after {
					content: '';
					position: absolute;
					top: 0px;
					right: 0px;
					bottom: 0px;
					left: 0px;
					width: 105%;
					border-top: 3px solid #ffe5c5;
					border-bottom: 3px solid #ffe5c5;
					}
					.table .hover td:first-child:after {
					border-left: 3px solid #ffe5c5;
					}
					.table .hover td:last-child:after {
					border-right: 3px solid #ffe5c5;
					width: auto;
					}

					/* Click */
					.table .active td:after {
					content: '';
					position: absolute;
					top: 0px;
					right: 0px;
					bottom: 0px;
					left: 0px;
					width: 105%;
					border-top: 3px solid orange;
					border-bottom: 3px solid orange;
					}
					.table .active td:first-child:after {
					border-left: 3px solid orange;
					}
					.table .active td:last-child:after {
					border-right: 3px solid orange;
					width: auto;
					}
				</style>

				<script src="axios.min.js">//</script>

				<script src="/js/konva.min.js">//</script>
				<script src="/js/touch-emulator.js">//</script>
				<script src="/js/hammer-konva.js">//</script>
				<script src="getimage.js">//</script>
			</head>

			<body>
				<div id="myModal" class="modal">
					<span class="close">Закрыть</span>
					<div class="modal-content">
						<div id="container" class="container">
							<img id="mainImage" />
						</div>
					</div>
				</div>
				<xsl:for-each select="report/trip">
					<div style="page-break-before:always;">
						<!-- <b>
                        <p align="center" style="color:black; font-size:15px">Ведомость состояния стыковых зазоров</p>
                    </b>
                    <table style="width:90%" align="center">
                  
                          <tr>
                                <td align="left">Дорога: <b><xsl:value-of select="@road" /></b> </td> <td> Проезд:  <b><xsl:value-of select="@date_statement" /></b></td>
                            </tr>
                            <tr>
                                <td align="left"> ПС:<b><xsl:value-of select="@ps" /></b> </td>  <td align="left">Период: <b><xsl:value-of select="@periodDate" /></b></td> <td> Проверка : <b><xsl:value-of select="@check" /></b></td>
                            </tr>
                    </table> -->
						<table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Ведомость состояния стыковых зазоров</p>
									</b>
								</td>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@road" />
										ЖД
									</b>
								</td>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@periodDate" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;

									<b>
										<xsl:value-of select="@check" />
									</b>

								</td>
								<td  align="left">
								</td>


							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@ps" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@date_statement" />
									</b>

								</td>
							</tr>
						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<tr>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">№ п/п</td>
									<td  align="center" class="border-head" style="padding:2.5px;" width="15%" rowspan="1">ПЧУ, ПД, ПДБ</td>
									<!-- <td class="border-head" style="padding:2.5px;" width="15%" rowspan="1">Перегон, станция</td> -->
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">км</td>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">пк</td>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">метр</td>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">Vпз</td>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">Зазор правой нити</td>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">Зазор левой нити</td>
									<td  align="center" class="border-head" style="padding:2.5px;" width="5%" rowspan="1">T°</td>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">Забег</td>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">Vдоп</td>
									<td  align="center" class="border-head" style="padding:2.5px;" rowspan="1">Отступление</td>
									<td  align="center" class="border-head" style="padding:2.5px;" width="10%" rowspan="1">Примечание</td>
								</tr>
							</thead>
							<tbody id="this">
								<xsl:for-each select="direction">
									<tr>
										<td align="right" style="padding:2.5px;" colspan="13">
											<b>
												<xsl:value-of select="@name" />
											</b>
											<!-- начало записи-->
											<xsl:for-each select="Note">
												<xsl:variable name="CarPosition" select="@CarPosition" />
												<xsl:variable name="repType" select="@repType" />

												<xsl:variable name="fileId" select="@fileId" />
												<xsl:variable name="Ms" select="@Ms" />
												<xsl:variable name="fNum" select="@fNum" />

												<xsl:variable name="fileId2" select="@fileId2" />
												<xsl:variable name="Ms2" select="@Ms2" />
												<xsl:variable name="fNum2" select="@fNum2" />
												<tr id="this" class="tr">
													<td id="this" align="center">
														<xsl:value-of select="@n" />
													</td>
													<td id="this" align="center">
														<xsl:value-of select="@PPP" />
													</td>
													<!-- <td align="center"><xsl:value-of select="@PeregonStancia" /></td> -->
													<td id="this" align="center">
														<xsl:value-of select="@km" />
													</td>
													<td align="center">
														<xsl:value-of select="@piket" />
													</td>
													<td align="center">
														<xsl:value-of select="@m" />
													</td>
													<td align="center">
														<xsl:value-of select="@Vpz" />
													</td>
													<td align="center">
														<xsl:choose>
															<xsl:when test="@Otst = 'З'">
																<b>
																	<xsl:value-of select="@ZazorR" />
																</b>
															</xsl:when>
															<xsl:otherwise>
																<xsl:value-of select="@ZazorR" />
															</xsl:otherwise>
														</xsl:choose>
													</td>
													<td align="center">
														<xsl:choose>
															<xsl:when test="@Otst = 'З'">
																<b>
																	<xsl:value-of select="@ZazorL" />
																</b>
															</xsl:when>
															<xsl:otherwise>
																<xsl:value-of select="@ZazorL" />
															</xsl:otherwise>
														</xsl:choose>
													</td>
													<td align="center">
														<xsl:value-of select="@T" />
													</td>
													<td align="center">
														<xsl:value-of select="@Zabeg" />
													</td>
													<td align="center">
														<xsl:choose>
															<xsl:when test="@Otst = 'АРЗ'">
																<div style="display:none">
																	<xsl:value-of select="@Vdop" />
																</div>
															</xsl:when>
															<xsl:otherwise>
																<xsl:value-of select="@Vdop" />
															</xsl:otherwise>
														</xsl:choose>
													</td>
													<td align="center">
														<xsl:choose>
															<xsl:when test="@Otst = 'З'">
																<b>
																	<xsl:value-of select="@Otst" />
																</b>
															</xsl:when>
															<xsl:otherwise>
																<xsl:value-of select="@Otst" />
															</xsl:otherwise>
														</xsl:choose>
													</td>
													<td align="center">
														<xsl:value-of select="@Primech" />
													</td>
													<td id="this" class="main dontprint button" align="center">
														<xsl:value-of select="@notice" />
														<button class="dontprint button" onClick="getImage({$CarPosition},{$fileId},{$Ms},{$fNum},{$repType})">Смотреть фото</button>
													</td>
												</tr>
											</xsl:for-each>
										</td>
									</tr>
								</xsl:for-each>
							</tbody>
						</table>
						<table style="font-size: 12px; width: 95%;height: 5%; margin: auto;">
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
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									<xsl:value-of select="@chief" />
								</td>

							</tr>
						</table>
					</div>
				</xsl:for-each>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>