using Godot;
using System;

public partial class MenuInventario : Control
{
	[Export] public ItemData FoiceDeTeste; 
	[Export] public SlotUI PrimeiroSlotDaMochila; 

	private ColorRect _cursorBolinha;
	private TextureRect _iconeArrastado;
	private CpuParticles2D _particulasArrasto; 
	private ItemData _itemSegurado = null;
	private SlotUI _slotDeOrigem = null; 

	public const float VelocidadeCursor = 900.0f; 
	private bool _usandoControle = false;

	public override void _Ready()
	{
		// 1. Esconde IMEDIATAMENTE.
		Visible = false; 

		// 2. Chama a configuração do item com atraso de 1 frame (CallDeferred)
		// Isso evita que o jogo quebre se os Slots ainda não tiverem carregado no Godot.
		Callable.From(ConfigurarItemTeste).CallDeferred();

		// --- CONFIGURAÇÃO DA BOLINHA BRANCA ---
		_cursorBolinha = new ColorRect
		{
			Color = new Color(1, 1, 1, 1),
			CustomMinimumSize = new Vector2(10, 10),
			Size = new Vector2(10, 10),
			ZIndex = 100,
			Visible = false,
			MouseFilter = MouseFilterEnum.Ignore // Para não bloquear os slots
		};
		AddChild(_cursorBolinha);

		// --- CONFIGURAÇÃO DO ÍCONE ARRASTADO ---
		_iconeArrastado = new TextureRect
		{
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			CustomMinimumSize = new Vector2(40, 40),
			Size = new Vector2(40, 40),
			Visible = false,
			ZIndex = 99,
			MouseFilter = MouseFilterEnum.Ignore 
		};
		AddChild(_iconeArrastado);

		// --- CONFIGURAÇÃO DAS PARTÍCULAS ---
		_particulasArrasto = new CpuParticles2D
		{
			Emitting = false,
			Amount = 15, 
			Lifetime = 0.5f,
			EmissionShape = CpuParticles2D.EmissionShapeEnum.Sphere,
			EmissionSphereRadius = 15f,
			Gravity = new Vector2(0, -50), 
			Color = new Color(0.8f, 0.8f, 1f, 0.7f), 
			ZIndex = 98
		};
		AddChild(_particulasArrasto);
	}

