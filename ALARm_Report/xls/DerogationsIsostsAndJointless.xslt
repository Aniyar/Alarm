<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>

			<head>
				<title>
					Ведомость отступлений на изостыках и
					бесстыковом пути
				</title>
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
					<div style="page-break-before:always;">
						<b>
							<p align="left" style="color:black; font-size:12px">
								<xsl:value-of select="@version" />
							</p>
						</b>
						<b>
							<p align="center" style="color:black; font-size:15px">
								Ведомость отступлений на изостыках и
								бесстыковом пути
							</p>
						</b>
						<table style="width:90%;font-size:13px" align="center">
							<tr>
								<th align="left">
									ДК №: <xsl:value-of select="@ps" />
								</th>
								<th align="left">
									Ведомость от: <xsl:value-of select="@date_statement" />
								</th>
								<th align="left">
									Поездка: <xsl:value-of select="@trip_date" />
								</th>
								<th align="left">
									ПЧ: <xsl:value-of select="@distance" />
								</th>



							</tr>
							<tr>
								<th align="left">
									Участок:<xsl:value-of select="@roadd" />
								</th>
								<th align="left">
									Направление:<xsl:value-of select="@direction" />
								</th>
								<th align="left">
									Путь: <xsl:value-of select="@track" />
								</th>
								<th align="left">
									Км: <xsl:value-of select="@km" />
								</th>
							</tr>
						</table>
						<table width="90%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<tr>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Км</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Метр</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Отступление</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Амплитуда</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Длина, м</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Степень</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">
										Скорость <br />
										установленная, км/ч
									</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">
										Допустимая <br />
										скорость,км/ч
									</th>
									<th class="border-head" style="padding:2.5px;" rowspan="1">Объект</th>
								</tr>
							</thead>
							<!-- начало записи-->
							<xsl:for-each select="direction/track/PCHU/PD/PDB/NOTE">
								<tr class="tr">
									<td>
										<xsl:value-of select="@km" />
									</td>
									<td>
										<xsl:value-of select="@meter" />
									</td>
									<td>
										<xsl:value-of select="@digression" />
									</td>
									<td>
										<xsl:value-of select="@value" />
									</td>
									<td>
										<xsl:value-of select="@length" />
									</td>
									<td>
										<xsl:value-of select="@typ" />
									</td>
									<td>
										<xsl:value-of select="@fullSpeed" />
									</td>
									<td>
										<xsl:value-of select="@allowSpeed" />
									</td>
									<td>
										<xsl:value-of select="@norma" />
									</td>
								</tr>
							</xsl:for-each>
						</table>
						<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
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