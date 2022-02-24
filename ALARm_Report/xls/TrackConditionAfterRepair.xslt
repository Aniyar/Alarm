<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>ФПО ВЕДОМОСТЬ ХАРАКТЕРИСТИК СОСТОЯНИЯ И УСТРОЙСТВА ПУТИ ПОСЛЕ РЕМОНТНО-ПУТЕВЫХ РАБОТ</title>
				<style>
					table {
					border-collapse: collapse;
					width: 90%;
					font-size: 12;
					}
					td {
					text-align: center;
					}
				</style>
			</head>

			<xsl:for-each select="report/trip">

				<div style="font-size: 8; font-weight: lighter;">
					<xsl:value-of select="@version" />
				</div>

				<table style="font-size: 14px;" align="center">
					<tr>
						<th>
							ВЕДОМОСТЬ ХАРАКТЕРИСТИК СОСТОЯНИЯ И УСТРОЙСТВА ПУТИ ПОСЛЕ РЕМОНТНО-ПУТЕВЫХ РАБОТ (ФПО)
						</th>
					</tr>
				</table>

				<table align="center" border="1">
					<thead>
						<tr>
							<td colspan="2">
								<strong>Направление:</strong> &#160; <xsl:value-of select="@direction" />
							</td>
							<td colspan="1">
								<strong>Путь:</strong> &#160; <xsl:value-of select="@put" />, &#160;
								<strong>ПЧ:</strong> &#160; <xsl:value-of select="@pch" />
							</td>
							<td colspan="2">
								<strong>Дата сдачи:</strong> &#160; <xsl:value-of select="@trip_date" />
							</td>
						</tr>
						<tr>
							<td colspan="1">
								<strong>Участок:</strong> &#160; <xsl:value-of select="@road" />
							</td>
							<td colspan="1">
								<strong>Нач. участка:</strong> &#160; <xsl:value-of select="@start" />
							</td>
							<td colspan="1">
								<strong>Кон. участка:</strong> &#160; <xsl:value-of select="@final" />
							</td>
							<td colspan="1">
								<strong>Дата поездки:</strong> &#160; <xsl:value-of select="@trip_date" />
							</td>
						</tr>
						<tr>
							<td colspan="5">
								<strong>Тип ремонта:</strong> &#160; <xsl:value-of select="@repairs_type" />
							</td>
						</tr>
					</thead>
				</table>

				<table align="center" border="1">
					<tr>
						<td class="border-head" style="padding:2.5px; text-align: left; font-weight: bold;" colspan="18">1. Общие характеристики участка ремонта</td>
						<td class="border-head" style="padding:2.5px; font-weight: bold;" >до ремонта</td>
						<td class="border-head" style="padding:2.5px; font-weight: bold;" >после ремонта</td>
					</tr>
					<tr>
						<td class="border-head" style="padding:2.5px; text-align: left;"  colspan="18">1.1. Максимальная скорость пассажирских поездов, км/ч</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@MAX_speed"/>
						</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@MAX_speed_after"/>
						</td>
					</tr>
					<tr>
						<td class="border-head" style="padding:2.5px; text-align: left;"  colspan="18">1.2. Категория пути</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@trackclasses" />
						</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@trackclasses_after" />
						</td>
					</tr>
					<tr>
						<td class="border-head" style="padding:2.5px; text-align: left;"  colspan="18">1.3. Максимальный уклон,</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@slope"/>
						</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@slope_after"/>
						</td>
					</tr>
					<tr>
						<td class="border-head" style="padding:2.5px; text-align: left;"  colspan="18">1.4. Максимальная разность уклонов смежных элементов профиля,</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@uklon"/>
						</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@uklon_after"/>
						</td>
					</tr>
					<tr>
						<td class="border-head" style="padding:2.5px; text-align: left;"  colspan="18">1.5 Величина СССП</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@cccp_max"/>
						</td>
						<td class="border-head" style="padding:2.5px;" >
							<xsl:value-of select="@cccp_max_after"/>
						</td>
					</tr>
				</table>

				<table align="center" border="1">
					<thead>
						<tr>
							<td class="border-head" style="padding:2.5px; text-align: left; font-weight: bold;" colspan="21">2. Характеристики кривых</td>
						</tr>
						<tr>
							<td class="border-head" style="padding:2.5px;" rowspan="2" colspan="1">N</td>
							<td class="border-head" style="padding:2.5px;" rowspan="1" colspan="2">Начало</td>
							<td class="border-head" style="padding:2.5px;" rowspan="1" colspan="2">Конец</td>
							<td class="border-head" style="padding:2.5px;" rowspan="2" colspan="1">Несовпад,м</td>
							<td class="border-head" style="padding:2.5px;" rowspan="1" colspan="3">Длина,м</td>
							<td class="border-head" style="padding:2.5px;" rowspan="1" colspan="5">Радиус,м</td>
							<td class="border-head" style="padding:2.5px;" rowspan="1" colspan="4">Возвышение,мм</td>
							<td class="border-head" style="padding:2.5px;" rowspan="2" colspan="1">
								α<sub>нп</sub>
							</td>
							<td class="border-head" style="padding:2.5px;" rowspan="2" colspan="1">Ψ</td>
							<td class="border-head" style="padding:2.5px;" rowspan="2" colspan="2">Оценка</td>
						</tr>
						<tr>
							<td class="border-head" style="padding:2.5px;">км</td>
							<td class="border-head" style="padding:2.5px;">м</td>
							<td class="border-head" style="padding:2.5px;">км</td>
							<td class="border-head" style="padding:2.5px;">м</td>
							<td class="border-head" style="padding:2.5px;">
								K;<sup>кр</sup>
							</td>
							<td class="border-head" style="padding:2.5px;">1 п.кр</td>
							<td class="border-head" style="padding:2.5px;">2 п.кр</td>
							<td class="border-head" style="padding:2.5px;">Проект</td>
							<td class="border-head" style="padding:2.5px;">Сред</td>
							<td class="border-head" style="padding:2.5px;">△R</td>
							<td class="border-head" style="padding:2.5px;">мин.</td>
							<td class="border-head" style="padding:2.5px;">Разброс,%</td>
							<td class="border-head" style="padding:2.5px;">Проект</td>
							<td class="border-head" style="padding:2.5px;">Сред</td>
							<td class="border-head" style="padding:2.5px;">△h</td>
							<td class="border-head" style="padding:2.5px;">Крутизна отвода</td>
						</tr>
					</thead>

					<xsl:for-each select="Curve">
						<tr>
							<td colspan="1">
								<!-- Номер -->
								<xsl:value-of select="@ind" />
							</td>
							<td colspan="1">
								<!-- начальый км -->
								<xsl:value-of select="@start_km" />
							</td>
							<td colspan="1">
								<!-- начальый м -->
								<xsl:value-of select="@start_m" />
							</td>
							<td colspan="1">
								<!-- начальый км -->
								<xsl:value-of select="@final_km" />
							</td>
							<td colspan="1">
								<!-- начальый м -->
								<xsl:value-of select="@final_m" />
							</td>
							<td colspan="1">
								<!-- Несовпад м -->
								<xsl:value-of select="@final_m" />
							</td>
							<td colspan="1">
								<!-- K кр -->
								<xsl:value-of select="@len" />
							</td>
							<td colspan="1">
								<!-- 1 п.кр -->
								<xsl:value-of select="@len2" />
							</td>
							<td colspan="1">
								<!-- 2 п.кр -->
								<xsl:value-of select="@len2_lvl" />
							</td>
							<td colspan="1">
								<!-- Р проект -->
								<xsl:value-of select="@P" />
							</td>
							<td colspan="1">
								<!-- Сред -->
								<xsl:value-of select="@radius" />
							</td>
							<td colspan="1">
								<!-- Треугольник R -->
								<xsl:value-of select="@DeltaR" />
							</td>
							<td colspan="1">
								<!-- Мин  -->
								<xsl:value-of select="@lvl_min" />
							</td>
							<td colspan="1">
								<!-- возвышение  разброс  -->
								<xsl:value-of select="@scatter" />
							</td>
							<td colspan="1">
								<!-- Проект -->
								<xsl:value-of select="@lvl" />
							</td>
							<td colspan="1">
								<!-- сред  -->
								<xsl:value-of select="@lvl_mid" />
							</td>

							<td colspan="1">
								<!-- h  -->
								<xsl:value-of select="@DeltaH" />
							</td>
							<td colspan="1">
								<!-- крутизна отвода  -->
								<xsl:value-of select="@withdrawal" />
							</td>
							<td colspan="1">
								<!-- альфа -->
								<xsl:value-of select="@anp" />
							</td>
							<td colspan="1">
								<!-- Пси -->
								<xsl:value-of select="@psi" />
							</td>
							<td colspan="1">
								<!-- Оценка -->
								<xsl:value-of select="@Ocenka" />
							</td>

						</tr>

					</xsl:for-each>
				</table>






				<table align="center" border="1">
					<thead>
						<tr>
							<td class="border-head" style="padding:2.5px; text-align: left; font-weight: bold;" colspan="20">
								3. Характеристики километра
							</td>
						</tr>

						<tr>
							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> Км</td>
							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> Макс. величина неровностей в профиле, мм</td>
							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> Максимальная величин неровностей в плане, мм</td>
							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> Отклонение от норм по уровню, мм</td>

							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;">  Отклонение от ширины колеи,мм</td>
							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> СССП</td>
							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> Количество отступлений 2-й/3-й/4-й степени</td>

							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> Планируемая скорость V пл , км/ч</td>
							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> Допустимая скорость V дп  , км/ч</td>
							<td valign="top" colspan="2" class="border-head" style="padding:2.5px;"> Оценка км	</td>
						</tr>

					</thead>
					<!-- начало записи-->
					<xsl:for-each select="Notes">
						<tr>
							<td colspan="2">
								<!-- Км -->
								<xsl:value-of select="@km" />
							</td>
							<td colspan="2">
								<!-- // Макс. величина неровностей в профиле, мм -->
								<xsl:value-of select="@MaxProfile" />
							</td>
							<td colspan="2">
								<!-- 3.2. Максимальная величин неровностей в плане, мм -->
								<xsl:value-of select="@MaxPlan" />
							</td>
							<td colspan="2">
								<!-- Отклонение от норм по уровню, мм -->
								<xsl:value-of select="@OtklanenieUroveni" />
							</td>
							<td colspan="2">
								<!--  - Отклонение от ширины  колеи , мм  -->
								<xsl:value-of select="@OtklanenieShiriny" />
							</td>
							<td colspan="2">
								<!-- Величина СССП</td> -->
								<xsl:value-of select="@cccp" />
							</td>
							<td colspan="2">
								<!-- Количество отступлений II (III) степени (по КВЛ-П) -->
								<xsl:value-of select="@KollichestvoOtst" />
							</td>
							<td colspan="2">
								<!-- Планируемая скорость  , км/ч -->
								<xsl:value-of select="@VP" />
							</td>
							<td colspan="2">
								<!-- Допустимая скорость (по параметрам устройства)  , км/ч -->
								<xsl:value-of select="@Vdop" />
							</td>

							<td colspan="2">
								<!-- Оценка км</td> -->
								<xsl:value-of select="@Ocenka" />
							</td>
						</tr>

					</xsl:for-each>
				</table>
			</xsl:for-each>

		</html>
	</xsl:template>
</xsl:stylesheet>