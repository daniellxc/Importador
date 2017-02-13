using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using LAB5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    public class AgendamentoController : BaseController
    {
        AgendamentoDAO _dao = new AgendamentoDAO();
        // GET: Agendamento
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        public ActionResult Salvar(Agendamento agendamento)
        {
            if (!ModelState.IsValid) return View("Cadastro", agendamento);
            string acao = agendamento.IdAgendamento == 0 ? "Salvar agendamento: " : "Editar agendamento: ";
            try
            {
               
                agendamento.DataCriacao = DateTime.Now; 
                _dao.Salvar(agendamento);
                LogINFO(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(agendamento));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), acao  + LAB5Utils.ReflectionUtils.GetObjectDescription(agendamento) + ex.Message);
                return View("Cadastro", agendamento);
            }
        }

        public ActionResult Editar(int IdAgendamento)
        {
            return View("Cadastro", _dao.Buscar(IdAgendamento));
        }

        public ActionResult Excluir(int IdAgendamento)
        {
            Agendamento agd = _dao.Buscar(IdAgendamento);
            try
            {
                
                _dao.Excluir(IdAgendamento);
                LogINFO(this.ToString(), "Excluir agendamento: " + LAB5Utils.ReflectionUtils.GetObjectDescription(agd));
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), "Erro ao excluir agendamento: " +  LAB5Utils.ReflectionUtils.GetObjectDescription(agd) + ex.Message);
                ViewBag.Erro = ex.Message;
            }
            return View("Index");
        }


        public ActionResult Iniciar(int IdAgendamento)
        {
            Agendamento agd = _dao.Buscar(IdAgendamento);
            try
            {
                
                new AgendamentoBO().IniciarAgendamento(agd);
                LogINFO(this.ToString(), "Iniciar agendamento: " + LAB5Utils.ReflectionUtils.GetObjectDescription(agd));
                Alert("Agendamento iniciado com sucesso!");
            }catch(Exception ex)
            {
                string alert = "Erro ao iniciar agendamento. " + LAB5Utils.ReflectionUtils.GetObjectDescription(agd) + ex.Message;
                Alert(alert + ex.Message);
                LogWARN(this.ToString(), alert);
            }
            return View("Index");
        }

        public ActionResult Parar(int idAgendamento,string job, string groupJob)
        {
            Agendamento agd = _dao.Buscar(idAgendamento);
            try
            {
                new AgendamentoBO().PararAgendamento(job, groupJob);
                LogINFO(this.ToString(), "Parar agendamento: " + LAB5Utils.ReflectionUtils.GetObjectDescription(agd));
            }catch(Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), "Erro ao parar agendamento: " + LAB5Utils.ReflectionUtils.GetObjectDescription(agd));
            }
            return View("Index");
        }

        public bool Status(string job, string groupJob)
        {
            return new AgendamentoBO().StatusAgendamento(job, groupJob);
        }

        public PartialViewResult ExecucoesAgendamento(int id)
        {
            return PartialView(new ExecucaoAgendamentoDAO().BuscarExecucoesDeUmAgendamento(id).OrderByDescending(a=>a.DataExecucao).ToList());
        }

    }
}