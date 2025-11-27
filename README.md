Tutorial de Utilização do Sistema
1. Inicializando o Projeto

Entre na pasta raiz do projeto, onde está localizado o docker-compose.yml.

Execute o comando para buildar os containers:

docker-compose build


Após a finalização do build, inicie os serviços com:

docker-compose up


Observação: durante a inicialização, alguns erros podem aparecer. Isso acontece porque o job depende do RabbitMQ para funcionar. Assim que todos os serviços forem inicializados, esses erros desaparecerão.

2. Acessando o Frontend

Abra o navegador e acesse:

http://localhost:4200/login


Existe um usuário Gerente já criado:

Email: admin@gmail.com
Senha: Desafio2025!

3. Funcionalidades do Usuário Gerente

Na aba Admin, o gerente pode:

Realizar CRUD de Produtos.

Listar todas as compras realizadas no sistema.

Cancelar compras quando necessário.

Gerar relatórios de vendas e produtos.

Além disso, o gerente também pode:

Comprar produtos na vitrine.

Visualizar suas próprias compras na aba Usuario / Compras.

4. Funcionalidades do Usuário Comum

Caso queira testar como um usuário sem privilégios de gerente:

Acesse a página de registro:

http://localhost:4200/register


ou clique em Criar conta na tela de login.

Usuários comuns podem:

Comprar produtos na vitrine.

Visualizar suas próprias compras na aba Usuario / Compras.

Observação: usuários comuns não têm acesso à aba Admin nem às funcionalidades de gerência.

Caso queira usar a Documentação Swagger do backend pode utilizar https://localhost:8081/swagger/index.html ou http://localhost:8080/swagger/index.html
