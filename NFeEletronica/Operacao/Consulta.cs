using System;
using System.Collections.Generic;
using System.Xml;
using NFeEletronica.Assinatura;
using NFeEletronica.Contexto;
using NFeEletronica.NfeRecepcao2;
using NFeEletronica.NfeAutorizacao;
using NFeEletronica.NotaFiscal;
using NFeEletronica.Utils;
using NFeEletronica.Versao;
using NFeEletronica.Exceptions;
using System.IO;
using NFeEletronica.Certificado;
using System.Security.Cryptography.X509Certificates;

namespace NFeEletronica.Operacao
{
    public class Consulta
    {
        public Consulta(NFeContexto nfeContexto) : base(nfeContexto)
        {
        }
        
        public XmlDocument ConsultarNFe(string idNota, string cUF)
        {
            string tpAmb;
            if (this.NFeContexto.Producao)
            {
                tpAmb = "1";
            }
            else
            {
                tpAmb = "2";
            }
            
            // Monta xml de operacao de consulta.
            string xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                "<consSitNFe versao=\"3.10\" xmlns=\"http://www.portalfiscal.inf.br/nfe\">" +
                                "<tpAmb>" + tpAmb + "</tpAmb>" +
                                "<xServ>CONSULTAR</xServ>" +
                                "<chNFe>" + idNota + "</chNFe>" +
                                "</consSitNFe>";
            XmlDocument xmlDocConsulta = new XmlDocument();
            xmlDocConsulta.LoadXml(xmlString);
            
            // Instancia webservice de consulta e cria objeto de cabeçalho.
            var nfeConsulta = new NfeConsulta21.NfeConsulta2();
            var nfeCabecalho = new NfeConsulta21.nfeCabecMsg();
            
            // Define cabeçalho da requisicao e adiciona certificado
            nfeCabecalho.cUF = cUF;
            nfeCabecalho.versaoDados = NFeContexto.Versao.VersaoString;
            nfeConsulta.nfeCabecMsgValue = nfeCabecalho;
            nfeConsulta.ClientCertificates.Add(this.NFeContexto.Certificado);
            
            string xmlResponse;
            
            // Submete xml de consulta e obtém resultado.
            xmlResponse = nfeConsulta.nfeConsultaNF2(xmlDocConsulta.DocumentElement).OuterXml;
            
            // Carrega string com a resposta da requisição em um objeto XmlDocument.
            XmlDocument xmlRetConsSitNFe = new XmlDocument();
            xmlRetConsSitNFe.LoadXml(xmlResponse);
            
            return xmlRetConsSitNFe;
        }
    }
}
