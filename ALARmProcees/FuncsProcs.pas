 unit FuncsProcs;

interface

uses Dialogs, Windows, Messages, SysUtils, Params, Forms, strutils, math,
  DateUtils;

Type

  masf = array of record f: real;
x:
integer;
end;
Extremum = record value: real;
index, sign: integer;
end;
remkm = set of byte;
skor = record Nc, Ec, Pv, Gv: integer;
end;
priam = record Nc, Ec, Nm, Rn: integer; // Rn - ðèõò. íèòü; Nm -íîðìà
end;
kriv = record Nc, Ec, Lk, Vz, Nr, Lp1, Lp2, Nm, Rd: integer;
end;
rvpuch = record Nc, Ec, Np: integer; // Np- íèòü ðàâí. ïó÷
end;
most = record Nc, Ec: integer; //
end;
jelez = record Nc, Ec: integer; //
god:
string;
end;

OtsInf = record st, bel, L0, Lm, Leng, count, v, vg, vop, vog, Vrp, Vrg,
  transitionIndex: integer;
s3, val: real;
flg, isriht, onswitch, isEqualTo4, isEqualTo3, isLong, iso_joint: boolean;

prim:
string;
end;

RCoor = record km, x, y: real;
end;

rmas = array of RCoor;

TFun = record km, fun, v: integer;
end;
MTFun = array of TFun;

TFun1 = record km, v, Nrm, Shpal, Rad: integer;
fun, f0: real;
end;
MTFun1 = array of TFun1;
{
  TUbd = record
  pik,mtr,leng,v:integer;
  f:real;
  rem,mst,str:boolean
  end;
  Ubd = array of TUbd; }

Matrix1 = array [1 .. 3, 1 .. 4, 1 .. 5] of integer;
Matrix2 = array [0 .. 1, 1 .. 2, 1 .. 4] of integer; // v1
Matrix3 = array [1 .. 2, 1 .. 2, 1 .. 5] of integer; // v2
Matrix4 = array [0 .. 1, 1 .. 2, 1 .. 6] of integer; // v3
MatrixLevDrawSkew = array [1 .. 4, 1 .. 5] of integer;
Matrix5 = array [1 .. 3, 1 .. 5] of integer;
MatrixGauge = array [1 .. 4, 1 .. 3, 1 .. 5] of integer;
// Altynbek
MatrixR8 = array [1 .. 3] of integer;

MatrixUR_Per = array [1 .. 3, 1 .. 5] of integer;
// новая матрица  перекоса вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
Matrix_Riht = array [1 .. 3, 1 .. 6] of integer;
// новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек

Matrix_Riht_New = array [1 .. 3, 1 .. 5] of integer;

Matrix6 = array [1 .. 16] of integer;
Matrix7 = array [1 .. 3, 1 .. 2, 1 .. 6] of integer;
Matrix11 = array [1 .. 2, 1 .. 5] of integer;
Matrix12 = array [1 .. 8] of string[3]; // ogSk
Matrix13 = array [1 .. 2, 1 .. 8] of integer; // ogSk
Matrix14 = array [1 .. 2, 1 .. 4] of integer;
Matrix15 = array [1 .. 3, 1 .. 6] of integer;
//
MatrixNew1 = array [1 .. 3, 1 .. 6] of integer;
MatrixNew2 = array [1 .. 3, 1 .. 6] of integer; // v1

//

Vector1 = array [1 .. 4] of integer;
Vector2 = array [1 .. 5] of integer;
Vector3 = array [1 .. 6] of integer;
Vector5 = array [1 .. 15] of string[3];
Vector6 = array [1 .. 8] of string[3];
Vector7 = array [1 .. 8] of integer;
Vector8 = array [1 .. 6] of string[3];
Vector9 = array [1 .. 9] of string[3];
Vector10 = array [1 .. 10] of string[3];

masots = array of OtsInf;
masi = array [0 .. 1000] of integer;
mas = array of integer;
masr = array of real;
mas2 = array of string[3];
mas3 = array of string[50];
mas4 = array of string[5];
maszd = array [1 .. 8] of string[40];
Tp_s = string[3];
strarr = array of string;

procedure SaveToUvedomlenie(ogranichenie_v, tip: integer; s: string;
  WUsh: OtsInf);

// function GetTuzu(var xx,ux1,ux2,uy1,uy2:integer):integer; stdcall; External 'KTJ.dll';
// function G_UkOt(var Fx:integer):integer;  stdcall;  External 'KTJ.dll';
// function AlpS(var Nms,Radius:integer):integer; stdcall; External 'KTJ.dll';
// function psi(var pa1,pb1,pa2,pb2:integer):boolean; stdcall; External 'KTJ.dll';
// ------------------------------------------------------------------------------
// function GShir(var xx,x0,xn,lp1,lp2,Nrm,Rad:integer):integer; stdcall; External 'KTJ.dll';
// function GUrb(var xx,x0,xn,lp1,lp2,NrRs,Voz,Nit:integer):integer; stdcall; External 'KTJ.dll';
// function GF0Rih(var xx,x0,xn,lp1,lp2,Rad:integer):integer; stdcall; External 'KTJ.dll';
// function RavPuch(var xx,x0,xn,NitRp,Nit:integer):integer; stdcall;  External 'KTJ.dll';
// ------------------------------------------------------------------------------
  function Convert(s: string): String;

    procedure GUsh_1548mm(FG: masr; FGm, Prds: mas; var WUsh: masots);
      procedure GUsh_4(FG: masr; FGm, Prds: mas; var WUsh: masots);
        procedure GUsh_44(FG: masr; FGm, Prds: mas; var WUsh: masots);
          procedure GUsh_3(FG: masr; FGm, Prds: mas; var WUsh: masots);
            procedure GUsh_2(FG: masr; FGm, Prds: mas; var WUsh: masots);
              procedure GUsh_1(FG: masr; FGm, Prds: mas; var WUsh: masots);
                procedure GetFsuj_1512mm(FG: masr; FGm, Prds, Shp: mas;
                  var WSuj: masots);
                  procedure GetFsuj_4(FG: masr; FGm, Prds, Shp: mas;
                    var WSuj: masots);
                    procedure GetFsuj_3(FG: masr; FGm, Prds, Shp: mas;
                      var WSuj: masots);
                      procedure GetFsuj_2(FG: masr; FGm, Prds, Shp: mas;
                        var WSuj: masots);
                        procedure GetFsuj_1(FG: masr; FGm, Prds, Shp: mas;
                          var WSuj: masots);

                        // 27.08.2015:
                          function GetCountSuj3(FG: masr; FGm, Prds, Shp: mas;
                            speed: integer): integer;
                            function GetCountUsh3(FG: masr; FGm, Prds: mas;
                              speed: integer): integer;

                              procedure GetRiht(Fm, FmK, FmTrapez: mas;
                                var WRih: masots; isriht: boolean);
                                procedure Fluc_Define(right, left: mas;
                                  xcoord: MFuncs2);
                                  function LengthInMetr(ots: OtsInf): integer;

                                    procedure RUroven_4(FG, FGm, Pnom: mas;
                                      var WUsh: masots);
                                      procedure GUroven_2(FG, FGm, Pnom: mas;
                                        var WUsh: masots);
                                        procedure GUroven_3(FG, FGm, Pnom: mas;
                                        var WUsh: masots);
                                        procedure GUroven_4(FG, FGm, Pnom: mas;
                                        var WUsh: masots);
                                        procedure GUroven_4_PaspData(FG,
                                        FGm: mas; Pnom: masr);

                                        procedure GUroven150(FG, FGm, Pnom: mas;
                                        var WUsh: masots);
                                        procedure GUroven75(FG, FGm, Pnom: mas;
                                        var WUsh: masots);
                                        procedure GUroven4(FG, FGm, Pnom: mas;
                                        var WUsh: masots);
                                        procedure GUroven3(FG, FGm, Pnom: mas;
                                        var WUsh: masots);
                                        procedure GUroven2(FG, FGm, Pnom: mas;
                                        var WUsh: masots);
                                        procedure GUroven1(FG, FGm, Pnom: mas;
                                        var WUsh: masots);

                                        procedure GetFpro(nt: string;
                                        Fm, FmK, FmV: mas; var WPro: masots);
                                        procedure GetPerekos(Fm, Fm_sr,
                                        FmK: mas; var WPer: masots);
                                        procedure GetPerekosLong(Fm, Fm_sr,
                                        FmK: mas; var WPer: masots);
                                        procedure Get_UU(Fm, FmK: mas);
                                        // new 10-09-2010

                                        procedure GetPro(nt: string;
                                        Fm, FmK: mas; var WPro: masots);

                                        procedure GetPro_Perekos(nt: string;
                                        Fm, FmK,FmTtrapezLevel,FmAvgTr: mas; var WPer: masots);

                                        // new 13-09-2010
                                        // procedure GetRihtovka(Fm,FmK:mas;var WPro:masots);

                                        // procedure GetFpot_new(FG,FGm,FgV:mas;var WPot:masots);
                                        procedure Tizim_sur(ptz: mas3;
                                        var gtz: mas3);
                                        procedure GetFtobe(FG, FGm, FgV: mas;
                                        var Ft, FtK, FtV: mas); // Far; +

                                        // ------------------------------------------------------------------------------
                                        // test proc
                                        // function GTF_su(var px,pf0,pna,pnb:integer; tsX,tsBel,tsDl,tsots:mas):integer;
                                        //
                                        // функция выдачи ограничения
                                        function OpredelenieOgr_SkorostiPer
                                        (var st, otkl: integer): integer;
                                        function OpredelenieOgr_SkorostiPro
                                        (var st, otkl, x, y,
                                        j: integer): integer;

                                        function OpredelenieOgr_SkorostiUsh
                                        (var st, otkl, x, y,
                                        j: integer): integer;
                                        function OpredelenieOgr_SkorostiSuj
                                        (var st, otkl, x, y,
                                        j: integer): integer;
                                        function OpredelenieOgr_SkorostiSuj_Uroben
                                        (var st, otkl, x, y,
                                        j: integer): integer;
                                        function OpredelenieOgr_SkorostiPro_Riht
                                        (var st, otkl, x, y,
                                        j: integer): integer;

                                        // altynbek remont km ushin
                                        function OpredelenieOgr_SkorostiRem_Uroben
                                        (var otkl: integer): integer;
                                        function G_Ind2_Remont(var s: string;
                                        Var v: integer): integer;
                                        // altynbek remont km ushin
                                        function G_Ind2_Skorost
                                        (var j: integer): string;
                                        function G_Ind2_skorost2(Pv,
                                        j: integer): integer;
                                        function G_Ind2_skorost2Shab
                                        (var j: integer): integer;
                                        function G_Ind2_skorost2Remont
                                        (var s: string; var j: integer)
                                        : integer;
                                        function G_Ind2_skorost2_Riht(Pv,
                                        j: integer): integer;

                                        function IndexV(Pv, v: integer)
                                        : integer;
                                        function IndexV_pros(Pv,
                                        v: integer): integer;

                                        function OpredelenieOgr_UklOtbShablon
                                        (var otkl: real): integer;

                                        function Most_Tonnel_Skorosti_Uroben
                                        (var st, otkl, x, y,
                                        j: integer): integer;
                                        function Most_Tonnel_Skorosti_Prosadka
                                        (var st, otkl, x, y,
                                        j: integer): integer;
                                        function Most_Tonnel_Skorosti_Perekos
                                        (var st, otkl, x, y,
                                        j: integer): integer;
                                        function Most_Tonnel_Skorosti_Rihtovka
                                        (var st, otkl, x, y,
                                        j: integer): integer;

                                        function OpredelenieOgr_Skorosti_Rihtovka
                                        (var otkl, dl: integer): integer;

                                        procedure S4_comment(k, pik: integer);
                                        function V_ogr_rih(L,
                                        H: integer): integer;
                                        function V_ogr_rih_tab_3dot3(Pv, v, L,
                                        H: integer;
                                        onJoint: boolean = false): integer;
                                        function V_ogr_rih_tab_3IsEqualTo4(Pv,
                                        v, L, H: integer): integer;
                                        function V_ogr_UPP(Pv, v, H, g: integer;
                                        iso_joint: boolean = false): integer;

                                        function Velich_table_3dot3(x, v,
                                        k: integer): real;
                                        function V_res(s: string;
                                        p, v: integer): integer;
                                        function Shekteu(a, b, vop,
                                        vog: integer): boolean;
                                        function V_shekti(v1,
                                        v2: integer): string;

                                        //

                                        Const
                                        n = 5;
                                        n1 = 6;
                                        m = 4;
                                        La = 600; // sm
                                        Lb = 2000; // sm
                                        Iz_step = 100; // sm
                                        Per_const = 10; // mm
                                        Ch_per = 3; // mm
                                        Rs_con = 10; // mm
                                        Ch_rs = 5; // mm
                                        path1 = 'MALMET\';
                                        path2 = 'Malmet\';
                                        path3 = 'd:\ALFA\';
                                        path4 = 'd:\OTCHET\';

                                        FileName: string = 'WM_Data.www';

                                        Vu1: Matrix2 =
                                        (((0, 26, 61, 101), (25, 60, 100, 140)),
                                        ((0, 26, 61, 81), (25, 60, 80, 90)));
                                        Vu2: Matrix3 =
                                        (((0, 16, 41, 61, 121),
                                        (15, 40, 60, 120, 140)),
                                        ((0, 16, 41, 61, 81),
                                        (15, 40, 60, 80, 90)));

                                        Vu3: Matrix4 =
                                        (((0, 16, 41, 61, 81, 121),
                                        (15, 40, 60, 80, 120, 140)),
                                        ((0, 16, 41, 61, 71, 81),
                                        (15, 40, 60, 70, 80, 90)));

                                        // Vu4: Matrix6 = ((120,110,100,90,85,80,75,70,65,60,55,50,40,25,0),
                                        // (140,120,110,100,90,85,80,75,70,65,60,55,50,40,25));

                                        Vu4: Matrix6 = (140, 120, 110, 100, 90,
                                        85, 80, 75, 70, 65, 60, 55, 50,
                                        40, 25, 0);

                                        Rnom: Matrix11 =
                                        ((1520, 1521, 1523, 1531, 1536),
                                        (1520, 1524, 1530, 1535, 1540));

                                        DeltaUsh: MatrixGauge =
                                        (((14, 10, 10, 6, 6),
                                        (16, 14, 11, 9, 6),
                                        (20, 16, 17, 11, 6)),

                                        ((16, 12, 12, 9, 6),
                                        (18, 14, 14, 10, 8),
                                        (22, 18, 16, 11, 8)),

                                        ((18, 14, 12, 9, 6),
                                        (20, 16, 14, 10, 8),
                                        (24, 20, 16, 11, 8)),

                                        ((18, 14, 14, 10, 7),
                                        (22, 16, 16, 11, 8),
                                        (26, 22, 16, 13, 8)));

                                        DeltaSuj: MatrixGauge =
                                        (((6, 8, 8, 8, 16), (7, 10, 10, 12, 18),
                                        (8, 12, 12, 15, 20)),

                                        ((6, 10, 10, 12, 16),
                                        (7, 11, 14, 16, 18),
                                        (8, 12, 15, 18, 20)),

                                        ((6, 10, 10, 12, 16),
                                        (7, 11, 14, 16, 18),
                                        (8, 12, 15, 18, 20)),

                                        ((6, 10, 10, 14, 16),
                                        (7, 11, 14, 16, 18),
                                        (8, 12, 15, 18, 20)));
                                        // Altinbek remont km ushin
                                        RDeltaPer: MatrixR8 = (20, 25, 30);
                                        RDeltaUrb: MatrixR8 = (20, 30, 40);
                                        RDeltaSmStr: MatrixR8 = (35, 50, 65);

                                        // Altinbek remont km ushin

                                        DeltaPer: MatrixLevDrawSkew =
                                        ((7, 9, 11, 13, 35),
                                        (9, 10, 14, 16, 35),
                                        (12, 14, 20, 25, 35),
                                        (16, 20, 25, 30, 50));

                                        // (  старый вариант до 01,06,2021
                                        // ( 7,  9, 11, 13, 13),
                                        // ( 9, 10, 14, 16, 16),
                                        // (12, 16, 20, 25, 30),
                                        // (16, 20, 25, 30, 50));

                                        DeltaUrb: MatrixLevDrawSkew =
                                        ((8, 9, 11, 13, 35),
                                        (10, 12, 14, 16, 35),
                                        (16, 20, 25, 30, 35),
                                        (20, 25, 30, 35, 50));

                                        DeltaPro: MatrixLevDrawSkew =
                                        ((10, 10, 11, 13, 35),
                                        (11, 12, 14, 16, 35),
                                        (15, 20, 25, 30, 35),
                                        (20, 25, 30, 35, 45));

                                        DeltaProIsoJoint
                                        : Matrix5 = ((10, 10, 12, 15, 18),
                                        (14, 19, 24, 29, 35),
                                        (15, 20, 25, 35, 45));
                                        DeltaPro_UR_Per
                                        : MatrixUR_Per = ((8, 10, 12, 14, 16),
                                        (12, 16, 20, 25, 30),
                                        (16, 20, 25, 30, 50));
                                        // новая матрица  перекоса вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
                                        // DeltaPro_Riht: Matrix_Riht=((10,10,15,15,20,25),(15,25,35,35,40,50),(25,35,40,40,50,65)); // новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
                                        // переделка по размерности согласно полетке
                                        DeltaPro_Riht
                                        : Matrix_Riht_New =
                                        ((10, 15, 15, 20, 25),
                                        (25, 35, 35, 40, 50),
                                        (35, 40, 40, 50, 65));
                                        // новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
                                        /// //
                                        // DeltaPro_Riht1: Matrix_Riht_New=((4,5,7,8,10),(8,11,12,15,20),(11,12,15,20,31)); // новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
                                        // DeltaPro_Riht2: Matrix_Riht_New=((3,4,4,5,7),(5,6,7,10,12),(6,7,10,12,18)); // новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
                                        // DeltaPro_Riht3: Matrix_Riht_New=((4,5,6,7,8),(7,8,11,14,15),(8,11,14,16,21)); // новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
                                        DeltaPro_Riht1
                                        : Matrix_Riht_New =
                                        ((8, 11, 14, 16, 20),
                                        (16, 22, 25, 31, 40),
                                        (22, 25, 31, 40, 62));
                                        // новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
                                        DeltaPro_Riht2
                                        : Matrix_Riht_New = ((6, 7, 8, 10, 13),
                                        (10, 13, 15, 20, 25),
                                        (13, 15, 20, 25, 37));
                                        // новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек
                                        DeltaPro_Riht3
                                        : Matrix_Riht_New =
                                        ((8, 11, 13, 15, 17),
                                        (14, 16, 22, 28, 31),
                                        (17, 22, 28, 32, 43));
                                        // новая матрица  рихтовки вычисляемая с помощью процедуры просадок в случе двух вершин и без ступенек

                                        DeltaRst: Matrix7 =
                                        (((10, 10, 15, 15, 20, 25),
                                        (10, 10, 15, 15, 20, 25)),
                                        ((15, 25, 35, 35, 40, 50),
                                        (15, 25, 35, 35, 40, 50)),
                                        ((25, 35, 40, 40, 50, 65),
                                        (25, 35, 40, 40, 50, 65)));


                                        // MatrixNew1=((10,10,15,15,20,25), (15,25,35,35,40,50) ,(25,35,40,40,50,65));
                                        // MatrixNew2=((15,15,25,25,30,35), (25,35,40,40,50,65) ,(35,40,50,50,65,90));

                                        DeltaPp1: Matrix15 =
                                        ((7, 10, 10, 13, 16, 19),
                                        (4, 6, 6, 8, 10, 12),
                                        (5, 7, 8, 9, 11, 14));
                                        DeltaPp2: Matrix15 =
                                        ((10, 16, 22, 25, 31, 40),
                                        (7, 10, 14, 19, 19, 25),
                                        (11, 15, 18, 22, 29, 38));
                                        DeltaPp3: Matrix15 =
                                        ((16, 22, 25, 31, 40, 62),
                                        (10, 14, 19, 25, 25, 38),
                                        (15, 18, 22, 29, 38, 44));
                                        { Матр. радиусов при ном. шир. колеи }
                                        MRd1: Matrix14 = ((0, 1, 300, 350),
                                        (0, 299, 349, 9000)); // 1520
                                        MRd2: Matrix11 = ((0, 1, 350, 450, 650),
                                        (0, 349, 449, 649, 9000)); // 1524
                                        Rnom1: Vector1 = (1520, 1535, 1530,
                                        1520); // 1520
                                        Rnom2: Vector2 = (1524, 1540, 1535,
                                        1530, 1524); // 1524
                                        { Дельта тест. продседуры }
                                        TDelta: Vector7 = (500, 500, 200, 200,
                                        500, 700, 500, 500);
                                        // 1-ush,2-cuj,3-prop,4-prol,5-per,6-pot,7-rixp,8-rixl
                                        { Норм. шир. колеи }
                                        R: Vector2 = (1520, 1524, 1530,
                                        1535, 1540);
                                        { Наз. отс. }

                                        Var
                                        // ush_:ubd;
                                        piket1, piket2, piket3, piket4, piket5,
                                        piket6, piket7, piket8, piket9, piket10,
                                        xo: integer;
                                        prp2_p, prp3_p, prl2_p, prl3_p, ush2_p,
                                        ush3_p, sj2_p, sj3_p, rp2_p, rp3_p,
                                        rl2_p, rl3_p, p2_p, p3_p, u2_p,
                                        u3_p: string;
                                        str_p: string;

                                        p, Vn, l1, l2, ii, ii1, jj, x, nn, i, j,
                                        rsn1, BlockCounter, Nkm, x00,
                                        xnn: integer;
                                        W: real;
                                        ff0, ff1, ff2, ff3, ff4: TextFile;
                                        F_file: TextFile; // file of Fdat;

                                        // - - - - - - - - - - - - - - - - -
                                        Vk, fa, fb, fc, f0a, f0b, L0, Lm, Ln,
                                        Hn, mn, i1: integer;
                                        max2, max3, max4, k, k2, k3, k4, p21,
                                        p22, p31, p32, p41, p42, ns, tet2, tet3,
                                        tet4: integer;

                                        Fm1, Fm01, FmK1, FmV1: mas;
                                        Fm1_strela, Fm01_strela, FmK1_strela,
                                        FmV1_strela: mas;

                                        s0, s1, s2, s3, sh1, sh2, sh3, Fh,
                                        F2Bel, F2St, F2L0, F2Lm, F3Bel, F3St,
                                        F3L0, F3Lm, F4Bel, F4St, F4L0, F4Lm,
                                        kv1, Ltob1: integer;
                                        S_ot: mas2;
                                        S_ots: mas4;
                                        Msh_km, Msh_mr, Msh_sm, Msh_mm, lng,
                                        dli2, dli3, dli4: integer;
                                        x1, x2, x21, x22, y1, y2, z11, z12, z21,
                                        z22, kk, Cu1, Cu2, AfV1, AfV2, AfV3,
                                        AfS, Nms1, Rds1: integer;

                                        F_Up, F_Ul, F_Pp, F_Pl, F_Rs, F0_Up,
                                        F0_Ul, F0_Pp, F0_Pl, Nom, Rad, VSpeed,
                                        Shpaly: mas;
                                        sz1, sz2, sz3, sz4, vk1, vk2,
                                        rc_s: integer;
                                        Akikat: boolean;
                                        Jelez_do96a, Jelez_do96b: mas;
                                        L_h, pc1, pc2: integer;
                                        ZD_inf, ptz1, ptz2, ptz3, gtz1, gtz2,
                                        gtz3: mas3;
                                        s_pch, s_naprav, s_put: string;
                                        x0_km, xn_km, x0_mtr, xn_mtr, msm1,
                                        msm2, msm3, dsm1, dsm2, Nk, vkp, vkg,
                                        pp1, pp2, ip, ig, OgrP, ogrG, Og_Ball,
                                        xxx: integer;
                                        Ot_f: mas4;
                                        N_km, N_m, E_km, E_m, E_sm,
                                        ErrCode: integer;
                                        // test proc
                                        x_a, x_b, x_c, x_d, y_a, y_b, y_c, y_d,
                                        Fi_km, Fi_m, Fi_bel, Fi_dl, Fi_v, ki,
                                        Fi_ots: integer;
                                        tsX1, tsBel1, tsDl1, tsots1: mas;
                                        NzOts: string[3];
                                        px1, pf01, pf02, pf03, pf04, pf05, pf06,
                                        pna1, pnb1, QRGrKm: integer;

                                        kilt, Nachal_km, Nachal_m, Nachal_sm,
                                        CchetKm, OldKmA, OldKmB,
                                        KolObrKm: integer;
                                        KEYPRESS_A: boolean;
                                        W1Ush, W1Suj, W1Pro1, W1Pro2, W1Per,
                                        W1Pot, W1Rst, W1Rst_riht, W1Rst_notriht,
                                        W2Rst: masots;
                                        //
                                        F_V, F_Vg, F_Vrp, F_Vrg, F_rkm, F_pik,
                                        F_RAD: mas;
                                        F_Pr1, F_Pr2, F_urb, F_Rh1, F_Rh2,
                                        F0_Pr1, F0_Pr2, F0_Rh1, F0_Rh2, F0_urb,
                                        F_fluk, f_fluk_notriht, Fluk_right,
                                        Fluk_left: mas; // r;
                                        F_Sh_sred, F0_Sh, F_Sht, F_Sht11, Urob,
                                        Fxxx, TrpzStr, TrapezLevel,
                                        SideWearLeft, SideWearRight: masr;
                                        F_Norm, S_Norm, Pros1, Pros2, Shirina,
                                       AvgTr, TrapezStr, TrapezLevel_Get_per,Fsr_Sh, Tsr_Urb, F_puch,
                                        CurvePointsLevel, CurvePointsStr: mas;
                                        F_Sh, F_Sh11, F_Wear, F_metr, ST_AVG,
                                        LV_AVG, ST_NAT, LV_NAT, ST_N,
                                        LV_N { ,Tm_mtr,Tm_sh,Tm_rh1,Tm_rh2,Tm_pr1,Tm_pr2,Tm_urb }
                                        : masr;
                                        F_mtr: mas;
                                        Tm_km, Tm_rkm, Tm_pic, Tm_v: mas;
                                        Rih1, Rih2: MTFun;

                                        massiv_F0_riht, massiv_0__F0_riht,
                                        F_urb_Per, F_urb_Per_sr,
                                        F_urb_Per1: mas;

                                        T_XKM, T_Rh, Tsr_Rh, T_URB, Tsr_UROB,
                                        FncX: mas;
                                        RIHT_sr: real;
                                        MTmp: MTFun1;
                                        MTmp1, MTmp2, MTmp22: MTFun;

                                        Frih1, Frih2, Furb, Fur1, Fur2, Fpro1,
                                        Fpro2, Fsr_Urb, Fsr_rh1, Fsr_rh2,
                                        Speed_, F0_urov, F0_rih1, F0_rih2,
                                        Ftemp, Ftemp_r1, Ftemp_r2,
                                        f_facstrel: masr;
                                        angY_sred, angX_sred, Atet: masr;
                                        cprom, cprom1: real;

                                        Furb1, Furb_sr, Furbx: mas;
                                        c1p, c2p, c3p, c4p, c5p, c6p, c7p, c8p,
                                        c9p, c10p, ogr_min: integer;

                                        Vu_rih: Matrix4;
                                        fp: masf;

implementation

uses DataModule, forots;

// ------------------------------------------------------------------------------
function VTab9(typeots, value, speed: integer): integer;
{ RemTab9 = 1: BPO, RemTab9 = 2: BPO or BPR, RemTab9 = 3: BPO+BPR, RemTab9 = 4: BPO+BPR+DSP }
{ typeots = 1: uroven, typeots = 2: perekos, typeots = 3: rihtovka, typeots = 4: prosadka }
var
  v: integer;
begin
  v := -1;

  case RemTab9 of
    1:
      begin
        case typeots of
          1:
            begin
              if (value <= 20) and (speed > 60) then
                v := 60;
              if (20 < value) and (value <= 30) and (speed > 40) then
                v := 40;
              if (30 < value) and (value <= 40) and (speed > 25) then
                v := 25;
              if (40 < value) and (speed >= 25) then
                v := 0;
            end;
          2:
            begin
              if (value <= 20) and (speed > 60) then
                v := 60;
              if (20 < value) and (value <= 25) and (speed > 40) then
                v := 40;
              if (25 < value) and (value <= 30) and (speed > 25) then
                v := 25;
              if (30 < value) and (speed >= 25) then
                v := 0;
            end;
          3:
            begin
              if (value <= 35) and (speed > 60) then
                v := 60;
              if (35 < value) and (value <= 50) and (speed > 40) then
                v := 40;
              if (50 < value) and (value <= 65) and (speed > 25) then
                v := 25;
              if (65 < value) and (speed >= 25) then
                v := 0;
            end;
          4:
            begin
              if (16 < value) and (value <= 20) and (speed > 120) then
                v := 120;
              if (20 < value) and (value <= 25) and (speed > 60) then
                v := 60;
              if (25 < value) and (value <= 30) and (speed > 40) then
                v := 40;
              if (30 < value) and (speed >= 5) then
                v := 0;
            end;
        end;
      end;
    2:
      begin
        case typeots of
          1:
            begin
              if (value <= 20) and (speed > 50) then
                v := 50;
              if (20 < value) and (value <= 30) and (speed > 25) then
                v := 25;
              if (30 < value) and (value <= 40) and (speed > 15) then
                v := 15;
              if (40 < value) and (speed >= 15) then
                v := 0
            end;
          2:
            begin
              if (value <= 20) and (speed > 50) then
                v := 50;
              if (20 < value) and (value <= 25) and (speed > 25) then
                v := 25;
              if (25 < value) and (value <= 30) and (speed > 15) then
                v := 15;
              if (30 < value) and (speed >= 15) then
                v := 0;
            end;
          3:
            begin
              if (value <= 35) and (speed > 50) then
                v := 50;
              if (35 < value) and (value <= 50) and (speed > 25) then
                v := 25;
              if (50 < value) and (value <= 65) and (speed > 15) then
                v := 15;
              if (65 < value) and (speed >= 15) then
                v := 0;
            end;
          4:
            begin
              if (16 < value) and (value <= 20) and (speed > 120) then
                v := 120;
              if (20 < value) and (value <= 25) and (speed > 60) then
                v := 60;
              if (25 < value) and (value <= 30) and (speed > 40) then
                v := 40;
              if (30 < value) and (speed >= 5) then
                v := 0;
            end;
        end;
      end;
    3:
      begin
        case typeots of
          1:
            begin
              if (value <= 20) and (speed > 60) then
                v := 60;
              if (20 < value) and (value <= 30) and (speed > 40) then
                v := 40;
              if (30 < value) and (value <= 40) and (speed > 25) then
                v := 25;
              if (40 < value) and (speed >= 15) then
                v := 0
            end;
          2:
            begin
              if (value <= 20) and (speed > 60) then
                v := 60;
              if (20 < value) and (value <= 25) and (speed > 40) then
                v := 40;
              if (25 < value) and (value <= 30) and (speed > 25) then
                v := 25;
              if (30 < value) and (speed >= 15) then
                v := 0;
            end;
          3:
            begin
              if (value <= 35) and (speed > 60) then
                v := 60;
              if (35 < value) and (value <= 50) and (speed > 40) then
                v := 40;
              if (50 < value) and (value <= 65) and (speed > 25) then
                v := 25;
              if (65 < value) and (speed >= 25) then
                v := 0;
            end;
          4:
            begin
              if (16 < value) and (value <= 20) and (speed > 120) then
                v := 120;
              if (20 < value) and (value <= 25) and (speed > 60) then
                v := 60;
              if (25 < value) and (value <= 30) and (speed > 40) then
                v := 40;
              if (30 < value) and (speed >= 5) then
                v := 0;
            end;
        end;
      end;
    4:
      begin
        case typeots of
          1:
            begin
              if (value <= 20) and (speed > 70) then
                v := 70;
              if (20 < value) and (value <= 30) and (speed > 50) then
                v := 50;
              if (30 < value) and (value <= 40) and (speed > 25) then
                v := 25;
              if (40 < value) and (speed >= 15) then
                v := 0
            end;
          2:
            begin
              if (value <= 20) and (speed > 70) then
                v := 70;
              if (20 < value) and (value <= 25) and (speed > 50) then
                v := 50;
              if (25 < value) and (value <= 30) and (speed > 25) then
                v := 25;
              if (30 < value) and (speed >= 25) then
                v := 0;
            end;
          3:
            begin
              if (value <= 35) and (speed > 70) then
                v := 70;
              if (35 < value) and (value <= 50) and (speed > 50) then
                v := 50;
              if (50 < value) and (value <= 65) and (speed > 25) then
                v := 25;
              if (65 < value) and (speed >= 25) then
                v := 0;
            end;
          4:
            begin
              if (16 < value) and (value <= 20) and (speed > 120) then
                v := 120;
              if (20 < value) and (value <= 25) and (speed > 60) then
                v := 60;
              if (25 < value) and (value <= 30) and (speed > 40) then
                v := 40;
              if (30 < value) and (speed >= 5) then
                v := 0;
            end;
        end;
      end;
  end;

  VTab9 := v;
end;

// ------------------------------------------------------------------------------
// Shektelgen zhyldamdyktardy text turine ozgertip, olardy biriktiru
// ------------------------------------------------------------------------------
function V_shekti(v1, v2: integer): string;
var
  v11, v12: string;
begin
  v11 := '-';
  v12 := '-';

  if (0 <= v1) and (v1 < 250) and (GlobPassSkorost > v1) then
    v11 := inttostr(v1);
  if (0 <= v2) and (v2 < 250) and (GlobGruzSkorost > v2) then
    v12 := inttostr(v2);

  // if (0 <= v1) and (v1 < 250) and (GlobPassSkorost > v1) then v11:= inttostr(v1);
  // if (0 <= v2) and (v2 < 250) and (GlobGruzSkorost > v2) then v12:= inttostr(v2);

  V_shekti := v11 + '/' + v12;
end;

// ------------------------------------------------------------------------------
// Жылдамдык шектеу
// ------------------------------------------------------------------------------
function Shekteu(a, b, vop, vog: integer): boolean;
var
  x, xx, vp, vg, vr, Vrg, i: integer;
  ret: boolean;
begin
  x := round((a + b) / 2);
  ret := false;

  for i := 0 to high(F_V) do
  begin
    xx := F_mtr[i]; // mod 1000;
    vp := F_V[i];
    vg := F_Vg[i];
    vr := F_Vrp[i];
    Vrg := F_Vrg[i];

    if (abs(xx - x) <= 3) and (vop < vp) and (0 < vr) and (vop < vr) then
    begin
      ret := true;
      break;
    end;

    if (abs(xx - x) <= 3) and (vog < vg) and (0 < Vrg) and (vog < Vrg) then
    begin
      ret := true;
      break;
    end;
  end;
  Shekteu := ret;
