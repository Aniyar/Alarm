<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>Ведомость отсутствующих болтовых соединений</title>
				<style>
					.pages {
					page-break-before: always;
					}

					table {
					border-collapse: collapse;
					font-size: 12px;

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
					background-color: rgb(0, 0, 0);
					background-color: rgba(0, 0, 0, 0.4);
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

					#mainImage {
					width: 100%;
					}

					.container {
					width: 100%;
					text-align: center;
					}
				</style>
				<style type="text/css" media="print">
					.dontprint {
					display: none;
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
				<xsl:for-each select="report/pages">
					<div class="pages">
						<table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;  ">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Ведомость отсутствующих болтовых соединений</p>
									</b>
								</td>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@road" /> ЖД
									</b>
								</td>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@period" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;

									<b>
										<xsl:value-of select="@type" />
									</b>

								</td>
								<td align="left">
								</td>


							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@car" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@data" />
									</b>

								</td>
							</tr>
						</table>


						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thread>
								<tr>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">№ п/п</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">
										ПЧУ, ПД, ПДБ
									</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">
										Перегон,
										станция
									</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">км</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">пк</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">м</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">Vуст</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">
										Тип накладки
									</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">Нить</td>
									<td class="border-head" style="padding:2.5px;" align="center" colspan="2">
										Отсутствие
										болтов
									</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">Vдоп</td>
									<td class="border-head" style="padding:2.5px;" align="center" rowspan="2">
										Примечание
									</td>
								</tr>
								<tr>
									<td class="border-head" style="padding:2.5px;" align="center">до стыка</td>
									<td class="border-head" style="padding:2.5px;" align="center">после стыка</td>
								</tr>
							</thread>
							<tbody id="this">
								<xsl:for-each select="tracks">
									<tr>
										<td align="right" style="padding:2.5px;" colspan="13">
											<b>
												<xsl:value-of select="@trackinfo" />
											</b>

											<xsl:for-each select="elements">
												<xsl:variable name="CarPosition" select="@CarPosition" />
												<xsl:variable name="fileId" select="@fileId" />
												<xsl:variable name="Ms" select="@Ms" />
												<xsl:variable name="fNum" select="@fNum" />
												<xsl:variable name="repType" select="@repType" />

												<tr id="this" class="tr">
													<td id="this" align="center">
														<xsl:value-of select="@n" />
													</td>
													<td id="this" align="center">
														<xsl:value-of select="@pchu" />
													</td>
													<td id="this" align="center">
														<xsl:value-of select="@station" />
													</td>
													<td id="this" align="center">
														<xsl:value-of select="@km" />
													</td>
													<td align="center">
														<xsl:value-of select="@piket" />
													</td>
													<td align="center">
														<xsl:value-of select="@meter" />
													</td>
													<td align="center">
														<xsl:value-of select="@speed" />
													</td>
													<td align="center">
														<xsl:value-of select="@overlay" />
													</td>
													<td align="center">
														<xsl:value-of select="@Threat_id" />
													</td>
													<td align="center">
														<xsl:value-of select="@before" />
													</td>
													<td align="center">
														<xsl:value-of select="@after" />
													</td>
													<td align="center">
														<xsl:value-of select="@speed2" />
													</td>
													<td align="center">
														<xsl:value-of select="@notice" />
													</td>
													<td id="this" class="main dontprint button" align="center">
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
								</td>


								<td>
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