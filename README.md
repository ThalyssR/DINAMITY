# 🗡️ Projeto Dimanity

Bem-vindo ao repositório oficial do **Dimanity**, um jogo de plataforma e ação 2D focado em combate preciso e mecânicas responsivas, desenvolvido na **Godot Engine 4** utilizando **C#**.

Este projeto está em desenvolvimento ativo e possui foco em entregar um "game feel" polido, inspirado em clássicos modernos do gênero *Metroidvania* e *Action-Platformer*.

---

## 🎮 Sobre o Jogo

Em **Dimanity**, você controla um guerreiro ágil capaz de dominar o cenário e engajar em combates viscerais. O jogo conta com um sistema de física avançado que permite grande controle aéreo, esquivas (dash/roll) e um sistema de armas modular e expansível.

### ✨ Características Principais

*   **Combate Responsivo:** Hitboxes geradas e processadas com precisão em C#. Diferentes tipos de armas (direcionais, estocadas livres) configuráveis via script.
*   **Movimentação Avançada:**
    *   Pulo Duplo e Pulo de Parede (Wall Jump).
    *   Deslizar em Paredes (Wall Slide).
    *   Dash aéreo e Rolamento no chão (Roll) com i-frames/cooldowns independentes.
    *   Ataque descendente (Slam Attack) para finalizações e movimentação rápida.
    *   Escalada automática de quinas (Ledge Climb) com interpolação suave.
*   **"Soft Collision" (Repulsão Suave):** Sistema de colisão inspirado em *Hollow Knight* e *Dead Cells*. Inimigos possuem uma massa física que repele o jogador suavemente, impedindo a "Síndrome do T-Rex" sem travar o movimento do jogador (permite atravessar inimigos escorregando pela hitbox deles).
*   **Sistemas Modulares:**
    *   **Criador de Armas (`ItemData.cs`):** Arquitetura baseada em `Resource` que permite criar novas espadas e equipamentos rapidamente apenas ajustando parâmetros no Inspector (Tamanho da Hitbox, Ângulos de Corte, Velocidade de Ataque).
    *   **Inimigos Independentes (`Enemy.cs`):** IA estruturada em Máquina de Estados (State Machine) configurável. Cada monstro pode ter sua visão 360º, raycasts de ataque customizados e lógica de patrulha ajustada diretamente pela interface do Editor.

---

## 🛠️ Tecnologias e Arquitetura

O projeto foi construído priorizando a performance e a organização de código (Clean Code):

*   **Engine:** Godot Engine 4.x
*   **Linguagem:** C# (.NET)
*   **Física:** Godot Physics 2D (Modificada com vetores e RayCasts customizados).
*   **Arquitetura:** Componentização de comportamentos, herança e uso intenso de `ExportGroups` para facilitar o Level Design diretamente no Editor, sem necessidade de hardcoding.

---

## 📁 Estrutura do Código (Destaques)

*   **`Player.cs`:** O núcleo de movimentação e status do protagonista. Controla o processamento da física, animações, sistema de quinas e troca/leitura das armas equipadas.
*   **`ItemData.cs`:** Classe Global (`[GlobalClass]`) de Resource. O cérebro por trás de todo item equipado. Define desde a textura até os vetores exatos da área de corte de uma arma.
*   **`Enemy.cs`:** Classe base para inimigos. Possui sistema de patrulha autônoma, detecção de parede (via RayCast), sistema de gravidade e knockback de dano, além da IA de perseguição e ataque.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
*   **Godot Engine 4 com suporte a .NET (C#)** instalado.
*   SDK do .NET Core atualizado.

### Passos
1.  Clone este repositório:
    ```bash
    git clone [https://github.com/SEU_USUARIO/dimanity.git](https://github.com/SEU_USUARIO/dimanity.git)
    ```
2.  Abra o Godot Engine, clique em **Import** e selecione o arquivo `project.godot` na pasta do repositório.
3.  No topo do editor, clique em **Build** (ou `Ctrl+B`) para compilar os scripts C# do projeto.
4.  Abra a cena principal e clique em **Play** (F5).

---

## 🗺️ Roadmap de Desenvolvimento

- [x] Movimentação Básica (Pulo, Movimento, Dash).
- [x] Interação com Cenário (Wall Jump, Ledge Climb).
- [x] Sistema Modular de Armas (`Resource` baseado).
- [x] Criação da IA Base do Inimigo (Patrulha, Visão, Soft Collision).
- [ ] Implementação de Sistema de UI e Vida (Health Bar).
- [ ] Criação de Diferentes Classes de Inimigos (Voadores, Atiradores).
- [ ] Sistema de Inventário.

---

## 📄 Licença

Este projeto é de código aberto. Sinta-se livre para usar as lógicas de movimentação e C# como estudo ou base para seus próprios jogos na Godot 4!

*Desenvolvido com ☕ e Godot.*