	private void ConfigurarItemTeste()
	{
		if (PrimeiroSlotDaMochila != null && FoiceDeTeste != null)
		{
			PrimeiroSlotDaMochila.ItemAtual = FoiceDeTeste;
			PrimeiroSlotDaMochila.AtualizarVisual();
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Detecta troca para MOUSE
		if (@event is InputEventMouseMotion || @event is InputEventMouseButton)
		{
			if (_usandoControle)
			{
				_usandoControle = false;
				_cursorBolinha.Visible = false;
				if (Visible) Input.MouseMode = Input.MouseModeEnum.Visible;
			}

			// Lógica de segurar/soltar do Mouse - SÓ RODA SE TIVER ABERTO
			if (Visible && @event is InputEventMouseButton mouseBtn && mouseBtn.ButtonIndex == MouseButton.Left)
			{
				if (mouseBtn.Pressed)
					ProcessarInteracao(GetGlobalMousePosition(), tentandoPegar: true);
				else
					ProcessarInteracao(GetGlobalMousePosition(), tentandoPegar: false);
			}
		}
		// Detecta troca para CONTROLE
		else if (@event is InputEventJoypadMotion joyMotion)
		{
			// Ignora toques acidentais/drift do analógico (deadzone)
			if (Mathf.Abs(joyMotion.AxisValue) > 0.2f && !_usandoControle)
				AlternarParaControle();
		}
		else if (@event is InputEventJoypadButton && !_usandoControle)
		{
			AlternarParaControle();
		}
	}

	private void AlternarParaControle()
	{
		_usandoControle = true;
		if (Visible)
		{
			_cursorBolinha.Visible = true;
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("inventario"))
		{
			Visible = !Visible;
			GetTree().CallGroup("Player", "SetInMenu", Visible); 
			
			if (Visible)
			{
				// Abrindo inventário
				_cursorBolinha.Position = Size / 2;
				if (_usandoControle)
				{
					_cursorBolinha.Visible = true;
					Input.MouseMode = Input.MouseModeEnum.Hidden;
				}
				else
				{
					_cursorBolinha.Visible = false;
					Input.MouseMode = Input.MouseModeEnum.Visible;
				}
			}
			else
			{
				// Fechando inventário
				if (_itemSegurado != null) ProcessarInteracao(Vector2.Zero, false, true);
				
				_cursorBolinha.Visible = false;
				
				// Atenção: Deixei Visible aqui para você não perder o cursor de novo. 
				// Se o seu jogo for estilo FPS e precisar esconder o mouse na gameplay, troque para Captured.
				Input.MouseMode = Input.MouseModeEnum.Visible; 
			}
		}

		if (!Visible) return;

		// MOVIMENTO DO CURSOR PELO CONTROLE
		Vector2 direcaoAnalogico = new Vector2(
			Input.GetJoyAxis(0, JoyAxis.LeftX), 
			Input.GetJoyAxis(0, JoyAxis.LeftY)  
		);

		if (direcaoAnalogico.Length() > 0.2f)
		{
			_cursorBolinha.Position += direcaoAnalogico * VelocidadeCursor * (float)delta;
			AlternarParaControle(); // Garante que a UI saiba que estamos no controle
		}

		_cursorBolinha.Position = new Vector2(
			Mathf.Clamp(_cursorBolinha.Position.X, 0, Size.X - 10),
			Mathf.Clamp(_cursorBolinha.Position.Y, 0, Size.Y - 10)
		);

		// ATUALIZA A POSIÇÃO DO ITEM ARRASTADO (GlobalPosition evita bugs de UI)
		if (_itemSegurado != null)
		{
			Vector2 posicaoAlvo = _usandoControle ? _cursorBolinha.GlobalPosition : GetGlobalMousePosition();
			_iconeArrastado.GlobalPosition = posicaoAlvo + new Vector2(5, 5);
			_particulasArrasto.GlobalPosition = _iconeArrastado.GlobalPosition + (_iconeArrastado.Size / 2);
			_particulasArrasto.Emitting = true; 
		}
		else
		{
			_particulasArrasto.Emitting = false; 
		}

		// CONTROLE: PEGAR/SOLTAR COM O X
		if (Input.IsActionJustPressed("pulo") && _usandoControle)
		{
			bool pegar = (_itemSegurado == null);
			ProcessarInteracao(_cursorBolinha.GlobalPosition, pegar);
		}
	}

	private SlotUI EncontrarSlotNaPosicao(Vector2 posicaoGlobal)
	{
		var todosOsSlots = GetTree().GetNodesInGroup("SlotsInventario");
		foreach (SlotUI slot in todosOsSlots)
		{
			if (slot.GetGlobalRect().HasPoint(posicaoGlobal))
				return slot;
		}
		return null;
	}

	private void ProcessarInteracao(Vector2 posicaoDeInteracao, bool tentandoPegar, bool forcarDevolucao = false)
	{
		SlotUI slotSobCursor = EncontrarSlotNaPosicao(posicaoDeInteracao);

		if (tentandoPegar && _itemSegurado == null)
		{
			if (slotSobCursor != null && slotSobCursor.ItemAtual != null)
			{
				_itemSegurado = slotSobCursor.ItemAtual;
				_slotDeOrigem = slotSobCursor; 
				
				slotSobCursor.ItemAtual = null; 
				slotSobCursor.AtualizarVisual();
				_iconeArrastado.Texture = _itemSegurado.Icone; 
				_iconeArrastado.Visible = true;

				if (slotSobCursor.SlotMapeado == SlotUI.TipoDeSlot.Mao1)
					GetTree().CallGroup("Player", "EquiparArma", new Variant());
			}
		}
		else if (!tentandoPegar && _itemSegurado != null)
		{
			if (!forcarDevolucao && slotSobCursor != null && slotSobCursor.AceitaItem(_itemSegurado))
			{
				ItemData itemQueJaTavaLa = slotSobCursor.ItemAtual;
				slotSobCursor.ItemAtual = _itemSegurado;
				slotSobCursor.AtualizarVisual();

				if (slotSobCursor.SlotMapeado == SlotUI.TipoDeSlot.Mao1)
					GetTree().CallGroup("Player", "EquiparArma", Variant.From(_itemSegurado));

				if (itemQueJaTavaLa != null && _slotDeOrigem != null)
				{
					_slotDeOrigem.ItemAtual = itemQueJaTavaLa;
					_slotDeOrigem.AtualizarVisual();
				}
				
				_itemSegurado = null;
				_iconeArrastado.Visible = false;
			}
			else 
			{
				if (_slotDeOrigem != null)
				{
					_slotDeOrigem.ItemAtual = _itemSegurado;
					_slotDeOrigem.AtualizarVisual();
				}
				_itemSegurado = null;
				_iconeArrastado.Visible = false;
			}
			_slotDeOrigem = null;
		}
	}
}
