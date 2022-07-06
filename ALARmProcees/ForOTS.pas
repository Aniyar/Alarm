unit ForOTS;

interface

USES
  SysUtils, DIALOGS, DataModule, Params, FUNCSPROCS, math, strutils,
  System.Generics.Collections, Generics.Defaults, DateUtils, System.Types;

function NKm_leng(pkm: integer): integer;
function gznak_nest(a, b: real): real;
function NKm_(pkm: integer): boolean;
function KrivoiUch(pmtr: integer): boolean;
procedure GSpeedKM;

procedure KrivoiNatur(kilometr: integer);

function ProberkaNaKrivoi(xx: integer): boolean;
function ProberkaNaStrelku(start, finish, cod: integer): boolean;
function CheckForFactSwitch(meter: integer): boolean;
function ProberkaNaStrelkuReal(currentCoord: real; cod: integer): boolean;
function ProberkaNaMostTonnel(start, finish: integer): boolean;
function CheckForIsoJoint(start_meter, final_meter: integer): boolean;
function CheckForJointlessPath(start_meter, final_meter: integer): boolean;
function ProberkaNaZhelezBetShpal(xx: integer): boolean;
function SearchStrelkaNatur(Fver, FverCoor: mas; xx: integer): boolean;
procedure rixtovoshnayanit(poy: integer);

function FlagKrivoi: boolean;
function LengthKm: real;

procedure Get_NullFunc;
function gznak(a, b: real): real;

PROCEDURE RWTB_INFOTS(pkm: integer);
PROCEDURE Most_Tonnel(var PrmOts: MASOTS; name_ots: string);
/// ///////////////////
// PROCEDURE OgrPoSochetaniu(m1,m2:masots; i:integer);
// sochetanie otstuplenii v plane 3 stepeni s prosadkoi

PROCEDURE OgrPoSochetaniu(m1, m2: MASOTS;
  stepen1, stepen2, INTERVAL_SCOROSTI: integer; ubedtxt: string);
PROCEDURE OgrSmezh_Prosadka(m1, m2: MASOTS);

// smezhnye otstupleniya po rihtovke na dline 75 m
PROCEDURE OgrSmezhnyhOts_Riht(m1: MASOTS; st, kol, dlinaSmezh: integer);
// massiv, ctepen, kolichestvo, dlina
// smezhnye otstupleniya po prosadke
PROCEDURE OgrSmezhnyhOts_Prosadka(m1: MASOTS; st, kol, dlinaSmezh: integer);
// massiv, ctepen, kolichestvo, dlina
PROCEDURE OgrSmezhnyhOts_Prosadka_st2(m1: MASOTS; st, kol, dlinaSmezh: integer);
// massiv, ctepen, kolichestvo, dlina

/// ///////////////////
function SiezdFilter(xa, xb: integer): boolean;
PROCEDURE BEDOMOST(TKMTrue: integer);
PROCEDURE WRT_Ogr;
PROCEDURE WRT_RemKorrInf;
procedure SearchSiezd;
function FlagSiezd(a, b: integer): boolean;

PROCEDURE FormirNulevoiShablon1;
PROCEDURE FormirPrPuch;

function GSpeedMtrP(xx: integer): integer;
function GSpeedMtrG(xx: integer): integer;
function RemontPiket(xa, xb, v: integer): boolean;

function CountPut: integer;
procedure wrt_baskalar(ots, prm: string; h: real; m, lng, st, styp, vp, vg, vop,
  vog, flg: integer);
// procedure wrt_baskalar(m:integer; txt:string);
procedure GetV_Remont(ots, xa, xb, vp, vg, otkl: integer);
function KrvFlag(a, b: integer): boolean;
function MultiRadiusCurve(RadMas: masr; n: integer; isShablon: boolean): masr;
// function Vflag(x,v,t:integer):boolean;
function Vflag(x, t: integer): integer;
function VRflag(x, t: integer): integer;
function SVflag(x, v1, v2: integer): string;
// procedure CalcNstKm;

implementation

uses
  // Mainmod,
  REPORTS, RWUn;

const
  sigma = 25;
  tetta = 100;

var
  flgRemont: boolean;

function IndexOfArray(Things: array of integer; Value: integer;
  out index: integer): boolean;
var
  i: integer;
begin
  Result := false;
  index := -1;
  for i := Low(Things) to High(Things) do
    if (Value = Things[i]) then
    begin
      index := i;
      Result := true;
      Break;
    end;
end;

function MultiRadiusCurve(RadMas: masr; n: integer; isShablon: boolean): masr;
var
  isIbigJ: boolean;
  avg_f0rih: real;
  i, j, j2, norma: integer;
begin
  for i := 15 to high(RadMas) - 15 do
  begin
    norma := 0;
    if isShablon then
      norma := s_norm[i];
    if (RadMas[i] - norma = 0) and
      ((RadMas[i - 5] - norma) * (RadMas[i + 5] - norma) > 0) then
    begin
      j := i - 1;
      while ((RadMas[j] - norma <> RadMas[j - 1] - norma) or
        (RadMas[j] - norma = 0)) and (j > 1) do
        j := j - 1;
      avg_f0rih := RadMas[j];
      j2 := j - 1;
      j := i + 1;
      while ((RadMas[j] - norma <> RadMas[j - 1] - norma) or
        (RadMas[j] - norma = 0)) and (j < 1000) do
        j := j + 1;

      if abs(RadMas[j] - norma) < abs(avg_f0rih - norma) then
      begin
        avg_f0rih := RadMas[j];
        j2 := j + 1;
      end;
      isIbigJ := j2 < i;

      while (isIbigJ and (j2 - i < n)) or (not(isIbigJ) and (j2 - i > -n)) do
      begin
        RadMas[j2] := avg_f0rih;
        if (isIbigJ) then
          j2 := j2 + 1
        else
          j2 := j2 - 1;

      end;

    end;

  end;
  Result := RadMas;
end;

procedure rixtovoshnayanit(poy: integer);
var
  i: integer;

  flagprim: boolean;
begin
  with (MainDataModule.fdReadPasport) do
  begin
    // рихтовочная нить
    Close;
    Sql.Clear;
    Sql.Add('SELECT CASE WHEN side_id = 2 THEN 1 ELSE -1 END as side FROM public.apr_straightening_thread as aps');
    Sql.Add('inner join tpl_period as tp on tp.id = aps.period_id');
    Sql.Add('inner join adm_track as atr on atr.id = tp.adm_track_id');
    Sql.Add('WHERE :travel_date BETWEEN tp.START_DATE and tp.FINAL_DATE');
    Sql.Add('and atr.id = :track_id');
    Sql.Add('and :coord between (aps.start_km + aps.start_m/10000) and  (aps.final_km + aps.final_m/10000)');
    ParamByName('travel_date').Value := GlbTripDate;
    ParamByName('track_id').Value := GlbTrackId;
    ParamByName('coord').Value := CoordinateToReal(GlbKmtrue, poy);
    Open;
    while not(eof) do
    begin
      riht_nit := FieldByName('side').AsInteger;
      Next;
    end;
  end;

end;

function KrvFlag(a, b: integer): boolean;
var
  j, sj, x_npk1, x_npk2, a0, b0, ab, c, d, km1, km2, m1, m2: integer;
  tmpval: boolean;
begin
  tmpval := false;

  for j := 0 to high(UKrv) do
  begin
    for sj := 0 to High(UKrv[j].strights) do
    begin
      km1 := UKrv[j].strights[sj].nkm;
      km2 := UKrv[j].strights[sj].kkm;
      m1 := UKrv[j].strights[sj].nmtr;
      m2 := UKrv[j].strights[sj].kmtr;

      if (nstCou > 0) and ((UKrv[j].strights[sj].nkm = GlbKmtrue) or
        (UKrv[j].strights[sj].kkm = GlbKmtrue)) and
        ((UKrv[j].nmtr > 1000) or (UKrv[j].kmtr > 1000)) and
        (UKrv[j].strights[sj].Radius <= 1200) then
      begin
        a0 := a mod 1000;
        b0 := b mod 1000;
        ab := round((a0 + b0) / 2);

        // -1-
        if (km1 <> GlbKmtrue) and (m1 < 1000) and (km2 = GlbKmtrue) and
          (m2 < 1000) and (nstPart = 1) then
        begin
          c := 1;
          d := m2;
        end;
        // -2-
        if (km1 = GlbKmtrue) and (1 <= m1) and (m1 < 1000) and (km2 = GlbKmtrue)
          and (1 <= m2) and (m2 < 1000) and (nstPart = 1) then
        begin
          c := m1;
          d := m2;
        end;
        // -3-
        if (km1 = GlbKmtrue) and (1000 < m1) and (m1 < 2000) and
          (km2 = GlbKmtrue) and (1000 < m2) and (m2 < 2000) and (nstPart = 2)
        then
        begin
          c := m1 - 1000;
          d := m2 - 1000;
        end;
        // -4-1
        if (km1 = GlbKmtrue) and (1 <= m1) and (m1 < 1000) and (km2 = GlbKmtrue)
          and (1000 < m2) and (m2 < 2000) and (nstPart = 1) then
        begin
          c := m1;
          d := 1000;
        end;
        // -4-
        if (km1 = GlbKmtrue) and (1 <= m1) and (m1 < 1000) and (km2 = GlbKmtrue)
          and (1000 < m2) and (m2 < 2000) and (nstPart = 2) then
        begin
          c := 1;
          d := m2 - 1000;
        end;
        // -5-
        if (km1 = GlbKmtrue) and (1000 < m1) and (m1 < 2000) and
          (km2 <> GlbKmtrue) and (1 < m2) and (m2 < 1000) and (nstPart = 2) then
        begin
          c := m1 - 1000;
          d := 1000;
        end;
        // -5-2
        if NKm_(km1) and (km1 <> GlbKmtrue) and (1000 < m1) and (m1 < 2000) and
          (km2 = GlbKmtrue) and (1 < m2) and (m2 < 1000) and (nstPart = 3) then
        begin
          c := 1;
          d := m2;
        end;

        if (c <= ab) and (ab < d) then
        begin
          tmpval := true;
          Break;
        end;
      end
      else
      begin
        x_npk1 := UKrv[j].strights[sj].nkm * 1000 + UKrv[j].strights[sj].nmtr;
        x_npk2 := UKrv[j].strights[sj].kkm * 1000 + UKrv[j].strights[sj].kmtr;

        if (((x_npk1 >= a) and (a >= x_npk2)) or
          ((x_npk1 <= a) and (a <= x_npk2)) or ((x_npk1 >= b) and (b >= x_npk2))
          or ((x_npk1 <= b) and (b <= x_npk2))) and
          (UKrv[j].strights[sj].Radius <= 1200) then
        begin
          tmpval := true;
          Break;
        end;
      end;
    end;

  end;

  KrvFlag := tmpval;
end;

// ----------------------------------------------------------------------
//
// ----------------------------------------------------------------------
function CountPut: integer;
var
  j, cput, s1, s2, s: integer;
  puts: string;
begin
  CountPut := 1;

  for j := 0 to high(UPrm) do
  begin
    puts := UPrm[j].put;
    cput := NPut(puts);

    case cput of
      1:
        s1 := 1;
      2:
        s2 := 1;
    end;
  end;
  s := s1 + s2;
  if s = 2 then
    CountPut := 2;
end;

// ----------------------------------------------------------------------
//
// ----------------------------------------------------------------------
PROCEDURE FormirNulevoiShablon1;
var
  i, j, si, xnorma, xshpal, xskor, ff0, vvv: integer;
  ak, aks, bks, bk, xk: real;
  puts: string;
begin

  for i := 0 to high(F_mtr) do
  begin
    xskor := round(Speed_[i]);
    xnorma := 1520;
    xshpal := -1;
    Shpaly[i] := -1;
    F_Wear[i] := 0;
    xk := CoordinateToReal(GlbKmtrue, F_mtr[i]);

    // На нулевую функцию присваеваем норму шаблона прямого участка
    for j := 0 to high(UPrm) do
    begin
      ak := CoordinateToReal(UPrm[j].nkm, UPrm[j].nmtr);
      bk := CoordinateToReal(UPrm[j].kkm, UPrm[j].kmtr);
      puts := UPrm[j].put;

      if (((ak <= xk) and (xk <= bk)) or ((ak >= xk) and (xk >= bk))) then
      begin
        xnorma := UPrm[j].norma;
        f_norm[i] := xnorma;
        s_norm[i] := xnorma;
        Break;
      end;
    end;

    // На нулевую функцию присваеваем норму шаблона кривого участка

    for j := 0 to high(UKrv) do
    begin
      ak := CoordinateToReal(UKrv[j].nkm, UKrv[j].nmtr);
      bk := CoordinateToReal(UKrv[j].kkm, UKrv[j].kmtr);

      if ((ak <= xk) and (xk <= bk)) or ((ak >= xk) and (xk >= bk)) then
      begin
        for si := 0 to High(UKrv[j].strights) do
        begin
          aks := CoordinateToReal(UKrv[j].strights[si].nkm,
            UKrv[j].strights[si].nmtr);
          bks := CoordinateToReal(UKrv[j].strights[si].kkm,
            UKrv[j].strights[si].kmtr);
          if (((aks <= xk) and (xk <= bks)) or ((aks >= xk) and (xk >= bks)))
          then
          begin
            xnorma := UKrv[j].strights[si].norma_width;
            case xnorma of
              1530 .. 1540:
                xshpal := 2;
            end;
            if (UKrv[j].strights[si].Radius <= 1200) then
              F_Wear[i] := UKrv[j].strights[si].wear;
          end;
        end;
        Shpaly[i] := xshpal;
        f_norm[i] := xnorma;

        // break;
      end;
    end;

    // Желез. бетон. шпалы выпуска до 96 года - 0,
    // после 96 года - 1,
    // дерев. шпалы - 2
    for j := 0 to high(UShp) do
    begin
      ak := CoordinateToReal(UShp[j].nkm, UShp[j].nmtr);
      bk := CoordinateToReal(UShp[j].kkm, UShp[j].kmtr);
      puts := UShp[j].put;

      { if ((odd(numput) and odd(NPut(puts)))
        or (not odd(numput) and not odd(NPut(puts))))
        and }
      if (((ak <= xk) and (xk <= bk)) or ((ak >= xk) and (xk >= bk))) then
      begin
        xshpal := UShp[j].god;
        Shpaly[i] := xshpal;
        {
          if xshpal <> 2 then
          begin
          xnorma:= 1520;
          f_norm[i]:= xnorma;
          end; }
        Break;
      end;
    end;

    // if glbKmTrue = 311 then sablog('xk= ' + inttostr(xk) + '  snorm = '+ inttostr(s_norm[i]));

  end;
end;

// ----------------------------------------------------------------------
PROCEDURE FormirPrPuch;
var
  i, j, xnorma, xshpal, xskor, ff0, vvv: integer;
  ak, bk, xk: real;
  puts: string;
begin

  for i := 0 to high(F_mtr) do
  begin
    xk := CoordinateToReal(GlbKmtrue, F_mtr[i]);
    // На нулевую функцию присваеваем норму шаблона прямого участка
    // sablog(inttostr(glbkmtrue) + inttostr(high(UPuch)));
    for j := 0 to high(UPuch) do
    begin
      ak := CoordinateToReal(UPuch[j].nkm, UPuch[j].nmtr);
      bk := CoordinateToReal(UPuch[j].kkm, UPuch[j].kmtr);
      puts := UPuch[j].put;

      if (((ak <= xk) and (xk <= bk)) or ((ak >= xk) and (xk >= bk))) then
      begin
        if NUMPUT = NPut(puts) then
        begin

          f_puch[i] := abs(UPuch[j].vozv);

        end;
      end;
    end;
  end;

end;

// ------------------------------------------------------------------------------
// instr N611 table N9
// ------------------------------------------------------------------------------
procedure GetV_Remont(ots, xa, xb, vp, vg, otkl: integer);
var
  i, pik, r_v, r_put, r_x0, r_xn: integer;
  r_type, r_pik, otss, ss: string;
  x, rti, otype: integer;
  ok: boolean;
  vop, vog: integer;
begin
  // ots = 1 => 'URV',  ots = 2 => 'PER', ots = 3 => 'RIH', ots = 4 => 'Pro.prab',ots = 4 => 'Pro.leb'
  // xa, xb - coordinatis cm, vp, vg - ust.v pas and gruz, otkl - vel. otkl
  x := round((xa + xb) / 2);
  pik := x div 100 + 1;

  if abs(xa - xb) > 100 then
    pik := xa div 100 + 1;
  vop := -1;
  vog := -1;

  case ots of
    1:
      otss := 'У';
    2:
      otss := 'П';
    3:
      otss := 'Р';
    4:
      otss := 'Пр.п';
    5:
      otss := 'Пр.л';
  end;

  otype := 0;

  case ots of
    1:
      case otkl of
        20:
          otype := 1;
        21 .. 30:
          otype := 2;
        31 .. 40:
          otype := 3;
      end;
    2:
      case otkl of
        16 .. 20:
          otype := 1;
        21 .. 25:
          otype := 2;
        26 .. 30:
          otype := 3;
      end;
    3:
      case otkl of
        25 .. 35:
          otype := 1;
        36 .. 50:
          otype := 2;
        51 .. 65:
          otype := 3;
      end;
    4 .. 5:
      case otkl of
        20 .. 25:
          otype := 4;
        26 .. 30:
          otype := 5;
        31 .. 55:
          otype := 6;
      end;
  end;

  for i := 0 to high(rem) do
  begin
    r_v := rem[i].v;
    r_put := NPut(rem[i].put);
    r_type := rem[i].rtype;

    rti := 0;
    ok := false;
    vop := -1;
    vog := -1;

    if r_type = 'ВПО' then
      rti := 1;
    if r_type = 'ВПО или ВПР' then
      rti := 2;
    if r_type = 'ВПО+ВПР' then
      rti := 3;
    if r_type = 'ВПО+ВПР+ДСП' then
      rti := 4;

    if (rem[i].km = GlbKmtrue) and (rem[i].pik > 0) and (rem[i].pik = pik) and
      (NUMPUT = r_put) then
      ok := true;

    if (rem[i].km = GlbKmtrue) and (rem[i].pik = 0) and (NUMPUT = r_put) then
      ok := true;

    if ok and (strtodate(rem[i].adat) <= strtodate(Dateofinser)) and
      (strtodate(Dateofinser) <= strtodate(rem[i].bdat)) and
      ((vp >= r_v) or (vg >= r_v)) then
    begin

      case otype of
        1:
          case rti of
            1, 3:
              begin
                if (r_v > 60) then
                  vop := 60;
                if (r_v > 60) then
                  vog := 60;
              end;
            2:
              begin
                if (r_v > 50) then
                  vop := 50;
                if (r_v > 50) then
                  vog := 50;
              end;
            4:
              begin
                if (r_v > 70) then
                  vop := 70;
                if (r_v > 70) then
                  vog := 70;
              end;
          end;

        2:
          case rti of
            1, 3:
              begin
                if (r_v > 40) then
                  vop := 40;
                if (r_v > 40) then
                  vog := 40;
              end;
            2:
              begin
                if (r_v > 25) then
                  vop := 25;
                if (r_v > 25) then
                  vog := 25;
              end;
            4:
              begin
                if (r_v > 50) then
                  vop := 50;
                if (r_v > 50) then
                  vog := 50;
              end;
          end;
        3:
          case rti of
            1:
              begin
                if (r_v > 25) then
                  vop := 25;
                if (r_v > 25) then
                  vog := 25;
              end;
            2:
              begin
                if (r_v > 15) then
                  vop := 15;
                if (r_v > 15) then
                  vog := 15;
              end;
            3 .. 4:
              begin
                if (r_v > 25) then
                  vop := 25;
                if (r_v > 25) then
                  vog := 25;
              end;
          end;
        4:
          case rti of
            1 .. 4:
              begin
                if (r_v > 120) then
                  vop := 120;
                if (r_v > 120) then
                  vog := 120;
              end;
          end;
        5:
          case rti of
            1 .. 4:
              begin
                if (r_v > 60) then
                  vop := 60;
                if (r_v > 60) then
                  vog := 60;
              end;
          end;
        6:
          case rti of
            1 .. 4:
              begin
                if (r_v > 0) then
                  vop := 0;
                if (r_v > 0) then
                  vog := 0;
              end;
          end;
      end; // case

      if (vop >= 0) or (vog >= 0) then
      begin
        lng := abs(xa - xb);
        // otss:= otss + ' ' + inttostr(otkl)  + 'мм '+ inttostr(lng) + 'м ' + r_type +' v=' + V_shekti(vop, vog);
        ss := otss + ':' + r_type;
        WRT_UBEDOM(xa, xb, 5, ss, vop, vog);
        wrt_baskalar(ss, '', otkl, xa div 100, lng, 5, 0, vp, vg, vop, vog, 1);
        GlbCountDGR := GlbCountDGR + 1;
      end;
    end; // if
  end;
end;

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
function RemontPiket(xa, xb, v: integer): boolean;
var
  i, pik, r_v, r_put, r_x0, r_xn: integer;
  ret: boolean;
  r_type, r_pik: string;
  x: integer;
begin
  ret := false;
  x := round((xa + xb) / 2);
  pik := ((x div 100) mod 1000) div 100 + 1;

  if abs(xa / 100 - xb / 100) > 100 then
    pik := ((xa div 100) mod 1000) div 100 + 1;

  for i := 0 to high(rem) do
  begin
    r_v := rem[i].v;
    r_put := NPut(rem[i].put);
    // r_type:= rem[i].rtype;
    // r_pik := ' ';

    if (rem[i].km = GlbKmtrue) and (rem[i].pik = pik) and (NUMPUT = r_put) and
      (strtodate(rem[i].adat) <= strtodate(Dateofinser)) and
      (strtodate(Dateofinser) <= strtodate(rem[i].bdat)) and (v < r_v) then
      ret := true;
    { r_x0:= rem[i].km*1000 + (rem[i].pik-1)*100;  r_xn:= rem[i].km*1000 +  rem[i].pik*100;
      r_pik:= ' пк' + inttostr(rem[i].pik); ret:= r_type + r_pik + ' V=' + inttostr(r_v); }
  end;
  RemontPiket := ret;
end;

// ------------------------------------------------------------------------------
// ОПРЕД. СКОРОСТИ В точке
// ------------------------------------------------------------------------------

function SVflag(x, v1, v2: integer): string;
var
  i: integer;
  v11, v12: string;
  flag: boolean;
