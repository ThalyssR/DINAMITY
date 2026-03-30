using Godot;
using System;

public partial class MenuPrincipal : Control
{
	private Button btnJogar;
	private Button btnSair;

	public override void _Ready()
	{
		// Encontra os botões que criamos na tela
		btnJogar = GetNode<Button>("VBoxContainer/BtnJogar");
		btnSair = GetNode<Button>("VBoxContainer/BtnSair");

		// Conecta os cliques aos comandos
		btnJogar.Pressed += AoClicarJogar;
		btnSair.Pressed += AoClicarSair;
	}

	private void AoClicarJogar()
	{
		// Troca a palavra "mundo.tscn" pelo nome exato da cena onde você desenhou o mapa!
		GetTree().ChangeSceneToFile("res://Scenes/mundo.tscn"); 
	}

	private void AoClicarSair()
	{
		// Fecha o jogo
		GetTree().Quit();
	}
}
