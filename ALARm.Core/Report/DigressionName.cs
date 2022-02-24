namespace ALARm.Core.Report
{
    public class DigressionName
    {
        public static DigName Undefined = new DigName() { Name = "неизвестный", Value = -1 };
        public static DigName TreadTiltLeft = new DigName() { Name = "Нпк.л", Value = 0 };
        public static DigName TreadTiltRight = new DigName() { Name = "Нпк.п", Value = 1 };
        public static DigName DownhillLeft = new DigName() { Name = "Пу.л.", Value = 12 };
        public static DigName DownhillRight = new DigName() { Name = "Пу.п.", Value = 13 };


        public static DigName Level = new DigName() { Name = "У", Value = 5 }; //уровень
        public static DigName Pru = new DigName() { Name = "ПрУ", Value = 51 }; 
        public static DigName Sag = new DigName() { Name = "П", Value = 6 };
        public static DigName Ramp = new DigName() { Name = "Укл", Value = 7 };
        public static DigName RampNear = new DigName() { Name = "?Уkл", Value = 8 };
        public static DigName SpeedUp = new DigName() { Name = "Анп", Value = 9 };
        public static DigName SpeedUpNear = new DigName() { Name = "?Анп", Value = 22 };

        public static DigName PatternRetraction = new DigName() { Name = "ОШК", Value = 10 };

        public static DigName DrawdownLeft = new DigName() { Name = "Пр.л", Value = 12 };
        public static DigName DrawdownRight = new DigName() { Name = "Пр.п", Value = 13 };
        public static DigName Broadening = new DigName() { Name = "Уш", Value = 4 };
        public static DigName Constriction = new DigName() { Name = "Суж", Value = 15 };

        public static DigName NoneStrigtSide = new DigName() { Name = "рн", Value = 11 };
        public static DigName StrighteningLeft = new DigName() { Name = "Р", Value = 16 };
        public static DigName StrighteningRight = new DigName() { Name = "Р", Value = 17 };
        public static DigName Strightening = new DigName() { Name = "Р", Value = 14 };
     
   

        public static DigName Psi = new DigName() { Name = "Пси", Value = 18 };
        public static DigName PsiNear = new DigName() { Name = "?Пси", Value = 19 }; //отвод шаблона
        public static DigName Gauge10 = new DigName() { Name = "Ш10", Value = 20 }; //ш10
        public static DigName Degree3 = new DigName() { Name = "3ст", Value = 21 }; //3ст
        
        public static DigName NoneStrightening = new DigName() { Name = "Рнр", Value = 23 };
        public static DigName NoneStrighteningST = new DigName() { Name = "Рнрст", Value = 231 };
        public static DigName StrgihtPlusSag = new DigName() { Name = "Р+П", Value = 24 };
        public static DigName StrgihtPlusDrawdown = new DigName() { Name = "Р+Пр", Value = 25 };
        public static DigName StrighteningOnSwitch = new DigName() { Name = "Рст", Value = 26 };
        public static DigName IzoGapNear = new DigName() { Name = "ИС?", Value = 27 };
        public static DigName PatternRetractionNear = new DigName() { Name = "ОШК?", Value = 28 };
        public static DigName LevelReverse = new DigName() { Name = "Уобр", Value = 29 };
        public static DigName Level150 = new DigName() { Name = "У150", Value = 30 };
        public static DigName Level75 = new DigName() { Name = "У75", Value = 31 };

        public static DigName gr = new DigName() { Name = "гр", Value = 100 };

        public static DigName ShortWaveLeft = new DigName() { Name = "КВ.л", Value = 101 };
        public static DigName ShortWaveRight = new DigName() { Name = "КВ.п", Value = 102 }; // кн
        public static DigName MiddleWaveRight = new DigName() { Name = "СВ.л", Value = 103 };
        public static DigName MiddleWaveLeft = new DigName() { Name = "СВ.п", Value = 104 }; //ср н
        public static DigName LongWaveRight = new DigName() { Name = "ДВ.л", Value = 105 };
        public static DigName LongWaveLeft = new DigName() { Name = "ДВ.п", Value = 106 }; // дн
        public static DigName ImpulsLeft = new DigName() { Name = "ИН.л", Value = 107 };
        public static DigName ImpulsRight = new DigName() { Name = "ИН.п", Value = 108 }; //импульс
        public static DigName FusingGap = new DigName() { Name = "СЗ", Value = 109 };

        public static DigName Gap = new DigName() { Name = "З", Value = 110 };
        public static DigName GapSimbol = new DigName() { Name = "З?", Value = 1100 };

        public static DigName GapSimbol_left = new DigName() { Name = "З?Л", Value = 1101 };
        public static DigName GapSimbol_right = new DigName() { Name = "З?П", Value = 1102 };
        public static DigName Gap_right = new DigName() { Name = "ЗП", Value = 1103 };
        public static DigName Gap_left = new DigName() { Name = "ЗЛ", Value = 1104 };
        public static DigName AnomalisticGap = new DigName() { Name = "АРЗ", Value = 111 };
        public static DigName None = new DigName() { Name = "Отсут.", Value = 112 };

        public static DigName SideWearLeft = new DigName() { Name = "Иб.л", Value = 120 };
        public static DigName SideWearRight = new DigName() { Name = "Иб.п", Value = 121 };
        public static DigName VertIznosL = new DigName() { Name = "Ив.л", Value = 122 };
        public static DigName VertIznosR = new DigName() { Name = "Ив.п", Value = 123 }; //верт износ
        public static DigName ReducedWearLeft = new DigName() { Name = "Ип.л", Value = 124 };
        public static DigName ReducedWearRight = new DigName() { Name = "Ип.п", Value = 125 };
        public static DigName HeadWearLeft = new DigName() { Name = "И45.л", Value = 126 };
        public static DigName HeadWearRight = new DigName() { Name = "И45.п", Value = 127 };


        public static DigName BrokenBasePlate = new DigName() { Name = "излом подкладки", Value = 200 };
        public static DigName ClampUnderRail = new DigName() { Name = "Клемма под подошвой рельса", Value = 201 };
        public static DigName BrokenMainBolts = new DigName() { Name = "излом закладных болтов", Value = 202 };
        public static DigName MissingArsClamp = new DigName() { Name = "отсутствие упругой клеммы", Value = 203 };
        public static DigName Missing2OrMoreMainSpikes = new DigName() { Name = "отсутствуют 2 закладных болта ", Value = 204 }; //отсутствие двух или более пришивочных костылей
        public static DigName BrokenArsClamp = new DigName() { Name = "Излом упруой клемми", Value = 205 };
        public static DigName Oir = new DigName() { Name = "Oir" };
        public static DigName MissingClamp = new DigName() { Name = "отсутствие клеммы", Value = 206 };
        public static DigName KNS = new DigName() { Name = "КНС", Value = 207 }; //куст негод скреплений
        public static DigName BallastNotEnough = new DigName() { Name = "BallastNotEnough", Value = 1014 }; //недост балласта
        public static DigName BallastOverage = new DigName() { Name = "BallastOverage", Value = 1015 }; // много балласта
        public static DigName LongitudinalCrack = new DigName() { Name = "LongitudinalCrack", Value = 1017 }; //Продольная трещина
        public static DigName SplitsAtTheEnds = new DigName() { Name = "SplitsAtTheEnds", Value = 1018 };
        public static DigName PrickingOutPiecesOfWood = new DigName() { Name = "PrickingOutPiecesOfWood", Value = 1019 };
        public static DigName FractureOfRCSleeper = new DigName() { Name = "FractureOfRCSleeper", Value = 1020 };
        public static DigName BushBadnessOfSleepers = new DigName() { Name = "BushBadnessOfSleepers", Value = 1021 };
        public static DigName PercentageBadnessOfSleepers = new DigName() { Name = "PercentageBadnessOfSleepers", Value = 1022 };
        public static DigName D65_NoPad = new DigName() { Name = "отсутствует подкладка", Value = 1023 };
        public static DigName KB65_NoPad = new DigName() { Name = "отсутствует подкладка", Value = 1024 };
        public static DigName SklNoPad = new DigName() { Name = "отсутствует подкладка", Value = 1025 };
        public static DigName SklBroken = new DigName() { Name = "SklBroken", Value = 1026 };
        public static DigName WW = new DigName() { Name = "WW", Value = 1027 };
        public static DigName KD65NB = new DigName() { Name = "отсутствуют 2 закладных болта", Value = 1028 };
        public static DigName KppNoPad = new DigName() { Name = "отсутствует подкладка", Value = 1029 };
        public static DigName KB65_MissingClamp = new DigName() { Name = "отсутствуют 2 закладных болта ", Value = 1030 };
        public static DigName P350MissingClamp = new DigName() { Name = "Отсутствие клеммного болта", Value = 1031 };
        public static DigName GBRNoPad = new DigName() { Name = "отсутствует подкладка", Value = 103 };
    }

}

