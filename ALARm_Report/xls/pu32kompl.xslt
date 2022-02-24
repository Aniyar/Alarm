<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>ПУ-32_комп Сводная ведомость оценки состояния главных путей </title>
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

					.tr {
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

					table.main,
					td.main,
					th.main {
					border-collapse: collapse;
					border: 1px solid black;
					}

					table.main {
					width: 100%;
					margin: auto;
					}

					b {
					font-size: 18px;
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

						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>
						<H4 align = "center"> Сводная ведомость оценки состояния главных путей (ПУ-32_комп) </H4>


						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;">

							<tr>
								<td align="left">
									ПЧ:<xsl:value-of select="@pch" />
								</td>
								<td align="left">
									Дорога:<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@car" />
								</td>
								<td align="left">
									Проверка:<xsl:value-of select="@check"/>  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="@trip_date" />
								</td>

							</tr>
						</table>

						<table align="center" class="main">
							<thead>
								<!-- шапка -->
								<!-- <tr>
                                    <th class="main" colspan="{@Count}" style="text-align:centre">
                                        <xsl:value-of select="@Info" />
                                    </th>
                                </tr> -->

								<tr style="font-size: 15px;" align="center">
									<xsl:for-each select="Pch">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@PchCode" />
										</td>
									</xsl:for-each>
									<th class="main" rowspan="2" colspan="2">
										Административное деление<br/> по пути
									</th>
									<!-- ПЧ -->

								</tr>
								<tr>
									<!-- ПД -->
									<xsl:for-each select="Pch/Pd">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@PdCode" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">КМ</th>
									<!-- КМ -->
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@KmCode" />
										</td>
									</xsl:for-each>
								</tr>

								<tr>
									<th class="main" rowspan="1" colspan="2">Перегон, станция</th>
									<!-- Перегон -->
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@Peregon" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Уст.скорости</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@Vust" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Огранич.скорости</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@Vdop" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Сумма баллов(осн/доп)</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											Сумма баллов   <xsl:value-of select="@Vdop" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Кол-во 3ст</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											Кол-во 3ст  <xsl:value-of select="@digression3and4" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">
										Кол-во <br/>4ст/кривые.другие/доп<br/>
									</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											Кол-во 4ст/кривые.другие/доп   <xsl:value-of select="@digression3and4" />
										</td>
									</xsl:for-each>
								</tr>


								<!-- <th class="main" rowspan="1" colspan="2">Всего отступлений</th>
                                    <xsl:for-each select="Pch/Pd/Km">
                                        <td class="main" colspan="{@Count}">
                                            <xsl:value-of select="@digression" />
                                        </td>
                                    </xsl:for-each>
                                </tr> -->

								<tr>
									<th class="main" rowspan="1" colspan="2">Оценка, СССП</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@SSSP" />
										</td>
									</xsl:for-each>
								</tr>


								<tr>
									<th class="main" rowspan="2" colspan="1">
										Кри<br/>вые
									</th>

									<th class="main" rowspan="1" colspan="1">Огр.скор</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@KrivVogr" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="1">Раст. 2/3</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@Rast2and3" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Сверхнорм.стыки</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@STYKSVERHNORMA" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Подуклонка</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@PODUKLON" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Бок.износ</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@BOKIZNOS" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Имп.неров.(л/пр)</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@IMPNEROV" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Кор.неров.(инд.)</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@KORNEROV" />
										</td>
									</xsl:for-each>
								</tr>

								<tr>
									<th class="main" rowspan="1" colspan="2">Состояние шпал</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@SOSTSHPAL" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Состояние балласта</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@SOSTBALAST" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Год посл. кап_рем</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@GODPOSLEKAPREM" />
										</td>
									</xsl:for-each>
								</tr>
								<tr>
									<th class="main" rowspan="1" colspan="2">Тоннаж (млн.т)</th>
									<xsl:for-each select="Pch/Pd/Km">
										<td class="main" colspan="{@Count}">
											<xsl:value-of select="@TONAG" />
										</td>
									</xsl:for-each>
								</tr>




								<tbody id="this">
									<xsl:for-each select="data">



										<tr class="tr">
											<td class="main">
												<xsl:value-of select="@km" />

											</td>
											<td class="main">
												<xsl:value-of select="@kmCheck" />

											</td>
											<td class="main">
												<xsl:value-of select="@check" />

											</td>
											<td class="main">
												<xsl:value-of select="@kmNotCheck" />

											</td>
											<td class="main">
												<xsl:value-of select="@notCheck" />

											</td>
										</tr>
									</xsl:for-each>
								</tbody>
							</thead>
						</table>

						<table style="font-size: 12px; width: 95%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@car" />
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