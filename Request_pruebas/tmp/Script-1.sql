
EM_K_GEN_WS

--datos poliza
SELECT * FROM TRON2000.P2000030;

--asegurado
SELECT * FROM TRON2000.P2000031;

--variable coverage
SELECT * FROM TRON2000.P2000025;

--variable
SELECT * FROM TRON2000.P2000020;

--solicitante beneficiario
SELECT * FROM TRON2000.P2000060;

--cobertura
SELECT * FROM TRON2000.P2000040;

SELECT * FROM TRON2000.W2990708_MMX;

SELECT * FROM TRON2000.W2990709_MMX;

SELECT * FROM TRON2000.P1001331;
--------------------------


--datos poliza
SELECT * FROM TRON2000.P2000030
WHERE NUM_POLIZA = '2411200003607';

--asegurado
SELECT * FROM TRON2000.P2000031
WHERE NUM_POLIZA = '2411200003611';

--variable coverage
SELECT * FROM TRON2000.P2000025
WHERE NUM_POLIZA = '2411200003607';
--variable
SELECT * FROM TRON2000.P2000020
WHERE NUM_POLIZA = '2411200003607';
--solicitante beneficiario
SELECT * FROM TRON2000.P2000060
WHERE NUM_POLIZA = '2411200003611';
--cobertura
SELECT * FROM TRON2000.P2000040
WHERE NUM_POLIZA = '2411200003607';

SELECT * FROM TRON2000.W2990708_MMX;

SELECT * FROM TRON2000.W2990709_MMX;

SELECT * FROM TRON2000.P1001331;

--Tercero.setCotizacionEmision

Con xml generado en emision se guardan tablas
y si regresa numero de cotizacion se llama al siguiente procedimiento para regresar datos para la impresion
 EM_K_GEN_WS.P_REGRESA_COTIZACION
 
 --Helper.EmisionVidaFM setEmision
 
 --Control tecnico
 EM_K_GEN_WS
 P_LANZA_PROCESO2_XML
 
 --coberturas
 
SELECT a.num_riesgo                ,
            a.cod_cob                   ,
            d.nom_cob        cobertura  ,
            g.suma_aseg_spto limmaxresp ,
            g.cod_franquicia deducible  ,
            SUM(a.imp_spto)  primas
       FROM p2100170 a,
            p2000020 b,
            a1001800 c,
            a1002150 d,
            a1002050 e,
            p2000030 f,
            p2000040 g,
            g2000020 h
     WHERE a.cod_cia                = '1'
       AND a.num_poliza             = '2411200003611'
       AND a.cod_cia                = b.cod_cia
       AND a.num_poliza             = b.num_poliza
       AND a.num_spto               = b.num_spto
       AND a.num_apli               = b.num_apli
       AND a.num_spto_apli          = b.num_spto_apli
       AND b.num_riesgo             = DECODE(b.tip_nivel,1,0,a.num_riesgo)
       AND b.cod_campo              = h.cod_campo
       AND f.cod_cia                = a.cod_cia
       AND f.num_poliza             = a.num_poliza
       AND f.num_spto               = a.num_spto
       AND f.num_apli               = a.num_apli
       AND f.num_spto_apli          = a.num_spto_apli
       AND g.cod_cia                = a.cod_cia
       AND g.num_poliza             = a.num_poliza
       AND g.num_spto               = a.num_spto
       AND g.num_apli               = a.num_apli
       AND g.num_spto_apli          = a.num_spto_apli
       AND g.num_riesgo             = a.num_riesgo
       AND g.cod_cob                = a.cod_cob
       AND g.num_periodo            = a.num_periodo
       AND c.cod_cia                = a.cod_cia
       AND c.cod_ramo               = a.cod_ramo
       AND d.cod_cia                = a.cod_cia
       AND d.cod_ramo               = a.cod_ramo
       AND TO_CHAR(d.cod_modalidad) = DECODE(c.cod_tratamiento,'V',b.val_campo,'99999')
       AND d.cod_cob                = a.cod_cob
       AND d.fec_validez            = f.fec_validez
       AND e.cod_cia                = d.cod_cia
       AND e.cod_cob                = d.cod_cob
       AND e.tip_cob                NOT IN (7,8,9)
       AND h.cod_cia                = c.cod_cia
       AND h.cod_ramo               = c.cod_ramo
       AND (h.mca_modalidad='S' OR h.cod_campo='COD_MODALIDAD')
       AND h.mca_inh                = 'N'
     GROUP BY a.num_riesgo     ,
              a.cod_cob        ,
              d.nom_cob        ,
              g.suma_aseg_spto ,
              g.cod_franquicia;
        
--primas
/*
 *  g_k_eco_prima        CONSTANT a2000161.cod_eco%TYPE := 1;
 g_k_eco_prima_10        CONSTANT a2000161.cod_eco%TYPE := 10; --JOSGOMEZ 07/06/2017
 g_k_eco_recargos     CONSTANT a2000161.cod_eco%TYPE := 2;
 g_k_eco_derechos     CONSTANT a2000161.cod_eco%TYPE := 3;
 g_k_eco_derechos_pr  CONSTANT a2000161.cod_eco%TYPE := 5;
 g_k_eco_impuestos    CONSTANT a2000161.cod_eco%TYPE := 4;
 g_k_eco_impuestos_pr CONSTANT a2000161.cod_eco%TYPE := 7;
 g_k_eco_prima_tot    CONSTANT a2000161.cod_eco%TYPE := 999;
 */
             
SELECT SUM(DECODE(a.cod_eco,999,a.imp_eco,0)) imp_prima_total,
            SUM(DECODE(a.cod_eco,1,    a.imp_eco,0)) imp_prima,
            SUM(DECODE(a.cod_eco,2, a.imp_eco,0)) imp_recargos,
            SUM(DECODE(a.cod_eco,3, a.imp_eco,
                                 5, a.imp_eco, 0)) imp_derechos,
            SUM(DECODE(a.cod_eco,4,a.imp_eco,
                                 7, a.imp_eco, 0)) imp_impuestos
       FROM p2000161 a
      WHERE a.cod_cia    = '1'
        AND a.num_poliza = '2411200003611';
       
       
 --recibos
       --l_num_cuotas
       SELECT max(a.num_cuota)
    FROM p2990700 a
   WHERE a.cod_cia    = '1'
     AND a.num_poliza = '2411200003611';
    --return
    SELECT imp_recibo
            FROM p2990700
           WHERE cod_cia    = '1'
             AND num_poliza = '2411200003611'
             AND num_cuota IN (1,2)
        ORDER BY num_cuota;
    
    
   --pagos = l_num_cuotas
       
-----------------------
       SELECT num_poliza, imp_recibo
            FROM p2990700
           WHERE cod_cia    = '1'
             AND num_cuota IN (1,2)
             AND rownum <= 200
             AND num_poliza LIKE '2427600000501%'
        ORDER BY num_poliza, num_cuota;
       
       SELECT num_poliza, count(*)
            FROM p2990700
           WHERE cod_cia    = '1'
             AND num_cuota IN (1,2)
             AND rownum <= 500
             AND num_poliza LIKE '24%'
             GROUP BY num_poliza
        ORDER BY num_poliza;
       
       
       
       
       
       ---------------------
       
       
       ppgraba_cadena
       
       SELECT * FROM g2009084_mmx
       ORDER BY FEC_ACTU DESC
       
       --MCA_BASICO_STD
       
       
       
       --tron2000.ev_k_productos_vida_mmx
       
       p_imprime_formapagos
