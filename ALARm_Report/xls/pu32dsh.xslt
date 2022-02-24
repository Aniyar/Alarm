<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>Сводная ведомость состояния шпал и скреплений (ПУ-32-Д_Ш)</title>
				<style>
					.rotate {
					writing-mode: vertical-rl;
					writing-mode: tb-rl;
					writing-mode: sideways-rl;
					font-weight: 15px;
					transform: rotate(-90deg);
					vertical-align: center;
					padding: 15px;

					}
					table.main,
					td.main,
					th.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					}
					td {
					<!-- text-align: center; -->
					vertical-align: middle;
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
				<xsl:for-each select="report/trip">
					<div id = "pageFooter" align="right" style="page-break-before:always;">

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>


						<table style="font-size: 16px; font-family: 'Times New Roman';width: 110%; margin: auto;margin-bottom:8px;border-collapse: collapse;   font-weight: bold;   ">
							<td align="center">
								Сводная ведомость состояния шпал и скреплений
							</td>
							<td align="rifht">
								(ПУ-32-Д_Ш)
								<br/>
								&#160;
							</td>
						</table>



						<!-- <H4 align = "center">Сводная ведомость состояния шпал и скреплений</H4>            <H4 </H4> -->
					</div>
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

								<xsl:value-of select="@ps" />
							</td>
							<td align="left">
								Проверка:
								<xsl:value-of select="@check" />
							</td>
							<td align="left">
								<xsl:value-of select="@periodDate" />
							</td>
						</tr>
					</table>
					<div align="center">
						<table style="border-collapse: collapse; width:100%;" align="center" border="1" height="150"    cellspacing="0">
							<thead>
								<tr>
									<td style="padding:2px; width:10%;" align="center" rowspan="2"> Км, пикет</td>
									<td style="padding:2px; width:10%;" align="center" colspan="3">тип</td>
									<td style="padding:2px; hight:10%;" align="center" rowspan="2" class="rotate">Эпюра шпал</td>
									<td style="padding:2px; width:10%;" align="center" colspan="2">дефектные</td>
									<td style="padding:2px; width:10%;" align="center" colspan="2">негодные</td>
									<td style="padding:2px; width:10%;" align="center" colspan="2">КУСТЫ</td>
									<td style="padding:2px; width:10%;" align="center" rowspan="2" >V уст</td>
									<td style="padding:2px; width:10%;" align="center" rowspan="2" >
										V доп
									</td>
								</tr>
								<tr>
									<td style="padding:2px; width:10%;" align="center" class="rotate">рельс</td>
									<td style="padding:2px; width:10%;" align="center" class="rotate">шпалы</td>
									<td style="padding:2px; width:10%;" align="center" class="rotate">скреп</td>
									<td style="padding:2px; width:10%;" align="center" class="rotate">шпалы</td>
									<td style="padding:2px; width:10%;" align="center" class="rotate">скреп</td>
									<td style="padding:2px; width:10%;" align="center" class="rotate">шпалы</td>
									<td style="padding:2px; width:10%;" align="center" class="rotate">скреп</td>
									<td style="padding:2px; width:10%;" align="center" >КНШ</td>
									<td style="padding:2px; width:10%;" align="center" >КНС</td>

								</tr>
							</thead>
							<xsl:for-each select="lev">
								<tr>
									<th align="right" colspan="13">
										<xsl:value-of select="@napr" />
									</th>
								</tr>
								<xsl:for-each select="note">
									<tr>
										<td align="left" >
											<xsl:value-of select="@km" />
										</td>
										<td align="center"  style=" border-right:0;">
											<xsl:value-of select="@rails" />
										</td>
										<td align="center" style=" border-right:0;border-left:0">
											<xsl:value-of select="@sleepers" />
										</td>
										<td align="center" style=" border-right:0;border-left:0">
											<xsl:value-of select="@fasteners" />
										</td>
										<td align="center" style=" border-right:0;border-left:0">
											<xsl:value-of select="@epur" />
										</td>
										<td align="center" style=" border-right:0;border-left:0">
											<xsl:value-of select="@def_sleepers" />
										</td>
										<td align="center" style=" border-right:0;border-left:0">
											<xsl:value-of select="@def_fasteners" />
										</td>
										<td align="center" style=" border-right:0;border-left:0">
											<xsl:value-of select="@bad_sleepers" />
										</td>
										<td align="center" style=" border-right:0;border-left:0">
											<xsl:value-of select="@bad_fasteners" />
										</td>
										<td align="center" style=" border-right:0;border-left:0">
											<xsl:value-of select="@kust_sleepers" />
										</td>
										<td align="center" style=" border-left:0">
											<xsl:value-of select="@kust_fasteners" />
										</td>
										<td align="center" >
											<xsl:value-of select="@vpz" />
										</td>
										<td align="center" >
											<xsl:value-of select="@vdop" />
										</td>
									</tr>
								</xsl:for-each>
							</xsl:for-each>
						</table>
						<table style="width:100%;height: 5%; font-size:10px" align="center" border="0" cellspacing="0" >
							<tr>
								<td>
									Начальник путеизмерителя
									<xsl:value-of select="@ps" />
								</td>
								<td>
									<xsl:value-of select="@chief" />
								</td>
							</tr>
						</table>
					</div>
				</xsl:for-each>
			</head>

		</html>
	</xsl:template>
</xsl:stylesheet>