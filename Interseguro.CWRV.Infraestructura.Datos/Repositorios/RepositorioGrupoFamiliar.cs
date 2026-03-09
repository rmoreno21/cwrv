using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data.SqlClient;


namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioGrupoFamiliar : IRepositorioGrupoFamiliar
    {

        public List<GrupoFamiliar> Listar(string cuspp)
        {
            List<GrupoFamiliar> listaGrupoFamiliar = new List<GrupoFamiliar>();
            GrupoFamiliar grupo;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_grupo_familiar", cuspp))
            {
                while (dr.Read())
                {
                    grupo = new GrupoFamiliar();
                    grupo.Id = Convert.ToInt32(dr["id_grupo_familiar"]);
                    grupo.ApellidoPaterno = dr["ape_paterno"].ToString();
                    grupo.ApellidoMaterno = dr["ape_materno"].ToString();
                    grupo.Nombre = dr["nom_persona"].ToString();
                    grupo.Parentesco = new Parentesco { Id = dr["cod_parentezco"].ToString(), Nombre = dr["gls_parentezco"].ToString() };
                    grupo.Sexo = Convert.ToChar(dr["cod_sexo"]);
                    grupo.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);
                    grupo.Invalido = (dr["ind_invalidez"].ToString() == "S") ? true : false;
                    grupo.TipoInvalidez = new TipoInvalidez { Id = dr["cod_tipo_invalidez"].ToString(), Nombre = dr["gls_tipo_invalidez"].ToString() };
                    if (dr["fec_invalidez"] != DBNull.Value)
                        grupo.FechaInvalidez = Convert.ToDateTime(dr["fec_invalidez"]);
                    grupo.Identificacion = new Identificacion { IdTipo = dr["cod_tipo_identificacion"].ToString(), Numero = dr["num_identificacion"].ToString() };

                    grupo.Nacionalidad = new Temporal { cod_parametro = dr["cod_nacionalidad"].ToString() };
                    grupo.Profesion = new Temporal { cod_parametro = dr["cod_profesion"].ToString() };
                    grupo.Residencia = new Temporal { cod_parametro = dr["cod_residencia"].ToString() };
                    grupo.ind_PEP = (dr["ind_PEP"].ToString() == "S") ? true : false;
                    grupo.ApellidoPaternoApdo = dr["ape_paterno_apoderado"].ToString();
                    grupo.ApellidoMaternoApdo = dr["ape_materno_apoderado"].ToString();
                    grupo.NombresApdo = dr["nom_persona_apoderado"].ToString();
                    grupo.IdentificacionApdo = new Identificacion { IdTipo = dr["cod_tipo_identificacion_apoderado"].ToString(), Numero = dr["num_identificacion_apoderado"].ToString() };
                    grupo.IndTieneApoderado = Convert.ToBoolean(dr["ind_tiene_apoderado"]);

                    listaGrupoFamiliar.Add(grupo);
                }
            }

            return listaGrupoFamiliar;
        }

        public GrupoFamiliar ObtenerDatos(int idGrupoFamiliar, string num_solicitud)//<INI.GTI_7012_V13>
        {
            GrupoFamiliar grupo = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_grupo_familiar", idGrupoFamiliar, num_solicitud))
            {
                if (dr.Read())
                {
                    grupo = new GrupoFamiliar();
                    grupo.Id = Convert.ToInt32(dr["id_grupo_familiar"]);
                    grupo.Afiliado = new Afiliado { CUSPP = dr["num_cuissp"].ToString() };
                    if (dr["ape_paterno"] != DBNull.Value)
                    {
                        grupo.ApellidoPaterno = dr["ape_paterno"].ToString();
                    }
                    if (dr["ape_materno"] != DBNull.Value)
                    {
                        grupo.ApellidoMaterno = dr["ape_materno"].ToString();
                    }
                    if (dr["nom_persona"] != DBNull.Value)
                    {
                        grupo.Nombre = dr["nom_persona"].ToString();
                    }
                    grupo.Identificacion = new Identificacion();
                    if (dr["cod_tipo_identificacion"] != DBNull.Value)
                    {
                        grupo.Identificacion.IdTipo = dr["cod_tipo_identificacion"].ToString();
                    }
                    if (dr["num_identificacion"] != DBNull.Value)
                    {
                        //grupo.Identificacion.Numero = Convert.ToInt32(dr["num_identificacion"]);
                        grupo.Identificacion.Numero = dr["num_identificacion"].ToString();
                    }
                    grupo.Parentesco = new Parentesco { Id = dr["cod_parentezco"].ToString() };
                    grupo.Sexo = Convert.ToChar(dr["cod_sexo"]);
                    grupo.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);
                    grupo.Invalido = (dr["ind_invalidez"].ToString() == "S") ? true : false;
                    grupo.TipoInvalidez = new TipoInvalidez { Id = dr["cod_tipo_invalidez"].ToString() };
                    if (dr["fec_invalidez"] != DBNull.Value)
                    {
                        grupo.FechaInvalidez = Convert.ToDateTime(dr["fec_invalidez"]);
                    }

                    //<INIGTI_7012>
                    grupo.Nacionalidad = new Temporal { cod_parametro = dr["cod_Nacionalidad"].ToString() };
                    grupo.Profesion = new Temporal { cod_parametro = dr["cod_Profesion"].ToString() };
                    grupo.Residencia = new Temporal { cod_parametro = dr["cod_Residencia"].ToString() };
                    grupo.ind_PEP = (dr["ind_PEP"].ToString() == "S") ? true : false;
                    grupo.ind_SujetoObligado = (dr["ind_SujetoObligado"].ToString() == "S") ? true : false;

                    grupo.Banco = new Parametro { Id = dr["cod_Banco"].ToString() };
                    grupo.Comunicacion = new Parametro { Id = dr["cod_Comunicacion"].ToString() };

                    grupo.TipoCtaBanco = new Parametro { Id = dr["cod_tipo_cta_banco"].ToString() };

                    if (dr["numerobanco"] != DBNull.Value)
                    {
                        grupo.NumeroBanco = dr["numerobanco"].ToString();
                    }

                    grupo.Confidencialidaddatos = new Parametro { Id = dr["cod_Confidencialidaddatos"].ToString() };
                    //<FINGTI_7012>

                    //S38
                    grupo.CorreoElectronico = dr["correo_electronico"].ToString();

                    //S40
                    if (dr["cod_estado_civil"] != DBNull.Value)
                    {
                        grupo.estadoCivil = dr["cod_estado_civil"].ToString();
                    }
                    if (dr["num_telefono"] != DBNull.Value)
                    {
                        grupo.telefono = dr["num_telefono"].ToString();
                    }
                    if (dr["num_celular"] != DBNull.Value)
                    {
                        grupo.celular = dr["num_celular"].ToString();
                    }
                    if (dr["gls_cargo"] != DBNull.Value)
                    {
                        grupo.cargo = dr["gls_cargo"].ToString();
                    }
                    if (dr["cod_moneda_ingreso"] != DBNull.Value)
                    {
                        grupo.monedaIngreso = new Moneda { Id = dr["cod_moneda_ingreso"].ToString() };
                    }
                    else
                    {
                        grupo.monedaIngreso = new Moneda { Id = "0" };
                    }

                    if (dr["gls_centro_laboral"] != DBNull.Value)
                    {
                        grupo.centroLaboral = dr["gls_centro_laboral"].ToString();
                    }
                    if (dr["gls_actividad_economica"] != DBNull.Value)
                    {
                        grupo.actividadEconomica = dr["gls_actividad_economica"].ToString();
                    }
                    if (dr["val_ingreso_neto"] != DBNull.Value)
                    {
                        grupo.ingresoNeto = Convert.ToSingle(dr["val_ingreso_neto"].ToString());
                    }
                    if (dr["cod_canal_distribucion"] != DBNull.Value)
                    {
                        grupo.SolicitudIFP = new SolicitudIFP { CodCanalDistribucion = dr["cod_canal_distribucion"].ToString() };
                    }
                    if (dr["ind_origen"] != DBNull.Value)
                    {
                        grupo.SolicitudIFP.OrigenCotizacion = dr["ind_origen"].ToString();
                    }

                    if (dr["ape_paterno_apoderado"] != DBNull.Value)
                    {
                        grupo.ApellidoPaternoApdo = dr["ape_paterno_apoderado"].ToString();
                    }
                    if (dr["ape_materno_apoderado"] != DBNull.Value)
                    {
                        grupo.ApellidoMaternoApdo = dr["ape_materno_apoderado"].ToString();
                    }
                    if (dr["nom_persona_apoderado"] != DBNull.Value)
                    {
                        grupo.NombresApdo = dr["nom_persona_apoderado"].ToString();
                    }

                    grupo.IdentificacionApdo = new Identificacion();
                    if (dr["cod_tipo_identificacion_apoderado"] != DBNull.Value)
                    {
                        grupo.IdentificacionApdo.IdTipo = dr["cod_tipo_identificacion_apoderado"].ToString();
                    }
                    if (dr["num_identificacion_apoderado"] != DBNull.Value)
                    {
                        grupo.IdentificacionApdo.Numero = dr["num_identificacion_apoderado"].ToString();
                    }

                    if (dr["cod_sexo_apoderado"] != DBNull.Value)
                    {
                        grupo.SexoApdo = Convert.ToChar(dr["cod_sexo_apoderado"]);
                    }

                    if (dr["fec_nacimiento_apoderado"] != DBNull.Value)
                    {
                        grupo.FechaNacimientoApdo = Convert.ToDateTime(dr["fec_nacimiento_apoderado"]);
                    }

                    if (dr["gls_declaracion_jurada"] != DBNull.Value)
                    {
                        grupo.OrigenFondo = new OrigenFondo() { declaracionJurada = dr["gls_declaracion_jurada"].ToString() };
                    }

                    if (dr["porcentaje_beneficio"] != DBNull.Value)
                    {
                        grupo.ValPjeRenta = Convert.ToDouble(dr["porcentaje_beneficio"].ToString());
                    }

                    if (dr["ind_tiene_apoderado"] != DBNull.Value)
                    {
                        grupo.IndTieneApoderado = Convert.ToBoolean(dr["ind_tiene_apoderado"]);
                    }

                }
            }

            return grupo;
        }

        public void Registrar(GrupoFamiliar entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_grupo_familiar");

                db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, entity.Afiliado.CUSPP);
                db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                db.AddInParameter(dbc, "@wl_nom_persona", DbType.String, entity.Nombre);
                db.AddInParameter(dbc, "@wl_cod_tipo_identificacion", DbType.String, entity.Identificacion.IdTipo);
                db.AddInParameter(dbc, "@wl_num_identificacion", DbType.String, entity.Identificacion.Numero);
                db.AddInParameter(dbc, "@wl_cod_parentesco", DbType.String, entity.Parentesco.Id);
                db.AddInParameter(dbc, "@wl_cod_sexo", DbType.String, entity.Sexo);
                db.AddInParameter(dbc, "@wl_fec_nacimiento", DbType.DateTime, entity.FechaNacimiento);
                db.AddInParameter(dbc, "@wl_ind_invalidez", DbType.String, entity.Invalido ? "S" : "N");
                db.AddInParameter(dbc, "@wl_cod_tipo_invalidez", DbType.String, entity.TipoInvalidez.Id);
                db.AddInParameter(dbc, "@wl_fec_invalidez", DbType.DateTime, entity.FechaInvalidez);
                db.AddInParameter(dbc, "@wl_usr_ingreso", DbType.String, entity.Usuario.NombreUsuario);
                //<INIGTI_7012>
                db.AddInParameter(dbc, "@wl_ind_PEP", DbType.String, entity.ind_PEP ? "S" : "N");
                db.AddInParameter(dbc, "@wl_ind_SujetoObligado", DbType.String, entity.ind_SujetoObligado ? "S" : "N");
                if (entity.Nacionalidad != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_Nacionalidad", DbType.String, entity.Nacionalidad.cod_parametro);
                    db.AddInParameter(dbc, "@wl_cod_Profesion", DbType.String, entity.Profesion.cod_parametro);
                    db.AddInParameter(dbc, "@wl_cod_Residencia", DbType.String, entity.Residencia.cod_parametro);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_Nacionalidad", DbType.String, "");
                    db.AddInParameter(dbc, "@wl_cod_Profesion", DbType.String, "");
                    db.AddInParameter(dbc, "@wl_cod_Residencia", DbType.String, "");
                }
                //<FINGTI_7012>

                db.AddInParameter(dbc, "@wl_ape_paterno_apod", DbType.String, valorDefecto(entity.ApellidoPaternoApdo, null));
                db.AddInParameter(dbc, "@wl_ape_materno_apod", DbType.String, valorDefecto(entity.ApellidoMaternoApdo, null));
                db.AddInParameter(dbc, "@wl_nom_persona_apod", DbType.String, valorDefecto(entity.NombresApdo, null));

                if (entity.IdentificacionApdo != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_tipo_identificacion_apod", DbType.String, valorDefecto(entity.IdentificacionApdo.IdTipo, null));
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_tipo_identificacion_apod", DbType.String, null);
                }

                if (entity.IdentificacionApdo != null)
                {
                    db.AddInParameter(dbc, "@wl_num_identificacion_apod", DbType.String, valorDefecto(entity.IdentificacionApdo.Numero, null));
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_num_identificacion_apod", DbType.String, null);
                }

                db.AddInParameter(dbc, "@wl_cod_sexo_apod", DbType.String, valorDefecto(entity.SexoApdo, null));
                db.AddInParameter(dbc, "@wl_fec_nacimiento_apod", DbType.DateTime, valorDefecto(entity.FechaNacimientoApdo, null));
                db.AddInParameter(dbc, "@wl_ind_tiene_apoderado", DbType.Boolean, valorDefecto(entity.IndTieneApoderado, null));

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(GrupoFamiliar entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_grupo_familiar");

                db.AddInParameter(dbc, "@wl_id_grupo_familiar", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, entity.Afiliado.CUSPP);
                db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                db.AddInParameter(dbc, "@wl_nom_persona", DbType.String, entity.Nombre);
                db.AddInParameter(dbc, "@wl_cod_tipo_identificacion", DbType.String, entity.Identificacion.IdTipo);
                db.AddInParameter(dbc, "@wl_num_identificacion", DbType.String, entity.Identificacion.Numero);
                db.AddInParameter(dbc, "@wl_cod_parentesco", DbType.String, entity.Parentesco.Id);
                db.AddInParameter(dbc, "@wl_cod_sexo", DbType.String, entity.Sexo);
                db.AddInParameter(dbc, "@wl_fec_nacimiento", DbType.DateTime, entity.FechaNacimiento);
                db.AddInParameter(dbc, "@wl_ind_invalidez", DbType.String, entity.Invalido ? "S" : "N");
                db.AddInParameter(dbc, "@wl_cod_tipo_invalidez", DbType.String, entity.TipoInvalidez.Id);
                if (entity.FechaInvalidez != null)
                    db.AddInParameter(dbc, "@wl_fec_invalidez", DbType.DateTime, entity.FechaInvalidez);
                else
                    db.AddInParameter(dbc, "@wl_fec_invalidez", DbType.DateTime, null);
                db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.Usuario.NombreUsuario);
                //<INIGTI_7012>
                db.AddInParameter(dbc, "@wl_ind_PEP", DbType.String, entity.ind_PEP ? "S" : "N");
                db.AddInParameter(dbc, "@wl_ind_SujetoObligado", DbType.String, entity.ind_SujetoObligado ? "S" : "N");
                if (entity.Nacionalidad != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_Nacionalidad", DbType.String, entity.Nacionalidad.cod_parametro);
                    db.AddInParameter(dbc, "@wl_cod_Profesion", DbType.String, entity.Profesion.cod_parametro);
                    db.AddInParameter(dbc, "@wl_cod_Residencia", DbType.String, entity.Residencia.cod_parametro);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_Nacionalidad", DbType.String, "");
                    db.AddInParameter(dbc, "@wl_cod_Profesion", DbType.String, "");
                    db.AddInParameter(dbc, "@wl_cod_Residencia", DbType.String, "");
                }

                if (entity.Banco != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_Banco", DbType.String, entity.Banco.Id);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_Banco", DbType.String, "");
                }

                if (entity.Comunicacion != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_Comunicacion", DbType.String, entity.Comunicacion.Id);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_Comunicacion", DbType.String, "");
                }

                db.AddInParameter(dbc, "@wl_numerobanco", DbType.String, entity.NumeroBanco);

                if (entity.Confidencialidaddatos != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_Confidencialidaddatos", DbType.String, entity.Confidencialidaddatos.Id);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_Confidencialidaddatos", DbType.String, "");
                }

                db.AddInParameter(dbc, "@wl_gls_flagrenta", DbType.String, entity.flagRenta);

                if (entity.TipoCtaBanco != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_tipo_cta_banco", DbType.String, entity.TipoCtaBanco.Id);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_tipo_cta_banco", DbType.String, "");
                }

                //<FINGTI_7012>

                //<INI.GTI_7012_V13>
                if (entity.SolicitudRPPlus != null)
                {
                    if (entity.SolicitudRPPlus.Id != null)
                        db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.SolicitudRPPlus.Id);
                    else
                        db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, "");
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, "");
                }
                //<FIN.GTI_7012_V13>

                //YRV.INI
                //if (entity.CorreoElectronico != null)
                //{
                //    db.AddInParameter(dbc, "@wl_gls_mail", DbType.String, entity.CorreoElectronico);
                //}
                //else
                //{
                //    db.AddInParameter(dbc, "@wl_gls_mail", DbType.String, "");
                //}
                db.AddInParameter(dbc, "@wl_gls_mail", DbType.String, valorDefecto(entity.CorreoElectronico, ""));
                db.AddInParameter(dbc, "@wl_cod_estado_civil", DbType.String, valorDefecto(entity.estadoCivil, ""));
                db.AddInParameter(dbc, "@wl_num_telefono", DbType.String, valorDefecto(entity.telefono, ""));
                db.AddInParameter(dbc, "@wl_num_celular", DbType.String, valorDefecto(entity.celular, ""));
                db.AddInParameter(dbc, "@wl_gls_centro_laboral", DbType.String, valorDefecto(entity.centroLaboral, ""));
                db.AddInParameter(dbc, "@wl_gls_cargo", DbType.String, valorDefecto(entity.cargo, ""));
                db.AddInParameter(dbc, "@wl_gls_actividad_economica", DbType.String, valorDefecto(entity.actividadEconomica, ""));

                if (entity.monedaIngreso != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_moneda_ingreso", DbType.String, valorDefecto(entity.monedaIngreso.Id, ""));
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_moneda_ingreso", DbType.String, "");
                }
                //YRV.FIN

                db.AddInParameter(dbc, "@wl_val_ingreso_neto", DbType.Double, valorDefecto(entity.ingresoNeto, 0));

                db.AddInParameter(dbc, "@wl_ape_paterno_apod", DbType.String, valorDefecto(entity.ApellidoPaternoApdo, null));
                db.AddInParameter(dbc, "@wl_ape_materno_apod", DbType.String, valorDefecto(entity.ApellidoMaternoApdo, null));
                db.AddInParameter(dbc, "@wl_nom_persona_apod", DbType.String, valorDefecto(entity.NombresApdo, null));

                if (entity.IdentificacionApdo != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_tipo_identificacion_apod", DbType.String, valorDefecto(entity.IdentificacionApdo.IdTipo, null));
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_tipo_identificacion_apod", DbType.String, null);
                }


                if (entity.IdentificacionApdo != null)
                {
                    db.AddInParameter(dbc, "@wl_num_identificacion_apod", DbType.String, valorDefecto(entity.IdentificacionApdo.Numero, null));
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_num_identificacion_apod", DbType.String, null);
                }


                db.AddInParameter(dbc, "@wl_cod_sexo_apod", DbType.String, valorDefecto(entity.SexoApdo, null));
                db.AddInParameter(dbc, "@wl_fec_nacimiento_apod", DbType.DateTime, valorDefecto(entity.FechaNacimientoApdo, null));

                if (entity.OrigenFondo != null)
                {
                    db.AddInParameter(dbc, "@wl_gls_declaracion_jurada", DbType.String, entity.OrigenFondo.declaracionJurada);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_gls_declaracion_jurada", DbType.String, "");
                }

                db.AddInParameter(dbc, "@wl_ind_tiene_apoderado", DbType.Boolean, entity.IndTieneApoderado);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private object valorDefecto(object valorEvaluar, object valorDefecto)
        {
            return valorEvaluar != null ? valorEvaluar : valorDefecto;
        }

        public void RegistrarBeneficiarios(List<GrupoFamiliar> lstEntity, int idGrupoFamiliar, string tipoPlan)
        {
            try
            {
                //int idGrupoFamiliar = 2;
                foreach (var entity in lstEntity)
                {
                    Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                    SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_grupo_familiar_ifp");

                    db.AddInParameter(dbc, "@wl_id_grupo_familiar", DbType.Int32, idGrupoFamiliar);
                    db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, entity.Afiliado.CUSPP);
                    db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                    db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                    db.AddInParameter(dbc, "@wl_nom_persona", DbType.String, entity.Nombre);
                    db.AddInParameter(dbc, "@wl_cod_tipo_identificacion", DbType.String, entity.Identificacion.IdTipo);
                    db.AddInParameter(dbc, "@wl_num_identificacion", DbType.String, entity.Identificacion.Numero);
                    db.AddInParameter(dbc, "@wl_cod_parentesco", DbType.String, entity.Parentesco.Id);
                    db.AddInParameter(dbc, "@wl_cod_sexo", DbType.String, entity.Sexo);
                    db.AddInParameter(dbc, "@wl_fec_nacimiento", DbType.DateTime, entity.FechaNacimiento);
                    db.AddInParameter(dbc, "@wl_ind_invalidez", DbType.String, entity.Invalido ? "S" : "N");
                    db.AddInParameter(dbc, "@wl_cod_tipo_invalidez", DbType.String, entity.TipoInvalidez.Id);
                    if (entity.FechaInvalidez != null)
                        db.AddInParameter(dbc, "@wl_fec_invalidez", DbType.DateTime, entity.FechaInvalidez);
                    else
                        db.AddInParameter(dbc, "@wl_fec_invalidez", DbType.DateTime, null);
                    db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.Usuario.NombreUsuario);

                    db.AddInParameter(dbc, "@wl_ind_PEP", DbType.String, entity.ind_PEP ? "S" : "N");
                    db.AddInParameter(dbc, "@wl_ind_SujetoObligado", DbType.String, entity.ind_SujetoObligado ? "S" : "N");
                    if (entity.Nacionalidad != null)
                    {
                        db.AddInParameter(dbc, "@wl_cod_Nacionalidad", DbType.String, entity.Nacionalidad.cod_parametro);
                        db.AddInParameter(dbc, "@wl_cod_Profesion", DbType.String, entity.Profesion.cod_parametro);
                        db.AddInParameter(dbc, "@wl_cod_Residencia", DbType.String, entity.Residencia.cod_parametro);
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_cod_Nacionalidad", DbType.String, "");
                        db.AddInParameter(dbc, "@wl_cod_Profesion", DbType.String, "");
                        db.AddInParameter(dbc, "@wl_cod_Residencia", DbType.String, "");
                    }

                    if (entity.Banco != null)
                    {
                        db.AddInParameter(dbc, "@wl_cod_Banco", DbType.String, entity.Banco.Id);
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_cod_Banco", DbType.String, "");
                    }

                    if (entity.Comunicacion != null)
                    {
                        db.AddInParameter(dbc, "@wl_cod_Comunicacion", DbType.String, entity.Comunicacion.Id);
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_cod_Comunicacion", DbType.String, "");
                    }

                    db.AddInParameter(dbc, "@wl_numerobanco", DbType.String, entity.NumeroBanco);

                    if (entity.Confidencialidaddatos != null)
                    {
                        db.AddInParameter(dbc, "@wl_cod_Confidencialidaddatos", DbType.String, entity.Confidencialidaddatos.Id);
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_cod_Confidencialidaddatos", DbType.String, "");
                    }

                    db.AddInParameter(dbc, "@wl_gls_flagrenta", DbType.String, entity.flagRenta);

                    if (entity.TipoCtaBanco != null)
                    {
                        db.AddInParameter(dbc, "@wl_cod_tipo_cta_banco", DbType.String, entity.TipoCtaBanco.Id);
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_cod_tipo_cta_banco", DbType.String, "");
                    }

                    if (entity.SolicitudIFP != null)
                    {
                        if (entity.SolicitudIFP.Id != null)
                            db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.SolicitudIFP.Id);
                        else
                            db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, "");
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, "");
                    }

                    db.AddInParameter(dbc, "@wl_pje_renta", DbType.Double, entity.ValPjeRenta);

                    db.AddInParameter(dbc, "@wl_id_tipo_plan", DbType.String, tipoPlan);

                    db.ExecuteNonQuery(dbc);
                    idGrupoFamiliar += 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GrupoFamiliar ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public GrupoFamiliar ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<GrupoFamiliar> Listar()
        {
            throw new NotImplementedException();
        }

        public void RegistrarPersonaVinculada(GrupoFamiliar entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_persona_vinculada");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.SolicitudRPPlus.Id);
                db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                db.AddInParameter(dbc, "@wl_nom_persona", DbType.String, entity.Nombre);
                db.AddInParameter(dbc, "@wl_cod_tipo_identificacion", DbType.String, entity.Identificacion.IdTipo);
                db.AddInParameter(dbc, "@wl_num_identificacion", DbType.String, entity.Identificacion.Numero);
                db.AddInParameter(dbc, "@wl_cod_parentesco", DbType.String, entity.Parentesco.Id);
                db.AddInParameter(dbc, "@wl_usr_ingreso", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarPersonaVinculada(GrupoFamiliar entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_persona_vinculada");

                db.AddInParameter(dbc, "@wl_id_persona_vinculada", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.SolicitudRPPlus.Id);
                db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                db.AddInParameter(dbc, "@wl_nom_persona", DbType.String, entity.Nombre);
                db.AddInParameter(dbc, "@wl_cod_tipo_identificacion", DbType.String, entity.Identificacion.IdTipo);
                db.AddInParameter(dbc, "@wl_num_identificacion", DbType.String, entity.Identificacion.Numero);
                db.AddInParameter(dbc, "@wl_cod_parentesco", DbType.String, entity.Parentesco.Id);
                db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EliminarPersonaVinculada(int idPersonaVinculada, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_persona_vinculada");

                db.AddInParameter(dbc, "@wl_id_persona_vinculada", DbType.Int32, idPersonaVinculada);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GrupoFamiliar> ObtenerPersonaVinculada(GrupoFamiliar entity)
        {
            try
            {
                List<GrupoFamiliar> listaGrupoFamiliar = new List<GrupoFamiliar>();
                GrupoFamiliar grupo;

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_persona_vinculada", entity.Id, entity.SolicitudRPPlus.Id, entity.ApellidoPaterno, entity.ApellidoMaterno, entity.Nombre, entity.Identificacion.IdTipo, entity.Identificacion.Numero, entity.Parentesco.Id, entity.Usuario.NombreUsuario))
                {
                    while (dr.Read())
                    {
                        grupo = new GrupoFamiliar();
                        grupo.Id = Convert.ToInt32(dr["id_persona_vinculada"]);
                        grupo.SolicitudRPPlus = new SolicitudRPPlus() { Id = dr["num_solicitud"].ToString() };
                        grupo.ApellidoPaterno = dr["gls_apellido_paterno"].ToString();
                        grupo.ApellidoMaterno = dr["gls_apellido_materno"].ToString();
                        grupo.Nombre = dr["gls_nombres"].ToString();
                        grupo.Parentesco = new Parentesco { Id = dr["cod_parentesco"].ToString(), Nombre = dr["gls_parentesco"].ToString() };
                        grupo.Identificacion = new Identificacion { IdTipo = dr["cod_tipo_identificacion"].ToString(), GlosaTipo = dr["gls_tipo_identificacion"].ToString(), Numero = dr["num_identificacion"].ToString() };

                        listaGrupoFamiliar.Add(grupo);
                    }
                }

                return listaGrupoFamiliar;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(GrupoFamiliar entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_grupo_familiar_api");

                db.AddInParameter(dbc, "@wl_id_grupo_familiar", DbType.String, entity.Id);
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void ActualizarBeneficiariosPNoG(List<GrupoFamiliar> lstEntity)
        {
            try
            {
                foreach (var entity in lstEntity)
                {
                    Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                    SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_grupo_familiar_ifp_cierre");

                    db.AddInParameter(dbc, "@wl_id_grupo_familiar", DbType.Int32, entity.Id);
                    db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, entity.Afiliado.CUSPP);
                    db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                    db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                    db.AddInParameter(dbc, "@wl_nom_persona", DbType.String, entity.Nombre);
                    db.AddInParameter(dbc, "@wl_cod_tipo_identificacion", DbType.String, entity.Identificacion.IdTipo);
                    db.AddInParameter(dbc, "@wl_num_identificacion", DbType.String, entity.Identificacion.Numero);
                    db.AddInParameter(dbc, "@wl_cod_parentesco", DbType.String, entity.Parentesco.Id);
                    db.AddInParameter(dbc, "@wl_cod_sexo", DbType.String, entity.Sexo);
                    db.AddInParameter(dbc, "@wl_fec_nacimiento", DbType.DateTime, entity.FechaNacimiento);
                    db.AddInParameter(dbc, "@wl_ind_invalidez", DbType.String, entity.Invalido ? "S" : "N");
                    db.AddInParameter(dbc, "@wl_cod_tipo_invalidez", DbType.String, entity.TipoInvalidez.Id);

                    if (entity.FechaInvalidez != null)
                    {
                        db.AddInParameter(dbc, "@wl_fec_invalidez", DbType.DateTime, entity.FechaInvalidez);
                    }
                    else
                    {
                            db.AddInParameter(dbc, "@wl_fec_invalidez", DbType.DateTime, null);
                    }

                    if (entity.Nacionalidad != null)
                    {
                        db.AddInParameter(dbc, "@wl_cod_Nacionalidad", DbType.String, entity.Nacionalidad.cod_parametro);
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_cod_Nacionalidad", DbType.String, null);
                    }
                    
                    if (entity.Profesion != null)
                    {
                        db.AddInParameter(dbc, "@wl_cod_Profesion", DbType.String, entity.Profesion.cod_parametro);
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_cod_Profesion", DbType.String, null);
                    }
                    
                    if (entity.Residencia != null)
                    {
                        db.AddInParameter(dbc, "@wl_cod_Residencia", DbType.String, entity.Residencia.cod_parametro);
                    }
                    else
                    {
                        db.AddInParameter(dbc, "@wl_cod_Residencia", DbType.String, null);
                    }

                    db.AddInParameter(dbc, "@wl_ind_PEP", DbType.String, entity.ind_PEP ? "S" : "N");
                    db.AddInParameter(dbc, "@wl_ind_SujetoObligado", DbType.String, entity.ind_SujetoObligado ? "S" : "N");
                    db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.SolicitudIFP.Id);
                    db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.Usuario.NombreUsuario);
                    
                    db.ExecuteNonQuery(dbc);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
