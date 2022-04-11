-- Table: public.s3

-- DROP TABLE public.s3;

CREATE TABLE public.s3_history
(
    id bigserial,
    original_id bigint,
    pch character varying(3) COLLATE pg_catalog."default",
    naprav character varying(30) COLLATE pg_catalog."default",
    put character varying(2) COLLATE pg_catalog."default",
    pchu integer DEFAULT 1,
    pd integer DEFAULT 1,
    pdb integer DEFAULT 1,
    km integer,
    meter integer,
    trip_id bigint,
    ots character varying(15) COLLATE pg_catalog."default",
    kol integer DEFAULT 0,
    otkl integer DEFAULT 0,
    len integer DEFAULT 1,
    primech character varying(149) COLLATE pg_catalog."default",
    tip_poezdki integer DEFAULT 0,
    cu integer DEFAULT 0,
    us integer DEFAULT 0,
    p1 integer DEFAULT 0,
    p2 integer DEFAULT 0,
    ur integer DEFAULT 0,
    pr integer DEFAULT 0,
    r1 integer DEFAULT 0,
    r2 integer DEFAULT 0,
    bas integer DEFAULT 0,
    typ integer DEFAULT 0,
    uv integer,
    uvg integer,
    ovp integer,
    ogp integer,
    is2to3 boolean DEFAULT false,
    track_id bigint,
    onswitch boolean DEFAULT false,
    isequalto4 boolean DEFAULT false,
    distance_id bigint,
    isequalto3 boolean,
    state_id integer,
    editor character varying(100) COLLATE pg_catalog."default", -- кто редактировал
    comment character varying(500) COLLATE pg_catalog."default", -- причина редактирования
    modified_date timestamp without time zone default current_timestamp, -- дата редактирования
    CONSTRAINT s3_history_pkey PRIMARY KEY (id),
    CONSTRAINT s3_history_trip_id_fkey FOREIGN KEY (trip_id)
        REFERENCES public.trips (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)

TABLESPACE pg_default;

ALTER TABLE public.s3_history
    OWNER to postgres;
-- Index: fki_s3_trip

-- DROP INDEX public.fki_s3_trip;

CREATE INDEX fki_s3_history_trip
    ON public.s3_history USING btree
    (trip_id ASC NULLS LAST)
    TABLESPACE pg_default;

-- Жөндеу жұмыстары бойынша өзгерістер
alter table repair_project add column if not exists accepted_at timestamp without time zone default null;
alter table repair_project add column if not exists speed int;

DROP FUNCTION IF EXISTS public.insertrepairproject(trackid int8, startkm int4, startm int4, finalkm int4, finalm int4, acceptid int4, typeid int4, repairdate timestamp);
CREATE OR REPLACE FUNCTION public.insertrepairproject(trackid int8, startkm int4, startm int4, finalkm int4, finalm int4, acceptid int4, typeid int4,speed int4, repairdate timestamp)
  RETURNS pg_catalog.int8 AS $BODY$
DECLARE
   v_id bigint;
BEGIN
   INSERT INTO public.repair_project(
	adm_track_id, type_id, start_km, start_m, final_km, final_m, accept_id, speed, repair_date)
	SELECT trackid, typeid, startkm, startm, finalkm, finalm, acceptid, speed, repairdate
   RETURNING id INTO v_id;
   RETURN coalesce(v_id,-1);
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;

CREATE TABLE public.bedemost_history
(
    id bigserial,
	original_id bigint,
    km integer,
    ots_iv_st character varying(10),
    ball integer DEFAULT 10,
    primech character varying(301),
    CONSTRAINT bedemost_history_pkey PRIMARY KEY (id)
);

-- Ведемостьқа 1 деңгейлі қателіктер үшін бағандар қосу


--alter table bedemost modify primech varchar(500) charset "utf-8";
alter table bedemost add column if not exists fdbroad int4 default 0;
alter table bedemost add column if not exists fdconstrict int4 default 0;
alter table bedemost add column if not exists fdskew int4 default 0;
alter table bedemost add column if not exists fddrawdown int4 default 0;
alter table bedemost add column if not exists fdstright int4 default 0;
alter table bedemost add column if not exists fdlevel int4 default 0;
alter table s3 add column if not exists islong bool default false;
alter table s3 add column if not exists isadditional int4 default 0;
alter table s3 add column if not exists fileid int4 default 0;
alter table s3 add column if not exists ms int4 default 0;
alter table s3 add column if not exists fnum int4 default 0;
alter table s3 add column if not exists reptype int4 default 0;
alter table s3 add column if not exists carposition int4 default 0;
