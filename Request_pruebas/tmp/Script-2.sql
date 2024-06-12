
--periodos
select EM_K_GEN_WS.GETLOV(1,112,'1','A1001402',10,'[cod_cia = 1][fec_efec_poliza = 21/02/2024][cod_ramo = 112][dvcod_modalidad = 11204]') from dual;

--datos de cotizacion
DECLARE
cur_coberturas sys_refcursor;
cur_primas sys_refcursor;
cur_recibos sys_refcursor;
num_pagos int;

coverageId varchar(100);
sumInsuredAmn varchar(100);
coverageDesc varchar(100);
deductible varchar(100);
primeAmn varchar(100);

total varchar(100);
prime varchar(100);
rec varchar(100);
der varchar(100);
imp varchar(100);

BEGIN
  tron2000.em_k_gen_ws_mmx.p_regresa_cotizacion(p_num_control => '2411200001259',
                                                p_cur_coberturas => cur_coberturas,
                                                p_cur_primas => cur_primas,
                                                p_cur_recibos => cur_recibos,
                                                p_num_pagos => num_pagos);
                                               
  LOOP
    FETCH cur_coberturas INTO coverageId, sumInsuredAmn, coverageDesc, deductible, primeAmn;
    EXIT when cur_coberturas%notfound;
    dbms_output.put_line('CoverageId: ' || coverageId || ' | sumInsuredAmn: ' || sumInsuredAmn || ' | coverageDesc: ' || coverageDesc || ' | deductible: ' || deductible || ' | primeAmn: ' || primeAmn );
    
  END LOOP;
 
  LOOP
    FETCH cur_primas INTO total, prime, rec, der, imp;
    EXIT when cur_primas%notfound;
    dbms_output.put_line('total: ' || total || ' | prime: ' || prime || ' | rec: ' || rec || ' | der: ' || der || ' | imp: ' || imp );
    
  END LOOP;
  
END;



-- fondos
select EM_K_GEN_WS.GETLOV(1,112,'','TA899039',3,'[cod_cia = 1][cod_ramo = 112][dvcod_modalidad = 11204][num_contrato = 11228][dvtip_perfil_inv  = ''CON''][cod_mon = 1]') from dual;

select EM_K_GEN_WS.GETLOV(1,112,'','TA899039',3,'[cod_cia = 1][cod_ramo = 112][dvcod_modalidad = 11204][num_contrato = 99999][dvtip_perfil_inv  = ''CON''][cod_mon = 1]') from dual;

--distribucion de fondos para cotizacion
select ev_k_112_unit.get_distribucion_fondos('2411200001329')
from dual;

select ev_k_112_unit.get_distribucion_fondos('2411200001329')
from dual;


--TRON2000.EV_K_112_UNIT_MMX

select cod_fundo    as codigo_fondo ,
             pct_fundo    as porcentaje_dist,
             capital_liq  as distribucion_inicial
        from p2309119_mmx --a2309119_mpt  --> APG SE CAMBIA LA TABLA POR LA VISTA 26/03/2018 v. 1.10
       where num_poliza = '2411200001329';
      

select * FROM tron2000.p2000025 p WHERE p.NUM_POLIZA  = '2411200001285';

--coberturas

select EM_K_GEN_WS.GETLOV(1,112,'',
			'A1002150',30,
		'[cod_cia = 1][cod_ramo = 112][cod_modalidad = 11204][num_edad = 37][num_contrato = 11228]') from dual;


	
	tron2000.EV_K_DEFINE_CONTRATOS_MMX
	
--ev_k_define_contratos.p_devuelve_coberturas
DECLARE
p_coberturas sys_refcursor;
begin
  -- Call the procedure
  tron2000.EV_K_DEFINE_CONTRATOS_MMX.p_devuelve_coberturas(p_cod_cia => 1,
                                                           p_cod_ramo => 112,
                                                           p_num_contrato => 11228,
                                                           p_cod_mon => 1,
                                                           p_fec_validez => TO_DATE('22/03/2024','dd/MM/YYYY'),
                                                           p_coberturas => p_coberturas);
end;

 SELECT a.cod_cob
          ,b.nom_cob
          ,a.mca_obligatoria
          ,a.cod_cob_dep
          ,a.suma_aseg_desde
          ,a.suma_aseg_hasta
      FROM a2300419_mmx a
      JOIN A1002050     b
        ON b.cod_cia      = a.cod_cia
       AND b.cod_cob      = a.cod_cob
     WHERE a.cod_cia      = 1
       AND a.cod_ramo     = 112
       AND a.num_contrato = 11228
       AND a.cod_mon      = 1
       AND  TO_DATE('22/03/2024','dd/MM/YYYY')  BETWEEN a.fec_desde AND a.fec_hasta
       AND a.mca_inh      = 'N'
     ORDER BY a.cod_cob ;

	
DECLARE
	datos_cob varchar(2000);
BEGIN
	tron2000.EV_K_COTIZA_VIDA_MMX.p_obtiene_datoscob(p_cod_cia => 1,
                                                   p_cod_ramo => 112,
                                                   p_cod_modalidad => 11204,
                                                   p_edad => 37,
                                                   p_datoscob => datos_cob);
	dbms_output.put_line('datos: ' || datos_cob);
END;



--primas
DECLARE
	cur_primas sys_refcursor;
	cobertura varchar(100);
	prima varchar(100);
BEGIN
	tron2000.ev_k_productos_vida_mmx.p_ul_ind_prima_riesgo(p_cod_cia => 1,
                                                         p_cod_ramo => 112,
                                                         p_cod_modalidad => 11204,
                                                         p_num_contrato => 11228,
                                                         p_cod_mon => 1,
                                                         p_sumas_aseg => '1000|30000|',
                                                         p_duracion => 1,
                                                         p_edad => 37,
                                                         p_fec_validez => TO_DATE('07/03/2024','dd/MM/YYYY'),
                                                         p_primas => cur_primas);
	LOOP
    	FETCH cur_primas INTO cobertura, prima;
    	EXIT when cur_primas%notfound;
    	dbms_output.put_line('cob: ' || cobertura || ' | prima: ' || prima );
    
 	 END LOOP;
	
END;



SELECT * FROM tron2000.P2000025 p WHERE p.NUM_POLIZA = '2411200001327';


SELECT * FROM tron2000.P2000025 p WHERE p.NUM_POLIZA = '2411200001332';

SELECT * FROM tron2000.P2000020 p WHERE p.NUM_POLIZA = '2411200001332';

TRON2000.EM_K_GEN_WS_MMX


ORA-06512: at "TRON2000.EM_K_GEN_WS_MMX", line 3344
ORA-06512: at "TRON2000.EM_K_GEN_WS_MMX", line 3794




