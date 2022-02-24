<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>Ф.О3.4 - Ведомость отступлений 3-й и 4-й степени</title>
				<style>
					h1 {

					string-set: doctitle content();

					}



					@page :right {

					@top-right {

					content: string(doctitle);

					margin: 30pt 0 10pt 0;

					font-size: 8pt;

					}

					}

					table {
					border-collapse: collapse;
					}

					thead {
					background: #f5e8d0;
					/* Цвет фона заголовка */
					}


					td {
					padding-left: 5px;
					align:centre;
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
					table.main, td.main, th.main {
					border-collapse: collapse;
					border: 1.5px solid black;
					font-size: 12px;
					font-family:  'Times New Roman';
					}
					table.main {
					width: 100%;
					margin: auto;
					}
					b {
					font-size: 18px;
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
					b {
					font-size: 12px;
					}
				</style>
				<style type="text/css" media="print">
					.dontprint {
					display: none;
					}


					#fixtitle{
					position:fixed;
					top:/*Здесь указать сколько отступ от верха*/
					left:/*Здесь указать сколько отступ слева*/
					right:/*Здесь указать сколько отступ справа*/
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
					<div id="pageFooter"    style="text-align: right;page-break-before:always; margin:10px 0px 0px;page">

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>


						<H4 align = "center">Ведомость отступлений 3-й и 4-й степени</H4>


					</div>
					<table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
						<tr>
							<td align="left">
								ПЧ:
								<xsl:value-of select="@distance" />
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
						<!-- <tr>
                                <td align="left">СПЛ: <xsl:value-of select="@ps" /></td>
                                <td align="left">Ведомость от: <xsl:value-of select="@date_statement" /></td>
                                <td align="left">Поездка: <xsl:value-of select="@trip_date" /></td>
                                <td align="left">ПЧ: <xsl:value-of select="@distance" /></td>
                             
                          
                             
                               
                            </tr>
                            <tr>
                               <td align="left">Участок:</td>
                               <td align="left">Направление:<xsl:value-of select="@direction" /></td>
                              
                               <td align="left">Путь: <xsl:value-of select="@track" /></td>
                                <td align="left">Км: <xsl:value-of select="@km" /></td>
                              </tr> -->
					</table>
					<table width="100%"  cellpadding="0" cellspacing="0" class="main" align="center">
						<thead>
							<tr>
								<td class="main" style="padding:2.5px;"  align="center" valign="middle" rowspan="1">Км</td>
								<td class="main" style="padding:2.5px;"  align="center" valign="middle" rowspan="1">M</td>
								<td class="main" style="padding:2.5px;"  align="center" valign="middle" rowspan="1">Отступление</td>
								<td class="main" style="padding:2.5px;"  align="center" valign="middle" rowspan="1">Амплитуда</td>
								<td class="main" style="padding:2.5px;"  align="center" valign="middle" rowspan="1">Длина, м</td>
								<td class="main" style="padding:2.5px;"  align="center" valign="middle" rowspan="1">Степень</td>
								<td class="main" style="padding:2.5px;"  align="center" valign="middle" rowspan="1">
									Скорость <br /> установленная,
									км/ч
								</td>
								<td class="main" style="padding:2.5px;" align="center" valign="middle" rowspan="1">
									Допустимая <br /> скорость, <br />км/ч
								</td>
								<td class="main" style="padding:2.5px;" align="center" valign="middle" rowspan="1">Объект</td>
							</tr>
						</thead>
						<!-- начало записи-->
						<xsl:for-each select="direction/track">

							<tr class="tr">
								<td  align="right"  colspan="9">
									<b>
										<xsl:value-of select="@trackinfo" />
									</b>
								</td>
							</tr>

							<xsl:for-each select="PCHU/PD/PDB/NOTE">
								<tr class="tr">
									<td  align="center" valign="middle">
										<xsl:value-of select="@km" />
									</td>
									<td  align="center" valign="middle">
										<xsl:value-of select="@meter" />
									</td>
									<td  align="center" valign="middle">
										<xsl:value-of select="@digression" />
									</td>
									<td  align="center" valign="middle">
										<xsl:value-of select="@value" />
									</td>
									<td  align="center" valign="middle">
										<xsl:value-of select="@length" />
									</td>
									<td  align="center" valign="middle">
										<xsl:value-of select="@typ" />
									</td>
									<td  align="center" valign="middle">
										<xsl:value-of select="@fullSpeed" />
									</td>
									<td  align="center" valign="middle">
										<xsl:value-of select="@allowSpeed" />
									</td>
									<td  align="center" valign="middle">
										<xsl:value-of select="@norma" />
									</td>
								</tr>
							</xsl:for-each>
						</xsl:for-each>
					</table>
					<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
						<tr>
							<td align="left">
								Начальник &#160;&#160;<xsl:value-of select="@ps" />
							</td>

							<td>
								<xsl:value-of select="@chief" />
							</td>
						</tr>
					</table>
				</xsl:for-each>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>