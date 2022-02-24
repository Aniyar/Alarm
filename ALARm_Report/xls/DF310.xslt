<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>ДФ-3.10 - Ведомость участков с вертикальным износом рельсов  </title>
				<style>
					<!-- .pages {	

                            page-break-after: always;			
                            }  
                    td {
                        padding-left: 5px;
                        text-align:center;
                    }
					th.vertical{
						transform: rotate(-90deg);
						} 
                     table.main, td.main, th.main {       
                                                            
                                                    font-size: 12px;    
                                                    font-family: 'Times New Roman';
                                                    border: 1.5px solid black;  
                                            }             -->
					td {
					padding-left: 5px;
					}

					.pages {
					page-break-after: always;
					}

					table.main {
					border-collapse: collapse;
					margin: auto;
					width: 100%;
					}

					table.main,
					td.main,
					th.main {
					font-size: 11px;
					font-family: 'Times New Roman';
					border: 1.5px solid black;
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
				</style>
			</head>
			<body>
				<xsl:for-each select="report/trip">
					<div id="pageFooter"  align="right" style = "page-break-before:always;">

						<p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
							<xsl:value-of select="@version" />
						</p>

						<table width="100%">

							<th align="center" style="color:black; font-size:15px">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Ведомость участков с вертикальным износом рельсов</th>
							<th align="right" style="color:black; font-size:15px"> (ДФ-3.10)</th>

						</table>
						<div align="center">
							<table style="width:100%;font-size: 12px" align="center"  >
								<tr>
									<td align="left">
										ПЧ:<xsl:value-of select="@pch" />
									</td>
									<td align="left">
										Дорога:<xsl:value-of select="@road" />
									</td>
								</tr>
								<tr>
									<td align="left">
										<xsl:value-of select="@ps" />
									</td>
									<td align="left">
										Проверка:
										<xsl:value-of select="@check" />  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;  <xsl:value-of select="@period" />
									</td>


								</tr>
							</table>
						</div>
						<div align="center">
							<table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center"   style="font-size: 12px;border-collapse: collapse;" >
								<thead>
									<tr>
										<td  align="center" rowspan="2">№</td>
										<td align="center" rowspan="2">
											Кривая <br/>Начало - Конец (км.м)
										</td>
										<td align="center" rowspan="2">
											Рельс<br/> (Л/Пр)
										</td>
										<td align="center" rowspan="2">V уст</td>
										<td  align="center" rowspan="1" colspan="2">Вертикальный износ(мм) </td>
										<td align="center" rowspan="1" colspan="3">Длина участка с износом более (м)</td>
									</tr>
									<tr>
										<td align="center" rowspan="1" >сред.</td>
										<td align="center" rowspan="1">макс.</td>

										<td align="center"  rowspan="1">4 мм</td>
										<td align="center" rowspan="1">10 мм</td>
										<td  align="center" rowspan="1">13 мм</td>
									</tr>

								</thead>
								<xsl:for-each select="./data/Pch">
									<xsl:for-each select="Put">
										<tr>
											<th class="info" align="right"  colspan="10">
												<!-- &#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;
                                        &#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;
                                        &#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195;&#8195; -->
												<xsl:value-of select="//@napr"/>
												<xsl:text>&#8195;Путь : </xsl:text>
												<xsl:value-of select="./@nput"/>
											</th>

										</tr>

										<xsl:for-each select="Prop">
											<tr>
												<td align="center">
													<xsl:value-of select="@iD"/>
												</td>
												<td align="center">
													<xsl:value-of select="@kM_a"/>
												</td>
												<td align="center">
													<xsl:value-of select="@rels"/>
												</td>
												<td align="center">
													<xsl:value-of select="@v"/>
												</td>
												<td align="center">
													<xsl:value-of select="@sred"/>
												</td>
												<td align="center">
													<xsl:value-of select="@max"/>
												</td>
												<td align="center">
													<xsl:value-of select="@fourMM"/>
												</td>
												<td align="center">
													<xsl:value-of select="@tenMM"/>
												</td>
												<td align="center">
													<xsl:value-of select="@thertyMM"/>
												</td>
											</tr>
										</xsl:for-each>
									</xsl:for-each>
								</xsl:for-each>
							</table>
						</div>
						<table style="font-size: 12px; width: 90%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник                                    <xsl:value-of select="@ps" />
								</td>
								<td align="right">
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