end;

// ------------------------------------------------------------------------------
// Жылдамдык шектеу 22.10.2011
// ------------------------------------------------------------------------------
function botos(b: boolean): String;
begin
  if (b = true) then
    botos := 'true'
  else
    botos := 'false';
end;

function V_res(s: string; p, v: integer): integer;
var
  tv: integer;
begin
  // p = 0 pass, p=1 gruz
  tv := v;
  if s = 'UPPR' then
  begin
    case p of
      0:
        case v of
          121 .. 140:
            tv := 120;
          61 .. 120:
            tv := 60;
          41 .. 60:
            tv := 40;
          16 .. 40:
            tv := 15;
          3 .. 15:
            tv := 0;
        end;

      1:
        case v of
          81 .. 90:
            tv := 80;
          61 .. 80:
            tv := 60;
          41 .. 60:
            tv := 40;
          16 .. 40:
            tv := 15;
          3 .. 15:
            tv := 0;
        end;
    end;
  end;

  if s = 'SHAB' then
  begin
    case p of
      0:
        case v of
          101 .. 140:
            tv := 100;
          61 .. 100:
            tv := 60;
          26 .. 60:
            tv := 25;
          3 .. 25:
            tv := 0;
        end;
      1:
        case v of
          81 .. 90:
            tv := 80;
          61 .. 80:
            tv := 60;
          26 .. 60:
            tv := 25;
          3 .. 25:
            tv := 0;
        end;
    end;
  end;
  result := tv;

end;

// ------------------------------------------------------------------------------
// Жылдамдык бойынша шаблон индексин кайтарады
// ------------------------------------------------------------------------------
function GIndexSpeedShab(Pv, v: integer): integer;
var
  k: integer;
begin

  case Pv of
    0:
      case v of
        101 .. 140:
          k := 1;
        61 .. 100:
          k := 2;
        26 .. 60:
          k := 3;
        0 .. 25:
          k := 4;
      else
        k := 1;
      end;

    1:
      case v of
        81 .. 90:
          k := 1;
        61 .. 80:
          k := 2;
        26 .. 60:
          k := 3;
        0 .. 25:
          k := 4;
      else
        k := 1;
      end;
  end;

  GIndexSpeedShab := k;
end;

// ------------------------------------------------------------------------------
// Жылдамдык бойынша urb,per,pro индексин кайтарады
// ------------------------------------------------------------------------------
function GIndexSpeed_UPP(Pv, v: integer): integer;
var
  k: integer;
begin

  case Pv of
    0:
      case v of
        121 .. 140:
          k := 1;
        61 .. 120:
          k := 2;
        41 .. 60:
          k := 3;
        16 .. 40:
          k := 4;
        0 .. 15:
          k := 5;
      else
        k := 2;
      end;
    1:
      case v of
        81 .. 90:
          k := 1;
        61 .. 80:
          k := 2;
        41 .. 60:
          k := 3;
        16 .. 40:
          k := 4;
        0 .. 15:
          k := 5;
      else
        k := 2;
      end;
  end;

  GIndexSpeed_UPP := k;
end;

// ------------------------------------------------------------------------------
// Норма бойынша шаблон индексин кайтарады
// ------------------------------------------------------------------------------
function GIndexNorm(norm: integer): integer;
var
  j: integer;
begin
  case norm of
    1520 .. 1523:
      j := 1;
    1524 .. 1529:
      j := 2;
    1530 .. 1534:
      j := 3;
    1535 .. 1539:
      j := 4;
    1540 .. 1560:
      j := 5;
  else
    j := 1;
  end;
  GIndexNorm := j;
end;

// ------------------------------------------------------------------------------
// Индекс бойынша шаблон шектеули жылдамдык кайтарады
// ------------------------------------------------------------------------------
function GOgrSpeedShab(Pv, k: integer): integer;
var
  v: integer;
begin
  case Pv of
    0:
      case k of
        1:
          v := 140;
        2:
          v := 120;
        3:
          v := 100;
        4:
          v := 60;
        5:
          v := 25;
      else
        v := 0;
      end;
    1:
      case k of
        1:
          v := 140;
        2:
          v := 90;
        3:
          v := 80;
        4:
          v := 60;
        5:
          v := 25;
      else
        v := 0;
      end;
  end;

  GOgrSpeedShab := v;
end;

// ------------------------------------------------------------------------------
// Индекс бойынша urb,per,pro шектеули жылдамдык кайтарады
// ------------------------------------------------------------------------------
function GOgrSpeed_UPP(Pv, k: integer): integer;
var
  v: integer;
begin

  case Pv of
    0:
      case k of
        1:
          v := 140;
        2:
          v := 140;
        3:
          v := 60;
        4:
          v := 40;
        5:
          v := 15
      else
        v := 0;
      end;
    1:
      case k of
        1:
          v := 90;
        2:
          v := 80;
        3:
          v := 60;
        4:
          v := 40;
        5:
          v := 15
      else
        v := 0;
      end;
  end;

  GOgrSpeed_UPP := v;
end;

// ------------------------------------------------------------------------------
// Шектелген жылдамдык уширение
// ------------------------------------------------------------------------------
function V_ogr_ush(Pv, v, pnorm, push: integer): integer;
var
  i, j, k, p, m, vtemp: integer;
begin
  k := GIndexSpeedShab(Pv, v);
  j := GIndexNorm(pnorm);

  vtemp := 0;

  m := 0;
  for i := k to 4 do
  begin
    p := pnorm + DeltaUsh[i, 3, j];
    // if push <= p then//02.10.2012
    if push <= p then
    begin
      m := i;
      vtemp := GOgrSpeedShab(Pv, m);
      break;
    end;
  end;

  if vtemp >= v then
    vtemp := -1;

  V_ogr_ush := vtemp;
end;

// ------------------------------------------------------------------------------
// Шектелген жылдамдык сужжение
// ------------------------------------------------------------------------------
function V_ogr_suj(Pv, v, pnorm, push: integer): integer;
var
  i, j, k, p, m, vtemp: integer;
begin
  k := GIndexSpeedShab(Pv, v);
  j := GIndexNorm(pnorm);

  m := 0;
  for i := k to 4 do
  begin
    p := pnorm - DeltaSuj[3, i, j];
    if push >= p then
    begin
      m := i;
      break;
    end;
  end;

  vtemp := GOgrSpeedShab(Pv, m);
  if vtemp >= v then
    vtemp := -1;

  // if GIndexSpeedShab(pv,v) < GIndexSpeedShab(pv,vtemp) then
  V_ogr_suj := vtemp
  // else V_ogr_suj:= -1;
end;

// ------------------------------------------------------------------------------
// Шектелген жылдамдык urb,per,pro
// ------------------------------------------------------------------------------
function V_ogr_UPP(Pv, v, H, g: integer; iso_joint: boolean = false): integer;
// g-tip ots, h-belich ots
{
  g = 1 prosadka
  g = 2 perekos
  g = 3 uroven
}
var
  i, j, k, p, vtemp: integer;
begin
  k := GIndexSpeed_UPP(Pv, v);

  j := 0;
  for i := k to 4 do
  begin

    case g of
      1:
        p := DeltaPro[3, i + 1];
      2:
        p := DeltaPer[3, i + 1];
      3:
        p := DeltaUrb[3, i + 1];
    else
      if Flag_sablog then
        System.writeln('error V_ogr_UPP');
    end;

    if H <= p then
    begin
      if iso_joint then
        j := i + 1

      else
        j := i;

      break;
    end;

  end;
  vtemp := GOgrSpeed_UPP(Pv, j);
  if vtemp >= v then
    vtemp := -1;
  // if GIndexSpeed_UPP(pv,v) < GIndexSpeed_UPP(pv,vtemp) then
  V_ogr_UPP := vtemp;
  // else V_ogr_UPP:= -1;
end;

// ==============================================================================
function DeltaR(Pv, x, v, k: integer): real; // k степень отс.
var
  a, b, C, D, kf1, kf2, h1, h2, xx, ab, bc, cd, res: real;
  j: integer;
begin
  try
    res := 1000;
    xx := x / 400;
    a := 0.0;
    b := 2.5;
    C := 5.0;
    D := 10.0; //
    j := IndexV(Pv, v);

    case k of
      1:
        begin
          h1 := abs(DeltaPp1[1, j] - DeltaPp1[2, j]);
          h2 := abs(DeltaPp1[3, j] - DeltaPp1[2, j]);
        end;
      2:
        begin
          h1 := abs(DeltaPp2[1, j] - DeltaPp2[2, j]);
          h2 := abs(DeltaPp2[3, j] - DeltaPp2[2, j]);
        end;
      3:
        begin
          h1 := abs(DeltaPp3[1, j] - DeltaPp3[2, j]);
          h2 := abs(DeltaPp3[3, j] - DeltaPp3[2, j]);
        end;
    end;

    kf1 := h1 / (C - b);
    kf2 := h2 / (D - C);

    if (a <= xx) and (xx <= b) then
    begin
      case k of
        1:
          res := DeltaPp1[1, j];
        2:
          res := DeltaPp2[1, j];
        3:
          res := DeltaPp3[1, j];
      end;
    end;

    if (b < xx) and (xx <= C) then
    begin
      case k of
        1:
          res := DeltaPp1[1, j] - kf1 * (xx - b);
        2:
          res := DeltaPp2[1, j] - kf1 * (xx - b);
        3:
          res := DeltaPp3[1, j] - kf1 * (xx - b);
      end;
    end;

    if (C < xx) and (xx <= D) then
    begin
      case k of
        1:
          res := DeltaPp1[2, j] + kf2 * (xx - C);
        2:
          res := DeltaPp2[2, j] + kf2 * (xx - C);
        3:
          res := DeltaPp3[2, j] + kf2 * (xx - C);
      end;
    end;

    DeltaR := res;

  except
  end;
end;

// =========================================
// 09.11.2010 палетка скорости рихтовки
// ========================================
function Velich_table_3dot3(x, v, k: integer): real; // k степень отс.
var
  xx: integer;
  vel: real;
begin
  try
    xx := x;
    vel := -1;
    //
    case v of
      121 .. 180:
        case xx of
          1 .. 20:
            case k of
              0:
                vel := 10;
              1:
                vel := 12;
              2:
                vel := 15;
              3:
                vel := 25;
            end;
          21 .. 40:
            case k of
              0:
                vel := 16;
              1:
                vel := 20;
              2:
                vel := 25;
              3:
                vel := 35;
            end;
        end;
      // ----------------------------------
      61 .. 120:
        case xx of
          1 .. 20:
            case k of
              0:
                vel := 10;
              1:
                vel := 15;
              2:
                vel := 25;
              3:
                vel := 35;
            end;
          21 .. 40:
            case k of
              0:
                vel := 16;
              1:
                vel := 20;
              2:
                vel := 35;
              3:
                vel := 40;
            end;
        end;
      // ----------------------------------
      41 .. 60:
        case xx of
          1 .. 20:
            case k of
              0:
                vel := 18;
              1:
                vel := 20;
              2:
                vel := 35;
              3:
                vel := 40;
            end;
          21 .. 40:
            case k of
              0:
                vel := 25;
              1:
                vel := 30;
              2:
                vel := 40;
              3:
                vel := 50;
            end;
        end;
      // ----------------------------------
      16 .. 40:
        case xx of
          1 .. 20:
            case k of
              0:
                vel := 18;
              1:
                vel := 20;
              2:
                vel := 40;
              3:
                vel := 50;
            end;
          21 .. 40:
            case k of
              0:
                vel := 25;
              1:
                vel := 30;
              2:
                vel := 50;
              3:
                vel := 65;
            end;
        end;
      // ----------------------------------
      0 .. 15:
        case xx of
          1 .. 20:
            case k of
              0:
                vel := 50;
              1:
                vel := 50;
              2:
                vel := 50;
              3:
                vel := 65;
            end;
          21 .. 40:
            case k of
              0:
                vel := 65;
              1:
                vel := 65;
              2:
                vel := 65;
              3:
                vel := 90;
            end;
        end;
      // ----------------------------------
    end;

    Velich_table_3dot3 := vel;
  except
  end;
end;

// =========================================
// ограничение скорости рихтовки
// ========================================
function V_ogr_rih(L, H: integer): integer;
var
  v, v1, v2: integer;
  h1, h2: real;
begin

  v := GlobPassSkorost;
  if DeltaR(0, L, 140, 3) < H then
    v := 120;
  if DeltaR(0, L, 120, 3) < H then
    v := 80;
  if DeltaR(0, L, 80, 3) < H then
    v := 60;
  if DeltaR(0, L, 60, 3) < H then
    v := 40;
  if DeltaR(0, L, 40, 3) < H then
    v := 15;
  if DeltaR(0, L, 15, 3) < H then
    v := 0;
  V_ogr_rih := v;
end;

// =========================================
// 09.11.2010 ограничение скорости рихтовки
// ========================================
function V_ogr_rih_tab_3dot3(Pv, v, L, H: integer;
  onJoint: boolean = false): integer;
var
  k1, k2, vo, LL: integer;
begin
  LL := L;

  case Pv of
    0:
      case LL of
        0 .. 20:
          case H of
            26 .. 35:
              vo := 120;
            36 .. 40:
              vo := 60;
            41 .. 50:
              vo := 40;
            51 .. 65:
              vo := 15;
            66 .. 180:
              vo := 0;
          end;
        21 .. 40:
          case H of
            36 .. 40:
              vo := 120;
            41 .. 50:
              vo := 60;
            51 .. 65:
              vo := 40;
            66 .. 90:
              vo := 15;
            91 .. 180:
              vo := 0;
          end;
      end;
    1:
      case LL of
        0 .. 20:
          case H of
            26 .. 35:
              vo := 80;
            36 .. 40:
              vo := 60;
            41 .. 50:
              vo := 40;
            51 .. 65:
              vo := 15;
            66 .. 180:
              vo := 0;
          end;
        21 .. 40:
          case H of
            36 .. 40:
              vo := 80;
            41 .. 50:
              vo := 60;
            51 .. 65:
              vo := 40;
            66 .. 90:
              vo := 15;
            91 .. 180:
              vo := 0;
          end;
      end;
  end;
  // --------------------------------------
  case Pv of
    0:
      case v of
        121 .. 140:
          k1 := 1;
        61 .. 120:
          k1 := 2;
        41 .. 60:
          k1 := 3;
        16 .. 40:
          k1 := 4;
        0 .. 15:
          k1 := 5;
      else
        k1 := 2;
      end;
    1:
      case v of
        81 .. 90:
          k1 := 1;
        61 .. 80:
          k1 := 2;
        41 .. 60:
          k1 := 3;
        16 .. 40:
          k1 := 4;
        0 .. 15:
          k1 := 5;
      else
        k1 := 2;
      end;
  end;
  // -------------------------------------
  case Pv of
    0:
      case vo of
        121 .. 140:
          k2 := 1;
        61 .. 120:
          k2 := 2;
        41 .. 60:
          k2 := 3;
        16 .. 40:
          k2 := 4;
        0 .. 15:
          k2 := 5;
      else
        k2 := 2;
      end;
    1:
      case vo of
        81 .. 90:
          k2 := 1;
        61 .. 80:
          k2 := 2;
        41 .. 60:
          k2 := 3;
        16 .. 40:
          k2 := 4;
        0 .. 15:
          k2 := 5;
      else
        k2 := 2;
      end;
  end;
  // -----------------------------------
  if k1 >= k2 then
    vo := -1;
  if vo >= v then
    vo := -1;

  if onJoint and (vo > -1) then
  begin
    case Pv of
      0:
        case vo of
          140:
            vo := 120;
          120:
            vo := 60;
          60:
            vo := 40;
          40:
            vo := 15;
          15:
            vo := 5;
        else
          vo := 0;
        end;
      1:
        case vo of
          90:
            vo := 80;
          80:
            vo := 60;
          60:
            vo := 40;
          40:
            vo := 15;
          15:
            vo := 5;
        else
          vo := 0;
        end;
    end;
  end;
  V_ogr_rih_tab_3dot3 := vo
end;

function V_ogr_rih_tab_3IsEqualTo4(Pv, v, L, H: integer): integer;
var
  k1, k2, vo, LL: integer;
begin
  LL := L;

  case Pv of
    0:
      case LL of
        0 .. 20:
          case H of
            16 .. 25:
              vo := 120;
            26 .. 35:
              vo := 60;
            36 .. 40:
              vo := 40;
            41 .. 50:
              vo := 15;
            51 .. 180:
              vo := 0;
          end;
        21 .. 40:
          case H of
            16 .. 25:
              vo := 120;
            36 .. 40:
              vo := 60;
            41 .. 50:
              vo := 40;
            51 .. 65:
              vo := 15;
            66 .. 180:
              vo := 0;
          end;
      end;
    1:
      case LL of
        0 .. 20:
          case H of
            16 .. 25:
              vo := 80;
            26 .. 35:
              vo := 60;
            36 .. 40:
              vo := 40;
            41 .. 50:
              vo := 15;
            51 .. 180:
              vo := 0;
          end;
        21 .. 40:
          case H of
            16 .. 25:
              vo := 80;
            36 .. 40:
              vo := 60;
            41 .. 50:
              vo := 40;
            51 .. 65:
              vo := 10;
            66 .. 180:
              vo := 0;
          end;
      end;
  end;
  // --------------------------------------
  case Pv of
    0:
      case v of
        121 .. 140:
          k1 := 1;
        61 .. 120:
          k1 := 2;
        41 .. 60:
          k1 := 3;
        16 .. 40:
          k1 := 4;
        0 .. 15:
          k1 := 5;
      else
        k1 := 2;
      end;
    1:
      case v of
        81 .. 90:
          k1 := 1;
        61 .. 80:
          k1 := 2;
        41 .. 60:
          k1 := 3;
        16 .. 40:
          k1 := 4;
        0 .. 15:
          k1 := 5;
      else
        k1 := 2;
      end;
  end;
  // -------------------------------------
  case Pv of
    0:
      case vo of
        121 .. 140:
          k2 := 1;
        61 .. 120:
          k2 := 2;
        41 .. 60:
          k2 := 3;
        16 .. 40:
          k2 := 4;
        0 .. 15:
          k2 := 5;
      else
        k2 := 2;
      end;
    1:
      case vo of
        81 .. 90:
          k2 := 1;
        61 .. 80:
          k2 := 2;
        41 .. 60:
          k2 := 3;
        16 .. 40:
          k2 := 4;
        0 .. 15:
          k2 := 5;
      else
        k2 := 2;
      end;
  end;
  // -----------------------------------
  if k1 >= k2 then
    vo := -1;
  if vo >= v then
    vo := -1;

  V_ogr_rih_tab_3IsEqualTo4 := vo;
end;

// =========================================
//
// ========================================
procedure S4_comment(k, pik: integer);
begin
  {
    1 - ush
    2 - suj
    3 - rih
    4 - pro1
    5 - pro2
    6 - per
    7 - urov
  }
  case k of
    1:
      begin
        if not AnsiContainsStr(p_ush4, inttostr(pik)) then
          p_ush4 := p_ush4 + inttostr(pik) + ',';
      end;
    2:
      begin
        if not AnsiContainsStr(p_suj4, inttostr(pik)) then
          p_suj4 := p_suj4 + inttostr(pik) + ',';
      end;
    3:
      begin
        if not AnsiContainsStr(p_rih, inttostr(pik)) then
          p_rih := p_rih + inttostr(pik) + ',';

      end;
    4:
      begin
        if not AnsiContainsStr(p_pr1, inttostr(pik)) then
          p_pr1 := p_pr1 + inttostr(pik) + ',';

      end;
    5:
      begin
        if not AnsiContainsStr(p_pr2, inttostr(pik)) then
          p_pr2 := p_pr2 + inttostr(pik) + ',';

      end;
    6:
      begin
        if not AnsiContainsStr(p_per4, inttostr(pik)) then
          p_per4 := p_per4 + inttostr(pik) + ',';

      end;
    7:
      begin
        if not AnsiContainsStr(p_urv4, inttostr(pik)) then
          p_urv4 := p_urv4 + inttostr(pik) + ',';
      end;
  end;
end;

// =========================================
// Опр. алфа от 1-го типа  уст. скор.
// ========================================

function G_Ind2_Remont(var s: string; Var v: integer): integer;
const
  cmdArray: Array [0 .. 3] of String = (BPO, BPOBPR, BPOIBPR, BPOBPRDCP);
var
  j: integer;
begin
  case AnsiIndexStr(s, cmdArray) of
    0:
      case v of
        60:
          j := 1;
        40:
          j := 2;
        25:
          j := 3;
      end;
    1:
      case v of
        50:
          j := 1;
        25:
          j := 2;
        15:
          j := 3;
      end;
    2:
      case v of
        60:
          j := 1;
        40:
          j := 2;
        25:
          j := 3;
      end;
    3:
      case v of
        70:
          j := 1;
        50:
          j := 2;
        25:
          j := 3;
      end;
    // else Beep;
  end;
  G_Ind2_Remont := j;
end;

function G_Ind1(var k, v: integer): integer;
var
  j: integer;
begin
  j := 2;

  case k of
    0:
      case v of
        121 .. 180:
          j := 1;
        101 .. 120:
          j := 2;
        61 .. 100:
          j := 3;
        26 .. 60:
          j := 4;
        0 .. 25:
          j := 4;
      end;
    1:
      case v of
        81 .. 90:
          j := 2;
        61 .. 80:
          j := 3;
        26 .. 60:
          j := 4;
        0 .. 25:
          j := 4;
      end;
  end;

  G_Ind1 := j;
end; // fun

// ========================================
// Опр. алфа от 2-го типа уст. скор.
// ========================================
function G_Ind2(var k, v: integer): integer;
var
  j: integer;
begin
  j := 2;

  case k of
    0:
      case v of
        121 .. 140:
          j := 1;
        61 .. 120:
          j := 2;
        41 .. 60:
          j := 3;
        16 .. 40:
          j := 4;
        0 .. 15:
          j := 5;
      end;
    1:
      case v of
        81 .. 90:
          j := 1;
        61 .. 80:
          j := 2;
        41 .. 60:
          j := 3;
        16 .. 40:
          j := 4;
        0 .. 15:
          j := 5;
      end;
  end;

  G_Ind2 := j;
end; // fun

// fun

// =========================================
// Опр. алфа от 3-го типа уст. скор.
// =========================================
function G_Ind3(var k, v: integer): integer;
var
  j: integer;
begin

  if (k = 0) or (k = 1) then
  begin
    case v of

      81 .. 120:
        j := 1;
      61 .. 80:
        j := 2;
      41 .. 60:
        j := 3;
      16 .. 40:
        j := 4;
      0 .. 15:
        j := 5;

      {
        121..140: j:= 1;
        81..120:  j:= 2;
        61..80:  j:= 3;
        41..60:   j:= 4;
        16..40:   j:= 5;
        0..15:    j:= 6;
      }
      // else      j:= 3;
    end;
  end
  else if (k = 100000000) then
  begin
    case v of
      81 .. 90:
        j := 1;
      71 .. 80:
        j := 2;
      61 .. 70:
        j := 3;
      41 .. 60:
        j := 4;
      16 .. 40:
        j := 5;
      0 .. 15:
        j := 6;
      // else    j:= 3;
    end;
  end
  else
  begin
    // G_Ind3:= 3;
  end;
  G_Ind3 := j;
end; // fun

// =====================================================
function AlpS(var Nms, Radius: integer): integer;
Var
  fsh, j: integer;
Begin
  j := 1;

  case Nms of
    1520:
      j := 1;
    1521 .. 1524:
      j := 2;
    1525 .. 1530:
      j := 3;
    1531 .. 1535:
      j := 4;
    1536 .. 1545:
      j := 5;
  end;
  {
    if Nms = 1520 then
    begin
    case Radius of
    1..299:    j:= 4;
    300..349 : j:= 3;
    350..10000: j:= 1;
    else j:= 1;
    end;
    end;

    if Nms = 1524 then
    begin
    case Radius of
    1..349    : j:= 5;
    350..449  : j:= 4;
    450..649  : j:= 3;
    650..10000: j:= 2;
    else j:= 2;
    end;
    end; }

  AlpS := j;
end;

// =========================================
// Опр. алфа от 3-го типа уст. скор.
// =========================================
function GStepen(var PHn, ps1, ps2, ps3: integer): integer;
var
  j: integer;
begin
  GStepen := 1;

  if (ps1 < PHn) and (PHn <= ps2) then
    GStepen := 2
  else if (ps2 < PHn) and (PHn <= ps3) then
    GStepen := 3
  else if (ps3 < PHn) then
    GStepen := 4;
end; // fun

// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
{ function GTF_su; //(px,pf0,pots,pkf:integer; tsX,tsBel,tsDl,tsots:mas):integer;
  var
  i,j,kf1,kf2,pkf:integer; Abool:boolean;
  Begin
  Try
  GTF_su:= pf0;

  For j:= 0 to  High(tsX) do
  begin
  pkf:= tsOts[j];
  case pkf of
  1,2,6: begin kf1:= 1; kf2:= 1; end;
  3,4,5,7,8: begin kf1:= -1; kf2:= 2; end;
  end;
  x_a:= tsX[j] - Tdelta[pkf];
  x_b:= tsX[j];
  x_c:= tsX[j] + tsDl[j];
  x_d:= x_c + Tdelta[pkf];

  y_a:= pf0;
  y_b:= y_a + round(tsBel[j]/kf2);
  y_c:= y_a + kf1*round(tsBel[j]/kf2);
  y_d:= y_a;

  if (tsOts[j] = pna) or (tsOts[j] = pnb) then ABool:= true else ABool:= false;

  if ABool and (x_a <= px) and (px <= x_b) then
  GTF_su:= getTuzu(px,x_a,x_b,y_a,y_b);

  if ABool and (x_b <= px) and (px <= x_c) then
  begin
  case kf1 of
  1: GTF_su:= pf0 + tsBel[j];
  else  GTF_su:= GetTuzu(px,x_b,x_c,y_b,y_c)
  end;
  end;
  if ABool and (x_c <= px) and (px <= x_d) then
  GTF_su:= getTuzu(px,x_c,x_d,y_c,y_d);

  end;
  except
  end;
  end; }
// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
function Convert; // (s:string): String;
var
  i, n, j: integer;
  s1: string;
begin
  s1 := s;
  n := length(s1);
  j := 1;
  for i := 1 to n do
  begin
    if s1[i] = ' ' then
    begin
      Delete(s, j, 1);
      j := j - 1
    end;
    j := j + 1
  end;
  Convert := s;
end;

// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
procedure Tizim_sur;
var
  i, j, k, sp: integer;
Begin
  Try
    Setlength(gtz, 1000);
    k := 0;
    gtz[0] := ptz[0];

    for i := Low(ptz) + 1 to High(ptz) do
    begin
      sp := 0;
      for j := 0 to k do
      begin
        if ptz[i] = gtz[j] then
          sp := sp + 1;
      end; // fr2

      if sp = 0 then
      begin
        k := k + 1;
        gtz[k] := ptz[i];
      end; // if
    end; // fr1
    Setlength(gtz, k);
  except
  end;
end;

// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
procedure GetFtobe;
var
  L, j, k: integer;
Begin
  // SabLog('GetFtobe - возвращает массив который состоит из вершин данного массива');
  Try
    Setlength(Ft, 3000);
    Setlength(FtK, 3000);
    Setlength(FtV, 3000);

    j := 0;
    For L := Low(FG) to High(FG) - 2 do
    begin
      k := L + 1;
      fa := FG[k - 1];
      fb := FG[k];
      fc := FG[k + 1];

      if ((fa > fb) and (fb < fc)) or ((fa < fb) and (fb > fc)) then
      begin
        Ft[j] := fb;
        FtK[j] := FGm[k];
        FtV[j] := FgV[k];
        j := j + 1;
      end;

    end;

    if j = 0 then
      j := 1;
    Setlength(Ft, j);
    Setlength(FtK, j);
    Setlength(FtV, j);

  except
  end;
end;

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
{ procedure SDA(ots:string; code,st,f,xa,xb,ogr:integer);
  var
  fSiezd, fStrel, fSkor, fRem, f4:boolean;
  name_ots:string;
  piket, cst:integer;
  begin
  fSiezd:=  not FlagSiezd(xa div 100, xb div 100);
  fStrel:=  not ProberkaNaStrelku(xa div 100,7) and not ProberkaNaStrelku(xb div 100,1);
  fSkor:=   (ogr < GlobPassSkorost) and fSiezd and fStrel;
  frem:= not GlbFlagRemontKm;
  f4:= (st = 4) and fSiezd and fStrel;

  piket:= ((xa div 100) mod 1000) div 100 + 1;
  cst:= code + st;

  if fRem or fSkor then
  begin

  case code of
  10: begin
  WUsh[ns].st:= st; WUsh[ns].bel:= f; WUsh[ns].L0:= xa; WUsh[ns].Lm:= xb;
  ns:= ns + 1;
  end;
  20: name_ots := 'Сж4 пк';
  end;

  // 4st: 14-ush, 24-suj, 34-pr1, 44-pr2, 54-rh1, 64-rh2, 74-per, 84-urb

  case cst of
  14: begin
  name_ots := 'Уш4 пк';
  glbCount_Ush4s:= glbCount_Ush4s + 1;
  end;
  end;

  end;

  if fSkor then
  WRT_UBEDOM(xa, xb, 4, 'Уш4 пк'
  + inttostr(piket) + ' ' +Inttostr(f) +'мм v=' + inttostr(ogranichenie1),
  Inttostr(f)+' ' ,inttostr(ogranichenie1),inttostr(ogranichenie1)); //FormatFloat('0.00',max4)
  S4_comment(1, piket);


  end; }
// ------------------------------------------------------------------------------
// Уширение для 1548 mm
// ------------------------------------------------------------------------------
procedure GUsh_1548mm;
label 1;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, Fhr, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  ogranichenie: string;
  ogranichenie1: integer;
  cou_4st: integer;
  flg2st2, flg3st2, flg4st2: boolean;
  flg2st3, flg3st3, flg4st3, flgk1, flgk2: boolean;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs: integer;

  xxxx, ogr_min, fff, imax, i0, vtg, coordx: integer;
  maxf: real;
  ogr_flg: boolean;

