unit REPORTS;

interface

uses
  SysUtils, Dialogs, Classes, DataModule, Params, StdCtrls, math,
  Graphics, strutils, XMLDoc, XMLDom, XMLIntf;

type
  OtsInf = record
    M: integer;
    Otst: string[11]; // string[5];  //string;
    Otkl, prich_ogr_v, prim: string;
    St, Dl, vop, vog: integer;
    stype: integer;
    isEqualTo4, isEqualTo3: boolean;
  end;

  OtsInfArray = array of OtsInf;

procedure FORM_BEDOTCHETKM(TKM: integer);
procedure GRAPHREP(TKM: integer);
function Fsred_krivoi(prad: integer): real;

procedure zamenamemo(var kolzamen: integer; var strr: string; var zam1: integer;
  var St: integer);
function opr_usk(var ubed: string): boolean;
function opr_otv(var ubed: string): boolean;
function opr_ukl(var ubed: string): boolean;
function opr_ot4(var ubed: string; var ot4: string): boolean;

var
  sOTSTUPLENIE, GlbStancia, NestCaption: string[10];
  OTSTUPLENIE: integer;
  yx, yxr, yxr2: real;
  GlbInfOts: OtsInfArray;
  IndGlbInfOts: integer = 0;
  pro_flg: boolean = false;

implementation

uses funcsProcs, forots, rwun;
// ----------------------------------------------------------------------
// Опред. f sred крив.
// ----------------------------------------------------------------------
//

function opr_ot4(var ubed: string; var ot4: string): boolean;
var
  i, j: integer;
begin
  opr_ot4 := false;
  j := length(ot4);
  for i := 0 to length(ubed) do
    if copy(ubed, i, j) = ot4 then
      opr_ot4 := true;
end;

function opr_usk(var ubed: string): boolean;
var
  i, j: integer;
begin
  opr_usk := false;
  for i := 0 to length(ubed) do
    if copy(ubed, i, 3) = 'Анп' then
      opr_usk := true;
end;

function opr_ukl(var ubed: string): boolean;
var
  i, j: integer;
begin
  opr_ukl := false;
  for i := 0 to length(ubed) do
    if copy(ubed, i, 3) = 'Укл' then
      opr_ukl := true;

end;

function opr_otv(var ubed: string): boolean;
var
  i, j: integer;
begin
  opr_otv := false;
  for i := 0 to length(ubed) do
    if copy(ubed, i, 5) = 'Oтв.ш' then
      opr_otv := true;
end;

procedure zamenamemo(var kolzamen: integer; var strr: string; var zam1: integer;
  var St: integer);
var
  lenclob, auys: integer;
  flaguz: boolean;
  // label 11;
begin
  flaguz := false;
  kolzamen := kolzamen + 1;
  setlength(masofzamech, kolzamen);
  masofzamech[kolzamen - 1] := TLabel.Create(nil);
  masofzamech[kolzamen - 1].Caption := strr;
  masofzamech[kolzamen - 1].Top := zam1;
  lenclob := length(GlobUbedOgr);
  lenclob := length(strr) - length(GlobUbedOgr);
  if ((St = 3) or (St = 4) or containstext(strr, 'Укл') or containstext(strr,
    'Анп') or (copy(strr, lenclob + 1, length(GlobUbedOgr) + 1) = GlobUbedOgr)
    and (GlobUbedOgr <> '')) { or GlbFlagRemontKm } then
  begin

    masofzamech[kolzamen - 1].font.style := [fsBold]; // tqrlabel.Font.Style +
  end;
  masofzamech[kolzamen - 1].Parent := nil;

end;

function Fsred_krivoi(prad: integer): real;
var
  j: integer;
  fs: real;
begin
  fs := 0;
  for j := 1 to TabKrivCnt - 1 do
  begin
    if ((TabKrv[j].Rad <= prad) and (prad < TabKrv[j + 1].Rad)) or
      (TabKrv[j].Rad >= prad) and (prad > TabKrv[j + 1].Rad) then
    begin
      fs := TabKrv[j].Fsr;
      break;
    end;
  end;

  Fsred_krivoi := fs * k_mashrihpasp;

end;
// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------

function InfKrivoi(pcoora, pcoorb: integer): string;
var
  center_krv, xk: integer;
  flg: boolean;
