namespace eMDR_BatchGenerator.Model
{
    public class TB_FDA_NOTIFICACAO
    {
        public string LAUDO { get; set; }
        public string ANO { get; set; }
        public string PROTOCOLO { get; set; }
        public string PRODUTO { get; set; }
        public string CONEXAO { get; set; }
        public string LINHA_DE_PRODUTO { get; set; }
        public string LINHA { get; set; }
        public string LOTE { get; set; }
        public string QTDE { get; set; }
        public string CLASSE { get; set; }
        public string OCORRENCIA { get; set; }
        public string EVENT { get; set; }
        public string ANALISE_CQ { get; set; }
        public string DATE { get; set; }
        public string CLIENTE { get; set; }
        public string CIDADE { get; set; }
        public string UF { get; set; }
        public string PAIS { get; set; }
        public string CLIENTE_SAP { get; set; }
        public string NOTIFICACAO_FDA { get; set; }
        /*******************************************/

        public string enderecoCliente { get; set; }
        public string numeroEndereco { get; set; }
        public string cepCliente { get; set; }
        public string cidadeCliente { get; set; }
        public string origem { get; set; }
        public string nomeRede { get; set; }
    }
}
