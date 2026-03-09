using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioAfiliado : IRepositorioAfiliado
    {
        public List<Afiliado> Listar(string apellidoPaterno, string apellidoMaterno, string nombres, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            List<Afiliado> listaAfiliados = new List<Afiliado>();
            Afiliado afiliado;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_consultar_afiliado");
            db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, apellidoPaterno);
            db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, apellidoMaterno);
            db.AddInParameter(dbc, "@wl_nom_nombre", DbType.String, nombres);
            db.AddInParameter(dbc, "@wl_pagina", DbType.Int32, indicePagina);
            db.AddInParameter(dbc, "@wl_registros", DbType.Int32, tamanhoPagina);
            db.AddInParameter(dbc, "@wl_orden", DbType.Int32, columnaOrdenar);
            db.AddInParameter(dbc, "@wl_direccion", DbType.String, direccionOrdenar);
            db.AddOutParameter(dbc, "@wo_registros", DbType.Int32, 0);

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    afiliado = new Afiliado();
                    afiliado.ApellidoPaterno = dr["ape_paterno"].ToString();
                    afiliado.ApellidoMaterno = dr["ape_materno"].ToString();
                    afiliado.Nombre = dr["nom_nombre"].ToString();
                    afiliado.CUSPP = dr["num_cuissp"].ToString();
                    listaAfiliados.Add(afiliado);
                }
            }
            totalRegistros = Convert.ToInt32(db.GetParameterValue(dbc, "@wo_registros"));

            return listaAfiliados;
        }

        public Afiliado ObtenerDatos(string solicitud, string CUSPP, string tipoIdentificacion, string numIdentificacion, string producto)
        {
            Afiliado afiliado = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_afiliado", solicitud, CUSPP, tipoIdentificacion, numIdentificacion, producto))
            {
                if (dr.Read())
                {
                    afiliado = new Afiliado();
                    afiliado.CUSPP = dr["num_cuissp"].ToString();
                    afiliado.ApellidoPaterno = dr["ape_paterno"].ToString();
                    afiliado.ApellidoMaterno = dr["ape_materno"].ToString();
                    afiliado.Nombre = dr["nom_nombre"].ToString();
                    if (dr["fec_nacimiento"] != DBNull.Value)
                        afiliado.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);
                    afiliado.Sexo = Convert.ToChar(dr["cod_sexo"]);
                    afiliado.CorreoElectronico = dr["gls_mail"].ToString();
                    afiliado.Categoria = new Categoria { Id = dr["cod_categoria"].ToString(), Nombre = dr["gls_categoria"].ToString() };
                    afiliado.AFP = new AFP { Id = dr["cod_afp"].ToString(), Nombre = dr["gls_afp"].ToString() };
                    if (dr["val_tot_cic"] != DBNull.Value)
                        afiliado.SaldoCIC = Convert.ToDouble(dr["val_tot_cic"]);
                    afiliado.NombreEmpresa = dr["gls_nom_empresa"].ToString();
                    afiliado.DireccionEmpresa = dr["gls_dir_empresa"].ToString();
                    afiliado.CiudadEmpresa = new Ciudad { Id = dr["cod_ciudad_emp"].ToString() };
                    afiliado.ComunaEmpresa = new Comuna { Id = dr["cod_comuna_emp"].ToString() };
                    afiliado.TelefonoEmpresa = dr["num_tel_emp_1"].ToString();
                    //<INIGTI_4022>
                    //afiliado.Agente = new Agente { Id = dr["num_vendedor"].ToString(), IdCartera = dr["cod_cartera"].ToString(), Nombre = dr["nom_agente"].ToString() };
                    afiliado.Agente = new Agente { Id = dr["num_vendedor"].ToString(), IdCartera = dr["cod_cartera"].ToString(), Nombre = dr["nom_agente"].ToString(), IdNivel = Convert.ToInt32(dr["cod_nivel"]) };
                    //<FINGTI_4022>

                    //<SRIINI06326>
                    afiliado.Consentimiento = (dr["ind_consentimiento"].ToString() == "S");
                    if (dr["fec_consentimiento"] != DBNull.Value)
                        afiliado.FechaConsentimiento = Convert.ToDateTime(dr["fec_consentimiento"]);
                    //<SRIFIN06326>

                    //<INIGTI_7012>
                    afiliado.EstadoCivil = new Temporal { cod_parametro = dr["cod_EstadoCivil"].ToString() };
                    //afiliado.ConfidencialidadDatos = new Parametro { Id = dr["cod_Confidencialidaddatos"].ToString() };
                    //<FINGTI_7012>

                    //<INIGTI_7012_S24>
                    afiliado.NumeroIdentificacion = dr["rut_persona"].ToString();
                    //<FINGTI_7012_S24>
                    afiliado.TipoIdentificacion = dr["tipo_identificacion"].ToString();

                    ////ConsentimientoAsesoria consentimientoAsesoria = new ConsentimientoAsesoria();
                    //consentimientoAsesoria.Consentimiento = dr["consentimiento_asesoria"].ToString();
                    //if (dr["fecha_consentimiento_asesoria"] != DBNull.Value)
                    //{
                    //    consentimientoAsesoria.FecUltimoConsentimiento = Convert.ToDateTime(dr["fecha_consentimiento_asesoria"]);
                    //}
                    //consentimientoAsesoria.Token = dr["gls_token"].ToString();
                    //afiliado.consentimientoAsesoria = consentimientoAsesoria;
                    afiliado.IdConsentimientoAsesoria = dr["id_consentimiento_asesoria"].ToString();
                    afiliado.Telefonos = dr["gls_telefono"].ToString();
                    afiliado.Celulares = dr["gls_celular"].ToString();

                    afiliado.RangoInversion = dr["gls_rango_inversion"].ToString();
                    afiliado.CentroLaboral = dr["gls_centro_laboral"].ToString();

                    afiliado.CorreoElectronicoCliente = string.Empty;
                    if (dr["gls_mail_cliente"] != DBNull.Value)
                        afiliado.CorreoElectronicoCliente = dr["gls_mail_cliente"].ToString();

                    afiliado.TelefonoCliente = string.Empty;
                    if (dr["gls_telefono_cliente"] != DBNull.Value)
                        afiliado.TelefonoCliente = dr["gls_telefono_cliente"].ToString();

                    afiliado.CelularCliente = string.Empty;
                    if (dr["gls_celular_cliente"] != DBNull.Value)
                        afiliado.CelularCliente = dr["gls_celular_cliente"].ToString();

                }
            }

            return afiliado;
        }

        public void Registrar(Afiliado entity)
        {
            throw new NotImplementedException();
        }

        //INI.YRV
        public void Registrar(Afiliado entity, string usuario, ref string numCUSPP)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_registrar_afiliado");

                db.AddInParameter(dbc, "@wl_cod_tipo_documento", DbType.String, entity.TipoIdentificacion);
                db.AddInParameter(dbc, "@wl_num_dociden", DbType.String, entity.NumeroIdentificacion);
                db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                db.AddInParameter(dbc, "@wl_nom_nombre", DbType.String, entity.Nombre);
                db.AddInParameter(dbc, "@wl_fec_nacimiento", DbType.DateTime, entity.FechaNacimiento);
                db.AddInParameter(dbc, "@wl_cod_sexo", DbType.String, entity.Sexo);
                db.AddInParameter(dbc, "@wl_gls_mail", DbType.String, entity.CorreoElectronico);
                db.AddInParameter(dbc, "@wl_cod_estadocivil", DbType.String, entity.EstadoCivil.cod_parametro);
                db.AddInParameter(dbc, "@wl_usr_ingreso", DbType.String, usuario);
                db.AddOutParameter(dbc, "@num_cuissp", DbType.String, 40);

                //if (entity.AFP != null)
                //{
                //    db.AddInParameter(dbc, "@wl_cod_afp", DbType.String, entity.AFP.Id);
                //}
                //else
                //{
                //    db.AddInParameter(dbc, "@wl_cod_afp", DbType.String, "");
                //}

                //db.AddInParameter(dbc, "@wl_val_tot_cic", DbType.Double, entity.SaldoCIC);

                //if (entity.Telefonos != null)
                //{
                //    db.AddInParameter(dbc, "@wl_gls_telefono", DbType.String, entity.Telefonos);
                //}
                //else
                //{
                //    db.AddInParameter(dbc, "@wl_gls_telefono", DbType.String, "");
                //}

                //if (entity.Celulares != null)
                //{
                //    db.AddInParameter(dbc, "@wl_gls_celular", DbType.String, entity.Celulares);
                //}
                //else
                //{
                //    db.AddInParameter(dbc, "@wl_gls_celular", DbType.String, "");
                //}

                //if (entity.RangoInversion != null)
                //{
                //    db.AddInParameter(dbc, "@wl_gls_rango_inversion", DbType.String, entity.RangoInversion);
                //}
                //else
                //{
                //    db.AddInParameter(dbc, "@wl_gls_rango_inversion", DbType.String, "");
                //}

                //if (entity.CentroLaboral != null)
                //{
                //    db.AddInParameter(dbc, "@wl_gls_centro_laboral", DbType.String, entity.CentroLaboral);
                //}
                //else
                //{
                //    db.AddInParameter(dbc, "@wl_gls_centro_laboral", DbType.String, "");
                //}

                db.ExecuteNonQuery(dbc);
                numCUSPP = db.GetParameterValue(dbc, "@num_cuissp").ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //FIN.YRV

        public void Actualizar(Afiliado entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_afiliado");

                db.AddInParameter(dbc, "@wl_num_cuispp", DbType.String, entity.CUSPP);
                db.AddInParameter(dbc, "@wl_gls_mail", DbType.String, entity.CorreoElectronico);

                if (entity.Categoria != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_categoria", DbType.String, entity.Categoria.Id);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_categoria", DbType.String, "");
                }

                if (entity.AFP != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_afp", DbType.String, entity.AFP.Id);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_afp", DbType.String, "");
                }

                db.AddInParameter(dbc, "@wl_val_tot_cic", DbType.Double, entity.SaldoCIC);

                //<INIGTI_7012>
                if (entity.EstadoCivil != null)
                {
                    db.AddInParameter(dbc, "@wl_cod_estadocivil", DbType.String, entity.EstadoCivil.cod_parametro);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_cod_estadocivil", DbType.String, "");
                }
                //INI.YRV
                db.AddInParameter(dbc, "@wl_cod_tipo_documento", DbType.String, entity.TipoIdentificacion);
                db.AddInParameter(dbc, "@wl_num_dociden", DbType.String, entity.NumeroIdentificacion);
                db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                db.AddInParameter(dbc, "@wl_nom_nombre", DbType.String, entity.Nombre);
                db.AddInParameter(dbc, "@wl_fec_nacimiento", DbType.DateTime, entity.FechaNacimiento);
                db.AddInParameter(dbc, "@wl_cod_sexo", DbType.String, entity.Sexo);
                //FIN.YRV

                if (entity.Telefonos != null)
                {
                    db.AddInParameter(dbc, "@wl_gls_telefono", DbType.String, entity.Telefonos);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_gls_telefono", DbType.String, "");
                }

                if (entity.Celulares != null)
                {
                    db.AddInParameter(dbc, "@wl_gls_celular", DbType.String, entity.Celulares);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_gls_celular", DbType.String, "");
                }

                if (entity.CorreoElectronicoCliente != null)
                {
                    db.AddInParameter(dbc, "@wl_gls_mail_cliente", DbType.String, entity.CorreoElectronicoCliente);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_gls_mail_cliente", DbType.String, "");
                }

                if (entity.TelefonoCliente != null)
                {
                    db.AddInParameter(dbc, "@wl_gls_telefono_cliente", DbType.String, entity.TelefonoCliente);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_gls_telefono_cliente", DbType.String, "");
                }

                if (entity.CelularCliente != null)
                {
                    db.AddInParameter(dbc, "@wl_gls_celular_cliente", DbType.String, entity.CelularCliente);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_gls_celular_cliente", DbType.String, "");
                }

                if (entity.RangoInversion != null)
                {
                    db.AddInParameter(dbc, "@wl_gls_rango_inversion", DbType.String, entity.RangoInversion);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_gls_rango_inversion", DbType.String, "");
                }

                if (entity.CentroLaboral != null)
                {
                    db.AddInParameter(dbc, "@wl_gls_centro_laboral", DbType.String, entity.CentroLaboral);
                }
                else
                {
                    db.AddInParameter(dbc, "@wl_gls_centro_laboral", DbType.String, "");
                }

                //if (entity.ConfidencialidadDatos != null)
                //{
                //    db.AddInParameter(dbc, "@wl_cod_Confidencialidaddatos", DbType.String, entity.ConfidencialidadDatos.Id);
                //}
                //else
                //{
                //    db.AddInParameter(dbc, "@wl_cod_Confidencialidaddatos", DbType.String, "");
                //}

                //<FINGTI_7012>

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(Afiliado entity)
        {
            throw new NotImplementedException();
        }

        public Afiliado ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Afiliado ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Afiliado> Listar()
        {
            throw new NotImplementedException();
        }

        //<INI_GTI_26560>
        public void ActualizarConsentimiento(Afiliado entity, ConsentimientoAsesoria entityConsentimientoAsesoria)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_consentimiento_afiliado");

                db.AddInParameter(dbc, "@wl_num_cuispp", DbType.String, entity.CUSPP);
                db.AddInParameter(dbc, "@wl_id_consentimiento_asesoria", DbType.String, entityConsentimientoAsesoria.IdConsentimientoAsesoria);
                db.AddInParameter(dbc, "@wl_id_contacto_asesoria", DbType.String, entityConsentimientoAsesoria.idContactoAsesoria);

                db.AddInParameter(dbc, "@wl_gls_token_adn", DbType.String, entityConsentimientoAsesoria.tokenADN);
                db.AddInParameter(dbc, "@wl_cod_producto", DbType.String, entityConsentimientoAsesoria.CodProducto);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, entityConsentimientoAsesoria.usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Beneficiario> ListarBeneficiarios(string numSolicitud, string usuario)
        {
            List<Beneficiario> listaBeneficiarios = new List<Beneficiario>();
            Beneficiario beneficiario;
            Identificacion identificacion;
            Parentesco parentesco;
            TipoInvalidez tipoInvalidez;
            Identificacion identificacionApoderado;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_listar_beneficiarios");
            db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, numSolicitud);
            db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    beneficiario = new Beneficiario();
                    identificacion = new Identificacion();
                    parentesco = new Parentesco();
                    tipoInvalidez = new TipoInvalidez();
                    identificacionApoderado = new Identificacion();

                    beneficiario.numSolicitud = dr["num_solicitud"].ToString();
                    beneficiario.numCorrelativo = Convert.ToInt32(dr["num_correlativo"]);
                    //beneficiario.CUSPP = dr["num_cuissp"].ToString();
                    beneficiario.ApellidoPaterno = dr["ape_paterno"].ToString();
                    beneficiario.ApellidoMaterno = dr["ape_materno"].ToString();
                    beneficiario.Nombre = dr["nom_persona"].ToString();

                    identificacion.IdTipo = dr["cod_tipo_identificacion"].ToString();
                    identificacion.Numero = dr["num_identificacion"].ToString();
                    identificacion.GlosaTipo = dr["gls_corta_identificacion"].ToString();
                    beneficiario.Identificacion = identificacion;

                    beneficiario.numTelefono = dr["num_telefono"].ToString();
                    beneficiario.numCelular = dr["num_celular"].ToString();

                    beneficiario.CorreoElectronico = dr["gls_mail"].ToString();
                    beneficiario.centroLaboral = dr["gls_centro_laboral"].ToString();

                    if (dr["fec_nacimiento"] != DBNull.Value) beneficiario.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);
                    beneficiario.Sexo = Convert.ToChar(dr["cod_sexo"]);

                    parentesco.Id = dr["cod_parentezco"].ToString();
                    parentesco.Nombre = dr["gls_parentezco"].ToString();
                    beneficiario.Parentesco = parentesco;

                    beneficiario.Invalido = (dr["ind_invalidez"].ToString() == "S") ? true : false;
                    tipoInvalidez.Id = dr["cod_tipo_invalidez"].ToString();
                    tipoInvalidez.Nombre = dr["gls_tipo_invalidez"].ToString();
                    beneficiario.TipoInvalidez = tipoInvalidez;
                    if (dr["fec_invalidez"] != DBNull.Value) beneficiario.FechaInvalidez = Convert.ToDateTime(dr["fec_invalidez"]);

                    beneficiario.numCuspp = dr["num_cuissp"].ToString();
                    beneficiario.envioPoliza = dr["envio_poliza"].ToString();

                    beneficiario.direccionPrincipal = new BeneficiarioDireccion
                    {
                        direccion = dr["gls_direccion_prn"].ToString(),
                        tipoVia = new Parametro { Id = dr["gls_tipo_via_prn"].ToString(), Nombre = dr["gls_cod_tipo_via_prn"].ToString() },
                        espacioUrbano = dr["gls_espacio_urbano_prn"].ToString(),
                        distrito = new Comuna { Id = dr["cod_distrito_prn"].ToString() },
                        provincia = new Ciudad { Id = dr["cod_provincia_prn"].ToString() },
                        departamento = new Departamento { Id = dr["cod_departamento_prn"].ToString() }
                    };

                    beneficiario.direccionAlterna = new BeneficiarioDireccion
                    {
                        direccion = dr["gls_direccion_ent_plz"].ToString(),
                        tipoVia = new Parametro { Id = dr["gls_tipo_via_ent_plz"].ToString(), Nombre = dr["gls_cod_tipo_via_ent_plz"].ToString() },
                        espacioUrbano = dr["gls_espacio_urbano_ent_plz"].ToString(),
                        distrito = new Comuna { Id = dr["cod_distrito_ent_plz"].ToString() },
                        provincia = new Ciudad { Id = dr["cod_provincia_ent_plz"].ToString() },
                        departamento = new Departamento { Id = dr["cod_departamento_ent_plz"].ToString() },
                        personaAutorizada = dr["gls_persona_autorizada_ent_plz"].ToString(),
                    };

                    beneficiario.indPEP = dr["ind_PEP"].ToString();

                    beneficiario.firmaDigital = new FirmaDigital { gls_token = dr["gls_token"].ToString() };

                    beneficiario.ApellidoPaternoApodero = dr["ape_paterno_apoderado"].ToString();
                    beneficiario.ApellidoMaternoApodero = dr["ape_materno_apoderado"].ToString();
                    beneficiario.NombreApodero = dr["nom_nombre_apoderado"].ToString();
                    identificacionApoderado.IdTipo = dr["cod_tipo_identificacion_apoderado"].ToString();
                    identificacionApoderado.Numero = dr["gls_nro_identificacion_apoderado"].ToString();
                    beneficiario.IdentificacionApodero = identificacionApoderado;

                    beneficiario.CorreoElectronicoValidacion = dr["gls_mail_valid"].ToString();
                    beneficiario.numTelefonoValidacion = dr["num_telefono_valid"].ToString();
                    beneficiario.numCelularValidacion = dr["num_celular_valid"].ToString();
                    beneficiario.glsCategoria = dr["gls_categoria"].ToString();

                    if (dr["ind_tiene_apoderado"] != DBNull.Value)
                    {
                        beneficiario.ind_tiene_apoderado = Convert.ToBoolean(dr["ind_tiene_apoderado"].ToString());
                    }

                    beneficiario.ind_seleccionado = dr["ind_seleccionado"].ToString();

                    Agente agente = new Agente { Id = dr["num_agente"].ToString() };
                    Afiliado afiliado = new Afiliado { Agente = agente };
                    beneficiario.Afiliado = afiliado;

                    //<GTI.59048-INI>
                    Temporal vinculo = new Temporal { cod_parametro = dr["cod_docparentesco"].ToString() };
                    Temporal paisOrigen = new Temporal { cod_parametro = dr["cod_pais_origen_doc"].ToString() };
                    Nacionalidad nacionalidad = new Nacionalidad();
                    Temporal essalud = new Temporal { cod_parametro = dr["cod_tipo_pacto_salud"].ToString() };

                    nacionalidad.cod_nacionalidad = dr["cod_nacionalidad"].ToString();
                    nacionalidad.gls_nacionalidad = dr["gls_nacionalidad"].ToString();

                    beneficiario.VinculoFamiliar = vinculo;
                    beneficiario.NroVinculoFamiliar = dr["num_docparentesco"].ToString();
                    if (dr["fec_fallecimiento"] != DBNull.Value) 
                        beneficiario.FechaFallecimiento = Convert.ToDateTime(dr["fec_fallecimiento"]);
                    beneficiario.PaisOrigen = paisOrigen;
                    beneficiario.Nacionalidad = nacionalidad;
                    beneficiario.DescuentoESSALUD = essalud;
                    //<GTI.59048-FIN>

                    listaBeneficiarios.Add(beneficiario);
                }
            }

            return listaBeneficiarios;
        }

        public void ActualizarBeneficiario(Beneficiario entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_beneficiario");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.numSolicitud);
                db.AddInParameter(dbc, "@wl_num_correlativo", DbType.Int32, entity.numCorrelativo);
                db.AddInParameter(dbc, "@wl_ape_paterno", DbType.String, entity.ApellidoPaterno);
                db.AddInParameter(dbc, "@wl_ape_materno", DbType.String, entity.ApellidoMaterno);
                db.AddInParameter(dbc, "@wl_nom_nombre", DbType.String, entity.Nombre);
                db.AddInParameter(dbc, "@wl_cod_tipo_identificacion", DbType.String, entity.Identificacion.IdTipo);
                db.AddInParameter(dbc, "@wl_num_identificacion", DbType.String, entity.Identificacion.Numero);
                db.AddInParameter(dbc, "@wl_num_telefono", DbType.String, entity.numTelefono);
                db.AddInParameter(dbc, "@wl_num_celular", DbType.String, entity.numCelular);
                db.AddInParameter(dbc, "@wl_correo_electronico", DbType.String, entity.CorreoElectronico);
                db.AddInParameter(dbc, "@wl_centro_laboral", DbType.String, entity.centroLaboral);
                db.AddInParameter(dbc, "@wl_envio_poliza", DbType.String, entity.envioPoliza);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, entity.usuario);
                db.AddInParameter(dbc, "@wl_apoderado", DbType.String, entity.Apoderado);
                //<GTI.59048-INI>
                db.AddInParameter(dbc, "@wl_cod_tipo_identificacion_pdt", DbType.String, entity.Identificacion.IdTipo);
                db.AddInParameter(dbc, "@wl_num_identificacion_pdt", DbType.String, entity.Identificacion.Numero);
                db.AddInParameter(dbc, "@wl_fec_fallecimiento", DbType.DateTime, entity.FechaFallecimiento);
                db.AddInParameter(dbc, "@wl_cod_nacionalidad", DbType.String, entity.Nacionalidad != null ? entity.Nacionalidad.cod_nacionalidad : null);
                db.AddInParameter(dbc, "@wl_cod_docparentesco", DbType.String, entity.VinculoFamiliar != null ? entity.VinculoFamiliar.cod_parametro : null);
                db.AddInParameter(dbc, "@wl_num_docparentesco", DbType.String, entity.NroVinculoFamiliar);
                db.AddInParameter(dbc, "@wl_cod_pais_origen_doc", DbType.String, entity.PaisOrigen != null ? entity.PaisOrigen.cod_parametro : null);
                db.AddInParameter(dbc, "@wl_cod_tipo_pacto_salud", DbType.String, entity.DescuentoESSALUD != null ? entity.DescuentoESSALUD.cod_parametro : null);
                //<GTI.59048-FIN>

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarDireccionBeneficiario(BeneficiarioDireccion entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_administrar_beneficiario_direccion");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.numSolicitud);
                db.AddInParameter(dbc, "@wl_direccion", DbType.String, entity.direccion);
                db.AddInParameter(dbc, "@wl_cod_tipo_via", DbType.String, entity.tipoVia.Id);
                db.AddInParameter(dbc, "@wl_espacio_urbano", DbType.String, entity.espacioUrbano);
                db.AddInParameter(dbc, "@wl_cod_departamento", DbType.String, entity.departamento.Id);
                db.AddInParameter(dbc, "@wl_cod_provincia", DbType.String, entity.provincia.Id);
                db.AddInParameter(dbc, "@wl_cod_distrito", DbType.String, entity.distrito.Id);
                db.AddInParameter(dbc, "@wl_persona_autorizada", DbType.String, entity.personaAutorizada);
                db.AddInParameter(dbc, "@wl_ind_tipo", DbType.String, entity.tipo);
                db.AddInParameter(dbc, "@wl_ind_vigencia", DbType.String, entity.vigencia);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, entity.usuario);
                //<GTI.59048-INI>
                db.AddInParameter(dbc, "@wl_num_telefono", DbType.String, entity.numeroTelefono);
                db.AddInParameter(dbc, "@wl_gls_nom_via", DbType.String, entity.nombreVia);
                db.AddInParameter(dbc, "@wl_gls_num_via", DbType.String, entity.numeroVia);
                db.AddInParameter(dbc, "@wl_gls_num_interior", DbType.String, entity.numeroInterior);
                db.AddInParameter(dbc, "@wl_cod_tipo_zona", DbType.String, entity.tipoZona != null ? entity.tipoZona.Id : null);
                db.AddInParameter(dbc, "@wl_gls_nom_zona", DbType.String, entity.nombreZona);
                db.AddInParameter(dbc, "@wl_gls_referencia", DbType.String, entity.referencia);
                db.AddInParameter(dbc, "@wl_cod_larga_distancia", DbType.String, entity.largaDistancia != null ? entity.largaDistancia.Id : null);
                db.AddInParameter(dbc, "@wl_gls_departamento", DbType.String, entity.numeroDepartamento);
                db.AddInParameter(dbc, "@wl_gls_manzana", DbType.String, entity.manzana);
                db.AddInParameter(dbc, "@wl_gls_lote", DbType.String, entity.numeroLote);
                db.AddInParameter(dbc, "@wl_gls_kilometro", DbType.String, entity.kilometro);
                db.AddInParameter(dbc, "@wl_gls_block", DbType.String, entity.block);
                db.AddInParameter(dbc, "@wl_gls_etapa", DbType.String, entity.etapa);
                //<GTI.59048-FIN>

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //<FIN_GTI_26560>

        public GrupoFamiliar ObtenerDatosCierre(int idGrupoFamiliar, string num_solicitud)
        {
            GrupoFamiliar grupo = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_benefi_cierre", idGrupoFamiliar, num_solicitud))
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

                    grupo.CorreoElectronico = dr["correo_electronico"].ToString();

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

                }
            }

            return grupo;
        }

        public void EliminarBeneficiarioIFP(string num_solicitud, string tipoPlan, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_benefi");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, num_solicitud);
                db.AddInParameter(dbc, "@wl_tipo_plan", DbType.String, tipoPlan);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
