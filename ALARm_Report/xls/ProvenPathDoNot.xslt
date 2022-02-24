<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/report">
		<html>

			<head>
				<title>Доп 2 Справка о путях, не проверенных путеизмерителем</title>
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
					font-size: 12px;
					border-collapse: collapse;
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
					#content {
					display: table;
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
					@page {
					@bottom-center {
					content: "Page " counter(page) " of " counter(pages);
					}
					}

				</style>
				<style media="print">
					@page {
					counter-increment: page;
					counter-reset: page 1;
					@top-right {
					content: "Page " counter(page) ;
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
				<xsl:for-each select="trip">





					<div  align="right"  style="page-break-before:always;"  >

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<b>
							<p align="center" style="color:black; font-size:18px">
								Справка о путях, не проверенных путеизмерителем
							</p>
						</b>
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

									<xsl:value-of select="@car" />
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
						<table align="center" class="main">
							<thead>
								<tr>
									<td class="main">Направление</td>
									<td class="main">Путь</td>
									<td class="main">Начало</td>
									<td class="main">Конец</td>
									<td class="main">Примечание</td>
								</tr>
							</thead>
							<xsl:for-each select="Note">
								<tr class="tr">
									<td class="main">
										<xsl:value-of select="@Direction" />
									</td>
									<td class="main">
										<xsl:value-of select="@Way" />
									</td>
									<td  align =" center" class="main">
										<xsl:value-of select="@Start" />
									</td>
									<td align =" center" class="main">
										<xsl:value-of select="@Final" />
									</td>
									<td class="main">
										<xsl:value-of select="@Note" />
									</td>
								</tr>
							</xsl:for-each>
						</table>
						<table style="width:70%" align="center">
							<xsl:for-each select="Note">
								<tr>
									<td align="left">
										Общая длина непроверенных участков
										<xsl:value-of select="@carrr" /> м
									</td>

								</tr>
							</xsl:for-each>
						</table>

						<table style="width:80%" align="center">
							<tr>
								<td  align="left">
									Начальник путеизмерителя:
									<xsl:value-of select="@car" />
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