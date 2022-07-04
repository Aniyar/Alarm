unit RWUn;

interface

uses
  Windows, Forms, dialogs, Messages, SysUtils, Controls, Classes, Params,
  FuncsProcs, math, StdCtrls;

type
  tfun = record
    kord, fun: real;
  end;

  pmas = array of tfun;

  tstrel = array of factstrel;

procedure LoadDefData;
procedure RWTB_PRK(Name_Km_SVGP: string); // ПОЛУЧ. ДАН. И ОБР.
procedure DopUklBozb; // Допускаемый уклон отв. возв.

function interpol(x1, x2, y1, y2, x: real): real;

procedure OprFactStrelki1; // определение фактической стрелки
procedure SredFuncSablon;

procedure DopUklBozb_Shablona;

procedure Nesob_nash_naras_voz_s_rih;
procedure VPiket;
function NPut(fput: string): INTEGER;
function SPut(fput: string): string;
procedure GetRemMas;
function CoordinateToReal(km, meter: INTEGER): real;

procedure ReadPassport(track_id: longint; trip_date: TDateTime; nkm: INTEGER);

// булева переменная для включения режима тестирования
var
  flag_test: boolean;
  fstrel: array of factstrel; // tstrel;

implementation

uses
  DataModule, ForOts, REPORTS, URemDistance;

// ------------------------------------------------------------------------------
procedure ReadPassport(track_id: longint; trip_date: TDateTime; nkm: INTEGER);
var
  i, sj, curveID: INTEGER;
  exist: boolean;
begin

  with (MainDataModule.fdReadPasport) do
  begin
    Close;
    Sql.Clear;
    Sql.Text := 'select trip_type from trips where id =' + inttostr(GTripID);
    Open;
    while not(eof) do
    begin
      GTipPoezdki := Fields[0].Value;
      Next;
    end;
    // System.Writeln(inttostr(GTripID) + '-' + inttostr(GTipPoezdki));
    // АДМИНИСТРАТИВНАЯ СТРУКТУРА
    Close;
    Sql.Clear;
    Sql.Add('Select distinct adc.code as distance, adc.id as distance_id,  apc.CODE as pchu, apd.CODE as pd, ap.CODE as pdb, apd.chief_fullname as chief, tps.start_km, tps.final_km from ADM_PDB as ap');
    Sql.Add('INNER JOIN ADM_PD as apd on apd.ID = ap.ADM_PD_ID');
    Sql.Add('INNER JOIN ADM_PCHU as apc on apc.ID = apd.ADM_PCHU_ID');
    Sql.Add('INNER JOIN ADM_DISTANCE as adc on adc.ID = apc.ADM_DISTANCE_ID');
    Sql.Add('INNER JOIN tpl_pdb_section as tps on tps.adm_pdb_id = ap.id');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = tps.PERIOD_ID');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID');
    Sql.Add('WHERE :travel_date BETWEEN tp.START_DATE and tp.FINAL_DATE');
    Sql.Add('and atr.id = :track_id');
    Sql.Add('and :nkm between tps.start_km and tps.final_km');
    ParamByName('travel_date').Value := trip_date;
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    Open;

    while not(eof) do
    begin
      setlength(UAdm, length(UAdm) + 1);
      with UAdm[length(UAdm) - 1] do
      begin
        pch := FieldByName('distance').Value;
        mex := FieldByName('pchu').Value;
        pd := FieldByName('pd').Value;
        pdb := FieldByName('pdb').Value;
        fio_pd := FieldByName('chief').Value;
        NPut := track_id;
        nkm := FieldByName('start_km').Value;
        kkm := FieldByName('final_km').Value;
        NazbN := 'нет данных';
        NazbK := 'нет данных';
        Glb_PutList_PCH := pch;
        GlbDistanceId := FieldByName('distance_id').Value;
        Glb_PList_PCHU := mex;
        Glb_PList_PD := pd;
        Glb_PList_PDB := pdb;

      end;
      Next;
    end;

    Close;
    // РАСПРЕДЕЛЕНИЕ РАЗДЕЛЕННЫХ ПУНКТОВ
    Sql.Clear;
    Sql.Add('Select distinct station.*, cpot.name as vid, tps.start_km, tps.start_m, tps.final_km, tps.final_m from adm_station as station');
    Sql.Add('INNER JOIN cat_point_object_type as cpot on cpot.id = station.type_id');
    Sql.Add('INNER JOIN tpl_station_section as tps on tps.station_id = station.id');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = tps.PERIOD_ID and :trip_date between tp.start_date and tp.final_date');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID');
    Sql.Add('WHERE atr.id = :trackId and :nkm between tps.start_km and tps.final_km ');
    Sql.Add('union');
    Sql.Add('Select station.*, cpot.name as vid, min(section.start_km * 1000 + section.start_m) / 1000 as start_km, min(section.start_km * 1000 + section.start_m) % 1000 as start_m, ');
    Sql.Add('max(section.final_km * 1000 + section.final_m) / 1000 as final_km, max(section.final_km * 1000 + section.final_m) % 1000 as final_m from stw_track as stwt');
    Sql.Add('INNER JOIN cat_track_type as trackt on trackt.ID = stwt.TYPE_ID');
    Sql.Add('inner join adm_track as track on track.id = stwt.adm_track_id');
    Sql.Add('inner join adm_station as station on stwt.adm_station_id = station.id');
    Sql.Add('INNER JOIN cat_point_object_type as cpot on cpot.id = station.type_id');
    Sql.Add('left join tpl_period period on period.adm_track_id = track.id and period.mto_type = 9  and :trip_date between period.start_date and period.final_date');
    Sql.Add('	and period.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = track.id and tpl_period.mto_type = 9)');
    Sql.Add('left join tpl_pdb_section section on section.period_id = period.id');
    Sql.Add('where track.id = :trackId and :nkm between start_km and final_km  ');
    Sql.Add('group by stwt.id,  cpot.name, trackt.id, station.id, track.id, station.code, station.name');
    Sql.Add('union');
    Sql.Add('Select station.*, cpot.name as vid, min(section.start_km * 1000 + section.start_m) / 1000 as start_km, min(section.start_km * 1000 + section.start_m) % 1000 as start_m, ');
    Sql.Add('max(section.final_km * 1000 + section.final_m) / 1000 as final_km, max(section.final_km * 1000 + section.final_m) % 1000 as final_m from stw_park_track as stwt');
    Sql.Add('INNER JOIN cat_track_type as trackt on trackt.ID = stwt.TYPE_ID');
    Sql.Add('inner join adm_track as track on track.id = stwt.adm_track_id');
    Sql.Add('left join stw_park as park on stwt.stw_park_id = park.id');
    Sql.Add('left join adm_station as station on station.id = park.adm_station_id');
    Sql.Add('INNER JOIN cat_point_object_type as cpot on cpot.id = station.type_id');
    Sql.Add('left join tpl_period period on period.adm_track_id = track.id and period.mto_type = 9 and :trip_date between period.start_date and period.final_date');
    Sql.Add('	and period.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = track.id and tpl_period.mto_type = 9)');
    Sql.Add('left join tpl_pdb_section section on section.period_id = period.id');
    Sql.Add('where track.id = :trackId and :nkm between start_km and final_km ');
    Sql.Add('group by stwt.id, cpot.name, trackt.id, park.id, track.id, station.id, park.name');
    ParamByName('trackId').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(URas, length(URas) + 1);
      with URas[length(URas) - 1] do
      begin
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        kodst := FieldByName('code').Value;
        naimst := FieldByName('name').Value;
        vid := FieldByName('vid').Value;
        put := inttostr(track_id);
      end;
      Next;
    end;
    // СТРЕЛОЧНЫЕ ПЕРЕВОДЫ
    Close;

    Sql.Clear;
    Sql.Add('Select tsw.*, ccm.itype as marka, ccm.len, getcoordbylen(tsw.km, tsw.meter, 0,atr.id, tp.start_date) as start_coord, ');
    Sql.Add('getcoordbylen(tsw.km, tsw.meter, ccm.len,atr.id, tp.start_date) as final_coord from TPL_SWITCH as tsw ');
    Sql.Add('INNER JOIN cat_cross_mark as ccm on tsw.mark_id = ccm.id');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = tsw.PERIOD_ID');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID');
    Sql.Add('WHERE atr.id = :trackId and :trip_date between tp.start_date and tp.final_date and ');
    Sql.Add('(:nkm between getcoordbylen(tsw.km, tsw.meter, 0,atr.id, tp.start_date) and ');
    Sql.Add('getcoordbylen(tsw.km, tsw.meter, ccm.len,atr.id, tp.start_date) or :nkm=tsw.km)');
    ParamByName('trackId').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(UStr, length(UStr) + 1);
      with UStr[length(UStr) - 1] do
      begin
        nkm := FieldByName('start_coord').Value div 1;
        nmtr := trunc((Frac(FieldByName('start_coord').Value) * 10000));
        kkm := FieldByName('final_coord').Value div 1;
        kmtr := trunc((Frac(FieldByName('final_coord').Value) * 10000));
        marka := FieldByName('marka').Value;
        posh := FieldByName('dir_id').Value;
        bix := FieldByName('side_id').Value;
        put := inttostr(track_id);
      end;
      Next;
    end;
    // ПРЯМЫЕ УЧАСТКИ
    Close;

    Sql.Clear;
    Sql.Add('Select aac.* from APR_NORMA_WIDTH as aac ');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID ');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID ');
    Sql.Add('WHERE atr.id = :track_id and :trip_date between tp.start_date and tp.final_date and :nkm between aac.start_km and aac.final_km');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(UPrm, length(UPrm) + 1);
      with UPrm[length(UPrm) - 1] do
      begin
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        norma := FieldByName('norma_width').Value;
        put := inttostr(track_id);
      end;
      Next;
    end;
    // ПУЧИНЫ
    Close;

    Sql.Clear;
    Sql.Add('Select deep.* from APR_ELEVATION as deep ');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = deep.PERIOD_ID ');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID ');
    Sql.Add('WHERE atr.id = :track_id and :trip_date between tp.start_date and tp.final_date and :nkm between deep.start_km and deep.final_km');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(UPuch, length(UPuch) + 1);
      with UPuch[length(UPuch) - 1] do
      begin
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        vozv := FieldByName('level_id').Value;
        put := inttostr(track_id);

      end;
      Next;
    end;
    // Кривые участки
    Close;

    Sql.Clear;
    Sql.Add('Select curve.* from APR_CURVE as curve ');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = curve.PERIOD_ID ');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID ');
    Sql.Add('WHERE atr.id = :track_id and :trip_date between tp.start_date and tp.final_date and :nkm between curve.start_km and curve.final_km');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(UKrv, length(UKrv) + 1);
      with UKrv[length(UKrv) - 1] do
      begin
        id := FieldByName('id').Value;
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        put := inttostr(track_id);
        with (MainDataModule.FDReadPasportInner) do
        begin
          Close;
          Sql.Clear;
          Sql.Add('Select * from apr_stcurve where curve_id = :curveId');
          ParamByName('curveId').Value :=
            MainDataModule.fdReadPasport.FieldByName('id').Value;
          Open;
          while not(eof) do
          begin
            setlength(strights, length(strights) + 1);
            strights[length(strights) - 1].nkm := FieldByName('start_km').Value;
            strights[length(strights) - 1].nmtr := FieldByName('start_m').Value;
            strights[length(strights) - 1].kkm := FieldByName('final_km').Value;
            strights[length(strights) - 1].kmtr := FieldByName('final_m').Value;
            strights[length(strights) - 1].radius :=
              FieldByName('radius').Value;
            strights[length(strights) - 1].norma_width :=
              FieldByName('width').Value;
            strights[length(strights) - 1].wear := FieldByName('wear').Value;
            strights[length(strights) - 1].l1 :=
              FieldByName('transition_1').Value;
            strights[length(strights) - 1].l2 :=
              FieldByName('transition_2').Value;
            Next;
          end;
          Close;
          Sql.Clear;
          Sql.Add('Select * from apr_elcurve where curve_id = :curveId');
          ParamByName('curveId').Value :=
            MainDataModule.fdReadPasport.FieldByName('id').Value;
          Open;
          while not(eof) do
          begin
            setlength(levels, length(levels) + 1);
            levels[length(levels) - 1].nkm := FieldByName('start_km').Value;
            levels[length(levels) - 1].nmtr := FieldByName('start_m').Value;
            levels[length(levels) - 1].kkm := FieldByName('final_km').Value;
            levels[length(levels) - 1].kmtr := FieldByName('final_m').Value;
            levels[length(levels) - 1].level := FieldByName('lvl').Value;
            levels[length(levels) - 1].l1 := FieldByName('transition_1').Value;
            levels[length(levels) - 1].l2 := FieldByName('transition_2').Value;
            Next;
          end;
          Close;

        end;

      end;
      Next;
    end;
    // Кривые участки
    Close;

    Sql.Clear;
    Sql.Add('Select curve.* from APR_CURVE as curve ');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = curve.PERIOD_ID ');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID ');
    Sql.Add('WHERE atr.id = :track_id and :trip_date between tp.start_date and tp.final_date and :nkm between curve.start_km and curve.final_km');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm - 1;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      exist := false;
      for i := 0 to High(UKrv) do
      begin
        curveID := FieldByName('id').AsInteger;
        if (UKrv[i].id = curveID) then
        begin
          exist := true;
          break;
        end;

      end;
      if exist then
      begin
        Next;
        Continue;
      end;
      setlength(UKrv, length(UKrv) + 1);
      with UKrv[length(UKrv) - 1] do
      begin
        id := FieldByName('id').Value;
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        put := inttostr(track_id);
        with (MainDataModule.FDReadPasportInner) do
        begin
          Close;
          Sql.Clear;
          Sql.Add('Select * from apr_stcurve where curve_id = :curveId');
          ParamByName('curveId').Value :=
            MainDataModule.fdReadPasport.FieldByName('id').Value;
          Open;
          while not(eof) do
          begin
            setlength(strights, length(strights) + 1);
            strights[length(strights) - 1].nkm := FieldByName('start_km').Value;
            strights[length(strights) - 1].nmtr := FieldByName('start_m').Value;
            strights[length(strights) - 1].kkm := FieldByName('final_km').Value;
            strights[length(strights) - 1].kmtr := FieldByName('final_m').Value;
            strights[length(strights) - 1].radius :=
              FieldByName('radius').Value;
            strights[length(strights) - 1].norma_width :=
              FieldByName('width').Value;
            strights[length(strights) - 1].wear := FieldByName('wear').Value;
            strights[length(strights) - 1].l1 :=
              FieldByName('transition_1').Value;
            strights[length(strights) - 1].l2 :=
              FieldByName('transition_2').Value;
            Next;
          end;
          Close;
          Sql.Clear;
          Sql.Add('Select * from apr_elcurve where curve_id = :curveId');
          ParamByName('curveId').Value :=
            MainDataModule.fdReadPasport.FieldByName('id').Value;
          Open;
          while not(eof) do
          begin
            setlength(levels, length(levels) + 1);
            levels[length(levels) - 1].nkm := FieldByName('start_km').Value;
            levels[length(levels) - 1].nmtr := FieldByName('start_m').Value;
            levels[length(levels) - 1].kkm := FieldByName('final_km').Value;
            levels[length(levels) - 1].kmtr := FieldByName('final_m').Value;
            levels[length(levels) - 1].level := FieldByName('lvl').Value;
            levels[length(levels) - 1].l1 := FieldByName('transition_1').Value;
            levels[length(levels) - 1].l2 := FieldByName('transition_2').Value;
            Next;
          end;
          Close;

        end;

      end;
      Next;
    end;
    // Кривые участки
    Close;

    Sql.Clear;
    Sql.Add('Select curve.* from APR_CURVE as curve ');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = curve.PERIOD_ID ');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID ');
    Sql.Add('WHERE atr.id = :track_id and :trip_date between tp.start_date and tp.final_date and :nkm between curve.start_km and curve.final_km');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm + 1;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      exist := false;
      for i := 0 to High(UKrv) do
      begin
        curveID := FieldByName('id').AsInteger;
        if (UKrv[i].id = curveID) then
        begin
          exist := true;
          break;
        end;

      end;
      if exist then
      begin
        Next;
        Continue;
      end;
      setlength(UKrv, length(UKrv) + 1);
      with UKrv[length(UKrv) - 1] do
      begin
        id := FieldByName('id').Value;
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        put := inttostr(track_id);
        with (MainDataModule.FDReadPasportInner) do
        begin
          Close;
          Sql.Clear;
          Sql.Add('Select * from apr_stcurve where curve_id = :curveId');
          ParamByName('curveId').Value :=
            MainDataModule.fdReadPasport.FieldByName('id').Value;
          Open;
          while not(eof) do
          begin
            setlength(strights, length(strights) + 1);
            strights[length(strights) - 1].nkm := FieldByName('start_km').Value;
            strights[length(strights) - 1].nmtr := FieldByName('start_m').Value;
            strights[length(strights) - 1].kkm := FieldByName('final_km').Value;
            strights[length(strights) - 1].kmtr := FieldByName('final_m').Value;
            strights[length(strights) - 1].radius :=
              FieldByName('radius').Value;
            strights[length(strights) - 1].norma_width :=
              FieldByName('width').Value;
            strights[length(strights) - 1].wear := FieldByName('wear').Value;
            strights[length(strights) - 1].l1 :=
              FieldByName('transition_1').Value;
            strights[length(strights) - 1].l2 :=
              FieldByName('transition_2').Value;
            Next;
          end;
          Close;
          Sql.Clear;
          Sql.Add('Select * from apr_elcurve where curve_id = :curveId');
          ParamByName('curveId').Value :=
            MainDataModule.fdReadPasport.FieldByName('id').Value;
          Open;
          while not(eof) do
          begin
            setlength(levels, length(levels) + 1);
            levels[length(levels) - 1].nkm := FieldByName('start_km').Value;
            levels[length(levels) - 1].nmtr := FieldByName('start_m').Value;
            levels[length(levels) - 1].kkm := FieldByName('final_km').Value;
            levels[length(levels) - 1].kmtr := FieldByName('final_m').Value;
            levels[length(levels) - 1].level := FieldByName('lvl').Value;
            levels[length(levels) - 1].l1 := FieldByName('transition_1').Value;
            levels[length(levels) - 1].l2 := FieldByName('transition_2').Value;
            Next;
          end;
          Close;

        end;

      end;
      Next;
    end;
    for i := 0 to high(UKrv) do
    begin

      UKrv[i].n100 := GetCoordByLen(UKrv[i].nkm, UKrv[i].nmtr, -200, GlbTrackId,
        GlbTripDate);
      UKrv[i].k100 := GetCoordByLen(UKrv[i].kkm, UKrv[i].kmtr, 200, GlbTrackId,
        GlbTripDate);
      for sj := 0 to high(UKrv[i].levels) do
      begin
        UKrv[i].levels[sj].x_npk1 := CoordinateToReal(UKrv[i].levels[sj].nkm,
          UKrv[i].levels[sj].nmtr);
        UKrv[i].levels[sj].x_kpk1 := GetCoordByLen(UKrv[i].levels[sj].nkm,
          UKrv[i].levels[sj].nmtr, +UKrv[i].levels[sj].l1, GlbTrackId,
          GlbTripDate);

        UKrv[i].levels[sj].x_kpk2 := GetCoordByLen(UKrv[i].levels[sj].kkm,
          UKrv[i].levels[sj].kmtr, -UKrv[i].levels[sj].l2, GlbTrackId,
          GlbTripDate);
        UKrv[i].levels[sj].x_npk2 := CoordinateToReal(UKrv[i].levels[sj].kkm,
          UKrv[i].levels[sj].kmtr);

      end;
      for sj := 0 to High(UKrv[i].strights) do
      begin
        UKrv[i].strights[sj].x_npk1 :=
          CoordinateToReal(UKrv[i].strights[sj].nkm, UKrv[i].strights[sj].nmtr);
        UKrv[i].strights[sj].x_kpk1 := GetCoordByLen(UKrv[i].strights[sj].nkm,
          UKrv[i].strights[sj].nmtr, +UKrv[i].strights[sj].l1, GlbTrackId,
          GlbTripDate);

        UKrv[i].strights[sj].x_kpk2 := GetCoordByLen(UKrv[i].strights[sj].kkm,
          UKrv[i].strights[sj].kmtr, -UKrv[i].strights[sj].l2, GlbTrackId,
          GlbTripDate);

      end;

    end;

    // УСТАНОВЛЕННАЯ СКОРОСТЬ
    Close;

    Sql.Clear;
    Sql.Add('Select speed.* from APR_SPEED as speed ');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = speed.PERIOD_ID ');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID ');
    Sql.Add('WHERE atr.id = :track_id and :trip_date between tp.start_date and tp.final_date and :nkm between speed.start_km and speed.final_km');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(USkr, length(USkr) + 1);
      with USkr[length(USkr) - 1] do
      begin
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        skp := FieldByName('passenger').Value;
        skg := FieldByName('freight').Value;
        sp := FieldByName('empty_freight').Value;
        put := inttostr(track_id);
      end;
      Next;
    end;
    // МОСТЫ И ТОННЕЛИ
    Close;
    Sql.Clear;
    Sql.Add('Select aac.len, lower(cac.name) as type, startentrance.km as start_km, startentrance.meter as start_m, finalentrance.km as final_km, finalentrance.meter as final_m  from apr_artificial_construction as aac ');
    Sql.Add('inner join cat_artificial_construction as cac on aac.type_id = cac.id ');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID');
    Sql.Add('inner join gettablecoordbylen(aac.km, aac.meter, aac.len / -2, tp.adm_track_id, :trip_date) as startcoords on true');
    Sql.Add('inner join gettablecoordbylen(aac.km, aac.meter, aac.len / 2, tp.adm_track_id, :trip_date) as finalcoords on true');
    Sql.Add('inner join gettablecoordbylen(startcoords.km, startcoords.meter, -case WHEN aac.len between 25 and 100 then 200 WHEN aac.len > 100 then 500 else 0 END, tp.adm_track_id, :trip_date) as startentrance on true');
    Sql.Add('inner join gettablecoordbylen(finalcoords.km, finalcoords.meter, case WHEN aac.len between 25 and 100 then 200 WHEN aac.len > 100 then 500 else 0 END, tp.adm_track_id, :trip_date) as finalentrance on true');
    Sql.Add('WHERE atr.id = :trackId and :trip_date between tp.start_date and tp.final_date and ');
    Sql.Add(':nkm between startentrance.km  and finalentrance.km ');

    ParamByName('trackId').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(UMT, length(UMT) + 1);
      with UMT[length(UMT) - 1] do
      begin
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        dl := FieldByName('len').Value;
        tip := FieldByName('type').AsString.Chars[0];
        put := inttostr(track_id);
      end;
      Next;
    end;
    // ЖЕЛЕЗОБЕТОННЫЕ И ДЕРЕВ. ШПАЛЫ
    Close;
    Sql.Clear;
    Sql.Add('SELECT shpal.*, case when shpal.crosstie_type_id > 2 then 2 else shpal.crosstie_type_id -1 end as god   FROM public.apr_crosstie as shpal');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = shpal.PERIOD_ID');
    Sql.Add('INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID');
    Sql.Add('WHERE atr.id = :track_id and :trip_date between tp.start_date and tp.final_date and :nkm between shpal.start_km and shpal.final_km');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(UShp, length(UShp) + 1);
      with UShp[length(UShp) - 1] do
      begin
        nkm := FieldByName('start_km').Value;
        nmtr := FieldByName('start_m').Value;
        kkm := FieldByName('final_km').Value;
        kmtr := FieldByName('final_m').Value;
        god := FieldByName('god').Value;
        put := inttostr(track_id);
      end;
      Next;
    end;
    // нестандартные километры
    Close;
    Sql.Clear;
    Sql.Add('SELECT nst.len as len FROM tpl_nst_km AS nst');
    Sql.Add('INNER JOIN TPL_PERIOD as tp on tp.ID = nst.PERIOD_ID');
    Sql.Add('WHERE tp.adm_track_id = :track_id and :trip_date between tp.start_date and tp.final_date and nst.km = :nkm');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(UNst, length(UNst) + 1);
      with UNst[length(UNst) - 1] do
      begin
        km := nkm;
        GlbKmLength := FieldByName('len').AsInteger;
        dlina := GlbKmLength;
      end;
      Next;
    end;
    Close;
    Sql.Clear;
    Sql.Add('select isojoint.* from tpl_profile_object as isojoint');
    Sql.Add('inner join tpl_period AS period on period.id = isojoint.period_id');
    Sql.Add('WHERE period.adm_track_id = :track_id and :trip_date between ');
    Sql.Add('period.start_date and period.final_date and isojoint.km = :nkm');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(IsoJoint, length(IsoJoint) + 1);
      with IsoJoint[length(IsoJoint) - 1] do
      begin
        km := nkm;
        meter := FieldByName('meter').AsInteger;

      end;
      Next;
    end;

    Close;
    Sql.Clear;
    Sql.Add('select rails.* from apr_long_rails as rails');
    Sql.Add('inner join tpl_period AS period on period.id = rails.period_id');
    Sql.Add('WHERE rails.type_id = 3 and period.adm_track_id = :track_id and :trip_date between ');
    Sql.Add('period.start_date and period.final_date and :nkm between rails.start_km and rails.final_km ');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(Jointlesspath, length(Jointlesspath) + 1);
      with Jointlesspath[length(Jointlesspath) - 1] do
      begin
        start_km := FieldByName('start_km').AsInteger;
        start_m := FieldByName('start_m').AsInteger;
        final_km := FieldByName('final_km').AsInteger;
        final_m := FieldByName('final_m').AsInteger;
      end;
      Next;
    end;
    // Действующиу ограничения
    Close;
    Sql.Clear;
    Sql.Add('select ts.* from apr_tempspeed as ts');
    Sql.Add('inner join tpl_period AS period on period.id = ts.period_id');
    Sql.Add('WHERE period.adm_track_id = :track_id and :trip_date between ');
    Sql.Add('period.start_date and period.final_date and :nkm between ts.start_km and ts.final_km ');
    ParamByName('track_id').Value := track_id;
    ParamByName('nkm').Value := nkm;
    ParamByName('trip_date').Value := trip_date;
    Open;
    while (not(eof)) do
    begin
      setlength(USkr, length(USkr) + 1);
      with USkr[length(USkr) - 1] do
      begin
        nkm := FieldByName('start_km').AsInteger;
        nmtr := FieldByName('start_m').AsInteger;
        kkm := FieldByName('final_km').AsInteger;
        kmtr := FieldByName('final_m').AsInteger;
        skp := FieldByName('passenger').AsInteger;
        skg := FieldByName('freight').AsInteger;
        put := inttostr(track_id);
      end;
      Next;
    end;

  end;

