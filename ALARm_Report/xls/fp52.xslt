<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>ФП-5.2 Ведомость статистических характеристик геометрии пути </title>
				<style type="text/css" media="print">
					.dontprint
					{
					display: none;
					}
					@page {
					counter-increment: page;
					counter-reset: page 1;
					@bottom-right {
					content: "Page " counter(page) " of " counter(pages);
					}
					}

				</style>
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
					padding-bottom: 100px;
					left: 0;
					bottom: 0;
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
					padding-bottom:4px;
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

					.rotate {

					padding: .5em 0;
					writing-mode: vertical-rl;
					writing-mode: tb-rl;
					writing-mode: sideways-rl;
					font-weight: bold;
					transform: rotate(-180deg);
					vertical-align: bottom;
					/* Выравнивание по верхнему краю /
					padding: 10px; / Поля вокруг содержимого ячеек */
					}

					td {
					padding-left: 5px;
					}




				</style>
				<script>
					function exportTableToExcel(tab, filename = ''){
					var downloadLink;
					var dataType = 'application/vnd.ms-excel';
					var tableSelect = document.getElementById(tab);
					var tableHTML = tableSelect.outerHTML.replace(/ /g, '%20');

					// Specify file name
					filename = filename?filename+'.xls':'excel_data.xls';

					// Create download link element
					downloadLink = document.createElement("a");

					document.body.appendChild(downloadLink);

					if(navigator.msSaveOrOpenBlob){
					var blob = new Blob(['\ufeff', tableHTML], {
					type: dataType
					});
					navigator.msSaveOrOpenBlob( blob, filename);
					}else{
					// Create a link to the file
					downloadLink.href = 'data:' + dataType + ', ' + tableHTML;

					// Setting the file name
					downloadLink.download = filename;

					//triggering the function
					downloadLink.click();
					}
					}
				</script>
				<script src="axios.min.js">//</script>
				<script src="/js/konva.min.js">//</script>
				<script src="/js/touch-emulator.js">//</script>
				<script src="/js/hammer-konva.js">//</script>
				<script src="getimage.js">//</script>



				<body >
					<div id="myModal" class="modal">
						<span class="close">Закрыть</span>
						<div class="modal-content">
							<div id="container" class="container">
								<img id="mainImage" />
							</div>
						</div>
					</div>
					<xsl:for-each select="report/trip">
						<button  class="dontprint button" onclick="exportTableToExcel('tab', 'members-data')" >Excel </button>
						<div  id="tab" style = "page-break-before:always;" >

							<p  align="left" style="color:black;width: 90%;height: 1%;font-size: 10px;">
								<xsl:value-of select="@version" />
							</p>

							<!-- class ="layer" -->
							<table style="font-size: 2 px;" align="centre">
								<tr>
									<th>
										<xsl:value-of select="@ALARmReport" />
									</th>

								</tr>
							</table>
							<table  style="font-size: 15px;" align="center">
								<tr>
									<th>
										Ведомость статистических характеристик геометрии пути (ФП-5.2)
									</th>
								</tr>
							</table>

							<table  id="tab"  style="width:100%" align="center">

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

							<table id="tab"  width="100%" border="1" cellpadding="0" cellspacing="0" class="border" align="center" style="font-size: 10px;">
								<thead>
									<tr>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate" >КМ</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate">М</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate">МО ширины</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate">МО кривизны</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate">МО уровня</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate">СКО ширины</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate">СКО перекоса</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate" >СКО правая просадка</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate" >СКО левая просадка</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate" >СКО правой рихтовки</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate" >СССП вертикальной плоскости</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate" >СССП горизонтальной плоскости</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate" >Стрелка</p>
										</td>
										<td valign="bottom">
											<p rowspan="3" colspan="2" class="rotate" >Объект(станция, мост, тоннель)</p>
										</td>

									</tr>
								</thead>
								<xsl:for-each select="Note">
									<!-- <tr>
                               <th class="main" align="right" colspan="14"><xsl:value-of select="@tripinfo" /></th>
                           </tr> -->
									<tr class="tr">
										<td>
											<xsl:value-of select="@Km" />
										</td>
										<td>
											<xsl:value-of select="@Meter" />
										</td>
										<td>
											<xsl:value-of select="@Gauge" />
										</td>
										<td>
											<xsl:value-of select="@Rihtovka" />
										</td>
										<td>
											<xsl:value-of select="@Lvl" />
										</td>
										<td>
											<xsl:value-of select="@Gauge_SKO" />
										</td>
										<td>
											<xsl:value-of select="@Skewness_SKO" />
										</td>
										<td>
											<xsl:value-of select="@Drawdown_left_SKO" />
										</td>
										<td>
											<xsl:value-of select="@Drawdown_right_SKO" />
										</td>
										<td>
											<xsl:value-of select="@Stright_right" />
										</td>
										<td>
											<xsl:value-of select="@Sssp_vert" />
										</td>
										<td>
											<xsl:value-of select="@Sssp_gor" />
										</td>
										<td>
											<xsl:value-of select="@Strelka" />
										</td>
										<td>
											<xsl:value-of select="@Object" />
										</td>
									</tr >

								</xsl:for-each>

							</table>
							<table  style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
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

			</head>

		</html>
	</xsl:template>
</xsl:stylesheet>