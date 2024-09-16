

---

# YouTube Video and Audio Downloader (Console App)

Este é um aplicativo de console em **C#** que permite o download de vídeos ou áudios do YouTube, com a opção de converter áudios para o formato MP3 usando o **FFmpeg**. O aplicativo apresenta uma barra de progresso interativa durante o download.

## Pré-requisitos

- **.NET SDK** instalado
- **FFmpeg** instalado e configurado no seu sistema (para conversão de áudio)
- Acesso à internet para baixar os vídeos/áudios

### Passos de Instalação

### 1. Instalar o .NET SDK

Se você ainda não possui o .NET SDK instalado, faça o download do SDK em:

[Download .NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet)

Siga as instruções para instalação.

### 2. Instalar o FFmpeg

O **FFmpeg** é necessário para a conversão de arquivos de áudio em formato MP3. Siga as instruções abaixo para instalar o FFmpeg no Windows.

#### 2.1. Baixar o FFmpeg

1. Acesse o site oficial do FFmpeg: [FFmpeg Downloads](https://ffmpeg.org/download.html).
2. Clique em **Windows builds from gyan.dev** (ou qualquer outra distribuição de sua preferência).
3. No site, baixe a versão "release full build" em formato zip.
   
#### 2.2. Extrair e Configurar o FFmpeg no Path

1. Após o download, extraia o arquivo zip em uma pasta de fácil acesso, como `C:\ffmpeg`.
   
2. Agora, adicione o FFmpeg ao **Path** do sistema:
   - Clique com o botão direito em **Este PC** e selecione **Propriedades**.
   - No painel da esquerda, clique em **Configurações avançadas do sistema**.
   - Na aba **Avançado**, clique em **Variáveis de Ambiente**.
   - Na seção **Variáveis do sistema**, localize a variável chamada **Path** e clique em **Editar**.
   - Clique em **Novo** e adicione o caminho da pasta onde você extraiu o FFmpeg, como `C:\ffmpeg\bin`.
   - Clique em **OK** para salvar.

#### 2.3. Verificar Instalação do FFmpeg

Abra o **Prompt de Comando** e execute:

```bash
ffmpeg -version
```

Se o FFmpeg estiver instalado corretamente, ele retornará a versão instalada e outras informações sobre o software.

### 3. Clonar o Repositório ou Baixar o Código

Você pode baixar ou clonar o código diretamente para o seu computador. Caso esteja utilizando Git, execute o seguinte comando no terminal:

```bash
git clone https://github.com/seu-repositorio/youtube-downloader.git
```

### 4. Restaurar Dependências

Certifique-se de que todas as dependências do projeto estão instaladas. No diretório do projeto, execute o seguinte comando para restaurar os pacotes necessários (incluindo o **YoutubeExplode**):

```bash
dotnet restore
```

### 5. Compilar e Executar

Para compilar e executar o aplicativo, abra o **Prompt de Comando** ou o **PowerShell** no diretório raiz do projeto e execute:

```bash
dotnet run
```

### 6. Como Usar o Aplicativo

Após executar o comando `dotnet run`, o aplicativo solicitará que você insira o link do vídeo do YouTube. Depois, você poderá escolher se deseja baixar o **vídeo** ou apenas o **áudio**:

1. Digite o link do vídeo do YouTube.
2. Escolha uma opção:
   - `1` para baixar o vídeo.
   - `2` para baixar o áudio (com conversão para MP3).

O arquivo será baixado para a pasta `Downloads` na raiz do projeto.

### Estrutura do Projeto

```
/YoutubeDownloader
    /Downloads       # Pasta onde os arquivos baixados serão salvos
    Program.cs       # Código fonte principal
    YoutubeDownloader.csproj  # Arquivo de projeto C#
```

---

## Exemplo de Uso

Aqui está um exemplo de como o aplicativo pode ser usado:

1. Execute o aplicativo com `dotnet run`.
2. Digite o link do vídeo do YouTube:
   ```
   Digite o link do vídeo do YouTube:
   https://www.youtube.com/watch?v=dQw4w9WgXcQ
   ```

3. Escolha o formato de download:
   ```
   Escolha uma opção:
   1 - Baixar vídeo
   2 - Baixar áudio (MP3)
   ```

4. Veja a barra de progresso enquanto o arquivo é baixado:
   ```
   Progresso: [===========---------------] 45%
   ```

5. O vídeo ou o áudio será salvo na pasta `Downloads` na raiz do projeto.

---

## Possíveis Erros e Soluções

1. **Erro: "FFmpeg não reconhecido como comando interno"**:
   - Certifique-se de que o caminho para o `ffmpeg\bin` foi adicionado corretamente ao **Path** do sistema.

2. **Erro: "Sistema não consegue encontrar o arquivo especificado"** ao converter áudio:
   - Verifique se o FFmpeg está corretamente instalado e acessível via linha de comando.

---

## Créditos

Este projeto usa as seguintes bibliotecas e ferramentas:

- **[YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode)** para o download de vídeos e áudios do YouTube.
- **FFmpeg** para a conversão de arquivos de áudio para o formato MP3.

---

