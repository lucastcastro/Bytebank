using System;
using System.Diagnostics.Tracing;
using System.Security.Cryptography;


namespace byteBank
{
    class Program
    {
        static void showMenu()
        {
            Console.WriteLine(" -------------------------------");
            Console.WriteLine(" 1 - Detalhes da conta");
            Console.WriteLine(" 2 - Ver Saldo");
            Console.WriteLine(" 3 - Operações no saldo");
            Console.WriteLine(" 4 - Manipular conta");
            Console.WriteLine(" 5 - Deletar conta");
            Console.WriteLine(" 6 - Histórico");
            Console.WriteLine(" 0 - Logout");
            Console.WriteLine();
            Console.Write(" Digite a opção desejada: ");
        }

        static int login(List<string> cpfs, List<string> senhas)
        {
            Console.Write("Informe o seu CPF: ");
            string cpf_user = Console.ReadLine();

            Console.Write("Agora nos informe sua senha: ");
            string password_user = Console.ReadLine();
            
            if(cpfs.Contains(cpf_user) && senhas.Contains(password_user))
                return cpfs.IndexOf(cpf_user);
            
            return -1;
        }

        static int createAccount(List<string> cpfs, List<string> titulares, List<string> senhas, List<double> saldos, List<string> historico)
        {
            Console.Write("Informe o seu CPF: ");
            string cpf = Console.ReadLine();

            if (!cpfs.Contains(cpf))
            {
                cpfs.Add(cpf);
                Console.WriteLine();

                Console.Write("Agora, nós informe o nome do titular: ");
                titulares.Add(Console.ReadLine());

                Console.WriteLine();

                Console.Write("Para terminar, cadastre uma senha forte: ");
                senhas.Add(Console.ReadLine());

                historico.Add("");
                saldos.Add(0);

                Console.Clear();
                Console.WriteLine("\nUsuário cadastrado com sucesso!!\n");
                return cpfs.IndexOf(cpf);
            }

            Console.Clear();
            Console.WriteLine("\nEste CPF já está registrado!!\n");
            return -1;
        }

        static void detailUser(List<string> cpfs, List<string> titulares, List<double> saldos, int indexCPFLogado)
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Nome do titular do cartão: {titulares[indexCPFLogado]}");
            Console.WriteLine($"CPF do titular do cartão: {cpfs[indexCPFLogado]}");
            Console.WriteLine($"Saldo em conta: {saldos[indexCPFLogado]}");

            Console.Write("\nPressione enter para voltar ao menu...");
            Console.ReadLine();
            Console.Clear();
        }

        static void showBalance(List<double> saldos, int indexCPFLogado)
        {
            Console.WriteLine($"Saldo: {saldos[indexCPFLogado]}");
        }

        static void manipulateAccount(List<string> cpfs, List<string> titulares, List<double> saldos, int indexCpfLogado, List<string> historico)
        {
            int option;

            do
            {
                Console.WriteLine();

                Console.WriteLine(" 1 - Sacar");
                Console.WriteLine(" 2 - Depositar");
                Console.WriteLine(" 3 - Transferir");
                Console.WriteLine(" 0 - Sair");
                Console.Write("\n Informe a opção desejada: ");

                option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        Console.Clear();

                        Console.Write("Informe o valor a ser sacado: ");
                        double valorSaque = double.Parse(Console.ReadLine());

                        if (sacar(saldos, indexCpfLogado, valorSaque, historico))
                            Console.WriteLine("\nSaque efetuado com sucesso!");
                        else
                            Console.WriteLine("\nSaldo insuficiente!");

                        break;
                    case 2:
                        Console.Clear();
                        Console.Write("Informe o valor do depósito: ");
                        double valorDeposito = double.Parse(Console.ReadLine());

                        depositar(saldos, indexCpfLogado, valorDeposito, historico);
                        Console.WriteLine("\nDepósito efetuado com sucesso!");

                        break;
                    case 3:
                        Console.Clear();
                        transferencia(saldos, titulares, cpfs, indexCpfLogado, historico);
                        break;
                }

            } while (option != 0);

