<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>Сводная ведомость оценки состояния рельсов и стыков (Форма ПУ-32-Д_РиС)</title>
				<style>
					.border {
					border: 1px solid #000;
					}

					.border-head {
					border-bottom: 1px solid #000;
					border-left: 1px solid black
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
			</head>

			<body>
				<xsl:for-each select="report/trip">
					<div   id = "pageFooter" align="right" style="page-break-before:always;">

						<p align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>




						<H4 align = "center">Сводная ведомость оценки состояния рельсов и стыков (Форма ПУ-32-Д_РиС)</H4>

						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<td align="left">
									ПЧ:
									<xsl:value-of select="@pch" />
								</td>
								<td align="left">
									Дорога:
									<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">

									<xsl:value-of select="@dki" />
								</td>
								<td align="left">
									Проверка:
									<xsl:value-of select="@check" />
								</td>
								<td align="left">
									<xsl:value-of select="@trip_date" />
								</td>
								<td align="right">
									Пороговое значение Из.В -
									<xsl:value-of select="@mm" />
									мм
								</td>
							</tr>

							<!-- <tr>
                                <td>
                                    ПС:
                                    <xsl:value-of select="@dki" />
                                </td>
                                <td>
                                    Дорога:
                                    <xsl:value-of select="@road" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <xsl:value-of select="@trip_date" />
                                </td>
                                <td>
                                    ПЧ:
                                    <xsl:value-of select="@pch" />
                                </td>
                            </tr> -->
						</table>
						<!-- <table width="90%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center" style="font-size: 10px;border-collapse: collapse;">
                                <thead> -->
						<table width="100%" border="0" cellpadding="0" cellspacing="0" class="border" align="center" style="font-size: 14px;border-collapse: collapse;">
							<thead>
								<tr>
									<td class="border-head" style="padding:2.5px;" rowspan="1" align="center">Км</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1" align="center">м</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1" align="center">Отступление</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1" align="center">Величина</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1" align="center">Длина (м)</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1" align="center">
										Пороговое										<br />
										значение
									</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1" align="center">V уст</td>
									<td class="border-head" style="padding:2.5px;" rowspan="1" align="center">V доп </td>
								</tr>
							</thead>
							<xsl:for-each select="lev">
								<tr>
									<th class="border-head" style="padding:2.5px;" colspan="8" align="right">
										Направление:&#160;   <xsl:value-of select="./@napr" />
										&#160; &#160;  Путь: &#160; <xsl:value-of select="@put" />
										<!--  &#160;ПЧ:  <xsl:value-of select="@pch" />&#160;&#160;Проверка:  <xsl:value-of select="@trip_date" /> -->
									</th>
								</tr>
								<xsl:for-each select="Note">
									<tr>
										<td align="center">
											<xsl:value-of select="@Km" />
										</td>
										<td align="center">
											<xsl:value-of select="@M" />
										</td>
										<td align="center">
											<xsl:value-of select="@Otst" />
										</td>
										<td align="center">
											<xsl:value-of select="@Velich" />
										</td>
										<td align="center">
											<xsl:value-of select="@Dlina" />
										</td>
										<td align="center">
											<xsl:value-of select="@Porog" />
										</td>
										<td align="center">
											<xsl:value-of select="@Vust" />
										</td>
										<td align="center">
											<xsl:value-of select="@Vdop" />
										</td>
									</tr>
								</xsl:for-each>
								<tr>
									<td class="border-head" style="padding:2.5px;" colspan="8">
										Всего по пути: &#160;
										<xsl:value-of select="./@vsego" />
										шт
									</td>
								</tr>


								<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
									<tr>
										<td style=" font-size: 12px; font-family: 'Times New Roman';text-align:left;" class="info">
											<xsl:text>Итого по ПЧ: &#160;</xsl:text>
											<xsl:value-of select="./@pch" />
											-										<xsl:value-of select="./@vsego" />
											шт


										</td>
									</tr>



								</table>
							</xsl:for-each>

							<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">

								<tr>
									<td style="text-align:left;font-size: 12px;     font-family: 'Times New Roman';" colspan="2">В том числе:</td>
								</tr>
								<xsl:for-each select="total">

									<tr>
										<td width="10%"></td>
										<td>
											<xsl:value-of select="@final" />
										</td>
									</tr>
								</xsl:for-each>

							</table>
						</table>


						<!-- <tr>
                               <td class="border-head" style="padding:2.5px;" colspan="2">
                                    <xsl:text>Итого: &#160;</xsl:text>
                                    <xsl:value-of select="@itogo" />
                                </td>
                               <td class="border-head" style="text-align:justify;" colspan="6">
                                    <xsl:value-of select="@DownhillLeft" /> &#8195;&#8195;&#8195;&#8195;
                                    <xsl:value-of select="@DownhillRight" />
                                    <br />
                                    <xsl:value-of select="@TreadTiltLeft" /> &#8195;&#8195;&#8195;&#8195;
                                    <xsl:value-of select="@TreadTiltRight" />
                                    <br />
                                    <xsl:value-of select="@SideWearLeft" /> &#8195;&#8195;&#8195;&#8195;
                                    <xsl:value-of select="@SideWearRight" />
                                    <br />
                                    <xsl:value-of select="@VertIznosL" /> &#8195;&#8195;&#8195;&#8195;
                                    <xsl:value-of select="@VertIznosR" />
                                    <br />
                                    <xsl:value-of select="@gapi" />
                                    <br />
                                </td>
                            </tr>  -->



						<table style="width:100%;height: 5%; font-size:10px" align="center" border="0" cellspacing="0">
							<tr>
								<td>
									Начальник путеизмерителя
									<xsl:value-of select="@dki" />
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