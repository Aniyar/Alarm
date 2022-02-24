<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/report">
		<html>
			<head>
				<TITLE>Интегральные показатели состояния поверхности катания рельсов</TITLE>
				<style>

					.pages {
					page-break-before:always;
					}
					table.main {
					border-collapse: collapse; margin:auto;
					width: 100%;
					}
					table.main, td.main, th.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					}
					display: table;
					}

					#pageFooter {
					display: table-footer-group;
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
					}
				</style>
			</head>
			<body>
				<div id="pageFooter" align="right" >
					<xsl:for-each select="trip">

						<table style="width:100%;text-align:left;border:none">
							<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
								<xsl:value-of select="@version" />
							</p>
						</table>

						<H4 align = "center">Интегральные показатели состояния поверхности катания рельсов (Ф.ДП1)</H4>

						<table  style="font-size: 12px; width: 110%;border-collapse: collapse; margin:auto;margin-bottom:8px;height: 5% ">
							<tr style="width:100%;text-align:left;border:none;height: 2%">
								<td >
									<xsl:value-of select="@npch"/>
								</td>
								<td style="text-align:left;border:none"></td>
								<td >
									Дорога:
									<xsl:value-of select="@way"/>
								</td>

								<td style="text-align:left;border:none"></td>
							</tr>
							<tr style="width:100%;text-align:left;border:none">
								<td style="text-align:left;border:none">
									<xsl:value-of select="@car"/>
								</td>
								<td style="text-align:left;border:none"></td>
								<td style="text-align:left;border:none">
									Проверка:  <xsl:value-of select="@addinfo"/>
								</td>
								<td style="text-align:left;border:none">
									<xsl:value-of select="@Period"/>
								</td>

								<td style="text-align:left;border:none">
									условия фильтрации:                                <i>
										I<sub>s</sub>
									</i> >
									<xsl:value-of select="@filter1"/>
								</td>
								<td style="text-align:left;border:none">
									<xsl:value-of select="@filter2"/>
								</td>
							</tr>
						</table>
						<table class="main">
							<thead>
								<tr>
									<td padding ="3px" class="main" align="center" style="width:15%;" colspan="2">Участок</td>
									<td class="main" align="center" style="width:7%;" rowspan="3">Признак участка</td>
									<td class="main" align="center" style="width:52%;" colspan="6">Среднее значение/среднеквадратичное отклонение глубины КН (мм)</td>
									<td class="main" align="center" style="width:15%;" colspan="2" rowspan="2">Индекс состояния поверхности катания рельсов</td>
									<td class="main" align="center" style="text-align: center; width:11%;font: unset;" rowspan="3">Оценка необходимого количества проходов РШП</td>
								</tr>
								<tr>
									<td class="main" align="center" rowspan="2">Начало (Км м)</td>
									<td class="main" align="center" rowspan="2">Конец (Км м)</td>
									<td class="main" align="center" colspan="2">Длинные волны</td>
									<td class="main" align="center" colspan="2">Средние волны</td>
									<td class="main" align="center"  colspan="2">Короткие волны</td>
								</tr>
								<tr>
									<td class="main" align="center">Правая нить</td>
									<td class="main" align="center" >Левая нить</td>
									<td class="main" align="center" >Правая нить</td>
									<td class="main" align="center" >Левая нить</td>
									<td class="main" align="center" >Правая нить</td>
									<td class="main" align="center" >Левая нить</td>
									<td class="main" align="center" >Правая нить;</td>
									<td class="main" align="center">Левая нить</td>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="nhapr">
									<tr class="main">
										<th style="text-align: right;" colspan="12">
											<xsl:value-of select="@name"/>
										</th>
									</tr>
									<xsl:for-each select="datarow">
										<tr  style="border-bottom: 1.5px solid black;    
                                            border-top: 1.5px solid black; font-size: 13px ">

											<td   style="text-align: center; ">
												<xsl:value-of select="begin"/>
											</td>
											<td   style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="end"/>
											</td>
											<td  style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="priznak"/>
											</td>
											<td   style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="rnitl"/>
											</td>
											<td   style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="lnitl"/>
											</td>
											<td style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="rnitm"/>
											</td>
											<td   style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="lnitm"/>
											</td>
											<td   style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="rnits"/>
											</td>
											<td  style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="lnits"/>
											</td>
											<td   style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="rstate"/>
											</td>
											<td   style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="lstate"/>
											</td>
											<td  style="text-align: center;border-bottom: 1px solid black;    
                                            border-top: 1px solid black; ">
												<xsl:value-of select="rshp_count"/>
											</td>
										</tr>
									</xsl:for-each>
								</xsl:for-each>
							</tbody>
						</table>
						<table style="font-size: 12px; width: 80%;height: 5%; margin: auto;">

							<tr>
								<td align="left">
									Начальник <xsl:value-of select="@car" />
								</td>
								<td align="right" >
									<xsl:value-of select="@chief" />
								</td>
							</tr>
						</table>
					</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>