Begin
  v11 := '-';
  v12 := '-';

  if v1 = -1 then
    v11 := '-';
  if v2 = -1 then
    v12 := '-';

  if IndexOfArray(F_mtr, x, i) then
  begin
    if (0 <= v1) and (v1 < F_V[i]) and (abs(F_mtr[i] - x) <= 1) and
      (not GlbFlagRemontKm) then
    begin
      v11 := inttostr(v1);
      GlbOgrSkorKm := true;
      if (v1 < Glb_vop) and( v1>0 ) then
        Glb_vop := v1;

    end;

    if (0 <= v1) and (v1 < F_Vrp[i]) and (abs(F_mtr[i] - x) <= 1) and GlbFlagRemontKm
    then
    begin
      v11 := inttostr(v1);
      GlbOgrSkorKm := true;
      if (v1 < Glb_vop) then
        Glb_vop := v1;

    end;
    if (0 <= v2) and (F_Vg[i] > v2) and (abs(F_mtr[i] - x) <= 1) and
      (not GlbFlagRemontKm) then
    begin
      v12 := inttostr(v2);
      GlbOgrSkorKm := true;
      if (v2 < Glb_vog) then
        Glb_vog := v2;

    end;

    if (0 <= v2) and (F_Vrg[i] > v2) and (abs(F_mtr[i] - x) <= 1) and GlbFlagRemontKm
    then
    begin
      v12 := inttostr(v2);
      GlbOgrSkorKm := true;
      if (v2 < Glb_vog) then
        Glb_vog := v2;

    end;

  end;

  SVflag := v11 + '/' + v12;

end;

// ------------------------------------------------------------------------------
// ОПРЕД. СКОРОСТИ В точке
// ------------------------------------------------------------------------------
function Vflag(x, t: integer): integer;
var
  i, v: integer;

Begin
  if t = 1 then
    v := GlobPassSkorost;
  if t = 2 then
    v := GlobGruzSkorost;

  for i := 0 to high(F_mtr) do
  begin
    if (t = 1) and (abs(F_mtr[i] - x) <= 1) then
    begin
      v := F_V[i];
      Break;
    end;

    // if (t = 1) and (abs(F_mtr[i] - x) <= 1)
    // and GlbFlagRemontKm then begin v:= F_Vrp[i]; break; end;

    if (t = 2) and (abs(F_mtr[i] - x) <= 1)
    // and (not GlbFlagRemontKm)
    then
    begin
      v := F_Vg[i];
      Break;
    end;

    // if (t = 2) and (abs(F_mtr[i] - x) <= 1)
    // and GlbFlagRemontKm then begin v:= F_Vrg[i]; break; end;

  end;
  Vflag := v;
end;

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
function VRflag(x, t: integer): integer;
var
  i, v: integer;

Begin
  if t = 1 then
    v := RGlobPassSkorost;
  if t = 2 then
    v := RGlobGruzSkorost;

  for i := 0 to high(F_mtr) do
  begin

    if (t = 1) and (abs(F_mtr[i] - x) <= 1) and GlbFlagRemontKm then
    begin
      v := F_Vrp[i];
      Break;
    end;

    if (t = 2) and (abs(F_mtr[i] - x) <= 1) and GlbFlagRemontKm then
    begin
      v := F_Vrg[i];
      Break;
    end;

  end;
  VRflag := v;
end;

// ------------------------------------------------------------------------------
// ОПРЕД. СКОРОСТИ В КМ
// ------------------------------------------------------------------------------
procedure GSpeedKM;
var
  SA, SB, j: integer;
  xskor, vvv, vvg, i, rem_ind, ff0: integer;
  r_x0, r_xn, xk, ak, bk: real;
  puts: string;
  r_put, r_v, cpikrem, pikrv, remcount: integer;
  r_pik, r_type, sumpik: string;
  mxVp, mxVg: integer;
BEGIN
  if Flag_sablog then
    SabLog('GSpeedKM - определения скорсти на км.');
  mxVp := 0;
  mxVg := 0;

  GlbFlagRemontKm := false;

  GlobPassSkorost := -1; // 80;
  GlobGruzSkorost := -1;

  if (NUMPUT > 2) then
    for i := 0 to high(F_mtr) do
    begin
      F_V[i] := 40;
      F_Vg[i] := 40;
      F_Vrp[i] := 40;
      F_Vrg[i] := 40;
    end;

  if (NUMPUT > 2) and (GlobPassSkorost = -1) and (GlobGruzSkorost = -1) then
  begin
    GlobPassSkorost := 40;
    GlobGruzSkorost := 40;

    RGlobPassSkorost := 40;
    RGlobGruzSkorost := 40;
  end;

  for j := 0 to High(USkr) do
  begin
    SA := USkr[j].nkm;
    SB := USkr[j].kkm;
    puts := USkr[j].put;

    if (((SA <= GlbKmtrue) and (GlbKmtrue <= SB)) or
      ((SA >= GlbKmtrue) and (GlbKmtrue >= SB)))  then
    begin { if (odd(NPut(puts)) and odd(numput)) or (not odd(NPut(puts)) and not odd(numput)) then }
      GlobPassSkorost := USkr[j].skp;
      GlobGruzSkorost := USkr[j].skg;
      RGlobPassSkorost := GlobPassSkorost;
      RGlobGruzSkorost := GlobGruzSkorost;

      if mxVp < GlobPassSkorost then
        mxVp := GlobPassSkorost;
      if mxVg < GlobGruzSkorost then
        mxVg := GlobGruzSkorost;
    end;
  end;

  if mxVp < GlobPassSkorost then
    mxVp := GlobPassSkorost;
  if mxVg < GlobGruzSkorost then
    mxVg := GlobGruzSkorost;
  { GlobPassSkorost:= 120; GlobGruzSkorost:= 80; }

  // Установленный скорость
  for j := 0 to high(USkr) do
  begin
    ak := CoordinateToReal(USkr[j].nkm, USkr[j].nmtr);
    bk := CoordinateToReal(USkr[j].kkm, USkr[j].kmtr);
    puts := USkr[j].put;
    vvv := USkr[j].skp;
    vvg := USkr[j].skg;

    for i := 0 to high(F_mtr) do
    begin
      xk := CoordinateToReal(GlbKmtrue, F_mtr[i]);

      if (((ak <= xk) and (xk <= bk)) or ((ak >= xk) and (xk >= bk))) then
      begin
        F_V[i] := vvv;
        F_Vg[i] := vvg;
        F_Vrp[i] := vvv;
        F_Vrg[i] := vvg;
      end;
    end;
  end;

  // Remont:
  r_pik := ' ';
  sumpik := ' пк';
  cpikrem := 0;
  remcount := 0;
  for i := 0 to high(rem) do
  begin

    r_v := rem[i].v;
    r_put := NPut(rem[i].put);
    r_type := rem[i].rtype;
    r_pik := ' ';

    if (rem[i].km = GlbKmtrue) and (NUMPUT = r_put) and
      (strtodate(rem[i].adat) <= strtodate(Dateofinser)) and
      (strtodate(Dateofinser) <= strtodate(rem[i].bdat)) then
    begin
      GlbFlagRemontKm := true;

      RemTab9 := 0;
      if (r_type = 'ВПО') then
        RemTab9 := 1;
      if (r_type = 'ВПО или ВПР') then
        RemTab9 := 2;
      if (r_type = 'ВПО+ВПР') then
        RemTab9 := 3;
      if (r_type = 'ВПО+ВПР+ДСП') then
        RemTab9 := 4;

      if (rem[i].pik > 0) then
      begin
        r_x0 := CoordinateToReal(rem[i].km, (rem[i].pik - 1) * 100);
        r_xn := CoordinateToReal(rem[i].km, rem[i].pik * 100);
        sumpik := sumpik + inttostr(rem[i].pik) + ',';
        pikrv := r_v;
        cpikrem := cpikrem + 1;

        for j := 0 to high(F_mtr) do
        begin
          xk := CoordinateToReal(GlbKmtrue, round(F_mtr[j]));
          if ((r_x0 <= xk) and (xk <= r_xn)) or ((r_x0 >= xk) and (xk >= r_xn))
          then
          begin
            F_Vrp[j] := r_v;
            F_Vrg[j] := r_v;
          end;
        end;

      end
      else if (rem[i].pik = 0) then
      begin
        r_x0 := CoordinateToReal(rem[i].km * 1000, 0);
        r_xn := CoordinateToReal(rem[i].km + 1, 0);
        GlbTempTipRemontKm := r_type + ' V=' + inttostr(r_v) + ' ';
        // GlobPassSkorost:= r_v;  // 04.12.2012
        // GlobGruzSkorost:= r_v;  // 04.12.2012 remontta ust V ozgermeidi
        RGlobPassSkorost := r_v;
        RGlobGruzSkorost := r_v;

        for j := 0 to high(F_mtr) do
        begin
          F_Vrp[j] := r_v;
          F_Vrg[j] := r_v;
        end;
      end;

      remcount := remcount + 1;
    end;
  end;

  if remcount > 0 then
    GlbCountRemKm := GlbCountRemKm + 1;

  if cpikrem >= 1 then
  begin
    for rem_ind := 0 to high(rem) do
    begin
      if (GlbKmtrue = rem[rem_ind].km) then
      begin
        r_type := rem[rem_ind].rtype;
        Break;
      end;
    end;
    GlbTempTipRemontKm := r_type + sumpik + ' V=' + inttostr(pikrv) + ' ';
  end;

  if mxVp < GlobPassSkorost then
    mxVp := GlobPassSkorost;
  if mxVg < GlobGruzSkorost then
    mxVg := GlobGruzSkorost;

  GlobPassSkorost := mxVp; // ;
  GlobGruzSkorost := mxVg; // ;

  if GlobPassSkorost = -1 then
    GlobPassSkorost := 80;
  if GlobGruzSkorost = -1 then
    GlobGruzSkorost := 70;

  // GlbSkorostRemontKm:= GlobPassSkorost;
END;

// ------------------------------------------------------------------------------
// ОПРЕД. СКОРОСТИ В КМ
// ------------------------------------------------------------------------------
function GSpeedMtrP(xx: integer): integer;
var
  SA, SB, j, vv: integer;
  puts: string;
BEGIN
  // SabLog('GSpeedKM - определения скорсти на км.');
  vv := 100;

  for j := 0 to High(USkr) do
  begin
    SA := USkr[j].nkm * 1000 + USkr[j].nmtr;
    SB := USkr[j].kkm * 1000 + USkr[j].kmtr;
    puts := USkr[j].put;

    if (((SA <= xx) and (xx <= SB)) or ((SA >= xx) and (xx >= SB))) and
      (NUMPUT = NPut(puts)) then
    begin
      vv := USkr[j].skp;
      Break;
    end;
  end;

  if (NUMPUT > 2) and (vv = 100) then
    vv := 40;

  GSpeedMtrP := vv;
END;

// ------------------------------------------------------------------------------
// ОПРЕД. СКОРОСТИ В КМ
// ------------------------------------------------------------------------------
function GSpeedMtrG(xx: integer): integer;
var
  SA, SB, j, vv: integer;
  puts: string;
BEGIN
  // SabLog('GSpeedKM - определения скорсти на км.');
  vv := 80;

  for j := 0 to High(USkr) do
  begin
    SA := USkr[j].nkm * 1000 + USkr[j].nmtr;
    SB := USkr[j].kkm * 1000 + USkr[j].kmtr;
    puts := USkr[j].put;

    if (((SA <= xx) and (xx <= SB)) or ((SA >= xx) and (xx >= SB))) and
      (NUMPUT = NPut(puts)) then
    begin
      vv := USkr[j].skg;
      Break;
    end;
  end;
  if (NUMPUT > 2) and (vv = 80) then
    vv := 40;
  GSpeedMtrG := vv;
END;

// -----------------------------------------------------------------------
//
// -----------------------------------------------------------------------
function LengthKm: real;
var
  i: integer;
  xy: real;
  puts: string;
begin
  Result := 1.0;

  for i := 0 to high(UNst) do
  begin
    Result := UNst[i].dlina / 1000.0;
    Break;
  end;
  if abs(Result - Length_Km) > 0.05 then
    Result := Length_Km;
  GlobCountObrKm := GlobCountObrKm + Result;

end;

// -----------------------------------------------------------------------
//
// -----------------------------------------------------------------------

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
function GV_Urb: integer;
var
  x: integer;
begin
  x := 21;
  case GlobPassSkorost of
    121 .. 140:
      x := 21;
    61 .. 120:
      x := 26;
    41 .. 60:
      x := 31;
    16 .. 40:
      x := 37;
    0 .. 15:
      x := 53;
  end;
  GV_Urb := x;
end;

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
function GV_Pro: integer;
var
  x: integer;
begin
  x := 21;
  case GlobPassSkorost of
    121 .. 140:
      x := 21;
    61 .. 120:
      x := 27;
    41 .. 60:
      x := 31;
    16 .. 40:
      x := 37;
    0 .. 15:
      x := 47;
  end;
  GV_Pro := x;
end;

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
function GV_Per: integer;
var
  x: integer;
begin
  x := 19;
  case GlobPassSkorost of
    121 .. 140:
      x := 19;
    61 .. 120:
      x := 21;
    41 .. 60:
      x := 27;
    16 .. 40:
      x := 33;
    0 .. 15:
      x := 53;
  end;
  GV_Per := x;
end;

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
function GV_Rih: integer;
var
  x: integer;
begin
  x := 17;
  case GlobPassSkorost of
    121 .. 140:
      x := 17;
    81 .. 120:
      x := 23;
    61 .. 80:
      x := 26;
    41 .. 60:
      x := 32;
    16 .. 40:
      x := 41;
    0 .. 15:
      x := 63;
  end;
  GV_Rih := x;
end;

// ================
function GV_Rih_tab3p3(L: integer): integer;
var
  x, LL: integer;
begin
  x := 26;
  LL := round(L / 100);

  case LL of
    0 .. 20:
      case GlobPassSkorost of
        121 .. 140:
          x := 26;
        61 .. 120:
          x := 36;
        41 .. 60:
          x := 41;
        16 .. 40:
          x := 51;
        0 .. 15:
          x := 66;
      end;
    21 .. 40:
      case GlobPassSkorost of
        121 .. 140:
          x := 36;
        61 .. 120:
          x := 41;
        41 .. 60:
          x := 51;
        16 .. 40:
          x := 66;
        0 .. 15:
          x := 91;
      end;
  end;
  GV_Rih_tab3p3 := x;
end;

// ------------------------------------------------------------------------------
// Жылдамдыкты бір саты томендету
// ------------------------------------------------------------------------------
function GetV(pv, uv: integer): integer;
var
  v: integer;
begin
  v := -1;
  case pv of
    0:
      case uv of
        121 .. 250:
          v := 120;
        61 .. 120:
          v := 60;
        41 .. 60:
          v := 40;
        16 .. 40:
          v := 15;
        0 .. 15:
          v := 0;
      else
        v := -1;
      end;
    1:
      case uv of
        81 .. 250:
          v := 80;
        61 .. 80:
          v := 60;
        41 .. 60:
          v := 40;
        16 .. 40:
          v := 15;
        0 .. 15:
          v := 0;
      else
        v := -1;
      end;
  end;

  GetV := v;
end;

// ------------------------------------------------------------------------------
// ОБРАБОТКА УЧАСТКА МОСТЫ   //030406
// ------------------------------------------------------------------------------
PROCEDURE Most_Tonnel(var PrmOts: MASOTS; name_ots: string);
const
  L1 = 200; // metr
  L2 = 500; // metr
var
  im0, xkord, ykord, ortasy: integer;
  shart, fstand: boolean;
  ab: integer;
  uch_a, uch_b, xy: real;
  xL0, xLm, j, dlina, Ln: integer;
  stv, belv, L0v, Lmv, i, ogranichenie1, xxxx, v1, v2, u1, u2, vr, vrg,
    coor: integer;
  ots: string;
BEGIN
  TRY
    if Flag_sablog then
      SabLog('Most_Tonnell - ОБРАБОТКА УЧАСТКА МОСТЫ И ТОННЕЛИ');

    for j := 0 to High(UMT) do
    begin

      if (((UMT[j].nkm <= GlbKmtrue) and (GlbKmtrue <= UMT[j].kkm)) or
        ((UMT[j].nkm >= GlbKmtrue) and (GlbKmtrue >= UMT[j].kkm))) then
      begin
        shart := false;
        // ------------------------------------------------------------------------------
        if (UMT[j].nmtr < GlbKmLength) and (UMT[j].kmtr < GlbKmLength) then
        begin
          uch_a := CoordinateToReal(UMT[j].nkm, UMT[j].nmtr);
          uch_b := CoordinateToReal(UMT[j].kkm, UMT[j].kmtr);
          // ab := GetCoordByLen(UMT[j].nkm,UMT[j].nmtr, GetDistanceBetween(UMT[j].nkm,UMT[j].nmtr,UMT[j].kkm, UMT[j].kmtr, GlbTrackId, GlbTripDate));

          // uch_a := ab - round(UMT[j].dl / 2);
          // uch_b := ab + round(UMT[j].dl / 2);

          if (26 <= UMT[j].dl) and (UMT[j].dl <= 100) then
          begin

            uch_a := GetCoordByLen(UMT[j].nkm, UMT[j].nmtr, -L1, GlbTrackId,
              GlbTripDate);
            uch_b := GetCoordByLen(UMT[j].kkm, UMT[j].kmtr, L1, GlbTrackId,
              GlbTripDate);
          end
          else

            if (100 < UMT[j].dl) and (UMT[j].dl < GlbKmLength) then
          begin
            uch_a := GetCoordByLen(UMT[j].nkm, UMT[j].nmtr, -L1, GlbTrackId,
              GlbTripDate);
            uch_b := GetCoordByLen(UMT[j].kkm, UMT[j].kmtr, L1, GlbTrackId,
              GlbTripDate);
          end;
          fstand := false;
        end
        else
        begin
          uch_a := CoordinateToReal(UMT[j].nkm, UMT[j].nmtr);
          uch_b := CoordinateToReal(UMT[j].kkm, UMT[j].kmtr);
          fstand := true;
        end;
        // ------------------------------------------------------------------------------
        for im0 := 0 to High(PrmOts) do
        begin
          xL0 := PrmOts[im0].L0;
          xLm := PrmOts[im0].Lm;

          Ln := round(abs(xL0 - xLm));

          xkord := xL0; //
          ykord := xLm; //

          ortasy := round((xL0 + xLm) / 2);
          xy := CoordinateToReal(GlbKmtrue, ortasy);

          belv := PrmOts[im0].bel;
          L0v := PrmOts[im0].L0;
          Lmv := PrmOts[im0].Lm;
          u1 := PrmOts[im0].v;
          u2 := PrmOts[im0].vg;
          vr := PrmOts[im0].vrp;
          vrg := PrmOts[im0].vrg;
          // xxxx:= (((L0v+Lmv) div 200) mod 1000) div 100 + 1;
          xxxx := (L0v div 100) + 1;
          shart := (((uch_a <= xy) and (xy <= uch_b)) or
            ((uch_a >= xy) and (xy >= uch_b)));

          if shart and (PrmOts[im0].st = 3) and (PrmOts[im0].prim = '') then
          begin
            PrmOts[im0].st := 3;
            PrmOts[im0].prim := PrmOts[im0].prim;
            v1 := GetV(0, u1);
            v2 := -1;
            if (v1 < u2) then
            begin
              v2 := GetV(1, u2);

            end;
            if (v1 < u1) then
            begin
              PrmOts[im0].vop := v1;
              PrmOts[im0].vog := v2;
            end;

            coor := round((L0v + Lmv) / 200);
            ots := 'V=' + inttostr(v1) + '/' + inttostr(v2) + ' пк' +
              inttostr(xxxx) + ' ' + name_ots + ' ' + inttostr(belv) + '/' +
              inttostr(Ln) + ' м; ';
            WRT_UBEDOM(L0v, Lmv, 7, ots, v1, v2);
            wrt_baskalar(name_ots, 'м;', belv, ortasy, Ln, 3, 0, u1, u2,
              v1, v2, 0);
            GlbCountDGR := GlbCountDGR + 1;
            shart := false;
            { end; }
          end;
        end; // for
      end; // if
    end; // for

  EXCEPT
  END;
END;

// ---------------------------------------------------------------------------
function KrivoiUch(pmtr: integer): boolean;
// Определение кривого участка по паспортным данным
var
  j, a, b, x: integer;
begin
  try
    KrivoiUch := false;
    GlbAmtr := 0;
    GlbBmtr := 0;
    GlbLnk := 0;
    GlbVoz := 0;
    GlbNrs := 0;
    GlbLpk1 := 0;
    GlbLpk2 := 0;
    GlbNrm := 0;
    GlbIzn := 0;
    GlbRad := 0;

    GlbPT1 := '';
    GlbPT2 := '';
    GlbPT3 := '';
    GlbPT4 := '';
    GlbPT5 := '';
    GlbPT6 := '';
    GlbPT7 := '';
    GlbPT8 := '';
    GlbPT9 := '';
    GlbPT10 := '';

    for j := 0 to High(UKrv) do
    begin
      a := UKrv[j].nkm * 1000 + UKrv[j].nmtr;
      b := UKrv[j].kkm * 1000 + UKrv[j].kmtr;
      x := TekKmTrue * 1000 + pmtr;

      if (((a <= x) and (x <= b)) or ((a >= x) and (x >= b))) then
      begin
        GlbAmtr := UKrv[j].nmtr;
        GlbBmtr := UKrv[j].kmtr;
        { GlbLnk:= UKrv[j].Lnk; } GlbVoz := UKrv[j].levels[0].level;
        { GlbNrs:= UKrv[j].nrs; } GlbLpk1 := UKrv[j].strights[0].L1;
        GlbLpk2 := UKrv[j].strights[0].L2;
        GlbNrm := UKrv[j].strights[0].norma_width;
        GlbIzn := UKrv[j].strights[0].wear;
        GlbRad := UKrv[j].strights[0].Radius;
        GlbKrvCenter := (GlbAmtr + GlbBmtr) div 2;

        GlbInfKrivoi := inttostr(GlbAmtr) +
        // ' Дл.: ' + Inttostr(GlbLnk)  +
          ' R: ' + inttostr(GlbRad) + ' h: ' + inttostr(GlbVoz) +
        // ' ПК1: ' + inttostr(GlbLpk1)  +
        // ' ПК2: ' + inttostr(GlbLpk2)  +
          ' Ш: ' + inttostr(GlbNrm) + ' И: ' + inttostr(GlbIzn);

        case GlbKrvCenter of
          0 .. 100:
            GlbPT1 := GlbInfKrivoi;
          101 .. 200:
            GlbPT2 := GlbInfKrivoi;
          201 .. 300:
            GlbPT3 := GlbInfKrivoi;
          301 .. 400:
            GlbPT4 := GlbInfKrivoi;
          401 .. 500:
            GlbPT5 := GlbInfKrivoi;
          501 .. 600:
            GlbPT6 := GlbInfKrivoi;
          601 .. 700:
            GlbPT7 := GlbInfKrivoi;
          701 .. 800:
            GlbPT8 := GlbInfKrivoi;
          801 .. 900:
            GlbPT9 := GlbInfKrivoi;
          901 .. 1000:
            GlbPT10 := GlbInfKrivoi;
        end;

        KrivoiUch := true;
        Break;
      end;
    end;
  except
  end;
