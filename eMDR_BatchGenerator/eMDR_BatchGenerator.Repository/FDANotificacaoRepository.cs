using Dapper;
using eMDR_BatchGenerator.Model;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eMDR_BatchGenerator.Repository
{
    public class FDANotificacaoRepository
    {
        private SqlConnection _connection;
        public FDANotificacaoRepository()
        {
            string conn = ConfigurationManager.ConnectionStrings["BaseLegadoMDRs"].ConnectionString;
            _connection = new SqlConnection(conn);
        }

        public IEnumerable<TB_FDA_NOTIFICACAO> ObterNotificacoesLegadoSin()
        {
            List<TB_FDA_NOTIFICACAO> ret = null;
            StringBuilder sqlQuery = new StringBuilder();
            //sqlQuery.AppendLine("SELECT LAUDO, ANO, PROTOCOLO, PRODUTO, CONEXAO, LINHA_DE_PRODUTO, LINHA, LOTE, QTDE, CLASSE, OCORRENCIA, [EVENT], ANALISE_CQ, [DATE], CLIENTE, CIDADE, UF, PAIS, CLIENTE_SAP, NOTIFICACAO_FDA FROM TB_FDA_NOTIFICACAO");

            string query = @"
                        SELECT TOP 1000 a.LAUDO, a.ANO, a.PROTOCOLO, a.PRODUTO, a.CONEXAO, a.LINHA_DE_PRODUTO, a.LINHA, a.LOTE, a.QTDE, a.CLASSE, a.OCORRENCIA, a.[EVENT], a.ANALISE_CQ, a.[DATE], a.CLIENTE, a.CIDADE, a.UF, a.PAIS, a.CLIENTE_SAP, a.NOTIFICACAO_FDA, a.XML_GERADO,
                            b.enderecoCliente, b.numeroEndereco, b.cepCliente, b.cidadeCliente, b.origem, b.nomeRede
                            FROM TB_FDA_NOTIFICACAO a
                            LEFT JOIN CLIENTES_NOTIFICACAO b
                            ON a.CLIENTE_SAP = convert(nvarchar(MAX),b.codigoCliente)
                            WHERE (a.XML_GERADO IS NULL OR a.XML_GERADO = 0) AND Laudo NOT LIKE '%#VALUE!%'
                        ";

            sqlQuery.Append(query);

            try
            {
                using (var dbConnection = _connection)
                {
                    dbConnection.Open();
                    ret = dbConnection.Query<TB_FDA_NOTIFICACAO>(sqlQuery.ToString(), commandType: CommandType.Text).ToList();
                    dbConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw;
            }

            return ret;
        }

        public void AtualizarLaudoComoXMLGerado(TB_FDA_NOTIFICACAO notificacao)
        {
            string conn = ConfigurationManager.ConnectionStrings["BaseLegadoMDRs"].ConnectionString;
            _connection = new SqlConnection(conn);

            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.AppendLine($"UPDATE TB_FDA_NOTIFICACAO SET XML_GERADO = 1 WHERE LAUDO = '{notificacao.LAUDO}'");

            try
            {
                using (var dbConnection = _connection)
                {
                    dbConnection.Open();
                    dbConnection.Execute(sqlQuery.ToString(), commandType: CommandType.Text);
                    dbConnection.Close();
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
