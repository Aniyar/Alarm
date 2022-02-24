<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>Список сопровождающих</title>
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

			</head>
			<body>
				<xsl:for-each select="report/trip">
					<div   align="right" id="pagefooter" style = "page-break-before:always;">

						<p  align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>
						<H4 align = "center">Список сопровождающих</H4>


						<table style="font-size: 14px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
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
						<table style="width:100%" align="center" border="1" cellspacing="0" cellpadding="5">
							<tr align="center">
								<td style="padding:2.5px; width: 20%;" >Дата</td>
								<td style="padding:2.5px; width: 20%;" >Направление, участок</td>
								<td style="padding:2.5px; width: 60%;"> Сопровождающие</td>
							</tr>

							<xsl:for-each select="escort">
								<!-- <tr>
                               <td align="left" style="padding:2.5px;" colspan="3">ПЧ: <xsl:value-of select="@distance" /> </td>
                            </tr>  -->


								<tr>
									<td>
										<xsl:value-of select="@tripdate" />
									</td>
									<td>
										<xsl:value-of select="@sector" />
									</td>
									<td>
										<xsl:value-of select="@fullname" />
									</td>

								</tr>

							</xsl:for-each>
						</table>
						<table style="width:100%;height: 5%;" align="center" border="0" cellspacing="0" cellpadding="5">
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