Begin
  Try
    if Flag_sablog then
      sablog('GUsh - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    Setlength(WUsh, 3000);

    ogr_flg := false;
    i := 0;
    k := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    flg4st := false;

    FG[Low(FG)] := F0_Sh[Low(FG)]; // 1520;
    FG[High(FG)] := F0_Sh[High(FG)]; // 1520;
    max4 := 0;

    while i <= High(FG) do
    begin
      Nms1 := round(F0_Sh[i]); // F_Norm[i];
      Fh := round(FG[i] - F0_Sh[i]);
      Rds1 := Prds[i];
      fkm := FGm[i];
      k := AlpS(Nms1, Rds1);

      vt := F_V[i];
      vtg := F_Vg[i];
      PasGruzSkor := 0;
      j := G_Ind1(PasGruzSkor, vt);

      s1 := DeltaUsh[j, 1, k];
      s2 := DeltaUsh[j, 2, k];
      s3 := DeltaUsh[j, 3, k];
      // -----------------------------------------------
      i0 := i;
      maxf := 1548;

      while (i0 <= High(FG)) and (FG[i0] > maxf) do
      begin
        maxf := FG[i0];
        fff := round(maxf);
        ogranichenie1 := 0;
        L04 := FGm[i];
        Lm4 := FGm[i0];
        vt := F_V[i0];
        vtg := F_Vg[i0];
        i0 := i0 + 1;
      end;

      if (ogranichenie1 = 0) then
      begin
        // flg4st:= true;
        fff := round(maxf);
        stv := 4;
        belv := max4;
        L0v := L04;
        Lmv := Lm4;
        coordx := (L0v + Lmv) div 200;

        if not FlagSiezd(L0v div 100, Lmv div 100) then
        begin
          // 0909
          WUsh[ns].st := 4;
          WUsh[ns].bel := fff;
          /// max4;
          WUsh[ns].L0 := L04;
          WUsh[ns].Lm := Lm4;
          WUsh[ns].v := vt;
          WUsh[ns].vg := vtg;
          WUsh[ns].vop := 0;
          WUsh[ns].vog := 0;
          WUsh[ns].flg := false;
          ns := ns + 1;
          // 0909
          glbCount_Ush4s := glbCount_Ush4s + 1;
          xxxx := (coordx mod 1000) div 100 + 1;

          { WRT_UBEDOM(L04, Lm4, 4,
            'Уш пк' + inttostr(xxxx) + ' ' +Inttostr(fff) +'мм v=' + inttostr(ogranichenie1),
            Inttostr(fff)+' ' ,inttostr(ogranichenie1),inttostr(ogranichenie1)); //FormatFloat('0.00',max4)
          }
          WRT_UBEDOM(L04, Lm4, 4, 'V=0/0 ' + ' пк' + inttostr(xxxx) +
            ' Уш; ', 0, 0);
          // FormatFloat('0.00',max4)
          // S4_comment(1, xxxx); //01.10.2012
        end;

        // сброс флагов
        flg4st := false;
        ogranichenie1 := 140;
        max4 := 0;
        ogr_flg := true;
        i := i + i0 - 1;
      end;

      flg4st := false;
      ogranichenie1 := 140;
      max2 := 0;
      max3 := 0;
      max4 := 0;
      k1 := 0;
      k2 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1

    Setlength(WUsh, ns);
    MTmp := Nil;
  except

  end; // try
end;

/// /////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------
// Уширение для 4 степени
// ------------------------------------------------------------------------------
procedure GUsh_4;
label 1;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, Fhr, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  ogranichenie: string;
  ogranichenie1: integer;
  cou_4st: integer;
  flg2st2, flg3st2, flg4st2: boolean;
  flg2st3, flg3st3, flg4st3, flgk1, flgk2: boolean;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs, maxf: integer;

  xxxx, ogr_min, fff, imax, i0, vt, vtg, vr, Vrg, PasGruzSkor, v1, v2,
    Norma: integer;
  ogr_flg, not_ots: boolean;
Begin
  Try
    // SabLog('GUsh - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    Setlength(WUsh, 3000);
    ogranichenie1 := 140;
    ogr_flg := false;
    PasGruzSkor := 0;
    i := 0;
    k := 0;

    k3 := 0;
    L04 := 0;
    Lm4 := 0;

    flg4st := false;

    FG[Low(FG)] := F0_Sh[Low(FG)]; // 1520;
    FG[High(FG)] := F0_Sh[High(FG)]; // 1520;

    j := G_Ind1(PasGruzSkor, GlobPassSkorost);

    max4 := 0;

    while i <= High(FG) do
    begin
      Nms1 := round(F0_Sh[i]);
      Norma := F_Norm[i];
      // Fh:= round(Fg[i] - F0_sh[i]);

      // Fh := round(F_Sht[i] - F_Wear[i] - F0_Sh[i]);
      Fh := round(F_Sht[i] - F0_Sh[i]);

      Rds1 := Prds[i];
      fkm := FGm[i];
      k := AlpS(Nms1, Rds1);

      vt := F_V[i];
      vtg := F_Vg[i];
      vr := F_Vrp[i];
      Vrg := F_Vrg[i];
      PasGruzSkor := 0;
      j := G_Ind1(PasGruzSkor, vt);

      s3 := DeltaUsh[j, 3, k];
      i0 := i;
      max4 := s3;
      Fhr := round(FG[i]);

      not_ots := true;

      while (i0 <= High(FG)) and (max4 < Fh) do
      begin
        max4 := Fh;
        Fhr := round(FG[i0]);

        // Fh:= round(Fg[i0] - F0_sh[i0]);
        Fh := round(F_Sht[i0] - F_Wear[i] - F0_Sh[i0]);
        Norma := F_Norm[i];
        L04 := FGm[i];
        Lm4 := FGm[i0];

        vt := F_V[i0];
        vtg := F_Vg[i0];
        vr := F_Vrp[i0];
        Vrg := F_Vrg[i0];
        i0 := i0 + 1;
      end;

      if (s3 < max4) { and (1548 > max4) } then
      begin

        fff := Fhr;
        stv := 4;
        belv := fff;
        L0v := L04;
        Lmv := Lm4;

        if fff > 1548 then
        begin
          v1 := 0;
          v2 := 0;
          i := i + 1;

        end
        else if fff <> 1548 then
        begin
          v1 := V_ogr_ush(0, vt, Nms1, fff);
          v2 := V_ogr_ush(1, vtg, Nms1, fff);
        end;

        // ---------------------------------------------
        if (fff = 1548) and (Norma = 1535) and (vt > 25) and (vt <= 60) then
        begin
          v1 := -1;
          stv := 3
        end;

        if (fff = 1548) and (Norma = 1535) and (vtg > 25) and (vtg <= 60) then
        begin
          stv := 3;
          v2 := -1;
        end;
        // ---------------------------------------------
        if (fff = 1548) and (Norma = 1535) and (vt > 60) then
        begin
          v1 := 60;
          stv := 4;
        end;

        if (fff = 1548) and (Norma = 1535) and (vtg > 60) then
        begin
          stv := 4;
          v2 := 60;
        end;

        // ------------------------------------------
        if (fff = 1548) and (Norma < 1540) and (vt <= 25) then
        begin
          stv := 3;
          v1 := -1;
        end;

        if (fff = 1548) and (Norma < 1540) and (vtg <= 25) then
        begin
          stv := 3;
          v2 := -1;
        end;
        // -------------------------------------------
        if (Norma = 1540) and (fff = 1548) // 24.10.2012
          and (vt > 25) and (vt < 101) and (vtg > 25) and (vtg < 81) then
        begin
          stv := 2;
          v1 := -1;
          v2 := -1;
        end;

        if (Norma = 1540) and (fff = 1548) and (vt <= 25) and (vtg <= 25) then
          not_ots := false;
        // ----------------------------------------------

        if not FlagSiezd(L0v div 100, Lmv div 100) and (abs(L04 - Lm4) > 0) and
          not NecorrPasportFlag and (v1 >= 0) and (v2 >= 0) and
          ((v1 < vt) or (v2 < vtg)) and not_ots then
        begin

          if (v1 >= vr) then
            v1 := -1;
          if (v2 >= Vrg) then
            v2 := -1;

          // 0909

          WUsh[ns].st := stv;
          WUsh[ns].bel := fff;
          /// max4;
          WUsh[ns].L0 := L04;
          WUsh[ns].Lm := Lm4 + 1;
          WUsh[ns].v := vt;
          WUsh[ns].vg := vtg;
          WUsh[ns].vop := v1;
          WUsh[ns].vog := v2;
          WUsh[ns].Leng := abs(L04 - Lm4) + 1;
          WUsh[ns].flg := false;
          WUsh[ns].prim := '';
          // WUsh[ns].onswitch := ProberkaNaStrelku(L0v, Lmv, 1);
          if (v1 = -1) and (v2 = -1) then
            WUsh[ns].prim := GlbTempTipRemontKm;
          ns := ns + 1;
          // 0909
          // if not(WUsh[ns].onswitch) then
          // begin
          glbCount_Ush4s := glbCount_Ush4s + 1;
          xxxx := L04 div 100 + 1;

          if stv = 4 then
            WRT_UBEDOM(L04, Lm4 + 1, 4, ' V=' + V_shekti(v1, v2) + ' пк' +
              inttostr(xxxx) + ' Уш;', v1, v2);
          // end;
          for j := 0 to high(FGm) do
            if ((L04 <= FGm[j]) and (FGm[j] <= Lm4 + 1)) or
              ((L04 + 1 >= FGm[j]) and (FGm[j] >= Lm4)) then
              F_Sht[j] := F0_Sh[j];

          // FormatFloat('0.00',max4)
          // S4_comment(1, xxxx); //01.10.2012
        end;

        // сброс флагов
        flg4st := false;
        ogranichenie1 := 140;
        max4 := 0;

        ogr_flg := true;

        i0 := i0 + 1;
      end;

      flg4st := false;
      ogranichenie1 := 140;
      max2 := 0;
      max3 := 0;
      max4 := 0;
      k1 := 0;
      k2 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;

    end; // while1

    Setlength(WUsh, ns);

  except

  end; // try
end;

// ------------------------------------------------------------------------------
// Уширение для 3 степени  shablon count > 10
// ------------------------------------------------------------------------------
function GetCountUsh3(FG: masr; FGm, Prds: mas; speed: integer): integer;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs: integer;
  xxxx, fff, imax: integer;
  xa, xb, Leng, bolshek, count, xo, retCount: integer;
Begin
  Try
    // SabLog('GUsh - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    retCount := 0;
    piket1 := 0;
    piket2 := 0;
    piket3 := 0;
    piket4 := 0;
    piket5 := 0;
    piket6 := 0;
    piket7 := 0;
    piket8 := 0;
    piket9 := 0;
    piket10 := 0;

    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    PasGruzSkor := 0;
    flg4st := false;
    FG[Low(FG)] := F0_Sh[Low(FG)]; // 1520;
    FG[High(FG)] := F0_Sh[High(FG)]; // 1520;

    while i <= High(FG) do
    begin
      Nms1 := round(F0_Sh[i]);
      Fh := round(F_Sht[i] - F0_Sh[i]);
      if Fh < 0 then
      begin
        i := i + 1;
        continue;
      end;
      Rds1 := Prds[i];
      fkm := FGm[i];
      k := AlpS(Nms1, Rds1);
      vt := speed;
      j := G_Ind1(PasGruzSkor, vt);

      s1 := DeltaUsh[j, 1, k];
      s2 := DeltaUsh[j, 2, k];
      s3 := DeltaUsh[j, 3, k];

      if (s2 < Fh) and (Fh <= s3) then
      begin
        OtklUsh := Fh; // round(fg[i]);
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklUsh) then
        BEGIN
          max4 := OtklUsh;
          imax := i;
          fff := round(FG[imax]);
        END;
        i := i + 1;
        continue;
      end; // if

      xa := L04;
      xb := Lm4;

      if (L04 > 0) and (Lm4 > 0) and (max4 > 0) and (fff > 1534) and (fff < 1548)
      then
      begin
        // -------------------------
        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;
        if Leng mod 4 > 0 then
          bolshek := 1;
        count := Leng div 4 + bolshek;
        if count = 0 then
          count := 1;

        retCount := retCount + count;

        for j := 0 to high(FGm) do
          if ((L04 <= FGm[j]) and (FGm[j] <= Lm4)) or
            ((L04 >= FGm[j]) and (FGm[j] >= Lm4)) then
            F_Sht[j] := 1520; // F0_sh[j];
        // сброс флагов
        flg4st := false;
      end;
      flg4st := false;
      max4 := 0;
      fff := 0;
      k3 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------
    str_p := '';

    // ------------------------------------------------------------------------------
    MTmp := Nil;
    GetCountUsh3 := retCount;
  except

  end; // try
end;

// ------------------------------------------------------------------------------
// Уширение для 4 новая из третьей степени
// ------------------------------------------------------------------------------
procedure GUsh_44;
label 1;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, Leng, count, bolshek, len4, xa,
    xb: integer;
  Fh, Fhr, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  ogranichenie: string;
  ogranichenie1: integer;
  cou_4st: integer;
  flg2st2, flg3st2, flg4st2: boolean;
  flg2st3, flg3st3, flg4st3, flgk1, flgk2: boolean;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs, maxf: integer;

  xxxx, ogr_min, fff, fff1549, imax, i0, vt, vtg, vr, Vrg, PasGruzSkor, v1, v2,
    Norma: integer;
  ogr_flg, not_ots: boolean;
Begin
  Try
    // SabLog('GUsh - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    Setlength(WUsh, 3000);

    piket1 := 0;
    piket2 := 0;
    piket3 := 0;
    piket4 := 0;
    piket5 := 0;
    piket6 := 0;
    piket7 := 0;
    piket8 := 0;
    piket9 := 0;
    piket10 := 0;

    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    PasGruzSkor := 0;

    flg4st := false;
    FG[Low(FG)] := F0_Sh[Low(FG)]; // 1520;
    FG[High(FG)] := F0_Sh[High(FG)]; // 1520;

    // j:= G_Ind1(PasGruzSkor,GlobPassSkorost);

    while i <= High(FG) do
    begin
      Nms1 := round(F0_Sh[i]); // Nms1:= F_Norm[i];
      // Fh:= round(Fg[i] - F0_sh[i]);//1520 );  F_Norm[i] F0_sh[i]
      Fh := round(F_Sht[i] - F0_Sh[i]);

      if Fh < 0 then
      begin
        i := i + 1;
        continue;
      end;

      Rds1 := Prds[i];
      fkm := FGm[i];
      k := AlpS(Nms1, Rds1);

      vt := F_V[i1];
      vtg := F_Vg[i1];
      j := G_Ind1(PasGruzSkor, vt);
      s3 := DeltaUsh[j, 3, k];

      if (s3 < Fh) then
      begin
        OtklUsh := Fh; // round(fg[i]);
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklUsh) then
        BEGIN
          max4 := OtklUsh;
          imax := i;
          fff := round(FG[imax]);
          fff1549 := round(FG[imax]);
        END;

        i := i + 1;
        continue;
      end; // if

      // xa:= L04 div 100;
      // xb:= Lm4 div 100;
      xa := L04;
      xb := Lm4;

      if (L04 > 0) and (Lm4 > 0) and (abs(L04 - Lm4) > 0) and (max4 > 0) and
        (fff > 1534) then
      begin
        // -------------------------
        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;
        if Leng mod 4 > 0 then
          bolshek := 1;
        count := Leng div 4 + bolshek;
        if count = 0 then
          count := 1;

        if fff > 1548 then
        begin
          v1 := 0;
          v2 := 0;

        end
        else if fff <> 1548 then
        begin
          v1 := V_ogr_ush(0, vt, Nms1, fff);
          v2 := V_ogr_ush(1, vtg, Nms1, fff);
        end;

        // ---------------------------------------------
        if (fff = 1548) and (Norma = 1535) and (vt > 25) and (vt <= 60) then
        begin
          v1 := -1;
          stv := 3
        end;

        if (fff = 1548) and (Norma = 1535) and (vtg > 25) and (vtg <= 60) then
        begin
          stv := 3;
          v2 := -1;
        end;
        // ---------------------------------------------
        if (fff = 1548) and (Norma = 1535) and (vt > 60) then
        begin
          v1 := 60;
          stv := 4;
        end;

        if (fff = 1548) and (Norma = 1535) and (vtg > 60) then
        begin
          stv := 4;
          v2 := 60;
        end;

        // ------------------------------------------
        if (fff = 1548) and (Norma < 1540) and (vt <= 25) then
        begin
          stv := 3;
          v1 := -1;
        end;

        if (fff = 1548) and (Norma < 1540) and (vtg <= 25) then
        begin
          stv := 3;
          v2 := -1;
        end;
        // -------------------------------------------
        if (Norma = 1540) and (fff = 1548) // 24.10.2012
          and (vt > 25) and (vt < 101) and (vtg > 25) and (vtg < 81) then
        begin
          stv := 2;
          v1 := -1;
          v2 := -1;
        end;

        WUsh[ns].st := 4;
        WUsh[ns].bel := fff; // max4; fff
        WUsh[ns].L0 := L04;
        WUsh[ns].Lm := Lm4;
        WUsh[ns].v := vt;
        WUsh[ns].vg := vtg;
        WUsh[ns].count := count;
        WUsh[ns].Leng := Leng;
        WUsh[ns].prim := '';
        // WUsh[ns].onswitch := ProberkaNaStrelku(L04,L04, 1);
        // if not(WUsh[ns].onswitch) then
        ush_s4 := ush_s4 + count;

        glbCount_Ush4s := glbCount_Ush4s + 1;
        xxxx := L04 div 100 + 1;

        if (WUsh[ns].st = 4) then
          WRT_UBEDOM(L04, Lm4 + 1, 4, ' V=' + V_shekti(v1, v2) + ' пк' +
            inttostr(xxxx) + ' Уш ' + inttostr(fff)+'/'+  inttostr(Leng) + ';  ', v1, v2);

        for j := 0 to high(FGm) do
          if ((L04 <= FGm[j]) and (FGm[j] <= Lm4)) or
            ((L04 >= FGm[j]) and (FGm[j] >= Lm4)) then
            F_Sht[j] := 1520; // F0_sh[j];
        // сброс флагов
        flg4st := false;

        ns := ns + 1;

      end;

      ogr_flg := true;
      flg4st := false;
      ogranichenie1 := 180;
      max2 := 0;
      max3 := 0;
      max4 := 0;
      k1 := 0;
      k2 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;

      max4 := 0;
      fff := 0;
      k3 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------
    str_p := '';

    // ------------------------------------------------------------------------------
    Setlength(WUsh, ns);
    MTmp := Nil;
  except

  end; // try
end;

// ------------------------------------------------------------------------------
// Уширение для 3 степени
// ------------------------------------------------------------------------------
procedure GUsh_3;
var
  i, j, k, kk, beta1, fkm, vt, vtg: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs: integer;
  xxxx, fff, imax: integer;
  xa, xb, Leng, bolshek, count, xo: integer;
Begin
  Try
    // SabLog('GUsh - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    Setlength(WUsh, 3000);

    piket1 := 0;
    piket2 := 0;
    piket3 := 0;
    piket4 := 0;
    piket5 := 0;
    piket6 := 0;
    piket7 := 0;
    piket8 := 0;
    piket9 := 0;
    piket10 := 0;

    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    PasGruzSkor := 0;

    flg4st := false;
    FG[Low(FG)] := F0_Sh[Low(FG)]; // 1520;
    FG[High(FG)] := F0_Sh[High(FG)]; // 1520;

    // j:= G_Ind1(PasGruzSkor,GlobPassSkorost);

    while i <= High(FG) do
    begin
      Nms1 := round(F0_Sh[i]); // Nms1:= F_Norm[i];
      // Fh:= round(Fg[i] - F0_sh[i]);//1520 );  F_Norm[i] F0_sh[i]
      Fh := round(F_Sht[i] - F0_Sh[i]);

      if Fh < 0 then
      begin
        i := i + 1;
        continue;
      end;

      Rds1 := Prds[i];
      fkm := FGm[i];
      k := AlpS(Nms1, Rds1);

      vt := F_V[i];
      vtg := F_Vg[i];
      j := G_Ind1(PasGruzSkor, vt);

      s1 := DeltaUsh[j, 1, k];
      s2 := DeltaUsh[j, 2, k];
      s3 := DeltaUsh[j, 3, k];

      if (s2 < Fh) and (Fh <= s3) then
      begin
        OtklUsh := Fh; // round(fg[i]);
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklUsh) then
        BEGIN
          max4 := OtklUsh;
          imax := i;
          fff := round(FG[imax]);
        END;

        i := i + 1;
        continue;
      end; // if

      // xa:= L04 div 100;
      // xb:= Lm4 div 100;
      xa := L04;
      xb := Lm4;

      if (L04 > 0) and (Lm4 > 0) and (abs(L04 - Lm4) > 0) and (max4 > 0) and
        (fff > 1534) and (fff < 1548) then
      begin
        // -------------------------
        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;
        if Leng mod 4 > 0 then
          bolshek := 1;
        count := Leng div 4 + bolshek;
        if count = 0 then
          count := 1;

        WUsh[ns].st := 3;
        WUsh[ns].bel := Nms1+max4; // max4; fff
        WUsh[ns].L0 := L04;
        WUsh[ns].Lm := Lm4;
        WUsh[ns].v := vt;
        WUsh[ns].vg := vtg;
        WUsh[ns].count := count;
        WUsh[ns].Leng := Leng;
        WUsh[ns].prim := '';
        WUsh[ns].onswitch := ProberkaNaStrelku(L04, L04, 1);
        if not(WUsh[ns].onswitch) then
          ush_s3 := ush_s3 + count;
        ns := ns + 1;

        for j := 0 to high(FGm) do
          if ((L04 <= FGm[j]) and (FGm[j] <= Lm4)) or
            ((L04 >= FGm[j]) and (FGm[j] >= Lm4)) then
            F_Sht[j] := 1520; // F0_sh[j];
        // сброс флагов
        flg4st := false;

      end;
      flg4st := false;

      max4 := 0;
      fff := 0;
      k3 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------
    str_p := '';

    // ------------------------------------------------------------------------------
    Setlength(WUsh, ns);
    MTmp := Nil;
  except

  end; // try
end;

// ------------------------------------------------------------------------------
// Уширение для 2 степени
// ------------------------------------------------------------------------------
procedure GUsh_2;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs, vt, vtg: integer;
  xxxx: integer;
  fff, imax: integer;
  xa, xb, Leng, bolshek, count: integer;
Begin
  Try
    // SabLog('GUsh - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');

    piket1 := 0;
    piket2 := 0;
    piket3 := 0;
    piket4 := 0;
    piket5 := 0;
    piket6 := 0;
    piket7 := 0;
    piket8 := 0;
    piket9 := 0;
    piket10 := 0;

    k := 0;
    Setlength(WUsh, 3000);
    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;

    flg4st := false;
    FG[Low(FG)] := F_Sh[low(F_Sh)]; // 1520;
    FG[High(FG)] := F_Sh[high(F_Sh)]; // 1520;

    // j:= G_Ind1(PasGruzSkor,GlobPassSkorost);

    while i <= High(FG) do
    begin
      Nms1 := round(F0_Sh[i]); // Nms1:= F_Norm[i];
      // Fh:= round(Fg[i]-F0_sh[i]);//1520;  - F_Norm[i]);

      // 1520;  - F_Norm[i]);

      if Fh < 0 then
        Fh := 0;

      Rds1 := Prds[i];
      fkm := FGm[i];

      k := AlpS(Nms1, Rds1);
      if (GlobPassSkorost < 50) then
        beta1 := 2
      else
        beta1 := 0; // and (k < 5)

      vt := F_V[i];
      vtg := F_Vg[i];
      if( vt = 40) then
      vt :=40;

      j := G_Ind1(PasGruzSkor, vt);

      s1 := DeltaUsh[j, 1, k] + beta1;
      s2 := DeltaUsh[j, 2, k];
      s3 := DeltaUsh[j, 3, k];

      Fh := round(F_Sht[i] - F0_Sh[i]);
      if fh > 18 then
      Fh:=fh;

      if ((F_Norm[i] = 1520) or (F_Norm[i] = 1524))
        and (Fh <= s2) and ( F_V[i]>60) then
        Fh := round(F_Sht[i] - F_Wear[i] - F0_Sh[i])
      else
        Fh := round(F_Sht[i] - F0_Sh[i]);

      if (s1 < Fh) and (Fh <= s2) then
      begin
        OtklUsh := Fh; // round(fg[i]);
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm;
        end
        else
          Lm4 := fkm;

        if (max4 < OtklUsh) then
        BEGIN
          max4 := OtklUsh;
          imax := i;
          fff := round(F_Sht[imax]);
        END;
        i := i + 1;
        continue;
      end; // if

      xa := L04;
      xb := Lm4;
      // if  KrvFlag(xa,xb) then
      // sablog('krvflag = ' + inttostr(GlbKmTrue));

      if (L04 > 0) and (Lm4 > 0) and (abs(L04 - Lm4) > 0) and (max4 > 0) and
        (fff < 1548) and not KrvFlag(xa, xb) then
      begin
        // -------------------------
        // if GlbKmTrue = 561 then sablog('xa = ' + inttostr(xa) + '  xb = ' + inttostr(xb));

        xa := L04;
        xb := Lm4;

        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;
        // sablog('kmtrue=' + inttostr(GlbKmTrue) + ' leng' + inttostr(leng) );
        if Leng mod 4 > 0 then
          bolshek := 1;
        count := Leng div 4 + bolshek;
        if count = 0 then
          count := 0; // 4 m dlya shablona
        // sablog('kmtrue=' + inttostr(GlbKmTrue) + ' ush_s2=' + inttostr(ush_s2));
        WUsh[ns].onswitch := ProberkaNaStrelku(L04, Lm4, 1);

        if not(WUsh[ns].onswitch) then
          ush_s2 := ush_s2 + count;
        // -------------------------
        WUsh[ns].st := 2;
        WUsh[ns].s3 := s2;
        WUsh[ns].bel := Nms1+max4; // max4;
        WUsh[ns].val := max4;
        WUsh[ns].L0 := L04;
        WUsh[ns].Lm := Lm4;
        WUsh[ns].v := vt;
        WUsh[ns].vg := vtg;
        // sablog('kmtrue=' + inttostr(GlbKmTrue) + ' L04=' + inttostr(xa) + ' Lm4=' + inttostr(xb));
        WUsh[ns].count := count;
        WUsh[ns].Leng := Leng;

        ns := ns + 1;
        // сброс флагов
        flg4st := false;
        k3 := 0;
      end;
      k3 := 0;
      flg4st := false;

      max4 := 0;
      fff := 0;
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------
    Setlength(WUsh, ns);
    // ------------------------------------------------------------------------------
    str_p := '';

  except

  end; // try
end;

// ------------------------------------------------------------------------------
// Уширение для 1 степени
// ------------------------------------------------------------------------------
procedure GUsh_1;
var
  i, j, k, k3, fkm, beta1, imax: integer;
  Fh, L04, Lm4, fff: integer;
  Nms1, Rds1, Vk, s1, vt, vtg: integer;
  max, OtklUsh: real;
Begin

  i := 0;
  k := 0;
  k3 := 0;
  max := 0;
  FG[Low(FG)] := F_Sh[low(F_Sh)]; // 1520;
  FG[High(FG)] := F_Sh[high(F_Sh)]; // 1520;

  while i <= High(FG) do
  begin
    Nms1 := round(F0_Sh[i]);
    Fh := round(F_Sht[i] - F0_Sh[i]); // 1520;  - F_Norm[i]);

    if Fh < 0 then
      Fh := 0;

    Rds1 := Prds[i];
    fkm := FGm[i];

    k := AlpS(Nms1, Rds1);
    if (GlobPassSkorost < 50) then
      beta1 := 2
    else
      beta1 := 0; // and (k < 5)

    vt := F_V[i];
    vtg := F_Vg[i];
    j := G_Ind1(PasGruzSkor, vt);

    s1 := DeltaUsh[j, 1, k] + beta1;

    if (8 < Fh) and (Fh <= s1) then
    begin
      OtklUsh := Fh;

      k3 := k3 + 1;
      OtklUsh := Fh; // round(fg[i]);

      if (k3 = 1) then
      begin
        L04 := fkm;
        Lm4 := fkm;
      end
      else
        Lm4 := fkm;

      if (max < OtklUsh) then
      BEGIN
        max := OtklUsh;
        imax := i;
        fff := round(F_Sht[imax]);
      END;

      i := i + 1;
      continue;
    end;

    if (k3 > 1) then
    begin
      fdBroad := fdBroad + (k3 div 4);
      if k3 mod 4 > 0 then
        fdBroad := fdBroad + 1;
      Setlength(WUsh, length(WUsh) + 1);
      // -------------------------
      WUsh[ns].st := 1;
      WUsh[ns].bel := fff; // max4;
      WUsh[ns].val := max4;
      WUsh[ns].L0 := L04;
      WUsh[ns].Lm := Lm4;
      WUsh[ns].v := vt;
      WUsh[ns].vg := vtg;
      // sablog('kmtrue=' + inttostr(GlbKmTrue) + ' L04=' + inttostr(xa) + ' Lm4=' + inttostr(xb));

      WUsh[ns].count := k3 div 4;
      if k3 mod 4 > 0 then
        WUsh[ns].count := WUsh[ns].count + 1;

      WUsh[ns].Leng := k3;

      ns := ns + 1;
    end;
    k3 := 0;
    i := i + 1;
    max := 0;
    fff := 0;

  end; // while1
  // ------------------------------------------------------------------------------
end;

/// /////////////////////////////////////////////////////////////
// REMONTNIY KM                                              //
/// /////////////////////////////////////////////////////////////
procedure RUroven_4;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  ogranichenie: string;
  ogranichenie1: integer;

  flg2st2, flg3st2, flg4st2: boolean;
  flg2st3, flg3st3, flg4st3, flgk1, flgk2: boolean;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs, j12, j13: integer;

  xxxx, ogr_min, v1, v2, u1, u2: integer;
  tmps: string;

Begin
  Try
    // SabLog('Gurv - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    Setlength(WUsh, 3000);
    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    flg4st := false;

    j12 := 0;
    j13 := 0;
    j := G_Ind2_Remont(GlbTempTipRemontKm, GlbSkorostRemontKm);
    FG[0] := Pnom[Low(Pnom)];
    FG[High(FG)] := Pnom[High(Pnom)];

    while i <= High(FG) do
    begin

      Fh := abs(abs(FG[i]) - abs(Pnom[i]));
      Nms1 := Pnom[i];
      fkm := FGm[i];
      s3 := RDeltaUrb[j];

      if (Fh > s3) then
      begin
        OtklUsh := Fh;
        // 4 st
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm + 100 * NAPR_DBIJ;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklUsh) then
          max4 := OtklUsh;

        i := i + 1;
        continue;
      end; // if

      if (L04 > 0) and (Lm4 > 0) and (abs(L04 - Lm4) > 2000) and (max4 > 0) then
      begin
        flg4st := true;

        stv := 4;
        belv := max4;
        L0v := L04;
        Lmv := Lm4;
        WUsh[ns].st := 4;
        WUsh[ns].bel := max4;
        WUsh[ns].L0 := L04;
        WUsh[ns].Lm := Lm4;
        WUsh[ns].Leng := k3;
        ogranichenie1 := OpredelenieOgr_SkorostiRem_Uroben(belv);
        v1 := G_Ind2_skorost2Remont(GlbTempTipRemontKm, ogranichenie1);

        { if not FlagSiezd(L0v div 100, Lmv div 100)
          and not ProberkaNaStrelku(L0v div 100,7) and not ProberkaNaStrelku(Lmv div 100,1) then
          begin
          if shekteu(L0v div 100 , Lmv div 100, ogranichenie1) then
          begin
          xxxx:= ((L0v div 100) mod 1000) div 100 + 1;

          WRT_UBEDOM(L0v, Lmv, 4,
          'У ' + Inttostr(max4) + ' пк' + inttostr(xxxx) + ' v=' + inttostr(ogranichenie1),
          Inttostr(max4),inttostr(ogranichenie1),inttostr(ogranichenie1)); //FormatFloat('0.00',max4)
          end;
          end; }

        { if not FlagSiezd(L0v div 100, Lmv div 100)
          and not ProberkaNaStrelku(L0v div 100,7) and not ProberkaNaStrelku(Lmv div 100,1) then
          begin
          if shekteu(L0v div 100 , Lmv div 100, v1) then
          begin
          xxxx:= ((L0v div 100) mod 1000) div 100 + 1;

          WRT_UBEDOM(L0v, Lmv, 4,
          'У v=' + v_shekti(v1, v1)+ ' пк' + inttostr(xxxx), v1, v1); //FormatFloat('0.00',max4)
          end;
          end; }
        ns := ns + 1;
        // сброс флагов
        flg4st := false;

      end;

      max2 := 0;
      max3 := 0;
      max4 := 0;
      k1 := 0;
      k2 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;
      j12 := 0;
      j13 := 0;

      // ----------------------------------------------------------------------------
      i := i + 1;

    end; // while1
    // ------------------------------------------------------------------------------

    Setlength(WUsh, ns);

    MTmp := Nil;

  except

  end; // try
end;

procedure GUroven150;
var
  i, j, pg, k4, ln4, k, x4a, x4b, xxxx, Leng, ogranichenie1, vt, vtg, bolshek10,
    v1, v2, vr, Vrg, count, fi: integer;
  m4, Fh: real;
  ots: string;
begin

  i := 2;
  while i < high(F_mtr) - 1 do
  begin
    j := i;
    m4 := 0;
    k4 := 0;
    x4a := 0;
    x4b := 0;

    Fh := abs(Furb1[i]);
    vt := F_V[i];
    pg := 0;
    vtg := F_Vg[i];
    vr := F_Vrp[i];
    Vrg := F_Vrg[i];
    k := G_Ind2(pg, vt);

    while (j <= high(F_mtr)) and (150 < Fh) do
    begin
      Fh := abs(Furb1[j]);

      if Urob[j] <> 0 then
        Fh := 0;

      vt := F_V[j];
      pg := 0;
      vtg := F_Vg[j];
      vr := F_Vrp[j];
      Vrg := F_Vrg[j];

      k := G_Ind2(pg, vt);

      if (Fh > 150) and (Urob[j] = 0) then
      begin
        k4 := k4 + 1;
        if Fh > m4 then
          m4 := Fh;
        if k4 = 1 then
        begin
          x4a := F_mtr[j];
          if j - 1 >= 0 then
            x4b := F_mtr[j - 1];
        end
        else
          x4b := F_mtr[j];
      end;
      if (abs(x4b - F_mtr[j]) = 1) and (k4 > 1) then
        break;
      j := j + 1;
    end;

    i := j;
    if (k4 > 0) then
    begin
      for fi := i - (k4 + 1) to i do
      begin
        Urob[fi] := 1;
      end;
      ln4 := k4;
      v1 := 0;
      v2 := 0;

      if ((v1 >= 0) and (v1 < vt)) or ((v2 >= 0) and (v2 < vtg)) then
      begin
        i := i + k4;

        pot_s4 := pot_s4 + k4;
        glbCount_Urv4s := glbCount_Urv4s + 1;
        xxxx := (x4a div 100) + 1;
        Setlength(WUsh, ns + 1);
        WUsh[ns].st := 4;
        WUsh[ns].bel := round(m4);
        WUsh[ns].L0 := x4a + k4 * NAPR_DBIJ;
        WUsh[ns].Lm := x4b;
        WUsh[ns].Leng := k4;
        WUsh[ns].count := 1;
        WUsh[ns].v := vt;
        WUsh[ns].vg := vtg;
        WUsh[ns].vop := v1; // ogranichenie1;
        WUsh[ns].vog := v2; // ogranichenie1;
        WUsh[ns].Vrp := vr;
        WUsh[ns].Vrg := Vrg;
        WUsh[ns].flg := false;
        if (v1 = -1) and (v2 = -1) then
          WUsh[ns].prim := GlbTempTipRemontKm;
        ns := ns + 1;

        WRT_UBEDOM(x4a, x4b, 4, 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx)
          + ' У; ', v1, v2);

        k4 := 0;
      end;
    end;
    // ---------------------------------------------
    v1 := -1;
    v2 := -1;
    i := i + 1;
  end;
end;

procedure GUroven75;
var
  i, j, pg, k4, ln4, k, x4a, x4b, xxxx, Leng, ogranichenie1, vt, vtg, bolshek10,
    v1, v2, vr, Vrg, count, fi: integer;
  m4, Fh: real;
  ots: string;
begin

  i := 2;
  while i < high(F_mtr) - 1 do
  begin
    j := i;
    m4 := 0;
    k4 := 0;
    x4a := 0;
    x4b := 0;

    Fh := abs(Furb1[i]);
    vt := F_V[i];
    pg := 0;
    vtg := F_Vg[i];
    vr := F_Vrp[i];
    Vrg := F_Vrg[i];
    k := G_Ind2(pg, vt);

    while (j <= high(F_mtr)) and (75 < Fh) and (CheckForFactSwitch(F_mtr[i])) do
    begin
      Fh := abs(Furb1[j]);

      if Urob[j] <> 0 then
        Fh := 0;

      vt := F_V[j];
      pg := 0;
      vtg := F_Vg[j];
      vr := F_Vrp[j];
      Vrg := F_Vrg[j];

      k := G_Ind2(pg, vt);

      if (Fh > 75) and (Urob[j] = 0) then
      begin
        k4 := k4 + 1;
        if Fh > m4 then
          m4 := Fh;
        if k4 = 1 then
        begin
          x4a := F_mtr[j];
          if j - 1 >= 0 then
            x4b := F_mtr[j - 1];
        end
        else
          x4b := F_mtr[j];
      end;
      if (abs(x4b - F_mtr[j]) = 1) and (k4 > 1) then
        break;
      j := j + 1;
    end;

    i := j;
    if (k4 > 0) then
    begin
      for fi := i - k4 to i do
      begin
        Urob[fi] := 1;
      end;
      ln4 := k4;
      v1 := 0;
      v2 := 0;

      if ((v1 >= 0) and (v1 < vt)) or ((v2 >= 0) and (v2 < vtg)) then
      begin
        i := i + k4;

        pot_s4 := pot_s4 + k4;
        glbCount_Urv4s := glbCount_Urv4s + 1;
        xxxx := (x4a div 100) + 1;
        Setlength(WUsh, ns + 1);
        WUsh[ns].st := 4;
        WUsh[ns].bel := round(m4);
        WUsh[ns].L0 := x4a + k4 * NAPR_DBIJ;
        WUsh[ns].Lm := x4b;
        WUsh[ns].Leng := k4;
        WUsh[ns].count := 1;
        WUsh[ns].v := vt;
        WUsh[ns].vg := vtg;
        WUsh[ns].vop := v1; // ogranichenie1;
        WUsh[ns].vog := v2; // ogranichenie1;
        WUsh[ns].Vrp := vr;
        WUsh[ns].Vrg := Vrg;
        WUsh[ns].flg := false;
        if (v1 = -1) and (v2 = -1) then
          WUsh[ns].prim := GlbTempTipRemontKm;
        ns := ns + 1;

        WRT_UBEDOM(x4a, x4b, 4, 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx)
          + ' У; ', v1, v2);

        k4 := 0;
      end;
    end;
    // ---------------------------------------------
    v1 := -1;
    v2 := -1;
    i := i + 1;
  end;
end;

procedure GUroven4;
var
  i, j, pg, k4, ln4, k, x4a, x4b, xxxx, Leng, ogranichenie1, s1, s2, s3, vt,
    vtg, bolshek10, v1, v2, vr, Vrg, count, fi: integer;
  m4, Fh: real;
  delta_length: integer;
  ots: string;
