using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
	// Adicionei Amuleto1 e Amuleto2 na lista
	public enum CategoriaItem { Consumivel, Capacete, Armadura, Bota, Mao1, Mao2, Amuleto1, Amuleto2 }

	[Export] public string Nome = "Novo Item";
	[Export] public CategoriaItem Categoria = CategoriaItem.Mao1;
	[Export] public Texture2D Icone; 

	[Export] public Vector2 PosicaoGuardada; 
	[Export] public Vector2 PosicaoAtaque;  
	[Export] public float RotacaoGuardada;
	[Export] public Vector2 EscalaGuardada = new Vector2(1, 1);
	[Export] public SpriteFrames AnimacaoAtaque;
	[Export] public Vector2 EscalaAtaque = new Vector2(1, 1);
}
