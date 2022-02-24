<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title> ФП-5.3 Ведомость километров, на которых СССП меньше Vпз для пассажирских поездов</title>
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
					.dontprint
					{
					display: none;
					}
					@page {
					counter-increment: page;
					counter-reset: page 1;
					@top-right {
					content: "Page " counter(page) " of " counter(pages);
					}
					}

				</style>
			</head>
			<script src="axios.min.js">//</script>
			<script src="/js/konva.min.js">//</script>
			<script src="/js/touch-emulator.js">//</script>
			<script src="/js/hammer-konva.js">//</script>
			<script src="getimage.js">//</script>
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
					<button class="dontprint button" onclick="exportTableToExcel('tab', 'members-data')">Excel </button>
					<input class="dontprint button" type="button" value="Create PDF"      id="btPrint" onclick="createPDF()" />

					<div    id="pageFooter"   style="text-align: right;page-break-before:always;">

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<H4 align = "center">Ведомость километров, на которых СССП меньше Vпз для пассажирских поездов (ФП-5.3)</H4>
						<table id="tab"  style="width:100%;     font-size: 14;" align="center">
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

						</table>
						<table id="tab"  width="100%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<tr >
									<td align="center" class="border-head" style="padding:2.5px;"  rowspan="1">№</td>
									<td align="center" class="border-head" style="padding:2.5px;" width="15%" rowspan="1">Км</td>
									<td align="center" class="border-head" style="padding:2.5px;" width="15%" rowspan="1">Перекос, мм</td>
									<td align="center" class="border-head" style="padding:2.5px;" rowspan="1">
										Прос.пр.,	<br/> мм
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" rowspan="1">
										Прослев.,	<br/> мм
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" rowspan="1">
										С <sub>в</sub>
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" rowspan="1">
										Шаблон,	<br/> мм
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" rowspan="1">
										Рихт.пр.,	<br/> мм
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" rowspan="1">
										Рихт.лев.,	<br/> мм
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" rowspan="1">
										С<sub>г</sub>
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" rowspan="1">
										С<sub>о </sub>
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" width="5%" rowspan="1">
										СССП, 	<br/>км/ч
									</td>
									<td align="center" class="border-head" style="padding:2.5px;" width="10%" rowspan="1">Vпз</td>
								</tr>
								<tr>
									<th  align="right" colspan="13"  style="font-size: 12px;">
										<xsl:value-of select="@track_info" />
									</th>
								</tr>
							</thead>

							<!-- начало записи-->
							<xsl:for-each select="Note">
								<tr class="tr">
									<td align="center">
										<xsl:value-of select="@n" />
									</td>
									<td align="center">
										<xsl:value-of select="@km" />
									</td>
									<td align="center">
										<xsl:value-of select="@Perekos" />
									</td>
									<td align="center">
										<xsl:value-of select="@prosR" />
									</td>
									<td align="center">
										<xsl:value-of select="@prosL" />
									</td>
									<td align="center">
										<xsl:value-of select="@cv" />
									</td>
									<td align="center">
										<xsl:value-of select="@shablon" />
									</td>
									<td align="center">
										<xsl:value-of select="@rihtR" />
									</td>
									<td align="center">
										<xsl:value-of select="@rihtL" />
									</td>
									<td align="center">
										<xsl:value-of select="@cr" />
									</td>
									<td align="center">
										<xsl:value-of select="@co" />
									</td>
									<td align="center">
										<xsl:value-of select="@cccp" />
									</td>
									<td align="center">
										<xsl:value-of select="@vpz" />
									</td>
								</tr>
							</xsl:for-each>
							<tr>
								<td align="left" colspan="13">
									Средняя СССП по участку <xsl:value-of select="@sred" />
								</td>
							</tr>

						</table>
					</div>
				</xsl:for-each>
			</body>
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
			<script>
				function createPDF() {
				var sTable = document.getElementById('tab').innerHTML;

				var style = "<style>
					";
					style = style + "table {width: 100%;font: 17px Calibri;}";
					style = style + "table, th, td {border: solid 1px #DDD; border-collapse: collapse;";
					style = style + "padding: 2px 3px;text-align: center;}";
					style = style + "
				</style>";

				// CREATE A WINDOW OBJECT.
				var win = window.open('', '', 'height=700,width=700');

				win.document.write('<html>
					<head>
						');
						win.document.write('<title>Profile</title>');  <!--  // <title> FOR PDF HEADER.-->
						win.document.write(style);          // ADD STYLE INSIDE THE HEAD TAG.
						win.document.write('
					</head>');
					win.document.write('<body>
						');
						win.document.write(sTable);         // THE TABLE CONTENTS INSIDE THE BODY TAG.
						win.document.write('
					</body>
				</html>');

				win.document.close(); 	// CLOSE THE CURRENT WINDOW.

				win.print();    // PRINT THE CONTENTS.
				}
			</script>
		</html>
	</xsl:template>
</xsl:stylesheet>