end;

function GetDistance(a, b: real): INTEGER;
var
  start_km, final_km, start_m, final_m, i: INTEGER;

begin
  result := 0;
  if (a < b) then
  begin
    start_km := trunc(a);
    start_m := round((a - start_km) * 10000);
    final_km := trunc(b);
    final_m := round((b - final_km) * 10000);

  end
  else
  begin
    start_km := trunc(b);
    start_m := round((b - start_km) * 10000);
    final_km := trunc(a);
    final_m := round((a - final_km) * 10000);
  end;
  if (start_km = final_km) then
    result := abs(final_m - start_m)
  else
    result := GetDistanceBetween(start_km, start_m, final_km, final_m,
      GlbTrackId, GlbTripDate);

end;

function NPut(fput: string): INTEGER;
var
  p: INTEGER;
begin
  p := 1;
  if (fput = 'Главный') or (fput = 'Нечетный') then
    p := 1;
  if (fput = 'Четный') then
    p := 2;
  if (fput = '1a') or (fput = '1а') then
    p := 99;
  if (fput = '2a') or (fput = '2а') then
    p := 98;
  if (fput = '3a') or (fput = '3а') then
    p := 97;

  if (fput <> 'Четный') and (fput <> 'Нечетный') and (fput <> 'Главный') and
    (p <> 98) and (p <> 99) and (p <> 97) then
    p := strtoint(fput);

  NPut := p;
end;

// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------
// remont info file-dan oku
// ------------------------------------------------------------------------------
procedure GetRemMas; // (km:integer);
var
  i: INTEGER;
  f: textfile;
  rem_distances: TRemDistances;
begin
  setlength(rem, 0);
  if FileExists('remfile.dat') then
    rem_distances := ReadFromFile();
  if length(rem_distances) > 0 then
    GenerateRemfileForOldVersion(rem_distances);

  if FileExists('RFile.txt') then
  begin
    AssignFile(f, 'RFile.txt');
    reset(f);

    i := 0;

    while not eof(f) do
    begin
      setlength(rem, i + 1);
      Readln(f, rem[i].adat);
      Readln(f, rem[i].bdat);
      Readln(f, rem[i].put);
      Readln(f, rem[i].km);
      Readln(f, rem[i].pik);
      Readln(f, rem[i].v);
      Readln(f, rem[i].rtype);
      i := i + 1;
    end;

    CloseFile(f);
  end;
end;

