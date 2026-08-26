namespace DownloadImagemAtestado.Cli;

internal static class HelpText
{
    public static void Print()
    {
        Console.WriteLine(
            """
            DownloadImagemAtestado - baixa o anexo de um atestado e o organiza em pasta

            Uso:
              DownloadImagemAtestado --numemp <valor> --tipcol <valor> --numcad <valor> --local <pasta> --link <url> [--nomdoc <nome_arquivo>]

            Parametros:
              --numemp   Numero da empresa
              --tipcol   Tipo de colaborador
              --numcad   Numero de cadastro (matricula)
              --local    Pasta base onde a pasta numemp-tipcol-numcad sera criada
              --link     URL do arquivo a ser baixado
              --nomdoc   (Opcional) Nome do arquivo a ser salvo. Se omitido, usa o nome extraido do link
              -h, --help Exibe esta mensagem

            Exemplo:
              DownloadImagemAtestado --numemp 1 --tipcol 1 --numcad 12345 --local "C:\Atestados" --link "https://s3.amazonaws.com/atestado.app/condor.ind.br/attachments/arquivo.jpg" --nomdoc "atestado.jpg"
            """);
    }
}
