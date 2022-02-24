<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    version="1.0" exclude-result-prefixes="msxsl">
    <xsl:template match="/">
        <html>

        <head>
            <title>
                Карточка кривой на скоростных линиях (ФП-3.2)
            </title>

            <style>
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
                table.main,
                td.main {
                    border-collapse: collapse;
                    margin: auto;
                    width: 680px;
                    border: 1px solid black;
                    font-size: 9px;
                    font-family: Arial;
                }

                h1.main {
                    page-break-before: always;
                }

                b {
                    font-size: 14px;
                    font-family: Arial;
                }

                .curve_chart {
                    width: 620px;
                    top: 0;
                    right: 0;
                    bottom: 0;
                    left: 0;
                    margin: auto;
                    height: 116px;
                }

                svg {
                    border: 1px solid;
                    height: 115px;
                    width: 620px;
                    preserveAspectRatio: none;
                }

                .zerotext {
                    top: -25px;
                    left: -32px;
                    position: relative;
                    text-align: right;
                    width: 30px;
                }

                .titletext {
                    top: -135px;
                    left: 10px;
                    position: relative;
                    text-align: left;
                    font-size: 9px;
                    font-family: Arial;
                }

                .xaxis {
                    vector-effect: non-scaling-stroke;
                    stroke: rgb(128, 128, 128);
                    stroke-width: 1;
                    stroke-dasharray: 4, 2;
                }

                .yaxis {
                    vector-effect: non-scaling-stroke;
                    stroke: red;
                    stroke-width: 1;
                    stroke-dasharray: 4, 2;
                }

                .rectangles {
                    vector-effect: non-scaling-stroke;
                    fill: red;
                    stroke: red;
                    stroke-width: 1;
                }
            </style>
        </head>

        <body>
            <xsl:for-each select="report/curve">
                <xsl:choose>
                    <xsl:when test="@ismulti">
                        <div align="center" style="page-break-before:always; margin:10px 0px 0px">

                            <b>Карточка многорадиусной кривой (ФП-3.3)</b>
                        </div>
                    </xsl:when>
                    <xsl:otherwise>
                        <div id="pageFooter"    style="page-break-before:always; text-align: right; margin:0px 0px 0px;page">
                            <p align="left" style=" color: black;
                                                    width: 98%;
                                                    height: 1%;
                                                    font-size: 10px;">
                                <xsl:value-of select="@version" />
                            </p>
                        </div>

                        <div align="center" style="margin:0px 0px 0px">
                            <b>Карточка кривой (ФП-3.2)</b>
                            <br/>
                            на скоростных линиях
                        </div>
                    </xsl:otherwise>
                </xsl:choose>
                <div align="center">
                    <table style="font-size: 9px;font-family: Arial; width: 98%;">
                        <tr>
                            <td align="left">
                                ПЧ:
                                <xsl:value-of select="../@distance" />
                            </td>

                            <td align="left">
                                Дорога:
                                <xsl:value-of select="@road" />
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <xsl:value-of select="@PC" />
                            </td>
                            <td align="left">Проверка:
                                <xsl:value-of select="@type" />
                            </td>
                            <td align="left">
                                <xsl:value-of select="@period" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div align="center">
                    <table class="main">
                        <tr>
                            <td colspan="2" rowspan="4" align="center" class="main">
                                <xsl:value-of select="@side" />
                                <br/>
                                <xsl:value-of select="@order" />
                            </td>
                            <td colspan="9" align="center" class="main">
                                Направление:
                                <xsl:value-of select="@direction" />
                            </td>
                            <td colspan="8" align="center" class="main">
                                Путь:
                                <xsl:value-of select="@track" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="9" align="center" class="main">
                                Характеристики кривой
                            </td>
                            <td class="main" style="border-right: 0;" colspan="2" align="center">
                                1-й
                            </td>
                            <td colspan="2" align="center" class="main" style="border-left: 0; border-right: 0;">
                                переходные
                            </td>
                            <td class="main" style="border-left: 0;" colspan="2" align="center">
                                2-й
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" class="main">Начало</td>
                            <td colspan="1" align="center" class="main">разн</td>
                            <td colspan="2" align="center" class="main">Конец</td>
                            <td colspan="1" align="center" class="main">разн</td>
                            <td colspan="1" align="center" class="main">Дл</td>
                            <td colspan="1" align="center" class="main">разн</td>
                            <td colspan="1" align="center" class="main">угол</td>

                            <td colspan="1" align="center" class="main">макс</td>
                            <td colspan="1" align="center" class="main">ср</td>
                            <td colspan="1" align="center" class="main">дл</td>
                            
                            <td colspan="1" align="center" class="main">макс</td>
                            <td colspan="1" align="center" class="main">ср</td>
                            <td colspan="1" align="center" class="main">дл</td>
                        </tr>
                        <tr>
                            <td align="center" class="main">км</td>
                            <td align="center" class="main">м</td>
                            <td align="center" class="main">м</td>

                            <td align="center" class="main">км</td>
                            <td align="center" class="main">м</td>
                            <td align="center" class="main">м</td>

                            <td align="center" class="main">м</td>
                            <td align="center" class="main">м</td>
                            <td align="center" class="main">град</td>

                            <td align="center" class="main">мм/м</td>
                            <td align="center" class="main">мм/м</td>
                            <td align="center" class="main">мм/м</td>

                            <td align="center" class="main">мм/м</td>
                            <td align="center" class="main">мм/м</td>
                            <td align="center" class="main">мм/м</td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" class="main">план</td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@start_km" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@start_m" />
                            </td>
                            <td class="main"></td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@final_km" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@final_m" />
                            </td>
                            <td class="main"></td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@len" />
                            </td>
                            <td class="main"></td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="param_curve/@angle" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_max1" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_mid1" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_len1" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_max2" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_mid2" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_len2" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" class="main">уровень</td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@start_lvl_km" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@start_lvl_m" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@razn1" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@final_lvl_km" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@final_lvl_m" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@razn2" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@len_lvl" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_curve/@razn3" />
                            </td>
                            <td colspan="1" class="main" />
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_max1_lvl" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_mid1_lvl" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_len1_lvl" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_max2_lvl" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_mid2_lvl" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="withdrawal/@tap_len2_lvl" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" />
                            <td colspan="10" align="center" class="main">
                                Характеристики однорадиусной кривой
                            </td>
                            <!-- <td colspan="2" align="center" class="main">
                                A <sub >нп </sub>
                            </td>
                            <td align="center" class="main">Ψ</td> -->
                            <td align="center" class="main">Тип</td>
                            <td colspan="1" align="center" class="main">Сапсан</td>
                            <td colspan="1" align="center" class="main">Ласточка</td>
                            <td colspan="2" align="center" class="main">Стриж</td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right" />
                            <td colspan="2" align="center" class="main">Начало</td>
                            <td colspan="1" align="center" class="main">разн</td>
                            <td colspan="2" align="center" class="main">Конец</td>
                            <td colspan="1" align="center" class="main">разн</td>
                            <td colspan="1" align="center" class="main">Дл</td>
                            <td colspan="3" align="center" class="main">
                                Радиус/Уровень
                            </td>
                            <!-- <td colspan="2" align="center" class="main">
                                <xsl:value-of select="computing/@a1" />
                                /
                                <xsl:value-of select="computing/@a2" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="computing/@psi1" />
                            </td> -->
                            <td align="center" class="main">V<sub>пз</sub>
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Saps1" />
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Last1" />
                            </td>
                            <td colspan="2" align="center" class="main">
                                <xsl:value-of select="speed/@Str1" />
                            </td>
                        </tr>
                        <tr>
                            <td rowspan="3" colspan="2" />
                            <td rowspan="3" align="center" class="main">км</td>
                            <td rowspan="3" align="center" class="main">м</td>
                            <td rowspan="3" align="center" class="main">м</td>

                            <td rowspan="3" align="center" class="main">км</td>
                            <td rowspan="3" align="center" class="main">м</td>
                            <td rowspan="3" align="center" class="main">м</td>

                            <td rowspan="3" align="center" class="main">м</td>

                            <td rowspan="3" align="center" class="main">мин</td>
                            <td rowspan="3" align="center" class="main">макс</td>
                            <td rowspan="3" align="center" class="main">ср</td>

                            <!-- <td colspan="2" align="center" class="main">
                                <xsl:value-of select="computing/@a3" />
                                /
                                <xsl:value-of select="computing/@a4" />
                            </td>
                            <td class="main" /> -->
                            <td align="center" class="main">
                                V<sub>кр</sub>
                            </td>

                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Saps2" />
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Last2" />
                            </td>
                            <td colspan="2" align="center" class="main">
                                <xsl:value-of select="speed/@Str2" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" class="main">
                                V<sub>пр</sub>
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Saps3" />
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Last3" />
                            </td>
                            <td colspan="2" align="center" class="main">
                                <xsl:value-of select="speed/@Str3" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" class="main">
                                V<sub>из</sub>
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Saps4" />
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Last4" />
                            </td>
                            <td colspan="2" align="center" class="main">
                                <xsl:value-of select="speed/@Str4" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" class="main">план</td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@start_km" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@start_m" />
                            </td>
                            <td align="center" class="main" />
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@final_km" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@final_m" />
                            </td>
                            <td align="center" class="main" />
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@len" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@rad_min" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@rad_max" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@rad_mid" />
                            </td>
                            <!-- <td colspan="2" align="center" class="main">
                                <xsl:value-of select="computing/@a5" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="computing/@psi2" />
                            </td> -->
                            <td align="center" class="main">V<sub>огр</sub>
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Saps5" />
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Last5" />
                            </td>
                            <td colspan="2" align="center" class="main">
                                <xsl:value-of select="speed/@Str5" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right" class="main">
                                уровень
                            </td>
                            <td align="center" class="main" >
                                <xsl:value-of select="param_circle_curve/@start_lvl_km" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@start_lvl_m" />
                            </td>
                            <td align="center" class="main" >
                                <xsl:value-of select="param_circle_curve/@razn1" />
                            </td>
                            <td align="center" class="main" >
                                <xsl:value-of select="param_circle_curve/@final_lvl_km" />
                            </td>
                            <td align="center">
                                <xsl:value-of select="param_circle_curve/@final_lvl_m" />
                            </td>
                            <td align="center" class="main" >
                                <xsl:value-of select="param_circle_curve/@razn2" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@len_lvl" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@lvl_min" />
                            </td>
                            <td align="center">
                                <xsl:value-of select="param_circle_curve/@lvl_max" />
                            </td>
                            <td align="center" class="main">
                                <xsl:value-of select="param_circle_curve/@lvl_mid" />
                            </td>
                            <!-- <td colspan="3" align="center" class="main">Р <sub>растр</sub>=
                                <xsl:value-of select="computing/@P" />
                            </td> -->
                            <td align="center" class="main">
                                А<sub>нп</sub>
                            </td>
                            
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Saps6" />
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Last6" />
                            </td>
                            <td colspan="2" align="center" class="main">
                                <xsl:value-of select="speed/@Str6" />
                            </td>
                        </tr>
                        
                        <tr>
                            <td rowspan="2" colspan="2" align="center" class="main">Бок. износ</td>
                            <td rowspan="2" colspan="3" align="center" class="main">&gt;6мм = 
                                <xsl:value-of select="side_wear/@mm6" />
                            </td>
                            <td rowspan="2" colspan="3" align="center" class="main">&gt;10мм = 
                                <xsl:value-of select="side_wear/@mm10" />
                            </td>
                            <td rowspan="2" colspan="2" align="center" class="main">&gt;15мм =
                                <xsl:value-of select="side_wear/@mm15" />
                            </td>
                            <td rowspan="2" align="center" class="main">
                                <xsl:value-of select="side_wear/@max" />
                            </td>
                            <td rowspan="2" align="center" class="main">
                                <xsl:value-of select="side_wear/@mid" />
                            </td>
                            <td>

                            </td>
                            <td colspan="4" align="center" class="main">
                                Величина / коорд
                            </td>                            
                        </tr>
                        <tr>
                            <td align="center" class="main">
                                Аг max
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Saps7" />
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Last7" />
                            </td>
                            <td colspan="2" align="center" class="main">
                                <xsl:value-of select="speed/@Str7" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="center" class="main">
                                Пикет с огр. V пз
                            </td>
                            <td colspan="8" align="center" class="main" />
                            <td align="center" class="main">
                                Ψ max
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Saps8" />
                            </td>
                            <td colspan="1" align="center" class="main">
                                <xsl:value-of select="speed/@Last8" />
                            </td>
                            <td colspan="2" align="center" class="main">
                                <xsl:value-of select="speed/@Str8" />
                            </td>
                        </tr>
                        <xsl:if test="@ismulti">
                            <tr>
                                <td colspan="2" rowspan="2" class="main" />
                                <td colspan="8" align="center" class="main">Характеристики
                                    элементарных кривых</td>
                                <td colspan="3" class="main" />
                                <td colspan="3" rowspan="2" class="main" />
                            </tr>
                            <tr>
                                <td colspan="2" align="center" class="main">Начало
                                </td>
                                <td colspan="2" align="center" class="main">Конец</td>
                                <td align="center" class="main">дл.</td>
                                <td align="center" class="main">ср.рад./уров.</td>
                                <td align="center" class="main">
                                    ср.отв.</td>
                                <td align="center" class="main">дл.</td>
                                <td colspan="2" align="center" class="main">A<sub>нп</sub>
                                </td>
                                <td align="center" class="main">Ψ</td>
                            </tr>
                            <xsl:for-each select="multicurves">
                                <tr>
                                    <td rowspan="2" valign="top" align="center" class="main">
                                        <xsl:value-of select="@order" />
                                    </td>
                                    <td align="right" class="main">план</td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@start_km" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@start_m" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@final_km" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@final_m" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@len" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@radius" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@midtap" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@len2" />
                                    </td>
                                    <td colspan="2" align="center" class="main">
                                        <xsl:value-of select="@anp" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@psi" />
                                    </td>
                                    <td align="center" class="main">
                                        V<sub>пз</sub>
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@pass1" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@frei1" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" class="main">уров.</td>
                                    <td class="main" />
                                    <td align="center" class="main">
                                        <xsl:value-of select="@start_lvl" />
                                    </td>
                                    <td class="main" />
                                    <td align="center" class="main">
                                        <xsl:value-of select="@final_lvl" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@len_lvl" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@lvl" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@midtap_lvl" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@len2_lvl" />
                                    </td>
                                    <td colspan="2" align="center" class="main">
                                        <xsl:value-of select="@anp2" />
                                    </td>
                                    <td class="main" />
                                    <td align="center" class="main">
                                        V<sub>дп</sub>
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@pass2" />
                                    </td>
                                    <td align="center" class="main">
                                        <xsl:value-of select="@frei2" />
                                    </td>
                                </tr>
                                <tr></tr>
                            </xsl:for-each>
                        </xsl:if>
                    </table>
                </div>
                <br />
                <div class="curve_chart">
                    <svg width="620px" preserveAspectRatio="none" vector-effect="non-scaling-stroke">
                        <xsl:attribute name="viewBox">
                            <xsl:value-of select="@viewbox" />
                        </xsl:attribute>
                        <xsl:for-each select="xaxis/line">
                            <line class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                                <xsl:attribute name="y1">
                                    <xsl:value-of select="@y1s" />
                                </xsl:attribute>
                                <xsl:attribute name="y2">
                                    <xsl:value-of select="@y2s" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                        <xsl:for-each select="xaxis/xparam">
                            <line y1="-30" y2="-30" class="yaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="0" y2="0" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="30" y2="30" class="yaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                        <xsl:for-each select="xaxis/rectangle">
                            <rect class="rectangles">
                                <xsl:attribute name="x">
                                    <xsl:value-of select="@x" />
                                </xsl:attribute>
                                <xsl:attribute name="y">
                                    <xsl:value-of select="@y" />
                                </xsl:attribute>
                                <xsl:attribute name="width">
                                    <xsl:value-of select="@width" />
                                </xsl:attribute>
                                <xsl:attribute name="height">
                                    <xsl:value-of select="@height" />
                                </xsl:attribute>
                            </rect>
                        </xsl:for-each>
                        <polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:black;stroke-width:1">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@radius" />
                            </xsl:attribute>
                        </polyline>
                        <polyline
                            style="vector-effect:non-scaling-stroke;fill:none; stroke-linejoin:round;stroke:green;stroke-width:1">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@radius-average" />
                            </xsl:attribute>
                        </polyline>
                        <polyline
                            style="vector-effect:non-scaling-stroke;fill:none; stroke-linejoin:round;stroke:red;stroke-width:1">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@radius-trapez" />
                            </xsl:attribute>
                        </polyline>
                    </svg>
                    <div style="{@radius-style0}">
                        <xsl:value-of select="@radius-val0" />
                    </div>
                     <div style="{@radius-style30}">
                        <xsl:value-of select="@radius-val30" />
                    </div>
                   
                </div>

                <div style="width: 620px;top: 0;right: 0;bottom: 0;left: 0;margin: auto;height: 18px;">
                    <svg preserveAspectRatio="none" vector-effect="non-scaling-stroke" viewBox="-10 0 968 25" style="border: 1px solid;height: 17px;width: 620px;">
                        <text x="0" y="18" style="font: bold 15px sans-serif;">Кривизна</text>
                    </svg>
                </div>

                <div class="curve_chart">
                    <svg preserveAspectRatio="none" vector-effect="non-scaling-stroke">
                        <xsl:attribute name="viewBox">
                            <xsl:value-of select="@viewbox-level" />
                        </xsl:attribute>
                        <xsl:for-each select="xaxis/line">
                            <line class="xaxis" y1="5">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                                <xsl:attribute name="y1">
                                    <xsl:value-of select="@y1-level" />
                                </xsl:attribute>
                                <xsl:attribute name="y2">
                                    <xsl:value-of select="@y2-level" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                        <xsl:for-each select="xaxis/xparam">

                            <line y1="100" y2="100" class="yaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="50" y2="50" class="yaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="0" y2="0" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-50" y2="-50" class="yaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-100" y2="-100" class="yaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                        <xsl:for-each select="xaxis/rectangle_lvl">
                            <rect class="rectangles">
                                <xsl:attribute name="x">
                                    <xsl:value-of select="@x" />
                                </xsl:attribute>
                                <xsl:attribute name="y">
                                    <xsl:value-of select="@y" />
                                </xsl:attribute>
                                <xsl:attribute name="width">
                                    <xsl:value-of select="@width" />
                                </xsl:attribute>
                                <xsl:attribute name="height">
                                    <xsl:value-of select="@height" />
                                </xsl:attribute>
                            </rect>
                        </xsl:for-each>
                        <polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:black;stroke-width:1">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@level" />
                            </xsl:attribute>
                        </polyline>
                        <polyline
                            style="vector-effect:non-scaling-stroke;fill:none; stroke-linejoin:round;stroke:green;stroke-width:1">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@level-average" />
                            </xsl:attribute>
                        </polyline>

                        <polyline
                            style="vector-effect:non-scaling-stroke;fill:none; stroke-linejoin:round;stroke:red;stroke-width:1">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@level-trapez" />
                            </xsl:attribute>
                        </polyline>
                    </svg>
                    
                    <div style="{@radius-style100l}">
                        <xsl:value-of select="@radius-val100l" />
                    </div>
                    <div style="{@radius-style50l}">
                        <xsl:value-of select="@radius-val50l" />
                    </div>
                    <div style="{@radius-style0l}">
                        <xsl:value-of select="@radius-val0l" />
                    </div>
                
                </div>

                <div style="width: 620px;top: 0;right: 0;bottom: 0;left: 0;margin: auto;height: 18px;">
                    <svg preserveAspectRatio="none" vector-effect="non-scaling-stroke" viewBox="-10 0 968 25" style="border: 1px solid;height: 17px;width: 620px;">
                        <text x="0" y="18" style="font: bold 15px sans-serif;">Возвышение (мм)</text>
                    </svg>
                </div>
                
                <div class="curve_chart">
                    <svg preserveAspectRatio="none" vector-effect="non-scaling-stroke">
                        <xsl:attribute name="viewBox">
                            <xsl:value-of select="@boost-level" />
                        </xsl:attribute>

                        <polyline style="fill: none;
                                         stroke: #2730d2;
                                         vector-effect: non-scaling-stroke;
                                         -linejoin: round;
                                         stroke-width: 2px;
                                         stroke-dasharray: 1 5 2;">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@lastochkaboost" />
                            </xsl:attribute>
                        </polyline>

                        <polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:red;stroke-width:2">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@passboost" />
                            </xsl:attribute>
                        </polyline>

                        <polyline style="fill: none;
                                         stroke: #2730d2;
                                         vector-effect: non-scaling-stroke;
                                         -linejoin: round;
                                         stroke-width: 2;
                                         stroke-dasharray: 0.5 0.5;">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@freightboost" />
                            </xsl:attribute>
                        </polyline>
                        
                        <!-- аг ласточка -->
                        <line y1="-0.95" y2="-0.95" class="" x1="0" x2="25" style="
                                fill: none;
                                stroke: #2730d2;
                                vector-effect: non-scaling-stroke;
                                -linejoin: round;
                                stroke-width: 2px;
                                stroke-dasharray: 1 5 2;">
                        </line>

                        <!-- аг пасс -->
                        <line y1="-0.75" y2="-0.75" class="" x1="0" x2="25" style="
                                vector-effect: non-scaling-stroke;
                                fill: none;
                                stroke: red;
                                stroke-width: 2;">
                        </line>

                        <!-- аг груз -->
                        <line y1="-0.55" y2="-0.55" class="" x1="0" x2="25" style="
                                fill: none;
                                stroke: #2730d2;
                                vector-effect: non-scaling-stroke;
                                                                    -
                                linejoin: round;
                                stroke-width: 2;
                                stroke-dasharray: 0.5 0.5;">
                        </line>
                        
                        <xsl:for-each select="xaxis/line">
                            <line y1="5" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                                <xsl:attribute name="y2">
                                    <xsl:value-of select="@y2" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                        <xsl:for-each select="xaxis/xparam">
                            <line y1="0" y2="0" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-0.3" y2="-0.3" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-0.7" y2="-0.7" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-0.9" y2="-0.9" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-1.1" y2="-1.1" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                    </svg>
                    <div style="top: -107px; left: 10px; position: relative; text-align: left; font-size: 9px; font-family: Arial;">Аг ласточка</div>
                    <div style="top: -99px; left: 10px; position: relative; text-align: left; font-size: 9px; font-family: Arial;">Аг пасс.</div>
                    <div style="top: -92px; left: 10px; position: relative; text-align: left; font-size: 9px; font-family: Arial;">Аг груз.</div>
                    <div style="top: -151.364px;left: -32px;position:relative;width: 676px;text-align: right;">1.1</div>
                    <div style="top: -152.364px;left: -32px;position:relative;width: 676px;text-align: right;">0.9</div>
                    <div style="top: -153.364px;left: -32px;position:relative;width: 676px;text-align: right;">0.7</div>
                    <div style="top: -137.18px;left:  -32px;position:relative;width: 676px;text-align: right;">0.3</div>
                    <div style="top: -127.18px;left:  -32px;position:relative;width: 676px;text-align: right;">0.0</div>
                </div>

                <!-- Скорость по приказу -->
                <div style="width: 620px; top: 0; right: 0; bottom: 0; left: 0; margin: auto; height: 23px;">
                    <svg preserveAspectRatio="none" vector-effect="non-scaling-stroke" viewBox="-10 0 968 32" style="border: 1px solid;height: 22px;width: 620px; BACKGROUND: yellow;">
                        <xsl:for-each select="Speeds/Speed">
                            <text x="470" y="23" style="font: 20px sans-serif;"><xsl:value-of select="@Value" /></text>
                        </xsl:for-each>
                    </svg>
                </div>

                <!-- Боковой износ -->
                <div class="curve_chart">
                    <svg preserveAspectRatio="none" vector-effect="non-scaling-stroke">
                        <xsl:attribute name="viewBox">
                            <xsl:value-of select="@viewbox_bok_iz_graph" />
                        </xsl:attribute>

                        <polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:blue;stroke-width:2">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@bok_iz_graph" />
                            </xsl:attribute>
                        </polyline>

                        <xsl:for-each select="xaxis/line">
                            <line y1="1" y2="-23" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                        <xsl:for-each select="xaxis/xparam">
                            <line y1="0" y2="0" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-9" y2="-9" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-18" y2="-18" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                    </svg>
                    <div class="zerotext">0</div>
                    <div style="font: bold 10px sans-serif;
                                top: -131px;
                                left: 10px;
                                position: relative;
                                text-align: left;">Износ (м/м)</div>
                    <div style="top: -138.364px;left: -32px;position:relative;width: 30px;text-align: right;">18.0</div>
                    <div style="top: -110.18px;left: -32px;position:relative;width: 30px;text-align: right;">9.0</div>
                </div>


                <!-- Отклонение (см) -->
                <div class="curve_chart">
                    <svg preserveAspectRatio="none" vector-effect="non-scaling-stroke">
                        <xsl:attribute name="viewBox">
                            <xsl:value-of select="@viewbox_dev_graph" />
                        </xsl:attribute>

                        <polyline style="vector-effect:non-scaling-stroke;fill:none;stroke:blue;stroke-width:2">
                            <xsl:attribute name="points">
                                <xsl:value-of select="@dev_graph" />
                            </xsl:attribute>
                        </polyline>

                        <xsl:for-each select="xaxis/line">
                            <line y1="-33" y2="66" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@x1" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                        <xsl:for-each select="xaxis/xparam">
                            <line y1="0" y2="0" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-25" y2="-25" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="25" y2="25" class="xaxis">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="-12" y2="-12" class="xaxis1">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                            <line y1="12" y2="12" class="xaxis1">
                                <xsl:attribute name="x1">
                                    <xsl:value-of select="@minX" />
                                </xsl:attribute>
                                <xsl:attribute name="x2">
                                    <xsl:value-of select="@maxX" />
                                </xsl:attribute>
                            </line>
                        </xsl:for-each>
                    </svg>
                    <div class="zerotext">-25</div>
                    <div class="titletext">Отклонение (см)</div>
                    <div style="top: -138.364px;left: -32px;position:relative;width: 30px;text-align: right;">25</div>
                </div>

                <div style="margin:auto;width:620px;position:relative;">
                    <xsl:for-each select="xaxis/labels/label">
                        <div>
                            <xsl:attribute name="style">
                                <xsl:value-of select="@style" />
                            </xsl:attribute>
                            <xsl:value-of select="@value" />
                        </div>
                    </xsl:for-each>
                </div>
                <br />
                <div style="margin:auto;width:620px;position:relative;">
                    <xsl:for-each select="xaxis/kmlabels/label">
                        <div>
                            <xsl:attribute name="style">
                                <xsl:value-of select="@style" />
                            </xsl:attribute>
                            <u>
                                <xsl:value-of select="@value" /> км
                            </u>
                        </div>
                    </xsl:for-each>
                </div>
            </xsl:for-each>
        </body>

        </html>
    </xsl:template>
</xsl:stylesheet>