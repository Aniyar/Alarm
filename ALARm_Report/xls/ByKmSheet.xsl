<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <head>
        <title>ПОКИЛОМЕТРОВАЯ ВЕДОМОСТЬ ОТСТУПЛЕНИЙ</title>
        <style>
          td {
          padding-left: 5px;
          }
          th.vertical{
          transform: rotate(-90deg);
          }
          .rotate {

          text-align: center;
          writing-mode: vertical-rl;

          font-weight: bold;
          transform: rotate(-180deg);
          padding: 1px; /* Поля вокруг содержимого ячеек */
          vertical-align: top;
          /* Выравнивание по верхнему краю /
          padding: 8px; / Поля вокруг содержимого ячеек */
          }
        </style>
      </head>
      <body>
        <xsl:for-each select="report/trip">
          <div style="page-break-before:always;">

            <table style="font-size: 14px; font-family:  'Times New Roman'; width: 100%; margin: auto;">
              <tr>
                <td>
                  <b>
                    <p align="left" style="color:black; font-size:14px">ПОКИЛОМЕТРОВАЯ ВЕДОМОСТЬ ОТСТУПЛЕНИЙ</p>
                  </b>
                </td>
              </tr>
              <tr>
                <td align="left">
                  <b>
                    <xsl:value-of select="@Poezdka" />
                    ЖД
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
                <td  align="left">
                </td>
              </tr>
              <tr>
                <td align="left">
                  <b>
                    <xsl:value-of select="@ps" />
                  </b>
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;
                  Проезд:                <b>
                    <xsl:value-of select="@trip_date" />
                  </b>
                </td>
              </tr>
            </table>

            <table width="100%" border="1" cellpadding="0" cellspacing="0" class="demotable" align="center">
              <thead>
                <tr>
                  <th class="border-head" style="padding:2.5px;" rowspan="2">КМ</th>
                  <th class="border-head" style="padding:2.5px;" rowspan="2">
                    <p class="rotate">Проверено, км</p>
                  </th>
                  <th class="border-head" style="padding:2.5px;" rowspan="1" colspan="6">Количество отступлений</th>
                  <th class="border-head" style="padding:2.5px;" rowspan="2">
                    <p class="rotate">ПРОЧИЕ</p>
                  </th>
                  <th class="border-head" style="padding:2.5px;" rowspan="2">
                    <p class="rotate">ИТОГО</p>
                  </th>
                  <th class="border-head" style="padding:2.5px;" rowspan="2">
                    Всего выданных
                    <br />
                    ограничений
                    <br />
                    скорости,шт
                  </th>
                </tr>
                <tr>
                  <th class="border-head" style="padding:2.5px;" rowspan="1">
                    <p class="rotate">Скрепления</p>
                  </th>
                  <th class="border-head" style="padding:2.5px;" rowspan="1">
                    <p class="rotate">Рельсовые стыки</p>
                  </th>
                  <th class="border-head" style="padding:2.5px;" rowspan="1">
                    <p class="rotate">Шпалы</p>
                  </th>
                  <th class="border-head" style="padding:2.5px;" rowspan="1">
                    <p class="rotate">Поверхность катания рельсов</p>
                  </th>
                  <th class="border-head" style="padding:2.5px;" rowspan="1">
                    <p class="rotate">Балласт</p>
                  </th>
                  <th class="border-head" style="padding:2.5px;" rowspan="1">
                    <p class="rotate">Бесстыковой путь</p>
                  </th>
                </tr>
              </thead>
              <xsl:for-each select="direction">
                <tr>
                  <th align="right" style="padding:2.5px;" colspan="11">
                    <b>
                      <xsl:value-of select="@name" />
                    </b>
                    <!-- начало записи-->
                  </th>
                </tr>
                <!--pchu-->
                <tr>
                  <th align="left" style="padding:2.5px;" colspan="11">
                    <b>
                      <xsl:value-of select="@pchuInfo" />
                    </b>
                  </th>
                </tr>
                <xsl:for-each select="Notes">
                  <tr align="center">
                    <td>
                      <xsl:value-of select="@km" />
                    </td>
                    <td>
                      <xsl:value-of select="@checkKm" />
                    </td>
                    <td>
                      <xsl:value-of select="@skrep" />
                    </td>
                    <td>
                      <xsl:value-of select="@styk" />
                    </td>
                    <td>
                      <xsl:value-of select="@shpal" />
                    </td>
                    <td>
                      <xsl:value-of select="@npk" />
                    </td>
                    <td>
                      <xsl:value-of select="@ballast" />
                    </td>
                    <td>
                      <xsl:value-of select="@bezStyk" />
                    </td>
                    <td>
                      <xsl:value-of select="@prochie" />
                    </td>
                    <td>
                      <xsl:value-of select="@itogo" />
                    </td>
                    <td>
                      <xsl:value-of select="@vsegoOgrSpeed" />
                    </td>
                  </tr>
                </xsl:for-each>
                <xsl:for-each select="Itogo">
                  <tr align="center">
                    <td>
                      ИТОГО
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_checkKm" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_skrep" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_styk" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_shpal" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_NPK_L" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_ballast" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_bezStyk" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_prochie" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_itogo" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogo_vsegoOgrSpeed" />
                    </td>
                  </tr>
                </xsl:for-each>
                
               
              </xsl:for-each>
              
              <xsl:for-each select="Itogs">
                  <tr align="center">
                    <td>
                      ИТОГО ПО ПЧ
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_checkKm" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_skrep" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_styk" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_shpal" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_NPK_L" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_ballast" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_bezStyk" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_prochie" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_Itogo" />
                    </td>
                    <td>
                      <xsl:value-of select="@Itogs_vsegoOgrSpeed" />
                    </td>
                  </tr>
                </xsl:for-each>

            </table>
            <table style="font-size: 12px; width: 95%;height: 5%; margin: auto;">
              <tr>
                <td align="left">
                  Начальник &#160;&#160;<xsl:value-of select="@ps" />
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                  &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
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