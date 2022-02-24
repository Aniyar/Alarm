<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Статистика</title>
				<style>
					.pages {
					page-break-before:always;
					}
					table.main {
					border-collapse: collapse;
					margin: auto;

					width: 100%;
					}
					table.main, td.main, th.main {
					font-size: 14px;

					font-family: 'Times New Roman';
					<!-- border: 1px solid black; -->
					}
					.underLine{
					text-decoration:underline;
					}
				</style>
			</head>
			<body>
				<xsl:for-each select="report/pages">
					<div class="pages">
						<table style="font-size: 14px; font-family: 'Times New Roman'; width: 100%; margin: auto;margin-bottom:20px;height:5%">
							<tr>
								<th align="center">СТАТИСТИКА</th>
							</tr>
							<tr>
								<td align="left">
									<b>
										<xsl:value-of select="@road" />
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
										<xsl:value-of select="@car" />
									</b>
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
									&#160;&#160;
									<b>
										Проезд:
										<xsl:value-of select="@data" />
									</b>

								</td>
							</tr>
							<!-- <tr>
                                <th align="left"><xsl:value-of select="@road" /></th>
                            </tr>
                            <tr>
                                <th align="left"><xsl:value-of select="@period" /> <xsl:value-of select="@type" /> проверка</th>
                            </tr>
                            <tr>
                                <th align="left"><xsl:value-of select="@car" /> <xsl:value-of select="@data" /></th>
                            </tr> -->
						</table>


						<table class="main" style="height:5%">
							<!-- <ul>
                            <li>1  Протяженность (метров)   <td class="main" align="left"><xsl:value-of select="@length" /></td></li>
                            <li>2  Рельсовые накладки     <td class="main"/> </li>
                                <ul>
                                    <li>2.1 Фактическое количество накладок   <td class="main" align="left"><xsl:value-of select="@countOverlay" /></td>
                                         <ul>
                                            <li>Подпункт 2.2.1.</li>
                                            <li>Подпункт 2.2.2.</li>
                                        </ul>
                                    </li>
                                </ul>
                              
                               
                        </ul> -->
							<tr>
								<td class="main" align="right"> 1&#160;&#160;&#160;&#160;</td>
								<td class="main;underLine" align="left" colspan="4">Протяженность (метров)</td>
								<td class="main" align="left">
									<xsl:value-of select="@length" />
								</td>
								<td class="main"/>
							</tr>
							<!--Рельсовые накладки-->
							<tr>
								<td class="main" align="right">2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="4">Рельсовые накладки</td>
								<td class="main"/>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">&#160;&#160;&#160;&#160;2.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество накладок</td>
								<td class="main" align="left">
									<xsl:value-of select="@countOverlay" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;2.1.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectOverlay" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectOverlay" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;2.1.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных неправильно (неправильно определен тип)</td>
								<td class="main" align="left">
									<xsl:value-of select="@countWrongOverlay" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentWrongOverlay" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;2.1.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingOverlay" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingOverlay" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">&#160;&#160;&#160;&#160;2.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Количество накладок определенных системой</td>
								<td class="main" align="left">
									<xsl:value-of select="@countOverlayIdentify" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;2.2.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Ложных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseOverlayIdentify" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseOverlayIdentify" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">&#160;&#160;&#160;&#160;2.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество безболтовых отверстий</td>
								<td class="main" align="left">
									<xsl:value-of select="@countBoltFree" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">2.3.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectBoltFree" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectBoltFree" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">2.3.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingBoltFree" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingyBoltFree" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="4">2.3.2.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left">Определенных как болтовые отверстие</td>
								<td class="main" align="left">
									<xsl:value-of select="@countBoltFreeAsBolt" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentBoltFreeAsBolt" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="4">2.3.2.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left">Неправильный тип накладки</td>
								<td class="main" align="left">
									<xsl:value-of select="@countBoltFreeAsWrongOverlay" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentBoltFreeAsWrongOverlay" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="4">2.3.2.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left">Накладка пропущена</td>
								<td class="main" align="left">
									<xsl:value-of select="@countBoltFreeAsMissingOverlay" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentBoltFreeAsMissingOverlay" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">2.4&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Количество безболтовых отверстий определенных системой</td>
								<td class="main" align="left">
									<xsl:value-of select="@countBoltFreeIdentify" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">2.4.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Ложных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseBoltFreeIdentify" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseBoltFreeIdentify" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="4">2.4.1.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left">Определенных на месте болтового отверстия</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseBoltFreeAsBolt" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseBoltFreeAsBolt" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="4">2.4.1.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left">Определенных на накладках неправильного типа</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseBoltFreeAsWrongOverlay" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseBoltFreeAsWrongOverlay" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="4">2.4.1.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left">Определенных на ложных накладках</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseBoltFreeAsMissingOverlay" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseBoltFreeAsMissingOverlay" />
								</td>
							</tr>
							<!--Рельсовые стыки-->
							<tr>
								<td class="main" align="right">3</td>
								<td class="main" align="left" colspan="4">Рельсовые стыки</td>
								<td class="main"/>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">3.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество стыков</td>
								<td class="main" align="left">
									<xsl:value-of select="@countJoint" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">3.1.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectJoint" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectJoint" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">3.1.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных неправильно (зазор скорректирован)</td>
								<td class="main" align="left">
									<xsl:value-of select="@countWrongJoint" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentWrongJoint" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="4">3.1.2.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left">Изолирующий</td>
								<td class="main" align="left">
									<xsl:value-of select="@countWrongIsoJoint" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentWrongIsoJoint" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">3.1.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingJoint" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingJoint" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="4">3.1.3.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left">Изолирующий</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingIsoJoint" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingIsoJoint" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">3.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Количество стыков определенных системой</td>
								<td class="main" align="left">
									<xsl:value-of select="@countJointIdentify" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">3.2.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Ложных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseJointIdentify" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseJointIdentify" />
								</td>
							</tr>
							<!--Маячные метки-->
							<tr>
								<td class="main" align="right">4&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="4">Маячные метки</td>
								<td class="main"/>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">4.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество маячных метки</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMarks" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">4.1.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectMarks" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectMarks" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">4.1.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных неправильно (подвижка скорректирована)</td>
								<td class="main" align="left">
									<xsl:value-of select="@countWrongMarks" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentWrongMarks" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">4.1.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingMarks" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingMarks" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">4.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Количество маячных меток определенных системой</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMarksIdentify" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">4.2.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Ложных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseMarksIdentify" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseMarksIdentify" />
								</td>
							</tr>
							<!--Дефекты рельсов-->
							<tr>
								<td class="main" align="right">5&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="4">Дефекты рельсов</td>
								<td class="main"/>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">5.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество дефектов</td>
								<td class="main" align="left">
									<xsl:value-of select="@countDefects" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">5.1.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectDefects" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectDefects" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">5.1.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingDefects" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingDefects" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">5.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Количество дефектов определенных системой</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMarksDefects" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">5.2.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Ложных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseDefectsIdentify" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseDefectsIdentify" />
								</td>
							</tr>
							<!--Рельсовые промежуточные скрепления-->
							<tr>
								<td class="main" align="right">6&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="4">Рельсовые промежуточные скрепления</td>
								<td class="main"/>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">6.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество отступлений в содержании скреплений</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFastening" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">6.1.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectFastening" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectFastening" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">6.1.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingFastening" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingFastening" />
								</td>
							</tr>
							<!--Прочие-->
							<tr>
								<td class="main" align="right">7&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="4">Прочие</td>
								<td class="main"/>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">7.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество отступлений в содержании шпал</td>
								<td class="main" align="left">
									<xsl:value-of select="@countSleeper" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.1.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectSleeper" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectSleeper" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.1.2</td>
								<td class="main" align="left" colspan="2">Определенных неправильно (результаты скорректированы)</td>
								<td class="main" align="left">
									<xsl:value-of select="@countWrongSleeper" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentWrongSleeper" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.1.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingSleeper" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingSleeper" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">7.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Количество отступлений в содержании шпал определенных системой</td>
								<td class="main" align="left">
									<xsl:value-of select="@countSleeperIdentify" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.2.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Ложных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseSleeperIdentify" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseSleeperIdentify" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">7.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество отсутствующих противоугонов</td>
								<td class="main" align="left">
									<xsl:value-of select="@countAntiTheft" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.3.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectAntiTheft" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectAntiTheft" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.3.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных неправильно (результаты скорректированы)</td>
								<td class="main" align="left">
									<xsl:value-of select="@countWrongAntiTheft" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentWrongAntiTheft" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.3.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingAntiTheft" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingAntiTheft" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">7.4&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Количество отсутствующих противоугонов определенных системой</td>
								<td class="main" align="left">
									<xsl:value-of select="@countAntiTheftIdentify" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.4.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Ложных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseAntiTheftIdentify" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseAntiTheftIdentify" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">7.5&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Фактическое количество стрелочных переводов</td>
								<td class="main" align="left">
									<xsl:value-of select="@countSwitch" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.5.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных правильно</td>
								<td class="main" align="left">
									<xsl:value-of select="@countCorrectSwitch" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentCorrectSwitch" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.5.2&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Определенных неправильно (результаты скорректированы)</td>
								<td class="main" align="left">
									<xsl:value-of select="@countWrongSwitch" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentWrongSwitch" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.5.3&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Пропущенных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countMissingSwitch" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentMissingSwitch" />
								</td>
							</tr>
							<tr>
								<td class="main" align="right" colspan="2">7.6&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="3">Количество стрелочных переводов определенных системой</td>
								<td class="main" align="left">
									<xsl:value-of select="@countSwitchIdentify" />
								</td>
								<td class="main"/>
							</tr>
							<tr>
								<td class="main" align="right" colspan="3">7.6.1&#160;&#160;&#160;&#160;</td>
								<td class="main" align="left" colspan="2">Ложных</td>
								<td class="main" align="left">
									<xsl:value-of select="@countFalseSwitchIdentify" />
								</td>
								<td class="main" align="left">
									<xsl:value-of select="@percentFalseSwitchIdentify" />
								</td>
							</tr>
						</table>


						<table style="font-size: 12px; width: 85%;height: 5%; margin: auto;">
							<tr>
								<td align="left">
									Начальник &#160;&#160;<xsl:value-of select="@car" />
								</td>

								<td>
									<xsl:value-of select="@chief" />
								</td>
							</tr>
						</table>
						<!-- <div style="width: 90%;margin: auto;font-size: 9px;font-family: Arial;margin-top: 10px;" align="center"><xsl:value-of select="@info" /></div> -->
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>