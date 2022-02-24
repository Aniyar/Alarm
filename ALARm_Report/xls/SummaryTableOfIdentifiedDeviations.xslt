<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>ФП-1.4 - Сводная таблица выявленных отступлений</title>
				<style>
					td {
					padding-left: 0.5px;
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
					padding-right:80px;
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
					.rotate {

					text-align: left;
					writing-mode: vertical-rl;

					font-weight: bold;
					transform: rotate(-180deg);
					padding: 1px; /* Поля вокруг содержимого ячеек */
					vertical-align: top;
					/* Выравнивание по верхнему краю /
					padding: 8px; / Поля вокруг содержимого ячеек */
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
				<!-- <style>
               

                    .exampleText{

                        padding: .5em 0;
                         writing-mode: vertical-rl;
                         writing-mode: tb-rl;
                         writing-mode: sideways-rl;
                         font-weight: bold;
                         transform: rotate(-180deg);
                         vertical-align: top; /* Выравнивание по верхнему краю */
                         padding: 5px; /* Поля вокруг содержимого ячеек */
                      }
                </style> -->
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
							win.document.write('<title>Profile</title>');                         <!--  // <title> FOR PDF HEADER.-->
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
				<script>
					function exportTableToExcel(tab, filename = '') {
					var downloadLink;
					var dataType = 'application/vnd.ms-excel';
					var tableSelect = document.getElementById(tab);
					var tableHTML = tableSelect.outerHTML.replace(/ /g, '%20');

					// Specify file name
					filename = filename ? filename + '.xls' : 'excel_data.xls';

					// Create download link element
					downloadLink = document.createElement("a");

					document.body.appendChild(downloadLink);

					if (navigator.msSaveOrOpenBlob) {
					var blob = new Blob(['\ufeff', tableHTML], {
					type: dataType
					});
					navigator.msSaveOrOpenBlob(blob, filename);
					} else {
					// Create a link to the file
					downloadLink.href = 'data:' + dataType + ', ' + tableHTML;

					// Setting the file name
					downloadLink.download = filename;

					//triggering the function
					downloadLink.click();
					}
					}
				</script>
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
			</head>

			<body>

				<xsl:for-each select="report/trip">


					<!-- <input type="button" value="Create PDF" id="tab" onclick="createPDF()" /> -->
					<div style="page-break-before:always;">

						<button class="dontprint button" onclick="exportTableToExcel('tab', 'members-data')">Excel </button>
						<button class="dontprint button" onclick="createPDF('tab', 'members-data')" value="Create PDF">
							PDF
						</button>

						<p align="left" style="color:black;width: 90%;height: 1%;font-size: 9px;">
							<xsl:value-of select="@version" />
						</p>

						<!-- <table style="font-size: 2 px;" align="left"> -->
						<!-- <table id="tab" style="width:90%" align="center">
                        <tr>
                            <xsl:value-of select="@ALARmReport" />
                        </tr>
                    </table> -->
						<H4 align = "center">Сводная таблица выявленных отступлений(ФП-1.4)</H4>

						<table width="90%" border="0" cellpadding="0" cellspacing="0" class="demotable" align="center">
							<tr>


								<td colspan="2">
									Дорога:
									<xsl:value-of select="@uch" />
								</td>
								<td colspan="2">
									Проверка:
									<xsl:value-of select="@check" />
								</td>
								<td>
									<xsl:value-of select="@trip_date" />
								</td>
							</tr>


						</table>
						<!-- <table id="tab" width="90%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center"> -->
						<table style="font-size:12px;  border-spacing: 0em;" border-spacing="0" id="tab" width="90%" border="1" cellpadding="0" cellspacing="0" class="border">
							<thead>
								<tr>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Код дороги</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Код направления</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Класс пути</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">ПЧ</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Дата проверки</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Путеизмеритель</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">№пути</p>
									</td>
									<td  height="80" text-align="left" rowspan="2">
										<p class="rotate">Км</p>
									</td>
									<td  height="80" text-align="left" rowspan="2">
										<p class="rotate">М</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">
											Вид                                        <br />
											отступления
										</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Норма</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">
											Величина                                        <br />
											отступления
										</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Длина</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Степень</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Балл</p>
									</td>
									<td  height="80" text-align="left" rowspan="2">
										<p class="rotate">
											V <sub>уст_скор</sub>
										</p>
									</td>


									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">
											V<sub>уст</sub>пасс
										</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">
											V <sub>уст_груз</sub>
										</p>
									</td>
									<td  height="80" text-align="left" rowspan="2">
										<p class="rotate">
											V<sub>огр_скор</sub>
										</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">
											V <sub>огр_пасс </sub>
										</p>
									</td>
									<td  height="80" text-align="left" rowspan="2">
										<p class="rotate">
											V <sub>огр_груз </sub>
										</p>
									</td>
									<!-- <td rowspan="2">
                                <p class="rotate">V <sub>огр_порож</sub></p>
                            </td> -->
									<th height="80" text-align="left" rowspan="1" colspan = "2" align="center">
										<p >БПД</p>
									</th>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Стрелка</p>
									</td>
									<td height="80" text-align="left" rowspan="2">
										<p class="rotate">Примечания</p>
									</td>
								</tr>
								<tr>
									<td height="80" text-align="left">
										<p class="rotate" colspan = "1">Радиус</p>
									</td>
									<td height="80" text-align="left">
										<p class="rotate" colspan = "1">Возвышение</p>
									</td>
								</tr>
							</thead>
							<xsl:for-each select="Note">
								<tr class="tr">
									<td align="center">
										<xsl:value-of select="@codDorogi" />
									</td>
									<td align="center">
										<xsl:value-of select="@codNapr" />
									</td>
									<td align="center">
										<xsl:value-of select="@classput" />
									</td>
									<td align="center">
										<xsl:value-of select="@pch" />
									</td>

									<td align="center">
										<xsl:value-of select="@checkDate" />
									</td>
									<td align="center">
										<xsl:value-of select="@nomerPS" />
									</td>
									<td align="center">
										<xsl:value-of select="@nomerPuti" />
									</td>
									<td align="center">
										<xsl:value-of select="@km" />
									</td>
									<td align="center">
										<xsl:value-of select="@m" />
									</td>
									<td align="center">
										<xsl:value-of select="@vidOts" />
									</td>
									<td align="center">
										<xsl:value-of select="@norma" />
									</td>
									<td align="center">
										<xsl:value-of select="@velichOts" />
									</td>
									<td align="center">
										<xsl:value-of select="@len" />
									</td>
									<td align="center">
										<xsl:value-of select="@stepen" />
									</td>
									<td align="center">
										<xsl:value-of select="@ball" />
									</td>
									<td align="center">

										<xsl:value-of select="@Vust" />

									</td>

									<td align="center">
										<xsl:value-of select="@vPass" />
									</td>
									<td align="center">
										<xsl:value-of select="@vGruz" />
									</td>
									<td align="center">

										<xsl:value-of select="@Vogr" />

									</td>

									<td align="center">
										<!-- <xsl:choose>
                                        <xsl:when test="@vOgrPass = '-'">
                                            <xsl:value-of select="999" />
                                        </xsl:when>
                                        <xsl:otherwise> -->
										<xsl:value-of select="@vOgrPass" />
										<!-- </xsl:otherwise>
                                    </xsl:choose> -->
									</td>
									<td align="center">
										<!-- <xsl:choose>
                                        <xsl:when test="@vOgrGruz = '-'">
                                            <xsl:value-of select="999" />
                                        </xsl:when>
                                        <xsl:otherwise> -->
										<xsl:value-of select="@vOgrGruz" />
										<!-- </xsl:otherwise>
                                    </xsl:choose> -->
									</td>
									<!-- <td align="center">
                                    <xsl:choose>
                                        <xsl:when test="@vOgrPorozh = '-'">
                                            <xsl:value-of select="999" />
                                        </xsl:when>
                                        <xsl:otherwise>
                                            <xsl:value-of select="@vOgrPorozh" />
                                        </xsl:otherwise>
                                    </xsl:choose>
                                </td> -->
									<td align="center">
										<xsl:value-of select="@radius" />
									</td>
									<td align="center">
										<xsl:value-of select="@elevation" />
									</td>
									<td align="center">
										<xsl:value-of select="@strelka" />
									</td>
									<td align="center">
										<xsl:value-of select="@primech" />
									</td>
								</tr>
							</xsl:for-each>
						</table>
					</div>
				</xsl:for-each>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>