function SPut(fput: string): string;
var
  p: INTEGER;
  s: string;
begin
  s := fput;
  if (fput = 'Главный') or (fput = 'Нечетный') then
    s := '1';
  if (fput = 'Четный') then
    s := '2';
  if (fput = '1a') or (fput = '1а') or (fput = '99') then
    s := '1a';
  if (fput = '2a') or (fput = '2а') or (fput = '98') then
    s := '2a';
  if (fput = '3a') or (fput = '3а') or (fput = '97') then
    s := '3a';

  if (fput <> 'Четный') and (fput <> 'Нечетный') and (fput <> 'Главный') and
    (s <> '1a') and (s <> '2a') and (s <> '3a') then
  begin
    p := strtoint(fput);
    if p = 99 then
      s := '1a';
    if p = 98 then
      s := '2a';
    if p = 97 then
      s := '3a';
  end;
  SPut := s;
  // if glbkmtrue = 585 then sablog(' 585 -' + s);

end;
// ------------------------------------------------------------------------------

function Piket(xmetr: INTEGER): INTEGER;
var
  pik: INTEGER;
begin
  pik := 1;
  if (x mod 100 = 0) and (x <> 0) then
    pik := xmetr div 100
  else if xmetr mod 100 <> 0 then
    pik := xmetr div 100 + 1;
  Piket := pik;
end;
// ------------------------------------------------------------------------------

procedure VPiket;
var
  i, j, v11, v12, v21, v22, km11, km12, km21, km22, c1, c2: INTEGER;
  a1, b1, a2, b2: real;
  puts1, puts2: string;
  count_, pt1, pt2: INTEGER;
begin
  if Flag_sablog then
    sablog('vpiket');

  count_ := 0;
  setlength(mVPik, 10);

  for i := 0 to high(USkr) do
  begin
    puts1 := USkr[i].put;
    km11 := USkr[i].nkm;
    km12 := USkr[i].kkm;
    a1 := CoordinateToReal(USkr[i].nkm, USkr[i].nmtr);
    b1 := CoordinateToReal(USkr[i].kkm, USkr[i].kmtr);
    v11 := USkr[i].skp;
    v12 := USkr[i].skg;
    c1 := USkr[i].kmtr;

    for j := i + 1 to high(USkr) do
    begin
      puts2 := USkr[j].put;
      km21 := USkr[j].nkm;
      km22 := USkr[j].kkm;
      a2 := CoordinateToReal(USkr[j].nkm, USkr[j].nmtr);
      b2 := CoordinateToReal(USkr[j].kkm, USkr[j].kmtr);
      v21 := USkr[j].skp;
      v22 := USkr[j].skg;
      c2 := USkr[j].nmtr;

      if (i <> j) and (km12 = km21) and (km12 = GlbKmTrue) and (b1 <= a2) and
        (GetDistance(b1, a2) <= 10) and (a1 < b1) and (a2 < b2) and
        ((v11 <> v21) or (v12 <> v22)) and (0 <= c1) and (c2 <= GlbKmLength)
      then
      begin

        pt1 := c1;
        pt2 := c2;

        if c1 - 10 >= 0 then
          pt1 := c1 - 10;
        if c2 + 10 <= GlbKmLength then
          pt2 := c2 + 10;

        pt1 := Piket(pt1);
        pt2 := Piket(pt2);

        mVPik[count_].mtr1 := c1;
        mVPik[count_].pik1 := pt1;
        mVPik[count_].mtr2 := c2;
        mVPik[count_].pik2 := pt2;
        mVPik[count_].v11 := v11;
        mVPik[count_].v12 := v12;
        mVPik[count_].v21 := v21;
        mVPik[count_].v22 := v22;

        if napr_dbij = 1 then
        begin
          mVPik[count_].v11 := v21;
          mVPik[count_].v12 := v22;
          mVPik[count_].v21 := v11;
          mVPik[count_].v22 := v12;
        end;
        count_ := count_ + 1;
        // sablog('OK-' +inttostr(km12));
      end;
    end;
  end;
  setlength(mVPik, count_);

end;
// ------------------------------------------------------------------------------

// ----------------------------------------------------------------------
// Доп. фльтрация горловины стрелки
// ----------------------------------------------------------------------

procedure OprFactStrelki1;
var
  i, j, k, i0, im, ii, L, a, b, a1, b1, x0, xm: INTEGER;
  f: real;
  maxh, h, Strel_gorl, sg: real;

begin
  // SabLog('OprFactStrelki1 %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%');
  i := 0;
  i0 := 0;
  im := 0;
  ii := 0;
  k := 0;

  while i <= high(Frih1) do
  begin
    f := abs(Frih1[i]);
    // ------------------------------------
    j := i;
    x0 := 0;
    xm := 1;
    im := 1;
    while (f > 34) and (j < high(Frih1)) and (abs(furb[i]) < 20) do
    begin

      k := k + 1;
      if k = 1 then
        x0 := F_Mtr[j]
      else
      begin
        xm := F_Mtr[j];
        im := j;
      end;

      f := abs(Frih1[j]);
      j := j + 1;
    end;

    if (20 < abs(xm - x0)) and (abs(xm - x0) < 100) then
    begin
      k := 0;
      a := x0;
      b := xm;
      a1 := min(a, b);
      b1 := max(a, b);

      if a1 > 90 then
        a := a1 - 90
      else
        a := 1;
      if b1 + 90 <= GlbKmLength then
        b := b1 + 90
      else
        b := 999;
      setlength(fstrel, ii + 1);
      fstrel[ii].nachm := a;
      fstrel[ii].konm := b;

      // SabLog(inttostr( fstrel[ii].nachm) + ' - ' + inttostr(fstrel[ii].konm));
      ii := ii + 1;
    end; // if

    i := i + im;
  end; // while
  // ==============================================================================
  Strel_gorl := 51;
  i := 0;
  while i <= high(F_Mtr) - 100 do
  begin
    j := 0;
    maxh := 0;
    sg := 0;
    while j <= 100 do
    begin
      h := abs(Frih1[i + j]);
      if (maxh < h) and (abs(furb[i + j]) < 20) then
      begin
        maxh := h;
        a := F_Mtr[i];
        b := F_Mtr[i + j];
      end;
      j := j + 1;
    end;

    if maxh > Strel_gorl then
    begin
      if max(a, b) = b then
      begin
        a := a - 200;
        b := b + 200;
      end
      else
      begin
        a := a + 200;
        b := b - 200;
      end;
      setlength(fstrel, ii + 1);
      fstrel[ii].nachm := a;
      fstrel[ii].konm := b;
      ii := ii + 1;
    end;

    i := i + 100;
  end;
  // sablog(' strel km-'  + inttostr(glbkmtrue)) ;

end;

// доп
function CoordinateToReal(km, meter: INTEGER): real;
begin
  result := km + meter / 10000;
end;

procedure DopUklBozb;

  function G_UkOt(var Fx: real): INTEGER;
  var
    i, v: INTEGER;
    s1, s2: real;
  begin
    s1 := 0;
    s2 := 0;
    v := 140;

    for i := 1 to 16 do
    begin
      s1 := Omega[i, 1];
      s2 := Omega[i, 2];
      if (s1 < Fx) and (Fx <= s2) then
        v := Vu4[i];
    end;
    G_UkOt := v;
  end; // fun

var
  i, si, j, jj, dl, indfxu, k: INTEGER;
  V_ogranich, ots: string;
  prom, Vogr, v1, V2, vop, vog, vt, vtg, vr, vrg, lng: INTEGER;
  xcoord_, ycoord_: INTEGER;
  Fxu, ffxx: real;
  x1, x2, x3, x4, xx, xbegin, xend: real;
  a_min, b_min, Flag: INTEGER;
begin
  try
    if Flag_sablog then
      sablog('DopUklBozb - Определение допускаемый ли уклон отвода возвышения наружного рельса в кривых');

    Flag := 0;
    a_min := 0;
    b_min := 0;
    v1 := GlobPassSkorost;
    V2 := GlobGruzSkorost;

    for i := 0 to High(UKrv) do
    begin
      for si := 0 to High(UKrv[i].levels) do
      begin
        if ((GlbKmTrue = UKrv[i].nkm) or (GlbKmTrue = UKrv[i].kkm)) then
        begin
          x1 := CoordinateToReal(UKrv[i].nkm, UKrv[i].nmtr);
          x2 := UKrv[i].levels[si].x_kpk1;
          x3 := CoordinateToReal(UKrv[i].kkm, UKrv[i].kmtr);
          x4 := UKrv[i].levels[si].x_kpk2;

          for jj := 30 to high(Furb_sr) - 30 do
          begin
            xx := CoordinateToReal(GlbKmTrue, F_Mtr[jj]);
            if (x1 <= xx) and (xx <= x2) then
            begin
              xbegin := x1;
              xend := x2;
            end;
            if (x1 >= xx) and (xx >= x2) then
            begin
              xbegin := x2;
              xend := x1;
            end;
            if (x3 <= xx) and (xx <= x4) then
            begin
              xbegin := x3;
              xend := x4;
            end;
            if (x3 >= xx) and (xx >= x4) then
            begin
              xbegin := x4;
              xend := x3;
            end;

            Fxu := 0;
            indfxu := jj;
            if (xbegin <= xx) and (xx <= xend) and
              (((napr_dbij = 1) and (xx = xbegin)) or
              ((napr_dbij = -1) and (xx = xend))) then
            begin
              for k := jj + 30 to (high(Furb_sr) - 30) do
              begin
                dl := round(abs(F_Mtr[jj] - F_Mtr[k]));
                // dl := 30;
                if (jj <> k) then
                  if (Fxu < abs(TrapezLevel[jj] - TrapezLevel[k]) / dl) then
                  begin
                    Fxu := roundto(abs(TrapezLevel[jj] - TrapezLevel[k])
                      / dl, -2);
                  end;
                indfxu := k;
              end;
              if (Fxu < 3.3) and (Fxu > 0) then
              begin
                xcoord_ := F_Mtr[jj];
                ycoord_ := F_Mtr[indfxu];
                if indfxu > high(FncX) then
                begin
                  ycoord_ := F_Mtr[high(F_Mtr) - 1] + indfxu - high(F_Mtr);
                end;

                Vogr := G_UkOt(Fxu);
                V_ogranich := inttostr(Vogr);
                vt := F_V[jj];
                vtg := F_Vg[jj];
                vr := F_Vrp[jj];
                vrg := F_Vrg[jj];
                if not FlagSiezd(xcoord_, ycoord_) and
                  not ProberkaNaStrelku(xcoord_, xcoord_, 7) then
                begin
                  if ((v1 > Vogr) and (vt > Vogr)) or
                    ((V2 > Vogr) and (vtg > Vogr)) then
                  begin
                    if (v1 > Vogr) and (vt > Vogr) then
                      v1 := Vogr;
                    if (V2 > Vogr) and (vtg > Vogr) then
                      V2 := Vogr;
                    if (0 < vr) and (vr < v1) then
                      v1 := -1;
                    if (0 < vrg) and (vrg < V2) then
                      V2 := -1;
                    a_min := xcoord_;
                    b_min := ycoord_;
                    Flag := 1;
                    ffxx := Fxu;
                  end;
                end; // if
              end;

              if (ffxx > 0.7) and (Flag = 1) then
              begin

                if (V2 = vtg) then
                  V2 := -1;

                ots := ' V=' + V_shekti(v1, V2) + ' пк' +
                  inttostr((xcoord_ mod 1000) div 100 + 1) + 'Укл ' +
                  FormatFloat('0.00', ffxx) + 'мм/м;';
                // + ' v=' + V_shekti(V1,V2);
                lng := round(abs(xcoord_ - ycoord_));
                WRT_UBEDOM(a_min, b_min, 1, ots, v1, V2);
                // Ubedom_db(xcoord,1,0,lng,7, vt, vtg, v1, v2, ots, GlbTempTipRemontKm);
                wrt_baskalar('Укл', '', ffxx, a_min, lng, 5, 0, vt, vtg,
                  v1, V2, 1);
                GlbCountUKL := GlbCountUKL + 1;
                Flag := 0;
              end;
              if (ffxx > 0.6) and (ffxx <= 0.7) then
              begin
                wrt_baskalar('?Уkл', '', Fxu, FncX[indfxu], dl, 5, 0, vt, vtg,
                  -1, -1, 1);
              end;

            end; // if
          end;

        end; // for

      end; // if

    end; // for
  except
  end;
end;
// ------------------------------------------------------------------------------

function G_UkOt(var Fx: real): INTEGER;
var
  ii, v: INTEGER;
  s1, s2: real;
  i: real;
begin
  s1 := 0;
  s2 := 0;
  v := 140;
  G_UkOt := 140;
  for ii := 2 to 16 do
  begin
    s1 := Omega[ii, 1];
    s2 := Omega[ii, 2];
    if (s1 <= Fx) and (Fx < s2) then
    begin
      v := Vu4[ii];

    end;
  end;
  G_UkOt := v;
end; // fun
// ----------------------------------------------------------------------
// Sredni func dlya shablona
// ----------------------------------------------------------------------

procedure SredFuncSablon;
var
  i, j, m, k, L: INTEGER;
  s, fsr: real;
begin
  // SabLog(inttostr(F_sh_sred[i]));
  L := 0;
  m := 30;

  for i := 0 to high(F_sh) do
  begin
    s := 0;
    k := 0;

    fsr := F_sh[i];

    if high(F_sh) - m < i then
    begin

      for j := i to high(F_sh) do
      begin
        s := s + F_sh[j];
        k := k + 1;
      end;
      fsr := s / k;
    end;

    if i <= high(F_sh) - m then
    begin
      for j := i to i + m do
      begin
        s := s + F_sh[j];
        k := k + 1;
      end;
      fsr := s / k;
    end;

    F_sh_sred[i] := fsr;
  end;

end;

// ----------------------------------------------------------------------
// Допускаемая крутизна отвода шаблона при переходе от одной нормы к другой
// ----------------------------------------------------------------------

procedure DopUklBozb_Shablona;
var
  i, si, j, k, start_coord, final_coord, ogr, n, v1, V2, vt, vtg, vr, vrg,
    picket: INTEGER;
  Fxu, x1, x2, x3, x4, xx: real;
  ots: string;