end;

// ==============================================================================
procedure KrivoiNatur(kilometr: integer);
// Определение кривого участка по натурным данным
var
  i, j, k, ipt, metr: integer;
  val, h1, h2, r1, r2: real;

begin
  // Данные из БПД
  GlbKrvEnable := false;

  kv := nil;

  k := 0;
  for i := 0 to high(UKrv) do
  begin
    if (UKrv[i].nkm = GlbKmtrue) then
    begin
      for j := 0 to High(UKrv[i].strights) do
      begin
        GlbKrvEnable := true;
        Setlength(kv, k + 1);
        kv[k].km := UKrv[i].nkm;
        kv[k].put := NPut(UKrv[i].put);
        metr := UKrv[i].nmtr;
        ipt := metr div 100 + 1;
        kv[k].piket_krv := ipt;
        kv[k].nach_krv := metr;
        kv[k].Fsr_Urob := UKrv[i].levels[j].level;
        kv[k].Fsr_Shab := UKrv[i].strights[j].norma_width; // 1520;
        kv[k].Radius := UKrv[i].strights[j].Radius;
        kv[k].wear := UKrv[i].strights[j].wear;
        k := k + 1;
      end;

    end;
  end;

end;

// ----------------------------------------------------------------------
// Определение существования кривого                                  //
// ----------------------------------------------------------------------
function Opr_sush_Krivoi(a, b: integer; h: real): boolean;
const
  Hf = 7; // ищем кривой если высота рихтовки 7 мм
  Lf = 100; // минимальная длина кривой который ищем

var
  i, xkx, k: integer;
  Hk, sk, s_max, Hp, Lp: real;
begin
  Opr_sush_Krivoi := false;

  Hp := h;

  if (h = 0) then
    Hp := 3.5;

  // hp:= 3.5;
  sk := 0;
  k := 0;
  for i := 0 to high(Fsr_rh1) do
  begin
    if flagpic then
      xkx := GlbKmtrue * 1000 + round(F_mtr[i]) + ppp * 100
    else
      xkx := GlbKmtrue * 1000 + round(F_mtr[i]);

    if (a <= xkx) and (xkx <= b) then
    begin
      Hk := abs(Fsr_rh1[i]);
      sk := sk + Hk;
      k := k + 1;
    end;

  end; // for

  s_max := Hp * k;

  if (sk > s_max) then
  begin
    Opr_sush_Krivoi := true;
    // SabLog('Opred_Krivoi - Найдено кривая: ' +GlbInfKrivoi);
  end;

end;

// ==============================================================================

// ==============================================================================
function NKm_leng(pkm: integer): integer;
// d-pereh.kriv, num- 1,2,3,4 pereh. kriv
var
  i, put, lng, kmn, npch: integer;
  ret: integer;
begin
  ret := 1000;
  for i := 0 to High(UNst) do
  begin
    put := NPut(UNst[i].put);
    lng := UNst[i].dlina;
    kmn := UNst[i].km;
    npch := UNst[i].pch;

    if (pkm = kmn) and (Glb_PutList_PCH = npch) and (NUMPUT = put) and
      (lng > 1000) then
    begin
      ret := lng;
      Break;
    end;
  end;
  NKm_leng := ret;
end;

// ==============================================================================
function NKm_(pkm: integer): boolean; // d-pereh.kriv, num- 1,2,3,4 pereh. kriv
var
  i, put, lng, kmn, npch: integer;
  ret: boolean;
begin
  ret := false;
  for i := 0 to High(UNst) do
  begin
    // put := NPut(UNst[i].put);
    lng := UNst[i].dlina;
    kmn := UNst[i].km;
    // npch := UNst[i].pch;

    if (pkm = kmn) and (lng > 1000) then
    begin
      ret := true;
      Break;
    end;
  end;
  NKm_ := ret;
end;

// ==============================================================================

// ==============================================================================
function Point(xk, xm, npk1, npm1, npk2, npm2, d1, d2, h, nrm, ntkm,
  ntd: real): real;
var
  y, delta1, delta2, deltaN, h1, h2: real;
  kpk1, kpm1, kpm2, kpk2: real;
begin
  y := nrm;

  deltaN := abs(h - nrm);
  // delta1:= xm - npm1;  delta2:= npm2 - xm;
  h1 := 0;
  h2 := 0;

  if d1 <> 0 then
    h1 := deltaN / d1;
  if d2 <> 0 then
    h2 := deltaN / d2;

  kpk1 := Int((npk1 * 1000 + npm1 + d1) / 1000);
  kpm1 := round(npk1 * 1000 + npm1 + d1) mod 1000;

  kpk2 := Int((npk2 * 1000 + npm2 - d2) / 1000);
  kpm2 := round(npk2 * 1000 + npm2 - d2) mod 1000;

  if (ntkm = npk1) and (npm1 + d1 < ntd) then
  begin
    kpk1 := npk1;
    kpm1 := npm1 + d1;
  end;

  if (ntkm = kpk2) and (ntkm <> npk2) and (d2 > npm2) then
  begin
    kpk2 := npk1;
    kpm2 := ntd - d2 - npm2;
  end;
  // -1-------------------------------------------------------------------
  if (xk = npk1) and (npm1 <= xm) and (xm - npm1 <= d1) then
    y := nrm + h1 * abs(xm - npm1);
  // if (xk < npk1) and (kpk1 = xk) and (1 <= xm) and (xm <= kpm1)
  // -2-------------------------------------------------------------------
  if (xk = kpk1) and (xk = kpk2) and (kpm1 <= xm) and (xm <= kpm2) then
    y := h;

  if (xk = kpk1) and (xk <> kpk2) and (kpm1 <= xm) and
    (((xk <> ntkm) and (xm <= 1000)) or ((xk = ntkm) and (xm <= ntd))) then
    y := h;
  // if (xk <> kpk1) and (xk = kpk2)
  // -3-------------------------------------------------------------------
  if (xk = kpk2) and (xk = npk2) and (kpm2 <= xm) and (xm <= npm2) then
    y := nrm + h2 * abs(xm - npm2);

  if (xk = kpk2) and (xk <> npk2) and
    (((ntkm <> kpk2) and (kpm2 <= xm) and (xm <= 1000)) or
    ((ntkm = kpk2) and (kpm2 <= xm) and (xm <= ntd))) then
    y := nrm + h2 * abs(xm - kpm1);

  // if (xk <> kpk2) and (xk = npk2)
  // --------------------------------------------------------------------
  Point := y;
end;
// ==============================================================================
{ procedure Get_NullFunc;
  var
  i, j, l,jj:integer;
  xkx, x_npk1, x_kpk1, x_npk2, x_kpk2, xx, rad :real;   //
  yxu, yxr01, yxr02, zxu, fsr, fsrk, fsr2, fsrs ,max, znak, gg1, gg2:real;
  s,s1,k,nrm, xxk:real;
  bool1, continue_flag, krv:boolean;
  xput :integer;
  kk,ss,xsd, dd, kmd, ww, a,b,kmn, npch, put,lng, xsd1, xsd2 :integer;
  flag, nstFlag:boolean;
  kpk1,kpk2,npm1,npm2,kpm1,kpm2,d1,d2,h, h1,h2,xm,y:real;
  begin

  znak:= 1;
  krv:= false;

  NstFlag:= false;

  for i := 0 to High(UNst) do
  begin
  put:= NPut(UNst[i].put);
  lng:= UNst[i].dlina;
  kmn:= UNst[i].km;
  npch:= UNst[i].pch;
  if (GlbKmTrue = kmn) and (Glb_PutList_PCH = npch)
  and (NumPut = put) and (lng > 1000) then   NstFlag:= true;

  end;

  for i:= 0 to high(x_k) do
  begin
  if flagpic then
  xkx:= glbKmTrue*1000 + round(F_mtr[i])+ppp*100 else
  xkx:= glbKmTrue*1000 + round(F_mtr[i]);

  //   kmn:= Fn0[i].m;
  xm:= Fn0[i].m;

  yxu:= 0; yxr01:= 0; yxr02:= 0;  nrm:= 1520;

  if (1524 <= f_norm[i]) and (f_norm[i] <= 1529) then nrm:= 1524;
  zxu:= nrm;
  y:= nrm;

  rad:= 10000; ss:= 0; kk:= 0; xsd:= 0; flag:= false;

  for j:= 0 to high(UKrv) do
  begin

  if (NPut(UKrv[j].put) = NUMPUT ) then
  begin

  x_npk1:= UKrv[j].nkm*1000 + UKrv[j].nmtr;
  x_kpk1:= UKrv[j].nkm*1000 + UKrv[j].nmtr + UKrv[j].d1;
  x_npk2:= UKrv[j].kkm*1000 + UKrv[j].kmtr;
  x_kpk2:= UKrv[j].kkm*1000 + UKrv[j].kmtr - UKrv[j].d2;

  fsr:= Fsred_krivoi(UKrv[j].rad);
  fsrs:=  abs(UKrv[j].norm - nrm); //1520;

  //  delta1:= xm - npm1;  delta2:= npm2 - xm;
  d1:= UKrv[j].d1;
  d2:= UKrv[j].d2;
  h1:= 0;   h2:= 0;     nrm :=1520;

  if d1 <> 0 then h1:= fsrs/d1;
  if d2 <> 0 then h2:= fsrs/d2;

  npk1:= UKrv[j].nkm; npk2:= UKrv[j].kkm;
  npm1:= UKrv[j].nmtr; npm2:= UKrv[j].kmtr;


  kpk1 := Int((npk1*1000 + npm1 + d1)/1000);
  kpm1 := round(npk1*1000 + npm1 + d1) mod 1000;

  kpk2 := Int((npk2*1000 + npm2 - d2)/1000);
  kpm2 := round(npk2*1000 + npm2 - d2) mod 1000;

  if (kmn = npk1) and (npm1 + d1 < lng) then
  begin
  kpk1 := npk1;
  kpm1 := npm1 + d1;
  end;

  if (kmn = kpk2) and (kmn <> npk2) and (d2 > npm2) then
  begin
  kpk2 := npk1;
  kpm2 := lng - d2 - npm2;
  end;
  //-1-------------------------------------------------------------------
  if (glbKmTrue = npk1) and (npm1 <= xm) and (xm-npm1 <= d1) then y:= nrm + h1*abs(xm-npm1);
  //if (xk < npk1) and (kpk1 = xk) and (1 <= xm) and (xm <= kpm1)
  //-2-------------------------------------------------------------------
  if (glbKmTrue = kpk1) and (glbKmTrue = kpk2)
  and (kpm1 <= xm) and (xm <= kpm2) then  y:= UKrv[j].norm;

  if (glbKmTrue = kpk1) and (glbKmTrue <> kpk2) and (kpm1 <= xm)
  and (
  ((glbKmTrue <> kmn) and (xm <= 1000))
  or ((glbKmTrue = kmn) and (xm <= lng))
  ) then  y:= UKrv[j].norm;
  // if (xk <> kpk1) and (xk = kpk2)
  //-3-------------------------------------------------------------------
  if (glbKmTrue = kpk2) and (glbKmTrue = npk2)
  and (kpm2 <= xm) and (xm <= npm2) then y:= nrm + h2*abs(xm-npm2);

  if (glbKmTrue = kpk2) and (glbKmTrue <> npk2)
  and (
  ((kmn <> kpk2) and (kpm2 <= xm) and (xm <= 1000))
  or ((kmn = kpk2) and (kpm2 <= xm) and (xm <= lng))
  )
  then y:= nrm + h2*abs(xm-kpm1);

  //    if (xk <> kpk2) and (xk = npk2)
  //--------------------------------------------------------------------


  if (x_npk1 > xkx) or (xkx > x_npk2) then  flag_dlya_Krivoi:= false;

  if ((x_npk1 <= xkx) and (xkx <= x_npk2))
  or ((x_npk1 >= xkx) and (xkx >= x_npk2))
  then
  begin
  flag_dlya_Krivoi:= true;
  GlbKrvZnak:= round(gznak(round(x_npk1),round(x_npk2)));
  rad:=  UKrv[j].rad;
  //--------------------------
  xxk:= round(xkx) mod 1000;
  npk1:= UKrv[j].nkm;
  npm1:= UKrv[j].nmtr;
  npk2:= UKrv[j].kkm;
  npm2:= UKrv[j].kmtr;
  d1:= UKrv[j].d1;
  d2:= UKrv[j].d2;
  h:= UKrv[j].norm;

  zxu:= abs(fsrs);
  //  delta1:= xm - npm1;  delta2:= npm2 - xm;
  h1:= 0;   h2:= 0;

  if d1 <> 0 then h1:= zxu/d1;
  if d2 <> 0 then h2:= zxu/d2;

  //--------------------------
  end;

  gg1:= abs(x_npk1 - x_kpk1);
  gg2:= abs(x_npk2 - x_kpk2);

  if gg1 = 0 then gg1 := 30;
  if gg2 = 0 then gg2 := 30;

  // нач. перех. крив.
  if (x_npk1 <= xkx) and (xkx <= x_kpk1) and (gg1 <> 0) then
  begin
  znak:= gznak(x_npk1,x_npk2);

  yxu:= UKrv[j].voz/gg1;          //abs(x_npk1 - x_kpk1); // dlia urovnia
  yxu:= yxu*(xkx - x_npk1); //(

  yxr01:= fsr/gg1;              //abs(x_npk1 - x_kpk1);  // dlia rihtovki
  yxr01:= yxr01*(xkx - x_npk1); //

  yxr02:= fsr/gg1;              //abs(x_npk1 - x_kpk1);  // dlia rihtovki
  yxr02:= yxr02*(xkx - x_npk1); //

  //zxu:= fsrs/gg1;
  zxu:= nrm + h1*abs(xkx - x_npk1);//zxu*(xkx - x_npk1);
  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end
  // середина кривой
  else
  if (x_kpk1 < xkx) and (xkx < x_kpk2) then
  begin
  znak:= gznak(x_npk1,x_npk2);

  yxu:= UKrv[j].voz;
  yxr01:= fsr;
  yxr02:= fsr;
  zxu:= UKrv[j].norm;

  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end
  else
  // кон. перех. крив
  if (x_kpk2 <= xkx) and (xkx <= x_npk2) and (gg2 <> 0) then
  begin
  znak:= gznak(x_npk1,x_npk2);

  yxu:= UKrv[j].voz/gg2;                         //abs(x_npk2 - x_kpk2);
  yxu:= yxu*(x_npk2 - xkx);

  yxr01:= fsr/gg2;                               //abs(x_npk2 - x_kpk2);
  yxr01:= yxr01*(x_npk2 - xkx);

  yxr02:= fsr/gg2;                               //abs(x_npk2 - x_kpk2);
  yxr02:= yxr02*(x_npk2 - xkx);

  //zxu:= fsrs/gg2;
  zxu:= nrm + h2*abs(xkx-x_npk2);//zxu*(x_npk2 - xkx);
  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end;
  end; //if
  end; //for

  F0_urov[i]:= Znak*yxu;
  F0_rih1[i]:= Znak*yxr01;
  F0_rih2[i]:= Znak*yxr02;
  F0_sh[i]:=  y;//zxu;//Point(GlbKmtrue,xxk,npk1,npm1,npk2,npm2,d1,d2,h,nrm, kmn,lng);//zxu;  //f_norm
  F_Rad[i]:=  round(rad);

  end; // for
  end;
}

// ------------------------------------------------------------------------------
function xKmLeng(x: integer): integer;
var
  i, npch, put, lng, kmn, val: integer;
begin

  val := 1000;
  for i := 0 to High(UNst) do
  begin
    put := NPut(UNst[i].put);
    lng := UNst[i].dlina;
    kmn := UNst[i].km;
    npch := UNst[i].pch;

    if (x = kmn) and (Glb_PutList_PCH = npch) and (NUMPUT = put) then
    begin
      val := lng;
      Break;
    end;
  end;
  xKmLeng := val;
end;

// ==============================================================================
function isNestKm(x: integer): boolean;
var
  i, npch, put, lng, kmn: integer;
  val: boolean;
begin

  val := false;
  for i := 0 to High(UNst) do
  begin
    put := NPut(UNst[i].put);
    lng := UNst[i].dlina;
    kmn := UNst[i].km;
    npch := UNst[i].pch;

    if (x = kmn) and (Glb_PutList_PCH = npch) and (NUMPUT = put) then
    begin
      val := true;
      Break;
    end;
  end;
  isNestKm := val;
end;

// ===========================================

// ==============================================================================

procedure Get_NullFunc;
var
  i, j, sj, L, jj, j2, incr: integer;
  xkx, xx, rad: real; //
  yxu, yxr01, yxr02, zxu, fsr, fsrk, fsr2, fsrs, max, znak, znak_level, gg1,
    gg2: real;
  s, s1, k, nrm, xxk: real;
  bool1, continue_flag, krv: boolean;
  xput: integer;
  kk, ss, xsd, dd, kmd, ww, kmn, npch, put, lng, utem: integer;
  flag, NstFlag, isIbigJ: boolean;
  npm1, npm2, d1, d2, h, sums, avg_f0rih, avg_f0rih2, distanceFrom: real;

  km11, km12, km21, km22, m11, m12, m21, m22, leng, lengKm, tmpkm1,
    tmpkm2: integer;
  fisnes1, fisnes2, iskriv: boolean;
  len1, len2: real;
begin

  znak := 1;
  krv := false;

  NstFlag := false;
  incr := 0;
  sums := 0;
  distanceFrom := 0.0021;
  for i := 0 to high(F_mtr) do
  begin
    iskriv := false;
    xkx := CoordinateToReal(GlbKmtrue, F_mtr[i]);
    kmn := Fn0[i].m;
    utem := 0;
    yxu := 0;
    yxr01 := 0;
    yxr02 := 0;
    nrm := s_norm[i];

    if (1524 < s_norm[i]) and (s_norm[i] <= 1529) then
      nrm := 1524;
    zxu := nrm;
    rad := 10000;
    ss := 0;
    kk := 0;
    xsd := 0;
    flag := false;
    fisnes1 := false;
    fisnes2 := false;

    for j := 0 to high(UKrv) do
    begin

      if (UKrv[j].n100 <= CoordinateToReal(GlbKmtrue, F_mtr[i])) and
        (UKrv[j].k100 >= CoordinateToReal(GlbKmtrue, F_mtr[i])) then
      begin
        iskriv := true;

        if UKrv[j].nmtr = GlbKmLength then
          UKrv[j].nmtr := GlbKmLength - 1;
        if UKrv[j].kmtr = GlbKmLength then
          UKrv[j].kmtr := GlbKmLength - 1;
        for sj := 0 to High(UKrv[j].strights) do
        begin

          fsr := Fsred_krivoi(UKrv[j].strights[sj].Radius); //
          fsrs := abs(UKrv[j].strights[sj].norma_width - s_norm[i]); // 1520;

          if ((UKrv[j].strights[sj].x_npk1 - distanceFrom <= xkx) and
            (xkx <= UKrv[j].strights[sj].x_npk2 + distanceFrom)) or
            ((UKrv[j].strights[sj].x_npk1 - distanceFrom >= xkx) and
            (xkx >= UKrv[j].strights[sj].x_npk2 + distanceFrom)) then
            utem := 0;

          if (UKrv[j].strights[sj].x_npk1 > xkx) or
            (xkx > UKrv[j].strights[sj].x_npk2) then
            flag_dlya_Krivoi := false;

          if ((UKrv[j].strights[sj].x_npk1 <= xkx) and
            (xkx <= UKrv[j].strights[sj].x_npk2)) or
            ((UKrv[j].strights[sj].x_npk1 >= xkx) and
            (xkx >= UKrv[j].strights[sj].x_npk2)) then
          begin
            flag_dlya_Krivoi := true;
            GlbKrvZnak := round(gznak(UKrv[j].strights[sj].x_npk1,
              UKrv[j].strights[sj].x_npk2));
            rad := UKrv[j].strights[sj].Radius;
          end;

          gg1 := UKrv[j].strights[sj].L1;
          gg2 := UKrv[j].strights[sj].L2;

          if gg1 = 0 then
            gg1 := 30;
          if gg2 = 0 then
            gg2 := 30;

          // нач. перех. крив.
          if (UKrv[j].strights[sj].x_npk1 <= xkx) and
            (xkx <= UKrv[j].strights[sj].x_kpk1) then
          begin
            znak := gznak(UKrv[j].strights[sj].x_npk1,
              UKrv[j].strights[sj].x_npk2);
            len1 := GetDistanceBetweenReal(xkx, UKrv[j].strights[sj].x_npk1,
              GlbTrackId, GlbTripDate); //
            yxr01 := fsr / gg1; // abs(x_npk1 - x_kpk1);  // dlia rihtovki
            yxr01 := yxr01 * len1; //
            yxr02 := fsr / gg1; // abs(x_npk1 - x_kpk1);  // dlia rihtovki
            yxr02 := yxr02 * len1; //

            fsrs := abs(UKrv[j].strights[sj].norma_width - s_norm[i]);
            zxu := fsrs / gg1;
            zxu := s_norm[i] + zxu * len1; //

            // if abs(Fsr_rh1[i]) > 4 then
            // Rih_Nit[i].fun := sign(Fsr_rh1[i]);
          end
          // середина кривой
          else if (UKrv[j].strights[sj].x_kpk1 < xkx) and
            (xkx < UKrv[j].strights[sj].x_kpk2) then
          begin
            znak := gznak(UKrv[j].strights[sj].x_npk1,
              UKrv[j].strights[sj].x_npk2);

            yxr01 := fsr;
            yxr02 := fsr;
            zxu := UKrv[j].strights[sj].norma_width;

            // if abs(Fsr_rh1[i]) > 4 then
            // Rih_Nit[i].fun := sign(Fsr_rh1[i]);
          end
          else
            // кон. перех. крив
            if (UKrv[j].strights[sj].x_kpk2 <= xkx) and
              (xkx <= UKrv[j].strights[sj].x_npk2) then
            begin
              znak := gznak(UKrv[j].strights[sj].x_npk1,
                UKrv[j].strights[sj].x_npk2);
              len2 := GetDistanceBetweenReal(xkx, UKrv[j].strights[sj].x_npk2,
                GlbTrackId, GlbTripDate);

              yxr01 := fsr / gg2; // abs(x_npk2 - x_kpk2);
              yxr01 := yxr01 * len2;

              yxr02 := fsr / gg2;
              yxr02 := yxr02 * len2;
              fsrs := abs(UKrv[j].strights[sj].norma_width - s_norm[i]);
              zxu := fsrs / gg2;
              zxu := s_norm[i] + zxu * len2;

              // if abs(Fsr_rh1[i]) > 4 then
              // Rih_Nit[i].fun := sign(Fsr_rh1[i]);
            end;
        end; // if
        for sj := 0 to High(UKrv[j].levels) do
        begin

          if ((UKrv[j].levels[sj].x_npk1 - distanceFrom <= xkx) and
            (xkx <= UKrv[j].levels[sj].x_npk2 + distanceFrom)) or
            ((UKrv[j].levels[sj].x_npk1 - distanceFrom >= xkx) and
            (xkx >= UKrv[j].levels[sj].x_npk2 + distanceFrom)) then
            utem := 0;

          if ((UKrv[j].levels[sj].x_npk1 <= xkx) and
            (xkx <= UKrv[j].levels[sj].x_npk2)) or
            ((UKrv[j].levels[sj].x_npk1 >= xkx) and
            (xkx >= UKrv[j].levels[sj].x_npk2)) then
          begin
            flag_dlya_Krivoi := true;
            GlbKrvZnak := round(gznak(UKrv[j].levels[sj].x_npk1,
              UKrv[j].levels[sj].x_npk2));
          end;

          gg1 := UKrv[j].levels[sj].L1;
          gg2 := UKrv[j].levels[sj].L2;

          if gg1 = 0 then
            gg1 := 30;
          if gg2 = 0 then
            gg2 := 30;

          // нач. перех. крив.
          if (UKrv[j].levels[sj].x_npk1 <= xkx) and
            (xkx <= UKrv[j].levels[sj].x_kpk1) then
          begin
            znak_level := gznak(UKrv[j].levels[sj].x_npk1,
              UKrv[j].levels[sj].x_npk2);
            yxu := UKrv[j].levels[sj].level / gg1;
            yxu := yxu * GetDistanceBetweenReal(xkx, UKrv[j].levels[sj].x_npk1,
              GlbTrackId, GlbTripDate);
          end
          // середина кривой
          else if (UKrv[j].levels[sj].x_kpk1 < xkx) and
            (xkx < UKrv[j].levels[sj].x_kpk2) then
          begin
            znak_level := gznak(UKrv[j].levels[sj].x_npk1,
              UKrv[j].levels[sj].x_npk2);
            yxu := UKrv[j].levels[sj].level;

          end
          else
            // кон. перех. крив
            if (UKrv[j].levels[sj].x_kpk2 <= xkx) and
              (xkx <= UKrv[j].levels[sj].x_npk2) then
            begin
              znak_level := gznak(UKrv[j].levels[sj].x_npk1,
                UKrv[j].levels[sj].x_npk2);
              yxu := UKrv[j].levels[sj].level / gg2; // abs(x_npk2 - x_kpk2);
              yxu := yxu * GetDistanceBetweenReal(UKrv[j].levels[sj].x_npk2,
                xkx, GlbTrackId, GlbTripDate);
            end;
        end; // if
      end;
    end;
    // for

    Urob[i] := utem;
    F0_urov[i] := znak_level * yxu;
    F0_rih1[i] := znak * yxr01;
    F0_rih2[i] := znak * yxr02;
    F0_sh[i] := zxu;
    F_Rad[i] := round(rad);
  end;
  // for
  F0_rih2 := MultiRadiusCurve(F0_rih2, 4, false);
  F0_rih1 := MultiRadiusCurve(F0_rih1, 4, false);
  F0_urov := MultiRadiusCurve(F0_urov, 12, false);
  // F0_sh:= MultiRadiusCurve(F0_sh,12, true);
