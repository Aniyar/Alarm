unit Params;

interface

uses
    Windows, Messages, StdCtrls, SysUtils;

Type

    tdefdata = record
        f1, f2, f3, f4: integer;
        f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15: real;
    end;

    defdata = array of tdefdata;

    Matrix9 = array [1 .. 16, 1 .. 2] of real;

    factstrel = record
        nachm: integer;
        konm: integer;
    end;

    TMainTrackObject = record
        km, meter, start_km, start_m, final_km, final_m: integer;
    end;

    PutListType = record
        pch: integer;
        GNaprav: string;
        put, akm, amtr, bkm, bmtr, count: integer;
    end;

    MasPutList = array of PutListType;

    TRem = record
        adat, bdat, put, rtype: string;
        km, pik, v, vg: integer;
    end;

    PsiAnp = record
        max: real;
        x1, x2: integer;
        v: real;
    end;

    MasRem = array of TRem;

    // ------------------------------------------------------------------------------
    TVNPik = record
        mtr1, mtr2, pik1, pik2, v11, v12, v21, v22: integer;
    end;

    VNPik = array of TVNPik;

    // ------------------------------------------------------------------------------
    TFKriv = record
        km, fun, fnrm: integer;
    end;

    MFKriv = array of TFKriv;

    // ------------------------------------------------------------------------------
    NaturInfKriv = record
        NumPik, NumInfPik: integer;
        InfKrivoi: string;
    end;

    MasNaturKriv = array of NaturInfKriv;

    // ------------------------------------------------------------------------------
    TFuncs = record
        km, fun: integer;
    end;

    MFuncs = array of TFuncs;

    // ------------------------------------------------------------------------------
    TFuncs2 = record
        x, fun: integer;
    end;

    MFuncs2 = array of TFuncs2;
    // ------------------------------------------------------------------------------

    // флаг для крив. уч.
    ConFlagsKrv = record
        FNpk1, FKpk1, FKpk2, FNpk2: boolean;
    end;

    // ------------------------------------------------------------------------------
    // массив для крив уч.
    FeatKrv = record
        km, Npk1, Kpk1, Kpk2, Npk2, NNB1, KNB1, KNB2, NNB2, nach_krv, piket_krv,
          Radius, v, put, pik, Wear: integer;
        Fsr_Rih1, Fsr_Rih2, Fsr_Shab, Fsr_Urob: real;
    end;

    MFeatKrv = array of FeatKrv;

    // ------------------------------------------------------------------------------
    TKriv = record
        Fsr: real;
        Rad: integer;
    end;

    TVecKriv = array [1 .. 162] of TKriv;
    // ------------------------------------------------------------------------------
    masint = array of record km, KmReal: integer;
end;
// *******************************************************************************
TPartKm = record pch, put, pchu, pd, pdb, km, a_mtr, b_mtr: integer;
end;
UPartKm = array of TPartKm;

// Для нового путевого листа
TAdmin = record pch, mex, pd, pdb, nkm, kkm: integer;
nput:
longint;
GlbNaim, NazbN, NazbK, pch_name: string;
fio_pd:
string[20];
end;
UAdmin = array of TAdmin;
UMainTrackObject = array of TMainTrackObject;

TRasp = record nkm, nmtr, kkm, kmtr, kodst, osk, osm: integer;
GlbNaim, naimst, vid, put: string;
end;
URasp = array of TRasp;

TPriamoy = record pch, nkm, nmtr, kkm, kmtr, norma: integer;
put, GlbNaim, tiprix: string;
end;
UPriamoy = array of TPriamoy;
TPuchenie = record pch, nkm, nmtr, kkm, kmtr, vozv: integer;
put, GlbNaim: string;
end;
UPuchenie = array of TPuchenie;

TCurveStright = record nkm, nmtr, kkm, kmtr, Radius, l1, l2, norma_width,
  Wear: integer;
x_npk1, x_kpk1, x_kpk2, x_npk2: real;
end;

TCurveLevel = record nkm, nmtr, kkm, kmtr, level, l1, l2: integer;
x_npk1, x_kpk1, x_kpk2, x_npk2: real;
end;

TCurve = record id, pch, nkm, nmtr, kkm, kmtr, ret: integer;
n100, k100: real;
strights:
array of TCurveStright;
levels:
array of TCurveLevel;
put, GlbNaim, rix: string;
end;
UKrivoy = array of TCurve;

TStrel = record pch, nkm, nmtr, kkm, kmtr, marka, posh, bix: integer;
GlbNaim, put, num, kod: string;
end;
UStrel = array of TStrel;

TStrel2 = record pch, nkm, nmtr, kkm, kmtr, marka, bix: integer;
GlbNaim, put, num, angl, rels, kod: string;
end;
UStrel2 = array of TStrel2;

TNesuKm = record pch, km: integer;
GlbNaim, put: string;
end;
UNesuKm = array of TNesuKm;