begin
  try
    k := 0;
    n := 2;
    v1 := GlobPassSkorost;
    V2 := GlobGruzSkorost;
    for i := 0 to High(UKrv) do
    begin

      for si := 0 to High(UKrv[i].strights) do
      begin
        x1 := GetCoordByLen(UKrv[i].strights[si].nkm, UKrv[i].strights[si].nmtr,
          -n, GlbTrackId, GlbTripDate);
        x2 := GetCoordByLen(UKrv[i].strights[si].nkm, UKrv[i].strights[si].nmtr,
          UKrv[i].strights[si].l1 + n, GlbTrackId, GlbTripDate);
        x3 := GetCoordByLen(UKrv[i].strights[si].kkm, UKrv[i].strights[si].kmtr,
          n, GlbTrackId, GlbTripDate);
        x4 := GetCoordByLen(UKrv[i].strights[si].kkm, UKrv[i].strights[si].kmtr,
          -UKrv[i].strights[si].l2 - n, GlbTrackId, GlbTripDate);

        if ((GlbKmTrue = UKrv[i].nkm) or (GlbKmTrue = UKrv[i].kkm)) then
        begin
          for j := n to High(F_sh) - n do
          begin
            xx := CoordinateToReal(GlbKmTrue, F_Mtr[j]);
            Fxu := abs(F_sh[j + n] - F_sh[j]) / n;
            if (j < low(F_sh) + 30) or (high(F_sh) - 30 < j) then
              Continue;
            if (Fxu > 1.25) and (Fxu < 6.0) and
              ((((x1 <= xx) and (xx <= x2)) or ((x1 >= xx) and (xx >= x2))) or
              (((x3 <= xx) and (xx <= x4)) or ((x3 >= xxx) and (xx >= x4))))
            then
            begin
              start_coord := F_Mtr[j];
              final_coord := F_Mtr[j + n];
              k := k + 1;
              vt := F_V[j];
              vtg := F_Vg[j];
              vr := F_Vrp[j];
              vrg := F_Vrg[j];
              ogr := OpredelenieOgr_UklOtbShablon(Fxu);
              if not FlagSiezd(start_coord, final_coord) and
                not ProberkaNaStrelku(start_coord, start_coord, 7) then
              begin
                if ((v1 > ogr) and (vt > ogr)) or ((V2 > ogr) and (vtg > ogr))
                then
                begin
                  if (v1 > ogr) and (vt > ogr) then
                    v1 := ogr;
                  if (V2 > ogr) and (vtg > ogr) then
                    V2 := ogr;
                  if (0 < vr) and (vr < v1) then
                    v1 := -1;
                  if (0 < vrg) and (vrg < V2) then
                    V2 := -1;
                  picket := start_coord div 100 + 1;
                  ots := 'пк' + inttostr(picket) + ' ОШК ' +
                    FormatFloat('0.0', Fxu) + 'мм/м' + '; ';
                  WRT_UBEDOM(start_coord, final_coord, 7, ots, v1, V2);
                  wrt_baskalar('ОШК', '', Fxu, start_coord,
                    round(abs(start_coord - final_coord)), 5, 0, vt, vtg,
                    v1, V2, 1);
                  GlbCountDGR := GlbCountDGR + 1;
                end;
              end;
            end; // if
          end; // for
        end;
      end; // if
    end;
  except
  end;
end;

function get_usk_v(f: real): INTEGER;
var
  v: INTEGER;
begin
  v := 140;
  if (0.7 < f) and (f <= 1.0) then
    v := 120
  else if (1.0 < f) and (f <= 1.2) then
    v := 110
  else if (1.2 < f) and (f <= 1.4) then
    v := 100
  else if (1.4 < f) and (f <= 1.6) then
    v := 90
  else if (1.6 < f) and (f <= 1.7) then
    v := 85
  else if (1.7 < f) and (f <= 1.9) then
    v := 80
  else if (1.9 < f) and (f <= 2.1) then
    v := 75
  else if (2.1 < f) and (f <= 2.3) then
    v := 70
  else if (2.3 < f) and (f <= 2.5) then
    v := 65
  else if (2.5 < f) and (f <= 2.7) then
    v := 60
  else if (2.7 < f) and (f <= 2.9) then
    v := 55
  else if (2.9 < f) and (f <= 3.0) then
    v := 50
  else if (3.0 < f) and (f <= 3.1) then
    v := 40
  else if (3.1 < f) and (f <= 3.2) then
    v := 25
  else if (3.2 < f) then
    v := 0;
  get_usk_v := v;
end;

// ----------------------------------------------------------------------
// 24.08.2015 Опр. огр. ск. по несовп. нач. нараст. возв. с нач. нарас. стрел.
// ----------------------------------------------------------------------

// procedure Nesob_nash_naras_voz_s_rih;
// const
// kfforAnp = 0.0061;
// CNepUsk = 0.7; //  m/c*c
// Cpsi = 0.6; //  m/c*c*c
// var
// ii, i, j, Vmx, k: integer;
// Ri, An, Hm, Us2, Mas_Vpsi, Us, Vdps_: array of real;
// y1, y2, y3, y4: real;
// Ri_, An_, Frh, Phsi, Hi, prom, prom1_min, prom1_max, prom1, min_Vpsi,
// min_Vdps, min_Vdps2, Vdps, Vpsi, Vdop: real;
// Usk_nep, skr_Usk_nep, max_Vdps, R_i, max_Us, max_Us2: real;
// pik1, pik2, pik11, pik12, xcoord1, xcoord2, pro1, pro2: integer;
// Vogr, xVogr: integer;
// x1, x11, x12, x2, x3, x4, xxxx, kij, kij2,
// v1, v2, vt, vtg, km1, km2, m1, m2, a0, b0, ab, c, d: integer;
// jal1, jal2, jal3, jal4, KrivHaving: boolean;
// ots: string;
// begin
//
// Vmx := GlobPassSkorost;
// k := 0;
// R_i := 100000;
// min_Vdps := 10000;
// min_Vdps2 := 10000;
// vt := GlobPassSkorost;
// vtg := GlobGruzSkorost;
// KrivHaving := false;
// for j := 0 to High(UKrv) do
// begin
// if NPut(UKrv[j].put) <> NUMPUT then
// continue;
//
// km1 := UKrv[j].nkm;
// km2 := UKrv[j].kkm;
// m1 := UKrv[j].nmtr;
// m2 := UKrv[j].kmtr;
//
// jal1 := false;
// jal2 := false;
// if (nstCou > 0) and ((UKrv[j].nkm = GlbKmTrue) or (UKrv[j].kkm = GlbKmTrue))
// and ((UKrv[j].nmtr > 1000) or (UKrv[j].kmtr > 1000)) and (NPut(UKrv[j].put)
// = NumPut) then
// begin
// jal1 := true;
// // -1-
// if (km1 <> GlbKmTrue) and (m1 < 1000) and (km2 = GlbKmTrue) and (m2 < 1000)
// and (nstPart = 1) then
// begin
// c := 1;
// d := m2;
// end;
// // -2-
// if (km1 = GlbKmTrue) and (1 <= m1) and (m1 < 1000)
// and (km2 = GlbKmTrue) and (1 <= m2) and (m2 < 1000) and (nstPart = 1)
// then
// begin
// c := m1;
// d := m2;
// end;
// // -3-
// if (km1 = GlbKmTrue) and (1000 < m1) and (m1 < 2000)
// and (km2 = GlbKmTrue) and (1000 < m2) and (m2 < 2000) and (nstPart = 2)
// then
// begin
// c := m1 - 1000;
// d := m2 - 1000;
// end;
// // -4-1
// if (km1 = GlbKmTrue) and (1 <= m1) and (m1 < 1000)
// and (km2 = GlbKmTrue) and (1000 < m2) and (m2 < 2000) and (nstPart = 1)
// then
// begin
// c := m1;
// d := 1000;
// end;
// // -4-
// if (km1 = GlbKmTrue) and (1 <= m1) and (m1 < 1000)
// and (km2 = GlbKmTrue) and (1000 < m2) and (m2 < 2000) and (nstPart = 2)
// then
// begin
// c := 1;
// d := m2 - 1000;
// end;
// // -5-
// if (km1 = GlbKmTrue) and (1000 < m1) and (m1 < 2000)
// and (km2 <> GlbKmTrue) and (1 < m2) and (m2 < 1000) and (nstPart = 2)
// then
// begin
// c := m1 - 1000;
// d := 1000;
// end;
// //-5-2
// if NKm_(km1) and (km1 <> GlbKmTrue) and (1000 < m1) and (m1 < 2000)
// and (km2 = GlbKmTrue) and (1 < m2) and (m2 < 1000) and (nstPart = 3)
// then
// begin
// c := 1;
// d := m2;
// end;
//
// x1 := c;
// if c > 20 then
// x1 := c - 20;
// x2 := c + UKrv[j].d1 + 20;
// x3 := d - UKrv[j].d2 - 20;
// x4 := d + 20;
// end;
//
// if (nstCou = 0) and (NPut(UKrv[j].put) = NumPut)
// and (UKrv[j].nkm <= GlbKmTrue) and (GlbKmTrue <= UKrv[j].kkm) then
// begin
//
// jal2 := true;
// krivHaving := true;
// c := UKrv[j].nkm * 1000 + UKrv[j].nmtr;
// d := UKrv[j].kkm * 1000 + UKrv[j].kmtr;
// x1 := UKrv[j].nkm * 1000 + UKrv[j].nmtr - 20;
// x2 := UKrv[j].nkm * 1000 + UKrv[j].nmtr + UKrv[j].d1 + 20;
// x3 := UKrv[j].kkm * 1000 + UKrv[j].kmtr - UKrv[j].d2 - 20;
// x4 := UKrv[j].kkm * 1000 + UKrv[j].kmtr;
// end;
//
// max_Vdps := 100000;
// setlength(Ri, 0);
// setlength(An, 0);
// setlength(Us, 0);
// setlength(Us2, 0);
// setlength(Vdps_, 0);
// setlength(Hm, 0);
//
// for i := 0 to high(Fsr_rh1) - 20 do
// begin
// xxx := glbKmTrue * 1000 + round(F_mtr[i]);
//
// ii := 10;
//
// jal3 := false;
// if jal1 and (((x1 <= F_mtr[i]) and (F_mtr[i] < x2))
// or ((x3 <= F_mtr[i]) and (F_mtr[i] < x4))) then
// jal3 := true;
//
// jal4 := false;
// if jal2 and (((x1 <= xxx) and (xxx < x2))
// or ((x3 <= xxx) and (xxx < x4))) then
// jal4 := true;
//
// if jal4 or jal3 then
// begin
//
// setlength(Ri, k + 1);
// setlength(An, k + 1);
// setlength(Hm, k + 1);
// setlength(Us, k + 1);
// setlength(Us2, k + 1);
// setlength(Vdps_, k + 1);
// Frh := F_rh1[i] / kfRih * k_nusk;
// Hi := abs(Furb1[i]); //abs(Fsr_Urb[i]);
// R_i := 17860 / (abs(Frh) + 0.00000001);
//
// Vdps := trunc(sqrt((CNepUsk + abs(kfforAnp * Hi)) * 13 * R_i) / 5) * 5;
// Vdps_[k] := Vdps;
//
// Ri[k] := R_i;
// Hm[k] := abs(Furb1[i]); //abs(Fsr_Urb[i]);
//
// An[k] := xxx;
// Us[k] := (F_V[i] * F_V[i]) / (13.0 * R_i) - kfforAnp * abs(Furb1[i]);
// //abs(Fsr_Urb[i]);
//
// Us2[k] := (F_Vg[i] * F_Vg[i]) / (13.0 * R_i) - kfforAnp * abs(Furb1[i]);
// //abs(Fsr_Urb[i]);
// k := k + 1;
// end; //if
// end; //for
//
// max_Us := 0;
// max_Us2 := 0;
// kij := 0;
// kij2 := 0;
//
// for ii := 0 to high(Us) do
// begin
// if (max_Us < abs(Us[ii])) and (0.7 < abs(Us[ii])) then
// begin
// if kij = 0 then
// xcoord1 := round(An[ii])
// else
// xcoord2 := round(An[ii]);
//
// max_Us := Us[ii];
// Hi := F_rh1[ii] / kfRih * k_nusk;
// R_i := 17860 / (abs(Hi) + 0.00000001);
//
// min_Vdps := Vdps_[ii];
// // min_Vdps:= round(sqrt((CNepUsk + abs(kfforAnp*Hi))*13*R_i)/5)*5;
//
// kij := kij + 1;
// end;
// if(abs(Us[ii])<=0.7) then
// kij :=0;
//
//
//
// //      if (max_Us2 < abs(Us2[ii])) and (0.7 < abs(Us2[ii])) then
// //      begin
// //        if kij2 = 0 then
// //          xcoord1 := round(An[ii])
// //        else
// //          xcoord2 := round(An[ii]);
// //
// //        max_Us2 := Us2[ii];
// //        Hi := F_rh1[ii] / kfRih * k_nusk;
// //        R_i := 17860 / (abs(Hi) + 0.001);
// //
// //        //min_Vdps2:= round(sqrt((CNepUsk + abs(kfforAnp*Hm[ii]))*13*Ri[ii])/5)*5;
// //        min_Vdps2 := Vdps_[ii];
// //        kij2 := kij2 + 1;
// //      end;
// end;
// if (StrtoFloat(FormatFloat('0.00', max_Us)) > 0.7) then
// begin
// pik11 := (xcoord1 mod 1000) div 100 + 1; //+' пк'+ inttostr(pik)
// pik12 := (xcoord2 mod 1000) div 100 + 1; //+' пк'+ inttostr(pik)
// x11 := GlbKmIndex * 1000 + (xcoord1 mod 1000); //
// x12 := GlbKmIndex * 1000 + (xcoord2 mod 1000); //
// //min_Vdps:= get_usk_v(max_Us); //02.10.2012
// //min_Vdps2:= get_usk_v(max_Us2);
// v1 := round(min_Vdps);
// v2 := round(min_Vdps2);
//
// if GlobPassSkorost <= round(min_Vdps) then
// v1 := -1;
// if GlobGruzSkorost <= round(min_Vdps2) then
// v2 := -1;
// if not FlagSiezd(x11, x12) and KrivHaving and not ProberkaNaStrelku(x1, 7)
// and not ProberkaNaStrelku(x2, 1) and not ProberkaNaStrelku(x3, 7)
// and not ProberkaNaStrelku(x4, 7) and ((v1 >= 0) or (v2 >= 0)) then
// begin
//
// pik1 := (xcoord1 mod 1000) div 100 + 1;
// pik2 := (xcoord2 mod 1000) div 100 + 1;
// if (xcoord1 div 1000 > xcoord2 div 1000) and (xcoord1 mod 1000 = 0) then
// xcoord1 := xcoord1 -1;
// if (xcoord2 div 1000 > xcoord1 div 1000) and (xcoord2 mod 1000 = 0) then
// xcoord2 := xcoord2 -1;
// //showmessage(inttostr(xcoord1 div 1000) + '-' + inttostr(xcoord1 mod 1000) + '-'+inttostr(xcoord2 div 1000) + '-' + inttostr(xcoord2 mod 1000)) ;
// x1 := GlbKmIndex * 1000 + (xcoord1 mod 1000);
// x2 := GlbKmIndex * 1000 + (xcoord2 mod 1000);
//
// xxxx := (((x1 + x1) div 2) mod 1000) div 100 + 1;
// if Shekteu(x1, x2, v1, v2) then
// begin
// max_Us := max(max_Us, max_Us2);
// //showmessage(inttostr(x1) + '-' + inttostr(x2)+'-' + inttostr(xxxx));
// ots := 'Уск ' + FormatFloat('0.00', max_Us)
// + ' п' + inttostr(xxxx) + ';'; // + ' v='+ V_SHEKTI(v1, v2);
//
// if (round(abs(x1 - x2)) <= 20) then
// begin
// v1 := vt;
// v2 := vtg;
// end;
//
// if (round(abs(x1 - x2)) > 20) then
// begin
//
// WRT_UBEDOM(round(x1 * 100), round(x2 * 100), 3, ots, v1, v2);
// GlbCountUSK := GlbCountUSK + 1;
// end;
// //x1:= round((x1 + x2)/2);
//
// wrt_baskalar('Уск', max_Us, xcoord1, round(abs(x1 - x2)), 5, 0, vt,
// vtg,
// v1, v2, 1); // flg=1 grapotchetka shygaru
//
// end;
// end; //if
// end;
// end; //for
//
// end;
// ----------------------------------------------------------------------
// ЗАДАНИЕ РАЗМЕРА МАССИВА
// ----------------------------------------------------------------------
PROCEDURE Nesob_nash_naras_voz_s_rih;
const
  kfforAnp = 0.0061;
  CNepUsk = 0.7; // m/c*c
  Cpsi = 0.6; // m/c*c*c
