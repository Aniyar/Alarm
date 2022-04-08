unit DataModule;

interface

uses
  System.SysUtils, System.Classes, Vcl.Dialogs, FireDAC.Stan.Intf,
  FireDAC.Stan.Option, FireDAC.Stan.Param, FireDAC.Stan.Error, FireDAC.DatS,
  FireDAC.Phys.Intf, FireDAC.DApt.Intf, FireDAC.Stan.Async, FireDAC.DApt,
  FireDAC.Phys.PGDef, FireDAC.UI.Intf, FireDAC.Stan.Def, FireDAC.Stan.Pool,
  FireDAC.Phys, FireDAC.Phys.PG, FireDAC.VCLUI.Wait, Vcl.ExtCtrls, Data.DB,
  FireDAC.Comp.Client, FireDAC.Comp.DataSet, StrUtils, Math;

type
  TMainDataModule = class(TDataModule)
    sqlGetCurveCoords: TFDQuery;
    FDPhysPgDriverLink1: TFDPhysPgDriverLink;
    spAddUbemDat: TFDStoredProc;
    spBedemost: TFDStoredProc;
    spMainPP: TFDStoredProc;
    FDTransaction: TFDTransaction;
    pgsConnection: TFDConnection;
    fdReadPasport: TFDQuery;
    FDReadPasportInner: TFDQuery;
    spAddUbemDatresult: TLargeintField;

  private
    { Private declarations }
  public
    { Public declarations }
  end;

procedure ActiveTransact2;
procedure CommitTransact2;
procedure Processing;

procedure ObrabotkaEndKm(i: integer);
PROCEDURE WRT_UBEDOM(Bcoor, Ecoor, tip: integer; Subed: string;
  SogrV, SogrGV: integer);
procedure Ubedom_db(m, kol, otkl, lng, typ, v, vg, vop, vog: integer;
  ots, prm: string; st: integer; is2to3, onswitch, isequalto4, islong: boolean);
procedure Ubedom_db3(m, kol, otkl, lng, typ, v, vg: integer; ots, prm: string;
  st: integer; is2to3, onswitch, isequalto4, isequalto3, islong: boolean);
function GetCoordByLen(km, meter, len: integer; trackId: longint;
  tripDate: TDateTime): real;
function GetDistanceBetween(start_km, start_m, final_km, final_m: integer;
  trackId: longint; tripDate: TDateTime): integer;
function GetDistanceBetweenReal(a, b: real; trackId: longint;
  tripDate: TDateTime): integer;

var
  MainDataModule: TMainDataModule;

implementation

uses forots, FuncsProcs, reports, params, RWUn;

function GetCoordByLen(km, meter, len: integer; trackId: longint;
  tripDate: TDateTime): real;
begin
  with (MainDataModule.fdReadPasport) do
  begin
    Close;
    Sql.clear;
    Sql.Add('select * from getcoordbylen(:km,:meter,:len, :trackId, :tripDate)');
    ParamByName('km').value := km;
    ParamByName('meter').value := meter;
    ParamByName('len').value := len;
    ParamByName('trackId').value := trackId;
    ParamByName('tripDate').value := tripDate;
    Open;
    result := Fields[0].value;
  end;

end;

function GetDistanceBetween(start_km, start_m, final_km, final_m: integer;
  trackId: longint; tripDate: TDateTime): integer;
begin
  with (MainDataModule.fdReadPasport) do
  begin
    Close;
    Sql.clear;
    Sql.Add('select * from getDistanceFrom(:start_km,:start_m,:final_km, :final_m, :trackId, :tripDate)');
    ParamByName('start_km').value := start_km;
    ParamByName('start_m').value := start_m;
    ParamByName('final_km').value := final_km;
    ParamByName('final_m').value := final_m;
    ParamByName('trackId').value := trackId;
    ParamByName('tripDate').value := tripDate;
    Open;
    result := Fields[0].value;
  end;

end;

function GetDistanceBetweenReal(a, b: real; trackId: longint;
  tripDate: TDateTime): integer;
var
  start_km, start_m, final_km, final_m: integer;
  start, finish: real;
