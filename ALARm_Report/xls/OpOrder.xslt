<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/report">
		<html>
			<head>
				<title>Журнал оперативных приказов</title>
				<style>
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
					margin: auto;
					width: 90%;
					border: 1px solid black;

					}
					h1.main {
					page-break-before: always;
					}
					thead {
					background: #f5e8d0;
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
					<b>
						<p align="center" style="color:black; font-size:15px">Журнал оперативных приказов по учету и контролю устранения отступлений 3,4 степени и отступлений, повлекших за собой выдачу                            ограничений скорости поездов</p>
					</b>
					<table style="width:90%" align="center">
						<tr>
							<td>
								<!--   ПЧ:  -->
								<xsl:value-of select="@distance" />
							</td>
							<td>
								Дорога:                                <xsl:value-of select="@road" />
							</td>
						</tr>
						<tr>
							<td>
								ПС:                                <xsl:value-of select="@ps" />
							</td>
							<td>
								Проверка:                                <xsl:value-of select="@check" />
							</td>
							<td>
								<xsl:value-of select="@periodDate" />
							</td>
						</tr>
					</table>

					<table align="center" class="main">
						<thead>
							<tr  align="center">
								<th class="main" rowspan="2">№ ПД</th>
								<th class="main" rowspan="2">№ пути</th>
								<th class="main" rowspan="2">Км</th>
								<th class="main" rowspan="2">Пикет</th>
								<th class="main" rowspan="2">Метр</th>
								<th class="main" colspan="4">Отступление</th>
								<th class="main" rowspan="2">Ограничение скорости, км/ч</th>
								<th class="main" rowspan="2">Повторы</th>
								<th class="main" rowspan="2">Срок устранения</th>
								<th class="main" rowspan="2">Ответственный за устранение</th>
								<th class="main" rowspan="2">Дата устранени</th>
								<th class="main" colspan="3">Подпись</th>
								<th class="main" rowspan="2">Примечание</th>
							</tr>

							<tr>
								<th class="main">Наименование</th>
								<th class="main">Степень</th>
								<th class="main">Величина</th>
								<th class="main">Протяженность, м</th>
								<th class="main">Кто принял</th>
								<th class="main">от ПД</th>
								<th class="main">от ПЧ</th>
							</tr>

							<tr>
								<th class="main">1</th>
								<th class="main">2</th>
								<th class="main">3</th>
								<th class="main">4</th>
								<th class="main">5</th>
								<th class="main">6</th>
								<th class="main">7</th>
								<th class="main">8</th>
								<th class="main">9</th>
								<th class="main">10</th>
								<th class="main">11</th>
								<th class="main">12</th>
								<th class="main">13</th>
								<th class="main">14</th>
								<th class="main">15</th>
								<th class="main">16</th>
								<th class="main">17</th>
								<th class="main">18</th>
							</tr>
						</thead>
						<xsl:for-each select="direction">
							<tr>
								<th align="left" class="main" colspan="18">
									Направление:<xsl:value-of select="@TravelDirection" />
									/                                    <xsl:value-of select="../@ps" />
									/                                    <xsl:value-of select="@date" />
								</th>
							</tr>

							<xsl:for-each select="note">
								<tr class="tr" align="center">
									<td class="main">
										<xsl:value-of select="@NPd" />
									</td>
									<td class="main">
										<xsl:value-of select="@NWay" />
									</td>
									<td class="main">
										<xsl:value-of select="@km" />
									</td>
									<td class="main">
										<xsl:value-of select="@p" />
									</td>
									<td class="main">
										<xsl:value-of select="@m" />
									</td>
									<td class="main">
										<xsl:value-of select="@name" />
									</td>
									<td class="main">
										<xsl:value-of select="@Power" />
									</td>
									<td class="main">
										<xsl:value-of select="@Value" />
									</td>
									<td class="main">
										<xsl:value-of select="@Length" />
									</td>
									<td class="main">
										<xsl:value-of select="@SpeedLimit" />
									</td>
									<td class="main">
										<!--<xsl:value-of select="@Repetitions" />-->
									</td>
									<td class="main">
										<!--<xsl:value-of select="@TermOfElimination" />-->
									</td>
									<td class="main">
										<!--<xsl:value-of select="@ResponsibleForElimination" />-->
									</td>
									<td class="main">
										<!--<xsl:value-of select="@EliminationDate" />-->
									</td>
									<td class="main">
										<!--<xsl:value-of select="@WhoAccepted" />-->
									</td>
									<td class="main">
										<!--<xsl:value-of select="@PD" />-->
									</td>
									<td class="main">
										<!--<xsl:value-of select="@PCh" />-->
									</td>
									<td class="main">
										<xsl:value-of select="@Note" />
									</td>
								</tr>
							</xsl:for-each>
						</xsl:for-each>
					</table>

					<table style="width:90%" align="center">
						<tr>
							<td>
								Начальник путеизмерителя:                                <xsl:value-of select="@ps" />
							</td>
							<td>
								<xsl:value-of select="@chief" />
							</td>
						</tr>
					</table>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>