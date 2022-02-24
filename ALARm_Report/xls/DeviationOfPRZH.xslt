<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>Ф.ПРЖ - Отступление ПРЖ с ограничением скорости </title>
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
					margin:auto;
					padding-right:50px;
					padding-top:4px;
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
					<div style = "page-break-before:always;">
						<b>
							<p align="left" style="color:black; font-size:12px">
								<xsl:value-of select="@version" />
							</p>
						</b>
						<b>
							<p align="center" style="color:black; font-size:15px">Отступления с ограничением скорости для грузовых поездов, имеющих в составе порожние вагоны</p>
						</b>

						<table style="width:90%" align="center">
							<tr>
								<th align="left">
									ПЧ: <xsl:value-of select="@distance" />
								</th>
								<th align="center">
									Дорога: <xsl:value-of select="@road" />
								</th>

							</tr>
							<tr>

								<th align="left">
									ДК: <xsl:value-of select="@ps" />
								</th>

								<th align="center">
									Проверка:<xsl:value-of select="@direction" />
								</th>

								<th align="center">
									<xsl:value-of select="@periodDate" />
								</th>

							</tr>
						</table>
						<table width="90%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<tr >
									<th class="border-head" style="padding:2.5px;" rowspan="1">ПЧУ</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">ПД</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">ПДБ</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Дата обнаружения</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">км</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Метр</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Отступление</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Откл, мм</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Длина, м</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Кол</th>
								</tr>
							</thead>
							<xsl:for-each select="direction">
								<tr class="tr">
									<td align="center" style="padding:2.5px;" colspan="10">
										<b>
											Направление:                                            <xsl:value-of select="@name" />
										</b>
										<tbody  >
											<xsl:for-each select="track">
												<tr class="tr" >
													<td align="left" style="padding:2.5px;" colspan="10">
														<b>
															&#8195;&#8195;&#8195;&#8195; Путь:                                                    <xsl:value-of select="@name" />
														</b>
														<!-- начало записи-->
														<xsl:for-each select="PCHU">
															<tr class="tr">
																<td align="center" valign="top" style="padding:2.5px;">
																	<xsl:attribute name="rowspan">
																		<xsl:value-of select="@recordCount" />
																	</xsl:attribute>
																	<xsl:value-of select="@number" />
																</td>
																<xsl:for-each select="PD">
																	<td align="center" valign="top" style="padding:2.5px;">
																		<xsl:attribute name="rowspan">
																			<xsl:value-of select="@recordCount" />
																		</xsl:attribute>
																		<xsl:value-of select="@number" />
																	</td>
																	<xsl:for-each select="PDB">
																		<td align="center" valign="top" style="padding:2.5px;">
																			<xsl:attribute name="rowspan">
																				<xsl:value-of select="@recordCount" />
																			</xsl:attribute>
																			<xsl:value-of select="@number" />
																		</td>
																		<xsl:for-each select="NOTE">
																			<td>
																				<xsl:value-of select="@founddate" />
																			</td>
																			<td>
																				<xsl:value-of select="@km" />
																			</td>
																			<td>
																				<xsl:value-of select="@meter" />
																			</td>
																			<td>
																				<xsl:value-of select="@digression" />
																			</td>
																			<td>
																				<xsl:value-of select="@value" />
																			</td>
																			<td>
																				<xsl:value-of select="@length" />
																			</td>
																			<td>
																				<xsl:value-of select="@count" />
																			</td>
																			<tr />
																		</xsl:for-each>
																	</xsl:for-each>
																</xsl:for-each>
															</tr>
														</xsl:for-each>
													</td>
												</tr>

												<tr>

													<th align="left" style="padding:2.5px;" colspan="10">
														Всего по пути                                                    <xsl:value-of select="@name" />
														-                                                    <xsl:value-of select="@recordCount" />
														шт.
													</th>
												</tr>
											</xsl:for-each>
										</tbody >
									</td>
								</tr>
							</xsl:for-each>
						</table>
						<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
							<tr>
								<td style="width:10%">
									Итого по ПЧ:                                    <xsl:value-of select="@totalCount" />
								</td>
							</tr>
							<tr>
								<td valign="top">                        В том числе:                        </td>
								<td style="width:10%" align="left">
									Суж -                                    <xsl:value-of select="@constrictionCount" />
									<br />
									Уш -                                    <xsl:value-of select="@broadeningCount" />
									<br />
									У -                                    <xsl:value-of select="@levelCount" />
									<br />
									П -                                    <xsl:value-of select="@skewnessCount" />
									<br />
									Р -                                    <xsl:value-of select="@straighteningCount" />
									<br />
									Пр -                                    <xsl:value-of select="@drawdownCount" />
									<br />
								</td>
							</tr>
						</table>
						<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
							<tr>
								<td>
									Начальник путеизмерителя:                                    <xsl:value-of select="@ps" />
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