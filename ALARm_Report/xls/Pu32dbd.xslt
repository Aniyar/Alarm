<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title> Сводная ведомость отступлений, угрожающих безопасности движения по параметрам, контролируемым ДКИ (Форма ПУ-32-Д_БД)</title>
				<style>
					<!-- td {
            padding-left: 5px;
            text-align: center;
            } -->

					td.info {
					text-align: center;
					font-weight: bold;
					}
					.pages {
					page-break-after:always;
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
					<div id = "pageFooter" align="right" style="page-break-before:always;">

						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>


						<table style="font-size: 16px; font-family: 'Times New Roman';width: 110%; margin: auto;margin-bottom:8px;border-collapse: collapse;   font-weight: bold;   ">
							<td align="center">
								Сводная ведомость отступлений, угрожающих безопасности
								<br /> движения по параметрам, контролируемым ДК
							</td>
							<td align="rifht">
								ПУ-32-Д_БД
								<br/>
								&#160;
							</td>
						</table>



						<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">

							<tr>
								<td align="left">
									ПЧ:                                        <xsl:value-of select="@pch" />
								</td>
								<td align="left">
									Дорога:                                        <xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@ps" />
								</td>
								<td align="left">
									Проверка:                       <xsl:value-of select="@check"/>  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="@date_statement" />
								</td>

							</tr>
						</table>
						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center" style="font-size: 12px;border-collapse: collapse;">
							<thead>
								<tr>
									<td align="center">ПЧУ</td>
									<td align="center">ПД</td>
									<td align="center">Км</td>
									<td align="center">ПК</td>
									<td align="center">Отступление</td>
									<td align="center">Величина</td>
									<td align="center">Длина м</td>
									<td align="center">Степень</td>
									<td align="center">V уст</td>
									<td align="center">V доп</td>
									<td align="center">Примечания</td>
								</tr>
								<xsl:for-each select="lev">
									<tr>
										<td align="right" style="font-weight: bold;" colspan="11">
											<xsl:value-of select="../@info" />
										</td>
									</tr>
									<xsl:for-each select="Note">
										<tr>
											<td align="center" style="border-right:0; border-bottom:0;border-top:0;">
												<xsl:value-of select="@Pchu" />
											</td>
											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@PD" />
											</td>
											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Km" />
											</td>
											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Pk" />
											</td>
											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Otst" />
											</td>
											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Velichina" />
											</td>
											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Dlina" />
											</td>
											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Stepen" />
											</td>

											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Vust" />
											</td>

											<td align="center" style=" border-right:0;border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Vogr" />
											</td>

											<td align="center" style=" border-left:0;border-bottom:0;border-top:0;">
												<xsl:value-of select="@Primech" />
											</td>
										</tr>
									</xsl:for-each>
									<tr>
										<td  class= "main " align="left" colspan="11" style=" border-top:0;">
											<xsl:value-of select="../@itogo" />
										</td>
									</tr>

									<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
										<tr>
											<td style=" font-size: 12px; font-family: 'Times New Roman';text-align:left;"  class="info" >
												<xsl:text>Итого по ПЧ: &#160;</xsl:text>
												<xsl:value-of select="./@pch" /> -  <xsl:value-of select="./@itogoPCH" /> шт


											</td>
										</tr>



									</table>
								</xsl:for-each>
								<!-- <table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
                                    <tr>
                                        <td style="text-align:left;font-size: 12px;     font-family: 'Times New Roman';" >
                                                <xsl:value-of select="./@final" />
                                            </td>
                                        </tr>
                                    </table> -->

								<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">

									<tr>
										<td style="text-align:left;font-size: 12px;     font-family: 'Times New Roman';" colspan="2">В том числе:</td>
									</tr>
									<xsl:for-each select="total">

										<tr>
											<td width="10%"></td>
											<td>
												<xsl:value-of select="./@final" />
											</td>
										</tr>
									</xsl:for-each>

								</table>



							</thead>

						</table>
						<table style="font-size: 12px; width: 95%;height: 5%; margin: auto;">


							<tr>
								<td align="left">
									Начальник <xsl:value-of select="@ps" />
								</td>
								<td align="right" >
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