begin
  InfKrivoi := '';
  xk := (pcoora + pcoorb) div 2;
  flg := KrivoiUch(xk);
  if flg and (((pcoora <= GlbKrvCenter) and (GlbKrvCenter <= pcoorb)) or
    ((pcoora >= GlbKrvCenter) and (GlbKrvCenter >= pcoorb))) then
  begin
    InfKrivoi := GlbInfKrivoi;
  end;
end;
// ------------------------------------------------------------------------------
// shablon masshtab
// ------------------------------------------------------------------------------

function kfWablon(M: real): real;
var
  center_krv, xk: integer;
  flg: boolean;
begin
  if (M <= 1520) then
    kfWablon := c_sh_1520
  else
    kfWablon := c_sh_1520 + +sh_koef;
end;
// ------------------------------------------------------------------------------
// ЭКСПОРТИРОВАТЬ ОТЧЕТ В XML ФАЙЛ
// ------------------------------------------------------------------------------

procedure ReportToXml;
var
  ReportDocument: IXMLDOCUMENT;
  RootNode, CurNode, CurNodeSecond, RihNode, ShpalNode, StrelNode, MostNode,
    ProsadkaLeft, FMeter, ProsadkaRight, F0_shNode, f_shNode, fsr_rh1Node,
    frih1Node, frih10Node, fsr_rh2Node, frih2Node, frih20Node, fsr_urbNode,
    furbNode, furb0Node, masofnote: IXMLNODE;

  tip_proezd, sind: string;
  stt, ii, i, j, shpal: integer;
  xi: real;
  str_PCH: string;
  str_PCHU: string;
  str_PD: string;
  str_PDB: string;
  korek: Int64;
  necorr: string;
  coord: string;
  currCoord: real;
