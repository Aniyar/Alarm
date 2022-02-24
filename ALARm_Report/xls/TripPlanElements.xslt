<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
    <xsl:output method="html"/>
    <xsl:template match="/">
        <html>
            <head>
                <title>Ведомость элементов плана пути</title>
                <style>                  
                    .pages {	

                            page-break-after: always;			
                            }           
                                            table.main {    
                                            
                                        border-collapse: collapse;   
                                                margin: auto;        
                                                width: 100%;           
                                    
                                        }               
                                            table.main, td.main, th.main {       
                                                            
                                                    font-size: 14px;    
                                                    font-family: 'Times New Roman';
                                                    border: 1.5px solid black;  
                                            }            
                </style>
            </head>
            <body>
                <xsl:for-each select="report/pages">
                    <div class="pages">


                        <p align="left" style="color:black;width: 105%;height: 1%;font-size: 10px;">
                            <xsl:value-of select="@version" />
                        </p>
                         <table width="100%">
                   
                        <th align="center" style="color:black; font-size:16px">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Ведомость элементов плана пути</th>
                        <th align="right" style="color:black; font-size:14px"> ДФ-Пл</th>
                    
                    </table>
                        
                        <table style="font-size: 14px; width: 100%;border-collapse: collapse; margin:auto;margin-bottom:8px;height: 1% ">

                            <tr>
                                <td align="left">
                                ПЧ:
                                        <xsl:value-of select="@distance" />
                                           &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                                  Дорога:
                                    <xsl:value-of select="@road" />
                               
                                </td>
                            </tr>
                           
                            <tr>
                                <td align="left">
                                
                                        <xsl:value-of select="@car" />
                                       &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                                     &#160;&#160;&#160;&#160;&#160;
                                        Поверка:                               
                                        <xsl:value-of select="@trip_info" />
                                           &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                                  &#160;&#160;&#160;&#160;&#160;
                                    <xsl:value-of select="@period" />

                                </td>
                            </tr>
                            <!-- <tr>

                                <td align="left">ПЧ:
                                    <xsl:value-of select="@distance" />
                                </td>
                                <td align="left">Дорога:
                                    <xsl:value-of select="@road" />
                                </td>

                            </tr>
                            <tr>
                                <td align="left">
                                    <xsl:value-of select="@car" />
                                </td>
                                <td align="left">Поверка:
                                    <xsl:value-of select="@trip_info" />
                                </td>
                                <td align="left">
                                    <xsl:value-of select="@period" />
                                </td>
                            </tr> -->
                        </table>
                        <table class="main">
                            <thead>
                                <tr>
                                    <td class="main" align="center" colspan="2">Точки поворота оси пути</td>
                                    <td class="main" align="center" rowspan="2">Угол по-<br/>ворота (гр)</td>
                                    <td class="main" align="center" colspan="2">Элемент плана</td>
                                    <td class="main" align="center" rowspan="2">Макс отклон.<br/> на элементе (см)</td>
                                </tr>
                                <tr>
                                    <td class="main" align="center">Начало (км.м)</td>
                                    <td class="main" align="center">Конец (км.м)</td>
                                    <td class="main" align="center">Длина (км)</td>
                                    <td class="main" align="center">Характеристики</td>
                                </tr>
                            </thead>

                            <tr>
                                <th class="main" align="right" colspan="9">
                                    <xsl:value-of select="@tripinfo" />
                                </th>
                            </tr>
                            <xsl:for-each select="elements">
                                <tr>
                                    <td class="main" align="center">
                                        <xsl:value-of select="@start" />
                                    </td>
                                    <td class="main" align="center">
                                        <xsl:value-of select="@final" />
                                    </td>
                                    <td class="main" align="center">
                                        <xsl:value-of select="@angle" />
                                    </td>
                                    <td class="main" align="center">
                                        <xsl:value-of select="@length" />
                                    </td>
                                    <td class="main" align="center">
                                        <xsl:value-of select="@characteristics" />
                                    </td>
                                    <td class="main" align="center">
                                        <xsl:value-of select="@deviation" />
                                    </td>
                                </tr>
                            </xsl:for-each>

                        </table>
                        <table style="font-size: 12px; width: 100%;height: 5%; margin: auto;">
                            <tr>
                                <td align="left">   Начальник                                    <xsl:value-of select="@car" />
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