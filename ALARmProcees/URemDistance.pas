unit URemDistance;

interface

uses
  SysUtils, Params;

type
  TRemDistance = record
    nkm, npiket, kkm, kpiket, speed: integer;
    nput, caption: ShortString;
    start, finish: TDate;

  end;

  TRemDistances = array of TRemDistance;
function WriteToFile(rem_distances: TRemDistances): boolean;
function ReadFromFile(): TRemDistances;
procedure GenerateRemfileForOldVersion(rem_distances: TRemDistances);

implementation

function WriteToFile(rem_distances: TRemDistances): boolean;
var
  f: file of TRemDistance;
  i: integer;
begin
  result := false;
  AssignFile(f, 'remfile.dat');
  Rewrite(f);
  try
    for i := 0 to high(rem_distances) do
      Write(f, rem_distances[i]);
  finally
    CloseFile(f);
    result := true;
  end;
end;

function ReadFromFile(): TRemDistances;
var
  f: file of TRemDistance;
begin
  AssignFile(f, 'remfile.dat');
  Reset(f);
  setlength(result, 0);
  try
    while not Eof(f) do
    begin
      setlength(result, length(result) + 1);
      Read(f, result[length(result) - 1]);
    end;
  finally
    CloseFile(f);
  end;
end;

function GetLastPiket(rem_direction: TRemDistance; nestkms: UNestKm): integer;
var
  nest_index: integer;
begin
  result := 10;
  for nest_index := 0 to high(nestkms) - 1 do
    if (nestkms[nest_index].km = rem_direction.nkm) and
      (nestkms[nest_index].put = rem_direction.nput) then
    begin
      result := nestkms[nest_index].dlina div 100 + 1;
      exit;
    end;

end;

procedure GenerateRemfileForOldVersion(rem_distances: TRemDistances);
var
  remfile, nestfile: textfile;
  direct_name: String;
  rem_index, nkm, npik, kkm, kpik, kpik2, nest_index: integer;
  nestkms: UNestKm;
begin
  if FileExists('PutList.txt') then
  begin
    AssignFile(nestfile, 'PutList.txt');
    Reset(nestfile);
    readln(nestfile);
    readln(nestfile);
    readln(nestfile, direct_name);
    close(nestfile);
  end;
  if FileExists(direct_name + '_nst.txt') then
  begin
    AssignFile(nestfile, direct_name + '_nst.txt');
    Reset(nestfile);
    nest_index := 0;
    while not(Eof(nestfile)) do
    begin
      setlength(nestkms, nest_index + 1);
      readln(nestfile, nestkms[nest_index].pch, nestkms[nest_index].km,
        nestkms[nest_index].dlina);
      readln(nestfile, nestkms[nest_index].put);
      nestkms[nest_index].GlbNaim := gnaim;
      nest_index := nest_index + 1;
    end;
    CloseFile(nestfile);

  end;

  AssignFile(remfile, 'RFile.txt');
  Rewrite(remfile);
  for rem_index := 0 to high(rem_distances) do
  begin
    nkm := rem_distances[rem_index].nkm;
    kkm := rem_distances[rem_index].kkm;
    npik := rem_distances[rem_index].npiket;
    kpik := rem_distances[rem_index].kpiket;
    while nkm <= kkm do
    begin
      if (nkm = kkm) then
        kpik2 := kpik
      else
        kpik2 := GetLastPiket(rem_distances[rem_index], nestkms);
      while (npik <= kpik2) do
      begin
        writeln(remfile, DateToStr(rem_distances[rem_index].start));
        writeln(remfile, DateToStr(rem_distances[rem_index].finish));
        writeln(remfile, rem_distances[rem_index].nput);
        writeln(remfile, nkm);
        writeln(remfile, npik);
        writeln(remfile, rem_distances[rem_index].speed);
        writeln(remfile, rem_distances[rem_index].caption);
        npik := npik + 1;
      end;
      npik := 1;
      nkm := nkm + 1;
    end;

  end;
  close(remfile);
end;

end.
