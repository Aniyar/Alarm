<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <xsl:for-each select="report">
      <html>

        <head>
          <title>
            ПУ-32 (
            <xsl:value-of select="@soft" />
            )
          </title>

          <style>
            table.main,
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
            }
          </style>
        </head>
        <body>
          <p align="left" style="color:black;width: 100%;height: 1%;font-size: 10px;">
            <xsl:value-of select="@version" />
          </p>
          <table style="font-size: 15px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse;height:8%;padding: 5px">
            <tr>
              <th style="color:black; font-size: 16px;" align="center">Сравнительная ведомость оценки состояния пути</th>
            </tr>
            <tr>
              <td style="height:3%;margin:auto; padding: 20px;margin-bottom:8px;">
                ПЧ:<xsl:value-of select="@pch" /> Дорога:<xsl:value-of select="@road" />
              </td>
            </tr>
            <tr>
              <td>
                По данным <xsl:value-of select="@triptype" /> проверки за <xsl:value-of select="@month" /> путеизмерителeм <xsl:value-of select="@car" />
              </td>
            </tr>
            <tr>
              <td style="height:3%;margin:auto; padding: 20px;margin-bottom:8px;">
                <xsl:value-of select="@tripdate" />
              </td>
            </tr>
          </table>
          <xsl:for-each select="bykilometer">
            <div>
              <table style="font-size: 15px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse;padding: 5px">
                <tr>
                  <th colspan="16">Количество километров с оценкой и средний балл</th>
                </tr>
              </table>
              <table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
                <tr>
                  <td rowspan="2">
                    № пути
                  </td>
                  <td rowspan="2">
                    Всего<br /> км
                  </td>
                  <td colspan="4">
                    Количество км с оценкой
                  </td>
                  <td rowspan="2">
                    Км <br /> с огр. <br /> скор.
                  </td>
                  <td colspan="3">
                    Отступления (шт)
                  </td>
                  <td rowspan="2">
                    Колич. <br /> км с <br /> путевыми <br /> работами
                  </td>
                  <td colspan="2">
                    Средний <br /> балл по участку
                  </td>
                  <td rowspan="2">
                    Величина <br /> N <sub>уч</sub><br />
                  </td>
                  <td rowspan="2">
                    Качественная<br /> оценка
                  </td>
                  <td rowspan="2">
                    Проверка
                  </td>
                </tr>
                <tr>
                  <td>отл</td>
                  <td>хор</td>
                  <td>уд</td>
                  <td>неуд</td>
                  <td>
                    IV <br /> степ.
                  </td>
                  <td>
                    Сочет.<br />Кривые <br />другие
                  </td>
                  <td>
                    Доп*
                  </td>
                  <td>
                    только <br /> по осн. <br /> парам
                  </td>
                  <td>
                    по всем <br /> (+доп)
                  </td>
                </tr>
                <xsl:for-each select="section">
                  <xsl:for-each select="ways">
                  <tr>
                    <td>
                      <xsl:value-of select="@track" />
                    </td>
                    <td>
                      <xsl:value-of select="@len" />
                    </td>
                    <td>
                      <xsl:value-of select="@excellent" />
                    </td>
                    <td>
                      <xsl:value-of select="@good" />
                    </td>
                    <td>
                      <xsl:value-of select="@satisfactory" />
                    </td>
                    <td>
                      <xsl:value-of select="@bad" />
                    </td>
                    <td>
                      <xsl:value-of select="@limit" />
                    </td>
                    <td>
                      <xsl:value-of select="@d4" />
                    </td>
                    <td>
                      <xsl:value-of select="@other" />
                    </td>
                    <td>
                      <xsl:value-of select="@add" />
                    </td>
                    <td>
                      <xsl:value-of select="@repair" />
                    </td>
                    <td>
                      <xsl:value-of select="@mainavg" />
                    </td>
                    <td>
                      <xsl:value-of select="@addavg" />
                    </td>
                    <td>
                      <xsl:value-of select="@ns" />
                    </td>
                    <td>
                      <xsl:value-of select="@rating" />
                    </td>
                    <td rowspan="{last()}">
                      <xsl:value-of select="@revision" />
                    </td>
                  </tr>
                  </xsl:for-each>
                </xsl:for-each>
                <tr>
                  <td>
                    Итого
                  </td>
                  <td>
                    <xsl:value-of select="@len" />
                  </td>
                  <td>
                    <xsl:value-of select="@excellent" />
                  </td>
                  <td>
                    <xsl:value-of select="@good" />
                  </td>
                  <td>
                    <xsl:value-of select="@satisfactory" />
                  </td>
                  <td>
                    <xsl:value-of select="@bad" />
                  </td>
                  <td>
                    <xsl:value-of select="@limit" />
                  </td>
                  <td>
                    <xsl:value-of select="@d4" />
                  </td>
                  <td>
                    <xsl:value-of select="@other" />
                  </td>
                  <td>
                    <xsl:value-of select="@add" />
                  </td>
                  <td>
                    <xsl:value-of select="@repair" />
                  </td>
                  <td>
                    <xsl:value-of select="@mainavg" />
                  </td>
                  <td>
                    <xsl:value-of select="@addavg" />
                  </td>
                  <td>
                    <xsl:value-of select="@ns" />
                  </td>
                  <td>
                    <xsl:value-of select="@ratings" />
                  </td>
                  <td>
                    <xsl:value-of select="@revision" />
                  </td>
                </tr>
              </table>
              <pre class="tab4">    *Оценка дополнительных параметров: износ, зазоры, неровности</pre>

              <br />
              <br />
            </div>

            <table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
              <!-- <table style="font-size: 12px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse"> -->
              <tr>
                <th colspan="12">Количество отступлений по видам</th>
              </tr>
              <tr>
                <td rowspan="2">Степень</td>
                <td colspan="10">Отступления</td>
                <td rowspan="2">Итого</td>
              </tr>
              <tr>
                <td>Суж</td>
                <td>Уш</td>
                <td>У</td>
                <td>П</td>
                <td>Пр</td>
                <td>Р</td>
                <td>Сочет</td>
                <td>Кривые</td>
                <td>Другие</td>
                <td>Доп</td>
              </tr>
             <xsl:for-each select="revise">
              <xsl:for-each select="countbytype">
                <tr>
                  <td>
                    <xsl:value-of select="@degree" />
                  </td>
                  <td>
                    <xsl:value-of select="@const" />
                  </td>
                  <td>
                    <xsl:value-of select="@broad" />
                  </td>
                  <td>
                    <xsl:value-of select="@level" />
                  </td>
                  <td>
                    <xsl:value-of select="@sag" />
                  </td>
                  <td>
                    <xsl:value-of select="@down" />
                  </td>
                  <td>
                    <xsl:value-of select="@stright" />
                  </td>
                  <td>
                    <xsl:value-of select="@combination" />
                  </td>
                  <td>
                    <xsl:value-of select="@curves" />
                  </td>
                  <td>
                    <xsl:value-of select="@other" />
                  </td>
                  <td>
                    <xsl:value-of select="@additional" />
                  </td>
                  <td>
                    <xsl:value-of select="@sum" />
                  </td>
                </tr>
              </xsl:for-each>
            </xsl:for-each>
            </table>
            <table style="font-size: 14px; font-family: 'Times New Roman';width: 100%; margin: auto;margin-bottom:8px;border-collapse: collapse">
              <tr>
                <td style="text-align:left">Данные обработали и оформили ведомость ПУ-32: ИНЖЕНЕР</td>
                <td style="text-align:right">
                  <xsl:value-of select="../@engineer" />
                </td>
              </tr>
              <tr>
                <td colspan="2" style="text-align:left">
                  Путеизмерительный вагон сопровождали: ПЧ; ПЧЗ; ПЧУ
                </td>
              </tr>
              <tr>
                <td style="text-align:left">
                  Начальник <xsl:value-of select="../@car" /> :
                </td>
                <td style="text-align:right">
                  <xsl:value-of select="../@chief" />
                </td>
              </tr>
            </table>
            <br />

          </xsl:for-each>
        </body>
      </html>
    </xsl:for-each>

  </xsl:template>
</xsl:stylesheet>