var
  ii, i, j, sj, Vmx, k, xmod, v1, V2, v1_psi, v2_psi, vt, vtg, xxxx,
    len: INTEGER;
  Ri, An, Hm, Us, Us2, Mas_Vpsi, Vdps_, Psi, V_psi: array of real;
  y1, y2, y3, y4: real;
  Ri_, An_, Frh, Phsi, Hi, ANP_Hi, prom, prom1_min, prom1_max, prom1, min_Vpsi,
    min_Vdps, min_Vdps2, min_Vd_psi, min_Vd_psi2, Vdps, Vpsi, Vdop: real;
  Usk_nep, skr_Usk_nep, max_Vdps, R_i, ANP_R_i, max_Us, max_Us2, maxPsi,
    dAn: real;
  pik1, pik2, pik11, pik12, xcoord1, xcoord2, modify_xcoord, psix1, psix2,
    psiStep, pro1, pro2, psiLen: INTEGER;
  Vogr, xVogr: INTEGER;
  x1, x11, x12, x2, x3, x4, kij, kij2, km1, km2, m1, m2, a0, b0, ab, c, d,
    current_coord, x1_psi, x2_psi, x3_psi, x4_psi: real;
  jal1, jal2, jal3, jal4, KrivHaving: boolean;
  PsiRanges: array of PsiAnp;
  ots: string;
begin

  Vmx := GlobPassSkorost;
  k := 0;
  R_i := 100000;
  min_Vdps := 10000;
  min_Vdps2 := 10000;
  min_Vd_psi := 10000;
  vt := GlobPassSkorost;
  vtg := GlobGruzSkorost;
  KrivHaving := false;
  for j := 0 to High(UKrv) do
  begin
    for sj := 0 to High(UKrv[j].strights) do
    begin

      km1 := UKrv[j].strights[sj].nkm;
      km2 := UKrv[j].strights[sj].kkm;
      m1 := UKrv[j].strights[sj].nmtr;
      m2 := UKrv[j].strights[sj].kmtr;

      jal1 := false;
      jal2 := false;
      if (F_Mtr[0] < F_Mtr[high(F_Mtr)]) then
        psiLen := UKrv[j].strights[sj].l1
      else
        psiLen := UKrv[j].strights[sj].l2;

      if (UKrv[j].levels[sj].nkm <= GlbKmTrue) and
        (GlbKmTrue <= UKrv[j].levels[sj].kkm) then
      begin

        jal2 := true;
        KrivHaving := true;
        c := CoordinateToReal(UKrv[j].levels[sj].nkm, UKrv[j].levels[sj].nmtr);
        d := CoordinateToReal(UKrv[j].levels[sj].kkm, UKrv[j].levels[sj].kmtr);
        x1 := GetCoordByLen(UKrv[j].levels[sj].nkm, UKrv[j].levels[sj].nmtr,
          -20, GlbTrackId, GlbTripDate);
        x2 := GetCoordByLen(UKrv[j].levels[sj].nkm, UKrv[j].levels[sj].nmtr,
          +UKrv[j].levels[sj].l1 + 20, GlbTrackId, GlbTripDate);
        x3 := GetCoordByLen(UKrv[j].levels[sj].kkm, UKrv[j].levels[sj].kmtr,
          -UKrv[j].levels[sj].l2 - 20, GlbTrackId, GlbTripDate);
        x4 := CoordinateToReal(UKrv[j].levels[sj].kkm, UKrv[j].levels[sj].kmtr);
      end;

      max_Vdps := 100000;
      setlength(Ri, 2000);
      setlength(An, 2000);
      setlength(Us, 2500);
      setlength(Us2, 2500);
      setlength(Vdps_, 2500);
      setlength(Hm, 2500);
      setlength(GUs, high(Fsr_rh1) + 1);
      setlength(GUs2, high(Fsr_rh1) + 1);
      setlength(ANP_GUs, high(Fsr_rh1) + 1);
      setlength(ANP_GUs2, high(Fsr_rh1) + 1);

      for i := 0 to high(Fsr_rh1) - 20 do
      begin
        current_coord := CoordinateToReal(GlbKmTrue, F_Mtr[i]);

        ii := 10;
        jal3 := false;
        jal4 := false;
        if jal2 and (((x1 <= current_coord) and (current_coord < x2)) or
          ((x3 <= current_coord) and (current_coord < x4))) then
          jal4 := true;

        Frh := Fsr_rh1[i] * k_nusk;

        Hi := abs(LV_AVG[i]); // abs(Fsr_Urb[i]);
        R_i := 17860 / (abs(ST_AVG[i]) + 0.00000001);

        ANP_Hi := abs(TrapezLevel[i]); // abs(Fsr_Urb[i]);
        ANP_R_i := 17860 / (abs(TrapezStr[i]) + 0.00000001);

        Vdps := trunc(sqrt((CNepUsk + abs(kfforAnp * Hi)) * 13 * R_i) / 5) * 5;

        GUs[i] := (F_V[i] * F_V[i]) / (13.0 * R_i) - kfforAnp * Hi;
        GUs2[i] := (F_Vg[i] * F_Vg[i]) / (13.0 * R_i) - kfforAnp * Hi;

        ANP_GUs[i] := (F_V[i] * F_V[i]) / (13.0 * ANP_R_i) - kfforAnp * ANP_Hi;
        ANP_GUs2[i] := (F_Vg[i] * F_Vg[i]) / (13.0 * ANP_R_i) -
          kfforAnp * ANP_Hi;

        if jal3 or jal4 then
        begin

          Frh := Fsr_rh1[i] * k_nusk;
          Hi := abs(Furb_sr[i]); // abs(Fsr_Urb[i]);
          R_i := 17860 / (abs(Frh) + 0.00000001);

          Vdps := trunc(sqrt((CNepUsk + abs(kfforAnp * Hi)) * 13 * R_i)
            / 5) * 5;
          Vdps_[k] := Vdps;

          Ri[k] := R_i;
          Hm[k] := abs(Fsr_Urb[i]); // abs(Fsr_Urb[i]);

          An[k] := F_Mtr[i];
          setlength(V_psi, k + 1);
          V_psi[k] := F_V[i];
          Us[k] := ((F_V[i] * F_V[i]) / (13.0 * R_i) - kfforAnp *
            abs(Fsr_Urb[i]));
          // abs(Fsr_Urb[i]);

          Us2[k] := ((F_Vg[i] * F_Vg[i]) / (13.0 * R_i) - kfforAnp *
            abs(Fsr_Urb[i]));
          // abs(Fsr_Urb[i]);

          k := k + 1;
        end; // if
      end;
    end; // for

    setlength(Ri, k);
    setlength(An, k);
    setlength(Hm, k);
    setlength(Us, k);
    setlength(Us2, k);
    setlength(Vdps_, k);

    max_Us := 0;
    max_Us2 := 0;
    maxPsi := 0;
    kij := 0;
    kij2 := 0;
    psiStep := 0;
    if (psiLen >= high(Us)) then
      psiLen := high(Us) - 1;

    for ii := 0 to high(Us) do
    begin
      if (V_psi[ii] > 140) then
        psiLen := 40
      else
        psiLen := 20;
      if ((ii + psiLen < high(Us)) and (psiLen > 0)) then
      begin
        setlength(Psi, ii + 1);
        dAn := (Us[ii + psiLen] - Us[ii]);
        Psi[ii] := (dAn * V_psi[ii]) / (3.6 * psiLen);

        if (Cpsi + 0.1 < abs(Psi[ii])) then
        begin
          if psiStep = 0 then
            psix1 := round(An[ii])
          else
            psix2 := round(An[ii]);
          if maxPsi < abs(Psi[ii]) then
          begin
            maxPsi := abs(Psi[ii]);
            min_Vd_psi := trunc(((Cpsi * 3.6 * psiLen) / dAn) / 5) * 5;
          end;
          psiStep := psiStep + 1;
        end
        else if (psix1 - psix2 <> 0) then
        begin
          setlength(PsiRanges, length(PsiRanges) + 1);
          PsiRanges[length(PsiRanges) - 1].max := maxPsi;
          PsiRanges[length(PsiRanges) - 1].x1 := psix1;
          PsiRanges[length(PsiRanges) - 1].x2 := psix2;
          PsiRanges[length(PsiRanges) - 1].v := min_Vd_psi;
          min_Vd_psi := 10000;
          maxPsi := 0;
          psix1 := 0;
          psix2 := 0;
          psiStep := 0;
        end;

      end;

      if (max_Us < (GUs[ii])) then
      begin

        if kij = 0 then
          xcoord1 := round(An[ii])
        else
          xcoord2 := round(An[ii]);

        max_Us := (GUs[ii]);
        Hi := F_rh1[ii] / kfRih * k_nusk;
        R_i := 17860 / (abs(Hi) + 0.00000001);

        min_Vdps := Vdps_[ii];
        min_Vdps2 := Vdps_[ii];
        // min_Vdps:= round(sqrt((CNepUsk + abs(kfforAnp*Hi))*13*R_i)/5)*5;

        kij := kij + 1;
      end;

      if (max_Us2 < (GUs2[ii])) then
      begin
        max_Us2 := (GUs2[ii]);
      end;
    end;

    if (psix1 - psix2 <> 0) then
    begin
      setlength(PsiRanges, length(PsiRanges) + 1);
      PsiRanges[length(PsiRanges) - 1].max := maxPsi;
      PsiRanges[length(PsiRanges) - 1].x1 := psix1;
      PsiRanges[length(PsiRanges) - 1].x2 := psix2;
      PsiRanges[length(PsiRanges) - 1].v := min_Vd_psi;
      min_Vd_psi := 10000;
      maxPsi := 0;
      psix1 := 0;
      psix2 := 0;
      psiStep := 0;
    end;

    pik11 := xcoord1 div 100 + 1;
    pik12 := xcoord2 div 100 + 1;
    x11 := xcoord1;
    x12 := xcoord2;

    v1 := round(min_Vdps);
    V2 := round(min_Vdps2);

    if GlobPassSkorost <= round(min_Vdps) then
      v1 := -1;
    if GlobGruzSkorost <= round(min_Vdps2) then
      V2 := -1;

  end; // for

  // скорость нарастания непогашного ускорения ПСИ
  for ii := 0 to High(PsiRanges) do
  begin
    psix1 := PsiRanges[ii].x1;
    psix2 := PsiRanges[ii].x2;
    min_Vd_psi := PsiRanges[ii].v;
    maxPsi := PsiRanges[ii].max;
    v1_psi := round(min_Vd_psi);
    v2_psi := round(min_Vd_psi);
    if (GlobPassSkorost < v1_psi) then
      v1_psi := -1;
    if (GlobGruzSkorost < v2_psi) then
      v2_psi := -1;

    if not FlagSiezd(psix1, psix2) and KrivHaving and
      not ProberkaNaStrelkuReal(x1, 7) and not ProberkaNaStrelkuReal(x2, 1) and
      not ProberkaNaStrelkuReal(x3, 7) and not ProberkaNaStrelkuReal(x4, 7) and
      ((v1_psi >= 0) or (v2_psi >= 0)) then
    begin

      pik1 := xcoord1 div 100 + 1;
      pik2 := xcoord2 div 100 + 1;
      x1_psi := CoordinateToReal(GlbKmTrue, psix1);
      x2_psi := CoordinateToReal(GlbKmTrue, psix2);

      xmod := GetDistance(x1_psi, x2_psi) mod 10;
      xxxx := ((psix1 + psix2) div 2) div 100 + 1;
      case round(GetDistance(x1_psi, x2_psi)) of
        0 .. 160:
          xxxx := xxxx;
        161 .. 200:
          begin
            x2_psi := x1_psi + (115 + xmod) / 10000;
          end;
        201 .. 300:
          begin
            x2_psi := x1_psi + (120 + xmod) / 10000;
          end;
        301 .. 500:
          begin
            x2_psi := x1_psi + (125 + xmod) / 10000;
          end;
        501 .. 600:
          begin
            x2_psi := x1_psi + (130 + xmod) / 10000;
          end;
        601 .. 700:
          begin
            x2_psi := x1_psi + (135 + xmod) / 10000;
          end;
        701 .. 800:
          begin
            x2_psi := x1_psi + (140 + xmod) / 10000;
          end;
      else
        begin
          x2_psi := x1_psi + (145 + xmod) / 10000;
        end;

      end;
      len := GetDistance(x1_psi, x2_psi);
      if Shekteu(psix1, psix2, v1_psi, v2_psi) then
      begin

        ots := 'пк' + inttostr(xxxx) + ' Пси ' + FormatFloat('0.00',
          maxPsi) + '; ';
        // + ' v='+ V_SHEKTI(v1, v2);

        if len <= 20 then
        begin
          v1_psi := vt;
          v2_psi := vtg;
        end;
        if ((VRflag(psix1, 1) <= v1_psi) and
          (GlobPassSkorost > VRflag(psix1, 1))) then
        begin
          v1_psi := -1;
          v2_psi := -1;
        end;
        len := GetDistance(x1_psi, x2_psi);
        if (len > 20) and (maxPsi > Cpsi) then
        begin

          WRT_UBEDOM(psix1, psix1 + len, 3, ots, v1_psi, v2_psi);
          GlbCountDGR := GlbCountDGR + 1;
        end;
        if (maxPsi > 0.6) then
          ots := 'Пси'
        else
          ots := '?Пси';
        wrt_baskalar(ots, '', maxPsi, psix1, len, 5, 0, vt, vtg, v1_psi,
          v2_psi, 1);
      end
      else if maxPsi > 0.5 then
        wrt_baskalar('?Пси', '', maxPsi, psix1, len, 5, 0, vt, vtg, -1, -1, 1);

    end; // скорость нарастания не погашенного ускорения
  end;

  if not FlagSiezd(xcoord1, xcoord2) and KrivHaving and
    not ProberkaNaStrelkuReal(x1, 7) and not ProberkaNaStrelkuReal(x2, 1) and
    not ProberkaNaStrelkuReal(x3, 7) and not ProberkaNaStrelkuReal(x4, 7) and
    ((v1 >= 0) or (V2 >= 0)) then
  begin
    pik1 := xcoord1 div 100 + 1;
    pik2 := xcoord2 div 100 + 1;
    x1 := CoordinateToReal(GlbKmTrue, xcoord1);
    x2 := CoordinateToReal(GlbKmTrue, xcoord2);

    xmod := GetDistance(x1, x2) mod 10;
    xxxx := ((xcoord1 + xcoord2) div 2) div 100 + 1;
    case round(GetDistance(x1, x2)) of
      0 .. 160:
        xxxx := xxxx;
      161 .. 200:
        begin
          x2 := x1 + (115 + xmod) / 10000;
        end;
      201 .. 300:
        begin
          x2 := x1 + (120 + xmod) / 10000;
        end;
      301 .. 500:
        begin
          x2 := x1 + (125 + xmod) / 10000;
        end;
      501 .. 600:
        begin
          x2 := x1 + (130 + xmod) / 10000;
        end;
      601 .. 700:
        begin
          x2 := x1 + (135 + xmod) / 10000;
        end;
      701 .. 800:
        begin
          x2 := x1 + (140 + xmod) / 10000;
        end;
    else
      begin
        x2 := x1 + (145 + xmod) / 10000;
      end;

    end;

    // Убрали АНП, теперь запись будет с АЛАРМ ПП ВЕБ
    // if Shekteu(xcoord1, xcoord2, v1, V2) then
    // begin

    // max_Us := max(max_Us, max_Us2);
    // ots := 'V=' + inttostr(v1) + '/' + '-' + ' пк' + IntToStr(xxxx) +' А_нп ' + FormatFloat('0.00', max_Us) +  '; ';
    // + ' v='+ V_SHEKTI(v1, v2);
    // len := GetDistance(x1,x2);
    // if (len <= 20) then
    // begin
    // v1 := vt;
    // V2 := vtg;
    // end;
    // if ((VRflag(xcoord1, 1) <= v1) and
    // (GlobPassSkorost > VRflag(xcoord1, 1))) then
    // begin
    // v1 := -1;
    // V2 := -1;
    // end;

    // if (len > 20) and (max_Us>CNepUsk) then
    // begin


    // WRT_UBEDOM(xcoord1, xcoord2, 3, ots, v1, V2);
    // GlbCountUSK := GlbCountUSK + 1;
    // end;
    // x1:= round((x1 + x2)/2);
    // if (max_Us>0.7) then
    // ots:='Анп' else
    // begin
    // ots:='?Анп';
    // Writeln('?Анп' + floattostr(max_Us));
    // end;

    // wrt_baskalar(ots,'П:' + FormatFloat('0.00', max_Us) + ' Г:'+ FormatFloat('0.00', max_Us2), max_Us, round((xcoord1 + xcoord2)/2), len, 5, 0, vt, vtg, v1, V2, 1); // flg=1 grapotchetka shygaru
    // end;
  end; // if

