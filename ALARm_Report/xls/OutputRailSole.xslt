<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>

				<title>ВЕДОМОСТЬ ВЫХОДА ПОДОШВЫ РЕЛЬСА ИЗ РЕБОРД ПОДКЛАДКИ</title>


				<script src="axios.min.js">//</script>
				<script src="getimage.js">//</script>

				<style>
					.pages {
					page-break-before:always;
					}
					table.main {
					border-collapse: collapse;
					margin: auto;
					width: 100%;
					}
					table.main, td.main, th.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
					}

					.dontprint {
					display: none;
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
					width: 80%;
					}


					.close {
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
				</style>





			</head>
			<body>
				<div id="myModal" class="modal">

					<!-- Modal content -->
					<div class="modal-content">
						<span class="close">Закрыть</span>
						<center>
							<img id="mainImage" />
						</center>
					</div>

				</div>
				<xsl:for-each select="report/pages">
					<div class="pages" >

						<table style="font-size: 12px; font-family: 'Times New Roman'; width: 100%; margin: auto;">

							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">ВЕДОМОСТЬ ВЫХОДА ПОДОШВЫ РЕЛЬСА ИЗ РЕБОРД ПОДКЛАДКИ </p>
									</b>
								</td>
							</tr>

							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@road" />
										ЖД
									</b>
								</td>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@period" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;

									<b>
										<xsl:value-of select="@type" />
									</b>

								</td>
								<td  align="left">
								</td>


							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@car" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@data" />
									</b>

								</td>
							</tr>
						</table>
						<table class="main">
							<tr>
								<td class="main" align="center">№ п/п</td>
								<td class="main" align="center">ПЧУ, ПД, ПДБ</td>
								<td class="main" align="center">Перегон, станция</td>
								<td class="main" align="center">км</td>
								<td class="main" align="center">пк</td>
								<td class="main" align="center">м</td>
								<td class="main" align="center">Vуст</td>
								<td class="main" align="center">Отступление</td>
								<td class="main" align="center">Нить</td>
								<td class="main" align="center">Величина</td>
								<td class="main" align="center">Скрепление</td>
								<td class="main" align="center">Характеристика пути</td>
								<td class="main" align="center">Vдоп</td>
								<td class="main" align="center">Примечание</td>
								<td class="main dontprint" align="center">Фото</td>
							</tr>
							<xsl:for-each select="tracks">
								<tr>
									<th class="main" align="right" colspan="14">
										<xsl:value-of select="@trackinfo" />
									</th>
								</tr>
								<xsl:for-each select="elements">
									<tr>
										<xsl:variable name ="fileId" select="@fileId"/>
										<xsl:variable name ="Ms" select="@Ms"/>
										<xsl:variable name ="fNum" select="@fNum"/>
										<td class="main" align="center">
											<xsl:value-of select="@n" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@pchu" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@station" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@km" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@piket" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@meter" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@speed" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@digression" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@thread" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@amount" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@fastening" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@tripplan" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@speed2" />
										</td>
										<td class="main" align="center">
											<xsl:value-of select="@notice" />
										</td>
										<td class="main dontprint" align="center">
											<xsl:value-of select="@notice" />
											<button style="background-color:green" onClick="getImage({$fileId},{$Ms},{$fNum})">Смотреть фото</button>
										</td>
									</tr>
								</xsl:for-each>
							</xsl:for-each>
						</table>
						<table style="font-size: 12px; width: 90%;height: 5%; margin: auto;">
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
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
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