end;

{
  //==============================================================================
  procedure Get_NullFunc;
  var
  i, j, l,jj, incr:integer;
  xkx, x_npk1, x_kpk1, x_npk2, x_kpk2, xx, rad :real;   //
  yxu, yxr01, yxr02, zxu, fsr, fsrk, fsr2, fsrs ,max, znak, gg1, gg2:real;
  s,s1,k,nrm, xxk:real;
  bool1, continue_flag, krv:boolean;
  xput :integer;
  kk,ss,xsd, dd, kmd, ww, a,b,kmn, npch, put,lng, xsd1, xsd2 :integer;
  flag, nstFlag:boolean;
  npm1,npm2,d1,d2,h,sums:real;
  begin

  znak:= 1;
  krv:= false;

  NstFlag:= false;

  // for i := 0 to High(UNst) do
  // begin
  //         put:= NPut(UNst[i].put);
  //         lng:= UNst[i].dlina;
  //         kmn:= UNst[i].km;
  //         npch:= UNst[i].pch;
  //     if (GlbKmTrue = kmn) and (Glb_PutList_PCH = npch)
  //        and (NumPut = put) and (lng > 1000) then   NstFlag:= true;
  //
  // end;

  incr:= 0; sums:= 0;

  for i:= 0 to high(x_k) do
  begin
  if flagpic then
  xkx:= glbKmTrue*1000 + round(F_mtr[i])+ppp*100 else
  xkx:= glbKmTrue*1000 + round(F_mtr[i]);

  kmn:= Fn0[i].m;

  yxu:= 0;
  yxr01:= 0;
  yxr02:= 0;
  nrm:= 1520;
  //      if (1524 = s_norm[i]) then nrm:= 1524;
  if (1524 <= f_norm[i]) and (f_norm[i] <= 1529) then nrm:= 1524;
  zxu:= nrm;
  rad:= 10000;
  ss:= 0;
  kk:= 0;
  xsd:= 0;
  flag:= false;

  nrm:= 1520;

  for j:= 0 to high(UKrv) do
  begin

  if (NPut(UKrv[j].put) = NUMPUT ) then
  begin
  xsd1:= 0; xsd2:= 0;
  if UKrv[j].nmtr > 1000 then xsd1:= - 1000;
  if UKrv[j].kmtr > 1000 then xsd2:= - 1000;

  x_npk1:= UKrv[j].nkm*1000 + UKrv[j].nmtr + xsd1;
  x_kpk1:= UKrv[j].nkm*1000 + UKrv[j].nmtr + UKrv[j].d1 + xsd1;
  x_npk2:= UKrv[j].kkm*1000 + UKrv[j].kmtr + xsd2;
  x_kpk2:= UKrv[j].kkm*1000 + UKrv[j].kmtr - UKrv[j].d2 + xsd2;

  if ((x_npk1-10 < xkx) and (xkx < x_npk1))
  or ((x_kpk2 < xkx) and (xkx < x_kpk2 + 10)) then
  begin
  sums:= sums + zxu;
  incr:= incr + 1;
  end;

  if incr > 0 then
  begin
  nrm:= sums/incr;
  sums:=0;
  incr:=0;
  end;

  fsr:= Fsred_krivoi(UKrv[j].rad);//
  fsrs:=  UKrv[j].norm - nrm;//1520;

  if (nrm > UKrv[j].norm) then
  begin
  fsrs:= 0;
  //   zxu:= 1524;
  end;

  if (x_npk1 > xkx) or (xkx > x_npk2) then  flag_dlya_Krivoi:= false;

  if ((x_npk1 <= xkx) and (xkx <= x_npk2))
  or ((x_npk1 >= xkx) and (xkx >= x_npk2))
  then
  begin
  flag_dlya_Krivoi:= true;
  GlbKrvZnak:= round(gznak(round(x_npk1),round(x_npk2)));
  rad:=  UKrv[j].rad;
  end;

  gg1:= abs(x_npk1 - x_kpk1);
  gg2:= abs(x_npk2 - x_kpk2);

  if gg1 = 0 then gg1 := 30;
  if gg2 = 0 then gg2 := 30;

  // нач. перех. крив.
  if (x_npk1 <= xkx) and (xkx <= x_kpk1) and (gg1 <> 0) then
  begin
  znak:= gznak(x_npk1,x_npk2);

  yxu:= UKrv[j].voz/gg1;          //abs(x_npk1 - x_kpk1); // dlia urovnia
  yxu:= yxu*(xkx - x_npk1); //(

  yxr01:= fsr/gg1;              //abs(x_npk1 - x_kpk1);  // dlia rihtovki
  yxr01:= yxr01*(xkx - x_npk1); //

  yxr02:= fsr/gg1;              //abs(x_npk1 - x_kpk1);  // dlia rihtovki
  yxr02:= yxr02*(xkx - x_npk1); //

  zxu:= fsrs/gg1;
  zxu:= nrm + zxu*(xkx - x_npk1);

  if abs(Fsr_rh1[i]) > 4 then
  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end
  // середина кривой
  else
  if (x_kpk1 < xkx) and (xkx < x_kpk2) then
  begin
  znak:= gznak(x_npk1,x_npk2);

  yxu:= UKrv[j].voz;
  yxr01:= fsr;
  yxr02:= fsr;
  zxu:= UKrv[j].norm;

  if abs(Fsr_rh1[i]) > 4 then
  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end
  else
  // кон. перех. крив
  if (x_kpk2 <= xkx) and (xkx <= x_npk2) and (gg2 <> 0) then
  begin
  znak:= gznak(x_npk1,x_npk2);

  yxu:= UKrv[j].voz/gg2;                         //abs(x_npk2 - x_kpk2);
  yxu:= yxu*(x_npk2 - xkx);

  yxr01:= fsr/gg2;                               //abs(x_npk2 - x_kpk2);
  yxr01:= yxr01*(x_npk2 - xkx);

  yxr02:= fsr/gg2;                               //abs(x_npk2 - x_kpk2);
  yxr02:= yxr02*(x_npk2 - xkx);

  zxu:= fsrs/gg2;
  zxu:= nrm + zxu*(x_npk2 - xkx);

  if abs(Fsr_rh1[i]) > 4 then
  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end;
  end; //if
  end; //for

  F0_urov[i]:= Znak*yxu;
  F0_rih1[i]:= Znak*yxr01;
  F0_rih2[i]:= Znak*yxr02;
  F0_sh[i]:=  zxu;//Point(GlbKmtrue,xxk,npk1,npm1,npk2,npm2,d1,d2,h,nrm, kmn,lng);//zxu;  //f_norm
  F_Rad[i]:=  round(rad);

  end; // for
  end;

}
// ==============================================================================
{ procedure Get_Null_Func1;
  var
  i, j, l,jj:integer;
  xkx, x_npk1, x_kpk1, x_npk2, x_kpk2, xx, rad :real;   //
  yxu, yxr01, yxr02, zxu, fsr, fsrk, fsr2, fsrs ,max, znak, gg1, gg2:real;
  s,s1,k,nrm:real;
  bool1, continue_flag, krv:boolean;
  xput :integer;
  kk,ss,xsd:integer;
  begin

  znak:= 1;
  krv:= false;

  for i:= 0 to high(x_k) do
  begin
  if flagpic then
  xkx:= glbKmTrue*1000 + round(F_mtr[i])+ppp*100 else
  xkx:= glbKmTrue*1000 + round(F_mtr[i]);
  yxu:= 0;
  yxr01:= 0;
  yxr02:= 0;
  nrm:= 1520;
  if (1524 = s_norm[i]) then nrm:= 1524;


  zxu:= nrm;

  // nrm:= zxu;

  rad:= 10000;

  ss:= 0;
  kk:= 0;
  xsd:= 0;

  for j:= 0 to high(UKrv1) do
  begin
  x_npk1:= UKrv1[j].nkm*1000 + UKrv1[j].nmtr + xsd;
  x_kpk1:= UKrv1[j].nkm*1000 + UKrv1[j].nmtr + UKrv1[j].d1 + xsd;
  x_npk2:= UKrv1[j].kkm*1000 + UKrv1[j].kmtr + xsd;
  x_kpk2:= UKrv1[j].kkm*1000 + UKrv1[j].kmtr - UKrv1[j].d2 + xsd;

  //------------------
  continue_flag:= false;
  for jj := 0 to high(unst) do
  begin
  if  ((UKrv1[j].nkm = unst[jj].km) or (UKrv1[j].kkm = unst[jj].km))
  and ( nput(unst[jj].put) = NUMPUT) and (GlbKmTrue = unst[jj].km) then
  continue_flag:= true;
  end;
  if continue_flag then continue;
  //---------------

  fsr:= Fsred_krivoi(UKrv1[j].rad);//
  fsrs:=  UKrv1[j].norm - nrm;//1520;

  if nrm > UKrv1[j].norm then
  begin
  fsrs:= 0;
  //   zxu:= 1524;
  end;
  //  nrm:=1520;

  /// if (x_npk1 > xkx) or (xkx > x_npk2) then krv:= true else krv:=false;

  if (x_npk1 > xkx) or (xkx > x_npk2) then  flag_dlya_Krivoi:= false;

  if ((x_npk1 <= xkx) and (xkx <= x_npk2))
  or ((x_npk1 >= xkx) and (xkx >= x_npk2)) // and not flag_dlya_Krivoi
  then
  begin
  flag_dlya_Krivoi:= true;
  GlbKrvZnak:= round(gznak(round(x_npk1),round(x_npk2)));
  rad:=  UKrv1[j].rad;
  // if fsr < GlbKrvRihMax then fsr:= GlbKrvRihMax;

  end;

  gg1:= abs(x_npk1 - x_kpk1);
  gg2:= abs(x_npk2 - x_kpk2);

  if gg1 = 0 then gg1 := 30;
  if gg2 = 0 then gg2 := 30;

  // нач. перех. крив.
  if (x_npk1 <= xkx) and (xkx <= x_kpk1) and (gg1 <> 0) then
  begin
  znak:= gznak(x_npk1,x_npk2);
  yxu:= UKrv1[j].voz/gg1;          //abs(x_npk1 - x_kpk1); // dlia urovnia
  yxu:= yxu*(xkx - x_npk1); //(

  yxr01:= fsr/gg1;              //abs(x_npk1 - x_kpk1);  // dlia rihtovki
  yxr01:= yxr01*(xkx - x_npk1); //

  yxr02:= fsr/gg1;              //abs(x_npk1 - x_kpk1);  // dlia rihtovki
  yxr02:= yxr02*(xkx - x_npk1); //

  zxu:= fsrs/gg1;
  zxu:= nrm + zxu*(xkx - x_npk1);

  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);

  end
  // середина кривой
  else
  if (x_kpk1 < xkx) and (xkx < x_kpk2) then
  begin
  znak:= gznak(x_npk1,x_npk2);
  yxu:= UKrv1[j].voz;
  yxr01:= fsr;
  yxr02:= fsr;
  zxu:= UKrv1[j].norm;

  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  //        if zxu < nrm then  zxu :=nrm;

  end
  else
  // кон. перех. крив
  if (x_kpk2 <= xkx) and (xkx <= x_npk2) and (gg2 <> 0) then
  begin
  znak:= gznak(x_npk1,x_npk2);
  yxu:= UKrv1[j].voz/gg2;                         //abs(x_npk2 - x_kpk2);
  yxu:= yxu*(x_npk2 - xkx);

  yxr01:= fsr/gg2;                               //abs(x_npk2 - x_kpk2);
  yxr01:= yxr01*(x_npk2 - xkx);

  yxr02:= fsr/gg2;                               //abs(x_npk2 - x_kpk2);
  yxr02:= yxr02*(x_npk2 - xkx);

  zxu:= fsrs/gg2;
  zxu:= nrm + zxu*(x_npk2 - xkx);

  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);

  end;

  end; //for


  F0_urov[i]:= Znak*yxu;
  F0_rih1[i]:= Znak*yxr01;
  F0_rih2[i]:= Znak*yxr02;

  F0_sh[i]:=  zxu;  //f_norm
  F_Rad[i]:=  round(rad);

  end; // for
  end;
  //==============================================================================
  procedure Get_Null_Func2;
  var
  i, j, l, jj:integer;
  xkx, x_npk1, x_kpk1, x_npk2, x_kpk2, xx, rad :real;   //
  yxu, yxr01, yxr02, zxu, fsr, fsrk, fsr2, fsrs ,max, znak, gg1, gg2:real;
  s,s1,k,nrm:real;
  bool1, continue_flag :boolean;
  xput, xsd :integer;
  begin

  znak:= 1;
  for i:= 0 to high(x_k) do
  begin
  if flagpic then
  xkx:= glbKmTrue*1000 + round(F_mtr[i])+ppp*100 else
  xkx:= glbKmTrue*1000 + round(F_mtr[i]);
  yxu:= 0;
  yxr01:= 0;
  yxr02:= 0;
  zxu:= 1520;
  if (1524 = s_norm[i]) then zxu:= 1524;
  nrm:= zxu;
  rad:= 10000;
  xsd:= 0;

  for j:= 0 to high(UKrv2) do
  begin
  x_npk1:= UKrv2[j].nkm*1000 + UKrv2[j].nmtr + xsd;
  x_kpk1:= UKrv2[j].nkm*1000 + UKrv2[j].nmtr + UKrv2[j].d1 + xsd;
  x_npk2:= UKrv2[j].kkm*1000 + UKrv2[j].kmtr + xsd;
  x_kpk2:= UKrv2[j].kkm*1000 + UKrv2[j].kmtr - UKrv2[j].d2 + xsd;

  //--------------
  continue_flag:= false;
  for jj := 0 to high(unst) do
  begin
  if  ((UKrv2[j].nkm = unst[jj].km) or (UKrv2[j].kkm = unst[jj].km))
  and ( nput(unst[jj].put) = NUMPUT) and (GlbKmTrue = unst[jj].km) then
  continue_flag:= true;
  end;
  if continue_flag then continue;
  //---------------

  fsr:= Fsred_krivoi(UKrv2[j].rad);// + 8;
  fsrs:=  UKrv2[j].norm - nrm;//1520;

  if nrm > UKrv2[j].norm then
  begin
  fsrs:= 0;
  //         nrm:= 1524;
  end;

  if (x_npk1 > xkx) or (xkx > x_npk2) then  flag_dlya_Krivoi:= false;

  if ((x_npk1 <= xkx) and (xkx <= x_npk2)) or ((x_npk1 >= xkx) and (xkx >= x_npk2)) // and not flag_dlya_Krivoi
  then
  begin
  flag_dlya_Krivoi:= true;
  GlbKrvZnak:= round(gznak(round(x_npk1),round(x_npk2)));
  rad:=  UKrv2[j].rad;
  // if fsr < GlbKrvRihMax then fsr:= GlbKrvRihMax;
  end;

  gg1:= abs(x_npk1 - x_kpk1);
  gg2:= abs(x_npk2 - x_kpk2);

  if gg1 = 0 then gg1 := 30;
  if gg2 = 0 then gg2 := 30;


  // нач. перех. крив.
  if (x_npk1 <= xkx) and (xkx <= x_kpk1) and (gg1 <> 0) then
  begin
  znak:= gznak(x_npk1,x_npk2);
  yxu:= UKrv2[j].voz/gg1;          //abs(x_npk1 - x_kpk1); // dlia urovnia
  yxu:= yxu*(xkx - x_npk1); //(

  yxr01:= fsr/gg1;              //abs(x_npk1 - x_kpk1);  // dlia rihtovki
  yxr01:= yxr01*(xkx - x_npk1); //

  yxr02:= fsr/gg1;              //abs(x_npk1 - x_kpk1);  // dlia rihtovki
  yxr02:= yxr02*(xkx - x_npk1); //

  zxu:= fsrs/gg1;
  zxu:= nrm + zxu*(xkx - x_npk1);

  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end
  // середина кривой
  else
  if (x_kpk1 < xkx) and (xkx < x_kpk2) then
  begin
  znak:= gznak(x_npk1,x_npk2);
  yxu:= UKrv2[j].voz;
  yxr01:= fsr;
  yxr02:= fsr;
  zxu:= UKrv2[j].norm;//fsrs;
  //         if nrm > zxu then zxu := nrm;
  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end
  else
  // кон. перех. крив
  if (x_kpk2 <= xkx) and (xkx <= x_npk2) and (gg2 <> 0) then
  begin
  znak:= gznak(x_npk1,x_npk2);
  yxu:= UKrv2[j].voz/gg2;                         //abs(x_npk2 - x_kpk2);
  yxu:= yxu*(x_npk2 - xkx);

  yxr01:= fsr/gg2;                               //abs(x_npk2 - x_kpk2);
  yxr01:= yxr01*(x_npk2 - xkx);

  yxr02:= fsr/gg2;                               //abs(x_npk2 - x_kpk2);
  yxr02:= yxr02*(x_npk2 - xkx);

  zxu:= fsrs/gg2;
  zxu:= nrm + zxu*(x_npk2 - xkx);

  Rih_Nit[i].fun:= sign(Fsr_rh1[i]);
  end;
  end; //for

  F0_urov[i]:= Znak*yxu;
  F0_rih1[i]:= Znak*yxr01;
  F0_rih2[i]:= Znak*yxr02;
  F0_sh[i]:=  zxu;  // f_norm
  F_Rad[i]:=  round(rad);

  end; // for
  end; }
// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------

// ------------------------------------------------------------------------------
// opredelenie znaka krivoi
// ------------------------------------------------------------------------------
function gznak(a, b: real): real;
var
  i, id: integer;
  x: real;
  max, znak, aa, bb: real;
begin
  max := 0;
  znak := 1;
  // nn := round(abs(b - a) / 3.0);

  for i := 0 to high(Fsr_rh1) do
  begin
    id := 0;
    if true then
      x := CoordinateToReal(GlbKmtrue + id, F_mtr[i]);
    if ((a <= x) and (x <= b)) or ((a >= x) and (x >= b)) then
    begin

      if (abs(Fsr_rh1[i]) > max) then
      begin
        max := abs(Fsr_rh1[i]);
        znak := Fsr_rh1[i] / max;
        GlbKrvRihMax := 0.99 * max;
        GlbKrvUrvMax := 0.99 * abs(Fsr_Urb[i]);
      end;

    end;
  end;
  gznak := znak;
end;