begin
  start := min(a, b);
  finish := max(a, b);
  start_km := trunc(start);
  start_m := trunc((Frac(start) * 10000));
  final_km := trunc(finish);
  final_m := trunc((Frac(finish) * 10000));
  with (MainDataModule.fdReadPasport) do
  begin
    Close;
    Sql.clear;
    Sql.Add('select * from getDistanceFrom(:start_km,:start_m,:final_km, :final_m, :trackId, :tripDate)');
    ParamByName('start_km').value := start_km;
    ParamByName('start_m').value := start_m;
    ParamByName('final_km').value := final_km;
    ParamByName('final_m').value := final_m;
    ParamByName('trackId').value := trackId;
    ParamByName('tripDate').value := tripDate;
    Open;
    result := Fields[0].value;
  end;

end;
{$R *.dfm}

// ------------------------------------------------------------------------------
// ЗАПИСЬ В ТАБ. BEDOMOST
// ------------------------------------------------------------------------------
{
  1- ukl
  2- soch
  3- usk
  4- 4stpn
  5- nesobp nach naras vozv s nach naras strel
  6- smezh
  7- drugie
}
PROCEDURE WRT_UBEDOM;
// (Bcoor,Ecoor, tip: INTEGER; Subed, SZnach, SogrV, SogrGV:string);
var
  i, j, xx, flgUbed, v01, v02: integer;
  ffilter, Rflg: boolean;
  Vpas, Vgrz: integer;
  Og_PV, Og_GV: string;
BEGIN

  TRY
    if Flag_sablog then
      SabLog('wrt_ubedom');
    flgUbed := 0;
    xx := round((Bcoor + Ecoor) / 2);
    // xx:= (xx div 100) mod 1000;

    Vpas := SogrV;
    Vgrz := SogrGV;

    v01 := Vflag(xx, 1);
    v02 := Vflag(xx, 2);

    // if GlbFlagRemontKm   then begin  GlbOgrSkorKm:= true;   end;  //0704 2014

    if ((v01 > Vpas) and (Vpas >= 0)) or ((v02 > Vgrz) ) then     //
    // or iv_
    begin

      if (v01 > Vpas   ) and (GlbMinOgrSk > Vpas)and(Vpas>=0)  then     //   and (Vpas <>-1)
        GlbMinOgrSk := Vpas;
      if ((v02 > Vgrz) and (GlbMinOgrSkGrz > Vgrz)  and ( Vgrz>=0)  ) then
        GlbMinOgrSkGrz := Vgrz; // or iv_

      // ---0504 2014
      if GlbFlagRemontKm and (Vpas < VRflag(xx, 1)) then
        GlbMinOgrSk := Vpas;
      if GlbFlagRemontKm and (Vgrz < VRflag(xx, 2)) then
        GlbMinOgrSkGrz := Vgrz;

      // ---

      if (tip <> 4) then
      begin
        UbedOgr4jok := Subed;
        UbedOgr4jok_V := Vpas;
        GlbMinOgrSk_ := Vpas;
      end;

      if (tip = 4) then
      begin
        UbedOgr := Subed;
        GlbMinOgrSk4 := Vpas;
      end;

      glbTipOgrV := tip;

      GlobUbedOgr := GlobUbedOgr + Subed;

      GlbMinOgrSkCoordA := Bcoor;
      GlbMinOgrSkCoordB := Ecoor;
      GlbOgrSkType := tip;

      // GlbOgrSkorKm:= true;   //0704 2014

      Glob_primech := GlobUbedOgr;
      // Glob_primech  + Subed + ' ' + SZnach + ', ';
      // Ubedom_Save2db;    //0909
      GlbOgrPasGrz := SVflag(xx, GlbMinOgrSk, GlbMinOgrSkGrz);

      // v_shekti(GlbMinOgrSk, GlbMinOgrSkGrz);
      iv_ := false;
    end;
  EXCEPT
  END;
END;

// ------------------------------------------------------------------------------
// 4 st
// ------------------------------------------------------------------------------
procedure Ubedom_db(m, kol, otkl, lng, typ, v, vg, vop, vog: integer;
  ots, prm: string; st: integer; is2to3, onswitch, isequalto4, islong: boolean);