end;

procedure LENGTH_MASFUNCS;
begin
  // SabLog('LENGTH_MASFUNCS - начальная длина динамических массивов');
  setlength(f_facstrel, 2000);
  setlength(FncX, 2000);
  setlength(F_rkm, 2000);
  setlength(F_pik, 2000);
  setlength(F_Mtr, 2000);
  setlength(F_sh, 2000);
  setlength(F_sh11, 2000);
  setlength(F_Wear, 2000);
  setlength(F_Sht, 2000);
  setlength(F_Sht11, 2000);

  setlength(F_Pr1, 2000);
  setlength(F_Pr2, 2000);
  setlength(F_rh1, 2000);
  setlength(F_Rh2, 2000);
  setlength(F_urb, 2000);
  setlength(Urob, 2000);
  setlength(F_V, 2000);
  setlength(F_Vg, 2000);
  setlength(F_Vrp, 2000);
  setlength(F_Vrg, 2000);

  setlength(F_urb_Per_sr, 2000);
  setlength(F_urb_Per, 2000);
  setlength(F_urb_Per1, 2000);
  setlength(Frih1, 2000);
  setlength(Frih2, 2000);
  setlength(furb, 2000);
  setlength(FPro1, 2000);
  setlength(FPro2, 2000);
  setlength(Speed_, 2000);
  setlength(angY_sred, 2000);
  setlength(angx_sred, 2000);

  setlength(Fsr_rh1, 2000);
  setlength(Fsr_Rh2, 2000);
  setlength(Fsr_Urb, 2000);
  setlength(F_NORM, 2000);
  setlength(F_PUCH, 2000);
  setlength(S_NORM, 2000);

  setlength(F0_sh, 2000);

  setlength(F0_urov, 2000);
  setlength(F0_rih1, 2000);
  setlength(F0_rih2, 2000);

  setlength(F_fluk, 2000);
  setlength(F_fluk_notriht, 2000);
  setlength(Fluk_right, 2000);
  setlength(Fluk_left, 2000);

  setlength(Furb1, 2000);
  setlength(Furbx, 2000);
  setlength(Furb_sr, 2000);
  setlength(F_sh_sred, 2000);
  setlength(F_rad, 2000);

  setlength(Shpaly, 2000);

  setlength(Rih_Nit, 2000);
  setlength(Fn0, 2000);

end;




// ----------------------------------------------------------------------
// Опред. радиуса крив.
// ----------------------------------------------------------------------

function Radius_krivoi(pfsr: real): INTEGER;
var
  j: INTEGER;
begin

  for j := 0 to TabKrivCnt - 1 do
  begin
    if (TabKrv[j].fsr >= pfsr) and (pfsr > TabKrv[j + 1].fsr) then
    begin
      Radius_krivoi := TabKrv[j].Rad;
      break;
    end;
  end;
end;
// ----------------------------------------------------------------------
// Определение кривого // жумыс кылады
// ----------------------------------------------------------------------

procedure Opred_Krivoi;
const
  Hf = 7; // ищем кривой если высота рихтовки 7 мм
  Lf = 100; // минимальная длина кривой который ищем
  Pconst = 10; // Площадь нижнеи части кривого возмем 10 процент макс. площади

var
  i, j, k, Ipik, i_nach, Rad, ipikinf: INTEGER;
  Hk, sk, sh, s_max, s_min, sk_min, smax, prm: real;
begin
  setlength(GlobNturKrv, 2000);

  s_min := Hf * 1000; // площад единичного прямоугольника
  s_max := s_min * Lf; // площадь 100 м -го прямоугольника ()

  sk_min := (s_min * Pconst) / 100;
  // 10 процентов площади единичного прямоугольника

  i_nach := 0;
  sh := 0;
  sk := 0;
  k := 0;
  smax := s_max;
  j := 0;

  for i := 0 to high(Fsr_rh1) - 1 do
  begin
    Hk := abs(Fsr_rh1[i + 1] + Fsr_rh1[i]) / 2.0;
    prm := Hk * abs(F_Mtr[i + 1] - F_Mtr[i]) * 1000; // переведем все на мм
    sk := sk + prm;

    if ((i mod 100) = 0) then
    begin
      if (s_max < sk) then
      begin
        sh := sh + Hk;
        Ipik := i div 100;
        GlobNturKrv[k].NumPik := Ipik;
        GlobNturKrv[k].NumInfPik := 0;
        GlobNturKrv[k].InfKrivoi := 'x';
        k := k + 1;
        if (k = 1) then
        begin
          Global_krv_nach := round(F_Mtr[i]);
          ipikinf := Ipik;
        end;
        if (smax < sk) then
          smax := sk;

      end;

      sk := 0;
    end; // if

  end; // for

  if (smax > s_max) then
  begin

    if (k > 0) then
      Global_Krv_Fsr := sh / k;
    Rad := Radius_krivoi(Global_Krv_Fsr);
    Global_radius := Rad;

    GlbInfKrivoi := 'м: ' + inttostr(Global_krv_nach) + ' R: ' + inttostr(Rad) +
      ' h: ' + FormatFloat('0.00', Global_Krv_Fsr) + ' Ш:1520';

    GlobNturKrv[k].NumPik := ipikinf;
    GlobNturKrv[k].NumInfPik := ipikinf;
    GlobNturKrv[k].InfKrivoi := GlbInfKrivoi;
    k := k + 1;

    // SabLog('Opred_Krivoi - Найдено кривая: ' +GlbInfKrivoi);
  end;
  setlength(GlobNturKrv, k);

end;
// ----------------------------------------------------------------------
//
// ----------------------------------------------------------------------

procedure Tegisteu_F0_sh;
var
  i, d, j, k: INTEGER;
  s1: real;
begin
  for i := 0 to high(F0_sh) - 1 do
  begin
    d := round(abs(F0_sh[i] - F0_sh[i + 1]));

    if d >= 4 then
    begin

      if (F0_sh[i] > F0_sh[i + 1]) and (abs(F0_sh[i] - F0_sh[i + 1]) >= 4) then
      begin
        k := i - 20;
        if (i - 20 < 0) then
          k := i;

        s1 := 1524;

        for j := k to i + 1 do
        begin
          s1 := s1 - 4 / 20;
          F0_sh[j] := s1;
        end;
      end;

      if (F0_sh[i] < F0_sh[i + 1]) and (abs(F0_sh[i] - F0_sh[i + 1]) >= 4) then
      begin
        k := i + 21;
        if (i + 21 > High(F0_sh)) then
          k := High(F0_sh);

        s1 := 1520;
        for j := i + 1 to k do
        begin
          s1 := s1 + 4 / 20;
          F0_sh[j] := s1;
        end;
      end;

    end;

  end;
end;
// ============================================================================

procedure LenMasEnd;
begin
  // U_LNG:= U_IND;
  { F_sh[0] := 1520;
    F_sh[1] := 1520;
    F_SH[U_LNG]  := 1520;
    F_SH[U_LNG-1]  := 1520; }
  setlength(f_facstrel, U_LNG);
  setlength(F_pik, U_LNG);
  setlength(F_rkm, U_LNG);
  setlength(F_Mtr, U_LNG);
  setlength(FncX, U_LNG);
  setlength(F_sh, U_LNG);
  setlength(F_sh11, U_LNG);
  setlength(F_Wear, U_LNG);
  setlength(F_Sht, U_LNG);
  setlength(F_Sht11, U_LNG);
  setlength(F_Pr1, U_LNG);
  setlength(F_Pr2, U_LNG);
  setlength(F_rh1, U_LNG);
  setlength(F_Rh2, U_LNG);
  setlength(F_urb, U_LNG);
  setlength(Urob, U_LNG);
  setlength(F_V, U_LNG);
  setlength(F_Vg, U_LNG);
  setlength(F_Vrp, U_LNG);
  setlength(F_Vrg, U_LNG);

  setlength(Rih1, U_LNG);
  setlength(Rih2, U_LNG);
  setlength(Frih1, U_LNG);
  setlength(Frih2, U_LNG);
  setlength(furb, U_LNG);
  setlength(FPro1, U_LNG);
  setlength(FPro2, U_LNG);
  setlength(Speed_, U_LNG);
  setlength(angY_sred, U_LNG);
  setlength(angx_sred, U_LNG);
  setlength(F_NORM, U_LNG);
  setlength(F_PUCH, U_LNG);
  setlength(S_NORM, U_LNG);

  setlength(F_rad, U_LNG);
  setlength(F0_sh, U_LNG);
  setlength(F0_Pr1, U_LNG);
  setlength(F0_Pr2, U_LNG);
  setlength(F0_Rh1, U_LNG);
  setlength(F0_Rh2, U_LNG);
  setlength(F0_urb, U_LNG);
  setlength(Fsr_Sh, U_LNG);
  setlength(Fsr_rh1, U_LNG);
  setlength(Fsr_Rh2, U_LNG);
  setlength(Fsr_Urb, U_LNG);
  setlength(F_urb_Per, U_LNG);

  setlength(F0_urov, U_LNG);
  setlength(F0_rih1, U_LNG);
  setlength(F0_rih2, U_LNG);

  setlength(F_fluk, U_LNG);
  setlength(F_fluk_notriht, U_LNG);
  setlength(Fluk_right, U_LNG);
  setlength(Fluk_left, U_LNG);
  setlength(Furb1, U_LNG);
  setlength(Furbx, U_LNG);
  setlength(Furb_sr, U_LNG);

  setlength(F_sh_sred, U_LNG);
  setlength(F_rad, U_LNG);
  setlength(Shpaly, U_LNG);

  setlength(Rih_Nit, U_LNG);
  setlength(Fn0, U_LNG);
end;
// ------------------------------------------------------------------------------
//
// ------------------------------------------------------------------------------

function FlagBirdeiKmTrue(kilometr: INTEGER): boolean;
var
  tmp_kmtrue, tmp_ind, reccount, tpt: INTEGER;
