
# Rodando o Projeto (Passo a passo)

* Executar a solução OnionApp, estará rodando na porta **5000**.
* O projeto também pode ser executado usando o comando <code> dotnet run</code> dentro da pasta **API**, ou também pressionando F5.
  
* Entrar dentro da pasta **view** e rodar o comando <code> npm i </code>
  para instalar todas dependencias necessarias.

* Entrar dentro de **view** e rodar o comando <code> npm start </code> para dar inicio ao projeto front-end, que estará rodando na porta **3000**.
  
#### Rotas back-end
    * http://localhost:5000/ImportOrder (POST)
    * http://localhost:5000/OrderHandler (POST)
    * http://localhost:5000/ApprovedOrders (POST)
    * http://localhost:5000/GetOrders (GET)

    * http://localhost:5000/swagger/index.html

# Caso de Uso

1 - Escolher o arquivo desejado que tenha as mesmas colunas como o arquivo de exemplo enviado anteriormente.

2 - Fazer o Upload do arquivo, no botão "Upload".

3 - Calcular os pedidos, sendo a data de entrega e os precos por pedido com e sem frete.

4 - Após seguir esses passos, ao clicar na aba superior "ORDERS" deverá ser possível visualizar os pedidos processados, mas ainda não aprovados pelo usuário.

5 - Assim que os pedidos forem aprovados, é salvo no banco de dados.

6 - Na aba superior "DASHBOARDS" será possivel ter uma análise de clientes por preço de produto com taxa de entrega (caso o back-end esteja rodando, pois o front-end é alimentado por um get nessa tabela de pedidos).

# Técnologias, dependencias e versões
### Back-end
* .NET 6.0
* SQLite
* Entity Framework
* EntityFrameworkCore.Design
* NewtonSoft.json
* Dependency Injection
* Automapper

### Front-end
* node (v20.5.1)
* npm (9.8.0)
* React.js
* react-apexcharts
* axios

### Observações extras

* Os dados das tabelas Product e Customer, foram salvos através de um Seed, de forma fixa, e os Pedidos, após a aprovação do usuário. Caminho até o banco: Persistence/Onion.db

Caso desejar visualizar os dados, será necessário instalar SQLite.<br>
#### Ubuntu 
<code> sudo apt update </code> <br>
<code> sudo apt install sqlite3 </code>
#### Windows
https://www.sqlitetutorial.net/download-install-sqlite/

