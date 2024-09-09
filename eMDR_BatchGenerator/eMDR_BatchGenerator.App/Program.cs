// See https://aka.ms/new-console-template for more information
using eMDR_BatchGenerator.App;
using eMDR_BatchGenerator.Model;
using eMDR_BatchGenerator.Repository;
using System.Xml;

Console.WriteLine("Hello, World!");

List<TB_FDA_NOTIFICACAO> lstNotificacacoes = new()
{
    //new(){
    //   LAUDO= "00001",
    //    ANO="2021",
    //    PROTOCOLO="-",
    //    PRODUTO="SWCM 3810",
    //    CONEXAO="MT 16°",
    //    LINHA_DE_PRODUTO="STRONG SW",
    //    LINHA="STRONG SW",
    //    LOTE="O010119530",
    //    QTDE="1",
    //    CLASSE="Implante",
    //    OCORRENCIA="FRATURA",
    //    EVENT="FRACTURE",
    //    ANALISE_CQ="44197",
    //    DATE="2021",
    //    CLIENTE="GUSTAVO MASCARENHAS",
    //    CIDADE="-",
    //    UF="-",
    //    PAIS="BRASIL",
    //    CLIENTE_SAP="-",
    //    NOTIFICACAO_FDA="0"

    //}
};

//string date = DateTime.Now.ToString("yyyyMMdd");

FDANotificacaoRepository repo = new FDANotificacaoRepository();
lstNotificacacoes = repo.ObterNotificacoesLegadoSin().ToList();

GeradorXmlFDA gerador = new GeradorXmlFDA();
//gerador.Notificacao = lstNotificacacoes.FirstOrDefault();
foreach (var item in lstNotificacacoes)
{
    XmlDocument doc = gerador.GerarExcelDoFDA(lstNotificacacoes, item);
    doc.Save($"C:\\Users\\luan.hora\\Downloads\\MassaLegadoFDA\\arquivoFDA_Laudo_{item.LAUDO}.xml");

    repo.AtualizarLaudoComoXMLGerado(item);
    Console.WriteLine($"Laudo {item.LAUDO} gerado com sucesso para envio ao FDA...");
}
Console.WriteLine($"Processamento finalizado com sucesso, clique em algo para encerrar o programa...");

Console.ReadKey();