            Console.Clear();
        }

        static bool sacar(List<double> saldos, int cpfIndex, double valorSaque, List<string> historico)
        {
            if (valorSaque <= saldos[cpfIndex])
            {
                saldos[cpfIndex] -= valorSaque;
                cadastrarTransacao($"Saque no valor de R$ {valorSaque:N2} efetuado com sucesso\n", historico, cpfIndex);
                return true;
            }
            cadastrarTransacao($"Saque no valor de R$ {valorSaque:N2} cancelado por falta de limite\n", historico, cpfIndex);

            return false;
        }

        static bool depositar(List<double> saldos, int cpfIndex, double valorDeposito, List<string> historico)
        {
            saldos[cpfIndex] += valorDeposito;
            cadastrarTransacao($"Depósito no valor de R$ {valorDeposito:N2} efetuado com sucesso!\n", historico, cpfIndex);
            return true;
        }

        static void transferencia(List<double> saldos, List<string> titulares, List<string> cpfs, int indexCpfOrigem, List<string> historico)
        {
            Console.Write("Informe o valor da transferência: ");
            double valorTransferencia = double.Parse(Console.ReadLine());

            Console.Write("Agora informe o CPF de destino: ");
            string cpfDestino = Console.ReadLine();

            int indexCpfDestino = cpfs.IndexOf(cpfDestino);

            if (indexCpfDestino != -1)
            {
                if (valorTransferencia <= saldos[indexCpfOrigem])
                {
                    saldos[indexCpfOrigem] -= valorTransferencia;
                    cadastrarTransacao($"Transferência para o titular {titulares[indexCpfOrigem]} no valor de R$ {valorTransferencia:N2} efetuado com sucesso\n", historico, indexCpfOrigem);
                    
                    saldos[indexCpfDestino] += valorTransferencia;
                    cadastrarTransacao($"Transferência no valor de R$ {valorTransferencia:N2} recebido do titular: {titulares[indexCpfDestino]}\n", historico, indexCpfDestino);
                }
                else
                {
                    cadastrarTransacao($"Erro na transferência, o valor R$ {valorTransferencia:N2} excede o limite do saldo\n", historico, indexCpfOrigem);
                }
            }
            else
            {
                Console.WriteLine("\nCPF de destino não encontrado!!");
            }
        }

        static void updateAccount(List<string> cpfs, List<string> titulares, List<string> senhas, int indexCpfLogado, List<string> historico)
        {
            int option;

            do {
                Console.WriteLine(" 1 - Alterar nome\n 2 - Alterar senha\n 0 - Voltar ao menu principal");
                Console.Write("\n Informe uma das opções acima: ");

                option = int.Parse(Console.ReadLine());

                switch (option) {
                    case 1:
                        updateName(titulares, indexCpfLogado, historico);
                        break;
                    case 2:
                        updatePassword(senhas, indexCpfLogado, historico);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Opção incorreta!!\n");
                        break;
                }
            } while(option != 0);

            Console.Clear();
        }

        static void updateName(List<string> titulares, int cpfIndex, List<string> historico)
        {
            Console.Clear();
            Console.Write("Informe o novo nome: ");
            string newHolder = Console.ReadLine();

            titulares[cpfIndex] = newHolder;
            cadastrarTransacao($"Nome alterado para {newHolder}\n", historico, cpfIndex);

            Console.WriteLine("\nNome Alterado com sucesso!!\n");
        }

        static void updatePassword(List<string> senhas, int cpfIndex, List<string> historico)
        {
            Console.Clear();
            Console.Write("Informe a nova senha: ");
            string newPassword = Console.ReadLine();

            senhas[cpfIndex] = newPassword;
            cadastrarTransacao($"Senha alterada com sucesso\n", historico, cpfIndex);

            Console.WriteLine("\nSenha Alterada com sucesso!!\n");
        }

        static void deleteUser(List<string> cpfs, List<string> titulares, List<string> senhas, List<double> saldos, int indexCpfLogado)
        {
            cpfs.RemoveAt(indexCpfLogado);
            titulares.RemoveAt(indexCpfLogado);
            senhas.RemoveAt(indexCpfLogado);
            saldos.RemoveAt(indexCpfLogado);

            Console.Clear();
            Console.WriteLine("Conta deletada com sucesso!!\n");
        }

        static void cadastrarTransacao(string transacao, List<string> historico, int cpfIndex)
        {
            historico[cpfIndex] += transacao;
        }

        static void Main()
        {
            List<string> cpfs = new();
            List<string> titulares = new();
            List<string> senhas = new();
            List<double> saldos = new();
            List<string> historico = new();

            while (true)
            {
                int optionWelcome;

                int indexCpfLogado;

                Console.WriteLine("Olá, Bem vindo ao byteBank\n");

                while (true)
                {
                    Console.WriteLine("1 - Logar\n2 - Criar conta\n");
                    Console.Write("Informe um opção: ");

                    optionWelcome = int.Parse(Console.ReadLine());

                    if (optionWelcome != 1 && optionWelcome != 2)
                    {
                        Console.Clear();
                        Console.WriteLine("Opção não encontrada, por favor digite uma opção válida!\n");
                    }

                    if (optionWelcome == 1)
                    {
                        Console.Clear();
                        indexCpfLogado = login(cpfs, senhas);

                        Console.WriteLine();

                        if (indexCpfLogado != -1)
                        {
                            Console.Clear();
                            Console.WriteLine($"Olá {titulares[indexCpfLogado]}, seja bem vindo!\n");
                            cadastrarTransacao("Usuário logado com sucesso\n", historico, indexCpfLogado);
                            break;
                        }

                        Console.WriteLine("CPF ou senha incorretos!\n");
                    }

                    if (optionWelcome == 2)
                    {
                        Console.Clear();
                        indexCpfLogado = createAccount(cpfs, titulares, senhas, saldos, historico);
                        cadastrarTransacao("Conta criada com sucesso\n", historico, indexCpfLogado);
                        break;
                    }
                }

                int option;

                do
                {
                    showMenu();
                    option = int.Parse(Console.ReadLine());

                    Console.Clear();

                    switch (option)
                    {
                        case 0:
                            Console.WriteLine("Saindo da conta, Bye Bye!!\n");
                            cadastrarTransacao("Saindo da conta\n", historico, indexCpfLogado);
                            break;
                        case 1:
                            detailUser(cpfs, titulares, saldos, indexCpfLogado);
                            break;
                        case 2:
                            showBalance(saldos, indexCpfLogado);
                            break;
                        case 3:
                            manipulateAccount(cpfs, titulares, saldos, indexCpfLogado, historico);
                            break;
                        case 4:
                            updateAccount(cpfs, titulares, senhas, indexCpfLogado, historico);
                            break;
                        case 5:
                            deleteUser(cpfs, titulares, senhas, saldos, indexCpfLogado);
                            option = 0;
                            break;
                        case 6:
                            Console.Clear();
                            Console.WriteLine(historico[indexCpfLogado]);
                            Console.Write("\nAperte qualquer tecla para ir ao menu...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        default:
                            Console.WriteLine("Opção inválida\n");
                            break;
                    }
                } while (option != 0);
            }
        }
    }
}
