object MainDataModule: TMainDataModule
  OldCreateOrder = False
  Height = 322
  Width = 572
  object sqlGetCurveCoords: TFDQuery
    Connection = pgsConnection
    SQL.Strings = (
      'INSERT INTO public.rd_curve('
      
        #9'trip_id,  km,  m,   radius, level,  gauge, passboost, freightbo' +
        'ost, passboost_anp, freightboost_anp, passspeed,  freightspeed, ' +
        'curve_id, wear, broadening,track_id,  point_level,  point_str, t' +
        'rapez_level, trapez_str, avg_level, avg_str)'
      
        'VALUES (:trip_id, :km, :m, :radius,:level,:gauge, :passboost,:fr' +
        'eightboost,:passboost_anp,:freightboost_anp,:passspeed, :freight' +
        'speed,:curve_id,:wear, :broad,     :track_id, :point_level, :poi' +
        'nt_str, :trapez_level, :trapez_str,:avg_level, :avg_str);')
    Left = 224
    Top = 120
    ParamData = <
      item
        Name = 'TRIP_ID'
        ParamType = ptInput
      end
      item
        Name = 'KM'
        ParamType = ptInput
      end
      item
        Name = 'M'
        ParamType = ptInput
      end
      item
        Name = 'RADIUS'
        ParamType = ptInput
      end
      item
        Name = 'LEVEL'
        ParamType = ptInput
      end
      item
        Name = 'GAUGE'
        ParamType = ptInput
      end
      item
        Name = 'PASSBOOST'
        ParamType = ptInput
      end
      item
        Name = 'FREIGHTBOOST'
        ParamType = ptInput
      end
      item
        Name = 'PASSBOOST_ANP'
        ParamType = ptInput
      end
      item
        Name = 'FREIGHTBOOST_ANP'
        ParamType = ptInput
      end
      item
        Name = 'PASSSPEED'
        ParamType = ptInput
      end
      item
        Name = 'FREIGHTSPEED'
        ParamType = ptInput
      end
      item
        Name = 'CURVE_ID'
        ParamType = ptInput
      end
      item
        Name = 'WEAR'
        ParamType = ptInput
      end
      item
        Name = 'BROAD'
        ParamType = ptInput
      end
      item
        Name = 'TRACK_ID'
        ParamType = ptInput
      end
      item
        Name = 'POINT_LEVEL'
        ParamType = ptInput
      end
      item
        Name = 'POINT_STR'
        ParamType = ptInput
      end
      item
        Name = 'TRAPEZ_LEVEL'
        ParamType = ptInput
      end
      item
        Name = 'TRAPEZ_STR'
        ParamType = ptInput
      end
      item
        Name = 'AVG_LEVEL'
        ParamType = ptInput
      end
      item
        Name = 'AVG_STR'
        ParamType = ptInput
      end>
  end
  object FDPhysPgDriverLink1: TFDPhysPgDriverLink
    VendorLib = 'C:\sntfi\ALARm5\ALARmProcees\pgbin32\bin\libpq.dll'
    Left = 351
    Top = 112
  end
  object spAddUbemDat: TFDStoredProc
    Connection = pgsConnection
    StoredProcName = 'inserts3'
    Left = 256
    Top = 16
    ParamData = <
      item
        Position = 1
        Name = 'pch'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 2
        Name = 'distance_id'
        DataType = ftLargeint
        FDDataType = dtInt64
        ParamType = ptInput
      end
      item
        Position = 3
        Name = 'naprav'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 4
        Name = 'put'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 5
        Name = 'track_id'
        DataType = ftLargeint
        FDDataType = dtInt64
        ParamType = ptInput
      end
      item
        Position = 6
        Name = 'pchu'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 7
        Name = 'pd'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 8
        Name = 'pdb'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 9
        Name = 'km'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 10
        Name = 'meter'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 11
        Name = 'trip_id'
        DataType = ftLargeint
        FDDataType = dtInt64
        ParamType = ptInput
      end
      item
        Position = 12
        Name = 'ots'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 13
        Name = 'kol'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 14
        Name = 'otkl'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 15
        Name = 'len'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 16
        Name = 'primech'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 17
        Name = 'tip_poezdki'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 18
        Name = 'cu'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 19
        Name = 'us'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 20
        Name = 'p1'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 21
        Name = 'p2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 22
        Name = 'ur'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 23
        Name = 'pr'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 24
        Name = 'r1'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 25
        Name = 'r2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 26
        Name = 'bas'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 27
        Name = 'typ'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 28
        Name = 'uv'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 29
        Name = 'uvg'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 30
        Name = 'ovp'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 31
        Name = 'ogp'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 32
        Name = 'is2to3'
        DataType = ftBoolean
        FDDataType = dtBoolean
        ParamType = ptInput
      end
      item
        Position = 33
        Name = 'onswitch'
        DataType = ftBoolean
        FDDataType = dtBoolean
        ParamType = ptInput
      end
      item
        Position = 34
        Name = 'isequalto4'
        DataType = ftBoolean
        FDDataType = dtBoolean
        ParamType = ptInput
      end
      item
        Position = 35
        Name = 'isequalto3'
        DataType = ftBoolean
        FDDataType = dtBoolean
        ParamType = ptInput
      end
      item
        Position = 36
        Name = 'islong'
        DataType = ftBoolean
        FDDataType = dtBoolean
        ParamType = ptInput
      end
      item
        Position = 37
        Name = 'result'
        DataType = ftLargeint
        FDDataType = dtInt64
        ParamType = ptResult
      end>
    object spAddUbemDatresult: TLargeintField
      AutoGenerateValue = arDefault
      FieldName = 'result'
      Origin = 'result'
      ProviderFlags = []
      ReadOnly = True
    end
  end
  object spBedemost: TFDStoredProc
    Connection = pgsConnection
    StoredProcName = 'insertbedemost'
    Left = 72
    Top = 16
    ParamData = <
      item
        Position = 1
        Name = 'pch'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 2
        Name = 'naprav'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 3
        Name = 'put'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 4
        Name = 'track_id'
        DataType = ftLargeint
        FDDataType = dtInt64
        ParamType = ptInput
      end
      item
        Position = 5
        Name = 'pchu'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 6
        Name = 'pd'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 7
        Name = 'pdb'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 8
        Name = 'km'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 9
        Name = 'kmtrue'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 10
        Name = 'suj_2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 11
        Name = 'suj_3'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 12
        Name = 'suj_4'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 13
        Name = 'ush_2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 14
        Name = 'ush_3'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 15
        Name = 'ush_4'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 16
        Name = 'pro_p2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 17
        Name = 'pro_p3'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 18
        Name = 'pro_p4'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 19
        Name = 'pro_l2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 20
        Name = 'pro_l3'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 21
        Name = 'pro_l4'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 22
        Name = 'per_2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 23
        Name = 'per_3'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 24
        Name = 'per_4'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 25
        Name = 'urb_2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 26
        Name = 'urb_3'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 27
        Name = 'urb_4'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 28
        Name = 'rih_p2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 29
        Name = 'rih_p3'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 30
        Name = 'rih_p4'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 31
        Name = 'rih_l2'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 32
        Name = 'rih_l3'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 33
        Name = 'rih_l4'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 34
        Name = 'ots_iv_st'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 35
        Name = 'ball'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 36
        Name = 'primech'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 37
        Name = 'tip_poezdki'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 38
        Name = 'trip_id'
        DataType = ftLargeint
        FDDataType = dtInt64
        ParamType = ptInput
      end
      item
        Position = 39
        Name = 'fio_pd'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 40
        Name = 'pch_name'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 41
        Name = 'rem_kor'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 42
        Name = 'korr_pasdat'
        DataType = ftWideString
        FDDataType = dtWideString
        ParamType = ptInput
      end
      item
        Position = 43
        Name = 'f_rem'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 44
        Name = 'f_ukl'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 45
        Name = 'f_usk'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 46
        Name = 'f_soch'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 47
        Name = 'f_drg'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 48
        Name = 'f_sum'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 49
        Name = 'lkm'
        DataType = ftFloat
        FDDataType = dtDouble
        ParamType = ptInput
      end
      item
        Position = 50
        Name = 'fdbroad'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 51
        Name = 'fdconstrict'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 52
        Name = 'fdskew'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 53
        Name = 'fddrawdown'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 54
        Name = 'fdstright'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 55
        Name = 'fdlevel'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 56
        Name = 'result'
        DataType = ftLargeint
        FDDataType = dtInt64
        ParamType = ptResult
      end>
  end
  object spMainPP: TFDStoredProc
    Connection = pgsConnection
    StoredProcName = 'insertprocess'
    Left = 144
    Top = 8
    ParamData = <
      item
        Position = 1
        Name = 'date_vrem'
        DataType = ftTimeStamp
        FDDataType = dtDateTimeStamp
        NumericScale = 1000
        ParamType = ptInput
      end
      item
        Position = 2
        Name = 'process_type'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptInput
      end
      item
        Position = 3
        Name = 'process_id'
        DataType = ftInteger
        FDDataType = dtInt32
        ParamType = ptOutput
      end>
  end
  object FDTransaction: TFDTransaction
    Connection = pgsConnection
    Left = 400
    Top = 16
  end
  object pgsConnection: TFDConnection
    Params.Strings = (
      'User_Name=postgres'
      'Database=Aniyar_COpy'
      'Password=alhafizu'
      'Server=DESKTOP-EMAFC5J'
      'ExtendedMetadata=True'
      'CharacterSet=UTF8'
      'DriverID=PG')
    LoginPrompt = False
    Transaction = FDTransaction
    Left = 64
    Top = 72
  end
  object fdReadPasport: TFDQuery
    Connection = pgsConnection
    Left = 72
    Top = 184
  end
  object FDReadPasportInner: TFDQuery
    Connection = pgsConnection
    Left = 72
    Top = 232
  end
end