begin
  delta_length := 10;
  i := 2;
  while i < high(F_mtr) - 1 do
  begin
    j := i;
    m4 := 0;
    k4 := 0;
    x4a := 0;
    x4b := 0;

   Fh := abs(abs(Furb1[j]) - abs(Trapezlevel[j]));



    vt := F_V[i];
    pg := 0;
    vtg := F_Vg[i];
    vr := F_Vrp[i];
    Vrg := F_Vrg[i];
    k := G_Ind2(pg, vt);
    s0 := DeltaUrb[1, k] + F_puch[i];
    s1 := DeltaUrb[2, k] + F_puch[i];

    while (j <= high(F_mtr)) and (s0 < Fh) do
    begin
      Fh := abs(abs(Furb1[j]) - abs(Trapezlevel[j]));

      if Urob[j] <> 0 then
        Fh := 0;
      vt := F_V[j];
      pg := 0;
      vtg := F_Vg[j];
      vr := F_Vrp[j];
      Vrg := F_Vrg[j];

      k := G_Ind2(pg, vt);

      s3 := DeltaUrb[4, k] + F_puch[i];

      if (Fh > s3) and (Urob[j] = 0) then
      begin
        k4 := k4 + 1;
        if Fh > m4 then
          m4 := Fh;
        if k4 = 1 then
        begin
          x4a := F_mtr[j];
          if j - 1 >= 0 then
            x4b := F_mtr[j - 1];
        end
        else
          x4b := F_mtr[j];
      end;
      if (abs(x4b - F_mtr[j]) = 1) and (k4 > 1) then
        break;
      j := j + 1;
    end;

    i := j;
    if (k4 > delta_length) and not NecorrPasportFlag then
    begin
      for fi := i - k4 to i do
      begin
        Urob[fi] := 1;
      end;
      ln4 := k4 - delta_length;
      v1 := V_ogr_UPP(0, vt, round(m4), 3);
      v2 := V_ogr_UPP(1, vtg, round(m4), 3);

      if GlbFlagRemontKm and (RemTab9 > 0) then
      begin
        v1 := VTab9(1, round(m4), vt);
        v2 := VTab9(1, round(m4), vtg);

        if v1 = -1 then
          v1 := V_ogr_UPP(0, vt, round(m4), 3);
        if v2 = -1 then
          v2 := V_ogr_UPP(1, vtg, round(m4), 3);
      end;

      if ((v1 >= 0) and (v1 < vt)) or ((v2 >= 0) and (v2 < vtg)) then
      begin
        i := i + k4;

        if (v1 >= vr) and (RemTab9 = 0) then
          v1 := -1;
        if (v2 >= Vrg) and (RemTab9 = 0) then
          v2 := -1;

        pot_s4 := pot_s4 + k4;
        glbCount_Urv4s := glbCount_Urv4s + 1;
        xxxx := (x4a div 100) + 1;
        Setlength(WUsh, ns + 1);
        WUsh[ns].st := 4;
        WUsh[ns].bel := round(m4);
        WUsh[ns].L0 := x4a ;
          //   WUsh[ns].L0 := x4a + delta_length * NAPR_DBIJ;
        WUsh[ns].Lm := x4b;
        WUsh[ns].Leng := k4;
        WUsh[ns].count := 1;
        WUsh[ns].v := vt;
        WUsh[ns].vg := vtg;
        WUsh[ns].vop := v1; // ogranichenie1;
        WUsh[ns].vog := v2; // ogranichenie1;
        WUsh[ns].Vrp := vr;
        WUsh[ns].Vrg := Vrg;
        WUsh[ns].flg := false;
        if (v1 = -1) and (v2 = -1) then
          WUsh[ns].prim := GlbTempTipRemontKm;
        ns := ns + 1;

        WRT_UBEDOM(x4a, x4b, 4, 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx)
          + ' У '+inttostr(WUsh[ns].Leng)+';  ', v1, v2);

        k4 := 0;
      end;
    end;
    // ---------------------------------------------

    v1 := -1;
    v2 := -1;
    i := i + 1;
  end;

end;

procedure GUroven3;
var
  i, j, pg, k4, ln4, k, x4a, x4b, xxxx, Leng, ogranichenie1, s1, s2, vt, vtg,
    bolshek10, v1, v2, vr, Vrg, count, fi: integer;
  m4, Fh: real;
  delta_length: integer;
begin
  delta_length := 10;
  i := 2;
  while i < high(F_mtr) - 1 do
  begin
    j := i;
    m4 := 0;
    k4 := 0;
    x4a := 0;
    x4b := 0;

    Fh := abs(abs(Furb1[j]) - abs(Trapezlevel[j]));



    vt := F_V[i];
    pg := 0;
    vtg := F_Vg[i];
    vr := F_Vrp[i];
    Vrg := F_Vrg[i];
    k := G_Ind2(pg, vt);
    s0 := DeltaUrb[1, k] + F_puch[i];
    s1 := DeltaUrb[2, k] + F_puch[i];

    while (j <= high(F_mtr)) and (s0 < Fh) do
    begin
      Fh := abs(abs(Furb1[j]) - abs(Trapezlevel[j]));

      if Urob[j] <> 0 then
        Fh := 0;
      vt := F_V[j];
      pg := 0;
      vtg := F_Vg[j];
      vr := F_Vrp[j];
      Vrg := F_Vrg[j];

      k := G_Ind2(pg, vt);

      s2 := DeltaUrb[3, k] + F_puch[i];

      if (Fh > s2) and (Urob[j] = 0) then
      begin
        k4 := k4 + 1;
        if Fh > m4 then
          m4 := Fh;
        if k4 = 1 then
        begin
          x4a := F_mtr[j];
          if j - 1 >= 0 then
            x4b := F_mtr[j - 1];
        end
        else
          x4b := F_mtr[j];
      end;
      if (abs(x4b - F_mtr[j]) = 1) and (k4 > 1) then
        break;
      j := j + 1;
    end;

    i := j;
    // ---------------------------------------------
    if (k4 > delta_length) then
    begin

      ln4 := k4 - delta_length; // round(abs(k3-k4));
      for fi := i - k4 to i do
      begin
        Urob[fi] := 1;
      end;
      if (x4a > 0) and (x4b > 0) then
      begin
        // ln3:= round(abs(x3a-x3b)/100);
        bolshek10 := 0;
        if ln4 mod 10 > 0 then
          bolshek10 := 1;
        count := (ln4 div 10) + bolshek10;
        if count = 0 then
          count := 1;
        pot_s3 := pot_s3 + count;
        Setlength(WUsh, ns + 1);
        WUsh[ns].st := 3;
        WUsh[ns].bel := min(round(m4),DeltaUrb[3, 4] )-5;  // round(m4); //
        WUsh[ns].L0 := x4a ;
           // WUsh[ns].L0 := x4a + delta_length * NAPR_DBIJ;
        WUsh[ns].Lm := x4b;
        WUsh[ns].Leng := k4;
        WUsh[ns].count := count;
        WUsh[ns].v := vt;
        WUsh[ns].vg := vtg;
        WUsh[ns].vop := -1;
        WUsh[ns].vog := -1;
        WUsh[ns].Vrp := vr;
        WUsh[ns].Vrg := Vrg;
        WUsh[ns].flg := false;
        WUsh[ns].prim := '';
        ns := ns + 1;

        x4a := 0;
        x4b := 0;
        k4 := 0;
      end;
    end;

    v1 := -1;
    v2 := -1;
    i := i + 1;
  end;
end;

procedure GUroven2;
var
  i, j, pg, k4, ln4, k, x4a, x4b, xxxx, Leng, ogranichenie1, s1, s2, vt, vtg,
    bolshek10, v1, v2, vr, Vrg, count, fi: integer;
  m4, Fh: real;
  delta_length: integer;
begin
  delta_length := 20;
  i := 2;
  while i < high(F_mtr) - 1 do
  begin
    j := i;
    m4 := 0;
    k4 := 0;
    x4a := 0;
    x4b := 0;

    Fh := abs(abs(Furb1[i]) - abs(Trapezlevel[i]));

    //if NecorrPasportFlag then
     // Fh := abs(Furb1[i]) - abs(Furbpersred[i]);

    vt := F_V[i];
    pg := 0;
    vtg := F_Vg[i];
    vr := F_Vrp[i];
    Vrg := F_Vrg[i];
    k := G_Ind2(pg, vt);
    s0 := DeltaUrb[1, k] + F_puch[i];
    s1 := DeltaUrb[2, k] + F_puch[i];

    while (j <= high(F_mtr)) and (s0 < Fh) do
    begin
      Fh := abs(abs(Furb1[j]) - abs(Trapezlevel[j]));

      if Urob[j] <> 0 then
        Fh := 0;

      vt := F_V[j];
      pg := 0;
      vtg := F_Vg[j];
      vr := F_Vrp[j];
      Vrg := F_Vrg[j];

      k := G_Ind2(pg, vt);

      s1 := DeltaUrb[2, k] + F_puch[i];

      if (Fh > s1) and (Urob[j] = 0) then
      begin
        k4 := k4 + 1;
        if Fh > m4 then
          m4 := Fh;
        if k4 = 1 then
        begin
          x4a := F_mtr[j];
          if j - 1 >= 0 then
            x4b := F_mtr[j - 1];
        end
        else
          x4b := F_mtr[j];
      end;
      if (abs(x4b - F_mtr[j]) = 1) and (k4 > 1) then
        break;
      j := j + 1;
    end;

    i := j;
    // ---------------------------------------------
    if (k4 > delta_length) then
    begin
      for fi := i - k4 to i do
      begin
        Urob[fi] := 1;
      end;
      if (x4a > 0) and (x4b > 0) then
      begin
        ln4 := k4; // - delta_length;

        bolshek10 := 0;

        count := 1;

        if (ln4 > 15) then
        begin
          if ((ln4 - 15) mod 10) > 0 then
            bolshek10 := 1;
          count := ((ln4 - 15) div 10) + bolshek10 + 1;
        end;

        pot_s2 := pot_s2 + count;
        Setlength(WUsh, ns + 1);
        WUsh[ns].st := 2;
        WUsh[ns].s3 := s2;
        WUsh[ns].bel := s3;
        WUsh[ns].L0 := x4a ;
           //WUsh[ns].L0 := x4a + delta_length * NAPR_DBIJ;
        WUsh[ns].Lm := x4b;
        WUsh[ns].v := vt;
        WUsh[ns].Leng := k4;
        WUsh[ns].count := count;
        WUsh[ns].vg := vtg;
        WUsh[ns].vop := -1;
        WUsh[ns].vog := -1;
        WUsh[ns].Vrp := vr;
        WUsh[ns].Vrg := Vrg;
        WUsh[ns].flg := false;
        ns := ns + 1;

        x4a := 0;
        x4b := 0;
        k4 := 0;
      end;
    end;

    v1 := -1;
    v2 := -1;
    i := i + 1;
  end;
end;

procedure GUroven1;
var
  i, j, pg, k4, ln4, k, x4a, x4b, xxxx, Leng, ogranichenie1, s0, vt, vtg,
    bolshek10, v1, v2, vr, Vrg, count, fi: integer;
  m4, Fh: real;
  delta_length: integer;
begin
  delta_length := 10;
  i := 2;
  while i < high(F_mtr) - 1 do
  begin
    j := i;
    m4 := 0;
    k4 := 0;
    x4a := 0;
    x4b := 0;

    Fh := abs(abs(Furb1[i]) - abs(Trapezlevel[i])); // F_urb_Per

    // Fh :=  abs(F_urb_Per[i]);         //      Fh := abs(abs(Furb1[i]) - abs(F_urb_Per_sr[i]));
    if NecorrPasportFlag then
      Fh := abs(abs(Furb1[i]) - abs(Trapezlevel[i]));

    // Fh :=  abs(F_urb_Per[i]);
    vt := F_V[i];
    pg := 0;
    vtg := F_Vg[i];
    vr := F_Vrp[i];
    Vrg := F_Vrg[i];
    k := G_Ind2(pg, vt);
    s0 := DeltaUrb[1, k] + F_puch[i];

    while (j <= high(F_mtr)) and (s0 < Fh) do
    begin
      Fh := abs(abs(Furb1[j]) - abs(Trapezlevel[j]));

      if Urob[j] <> 0 then
        Fh := 0;

      vt := F_V[j];
      pg := 0;
      vtg := F_Vg[j];
      vr := F_Vrp[j];
      Vrg := F_Vrg[j];

      k := G_Ind2(pg, vt);

      s0 := DeltaUrb[1, k] + F_puch[i];

      if (Fh > s0) and (Urob[j] = 0) then
      begin
        k4 := k4 + 1;
        if Fh > m4 then
          m4 := Fh;
        if k4 = 1 then
        begin
          x4a := F_mtr[j];
          if j - 1 >= 0 then
            x4b := F_mtr[j - 1];
        end
        else
          x4b := F_mtr[j];
      end;
      if (abs(x4b - F_mtr[j]) = 1) and (k4 > 1) then
        break;
      j := j + 1;
    end;

    i := j;
    // ---------------------------------------------
    if (k4 > delta_length) then
    begin
      for fi := i - k4 to i do
      begin
        Urob[fi] := 0;
      end;
      if (x4a > 0) and (x4b > 0) then
      begin
        ln4 := k4 - delta_length;

        bolshek10 := 0;

        count := 1;

        if (ln4 > 15) then
        begin
          if ((ln4 - 15) mod 10) > 0 then
            bolshek10 := 1;
          count := ((ln4 - 15) div 10) + bolshek10 + 1;


        pot_s2 := pot_s2 + count;

        // bolshek10:= 0;
        // if ln2 mod 10 > 0 then bolshek10:= 1;
        // count:= (ln2 div 10) + bolshek10;
        // if count = 0 then count:= 1;
        // pot_s2:= pot_s2 + count;
        Setlength(WUsh, ns + 1);
        WUsh[ns].st := 1;
        WUsh[ns].bel := round(m4);
        WUsh[ns].L0 := x4a ;
        // WUsh[ns].L0 := x4a + delta_length * NAPR_DBIJ;
        WUsh[ns].Lm := x4b;
        WUsh[ns].v := vt;
        WUsh[ns].Leng := ln4;
        WUsh[ns].count := count;
        WUsh[ns].vg := vtg;
        WUsh[ns].vop := -1;
        WUsh[ns].vog := -1;
        WUsh[ns].Vrp := vr;
        WUsh[ns].Vrg := Vrg;
        WUsh[ns].flg := false;
        ns := ns + 1;
        x4a := 0;
        x4b := 0;
        k4 := 0;
       end;
      end;
    end;
    v1 := -1;
    v2 := -1;
    i := i + 1;
  end;
end;

// ==============================================================================
{ procedure GUroven2;
  var
  i,j,pg,k2,k3,k4,ln2,ln3,ln4,k, x2a,x2b,x3a,x3b,x4a,x4b,xxxx,leng,
  ogranichenie1,s1,s2,s3,vt,vtg,bolshek, v1, v2, vr, vrg, count:integer;
  m2,m3,m4,fh:real;
  begin
  Setlength(WUsh,3000);
  i:= 0;
  while i<= high(X_k) do
  begin
  j:= i;
  m2:= 0; m3:= 0; m4:= 0;
  k2:= 0; k3:= 0; k4:= 0;
  x2a:= 0; x2b:= 0;  x3a:= 0; x3b:= 0; x4a:= 0; x4b:= 0;

  //Fh:= abs(abs(Furb1[i]) - abs(Furb_sr[i]));
  Fh:= abs(abs(Furb1[i]) - abs(F0_urov[i]));


  vt:= F_V[i];  pg:= 0;
  vtg:= F_Vg[i];
  vr:=  F_Vrp[i];   vrg:= F_Vrg[i];

  k:= G_Ind2(pg, vt);
  s1:= DeltaUrb[1,k];


  while (j <= high(x_k)) and (s1 < fh) do
  begin
  // Fh:= abs(abs(Furb1[j]) - abs(Furb_sr[j]));



  Fh:= Furb1[j]; //abs(abs(Furb1[j]) - abs(F0_urov[j]))
  if Urob[j] <> 0 then
  Fh:= abs(abs(Furb1[j]) - abs(Fsr_Urb[j]));//abs(Furb_sr[j]));

  vt:= F_V[j];  pg:= 0;
  vtg:= F_Vg[j];
  vr:=  F_Vrp[j];   vrg:= F_Vrg[j];

  k:= G_Ind2(pg, vt);
  s1:= DeltaUrb[1,k];
  s2:= DeltaUrb[2,k];
  s3:= DeltaUrb[3,k];

  k2:= k2 + 1;

  if (fh > m2) and (fh <= s2) then m2:= fh;


  if (k2 = 1)   then
  begin
  x2a:= X_k[j];
  if j - 1 >= 0 then x2b:= X_k[j-1];
  end
  else
  if (k2 > 1)  then x2b:= X_k[j];


  if ((s2 < fh)  and (Urob[j] = 0)) then
  begin
  k3:= k3 + 1;
  if (fh > m3) and (fh <= s3) then m3:= fh;
  if k3 = 1 then
  begin
  x3a:= X_k[j];
  if j-1 >= 0 then x3b:= X_k[j-1];
  end else x3b:= X_k[j];


  end;


  if (fh > s3) and (Urob[j] = 0) then  //1012_2013 22:24 edit: and (Urob[j] = 0)
  begin
  k4:= k4 + 1;
  if fh > m4 then m4:= fh;
  if k4 = 1 then
  begin
  x4a:= X_k[j];
  if j-1 >= 0 then x4b:= X_k[j-1];
  end else x4b:= X_k[j];
  end;

  j:= j + 1;
  end;

  i:= j;
  //---------------------------------------------
  //  if (k2 = 1) and (i <= high(x_k)) then x2b:= X_k[i];

  if (k2 >= 10) then
  begin

  if (x2a > 0) and (x2b > 0) then
  begin
  ln2:= round(abs(x2a-x2b)/100);

  bolshek:= 0;
  if ln2 mod 10 > 0 then bolshek:= 1;
  count:= (ln2 div 10) + bolshek;
  if count = 0 then count:= 1;
  pot_s2:= pot_s2 + count;

  WUsh[ns].st:= 2;   WUsh[ns].bel:= round(m2);
  WUsh[ns].L0:= x2a; WUsh[ns].Lm:=  x2b;
  WUsh[ns].v := vt;
  WUsh[ns].Leng:= ln2;
  WUsh[ns].count:= count;
  WUsh[ns].vg := vtg;
  WUsh[ns].vop:= -1;
  WUsh[ns].vog:= -1;
  WUsh[ns].vrp:= vr;
  WUsh[ns].vrg:= vrg;
  WUsh[ns].flg:= false;
  ns:= ns + 1;

  x2a:= 0;
  x2b:= 0; k2:= 0;
  end;
  end;

  if (k3 >= 1) then
  begin
  ln3:= round(abs(k3-k4));
  if (x3a > 0) and (x3b > 0) then
  begin
  ln3:= round(abs(x3a-x3b)/100);
  bolshek:= 0;
  if ln3 mod 10 > 0 then bolshek:= 1;
  count:= (ln3 div 10) + bolshek;
  if count = 0 then count:= 1;
  pot_s3:= pot_s3 + count;

  WUsh[ns].st:= 3;   WUsh[ns].bel:= round(m3);
  WUsh[ns].L0:= x3a; WUsh[ns].Lm:=  x3b;
  WUsh[ns].Leng:= ln3;
  WUsh[ns].count:= count;
  WUsh[ns].v := vt;
  WUsh[ns].vg := vtg;
  WUsh[ns].vop:= -1;
  WUsh[ns].vog:= -1;
  WUsh[ns].vrp:= vr;
  WUsh[ns].vrg:= vrg;
  WUsh[ns].flg:= false;
  WUsh[ns].prim:= '';
  ns:= ns + 1;

  x3a:= 0;
  x3b:= 0;  k3:=0;
  end;
  end;


  if (k4 >= 1) then
  begin
  ln4:= k4;
  v1:= V_ogr_UPP(0, vt, round(m4), 3);
  v2:= V_ogr_UPP(1, vtg, round(m4), 3);

  if GlbFlagRemontKm and (RemTab9 > 0) then
  begin
  v1:= VTab9(1, round(m4), vt);
  v2:= VTab9(1, round(m4), vtg);

  if v1 = -1 then v1:= V_ogr_UPP(0, vt, round(m4), 3);
  if v2 = -1 then v2:= V_ogr_UPP(1, vtg, round(m4), 3);
  end;

  if ((v1 >= 0) and (v1 < vt))
  or ((v2 >= 0) and (v2 < vtg))  then
  begin
  i:= i + k4;

  if (v1 >= vr) and (RemTab9 = 0) then v1:= -1;
  if (v2 >= vrg) and (RemTab9 = 0) then v2:= -1;

  // ogranichenie1:= V_ogr_UPP(round(m4), 3);
  pot_s4:= pot_s4 + k4;
  glbCount_Urv4s:= glbCount_Urv4s + 1;
  //            xxxx:= (((x4a + x4b) div 200) mod 1000) div 100 + 1;
  xxxx:= ((x4a div 100) mod 1000) div 100 + 1;
  //0909

  WUsh[ns].st:= 4;
  WUsh[ns].bel:= round(m4);
  WUsh[ns].L0:= x4a;
  WUsh[ns].Lm:= x4b;
  WUsh[ns].Leng:= k4;
  WUsh[ns].count:= 1;
  WUsh[ns].v := vt;
  WUsh[ns].vg := vtg;
  WUsh[ns].vop:= v1;//ogranichenie1;
  WUsh[ns].vog:= v2;//ogranichenie1;
  WUsh[ns].vrp:= vr;
  WUsh[ns].vrg:= vrg;
  WUsh[ns].flg:= false;
  if (v1 = -1) and (v2 = -1) then WUsh[ns].prim:= GlbTempTipRemontKm;
  ns:= ns + 1;
  //0909
  WRT_UBEDOM(x4a, x4b, 4, 'У v=' + v_shekti(v1, v2) + ' пк'+ inttostr(xxxx) ,v1,v2);
  //FormatFloat('0.00',max4)
  //            S4_comment(7, xxxx); //01.10.2012
  k4:= 0;
  end;
  end;


  v1:= -1; v2:= -1;
  i:= i + 1;
  end;
  Setlength(WUsh,ns);
  end; }
// ------------------------------------------------------------------------------
// Uroven для 4 степени
// ------------------------------------------------------------------------------
procedure GUroven_4;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  ogranichenie: string;
  ogranichenie1: integer;

  flg2st2, flg3st2, flg4st2: boolean;
  flg2st3, flg3st3, flg4st3, flgk1, flgk2: boolean;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs, j12, j13: integer;

  xxxx, ogr_min, Leng: integer;
  tmps: string;

  maxmax: real;
Begin
  Try
    // SabLog('Gurv - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    Setlength(WUsh, 3000);
    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    flg4st := false;

    j12 := 0;
    j13 := 0;

    j := G_Ind2(PasGruzSkor, GlobPassSkorost);
    FG[0] := Pnom[Low(Pnom)];
    FG[High(FG)] := Pnom[High(Pnom)];

    while i <= High(FG) do
    begin

      maxmax := 0;

      Fh := round(abs(abs(FG[i]) - abs(Pnom[i])));
      fkm := FGm[i];

      vt := F_V[i];
      j := G_Ind2(PasGruzSkor, vt);
      s3 := DeltaUrb[3, j];

      if (Fh > s3) then
      begin
        OtklUsh := Fh; // Ошибка нулевой

        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm + 50 * NAPR_DBIJ;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklUsh) then
          max4 := OtklUsh;
        i := i + 1;
        continue;
      end; // if

      if (L04 > 0) and (Lm4 > 0) and (abs(L04 - Lm4) > 900) and
        (abs(L04 - Lm4) / 100 < 400) and (max4 > 0) then
      begin
        flg4st := true;

        stv := 4;
        belv := max4;
        L0v := L04;
        Lmv := Lm4;

        {
          WUsh[ns].st:= 4;   WUsh[ns].bel:= max4;
          WUsh[ns].L0:= L04; WUsh[ns].Lm:=  Lm4;
          ns:= ns + 1; }
        /// ogranichenie1:= V_ogr_UPP(max4, 3);

        { not FlagSiezd(L0v div 100, Lmv div 100) and not ProberkaNaStrelku(L0v div 100,7) and not ProberkaNaStrelku(Lmv div 100,1) and }
        glbCount_Urv4s := glbCount_Urv4s + 1;
        xxxx := ((L0v div 100) mod 1000) div 100 + 1;
        Leng := round(abs(L04 - Lm4) / 100);

        { WRT_UBEDOM(L0v, Lmv, 4,
          'У4 пк' + inttostr(xxxx) + ' ' + Inttostr(max4) +'мм ' + inttostr(leng) + 'м v=' + inttostr(ogranichenie1)
          ,Inttostr(max4)+' ',inttostr(ogranichenie1),inttostr(ogranichenie1)); //FormatFloat('0.00',max4)

          S4_comment(7, xxxx); }

        // сброс флагов
        flg4st := false;

      end;

      max2 := 0;
      max3 := 0;
      max4 := 0;
      k1 := 0;
      k2 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;
      j12 := 0;
      j13 := 0;

      // ----------------------------------------------------------------------------
      i := i + 1;

    end; // while1
    // ------------------------------------------------------------------------------

    Setlength(WUsh, ns);

    MTmp := Nil;

  except

  end; // try
end;

/// /////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------
// Uroven для 3 степени
// ------------------------------------------------------------------------------
procedure GUroven_3;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  ogranichenie: string;
  ogranichenie1: integer;

  flg2st2, flg3st2, flg4st2: boolean;
  flg2st3, flg3st3, flg4st3, flgk1, flgk2: boolean;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs, j12, j13: integer;

  L02_new: array [0 .. 2] of integer; // (j12)
  Lm2_New: array [0 .. 2] of integer;

  L03_new: array [0 .. 2] of integer; // (j12)
  Lm3_New: array [0 .. 2] of integer;

  max2_new: array [0 .. 2] of integer;
  max3_new: array [0 .. 2] of integer;

  xxxx: integer;
  tmps: string;
  maxmax: real;
  xa, xb, count, bolshek, Leng, typev: integer;
Begin
  Try
    // SabLog('Gurv - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    k := 0;
    Setlength(WUsh, 3000);
    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;

    L04 := 0;
    Lm4 := 0;

    flg4st := false;
    // j:= G_Ind2(PasGruzSkor,GlobPassSkorost);

    FG[0] := Pnom[Low(Pnom)];
    FG[High(FG)] := Pnom[High(Pnom)];

    while i <= High(FG) do
    begin

      Fh := abs(abs(FG[i]) - abs(Pnom[i]));

      Nms1 := Pnom[i];
      fkm := FGm[i];

      vt := F_V[i];
      typev := 0;
      j := G_Ind2(typev, vt);
      s1 := DeltaUrb[1, j];
      s2 := DeltaUrb[2, j];
      s3 := DeltaUrb[3, j];

      if (s2 < Fh) and (Fh <= s3) then
      begin
        OtklUsh := Fh;

        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm + 50 * NAPR_DBIJ;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklUsh) then
          max4 := OtklUsh;
        i := i + 1;
        continue;
      end; // if

      if (L04 > 0) and (Lm4 > 0) and (abs(L04 - Lm4) / 100 < 400) and (max4 > 0)
        and (abs(L04 - Lm4) > 900) then
        flg4st := true;

      if flg4st then
      begin
        stv := 3;
        belv := max4;
        L0v := L04;
        Lmv := Lm4;

        WUsh[ns].st := 3;
        WUsh[ns].bel := s3;
        WUsh[ns].L0 := L04;
        WUsh[ns].Lm := Lm4;
        ns := ns + 1;

        // ------------------------------
        xa := L04 div 100;
        xb := Lm4 div 100;
        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;
        if Leng mod 10 > 0 then
          bolshek := 1;
        count := Leng div 10 + bolshek; // dlya plabnogo otkl 10 metr
        pot_s3 := pot_s3 + count;

        xa := (L04 div 100) mod 1000;
        xb := (Lm4 div 100) mod 1000;
        glb3 := glb3 + 'У(' + inttostr(xa) + 'м,L-' +
          floattostr(abs(xa - xb)) + 'м) ';

        // ------------------------------

        // сброс флагов
        flg4st := false;
      end;
      flg4st := false;

      max2 := 0;
      max3 := 0;
      max4 := 0;
      k3 := 0;

      L04 := 0;
      Lm4 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------
    Setlength(WUsh, ns);
    MTmp := Nil;
  except

  end; // try
end;
// ------------------------------------------------------------------------------
// Uroven для 3 степени
// ------------------------------------------------------------------------------
{ procedure GUroven_2;
  var
  i,j,k,kk,beta1,fkm:integer;
  k1,k2,k3,dl2,dl3,dl4,Len2,Len3,len4 :integer;
  Fh,OtklUsh,L02,Lm2,L03,Lm3, Lm33,L04,Lm4,Lmv :integer;
  flg2st,flg3st,flg4st,startWhile, flg2st1, flg3st1,  flg4st1: boolean;
  stv,belv,L0v:integer;     ogranichenie:string;    ogranichenie1:integer;

  flg2st2, flg3st2,  flg4st2: boolean;
  flg2st3, flg3st3,  flg4st3, flgk1, flgk2: boolean;
  Nms1,Rds1,vk,max2,max3,max33,max4,s1,s2,s3,LenMs,j12,j13:integer;
  xxxx:integer;
  tmps:string;
  maxmax:real;
  xa,xb,count,bolshek,leng,flguuu,leng_urv,
  max_sum:integer;
  Begin
  Try
  //SabLog('Gurv - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
  //------------------------------------------------------------------------------
  if flg_urv then flguuu:= 1 else flguuu:=0;

  count_urv:=count_urv+1;
  SabLog('km-'+inttostr(glbkmindex)+' count= '+inttostr(count_urv)+
  'urv_dlina2= '+ inttostr(urv_dlina2) + 'flg_urv=' + inttostr(flguuu));

  k:= 0;
  //------------------------------------------------------------------------------
  Setlength(WUsh,3000);
  i:= 0;    k:= 0;
  max4:= 0;
  k3:= 0;

  L04:= 0; Lm4:= 0;
  flg4st:=false;
  j:= G_Ind2(PasGruzSkor,GlobPassSkorost);

  fg[0]:= Pnom[Low(Pnom)];
  fg[High(fg)]:= Pnom[High(Pnom)];

  max_sum:= 0;

  while i <= High(fg) do
  begin
  Fh:= abs( abs(fg[i]) -abs(Pnom[i]));
  Nms1:= Pnom[i];
  fkm:= fgm[i];

  vt:= F_V[i];
  j:= G_Ind2(PasGruzSkor, vt);
  s1:= DeltaUrb[1,j];
  s2:= DeltaUrb[2,j];
  s3:= DeltaUrb[3,j];

  if (s1 < fh) and (Fh <= s2) then
  begin
  OtklUsh:= fh;
  k3 := k3 + 1;
  if (k3 = 1) then begin L04:= Fkm; Lm4:= Fkm + 50*NAPR_DBIJ; end
  else Lm4:= Fkm;
  if (max4 < OtklUsh) then max4:= OtklUsh;
  i0_urv:=i;
  i:= i + 1;

  leng_urv:= round(abs(L04-Lm4));
  // SabLog('len-'+inttostr(leng_urv));
  // continue;


  end; //if

  if (s1 < fh) and (Fh <= s2) then
  begin
  OtklUsh:= fh;
  k3 := k3 + 1;
  if (k3 = 1) then begin L04:= Fkm; Lm4:= Fkm + 50*NAPR_DBIJ; end
  else Lm4:= Fkm;
  if (max4 < OtklUsh) then max4:= OtklUsh;
  i0_urv:=i;
  i:= i + 1;

  leng_urv:= round(abs(L04-Lm4));
  // SabLog('len-'+inttostr(leng_urv ));  //  + urv_dlina2


  if ((L04 div 100) mod 1000 >1)    // and (leng_urv + urv_dlina2 > 3000
  then

  begin
  // SabLog('len-'+inttostr(leng_urv ));
  stv := 2;
  belv := max4;
  L0v := urv_nach2;//L04;
  Lmv := Lm4;

  end;

  if (leng_urv + urv_dlina2 > max_sum) then // (leng_urv + urv_dlina2 > 3000)  then
  begin
  max_sum:= leng_urv + urv_dlina2;

  SabLog('len-'+inttostr(leng_urv + urv_dlina2 ));
  flg4st:= true; // and (leng_urv + urv_dlina2 > 3000
  stv := 2;
  belv := max4;
  L0v := urv_nach2;//L04;
  Lmv := Lm4;


  end;

  continue;

  end; //if

  if (L04 > 0) and (Lm4 > 0) and (abs(L04-Lm4) > 0)  and (abs(L04-Lm4)/100 < 400)
  and (max4 > 0) and (abs(L04-Lm4) > 3000) then
  begin
  flg4st:= true;
  L0v := L04;


  end;

  if flg4st  then
  begin

  stv := 2;
  belv := max4;
  L0v := urv_nach2;//L04;
  Lmv := Lm4;


  if flg_urv then L0v := urv_nach2;

  // urv_dlina2:= 0;

  WUsh[ns].st:= 2;   WUsh[ns].bel:= max4;
  WUsh[ns].L0:= L0v; WUsh[ns].Lm:= Lm4;
  ns:= ns + 1;
  //------------------------------
  xa:= L04 div 100;
  xb:= Lm4 div 100;
  leng:= k3;//round(abs(xb - xa));
  bolshek:= 0;
  if leng mod 10 > 0 then bolshek:= 1;
  count:= leng div 10 + bolshek;                                     //dlya plabnogo otkl 10 metr
  pot_s2:= pot_s2 + count;
  //------------------------------
  //сброс флагов
  flg4st:= false;
  end;
  flg4st:= false;
  max2:= 0; max3:= 0; max4:= 0;
  k3:= 0;
  L04:= 0; Lm4:= 0;
  //----------------------------------------------------------------------------
  i:=i+1;
  end;//while1

  //------------------------------------------------------------------------------

  Setlength(WUsh,ns);

  MTmp:= Nil;
  except

  end; // try
  flg_urv:= false;
  //if ((i0 = High(Pnom)-1) or (i0 = High(Pnom)) then flg_urv:= true;
  if  (i0_urv >High(Pnom)-10) then flg_urv:= true;

  if flg_urv then
  begin
  urv_dlina2:= leng_urv;//round(abs(L04-Lm4));
  urv_nach2:= L04;
  flg_urv:=false;
  end;


  SabLog(' urv_dlina2= '+ inttostr(urv_dlina2) + ' flg_urv=' + inttostr(flguuu)
  +' a='+inttostr(round(abs(L04-Lm4)) + urv_dlina2));
  end; }

procedure GUroven_2;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  ogranichenie: string;
  ogranichenie1: integer;

  flg2st2, flg3st2, flg4st2: boolean;
  flg2st3, flg3st3, flg4st3, flgk1, flgk2: boolean;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs, j12, j13: integer;
  xxxx: integer;
  tmps: string;
  maxmax: real;
  xa, xb, count, bolshek, Leng, typev: integer;
