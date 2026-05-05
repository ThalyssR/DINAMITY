#  Projeto Dimanity

Bem-vindo ao repositório de apresentação do **Dimanity**, um jogo de plataforma e ação 2D focado em combate preciso e mecânicas responsivas, desenvolvido na **Godot Engine 4** utilizando **C#**.

Atualmente em desenvolvimento ativo, o projeto busca entregar um "game feel" polido, inspirado em clássicos modernos do gênero *Metroidvania* e *Action-Platformer*.

---

<img width="2730" height="1536" alt="Promo2" src="https://github.com/user-attachments/assets/07d8a919-2371-4545-baf4-d7e57a6b58ce" />

---

## 🎮 Sobre o Jogo

Em **Dimanity**, você controla um guerreiro ágil capaz de dominar o cenário e engajar em combates viscerais. O jogo conta com um sistema de física avançado que permite grande controle aéreo, esquivas e um sistema de armas modular.

### ✨ Características Principais

*   **Combate Responsivo:** Hitboxes geradas e processadas com precisão em C#. Diferentes tipos de armas (ataques direcionais, estocadas) configuráveis dinamicamente.
*   **Movimentação Avançada:**
    *   Pulo Duplo e Pulo de Parede (Wall Jump).
    *   Deslizar em Paredes (Wall Slide).
    *   Dash aéreo e Rolamento no chão (Roll) com cooldowns independentes.
    *   Ataque descendente (Slam Attack) para finalizações e movimentação rápida.
    *   Escalada automática de quinas (Ledge Climb) com interpolação suave.
*   **"Soft Collision" (Repulsão Suave):** Sistema de colisão inspirado em *Hollow Knight* e *Dead Cells*. Inimigos possuem uma massa física que repele o jogador suavemente, impedindo travamentos e permitindo uma fluidez maior nos combates corpo a corpo.
*   **Sistemas Modulares (Desenvolvimento):**
    *   **Criador de Armas:** Arquitetura baseada em `Resource` que permite criar novas espadas e equipamentos rapidamente apenas ajustando parâmetros (Tamanho da Hitbox, Ângulos de Corte, Velocidade).
    *   **IA de Inimigos:** Inteligência Artificial estruturada em Máquina de Estados (State Machine). Cada monstro possui visão adaptável, raycasts de ataque customizados e lógica de patrulha ajustada diretamente pela interface.

---

## 🛠️ Tecnologias e Arquitetura

O projeto foi construído priorizando a performance e a organização do código (Clean Code), servindo também como um laboratório de sistemas de jogos complexos:

*   **Engine:** Godot Engine 4.x
*   **Linguagem principal:** C# (.NET)
*   **Física:** Godot Physics 2D (Modificada com vetores e RayCasts customizados para maior precisão em jogos de plataforma).

<img width="2390" height="1792" alt="promo1" src="https://github.com/user-attachments/assets/7f4055f8-982a-490c-be6f-85debe36e24c" />


## 🚀 Status do Projeto e Download

O jogo atualmente é de **código fechado** e está em fase de produção. 

Uma versão jogável (Build) estará disponível para download no futuro para testes e feedback. Fique de olho neste repositório para atualizações e links de download das futuras versões Alpha/Beta!

---

## 🗺️ Roadmap de Desenvolvimento

- [x] Movimentação Básica (Pulo, Movimento, Dash).
- [x] Interação com Cenário (Wall Jump, Ledge Climb).
- [x] Sistema Modular de Armas (`Resource` baseado).
- [x] Criação da IA Base dos Inimigos (Patrulha, Visão, Soft Collision).
- [ ] Implementação de Sistema de UI e Vida (Health Bar).
- [ ] Criação de Diferentes Classes de Inimigos (Voadores, Atiradores).
- [ ] Sistema de Inventário.

---

*© 2026 Dimanity. Todos os direitos reservados.*