begin
  FlagBirdeiKmTrue := false;
  reccount := 0;
  AUYSU_flg := false;

  if FileExists(Path_km_shifrovka_file + 'km_' + inttostr(kilometr + napr_dbij)
    + '.svgpdat') then
  begin
    AssignFile(km_shifrovka_file, Path_km_shifrovka_file + 'km_' +
      inttostr(kilometr + napr_dbij) + '.svgpdat');
    reset(km_shifrovka_file);
    Readln(km_shifrovka_file); // начальная станция
    Readln(km_shifrovka_file); // конечная станция
    Readln(km_shifrovka_file); // начальник смены
    Readln(km_shifrovka_file); // стартовый км
    Readln(km_shifrovka_file, PARITY_F); // главный -1, четный - 2, нечетный - 1
    Readln(km_shifrovka_file); // прямой(+1) или обратный (-1)
    Readln(km_shifrovka_file); // дата проезда
    Readln(km_shifrovka_file); // номер вагона
    Readln(km_shifrovka_file, tmp_ind); // км
    Readln(km_shifrovka_file, tmp_kmtrue); // настоящий км

    while not eof(km_shifrovka_file) do
    begin
      Readln(km_shifrovka_file);
      reccount := reccount + 1;
    end;

    if reccount = 0 then
      END_km := true;

    if (0 <= reccount) and (reccount <= 100) then
      Korreksya := reccount;

    if (reccount > 30) then
    begin

      // --------------------------
      GFlag_Km1683 := false;
      if (Glb_PutList_GNapr = 'Илецк-Кандагач') and (reccount > 900) and
        (tmp_kmtrue = 1683) then
      begin
        GFlag_Km1683 := true;
        Id_Km1683 := kilometr + napr_dbij;
      end;
      // --------------------------
      GFlag_Km2501 := false;
      if (Glb_PutList_GNapr = 'Кандагач-Алматы1') and (reccount > 900) and
        (tmp_kmtrue = 2501) then
      begin
        GFlag_Km2501 := true;
        Id_Km2501 := kilometr + napr_dbij;
      end;
      // --------------------------

      POP_flg := false;
      tpt := NPut(PARITY_F);
      if (tpt > 2) then
        POP_flg := true;

      if (GlbKmTrue = tmp_kmtrue) and (tpt = numput) and (numput <= 2)
      { and (Glb_NestKm_count = 0) } then
      begin
        FlagBirdeiKmTrue := true;
        GlbBirdeiKmLen := GlbBirdeiKmLen + reccount / 1000;
        gk := gk + 1;
      end;

      // Vstavka 03/06/2012 begin

      if (GlbKmTrue <> 0) and (abs(GlbKmTrue - tmp_kmtrue) > 1) then
        Vstavka_flag := true;
      // Vstavka 03/06/2012 end

      if (tpt <> numput) then
      begin
        AUYSU_flg := true;
        aldyngy_km := kilometr + napr_dbij;
      end;

      // if GFlag_Km2501 or GFlag_Km1683 then FlagBirdeiKmTrue:= false;

    end;
  end;
end;
// ----------------------------------------------------------------------
//
// ----------------------------------------------------------------------

procedure LoadDefData;
var
  fw, sw, dw: textfile;
  D_km, D_PIC, D_sm, xk, D_v, NU, i, tleng: INTEGER;
  D_rh1, D_rh2, D_sh, D_ur1, D_ur2, D_urb, x_Angle, y_angle, s, prom,
    factst: real;
  D_mtr, jj, lng, ii, i5, prom1: INTEGER;
  Z_Angle, dv, dsurb, dsurb0, drih0: real;
  tmfil: string;
begin
  try
    if Flag_sablog then
      sablog('Defdata');
    tmfil := pathMB + 'loginfo\default.svgpdat';

    setlength(farr, 0);

    if FileExists(tmfil) then
    begin
      // SabLog('Defdata');
      tleng := 0;
      setlength(farr, 1100);
      AssignFile(dw, tmfil);
      reset(dw);

      while not eof(dw) do
      begin
        Readln(dw, indi, dv, D_PIC, D_mtr, D_ur2, D_ur1, D_sh, D_rh2, D_rh1,
          D_urb, Z_Angle, dsurb, dsurb0); // ,factst);
        farr[tleng].f1 := indi;
        farr[tleng].f2 := round(dv);
        farr[tleng].f3 := D_PIC;
        farr[tleng].f4 := D_mtr;
        farr[tleng].f5 := D_ur2;
        farr[tleng].f6 := D_ur1;
        farr[tleng].f7 := D_sh;
        farr[tleng].f8 := D_rh2;
        farr[tleng].f9 := D_rh1;
        farr[tleng].f10 := D_urb;
        farr[tleng].f11 := Z_Angle;
        farr[tleng].f12 := dsurb;
        farr[tleng].f13 := dsurb0;
        // farr[tleng].f14 := factst;
        tleng := tleng + 1;
      end;

      CloseFile(dw);
      setlength(farr, tleng);
    end;
  except
  end;
end;
// ----------------------------------------------------------------------
//
// ----------------------------------------------------------------------

procedure KM2File(Name_Km_SVGP: string);
var
  // farr:mtmpr;
  fw, sw, dw: textfile;
  D_km, D_PIC, D_sm, xk, D_v, NU, i: INTEGER;
  D_rh1, D_rh2, D_sh, D_ur1, D_ur2, D_urb, x_Angle, y_angle, s, prom,
    factst: real;
  D_mtr, jj, lng, ii, i5, prom1: INTEGER;
  Z_Angle, dv, dsurb, dsurb0, drih0: real;

begin
  try
    // if FileExists(Path_km_shifrovka_file + 'km_'+IntToStr(IPKM)+'.svgpdat') then begin
    if Flag_sablog then
      sablog('RWTB_PRK - Чтение определенного километра из БД Айдына');
    // ------------------------------------------------------------------------------

    if GFlag_Km1683 then
    begin
      U_IND := 0;

      AssignFile(dw, Name_Km_SVGP);

      reset(dw);
      Readln(dw, START_ST_F); // начальная станция
      Readln(dw, END_ST_F); // конечная станция
      Readln(dw, CHIEF_F); // начальник смены
      Readln(dw, STARTKM); // стартовый км
      Readln(dw, PARITY_F); // главный -1, четный - 2, нечетный - 1
      Readln(dw, DIRECTION_F); // прямой(+1) или обратный (-1)
      Readln(dw, Dateofinser); // дата проезда
      Readln(dw, NUMBERCAR_F); // номер вагона
      Readln(dw, D_km); // км
      Readln(dw, D_rkm); // настоящий км

      // AssignFile(sw, Path_km_shifrovka_file + '1683p1.svgpdat');
      AssignFile(sw, '1683p1.svgpdat');
      rewrite(sw);
      writeln(sw, START_ST_F); // начальная станция
      writeln(sw, END_ST_F); // конечная станция
      writeln(sw, CHIEF_F); // начальник смены
      writeln(sw, STARTKM); // стартовый км
      writeln(sw, PARITY_F); // главный -1, четный - 2, нечетный - 1
      writeln(sw, DIRECTION_F); // прямой(+1) или обратный (-1)
      writeln(sw, Dateofinser); // дата проезда
      writeln(sw, NUMBERCAR_F); // номер вагона
      writeln(sw, D_km); // км
      writeln(sw, D_rkm);

      // AssignFile(fw, Path_km_shifrovka_file + '1683p2.svgpdat');
      AssignFile(fw, '1683p2.svgpdat');
      rewrite(fw);
      writeln(fw, START_ST_F); // начальная станция
      writeln(fw, END_ST_F); // конечная станция
      writeln(fw, CHIEF_F); // начальник смены
      writeln(fw, STARTKM); // стартовый км
      writeln(fw, PARITY_F); // главный -1, четный - 2, нечетный - 1
      writeln(fw, DIRECTION_F); // прямой(+1) или обратный (-1)
      writeln(fw, Dateofinser); // дата проезда
      writeln(fw, NUMBERCAR_F); // номер вагона
      writeln(fw, D_km); // км
      writeln(fw, D_rkm);

      while not eof(dw) do
      begin
        { readln(km_shifrovka_file, indi, dv, D_pic, D_mtr, D_ur2, D_ur1, D_sh,
          D_rh2, D_rh1, D_urb, Z_Angle, dsurb,dsurb0); }

        Readln(dw, indi, dv, D_PIC, D_mtr, D_ur2, D_ur1, D_sh, D_rh2, D_rh1,
          D_urb, Z_Angle, dsurb, dsurb0);
        // ,factst);//,factst,factst,factst,factst,factst,factst);

        if (1 <= D_PIC) and (D_PIC <= 4) then
          writeln(sw, indi, dv:8:1, D_PIC:8, D_mtr:8, D_ur2:8:3, D_ur1:8:3,
            D_sh:12:3, D_rh2:8:3, D_rh1:8:3, D_urb:8:3, Z_Angle:8:3, dsurb:8:3,
            dsurb0:8:3)
          // ,factst:8:3)//,factst,factst,factst,factst,factst,factst)
        else if (5 <= D_PIC) and (D_PIC <= 10) then
          writeln(fw, indi, dv:8:1, D_PIC:8, D_mtr:8, D_ur2:8:3, D_ur1:8:3,
            D_sh:12:3, D_rh2:8:3, D_rh1:8:3, D_urb:8:3, Z_Angle:8:3, dsurb:8:3,
            dsurb0:8:3);
        // ,factst:8:3);//,factst,factst,factst,factst,factst,factst)

      end; // WHILE

      CloseFile(dw);

      CloseFile(sw);
      CloseFile(fw);
    end;

    if GFlag_Km2501 then
    begin
      U_IND := 0;

      Name_Km_SVGP := Path_km_shifrovka_file + 'km_' + inttostr(Id_Km2501) +
        '.svgpdat';
      AssignFile(dw, Name_Km_SVGP);

      reset(dw);
      Readln(dw, START_ST_F); // начальная станция
      Readln(dw, END_ST_F); // конечная станция
      Readln(dw, CHIEF_F); // начальник смены
      Readln(dw, STARTKM); // стартовый км
      Readln(dw, PARITY_F); // главный -1, четный - 2, нечетный - 1
      Readln(dw, DIRECTION_F); // прямой(+1) или обратный (-1)
      Readln(dw, Dateofinser); // дата проезда
      Readln(dw, NUMBERCAR_F); // номер вагона
      Readln(dw, D_km); // км
      Readln(dw, D_rkm); // настоящий км

      // AssignFile(sw, Path_km_shifrovka_file + '2501p1.svgpdat');
      AssignFile(sw, '2501p1.svgpdat');
      rewrite(sw);
      writeln(sw, START_ST_F); // начальная станция
      writeln(sw, END_ST_F); // конечная станция
      writeln(sw, CHIEF_F); // начальник смены
      writeln(sw, STARTKM); // стартовый км
      writeln(sw, PARITY_F); // главный -1, четный - 2, нечетный - 1
      writeln(sw, DIRECTION_F); // прямой(+1) или обратный (-1)
      writeln(sw, Dateofinser); // дата проезда
      writeln(sw, NUMBERCAR_F); // номер вагона
      writeln(sw, D_km); // км
      writeln(sw, D_rkm);

      // AssignFile(fw, Path_km_shifrovka_file + '2501p2.svgpdat');
      AssignFile(fw, '2501p2.svgpdat');
      rewrite(fw);
      writeln(fw, START_ST_F); // начальная станция
      writeln(fw, END_ST_F); // конечная станция
      writeln(fw, CHIEF_F); // начальник смены
      writeln(fw, STARTKM); // стартовый км
      writeln(fw, PARITY_F); // главный -1, четный - 2, нечетный - 1
      writeln(fw, DIRECTION_F); // прямой(+1) или обратный (-1)
      writeln(fw, Dateofinser); // дата проезда
      writeln(fw, NUMBERCAR_F); // номер вагона
      writeln(fw, D_km); // км
      writeln(fw, D_rkm);

      while not eof(dw) do
      begin
        Readln(dw, indi, dv, D_PIC, D_mtr, D_ur2, D_ur1, D_sh, D_rh2, D_rh1,
          D_urb, Z_Angle, dsurb, dsurb0); // ,factst);

        if (1 <= D_PIC) and (D_PIC <= 4) then
          writeln(sw, indi, dv:8:1, D_PIC:8, D_mtr:8, D_ur2:8:3, D_ur1:8:3,
            D_sh:12:3, D_rh2:8:3, D_rh1:8:3, D_urb:8:3, Z_Angle:8:3, dsurb:8:3,
            dsurb0:8:3) // ,factst:8:3)
        else if (5 <= D_PIC) and (D_PIC <= 10) then
          writeln(fw, indi, dv:8:1, D_PIC:8, D_mtr:8, D_ur2:8:3, D_ur1:8:3,
            D_sh:12:3, D_rh2:8:3, D_rh1:8:3, D_urb:8:3, Z_Angle:8:3, dsurb:8:3,
            dsurb0:8:3); // ,factst:8:3);
      end; // WHILE

      CloseFile(dw);
      CloseFile(sw);
      CloseFile(fw);
    end;
  except
  end;
end;

procedure UsredOnLittleV;
var
  j2: INTEGER;
  j: INTEGER;
  i: INTEGER;
  r_min: real;
begin
  for i := 0 to U_IND - 1 do
  begin
    j := 0;
    if (Speed_[i] < 25) then
    begin
      r_min := abs(Fsr_rh1[i]);
      while ((Speed_[i + j] < 25) and (j + i < U_IND)) do
      begin
        j := j + 1;
        if (abs(Fsr_rh1[i + j]) < r_min) then
          r_min := abs(Fsr_rh1[i + j]);
      end;
      for j2 := i to i + j do
      begin
        if (Fsr_rh1[j2] < 0) then
          Fsr_rh1[j2] := r_min * -1
        else
          Fsr_rh1[j2] := r_min;
        if (Fsr_Rh2[j2] < 0) then
          Fsr_Rh2[j2] := r_min * -1
        else
          Fsr_Rh2[j2] := r_min;
      end;
    end;
  end;
end;
// ----------------------------------------------------------------------
// Корректируем рихтовки с съездов
// ----------------------------------------------------------------------

function CorrectSiezd(Z_Angle: real; fluk: INTEGER): INTEGER;
begin
  CorrectSiezd := round(fluk / (1 + 0.01 * abs(Z_Angle)));
end;
// ----------------------------------------------------------------------
// Получаем данных от БД PutemerDb_09 и записываем БД JBGJ
// ----------------------------------------------------------------------

procedure RWTB_PRK(Name_Km_SVGP: string);
var
  fw, sw: textfile;
  D_km, D_PIC, D_sm, xk, D_v, NU: INTEGER;
  D_rh1, D_rh2, D_sh, D_ur1, D_ur2, D_urb, x_Angle, y_angle, s, prom, factst,
    stright_avg, level_avg: real;
  D_mtr, jj, lng, ii, i5, prom1, curvePointLevel, curvePointStr, sign: INTEGER;
  x1, x2, y1, y2, yy, f1, summ, summ1, axc, ayc, azc: real;
  Z_Angle, dv, dsurb, dsurb0, drih0, kif1, kif2, kif3, kif4, kif5,
    delta_zangle: real;
  fbool: boolean;
  _D_urb, _dsurb, _dsurb0, skip, side_wear_left, side_wear_right: real;

  kkkkk, ijk, iik, iost, jost: INTEGER;
  avgOfUrb: real;

label
  1;