Begin
  Try
    // SabLog('Gurv - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    // ------------------------------------------------------------------------------

    k := 0;
    // ------------------------------------------------------------------------------
    Setlength(WUsh, 3000);
    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;

    L04 := 0;
    Lm4 := 0;
    flg4st := false;
    j := G_Ind2(PasGruzSkor, GlobPassSkorost);

    FG[0] := Pnom[Low(Pnom)];
    FG[High(FG)] := Pnom[High(Pnom)];

    while i <= High(FG) do
    begin
      Fh := abs(abs(FG[i]) - abs(Pnom[i]));
      Nms1 := Pnom[i];
      fkm := FGm[i];

      vt := F_V[i];
      typev := 0;
      j := G_Ind2(typev, vt);
      s1 := DeltaUrb[1, j];
      s2 := DeltaUrb[2, j];
      s3 := DeltaUrb[3, j];

      if (s1 < Fh) and (Fh <= s2) then
      begin
        OtklUsh := Fh;
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm + 50 * NAPR_DBIJ;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklUsh) then
          max4 := OtklUsh;
        i := i + 1;
        continue;
      end; // if

      if (L04 > 0) and (Lm4 > 0) and (abs(L04 - Lm4) > 0) and
        (abs(L04 - Lm4) / 100 < 400) and (max4 > 0) and (abs(L04 - Lm4) > 3000)
      then
        flg4st := true;

      if flg4st then
      begin
        stv := 2;
        belv := max4;
        L0v := L04;
        Lmv := Lm4;

        WUsh[ns].st := 2;
        WUsh[ns].bel := max4;
        WUsh[ns].L0 := L04;
        WUsh[ns].Lm := Lm4;
        ns := ns + 1;
        // ------------------------------
        xa := L04 div 100;
        xb := Lm4 div 100;
        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;
        if Leng mod 10 > 0 then
          bolshek := 1;
        count := Leng div 10 + bolshek; // dlya plabnogo otkl 10 metr
        pot_s2 := pot_s2 + count;
        // ------------------------------
        // сброс флагов
        flg4st := false;
      end;
      flg4st := false;
      max2 := 0;
      max3 := 0;
      max4 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------

    Setlength(WUsh, ns);
    MTmp := Nil;
  except

  end; // try
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// Uroven для 4 степени  для коррек.
// ------------------------------------------------------------------------------
procedure GUroven_4_PaspData;
var
  i, j, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, OtklUsh, L02, Lm2, L03, Lm3, Lm33, L04, Lm4, Lmv: integer;
  flg2st, flg3st, flg4st, startWhile, flg2st1, flg3st1, flg4st1: boolean;
  stv, belv, L0v: integer;
  ogranichenie: string;
  ogranichenie1: integer;

  flg2st2, flg3st2, flg4st2: boolean;
  flg2st3, flg3st3, flg4st3, flgk1, flgk2: boolean;
  Nms1, Rds1, Vk, max2, max3, max33, max4, s1, s2, s3, LenMs, j12, j13: integer;

  xxxx, ogr_min: integer;
  tmps: string;
Begin
  Try
    // SabLog('Gurv - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по уширению');
    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    flg4st := false;

    j12 := 0;
    j13 := 0;

    j := G_Ind2(PasGruzSkor, GlobPassSkorost);
    FG[0] := round(Pnom[Low(Pnom)]);
    FG[High(FG)] := round(Pnom[High(Pnom)]);

    while i <= High(FG) do
    begin
      Fh := round(abs(abs(FG[i]) - abs(Pnom[i])));
      if Fh > 20 then
        Nms1 := round(Pnom[i]);
      fkm := FGm[i];
      s3 := DeltaUrb[3, j];

      if (Fh > s3) then
      begin
        OtklUsh := Fh;
        // 4 st
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm + 50 * NAPR_DBIJ;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklUsh) then
          max4 := OtklUsh;

        i := i + 1;
        continue;
      end; // if

      if (L04 > 0) and (Lm4 > 0) and (abs(L04 - Lm4) > 30) and
        (abs(L04 - Lm4) < 400) and (max4 > 0) then
      begin
        flg4st := true;
        stv := 4;
        belv := max4;
        L0v := L04;
        Lmv := Lm4;

        ogranichenie1 := V_ogr_UPP(0, GlobPassSkorost, max4, 3);

        { ogranichenie1:=  OpredelenieOgr_SkorostiSuj_Uroben(stv, belv, L0v,Lmv,j);
          ogranichenie1:= G_Ind2_skorost2(ogranichenie1);
        }

        { if not FlagSiezd(L0v div 100, Lmv div 100)
          and not ProberkaNaStrelku(L0v div 100,7) and not ProberkaNaStrelku(Lmv div 100,1) then begin }

        if (not GlbFlagRemontKm and (GlobPassSkorost > ogranichenie1)) or
          (GlbFlagRemontKm and (ogranichenie1 < GlobPassSkorost)) then
        begin
          GlbCommentPaspData := 'Не корр.паспорт';
          NecorrPasportFlag := true;
        end
        else

          { end; }
          ns := ns + 1;
        flg4st := false;
      end;
      max2 := 0;
      max3 := 0;
      max4 := 0;
      k1 := 0;
      k2 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;
      j12 := 0;
      j13 := 0;
      i := i + 1;
    end; // while1
    MTmp := Nil;
  except
  end; // try
end;

// ------------------------------------------------------------------------------
// Sujenie 4 i 3
// ------------------------------------------------------------------------------

procedure GetFsuj_1512mm(FG: masr; FGm, Prds, Shp: mas; var WSuj: masots);
var
  i, j, ij, k, kk, beta1, fkm, i0: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, Fh1, OtklSuj, L02, Lm2, L03, Lm3, L04, Lm4: integer;
  flg2st, flg3st, flg4st, startWhile: boolean;
  Nms1, Rds1, Vk, max2, max3, max4, s1, s2, s3, LenOtkl: integer;
  stv, belv, L0v, Lmv: integer;
  ogranichenie: string;
  ogranichenie1: integer;
  xxxx, cou_4st, delta_shpal: integer;
  flg_add_ubed: boolean;
  imax, Leng, PasGruzSkor, vtg: integer;
  fff, minf, Fhr: real;
Begin
  Try
    // SabLog('GetFsuj - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по сужжению');
    ogranichenie1 := 140;
    cou_4st := 0;
    flg_add_ubed := false;
    Setlength(WSuj, 3000);
    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    flg4st := false;
    PasGruzSkor := 0;

    while i <= High(FG) do
    begin
      Nms1 := round(F0_Sh[i]);
      Rds1 := Prds[i];

      delta_shpal := 0;
      if Shp[i] = 0 then
      begin
        delta_shpal := 2;
      end;

      k := AlpS(Nms1, Rds1);

      vt := F_V[i];
      vtg := F_Vg[i];

      j := G_Ind1(PasGruzSkor, vt);

      s1 := DeltaSuj[j, 1, k]; // + delta_shpal;
      s2 := DeltaSuj[j, 2, k]; // + delta_shpal;
      s3 := DeltaSuj[j, 3, k]; // + delta_shpal;
      // -----------------------------------------------
      i0 := i;
      minf := 1512 - delta_shpal;

      while (i0 <= High(FG)) and (round(FG[i0]) < round(minf)) do
      begin
        minf := FG[i0];
        vt := F_V[i0];
        vtg := F_Vg[i0];

        fff := minf;
        ogranichenie1 := 0;
        L04 := FGm[i];
        Lm4 := FGm[i0];
        i0 := i0 + 1;
      end;
      // ----------------------------------------------------------------------------
      if (ogranichenie1 = 0) and (vt > 5) and (vtg > 5) then
      begin
        stv := 4;
        belv := round(fff);
        L0v := L04;
        Lmv := Lm4;
        Leng := abs(L04 - Lm4) + 1;
        flg4st := false;
        flg_add_ubed := true;

        if not FlagSiezd(L0v, Lmv) and (abs(L04 - Lm4) > 1) then
        begin
          WSuj[ns].st := 4;
          WSuj[ns].bel := round(fff);
          WSuj[ns].L0 := L04;
          WSuj[ns].Lm := Lm4;
          WSuj[ns].Leng := Leng;
          WSuj[ns].v := vt;
          WSuj[ns].vg := vtg;
          WSuj[ns].vop := 0;
          WSuj[ns].vog := 0;
          WSuj[ns].flg := false;
          WSuj[ns].onswitch := ProberkaNaStrelku(L0v, Lm4, 1);

          ns := ns + 1;
          // 0909
          if (WSuj[ns].onswitch) then
          begin
            glbCount_suj4s := glbCount_suj4s + 1;
            xxxx := L0v div 100 + 1;
            WRT_UBEDOM(L0v, Lmv, 4, 'V=0/0;' + inttostr(round(fff)) + ' пк' +
              inttostr(xxxx) + ' Суж; ', 0, 0);
          end;
          // FormatFloat('0.00',belv)  FormatFloat('0.00',fff) Inttostr(fff)
          // S4_comment(2, xxxx); //01.10.2012
        end;
        i := i + i0;
        ogranichenie1 := 140;
      end;
      max4 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;
      i := i + 1;
    end; // while1
    Setlength(WSuj, ns);
  except

  end; // try
end;

// ------------------------------------------------------------------------------
// Sujenie 4 i 3
// ------------------------------------------------------------------------------
procedure GetFsuj_4(FG: masr; FGm, Prds, Shp: mas; var WSuj: masots);
var
  i, j, ij, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, Fh1, OtklSuj, L02, Lm2, L03, Lm3, L04, Lm4: integer;
  flg2st, flg3st, flg4st, startWhile: boolean;
  Nms1, Rds1, Vk, max2, max3, max4, s1, s2, s3, LenOtkl: integer;
  stv, belv, L0v, Lmv: integer;
  ogranichenie: string;
  ogranichenie1: integer;
  xxxx, cou_4st, delta_shpal: integer;
  flg_add_ubed: boolean;
  imax: integer;
  fff: real;
  Leng, vtg, v1, v2, vr, Vrg: integer;
Begin
  Try
    if Flag_sablog then
      sablog('GetFsuj - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по сужжению');
    // ------------------------------------------------------------------------------
    cou_4st := 0;
    flg_add_ubed := false;
    Setlength(MTmp, 3000);
    k := 0;
    for i := 0 to High(FG) do
    begin
      if (F0_Sh[i] - FG[i] > 0) then
      begin
        MTmp[k].km := FGm[i];
        MTmp[k].fun := FG[i];

        MTmp[k].Nrm := round(F0_Sh[i]); // F_Norm[i];
        MTmp[k].f0 := F0_Sh[i];
        MTmp[k].Rad := Prds[i];
        // если шпал до 2005 года 1524 берем как 1520
        MTmp[k].Shpal := Shp[i];
        if (Shp[i] = 0) then
          MTmp[k].Nrm := 1520;
        k := k + 1;
      end;
    end;
    Setlength(MTmp, k);
    // ------------------------------------------------------------------------------
    // Setlength(WSuj,3000);
    i := 0;
    k := 0;
    max4 := 0;
    k3 := 0;
    L04 := 0;
    Lm4 := 0;
    flg4st := false;

    FG[Low(FG)] := F0_Sh[Low(FG)]; // 1520;
    FG[High(FG)] := F0_Sh[High(FG)]; // 1520;

    while i <= High(MTmp) do
    begin
      Fh := math.Floor(MTmp[i].f0) - math.Floor(MTmp[i].fun);
      if Fh < 0 then
        Fh := 0;

      Nms1 := MTmp[i].Nrm;
      Rds1 := MTmp[i].Rad;
      fkm := MTmp[i].km;
      k := AlpS(Nms1, Rds1);

      vt := F_V[i];
      vtg := F_Vg[i];
      vr := F_Vrp[i];
      Vrg := F_Vrg[i];

      j := G_Ind1(PasGruzSkor, vt);

      s1 := DeltaSuj[j, 1, k];
      s2 := DeltaSuj[j, 2, k];
      s3 := DeltaSuj[j, 3, k];

      fff := math.Floor(MTmp[i].fun);
      // ----------------------------------------------------------------------------
      if (Fh < 50) and (1512 > math.Floor(fff)) then // (Fh > s3) and
      begin
        OtklSuj := Fh;
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm { + 50*NAPR_DBIJ };
        end
        else
          Lm4 := fkm;

        if (max4 < OtklSuj) then
        BEGIN
          max4 := OtklSuj;
          imax := i;
          fff := math.Floor(MTmp[imax].fun);
        END;
        i := i + 1;
        continue;
      end; // if
      // ----------------------------------------------------------------------------
      if (L04 > 0) and (Lm4 > 0) and (CVSredKm > 5) then
      begin

        stv := 4;
        belv := max4;
        L0v := L04;
        Lmv := Lm4;
        Leng := abs(L04 - Lm4);
        // ogranichenie1 := V_ogr_suj(Nms1, round(fff));
        // сброс флагов
        flg4st := false;
        flg_add_ubed := true;

        v1 := V_ogr_suj(0, vt, Nms1, math.Floor(fff));
        v2 := V_ogr_suj(1, vtg, Nms1, math.Floor(fff));

        if not FlagSiezd(L0v, Lmv) and (k3 > 1) and ((v1 >= 0) and (v1 < vt)) or
          ((v2 >= 0) and (v2 < vtg)) then
        begin

          if (v1 >= vr) then
            v1 := -1;
          if (v2 >= Vrg) then
            v2 := -1;
          // 0909
          Setlength(WSuj, ns + 1);
          WSuj[ns].st := 4;
          WSuj[ns].bel := math.Floor(MTmp[imax].fun);
          WSuj[ns].L0 := L0v;
          WSuj[ns].Lm := Lmv;
          WSuj[ns].Leng := k3;
          WSuj[ns].v := vt;
          WSuj[ns].vg := vtg;
          WSuj[ns].vop := v1; // ogranichenie1;
          WSuj[ns].vog := v2; // ogranichenie1;
          WSuj[ns].flg := false;

          if (v1 = -1) and (v2 = -1) then
            WSuj[ns].prim := GlbTempTipRemontKm;
          ns := ns + 1;
          // 0909
          WSuj[ns].onswitch := ProberkaNaStrelku(L0v, Lmv, 1);
          if not(WSuj[ns].onswitch) then
          begin
            glbCount_suj4s := glbCount_suj4s + 1;
            xxxx := L0v div 100 + 1;
            WRT_UBEDOM(L0v, Lmv, 4, 'V=' + V_shekti(v1, v2) + ' пк' +
              inttostr(xxxx) + ' Суж '+ inttostr( WSuj[ns].Leng)+';  ', v1, v2);
          end;

          // FormatFloat('0.00',belv)  FormatFloat('0.00',fff)
        end;
      end;
      max4 := 0;
      k3 := 0;
      L04 := 0;
      Lm4 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------

    MTmp := nil;
  except

  end; // try
end;

procedure GetFsuj_3(FG: masr; FGm, Prds, Shp: mas; var WSuj: masots);
var
  i, j, ij, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, Fh1, OtklSuj, L02, Lm2, L03, Lm3, L04, Lm4: integer;
  flg2st, flg3st, flg4st, startWhile: boolean;
  Nms1, Rds1, Vk, max2, max3, max4, s1, s2, s3, LenOtkl: integer;
  stv, belv, L0v, Lmv, delta_shpal: integer;
  fff: integer;
  xa, xb, Leng, bolshek, count, imax, vtg: integer;
Begin
  Try
    if Flag_sablog then
      sablog('GetFsuj2 - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по сужжению');
    // ------------------------------------------------------------------------------
    // Setlength(WSuj,3000);
    i := 0;
    k := 0;
    max4 := 0;
    k1 := 0;
    k2 := 0;
    k3 := 0;
    flg4st := false;
    L04 := 0;
    Lm4 := 0;
    PasGruzSkor := 0;

    while i <= High(F_Sht) do
    begin
      Fh := math.Floor(F0_Sh[i]) - math.Floor(F_Sht[i]);

      if Fh < 0 then
      begin
        i := i + 1;
        continue;
      end;

      Nms1 := math.Floor(F0_Sh[i]);

      // если шпал до 2005 года 1524 берем как 1520
      // if (Shp[i] = 0) and (Nms1 = 1524) then
      // Nms1 := 1520;

      Rds1 := Prds[i];
      fkm := FGm[i];

      k := AlpS(Nms1, Rds1);
      count := 0; // 07.06.2013

      delta_shpal := 0;
      if (Shp[i] = 0) and (Nms1 = 1520) then
        delta_shpal := 2;

      vt := F_V[i];
      vtg := F_Vg[i];
      j := G_Ind1(PasGruzSkor, vt);
      // s1 := DeltaSuj[j, 1, k] + delta_shpal;
      // s2 := DeltaSuj[j, 2, k] + delta_shpal;
      // s3 := DeltaSuj[j, 3, k] + delta_shpal;
      s1 := DeltaSuj[j, 1, k];
      s2 := DeltaSuj[j, 2, k];
      s3 := DeltaSuj[j, 3, k] + delta_shpal;

      // if fh < s2 then fh := s2; //07.06.2013
      // ----------------------------------------------------------------------------

      if (s2 < Fh) and (Fh <= s3) then
      begin
        OtklSuj := Fh; // round(MTmp[i].fun);
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklSuj) then
        BEGIN
          max4 := OtklSuj;
          imax := i;
          belv := math.Floor(F_Sht[imax]);
        END;
        i := i + 1;
        continue;
      end; // if

      // ----------------------------------------------------------------------------
      if // (L04 <> Lm4) and
        (L04 > 0) and (Lm4 > 0) and (k3 > 1) and (max4 > 0) then
        flg4st := true;

      if flg4st then
      begin
        // ---------------------------------
        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;
        if Leng mod 4 > 0 then
          bolshek := 1;
        count := Leng div 4 + bolshek;
        if count = 0 then
          count := 1; // 07.06.2013

        // ---------------------------------
        Setlength(WSuj, ns + 1);
        WSuj[ns].st := 3;
        WSuj[ns].bel := math.Floor(F_Sht[imax]);
        WSuj[ns].L0 := L04;
        WSuj[ns].Lm := Lm4; // + LenOtkl;
        WSuj[ns].Leng := Leng;
        WSuj[ns].count := count;
        WSuj[ns].v := vt;
        WSuj[ns].vg := vtg;
        WSuj[ns].prim := '';
        WSuj[ns].onswitch := ProberkaNaStrelku(L04, Lm4, 1);
        if not(WSuj[ns].onswitch) then
          suj_s3 := suj_s3 + count;
        ns := ns + 1;

        for j := 0 to high(F_mtr) do
          if ((L04 <= F_mtr[j]) and (F_mtr[j] <= Lm4)) or
            ((L04 >= F_mtr[j]) and (F_mtr[j] >= Lm4)) then
            F_Sht[j] := 1570; // F0_sh[j];

        flg4st := false;

      end;
      L04 := 0;
      Lm4 := 0;
      max4 := 0;
      k3 := 0;
      // ----------------------------------------------------------------------------

      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------

  except
  end; // try
end;

// ------------------------------------------------------------------------------
// Sujenie3 for count > 10
// ------------------------------------------------------------------------------
function GetCountSuj3(FG: masr; FGm, Prds, Shp: mas; speed: integer): integer;
var
  i, j, ij, k, kk, beta1, fkm: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, Fh1, OtklSuj, L02, Lm2, L03, Lm3, L04, Lm4: integer;
  flg2st, flg3st, flg4st, startWhile: boolean;
  Nms1, Rds1, Vk, max2, max3, max4, s1, s2, s3, LenOtkl: integer;
  stv, belv, L0v, Lmv, delta_shpal: integer;
  fff: integer;
  xa, xb, Leng, bolshek, count, imax, retCount: integer;
Begin
  Try
    if Flag_sablog then
      sablog('GetFsuj3 - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по сужжению');
    // ------------------------------------------------------------------------------
    i := 0;
    k := 0;
    max4 := 0;
    k1 := 0;
    k2 := 0;
    k3 := 0;
    flg4st := false;
    L04 := 0;
    Lm4 := 0;
    PasGruzSkor := 0;
    retCount := 0;

    while i <= High(F_Sht) do
    begin
      Fh := round(F0_Sh[i] - F_Sht[i]);
      if Fh < 0 then
      begin
        i := i + 1;
        continue;
      end;
      Nms1 := round(F0_Sh[i]);
      Rds1 := Prds[i];
      fkm := FGm[i];
      k := AlpS(Nms1, Rds1);
      count := 0; // 07.06.2013

      delta_shpal := 0;
      if (Shp[i] = 0) // and (Nms1 = 1520)
      then
        delta_shpal := 2;

      vt := speed;
      j := G_Ind1(PasGruzSkor, vt);

      // s1 := DeltaSuj[j, 1, k] + delta_shpal;
      // s2 := DeltaSuj[j, 2, k] + delta_shpal;
      // s3 := DeltaSuj[j, 3, k] + delta_shpal;
      s1 := DeltaSuj[j, 1, k];
      s2 := DeltaSuj[j, 2, k];
      s3 := DeltaSuj[j, 3, k] + delta_shpal;
      // ----------------------------------------------------------------------------

      if (s2 < Fh) and (Fh <= s3) and not(ProberkaNaStrelku(L04, L04, 7)) then
      begin
        OtklSuj := Fh;
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm;
        end
        else
          Lm4 := fkm;
        if (max4 < OtklSuj) then
        BEGIN
          max4 := OtklSuj;
          imax := i;
          belv := round(F_Sht[imax]);
        END;
        i := i + 1;
        continue;
      end; // if

      // ----------------------------------------------------------------------------
      if // (L04 <> Lm4) and
        (L04 > 0) and (Lm4 > 0) and (max4 > 0) and
        not(ProberkaNaStrelku(L04, L04, 7)) then
        flg4st := true;

      if flg4st then
      begin
        // ---------------------------------
        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;
        if Leng mod 4 > 0 then
          bolshek := 1;
        count := Leng div 4 + bolshek;
        if count = 0 then
          count := 1; // 07.06.2013
        retCount := retCount + count;
        // ---------------------------------
        for j := 0 to High(F_mtr) do
          if ((L04 <= F_mtr[j]) and (F_mtr[j] <= Lm4)) or
            ((L04 >= F_mtr[j]) and (F_mtr[j] >= Lm4)) then
            F_Sht[j] := 1570; // F0_sh[j];

        flg4st := false;
      end;
      L04 := 0;
      Lm4 := 0;
      max4 := 0;
      k3 := 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------
    GetCountSuj3 := retCount;
  except
  end; // try
end;

// ------------------------------------------------------------------------------
// Sujenie 2
// ------------------------------------------------------------------------------
procedure GetFsuj_2(FG: masr; FGm, Prds, Shp: mas; var WSuj: masots);
var
  i, j, ij, k, kk, beta1, fkm, kkkkk: integer;
  k1, k2, k3, dl2, dl3, dl4, Len2, Len3, len4: integer;
  Fh, Fh1, OtklSuj, L02, Lm2, L03, Lm3, L04, Lm4: integer;
  flg2st, flg3st, flg4st, startWhile: boolean;
  Nms1, Rds1, Vk, max2, max3, max4, s1, s2, s3, LenOtkl: integer;
  stv, belv, L0v, Lmv, delta_shpal, vt, vtg: integer;

  xa, xb, Leng, bolshek, count, imax: integer;
Begin
  Try

    // SabLog('GetFsuj - Определение Отс, степен, величина откл., нач. и конеч. коорд. отступления по сужжению');
    // ------------------------------------------------------------------------------
    Setlength(WSuj, 3000);
    i := 0;
    k := 0;
    max4 := 0;
    k1 := 0;
    k2 := 0;
    k3 := 0;
    flg4st := false;
    L04 := 0;
    Lm4 := 0;
    PasGruzSkor := 0;

    while i <= High(FG) do
    begin
      Fh := math.Floor(F0_Sh[i]) - math.Floor(F_Sht[i]);

      if Fh < 0 then
      begin
        i := i + 1;
        continue;
      end;

      Nms1 := math.Floor(F0_Sh[i]);
      Rds1 := Prds[i];
      fkm := FGm[i];
      k := AlpS(Nms1, Rds1);

      delta_shpal := 0;
      if Shp[i] = 0 then
        delta_shpal := 2;

      k := AlpS(Nms1, Rds1);
      vt := F_V[i];
      vtg := F_Vg[i];
      j := G_Ind1(PasGruzSkor, vt);

      // s1 := DeltaSuj[j, 1, k] + delta_shpal;
      // s2 := DeltaSuj[j, 2, k] + delta_shpal;
      // s3 := DeltaSuj[j, 3, k] + delta_shpal;
      s1 := DeltaSuj[j, 1, k];
      s2 := DeltaSuj[j, 2, k];
      s3 := DeltaSuj[j, 3, k];
      if true then

        count := 0;
      // if fh < s1 then fh := s1;  //07.06.2013
      // ----------------------------------------------------------------------------
      if (s1 < Fh) and (Fh <= s2) then
      begin
        // sablog('km=' + inttostr(tekkmtrue) + ' s1 =' + floattostr(s1) + ' fh=' + floattostr(fh) +' s2=' + floattostr(s2));
        OtklSuj := Fh; // round(MTmp[i].fun);
        k3 := k3 + 1;
        if (k3 = 1) then
        begin
          L04 := fkm;
          Lm4 := fkm;
        end
        else
          Lm4 := fkm;

        if (max4 <= OtklSuj) then
        BEGIN
          max4 := OtklSuj;
          imax := i;
          belv := math.Floor(F_Sht[imax]);

        END;
        i := i + 1;
        continue;
        // sablog('belv=' + inttostr(belv) + ' --' + floattostr(f_sht[imax]));
      end; // if
      // ----------------------------------------------------------------------------
      if // (L04 <> Lm4) and
        (L04 > 0) and (Lm4 > 0) and (k3 > 1) and (max4 > 0) then
        flg4st := true;
      LenOtkl := 200;

      if flg4st then
      begin
        // ---------------------------------
        Leng := k3; // round(abs(xb - xa));
        bolshek := 0;

        if (Leng mod 4 > 0) then
          bolshek := 1;

        count := Leng div 4 + bolshek;
        // if count = 0 then count:= 1;
        // suj_s2 := suj_s2 + count;
        // suj_s2:= Suj_s2 + leng;
        // ---------------------------------
        WSuj[ns].st := 2;
        WSuj[ns].s3 := s2 - delta_shpal;
        WSuj[ns].bel := math.Floor(F_Sht[imax]);
        WSuj[ns].val := max4;
        WSuj[ns].L0 := L04;
        WSuj[ns].Lm := Lm4; // + LenOtkl;
        WSuj[ns].count := count;
        WSuj[ns].Leng := Leng;
        WSuj[ns].v := vt;
        WSuj[ns].vg := vtg;
        ns := ns + 1;
        WSuj[ns].onswitch := ProberkaNaStrelku(L04, Lm4, 1);
        if not(WSuj[ns].onswitch) then
          suj_s2 := suj_s2 + count;
        k3 := 0;
        flg4st := false;

      end;
      L04 := 0;
      Lm4 := 0;
      max4 := 0;
      // k3:= 0;
      // ----------------------------------------------------------------------------
      i := i + 1;
    end; // while1
    // ------------------------------------------------------------------------------

    { bolshek:= 0;   count := 0;
      if Suj_s2 mod 4 > 0 then bolshek:= 1;
      count:= Suj_s2 div 4 + bolshek;
      suj_s2:= count; }

    Setlength(WSuj, ns);
  except
  end; // try
end;

procedure GetFsuj_1(FG: masr; FGm, Prds, Shp: mas; var WSuj: masots);
var
  i, j, k, k3, fkm: integer;
  Fh, L04, Lm4: integer;
  Nms1, Rds1, s1: integer;
  delta_shpal, vt, vtg: integer;
  OtklSuj, max4, imax, belv: integer;
Begin
  i := 0;
  k := 0;
  k3 := 0;

  while i <= High(FG) do
  begin
    Fh := math.Ceil(F0_Sh[i] - math.Ceil(F_Sht[i]));

    if Fh < 0 then
    begin
      i := i + 1;
      continue;
    end;

    Nms1 := math.Floor(F0_Sh[i]);
    Rds1 := Prds[i];
    fkm := FGm[i];
    k := AlpS(Nms1, Rds1);

    delta_shpal := 0;
    if Shp[i] = 0 then
      delta_shpal := 2;

    k := AlpS(Nms1, Rds1);
    vt := F_V[i];
    vtg := F_Vg[i];
    j := G_Ind1(PasGruzSkor, vt);

    s1 := DeltaSuj[j, 1, k];

    // ----------------------------------------------------------------------------
    if (4 < Fh) and (Fh < s1) then
    begin
      k3 := k3 + 1;

      OtklSuj := Fh; // round(MTmp[i].fun);

      if (k3 = 1) then
      begin
        L04 := fkm;
        Lm4 := fkm;
      end
      else
        Lm4 := fkm;

      if (max4 <= OtklSuj) then
      BEGIN
        max4 := OtklSuj;
        imax := i;
        belv := Floor(F_Sht[imax]);

      END;
      i := i + 1;
      continue;
      // sablog('belv=' + inttostr(belv) + ' --' + floattostr(f_sht[imax]));
    end
    else // if
      if k3 > 1 then
      begin
        fdConstrict := fdConstrict + (k3 div 4);
        if k3 mod 4 > 1 then
          fdConstrict := fdConstrict + 1;
        Setlength(WSuj, length(WSuj) + 1);
        WSuj[ns].st := 1;
        WSuj[ns].bel := Floor(F_Sht[imax]);
        WSuj[ns].val := max4;
        WSuj[ns].L0 := L04;
        WSuj[ns].Lm := Lm4; // + LenOtkl;
        WSuj[ns].count := k3 div 4;
        if k3 mod 4 > 0 then
          WSuj[ns].count := WSuj[ns].count + 1;
        WSuj[ns].Leng := k3;
        WSuj[ns].v := vt;
        WSuj[ns].vg := vtg;
        ns := ns + 1;
      end;
    max4 := 0;
    belv := 0;
    // ----------------------------------------------------------------------------
    k3 := 0;
    // ----------------------------------------------------------------------------
    i := i + 1;
  end; // while1
  // ------------------------------------------------------------------------------

end;

// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
// procedure GetFpro(nt:string; Fm,FmK,FmV:mas;var WPro:masots);  //
procedure GetFpro;
const
  La = 600; // sm
var
  i, j, fa, fb, Hn, i1, kk, k0: integer;
  stv, belv, L0v, Lmv: integer;
  flgPro: boolean;
  ogranichenie: string;
  ogranichenie1: integer;
  xxxx: integer;
Begin
  Try
    // SabLog('GetFpro - Определение Отс, степени, величина откл., нач. и конеч. коорд. отступления по просадке');

    ns := 0;
    kk := PasGruzSkor;
    Setlength(WPro, 3000);

    For i := Low(Fm) to High(Fm) - 1 do
    begin
      L0 := FmK[i];
      Lm := FmK[i + 1];
      Ln := round(abs(Lm - L0));
      fa := Fm[i];
      fb := Fm[i + 1];
      Hn := round(abs(fa - fb));
      Vk := FmV[i];
      flgPro := false;
      if ((fa > 0) and (0 > fb)) or ((fa < 0) and (0 < fb)) then
        flgPro := true;
      // ------------------------------------------------------------------------------
      s1 := 0;
      s2 := 0;
      s3 := 0;
      // ------------------------------------------------------------------------------
      j := G_Ind2(kk, Vk);
      s1 := DeltaPro[1, j];
      s2 := DeltaPro[2, j];
      s3 := DeltaPro[3, j];

      if (Ln <= La) and (s1 < Hn) and flgPro then
      begin
        belv := Hn;
        L0v := L0;
        Lmv := Lm;

        if (s1 < Hn) and (Hn <= s2) then
        begin
          stv := 2;
        end;
        if (s2 < Hn) and (Hn <= s3) then
        begin
          stv := 3;
        end;
        if (s3 < Hn) then
        begin
          stv := 4;
        end;

        WPro[ns].st := stv;
        WPro[ns].bel := belv;
        WPro[ns].L0 := L0v;
        WPro[ns].Lm := Lmv;

        /// opredelenie ogranichenia

        if stv = 4 then
        begin
          { ogranichenie1 := OpredelenieOgr_SkorostiPro(stv, belv, L0v,Lmv,j);
            ogranichenie1 := G_Ind2_skorost2(ogranichenie1);

            xxxx:= L0 div 100;
            pik:= (xxxx mod 1000) div 100 + 1;

            if not ProberkaNaStrelku(L0 div 100,7) then begin
            if shekteu(L0v div 100 , Lmv div 100, ogranichenie1) then
            WRT_UBEDOM(L0v, Lmv, 4 , 'Пр4', Inttostr(belv)+' '+ inttostr(pik), inttostr(ogranichenie1),inttostr(ogranichenie1));   //FormatFloat('0.00',belv)

            end; }

        end;
        stv := 0;

        ns := ns + 1;
      end;

    end; // for

    if ns = 0 then
      ns := 1
    else
      ns := ns;
    Setlength(WPro, ns);

  except
  end;
end;


// ------------------------------------------------------------------------------
// Новая процедура Просадки 11-09-2010
// ------------------------------------------------------------------------------

// ------------------------------------------------------------------------------
// Новая процедура Просадки 12-09-2010
// ------------------------------------------------------------------------------
function GetExtremum(Fm: mas; start, final: integer): Extremum;
var
  i: integer;
begin
  result.index := start+1;
  result.sign := 0;
  result.value := abs(Fm[start]);
  for i := start to final do
    if (result.value <= abs(Fm[i])) then
    begin
      result.value := abs(Fm[i]);
      result.index := i;
    end;

  result.value := Fm[result.index];
  if (result.value < 0) then
    result.sign := -1
  else if (result.value > 0) then
    result.sign := 1
  else
    result.sign := 0;
end;

procedure WriteTempOtsToRihOts(var temp: masots; D: integer; var ns: integer;
  isriht: boolean; var WRih: masots);

var
  StartStr, FinshStr,  delta, L0v, Lmv, belv, lnsm, stv, v1, v2, vt, vtg, vr, Vrg, xxxx,
 Ln2, kl,pg,Gap_index,  tempIndex: integer;
 FlagkrivPlus,FlagkrivMinus,FlagKriv, filter, filter2, is2to3OnJoint, jointless, is3onJoint, is4OnJoint: boolean;
 ots, comment: string;
C1,C2,dl1,dl2,s0, s1, s2, s3: real;

