# DownloadImagemAtestado

Utilitário de linha de comando (.exe) para baixar o anexo de um atestado a partir de uma URL e organizá-lo em disco por empresa/colaborador. Criado para substituir a rotina de download em LSP/4GL do Senior (`HttpObjeto` + `HttpDownload` + `.bat` de criação de diretório), sendo chamado diretamente via `ExecProg`.

## Sumário

- [Como funciona](#como-funciona)
- [Requisitos](#requisitos)
- [Build e publicação](#build-e-publicação)
- [Uso](#uso)
- [Parâmetros](#parâmetros)
- [Regra de nome da pasta](#regra-de-nome-da-pasta)
- [Regra de nome do arquivo](#regra-de-nome-do-arquivo)
- [Códigos de saída](#códigos-de-saída)
- [Log de erros](#log-de-erros)
- [Modo silencioso (`--quiet`)](#modo-silencioso---quiet)
- [Integração com a Senior (LSP/4GL)](#integração-com-a-senior-lsp4gl)
- [Estrutura do projeto](#estrutura-do-projeto)

## Como funciona

Dado um número de empresa, tipo de colaborador, número de cadastro, uma pasta base e uma URL, o utilitário:

1. Cria a pasta `numemp-tipcol-numcad` dentro da pasta base (`--local`), incluindo subpastas intermediárias se necessário.
2. Baixa o arquivo do link informado via HTTP(S).
3. Salva o arquivo dentro dessa pasta, com o nome definido por `--nomdoc` ou, na ausência dele, derivado automaticamente da URL.
4. Registra qualquer erro ocorrido em um arquivo de log ao lado do `.exe`.
5. Retorna um código de saída (exit code) indicando sucesso ou o tipo de falha.

O caminho final do arquivo é **totalmente previsível** a partir dos parâmetros de entrada — não é necessário capturar a saída padrão do processo para descobrir onde o arquivo foi salvo:

```
{local}\{numemp}-{tipcol}-{numcad}\{nomdoc}
```

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download) para compilar o projeto.
- Para executar o `.exe` publicado (framework-dependent): [.NET 10 Runtime](https://dotnet.microsoft.com/download) instalado na máquina de destino.

## Build e publicação

Compilar em modo Debug (para desenvolvimento/testes):

```bash
dotnet build
```

Publicar o executável final (Release, single-file, para Windows x64):

```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

O executável é gerado em:

```
DownloadImagemAtestado\bin\Release\net10.0\win-x64\publish\DownloadImagemAtestado.exe
```

> Esse build é *framework-dependent* (menor, mas exige o .NET 10 Runtime instalado na máquina que vai executar). Para gerar um `.exe` autocontido (maior, sem exigir runtime instalado), adicione `--self-contained true`.

## Uso

```bash
DownloadImagemAtestado.exe --numemp <valor> --tipcol <valor> --numcad <valor> --local <pasta> --link <url> [--nomdoc <nome_arquivo>] [--quiet]
```

Exemplo:

```bash
DownloadImagemAtestado.exe --numemp 1 --tipcol 1 --numcad 12345 --local "C:\Atestados" --link "https://s3.amazonaws.com/atestado.app/condor.ind.br/attachments/bc94c05e4e69913ec86dc23825c1b614-17873191746113615917289379782305.jpg" --nomdoc "atestado.jpg"
```

Resultado: o arquivo é salvo em `C:\Atestados\1-1-12345\atestado.jpg`.

Os parâmetros podem ser passados como `--chave valor` ou `--chave=valor`. A ordem não importa.

## Parâmetros

| Parâmetro | Obrigatório | Descrição |
|---|---|---|
| `--numemp` | Sim | Número da empresa |
| `--tipcol` | Sim | Tipo de colaborador |
| `--numcad` | Sim | Número de cadastro (matrícula) |
| `--local` | Sim | Pasta base onde a pasta `numemp-tipcol-numcad` será criada |
| `--link` | Sim | URL (http/https) do arquivo a ser baixado |
| `--nomdoc` | Não | Nome do arquivo a ser salvo. Se omitido, é derivado automaticamente do link |
| `-q`, `--quiet` | Não | Não escreve nada no console (stdout/stderr). Erros continuam sendo gravados no log |
| `-h`, `--help` | Não | Exibe a ajuda e encerra |

Se nenhum parâmetro for informado, ou se algum obrigatório estiver faltando, ou se `--link` não for uma URL http(s) válida, o programa exibe a ajuda no console (a menos que `--quiet` esteja presente) e encerra com código de saída `1`.

## Regra de nome da pasta

A pasta de destino segue exatamente a mesma convenção usada pela rotina Senior (`vaDirCol = vaNumEmp + "-" + vaTipCol + "-" + vaNumCad`):

```
{numemp}-{tipcol}-{numcad}
```

Caracteres inválidos para nomes de pasta no Windows são substituídos por `_`.

## Regra de nome do arquivo

- **Com `--nomdoc`**: o valor informado é usado literalmente como nome do arquivo salvo.
- **Sem `--nomdoc`**: o nome é derivado da URL replicando a regra usada pela Senior (`PosicaoAlfa("/attachments/", ...)` + `CopiarAlfa(...)`), porém mantendo apenas o hash inicial do nome do anexo (a parte antes do primeiro `-`) e sua extensão original.

  Exemplo — para o link:

  ```
  https://s3.amazonaws.com/atestado.app/condor.ind.br/attachments/bc94c05e4e69913ec86dc23825c1b614-17873191746113615917289379782305.jpg
  ```

  o arquivo é salvo como `bc94c05e4e69913ec86dc23825c1b614.jpg`.

  Se a URL não contiver `/attachments/`, o nome cai para o último segmento do caminho da URL como alternativa.

## Códigos de saída

| Código | Situação |
|---|---|
| `0` | Sucesso (ou `--help` exibido) |
| `1` | Erro de parâmetros/uso (parâmetro obrigatório ausente, URL inválida, argumento mal formado) |
| `2` | Erro ao criar a pasta de destino (permissão, caminho inválido, etc.) |
| `3` | Erro ao baixar o arquivo (host inexistente, timeout, erro HTTP, etc.) |

## Log de erros

Como o processo costuma ser chamado via `ExecProg` (que normalmente não captura a saída padrão do programa), todo erro é também gravado em um arquivo de log **na mesma pasta do `.exe`**:

```
DownloadImagemAtestado.log
```

Cada linha contém data/hora, o tipo de erro e o contexto (`numemp`, `tipcol`, `numcad`, `link`), por exemplo:

```
2026-08-26 11:51:32 [ERRO] Falha ao baixar o arquivo (numemp=1 tipcol=1 numcad=1 link=https://...): Este host não é conhecido. (host-invalido:443)
```

O arquivo é criado automaticamente no primeiro erro e as linhas são *appendadas* nas execuções seguintes (não é rotacionado nem limpo automaticamente).

## Modo silencioso (`--quiet`)

Ao passar `--quiet` (ou `-q`), nenhuma mensagem é escrita no console — nem de sucesso, nem de erro, nem a ajuda. O código de saída e o log de erros continuam funcionando normalmente. Use esse modo em chamadas automatizadas (como a partir da Senior) onde a saída de console não é lida por ninguém.

## Integração com a Senior (LSP/4GL)

Chamada equivalente à rotina atual de download de anexos, via `ExecProg`:

```
vaCamExe = "C:\Utilitarios\DownloadImagemAtestado.exe";
vaParametros = "--numemp " + vaNumEmp +
               " --tipcol " + vaTipCol +
               " --numcad " + vaNumCad +
               " --local \"" + vaArqLoc + "\"" +
               " --link \"" + vaUrlAnx + "\"" +
               " --quiet";

ExecProg(vaCamExe, vaParametros, 1);
```

Como o caminho final do arquivo é previsível (`{local}\{numemp}-{tipcol}-{numcad}\{nomdoc ou hash+extensao}`), a rotina Senior pode montá-lo sem depender da saída do processo. Em caso de falha, consulte `DownloadImagemAtestado.log` (ao lado do `.exe`) para o motivo.

## Estrutura do projeto

```
DownloadImagemAtestado/
├── Program.cs                              # Orquestração: parse -> cria pasta -> baixa -> trata erros
├── app.ico                                 # Ícone do executável
├── Cli/
│   ├── CommandLineOptions.cs                # Parâmetros já validados
│   ├── CommandLineParser.cs                 # Parsing e validação dos argumentos
│   ├── ParseResult.cs / ParseStatus.cs      # Resultado tipado do parsing (sucesso/ajuda/erro)
│   ├── HelpText.cs                          # Texto de uso/ajuda
│   └── ExitCode.cs                          # Constantes de código de saída
└── Services/
    ├── DestinationFolderBuilder.cs          # Cria a pasta numemp-tipcol-numcad
    ├── AttachmentDownloader.cs              # Baixa o arquivo via HTTP
    ├── AttachmentFileNameResolver.cs        # Deriva o nome do arquivo a partir do link
    └── FileErrorLogger.cs                   # Grava erros em log ao lado do .exe
```
