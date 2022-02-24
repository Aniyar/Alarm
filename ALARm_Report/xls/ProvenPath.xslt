<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/report">
		<html>

			<head>
				<title>Ф.ДК - Справка о путях, проверенных путеизмерителем</title>
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
					table.main,
					td.main,
					th.main {
					border-collapse: collapse;
					border: 1.5px solid black;
					font-family: 'Times New Roman';
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
				<xsl:for-each select="trip">
					<div id="pageFooter" style="text-align: right;page-break-before:always;">

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>



						<H4 align = "center">Справка о путях, проверенных путеизмерителем</H4>
						<table  style="font-size: 12px; width: 100%;border-collapse: collapse; margin:auto;margin-bottom:8px;">
							<tr>
								<td align="left">
									ПЧ: <xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога : <xsl:value-of select="@road" />
								</td>

							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@ps" />
								</td>
								<td align="left">
									Проверка:<xsl:value-of select="@check" />
								</td>
								<td align="left">
									<xsl:value-of select="@periodDate" />
								</td>

							</tr>
						</table>
						<table align="center" class="main">
							<thead>
								<tr>
									<td class="main" rowspan="2">Направление</td>
									<td class="main" rowspan="2">ПЧУ</td>
									<td class="main" rowspan="2">ПД</td>
									<td class="main" rowspan="2">Путь</td>
									<td class="main">Всего</td>
									<td class="main" colspan="2">Проверено</td>
									<td class="main" colspan="2">Не проверено</td>
								</tr>
								<tr>
									<td class="main">км</td>
									<td class="main">км</td>
									<td class="main">%</td>
									<td class="main">км</td>
									<td class="main">%</td>
								</tr>
								<!-- <tr>
                       <td class="main" colspan="4">Итого по ПЧ</td>
                       
                    </tr> -->
							</thead>
							<xsl:for-each select="note">
								<xsl:if test="@countDirection != 0 and @total = ''">
									<tr class="tr">
										<td valign="top" class="main">
											<xsl:attribute name="rowspan">
												<xsl:value-of select="@countDirection" />
											</xsl:attribute>
											<xsl:value-of select="@direction" />
										</td>
										<td valign="top" class="main">
											<xsl:attribute name="rowspan">
												<xsl:value-of select="@countPchu" />
											</xsl:attribute>
											<xsl:value-of select="@pchu" />
										</td>
										<td class="main">
											<xsl:value-of select="@pd" />
										</td>
										<td class="main">
											<xsl:value-of select="@track" />
										</td>
										<td class="main">
											<xsl:value-of select="@kmAll" />
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
								</xsl:if>
								<xsl:if test="@countDirection = 0 and @countPchu != 0 and @total = ''">
									<tr class="tr">
										<td valign="top" class="main">
											<xsl:attribute name="rowspan">
												<xsl:value-of select="@countPchu" />
											</xsl:attribute>
											<xsl:value-of select="@pchu" />
										</td>
										<td class="main">
											<xsl:value-of select="@pd" />
										</td>
										<td class="main">
											<xsl:value-of select="@track" />
										</td>
										<td class="main">
											<xsl:value-of select="@kmAll" />
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
								</xsl:if>
								<xsl:if test="@countDirection = 0 and @countPchu = 0 and @total = ''">
									<tr class="tr">
										<td class="main">
											<xsl:value-of select="@pd" />
										</td>
										<td class="main">
											<xsl:value-of select="@track" />
										</td>
										<td class="main">
											<xsl:value-of select="@kmAll" />
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
								</xsl:if>
								<xsl:if test="@total != ''">
									<tr class="tr">

										<td align="left" colspan="4" class="main">Итого по ПЧ</td>

										<td align="left" class="main">
											<xsl:value-of select="@COUNTallKm" />
										</td>
										<td align="left" class="main">
											<xsl:value-of select="@COUNTcheckKm" />
										</td>
										<td align="left" class="main">
											<xsl:value-of select="@COUNTcheckPersent" />
										</td>
										<td align="left" class="main">
											<xsl:value-of select="@COUNTnotcheckKm" />
										</td>
										<td align="left" class="main">
											<xsl:value-of select="@COUNTnotcheckPersent" />
										</td>
									</tr>
								</xsl:if>
								<!-- <xsl:if test="@countDirection != 0 and @total = ''">
                            <tr class="tr">
                               
                                <td align="left" colspan="4" class="main">Итого по ПЧ</td>
                         
                                <td align="left" class="main">
                                    <xsl:value-of select="@kmAll" />
                                </td>
                                <td align="left" class="main">
                                    <xsl:value-of select="@kmCheck" />
                                </td>
                                <td align="left" class="main">
                                    <xsl:value-of select="@check" />
                                </td>
                                <td align="left" class="main">
                                    <xsl:value-of select="@kmNotCheck" />
                                </td>
                                <td align="left" class="main">
                                    <xsl:value-of select="@notCheck" />
                                </td>
                            </tr>
                      </xsl:if> -->
							</xsl:for-each>
						</table>
						<table style="width:100%" align="center">
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