begin
  // ReportDocument := NewXMLDocument;
  // ReportDocument.Encoding := 'utf-8';
  // ReportDocument.Options := [doNodeAutoIndent]; // looks better in Editor ;)
  // RootNode := ReportDocument.AddChild('report');
  // RihNode := RootNode.AddChild('straightening');
  // ShpalNode := RootNode.AddChild('sleepers');
  // StrelNode := RootNode.AddChild('switches');
  // MostNode := RootNode.AddChild('bridges');
  // ProsadkaLeft := RootNode.AddChild('prosleft');
  // FMeter := RootNode.AddChild('meter');
  // ProsadkaRight := RootNode.AddChild('prosright');
  // F0_shNode := RootNode.AddChild('fsh0');
  // f_shNode := RootNode.AddChild('fsh');
  //
  // fsr_rh1Node := RootNode.AddChild('fsr_rh1');
  // frih1Node := RootNode.AddChild('frih1');
  // frih10Node := RootNode.AddChild('frih10');
  // fsr_rh2Node := RootNode.AddChild('fsr_rh2');
  // frih2Node := RootNode.AddChild('frih2');
  // frih20Node := RootNode.AddChild('frih20');
  // fsr_urbNode := RootNode.AddChild('fsr_urb');
  // furbNode := RootNode.AddChild('furb');
  // furb0Node := RootNode.AddChild('furb0');

  Shab_s2 := 0;
  Shab_s3 := 0;
  Shab_s4 := 0;
  Suj_s2 := 0;
  Suj_s3 := 0;
  glbCount_suj4s := 0;
  ush_s2 := 0;
  ush_s3 := 0;
  glbCount_Ush4s := 0;
  rih_s2 := 0;
  rih_s3 := 0;
  rih_s4 := 0;
  pot_s2 := 0;
  pot_s3 := 0;
  glbCount_Urv4s := 0;
  per_s2 := 0;
  per_s3 := 0;
  glbCount_Per4s := 0;
  Urb_s2 := 0;
  Urb_s3 := 0;
  Urb_s4 := 0;
  pro_s2 := 0;
  pro_s3 := 0;
  pro_s4 := 0;
  str_PCH := '';
  str_PCHU := '';
  str_PD := '';
  str_PDB := '';

  necorr := '';
  if GlbCommentPaspData = 'Не корр.паспорт' then
    necorr := ' '#13'Режим оценки: Не корр.пасп.';
  coord := '';



  // for i := 0 to high(mVPik) do
  // begin
  // CurNode := RootNode.AddChild('piketspeed');
  // CurNode.Text := inttostr(mVPik[i].mtr2);
  // end;

  // for j := 0 to high(UMT) do
  // begin
  // if ((UMT[j].nkm <= GlbKmTrue) and (GlbKmTrue <= UMT[j].kkm) and
  // (NPut(UMT[j].put) = NumPut)) then
  // begin
  // CurNode := MostNode.AddChild('bridge');
  // CurNode.Attributes['pch'] := UMT[j].pch;
  // CurNode.Attributes['nkm'] := UMT[j].nkm;
  // CurNode.Attributes['nmtr'] := UMT[j].nmtr;
  // CurNode.Attributes['kkm'] := UMT[j].kkm;
  // CurNode.Attributes['kmtr'] := UMT[j].kmtr;
  // CurNode.Attributes['length'] := UMT[j].Dl;
  // CurNode.Attributes['type'] := UMT[j].tip;
  // end;
  //
  // end;
  for stt := 0 to High(UKrv) do
  begin
    // with (MainDataModule.fdReadPasport) do
    // begin
    // Close;
    // Sql.clear;
    // Sql.Add('delete from rd_curve where km=:km and trip_id =:trip_id and curve_id = :curve_id');
    // ParamByName('km').value := GlbKmTrue;
    // ParamByName('trip_id').value := GTripId;
    // ParamByName('curve_id').value := UKrv[stt].id;
    // ExecSql;
    // end;
  end;

  for i := 0 to high(F_mtr) do
  begin
    // CurNode := FMeter.AddChild('m');
    // CurNode.Text := inttostr(F_mtr[i]);
    // CurNode := RihNode.AddChild('s');
    // CurNode.Text := inttostr(Rih_Nit[i].fun);
    // CurNode := ProsadkaLeft.AddChild('pr');
    // CurNode.Text := floattostr(FPro1[i]);
    // CurNode := ProsadkaRight.AddChild('pr');
    // CurNode.Text := floattostr(FPro2[i]);
    // CurNode := F0_shNode.AddChild('sh0');
    // CurNode.Text := floattostr(F0_sh[i]);
    // CurNode := f_shNode.AddChild('sh');
    // CurNode.Text := floattostr(F_sh[i]);
    //
    // CurNode := fsr_rh1Node.AddChild('rh');
    // CurNode.Text := floattostr(Fsr_rh1[i]);
    // CurNode := frih1Node.AddChild('rh');
    // CurNode.Text := floattostr(Frih1[i]);
    // CurNode := frih10Node.AddChild('rh');
    // CurNode.Text := floattostr(F0_rih1[i]);
    //
    // CurNode := fsr_rh2Node.AddChild('rh');
    // CurNode.Text := floattostr(Fsr_rh2[i]);
    // CurNode := frih2Node.AddChild('rh');
    // CurNode.Text := floattostr(Frih2[i]);
    // CurNode := frih20Node.AddChild('rh');
    // CurNode.Text := floattostr(F0_rih2[i]);
    //
    // CurNode := fsr_urbNode.AddChild('urb');
    // CurNode.Text := floattostr(Fsr_Urb[i]);
    // CurNode := furbNode.AddChild('urb');
    // CurNode.Text := floattostr(Furb[i]);
    // CurNode := furb0Node.AddChild('urb');
    // CurNode.Text := floattostr(F0_urov[i]);

    x := round(F_mtr[i]);
    currCoord := CoordinateToReal(glbKmTrue, x);

    for stt := 0 to High(UKrv) do
    begin
      // if ((CoordinateToReal(UKrv[stt].nkm, UKrv[stt].nmtr)-0.0100<=currCoord) and
      // (CoordinateToReal(UKrv[stt].kkm, UKrv[stt].kmtr)+0.0100>=currCoord)) then

      if ((UKrv[stt].n100 <= currCoord) and (UKrv[stt].k100 >= currCoord)) then

        with (MainDataModule.sqlGetCurveCoords) do
        begin
          ParamByName('TRIP_ID').Value := GTripId;
          ParamByName('KM').Value := glbKmTrue;
          ParamByName('M').Value := x;

          ParamByName('RADIUS').Value := abs((F_fluk[i] / 2) + ST_AVG[i]);
          ParamByName('LEVEL').Value := abs(LV_N[i]);

          ParamByName('GAUGE').Value := abs(F_sh[i] - F0_sh[i]);

          ParamByName('PASSBOOST').Value :=GUs[i];

          ParamByName('FREIGHTBOOST').Value := GUs2[i];

          ParamByName('PASSBOOST_ANP').Value := ANP_GUs[i];
          ParamByName('FREIGHTBOOST_ANP').Value := ANP_GUs2[i];

          ParamByName('PASSSPEED').Value := F_V[i];
          ParamByName('FREIGHTSPEED').Value := F_Vg[i];
          ParamByName('BROAD').Value := 0;
          ParamByName('WEAR').Value := 0;
          ParamByName('TRACK_ID').Value := GlbTrackId;
          ParamByName('CURVE_ID').Value := UKrv[stt].id;

          ParamByName('POINT_LEVEL').Value := CurvePointsLevel[i];
          ParamByName('POINT_STR').Value := CurvePointsStr[i];

          ParamByName('TRAPEZ_LEVEL').Value := TrapezLevel[i];
          ParamByName('TRAPEZ_STR').Value := TrpzStr[i];

          ParamByName('AVG_LEVEL').Value := LV_AVG[i];
          ParamByName('AVG_STR').Value := ST_AVG[i];

          ExecSQl();
        end;
    end;
  end;

  sind := '';

  if flag_sablog then
    Writeln('отчетный файл сформирован');

