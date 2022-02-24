<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>Ведомость неисправностей, требующих ограничения скорости по основным и дополнительным параметрам </title>
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
					font-size: 18px;
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

					<div id="pageFooter"  align="right"   style="page-break-before:always; margin:50px 0px 0px">


						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>


						<H4 align = "center">
							Ведомость неисправностей, требующих ограничения скорости <br/> по основным и дополнительным параметрам
						</H4>


					</div>
					<div align="center">
						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
							<tr>
								<td align="left">
									ПЧ: <xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога: <xsl:value-of select="@road" />
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
					</div>
					<div align="center">

						<table class="main">
							<thead>
								<tr>
									<td colspan="8">
										Направление:   <xsl:value-of select="@direction"/>
									</td>
									<td colspan="2">
										Путь:  <xsl:value-of select="@track"/>
									</td>
								</tr>
								<tr>
									<td align="center" colspan="10" class="main">По основным параметрам</td>
								</tr>
								<tr>
									<td align="center" colspan="1" class="main">км</td>
									<td align="center" colspan="1" class="main">м</td>
									<td align="center" colspan="1" class="main">
										Дата<br/>обнаружения
									</td>
									<td align="center" colspan="1" class="main">Отступления</td>
									<td align="center" colspan="1" class="main">Откл </td>
									<td align="center" colspan="1" class="main">Длина, м </td>
									<td align="center" colspan="1" class="main">Степень </td>
									<td align="center" colspan="1" class="main">Vпз</td>
									<td align="center" colspan="1" class="main">Vогр </td>
									<td align="center" colspan="1" class="main">Примечание</td>
								</tr>

							</thead>
							<tbody>
								<xsl:for-each select="directions/tracks/main">

									<tr class="tr">

										<td class="main" align="center">
											<xsl:value-of select="@km"/>
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@m"/>
										</td>
										<td  class="main" align="center">
											<xsl:value-of select="@Data"/>
										</td>
										<td  class="main" align="center">
											<xsl:value-of select="@Ots"/>
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@Otkl"/>
										</td>

										<td class="main" align="center" >
											<xsl:value-of select="@len"/>
										</td>
										<td class="main" align="center" >
											<xsl:value-of select="@Stepen"/>
										</td>
										<td class="main" align="center" >
											<xsl:value-of select="@vpz"/>
										</td>
										<td class="main" align="center" >
											<xsl:value-of select="@vogr"/>
										</td>

										<td class="main" align="center" >
											<xsl:value-of select="@Primech"/>
										</td>
									</tr>

								</xsl:for-each>


								<tr>
									<td align="center" colspan="10" class="main">По дополнительным параметрам </td>
								</tr>
								<tr>
									<td align="center" colspan="1" class="main">км</td>
									<td align="center" colspan="1" class="main">м</td>
									<td align="center" colspan="1" class="main">
										Дата<br/>обнаружения
									</td>
									<td align="center" colspan="1" class="main">Отступления</td>
									<td align="center" colspan="1" class="main">Откл </td>
									<td align="center" colspan="2" class="main">Длина, м </td>
									<td align="center" colspan="1" class="main">Vпз</td>
									<td align="center" colspan="1" class="main">Vогр </td>
									<td align="center" colspan="1" class="main">Примечание</td>
								</tr>
								<xsl:for-each select="directions/tracks/add">

									<tr class="tr">

										<td class="main" align="center">
											<xsl:value-of select="@km"/>
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@m"/>
										</td>
										<td  class="main" align="center">
											<xsl:value-of select="@Data"/>
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@Ots"/>
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@Otkl"/>
										</td>

										<td colspan="2" class="main" align="center" >
											<xsl:value-of select="@len"/>
										</td>

										<td class="main" align="center" >
											<xsl:value-of select="@vpz"/>
										</td>
										<td class="main" align="center" >
											<xsl:value-of select="@vogr"/>
										</td>

										<td class="main" align="center" >
											<xsl:value-of select="@Primech"/>
										</td>
									</tr>

								</xsl:for-each>
								<xsl:for-each select="directions/tracks">
									<tr>
										<th align="left" colspan="9" class="main">
											Итого по пути             <xsl:value-of select="@track"/>
										</th>
										<th align="center" colspan="1" class="main">
											<xsl:value-of select="count(main)+count(add)" />
										</th>

									</tr>
									<tr>
										<th align="left" colspan="9"  class="main">Итого по ПЧ                     </th>
										<th align="center" colspan="1" class="main">
											<xsl:value-of select="@countDistance"/>
										</th>
									</tr>
								</xsl:for-each>
							</tbody>

						</table>
					</div>

					<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
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

						<tr>
							<td colspan="2" align="left">
								Начальник <xsl:value-of select="@ps" />
							</td>


							<td align="right">
								<xsl:value-of select="@chief" />
							</td>
						</tr>
					</table>

				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>