<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>СВОДНАЯ ВЕДОМОСТЬ ОТСТУПЛЕНИЙ</title>

				<style>
					table.main {          border-collapse: collapse; margin:auto;
					width: 100%;          }          table.main, td.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					<!-- border: 1.5px solid black; -->
					margin: auto;
					margin-bottom:8px;
					text-align: center;
					<!-- border-collapse: collapse; -->

					}
					thead{
					font-size: 12px;
					font-family: 'Times New Roman';
					text-align: center;
					margin: auto;
					}
					tr{
					font-size: 12px;
					font-family: 'Times New Roman';
					text-align: center;
					margin: auto;
					}
					<!-- td {
                    padding-left: 5px;
                    } -->
					th.vertical{
					transform: rotate(-90deg);
					}
					.time-col {
					font-family: 'Times New Roman';
					padding: 5px;
					margin: auto;
					text-align: center;
					position: relative;
					writing-mode: vertical-rl;
					<!-- height: 70px; -->
					<!-- padding: 5px 0; -->
					transform: rotate(180deg);
					<!-- left: 19px; -->
					}

					.col{
					<!-- background-color: #F0F; -->
					position: relative;
					overflow: visible;
					padding-left: 5px;

					<!-- vertical-align: bottom; -->
					}
				</style>
			</head>
			<body>
				<xsl:for-each select="report/trip">
					<div style="page-break-before:always;">
						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>
						<table style="font-size: 12px; font-family:  'Times New Roman'; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">СВОДНАЯ ВЕДОМОСТЬ ОТСТУПЛЕНИЙ</p>
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
									&#160;&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@date_statement" />
									</b>

								</td>

							</tr>
						</table>

						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center" syle="font-size: 12px; font-family:'Times New Roman'; width: 100%; margin: auto;">
							<thead>
								<tr>
									<!-- <td class="border-head" style="padding:2.5px;" rowspan="3">Дистанция пути</td> -->
									<th class="col" rowspan="3">
										<div class='time-col'>Дистанция пути</div>
									</th>
									<td class="col" rowspan="3">
										<div class='time-col'>Проверено, км</div>
									</td>
									<th class="col" rowspan="1" colspan="7">Отступления в содержании скреплений</th>
									<th class="col" rowspan="1" colspan="7">Отступления в содержании рельсовых стыков</th>
									<th class="col" rowspan="1" colspan="7">Отступления в содержании шпал</th>
									<th class="col" rowspan="1" colspan="7">Отступления в содержании рельсов</th>
									<th class="col" rowspan="1" colspan="7">Отступления в содержании балласта</th>
									<th class="col" rowspan="1" colspan="7">Отступления в содержании бесстыкового пути</th>
									<th class="col" rowspan="3">
										<div class='time-col'>ПРОЧИЕ</div>
									</th>
									<th class="col" rowspan="3">
										<div class='time-col'>ИТОГО</div>
									</th>
									<th class="col" rowspan="3">
										<div class='time-col'>
											Всего выданных
											<br />
											ограничений
											<br />
											скорост
										</div>
									</th>
								</tr>
								<tr>
									<td class="col" rowspan="1" colspan="5">Из них с ограничением скорости</td>
									<td class="col" rowspan="2">
										<div class='time-col'>без огр.скорости</div>
									</td>
									<th class="col" rowspan="2">
										<div class='time-col'>ВСЕГО</div>
									</th>

									<td class="col" rowspan="1" colspan="5">Из них с ограничением скорости</td>
									<td class="col" rowspan="2">
										<div class='time-col'>без огр.скорости</div>
									</td>
									<th class="col" rowspan="2">
										<div class='time-col'>ВСЕГО</div>
									</th>

									<td class="col" rowspan="1" colspan="5">Из них с ограничением скорости</td>
									<td class="col" rowspan="2">
										<div class='time-col'>без огр.скорости</div>
									</td>
									<th class="col" rowspan="2">
										<div class='time-col'>ВСЕГО</div>
									</th>

									<td class="col" rowspan="1" colspan="5">Из них с ограничением скорости</td>
									<td class="col" rowspan="2">
										<div class='time-col'>без огр.скорости</div>
									</td>
									<th class="col" rowspan="2">
										<div class='time-col'>ВСЕГО</div>
									</th>

									<td class="col" rowspan="1" colspan="5">Из них с ограничением скорости</td>
									<td class="col" rowspan="2">
										<div class='time-col'>без огр.скорости</div>
									</td>
									<th class="col" rowspan="2">
										<div class='time-col'>ВСЕГО</div>
									</th>

									<td class="col" rowspan="1" colspan="5">Из них с ограничением скорости</td>
									<td class="col" rowspan="2">
										<div class='time-col'>без огр.скорости</div>
									</td>
									<th class="col" rowspan="2">
										<div class='time-col'>ВСЕГО</div>
									</th>
								</tr>
								<tr>
									<td class="col" rowspan="1">
										<div class='time-col'>0</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>15</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>25</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>40</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>60 и более</div>
									</td>

									<td class="col" rowspan="1">
										<div class='time-col'>0</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>15</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>25</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>40</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>60 и более</div>
									</td>

									<td class="col" rowspan="1">
										<div class='time-col'>0</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>15</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>25</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>40</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>60 и более</div>
									</td>

									<td class="col" rowspan="1">
										<div class='time-col'>0</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>15</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>25</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>40</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>60 и более</div>
									</td>

									<td class="col" rowspan="1">
										<div class='time-col'>0</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>15</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>25</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>40</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>60 и более</div>
									</td>

									<td class="col" rowspan="1">
										<div class='time-col'>0</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>15</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>25</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>40</div>
									</td>
									<td class="col" rowspan="1">
										<div class='time-col'>60 и более</div>
									</td>
								</tr>

							</thead>
							<xsl:for-each select="Prop">
								<tr>
									<td>
										<xsl:value-of select="@pch" />
									</td>
									<td>
										<xsl:value-of select="@checkKM" />
									</td>

									<td>
										<xsl:value-of select="@skrep0" />
									</td>
									<td>
										<xsl:value-of select="@skrep15" />
									</td>
									<td>
										<xsl:value-of select="@skrep25" />
									</td>
									<td>
										<xsl:value-of select="@skrep40" />
									</td>
									<td>
										<xsl:value-of select="@skrep60" />
									</td>
									<td>
										<xsl:value-of select="@skrepBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@skrepAll" />
									</td>

									<td>
										<xsl:value-of select="@gap0" />
									</td>
									<td>
										<xsl:value-of select="@gap15" />
									</td>
									<td>
										<xsl:value-of select="@gap25" />
									</td>
									<td>
										<xsl:value-of select="@gap40" />
									</td>
									<td>
										<xsl:value-of select="@gap60" />
									</td>
									<td>
										<xsl:value-of select="@gapBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@gapAll" />
									</td>

									<td>
										<xsl:value-of select="@shpal0" />
									</td>
									<td>
										<xsl:value-of select="@shpal15" />
									</td>
									<td>
										<xsl:value-of select="@shpal25" />
									</td>
									<td>
										<xsl:value-of select="@shpal40" />
									</td>
									<td>
										<xsl:value-of select="@shpal60" />
									</td>
									<td>
										<xsl:value-of select="@shpalBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@shpalAll" />
									</td>

									<td>
										<xsl:value-of select="@rail0" />
									</td>
									<td>
										<xsl:value-of select="@rail15" />
									</td>
									<td>
										<xsl:value-of select="@rail25" />
									</td>
									<td>
										<xsl:value-of select="@rail40" />
									</td>
									<td>
										<xsl:value-of select="@rail60" />
									</td>
									<td>
										<xsl:value-of select="@railBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@railAll" />
									</td>

									<td>
										<xsl:value-of select="@ballast0" />
									</td>
									<td>
										<xsl:value-of select="@ballast15" />
									</td>
									<td>
										<xsl:value-of select="@ballast25" />
									</td>
									<td>
										<xsl:value-of select="@ballast40" />
									</td>
									<td>
										<xsl:value-of select="@ballast60" />
									</td>
									<td>
										<xsl:value-of select="@ballastBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@ballastAll" />
									</td>

									<td>
										<xsl:value-of select="@bezstyk0" />
									</td>
									<td>
										<xsl:value-of select="@bezstyk15" />
									</td>
									<td>
										<xsl:value-of select="@bezstyk25" />
									</td>
									<td>
										<xsl:value-of select="@bezstyk40" />
									</td>
									<td>
										<xsl:value-of select="@bezstyk60" />
									</td>
									<td>
										<xsl:value-of select="@bezstykBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@bezstykAll" />
									</td>

									<td>
										<xsl:value-of select="@Prochie" />
									</td>
									<td>
										<xsl:value-of select="@Itogo" />
									</td>
									<td>
										<xsl:value-of select="@allOgrSpeed" />
									</td>
								</tr>
							</xsl:for-each>
							<xsl:for-each select="Itogo">
								<tr>
									<td>Итого по ЖД</td>
									<td>
										<xsl:value-of select="@checkKM" />
									</td>

									<td>
										<xsl:value-of select="@skrep0" />
									</td>
									<td>
										<xsl:value-of select="@skrep15" />
									</td>
									<td>
										<xsl:value-of select="@skrep25" />
									</td>
									<td>
										<xsl:value-of select="@skrep40" />
									</td>
									<td>
										<xsl:value-of select="@skrep60" />
									</td>
									<td>
										<xsl:value-of select="@skrepBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@skrepAll" />
									</td>

									<td>
										<xsl:value-of select="@gap0" />
									</td>
									<td>
										<xsl:value-of select="@gap15" />
									</td>
									<td>
										<xsl:value-of select="@gap25" />
									</td>
									<td>
										<xsl:value-of select="@gap40" />
									</td>
									<td>
										<xsl:value-of select="@gap60" />
									</td>
									<td>
										<xsl:value-of select="@gapBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@gapAll" />
									</td>

									<td>
										<xsl:value-of select="@shpal0" />
									</td>
									<td>
										<xsl:value-of select="@shpal15" />
									</td>
									<td>
										<xsl:value-of select="@shpal25" />
									</td>
									<td>
										<xsl:value-of select="@shpal40" />
									</td>
									<td>
										<xsl:value-of select="@shpal60" />
									</td>
									<td>
										<xsl:value-of select="@shpalBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@shpalAll" />
									</td>

									<td>
										<xsl:value-of select="@rail0" />
									</td>
									<td>
										<xsl:value-of select="@rail15" />
									</td>
									<td>
										<xsl:value-of select="@rail25" />
									</td>
									<td>
										<xsl:value-of select="@rail40" />
									</td>
									<td>
										<xsl:value-of select="@rail60" />
									</td>
									<td>
										<xsl:value-of select="@railBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@railAll" />
									</td>

									<td>
										<xsl:value-of select="@ballast0" />
									</td>
									<td>
										<xsl:value-of select="@ballast15" />
									</td>
									<td>
										<xsl:value-of select="@ballast25" />
									</td>
									<td>
										<xsl:value-of select="@ballast40" />
									</td>
									<td>
										<xsl:value-of select="@ballast60" />
									</td>
									<td>
										<xsl:value-of select="@ballastBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@ballastAll" />
									</td>

									<td>
										<xsl:value-of select="@bezstyk0" />
									</td>
									<td>
										<xsl:value-of select="@bezstyk15" />
									</td>
									<td>
										<xsl:value-of select="@bezstyk25" />
									</td>
									<td>
										<xsl:value-of select="@bezstyk40" />
									</td>
									<td>
										<xsl:value-of select="@bezstyk60" />
									</td>
									<td>
										<xsl:value-of select="@bezstykBezOgr" />
									</td>
									<td>
										<xsl:value-of select="@bezstykAll" />
									</td>

									<td>
										<xsl:value-of select="@Prochie" />
									</td>
									<td>
										<xsl:value-of select="@Itogo" />
									</td>
									<td>
										<xsl:value-of select="@allOgrSpeed" />
									</td>
								</tr>
							</xsl:for-each>
						</table>

						<table style="font-size: 12px; width: 90%;height: 5%; margin: auto;">
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
									&#160;&#160;&#160;&#160;&#160;
									<xsl:value-of select="@chief" />
								</td>
							</tr>
						</table>
						<!-- <div style="width: 90%;margin: auto;font-size: 15px;font-family: Arial;margin-top: 15px;" align="left">Начальник:<xsl:value-of select="@info" /></div> -->
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>