<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:template match="/">
		<html>
			<head>
				<title>Ф.О2 - Отступления 2 степени</title>
				<style>

					tbody {
					display: table-row-group
					}




					thead {
					background: #f5e8d0;
					/* Цвет фона заголовка */
					display: table-header-group;
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
					table.main, td.main, th.main {
					border-collapse: collapse;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					font-size: 12px;

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
				<style type="text/css" media="print">
					@media print {
					h1 {page-break-before: always;}
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
					<div align="right" id="pageFooter" style="page-break-inside:always">

						<p align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<b>
							<p align="center" style="color:black; font-size:15px">Отступления 2 степени (Ф.О2)</p>
						</b>



						<table style="font-size: 12px; width: 100%;border-collapse: collapse; margin:auto;margin-bottom:12px;">

							<tr>
								<td align="left">
									ПЧ:                                    <xsl:value-of select="@distance" />
								</td>
								<td align="left">
									Дорога:                                    <xsl:value-of select="@road" />
								</td>

							</tr>
							<tr>
								<td align="left">
									<xsl:value-of select="@ps" />
								</td>
								<td align="left">
									Проверка:
									<xsl:value-of select="@check" />
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;  <xsl:value-of select="@periodDate" />
								</td>

							</tr>

						</table>

					</div>
					<div align="center">
						<div align="center" style="column-count: 2;
                            -moz-column-count: 2;
                            -webkit-column-count: 2;
                            column-gap:0px;
                            -moz-column-gap:0px;
                            -webkit-column-gap:0px;">
							<!-- <div align="center" style="
                            column-count: 2;
                             page-break-inside: avoid; 
                             break-inside: avoid; 
                            /* font-size: 8px; */
                        "> -->

							<!-- <div align="center"> -->
							<table width="100%" border="0" cellpadding="5" cellspacing="0" class="border" align="center"
					style="font-size: 12px; border-collapse: collapse;  border: 1.5px solid black; font-family: 'Times New Roman';page-break-inside:avto;">
								<thead  style="page-break-inside:avto">
									<tr>
										<td align="center" style="border-right:0;" class="main">ПЧУ</td>
										<td align="center" style="border-left:0; border-right:0;" class="main">ПД</td>
										<td align="center" style="border-left:0;" class="main">ПДБ</td>
										<td align="center" class="main">км</td>
										<td align="center" class="main">м</td>
										<td align="center" class="main">Отступления</td>
										<td align="center" class="main">
											Откл,                                            <br/>
											мм
										</td>
										<td align="center" class="main">
											Длина,                                            <br/>
											м
										</td>
										<td align="center" class="main">Кол</td>
									</tr>
								</thead>
								<tbody>
									<xsl:for-each select="directions">
										<!-- <tr class="tr">
                                    <td align="left" colspan="9">
                                        Направление:
                                        <xsl:value-of select="@direction" />   &#160;&#160;&#160;
                                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Путь:<xsl:value-of select="@track" /> 
                                    </td>
                                </tr> -->
										<tr class="tr">
											<td align="left" colspan="9">
												Направление:
												<xsl:value-of select="@direction" />
												&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Путь:<xsl:value-of select="@track" />
											</td>
											<xsl:for-each select="tracks">

												<xsl:for-each select="note">
													<tr class="tr">
														<td align="center">
															<xsl:value-of select="@pchu"/>
														</td>
														<td align="center">
															<xsl:value-of select="@pd"/>
														</td>
														<td align="center">
															<xsl:value-of select="@pdb"/>
														</td>
														<td align="center">
															<xsl:value-of select="@km"/>
														</td>
														<td align="center">
															<xsl:value-of select="@m"/>
														</td>
														<td align="center">
															<xsl:value-of select="@deviation"/>
														</td>
														<td align="center">
															<xsl:value-of select="@digression"/>
														</td>
														<td align="center">
															<xsl:value-of select="@len"/>
														</td>
														<td align="center">
															<xsl:value-of select="@count"/>
														</td>
													</tr>
												</xsl:for-each>

												<tr>
													<th align="left" colspan="9">
														Всего по пути                                                <xsl:value-of select="@track"/>
														-                                                <xsl:value-of select="@count" />
														шт
													</th>
													<!-- <th align="left" colspan="9">Всего по пути <xsl:value-of select="@track"/> - <xsl:value-of select="count" /> шт</th> -->
												</tr>
											</xsl:for-each>
										</tr>
									</xsl:for-each>
								</tbody>

							</table>
							<table width="90%" style="font-size: 12px;
                            font-family: Arial;">
								<tr>
									<td colspan="2">
										Итого по ПЧ:                                        <xsl:value-of select="@countDistance" />
										шт
									</td>
								</tr>
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
									<td align="left">
										Начальник путеизмерителя                                        <xsl:value-of select="@car" />
									</td>
									<td align="right">
										<xsl:value-of select="@chief" />
									</td>
								</tr>
							</table>

						</div>
					</div>
				</xsl:for-each>

			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>