// ------------------------------------------------------------------------------
// opredelenie znaka krivoi
// ------------------------------------------------------------------------------
function gznak_nest(a, b: real): real;
var
  i, x, { nn, } id: integer;
  max, znak, aa, bb: real;
begin
  max := 0;
  znak := 1;
  // nn:= round(abs(b-a)/3.0);

  for i := 0 to high(Fsr_rh1) do
  begin
    x := round(Fn0[i].m); // F_mtr[i]);   if (a-nn <= x) and (x <= b-nn) then

    if ((a <= x) and (x <= b)) or ((a >= x) and (x >= b)) then
    begin

      if (abs(Fsr_rh1[i]) > max) then
      begin
        max := abs(Fsr_rh1[i]);
        znak := Fsr_rh1[i] / max;
        GlbKrvRihMax := 0.99 * max;
        GlbKrvUrvMax := 0.99 * abs(Fsr_Urb[i]);
      end; // if
    end; // if
  end; // for
  gznak_nest := znak;
end;

// ------------------------------------------------------------------------------
// opredelenie znaka krivoi
// ------------------------------------------------------------------------------
function GetZnak(a, b: integer): integer;
var
  i, x, nn, mind, tet, znak: integer;
  f, maxr, maxu, znk: real;
begin
  maxr := 0;
  maxu := 0;
  f := 0;

  for i := 0 to high(Fsr_rh1) do
  begin
    x := GlbKmtrue * 1000 + round(F_mtr[i]);

    // 1  na perehodnoi kriboi
    if (a <= x) and (x <= b) then

      if (abs(Fsr_rh1[i]) > maxr) then
      begin

        maxr := abs(Fsr_rh1[i]);
        maxu := abs(Fsr_Urb[i]);
        f := Fsr_rh1[i];
        znk := f / maxr;
      end;

  end; // for

  znak := 1;
  if (znk < 0) then
    znak := -1;

  GlbKrvRihMax := maxr;
  GlbKrvUrvMax := maxu;

  Result := znak;
end;

// ------------------------------------------------------------------------------
// ПРОВЕРКА ОПРЕДЕЛЕННОЙ ТОЧКИ НА ПРИНАДЛЕЖНОСТИ КРИВОЙ    UMT
// ------------------------------------------------------------------------------
function ProberkaNaKrivoi(xx: integer): boolean;
var
  i, a, b: integer;
begin
  ProberkaNaKrivoi := false;

  for i := 0 to high(kv_natur) do
  begin
    a := kv_natur[i].npk1 ;
    b := kv_natur[i].npk2 ;

    if (a <= xx) and (xx <= b) then
      ProberkaNaKrivoi := true;
  end;
end;
// ------------------------------------------------------------------------------
// ПРОВЕРКА ОПРЕДЕЛЕННОЙ KM НА ПРИНАДЛЕЖНОСТИ КРИВОЙ    UMT
// ------------------------------------------------------------------------------

function FlagKrivoi: boolean;
var
  i, k, a, b, c, d, a0, b0: integer;
begin
  FlagKrivoi := false;

  for i := 0 to high(UKrv) do
    if ((GlbKmtrue >= UKrv[i].nkm) and (GlbKmtrue <= UKrv[i].kkm)) and
      (NPut(UKrv[i].put) = NUMPUT) then
      FlagKrivoi := true;

end;

// ------------------------------------------------------------------------------
// ПРОВЕРКА ОПРЕДЕЛЕННОЙ ТОЧКИ НА ПРИНАДЛЕЖНОСТИ К МОСТУ ИЛИ ТОННЕЛИ   UMT
// ------------------------------------------------------------------------------
function ProberkaNaMostTonnel(start, finish: integer): boolean;
const
  L1 = 200; // metr
  L2 = 500; // metr
var
  uch_a, uch_b, ortasy: real;
  j: integer;

BEGIN
  result := false;
  ortasy := CoordinateToReal(GlbKmtrue, round((start + finish) / 2));
  for j := 0 to High(UMT) do
  begin

    if (((UMT[j].nkm <= GlbKmtrue) and (GlbKmtrue <= UMT[j].kkm)) or
      ((UMT[j].nkm >= GlbKmtrue) and (GlbKmtrue >= UMT[j].kkm))) then
    begin
      uch_a := CoordinateToReal(UMT[j].nkm, UMT[j].nmtr);
      uch_b := CoordinateToReal(UMT[j].kkm, UMT[j].kmtr);

      if   ((uch_a <= ortasy) and (ortasy <= uch_b) and (UMT[j].dl>10)) then
      begin
        result := true;
        break;
      end;
    end;
  end;

END;

function CheckForIsoJoint(start_meter, final_meter: integer): boolean;
var
  i: integer;
  start_coord, final_coord, iso_coord: real;
begin

  CheckForIsoJoint := false;
  start_coord := CoordinateToReal(GlbKmtrue, start_meter);
  final_coord := CoordinateToReal(GlbKmtrue, final_meter);
  for i := 0 to high(IsoJoint) do
  begin
    iso_coord := CoordinateToReal(IsoJoint[i].km, IsoJoint[i].meter);
    if ( abs( (start_coord +final_coord)/2 - iso_coord)<+0.00065)

    //if ((start_coord <= iso_coord) and (iso_coord <= final_coord) or
     //        (start_coord <= iso_coord) and (iso_coord >= final_coord)
      //   )



  // if    abs (iso_coord  -  (start_coord+final_coord)/2.0)<12     then
       then
    begin
      CheckForIsoJoint := true;
      Break;
    end;
  end;
end;

// Проверка на бесстык путь
function CheckForJointlessPath(start_meter, final_meter: integer): boolean;
var
  i: integer;
  start_coord, final_coord, path_start_coord, path_final_coord: real;
begin
  CheckForJointlessPath := false;
  start_coord := CoordinateToReal(GlbKmtrue, start_meter);
  final_coord := CoordinateToReal(GlbKmtrue, final_meter);
  for i := 0 to high(JointlessPath) do
  begin
    path_start_coord := CoordinateToReal(JointlessPath[i].start_km,
      JointlessPath[i].start_m);
    path_final_coord := CoordinateToReal(JointlessPath[i].final_km,
      JointlessPath[i].final_m);
    if ((path_start_coord <= start_coord) and (start_coord <= path_final_coord))
      or ((path_start_coord >= start_coord) and
      (start_coord >= path_final_coord)) or
      ((path_start_coord <= final_coord) and (final_coord <= path_final_coord))
      or ((path_start_coord >= final_coord) and
      (final_coord >= path_final_coord)) then
    begin
      CheckForJointlessPath := true;
      Break;
    end;
  end;
end;

// ------------------------------------------------------------------------------
// ПРОВЕРКА ОПРЕДЕЛЕННОЙ ТОЧКИ НА ПРИНАДЛЕЖНОСТИ СТРЕЛОЧ. ПЕРЕВ.
// ------------------------------------------------------------------------------
function SearchStrelkaNatur(Fver, FverCoor: mas; xx: integer): boolean;
const
  La = 2000; // sm    // 20 metr
  h = 45; // mm
var
  flgPro: boolean;
  i: integer;
  L0, Lm, Ln: integer;
  fa, fb, Hn: real;
begin
  SearchStrelkaNatur := false;

  For i := Low(FverCoor) to High(FverCoor) - 1 do
  begin
    L0 := FverCoor[i];
    Lm := FverCoor[i + 1];
    Ln := round(abs(Lm - L0));
    fa := Fver[i];
    fb := Fver[i + 1];
    Hn := round(abs(fa - fb));

    flgPro := false;
    if ((fa > 0) and (0 > fb)) or ((fa < 0) and (0 < fb)) then
      flgPro := true;

    if (Ln <= La) and (h < Hn) and flgPro and (L0 <= xx) and (xx <= Lm) then
      SearchStrelkaNatur := true;
  end;

end;

// ------------------------------------------------------------------------------
// ПРОВЕРКА ОПРЕДЕЛЕННОЙ ТОЧКИ НА ПРИНАДЛЕЖНОСТИ СТРЕЛОЧ. ПЕРЕВ.
// ------------------------------------------------------------------------------
procedure SearchSiezd;
const
  La = 2000; // sm    // 20 metr
  h = 45; // mm
var
  flg: boolean;
  i, j: integer;
  L0, Lm, Ln: integer;
  fa, fb, fc, Hn, maxf: real;
  n: integer;
  x, a, b: integer;
begin
  Setlength(fstrel, 2500);

  flg := false;
  n := 20; // flg:= false;

  i := 0;
  j := 0;
  a := 0;
  b := 0;

  while i <= High(Fsr_rh1) - 2 * n do
  begin
    fa := Fsr_rh1[i];
    fb := Fsr_rh1[i + n];
    fc := Fsr_rh1[i + 2 * n];

    L0 := round(F_mtr[i]); // x_k[i+n] div 100000;// - n;
    Lm := round(F_mtr[i + 2 * n]); // x_k[i+2*n] div 100;// + n;

    Hn := abs(fa - fb) + abs(fb - fc);

    if min(L0, Lm) > 150 then
      a := 150
    else
      a := min(L0, Lm);
    if max(L0, Lm) > F_mtr[High(F_mtr)] - 150 then
      b := 150
    else
      b := round(F_mtr[High(F_mtr)]) - max(L0, Lm);

    if (Hn > 31 * 1.3) or (abs(fb) > 110) or (abs(fa) > 110) or (abs(fc) > 110)
    then
    begin

      if NAPR_DBIJ > 0 THEN
      begin
        fstrel[j].nachm := min(L0, Lm) - a;
        fstrel[j].konm := max(L0, Lm) + b;
      end;

      if NAPR_DBIJ < 0 THEN
      begin
        fstrel[j].nachm := max(L0, Lm) + b;
        fstrel[j].konm := min(L0, Lm) - a;
      end;
      { sablog(inttostr(GlbKmTrue) + ' l0 -'
        + inttostr(GlbKmIndex*1000 + max(L0,Lm) + b) + ' lm -'
        + inttostr(GlbKmIndex*1000 + min(L0,Lm) - a)); }
      i := i + n;
      j := j + 1;
    end;

    i := i + 1;
  end;

  Setlength(fstrel, j);
end;

// ==============================================================================
function FlagSiezd(a, b: integer): boolean; // a, b -metr
var
  i, n: integer;
  x1, x2, xx: real;
  flg, fkrv: boolean;
begin
  flg := false;

  n := 25;
  xx := round((a + b) / 2);
  xx := CoordinateToReal(GlbKmtrue, round(xx));
  // showmessage(inttostr(xx));
  fkrv := false;

  for i := 0 to High(UKrv) do
  begin
    x1 := GetCoordByLen(UKrv[i].nkm, UKrv[i].nmtr, -n, GlbTrackId, GlbTripDate);
    x2 := GetCoordByLen(UKrv[i].kkm, UKrv[i].kmtr, n, GlbTrackId, GlbTripDate);

    if ((x1 <= xx) and (xx <= x2)) or ((x1 >= xx) and (xx >= x2)) then
    // and (abs(x1-x2) < 2000)
    begin
      fkrv := true;

    end;
  end;


  // fkrv:= KrvFlag(a,b);

  // fkrv:= false; // 24.08.2015

  if not fkrv then
  begin
    for i := 0 to high(fstrel) do
    begin
      if (((fstrel[i].nachm <= xx) and (xx <= fstrel[i].konm)) or
        ((fstrel[i].nachm >= xx) and (xx >= fstrel[i].konm))) then
      begin
        flg := true;
        // break;
      end;
    end;
  end;

  FlagSiezd := flg;
end;

// ------------------------------------------------------------------------------
// ПРОВЕРКА ОПРЕДЕЛЕННОЙ ТОЧКИ НА ПРИНАДЛЕЖНОСТИ СТРЕЛОЧ. ПЕРЕВ.
// ------------------------------------------------------------------------------
function ProberkaNaStrelku(start, finish, cod: integer): boolean;
var
  i, j: integer;
  flagxx, ftemp, fkrv: boolean;
  pm: integer;
  start_coord, final_coord, currentCoord: real;
  prom:integer;
begin
  // result := false; exit;
  ftemp := false;
  flagxx := true;
  pm := 30;
  for i := start to finish do
  begin
    currentCoord := CoordinateToReal(GlbKmtrue, i);
    for j := 0 to high(UStr) do
    begin
      start_coord := GetCoordByLen(UStr[j].nkm, UStr[j].nmtr, -pm*0, GlbTrackId,
        GlbTripDate);


    //  final_coord := GetCoordByLen(UStr[j].kkm, UStr[j].kmtr, pm, GlbTrackId,
      //  GlbTripDate);
           final_coord := start_coord + pm*UStr[j].posh/10000;
      if ((start_coord <= currentCoord) and (currentCoord <= final_coord)) or
        ((start_coord >= currentCoord) and (currentCoord >= final_coord)) then
      begin
        ftemp := true;
        flagxx := false;
                  prom :=UStr[j].bix;
            Gl_Switches_Side:= prom;
        Break;
      end;

    end;
     pm:=30;
    if flagxx then
    begin
    pm:= round(pm);
      for j := 0 to high(UStr2) do
      begin

        start_coord := GetCoordByLen(UStr2[j].nkm, UStr2[j].nmtr,
          -0*pm, GlbTrackId, GlbTripDate);
        final_coord := GetCoordByLen(UStr2[j].kkm, UStr2[j].kmtr,
          pm, GlbTrackId, GlbTripDate);

        if ((start_coord <= currentCoord) and (currentCoord <=  final_coord)) or
          ((start_coord >= currentCoord) and (currentCoord >= final_coord)) then
        begin
          ftemp := true;
                prom :=UStr[j].bix;
            Gl_Switches_Side:= prom;
          Break;
        end;
      end;

    end;
  end;

  // ftemp:= false;
  ProberkaNaStrelku := ftemp;
end;

function SwitchInCurve(xx: integer): boolean;
var
  i, j, s: integer;
  currentCoord: real;
begin
  // result := false; exit;
  SwitchInCurve := false;
  currentCoord := CoordinateToReal(GlbKmtrue, xx);

  if not(xx > 0) then
    exit;
  begin
    for j := 0 to high(UStr) do
    begin
      if (currentCoord >= CoordinateToReal(UStr[j].nkm, UStr[j].nmtr)) and
        (currentCoord <= CoordinateToReal(UStr[j].kkm, UStr[j].kmtr)) then
      begin
        for i := 0 to High(UKrv) do
        begin
          for s := 0 to High(UKrv[i].strights) do
          begin
            if ((currentCoord >= CoordinateToReal(UKrv[i].strights[s].nkm,
              UKrv[i].strights[s].nmtr)) and
              (currentCoord <= CoordinateToReal(UKrv[i].strights[s].kkm,
              UKrv[i].strights[s].kmtr))) and
              ((((UStr[j].marka = 9) or (UStr[j].marka = 10)) and
              (UKrv[i].strights[s].Radius < 1500)) or
              (not((UStr[j].marka = 9) or (UStr[j].marka = 10)) and
              (UKrv[i].strights[s].Radius < 100))) then
              SwitchInCurve := true;
          end;
        end;
      end;
    end;
  end;

end;

function CheckForFactSwitch(meter: integer): boolean;
begin
  Result := ProberkaNaStrelku(meter, meter + 1, 1);
end;

function ProberkaNaStrelkuReal(currentCoord: real; cod: integer): boolean;
var
  i, j: integer;
  flagxx, ftemp, fkrv: boolean;
  pm: integer;
  start_coord, final_coord: real;
begin
  ftemp := false;
  flagxx := true;
  pm := 30;

  if (currentCoord > 0) then
  begin

    for j := 0 to high(UStr) do
    begin
      start_coord := GetCoordByLen(UStr[j].nkm, UStr[j].nmtr, -round(NAPR_DBIJ *0),
        GlbTrackId, GlbTripDate);
    //final_coord := GetCoordByLen(UStr[j].kkm, UStr[j].kmtr, round(NAPR_DBIJ * pm/2),
     //   GlbTrackId, GlbTripDate);;
       final_coord :=  start_coord - pm *UStr[j].posh/10000;
        //GlbTrackId, GlbTripDate);;


      if ((start_coord <= currentCoord) and (currentCoord <= final_coord)) or
        ((start_coord >= currentCoord) and (currentCoord >= final_coord)) then
      begin
        ftemp := true;
        flagxx := false;
        Break;
      end;

    end;

    if flagxx then
    begin
      for j := 0 to high(UStr2) do
      begin

        start_coord := GetCoordByLen(UStr2[j].nkm, UStr2[j].nmtr,
          -round(NAPR_DBIJ * 0), GlbTrackId, GlbTripDate);
        final_coord := GetCoordByLen(UStr2[j].kkm, UStr2[j].kmtr,
          //final_coord :=  start_coord - pm*UStr2[j].posh/10000;
          round( 1 * pm/2), GlbTrackId, GlbTripDate);

        if ((start_coord <= currentCoord) and (currentCoord <= b)) or
          ((start_coord >= currentCoord) and (currentCoord >= final_coord)) then
        begin
          ftemp := true;
          Break;
        end;
      end;

    end;

  end;
  // ftemp:= false;
  ProberkaNaStrelkuReal := ftemp;
end;

// ------------------------------------------------------------------------------
// ПРОВЕРКА ОПРЕДЕЛЕННОГО МЕТРА НА ЖЕЛЕЗОБЕТОН. ШПАЛЫ
// ------------------------------------------------------------------------------
function ProberkaNaZhelezBetShpal(xx: integer): boolean;
var
  i, a, b: integer;
begin
  ProberkaNaZhelezBetShpal := false;

  for i := 0 to high(UShp) do
  begin
    a := UShp[i].nkm * 1000 + UShp[i].nmtr;
    b := UShp[i].kkm * 1000 + UShp[i].kmtr;

    if (UShp[i].god < 2) and (((a <= xx) and (xx <= b)) or
      ((a >= xx) and (xx >= b))) then
    begin
      ProberkaNaZhelezBetShpal := true;
      Break;
    end;

  end;
end;


// ------------------------------------------------------------------------------

// ------------------------------------------------------------------------------
function SiezdFilter(xa, xb: integer): boolean;
var
  a, b, j, i, c, d, xc, e, f: integer;
  ffilter: boolean;
