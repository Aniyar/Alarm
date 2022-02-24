<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html" />
	<xsl:template match="/">
		<html>

			<head>
				<title>Ведомость негодных скреплений</title>
				<style>

					table.main {
					border-collapse: collapse;
					margin: auto;
					width: 100%;
					}

					table.main,
					td.main,
					th.main {
					font-size: 12px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
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
					#pageFooter:before {

					counter-increment: page;
					content:"Страница "  counter(page);
					left: 100%;
					top: 100%;
					white-space: nowrap;
					z-index: 20;
					-moz-border-radius: 5px;
					-moz-box-shadow: 0px 0px 4px #222;
					background-image: -moz-linear-gradient(top, #eeeeee, #cccccc);
					}
				</style>
				<script src="/axios.min.js">//</script>
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
						<div id="menu">
							<div>
								<button id="edit-button">Изменить</button>
								<!-- <button id="delete-button">Удалить</button> -->
							</div>
						</div>
					</div>
				</div>
				<!-- Custom HTML footer -->
				<xsl:for-each select="report/pages">
					<div class="pages" align="center">


						<table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;">
							<tr>
								<td>
									<b>
										<p align="left" style="color:black; font-size:14px">Ведомость негодных скреплений</p>
									</b>
								</td>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@road" /> ЖД
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
								<td align="left">
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
					</div>
					<div align="center">
						<table class="main">
							<thead>
								<tr>
									<td class="main" align="center">№ п/п</td>
									<td class="main" align="center">ПЧУ, ПД, ПДБ</td>
									<td class="main" align="center">Перегон, станция</td>
									<td class="main" align="center">КМ</td>
									<td class="main" align="center">ПК</td>
									<td class="main" align="center">М</td>
									<td class="main" align="center">Отступление</td>
									<td class="main" align="center">Нить</td>
									<td class="main" align="center">Скрепление</td>
									<td class="main" align="center">Примечание</td>

									<!--<td class="main" align="center">Фото</td> -->
								</tr>
							</thead>

							<xsl:for-each select="tracks">
								<div >
									<tr>
										<th class="main" align="right" colspan="10">
											<xsl:value-of select="@trackinfo" />
										</th>
									</tr>
									<xsl:for-each select="elements">

										<xsl:variable name="CarPosition" select="@CarPosition" />
										<xsl:variable name="repType" select="@repType" />
										<xsl:variable name="fileId" select="@fileId" />
										<xsl:variable name="Ms" select="@Ms" />
										<xsl:variable name="fNum" select="@fNum" />
										<tr>
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
												<xsl:value-of select="@mtr" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@otst" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@threat_id" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@fastening" />
											</td>
											<td class="main" align="center">
												<xsl:value-of select="@notice" />
											</td>

											<td id="this" class="main dontprint button" align="center">
												<button class="dontprint button"
												  onClick="getImage({$CarPosition}, {$fileId}, {$Ms}, {$fNum}, {$repType})">
													Смотреть
													фото
												</button>
											</td>
										</tr>

									</xsl:for-each>
								</div>
							</xsl:for-each>

						</table>
						<table style="font-size: 12px; width: 95%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@ps" />
								</td>

								<td align="right">
									<xsl:value-of select="@chief" />
								</td>

							</tr>
						</table>
					</div>
				</xsl:for-each>
				<div id="pageFooter">
				</div>
			</body>

		</html>
	</xsl:template>
</xsl:stylesheet>