begin

  tempIndex := length(temp) - 1;
  while tempIndex >= 0 do
  begin
         pg := 0;
    vt := temp[tempIndex].v;
    vtg := temp[tempIndex].vg;
    vr := temp[tempIndex].Vrp;
    Vrg := temp[tempIndex].Vrg;
    belv := temp[tempIndex].bel;
    L0v := temp[tempIndex].L0;
    Lmv := temp[tempIndex].Lm;
    Ln := abs(L0v - Lmv);
       delta := abs(Lmv - L0v);
          if temp[tempIndex].isLong   then
          begin
             Ln := 20;
       delta := 20;
                  end;

    s0 := Velich_table_3dot3(Ln*2, vt, 0);
   s1 := Velich_table_3dot3(Ln*2, vt, 1);
    s2 := Velich_table_3dot3(Ln*2, vt, 2);
    s3 := Velich_table_3dot3(Ln*2, vt, 3);






    filter2 := false;
    jointless := CheckForJointlessPath(L0v, Lmv);
                FlagkrivPlus:=false;
                  FlagkrivMinus:=false;
                     FlagKriv:=false;
                     if abs(Trapezlevel[L0v]-Trapezlevel[Lmv])>0.1  then  FlagKriv:=true;

           if ( (Trapezlevel[L0v]>0.01 ) or (Trapezlevel[Lmv]>0.1 ) )
           then FlagkrivPlus:=true;
                if  ( (Trapezlevel[L0v]<-0.01 ) or (Trapezlevel[Lmv]<-0.01 ) )
                 then FlagkrivMinus:=true;

             StartStr:=  round((L0v+Lmv)/2-Ln) ;
             FinshStr   :=  round((L0v+Lmv)/2+Ln) ;

             StartStr:=  round((L0v+Lmv)/2-Ln/2) ;
             FinshStr   :=  round((L0v+Lmv)/2+Ln/2) ;
                  WRih[ns].onswitch := ProberkaNaStrelku(StartStr,  FinshStr  , 1);
                       isriht:= isriht;
        if   (( ProberkaNaStrelku(StartStr,  FinshStr  , 1)) and (FlagkrivPlus or FlagkrivMinus )   ) then
               isriht:=false;

            if   (  Gl_Switches_Side =0) then  isriht :=false;
                         Gl_Switches_Side:=10;
                      WRih[ns].isriht := isriht;

              if not(isriht) and (not(WRih[ns].prim.Contains('рн;'))) then
        WRih[ns].prim := WRih[ns].prim + 'рн;';



    if not FlagSiezd(L0v, Lmv) then
      filter2 := true;
    is2to3OnJoint := ((jointless) and (((2 * Ln <= 20) and (belv >= 24)) or
      ((2 * Ln >= 20) and (belv >= 33)))) and
      ((s1 < belv) and (belv <= s2) and (0 < s1) and (0 < s2) and
      (2 * delta <= 2 * D));

    if (s0 > -1) and (belv >= s0) and (belv <= s1) and isriht then
    begin
      fdStright := fdStright + 1;
      WRih[ns].Lm := 0;
      GetV_Remont(3, L0v, Lmv, vt, vtg, belv);
      WRih[ns].st := 1;
      WRih[ns].bel := belv;
      WRih[ns].L0 := L0v;
      WRih[ns].Lm := Lmv;
      WRih[ns].v := vt;
      WRih[ns].vg := vtg;
      WRih[ns].vop := -1;
      WRih[ns].vog := -1;
      WRih[ns].Vrp := vr;
      WRih[ns].Vrg := Vrg;
      WRih[ns].flg := GlbFlagRihtUbed;
      WRih[ns].prim := '';
      WRih[ns].isriht := isriht;
      WRih[ns].isLong := temp[tempIndex].isLong;
      ns := ns + 1;
    end;

    // --------------2 st ---------------------------------

        if is2to3OnJoint then
      begin
        stv := 2;
        WRih[ns].isEqualTo3 := true;
      end  ;
    if ((s1 < belv) and (belv <= s2) and (0 < s1) and (0 < s2) and (delta <= D))
       then
       //  and not(is2to3OnJoint) then
          //then
    begin
     // if isriht and ( ns>0)and   (  abs(WRih[ns].L0-Lmv ) >75) and  ( abs(WRih[ns].Lm-L0v ) >75  )
       if  ( isriht  )
       then
      begin



        WRih[ns].Lm := 0;
        stv := 2;
        WRih[ns].st := stv;
        WRih[ns].s3 := s2;
        WRih[ns].bel := belv;
        WRih[ns].L0 := L0v;
        WRih[ns].Lm := Lmv;
        WRih[ns].v := vt;
        WRih[ns].vg := vtg;
        WRih[ns].vop := -1;
        WRih[ns].vog := -1;
        WRih[ns].Vrp := vr;
        WRih[ns].Vrg := Vrg;
        WRih[ns].flg := GlbFlagRihtUbed;
        WRih[ns].prim := '';
        WRih[ns].isLong := temp[tempIndex].isLong;

        // близкие к 3 степени на изостыке
        Ln2:=round(Ln);


        //if not(WRih[ns].onswitch)  then
              if isriht  then
        begin
          rih_s2 := rih_s2 + 1;
          rih1_s2 := rih1_s2 + 1;
        end;

        ns := ns + 1;
      end

    end;

    if Lmv >830then
           belv := belv;

    is3onJoint := ((s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and
      (delta <= D)) and jointless;
    // ------------------3 st---------------------------------
    //if (((s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and (delta <= D)
     // ) and (not(is3onJoint)) or is2to3OnJoint) then
      if (((s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and (delta <= D)
      )  ) then


    begin
     // GetV_Remont(3, L0v, Lmv, vt, vtg, belv);
     // if is2to3OnJoint then
      //begin
       // stv := 2;
       // WRih[ns].isEqualTo3 := true;
      //end
      //else
        stv := 3;
      WRih[ns].st := stv;
      WRih[ns].bel := belv;
      WRih[ns].L0 := L0v;
      WRih[ns].Lm := Lmv; // Lmv;     ////////bx3   Lmv;
      WRih[ns].v := vt;
      WRih[ns].vg := vtg;
      WRih[ns].vop := -1;
      WRih[ns].vog := -1;
      WRih[ns].Vrp := vr;
      WRih[ns].Vrg := Vrg;
      WRih[ns].flg := GlbFlagRihtUbed;
      WRih[ns].isLong := temp[tempIndex].isLong;
      WRih[ns].isriht := isriht;

      //    if (((s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and (delta <= D)
      //) and (not(is3onJoint)) or is2to3OnJoint) then

   // begin
    //  GetV_Remont(3, L0v, Lmv, vt, vtg, belv);
    //  if is2to3OnJoint then
    //  begin
    //    stv := 2;
    //    WRih[ns].isEqualTo3 := true;
    ///  end
    //  else
     //   stv := 3;
     // WRih[ns].st := stv;
     // WRih[ns].bel := belv;
     // WRih[ns].L0 := L0v;
     // WRih[ns].Lm := Lmv; // Lmv;     ////////bx3   Lmv;
     // WRih[ns].v := vt;
     // WRih[ns].vg := vtg;
     // WRih[ns].vop := -1;
     // WRih[ns].vog := -1;
     // WRih[ns].Vrp := vr;
     // WRih[ns].Vrg := Vrg;
     // WRih[ns].flg := GlbFlagRihtUbed;
      //WRih[ns].isLong := temp[tempIndex].isLong;
      //WRih[ns].isriht := isriht;




      //if isriht and not(WRih[ns].onswitch) then
        if isriht  then
      begin
        rih_s3 := rih_s3 + 1;
        rih1_s3 := rih1_s3 + 1;
      end;
      if(stv=3) and  not(isriht) then
        WRih[ns].prim := WRih[ns].prim ;
      if (jointless) and (not(WRih[ns].prim.Contains('t+;'))) then
      begin
        // WRih[ns].prim := WRih[ns].prim + 't+;';
      end;
           if (belv/abs(L0v-Lmv)>5) then
      begin
        // WRih[ns].prim := WRih[ns].prim + 'СбРихт';
      end;



      ns := ns + 1;

    end;
    is4OnJoint := ((s3 < belv) and (0 < s3) and (delta <= D) and filter2) and
      jointless;
    // ------------------ 4 st ----------------------------------
    if ((s3 < belv) and (0 < s3) and (delta <= D) and filter2) or (is3onJoint)
    then
    begin
      if (is3onJoint) then
      begin
        stv := 3;
        WRih[ns].isEqualTo4 := true;
        lnsm := 20;
        v1 := V_ogr_rih_tab_3IsEqualTo4(0, vt, lnsm, belv);
        v2 := V_ogr_rih_tab_3IsEqualTo4(1, vtg, lnsm, belv);
      end
      else
      begin
        stv := 4;
        v1 := V_ogr_rih_tab_3dot3(0, vt, round(abs(L0v - Lmv) * 2), belv);
        v2 := V_ogr_rih_tab_3dot3(1, vtg, round(abs(L0v - Lmv) * 2), belv);
      end;

      if GlbFlagRemontKm and (RemTab9 > 0) then
      begin
        v1 := VTab9(3, belv, vt);
        v2 := VTab9(3, belv, vtg);
        if v1 = -1 then
          v1 := V_ogr_rih_tab_3dot3(0, vt, lnsm, belv);
        if v2 = -1 then
          v2 := V_ogr_rih_tab_3dot3(1, vtg, lnsm, belv);
      end;

      if ((15 < v1) and (v1 < vt)) or ((15 < v2) and (v2 < vtg)) then
      begin
        filter := false;
        filter2 := false;
        if GlbFlagRemontKm and (v1 >= vr) and (0 <= vr) and (RemTab9 = 0) then
          v1 := -1;
        if GlbFlagRemontKm and (v2 >= Vrg) and (0 <= Vrg) and (RemTab9 = 0) then
          v2 := -1;

        // Если рихтовка 3 степ и Т+ и бесстык пусть то не ограничиваем (раньше ограничивали 615)
        if ((stv = 3) and (jointless)) then
        begin
          v1 := -1;
          v2 := -1;
        end;
                 comment:='';
        xxxx := (L0v div 100) + 1;

            WRih[ns].onswitch := ProberkaNaStrelku(StartStr,  FinshStr  , 1);

           if (not( ProberkaNaStrelku(L0v, L0v + Ln, 1)) and  not(isriht) )then
            comment:='V=' + V_shekti(v1, v2)+'пк'+inttostr(xxxx)+' Рнр '+inttostr(belv) +'/'+ inttostr(2*Ln)+ '; ' ;


        if (isriht) then comment := 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx) + ' Р '+inttostr(belv) + '/' + inttostr(2*Ln) + '; ' ;


            if  (stv=4)and (comment<> '') then
       begin


        WRih[ns].st := stv;
        WRih[ns].bel := belv;
        WRih[ns].L0 := L0v;
        WRih[ns].Lm := Lmv;
        WRih[ns].v := vt;
        WRih[ns].vg := vtg;
        WRih[ns].vop := v1;
        WRih[ns].vog := v2;
        WRih[ns].Vrp := vr;
        WRih[ns].Vrg := Vrg;
        WRih[ns].flg := false;
        WRih[ns].isriht := isriht;
        WRih[ns].isLong := temp[tempIndex].isLong;
       WRih[ns].onswitch := ProberkaNaStrelku(L0v, L0v + Ln, 1);


                  WRT_UBEDOM(L0v, Lmv, 4, comment, v1, v2);
            end;

           comment:='';

        if not(WRih[ns].onswitch) then
        begin
          //if not((stv = 3) and (jointless)) then
              if (stv = 4)  then
            glbCount_Rih4s := glbCount_Rih4s + 1;

         // WRT_UBEDOM(L0v, Lmv, 4, comment, v1, v2);


        end;
        if (v1 = -1) and (v2 = -1) then
          WRih[ns].prim := GlbTempTipRemontKm;
        if not(isriht) then
          WRih[ns].prim := WRih[ns].prim + 'рн;';
        if (jointless) then
        begin
          // WRih[ns].prim := WRih[ns].prim + 't+;';
        end;
        ns := ns + 1;

      end;
    end; // 4 st
    tempIndex := tempIndex - 2;
  end;
end;

procedure WriteTempOtsToProsOts(var temp: masots; pg: integer; D: real;
  nt: string; var ns: integer; var WPro: masots);
var
  Ln: real;
  vtg, vr, Vrg, jj, belv, L0v, Lmv, tempIndex, v1, v2, xxxx, stv: integer;
  is2to3, iso_joint, otsf: boolean;
  ots: string;
begin
  tempIndex := length(temp) - 1;
  while tempIndex >= 0 do
  begin

    vt := temp[tempIndex].v;
    vtg := temp[tempIndex].vg;
    vr := temp[tempIndex].Vrp;
    Vrg := temp[tempIndex].Vrg;
    jj := G_Ind2(pg, vt);
    belv := temp[tempIndex].bel;
    L0v := temp[tempIndex].L0;
    Lmv := temp[tempIndex].Lm;
    Ln := abs(L0v - Lmv);
    s0 := DeltaPro[1, jj];
    s1 := DeltaPro[2, jj];
    s2 := DeltaPro[3, jj];
    s3 := DeltaPro[4, jj];
    is2to3 := false;
          iso_joint :=false;
    iso_joint := CheckForIsoJoint(L0v, Lmv);
    WPro[ns].prim := '';
            WPro[ns].prim :='';

               if (iso_joint )then
      begin
        WPro[ns].prim := WPro[ns].prim + 'ис';
      end;
      if ((iso_joint )and (belv >=18 ))then
      begin
             WPro[ns].prim := '';
        WPro[ns].prim := WPro[ns].prim + 'ис?; ';
      end;


    if (belv >= s0) and (belv <= s1) then
    begin
      fdDrawdown := fdDrawdown + 1;
      WPro[ns].st := 1;
      WPro[ns].bel := belv;
      WPro[ns].L0 := L0v;
      WPro[ns].Lm := Lmv;
      WPro[ns].v := vt;
      WPro[ns].vg := vtg;
      WPro[ns].vop := -1;
      WPro[ns].vog := -1;
      WPro[ns].Vrp := vr;
      WPro[ns].Vrg := Vrg;
      WPro[ns].flg := false;
      WPro[ns].isLong := temp[tempIndex].isLong;
      ns := ns + 1;
    end;




    if (((s1 <belv) and (belv <= s2) and not(iso_joint)) or
      (iso_joint and (s1 < belv) and (belv <= s2) and (s1 <= belv + 2) and
      (belv + 2 <= s2))) and (Ln <= D) then
    begin
      stv := 2;
      pro_s2 := pro_s2 + 1;
      if nt = 'п' then
      begin
        pro1_s2 := pro1_s2 + 1;
        GetV_Remont(4, L0v, Lmv, vt, vtg, belv);
      end;
      if nt = 'л' then
      begin
        pro2_s2 := pro2_s2 + 1;
        GetV_Remont(5, L0v, Lmv, vt, vtg, belv);
      end;
      otsf := true;
    end;
    if (((s2 < belv) and (belv <= s3)) or (iso_joint and (s1 < belv) and
      (belv <= s2) and (s2 < belv + 2) and (belv + 2 <= s3))) and (Ln <= D) then
    begin
      is2to3 := (iso_joint and (s1 < belv) and (belv <= s2) and (s2 < belv + 2)
        and (belv + 2 <= s3));
      stv := 3;
      if (not(iso_joint)) then
      begin
        pro_s3 := pro_s3 + 1;
        if nt = 'п' then
        begin
          pro1_s3 := pro1_s3 + 1;
          GetV_Remont(4, L0v, Lmv, vt, vtg, belv);
        end;
        if nt = 'л' then
        begin
          pro2_s3 := pro2_s3 + 1;
          GetV_Remont(5, L0v, Lmv, vt, vtg, belv);
        end;
      end;
      otsf := is2to3 or not(iso_joint);
            if (iso_joint )then


    end;
    if (s3 < belv) and (Ln <= D) then
      stv := 4;
    if otsf then
    begin
      WPro[ns].st := stv;
      WPro[ns].s3 := s2;
      WPro[ns].bel := belv;
      WPro[ns].L0 := L0v;
      WPro[ns].Lm := Lmv;
      WPro[ns].v := vt;
      WPro[ns].vg := vtg;
      WPro[ns].vop := -1;
      WPro[ns].vog := -1;
      WPro[ns].Vrp := vr;
      WPro[ns].Vrg := Vrg;
      WPro[ns].flg := false;
      WPro[ns].isLong := temp[tempIndex].isLong;
      ns := ns + 1;
      otsf := false;

    end;
    if ((stv = 4) or ((stv = 3) and iso_joint and not(is2to3))) then
    begin
      v1 := V_ogr_UPP(0, vt, belv, 1, iso_joint);
      v2 := V_ogr_UPP(1, vtg, belv, 1, iso_joint);


      if GlbFlagRemontKm and (RemTab9 > 0) then
      begin
        v1 := VTab9(4, belv, vt);
        v2 := VTab9(4, belv, vtg);
        if v1 = -1 then
          v1 := V_ogr_UPP(0, vt, belv, 1);
        if v2 = -1 then
          v2 := V_ogr_UPP(1, vtg, belv, 1);
      end;
      if ((v1 >= 0) and (v1 < vt)) or ((v2 >= 0) and (v2 < vtg)) then
      begin
        if (v1 >= vr) then
          v1 := -1;
        if (v2 >= Vrg) then
          v2 := -1;
        // ogranichenie1:= V_ogr_UPP(belv, 1);
        if nt = 'п' then
          glbCount_1Pro4s := glbCount_1Pro4s + 1;
        if nt = 'л' then
          glbCount_2Pro4s := glbCount_2Pro4s + 1;
        // xxxx:= (((L0v + Lmv) div 200) mod 1000) div 100 + 1;
        xxxx := L0v div 100 + 1;
        // 0909
        WPro[ns].st := stv;
        WPro[ns].bel := belv;
        WPro[ns].L0 := L0v;
        WPro[ns].Lm := Lmv;
        WPro[ns].v := vt;
        WPro[ns].vg := vtg;
        WPro[ns].vop := v1;
        WPro[ns].vog := v2;
        // ogranichenie1;
        WPro[ns].Vrp := vr;
        WPro[ns].Vrg := Vrg;
        WPro[ns].flg := false;
        WPro[ns].isEqualTo4 := stv = 3;
        WPro[ns].isLong := temp[tempIndex].isLong;
        if (v1 = -1) and (v2 = -1) then
          WPro[ns].prim := GlbTempTipRemontKm;
        ns := ns + 1;
         ots:= inttostr(belv)+'/'+inttostr(abs(L0v- Lmv)) ;
        if (V_shekti(v1, v2)<> '-/-')then

        WRT_UBEDOM(L0v, Lmv, stv, 'V=' + V_shekti(v1, v2) + ' пк' +
          inttostr(xxxx) + ' Пр.' + nt +ots+ ';  ', v1, v2);


      end;
    end;
    stv := 0;
    tempIndex := tempIndex - 2;
  end;

end;

procedure GetPro;

const
  D = 10.0; // metr
  hmin = 10; // min h prosadka
var
  i, j, L, m, i1, i2, xxxx, pg, k1, jj, jalauCount: integer;
  H, Ln, mx: real;

  stv, belv, L0v, Lmv, ogranichenie1, a, b, xa, xb, vtg, v1, v2, vr,
    Vrg: integer;
  z1, z2, imin, imax, tran_length, tempIndex: integer;
  minx, maxx: real;
  otsf, jalau: boolean;
  transitions: array of integer;
  e1, e2: Extremum;
  iso_joint, is2to3, isLong: boolean;
  temp: masots;
begin
  // sablog('Prosadka');
  try
    Setlength(WPro, 3000);
    Setlength(temp, 0);

    pg := 0;
    m := 6;
    p := 0;
    jalau := true;
    otsf := false;
    jalauCount := 0;
    i := 0;
    z2 := 1;
    tran_length := 0;
    Setlength(transitions, 1);
    transitions[0] := 0;
    while i < high(F_mtr) - 2 do
    begin
      z1 := z2;
      if (Fm[i] * Fm[i + 1] <= 0) then
      begin
        Setlength(transitions, tran_length + 1);
        transitions[tran_length] := i;
        if (Fm[i] = 0) then
          transitions[tran_length] := i
        else if (Fm[i + 1] = 0) then
        begin
          transitions[tran_length] := i + 1;
          i := i + 1;
        end;

        tran_length := tran_length + 1;

      end;
      i := i + 1;
    end;
    i := 1;
    // i:=z2;
    while i < High(transitions) do
    begin
      e1 := GetExtremum(Fm, transitions[i - 1], transitions[i]);
      e2 := GetExtremum(Fm, transitions[i], transitions[i + 1]);
      Ln := abs(FmK[e1.index] - FmK[e2.index]);
      H := abs(Fm[e1.index] - Fm[e2.index]);
      i := i + 1;
      isLong := false;
      if (H > 10) and (Ln <= 20) then
      begin
        if (Ln > 6) then
        begin
          isLong := true;
          if (abs(Fm[e1.index] - Fm[e1.index + 6]) <
            abs(Fm[e2.index] - Fm[e2.index - 6])) then
          begin
            H := abs(Fm[e2.index] - Fm[e2.index - 6]);
            e1.index := e2.index - 6;
          end
          else
          begin
            H := abs(Fm[e1.index] - Fm[e1.index + 6]);
            e2.index := e1.index + 6;
          end;

        end;

        vt := F_V[e1.index];
        vtg := F_Vg[e1.index];
        vr := F_Vrp[e1.index];
        Vrg := F_Vrg[e1.index];
        jj := G_Ind2(pg, vt);

        belv := round(H);
        if (H - belv < 0.5) and (H - belv > 0) then
          belv := belv ;
        L0v := FmK[e1.index];
        Lmv := FmK[e2.index];

        s0 := DeltaPro[1, jj];
        s1 := DeltaPro[2, jj];
        s2 := DeltaPro[3, jj];
        s3 := DeltaPro[4, jj];

        if (s0 >= belv) then
          continue;

        if ((length(temp) > 0) and ((temp[length(temp) - 1].bel > belv) or
          (i - temp[length(temp) - 1].transitionIndex > 1))) then
        begin
          WriteTempOtsToProsOts(temp, pg, D, nt, ns, WPro);
          if (temp[length(temp) - 1].bel > belv) and
            (i - temp[length(temp) - 1].transitionIndex = 1) then
          begin
            Setlength(temp, 0);
            continue;
          end;
          Setlength(temp, 0);
        end;

        tempIndex := length(temp);
        Setlength(temp, tempIndex + 1);
        temp[tempIndex].bel := belv;
        temp[tempIndex].L0 := L0v;
        temp[tempIndex].Lm := Lmv;
        temp[tempIndex].v := vt;
        temp[tempIndex].vg := vtg;
        temp[tempIndex].Vrp := vr;
        temp[tempIndex].Vrg := Vrg;
        temp[tempIndex].transitionIndex := i;
        temp[tempIndex].isLong := isLong;
        temp[tempIndex].iso_joint := iso_joint;

      end

    end;
    WriteTempOtsToProsOts(temp, pg, D, nt, ns, WPro);
    Setlength(WPro, ns);
  except
  end;

end;
/// ////////  запись  перекосов////////

procedure WriteTempOtsToPerOts(var temp: masots; pg: integer; D: real;
  nt: string; var ns: integer; var WPer: masots);
var
 Dl_per, xa_,xb_,Ln: real;
 delta, vt1,vt2,vtg, vr, Vrg, jj, belv, L0v, Lmv, tempIndex, v1, v2, xxxx, stv: integer;
  is2to3, iso_joint, otsf: boolean;
  ots,  comment,comment_gr: string;
begin
  otsf:=false;
  tempIndex := length(temp) - 1;
  while tempIndex >= 0 do
  begin
    vt := temp[tempIndex].v;
    vtg := temp[tempIndex].vg;
    vr := temp[tempIndex].Vrp;
    Vrg := temp[tempIndex].Vrg;
//
          jj := G_Ind2(pg, vt);



    jj := G_Ind2(pg, vr);
    belv := temp[tempIndex].bel;
    L0v := temp[tempIndex].L0;
    Lmv := temp[tempIndex].Lm;
    Ln := abs(L0v - Lmv);
    s0 := DeltaPer[1, jj];
    s1 := DeltaPer[2, jj];
    s2 := DeltaPer[3, jj];
    s3 := DeltaPer[4, jj];
    is2to3 := false;
   ////
         Dl_per:=10;
     xa_:= L0v - NAPR_DBIJ * Dl_per;
        xb_ := Lmv + NAPR_DBIJ * Dl_per;

    iso_joint := CheckForIsoJoint(L0v, Lmv);
    WPer[ns].prim := '';  //WPer
      //if iso_joint then
    ///  begin
    //    WPer[ns].prim := WPer[ns].prim + 'ис;';
    //  end;

       // if (Fm[i] * Fm[i + 1] <= 0) then
     // begin
            // end;
             delta :=round( abs(Lmv - L0v));
                ots:=  inttostr(belv)+'/'+inttostr(delta );
        if (s0 <= belv) and (s1 >= belv) and (Ln < 20) then
        begin
          fdSkew := fdSkew + 1;
          WPer[ns].st := 1;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagPerUbed;
          WPer[ns].prim := '';
          WPer[ns].isLong := abs(L0v - Lmv) > 20;
          ns := ns + 1;
        end;
    // --------------2 st ---------------------------------
        if (s1 < belv) and (belv <= s2) and (0 < s1) and (0 < s2) and
          (delta < 21) then
        begin
          stv := 2;
          per_s2 := per_s2 + 1;
          GetV_Remont(2, L0v, Lmv, vt, vtg, belv);
          WPer[ns].s3 := s2;
          WPer[ns].st := stv;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagPerUbed;
          WPer[ns].prim := '';
          WPer[ns].isLong := abs(L0v - Lmv) > 20;
          ns := ns + 1;

        for j := i to high(F_urb_Per1 ) do
          begin
            if ((xa_ <= f_mtr[j]) and (f_mtr[j] <= xb_)) or
              ((xa_ >= f_mtr[j]) and (f_mtr[j] >= xb_)) then
            begin
              F_urb_Per1[j] := 0;
              Furb1[j] := 0;
              Urob[j] := 1;
            end;
          end;
        end;
     // ------------------3 st---------------------------------
        if (s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and
          (delta <=20) then
        begin

               if (delta<=10)and (belv>18)  then
           begin
           comment_gr:= '';
           v1:=500;
           v2:=60;
                     xxxx := L0v div 100 + 1;

          WRT_UBEDOM(L0v, Lmv, stv, 'V=' + V_shekti(v1, v2)+ 'пк' +
          inttostr(xxxx) + 'П'+ots  +'гр' +'; ', v1, v2);
                    comment_gr:='гр';
             // WPer[ns].prim comment_gr
             //     WRT_UBEDOM(L0v, Lmv, stv, 'V=' + V_shekti(v1, v2) + ' пк' +
          //inttostr(xxxx) + ' П' + nt + '; ', v1, v2);

             ////
             end;

          stv := 3;
          per_s3 := per_s3 + 1;
          GetV_Remont(2, L0v, Lmv, vt, vtg, belv);
          WPer[ns].st := stv;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagPerUbed;
          WPer[ns].prim :=comment_gr;
          WPer[ns].isLong := abs(L0v - Lmv) =20;


          ns := ns + 1;

         for j := i to high(F_urb_Per1 ) do
          begin
            if ((xa_ <= f_mtr[j]) and (f_mtr[j] <= xb_)) or
              ((xa_ >= f_mtr[j]) and (f_mtr[j] >= xb_)) then
            begin
              F_urb_Per1[j] := 0;
              Furb1[j] := 0;
              Urob[j] := 1;
            end;
          end;
        end;
    if (s3 < belv) and (Ln <= D) then
      stv := 4;
    if otsf then
    begin




    ////
      WPer[ns].st := stv;
      WPer[ns].s3 := s2;
      WPer[ns].bel := belv;
      WPer[ns].L0 := L0v;
      WPer[ns].Lm := Lmv;
      WPer[ns].v := vt;
      WPer[ns].vg := vtg;
      WPer[ns].vop := -1;
      WPer[ns].vog := -1;
      WPer[ns].Vrp := vr;
      WPer[ns].Vrg := Vrg;
      WPer[ns].flg := false;
      WPer[ns].isLong := temp[tempIndex].isLong;
      ns := ns + 1;
      otsf := false;

    end;

          for j := i to high(F_urb_Per1 ) do
          begin
            if ((xa_ <= f_mtr[j]) and (f_mtr[j] <= xb_)) or
              ((xa_ >= f_mtr[j]) and (f_mtr[j] >= xb_)) then
            begin
              F_urb_Per1[j] := 0;
              Furb1[j] := 0;
              Urob[j] := 1;
            end;
          end;

    if (stv = 4) then
    begin

       v1 := V_ogr_UPP(0, vt, belv, 2);
          v2 := V_ogr_UPP(1, vtg, belv, 2);
               for j := i to high(F_urb_Per1 ) do
          begin
            if ((xa_ <= F_urb_Per1[j]) and (F_urb_Per1[j] <= xb_)) or
              ((xa_ >= F_urb_Per1[j]) and (F_urb_Per1[j] >= xb_)) then
            begin
              F_urb_Per1[j] := 0;
              Furb1[j] := 0;
              Urob[j] := 1;
            end;
          end;

         if GlbFlagRemontKm and (RemTab9 > 0) then
              begin
                v1:= VTab9(2, belv, vt);
               v2:= VTab9(2, belv, vtg);

              if v1 = -1 then v1:= V_ogr_UPP(0, vt, belv, 2);
                if v2 = -1 then v2:= V_ogr_UPP(1, vtg, belv, 2);
              end;



      if ((v1 >= 0) and (v1 < vt)) or ((v2 >= 0) and (v2 < vtg)) then
      begin
        if (v1 >= vr) then
          v1 := -1;
        if (v2 >= Vrg) then
          v2 := -1;
        // ogranichenie1:= V_ogr_UPP(belv, 1);
       // if nt = 'п' then
           // glbCount_Per4s  := glbCount_Per4s + 1;
        //if nt = 'л' then
         glbCount_Per4s := glbCount_Per4s + 1;
        // xxx   glbCount_Per4s := 0;x:= (((L0v + Lmv) div 200) mod 1000) div 100 + 1;
        xxxx := L0v div 100 + 1;
        // 0909
        WPer[ns].st := stv;
        WPer[ns].bel := belv;
        WPer[ns].L0 := L0v;
        WPer[ns].Lm := Lmv;
        WPer[ns].v := vt;
        WPer[ns].vg := vtg;
        WPer[ns].vop := v1;
        WPer[ns].vog := v2;
        // ogranichenie1;
        WPer[ns].Vrp := vr;
        WPer[ns].Vrg := Vrg;
        WPer[ns].flg := false;
        WPer[ns].isEqualTo4 := stv = 3;
        WPer[ns].isLong := temp[tempIndex].isLong;
        if (v1 = -1) and (v2 = -1) then
          WPer[ns].prim := GlbTempTipRemontKm;
        ns := ns + 1;
        if V_shekti(v1, v2) <> '-/-'then

        WRT_UBEDOM(L0v, Lmv, stv, 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx) + ' П'  +ots+ ';  ', v1, v2);
      end;
    end;
    stv := 0;
    tempIndex := tempIndex - 2;
  end;

end;

   ///////=======
procedure GetPro_Perekos;

const
  D = 30.0; // metr
  hmin = 10; // min h prosadka

var
  i, j, L, m, i1, i2, xxxx, pg, k1, jj, jalauCount: integer;
 df1,df2, dTrap1, dTrap2,popr,H,H2, Ln,Ln2, mx: real;

     stv, belv, L0v, Lmv, ogranichenie1, a, b, xa, xb, vtg, v1, v2, vr,
    Vrg: integer;
 ip1,ip, z1, z2, imin, imax, tran_length, tempIndex: integer;
 H3,H4,Hh0,h0, minx, maxx: real;
  otsf, jalau: boolean;
 H_trans,Index_Trans, H_extr,extr,transitions: array of integer;
  e1, e2,e3,e4: Extremum;
   flagPer, flag0,iso_joint, is2to3, isLong: boolean;
  temp: masots;
begin
  // sablog('Perekos');
  try
    Setlength(WPer, 3000);
    Setlength(temp, 0);
        Setlength(H_trans, 3000);
        Setlength(Index_Trans, 3000);
    pg := 0;
    m := 6;
    p := 0;
         flagPer:=false;
    jalau := true;
    otsf := false;
    jalauCount := 0;
    i := 0;
    z2 := 1;
    tran_length := 0;
    Setlength(transitions, 1);
    Setlength(extr, 3000);
    transitions[0] := 0;
    flag0:=true;
    Fm[0] :=0;
        Fm[1] :=0;
    Fm[high(F_mtr)] :=0;
         Fm[high(F_mtr)-1] :=0;

    //i:=1;

      while i < high(F_mtr)-1  do
  begin
    if ( (Fm[i]* Fm[i+1] )<=0 ) then
    begin

      Setlength(transitions, tran_length + 1);
      transitions[tran_length] := i;
      if (abs(Fm[i] ) <1) then
        transitions[tran_length] := i
      else if (abs(Fm[i + 1])<1) then
      begin
        transitions[tran_length] := i + 1;
        i := i + 1;
      end;
      if (tran_length >0) then
      begin

             e1 := GetExtremum(Fm, transitions[tran_length - 2],
            transitions[tran_length - 1]);
          e2 := GetExtremum(Fm, transitions[tran_length - 1],
            transitions[tran_length]);
        if( abs(Fm[transitions[tran_length]] - Fm[transitions[tran_length - 1]])  < 5  )
        and ( abs(e1.value)<3) and ( abs(e2.value)<3)then
        begin
          e1 := GetExtremum(Fm, transitions[tran_length - 2],
            transitions[tran_length - 1]);
          e2 := GetExtremum(Fm, transitions[tran_length - 1],
            transitions[tran_length]);
         // if (abs(e1.value) < 10) or  (abs(e2.value) < 10   then
             if ( abs(FmK[e1.index] - FmK[e2.index])<4 )
             or (abs(transitions[tran_length - 2] -transitions[tran_length - 1] ) <3  )
              then
            tran_length := tran_length - 2;
        end;

      end;
      tran_length := tran_length + 1;
    end;
    i := i + 1;
  end;
      // Setlength(transitions, tran_length+1 );
    //   transitions[tran_length -1]  :=high(F_mtr);

    i := 0;
    // i:=z2;


          //i:=High(transitions)-3;

    while i < High(transitions)-2 do
    begin
                i := i + 1;

        // h0:= abs(TrapezLevel_Get_per[e1.index]- TrapezLevel_Get_per[e2.index]  );
      e1 := GetExtremum(Fm, transitions[i - 1], transitions[i]);
      e2 := GetExtremum(Fm, transitions[i], transitions[i + 1]);
      e3 := GetExtremum(Fm, transitions[i ], transitions[i+1]);
      e4:= GetExtremum(Fm, transitions[i+1], transitions[i + 2]) ;

                dTrap1:= abs(Fm[e1.index] +FmAvgTr[e1.index])- abs(TrapezLevel[e1.index]);
               dTrap2:= abs( Fm[e2.index] +FmAvgTr[e2.index])- abs(TrapezLevel[e2.index]);
              // H:=0;

                 df1:= Fm[e1.index] +FmAvgTr[e1.index]-TrapezLevel[e1.index ];
                    df2:= Fm[e2.index] +FmAvgTr[e2.index]-TrapezLevel[e2.index] ;
        // Index_Trans[e1.index] :=round(e1.index);
        //if ( dTrap1* dTrap2<0) then
               //    begin
               H:= (abs(Fm[e1.index] - Fm[e2.index]+(FmAvgTr[e1.index] - FmAvgTr[e2.index]))) ;
                Ln:= abs(FmK[e1.index] - FmK[e2.index]);
               //   end;
                if Ln<20 then  H_Trans[e1.index] :=round(abs(e1.value )+abs(e2.value ));

               Ln2:= abs(FmK[e3.index] - FmK[e4.index]);
                 H2 := (abs(Fm[e3.index] - Fm[e4.index]+(FmAvgTr[e3.index] - FmAvgTr[e4.index])));

          if  i=High(transitions)-2 then
          begin
                  e1 := GetExtremum(Fm, transitions[i - 1], transitions[i]);
           e2 := GetExtremum(Fm, transitions[i], high(F_mtr)-1);
                H:= (abs(Fm[e1.index] - Fm[e2.index]+(FmAvgTr[e1.index] - FmAvgTr[e2.index]))) ;
                Ln:= abs(FmK[e1.index] - FmK[e2.index]);
                H2:=H;
          end;



            popr:=exp(-2*abs(h-10))  ;
            if H <11 then     H:=H+1;
               if H2 <11 then     H2:=H2+1;
                h0:= abs(TrapezLevel[e1.index]- TrapezLevel[e2.index]  );
            //if (h>13)and (h0<=1)  then  H:=H/1.15;
              if (h2>15)and (h0<=1)   then  H2:=H2/1.15;


//      if H2>H then
//      begin
//         H:=H2;
//         e1:=e3;
//            e2:=e4;
//               Ln:= abs(FmK[e1.index] - FmK[e2.index]);
//               //i:=i+1;
//      end;



     // if  Ln<3 then    H:=H/10.0;


      //i := i + 1;
      isLong := false;
      if (H > 7) and (Ln <= 30) and( Ln>2)then
      begin
        if (Ln > 20) then
        begin
          isLong := true;
          if (abs(Fm[e1.index] - Fm[e1.index + 20]) <
            abs(Fm[e2.index] - Fm[e2.index - 20])) then
          begin
            H := abs(Fm[e2.index] - Fm[e2.index - 20]);
            e1.index := e2.index - 20;
          end
          else
          begin
            H := abs(Fm[e1.index] - Fm[e1.index + 20]);
            e2.index := e1.index + 20;
          end;

        end;
           Hh0:=0;
           h0:=0;
          h0:= abs(TrapezLevel[e1.index]- TrapezLevel[e2.index]  );
                  dTrap1:= (Fm[e1.index] +FmAvgTr[e1.index])- (TrapezLevel[e1.index]);
               dTrap2:= ( Fm[e2.index] +FmAvgTr[e2.index])- (TrapezLevel[e2.index]);
               // if (( h0>1 )  and (Fm[e1.index]*Fm[e2.index]<0) )  then
                  // begin
                //  if  (dTrap1*dTrap1<0) then
                     if  (Fm[e1.index]*Fm[e2.index]<=0) then
                       begin
                        Hh0:=  abs((Fm[e1.index] +FmAvgTr[e1.index])- (TrapezLevel[e1.index]) );
                       Hh0:=Hh0+  abs( (Fm[e2.index] +FmAvgTr[e2.index])- (TrapezLevel[e2.index]) );
                          H2:= abs (Fm[e1.index] )+  abs (Fm[e2.index]);
                         end;

             H3:= abs((Fm[e1.index] +FmAvgTr[e1.index])-( Fm[e2.index] +FmAvgTr[e2.index]));
              // H:=0;
                 df2:=0;
                 ip:=-10;
                     df1:=0;
              while ip < 20 do
              begin
                 df1:= abs(TrapezLevel[e1.index+ip ]-TrapezLevel[e1.index +ip+2 ]);
                 if (df1>df2) then
                 begin
                  df2:=df1;
                  ip1:=abs(ip);
                    end;
                    ip:=ip+1;
                 end;

                    H4:=0;
                    H:=0;
               if ( h0>0.01) then H:=0;
          if ( h0>0.01   )    then
           H4:=min(Hh0/1.15,H2/1.15);

           //if ( (H>20) and (df2>2) )    then    H:=0;   //  and (dTrap1*dTrap2<0)    H:=min(Hh0/1.15,H2);

           //  and (dTrap1*dTrap2<0)    H:=min(Hh0/1.15,H2);
          if Ln<3 then  H:=0;
            if ( h0< 0.01   )  and (H3>11 )  then    H:=H3;
               if ( h0< 0.01   )  and (H3<=11 )  then    H:=H3+exp(-2*abs(h-10));
         if ( h0>0.01   ) then
           H:=min(H4,H3);
        vt := F_V[e1.index];
        vtg := F_Vg[e1.index];
       vr := F_Vrp[e1.index];
       Vrg := F_Vrg[e1.index];

       // vt:=  F_V[ L0v];

         // vtg := F_Vg[L0v];
       //// vr := F_Vrp[L0v];
        //Vrg := F_Vrg[L0v];
        jj := G_Ind2(pg, vr);

        belv := Ceil(H);
        if (H - belv < 0.7 ) and (H - belv > 0) then
          belv := belv + 1;
        L0v := FmK[e1.index];
        Lmv := FmK[e2.index];

        s0 := DeltaPer[1, jj];
        s1 := DeltaPer[2, jj];
        s2 := DeltaPer[3, jj];
        s3 := DeltaPer[4, jj];

        if (s0 >= belv) then
          continue;

        if ((length(temp) > 0) and ((temp[length(temp) - 1].bel > belv) or
          (i - temp[length(temp) - 1].transitionIndex > 1))) then
        begin
          WriteTempOtsToPerOts(temp, pg, D, nt, ns, WPer);
          if (temp[length(temp) - 1].bel > belv) and
            (i - temp[length(temp) - 1].transitionIndex = 1) then
          begin
            Setlength(temp, 0);
            continue;
          end;
          Setlength(temp, 0);
        end;

        tempIndex := length(temp);
        Setlength(temp, tempIndex + 1);
        temp[tempIndex].bel := belv;
        temp[tempIndex].L0 := L0v;
        temp[tempIndex].Lm := Lmv;
        temp[tempIndex].v := vt;
        temp[tempIndex].vg := vtg;
        temp[tempIndex].Vrp := vr;
        temp[tempIndex].Vrg := Vrg;
        temp[tempIndex].transitionIndex := i;
        temp[tempIndex].isLong := isLong;
        temp[tempIndex].iso_joint := iso_joint;

      end

    end;
    WriteTempOtsToPerOts(temp, pg, D, nt, ns, WPer);
    Setlength(WPer, ns);
  except
  end;

end;

// = = = = = = = = = = = = = = = = = = = = = = = = ============================================================================================================
// ------------------------------------------------------------------------------
// процедура Перекоса 12-09-2010
// ------------------------------------------------------------------------------
{procedure GetPerekos;
  const
  d = 20.0; //metr
  Dl_per = 10; // 5 m
  var
  i,j,l,m,i1,i2, xxxx, pg,k1, s11,s21,s31:integer;
  h, Ln, mx:real;
  Ikf, stv, belv, L0v, Lmv, ogranichenie1, a,b,xa,xb,xa_,xb_,vtg, v1, v2, vr, vrg:integer;
  otsf, flagv, filter, Filter_Siezd:boolean;
  rem_inf:string;
  begin
  try
  otsf:= false;
  Setlength(WPro,3000);
  pg:= 0;
  Ikf:= NAPR_DBIJ;
  m:= 20;  p:= 0;

  while p <= high(Fm)/20 do
  begin
  mx:= 0; i1:= 0;  i2:= 0;  i:= 0;

  while i <= high(Fm) - m do
  begin
  for j:= i + 1 to i + m do
  begin
  //h:= abs(Fm[i] - Fm[j]);
  h:= abs(F_urb_Per1[i] - F_urb_Per1[j]);
  if mx < h then begin
  mx:= h;
  Ln:= abs(Fmk[i] - Fmk[j])/100;
  i1:= i;
  i2:= j;
  end;
  end;
  i:= i + 1;
  end;

  vt:= F_V[i1];
  vtg:= F_Vg[i1];
  vr:=  F_Vrp[i1];   vrg:= F_Vrg[i1];

  j:= G_Ind2(pg, vt);
  s1:= DeltaPro_UR_per[1,j];
  s2:= DeltaPro_UR_per[2,j];
  s3:= DeltaPro_UR_per[3,j];

  if (2 < Ln) and (Ln <= d) and ((mx > s1) or (mx > s11))  then
  begin
  // for l:= i1 to i2 do F_urb_Per1[l]:= 0; //Fm[l]:= 0;
  a:= Fmk[i1];     b:= Fmk[i2];
  L0v:= min(a,b);  Lmv:= max(a,b);
  belv:= round(mx);
  filter:= false;

  if NAPR_DBIJ < 0 then
  begin
  L0v:= max(a,b);  Lmv:= min(a,b);
  end;

  if L0v <= 0 then L0v := 1;
  if Lmv <= 0 then Lmv := 1;

  xa:= L0v; // - NAPR_DBIJ*Dl_per;
  xb:= Lmv; // + NAPR_DBIJ*Dl_per;
  xa_:= L0v - NAPR_DBIJ*Dl_per;
  xb_:= Lmv + NAPR_DBIJ*Dl_per;

  if (L0v div 100) mod 1000 > 995 then xa:= L0v;
  if (Lmv div 100) mod 1000 > 995 then xb:= Lmv;

  Filter_Siezd:= not FlagSiezd(L0v div 100, Lmv div 100);
  //------------------------------------------------------------------------------
  if (s1 < belv) and (belv <= s2)
  then begin   //and not GlbFlagRemontKm
  stv:= 2;
  GetV_Remont(2, xa, xb, vt, vtg, belv);

  per_s2:= per_s2 + 1;

  WPro[ns].st:= stv;
  WPro[ns].bel:= belv;
  WPro[ns].L0:= xa;
  WPro[ns].Lm:= xb;
  WPro[ns].v := vt;
  WPro[ns].vg := vtg;
  WPro[ns].vop:= -1;
  WPro[ns].vog:= -1;
  WPro[ns].vrp:= vr;
  WPro[ns].vrg:= vrg;
  WPro[ns].flg:= false;
  ns:= ns + 1;

  for j:= 0 to high(FmK) do begin
  if ((xa_ <= FmK[j]) and (FmK[j] <= xb_))
  or ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
  begin
  F_urb_Per1[j]:= 0;
  Furb1[j]:= 0;//Furb_sr[j]; //18.06.2013
  end;
  end;
  end;

  if (s2 < belv) and (belv <= s3)
  then begin     //and not GlbFlagRemontKm
  stv:= 3;
  GetV_Remont(2, xa, xb, vt, vtg, belv);

  per_s3:= per_s3 + 1;
  WPro[ns].st:= stv;
  WPro[ns].bel:= belv;
  WPro[ns].L0:= xa;
  WPro[ns].Lm:= xb;
  WPro[ns].v := vt;
  WPro[ns].vg := vtg;
  WPro[ns].vop:= -1;
  WPro[ns].vog:= -1;
  WPro[ns].vrp:= vr;
  WPro[ns].vrg:= vrg;
  WPro[ns].flg:= false;
  WPro[ns].prim:= '';
  ns:= ns + 1;

  for j:= 0 to high(FmK) do begin
  if ((xa_ <= FmK[j]) and (FmK[j] <= xb_))
  or ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
  begin
  F_urb_Per1[j]:= 0;
  Furb1[j]:= 0;//Furb_sr[j]; //18.06.2013
  end;
  end;
  end;

  if (s3 < belv) then
  begin
  stv:= 4;
  otsf:= true;
  end;

  if otsf and (stv = 4) and Filter_Siezd then
  begin
  otsf:= false;
  v1:= V_ogr_UPP(0, vt, belv, 2);
  v2:= V_ogr_UPP(1, vtg, belv, 2);

  if GlbFlagRemontKm and (RemTab9 > 0) then
  begin
  v1:= VTab9(2, belv, vt);
  v2:= VTab9(2, belv, vtg);

  if v1 = -1 then v1:= V_ogr_UPP(0, vt, belv, 2);
  if v2 = -1 then v2:= V_ogr_UPP(1, vtg, belv, 2);
  end;


  if ((v1 >= 0) and (v1 < vt))
  or ((v2 >= 0) and (v2 < vtg)) then
  begin

  if (v1 >= vr) then v1:= -1;
  if (v2 >= vrg) then v2:= -1;

  filter:= false;
  glbCount_Per4s:= glbCount_Per4s + 1;

  //xxxx:= (((L0v + Lmv) div 200) mod 1000) div 100 + 1;
  xxxx:= ((L0v div 100) mod 1000) div 100 + 1;
  //0909
  WPro[ns].st:= 4;
  WPro[ns].bel:= belv;
  WPro[ns].L0:= xa;
  WPro[ns].Lm:= xb;
  WPro[ns].v := vt;
  WPro[ns].vg := vtg;
  WPro[ns].vop:= v1; //ogranichenie1;
  WPro[ns].vog:= v2; //ogranichenie1;
  WPro[ns].vrp:= vr;
  WPro[ns].vrg:= vrg;
  WPro[ns].flg:= false;

  if GlbFlagRemontKm  then WPro[ns].prim:= GlbTempTipRemontKm;

  ns:= ns + 1;
  //0909
  WRT_UBEDOM(L0v, Lmv, 4 ,
  'П п'+ inttostr(xxxx)+ ' v=' + v_shekti(v1, v2) + ';' , v1, v2); //FormatFloat('0.00',belv)


  for j:= 0 to high(FmK) do begin
  if ((xa_ <= FmK[j]) and (FmK[j] <= xb_))
  or ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
  begin
  F_urb_Per1[j]:= 0;
  Furb1[j]:= 0;//Furb_sr[j]; //18.06.2013
  end;
  end;

  end;

  // VTab9(typeots, value, speed: integer)

  stv:= 0;
  end;
  i:= i2 + 1;
  end;
  stv:= 0;

  p:= p + 1;
  end;

  Setlength(WPro,ns);
  except
  end;
  end;
  }
// ------------------------------------------------------------------------------
// Жана процедура Рихтовка 13-08-2015
// ------------------------------------------------------------------------------
function CoordinateToReal(km, meter: integer): real;
begin
  result := km + meter / 10000;
end;

procedure GetPerekos(Fm, Fm_sr, FmK: mas; var WPer: masots);
const
  D = 20.0; // metr
  Dl_per = 13; // 5 m
var
  i, j, ik, m, i1, i2, xxxx, pg, Ikf, jalauCount: integer;
  H, h0, Ln, mx, hmin, s1, s2, s3: real;
  stv, belv, L0v, Lmv, ogranichenie1, a, b, LnInt, vtg, v1, v2, vr, Vrg, delta,
    xa_, xb_: integer;
  filter, otsf, filter2, jalau, Filter_Siezd: boolean;
  comment, rem_inf: string;

begin
  try
    otsf := false;
    Ikf := NAPR_DBIJ;
    m := 20;
    p := 0;
    Setlength(WPer, 3000);
    pg := 0;
    jalauCount := 0;
    mx := 8;
    jalau := true;
    h0 := 0;
    while (jalau and (jalauCount <= high(Fm))) do
    begin
      jalauCount := jalauCount + 1;
      i1 := -1;
      i2 := -1;
      Ln := 0;

      for i := 1 to high(Fm) - m - 1 do

      begin

        for j := i to i + m do
          if (Fm[i] * Fm[j] <= 0)

          then
          begin
            h0 := abs(Fm_sr[i] - Fm_sr[j]);
            H := (abs(Fm[i]) + abs(Fm[j]));
            if (h0 > 3) then
              H := min(abs(Fm[i]), abs(Fm[j]));

            // for ik := 0 to high(UKrv) do
            // begin
            // if ((CoordinateToReal(Ukrv[ik].nkm,Ukrv[ik].nmtr) <= CoordinateToReal(GlbKmTrue,F_Mtr[i])) and
            // (CoordinateToReal(GlbKmTrue,F_Mtr[i])<= CoordinateToReal(Ukrv[ik].kkm,Ukrv[ik].kmtr)))   then
            // begin
            // h0 :=abs(Fm_sr[i] - Fm_sr[j]);
            // H:=    abs(Fm[i]) + abs(Fm[j]);
            // if  ( h0> 2) then             H :=  min(abs(Fm[i]),abs(Fm[j]));

            // end;

            // end;
            if ((mx <= H) and (Fm[i] * Fm[j] <= 0)) then
            // and  ( (  abs(Fm[i])<5 ) or (i2 <0 ) )
            begin
              // writeln('h0=' + floattostr(h0) + ' H=' + floattostr(H));
              mx := H;
              Ln := abs(FmK[i] - FmK[j]);
              i1 := i;
              i2 := j;
            end;
          end;
      end;

      if mx <= 8 then
        break;

      if (i1 >= 0) and (i2 >= 0) and (0 < Ln) and (Ln <= 20) then
      begin

        vt := F_V[i1];
        vtg := F_Vg[i1];
        vr := F_Vrp[i1];
        Vrg := F_Vrg[i1];
        j := G_Ind2(pg, vt);
        s0 := DeltaPer[1, j];
        s1 := DeltaPer[2, j];
        s2 := DeltaPer[3, j];
        s3 := DeltaPer[4, j];

        belv := round(mx);
        a := FmK[i1];
        b := FmK[i2];
        L0v := min(a, b);
        Lmv := max(a, b);
        if NAPR_DBIJ < 0 then
        begin
          L0v := max(a, b);
          Lmv := min(a, b);
        end;
        if L0v <= 0 then
          L0v := 1;
        if Lmv <= 0 then
          Lmv := 1;

        xa_ := L0v - NAPR_DBIJ * Dl_per;
        xb_ := Lmv + NAPR_DBIJ * Dl_per;

        Filter_Siezd := not FlagSiezd(L0v, Lmv);
        delta := abs(Lmv - L0v);
        if (s0 <= belv) and (s1 >= belv) then
        begin
          fdSkew := fdSkew + 1;
          WPer[ns].st := 1;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagRihtUbed;
          WPer[ns].prim := '';
          WPer[ns].isLong := abs(L0v - Lmv) > 20;
          ns := ns + 1;
        end;
        // --------------2 st ---------------------------------
        if (s1 < belv) and (belv <= s2) and (0 < s1) and (0 < s2) and
          (delta <= 20) and (delta > 1) then
        begin
          stv := 2;
          per_s2 := per_s2 + 1;
          GetV_Remont(2, L0v, Lmv, vt, vtg, belv);
          WPer[ns].s3 := s2;
          WPer[ns].st := stv;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagRihtUbed;
          WPer[ns].prim := '';
          WPer[ns].isLong := abs(L0v - Lmv) > 20;
          ns := ns + 1;

          for j := i to high(FmK) do
          begin
            if ((xa_ <= FmK[j]) and (FmK[j] <= xb_)) or
              ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
            begin
              F_urb_Per1[j] := 0;
              Furb1[j] := 0;
              Urob[j] := 1;
            end;
          end;

          mx := 8;
        end;
        // ------------------3 st---------------------------------
        if (s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and
          (delta <= 20) then
        begin
          stv := 3;
          per_s3 := per_s3 + 1;
          GetV_Remont(2, L0v, Lmv, vt, vtg, belv);
          WPer[ns].st := stv;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagRihtUbed;
          WPer[ns].prim := '';
          WPer[ns].isLong := abs(L0v - Lmv) > 20;
          ns := ns + 1;

          for j := 0 to high(FmK) do
            if ((xa_ <= FmK[j]) and (FmK[j] <= xb_)) or
              ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
            begin
              F_urb_Per1[j] := 0;
              Furb1[j] := 0;
              Urob[j] := 1;
            end;

          mx := 8;
        end;

        // ------------------ 4 st ----------------------------------
        if (s3 < belv) and (0 < s3) and (delta <= 20) and (delta > 1) and Filter_Siezd
        then
        begin
          stv := 4;
          v1 := V_ogr_UPP(0, vt, belv, 2);
          v2 := V_ogr_UPP(1, vtg, belv, 2);

          if GlbFlagRemontKm and (RemTab9 > 0) then
          begin
            v1 := VTab9(2, belv, vt);
            v2 := VTab9(2, belv, vtg);

            if v1 = -1 then
              v1 := V_ogr_UPP(0, vt, belv, 2);
            if v2 = -1 then
              v2 := V_ogr_UPP(1, vtg, belv, 2);
          end;

          if ((v1 >= 0) and (v1 < vt)) or ((v2 >= 0) and (v2 < vtg)) then
          begin

            if (v1 >= vr) then
              v1 := -1;
            if (v2 >= Vrg) then
              v2 := -1;

            filter := false;
            glbCount_Per4s := glbCount_Per4s + 1;
            xxxx := (L0v div 100) + 1;
            comment := 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx)
              + ' П; ';

            WPer[ns].st := 4;
            WPer[ns].bel := belv;
            WPer[ns].L0 := L0v;
            WPer[ns].Lm := Lmv;
            WPer[ns].v := vt;
            WPer[ns].vg := vtg;
            WPer[ns].vop := v1;
            WPer[ns].vog := v2;
            WPer[ns].Vrp := vr;
            WPer[ns].Vrg := Vrg;
            WPer[ns].flg := false;
            WPer[ns].isLong := abs(L0v - Lmv) > 20;
            if GlbFlagRemontKm then
              WPer[ns].prim := GlbTempTipRemontKm;
            ns := ns + 1;
            WRT_UBEDOM(L0v, Lmv, 4, comment, v1, v2);

            for j := 0 to high(FmK) do
              if ((xa_ <= FmK[j]) and (FmK[j] <= xb_)) or
                ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
              begin
                F_urb_Per1[j] := 0;
                Furb1[j] := 0;
                Urob[j] := 1;
              end;

          end;
          mx := 8;
        end;
        if (s1 >= belv) or (s1 < 0) then
          jalau := false;

      end
      else
        break;
    end;
    Setlength(WPer, ns);
  except
  end;
end;

// = = = = = = = = = = = = = = = = = = = = = = = = =       длинные перекосы

procedure GetPerekosLong(Fm, Fm_sr, FmK: mas; var WPer: masots);
const
  D = 30.0; // metr
  Dl_per = 13; // 5 m
var
  i, j, ik, m, i1, i2, xxxx, pg, Ikf, jalauCount: integer;
  H, h0, Hl, Hr, Ln, mx, mxL, Mxr, hmin, s1, s2, s3: real;
  flag, stv, belv, L0v, Lmv, ogranichenie1, a, b, LnInt, vtg, v1, v2, vr, Vrg, delta,
    xa_, xb_: integer;
  flagPer, filter, otsf, filter2, jalau, Filter_Siezd: boolean;
  comment, rem_inf: string;

begin
  try
     flagPer:=false;
    otsf := false;
    Ikf := NAPR_DBIJ;
    m := 30;
    p := 0;
    Setlength(WPer, 3000);
    pg := 0;
    jalauCount := 0;
    mx := 8;
    jalau := true;
    h0 := 0;
    while (jalau and (jalauCount <= high(Fm))) do
    begin
      jalauCount := jalauCount + 1;
      i1 := -1;
      i2 := -1;
      Ln := 0;

      for i := 0 to high(Fm) - m - 1 do
      begin
        for j := i to i + m do
          if (Fm[i] * Fm[j] < 0) then
          begin
            h0 := abs(Fm_sr[i] - Fm_sr[j]);

            H := (abs(Fm[i]) + abs(Fm[j]));
            if (h0 > 3) then
              H := min(abs(Fm[i]), abs(Fm[j]));
            for ik := 0 to high(UKrv) do
            begin
              // if ((CoordinateToReal(Ukrv[ik].nkm,Ukrv[ik].nmtr) <= CoordinateToReal(GlbKmTrue,F_Mtr[i])) and
              // (CoordinateToReal(GlbKmTrue,F_Mtr[i])<= CoordinateToReal(Ukrv[ik].kkm,Ukrv[ik].kmtr)))   then
              begin
                h0 := (abs(Fm_sr[i] - Fm_sr[j]));

                H := (abs(Fm[i]) + abs(Fm[j]));
                if (h0 > 2) then
                  H := min(abs(Fm[i]), abs(Fm[j]));

                Hl := (abs(Fm[i]) + abs(Fm[i + 20])) * h0;
                Hr := (abs(Fm[j]) + abs(Fm[j - 20])) * h0;

              end;

            end;
            if (mx < H) then
            begin
              // writeln('h0=' + floattostr(h0) + ' H=' + floattostr(H));
              mx := H;
              mxL := Hl;
              Mxr := Hr;
              Ln := abs(FmK[i] - FmK[j]);
              i1 := i;
              i2 := j;
            end;
          end;
      end;

      if mx <= 8 then
        break;

      if (i1 >= 0) and (i2 >= 0) and (0 < Ln) and (Ln <= 30) then
      begin

        vt := F_V[i1];
        vtg := F_Vg[i1];
        vr := F_Vrp[i1];
        Vrg := F_Vrg[i1];
        j := G_Ind2(pg, vt);
        s0 := DeltaPer[1, j];
        s1 := DeltaPer[2, j];
        s2 := DeltaPer[3, j];
        s3 := DeltaPer[4, j];

        belv := round(max(Hr, Hl));
        a := FmK[i1];
        b := FmK[i2];
        L0v := min(a, b);
        Lmv := max(a, b);

        if (Hr > Hl) then
          L0v := Lmv - 19;

        if (Hr <= Hl) then
          Lmv := L0v + 19;

        if L0v <= 0 then
          L0v := 1;
        if Lmv <= 0 then
          Lmv := 1;

        xa_ := L0v - NAPR_DBIJ * Dl_per;
        xb_ := Lmv + NAPR_DBIJ * Dl_per;

        Filter_Siezd := not FlagSiezd(L0v, Lmv);
        delta := abs(Lmv - L0v);
        if (s0 <= belv) and (s1 >= belv) and (Ln > 20) then
        begin
          fdSkew := fdSkew + 1;
          WPer[ns].st := 1;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagRihtUbed;
          WPer[ns].prim := '';
          WPer[ns].isLong := abs(L0v - Lmv) > 15;
          ns := ns + 1;
        end;
        // --------------2 st ---------------------------------
        if (s1 < belv) and (belv <= s2) and (0 < s1) and (0 < s2) and
          (delta > 20) then
        begin
          stv := 2;
          per_s2 := per_s2 + 1;
          GetV_Remont(2, L0v, Lmv, vt, vtg, belv);
          WPer[ns].s3 := s2;
          WPer[ns].st := stv;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagRihtUbed;
          WPer[ns].prim := '';
          WPer[ns].isLong := abs(L0v - Lmv) > 18;
          ns := ns + 1;

          for j := 0 to high(FmK) do
            if ((xa_ <= FmK[j]) and (FmK[j] <= xb_)) or
              ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
            begin
              F_urb_Per1[j] := 0;
              Furb1[j] := 0;
              Urob[j] := 1;
            end;

          mx := 8;
        end;
        // ------------------3 st---------------------------------
        if (s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and
          (delta > 20) then
        begin
          stv := 3;
          per_s3 := per_s3 + 1;
          GetV_Remont(2, L0v, Lmv, vt, vtg, belv);
          WPer[ns].st := stv;
          WPer[ns].bel := belv;
          WPer[ns].L0 := L0v;
          WPer[ns].Lm := Lmv;
          WPer[ns].v := vt;
          WPer[ns].vg := vtg;
          WPer[ns].vop := -1;
          WPer[ns].vog := -1;
          WPer[ns].Vrp := vr;
          WPer[ns].Vrg := Vrg;
          WPer[ns].flg := GlbFlagRihtUbed;
          WPer[ns].prim := '';
          WPer[ns].isLong := abs(L0v - Lmv) > 18;
          ns := ns + 1;

          for j := 0 to high(FmK) do
            if ((xa_ <= FmK[j]) and (FmK[j] <= xb_)) or
              ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
            begin
              F_urb_Per1[j] := 0;
              Furb1[j] := 0;
              Urob[j] := 1;
            end;

          mx := 8;
        end;

        // ------------------ 4 st ----------------------------------
        if (s3 < belv) and (0 < s3) and (delta > 20) and Filter_Siezd then
        begin
          stv := 4;
          v1 := V_ogr_UPP(0, vt, belv, 2);
          v2 := V_ogr_UPP(1, vtg, belv, 2);

          if GlbFlagRemontKm and (RemTab9 > 0) then
          begin
            v1 := VTab9(2, belv, vt);
            v2 := VTab9(2, belv, vtg);

            if v1 = -1 then
              v1 := V_ogr_UPP(0, vt, belv, 2);
            if v2 = -1 then
              v2 := V_ogr_UPP(1, vtg, belv, 2);
          end;

          if ((v1 >= 0) and (v1 < vt)) or ((v2 >= 0) and (v2 < vtg)) then
          begin

            if (v1 >= vr) then
              v1 := -1;
            if (v2 >= Vrg) then
              v2 := -1;

            filter := false;
            glbCount_Per4s := glbCount_Per4s + 1;
            xxxx := (L0v div 100) + 1;
            comment := 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx)
              + ' П; ';

            WPer[ns].st := 4;
            WPer[ns].bel := belv;
            WPer[ns].L0 := L0v;
            WPer[ns].Lm := Lmv;
            WPer[ns].v := vt;
            WPer[ns].vg := vtg;
            WPer[ns].vop := v1;
            WPer[ns].vog := v2;
            WPer[ns].Vrp := vr;
            WPer[ns].Vrg := Vrg;
            WPer[ns].flg := false;
            WPer[ns].isLong := abs(L0v - Lmv) > 15;
            if GlbFlagRemontKm then
              WPer[ns].prim := GlbTempTipRemontKm;
            ns := ns + 1;
            WRT_UBEDOM(L0v, Lmv, 4, comment, v1, v2);

            for j := 0 to high(FmK) do
              if ((xa_ <= FmK[j]) and (FmK[j] <= xb_)) or
                ((xa_ >= FmK[j]) and (FmK[j] >= xb_)) then
              begin
                F_urb_Per1[j] := 0;
                Furb1[j] := 0;
                Urob[j] := 1;
              end;

          end;
          mx := 8;
        end;
        if (s1 >= belv) or (s1 < 0) then
          jalau := false;

      end
      else
        break;
    end;
    Setlength(WPer, ns);
  except
  end;
end;

/// ///////////////////////////////////
procedure Get_UU; // ??
const
  // La  = 600;  // sm
  La = 2000; // sm
  Dl_per = 500; // 5 m
var
  //
  k, kz, ky, kzy: integer;
  max: integer;
  //
  Ikf, i, j, fa, fb, Hn, i1, kk: integer;
  stv, belv, L0v, Lmv: integer;
  flgPro: boolean;
  ogranichenie, ots: string;
  ogranichenie1: integer;
  xxxx, cou_4st, v1, v2, vt, vtg, pg, vr, Vrg: integer;
  xk: integer;
Begin
  Try
    // SabLog('GetFpro - ??????????? ???, ??????, ???????? ????., ???. ? ?????. ?????. ??????????? ?? ????????? ?? ?????? ? ?????? ??? ?????? ');
    Ikf := NAPR_DBIJ;
    i := Low(Fm);

    s1 := 0;
    s2 := 0;
    s3 := 0;
    {
      s1:= DeltaPro_UR_per[1,j];
      s2:= DeltaPro_UR_per[2,j];
      s3:= DeltaPro_UR_per[3,j];
    }

    while i < (High(Fm) - 46) do
    begin
      max := 0;
      kz := 0;
      vt := F_V[i];
      pg := 0;
      vtg := F_Vg[i];
      vr := F_Vrp[i];
      Vrg := F_Vrg[i];

      j := G_Ind2(pg, vt);
      s1 := DeltaUrb[1, j];
      s2 := DeltaUrb[2, j];
      s3 := DeltaUrb[3, j];

      For ky := 1 to 10 do
      begin
        L0 := FmK[i + ky];
        fa := Fm[i + ky];

        For k := 1 to 35 do
        begin
          Lm := FmK[i + k + ky];
          Ln := round(abs(Lm - L0));
          fb := Fm[i + k + ky];
          Hn := round(abs(fa - fb));

          if (Hn > max) and (2000 < Ln) and (Ln < 3000) and (Hn > 2 * s2) then
          begin
            max := Hn;
            kz := k;
            kzy := ky;
          end;

        end; // for po K

      end; // for po Ky
      Hn := round(abs(fa - Fm[i + kz + kzy]));
      Ln := round(abs(FmK[i + kz + kzy] - L0));
      Lm := FmK[i + kz + kzy];

      if (2000 < Ln) and (Ln < 3000) and (s2 * 2 < Hn) then
      begin
        belv := Hn;
        L0v := L0;
        Lmv := Lm;
        stv := 4;
        i := i + kz + kzy;

        ogranichenie1 := OpredelenieOgr_SkorostiPer(stv, belv);
        v1 := G_Ind2_skorost2(0, ogranichenie1);
        v2 := G_Ind2_skorost2(1, ogranichenie1);

        if not FlagSiezd(L0v div 100, Lmv div 100) and
          not ProberkaNaStrelku(L0v div 100, Lmv div 100, 7) and
          (((v1 >= 0) and (v1 < vt)) or ((v2 >= 0) and (v2 < vtg))) then
        begin
          if GlbFlagRemontKm and (0 < vr) and (vr < v1) then
            v1 := -1;
          if GlbFlagRemontKm and (0 < Vrg) and (Vrg < v2) then
            v2 := -1;

          xxxx := ((L0v div 100) mod 1000) div 100 + 1;
          ots := 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx) + ' УУ; ';
          WRT_UBEDOM(L0v, Lmv, 6, ots, v1, v2);
          // FormatFloat('0.00',belv)
          wrt_baskalar('УУ', '', Hn, L0 div 100, round(abs(L0 - Lm) / 100), 5,
            0, vt, vtg, v1, v2, 1);
          GlbCountDGR := GlbCountDGR + 1;
        end;
        ns := ns + 1;
      end;
      i := i + 1;
    end; // for
  except
  end;