begin
  try
    // if FileExists(Path_km_shifrovka_file + 'km_'+IntToStr(IPKM)+'.svgpdat') then begin
    if Flag_sablog then
      sablog('RWTB_PRK - Чтение определенного километра из БД');
    // ------------------------------------------------------------------------------

    GlbCou_3stShab_UklOtb := 0;
    LENGTH_MASFUNCS;
    U_IND := 0;
  1:
    flagpic := false;
    prom := 0;
    prom1 := 0;
    ppp := 0;
    ID_global := 0;

    // promstr := copy(PARITY_F, 0, 1);

    k := 0;

    Vstavka_flag := false;
    if (GlbKmTrue <> 0) and (abs(GlbKmTrue - D_rkm) > 1) then
    begin
      { Vstavka_Length:= 0;
        if not ZhokKm(GlbKmTrue) then prvKmLen:= 0;
        if prvKmLen > 0 then Vstavka_Length:= prvKmLen; }
      if not ZhokKm(GlbKmTrue) then
        Vstavka_flag := true;
    end;

    numput := GlbTrackId;
    Glb_PutList_Put := numput;
    // if IPKM > 0 then

    ijk := 1;
    delta_zangle := 0;

    while not eof(km_shifrovka_file) do
    begin

      // 0     1     2     3     4      5     6      7      8       9       10     11      12       13      14   15    16
      Readln(km_shifrovka_file, indi, dv, D_PIC, D_mtr, D_ur2, D_ur1, D_sh,
        D_rh1, D_rh2, _D_urb, Z_Angle, _dsurb, _dsurb0, skip, skip, skip, skip,
        // 17             18            19   20   21    22             23           24             25
        side_wear_right, side_wear_left, skip, skip, skip, stright_avg,
        level_avg, curvePointLevel, curvePointStr, sign);

      // D_sh - шаблон  Gauge
      // 9 _D_urb - измеренный уровень
      // 8 D_rh2 - измеренная рихт

      // 10 - Z_Angle - трапезойд для рихтовки
      // 11 _dsurb - трапезойд для уровня
      // _dsurb0 - средний уровень

      // ,factst);//,factst,factst,factst,factst,factst,factst);
      setlength(SideWearLeft, U_IND + 1);
      setlength(SideWearRight, U_IND + 1);

      setlength(CurvePointsLevel, U_IND + 1);
      setlength(CurvePointsStr, U_IND + 1);

      setlength(TrapezLevel, U_IND + 1);
      setlength(TrapezStr, U_IND + 1);

      setlength(AvgTr, U_IND + 1);
      setlength(TrapezLevel_Get_per, U_IND + 1);
      setlength(TrpzStr, U_IND + 1);

      setlength(ST_AVG, U_IND + 1);
      setlength(LV_AVG, U_IND + 1);

      setlength(ST_NAT, U_IND + 1);
      setlength(LV_NAT, U_IND + 1);

      setlength(ST_N, U_IND + 1);
      setlength(LV_N, U_IND + 1);

      SideWearRight[U_IND] := side_wear_right;
      SideWearLeft[U_IND] := side_wear_left;

      CurvePointsLevel[U_IND] := curvePointLevel;
      CurvePointsStr[U_IND] := curvePointStr;
      TrapezLevel_Get_per[U_IND] := round(_dsurb);
      TrapezLevel[U_IND] := _dsurb;
      TrapezStr[U_IND] := round(Z_Angle);
      AvgTr[U_IND] := round(_dsurb0);
      TrpzStr[U_IND] := Z_Angle;

      // ST_AVG[U_IND] := tanba_rih * stright_avg;
      // LV_AVG[U_IND] := tanba_rih * level_avg;
      // D_rh1 := tanba_rih *  D_rh1;
      // D_rh2 := tanba_rih *  D_rh2;
      // Z_Angle := tanba_rih * Z_Angle;
      // D_urb := tanba_rih * _D_urb;
      // dsurb := tanba_rih * _dsurb;

      ST_AVG[U_IND] := stright_avg;
      LV_AVG[U_IND] := level_avg;

      ST_NAT[U_IND] := D_rh2;
      LV_NAT[U_IND] := _D_urb;

      ST_N[U_IND] := D_rh2;
      LV_N[U_IND] := _D_urb;

      D_rh1 := D_rh1;
      D_rh2 := D_rh2;
      Z_Angle := Z_Angle;
      D_urb := _D_urb;
      dsurb := _dsurb;

      //
      dsurb0 := _dsurb0; // *1.027;

      if napr_dbij = -1 then
      begin
        D_ur2 := D_ur2 * napr_dbij;
        D_ur1 := D_ur1 * napr_dbij;
        D_rh2 := D_rh1 * napr_dbij;
        D_rh1 := D_rh2 * napr_dbij;
        D_urb := D_urb * napr_dbij;
        Z_Angle := Z_Angle * napr_dbij;
        ST_AVG[U_IND] := napr_dbij * ST_AVG[U_IND];
        LV_AVG[U_IND] := napr_dbij * LV_AVG[U_IND];
        dsurb := dsurb * napr_dbij;
        dsurb0 := dsurb0 * napr_dbij;
      end;

      D_v := 80;
      D_mtr := (D_PIC - 1) * 100 + D_mtr;
      Fn0[U_IND].km := GlbKmTrue;
      Fn0[U_IND].m := D_mtr;

      // if (U_IND=0) then
      // rixtovoshnayanit(D_Mtr);
      kif1 := 0;
      kif2 := 0;
      kif3 := 0;

      Fn0[U_IND].m_ := D_mtr;
      if (U_IND mod 20 = 0) then
        delta_zangle := Z_Angle;
      Rih_Nit[U_IND].x := D_mtr;
      // Rih_Nit[U_IND].fun := riht_nit;
      Rih_Nit[U_IND].fun := sign;

      f_facstrel[U_IND] := 0; // factst;
      F_rkm[U_IND] := D_rkm; //
      F_pik[U_IND] := D_PIC - prom1;
      F_Mtr[U_IND] := D_mtr; //

      FncX[U_IND] := xk div 100; //
      F_V[U_IND] := D_v;
      F_Vg[U_IND] := D_v;
      F_Vrp[U_IND] := -1;
      F_Vrg[U_IND] := -1;
      //
      Speed_[U_IND] := dv; // скорость

      if (1548 <= D_sh) and (D_sh < 1549) then
        D_sh := 1548; // 11.03.2012

      F_sh11[U_IND] := D_sh;
      F_sh[U_IND] := D_sh;

      F_Wear[U_IND] := 0;
      F_Pr2[U_IND] := round(kfPro * D_ur2); //
      F_Pr1[U_IND] := round(kfPro * D_ur1); //

      F_rh1[U_IND] := round(D_rh1 * kfRih); //
      F_Rh2[U_IND] := round(D_rh2 * kfRih); //

      FPro2[U_IND] := kfPro * D_ur2;
      FPro1[U_IND] := kfPro * D_ur1;

      Frih1[U_IND] := Z_Angle + kf_rih * (D_rh1 - Z_Angle); // 2*
      Frih2[U_IND] := Z_Angle + kf_rih * (D_rh2 - Z_Angle); // 2*

      F_fluk[U_IND] := round(kf_rih * 2 * (D_rh2 - Z_Angle)); // 1.35 //1.125

      Fluk_right[U_IND] := round(kf_rih * 2 * (D_rh1 - Z_Angle));
      Fluk_left[U_IND] := round(kf_rih * 2 * (D_rh2 - Z_Angle));
      // AvgTr[U_IND] := round(_dsurb0* (exp(-10*abs( _dsurb   )) + dsurb )    );
      if (riht_nit > 0) then
      begin
        F_fluk[U_IND] := round(kf_rih * 2 * (D_rh1 - Z_Angle)); // 1.35 //1.125
      end;

      // if tanba_rih < 0 then
      // begin
      // Frih2[U_IND] := Z_Angle + kf (D_rh1 - Z_Angle); // 2*
      // Frih1[U_IND] := Z_Angle + tanba_rih * (D_rh2 - Z_Angle); // 2*
      // end;

      if GlbMaxRih < D_rh1 then
        GlbMaxRih := D_rh1;
      furb[U_IND] := D_urb;

      Fsr_rh1[U_IND] := Z_Angle; // drih0;
      Fsr_Rh2[U_IND] := Z_Angle; // drih0;

      F_NORM[U_IND] := 1520;
      F_PUCH[U_IND] := 0;
      /// /////////////////////////////////////////////////////////////////////////////????
      S_NORM[U_IND] := 1520;

      F0_sh[U_IND] := 1520;

      Fsr_Urb[U_IND] := dsurb; // round(dsurb0);
      Furb1[U_IND] := round(D_urb); // 1.09*
      Furbx[U_IND] := round(D_urb);
      Furb_sr[U_IND] := round(dsurb0); //
      if (abs(dsurb) < 1) then

        F_urb_Per[U_IND] := round(1.10 * (D_urb - dsurb0))
      else
        F_urb_Per[U_IND] := round(1.10 * (D_urb - dsurb));
      if (abs(dsurb) < 1) then

        AvgTr[U_IND] := round(1.0 * dsurb0)
      else
        AvgTr[U_IND] := round(1.0 * dsurb);

      F_urb_Per_sr[U_IND] := round(_dsurb);
      // round(D_urb - dsurb); * 1.09

      F0_urov[U_IND] := 0;
      F0_rih1[U_IND] := 0;
      F0_rih2[U_IND] := 0;

      F_rad[U_IND] := 50000;
      Shpaly[U_IND] := 2;

      if U_IND = 0 then
        km_first_mtr := D_mtr
      else
        km_last_mtr := D_mtr;

      U_IND := U_IND + 1;
      Application.ProcessMessages;
      if StopFlag then
        break;

    end; // WHILE

    // -------------------------------------------------------------------------
    U_LNG := U_IND;
    prvKmLen := U_LNG;
    lng := k;
    CVSredKm := round(dv); // ckorost b km
    CloseFile(km_shifrovka_file);

    GlbTempTipRemontPiket := '';
    GlbTempTipRemontKm := '';
    ball500_sebep := '';
    GlbOgrPasGrz := '-/-';
    glb_vop := 999;
    glb_vog := 999;

    GlbMinOgrSk := 140; // GlobPassSkorost;
    GlbMinOgrSkGrz := 90; // GlobGruzSkorost;
    GlbMinOgrSk_ := 140; // GlobPassSkorost;

    GlbSkorostRemontKm := GlobPassSkorost;
    RGlobPassSkorost := GlobPassSkorost;
    RGlobGruzSkorost := GlobPassSkorost;

    GlbCountRemKm := 0;
    GlbCountUKL := 0;
    GlbCountUSK := 0;
    GlbCountSOCH := 0;
    GlbCountDGR := 0;

    glbCount_Rih4s := 0;
    glbCount_Ush4s := 0;
    glbCount_suj4s := 0;
    glbCount_Urv4s := 0;
    glbCount_Per4s := 0;
    glbCount_1Pro4s := 0;
    glbCount_2Pro4s := 0;

    cnt_ush4 := 0;
    cnt_suj4 := 0;
    cnt_urv4 := 0;
    cnt_per4 := 0;
    cnt_pr1 := 0;
    cnt_pr2 := 0;
    cnt_rih := 0;

    GlobUbedOgr := '';
    UbedOgr := '';
    UbedOgr4jok := '';
    UbedOgr4jok_V := 120;
    GlbMinOgrSk4 := 120;

    Glob_primech := '';
    Glb3 := '';
    GlobLentaS := '';
    GlbCommentPaspData := ' ';
    GlbFlagOgrSkorosti := false;
    GlbFlagCorrKm := false;
    glbTipOgrV := 0;

    POP_flg := false; // ПОП флагы
    END_km := false; // соңғы км флагы

    p_ush4 := '';
    p_suj4 := '';
    p_urv4 := '';
    p_per4 := '';
    p_pr1 := '';
    p_pr2 := '';
    p_rih := '';
    str_4st := '';

    Shab_s2 := 0;
    Suj_s2 := 0;
    ush_s2 := 0;
    Shab_s3 := 0;
    Suj_s3 := 0;
    ush_s3 := 0;
    Shab_s4 := 0;
    Suj_s4 := 0;
    ush_s4 := 0;

    rih_s2 := 0;
    rih1_s2 := 0;
    rih2_s2 := 0;
    rih_s3 := 0;
    rih1_s3 := 0;
    rih2_s3 := 0;
    rih_s4 := 0;
    rih1_s4 := 0;
    rih2_s4 := 0;

    pro_s2 := 0;
    pro1_s2 := 0;
    pro2_s2 := 0;
    pro_s3 := 0;
    pro1_s3 := 0;
    pro2_s3 := 0;
    pro_s4 := 0;
    pro1_s4 := 0;
    pro2_s4 := 0;

    urb_s2 := 0;
    pot_s2 := 0;
    per_s2 := 0;
    urb_s3 := 0;
    pot_s3 := 0;
    per_s3 := 0;
    urb_s4 := 0;
    pot_s4 := 0;
    per_s4 := 0;

    GlbFlagRemontKm := false;

    if (U_IND > 30) and not StopFlag then // меньше 30 точек км не обрабат-ся
    begin

      IndGlbInfOts := 0;
      setlength(GlbInfOts, 3500); // 02.10.2012

      LenMasEnd;

      avgOfUrb := 0;
      for kkkkk := 0 to length(Furb1) - 1 do
      begin
        avgOfUrb := avgOfUrb + Furb1[k];
      end;
      avgOfUrb := avgOfUrb / (length(Furb1) - 1);

      if (avgOfUrb < 0) then
        tanba_urb := -1;

      GetRemMas; // 20.05 2012 //ToDo Read FromBase
      GSpeedKM;

      // GlbFlagRemontKm := GetRemKorKm(GlbKmTrue);
      GetCorrKm(GlbKmTrue);
      // ortaman; //30.10.2011
      // shablonsuzgi;
      FormirPrPuch;

      FormirNulevoiShablon1;

      // if not NKm_(GlbKmTrue) then
      Get_NullFunc;
      // else
      // Get_NullFunc_ttemp; // 22.06.2013

      // Get_Null_2Kriv;
      OprFactStrelki1;
      DopUklBozb;
      // DopUklBozb_Shablona;
      Nesob_nash_naras_voz_s_rih;
      VPiket;

      Tegisteu_F0_sh;

      Length_Km := U_LNG / 1000;

      RWTB_INFOTS(GlbKmTrue);

      KM2File(FileName);
    end; // if

  except
  end;

end;
// ----------------------------------------------------------------------

function interpol(x1, x2, y1, y2, x: real): real;
begin
  interpol := y1 + x * (y2 - y1) / (x2 - x1) - x1 * (y2 - y1) / (x2 - x1);
end;

// -------------------------------------------------------------------
end.
