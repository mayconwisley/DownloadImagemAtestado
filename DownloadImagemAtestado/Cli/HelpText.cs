namespace DownloadImagemAtestado.Cli;

internal static class HelpText
{
    public static void Print()
    {
        Console.WriteLine(
            """
            DownloadImagemAtestado - baixa o anexo de um atestado e o organiza em pasta

            Uso:
              DownloadImagemAtestado --numemp <valor> --tipcol <valor> --numcad <valor> --local <pasta> --link <url> [--nomdoc <nome_arquivo>] [--quiet]

            Parametros:
              --numemp     Numero da empresa
              --tipcol     Tipo de colaborador
              --numcad     Numero de cadastro (matricula)
              --local      Pasta base onde a pasta numemp-tipcol-numcad sera criada
              --link       URL do arquivo a ser baixado
              --nomdoc     (Opcional) Nome do arquivo a ser salvo. Se omitido, usa o nome extraido do link
              -q, --quiet  (Opcional) Nao escreve nada no console (stdout/stderr). Erros continuam sendo gravados no log
              -h, --help   Exibe esta mensagem

            Exemplo:
              DownloadImagemAtestado --numemp 1 --tipcol 1 --numcad 12345 --local "C:\Atestados" --link "https://exemplo.com.br/attachments/arquivo.jpg" --nomdoc "atestado.jpg" --quiet
            """);
    }
}
