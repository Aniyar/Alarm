<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>(Ф.О3)Ведомость отступлений 3 степени </title>
				<style>
					td {
					padding-left: 5px;
					}
					table.main, td.main, th.main {

					border-collapse: collapse;
					border: 1.5px solid black;
					font-size: 12px;
					font-family: 'Times New Roman';
					}

					table.main {
					width: 100%;
					margin: auto;
					}
					b {
					font-size: 12px;
					}
					thead {
					background: #f5e8d0;
					/* Цвет фона заголовка */
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

					<div id="pageFooter" style="text-align: right;page-break-before:always; margin:50px 0px 0px">


						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>


						<H4 align="center">Ведомость отступлений 3 степени (Ф.О3) </H4>


					</div>
					<div align="center">
						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
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
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									<xsl:value-of select="@periodDate" />
								</td>


							</tr>
						</table>
					</div>
					<div align="center">

						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center" style="font-size: 12px;border-collapse: collapse;">
							<thead>
								<tr>
									<!-- <td align="center" style="border-right:0;" class="main">ПЧУ</td>
                                <td align="center" style="border-left:0; border-right:0;" class="main">ПД</td>
                                <td align="center" style="border-left:0;" class="main">ПДБ</td> -->
									<td align="center" class="main">ПЧУ</td>
									<td align="center" class="main">ПД</td>
									<td align="center" class="main">ПДБ</td>
									<td align="center" class="main">км</td>
									<td align="center" class="main">м</td>
									<td align="center" class="main">
										Дата
										<br />
										обнаруж.
									</td>
									<td align="center" class="main">Отступления</td>
									<td align="center" class="main">
										Отклонение,
										<br />
										мм
									</td>
									<td align="center" class="main">Длина, м</td>
									<td align="center" class="main">Кол</td>
									<td align="center" class="main">Прим.</td>
								</tr>
							</thead>
							<xsl:for-each select="directions">
								<tr>
									<td align="left" colspan="11">
										Направление:
										<xsl:value-of select="@direction" />
										<b>
											(
											<xsl:value-of select="@directioncode" />
											)
										</b>
										&#160;&#160;&#160;
										&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Путь:
										<b>
											<xsl:value-of select="@track" />
										</b>
									</td>
								</tr>

								<xsl:for-each select="tracks">


									<xsl:for-each select="note">
										<tr class="tr">
											<td align="center">
												<xsl:value-of select="@pchu" />
											</td>
											<td align="center">
												<xsl:value-of select="@pd" />
											</td>
											<td align="center">
												<xsl:value-of select="@pdb" />
											</td>
											<td align="center">
												<xsl:value-of select="@km" />
											</td>
											<td align="center">
												<xsl:value-of select="@m" />
											</td>
											<td align="center">
												<xsl:value-of select="@found_date" />
											</td>

											<td align="center">
												<xsl:value-of select="@deviation" />
											</td>
											<td align="center">
												<xsl:value-of select="@digression" />
											</td>
											<td align="center">
												<xsl:value-of select="@len" />
											</td>

											<td align="center">
												<xsl:value-of select="@count" />
											</td>


											<td align="center">
												<b>
													<xsl:value-of select="@primech" />
												</b>
											</td>
										</tr>
									</xsl:for-each>
									<tr>
										<th align="left" colspan="10" class="main">
											Итого по пути
											<xsl:value-of select="@track" />
										</th>
										<th align="center" colspan="1" class="main">
											<xsl:value-of select="@countDistance" />
										</th>

									</tr>
									<tr>
										<th align="left" colspan="10" class="main">Итого по ПЧ                     </th>
										<th align="center" colspan="1" class="main">
											<xsl:value-of select="@countDistance" />
										</th>
									</tr>
								</xsl:for-each>
							</xsl:for-each>
						</table>
					</div>
					<div align="center">
						<table width="100%" style="font-size: 12px;border-collapse: collapse;height: 5%;">

							<tr>
								<td colspan="2">В том числе:</td>
							</tr>
							<xsl:for-each select="total">
								<tr>
									<td width="10%"></td>
									<td>
										<xsl:value-of select="@totalinfo" />
									</td>
								</tr>
							</xsl:for-each>
							<!-- <tr>
                                <td width="10%"></td>
                                <td>Суж -                                    <xsl:value-of select="@countSuj" />
                                </td>
                            </tr>
                            <tr>
                                <td width="10%"></td>
                                <td>Уш -                                    <xsl:value-of select="@countUsh" />
                                </td>
                            </tr>
                            <tr>
                                <td width="10%"></td>
                                <td>У -                                    <xsl:value-of select="@countU" />
                                </td>
                            </tr>
                            <tr>
                                <td width="10%"></td>
                                <td>П -                                    <xsl:value-of select="@countP" />
                                </td>
                            </tr>
                            <tr>
                                <td width="10%"></td>
                                <td>Р -                                    <xsl:value-of select="@countR" />
                                </td>
                            </tr>
                            <tr>
                                <td width="10%"></td>
                                <td>Пр -                                    <xsl:value-of select="@countPr" />
                                </td>
                            </tr> -->
							<tr>
								<td align="left" width="10%">
									Начальник путеизмерителя
									<xsl:value-of select="@ps" />
								</td>
								<td align="right">
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