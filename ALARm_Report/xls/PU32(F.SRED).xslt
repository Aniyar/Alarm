<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<xsl:for-each select="report">
			<html>

				<head>
					<title> Средний балл по участкам </title>

					<style>
						table {
						border-collapse: collapse;
						}

						td {
						padding-left: 5px;
						}

						#pageFooter:before {
						counter-increment: page;
						content: "Страница"counter(page) "из"counter(page);
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
				</head>

				<body>

					<div align="right" id="pageFooter" style="text-align: right;page-break-before:always;">
						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<H4 align="center">
							Средний балл по участкам
						</H4>

						<table style="width:100%" align="center">
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
									<xsl:value-of select="@car" />
								</td>

								<td align="left">
									Проверка:
									<xsl:value-of select="@triptype" />
								</td>

								<td align="left">
									<xsl:value-of select="@month" />
								</td>

							</tr>
						</table>

						<table width="100%" border="1" cellpadding="0" cellspacing="0" class="border" align="center">
							<thead>
								<tr align="center">
									<td style="padding:2px; width:5%;" rowspan="2">ПЧ</td>
									<td style="padding:2px; width:5%;" rowspan="2">ПЧУ</td>
									<td style="padding:2px; width:5%;" rowspan="2">
										Ср.балл <br />
										Nуч/Оценка
									</td>
									<td style="padding:2px; width: 85%;" colspan="9">№ отделения ПД</td>
								</tr>
								<tr>
									<td align="center" style="padding:2px; width: 85%;" colspan="9">Ср.балл/Оценка</td>
								</tr>
							</thead>


							<!-- <table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
                            <thead>

                                <tr>
                                    <td colspan="1" style="width:10%">ПД Ср.балл / Оценка </td>
                                    <td colspan="9">
                                        Номера отделений
                                        <br />
                                        Средний балл / Оценка
                                    </td>
                                </tr>
                            </thead> -->


							<xsl:for-each select="bykilometer/section/pchu">
								<tr>
									<td style="text-align: center; width:5% " rowspan="2" colspan="1">
										<xsl:value-of select="@pch" />
									</td>

									<td style="text-align: center; width:5%   " rowspan="2" colspan="1">
										<xsl:value-of select="pd/@pchu" />
									</td>

									<td style="text-align: center;" rowspan="2">
										Ср.балл -
										<xsl:value-of select="@sperdball" /><br />
										&#160;&#160;&#160;

										<xsl:value-of select="@rating" />
									</td>

									<xsl:for-each select="pd">
										<td style="width:10%;text-align: center;" rowspan="1">
											<xsl:value-of select="@code" />
										</td>
									</xsl:for-each>
									<tr>
										<xsl:for-each select="pd">

											<td rowspan="1" style="width:10%;text-align: center;">

												<xsl:value-of select="@avgLine" />
											</td>
										</xsl:for-each>
									</tr>
									<!-- <xsl:if test="9 - count(pdb)>0">
                                        <td style="width:10%">
                                            <xsl:attribute name="colspan">
                                                <xsl:value-of select="9 - count(pdb)" />
                                            </xsl:attribute>
                                        </td>
                                    </xsl:if> -->
									<xsl:if test="9 - count(pdb)>0">
										<td style="width:10%">
											<xsl:attribute name="colspan">
												<xsl:value-of select="9 - count(pdb)" />
											</xsl:attribute>
										</td>
									</xsl:if>

								</tr>

							</xsl:for-each>

						</table>
						<table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;
									<xsl:value-of select="@car" />
								</td>

								<td>
									<xsl:value-of select="@chief" />
								</td>
							</tr>
						</table>
					</div>

				</body>

			</html>
		</xsl:for-each>

	</xsl:template>
</xsl:stylesheet>


<!-- table.main,
                        td.main {
                            font-size: 12px;
                            font-family: 'Times New Roman';
                            border: 1.5px solid black;
                            margin: auto;
                            margin-bottom: 8px;
                            border-collapse: collapse;
                            padding: 5px;

                        }

                        tr {
                            padding: 5px;
                        }

                        td {
                            padding: 5px;
                            text-align: center;
                            vertical-align: middle;

                        }

                        td.vertical {}

                        .main {
                            text-align: left;
                            vertical-align: middle;
                        }

                        .time-col {
                            position: relative;
                            writing-mode: vertical-rl;
                            transform: rotate(180deg);
                        }

                        .col {
                            position: relative;
                            overflow: visible;
                        }

                        span {
                            white-space: pre-wrap;
                            display: inline-table;
                        }

                        .rotate {
                            text-align: center;
                            white-space: nowrap;
                            vertical-align: middle;
                            width: 1.5em;
                            height: 8em;
                            padding: 5px;
                        }

                        .rotate div {
                            -moz-transform: rotate(-90.0deg);
                            /* FF3.5+ */
                            -o-transform: rotate(-90.0deg);
                            /* Opera 10.5 */
                            -webkit-transform: rotate(-90.0deg);
                            /* Saf3.1+, Chrome */
                            filter: progid:DXImageTransform.Microsoft.BasicImage(rotation=0.083);
                            /* IE6,IE7 */
                            -ms-filter: "progid:DXImageTransform.Microsoft.BasicImage(rotation=0.083)";
                            /* IE8 */
                            margin-left: -10em;
                            margin-right: -10em;
                        }
                        .tab4 {
                            tab-size: 8;
                            font-size: 15px; 
                            font-family: 'Times New Roman';
                            width: 100%; 
                            margin: auto;
                            margin-bottom:8px;
                            border-collapse: collapse;
                            padding: 5px
                        } -->