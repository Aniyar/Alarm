<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>
					(Доп4)Протокол корректировки результатов
					контроля
				</title>
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

					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
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



					<div  id = "pageFooter"   align="right"  style="page-break-before:always;">
						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>
						<!-- <b>
                    
                        <p align="center" style="color:black; font-size:15px">Протокол корректировки результатов
                            контроля</p>
                    </b> -->
						<H4 align = "center">Протокол корректировки результатов  контроля</H4>
						<table style="width:100%" align="center">
							<tr>
								<td>
									ПЧ:
									<xsl:value-of select="@distance" />
								</td>
								<td>
									Дорога:
									<xsl:value-of select="@road" />
								</td>
							</tr>
							<tr>
								<td>
									<xsl:value-of select="@ps" />
								</td>
								<td>
									Проверка:
									<xsl:value-of select="@check" />
								</td>
								<td>
									<xsl:value-of select="@periodDate" />
								</td>
							</tr>
						</table>
						<!-- <b>
                        <p align="center" style="color:black; font-size:15px">Корректировка отступлений</p>
                    </b> -->
						<table style="width:100%" align="center" border="1" cellspacing="0" cellpadding="5">
							<thead>
								<tr align="center">
									<td style="padding:2.5px; width: 10%;" colspan="2" rowspan="1">
										Координата <br/>
									</td>
									<td style="padding:2.5px; width: 10%;" rowspan="2">
										Вид<br/> корректировки
									</td>
									<td style="padding:2.5px; width: 10%;" rowspan="2">Причина</td>
									<td style="padding:2.5px; width: 10%;" rowspan="2">Отступление </td>
									<td style="padding:2.5px;"  colspan="6">До корректировки</td>
									<td style="padding:2.5px;"  colspan="6">После корректировки</td>
								</tr>
								<tr align="center">
									<td style="padding:2.5px; width: 10%;" colspan="1" rowspan="1"> км </td>
									<td style="padding:2.5px; width: 10%;" colspan="1" rowspan="1"> м </td>
									<td align="center" style="padding:2.5px;">ст</td>
									<td align="center" style="padding:2.5px;">Откл</td>
									<td align="center" style="padding:2.5px;">Дл</td>

									<td align="center" style="padding:2.5px;">Огр.ск</td>
									<td align="center" style="padding:2.5px;">стр</td>
									<td align="center" style="padding:2.5px;">мост</td>
									<td align="center" style="padding:2.5px;">cт</td>
									<td align="center" style="padding:2.5px;">Откл</td>
									<td align="center" style="padding:2.5px;">Дл</td>

									<td align="center" style="padding:2.5px;">Огр.ск</td>
									<td align="center"  style="padding:2.5px;">стр</td>
									<td align="center" style="padding:2.5px;">мост</td>
								</tr>
								<tr>
									<td  align="center" colspan="17">
										<!-- <pre style="font:inherit;heigth:20px;text-align:left" > -->

										<xsl:value-of select="@direction" />
										(
										<xsl:value-of select="@code" />)
										Путь:
										<xsl:value-of select="@track" />

										<!-- </pre> -->
									</td>
								</tr>




							</thead>
							<xsl:for-each select="NOTES">
								<xsl:for-each select="NOTE">
									<tr>
										<td >
											<xsl:value-of select="@KmMtr" />
										</td>
										<td >
											<xsl:value-of select="@CorrectType" />
										</td>
										<td >
											<xsl:value-of select="@Comment" />
										</td>
										<td >
											<xsl:value-of select="@Otst" />
										</td>


										<td >
											<xsl:value-of select="@Stepen" />
										</td>
										<td >
											<xsl:value-of select="@value" />
										</td>
										<td >
											<xsl:value-of select="@length" />
										</td>
										<!-- <td >
                                        <xsl:value-of select="@count" />
                                    </td> -->
										<td >
											<!-- todo -->
										</td>
										<td >
											<xsl:value-of select="@strelka" />
										</td>
										<td >
											<xsl:value-of select="@most" />
										</td>



										<td >
											<xsl:value-of select="@old_Stepen" />
										</td>
										<td >
											<xsl:value-of select="@old_value" />
										</td>
										<td >
											<xsl:value-of select="@old_length" />
										</td>
										<!-- <td >
                                        <xsl:value-of select="@old_count" />
                                    </td> -->
										<td >
											<!-- todo -->
										</td>
										<td >
											<xsl:value-of select="@old_strelka" />
										</td>
										<td >
											<xsl:value-of select="@old_most" />
										</td>
									</tr>
								</xsl:for-each>
							</xsl:for-each>
						</table>
						<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
							<tr>
								<td>
									Начальник путеизмерителя:
									<xsl:value-of select="@ps" />
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