begin
  ffilter := true;
  // ===

  e := GlbKmtrue * 1000 + (xa div 100) mod 1000;
  f := GlbKmtrue * 1000 + (xb div 100) mod 1000;
  {
    for i:= 0 to high(UKrv) do
    begin
    c:= UKrv[i].nkm*1000 + UKrv[i].nmtr;
    d:= UKrv[i].kkm*1000 + UKrv[i].kmtr;

    if ((c <= e) and (e <= d)) or ((c <= f) and (f <= d)) then begin
    //=== }
  for j := 0 to high(fstrel) do
  begin

    a := fstrel[j].nachm;
    b := fstrel[j].konm;
    if ((a <= xa) and (xa <= b)) or ((a <= xb) and (xb <= b)) then
    begin
      ffilter := false;
      Break;
    end;
  end;
  // ===
  // end;
  // end;

  SiezdFilter := ffilter; // or ((PWOts[iii].bel > 25) and (GlbProcTip = 1))
end;

// ------------------------------------------------------------------------------
// ЗАПИСЬ В ТАБ. INFOTS
// ------------------------------------------------------------------------------
PROCEDURE WRT_INFOTS(PNmOTS: STRING; PWOts: MASOTS);
var
  iii, i, j, k, ABSTKM, XC0, XCM, flgVych, foundIndex: integer; // , cod
  forstr: tstrel;
  aa, bb, abdelt, dlina_ots, mx, tempLo: integer;
  ffilter, str_filter, is2to3: boolean;
  tmpmas: mas;
  kol, otkl, lng, typ, v, vg, vop, vog: integer;
  ots, prm, sval: string;
  norm: real;
BEGIN
  TRY
    if Flag_sablog then
      SabLog('WRT_INFOTS - Запись вычесленных данных по отступлениям');



    // ------------------------------------------------------------------------------
    // 02.03.2013

    for i := 0 to High(PWOts) do
    begin
      if PWOts[i].L0 > PWOts[i].Lm then
      begin
        tempLo := PWOts[i].L0;
        PWOts[i].L0 := PWOts[i].Lm;
        PWOts[i].Lm := tempLo;

      end;
      if (PWOts[i].prim.Contains('рн')) then
      if (PWOts[i].st < 2) then
      begin
        continue;
      end;


      for j := i + 1 to High(PWOts) do
      begin
        if (PWOts[i].st > PWOts[j].st) and (PWOts[i].L0 <= PWOts[j].L0) and
          (PWOts[j].Lm <= PWOts[i].Lm) then
        begin
          PWOts[j].L0 := 0;
          PWOts[j].Lm := 0;
        end;
      end;
    end;

    // ------------------------------------------------------------------------------

    for iii := 0 to High(PWOts) do
    begin

      XC0 := PWOts[iii].L0;
      XCM := PWOts[iii].Lm;

      if GlbFlagOts_2st and (PWOts[iii].st = 2) then
        continue;
      // ------------------------------------------------------------------------------
      if (XC0 > 0) and (XCM > 0) { and not str_filter } then
      begin

        dlina_ots := round(abs(PWOts[iii].L0 - PWOts[iii].Lm));
        kol := 1;
        lng := dlina_ots;

        if PNmOTS = 'USH' then
        begin
          ots := 'Уш';
          kol := 1;
          lng := PWOts[iii].leng;

          if PWOts[iii].st = 3 then
          begin
            kol := PWOts[iii].count;
            lng := PWOts[iii].leng
          end;
        end
        else if PNmOTS = 'SUJ' then
        begin
          ots := 'Суж';
          kol := 1;
          lng := PWOts[iii].leng;

          if PWOts[iii].st = 3 then
          begin
            kol := PWOts[iii].count;
            lng := PWOts[iii].leng
          end;
        end
        else if PNmOTS = 'PR1' then
        begin
          ots := 'Пр.п';
          if tanba_rih < 0 then
            ots := 'Пр.л';
        end
        else if PNmOTS = 'PR2' then
        begin
          ots := 'Пр.л';
          if tanba_rih < 0 then
            ots := 'Пр.п';
        end
        else if PNmOTS = 'PER' then
          ots := 'П'
        else if PNmOTS = 'URB' then
        begin
          ots := 'У';
          kol := 1;
          lng := PWOts[iii].leng;

          if PWOts[iii].st = 3 then
          begin
            kol := PWOts[iii].count;
            lng := PWOts[iii].leng
          end;
        end
        else if PNmOTS = 'RH1' then
        /// /bx4
        begin
          ots := 'Р';
          if not(PWOts[iii].isriht) then
            ots := ots + 'нр';
                //ots := ots + 'нр';
          if (PWOts[iii].onswitch) then
          begin
            ots := ots + 'ст';
          end;

          dlina_ots := round(abs((PWOts[iii].L0 - PWOts[iii].Lm))); // ?
          lng := dlina_ots;
          // sablog('RRRRRR lng = ' + inttostr(lng));
        end;

        if lng <= 0 then
          lng := 1;
        if (30 < lng) and (PNmOTS = 'PER') then
          lng := 30;
        // if ((ots = 'Р') or (ots = 'Рнр') or (ots = 'Р+') or (ots = 'Рнр+') or (ots = 'Рст')) then
        // mx := PWOts[iii].L0 +  round(lng / 2)
        // else

        mx := PWOts[iii].L0 + round(lng / 2);
        if (PNmOTS = 'RH1') then
        begin
          lng := lng * 2;

        end;
        sval := PWOts[iii].prim;
        // if (PWOts[iii].onswitch) then
        // sval := sval + 'Стр.;';
        if (PNmOTS = 'PER') or (PNmOTS = 'RH1') or (PNmOTS = 'URB') or
          (PNmOTS = 'PR1') or (PNmOTS = 'PR2') then
        begin
          if ProberkaNaMostTonnel(PWOts[iii].L0, PWOts[iii].Lm) then
          begin
            sval := sval + 'м;';
            if (PWOts[iii].st = 3) and
              ((ots = 'Пр.п') or (ots = 'Пр.л') or (ots = 'П') or (ots = 'У') or
              (ots = 'Р')) then
              continue;
          end;
        end;

        GlbInfOts[IndGlbInfOts].m := mx;
        GlbInfOts[IndGlbInfOts].st := PWOts[iii].st;
        GlbInfOts[IndGlbInfOts].otkl := inttostr(PWOts[iii].bel);
        GlbInfOts[IndGlbInfOts].dl := lng; // dlina_ots;
        GlbInfOts[IndGlbInfOts].stype := 0;
        GlbInfOts[IndGlbInfOts].vop := PWOts[iii].vop;
        GlbInfOts[IndGlbInfOts].vog := PWOts[iii].vog;
        GlbInfOts[IndGlbInfOts].Otst := ots;
        GlbInfOts[IndGlbInfOts].isEqualTo4 := PWOts[iii].isEqualTo4;
        GlbInfOts[IndGlbInfOts].isEqualTo3 := PWOts[iii].isEqualTo3;
        if (length(sval) < 255) then
          GlbInfOts[IndGlbInfOts].prim := sval;
        IndGlbInfOts := IndGlbInfOts + 1;

        if PWOts[iii].st >= 1 then
        begin
          if mx = 0 then
            mx := 1;

          otkl := PWOts[iii].bel;

          case PWOts[iii].st of
            3:
              typ := 3;
            4:
              typ := 4;
            2:
              typ := 2;
            1:
              typ := 1;
          else
            typ := 0;
          end;

          v := PWOts[iii].v;
          vg := PWOts[iii].vg;
          vop := PWOts[iii].vop;
          vog := PWOts[iii].vog;
          prm := '';
          if (sval <> '') and (length(sval) < 254) then
            prm := sval; // PWOts[iii].prim;

          if (PWOts[iii].st = 3) or (PWOts[iii].isEqualTo3) then
            Ubedom_db3(mx, kol, otkl, lng, typ, v, vg, ots, prm, PWOts[iii].st,
              false, PWOts[iii].onswitch, PWOts[iii].isEqualTo4,
              PWOts[iii].isEqualTo3, PWOts[iii].isLong)
          else if PWOts[iii].st = 2 then
          begin
            is2to3 := false;
            if ((ots = 'Суж') or (ots = 'Уш')) then
            begin
              // is2to3:= PWOts[iii].s3*0.9<=PWOts[iii].val;
            end
            else
            begin
              is2to3 := PWOts[iii].s3 * 0.9 <= otkl;

            end;

            Ubedom_db3(mx, kol, otkl, lng, typ, v, vg, ots, prm, PWOts[iii].st,
              is2to3, PWOts[iii].onswitch, PWOts[iii].isEqualTo4,
              PWOts[iii].isEqualTo3, PWOts[iii].isLong)

          end
          else
            Ubedom_db(mx, kol, otkl, lng, typ, v, vg, vop, vog, ots, prm,
              PWOts[iii].st, false, PWOts[iii].onswitch, PWOts[iii].isEqualTo4,
              PWOts[iii].isLong);

        end;

      end;

    end; // for;

  EXCEPT
  END;
END;

// ==============================================================================
procedure wrt_baskalar(ots, prm: string; h: real; m, lng, st, styp, vp, vg, vop,
  vog, flg: integer);
var
  kol, otkl: integer;
  otkls: string;
begin
  kol := 1;

  if (ots = '?Укл') or (ots = 'Укл') or (ots = 'Пси') or (ots = '?Пси') or
    (ots = 'Анп') or (ots = '?Анп') then
    otkl := round(h * 100)
  else
    otkl := round(h);

  if h = 0 then
    otkls := ''
  else
    otkls := FormatFloat('0.00', h);
  // prm := ''; // 'вел.откл=' + FormatFloat('0.0',h);
  // if frac(h) <> 0 then prm:= 'откл=' + FormatFloat('0.0',h);
  if (vop = -1) and (vog = -1) then
    prm := GlbTempTipRemontKm;

  if (flg = 1) and (ots <> '3Пр') then
  begin
    GlbInfOts[IndGlbInfOts].m := m;
    GlbInfOts[IndGlbInfOts].st := st;
    GlbInfOts[IndGlbInfOts].otkl := otkls; // floattostr(h);
    GlbInfOts[IndGlbInfOts].dl := lng;
    GlbInfOts[IndGlbInfOts].stype := styp;
    GlbInfOts[IndGlbInfOts].vop := vop;
    GlbInfOts[IndGlbInfOts].vog := vog;
    GlbInfOts[IndGlbInfOts].Otst := ots;
    IndGlbInfOts := IndGlbInfOts + 1;
  end;

  if not((((ots = 'Анп') or (ots = '?Анп')) and (vp = vop) and (vg = vog)) or
    (ots = '?Уkл')) then

    Ubedom_db(m, kol, otkl, lng, st, vp, vg, vop, vog, ots, prm, st, false,
      false, false, false);
end;

{
  procedure wrt_baskalar(m:integer; txt:string);
  begin
  GlbInfOts[IndGlbInfOts].M:= m mod 1000;
  GlbInfOts[IndGlbInfOts].prich_ogr_v:=  txt;
  GlbInfOts[IndGlbInfOts].stype:= 1;
  Glbinfots[IndGlbInfOts].St:= 5;
  IndGlbInfOts:= IndGlbInfOts + 1;
  end; }
// ==============================================================================
PROCEDURE WRT_Ogr;
// var
begin
  if GlobUbedOgr <> '' then
  begin
    GlbInfOts[IndGlbInfOts].m := GlbMinOgrSkCoordA;
    GlbInfOts[IndGlbInfOts].prich_ogr_v := GlobUbedOgr; // GlobLentaS;//
    GlbInfOts[IndGlbInfOts].stype := 1;
    GlbInfOts[IndGlbInfOts].st := 5;
    IndGlbInfOts := IndGlbInfOts + 1;
  end;
end;

// ==============================================================================
PROCEDURE WRT_RemKorrInf;
var
  ipic: integer;
  fi: boolean;
begin
  if GlbFlagRemontKm then
  begin
    ipic := 1;
    fi := false;
    while ipic <= 10 do
    begin
      /// fi:= getRemKm(GlbKmTrue, ipic, GlobPassSkorost);  // true;//
      if fi then
      begin
        GlbInfOts[IndGlbInfOts].m := (ipic - 1) * 100 + 10;
        GlbInfOts[IndGlbInfOts].prich_ogr_v := GlbTempTipRemontPiket;
        GlbInfOts[IndGlbInfOts].stype := 2;
        GlbInfOts[IndGlbInfOts].st := 5;
        IndGlbInfOts := IndGlbInfOts + 1;

      end;
      ipic := ipic + 1;
    end;

  end;

end;

// ------------------------------------------------------------------------------
// BALL  Ust V > 60
// ------------------------------------------------------------------------------
function GetBall(cou2, sum2s_pro_per_rih, Sum3_urv_pro_per_rih, CouShab3,
  cou4: integer): integer;
var
  k: integer;
begin
  if cou2 <= 5 then
    k := 10;
  if (6 <= cou2) and (cou2 <= 25) then
    k := 40;

  if ((25 < cou2) and (cou2 <= 100)) or
    ((25 < sum2s_pro_per_rih) and (sum2s_pro_per_rih <= 60)) then
    k := 150;
  if (1 <= Sum3_urv_pro_per_rih) and (Sum3_urv_pro_per_rih <= 6) then
    k := 150;

  if (1 <= CouShab3) and (CouShab3 <= 10) then
    k := 150;

  // if (6 < Sum3_urv_pro_per_rih) or ((CouShab3 > 10)) then
  if (6 < Sum3_urv_pro_per_rih) then
  begin
    k := 500;
    if (6 < Sum3_urv_pro_per_rih) then
      ball500_sebep := '3cт V=60';
    // if (CouShab3 > 10) then
    // ball500_sebep := 'Ш10 V=60';
    // Glob_primech:= ball500_sebep;
  end;

  // K100 or K60
  // if (60 < sum2s_pro_per_rih) then
  // begin
  // k := 500;
  // ball500_sebep := ball500_sebep + ' K60';
  // // Glob_primech:= ball500_sebep;
  // end;
  //
  // if (100 < cou2) then
  // begin
  // k := 500;
  // ball500_sebep := ball500_sebep + ' K100 V=60/60';
  // if GlbCountDGR = 0 then
  // GlbCountDGR := GlbCountDGR + 1;
  // // Glob_primech:= ball500_sebep;
  // end;

  // if (1 <= cou4) and ((GlobUbedOgr <> '') or GlbOgrSkorKm) then begin
  if (1 <= cou4) then
  begin
    k := 500;
    ball500_sebep := '4ст';
    // if not GlbOgrSkorKm then Glob_primech:= str_4st;
  end;

  if GlbFlagRemontKm and not GlbOgrSkorKm and (k = 500) then
    k := 500; // 150; //02.07.2013
  // sablog('k='+ inttostr(k));
  if GlbFlagCorrKm then // GlbFlagRemontKm or
    case k of
      500:
        k := 150;
      150:
        k := 40;
      40:
        k := 10;
    end;

  if GlbOgrSkorKm then
    k := 500;

  GetBall := k;
end;

// ------------------------------------------------------------------------------
// BALL  Ust V <= 60
// ------------------------------------------------------------------------------
function GetBallDo60(cou2, Sum3_urv_pro_per_rih, CouShab3,
  cou4: integer): integer;
var
  k: integer;
  txt: string;
begin
  ball500_sebep := '';
  if cou2 <= 3 then
    k := 10;
  if (4 <= cou2) and (cou2 <= 12) then
    k := 40;

  if (12 < cou2) then
    k := 150;
  if (1 <= Sum3_urv_pro_per_rih) and (Sum3_urv_pro_per_rih <= 3) then
    k := 150;
  if (1 <= CouShab3) and (CouShab3 <= 10) then
    k := 150;

  if (3 < Sum3_urv_pro_per_rih) or ((CouShab3 > 10)) then
  begin
    k := 500;
    // -----------------

    // -----------------------
    if (3 < Sum3_urv_pro_per_rih) then
    begin
      ball500_sebep := '3cт;';
      GlbCountDGR := GlbCountDGR + 1
    end;
    // if (CouShab3 > 10) then
    // begin
    // ball500_sebep:= ball500_sebep + 'Ш10;';  GlbCountDGR:= GlbCountDGR + 1
    // end;
    if (length(GlobUbedOgr) + length(ball500_sebep) < 254) then
    begin
      GlobUbedOgr := GlobUbedOgr + ball500_sebep;
    end;
    if (length(ball500_sebep) > 0) then
      Ubedom_db(0, 1, 0, 0, 5, GlobPassSkorost, GlobGruzSkorost, -1, -1,
        ball500_sebep, '', -1, false, false, false, false);
  end;

  // if (0 < cou4) and (GlobUbedOgr <> '') then begin
  // if (1 <= cou4) and ((GlobUbedOgr <> '') or GlbOgrSkorKm) then begin
  if (1 <= cou4) then
  begin
    k := 500;
    // if not GlbOgrSkorKm then Glob_primech:= str_4st;
    ball500_sebep := '4ст';
  end;

  if GlbFlagRemontKm and not GlbOgrSkorKm and (k = 500) then
    k := 500; // 150; //02.07.2013

  if GlbFlagCorrKm then // GlbFlagRemontKm or
    case k of
      500:
        k := 150;
      150:
        k := 40;
      40:
        k := 10;
    end;
  if GlbOgrSkorKm then
    k := 500; // 09.11.2011

  GetBallDo60 := k;
end;

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
procedure UbedStr;
begin
  UbedOgr := GlobUbedOgr;
end;

// ------------------------------------------------------------------------------
// ЗАПИСЬ В ТАБ. BEDOMOST  new
// ------------------------------------------------------------------------------
PROCEDURE BEDOMOST(TKMTrue: integer);
var
  otsenka, sprim, sq, s: string;
  k, ball, st2, st2_pro_per_rih, st3_shb, st3_urv_pro_per_rih, st4, i, j, nn,
    kk: integer;
BEGIN
  TRY
    if Flag_sablog then
      SabLog('BEDOMOST - запись данных таблицу Ведомость');

    nn := length(Glb3);
    if (nn > 150) then
    begin
      kk := nn - 150;
      delete(Glb3, 149, kk);

    end;

    WITH MainDataModule DO
    BEGIN
      // ActiveTransact2;
      spBedemost.ParamByName('pch').Value := inttostr(Glb_PutList_PCH);
      spBedemost.ParamByName('naprav').Value := Glb_PutList_GNapr;
      spBedemost.ParamByName('put').Value := GlbNumPut; // NUMPUT;
      spBedemost.ParamByName('track_id').Value := GlbTrackId; // NUMPUT;
      spBedemost.ParamByName('pchu').Value := Glb_PList_PCHU;
      spBedemost.ParamByName('pd').Value := Glb_PList_PD;
      spBedemost.ParamByName('pdb').Value := Glb_PList_PDB;
      spBedemost.ParamByName('km').Value := GlbKmtrue;
      spBedemost.ParamByName('kmtrue').Value := TKMTrue;
      spBedemost.ParamByName('suj_2').Value := suj_s2;
      spBedemost.ParamByName('suj_3').Value := suj_s3;
      spBedemost.ParamByName('suj_4').Value := glbCount_suj4s;
      // suj_s4;

      spBedemost.ParamByName('ush_2').Value := ush_s2;
      spBedemost.ParamByName('ush_3').Value := ush_s3;
      spBedemost.ParamByName('ush_4').Value := glbCount_Ush4s;
      // ush_s4;

      spBedemost.ParamByName('pro_p2').Value := pro1_s2;
      spBedemost.ParamByName('pro_p3').Value := pro1_s3;
      spBedemost.ParamByName('pro_p4').Value := glbCount_1Pro4s;
      // pro1_s4;

      spBedemost.ParamByName('pro_l2').Value := pro2_s2;
      spBedemost.ParamByName('pro_l3').Value := pro2_s3;
      spBedemost.ParamByName('pro_l4').Value := glbCount_2Pro4s;
      // pro2_s4;

      spBedemost.ParamByName('per_2').Value := per_s2;
      spBedemost.ParamByName('per_3').Value := per_s3;
      spBedemost.ParamByName('per_4').Value := glbCount_Per4s;
      // per_s4;

      spBedemost.ParamByName('urb_2').Value := pot_s2;
      spBedemost.ParamByName('urb_3').Value := pot_s3;
      spBedemost.ParamByName('urb_4').Value := glbCount_Urv4s;
      // pot_s4;

      spBedemost.ParamByName('rih_p2').Value := rih1_s2;
      spBedemost.ParamByName('rih_p3').Value := rih1_s3;
      spBedemost.ParamByName('rih_p4').Value := glbCount_Rih4s;
      // rih1_s4;
      spBedemost.ParamByName('rih_l2').Value := rih2_s2;
      spBedemost.ParamByName('rih_l3').Value := rih2_s3;
      spBedemost.ParamByName('rih_l4').Value := 0; // rih2_s4;

      // ******************************************************************************

      if (ball = 500) and (GlobUbedOgr = '') and (ball500_sebep = '') and
        (GlbOgrPasGrz <> '-/-') then
        GlbOgrPasGrz := '-/-';

      // ******************************************************************************


      spBedemost.ParamByName('ots_iv_st').Value := GlbOgrPasGrz;
      // Glob_primech + GlobUbedOgr;

      spBedemost.ParamByName('korr_pasdat').Value := GlbCommentPaspData;

      st2 := Shab_s2 + rih_s2 + pro_s2 + urb_s2;
      st3_urv_pro_per_rih := pot_s3 + pro_s3 + per_s3 + rih_s3;

      st4 := glbCount_Rih4s + glbCount_Ush4s + glbCount_suj4s + glbCount_Urv4s +
        glbCount_Per4s + glbCount_1Pro4s + glbCount_2Pro4s;
      // Shab_s4 + rih_s4 + pro_s4 + urb_s4;

      st3_shb := Shab_s3;
      st2_pro_per_rih := pro_s2 + per_s2 + rih_s2;

      if GlobPassSkorost <= 60 then
      //  ball := GetBallDo60(st2, st3_urv_pro_per_rih, st3_shb, st4)
                ball := 0
      else
       // ball := GetBall(st2, st2_pro_per_rih, st3_urv_pro_per_rih,st3_shb, st4);
          ball := 0;
      if (ball = 500) and (ball500_sebep <> '') and (GlobUbedOgr = '') then
        GlobUbedOgr := ball500_sebep; // + ',' + GlobUbedOgr;
      // ******************************************************************************

      if (length(Glob_primech) > 0) and (ball <> 500) then
       // ball := 500;
            ball := 0 ;
      GlobBall := ball;
      spBedemost.ParamByName('ball').Value := ball;

      if GlbFlagRemontKm or GlbFlagCorrKm then
      begin

        spBedemost.ParamByName('rem_kor').Value := GlbTempTipRemontKm;
      end
      else
      begin

        spBedemost.ParamByName('rem_kor').Value := ' ';
      end;

      if length(GlobUbedOgr) > 240 then
        delete(GlobUbedOgr, 240, length(GlobUbedOgr)); // 1511_2012

      if GlbFlagRemontKm or GlbFlagCorrKm then
      begin

        spBedemost.ParamByName('primech').Value := GlbTempTipRemontKm +
          GlobUbedOgr;

      end
      else
      begin

        spBedemost.ParamByName('primech').Value := GlobUbedOgr;
      end;

      spBedemost.ParamByName('tip_poezdki').Value := GTipPoezdki;
      spBedemost.ParamByName('trip_id').Value := GTripId;
      // now;

      spBedemost.ParamByName('fio_pd').Value := Glb_FIO_PD_master;
      spBedemost.ParamByName('pch_name').Value := Glb_PCH_name;

      spBedemost.ParamByName('f_rem').Value := GlbCountRemKm;
      spBedemost.ParamByName('f_ukl').Value := GlbCountUKL;
      spBedemost.ParamByName('f_usk').Value := GlbCountUSK;
      spBedemost.ParamByName('f_soch').Value := GlbCountSOCH;
      spBedemost.ParamByName('f_drg').Value := GlbCountDGR;
      spBedemost.ParamByName('fdbroad').Value := fdBroad;
      spBedemost.ParamByName('fdconstrict').Value := fdConstrict;
      spBedemost.ParamByName('fdskew').Value := fdSkew;
      spBedemost.ParamByName('fddrawdown').Value := fdDrawdown;
      spBedemost.ParamByName('fdstright').Value := fdStright;
      spBedemost.ParamByName('fdlevel').Value := fdLevel;

      spBedemost.ParamByName('lkm').Value := LengthKm;
      // Length_all_Km

      spBedemost.ExecProc;
      // spBedemost.Connection.Close;
      // CommitTransact2;
      if gkmtrueind = 0 then
        gkmtrue := TKMTrue;
      gkmtrueind := gkmtrueind + 1;
    END; // WITH1
  EXCEPT
  END;
END;

// ------------------------------------------------------------------------------
// ФИЛЬТРАЦИЯ ОТСТУПЛЕНИЙ 3 И 4 СТ. РИХТОВКИ И ШАБЛОНА ПРИ СОЧЕТАНИЙ
// ------------------------------------------------------------------------------
PROCEDURE Filter3and4_RihShablon;
var
  i, j, a, b, c, d: integer;
begin

  for i := 0 to high(W1Ush) do
  begin
    if (W1Ush[i].st = 3) or (W1Ush[i].st = 4) then
    begin
      a := W1Ush[i].L0;
      b := W1Ush[i].Lm;

      for j := 0 to high(W1Rst) do
      begin
        if (W1Rst[j].st = 3) or (W1Rst[j].st = 4) then
        begin
          c := W1Rst[j].L0;
          d := W1Rst[j].Lm;

          if (((a <= c) and (c <= b)) or ((a >= c) and (c >= b))) or
            (((a <= d) and (d <= b)) or ((a >= d) and (d >= b))) then
          begin

            W1Ush[i].st := 2;
            W1Ush[i].L0 := 0;
            W1Ush[i].Lm := 0;
            W1Rst[j].st := 2;
            W2Rst[j].st := 2;
            W1Rst[j].L0 := 0;
            W1Rst[j].Lm := 0;
            W2Rst[j].L0 := 0;
            W2Rst[j].Lm := 0;
          end; // if
        end; // if
      end; // for
    end; // if
  end; // for

  for i := 0 to high(W1Suj) do
  begin
    if (W1Suj[i].st = 3) or (W1Suj[i].st = 4) then
    begin
      a := W1Suj[i].L0;
      b := W1Suj[i].Lm;
      for j := 0 to high(W1Rst) do
      begin
        if (W1Rst[j].st = 3) or (W1Rst[j].st = 4) then
        begin
          c := W1Rst[j].L0;
          d := W1Rst[j].Lm;

          if (((a <= c) and (c <= b)) or ((a >= c) and (c >= b))) or
            (((a <= d) and (d <= b)) or ((a >= d) and (d >= b))) then
          begin
            W1Suj[i].st := 2;
            W1Suj[i].L0 := 0;
            W1Suj[i].Lm := 0;
            W1Rst[j].st := 2;
            W2Rst[j].st := 2;
            W1Rst[j].L0 := 0;
            W1Rst[j].Lm := 0;
            W2Rst[j].L0 := 0;
            W2Rst[j].Lm := 0;
          end; // if
        end; // if
      end; // for
    end; // if
  end; // for

end;

// ------------------------------------------------------------------------------
// Корректировка отс 4 ст на 3 ст
// ------------------------------------------------------------------------------
PROCEDURE KORRECT;
var
  fkorr: textfile;
  arr_km, i: integer;
begin
  if FileExists('Km_U4t3.txt') then
  begin
    AssignFile(fkorr, 'Km_U4t3.txt');
    reset(fkorr);

    while not eof(fkorr) do
    begin
      readln(fkorr, arr_km);
      if (arr_km = GlbKmtrue) then
      begin

        for i := 0 to High(W1Ush) do
          if W1Ush[i].st = 4 then
            W1Ush[i].st := 3;

        for i := 0 to High(W1Suj) do
          if W1Suj[i].st = 4 then
            W1Suj[i].st := 3;

        for i := 0 to High(W1pro1) do
          if W1pro1[i].st = 4 then
            W1pro1[i].st := 3;

        for i := 0 to High(W1pro2) do
          if W1pro2[i].st = 4 then
            W1pro2[i].st := 3;

        for i := 0 to High(W1Per) do
          if W1Per[i].st = 4 then
            W1Per[i].st := 3;

        for i := 0 to High(W1Pot) do
          if W1Pot[i].st = 4 then
            W1Pot[i].st := 3;

        for i := 0 to High(W1Rst) do
          if W1Rst[i].st = 4 then
            W1Rst[i].st := 3;

        for i := 0 to High(W2Rst) do
          if W2Rst[i].st = 4 then
            W2Rst[i].st := 3;
      end; // if
    end; // while

    CloseFile(fkorr);
  end;
end;

// ------------------------------------------------------------------------------
// Количество I, II, III - отступлений разных отступлений
// ------------------------------------------------------------------------------
PROCEDURE Kol_Ots;
var
  i, xa, xb, pik, leng, bolshek, count, count1, count2, vop, vog: integer;
  ts2, ts3, ts4, tu2, tu3, tu4: integer;
  pstr, ots: string;
begin
  pstr := '';
  ush_s3 := ush_s3 + GlbCou_3stShab_UklOtb;

  { ts2:= Suj_s2;
    ts3:= Suj_s3;
    ts4:= glbCount_suj4s;
    tu2:= ush_s2;
    tu3:= ush_s3;
    tu4:= glbCount_Ush4s;

    if (ts3 > ts4) then Suj_s3:= ts3 - ts4;
    if (tu3 > tu4) then ush_s3:= tu3 - tu4;

    if (ts2 > ts3) then Suj_s2:= ts2 - ts3;
    if (tu2 > tu3) then ush_s2:= tu2 - tu3; }

  Shab_s2 := suj_s2 + ush_s2;
  Shab_s3 := suj_s3 + ush_s3;
  Shab_s4 := glbCount_Ush4s + glbCount_suj4s; // Suj_s4 + ush_s4;

  // rih_s2:= rih1_s2 + rih2_s2; rih_s3:= rih1_s3 + rih2_s3;
  rih_s4 := glbCount_Rih4s; // rih1_s4 + rih2_s4;

  // pro_s2:= pro1_s2 + pro2_s2; pro_s3:= pro1_s3 + pro2_s3;
  pro_s4 := glbCount_1Pro4s + glbCount_2Pro4s; // pro1_s4 + pro2_s4;

  urb_s2 := pot_s2 + per_s2;
  urb_s3 := pot_s3 + per_s3;
  urb_s4 := glbCount_Urv4s + glbCount_Per4s; // pot_s4 + per_s4;

  if (pro_s3 + rih_s3 + urb_s3 > 60) and (GlobPassSkorost > 60) then
  begin
    vop := -1;
    vog := -1;
    ots := '3ст';
    if GlobPassSkorost > 60 then
      vop := V_res('UPPR', 0, GlobPassSkorost); // vop:= 60;
    if GlobGruzSkorost > 60 then
      vog := V_res('UPPR', 0, GlobGruzSkorost);
    // if GlobGruzSkorost > 60 then vog:= -1;//V_res('UPPR', 1, GlobGruzSkorost);//vog:= 60;
    if (vop >= GlobGruzSkorost) and (GlobPassSkorost > 120) then
      vog := -1;
    if (RGlobPassSkorost < 60) and GlbFlagRemontKm then
    begin
      vop := -1;
      pstr := GlbTempTipRemontKm;
    end;
    if (RGlobGruzSkorost < 60) and GlbFlagRemontKm then
    begin
      vog := -1;
      pstr := GlbTempTipRemontKm;
    end;

    Glob_primech := '3ст V=' + V_shekti(vop, vog) + ';';
    if (vop = -1) and (vog = -1) and GlbFlagRemontKm then
      GlobUbedOgr := GlobUbedOgr + Glob_primech;
    WRT_UBEDOM(0, 0, 7, Glob_primech, vop, vog);
    Ubedom_db(0, 1, 0, 0, 5, GlobPassSkorost, GlobGruzSkorost, vop, vog, ots,
      pstr, -1, false, false, false, false); // GlbTempTipRemontKm);
    GlbCountDGR := GlbCountDGR + 1;
  end;

  if (pro_s3 + rih_s3 + urb_s3 > 3) and (GlobPassSkorost <= 60) then
  begin

    vop := -1;
    vog := -1;
    ots := '3ст';
    if GlbFlagRemontKm then
      pstr := GlbTempTipRemontKm;

    if ((GlobPassSkorost <= 60) and (GlobPassSkorost > 40) and
      not(GlbFlagRemontKm)) then
      vop := 40;
    if ((GlobPassSkorost <= 40) and not(GlbFlagRemontKm)) then
      vop := 15;

    if ((GlobGruzSkorost <= 60) and (GlobGruzSkorost > 40) and
      not(GlbFlagRemontKm)) then
      vog := 40;
    if (GlobGruzSkorost <= 40) and not(GlbFlagRemontKm) then
      vog := 15;

    if (vop >= GlobGruzSkorost) and (GlobGruzSkorost > 120) then
      vog := -1;
    if (vop = -1) and (vog = -1) and GlbFlagRemontKm then
      GlobUbedOgr := GlobUbedOgr + Glob_primech;
    Glob_primech := '3ст V=' + V_shekti(vop, vog) + ';';
    WRT_UBEDOM(0, 0, 7, Glob_primech, vop, vog);
    Ubedom_db(0, 1, 0, 0, 5, GlobPassSkorost, GlobGruzSkorost, vop, vog, ots,
      pstr, -1, false, false, false, false); // GlbTempTipRemontKm);
    GlbCountDGR := GlbCountDGR + 1;

  end;

  // if (Shab_s3 > 10) or (newCountSuj > 10) then
  // begin
  // vop := -1;
  // vog := -1;
  // ots := 'Ш10';
  //
  // if (newCountUsh + newCountSuj < 10) then
  // begin
  // if (GlobPassSkorost > 100) then
  // vop := 100
  // else
  // vop := 60;
  //
  // if (vop < GlobGruzSkorost) then
  // begin
  // if GlobGruzSkorost > 80 then
  // vog := 80
  // else
  //
  // if GlobGruzSkorost > 60 then
  // vog := 60;
  // end;
  // if (RGlobPassSkorost < 60) and GlbFlagRemontKm then
  // begin
  // vop := -1;
  // pstr := GlbTempTipRemontKm;
  // end;
  // if (RGlobGruzSkorost < 60) and GlbFlagRemontKm then
  // begin
  // vog := -1;
  // pstr := GlbTempTipRemontKm;
  // end;
  // end;
  // if (newCountUsh + newCountSuj > 10) then
  // begin
  // vop := 25;
  // vog := 25;
  //
  // if (RGlobPassSkorost < 25) and GlbFlagRemontKm then
  // begin
  // vop := -1;
  // pstr := GlbTempTipRemontKm;
  // end;
  // if (RGlobGruzSkorost < 25) and GlbFlagRemontKm then
  // begin
  // vog := -1;
  // pstr := GlbTempTipRemontKm;
  // end;
  // end;
  // if (Flag_sablog) then
  //
  // SabLog('проверка сужения ич  10 отсуплении');
  // /// /        проверка сужения ич  10 отсуплении
  // if (newCountSuj > 10) then
  // begin
  // vop := 25;
  // vog := 25;
  //
  // if (RGlobPassSkorost < 25) and GlbFlagRemontKm then
  // begin
  // vop := -1;
  // pstr := GlbTempTipRemontKm;
  // end;
  // if (RGlobGruzSkorost < 25) and GlbFlagRemontKm then
  // begin
  // vog := -1;
  // pstr := GlbTempTipRemontKm;
  // end;
  // if (Flag_sablog) then
  // SabLog('нашли ич  10 отсуплении' + '  ' + inttostr(vop) + '  ' +
  // inttostr(vog));
  // end;
  // if GlobPassSkorost < vop then
  // begin
  // vop := -1;
  // vog := -1;
  // end;
  //
  // ///
  //
  // Glob_primech := 'Ш10 V=' + V_shekti(vop, vog) + ';';
  // WRT_UBEDOM(0, 0, 7, Glob_primech, vop, vog);
  // // showmessage(pstr);
  // Ubedom_db(0, 1, 0, 0, 5, GlobPassSkorost, GlobGruzSkorost, vop, vog, ots,
  // pstr, -1, false, false, false); // GlbTempTipRemontKm);
  // GlbCountDGR := GlbCountDGR + 1;
  // newCountUsh := 0;
  // newCountSuj := 0;
  // end;

end;

// ------------------------------------------------------------------------------
// ОПР. ИНФ. О ОТСТУПЛЕНИЯХ.
// ------------------------------------------------------------------------------
PROCEDURE RWTB_INFOTS(pkm: integer);
LABEL
  Kelesi;
var
  i, j, iiii, kkkkk: integer;
  przh_var: boolean;
BEGIN
  przh_var := false;
  TRY
    if Flag_sablog then
      Writeln('RWTB_INFOTS - Главная процедура определения информации по различным видам отступлений');
    TEKKM := pkm;
    TekKmTrue := GlbKmtrue; //

    SearchSiezd;

    KrivoiNatur(GlbKmtrue);

    // PCH_F := inttostr(Glb_PutList_PCH);//GlbPCH;

    // izk := 0;

    if (zkm <> TEKKM) and (zkmtrue = TekKmTrue) then
      izk := izk + 1
    else
      izk := 0;

    // if not fileexists(pathMB +  'GRAPHREP\' + 'G_' + IntToStr(TekKmTrue) + '.qrp') then izk:= 0;
    // else izk:= izk + 1;

    flgRemont := false;

    NecorrPasportFlag := false;

    GUroven_4_PaspData(Furbx, F_mtr, F0_urov);

    // ------------------------------------------------------------------------------
    // ОПР. ОТС., СТ., L0 B LM ПО УШИРЕНИЮ
    Setlength(W1Ush, 3000);
    ns := 0;
    // GUsh_1548mm(F_sh, X_k, F_Rad, W1Ush);   //02.10.2012

    for i := 0 to high(F_sh) do
      F_sht[i] := F_sh[i];

    // Tolegen 17.03.2021
    // Убираем суж и уш на 1м
    // 1 итер
    for kkkkk := 0 to high(F_sh) - 3 do
    begin
      if ((F_sh[kkkkk] < F_sh[kkkkk + 1]) and
        (F_sh[kkkkk + 1] > F_sh[kkkkk + 2])) then
      begin
        // showmessage(floattostr(F_sh[kkkkk+1])+ ' '+ floattostr(F_sh11[kkkkk+1]));
        F_sht[kkkkk + 1] := F_sh[kkkkk + 2];

        if (F_sh[kkkkk] > F_sh[kkkkk + 2]) then
          F_sht[kkkkk + 1] := F_sh[kkkkk];
        // showmessage(floattostr(F_sh[kkkkk+1])+ ' '+ floattostr(F_sh11[kkkkk+1]));
      end;

      if ((F_sh[kkkkk] > F_sh[kkkkk + 1]) and
        (F_sh[kkkkk + 1] < F_sh[kkkkk + 2])) then
      begin

        F_sht[kkkkk + 1] := F_sh[kkkkk];

        if (F_sh[kkkkk] < F_sh[kkkkk + 2]) then
          F_sht[kkkkk + 1] := F_sh[kkkkk + 2];
      end;
    end;
    // 2 итер
    for i := 0 to high(F_sh) do
      F_sht11[i] := F_sht[i];

    for kkkkk := 0 to high(F_sht11) - 3 do
    begin
      if ((F_sht11[kkkkk] < F_sht11[kkkkk + 1]) and
        (F_sht11[kkkkk + 1] > F_sht11[kkkkk + 2])) then
      begin
        // showmessage(floattostr(F_sh[kkkkk+1])+ ' '+ floattostr(F_sh11[kkkkk+1]));
        F_sht[kkkkk + 1] := F_sht11[kkkkk + 2];

        if (F_sht11[kkkkk] > F_sht11[kkkkk + 2]) then
          F_sht[kkkkk + 1] := F_sht11[kkkkk];
        // showmessage(floattostr(F_sh[kkkkk+1])+ ' '+ floattostr(F_sh11[kkkkk+1]));
      end;

      if ((F_sht11[kkkkk] > F_sht11[kkkkk + 1]) and
        (F_sht11[kkkkk + 1] < F_sht11[kkkkk + 2])) then
      begin

        F_sht[kkkkk + 1] := F_sht11[kkkkk];

        if (F_sht11[kkkkk] < F_sh[kkkkk + 2]) then
          F_sht[kkkkk + 1] := F_sh[kkkkk + 2];
      end;
    end;
    GUsh_44(F_sh, F_mtr, F_Rad, W1Ush);
    // GUsh_4(F_sh, F_mtr, F_Rad, W1Ush); // shi +
    GUsh_3(F_sh, F_mtr, F_Rad, W1Ush);
    GUsh_2(F_sh, F_mtr, F_Rad, W1Ush);
    GUsh_1(F_sh, F_mtr, F_Rad, W1Ush);
    if Flag_sablog then
      Writeln('Уширение');
    // ------------------------------------------------------------------------------
    // ОПР. ОТС., СТ., L0 B LM ПО СУЖЖЕНИЮ

    for i := 0 to high(F_sh) do
      F_sht[i] := F_sh[i];

    Setlength(W1Suj, 3000);
    ns := 0;

    // GetFsuj_1512mm(F_sh, F_mtr, F_Rad, Shpaly, W1Suj);
    GetFsuj_4(F_sh, F_mtr, F_Rad, Shpaly, W1Suj); // shi +
    GetFsuj_3(F_sh, F_mtr, F_Rad, Shpaly, W1Suj); // shi +
    GetFsuj_2(F_sh, F_mtr, F_Rad, Shpaly, W1Suj); // shi +   // 05.06.2013
    GetFsuj_1(F_sh, F_mtr, F_Rad, Shpaly, W1Suj); // shi +   // 05.06.2013

    // ------------------------------------------------------------------------------
    if Flag_sablog then
      Writeln('Сужение');
    if ush_s3 > 10 then
    begin
      for i := 0 to high(F_sh) do
        F_sht[i] := F_sh[i];
      newCountUsh := GetCountUsh3(F_sh, F_mtr, F_Rad, 26);
    end;

    if suj_s3 > 10 then
    begin
      for i := 0 to high(F_sh) do
        F_sht[i] := F_sh[i];
      newCountSuj := GetCountSuj3(F_sh, F_mtr, F_Rad, Shpaly, 26);
    end;
    // ------------------------------------------------------------------------------
    // ОПР. ОТС., СТ., L0 B LM ПО ПРОСАДКЕ ПР.
    pnt := 'п';
    if tanba_rih < 0 then
      pnt := 'л';
    ns := 0;
    Setlength(W1pro1, 3000);

    // GetProsadka(pnt, F_Pr1, X_k, W1Pro1);
    GetPro(pnt, F_Pr1, F_mtr, W1pro1);
    if Flag_sablog then
      Writeln('Просадка');
    // ------------------------------------------------------------------------------
    // ОПР. ОТС., СТ., L0 B LM ПО ПРОСАДКЕ ЛЕВ.
    pnt := 'л';
    if tanba_rih < 0 then
      pnt := 'п';
    ns := 0;
    Setlength(W1pro2, 3000);

    GetPro(pnt, F_Pr2, F_mtr, W1pro2);

    // ------------------------------------------------------------------------------
    // ОПР. ОТС., СТ., L0 B LM ПО ПЕРЕКОСУ.
   pnt := 'п';
    ns := 0;
    Setlength(W1Per, 3000);

    F_urb_Per1 := F_urb_Per;
    GetPro_Perekos(pnt, F_urb_Per1, F_mtr,TrapezLevel_Get_per,AvgTr, W1Per);
       //    procedure GetRiht(Fm, FmK, FmTrapez: mas; var WRih: masots; isriht: boolean);
     //GetPerekos(F_urb_Per, F_urb_Per_sr, F_mtr, W1Per);
    /// GetPro_perekos(F_urb_Per, F_urb_Per_sr, F_mtr,  W1Per);

    // GetPerekosLong(F_urb_Per, F_urb_Per_sr, F_mtr, W1Per);

    if Flag_sablog then
      Writeln('Перекос');
    // ------------------------------------------------------------------------------
    // ОПР. ОТС., СТ., L0 B LM ПО ПЛАВН. ОТКЛ.
    ns := 0;
    Setlength(W1Pot, 3000);

    // GUroven150(Furb1, F_mtr, F_urb_Per_sr, W1Pot);
    // GUroven75(Furb1, F_mtr, F_urb_Per_sr, W1Pot);
    GUroven4(Furb1, F_mtr, F_urb_Per_sr, W1Pot);
    GUroven3(Furb1, F_mtr, F_urb_Per_sr, W1Pot);
    GUroven2(Furb1, F_mtr, F_urb_Per_sr, W1Pot);
    GUroven1(Furb1, F_mtr, F_urb_Per_sr, W1Pot);

    ns := 0;
    Setlength(W1Rst, 3000);

    // GetRihtovka(F_fluk, X_k, W1Rst);

    // 13.08.2015

    TrapezStr[0] := TrapezStr[0];
    // рихтовканы анықтау
    Fluc_Define(Fluk_right, Fluk_left, Rih_Nit);

    GetRiht(F_fluk, F_mtr, TrapezStr, W1Rst, true);

    GetRiht(F_fluk_notriht, F_mtr, TrapezStr, W1Rst, false);    // если не ннужно считать в количествоо РСТ

    //GetRiht(F_fluk_notriht, F_mtr, TrapezStr, W1Rst, false);
    // если  ннужно считать в количествоо РСТ
    if Flag_sablog then
      Writeln('Рихтовка');

    // GetRihtovka(F_fluk2, X_k, W1Rst);
    // Rst2--------------------------------------------------------------------------
    // ОПР. ОТС., СТ., L0 B LM ПО РИХТОВКЕ ЛЕВ.
    {
      Setlength(W2Rst,3000);
      GetRihtovka(F_fluk, X_k, W2Rst);
      Datamodule2.RefreshMainForm;
    }
    // Обработка участка моста и тоннелла
    Most_Tonnel(W1pro1, 'Пр.п');
    Most_Tonnel(W1pro2, 'Пр.л');
    Most_Tonnel(W1Per, 'П');
    Most_Tonnel(W1Pot, 'У');
    Most_Tonnel(W1Rst, 'Р');
    // ------------------------------------------------------------------------------
    // Коррекция км. Изменение отступлений 4 ст на 3 ст
    KORRECT;

    Kol_Ots;
    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    if przh_var then
    begin
      // PRZH;
    end;

    // opredelenie sochetanii  az
    if GlobPassSkorost > 15 then
    begin
      OgrPoSochetaniu(W1Rst, W1Per, 3, 3, 1, 'Р+П');
      // ' Сочетание Рих.3 ст с Пер.3 ст');
      OgrPoSochetaniu(W1Rst, W1pro1, 3, 3, 1, 'Р+Пр');
      // ' Сочетание Рих.3 ст с Пр.3 ст');
      OgrPoSochetaniu(W1Rst, W1pro2, 3, 3, 1, 'Р+Пр');
      // ' Сочетание Рих.3 ст с Пр.3 ст');
    end;

    OgrPoSochetaniu(W1Rst, W1Per, 3, 4, 2, 'Р+П');
    // ' Сочетание Рих.3 ст с Пер.4 ст');
    OgrPoSochetaniu(W1Rst, W1pro1, 3, 4, 2, 'Р+Пр');
    // 'Сочетание Рих.3 ст с Пр.п. 4 ст');
    OgrPoSochetaniu(W1Rst, W1pro2, 3, 4, 2, 'Р+Пр');
    // 'Сочетание Рих.3 ст с Пр.л.4 ст');
    OgrPoSochetaniu(W1Rst, W1Per, 4, 3, 2, 'Р+П');
    // 'Сочетание Рих.4 ст с Пер.3 ст');
    OgrPoSochetaniu(W1Rst, W1pro1, 4, 3, 2, 'Р+Пр');
    // 'Сочетание Рих.4 ст с Пр.п.3 ст');
    OgrPoSochetaniu(W1Rst, W1pro2, 4, 3, 2, 'Р+Пр');
    // 'Сочетание Рих.4 ст с Пр.л.3 ст');
    OgrPoSochetaniu(W1Rst, W1Per, 4, 4, 2, 'Р+П');
    // 'Сочетание Рих.4 ст с Пер.4 ст');
    OgrPoSochetaniu(W1Rst, W1pro1, 4, 4, 2, 'Р+Пр');
    // 'Сочетание Рих.4 ст с Пр.п.4 ст');
    OgrPoSochetaniu(W1Rst, W1pro2, 4, 4, 2, 'Р+Пр');
    // 'Сочетание Рих.4 ст с Пр.л.4 ст');
    /// opredelenie smezhnyh otstuplenii
    OgrSmezhnyhOts_Riht(W1Rst, 3, 3, 75); //
    // OgrSmezhnyhOts_Riht(W2Rst,3,3,75); //

    // -------------
    OgrSmezh_Prosadka(W1pro1, W1pro2); // 20.06.2012
    {
      if (GlobPassSkorost > 60) or (GlobGruzSkorost > 60) then begin
      OgrSmezhnyhOts_Prosadka_st2(W1pro1,2,4,3000);//massiv, ctepen, kolichestvo, dlina
      OgrSmezhnyhOts_Prosadka_st2(W1Pro2,2,4,3000);//massiv, ctepen, kolichestvo, dlina
      end;
    }
    // OgrSkorostiNaKriv;   // ОГРАНИЧЕНИЕ СКОРОСТИ НА КРИВОМ УЧАСТКЕ груз. поездов в составе порожн. вогон

    { IndGlbInfOts:= 0;
      setlength(GlbInfOts, 3000); } // 02.10.2012
    // ActiveTransact2;
    // MainDataModule.pgsConnection.Connected := false;

    WRT_INFOTS('USH', W1Ush);
    WRT_INFOTS('SUJ', W1Suj);
    WRT_INFOTS('PR1', W1pro1);
    WRT_INFOTS('PR2', W1pro2);
    WRT_INFOTS('PER', W1Per);
    WRT_INFOTS('URB', W1Pot);

    WRT_INFOTS('RH1', W1Rst);

    if Flag_sablog then
      Writeln('Пишем отступление');
    // WRT_INFOTS('PR1', W1pro1);
    // WRT_INFOTS('PR2', W1pro2);

    /// WRT_Ogr;  //01.10.2012
    /// WRT_RemKorrInf;
    Setlength(GlbInfOts, IndGlbInfOts);
    // CommitTransact2;
    if Flag_sablog then
      Writeln('Сделали тарнзакцию');
    { //10.12.2012
      case glbTipOgrV of
      1: GlbCountUKL:=   GlbCountUKL + 1;
      2: GlbCountSOCH:=  GlbCountSOCH + 1;
      3: GlbCountUSK:=   GlbCountUSK + 1;
      6,7:  GlbCountDGR:=   GlbCountDGR + 1; // Smezh, k100, k60, 3ст>6шт, 3ст>10шт , krutizna otb shiriny
      end;
    }

    UbedStr;

    if (Glb_NestKm_count = 1) then
    begin
      suj_s2_s := suj_s2;
      suj_s3_s := suj_s3;
      glbCount_suj4s_s := glbCount_suj4s;
      ush_s2_s := ush_s2;
      ush_s3_s := ush_s3;
      glbCount_Ush4s_s := glbCount_Ush4s;
      pro1_s2_s := pro1_s2;
      pro1_s3_s := pro1_s3;
      glbCount_1Pro4s_s := glbCount_1Pro4s;
      pro2_s2_s := pro2_s2;
      pro2_s3_s := pro2_s3;
      glbCount_2Pro4s_s := glbCount_2Pro4s;
      per_s2_s := per_s2;
      per_s3_s := per_s3;
      glbCount_Per4s_s := glbCount_Per4s;
      pot_s2_s := pot_s2;
      pot_s3_s := pot_s3;
      glbCount_Urv4s_s := glbCount_Urv4s;
      rih1_s2_s := rih1_s2;
      rih1_s3_s := rih1_s3;
      glbCount_Rih4s_s := glbCount_Rih4s;
      rih2_s2_s := rih2_s2;
      rih2_s3_s := rih2_s3;
      GlbCountRemKm_s := GlbCountRemKm;
      GlbCountUKL_s := GlbCountUKL;
      GlbCountUSK_s := GlbCountUSK;
      GlbCountSOCH_s := GlbCountSOCH;
      GlbCountDGR_s := GlbCountDGR;
      LengthKm_s := LengthKm;
    end;

    if glb_BirdeiKmFlag or ((Glb_NestKm_count = 1) and (not glb_BirdeiKmFlag))
    then
    begin
      BEDOMOST(0);
    end
    else if GFlag_Km2501 or GFlag_Km1683 then
      BEDOMOST(TekKmTrue)
    else
      // ------------------------------------------------------------------------------
      BEDOMOST(TekKmTrue);
    if Flag_sablog then
      Writeln('Создали ведемост');
  Kelesi:
    // ----------------------------------

    zkm := TEKKM;
    zkmtrue := TekKmTrue;

    GRAPHREP(pkm);
    if Flag_sablog then
      Writeln('Рисуем график тарнзакцию');
  EXCEPT
  END;
END;

// ------------------------------------------------------------------------------
// procedure opredelenia sochetanii prosadka
// v1 -  огр.скорости пасс.
// v2 -  огр.скорости груз.Встроенное изображение 1
// u1 - уст. пасс. скорость
// u2 - уст. груз. скорость
// vr -  уст. ремонтный пасс. скорость
// vrg -уст. ремонтный груз. скорость
// -----------------------------------------------------------------------------
PROCEDURE OgrPoSochetaniu(m1, m2: MASOTS;
  stepen1, stepen2, INTERVAL_SCOROSTI: integer; ubedtxt: string);
var
  i1, i2, i: integer; //
  ogranichenie: integer;
  ogranichenie1: integer;
  a, b, c, d, xxxx, v1, v2, u1, u2, vr, vrg: integer;
BEGIN

  for i1 := 0 to length(m1) - 1 do
  begin
    if length(m1) > 0 then
    begin
      if ((m1[i1].st < 4)) then
        continue;
    end;
    a := m1[i1].L0;
    b := m1[i1].Lm;
             if (( abs(a-b)) <20)then continue ;

    for i2 := 0 to High(m2) do
    begin
      c := m2[i2].L0;
      d := m2[i2].Lm;

      if (((m1[i1].st = stepen1) and (m2[i2].st = stepen2)) and
        (((min(a, b) < min(c, d)) and (min(c, d) < max(a, b))) or
        ((min(a, b) < max(c, d)) and (max(c, d) < max(a, b))))) then
      begin

        u1 := m1[i1].v;
        u2 := m1[i1].vg;
        vr := m1[i1].vrp;
        vrg := m1[i1].vrg;

        i := IndexV_pros(0, u1) + INTERVAL_SCOROSTI;
        v1 := G_Ind2_skorost2(0, i);
        if (u1 > 120) then
          i := IndexV_pros(1, u2) + INTERVAL_SCOROSTI - 1
        else
          i := IndexV_pros(1, u2) + INTERVAL_SCOROSTI;
        v2 := G_Ind2_skorost2(1, i);

        xxxx := (c div 100) + 1;

        if (v1 < u1) or (v2 < u2) then
        begin

          // при установленной скорости больше 120 ограничиваем только
          if (u1 > 120) and (v1 >= u2) then
            v2 := -1;
          WRT_UBEDOM(a, b, 2, 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx) +
            ' ' + ubedtxt + '; ', v1, v2);
          if not(m1[i1].isriht) then
            ubedtxt := StringReplace(ubedtxt, 'Р+', 'Рнр+',
              [rfReplaceAll, rfIgnoreCase]);
          wrt_baskalar(ubedtxt, '', 0, a, abs(a - b), 5, 0, u1, u2, v1, v2, 1);
          GlbCountSOCH := GlbCountSOCH + 1;

        end;
      end; // if
    end; // i2
  end; // for i1