var
  crd, cu, us, p1, p2, ur, pr, r1, r2, pdb, bas, vop_, vog_: integer;
begin
  if ((st < 4) and (prm.Contains('рн'))) then
    exit;
  cu := 0;
  us := 0;
  p1 := 0;
  p2 := 0;
  ur := 0;
  pr := 0;
  r1 := 0;
  r2 := 0;

  vop_ := vop;
  vog_ := vog;
  if (not(ContainsText(ots, 'Ш10')) and not(GlbOgrSkorKm)) then
  begin
    vop_ := -1;
    vog_ := -1;
  end;

  if (vop <> -1) and (vop < glb_vop) then
    glb_vop := vop;

  if (vog <> -1) and (vog < glb_vog) then
    glb_vog := vog;

  if ots = 'Уш' then
    us := 1
  else if ots = 'Суж' then
    cu := 1
  else if ots = 'Пр.п' then
    p1 := 1
  else if ots = 'Пр.л' then
    p2 := 1
  else if ots = 'П' then
    pr := 1
  else if ots = 'У' then
    ur := 1
  else if ots = 'Р' then
    r1 := 1;
  r2 := r1;
  pdb := 1;

  bas := 0;
  if typ = 5 then
    bas := 1;

  if length(prm) > 95 then
    Delete(prm, 95, length(prm)); // 1511_2012

  with MainDataModule.spAddUbemDat do
  begin
    ParamByName('pch').value := inttostr(Glb_PutList_PCH);
    ParamByName('distance_id').value := inttostr(GlbDistanceId);
    ParamByName('naprav').value := Glb_PutList_GNapr;
    ParamByName('put').value := GlbNumPut;
    ParamByName('track_id').value := GlbTrackId;
    ParamByName('pchu').value := Glb_PList_PCHU;
    ParamByName('pd').value := Glb_PList_PD;
    ParamByName('pdb').value := Glb_PList_PDB;
    ParamByName('km').value := GlbKmTrue;
    ParamByName('meter').value := EnsureRange(m, Low(SmallInt), High(SmallInt));
    ParamByName('trip_id').value := GTripId;
    ParamByName('ots').value := ots;
    ParamByName('kol').value := kol;
    ParamByName('otkl').value := otkl;
    ParamByName('len').value := lng;
    ParamByName('primech').value := prm;
    ParamByName('tip_poezdki').value := GTipPoezdki;
    ParamByName('cu').value := cu;
    ParamByName('us').value := us;
    ParamByName('p1').value := p1;
    ParamByName('p2').value := p2;
    ParamByName('ur').value := ur;
    ParamByName('pr').value := pr;
    ParamByName('r1').value := r1;
    ParamByName('r2').value := 0; // r2;
    ParamByName('bas').value := bas;
    ParamByName('typ').value := typ;
    ParamByName('uv').value := v;
    ParamByName('uvg').value := vg;
    ParamByName('ovp').value := vop_; // vop;
    ParamByName('ogp').value := vog_; // vog;
    ParamByName('is2to3').value := is2to3; // vog;
    ParamByName('onswitch').value := onswitch; // vog;
    ParamByName('isequalto3').value := false; // vog;
    ParamByName('isequalto4').value := isequalto4; // vog;
    ParamByName('islong').value := isequalto4; // vog;
    ExecProc;
  end; // with

end;

// ------------------------------------------------------------------------------
// 4 st
// ------------------------------------------------------------------------------
procedure Ubedom_db3(m, kol, otkl, lng, typ, v, vg: integer; ots, prm: string;
  st: integer; is2to3, onswitch, isequalto4, isequalto3, islong: boolean);
var
  crd, cu, us, p1, p2, ur, pr, r1, r2, pdb, bas: integer;
