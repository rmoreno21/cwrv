using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioSolicitudEscenario : IRepositorioSolicitudEscenario
    {
        public List<SolicitudEscenario> Listar(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol)
        {
            List<SolicitudEscenario> listaSolicitudesEscenario = new List<SolicitudEscenario>();
            SolicitudEscenario solicitudEscenario;
                
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_rvi_propue_escenario", numJefe, numSupervisor, numAgente, codUserName, codRol))
            {
                while (dr.Read())
                {
                    solicitudEscenario = new SolicitudEscenario();
                    solicitudEscenario.FechaPresentacion = Convert.ToDateTime(dr["fec_presentacion"]);
                    if (dr["fec_cierre_lote"] != DBNull.Value && dr["fec_cierre_lote"].ToString() != string.Empty)
                        solicitudEscenario.FechaCierreLote = Convert.ToDateTime(dr["fec_cierre_lote"]);
                    if (dr["num_operacion"] != DBNull.Value && dr["num_operacion"].ToString() != string.Empty)
                        solicitudEscenario.NumOperacion = Convert.ToInt64(dr["num_operacion"]);
                    if (dr["num_solicitud"] != DBNull.Value && dr["num_solicitud"].ToString() != string.Empty)
                        solicitudEscenario.NumSolicitud = dr["num_solicitud"].ToString();
                    if (dr["fec_registro_escenario"] != DBNull.Value && dr["fec_registro_escenario"].ToString() != string.Empty)
                        solicitudEscenario.FecRegistroEscenario = Convert.ToDateTime(dr["fec_registro_escenario"]);
                    if (dr["cod_pje_cesion_comision"] != DBNull.Value && dr["cod_pje_cesion_comision"].ToString() != string.Empty)
                        solicitudEscenario.CodPjeCesionComision = Convert.ToDouble(dr["cod_pje_cesion_comision"]);
                    if (dr["pje_aumento_comision"] != DBNull.Value && dr["pje_aumento_comision"].ToString() != string.Empty)
                        solicitudEscenario.PjeAumentoComision = Convert.ToDouble(dr["pje_aumento_comision"]);
                    if (dr["val_tasa_ajuste_tra"] != DBNull.Value && dr["val_tasa_ajuste_tra"].ToString() != string.Empty)
                        solicitudEscenario.ValTasaAjusteTra = Convert.ToDouble(dr["val_tasa_ajuste_tra"]);
                    if (dr["ind_condicion_especial"] != DBNull.Value && dr["ind_condicion_especial"].ToString() != string.Empty)
                        solicitudEscenario.IndCondicionEspecial = dr["ind_condicion_especial"].ToString();
                    if (dr["ind_aprueba"] != DBNull.Value && dr["ind_aprueba"].ToString() != string.Empty)
                        solicitudEscenario.IndAprueba = dr["ind_aprueba"].ToString();
                    if (dr["fec_respuesta"] != DBNull.Value && dr["fec_respuesta"].ToString() != string.Empty)
                        solicitudEscenario.FecRespuesta = Convert.ToDateTime(dr["fec_respuesta"]);
                    if (dr["cod_username"] != DBNull.Value && dr["cod_username"].ToString() != string.Empty)
                        solicitudEscenario.Usuario = new Usuario { NombreUsuario = dr["cod_username"].ToString() };
                    if (dr["val_total_cic"] != DBNull.Value && dr["val_total_cic"].ToString() != string.Empty)
                        solicitudEscenario.ValTotalCic = Convert.ToDouble(dr["val_total_cic"]);
                    if (dr["val_bono_actualizado"] != DBNull.Value && dr["val_bono_actualizado"].ToString() != string.Empty)
                        solicitudEscenario.ValBonoActualizado = Convert.ToDouble(dr["val_bono_actualizado"]);
                    if (dr["num_cuspp"] != DBNull.Value && dr["num_cuspp"].ToString() != string.Empty)
                        solicitudEscenario.Afiliado = new Afiliado
                        {
                            CUSPP = dr["num_cuspp"].ToString(),
                            NombreEmpresa = (dr["gls_nom_persona"] != DBNull.Value && dr["gls_nom_persona"].ToString() != string.Empty) ? dr["gls_nom_persona"].ToString() : null,
                            FechaNacimiento = (dr["fec_nacimiento"] != DBNull.Value && dr["fec_nacimiento"].ToString() != string.Empty) ? Convert.ToDateTime(dr["fec_nacimiento"]) : Convert.ToDateTime("01/01/1900"),
                            DireccionEmpresa = (dr["gls_ciudad_comuna"] != DBNull.Value && dr["gls_ciudad_comuna"].ToString() != string.Empty) ? dr["gls_ciudad_comuna"].ToString() : null,
                            CorreoElectronico = (dr["gls_mail"] != DBNull.Value && dr["gls_mail"].ToString() != string.Empty) ? dr["gls_mail"].ToString() : null
                        };
                    if (dr["cod_padre"] != DBNull.Value && dr["cod_padre"].ToString() != string.Empty)
                        solicitudEscenario.CodPadre = Convert.ToInt64(dr["cod_padre"]);
                    if (dr["cod_nodo"] != DBNull.Value && dr["cod_nodo"].ToString() != string.Empty)
                        solicitudEscenario.CodNodo = dr["cod_nodo"].ToString();
                    if (dr["cod_categoria"] != DBNull.Value && dr["cod_categoria"].ToString() != string.Empty)
                        solicitudEscenario.Categoria = new Categoria
                        {
                            Id = dr["cod_categoria"].ToString(),
                            Nombre = (dr["gls_categoria"] != DBNull.Value && dr["gls_categoria"].ToString() != string.Empty) ? dr["gls_categoria"].ToString() : null
                        };
                    if (dr["num_agente"] != DBNull.Value && dr["num_agente"].ToString() != string.Empty)
                        solicitudEscenario.Agente = new Agente
                        {
                            Id = dr["num_agente"].ToString(),
                            Nombre = (dr["nom_agente"] != DBNull.Value && dr["nom_agente"].ToString() != string.Empty) ? dr["nom_agente"].ToString() : null
                        };
                    if (dr["num_supervisor"] != DBNull.Value && dr["num_supervisor"].ToString() != string.Empty)
                        solicitudEscenario.Supervision = new Supervision
                        {
                            NumeroRegistro = Convert.ToInt32(dr["num_supervisor"]),
                            Supervisor = (dr["nom_supervisor"] != DBNull.Value && dr["nom_supervisor"].ToString() != string.Empty) ? dr["nom_supervisor"].ToString() : null
                        };
                    if (dr["ind_estado_seleccion"] != DBNull.Value && dr["ind_estado_seleccion"].ToString() != string.Empty)
                        solicitudEscenario.IndEstadoSeleccion = dr["ind_estado_seleccion"].ToString();
                    if (dr["fec_dia_cita"] != DBNull.Value && dr["fec_dia_cita"].ToString() != string.Empty)
                        solicitudEscenario.FecDiaCita = Convert.ToDateTime(dr["fec_dia_cita"]);
                    if (dr["ind_vigencia_agente"] != DBNull.Value && dr["ind_vigencia_agente"].ToString() != string.Empty)
                        solicitudEscenario.IndVigenciaAgente = dr["ind_vigencia_agente"].ToString();
                    if (dr["val_mto_acom_agente"] != DBNull.Value && dr["val_mto_acom_agente"].ToString() != string.Empty)
                        solicitudEscenario.ValMtoAgenteAcom = Convert.ToDouble(dr["val_mto_acom_agente"]);
                    if (dr["num_cotizacion_elegida"] != DBNull.Value && dr["num_cotizacion_elegida"].ToString() != string.Empty)
                        solicitudEscenario.NumCotizacionElegida = Convert.ToInt64(dr["num_cotizacion_elegida"].ToString());
                    if (dr["fec_cierre"] != DBNull.Value && dr["fec_cierre"].ToString() != string.Empty)
                        solicitudEscenario.FecCierre = Convert.ToDateTime(dr["fec_cierre"]);
                    if (dr["fec_solicitud"] != DBNull.Value && dr["fec_solicitud"].ToString() != string.Empty)
                        solicitudEscenario.FecSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        solicitudEscenario.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"]),
                            Nombre = (dr["gls_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty) ? dr["gls_tipo_movimiento"].ToString() : null
                        };
                    solicitudEscenario.NumNivel = Convert.ToInt64(dr["num_nivel"].ToString());
                    solicitudEscenario.ValidarACOM = (dr["ind_validar_acom"].ToString() == "S") ? true : false;
                    solicitudEscenario.ValidarDTRA = (dr["ind_validar_dtra"].ToString() == "S") ? true : false;

                    listaSolicitudesEscenario.Add(solicitudEscenario);
                }
            }

            return listaSolicitudesEscenario;
        }

        //<INIGTI_4081>
        public List<SolicitudEscenario> ListarCambios(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol)
        {
            List<SolicitudEscenario> listaSolicitudesEscenario = new List<SolicitudEscenario>();
            SolicitudEscenario solicitudEscenario;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_rvi_solicitudes_cambio", numJefe, numSupervisor, numAgente, codUserName, codRol))
            {
                while (dr.Read())
                {
                    solicitudEscenario = new SolicitudEscenario();
                    solicitudEscenario.FechaPresentacion = Convert.ToDateTime(dr["fec_presentacion"]);

                    if (dr["num_solicitud"] != DBNull.Value && dr["num_solicitud"].ToString() != string.Empty)
                        solicitudEscenario.NumSolicitud = dr["num_solicitud"].ToString();

                    if (dr["cod_pje_cesion_comision"] != DBNull.Value && dr["cod_pje_cesion_comision"].ToString() != string.Empty)
                        solicitudEscenario.CodPjeCesionComision = Convert.ToDouble(dr["cod_pje_cesion_comision"]);
                    
                    if (dr["pje_aumento_comision"] != DBNull.Value && dr["pje_aumento_comision"].ToString() != string.Empty)
                        solicitudEscenario.PjeAumentoComision = Convert.ToDouble(dr["pje_aumento_comision"]);

                    if (dr["num_cuspp"] != DBNull.Value && dr["num_cuspp"].ToString() != string.Empty)
                        solicitudEscenario.Afiliado = new Afiliado
                        {
                            CUSPP = dr["num_cuspp"].ToString(),
                            NombreEmpresa = (dr["gls_nom_persona"] != DBNull.Value && dr["gls_nom_persona"].ToString() != string.Empty) ? dr["gls_nom_persona"].ToString() : null,
                            
                        };

                   
                    if (dr["cod_categoria"] != DBNull.Value && dr["cod_categoria"].ToString() != string.Empty)
                        solicitudEscenario.Categoria = new Categoria
                        {
                            Id = dr["cod_categoria"].ToString(),
                            Nombre = (dr["gls_categoria"] != DBNull.Value && dr["gls_categoria"].ToString() != string.Empty) ? dr["gls_categoria"].ToString() : null
                        };

                    if (dr["num_agente"] != DBNull.Value && dr["num_agente"].ToString() != string.Empty)
                        solicitudEscenario.Agente = new Agente
                        {
                            Id = dr["num_agente"].ToString(),
                            Nombre = (dr["nom_agente"] != DBNull.Value && dr["nom_agente"].ToString() != string.Empty) ? dr["nom_agente"].ToString() : null
                        };

                    if (dr["num_supervisor"] != DBNull.Value && dr["num_supervisor"].ToString() != string.Empty)
                        solicitudEscenario.Supervision = new Supervision
                        {
                            NumeroRegistro = Convert.ToInt32(dr["num_supervisor"]),
                            Supervisor = (dr["nom_supervisor"] != DBNull.Value && dr["nom_supervisor"].ToString() != string.Empty) ? dr["nom_supervisor"].ToString() : null
                        };

                    
                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        solicitudEscenario.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"]),
                            Nombre = (dr["gls_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty) ? dr["gls_tipo_movimiento"].ToString() : null
                        };

                    if (dr["cod_afp"] != DBNull.Value && dr["gls_afp"].ToString() != string.Empty)
                        solicitudEscenario.AFP = new AFP
                        {
                            Id = dr["cod_afp"].ToString(),
                            Nombre = (dr["gls_afp"] != DBNull.Value && dr["gls_afp"].ToString() != string.Empty) ? dr["gls_afp"].ToString() : null
                        };

                    if (dr["val_mto_cta_individual"] != DBNull.Value)
                        solicitudEscenario.ValTotalCic = Convert.ToDouble(dr["val_mto_cta_individual"]);

                    if (dr["ind_recotiza"] != DBNull.Value)
                        solicitudEscenario.Recotizacion = dr["ind_recotiza"].ToString();

                    if (dr["cia_competidora"] != DBNull.Value && dr["gls_cia_competidora"].ToString() != string.Empty)
                        solicitudEscenario.Compania = new Compania
                        {
                            Id = dr["cia_competidora"].ToString(),
                            Nombre = (dr["gls_cia_competidora"] != DBNull.Value && dr["gls_cia_competidora"].ToString() != string.Empty) ? dr["gls_cia_competidora"].ToString() : null
                        };

                    if (dr["val_tasa_ajuste_tra"] != DBNull.Value && dr["val_tasa_ajuste_tra"].ToString() != string.Empty)
                        solicitudEscenario.ValTasaAjusteTra = Convert.ToDouble(dr["val_tasa_ajuste_tra"]);

                    
                    List<Cotizacion> lstCotizacion = new List<Cotizacion>();
                    Cotizacion cotizacion = new Cotizacion();

                    if (dr["val_tasa_venta_max"] != DBNull.Value)
                        cotizacion.TasaVentaMaxima = Convert.ToDouble(dr["val_tasa_venta_max"]);

                    if (dr["val_tra_min"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionistaMinimo = Convert.ToDouble(dr["val_tra_min"]);

                    if (dr["val_tasa_int_vit"] != DBNull.Value)
                        cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_int_vit"]);

                    if (dr["val_tra"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["val_tra"]);

                    if (dr["val_tasa_int_vit_objetivo"] != DBNull.Value)
                        cotizacion.TasaVentaSbsObjetivo = Convert.ToDouble(dr["val_tasa_int_vit_objetivo"]);

                    if (dr["val_tra_objetivo"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionistaObjetivo = Convert.ToDouble(dr["val_tra_objetivo"]);

                        
                    lstCotizacion.Add(cotizacion);

                    solicitudEscenario.Cotizaciones = lstCotizacion;

                    listaSolicitudesEscenario.Add(solicitudEscenario);
                }
            }

            return listaSolicitudesEscenario;
        }


        public List<SolicitudEscenario> ListarSolicitudesEmail(string idSolicitudes)
        {
            List<SolicitudEscenario> listaSolicitudesEscenario = new List<SolicitudEscenario>();
            SolicitudEscenario solicitudEscenario;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_rvi_solicitudes_email", idSolicitudes))
            {
                while (dr.Read())
                {
                    solicitudEscenario = new SolicitudEscenario();

                    if (dr["num_solicitud"] != DBNull.Value && dr["num_solicitud"].ToString() != string.Empty)
                        solicitudEscenario.NumSolicitud = dr["num_solicitud"].ToString();

                    if (dr["cod_compania"] != DBNull.Value && dr["gls_compania"].ToString() != string.Empty)
                        solicitudEscenario.Compania = new Compania
                        {
                            Id = dr["cod_compania"].ToString(),
                            Nombre = (dr["gls_compania"] != DBNull.Value && dr["gls_compania"].ToString() != string.Empty) ? dr["gls_compania"].ToString() : null
                        };

                    if (dr["cod_categoria"] != DBNull.Value && dr["cod_categoria"].ToString() != string.Empty)
                        solicitudEscenario.Categoria = new Categoria
                        {
                            Id = dr["cod_categoria"].ToString(),
                            Nombre = (dr["gls_categoria"] != DBNull.Value && dr["gls_categoria"].ToString() != string.Empty) ? dr["gls_categoria"].ToString() : null
                        };

                    if (dr["val_mto_cic"] != DBNull.Value)
                        solicitudEscenario.ValTotalCic = Convert.ToDouble(dr["val_mto_cic"]);

                    if (dr["cod_afp"] != DBNull.Value && dr["gls_afp"].ToString() != string.Empty)
                        solicitudEscenario.AFP = new AFP
                        {
                            Id = dr["cod_afp"].ToString(),
                            Nombre = (dr["gls_afp"] != DBNull.Value && dr["gls_afp"].ToString() != string.Empty) ? dr["gls_afp"].ToString() : null
                        };

                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        solicitudEscenario.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"]),
                            Nombre = (dr["gls_movimiento"] != DBNull.Value && dr["gls_movimiento"].ToString() != string.Empty) ? dr["gls_movimiento"].ToString() : null
                        };

                    if (dr["num_agente"] != DBNull.Value && dr["num_agente"].ToString() != string.Empty)
                        solicitudEscenario.Agente = new Agente
                        {
                            Id = dr["num_agente"].ToString(),
                            Nombre = (dr["gls_agente"] != DBNull.Value && dr["gls_agente"].ToString() != string.Empty) ? dr["gls_agente"].ToString() : null
                        };

                    if (dr["nom_supervisor"] != DBNull.Value && dr["nom_supervisor"].ToString() != string.Empty)
                        solicitudEscenario.Supervision = new Supervision
                        {
                             Supervisor = dr["nom_supervisor"].ToString(),
                             Jefe = dr["nom_jefe"].ToString()
                        };

                    solicitudEscenario.PjeAumentoComision = Convert.ToDouble(dr["val_pje_acom"]);

                    solicitudEscenario.CodPjeCesionComision = Convert.ToDouble(dr["val_pje_dcom"]);

                    Cotizacion cotizacion = new Cotizacion();
                    cotizacion.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };

                    cotizacion.Modalidad = new Modalidad { Nombre = dr["modalidad"].ToString(), Id = dr["cod_modalidad"].ToString() };

                    cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_gar"]);

                    cotizacion.PeriodoDiferido = Convert.ToInt32(dr["val_per_dif"]);

                    cotizacion.PensionCia = Convert.ToInt32(dr["val_mto_pen"]);

                    cotizacion.AjusteTRA = Convert.ToDouble(dr["val_dif_tra"]);

                    cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_SBS"]);
                    cotizacion.TasaVentaSbsObjetivo = Convert.ToDouble(dr["val_tasa_SBS_Esperada"]);
                    cotizacion.TasaVentaMaxima = Convert.ToDouble(dr["val_tasa_venta_max"]);

                    cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["val_TIR"]);
                    cotizacion.TasaRetornoAccionistaObjetivo = Convert.ToDouble(dr["val_TIR_Esperada"]);
                    cotizacion.TasaRetornoAccionistaMinimo = Convert.ToDouble(dr["val_tra_min"]);

                    cotizacion.pbs = Convert.ToDouble(dr["pbs"]);
                    cotizacion.PensionCiaObjetivo = Convert.ToDouble(dr["val_pen_cia_Esperada"]);
                    
                    //<INIGTI_6623>
                    cotizacion.PensionCiaMOObjetivo = Convert.ToDouble(dr["val_pen_cia_mo_Esperada"]);
                    cotizacion.PensionAFPObjetivo = Convert.ToDouble(dr["val_pen_afp_Esperada"]);
                    cotizacion.PensionCiaMO = Convert.ToDouble(dr["val_mto_pen_mo"]);
                    cotizacion.PensionAFP = Convert.ToDouble(dr["val_pen_afp"]);
                    //<FINGTI_6623>

                    solicitudEscenario.Cotizaciones = new List<Cotizacion>();
                    solicitudEscenario.Cotizaciones.Add(cotizacion);

                    listaSolicitudesEscenario.Add(solicitudEscenario);
                }
            }

            return listaSolicitudesEscenario;
        }

        public List<SolicitudEscenario> ListarSolicitudesValidaFlujo(string NumSolicitud, string NumOperacion)
        {
            List<SolicitudEscenario> listaSolicitudesEscenario = new List<SolicitudEscenario>();
            SolicitudEscenario solicitudEscenario;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_valida_flujo_solicitud", NumSolicitud, NumOperacion))
            {
                while (dr.Read())
                {
                    solicitudEscenario = new SolicitudEscenario();

                    if (dr["num_solicitud"] != DBNull.Value && dr["num_solicitud"].ToString() != string.Empty)
                        solicitudEscenario.NumSolicitud = dr["num_solicitud"].ToString();

                    if (dr["num_operacion"] != DBNull.Value && dr["num_operacion"].ToString() != string.Empty)
                        solicitudEscenario.NumOperacion = Convert.ToInt64(dr["num_operacion"]);

                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty)
                        solicitudEscenario.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"]),
                            Nombre = (dr["gls_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty) ? dr["gls_tipo_movimiento"].ToString() : null
                        };

                    listaSolicitudesEscenario.Add(solicitudEscenario);
                }
            }

            return listaSolicitudesEscenario;
        }

        //<INIGTI_4081>

        //<INIGTI_6556>
        public List<SolicitudEscenario> ListarSolicitudesPendientesEmail(string cod_rol)
        {
            List<SolicitudEscenario> listaSolicitudesEscenario = new List<SolicitudEscenario>();
            SolicitudEscenario solicitudEscenario;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_lista_solicitudes_pendiente_email",cod_rol))
            {
                while (dr.Read())
                {
                    solicitudEscenario = new SolicitudEscenario();

                    if (dr["num_solicitud"] != DBNull.Value && dr["num_solicitud"].ToString() != string.Empty)
                        solicitudEscenario.NumSolicitud = dr["num_solicitud"].ToString();

                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        solicitudEscenario.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"]),
                            Nombre = dr["movimiento"].ToString()
                        };

                    if (dr["num_vendedor"] != DBNull.Value && dr["num_vendedor"].ToString() != string.Empty)
                        solicitudEscenario.Agente = new Agente
                        {
                            Id = dr["num_vendedor"].ToString()
                        };

                    listaSolicitudesEscenario.Add(solicitudEscenario);
                }
            }

            return listaSolicitudesEscenario;
        }
        //<FINGTI_6556>

        public void Registrar(ref SolicitudEscenario entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_ins_rvi_propue_escenario");

                db.AddInParameter(dbc, "@wl_empdata", DbType.String, entity.XMLSolicitudEscenario());
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, entity.Usuario.NombreUsuario);
                db.AddOutParameter(dbc, "@wl_num_solicitud", DbType.String, 20);
                
                db.ExecuteNonQuery(dbc);

                entity.NumSolicitud = db.GetParameterValue(dbc, "@wl_num_solicitud").ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegistrarExtraoficial(ref SolicitudEscenario entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_solicitud_extraoficial");

                db.AddInParameter(dbc, "@wl_num_solicitud_in", DbType.String, entity.NumSolicitud);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, entity.Usuario.NombreUsuario);
                db.AddOutParameter(dbc, "@wl_num_solicitud", DbType.String, 20);

                db.ExecuteNonQuery(dbc);

                entity.NumSolicitud = db.GetParameterValue(dbc, "@wl_num_solicitud").ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Registrar(SolicitudEscenario entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(SolicitudEscenario entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_upd_rvi_propue_escenario_operacion");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.NumSolicitud);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(ref SolicitudEscenario entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_upd_rvi_propue_escenario");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.NumSolicitud);
                db.AddInParameter(dbc, "@wl_val_pje_acom", DbType.Double, entity.PjeAumentoComision);
                db.AddInParameter(dbc, "@wl_val_pje_dcom", DbType.Double, entity.CodPjeCesionComision);
                db.AddInParameter(dbc, "@wl_val_mto_acom", DbType.Double, entity.ValMtoAgenteAcom);
                db.AddInParameter(dbc, "@wl_num_cotizacion_elegida", DbType.Int64, entity.NumCotizacionElegida);
                db.AddInParameter(dbc, "@wl_ind_estado_seleccion", DbType.String, entity.IndEstadoSeleccion);
                db.AddInParameter(dbc, "@wl_xml_cotizaciones", DbType.String, entity.XMLCotizacionesEscenario());
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, entity.Usuario.NombreUsuario);
                //<INIGTI_4081>
                db.AddInParameter(dbc, "@wl_cod_compania", DbType.String, entity.Compania.Id);
                //<FINGTI_4081>

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(SolicitudEscenario entity)
        {
            throw new NotImplementedException();
        }

        public SolicitudEscenario ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public SolicitudEscenario ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public SolicitudEscenario ObtenerPorId(string numSolicitud, string codUserName, string codRol)
        {
            SolicitudEscenario solicitudEscenario = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_rvi_propue_escenario", numSolicitud, codUserName, codRol))
            {
                // Datos de Solicitud Escenario
                if (dr.Read())
                {
                    solicitudEscenario = new SolicitudEscenario();
                    solicitudEscenario.FechaPresentacion = Convert.ToDateTime(dr["fec_presentacion"]);
                    //<SRIINI-25781>
                    if (dr["fec_cierre_lote"] != DBNull.Value && dr["fec_cierre_lote"].ToString() != string.Empty)
                        solicitudEscenario.FechaCierreLote = Convert.ToDateTime(dr["fec_cierre_lote"]);
                    //<SRIFIN-25781>
                    if (dr["num_operacion"] != DBNull.Value && dr["num_operacion"].ToString() != string.Empty)
                        solicitudEscenario.NumOperacion = Convert.ToInt64(dr["num_operacion"]);
                    if (dr["num_solicitud"] != DBNull.Value && dr["num_solicitud"].ToString() != string.Empty)
                        solicitudEscenario.NumSolicitud = dr["num_solicitud"].ToString();
                    if (dr["fec_registro_escenario"] != DBNull.Value && dr["fec_registro_escenario"].ToString() != string.Empty)
                        solicitudEscenario.FecRegistroEscenario = Convert.ToDateTime(dr["fec_registro_escenario"]);
                    if (dr["cod_pje_cesion_comision"] != DBNull.Value && dr["cod_pje_cesion_comision"].ToString() != string.Empty)
                        solicitudEscenario.CodPjeCesionComision = Convert.ToDouble(dr["cod_pje_cesion_comision"]);
                    if (dr["pje_aumento_comision"] != DBNull.Value && dr["pje_aumento_comision"].ToString() != string.Empty)
                        solicitudEscenario.PjeAumentoComision = Convert.ToDouble(dr["pje_aumento_comision"]);
                    if (dr["val_tasa_ajuste_tra"] != DBNull.Value && dr["val_tasa_ajuste_tra"].ToString() != string.Empty)
                        solicitudEscenario.ValTasaAjusteTra = Convert.ToDouble(dr["val_tasa_ajuste_tra"]);
                    if (dr["ind_condicion_especial"] != DBNull.Value && dr["ind_condicion_especial"].ToString() != string.Empty)
                        solicitudEscenario.IndCondicionEspecial = dr["ind_condicion_especial"].ToString();
                    if (dr["ind_aprueba"] != DBNull.Value && dr["ind_aprueba"].ToString() != string.Empty)
                        solicitudEscenario.IndAprueba = dr["ind_aprueba"].ToString();
                    if (dr["fec_respuesta"] != DBNull.Value && dr["fec_respuesta"].ToString() != string.Empty)
                        solicitudEscenario.FecRespuesta = Convert.ToDateTime(dr["fec_respuesta"]);
                    if (dr["cod_username"] != DBNull.Value && dr["cod_username"].ToString() != string.Empty)
                        solicitudEscenario.Usuario = new Usuario { NombreUsuario = dr["cod_username"].ToString() };
                    if (dr["val_total_cic"] != DBNull.Value && dr["val_total_cic"].ToString() != string.Empty)
                        solicitudEscenario.ValTotalCic = Convert.ToDouble(dr["val_total_cic"]);
                    if (dr["val_bono_actualizado"] != DBNull.Value && dr["val_bono_actualizado"].ToString() != string.Empty)
                        solicitudEscenario.ValBonoActualizado = Convert.ToDouble(dr["val_bono_actualizado"]);
                    if (dr["num_cuspp"] != DBNull.Value && dr["num_cuspp"].ToString() != string.Empty)
                        solicitudEscenario.Afiliado = new Afiliado
                        {
                            CUSPP = dr["num_cuspp"].ToString(),
                            NombreEmpresa = (dr["gls_nom_persona"] != DBNull.Value && dr["gls_nom_persona"].ToString() != string.Empty) ? dr["gls_nom_persona"].ToString() : null,
                            FechaNacimiento = (dr["fec_nacimiento"] != DBNull.Value && dr["fec_nacimiento"].ToString() != string.Empty) ? Convert.ToDateTime(dr["fec_nacimiento"]) : Convert.ToDateTime("01/01/1900"),
                            DireccionEmpresa = (dr["gls_ciudad_comuna"] != DBNull.Value && dr["gls_ciudad_comuna"].ToString() != string.Empty) ? dr["gls_ciudad_comuna"].ToString() : null,
                            CorreoElectronico = (dr["gls_mail"] != DBNull.Value && dr["gls_mail"].ToString() != string.Empty) ? dr["gls_mail"].ToString() : null
                        };
                    if (dr["cod_padre"] != DBNull.Value && dr["cod_padre"].ToString() != string.Empty)
                        solicitudEscenario.CodPadre = Convert.ToInt64(dr["cod_padre"]);
                    if (dr["cod_nodo"] != DBNull.Value && dr["cod_nodo"].ToString() != string.Empty)
                        solicitudEscenario.CodNodo = dr["cod_nodo"].ToString();
                    if (dr["cod_categoria"] != DBNull.Value && dr["cod_categoria"].ToString() != string.Empty)
                        solicitudEscenario.Categoria = new Categoria
                        {
                            Id = dr["cod_categoria"].ToString(),
                            Nombre = (dr["gls_categoria"] != DBNull.Value && dr["gls_categoria"].ToString() != string.Empty) ? dr["gls_categoria"].ToString() : null
                        };
                    if (dr["num_agente"] != DBNull.Value && dr["num_agente"].ToString() != string.Empty)
                        solicitudEscenario.Agente = new Agente
                        {
                            Id = dr["num_agente"].ToString(),
                            Nombre = (dr["nom_agente"] != DBNull.Value && dr["nom_agente"].ToString() != string.Empty) ? dr["nom_agente"].ToString() : null
                        };
                    if (dr["num_supervisor"] != DBNull.Value && dr["num_supervisor"].ToString() != string.Empty)
                        solicitudEscenario.Supervision = new Supervision
                        {
                            NumeroRegistro = Convert.ToInt32(dr["num_supervisor"]),
                            Supervisor = (dr["nom_supervisor"] != DBNull.Value && dr["nom_supervisor"].ToString() != string.Empty) ? dr["nom_supervisor"].ToString() : null
                        };
                    if (dr["ind_estado_seleccion"] != DBNull.Value && dr["ind_estado_seleccion"].ToString() != string.Empty)
                        solicitudEscenario.IndEstadoSeleccion = dr["ind_estado_seleccion"].ToString();
                    if (dr["fec_dia_cita"] != DBNull.Value && dr["fec_dia_cita"].ToString() != string.Empty)
                        solicitudEscenario.FecDiaCita = Convert.ToDateTime(dr["fec_dia_cita"]);
                    if (dr["ind_vigencia_agente"] != DBNull.Value && dr["ind_vigencia_agente"].ToString() != string.Empty)
                        solicitudEscenario.IndVigenciaAgente = dr["ind_vigencia_agente"].ToString();
                    if (dr["val_mto_acom_agente"] != DBNull.Value && dr["val_mto_acom_agente"].ToString() != string.Empty)
                        solicitudEscenario.ValMtoAgenteAcom = Convert.ToDouble(dr["val_mto_acom_agente"]);
                    if (dr["num_cotizacion_elegida"] != DBNull.Value && dr["num_cotizacion_elegida"].ToString() != string.Empty)
                        solicitudEscenario.NumCotizacionElegida = Convert.ToInt64(dr["num_cotizacion_elegida"].ToString());
                    if (dr["fec_cierre"] != DBNull.Value && dr["fec_cierre"].ToString() != string.Empty)
                        solicitudEscenario.FecCierre = Convert.ToDateTime(dr["fec_cierre"]);
                    if (dr["fec_solicitud"] != DBNull.Value && dr["fec_solicitud"].ToString() != string.Empty)
                        solicitudEscenario.FecSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        solicitudEscenario.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"]),
                            Nombre = (dr["gls_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty) ? dr["gls_tipo_movimiento"].ToString() : null
                        };
                    solicitudEscenario.ValidarACOM = dr["ind_validar_acom"].ToString() == "S" ? true : false;
                    solicitudEscenario.ValidarDTRA = dr["ind_validar_dtra"].ToString() == "S" ? true : false;

                    //<INIGTI_4081>
                    if (dr["cod_compania"] != DBNull.Value)
                        solicitudEscenario.Compania = new Compania {Id = dr["cod_compania"].ToString()};
                    //<FINGTI_4081>
                }
            }

            return solicitudEscenario;
        }

        public List<SolicitudEscenario> Listar()
        {
            throw new NotImplementedException();
        }

        //<GTI.INI-29372>
        public void ActualizarEnvioObligatorio(ref SolicitudEscenario entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_upd_rvi_cotiza_envio_obligatorio");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.NumSolicitud);
                
                db.AddInParameter(dbc, "@wl_xml_cotizaciones", DbType.String, entity.XMLCotizacionesEscenario());
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //<GTI.FIN-29372>


    }
}
