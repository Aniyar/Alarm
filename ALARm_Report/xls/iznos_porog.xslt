<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>Ведомость участков с износом, превышающим порог</title>
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

					<div   style="page-break-before:always; margin:50px 0px 0px">


						<p  align="left" style="color:black;width: 100%;height: 1%;font-size: 9px;margin: auto;">
							<xsl:value-of select="@version" />
						</p>


						<H4 align = "center">Ведомость участков с износом, превышающим порог</H4>


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
								<td align="left">
									Пороговое значение - <xsl:value-of select="@periodDate" />
								</td>

							</tr>
						</table>
					</div>
					<div align="center">

						<table class="main">
							<thead>
								<tr>
									<th align="center" colspan="3" rowspan="2" class="main">Кривая</th>
									<th align="center" colspan="3" rowspan="1" class="main">Износ, мм </th>
									<th align="center" colspan="3" rowspan="1" class="main">Длина, м </th>
									<th align="center" colspan="1" rowspan="3" class="main">Vпз</th>
									<th align="center" colspan="1" rowspan="3" class="main">Vогр</th>
								</tr>
								<tr>
									<th align="center" colspan="1" rowspan="2" class="main">
										Сред.<br/>/Макс.
									</th>
									<th align="center" colspan="1" rowspan="2" class="main">№ПК </th>
									<th align="center" colspan="1" rowspan="2" class="main">
										Сред.<br/>/Макс.
									</th>
									<th align="center" colspan="1" rowspan="2" class="main">12-15мм</th>
									<th align="center" colspan="1" rowspan="2" class="main">16-20мм</th>
									<th align="center" colspan="1" rowspan="2" class="main">>20мм</th>
								</tr>
								<tr>
									<th align="center" colspan="1" class="main">Напр. </th>
									<th align="center" colspan="1" class="main">Начало – Конец </th>
									<th align="center" colspan="1" class="main">Радиус, м</th>
								</tr>
								<tr>
									<td colspan="9">
										Направление:   <xsl:value-of select="@direction"/>
									</td>
									<td colspan="2">
										Путь:  <xsl:value-of select="@track"/>
									</td>

								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="note">
									<tr class="tr">

										<td align="center">
											<xsl:value-of select="@km"/>
										</td>
										<td align="center">
											<xsl:value-of select="@m"/>
										</td>
										<td align="center">
											<xsl:value-of select="@found_date"/>
										</td>

										<td align="center" >
											<xsl:value-of select="@deviation"/>
										</td>
										<td align="center" >
											<xsl:value-of select="@count"/>
										</td>
									</tr>
								</xsl:for-each>

							</tbody>

						</table>
					</div>

					<table style="font-size: 12px; width: 80%;height: 5%; margin: auto;">
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


								<xsl:value-of select="@chief" />
							</td>
						</tr>
					</table>

				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>