END; //

// ------------------------------------------------------------------------------
// 3<= kaitalanatyn rihtovka 3 darejeli
// -----------------------------------------------------------------------------
PROCEDURE OgrSmezhnyhOts_Riht(m1: MASOTS; st, kol, dlinaSmezh: integer);
// massiv, ctepen, kolichestvo, dlina
var
  i1, i2, i, j, k, ii: integer;
  ogranichenie1, xxxx, belv: integer;
  p1, p2: real;
  d, v1, v2, u1, u2, vr, vrg: integer;
  ots: string;
BEGIN

  i := 0;
  while i <= high(m1) - 7 do
  begin
    k := 0;
    for j := i + 1 to i + 7 do
    begin
      p1 := (m1[i].L0 + m1[i].Lm) / 2;
      p2 := (m1[j].L0 + m1[j].Lm) / 2;
      d := round(abs(p1 - p2));

      if (m1[i].st = st) and (m1[j].st = st) and (d <= dlinaSmezh) then
        k := k + 1;
    end;

    if k >= 3 then
    begin
      u1 := m1[i].v;
      u2 := m1[i].vg;
      vr := m1[i].vrp;
      vrg := m1[i].vrg;

      ii := IndexV(0, u1) + 1;
      belv := m1[i].bel;
      v1 := G_Ind2_skorost2_Riht(0, ii);

      ii := IndexV(1, u2) + 1;
      v2 := G_Ind2_skorost2_Riht(1, ii);

      if not FlagSiezd(m1[i].L0, m1[i].Lm) and not ProberkaNaStrelku(m1[i].L0,
        m1[i].L0, 7) and ((v1 < u1) or (v2 < u2)) then
      begin

        if GlbFlagRemontKm and (0 < vr) and (vr < v1) then
          v1 := -1;
        if GlbFlagRemontKm and (0 < vrg) and (vrg < v2) then
          v2 := -1;

        xxxx := m1[i].L0 div 100 + 1;
        { WRT_UBEDOM(m1[i].L0, m1[j].Lm, 6,
          'РР ' + Inttostr(m1[i].bel) + ' пк' + inttostr(xxxx) + ' V=' + inttostr(ogranichenie1)
          , '-', inttostr(ogranichenie1),inttostr(ogranichenie1)); }

        ots := 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx) + ' РР; ';
        WRT_UBEDOM(m1[i].L0, m1[j].Lm, 6, ots, v1, v2);
        wrt_baskalar('РР', '', belv, m1[i].L0, abs(m1[i].L0 - m1[i].Lm), 5, 0,
          u1, u2, v1, v2, 0);
        GlbCountDGR := GlbCountDGR + 1;
      end;
      i := i + 2;
    end;
    i := i + 5;
  end;