end;
// ------------------------------------------------------------------------------
// ФОРМИР. ГРАФОТЧЕТА, ПЕЧАТЬ И СОХР. НА ФАЙЛ
// ------------------------------------------------------------------------------

procedure PaintGrph;
var
  i, ii, x, j: integer;

  tip_proezd, Caption: string;

begin
  lenmasofstr := 0;
  NestCaption := '';
  GlbStancia := '';
  try
    thetta := 0;
    // if GlbMaxRih > 52 then thetta:= 8;

    case GTipPoezdki of
      0:
        tip_proezd := 'раб.';
      1:
        tip_proezd := 'контр.';
      2:
        tip_proezd := 'доп.';
      3:
        tip_proezd := 'колиб.';
    end;

    Caption := 'ТУЛПАР-ИНТЕХ: ПО версии ' + ProgVER + Ins + VAgon + NUMBERCAR_F
      + '(' + chief_f + ')<КАЗ><' + Dateofinser + '><д.о.:' + datetimetostr(now)
      + '>' + '<' + direction_f + '><' + Gays + '-' + Gjyl + '><' + tip_proezd +
      '><Проезд:' + num_proberki + '><' + GlbTypeGrp + '>'; // Дубликат>

    Caption := inttostr(glbKmTrue) + ' км'; //
    for ii := 0 to high(UNst) do
    begin
      NestCaption := 'Нестанд.км';
      break;
    end;
    for ii := 0 to high(Uras) do
    begin
      GlbStancia := Uras[ii].naimst;
    end;

  except
  end;
end;
// ------------------------------------------------------------------------------

procedure ClearGrph;
var
  ii: integer;
begin
  try
    for ii := 0 to high(masofstr) do
      masofstr[ii].Free;
    masofstr := nil;
    for ii := 0 to high(masofzamech) do
    begin
      masofzamech[ii].Free;
    end;
    masofzamech := nil;
    lenofzamen := 0;

    // ----
    // ----
  except
  end;
end;
// ------------------------------------------------------------------------------

procedure RefreshGrph;
var
  proverka: tstrel;
begin
  try

    // ~~~~~
  except
  end;
end;
// ------------------------------------------------------------------------------
// ФОРМИР. ОТЧЕТА BEDEMOST, ПЕЧАТЬ И СОХР. НА ФАЙЛ
// ------------------------------------------------------------------------------

procedure FORM_BEDOTCHETKM(TKM: integer);
begin
  try
    with MainDataModule do
    begin
      {
        GBEDDATAKM.Close;
        GBEDDATAKM.ParamByName('PPKM').AsInteger:= TKM;
        GBEDDATAKM.Open; }
      // ibquery1.Close;
      // ibquery1.Open;
    end;
  except
  end;
end;

function is2to3StToString(St: integer; is2to3: boolean): string;
begin
  result := inttostr(St);
  if (is2to3) then
    result := result + 'b';