begin
  if ((st < 3) and (prm.Contains('рн'))) then
    exit;

  cu := 0;
  us := 0;
  p1 := 0;
  p2 := 0;
  ur := 0;
  pr := 0;
  r1 := 0;
  r2 := 0;
  if ots = 'Уш' then
    us := kol
  else if ots = 'Суж' then
    cu := kol
  else if ots = 'Пр.п' then
    p1 := 1
  else if ots = 'Пр.л' then
    p2 := 1
  else if ots = 'П' then
    pr := 1
  else if ots = 'У' then
    ur := kol
  else if ots = 'Р' then
    r1 := 1;
  r2 := r1;

  bas := 0;
  if typ = 5 then
    bas := 1;

  if length(prm) > 95 then
    Delete(prm, 95, length(prm)); // 1511_2012

  with MainDataModule.spAddUbemDat do
  begin
    ParamByName('pch').value := inttostr(Glb_PutList_PCH);
    ParamByName('distance_id').value := inttostr(GlbDistanceId);
    ParamByName('naprav').value := Glb_PutList_GNapr;
    ParamByName('put').value := GlbNumPut;
    ParamByName('track_id').value := GlbTrackId;
    ParamByName('pchu').value := Glb_PList_PCHU;
    ParamByName('pd').value := Glb_PList_PD;
    ParamByName('pdb').value := Glb_PList_PDB;
    ParamByName('km').value := GlbKmTrue;
    ParamByName('meter').value := EnsureRange(m, Low(SmallInt), High(SmallInt));
    ParamByName('trip_id').value := GTripId;
    ParamByName('ots').value := ots;
    ParamByName('kol').value := kol;
    ParamByName('otkl').value := otkl;
    ParamByName('len').value := lng;
    ParamByName('primech').value := prm;
    ParamByName('tip_poezdki').value := GTipPoezdki;
    ParamByName('cu').value := cu;
    ParamByName('us').value := us;
    ParamByName('p1').value := p1;
    ParamByName('p2').value := p2;
    ParamByName('ur').value := ur;
    ParamByName('pr').value := pr;
    ParamByName('r1').value := r1;
    ParamByName('r2').value := 0; // r2;
    ParamByName('bas').value := bas;
    ParamByName('typ').value := typ;
    ParamByName('uv').value := v;
    ParamByName('uvg').value := vg;
    ParamByName('ovp').value := -1;
    ParamByName('ogp').value := -1;
    ParamByName('is2to3').value := is2to3;
    ParamByName('onswitch').value := onswitch;
    ParamByName('isequalto4').value := isequalto4;
    ParamByName('isequalto3').value := isequalto3;
    ParamByName('islong').value := islong;
    ExecProc;
  end; // with

end;

// ------------------------------------------------------------------------------
procedure Deconnect;
begin
  try
    // SabLog('Deconnect - отключить соединение БД');

    // if (DBDATAPRK.Connected = TRUE) then
    if MainDataModule.pgsConnection.Connected then
    begin
      MainDataModule.FDTransaction.Commit;
      MainDataModule.pgsConnection.Connected := false;
    end;

  except
  end;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
procedure ActiveTransact2;
begin
  Try

    with (MainDataModule.pgsConnection) do
    begin
      Connected := false;
      Connected := TRUE;
    end;
    MainDataModule.FDTransaction.StartTransaction;
  except
  end;
end;

// ------------------------------------------------------------------------------
procedure CommitTransact2;

begin
  Try

    MainDataModule.FDTransaction.Commit;

  except
  end;
end;

// ------------------------------------------------------------------------------
procedure Startup;
begin
  try
    // SabLog('Startup - процедура запуска таймера или события');
    if not(MainDataModule.pgsConnection.Connected) then
    begin
      MainDataModule.pgsConnection.Connected := TRUE;
    end;
  except
  end;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------
procedure Processing;
var
  j: integer;
begin
  try
    if not StopFlag then
    begin
      // SabLog('Старт обработки данных');

      RWTB_PRK(FileName);

      j := j + NAPR_DBIJ;
      // Startup;
    end;
  except
  end;
end;

// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------

procedure ObrabotkaEndKm(i: integer);
begin
  if FileExists(Path_km_shifrovka_file + 'km_' + inttostr(i) + '.svgpdat') then
  // or (GlbTimerClickCount = 3) then
  begin

    RWTB_PRK(FileName);

  end;
end;

end.