END; //

// ------------------------------------------------------------------------------
// procedure opredelenia smezhnyh otstuplenii po prosadke
// -----------------------------------------------------------------------------
PROCEDURE OgrSmezhnyhOts_Prosadka(m1: MASOTS; st, kol, dlinaSmezh: integer);
// massiv, ctepen, kolichestvo, dlina
var
  i1, i2, i: integer; //
  st1: integer;
  vel1: integer;
  l01: integer;
  lm1: integer;

  lx: array [1 .. 6] of integer;

  ogranichenie: integer;
  ogranichenie1, v1, v2, u1, u2, xxxx: integer;
  ots: string;
BEGIN
  for i1 := 0 to High(m1) do
  begin
    if (m1[i1].st >= st) then
    begin
      for i2 := 1 to (kol - 1) do
      begin
        lx[i2] := lx[i2 + 1]
      end;
      lx[kol] := m1[i1].L0;

      if (abs(lx[kol] - lx[1]) <= dlinaSmezh) and (lx[1] <> 0) and (lx[kol] <> 0)
      then
      begin
        u1 := m1[i1].v;
        i := IndexV_pros(0, u1) + 1;
        v1 := G_Ind2_skorost2(0, i);

        u2 := m1[i1].vg;
        i := IndexV_pros(1, u2) + 1;
        v2 := G_Ind2_skorost2(1, i);

        xxxx := (m1[i1].L0 + m1[i1].Lm) div 200;
        xxxx := (xxxx mod 1000) div 100 + 1;

        { if not FlagSiezd(m1[i1].L0 div 100, m1[i1].Lm div 100)
          and not ProberkaNaStrelku(m1[i1].L0 div 100,7) then begin
          if shekteu(m1[i1].L0 div 100 , m1[i1].Lm div 100, ogranichenie1) then
          WRT_UBEDOM(m1[i1].L0, m1[i1].Lm, 6,'3ст:3шт>Пр', inttostr(ogranichenie1),inttostr(ogranichenie1));
          end; }
        ots := 'V=' + V_shekti(v1, v2) + ' пк' + inttostr(xxxx) + ' 3ст; ';
        WRT_UBEDOM(m1[i1].L0, m1[i1].Lm, 6, ots, v1, v2);
        wrt_baskalar('3ст', '', 0, m1[i1].L0 div 100,
          round(abs(m1[i1].L0 - m1[i1].Lm) / 100), 5, 0, u1, u2, v1, v2, 0);
        GlbCountDGR := GlbCountDGR + 1;

        for i2 := 1 to kol do
        begin
          lx[i2] := 0;
        end;

      end; // if
    end; // if
  end; // for i1
END; //

// Ограничение смежные отступления по просадке
PROCEDURE New_OgrSmezhnyhOts_Prosadka(m1, m2: MASOTS;
  st, kol, dlinaSmezh: integer); // massiv, ctepen, kolichestvo, dlina
var
  i1, i2, i: integer; //

  st1: integer;
  vel1: integer;
  l01: integer;
  lm1: integer;

  lx: array [1 .. 6] of integer;

  ogranichenie: integer;
  ogranichenie1: integer;
  x1, x2, y1, y2, u1, u2, v1, v2, xxxx: integer;
BEGIN
  for i1 := 0 to High(m1) do
  begin
    /// ////////////
    if (m1[i1].st >= st) then
    begin

      for i2 := 1 to (kol - 1) do
      begin
        lx[i2] := lx[i2 + 1]
      end; //
      lx[kol] := m1[i1].L0;

      if (abs(lx[kol] - lx[1]) <= dlinaSmezh) and (lx[1] <> 0) and (lx[kol] <> 0)
      then
      begin

        x1 := m1[i1].L0;
        x2 := m1[i1].Lm;
        for i2 := 1 to kol do
        begin
          lx[i2] := 0;
        end;
      end; // if

    end; // if
  end; // for i1
  // ==============================================================================

  for i1 := 0 to High(m1) do
  begin
    /// ////////////
    if (m1[i1].st >= st) then
    begin

      for i2 := 1 to (kol - 1) do
      begin
        lx[i2] := lx[i2 + 1]
      end; //
      lx[kol] := m1[i1].L0;

      if (abs(lx[kol] - lx[1]) <= dlinaSmezh) and (lx[1] <> 0) and (lx[kol] <> 0)
      then
      begin
        u1 := m1[i1].v;
        i := IndexV_pros(0, u1) + 1;
        v1 := G_Ind2_skorost2(0, i);

        u2 := m1[i1].vg;
        i := IndexV_pros(1, u2) + 1;
        v2 := G_Ind2_skorost2(1, i);

        y1 := m1[i1].L0;
        y2 := m1[i1].Lm;

        xxxx := (y1 + y2) div 200;
        xxxx := (xxxx mod 1000) div 100 + 1;

        for i2 := 1 to kol do
        begin
          lx[i2] := 0;
        end;

      end; // if

    end; // if
  end; // for i1
  // ==============================================================================

  { if not FlagSiezd(m1[i1].L0 div 100, m1[i1].Lm div 100)
    and not ProberkaNaStrelku(m1[i1].L0 div 100,7) then begin
    if shekteu(m1[i1].L0 div 100 , m1[i1].Lm div 100, ogranichenie1) then
    WRT_UBEDOM(m1[i1].L0, m1[i1].Lm, 6,'3ст:3шт>Пр', '-', inttostr(ogranichenie1),inttostr(ogranichenie1));
    end; }

  { WRT_UBEDOM(y1, y2, 6,'3ст v=' + v_shekti(v1, v2) + ' пк' + inttostr(xxxx), v1, v2);
    wrt_baskalar('3ст', 0, y1,
    round(abs(y1-y2)/100),5, 0, u1, u2, v1, v2, 0);
    GlbCountDGR:=   GlbCountDGR + 1; }

END; //

// ==============================================================================
// 3 и более просадки подряд величиной более 15 мм(без учета засечек) периодически
// повторяющиеся по обеим нитям на длине 30 м на участках со скоростями движения
// более 60 км.ч
// ------------------------------------------------------------------------------
PROCEDURE OgrSmezh_Prosadka(m1, m2: MASOTS);
// massiv, ctepen, kolichestvo, dlina
type
  prr = record
    x, vp, vg, vrp, vrg: integer;
  end;

var
  i, j, xxxx, k, ii: integer; //
  ogranichenie: integer;
  ogranichenie1: integer;
  x1, x2, count, u1, u2, v1, v2, vr, vrg, delta: integer;
  pr3: array of prr;

BEGIN
  Setlength(pr3, 1000);
  k := 0;

  for i := 0 to High(m1) do
  begin
    x1 := round((m1[i].L0 + m1[i].Lm) / 2);
    for j := 0 to High(m2) do
    begin
      x2 := round((m2[j].L0 + m2[j].Lm) / 2);

      if (abs(x1 - x2) <= 6) and (m1[i].v > 60) and (m2[j].v > 60) and
        (m1[i].bel > 15) and (m2[j].bel > 15) then
      begin
        pr3[k].x := x1;
        pr3[k].vp := m1[i].v;
        pr3[k].vg := m1[i].vg;
        pr3[k].vrp := m1[i].vrp;
        pr3[k].vrg := m1[i].vrg;
        k := k + 1;
        Break;
      end;
    end;
  end;
  Setlength(pr3, k);

  for i := 0 to high(pr3) do
  begin
    k := 0;
    for j := i + 1 to high(pr3) do
      if abs(pr3[i].x - pr3[j].x) < 30 then
      begin
        k := k + 1;
        ii := j;
        if k >= 3 then
          Break;
      end;

    if k >= 3 then
    begin
      x1 := pr3[i].x;
      x2 := pr3[ii].x;
      u1 := pr3[i].vp;
      u2 := pr3[i].vg;
      vr := pr3[i].vrp;
      vrg := pr3[i].vrg;
      v1 := V_res('UPPR', 0, u1);
      v2 := V_res('UPPR', 1, u2);

      if (v1 < u1) or (v2 < u2) then
      begin
        if GlbFlagRemontKm and (0 < vr) and (vr < v1) then
          v1 := -1;
        if GlbFlagRemontKm and (0 < vrg) and (vrg < v2) then
          v2 := -1;

        xxxx := round((x1 + x2) / 2) div 100 + 1;
        WRT_UBEDOM(x1 * 100, x2 * 100, 6, '3Пр V=' + V_shekti(v1, v2), v1, v2);
        GlbCountDGR := GlbCountDGR + 1;

        wrt_baskalar('3Пр', '', 0, x1, round(abs(x1 - x2)), 5, 0, u1, u2,
          v1, v2, 1);
      end;
    end;
  end;
  pr3 := nil;
END;

// ==============================================================================
// 3 и более просадки подряд величиной более 15 мм(без учета засечек) периодически
// повторяющиеся по обеим нитям на длине 30 м на участках со скоростями движения
// более 60 км.ч
// ------------------------------------------------------------------------------
{ PROCEDURE OgrSmezh_Prosadka(m1, m2:masots); //massiv, ctepen, kolichestvo, dlina
  var
  i,j,xxxx: integer;//
  ogranichenie:integer;
  ogranichenie1:integer;
  x1,x2,count, u1, u2, v1, v2, vr, vrg :integer;

  BEGIN

  x1:= 0;
  x2:= 0;
  count:= 0;
  i:= 0;
  // for i:=0 to High(m1) - 6 do
  while i <= High(m1) - 6 do
  begin

  for j:= i to High(m2) - 6 do
  begin
  if  (High(m1) > 5)  and (High(m2) > 5)
  and (m1[i].bel > 15) and (m2[j].bel > 15) then
  begin
  x1:= min(m1[i].L0, m2[j].L0);
  x2:= max(m1[i+6].L0, m2[j+6].L0);
  u1:= m1[i].v;
  u2:= m1[i].vg;
  vr:= m1[i].vrp;
  vrg:= m1[i].vrg;

  if abs(x1 - x2) < 3000 then
  begin
  count:= count + 1;
  if count >= 3 then
  begin
  m2[i].prim := '3Пр';
  break;
  end;
  end;
  end;
  end;
  //==============================================================================
  if ((x1 <> 0) or (x2 <> 0)) and (count >= 3) then
  begin

  if (u1 > 60) and (u2 > 60)
  and (RGlobPassSkorost > 60) and (RGlobGruzSkorost > 60) then
  begin
  v1:= V_res('UPPR', 0, u1);
  v2:= V_res('UPPR', 1, u2);

  if (v1 < u1) or (v2 < u2) then
  begin
  if GlbFlagRemontKm and (0 < vr) and (vr < v1) then v1:= -1;
  if GlbFlagRemontKm and (0 < vrg) and (vrg < v2) then v2:= -1;

  xxxx:= (round((x1+x2)/200) mod 1000) div 100 + 1;
  WRT_UBEDOM(x1, x2, 6,
  '3Пр V='+ v_shekti(v1, v2) + ' пк' + inttostr(xxxx), v1, v2);
  GlbCountDGR:=   GlbCountDGR + 1;

  wrt_baskalar('3Пр', 0, x1 div 100, round(abs(x1-x2)/100),5, 0, u1, u2, v1, v2, 1);

  count:= 0;
  i:= i + 30;
  end;
  end;

  end;
  //=============================================================================

  i:= i + 1;
  end;//for i1
  END;//
}
// ------------------------------------------------------------------------------
PROCEDURE OgrSmezhnyhOts_Prosadka_st2(m1: MASOTS; st, kol, dlinaSmezh: integer);
// massiv, ctepen, kolichestvo, dlina
var
  i1, i2, i, u1, u2, v1, v2, x1, x2: integer; //
  st1: integer;
  vel1: integer;
  l01: integer;
  lm1: integer;
  lx: array [1 .. 6] of integer;

BEGIN
  for i1 := 0 to High(m1) do
  begin
    /// ////////////
    if ((m1[i1].st >= st) and (m1[i1].bel > 15)) then
    begin

      for i2 := 1 to (kol - 1) do
      begin
        lx[i2] := lx[i2 + 1]
      end; //
      lx[kol] := m1[i1].L0;

      if (abs(lx[kol] - lx[1]) <= dlinaSmezh) and (lx[1] <> 0) and (lx[kol] <> 0)
      then
      begin

        i := IndexV_pros(0, GlobPassSkorost) + 1;
        v1 := G_Ind2_skorost2(0, i);

        i := IndexV_pros(1, GlobPassSkorost) + 1;
        v2 := G_Ind2_skorost2(1, i);
        x1 := m1[i1].L0;
        x2 := m1[i1].Lm;

        WRT_UBEDOM(x1, x2, 6, '3Пр v=' + V_shekti(v1, v2), v1, v2);
        GlbCountDGR := GlbCountDGR + 1;

        { wrt_baskalar('3Пр', 0, x1,
          round(abs(x1-x2)/100),5, 0, u1, u2, v1, v2); }

        for i2 := 1 to kol do
        begin
          lx[i2] := 0;
        end;

      end; // if
    end; // if

  end; // for i1
END; //

// ------------------------------------------------------------------------------
// ОГРАНИЧЕНИЕ СКОРОСТИ НА КРИВ. УЧАСТКЕ
// ------------------------------------------------------------------------------


// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------

// ------------------------------------------------------------------------------

end.
