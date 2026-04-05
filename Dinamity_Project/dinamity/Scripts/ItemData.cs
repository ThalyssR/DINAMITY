using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
	public enum CategoriaItem { Consumivel, Capacete, Armadura, Bota, Mao1, Mao2, Amuleto1, Amuleto2 }
	
	public enum TipoAtaque { 
		MeleeDirecional, 
		MeleeLivre,      
		Estocada,
		EstocadaLivre // ---> NOVO: Tipo de estocada que segue o mouse <---
	}

	[ExportGroup("Informações Básicas")]
	[Export] public string Nome = "Novo Item";
	[Export] public CategoriaItem Categoria = CategoriaItem.Mao1;
	[Export] public Texture2D Icone;

	[ExportGroup("Arma Guardada (Costas)")]
	[Export] public Vector2 PosicaoGuardada;
	[Export] public float RotacaoGuardada;
	[Export] public Vector2 EscalaGuardada = new Vector2(1, 1);
	
	// ---> COMPENSAÇÃO PARA O ROLL/SLIDE <---
	[Export] public float DescerArmaAoDeslizar = 15.0f; 

	[ExportGroup("Configurações de Combate (Procedural)")]
	[Export] public TipoAtaque EstiloAtaque = TipoAtaque.MeleeDirecional;
	[Export] public float TempoAtaque = 0.15f; 
	[Export] public Vector2 EscalaNaMao = new Vector2(1, 1);
	[Export] public float RotacaoSpriteBaseGraus = 90.0f; 
	
	// Valores negativos no Y puxam a espada para cima, colocando o cabo na mão do Player
	[Export] public Vector2 AjusteDoCabo = new Vector2(0, -20); 

	[Export] public float AnguloInicioCorteGraus = -60.0f; 
	[Export] public float AnguloFimCorteGraus = 60.0f; 
	[Export] public float DistanciaEstocada = 25.0f; 
	
	// ---> NOVO: Força que o personagem é jogado pra frente durante a estocada <---
	[Export] public float ForcaImpulsoEstocada = 800.0f; 

	[ExportGroup("Hitbox de Dano")]
	[Export] public Vector2 TamanhoHitbox = new Vector2(40, 40); 
	[Export] public Vector2 PosicaoHitbox = new Vector2(20, 0); 
	
	// ---> CORRIGIDO: Agora configurando o SpriteMao (Sem Textura) <---
	[ExportGroup("Configurações da Mão")]
	[Export] public Vector2 MaoEscala = new Vector2(1, 1);
	[Export] public Vector2 MaoOffset = new Vector2(0, -10); 
}