end;

// ------------------------------------------------------------------------------
// с помощью процедуры уширения
// ------------------------------------------------------------------------------
function IndexV(Pv, v: integer): integer;
var
  j: integer;
begin
  j := 2;
  case Pv of
    0:
      case v of
        121 .. 140:
          j := 1;
        61 .. 120:
          j := 2;
        41 .. 60:
          j := 3;
        16 .. 40:
          j := 4;
        0 .. 15:
          j := 5;
      end;
    1:
      case v of
        81 .. 90:
          j := 1;
        61 .. 80:
          j := 2;
        41 .. 60:
          j := 3;
        16 .. 40:
          j := 4;
        0 .. 15:
          j := 5;
      end;
  end;
  IndexV := j;
end;

// ==============================================================================
function IndexV_pros(Pv, v: integer): integer;
var
  j: integer;
begin
  j := 1;
  if Pv = 0 then
    case v of
      121 .. 140:
        j := 1;
      61 .. 120:
        j := 2;
      41 .. 60:
        j := 3;
      16 .. 40:
        j := 4;
      0 .. 15:
        j := 5;
    end
  else
    case v of
      81 .. 90:
        j := 1;
      61 .. 80:
        j := 2;
      41 .. 60:
        j := 3;
      16 .. 40:
        j := 4;
      0 .. 15:
        j := 5;
    end;

  IndexV_pros := j;
