<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<xsl:for-each select="report">
			<html>

				<head>
					<title>
						ПУ-32 (
						<xsl:value-of select="@soft" />
						)
					</title>

					<style>
						table.main,
						td.main {
						font-size: 12px;
						font-family: 'Times New Roman';
						border: 1.5px solid black;
						margin: auto;
						margin-bottom: 8px;
						border-collapse: collapse;
						padding: 5px;

						}

						tr {
						padding: 5px;
						}

						td {
						padding: 5px;
						text-align: center;
						vertical-align: middle;

						}

						td.vertical {}

						.main {
						text-align: left;
						vertical-align: middle;
						}

						.time-col {
						position: relative;
						writing-mode: vertical-rl;
						transform: rotate(180deg);
						}

						.col {
						position: relative;
						overflow: visible;
						}

						span {
						white-space: pre-wrap;
						display: inline-table;
						}

						.rotate {
						text-align: center;
						white-space: nowrap;
						vertical-align: middle;
						width: 1.5em;
						height: 8em;
						padding: 5px;
						}

						.rotate div {
						-moz-transform: rotate(-90.0deg);
						/* FF3.5+ */
						-o-transform: rotate(-90.0deg);
						/* Opera 10.5 */
						-webkit-transform: rotate(-90.0deg);
						/* Saf3.1+, Chrome */
						filter: progid:DXImageTransform.Microsoft.BasicImage(rotation=0.083);
						/* IE6,IE7 */
						-ms-filter: "progid:DXImageTransform.Microsoft.BasicImage(rotation=0.083)";
						/* IE8 */
						margin-left: -10em;
						margin-right: -10em;
						}
					</style>
				</head>

				<body>
					<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
						<xsl:value-of select="@version" />
					</p>
					<table style="font-size: 15px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse;height:8%;padding: 5px">
						<tr>
							<th style="color:black; font-size: 20px;" align="center">ВЕДОМОСТЬ </th>
						</tr>
						<tr>
							<th style="color:black; font-size: 16px;" align="center">оценки состояния нути (форма ПУ-32)</th>
						</tr>
						<tr>
							<td style="height:3%;margin:auto; padding: 20px;margin-bottom:8px;">
								ПЧ
								<xsl:value-of select="@pch" />
								Дорога:
								<xsl:value-of select="@road" />
							</td>
						</tr>
						<tr>
							<td>
								По данным
								<xsl:value-of select="@triptype" />
								проверки за
								<xsl:value-of select="@month" />
								путеизмерительным вагоном №
								<xsl:value-of select="@car" />
								<br />
							</td>


						</tr>
						<tr>
							<td style="height:3%;margin:auto; padding: 20px;margin-bottom:8px;">
								Дата проверки:
								<xsl:value-of select="@tripdate" />
							</td>
						</tr>


					</table>
					<xsl:for-each select="bykilometer">
						<br />
						<div>

							<table style="font-size: 15px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse;padding: 5px">
								<tr>
									<td colspan="16">Количество километров с оценкой и качественная оценка участка </td>
								</tr>
							</table>
							<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
								<tr>
									<td rowspan="2">
										№ пути
									</td>
									<td rowspan="2">
										Всего
										<br />
										км
									</td>
									<td colspan="4">Количество км с оценкой</td>
									<td rowspan="2">
										Км
										<br />
										с огр.
										<br />
										скорости
									</td>
									<td colspan="3">Отступления (шт)</td>
									<td rowspan="2">
										Колич.
										<br />
										км с
										<br />
										путевыми
										<br />
										работами
									</td>
									<td colspan="2">
										Средний
										<br />
										балл по участку
									</td>
									<td rowspan="2">
										Вели-
										<br />
										чина
										<br />
										N
										<sub>уч</sub>
										<br />
										<sup>4)</sup>
									</td>
									<td rowspan="2">
										Качест-
										<br />
										венная
										<br />
										оценка
										<sup>4)</sup>
									</td>
								</tr>
								<tr>
									<td>отл</td>
									<td>хор</td>
									<td>уд</td>
									<td>неуд</td>
									<td>
										IV
										<br />
										степ.
									</td>
									<td>
										Сочет.
										<br />
										Кривые
										<sup>1)</sup>
										<br />
										другие
										<sup>2)</sup>
									</td>
									<td>
										Доп <sup>3)</sup>
									</td>

									<td>
										только
										<br />
										по осн.
										<br />
										парам
									</td>
									<td>
										по всем
										<br />
										(+доп)
									</td>
								</tr>
								<xsl:for-each select="section">
									<tr>
										<td>
											<xsl:value-of select="@track" />
										</td>
										<td>
											<xsl:value-of select="@len" />
										</td>
										<td>
											<xsl:value-of select="@excellent" />
										</td>
										<td>
											<xsl:value-of select="@good" />
										</td>
										<td>
											<xsl:value-of select="@satisfactory" />
										</td>
										<td>
											<xsl:value-of select="@bad" />
										</td>
										<td>
											<xsl:value-of select="@limit" />
										</td>
										<td>
											<xsl:value-of select="@d4" />
										</td>
										<td>
											<xsl:value-of select="@other" />
										</td>
										<td>
											<xsl:value-of select="@add" />
										</td>
										<td>
											<xsl:value-of select="@repair" />
										</td>
										<td>
											<xsl:value-of select="@mainavg" />
										</td>
										<td>
											<xsl:value-of select="@addavg" />
										</td>
										<td>
											<xsl:value-of select="@ns" />
										</td>
										<td>
											<xsl:value-of select="@rating" />
										</td>
									</tr>
								</xsl:for-each>
								<tr>
									<td>
										Итого
									</td>
									<td>
										<xsl:value-of select="@len" />
									</td>
									<td>
										<xsl:value-of select="@excellent" />
									</td>
									<td>
										<xsl:value-of select="@good" />
									</td>
									<td>
										<xsl:value-of select="@satisfactory" />
									</td>
									<td>
										<xsl:value-of select="@bad" />
									</td>
									<td>
										<xsl:value-of select="@limit" />
									</td>
									<td>
										<xsl:value-of select="@d4" />
									</td>
									<td>
										<xsl:value-of select="@other" />
									</td>
									<td>
										<xsl:value-of select="@add" />
									</td>
									<td>
										<xsl:value-of select="@repair" />
									</td>
									<td>
										<xsl:value-of select="@mainavg" />
									</td>
									<td>
										<xsl:value-of select="@addavg" />
									</td>
									<td>
										<xsl:value-of select="@ns" />
									</td>
									<td>
										<xsl:value-of select="@ratings" />
									</td>
								</tr>
							</table>
							<br />
						</div>

						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
							<!-- <table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse"> -->
							<tr>
								<th colspan="12">Количество отступлений по видам </th>
							</tr>
							<tr>
								<td rowspan="2">Степень</td>
								<td colspan="10">Отступления</td>
								<td>Итого</td>
							</tr>
							<tr>
								<td>Суж</td>
								<td>Уш</td>
								<td>У</td>
								<td>П</td>
								<td>Пр</td>
								<td>Р</td>
								<td>Сочет</td>
								<td>Другие</td>
								<td>Кривые</td>
								<td>Доп.</td>
								<td></td>
							</tr>
							<xsl:for-each select="countbytype">
								<tr>
									<td>
										<xsl:value-of select="@degree" />
									</td>
									<td>
										<xsl:value-of select="@const" />
									</td>
									<td>
										<xsl:value-of select="@broad" />
									</td>
									<td>
										<xsl:value-of select="@level" />
									</td>
									<td>
										<xsl:value-of select="@sag" />
									</td>
									<td>
										<xsl:value-of select="@down" />
									</td>
									<td>
										<xsl:value-of select="@stright" />
									</td>
									<td>
										<xsl:value-of select="@combination" />
									</td>
									<td>
										<xsl:value-of select="@other" />
									</td>
									<td>
										<xsl:value-of select="@curves" />
									</td>
									<td>
										<xsl:value-of select="@additional" />
									</td>
									<td>
										<xsl:value-of select="@sum" />
									</td>
								</tr>
							</xsl:for-each>
						</table>
						<table style="font-size: 14px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<td style="text-align:left">Данные обработали и оформили ведомость ПУ-32: ИНЖЕНЕР:</td>
								<td style="text-align:right">
									<xsl:value-of select="../@engineer" />
								</td>
							</tr>
							<tr>
								<td colspan="2" style="text-align:left">
									Путеизмерительный вагон сопровождали:
									<br />
									<br />
									<br />
								</td>

							</tr>

							<tr>
								<td style="text-align:left">
									Начальник путеизмерительного вагона №
									<xsl:value-of select="../@car" />
									:
								</td>
								<td style="text-align:right">
									<xsl:value-of select="../@chief" />
								</td>
							</tr>
						</table>

						<br />

						<div style="page-break-before:always;">
							<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
								<xsl:value-of select="@version" />
							</p>
							<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
								<!-- <table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse"> -->
								<thead>
									<tr>
										<td colspan="14">Покилометровое количество отсуплений</td>
									</tr>
									<tr>
										<!-- <td class="border-head" style="padding:2.5px;" rowspan="3">Дистанция пути</td> -->
										<td rowspan="2" class="rotate">
											<div>
												№ км/
												<br />
												подразделения
											</div>
										</td>
										<td rowspan="2" class="rotate">
											<div>Проверено км</div>
										</td>
										<td rowspan="1" colspan="7">
											Количество отсуплений
											<br />
											II степени(числитель)/III степени(знаменатель)
										</td>
										<td rowspan="2" colspan="1">
											IV ст./
											<br />
											сочет.
											<br />
											Кривые,
											<br />
											другие./
											<br />
											доп
										</td>
										<td rowspan="2" colspan="1">
											Сумма баллов
											<br />
											Осн/Доп
											<br />
											Средний
											<br />
											балл по
											<br />
											участку
											<!-- и ПЧ <br/> ОСН/ Сумм -->
										</td>
										<td rowspan="2" colspan="1">
											N уч сумм/
											<br />
											Качест-
											<br />
											венная
											<br />
											оценка
											<!-- для участка и ПЧ -->
										</td>
										<td rowspan="2" colspan="2">Примечания </td>
									</tr>
									<tr>
										<td rowspan="1">
											<div>Суж</div>
										</td>
										<td rowspan="1">
											<div>Уш</div>
										</td>
										<td rowspan="1">
											<div>У</div>
										</td>
										<td rowspan="1">
											<div>П</div>
										</td>
										<td rowspan="1">
											<div>Пр</div>
										</td>
										<td rowspan="1">
											<div>Р</div>
										</td>
										<td rowspan="1">
											<div>Итого</div>
										</td>
									</tr>
								</thead>
								<xsl:for-each select="section">
									<tr>
										<td class="main" text-align="left" colspan="13">
											<!-- <pre style="font:inherit;heigth:20px;text-align:left" > -->
											Направление:
											<b>
												<xsl:value-of select="@name" />
												&#160;&#160;&#160;&#160;&#160;&#160;&#160; Код:
												<xsl:value-of select="@code" />
												&#160;&#160;&#160;&#160;&#160;&#160;&#160; Путь:
												<xsl:value-of select="@track" />
											</b>
											<!-- </pre> -->
										</td>
									</tr>
									<xsl:for-each select="pchu">

										<xsl:for-each select="pd">
											<tr>
												<td style="text-align: left;  border-right-style: hidden" colspan="2">
													<b>
														ПЧ
														<xsl:value-of select="../@pch" />
													</b>
												</td>
												<td  style="text-align: center;  border-left-style: hidden;  border-right-style: hidden" colspan="2">
													<b>
														ПЧУ
														<xsl:value-of select="../@code" />
													</b>
												</td>
												<td style="text-align: center; border-left-style: hidden;  border-right-style: hidden" colspan="2">
													<b>
														ПД
														<xsl:value-of select="@code" />
													</b>
												</td>
												<td style="text-align: left;  border-left-style: hidden" colspan="8">
													<b>
														Мастер -
														<xsl:value-of select="@chief" />
													</b>
												</td>


											</tr>
											<xsl:for-each select="pdb">
												<xsl:for-each select="km">
													<tr>
														<td>
															<xsl:value-of select="@n" />
															<sup>
																<xsl:value-of select="@com" />
															</sup>
														</td>
														<td>
															<xsl:value-of select="@len" />
														</td>
														<td>
															<xsl:value-of select="@c1" />
														</td>
														<td>
															<xsl:value-of select="@c2" />
														</td>
														<td>
															<xsl:value-of select="@c3" />
														</td>
														<td>
															<xsl:value-of select="@c4" />
														</td>
														<td>
															<xsl:value-of select="@c5" />
														</td>
														<td>
															<xsl:value-of select="@c6" />
														</td>
														<td>
															<xsl:value-of select="@c7" />
														</td>
														<td>
															<xsl:value-of select="@c8" />
														</td>
														<td>
															<xsl:value-of select="@c9" />
														</td>
														<td>
															<xsl:value-of select="@c10" />
														</td>
														<td>
															<xsl:value-of select="@c11" />
															<xsl:value-of select="@c12" />
														</td>
													</tr>
												</xsl:for-each>
												<tr>
													<td colspan="2" rowspan="2" style="text-align:left">
														ПДБ
														<xsl:value-of select="@code" />
														<br />
														Итого -
														<xsl:value-of select="@len" />

													</td>
													<td>
														<xsl:value-of select="@c1" />
													</td>
													<td>
														<xsl:value-of select="@c2" />
													</td>
													<td>
														<xsl:value-of select="@c3" />
													</td>
													<td>
														<xsl:value-of select="@c4" />
													</td>
													<td>
														<xsl:value-of select="@c5" />
													</td>
													<td>
														<xsl:value-of select="@c6" />
													</td>
													<td>
														<xsl:value-of select="@c7" />
													</td>
													<td>
														<xsl:value-of select="@c8" />
													</td>

													<!-- <td colspan="8">
													Бригадир - <xsl:value-of select="@chief"/>
												</td> -->
													<td>
														<xsl:value-of select="@point" />
													</td>
													<td>
														<xsl:value-of select="@rating" />
													</td>
													<td></td>
												</tr>
												<tr>
													<td style="text-align:left" colspan="11">
														<xsl:value-of select="@ratecount" />
													</td>
												</tr>
											</xsl:for-each>
											<tr>
												<td colspan="2" rowspan="2" align="left">
													ПД
													<xsl:value-of select="@code" />
													<br />
													Итого -
													<xsl:value-of select="@len" />
												</td>
												<td>
													<xsl:value-of select="@c1" />
												</td>
												<td>
													<xsl:value-of select="@c2" />
												</td>
												<td>
													<xsl:value-of select="@c3" />
												</td>
												<td>
													<xsl:value-of select="@c4" />
												</td>
												<td>
													<xsl:value-of select="@c5" />
												</td>
												<td>
													<xsl:value-of select="@c6" />
												</td>
												<td>
													<xsl:value-of select="@c7" />
												</td>
												<td>
													<xsl:value-of select="@c8" />
												</td>
												<td>
													<xsl:value-of select="@point" />
												</td>
												<td>
													<xsl:value-of select="@rating" />
												</td>
												<td>
													<xsl:value-of select="@c11" />
													<xsl:value-of select="@c12" />
												</td>
											</tr>
											<tr>
												<td style="text-align:left" colspan="11">
													<xsl:value-of select="@ratecount" />
												</td>
											</tr>
										</xsl:for-each>

									</xsl:for-each>
								</xsl:for-each>

								<tr>
									<td style="text-align:left" colspan="13">
										Всего по ПЧ
										<xsl:value-of select="@pch" />
										&#160;&#160;&#160;&#160;&#160;&#160;&#160; Код:
										<b>
											<xsl:value-of select="@code" />
										</b>
										&#160;&#160;&#160;&#160;&#160;&#160;&#160; Путь:
										<b>
											<xsl:value-of select="@track" />
										</b>

									</td>
								</tr>

								<tr>
									<td colspan="2" rowspan="2" align="left">
										<b>
											Итого -
											<br />
											<xsl:value-of select="@len" />
										</b>
									</td>
									<td>
										<xsl:value-of select="@c1" />
									</td>
									<td>
										<xsl:value-of select="@c2" />
									</td>
									<td>
										<xsl:value-of select="@c3" />
									</td>
									<td>
										<xsl:value-of select="@c4" />
									</td>
									<td>
										<xsl:value-of select="@c5" />
									</td>
									<td>
										<xsl:value-of select="@c6" />
									</td>
									<td>
										<xsl:value-of select="@c7" />
									</td>
									<td>
										<xsl:value-of select="@c8" />
									</td>
									<td>
										<xsl:value-of select="@point" />
									</td>
									<td>
										<xsl:value-of select="@rating" />
									</td>
									<td></td>
								</tr>
								<tr>
									<td style="text-align:left" colspan="13">
										<xsl:value-of select="@ratecount" />
									</td>
								</tr>
							</table>
						</div>

					</xsl:for-each>
					<br />
					<div style="page-break-before:always;">
						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<th colspan="10" style="color:black; font-size: 14px;" align="center">
									Средняя качественная оценка по линейным участкам и отделениям ПЧ
									<xsl:value-of select="@pch" />
								</th>
							</tr>
						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
							<thead>

								<tr>
									<td colspan="1" style="width:10%">ПД Ср.балл / Оценка </td>
									<td colspan="9">
										Номера отделений
										<br />
										Средний балл / Оценка
									</td>
								</tr>
							</thead>
							<xsl:for-each select="bykilometer/section/pchu/pd">
								<tr>
									<td>
										<span>
											<xsl:value-of select="@code" />
											<xsl:text></xsl:text>
											<xsl:value-of select="@rating" />
										</span>
									</td>
									<xsl:for-each select="pdb">
										<td style="width:10%">
											<span>
												<xsl:value-of select="@code" />
												<xsl:text></xsl:text>
												<xsl:value-of select="@rating" />
											</span>
										</td>
									</xsl:for-each>
									<xsl:if test="9 - count(pdb)>0">
										<td style="width:10%">
											<xsl:attribute name="colspan">
												<xsl:value-of select="9 - count(pdb)" />
											</xsl:attribute>
										</td>
									</xsl:if>
								</tr>
							</xsl:for-each>
						</table>
					</div>
					<br />
					<div style="page-break-before:always;">
						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<th colspan="10" style="color:black; font-size: 14px;" align="center">
									Перечень неудовлетворительных километров по ПЧ
									<xsl:value-of select="@pch" />
								</th>
							</tr>
						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
							<tr>
								<td class="main" text-align="center" colspan="13">
									<!-- <pre style="font:inherit;heigth:20px;text-align:left" > -->
									Направление:
									<b>
										<xsl:value-of select="@name" />
										&#160;&#160;&#160;&#160;&#160;&#160;&#160; Код:
										<xsl:value-of select="@code" />
										&#160;&#160;&#160;&#160;&#160;&#160;&#160; Путь:
										<xsl:value-of select="@track" />
									</b>
									<!-- </pre> -->
								</td>
							</tr>

							<tr>
								<td rowspan="3">км</td>
								<td rowspan="3">
									V
									<sub>пз</sub>
								</td>
								<td colspan="3">Основные параметры</td>
								<td colspan="2">Дополнительные параметры</td>
								<td></td>
							</tr>
							<tr>

								<td colspan="2">Балловая оценка</td>
								<td rowspan="2">Примечания</td>
								<td rowspan="2">Балловая оценка</td>
								<td rowspan="2">Примечания (ограничение скорости, неисправность)</td>
								<td rowspan="2">
									Сумма баллов
									Осн. +доп
								</td>
							</tr>
							<tr>
								<td>
									ГРК;
									<br />
									сочетания
									<br />
									и др
								</td>
								<td>Кривые</td>
							</tr>
							<xsl:for-each select="bykilometer/section">
								<!-- <tr>
								<td></td>
								<td colspan="4">Код направления</td>
								<td>
									<xsl:value-of select="@code"/>
								</td>
								<td>
									Путь <xsl:value-of select="@track"/>
								</td>
								<td></td>
							</tr> -->
								<xsl:for-each select="pchu/pd/pdb/km">
									<xsl:if test="@c10 ='Н'">
										<tr>
											<td>
												<xsl:value-of select="@n" />
											</td>
											<td>
												<xsl:value-of select="@speed" />
											</td>
											<td>
												<xsl:value-of select="@mpoint" />
											</td>
											<td>
												<xsl:value-of select="@cpoint" />
											</td>
											<td>
												<xsl:value-of select="@c11" />
											</td>
											<td>
												<xsl:value-of select="@apoint" />
											</td>
											<td>
												<xsl:value-of select="@c12" />
											</td>
											<td>
												<xsl:value-of select="@allsum" />
											</td>
										</tr>
									</xsl:if>
								</xsl:for-each>
							</xsl:for-each>
						</table>
					</div>

					<div style="page-break-before:always;">
						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<th colspan="10" style="color:black; font-size: 14px;" align="center">
									Количество километров с неисправностями по видам по ПЧ
									<xsl:value-of select="@pch" />
								</th>
							</tr>
						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
							<tr>
								<td rowspan="2" colspan="2">
									Код
									<br />
									направления
									<br />
									- № пути
								</td>
								<td rowspan="2" colspan="1">
									Всего
									<br />
									км
								</td>
								<td colspan="8" rowspan="1">Количество км с неисправностями по видам </td>
								<td rowspan="2">
									Км
									<br />
									с
									<br />
									огран.
									<br />
									скор.
								</td>
								<td rowspan="1" colspan="2">
									Средний балл по
									<br />
									параметрам
								</td>
							</tr>
							<tr>
								<td>ГРК</td>
								<td>Сочет.</td>
								<td>Кривые</td>
								<td>ПрУ</td>
								<td>
									ОШК,
									<br />
									У обр
								</td>
								<td>Износ</td>
								<td>Зазоры</td>
								<td>
									Нер.
									<br />
									проф.
								</td>
								<td colspan="1">Основным</td>
								<td colspan="1">Дополнительным </td>
							</tr>

							<xsl:for-each select="bykilometer/section">
								<tr>
									<td colspan="2">
										<xsl:value-of select="@code" />
										-
										<xsl:value-of select="@track" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@len" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@Grk" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@Sochet" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@KRIV" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@PRU" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@OSHK" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@IZNOS" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@ZAZOR" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@NEROVNOSTY" />
									</td>
									<td colspan="1">
										<xsl:value-of select="@KMSOGRSKOROST" />
									</td>
									<td colspan="1">
										<xsl:value-of select="../@mainavg" />
									</td>
									<td colspan="1">
										<xsl:value-of select="../@addavg" />
									</td>
								</tr>

								<tr>
									<td colspan="2" rowspan="2" align="left">
										Итого
									</td>
									<td>
										<xsl:value-of select="@len" />
									</td>

									<td>
										<xsl:value-of select="@d4" />
									</td>
									<td>
										<xsl:value-of select="@Sochet" />
									</td>
									<td>
										<xsl:value-of select="@KRIV" />
									</td>
									<td>
										<xsl:value-of select="@PRU" />
									</td>
									<td>
										<xsl:value-of select="@OSHK" />
									</td>
									<td>
										<xsl:value-of select="@IZNOS" />
									</td>
									<td>
										<xsl:value-of select="@ZAZOR" />
									</td>
									<td>
										<xsl:value-of select="@NEROVNOSTY" />
									</td>
									<td>
										<xsl:value-of select="@KMSOGRSKOROST" />
									</td>
									<td>
										<xsl:value-of select="../@mainavg" />
									</td>
									<td>
										<xsl:value-of select="../@addavg" />
									</td>
									<!-- <td>
                  <xsl:value-of select="../@AvgBallPch" />
                </td>
                <td>
                  <xsl:value-of select="@DOP" />
                </td> -->
								</tr>
							</xsl:for-each>
						</table>
					</div>

				</body>

			</html>
		</xsl:for-each>

	</xsl:template>
</xsl:stylesheet>