end;

procedure Write_InfoOts34(OtsInf: OtsInfArray);
var

  strm, skr, ubed1: string;
  FFF: TEXTFILE;
  flgk, f1, f2, f3, f4, f5, f6, f7, f8, f9, f10: boolean;
  korek: integer;
  i_krap, j_krap, jjjj, iii, i, si, ll, id, JJ: integer;
  ijk: integer;
  // для сортировки отсинф
  m1: integer;
  Otst1: string[5]; // string;
  prich_ogr_v1: string;
  St1, Otkl1, Dl1: integer;
  stype1, z011, z012: integer;
  necorr: string;
begin
  ll := 0;
  for i := 0 to high(kv) do
  begin
    strm := inttostr(kv[i].nach_krv) + ' R:' + inttostr(kv[i].Radius) + ' h:' +
      inttostr(round(kv[i].Fsr_Urob)) + ' Ш:' + inttostr(round(kv[i].Fsr_Shab))
      + ' И:' + inttostr(kv[i].Wear);

    zamenamemo(lenofzamen, strm, kv[i].nach_krv, ll);
    if flagpic then
      kkk := kv[i].piket_krv + ppp
    else
      kkk := kv[i].piket_krv;

  end;
  kv := nil;
  // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  ll := 5;
  for i := 0 to high(mVPik) do
  begin

    strm := inttostr(mVPik[i].mtr2) + ' Уст.ск:' + inttostr(mVPik[i].v11) + '/'
      + inttostr(mVPik[i].v12) + ' Уст.ск:' + inttostr(mVPik[i].v21) + '/' +
      inttostr(mVPik[i].v22);
    zamenamemo(lenofzamen, strm, mVPik[i].mtr2, ll);
  end;
  ll := 0;
  strm := ' ';

  for JJ := 0 to HIGH(OtsInf) do
  begin

    sOTSTUPLENIE := OtsInf[JJ].Otst;
    OTSTUPLENIE := 0;

    if UpperCase(sOTSTUPLENIE) = UpperCase('Пр.п') then
      OTSTUPLENIE := 1;
    if UpperCase(sOTSTUPLENIE) = UpperCase('Пр.л') then
      OTSTUPLENIE := 2;

    if tanba_rih < 0 then
    begin
      if UpperCase(sOTSTUPLENIE) = UpperCase('Пр.п') then
        OTSTUPLENIE := 2;
      if UpperCase(sOTSTUPLENIE) = UpperCase('Пр.л') then
        OTSTUPLENIE := 1;
    end;

    if UpperCase(sOTSTUPLENIE) = UpperCase('Уш') then
      OTSTUPLENIE := 3;
    if UpperCase(sOTSTUPLENIE) = UpperCase('Суж') then
      OTSTUPLENIE := 6;

    if UpperCase(sOTSTUPLENIE) = UpperCase('Р') then
      OTSTUPLENIE := 4;
    if UpperCase(sOTSTUPLENIE) = UpperCase('Р') then
      OTSTUPLENIE := 4;
    if UpperCase(sOTSTUPLENIE) = UpperCase('Рнр') then
      OTSTUPLENIE := 4;

    if UpperCase(sOTSTUPLENIE) = UpperCase('П') then
      OTSTUPLENIE := 5;
    if UpperCase(sOTSTUPLENIE) = UpperCase('У') then
      OTSTUPLENIE := 5;

    if not LftAxisINV then
      id := HIGH(OtsInf) - JJ
    else
      id := JJ;

    if OtsInf[id].stype = 0 then
    begin

      if OtsInf[id].St < 4 then
        strm := inttostr(OtsInf[id].M) + ' ' + OtsInf[id].Otst + ' ' +
          is2to3StToString(OtsInf[id].St, OtsInf[id].isEqualTo3) + ' ' +
          OtsInf[id].Otkl + ' ' + inttostr(OtsInf[id].Dl);
      if (OtsInf[id].vop > 0) or (OtsInf[id].vog > 0) then
        strm := strm + ' ' + V_shekti(OtsInf[id].vop, OtsInf[id].vog);
      strm := strm + ' ' + OtsInf[id].prim;

      if OtsInf[id].St = 5 then
      begin
        strm := inttostr(OtsInf[id].M) + ' ' + OtsInf[id].Otst + ' ' +
          OtsInf[id].Otkl + ' ';
        if ((OtsInf[id].Otst = 'Анп') or (OtsInf[id].Otst = '?Анп') or
          (containstext(OtsInf[id].Otst, 'Пси'))) and (OtsInf[id].Dl > 20) then
          strm := strm + '>20 '
        else
          strm := strm + inttostr(OtsInf[id].Dl) + ' ';
        strm := strm + ' ' + V_shekti(OtsInf[id].vop, OtsInf[id].vog);

      end;
      if (OtsInf[id].St = 4) or (OtsInf[id].isEqualTo4) then
      begin
        strm := inttostr(OtsInf[id].M) + ' ' + OtsInf[id].Otst + ' ' +
          inttostr(OtsInf[id].St) + ' ' + OtsInf[id].Otkl + ' ' +
          inttostr(OtsInf[id].Dl);

        if (OtsInf[id].prim.Contains('Стр.')) then
          strm := strm + ' ' + OtsInf[id].prim
        else
          strm := strm + ' ' + V_shekti(OtsInf[id].vop, OtsInf[id].vog);
        if (OtsInf[id].prim.Contains('ис') or OtsInf[id].prim.Contains('t+'))
        then
          strm := strm + ' ' + OtsInf[id].prim;

      end;

    end
    else if (OtsInf[id].stype = 1) and flagNadpisOgr then
    begin

      strm := inttostr(OtsInf[id].M) + ' ' + OtsInf[id].Otst + ' ' +
        OtsInf[id].Otkl + ' ' + inttostr(OtsInf[id].Dl) + ' ' +
        V_shekti(OtsInf[id].vop, OtsInf[id].vog);

      flagNadpisOgr := false;

      ijk := ijk + 1;

      ubed1 := 'П ';

    end // ;
    // *****************************************
    else if (OtsInf[id].stype = 2) and GlbFlagRemontKm then
    begin
      strm := inttostr(OtsInf[id].M) + ' ' + OtsInf[id].prich_ogr_v;
    end;

    zamenamemo(lenofzamen, strm, OtsInf[id].M, OtsInf[id].St);

  end;