end;
      ////===
procedure GetRiht(Fm, FmK, FmTrapez: mas; var WRih: masots; isriht: boolean);
const
  D = 20; // metr
var
 i01, i02, k,i, j, m, i1, i2, Ln,Ln2, tran_length, tempIndex: integer;
h20, H00,hprom,Hstr1,Hstr2,Hh0 ,H,H2,H3,H4 ,s0, s1, s2, s3, h0,h_per: real;
  belv, L0v, Lmv, vtg, v1, v2, vr, Vrg: integer;
  H_Trans_ext,H_Trans,transitions: array of integer;
  ee0,ee1, ee2, ee3,ee4,e1, e2, e3,e4,e5,e6,b1, b2: Extremum;
  h1: real;
  temp: masots;
  isnotcorrect,isLong: boolean;
begin

  // if  then
      Setlength(WRih, 3000);
  Setlength( H_Trans, 3000);
    Setlength(  H_Trans_ext, 3000);
  Setlength(transitions, 1);
  transitions[0] := 0;
  tran_length := 1;
  h0 := 0;
  i := 0;
 Fm[0] :=0;
       //Fm[1] :=0;
    Fm[high(F_mtr)] :=0;
     // Fm[high(F_mtr)-1] :=0;

    i:=1;

      while i < high(F_mtr)  do
  begin
    if ( (Fm[i]* Fm[i+1] )<=0 ) then
    begin

      Setlength(transitions, tran_length + 1);
      transitions[tran_length] := i;
      if (abs(Fm[i] ) <0.01) then
        transitions[tran_length] := i
      else if (abs(Fm[i + 1])<=0.01) then
      begin
        transitions[tran_length] := i + 1;
        i := i + 1;
      end;
      if (tran_length >0) then
      begin

             e1 := GetExtremum(Fm, transitions[tran_length - 2],
            transitions[tran_length - 1]);
          e2 := GetExtremum(Fm, transitions[tran_length - 1],
            transitions[tran_length]);
        if( abs(Fm[transitions[tran_length]] - Fm[transitions[tran_length - 1]])  < 10 )and ( abs(e1.value)<10) and ( abs(e2.value)<10)
        then
        begin                                                                           //  and ( abs(e1.value)<3) and ( abs(e2.value)<3)
          e1 := GetExtremum(Fm, transitions[tran_length - 2],
            transitions[tran_length - 1]);
          e2 := GetExtremum(Fm, transitions[tran_length - 1],
            transitions[tran_length]);
         // if (abs(e1.value) < 10) or  (abs(e2.value) < 10   then
             if (( abs(FmK[e1.index] - FmK[e2.index])<7 )
             or (abs(transitions[tran_length - 2] -transitions[tran_length - 1] ) <10  ))  and ( abs(e1.value)<10) and ( abs(e2.value)<10)
              then
            tran_length := tran_length - 2;
        end;

      end;
      tran_length := tran_length + 1;
    end;
    i := i + 1;
  end;
      // Setlength(transitions, tran_length+1 );
    //   transitions[tran_length -1]  :=high(F_mtr);

    i := 0;
    // i:=z2;


          //i:=High(transitions)-3;

    while i < High(transitions)-2 do
    begin
                i := i + 1;

        // h0:= abs(TrapezLevel_Get_per[e1.index]- TrapezLevel_Get_per[e2.index]  );
     e1 := GetExtremum(Fm, transitions[i - 1], transitions[i]);
      e2 := GetExtremum(Fm, transitions[i], transitions[i + 1]);

      e3 := GetExtremum(Fm, transitions[i ], transitions[i+1]);
      e4:= GetExtremum(Fm, transitions[i+1], transitions[i + 2]) ;

              H:=0;
              H2:=0;
       h0:=  abs(FmTrapez[e1.index]-FmTrapez[e2.index]);
          h20:=  abs(FmTrapez[e3.index]-FmTrapez[e4.index]);
            Ln:= abs(FmK[e1.index] -FmK[e2.index]) ;
                Ln2:= abs(FmK[e3.index] - FmK[e4.index]);
              if ( (e1.sign*e2.sign<0)and (Ln<=30)  ) then   H:= (abs(Fm[e1.index])+ abs(Fm[e2.index]) ) ; // and (h0<=1)
                 if ( (Fm[e1.index]*Fm[e2.index]<0)and (Ln<=20)  ) then   H:= (abs(Fm[e1.index])+ abs(Fm[e2.index]) ) ;

                      if ( (e3.sign*e4.sign<0)and (Ln2<=20) and (h20<=1) ) then  H2:= (abs(Fm[e3.index])+ abs(Fm[e4.index]) ) ;

                if Ln<20 then  H_Trans[e1.index] :=round(abs(e1.value )+abs(e2.value ));
                 if (H2>H) and(Ln<=20 )and (Ln2<=20) then
                 begin
                   //e1:= e3;
                   //e2:=e4;
                   //H:=H2;
                  // Ln:=Ln2;
                 end;


                   h0:=  abs(FmTrapez[e1.index]-FmTrapez[e2.index]);
          Hh0:=   Fm[e1.index] +FmTrapez[e1.index] -(Fm[e2.index] +FmTrapez[e2.index]);
        //  Hh0:=Hh0+  abs(abs (Fm[e2.index] +FmTrapez[e2.index])- abs(FmTrapez[e2.index]) );  ;

       //  if ( h0>1 ) then    H:=abs( Fm[e1.index])+ abs( Fm[e2.index]);
           //   if ( h0>1 ) then    H:= min(H,Hh0);


                 Hstr1:=   100;
      Hstr2:=   100;
      if  (abs(FmTrapez[e1.index-10])>40) or (abs(FmTrapez[e1.index+10])>40 ) or ( abs(FmTrapez[e2.index+10])>40 )
      or ( abs(FmTrapez[e2.index-10])>40 )   then
         begin
      Hstr1:=   abs(Fm[e1.index]-FmTrapez[e1.index] ) ;
       Hstr2:=   abs(Fm[e2.index]-FmTrapez[e2.index] ) ;
         H := min (abs(Fm[e1.index]) ,abs(Fm[e2.index]));
            H := min (abs(Fm[e1.index]) ,10);
      end;


          isLong := false;
    i01 := e1.index;
    i02 := e2.index;
      if  i01 >390 then
          L0v := FmK[i01];
         if (i1 >= 0) and (i2 >= 0) and (20 < Ln) and (Ln <= 30) then
         begin
            isLong := true;
           Ln:=20;
         end;
    if (i1 >= 0) and (i2 >= 0) and (0 < Ln) and (Ln <= D) then
    begin
      vt := F_V[i01];
      vtg := F_Vg[i01];
      vr := F_Vrp[i01];
      Vrg := F_Vrg[i01];
      L0v := FmK[i01];
      Lmv := FmK[i02];
      Ln := Ln * 2;

      s1 := Velich_table_3dot3(Ln, vt, 1);
      s0 := Velich_table_3dot3(Ln, vt, 0);

      if (Ln <=8) then
        belv := round(H / 1.3)
         else if (Ln > 8) and (Ln <= 10) then
        belv := round(  H / (1.3 - (0.03 / 2) * (Ln - 8))  );
       if (Ln >=10) and (Ln <= 21) then
        belv := round(  H / (1.27 - (0.54 / 12.0) * (Ln - 9) )  );
          if (Ln >= 20) and (Ln<=25)  then
                 belv := round(  H / ( 0.75 - (0.045 / 5.0) * (Ln - 20))   );
      if (Ln >= 25)  and (Ln <= 40 ) then
        belv := round(H / 0.75);
          if ((Ln >= 40 )and ( islong ) ) then
        belv := round(0.9*H / 0.75);
        if (H/Ln)>2 then
        begin
          H:=0;
           isnotcorrect:=true;
        end;

      //if belv >= 18 then
      //  belv := belv;
      // ln := ln div 2;

      if (s0 > belv +5) then
        continue;

      if ((length(temp) > 0) and ((temp[length(temp) - 1].bel > belv) or
        (i - temp[length(temp) - 1].transitionIndex > 1))) then
      begin
        WriteTempOtsToRihOts(temp, D, ns, isriht, WRih);
        if (temp[length(temp) - 1].bel > belv) and
          (i - temp[length(temp) - 1].transitionIndex = 1) then
        begin
          Setlength(temp, 0);
          continue;
        end;
        Setlength(temp, 0);
      end;

      tempIndex := length(temp);
      Setlength(temp, tempIndex + 1);
      temp[tempIndex].bel := belv;
      temp[tempIndex].L0 := L0v;
      temp[tempIndex].Lm := Lmv;
      temp[tempIndex].v := vt;
      temp[tempIndex].vg := vtg;
      temp[tempIndex].Vrp := vr;
      temp[tempIndex].Vrg := Vrg;
      temp[tempIndex].transitionIndex := i;
      temp[tempIndex].isLong := isLong;
    end

  end;
  WriteTempOtsToRihOts(temp, D, ns, isriht, WRih);
  Setlength(WRih, ns);

end;
     ////--get
// procedure GetRiht(Fm, FmK: mas; var WRih: masots; isriht: boolean);
// const
// length = 40; // metr
// var
// i, j, m, i1, i2, xxxx, pg, Ikf, Dl_rix, jalauCount, k, imin, imax, z1, z2,
// Ln: integer;
// H, hmin, s1, s2, s3, minx, maxx: real;
// stv, belv, L0v, Lmv, ogranichenie1, a, b, LnSm, LnInt, vtg, v1, v2, vr, Vrg,
// delta: integer;
// filter, otsf, filter2, jalau, jointless, is2to3OnJoint, is3OnJoint, is4OnJoint: boolean;
// comment, rem_inf: string;
// begin
// try
// otsf := false;
// Ikf := NAPR_DBIJ;
// Dl_rix := 1500;
// p := 0;
// Setlength(WRih, 3000);
// pg := 0;
// i := 0;
//
// z2 := 1;
// while i <= high(Fm) - 2 do
// begin
//
// z1 := z2;
// if (Fm[i] * Fm[i + 1] <= 0) then
// begin
// z2 := i;
// minx := 0;
// imin := z1;
//
// for k := z1 to z2 do
// begin
// if minx < abs(Fm[k]) then
// begin
// minx := abs(Fm[k]);
// imin := k;
// end;
// end;
//
// maxx := 0;
// imax := z2;
//
// for k := z2 to imin + 40 do
// begin
// if (maxx < abs(Fm[k])) then
// begin
// maxx := abs(Fm[k]);
// imax := k;
// end;
// if (Fm[z2] * Fm[k] < 1) and (abs(z2 - k) > 7) then
// break;
// end;
//
// // i:=z2;
//
// Ln := abs(FmK[imin] - FmK[imax]) * 2;
//
// LnSm := Ln;
// i1 := imin;
// i2 := imax;
//
// H := abs(Fm[imin] - Fm[imax]);
//
// if (i1 >= 0) and (i2 >= 0) and (0 < Ln) and (Ln <= length) then
// begin
//
// vt := F_V[i1];
// vtg := F_Vg[i1];
// vr := F_Vrp[i1];
// Vrg := F_Vrg[i1];
// s1 := Velich_table_3dot3(LnSm, vt, 1);
// s2 := Velich_table_3dot3(LnSm, vt, 2);
// s3 := Velich_table_3dot3(LnSm, vt, 3);
// jointless := CheckForJointlessPath(FmK[imin],FmK[imax]);
// if (Ln < 9) then
// belv := round(H / 1.2)
// else if (Ln >= 9) and (Ln <= 21) then
// belv := round(H / (1.2 - (0.53 / 12) * (Ln - 9)))
// else
// belv := round(H / 0.67);
// if belv >= 18 then
// belv := belv;
// a := FmK[i1];
// b := FmK[i2];
// L0v := min(a, b);
// Lmv := max(a, b);
// if NAPR_DBIJ < 0 then
// begin
// L0v := max(a, b);
// Lmv := min(a, b);
// end;
//
// if L0v <= 0 then
// L0v := 1;
// if Lmv <= 0 then
// Lmv := 1;
// delta := abs(Lmv - L0v) * 2;
//
// filter2 := false;
// if not FlagSiezd(L0v, Lmv) then
// filter2 := true;
// is2to3OnJoint := ((jointless) and (((ln <= 20) and (belv >= 24)) or
// ((ln >= 20) and (belv >= 33)))) and ((s1 < belv) and (belv <= s2) and (0 < s1) and (0 < s2) and
// (delta <= length));
//
// // --------------2 st ---------------------------------
// if ((s1 < belv) and (belv <= s2) and (0 < s1) and (0 < s2) and
// (delta <= length)) and not(is2to3OnJoint) then
// begin
//
// if isriht then
// begin
// if (ns > 0) then
// begin
// if (((NAPR_DBIJ = -1) and
// ((WRih[ns - 1].L0 = L0v + abs(WRih[ns - 1].L0 - WRih[ns - 1]
// .Lm)) or (WRih[ns - 1].L0 < L0v + abs(WRih[ns - 1].L0 -
// WRih[ns - 1].Lm) * 2))) or
// ((NAPR_DBIJ = 1) and ((L0v = WRih[ns - 1].Lm) or
// (L0v < (WRih[ns - 1].Lm - WRih[ns - 1].L0) / 0.5)))) then
// begin
// if (WRih[ns - 1].st > 2) then
// begin
// i := imax;
// z2 := imax;
// continue;
//
// end;
//
// if (abs(belv - Velich_table_3dot3(LnSm, vt, 2)) >
// abs(WRih[ns - 1].bel - Velich_table_3dot3
// (LengthInMetr(WRih[ns - 1]), vt, 2))) then
// begin
// i := imax;
// z2 := imax;
// continue;
// end
// else
// begin
// ns := ns - 1;
// rih_s2 := rih_s2 - 1;
// rih1_s2 := rih1_s2 - 1;
// end;
//
// end;
//
// end;
// WRih[ns].Lm := 0;
// stv := 2;
/// /              rih_s2 := rih_s2 + 1;
/// /              rih1_s2 := rih1_s2 + 1;
// GetV_Remont(3, L0v, Lmv, vt, vtg, belv);
// WRih[ns].st := stv;
// WRih[ns].bel := belv;
// WRih[ns].L0 := L0v;
// WRih[ns].Lm := L0v + Ln; // Lmv;     ////////bx3
// WRih[ns].v := vt;
// WRih[ns].vg := vtg;
// WRih[ns].vop := -1;
// WRih[ns].vog := -1;
// WRih[ns].Vrp := vr;
// WRih[ns].Vrg := Vrg;
// WRih[ns].flg := GlbFlagRihtUbed;
// WRih[ns].prim := '';
// //близкие к 3 степени на изостыке
//
//
//
//
// WRih[ns].onswitch := ProberkaNaStrelku(L0v, 1) or
// ProberkaNaStrelku(L0v + Ln, 1);
// WRih[ns].isriht := isriht;
// if not(WRih[ns].onswitch) then
// begin
// rih_s2 := rih_s2 + 1;
// rih1_s2 := rih1_s2 + 1;
// end;
//
// ns := ns + 1;
// end;
// i := imax;
// z2 := imax;
//
// end;
// is3onJoint := ((s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and
// (delta <= length)) and jointless;
// // ------------------3 st---------------------------------
// if (((s2 < belv) and (belv <= s3) and (0 < s2) and (0 < s3) and
// (delta <= length)) and not(is3onJoint)) or is2to3OnJoint then
// begin
//
//
// GetV_Remont(3, L0v, Lmv, vt, vtg, belv);
// if (ns > 0) then
//
// if (((NAPR_DBIJ = -1) and (WRih[ns - 1].L0 = L0v +
// abs(WRih[ns - 1].L0 - WRih[ns - 1].Lm))) or
// ((NAPR_DBIJ = 1) and (L0v = WRih[ns - 1].Lm))) then
// begin
// if (WRih[ns - 1].st > 3) then
// begin
// i := imax;
// z2 := imax;
// continue;
// end;
// if (WRih[ns - 1].st < 3) then
// begin
// ns := ns - 1;
// Wrih[ns].prim:='';
// Wrih[ns].isEqualTo4:=false;
// Wrih[ns].isEqualTo3:=false;
//
// end
// else
//
// if (abs(belv - Velich_table_3dot3(LnSm, vt, 3)) >
// abs(WRih[ns - 1].bel - Velich_table_3dot3
// (LengthInMetr(WRih[ns - 1]), vt, 3))) then
// begin
// i := imax;
// z2 := imax;
// continue;
// end
// else
// begin
// ns := ns - 1;
// if isriht then
// begin
// rih_s3 := rih_s3 - 1;
// rih1_s3 := rih1_s3 - 1;
// end;
// end;
// end;
// if Is2to3OnJoint then
// begin
// stv := 2;
// WRih[ns].isEqualTo3 := true;
// end else
// stv :=3;
// WRih[ns].st := stv;
// WRih[ns].bel := belv;
// WRih[ns].L0 := L0v;
// WRih[ns].Lm := L0v + Ln; // Lmv;     ////////bx3   Lmv;
// WRih[ns].v := vt;
// WRih[ns].vg := vtg;
// WRih[ns].vop := -1;
// WRih[ns].vog := -1;
// WRih[ns].Vrp := vr;
// WRih[ns].Vrg := Vrg;
// WRih[ns].flg := GlbFlagRihtUbed;
//
//
// WRih[ns].isriht := isriht;
// WRih[ns].onswitch := ProberkaNaStrelku(L0v, 1) or
// ProberkaNaStrelku(L0v + Ln, 1);
// if isriht and not(WRih[ns].onswitch) then
// begin
// rih_s3 := rih_s3 + 1;
// rih1_s3 := rih1_s3 + 1;
// end;
// if not(isriht) and (not(WRih[ns].prim.Contains('рн;'))) then
// WRih[ns].prim:=WRih[ns].prim + 'рн;';
// if (jointless) and (not(WRih[ns].prim.Contains('t+;'))) then
// begin
// WRih[ns].prim := WRih[ns].prim + 't+;';
// end;
// ns := ns + 1;
// i := imax;
// z2 := imax;
// end;
// is4OnJoint :=  ((s3 < belv) and (0 < s3) and (delta <= length) and filter2) and jointless;
// // ------------------ 4 st ----------------------------------
// if ((s3 < belv) and (0 < s3) and (delta <= length) and filter2) or (is3onJoint) then
// begin
// if (imin > imax) then
// i := imin
// else
// i := imax;
// if (is3onJoint) then
// begin
// stv := 3;
// WRih[ns].isEqualTo4 := true;
// v1 := V_ogr_rih_tab_3IsEqualTo4(0, vt, LnSm,belv);
// v2 := V_ogr_rih_tab_3IsEqualTo4(1, vtg, LnSm, belv);
// end else
// begin
// stv := 4;
// v1 := V_ogr_rih_tab_3dot3(0, vt, LnSm, belv);
// v2 := V_ogr_rih_tab_3dot3(1, vtg, LnSm, belv);
// end;
//
//
// if GlbFlagRemontKm and (RemTab9 > 0) then
// begin
// v1 := VTab9(3, belv, vt);
// v2 := VTab9(3, belv, vtg);
// if v1 = -1 then
// v1 := V_ogr_rih_tab_3dot3(0, vt, LnSm, belv);
// if v2 = -1 then
// v2 := V_ogr_rih_tab_3dot3(1, vtg, LnSm, belv);
// end;
//
// if ((15 < v1) and (v1 < vt)) or ((15 < v2) and (v2 < vtg)) then
// begin
// filter := false;
// filter2 := false;
// if GlbFlagRemontKm and (v1 >= vr) and (0 <= vr) and (RemTab9 = 0)
// then
// v1 := -1;
// if GlbFlagRemontKm and (v2 >= Vrg) and (0 <= Vrg) and (RemTab9 = 0)
// then
// v2 := -1;
//
// xxxx := (L0v div 100) + 1;
// comment := 'Р п' + inttostr(xxxx) + ' v=' +
// V_shekti(v1, v2) + ';';
//
// WRih[ns].st := stv;
// WRih[ns].bel := belv;
// WRih[ns].L0 := L0v;
// WRih[ns].Lm := Lmv;
// WRih[ns].v := vt;
// WRih[ns].vg := vtg;
// WRih[ns].vop := v1;
// WRih[ns].vog := v2;
// WRih[ns].Vrp := vr;
// WRih[ns].Vrg := Vrg;
// WRih[ns].flg := false;
// WRih[ns].isriht := isriht;
// WRih[ns].onswitch := ProberkaNaStrelku(L0v, 1) or
// ProberkaNaStrelku(Lmv, 1);
// if not(WRih[ns].onswitch) then
// begin
// glbCount_Rih4s := glbCount_Rih4s + 1;
// WRT_UBEDOM(L0v, Lmv, 4, comment, v1, v2);
// end;
// if (v1 = -1) and (v2 = -1) then
// WRih[ns].prim := GlbTempTipRemontKm;
// if not(isriht) then
// WRih[ns].prim := WRih[ns].prim +'рн;';
// if (jointless) then
// begin
// WRih[ns].prim := WRih[ns].prim + 't+;';
// end;
// ns := ns + 1;
// i := imax;
// z2 := imax;
// end;
// end; // 4 st
// end;
// end;
// i := i + 1;
//
// end;
//
// Setlength(WRih, ns);
// except
// end;
// end;

function LengthInMetr(ots: OtsInf): integer;
begin
  LengthInMetr := abs(round((ots.Lm - ots.L0)));
end;

procedure Fluc_Define(right, left: mas; xcoord: MFuncs2);
var
  i: integer;
begin
  for i := 0 to high(F_fluk) do
  begin

    if (xcoord[i].fun > 0) then
    begin
      F_fluk[i] := left[i];
      f_fluk_notriht[i] := right[i] ;
    end
    else
    begin
      F_fluk[i] := right[i];
      f_fluk_notriht[i] := left[i]   ;
    end;

  end;
end;

// ==============================================================================
function OpredelenieOgr_Skorosti(var st, otkl, x, y, j: integer): integer;
var

  i: integer;
  s3: integer;
begin
  // присваиваем границы степеней
  OpredelenieOgr_Skorosti := 6;
  for i := 1 to 6 do
  begin
    s3 := DeltaPro[3, i];
    if (otkl <= s3) then
    begin
      OpredelenieOgr_Skorosti := i;
      break;
    end;
  end; // for

end; // function
// ====================================================

function OpredelenieOgr_SkorostiPro(var st, otkl, x, y, j: integer): integer;
var

  i: integer;
  s3: integer;
begin
  // присваиваем границы степеней
  OpredelenieOgr_SkorostiPro := 6;
  for i := 1 to 6 do
  begin
    s3 := DeltaPro[3, i];
    if (otkl > s3) then
    begin
      OpredelenieOgr_SkorostiPro := i;
    end;
  end; // for

end; // function

// ===============================================================================
function OpredelenieOgr_SkorostiPer(var st, otkl: integer): integer;
var

  i, j: integer;
  s3: integer;
begin
  // присваиваем границы степеней
  OpredelenieOgr_SkorostiPer := 6;
  for i := 1 to 6 do
  begin
    s3 := DeltaPer[3, i];
    if (otkl <= s3) then
    begin
      j := i;
    end;
  end; // for

  OpredelenieOgr_SkorostiPer := j;

end; // function

// ===============================================================================
function OpredelenieOgr_Skorosti_Rihtovka(var otkl, dl: integer): integer;
var

  i, j, v: integer;
  s2: real;
begin
  // присваиваем границы степеней

  // DeltaR(Ln, GlobPassSkorost,2)
  j := 6;

  OpredelenieOgr_Skorosti_Rihtovka := Vu3[0, 1, 6]; // 6;
  for i := 6 downto 1 do
  begin
    v := Vu3[0, 1, i];;
    s2 := DeltaR(0, dl, v, 2); //

    if (otkl <= s2) then
    begin
      j := i;
      break;
    end;
  end; // for

  OpredelenieOgr_Skorosti_Rihtovka := Vu3[0, 1, j]; // j;

end; // function

// =============================================================================
function OpredelenieOgr_SkorostiUsh(var st, otkl, x, y, j: integer): integer;
var

  i: integer;
  s3: integer;
begin
  // присваиваем границы степеней
  OpredelenieOgr_SkorostiUsh := 5;

  for i := 1 to 4 do
  begin
    s3 := DeltaUsh[i, 3, 1];
    if (otkl > s3) then
    begin
      OpredelenieOgr_SkorostiUsh := i;
      // break;
    end;
  end; // for

end; // function

// =============================================================================
function OpredelenieOgr_UklOtbShablon(var otkl: real): integer;
var
  v: integer;
begin
  if (2 > otkl) and (otkl <= 2.5) then
    v := 140;
  if (2.5 < otkl) and (otkl <= 3) then
    v := 120;
  if (3 < otkl) and (otkl <= 3.5) then
    v := 100;
  if (3.5 < otkl) and (otkl <= 4.0) then
    v := 80;
  if (4.0 < otkl) and (otkl <= 4.5) then
    v := 60;
  if (4.5 < otkl) and (otkl <= 5) then
    v := 40;
  if 5 < otkl then
    v := 25;

  OpredelenieOgr_UklOtbShablon := v;
end; // function
// =============================================================================

function OpredelenieOgr_SkorostiSuj(var st, otkl, x, y, j: integer): integer;
var

  i: integer;
  s3: integer;
begin
  // присваиваем границы степеней

  OpredelenieOgr_SkorostiSuj := 5;

  for i := 1 to 4 do
  begin
    s3 := DeltaSuj[i, 3, 1];
    if (otkl > s3) then
    begin
      OpredelenieOgr_SkorostiSuj := i;
      break;
    end;
  end; // for

end; // function

// remont km
function OpredelenieOgr_SkorostiRem_Uroben(var otkl: integer): integer;
var
  i: integer;
  s3: integer;
begin
  // присваиваем границы степеней

  OpredelenieOgr_SkorostiRem_Uroben := 4;

  for i := 1 to 3 do
  begin
    s3 := RDeltaUrb[i];
    if (otkl <= s3) then
    begin
      OpredelenieOgr_SkorostiRem_Uroben := i;
      break;
    end;
  end; // for

end; // function

function OpredelenieOgr_SkorostiSuj_Uroben(var st, otkl, x, y,
  j: integer): integer;
var

  i: integer;
  s3: integer;
begin
  // присваиваем границы степеней

  OpredelenieOgr_SkorostiSuj_Uroben := 6;

  for i := 1 to 6 do
  begin
    s3 := DeltaUrb[3, i];
    if (otkl > s3) then
    begin
      OpredelenieOgr_SkorostiSuj_Uroben := i;
    end;
  end; // for

end; // function

// -------------------------------------------------------------------------------
function Most_Tonnel_Skorosti_Uroben(var st, otkl, x, y, j: integer): integer;
var

  i: integer;
  s2: integer;
begin
  // присваиваем границы степеней

  Most_Tonnel_Skorosti_Uroben := 6;

  for i := 1 to 6 do
  begin
    s2 := DeltaUrb[2, i];
    if (otkl <= s2) then
    begin
      Most_Tonnel_Skorosti_Uroben := i;
      break;
    end;
  end; // for

end; // function

// ------------------------------------------------------------------------------
function Most_Tonnel_Skorosti_Prosadka(var st, otkl, x, y, j: integer): integer;
var

  i: integer;
  s2: integer;
begin
  // присваиваем границы степеней

  Most_Tonnel_Skorosti_Prosadka := 6;

  for i := 1 to 6 do
  begin
    s2 := DeltaPro[2, i];
    if (otkl <= s2) then
    begin
      Most_Tonnel_Skorosti_Prosadka := i;
      break;
    end;
  end; // for

end; // function

// ==============================================================================
function Most_Tonnel_Skorosti_Perekos(var st, otkl, x, y, j: integer): integer;
var

  i: integer;
  s2: integer;
begin
  // присваиваем границы степеней

  Most_Tonnel_Skorosti_Perekos := 6;

  for i := 1 to 6 do
  begin
    s2 := DeltaPro_UR_Per[2, i];
    if (otkl <= s2) then
    begin
      Most_Tonnel_Skorosti_Perekos := i;
      break;
    end;
  end; // for

end; // function

// ==============================================================================
function Most_Tonnel_Skorosti_Rihtovka(var st, otkl, x, y, j: integer): integer;
var

  i: integer;
  s2: integer;
begin
  // присваиваем границы степеней

  Most_Tonnel_Skorosti_Rihtovka := 6;

  for i := 1 to 7 do
  begin
    s2 := DeltaPp2[2, i];
    if (otkl <= s2) then
    begin
      Most_Tonnel_Skorosti_Rihtovka := i;
      break;
    end;
  end; // for

end; // function

// ==============================================================================
function OpredelenieOgr_SkorostiPro_Riht(var st, otkl, x, y,
  j: integer): integer;
var

  i: integer;
  sh3: integer;
begin
  OpredelenieOgr_SkorostiPro_Riht := 6;

  for i := 1 to 6 do
  begin
    sh3 := DeltaPro_Riht[3, i];
    if (otkl <= sh3) then
    begin
      OpredelenieOgr_SkorostiPro_Riht := i;
      break;
    end;
  end; // for

end; // function
// -----------------------------------------------------------------------------

function G_Ind2_Skorost(var j: integer): string;
begin
  case j of
    1:
      G_Ind2_Skorost := '140/90';
    2:
      G_Ind2_Skorost := '120/80';
    3:
      G_Ind2_Skorost := '60/60';
    4:
      G_Ind2_Skorost := '40/40';
    5:
      G_Ind2_Skorost := '15/15';
    6:
      G_Ind2_Skorost := '0/0';
  end; // case
end; // fun

// =========================================
function G_Ind2_skorost2_Riht(Pv, j: integer): integer;
begin
  case Pv of
    0:
      case j of
        1:
          G_Ind2_skorost2_Riht := 140;
        2:
          G_Ind2_skorost2_Riht := 120;
        3:
          G_Ind2_skorost2_Riht := 60;
        4:
          G_Ind2_skorost2_Riht := 40;
        5:
          G_Ind2_skorost2_Riht := 15;
      else
        G_Ind2_skorost2_Riht := 0;
      end; // case
    1:
      case j of
        1:
          G_Ind2_skorost2_Riht := 90;
        2:
          G_Ind2_skorost2_Riht := 80;
        3:
          G_Ind2_skorost2_Riht := 60;
        4:
          G_Ind2_skorost2_Riht := 40;
        5:
          G_Ind2_skorost2_Riht := 15;
      else
        G_Ind2_skorost2_Riht := 0;
      end; // case
  end;

end; // fun

// ====================================================
function G_Ind2_skorost2(Pv, j: integer): integer;
begin
  case Pv of
    0:
      case j of
        1:
          G_Ind2_skorost2 := 140;
        2:
          G_Ind2_skorost2 := 120;
        3:
          G_Ind2_skorost2 := 60;
        4:
          G_Ind2_skorost2 := 40;
        5:
          G_Ind2_skorost2 := 15;
      else
        G_Ind2_skorost2 := 0;
      end; // case
    1:
      case j of
        1:
          G_Ind2_skorost2 := 90;
        2:
          G_Ind2_skorost2 := 80;
        3:
          G_Ind2_skorost2 := 60;
        4:
          G_Ind2_skorost2 := 40;
        5:
          G_Ind2_skorost2 := 15;
      else
        G_Ind2_skorost2 := 0;
      end; // case
  end; // fun
end;

// -----------
function G_Ind2_skorost2Shab(var j: integer): integer;
begin
  case j of
    1:
      G_Ind2_skorost2Shab := 140;
    2:
      G_Ind2_skorost2Shab := 100;
    3:
      G_Ind2_skorost2Shab := 60;
    4:
      G_Ind2_skorost2Shab := 25;
    5:
      G_Ind2_skorost2Shab := 0;
  end; // case
end; // fun

/// remont km
function G_Ind2_skorost2Remont(Var s: string; var j: integer): integer;
const
  cmdArray: Array [0 .. 3] of String = (BPO, BPOBPR, BPOIBPR, BPOBPRDCP);

begin

  case AnsiIndexStr(s, cmdArray) of
    0:
      case j of
        1:
          G_Ind2_skorost2Remont := 60;
        2:
          G_Ind2_skorost2Remont := 40;
        3:
          G_Ind2_skorost2Remont := 25;
        4:
          G_Ind2_skorost2Remont := 0;
      end;
    1:
      case j of
        1:
          G_Ind2_skorost2Remont := 50;
        2:
          G_Ind2_skorost2Remont := 20;
        3:
          G_Ind2_skorost2Remont := 15;
        4:
          G_Ind2_skorost2Remont := 0;
      end;
    2:
      case j of
        1:
          G_Ind2_skorost2Remont := 60;
        2:
          G_Ind2_skorost2Remont := 40;
        3:
          G_Ind2_skorost2Remont := 25;
        4:
          G_Ind2_skorost2Remont := 0;
      end;
    3:
      case j of
        1:
          G_Ind2_skorost2Remont := 70;
        2:
          G_Ind2_skorost2Remont := 50;
        3:
          G_Ind2_skorost2Remont := 25;
        4:
          G_Ind2_skorost2Remont := 0;
      end;
  else
    Beep;
  end;
end;

// =========================================
procedure SaveToUvedomlenie; // Запись в таблицу Уведомления
begin
  with MainDataModule.spAddUbemDat do
  begin
    ParamByName('ind_').value := tip;
    ParamByName('PCH_').value := inttostr(Glb_PutList_PCH); // PCH_F;
    ParamByName('NAPRAV_').value := NAPRABLENIE;
    ParamByName('PUT_').value := GlbNumPut; // inttostr(Glb_PutList_PUT);
    ParamByName('track_id').value := GlbTrackId; // inttostr(Glb_PutList_PUT);
    ParamByName('KM_').value := GlbKmTrue;
    ParamByName('KMTRUE_').value := GlbKmTrue;
    ParamByName('UBEDOM_').value := s;
    ParamByName('UST_V_').value := inttostr(GlobPassSkorost) + '/' +
      inttostr(GlobGruzSkorost);
    ParamByName('OGR_V_').value := ogranichenie_v;
    ParamByName('NCH_KOORD_').value := WUsh.L0;
    ParamByName('KON_KOORD_').value := WUsh.Lm;
    ParamByName('TIPP_').value := GTipPoezdki;
    ParamByName('DVREM_').value := GDatVremForBedomost;
    ExecProc;
  end; // with
end;

end.
  ///  Aset


