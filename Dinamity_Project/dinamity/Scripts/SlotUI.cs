using Godot;
using System;

public partial class SlotUI : TextureRect
{
	// Agora a lista de slots bate certinho com a do ItemData
	public enum TipoDeSlot { Mochila = -1, Capacete = 1, Armadura = 2, Bota = 3, Mao1 = 4, Mao2 = 5, Amuleto1 = 6, Amuleto2 = 7 }
	
	[Export] public TipoDeSlot SlotMapeado = TipoDeSlot.Mochila;
	[Export] public ItemData ItemAtual;

	private Texture2D _texturaOriginalVazia;

	public override void _Ready()
	{
		_texturaOriginalVazia = Texture;
		AddToGroup("SlotsInventario");
		AtualizarVisual();
	}

	public void AtualizarVisual()
	{
		if (ItemAtual != null && ItemAtual.Icone != null)
		{
			Texture = ItemAtual.Icone;
		}
		else
		{
			Texture = _texturaOriginalVazia;
		}
	}

	public bool AceitaItem(ItemData itemTestado)
	{
		if (itemTestado == null) return false;
		if (SlotMapeado == TipoDeSlot.Mochila) return true;

		// Verifica se a categoria do item é a mesma do slot
		return (int)SlotMapeado == (int)itemTestado.Categoria;
	}
}
