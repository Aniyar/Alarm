<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title> Экран сопровождения</title>
				<style>
					td {
					padding-left: 1px;
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

			</head>
			<body>
				<xsl:for-each select="report/trip">

					<div style = "page-break-before:always;">

						<p align="left" style="color:black; font-size:10px">
							<xsl:value-of select="@version" />
						</p>

						<b>
							<p align="center" style="color:black; font-size:15px">Экран сопровождения</p>
						</b>

						<table style="width:90%" align="center">
							<tr>
								<td>
									Дорога:
									<xsl:value-of select="@road" />
								</td>

								<td>
									Проверка:
									<xsl:value-of select="@check" />
								</td>
								<td>
									<xsl:value-of select="@periodDate" />
								</td>
							</tr>



							<!-- <tr>
                                <td>                                Дорога:                                    <xsl:value-of select="@road" />
                                </td>
                                <td>                                Проверка:                                    <xsl:value-of select="@check" />
                                </td>
                                <td>
                                    <xsl:value-of select="@periodDate" />
                                </td>
                            </tr> -->
						</table>
						<table style="width:90%" align="center" border="1" cellspacing="0" cellpadding="5">
							<tr class="tr" align="center">
								<th style="padding:2.5px; width: 10%;">ПЧ</th>
								<th style="padding:2.5px; width: 70%;"></th>
								<th style="padding:2.5px; width: 20%;">Всего</th>
							</tr>
							<xsl:for-each select="escort">
								<tr>
									<td>
										<xsl:value-of select="@distance" />
									</td>
									<td>
										<xsl:value-of select="@fullname" />
									</td>
									<td>
										<xsl:value-of select="@count" />
									</td>

								</tr>

							</xsl:for-each>
						</table>
						<table style="width:90%" align="center" border="0" cellspacing="0" cellpadding="5">
							<tr>
								<td>
									Начальник путеизмерителя                                    <xsl:value-of select="@ps" />
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