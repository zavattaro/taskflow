# TaskFlow - MVP

## Objetivo
Construir um sistema de gestão de tarefas onde usuários autenticados possam organizar projetos e tarefas de forma simples.

## Entidades
- User
- Project
- Task

## Funcionalidades da V1
- Cadastro de usuário
- Login com JWT
- Criação de projeto
- Listagem de projetos do usuário
- Criação de tarefa vinculada a um projeto
- Listagem de tarefas por projeto
- Alteração de status da tarefa

## Regras iniciais
- Cada usuário acessa apenas os próprios dados
- Cada projeto pertence a um único usuário
- Cada tarefa pertence a um único projeto
- Status da tarefa: Todo, Doing, Done

## Fora do escopo da V1
- Compartilhamento entre usuários
- Comentários
- Anexos
- Etiquetas
- Dashboard analítico
- Notificações
- Refresh token
- Permissões avançadas