end;
// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// ФОРМИР. ОТЧЕТЬ 'Graph', ПЕЧАТЬ И СОХР. НА ФАЙЛ
// ------------------------------------------------------------------------------

procedure GRAPHREP(TKM: integer);
var
  Otst34s: OtsInfArray;
  i, ic, RCOUNT, RCOUNT1, RCOUNT2: integer;
  sind, str_PCH, str_PCHU, str_PD, str_PDB, str_PV, str_GV: string;
  skp, skg: integer;
  ogrprim: string;

  Mf: TMetaFile;

begin
  if flag_sablog then
    SabLog('GRAPHREP - формирование граф отчета и распечатка');
  if flag_sablog then
    Writeln('GRAPHREP - формирование граф отчета и распечатка');
  ClearGrph;
  FORM_BEDOTCHETKM(gkmtrue);

  PaintGrph;
  Glob_page_GReport := Glob_page_GReport + 1;

  if (glb_vop <> 999) then
    GlbOgrPasGrz := inttostr(glb_vop)
  else
    GlbOgrPasGrz := '-';

  GlbOgrPasGrz := GlbOgrPasGrz + '/';

  if (glb_vog <> 999) then
    GlbOgrPasGrz := GlbOgrPasGrz + inttostr(glb_vog)
  else
    GlbOgrPasGrz := GlbOgrPasGrz + '-';

  RCOUNT1 := length(F_mtr);
  if flag_sablog then
    Writeln('GRAPHREP - рисование');
  Write_InfoOts34(GlbInfOts);
  if flag_sablog then
    Writeln('GRAPHREP - пишем отступление');
  // PaintGrph;
  // ------------------------------------------------------------------------------
  // FORM_BEDOTCHETKM(TKM);
  // GRAFOTCH.QRLabel3.Caption := inttostr(GlbKmTrue);
  // GRAFOTCH.QRLabel20.Caption := inttostr(CVSredKm);
  ReportToXml;
  if flag_sablog then
    Writeln('GRAPHREP - to xml');
  sind := '';
  if (izk <> 0) then
    sind := '_' + inttostr(izk);

end;

end.