TNestKm = record pch, km, dlina: integer;
GlbNaim, put: string;
end;
UNestKm = array of TNestKm;

TSkorost = record pch, nkm, nmtr, kkm, kmtr, skp, skg: integer;
GlbNaim, sp, put: string;
end;
USkorost = array of TSkorost;

TMostTonnel = record pch, nkm, nmtr, kkm, kmtr, dl, id: integer;
GlbNaim, tip, put: string;
end;
UMostTonnel = array of TMostTonnel;

TShpal = record pch, nkm, nmtr, kkm, kmtr, god: integer;
GlbNaim, put: string;
end;
UShpal = array of TShpal;

masoftqrlabel = array of tlabel;

TNst = record xw, xr: integer;
end;
KNst = array of TNst;

TNstInf = record km: integer;
j:
integer;
c:
integer;
end;

TFn0 = record km, m, m_: integer;
end;
Fn0_ = array of TFn0;

tfls = record dir, fname: string;
end;

// *******************************************************************************
procedure SabLog(NamePnt: string);
    procedure CleanerTemp;

        function GetRemKorKm(km: integer): boolean;
        // function KrvFlag(a,b:integer):boolean;
            function ZhokKm(pkm: integer): boolean;
                procedure GetCorrKm(km: integer);

                const
                    // ProgVer='1.93.1 ';
                    Ins = 'ЦП-617/11 от 27.06.11 '; // 'ЦП-515 от 2006 ';
                    Vagon = 'МДК-АЛАРм:';

                    BPO = 'ВПО';
                    BPOBPR = 'ВПО или ВПР';
                    BPOIBPR = 'ВПО+ВПР';
                    BPOBPRDCP = 'ВПО+ВПР+ДСП';

                    RD_CURVE_TYPE_RADIUS = 1;

                    CouPointAvg = 50;
                    // CouPointAvgSR = 28;
                    CONS_ECOUNT = 9;

                    // Для масштаба график
                    k_mash = 5.28 * 16 / 14 * 60 / 59;
                    k_jyl = -25; // 0;//-8;
                    k_jylrih = 0; // 28;
                    k_jylpros1 = 42.9; // 45;
                    k_jylpros2 = 52.9; // 55.6;
                    // k_mashrih= 5.28*16/14*60/59/2;   // 3.068 do 15.10.2011

                    k_mashShab = 2.81;
                    // 2.83//2.865;//2.93;//2.80//2.86;//3.068;
                    k_mashPros = 2.749; // 3.068;

                    k_jylrih1 = 3.4; // 2.55; //3.4;
                    k_jylrih2 = -3.4; // -2.55; //-3.4;
                    // Для линейкй шаблона
                    c_sh_1520 = 25; // 25.0;
                    sh_koef = 1.44; // 1.33;

                    // Локальная папка
                    {
                      Path_info_put_file=   'd:\work_shifrovka\info_put.svgp';// '\\192.168.1.2\work_shifrovka\info_put.svgp'; //; //'.\work_shifrovka\info_put.svgp';    //
                      Path_km_shifrovka_file= 'd:\work_shifrovka\';  //'\\192.168.1.2\work_shifrovka\'; //; //'.\work_shifrovka\';                //
                    }

                    // сетевая папка
                    {
                      Path_info_put_file=      '\\192.168.1.2\work_shifrovka\info_put.svgp';
                      Path_km_shifrovka_file=  '\\192.168.1.2\work_shifrovka\';
                    }

                    Path = '.\PutList\';
                    foldots = 'MALMET\';
                    FoldGr = '.\GRAPHREP\';
                    FoldBED = 'BEDREP\';
                    FoldOTS34 = 'OTSINF\';
                    FoldUBED = 'UBEDOM\';
                    // FoldDATA = 'DATA\';   //'.\DATA\';
                    FoldBckpRest = 'BCKRST\';
                    NetPathBx = { 'd:\work\filesBx\'; // }
                      '\\192.168.16.1\d\work\files\';
                    NetPathAx = { 'd:\work\filesAx\'; // }
                      '\\192.168.16.20\d\work\files\';

                    // Коеффиценты поправки для параметров рельсовой колеи

                    koefUrob = 1; // 0.5; //Коефицент для уровня
                    kfPro = 0.9; //
                    kfShab = 1.0; // 1.002;
                    kfUrb = 1.0;
                    kfRih = 2;
                    // ------------------------------------------------------------------------------
                    TabKrivCnt = 162;

                    TabKrv: TVecKriv = ((Fsr: 3.5; Rad: 5102), (Fsr: 4;
                      Rad: 4464), (Fsr: 4.5; Rad: 3968), (Fsr: 5; Rad: 3571),
                      (Fsr: 5.5; Rad: 3247), (Fsr: 6; Rad: 2976), (Fsr: 6.5;
                      Rad: 2758), (Fsr: 7; Rad: 2552), (Fsr: 7.5; Rad: 2380),
                      (Fsr: 8; Rad: 2232), (Fsr: 8.5; Rad: 2101), (Fsr: 9;
                      Rad: 1984), (Fsr: 9.5; Rad: 1880), (Fsr: 10; Rad: 1786),
                      (Fsr: 10.5; Rad: 1701), (Fsr: 11; Rad: 1623), (Fsr: 11.5;
                      Rad: 1553), (Fsr: 12; Rad: 1488), (Fsr: 12.5; Rad: 1429),
                      (Fsr: 13; Rad: 1374), (Fsr: 13.5; Rad: 1323), (Fsr: 14;
                      Rad: 1276), (Fsr: 14.5; Rad: 1232), (Fsr: 15; Rad: 1190),
                      (Fsr: 15.5; Rad: 1152), (Fsr: 16; Rad: 1116), (Fsr: 16.5;
                      Rad: 1082), (Fsr: 17; Rad: 1050), (Fsr: 17.5; Rad: 1020),
                      (Fsr: 18; Rad: 992), (Fsr: 18.5; Rad: 965), (Fsr: 19;
                      Rad: 940), (Fsr: 19.5; Rad: 916), (Fsr: 20; Rad: 893),
                      (Fsr: 20.5; Rad: 871), (Fsr: 21; Rad: 850), (Fsr: 21.5;
                      Rad: 831), (Fsr: 22; Rad: 812), (Fsr: 22.5; Rad: 794),
                      (Fsr: 23; Rad: 776), (Fsr: 23.5; Rad: 760), (Fsr: 24;
                      Rad: 744), (Fsr: 24.5; Rad: 729), (Fsr: 25; Rad: 714),
                      (Fsr: 25.5; Rad: 700), (Fsr: 26; Rad: 687), (Fsr: 26.5;
                      Rad: 674), (Fsr: 27; Rad: 661), (Fsr: 27.5; Rad: 649),
                      (Fsr: 28; Rad: 638), (Fsr: 28.5; Rad: 627), (Fsr: 29;
                      Rad: 616), (Fsr: 29.5; Rad: 605), (Fsr: 30; Rad: 595),
                      (Fsr: 30.5; Rad: 585), (Fsr: 31; Rad: 576), (Fsr: 31.5;
                      Rad: 567), (Fsr: 32; Rad: 558), (Fsr: 32.5; Rad: 549),
                      (Fsr: 33; Rad: 541), (Fsr: 33.5; Rad: 533), (Fsr: 34;
                      Rad: 525), (Fsr: 34.5; Rad: 518), (Fsr: 35; Rad: 510),
                      (Fsr: 35.5; Rad: 503), (Fsr: 36; Rad: 496), (Fsr: 36.5;
                      Rad: 489), (Fsr: 37; Rad: 483), (Fsr: 37.5; Rad: 476),
                      (Fsr: 38; Rad: 470), (Fsr: 38.5; Rad: 464), (Fsr: 39;
                      Rad: 458), (Fsr: 39.5; Rad: 452), (Fsr: 40; Rad: 446),
                      (Fsr: 40.5; Rad: 441), (Fsr: 41; Rad: 436), (Fsr: 41.5;
                      Rad: 430), (Fsr: 42; Rad: 425), (Fsr: 42.5; Rad: 420),
                      (Fsr: 43; Rad: 415), (Fsr: 43.5; Rad: 411), (Fsr: 44;
                      Rad: 406), (Fsr: 44.5; Rad: 401), (Fsr: 45; Rad: 397),
                      (Fsr: 45.5; Rad: 392), (Fsr: 46; Rad: 388), (Fsr: 46.5;
                      Rad: 384), (Fsr: 47; Rad: 330), (Fsr: 47.5; Rad: 376),
                      (Fsr: 48; Rad: 372), (Fsr: 48.5; Rad: 368), (Fsr: 49;
                      Rad: 364), (Fsr: 49.5; Rad: 361), (Fsr: 50; Rad: 357),
                      (Fsr: 50.5; Rad: 354), (Fsr: 51; Rad: 350), (Fsr: 51.5;
                      Rad: 347), (Fsr: 52; Rad: 343), (Fsr: 52.5; Rad: 340),
                      (Fsr: 53; Rad: 337), (Fsr: 53.5; Rad: 334), (Fsr: 54;
                      Rad: 331), (Fsr: 54.5; Rad: 328), (Fsr: 55; Rad: 325),
                      (Fsr: 55.5; Rad: 322), (Fsr: 56; Rad: 319), (Fsr: 56.5;
                      Rad: 316), (Fsr: 57; Rad: 313), (Fsr: 57.5; Rad: 311),
                      (Fsr: 58; Rad: 308), (Fsr: 58.5; Rad: 305), (Fsr: 59;
                      Rad: 303), (Fsr: 59.5; Rad: 300), (Fsr: 60; Rad: 298),
                      (Fsr: 60.5; Rad: 295), (Fsr: 61; Rad: 293), (Fsr: 62.5;
                      Rad: 288), (Fsr: 63; Rad: 283), (Fsr: 63.5; Rad: 279),
                      (Fsr: 64; Rad: 274), (Fsr: 64.5; Rad: 270), (Fsr: 65;
                      Rad: 266), (Fsr: 65.5; Rad: 263), (Fsr: 66; Rad: 259),
                      (Fsr: 66.5; Rad: 255), (Fsr: 67; Rad: 252), (Fsr: 67.5;
                      Rad: 250), (Fsr: 68; Rad: 247), (Fsr: 68.5; Rad: 245),
                      (Fsr: 69; Rad: 244), (Fsr: 71.5; Rad: 238), (Fsr: 73;
                      Rad: 234), (Fsr: 74.5; Rad: 231), (Fsr: 76; Rad: 228),
                      (Fsr: 77.5; Rad: 225), (Fsr: 78; Rad: 223), (Fsr: 79.5;
                      Rad: 220), (Fsr: 81; Rad: 217), (Fsr: 81.5; Rad: 215),
                      (Fsr: 83; Rad: 212), (Fsr: 83.5; Rad: 210), (Fsr: 85;
                      Rad: 207), (Fsr: 85.5; Rad: 205), (Fsr: 86; Rad: 203),
                      (Fsr: 87.5; Rad: 200), (Fsr: 88; Rad: 198), (Fsr: 88.5;
                      Rad: 196), (Fsr: 89; Rad: 194), (Fsr: 90.5; Rad: 191),
                      (Fsr: 91; Rad: 189), (Fsr: 91.5; Rad: 187), (Fsr: 92;
                      Rad: 185), (Fsr: 92.5; Rad: 183), (Fsr: 93; Rad: 181),
                      (Fsr: 93.5; Rad: 180), (Fsr: 94; Rad: 178), (Fsr: 94.5;
                      Rad: 176), (Fsr: 95; Rad: 174), (Fsr: 95.5; Rad: 172),
                      (Fsr: 96; Rad: 170), (Fsr: 96.5; Rad: 168), (Fsr: 97;
                      Rad: 166));

                    Omega: Matrix9 = ((0, 0.7), (0.7, 1.0), (1.0, 1.2),
                      (1.2, 1.4), (1.4, 1.6), (1.6, 1.7), (1.7, 1.9),
                      (1.9, 2.1), (2.1, 2.3), (2.3, 2.5), (2.5, 2.7),
                      (2.7, 2.9), (2.9, 3.0), (3.0, 3.1), (3.1, 3.2),
                      (3.2, 100.0));

                    // ------------------------------------------------------------------------------
                var
                    nstCou: integer = 0;
                    RetMost: integer = 0;
                    fdBroad: integer = 0;
                    fdConstrict: integer = 0;
                    fdLevel: integer = 0;
                    fdDrawdown: integer = 0;
                    fdStright: integer = 0;
                    fdSkew: integer = 0;
                    fls: array of tfls;
                    ANP_GUs, ANP_GUs2, GUs, GUs2: array of real;
                    ifls: integer = 0;
                    sDirs: array of string;
                    nstPart: integer = 0;
                    newCountUsh: integer = 0;
                    newCountSuj: integer = 0;
                    k_mashrih, k_mashrihpasp, k_nusk: real;
                    Flag_sablog: boolean = false;
                    NecorrPasportFlag: boolean = false;

                    iv_: boolean = false;

                    kf_rih: real = 1;
                    RemTab9: integer = 0;
                    farr: defdata;
                    ID_global: integer = 0;
                    GFlag_Km1683: boolean = false;
                    Id_Km1683: integer = 0;

                    GFlag_Km2501: boolean = false;
                    Id_Km2501: integer = 0;

                    ProgVer: string = '';
                    Fn0: Fn0_;

                    Fnst: KNst;
                    GNs: TNstInf;

                    tanba_rih: integer = 1;
                    riht_nit: integer = 1;
                    tanba_urb: integer = 1;
                    num_proberki: string;

                    Glb_NestKm: boolean = false;
                    Glb_NestKm_: boolean = false;
                    GlbKmLength: integer = 1000;
                    Glb_NestKm_count: integer = 0;
                    Glb_NestKm_count2: integer = 0;
                    glbCount_Per4s_s: integer = 0;

                    suj_s2_s, suj_s3_s, glbCount_suj4s_s, ush_s2_s, ush_s3_s,
                      glbCount_Ush4s_s, pro1_s2_s, pro1_s3_s, glbCount_1Pro4s_s,
                      pro2_s2_s, pro2_s3_s, glbCount_2Pro4s_s, per_s2_s,
                      per_s3_s, pot_s2_s, pot_s3_s, glbCount_Urv4s_s, rih1_s2_s,
                      rih1_s3_s, glbCount_Rih4s_s, rih2_s2_s, rih2_s3_s,
                      GlbCountRemKm_s, GlbCountUKL_s, GlbCountUSK_s,
                      GlbCountSOCH_s, GlbCountDGR_s: integer;
                    LengthKm_s: real = 0;

                    i0_urv: integer;
                    count_urv: integer = 0;
                    flg_urv: boolean = false;
                    urv_dlina2: integer = 0;
                    urv_dlina3: integer = 0;
                    urv_dlina4: integer = 0;

                    Rih_Nit: MFuncs2;

                    urv_nach2, urv_nach3, urv_nach4: integer;

                    Part_km: UPartKm;
                    km_first_mtr: integer = 0;
                    km_last_mtr: integer = 0;

                    Vstavka_flag: boolean = false;

                    Glb_Put_PDB: integer = 1;

                    FoldDATA_host, FoldApp, FoldDATA, FoldUakyt, DName2,
                      Path_work_shifrovka, Path_work_shifrovka_net: string;
                    host: string = '';

                    regim: integer;

                    gnaim: string;
                    nkm, PGKMA, PGKMB, IKM, A, B, HKM, ECOUNT, x0, xn, kk,
                      TekKm, TekKmTrue: integer;
                    AvgV_km, uklkoef: integer;
                    GlbKmTrue: integer = 0;
                    GlbTrackId: longint;
                    Vstavka_Length: integer = 0;
                    prvKmLen: integer = 0;

                    PrintFlag, PrintFlagBED, PrintFlagOTS, BtmAxisINV,
                      LftAxisINV: boolean;

                    GlbGraphRepFlag: boolean = true;
                    BadKMFlag: boolean = true; // flag dlya pechati neud km

                    // altynbek
                    masofstr, masofzamech: masoftqrlabel;
                    lenmasofstr, lenmasofusk, lenofzamen: integer;
                    rixtyp: byte;
                    rixtyp1: real;
                    Const_Urb1, Const_Urb2, lng, NJ: integer;
                    F01, PL: TEXTFILE;
                    pnt: string;
                    NAPR_DBIJ: integer;
                    GlbTripDate: TDateTime;
                    PBar_i, MetZap, STARTKM, STARTKMTRUE, CNT, COUNT_PROIDKM,
                      AA_KM, BB_KM: integer;
                    NUMPUT: longint;

                    NAPRABLENIE, PCH_F, PathMB, GlbPCH: STRING;

                    START_ST_F, END_ST_F, CHIEF_F, NUMBERCAR_F: STRING; // [20];
                    PARITY_F, DIRECTION_F: STRING; // [8];
                    PasGruzSkor, CVSredKm: integer;

                    Urb_sr, Sab_sr, Rh1_sr, Rh2_sr: real;

                    FlagKrv: ConFlagsKrv; // flag dlia krv uch
                    InfKvUch, kv, kv_natur: MFeatKrv;
                    kkk, U_IND, U_LNG, U_KMTRUE, N_PCHU, N_PD, N_PDB,
                      CountInLog, VarKm, VarI: integer;
                    Flog: TEXTFILE;
                    Urb_min_km: real;
                    T_NORM: integer;
                    T_UCHASTOK: STRING;

                    DatKms, PrkKms: masint;
                    SName, DName: String;
                    EndNumKm, BeginNumKm, AIndex, BIndex: integer;
                    D_rkm0, D_rkm: integer;
                    D_mtr: integer;
                    Izk, zkm, zkmtrue: integer;
                    Npk1, Kpk1, Kpk2, Npk2, NNB1, NNB2, KNB1, KNB2, ski,
                      iiii: integer;
                    flgA, flgB: boolean;
                    KFnc: MFuncs;
                    MasKmTrue: masint;
                    // -----------------------------
                    Glb_PutList_PCH, Glb_PutList_Put, Glb_PList_PCHU,
                      Glb_PList_PD, Glb_PList_PDB, Glb_PutList_ind: integer;
                    Glb_PutList_GNapr, Glb_Putlist_Date, Glb_PutList_StA,
                      Glb_PutList_StB, Glb_FIO_PD_master, Glb_PCH_name: string;
                    Glb_GNapr, GlbDistanceId: longint;

                    Txt_PCH, Txt_GNApr, Txt_Put, fstr: string;
                    PList_GNapr, PList_Naprav: string;
                    PList_PCH, PList_Put, PList_PCHU, PList_PD,
                      PList_PDB: integer;
                    a_km, a_mtr, b_km, b_mtr: integer;

                    PutList: MasPutList;
                    FileExist, FileExist1: boolean;
                    PL2: TEXTFILE;

                    info_put_file, km_shifrovka_file, ff: TEXTFILE;
                    Dateofinser: string;
                    DIRECTION, indi: integer;
                    Rem: MasRem;
                    GlbAmtr, GlbBmtr, GlbLnk, GlbVoz, GlbNrs, GlbLpk1, GlbLpk2,
                      GlbNrm, GlbIzn, GlbRad, GlbKrvCenter: integer;
                    GlbInfKrivoi, GlbPT1, GlbPT2, GlbPT3, GlbPT4, GlbPT5,
                      GlbPT6, GlbPT7, GlbPT8, GlbPT9, GlbPT10: string;
                    Jagdai, TempPCH: string;
                    Glob_page_GReport: integer;

                    Global_radius, Global_krv_nach: integer;
                    Global_Krv_Fsr: real;
                    GlobNturKrv: MasNaturKriv;
                    FoldDATA_DB: string;

                    GlbParamShifNesOtbBoz: integer = 1;
                    flagNadpisOgr: boolean = false;
                    // *******************************************************************
                    UPrm: UPriamoy;
                    UPuch: UPuchenie;
                    UKrv, UKrv_: UKrivoy;
                    UAdm: UAdmin;
                    URas: URasp;
                    IsoJoint: UMainTrackObject;
                    JointlessPath: UMainTrackObject;
                    UStr: UStrel;
                    UStr2: UStrel2;
                    UNes: UNesuKm;
                    UNst: UNestKm;
                    USkr: USkorost;

                    UMT: UMostTonnel;
                    UShp: UShpal;
                    StopFlag: boolean;
                    flgStrel: boolean = false;
                    Korreksya: integer = 0;
                    GlobPassSkorost, GlobGruzSkorost, RGlobPassSkorost,
                      RGlobGruzSkorost: integer;
                    GDatVremForBedomost: TDateTime;
                    GTripId: longint;
                    GTipPoezdki: smallint;
                    GlbTimerClickCount: integer = 0;
                    GlbKrvEnable: boolean = false;

                    GlbKrvRihMax: real = 0;
                    GlbKrvUrvMax: real = 0;
                    GlbKrvZnak: integer = 0;
                       Side_Str: integer = 10;
                    DeltaForNatur_urb: integer = 0;
                    DeltaNaturKPK_urb: integer = 0;

                    DeltaForNatur_rih: integer = 0;
                    DeltaNaturKPK_rih: integer = 0;

                    DeltaNPK_urb: integer = 0;
                    DeltaKPK_urb: integer = 0;
                    DeltaNPK_rih: integer = 0;
                    DeltaKPK_rih: integer = 0;

                    GlbSushKrvFlg: boolean = false;

                    Glob_H: real = 0.0;

                    Shab_s2: integer = 0;
                    Shab_s3: integer = 0;
                    Shab_s4: integer = 0;

                    suj_s2: integer = 0;
                    Suj_s3: integer = 0;
                    Suj_s4: integer = 0;
                    ush_s2: integer = 0;
                    ush_s3: integer = 0;
                    ush_s4: integer = 0;

                    rih_s2: integer = 0;
                    rih_s3: integer = 0;
                    rih_s4: integer = 0;
                    rih1_s2: integer = 0;
                    rih1_s3: integer = 0;
                    rih1_s4: integer = 0;
                    rih2_s2: integer = 0;
                    rih2_s3: integer = 0;
                    rih2_s4: integer = 0;

                    Urb_s2: integer = 0;
                    Urb_s3: integer = 0;
                    Urb_s4: integer = 0;
                    per_s2: integer = 0;
                    per_s3: integer = 0;
                    per_s4: integer = 0;
                    pot_s2: integer = 0;
                    pot_s3: integer = 0;
                    pot_s4: integer = 0;

                    pro_s2: integer = 0;
                    pro_s3: integer = 0;
                    pro_s4: integer = 0;
                    pro1_s2: integer = 0;
                    pro1_s3: integer = 0;
                    pro1_s4: integer = 0;
                    pro2_s2: integer = 0;
                    pro2_s3: integer = 0;
                    pro2_s4: integer = 0;

                    ind_prim: integer = 0;
                    Glob_primech: string = ' ';

                    flag_dlya_Krivoi: boolean = false;
                    pik, xcoord: integer;
                    GlbProcTip: integer = 0;

                    ind_ubed: integer = 0;
                    global_flag_dlya_tbed: boolean = false;
                    // *******************************************************************

                    GlbMaxRih: real = 0.0;
                    thetta: integer = 0;

                    GlbSkorostPrim: string = ' ';
                    GlbFlagOts_2st: boolean = false;

                    GlbOgrSkorKm: boolean = false;
                    GlobBall: integer = 0;

                    GlbMinOgrSk: integer = 140;
                    GlbMinOgrSkGrz: integer = 90;

                    GlbMinOgrSk_: integer = 140;

                    GlbMinOgrSkCoordA: integer = 0;
                    GlbMinOgrSkCoordB: integer = 0;
                    GlbOgrSkType: integer = 0;

                    GlobUbedOgr: string = '';
                    UbedOgr: string = '';
                    UbedOgr4jok: string = '';
                    UbedOgr4jok_V: integer = 120;
                    GlbMinOgrSk4: integer = 120;

                    GlobLentaS: string = '';

                    GlbOgrPasGrz: string = '-/-';
                    Glb_vop: integer = 999;
                    Glb_vog: integer = 999;

                    cnt_ush4, cnt_suj4, cnt_urv4, cnt_per4, cnt_pr1, cnt_pr2,
                      cnt_rih: integer;
                    p_ush4, p_suj4, p_urv4, p_per4, p_pr1, p_pr2, p_rih,
                      str_4st, end_ubed: string;

                    GlbCou_3stShab_UklOtb: integer = 0;
                         GlbFlagPerUbed: boolean = false;
                    GlbFlagRihtUbed: boolean = false;
                    GlbFlagRemontKm: boolean = false;
                    GlbSkorostRemontKm: integer = 120;

                    gnkm_rem, gkkm_rem, ppp: integer;

                    Path_info_put_file, Path_km_shifrovka_file: string;

                    glbTypeKorrOrRemont: string;
                    GlbTempSkorRemontKm: integer = 15;

                    GlbTempTipRemontKm: string = '';
                    GlbTempTipRemontPiket: string = '';

                    I_ListV: integer = 0;
                    gRemInd: integer = 0;

                    glb_BirdeiKmFlag: boolean = false;
                    glb_BirdeiKmCount: integer = 0;

                    ball500_sebep: string = ' ';

                    GlbCommentPaspData: string = ' ';

                    flagpic: boolean;

                    GlobCountObrKm: real = 0;
                    GlobCountObrKmReal: real = 0;

                    GlbFlagOgrSkorosti: boolean = false;
                    GlbremontTekPik: integer = 1;

                    GlbCountRemKm, GlbCountUKL, GlbCountUSK, GlbCountSOCH,
                      GlbCountDGR: smallint;
                    glbTipOgrV: integer = 0;

                    glbCount_Rih4s, glbCount_Ush4s, glbCount_suj4s,
                      glbCount_Urv4s, glbCount_Per4s, glbCount_1Pro4s,
                      glbCount_2Pro4s: integer;

                    GJyl, GAyS: string;

                    GlbNumPut: string = '1';
                    FirGlbNumPut: string = '1';
                    Length_all_Km: real = 0;
                    Length_Km: real = 0;
                    GlbBirdeiKmLen: real = 0;
                    gk: integer = 0;
                    gkmtrueind: integer = 0;
                    gkmtrue: integer = 0;

                    GlbTypeGrp: string = 'Оригинал'; // Дубликат
                    Glb3: string = '';
                    vt: integer = 80;

                    POP_flg: boolean = false;
                    END_km: boolean = false;
                    AUYSU_flg: boolean = false;
                    aldyngy_km: integer = 0;

                    mVPik: VNPik;
                    GlbFlagCorrKm: boolean = false;
                    ald_put: integer = 0;
                    // Flag_sablog : boolean ;

                    GNstdKmLen: integer = 0;
                    GNstCount: integer = 0;

implementation

// =========================================
// Процедура записи на лог файл
// ========================================
procedure SabLog(NamePnt: string);
begin
    try

        if not FileExists(PathMB + 'LOGINFO\' + 'LogFile.txt') then
        begin
            Writeln(PathMB + 'LOGINFO\' + 'LogFile.txt');
            assignfile(Flog, PathMB + 'LOGINFO\' + 'LogFile.txt');
            rewrite(Flog);
            closefile(Flog);
        end;
        assignfile(Flog, PathMB + 'LOGINFO\' + 'LogFile.txt');
        append(Flog);
        CountInLog := CountInLog + 1;
        Writeln(Flog, CountInLog, ' : ' + NamePnt);
        flush(Flog);
        closefile(Flog);
    finally

    end;
end;

// ------------------------------------------------
procedure CleanerTemp;
var
    s, r: string;
    APath: string;
    MySearch: TSearchRec;
begin
    try
        s := GetEnvironmentVariable('TEMP');
        r := StringReplace(s, 'DOCUME~1', 'Documents and settings',
          [rfReplaceAll]);
        r := StringReplace(r, 'LOCALS~1', 'Local Settings', [rfReplaceAll]);

        APath := r;
        FindFirst(APath + '\*.*', faAnyFile + faReadOnly, MySearch);
        DeleteFile(APath + '\' + MySearch.Name);
        while FindNext(MySearch) = 0 do
        begin
            DeleteFile(APath + '\' + MySearch.Name);
        end;
        FindClose(MySearch);

    finally
    end;

end;

// ------------------------------------------------------------------------------
function GetRemKorKm(km: integer): boolean;
var
    vmin, i: integer;
    tb, newRem: boolean;
    tmp_km, tmp_pic, tmp_v: integer;
    tmp_adat, tmp_bdat: string;
    tmp_type, tmp_put, ip: string;
    puts: string;
    ak, bk, xk: integer;

    fffff: TEXTFILE;
begin
    tb := false;

    GlbSkorostRemontKm := GlobPassSkorost;
    GlbTempSkorRemontKm := GlobPassSkorost;

    newRem := false;
    if (GlobCountObrKm <= 1) or FileExists('fr.tmp') then
    begin
        newRem := true;
        DeleteFile('fr.tmp');
    end;

    if FileExists('RFile.txt') { and newRem } then
    begin
        assignfile(fffff, 'RFile.txt');
        reset(fffff);

        vmin := GlobPassSkorost;
        i := 0;
        // setlength(PL_Putrabota, 100);

        while not eof(fffff) do
        begin
            readln(fffff, tmp_adat);
            readln(fffff, tmp_bdat);
            readln(fffff, tmp_put);
            readln(fffff, tmp_km);
            readln(fffff, tmp_pic);
            readln(fffff, tmp_v);
            readln(fffff, tmp_type);

            // PL_Putrabota[i].adat  :=  tmp_adat; PL_Putrabota[i].bdat  :=  tmp_bdat;
            { if (tmp_adat <= datetostr(date)) and (datetostr(date) <= tmp_bdat) then
              begin
              PL_Putrabota[i].put   :=  tmp_put;
              PL_Putrabota[i].rtype :=  tmp_type;
              PL_Putrabota[i].km    :=  tmp_km;
              PL_Putrabota[i].pik   :=  tmp_pic;
              PL_Putrabota[i].v     :=  tmp_v;
              end; }

            if (km = tmp_km) and (GlbNumPut = tmp_put) and
              (tmp_adat <= datetostr(date)) and (datetostr(date) <= tmp_bdat)
            { and (tmp_pic = 0) } then
            begin
                tb := true;
                ip := '';
                if tmp_pic > 0 then
                    ip := ' пк' + inttostr(tmp_pic);
                if vmin > tmp_v then
                    vmin := tmp_v;
                GlbTempTipRemontKm := tmp_type + ip + ' V=' +
                  inttostr(tmp_v) + ' ';
                // GlbFlagRemontKm:= true;
                GlbCountRemKm := GlbCountRemKm + 1;
            end;
            i := i + 1;
        end;
        // GlobPassSkorost:= vmin;
        // GlobGruzSkorost:= vmin;
        GlbSkorostRemontKm := vmin;
        GlbTempSkorRemontKm := vmin;
        // setlength(PL_Putrabota, i);
        closefile(fffff);
    end;
    {
      for i := 0 to high(PL_Putrabota) do
      begin
      puts:= PL_Putrabota[i].put;
      xk:=  PL_Putrabota[i].km;
      // ak:= PL_Putrabota[i].km*1000 +  (PL_Putrabota[i].pik-1)*100;
      // bk:= PL_Putrabota[i].km*1000 +  (PL_Putrabota[i].pik-1)*100 + 99;

      if (numput = strtoint(puts)) and (km = xk) // and (ak <= xk) and (xk <= bk)
      then
      begin
      GlobPassSkorost:= PL_Putrabota[i].v;
      GlobGruzSkorost:= PL_Putrabota[i].v;
      GlbSkorostRemontKm:= PL_Putrabota[i].v;
      GlbTempSkorRemontKm:= PL_Putrabota[i].v;
      //GlbFlagRemontKm:= true;
      tb:= true;
      break;
      end;
      end;
    }
    GetRemKorKm := tb;
end;

// ------------------------------------------------------------------------------
procedure GetCorrKm(km: integer);
var
    tmp_km, tmp_pic: integer;
    tmp_adat, tmp_type: string;
    fffff: TEXTFILE;
begin
    if FileExists('correction.txt') then
    begin
        assignfile(fffff, 'correction.txt');
        reset(fffff);

        while not eof(fffff) do
        begin
            readln(fffff, tmp_adat);
            readln(fffff, tmp_km);
            readln(fffff, tmp_pic);
            readln(fffff, tmp_type);
            // readln(fffff, tmp_v);

            if (km = tmp_km) and (tmp_adat = datetostr(date)) then
            begin
                GlbFlagCorrKm := true;
                GlbTempTipRemontKm := 'Коррек. ' + tmp_type + ' Пк' +
                  inttostr(tmp_pic) + ' ';
                if tmp_pic = 0 then
                    GlbTempTipRemontKm := 'Коррек. ' + tmp_type + ' ';
            end;
        end;
        closefile(fffff);

    end;
end;

// ------------------------------------------------------------------------------
// Жок км-ге тексеру
// ------------------------------------------------------------------------------
function ZhokKm(pkm: integer): boolean;
var
    i: integer;
    tbool: boolean;
begin
    tbool := false;
    for i := 0 to high(UNes) do
    begin
        if (Glb_PutList_PCH = UNes[i].pch) and (GlbNumPut = UNes[i].put) and
          (UNes[i].km = pkm) then
        begin
            tbool := true;
            break;
        end;
    end;
    ZhokKm := tbool;
end;

// ---------------------------------------------------

end.
