using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// --- STATUS BASE ---
	public const float Speed = 550.0f;
	public const float JumpVelocity = -500.0f;
	public const float DoubleJumpVelocity = JumpVelocity * (2.0f / 3.0f); 
	public const float RollSpeed = 650.0f;
	public const float DashSpeed = 700.0f;
	public const float ActionDuration = 0.3f; 
	public const float WallSlideSpeed = 100.0f; 
	public const float SlamSpeed = 1200.0f; 
	public const float ImpactDuration = 0.4f; 
	public const float WallJumpPushForce = 400.0f; 
	public const float WallJumpLockDuration = 0.15f; 
	public const float FallGravityMultiplier = 2.1f; 
	
	public const float VelocidadeAndandoAtacando = 0.4f;
	
	// ---> NOVO: COOLDOWN DO DASH/ROLL <---
	public const float DashCooldownDuration = 0.5f;

	[ExportGroup("Configurações de Quina e Escalada")]
	[Export] public float TempoEscalada = 0.20f; 
	[Export] public bool SubidaSuave = true;
	[Export] public Vector2 OffsetTeleportQuina = new Vector2(30, -70); 
	[Export] public float AfastarArmaNaEscalada = 10.0f; 
	
	private float _cooldownDescida = 0.0f; 
	private Vector2 _posicaoAlvoTeleporte; 

	// --- CONTROLE DE ESTADOS ---
	private bool isSlamming = false; 
	private bool isImpacting = false; 
	private float impactTimer = 0.0f;
	private float wallJumpTimer = 0.0f;
	private float actionTimer = 0.0f;
	private bool isRolling = false;
	private bool isDashing = false;
	private float moveDirection = 1.0f; 
	private float attackDirection = 1.0f; 
	private bool canAirDash = false; 
	private bool canWallInteract = true; 
	private bool isAttacking = false;
	private bool isHoldingWall = false; 
	
	private bool _inMenu = false;
	private bool isClimbingLedge = false;
	private int _jumpCount = 0;
	private float wallReattachTimer = 0.0f;

	// --- VARIÁVEIS DE TEMPO DE RECARGA E IMPULSO ---
	private float _impulsoTimer = 0.0f;
	private Vector2 _impulsoVelocidade = Vector2.Zero;
	private float _dashCooldownTimer = 0.0f; // ---> NOVO: Cronômetro do cooldown

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// --- REFERÊNCIAS VISUAIS E SISTEMA DE ARMAS ---
	private AnimatedSprite2D anim;
	private Sprite2D _armaNasCostas;
	private Node2D _pivotArma;
	private Sprite2D _spriteArma;
	private Sprite2D _spriteMao;
	private Camera2D _camera;
	
	private Vector2 _posicaoBasePivot; 
	
	private Area2D _areaHitbox;
	private CollisionShape2D _colisaoHitbox;
	
	private RayCast2D _rayPeito;
	private RayCast2D _rayCabeca;
	private RayCast2D _rayPlataforma; 

	[Export] public ItemData ArmaEquipada; 

	public override void _Ready()
	{
		AddToGroup("Player"); 

		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_armaNasCostas = GetNodeOrNull<Sprite2D>("ArmaNasCostas");
		
		_rayPeito = GetNode<RayCast2D>("RayPeito");
		_rayCabeca = GetNode<RayCast2D>("RayCabeca");
		_rayPlataforma = GetNodeOrNull<RayCast2D>("RayPlataforma");

		_camera = GetNodeOrNull<Camera2D>("Camera2D");

		_pivotArma = GetNode<Node2D>("PivotArma");
		_spriteArma = _pivotArma.GetNode<Sprite2D>("SpriteArma");
		
		_spriteMao = _spriteArma.GetNodeOrNull<Sprite2D>("SpriteMao");

		_areaHitbox = _pivotArma.GetNodeOrNull<Area2D>("AreaHitbox");
		_colisaoHitbox = _areaHitbox?.GetNodeOrNull<CollisionShape2D>("ColisaoHitbox");
		
		_posicaoBasePivot = _pivotArma.Position; 
		_pivotArma.Visible = false; 

		if (_areaHitbox != null)
		{
			_areaHitbox.BodyEntered += AoAcertarInimigo;
			if (_colisaoHitbox != null) _colisaoHitbox.SetDeferred("disabled", true);
		}

		EquiparArma(ArmaEquipada);
	}

	public void SetInMenu(bool estado)
	{
		_inMenu = estado;
		if (_inMenu)
		{
			isAttacking = false;
			isRolling = false;
			isDashing = false;
			isSlamming = false;
			isClimbingLedge = false;
			_impulsoTimer = 0.0f; 
		}
	}

	public void EquiparArma(ItemData novaArma)
	{
		ArmaEquipada = novaArma;
		if (ArmaEquipada != null)
		{
			if (_armaNasCostas != null)
			{
				_armaNasCostas.Texture = ArmaEquipada.Icone;
				_armaNasCostas.Position = ArmaEquipada.PosicaoGuardada;
				_armaNasCostas.RotationDegrees = ArmaEquipada.RotacaoGuardada;
				_armaNasCostas.Scale = ArmaEquipada.EscalaGuardada;
				_armaNasCostas.Visible = true;
			}
			
			if (_spriteArma != null)
			{
				_spriteArma.Texture = ArmaEquipada.Icone;
				_spriteArma.Scale = ArmaEquipada.EscalaNaMao; 
				_spriteArma.Offset = ArmaEquipada.AjusteDoCabo; 
			}

			if (_spriteMao != null)
			{
				_spriteMao.Scale = ArmaEquipada.MaoEscala;
				_spriteMao.Position = ArmaEquipada.MaoOffset;
			}

			if (_colisaoHitbox != null)
			{
				RectangleShape2D shape = new RectangleShape2D();
				shape.Size = ArmaEquipada.TamanhoHitbox;
				_colisaoHitbox.Shape = shape;
				_colisaoHitbox.Position = ArmaEquipada.PosicaoHitbox;
			}
		}
		else if (_armaNasCostas != null) _armaNasCostas.Visible = false;
	}

	private void AtualizarDirecaoRays(float direcao)
	{
		if (_rayPeito == null || _rayCabeca == null) return;

		_rayPeito.Position = new Vector2(Mathf.Abs(_rayPeito.Position.X) * direcao, _rayPeito.Position.Y);
		_rayCabeca.Position = new Vector2(Mathf.Abs(_rayCabeca.Position.X) * direcao, _rayCabeca.Position.Y);

		_rayPeito.TargetPosition = new Vector2(Mathf.Abs(_rayPeito.TargetPosition.X) * direcao, _rayPeito.TargetPosition.Y);
		_rayCabeca.TargetPosition = new Vector2(Mathf.Abs(_rayCabeca.TargetPosition.X) * direcao, _rayCabeca.TargetPosition.Y);
	}

	public override void _Process(double delta)
	{
		if (_camera != null && !_inMenu)
		{
			Vector2 mouseLocal = GetLocalMousePosition();
			Vector2 offsetAlvo = mouseLocal * 0.20f; 
			
			if (offsetAlvo.Length() > 120f) 
			{
				offsetAlvo = offsetAlvo.Normalized() * 120f;
			}
			
			_camera.Position = _camera.Position.Lerp(offsetAlvo, 5f * (float)delta);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_inMenu)
		{
			Vector2 velMenu = Velocity;
			if (!IsOnFloor()) velMenu.Y += gravity * (float)delta; 
			velMenu.X = Mathf.MoveToward(Velocity.X, 0, Speed); 
			Velocity = velMenu;
			MoveAndSlide();
			if (IsOnFloor()) anim.Play("idle");
			else anim.Play("fall");
			return; 
		}

		if (isClimbingLedge) return;

		Vector2 velocity = Velocity;
		if (wallJumpTimer > 0) wallJumpTimer -= (float)delta;
		if (_cooldownDescida > 0) _cooldownDescida -= (float)delta;
		
		// ---> NOVO: Reduz o timer do cooldown do dash a cada frame
		if (_dashCooldownTimer > 0) _dashCooldownTimer -= (float)delta;

		if (!canWallInteract)
		{
			wallReattachTimer -= (float)delta;
			if (wallReattachTimer <= 0) canWallInteract = true;
		}

		if (Input.IsActionJustPressed("ataque") && !isAttacking && !isRolling && !isSlamming && ArmaEquipada != null)
		{
			IniciarAtaque();
		}

		if (isImpacting)
		{
			impactTimer -= (float)delta;
			if (impactTimer <= 0) isImpacting = false; 
			else
			{
				Velocity = Vector2.Zero; MoveAndSlide(); UpdateAnimation(); return; 
			}
		}

		if (isSlamming)
		{
			velocity.Y = SlamSpeed; velocity.X = 0; 
			if (IsOnFloor()) { isSlamming = false; isImpacting = true; impactTimer = ImpactDuration; _impulsoTimer = 0; }
			Velocity = velocity; MoveAndSlide(); UpdateAnimation(); return;
		}

		if (isRolling || isDashing)
		{
			if (Input.IsActionJustPressed("ui_up")) { isRolling = false; isDashing = false; actionTimer = 0; }
			else
			{
				actionTimer -= (float)delta;
				if (actionTimer <= 0) { isRolling = false; isDashing = false; }
				else
				{
					velocity.X = moveDirection * (isDashing ? DashSpeed : RollSpeed);
					velocity.Y = 0; Velocity = velocity; MoveAndSlide(); UpdateAnimation(); return; 
				}
			}
		}

		float direction = Input.GetAxis("ui_left", "ui_right");
		float velocidadeAtual = isAttacking && IsOnFloor() ? Speed * VelocidadeAndandoAtacando : Speed;
		
		if (direction != 0 && wallJumpTimer <= 0) 
		{
			if (!isAttacking) 
			{
				moveDirection = direction > 0 ? 1.0f : -1.0f;
				AtualizarDirecaoRays(moveDirection); 
			}
			
			velocity.X = direction * velocidadeAtual; 
		}
		else if (wallJumpTimer <= 0) 
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, velocidadeAtual);
		}

		if (!IsOnFloor() && Input.IsActionPressed("ui_down") && Input.IsActionJustPressed("ui_up")) isSlamming = true;
		
		// ---> MODIFICADO: Verifica se o cooldown permite rolar/dash
		if (Input.IsActionJustPressed("dash") && !isSlamming && !isAttacking && _dashCooldownTimer <= 0) 
		{
			if (IsOnFloor()) { isRolling = true; actionTimer = ActionDuration; _dashCooldownTimer = DashCooldownDuration; }
			else if (canAirDash) { isDashing = true; canAirDash = false; actionTimer = ActionDuration; _dashCooldownTimer = DashCooldownDuration; }
		}

		isHoldingWall = false;
		if (!IsOnFloor() && IsOnWall() && canWallInteract)
		{
			Vector2 wallNormal = GetWallNormal();
			if ((wallNormal.X < 0 && direction > 0) || (wallNormal.X > 0 && direction < 0))
			{
				isHoldingWall = true;
			}
		}

		if (!IsOnFloor())
		{
			float appliedGravity = velocity.Y > 0 ? gravity * FallGravityMultiplier : gravity;
			
			if (isHoldingWall && !Input.IsActionPressed("ui_down") && !isAttacking) 
			{
				if (velocity.Y > 0)
					velocity.Y = Mathf.MoveToward(velocity.Y, WallSlideSpeed, appliedGravity * (float)delta);
				else
					velocity.Y += appliedGravity * (float)delta;
			}
			else
			{
				velocity.Y += appliedGravity * (float)delta; 
			}
		}
		else 
		{ 
			canAirDash = false; canWallInteract = true; _jumpCount = 0; wallReattachTimer = 0.0f; 
		}

		if (Input.IsActionJustPressed("ui_up") && !isAttacking)
		{
			if (Input.IsActionPressed("ui_down") && IsOnFloor())
			{
				Position = new Vector2(Position.X, Position.Y + 2);
				_cooldownDescida = 0.4f; 
			}
			else if (IsOnFloor()) 
			{ 
				velocity.Y = JumpVelocity; _jumpCount = 1; canAirDash = true; 
			}
			else if (IsOnWall() && canWallInteract && !Input.IsActionPressed("ui_down")) 
			{
				Vector2 wallNormal = GetWallNormal(); 
				velocity.Y = JumpVelocity; velocity.X = wallNormal.X * WallJumpPushForce; 
				moveDirection = wallNormal.X; wallJumpTimer = WallJumpLockDuration; 
				canAirDash = true; canWallInteract = false; wallReattachTimer = 1.5f;
				_jumpCount = 1; 
			}
			else if (_jumpCount < 2 && !Input.IsActionPressed("ui_down")) 
			{ 
				velocity.Y = DoubleJumpVelocity; _jumpCount = 2; 
			}
		}

		if (_impulsoTimer > 0)
		{
			_impulsoTimer -= (float)delta;
			velocity = _impulsoVelocidade;
		}

		if (!IsOnFloor() && velocity.Y >= 0 && !Input.IsActionPressed("ui_down") && _cooldownDescida <= 0) 
		{
			_rayPeito.ForceRaycastUpdate();
			_rayCabeca.ForceRaycastUpdate();

			if (_rayPeito.IsColliding() && !_rayCabeca.IsColliding())
			{
				Node collider = (Node)_rayPeito.GetCollider();
				if (!(collider is Area2D) && !collider.IsInGroup("Player"))
				{
					TentarEscalada(collider, false);
					return; 
				}
			}

			if (_rayPlataforma != null)
			{
				_rayPlataforma.ForceRaycastUpdate();
				if (_rayPlataforma.IsColliding())
				{
					Node colisor = (Node)_rayPlataforma.GetCollider();
					if (colisor != null && colisor.Name.ToString().ToLower().Contains("plataforma"))
					{
						TentarEscalada(colisor, true);
						return;
					}
				}
			}
		}

		Velocity = velocity;
		MoveAndSlide();
		UpdateAnimation();
	}

	private void IniciarAtaque()
	{
		isAttacking = true;
		if (_armaNasCostas != null) _armaNasCostas.Visible = false;
		_pivotArma.Visible = true;

		if (_colisaoHitbox != null) _colisaoHitbox.SetDeferred("disabled", false);

		Vector2 mouseGlobal = GetGlobalMousePosition();
		Vector2 direcaoMouse = (mouseGlobal - GlobalPosition).Normalized();
		
		attackDirection = direcaoMouse.X > 0 ? 1.0f : -1.0f;

		AtualizarDirecaoRays(attackDirection);

		_pivotArma.Position = new Vector2(Mathf.Abs(_posicaoBasePivot.X) * attackDirection, _posicaoBasePivot.Y);
		
		_spriteArma.FlipV = false; 
		_spriteArma.FlipH = attackDirection < 0; 
		_spriteArma.RotationDegrees = ArmaEquipada.RotacaoSpriteBaseGraus; 
		
		if (_spriteMao != null)
		{
			_spriteMao.FlipV = false;
			_spriteMao.FlipH = attackDirection < 0;
		}

		anim.FlipH = attackDirection < 0; 

		float anguloBase = 0f;

		if (ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.MeleeDirecional || ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.Estocada)
			anguloBase = attackDirection > 0 ? 0f : Mathf.Pi;
		else
			anguloBase = direcaoMouse.Angle();

		if (ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.Estocada || ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.EstocadaLivre)
		{
			_impulsoTimer = ArmaEquipada.TempoAtaque * 0.5f; 
			if (ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.Estocada)
			{
				_impulsoVelocidade = new Vector2(attackDirection * ArmaEquipada.ForcaImpulsoEstocada, 0); 
			}
			else
			{
				_impulsoVelocidade = direcaoMouse * ArmaEquipada.ForcaImpulsoEstocada; 
			}
		}

		Tween tween = CreateTween();

		if (ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.MeleeDirecional || ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.MeleeLivre)
		{
			float anguloInicio = Mathf.DegToRad(ArmaEquipada.AnguloInicioCorteGraus);
			float anguloFim = Mathf.DegToRad(ArmaEquipada.AnguloFimCorteGraus);
			
			float inicioReal = anguloBase + (anguloInicio * attackDirection);
			float fimReal = anguloBase + (anguloFim * attackDirection);
			
			_pivotArma.Rotation = inicioReal; 
			
			tween.TweenProperty(_pivotArma, "rotation", fimReal, ArmaEquipada.TempoAtaque)
				 .SetTrans(Tween.TransitionType.Sine)
				 .SetEase(Tween.EaseType.InOut);
		}
		else if (ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.Estocada || ArmaEquipada.EstiloAtaque == ItemData.TipoAtaque.EstocadaLivre)
		{
			_pivotArma.Rotation = anguloBase;
			_spriteArma.Position = Vector2.Zero; 
			
			float metadeTempo = ArmaEquipada.TempoAtaque / 2;
			tween.TweenProperty(_spriteArma, "position", new Vector2(ArmaEquipada.DistanciaEstocada, 0), metadeTempo);
			tween.TweenProperty(_spriteArma, "position", Vector2.Zero, metadeTempo);
		}

		tween.Finished += AoTerminarAtaque;

		if (anim.SpriteFrames.HasAnimation("ataque")) anim.Play("ataque");
		else anim.Play("idle"); 
	}

	private void AoTerminarAtaque()
	{
		isAttacking = false;
		_pivotArma.Visible = false; 
		_spriteArma.Position = Vector2.Zero; 
		_impulsoTimer = 0.0f; 

		if (_colisaoHitbox != null) _colisaoHitbox.SetDeferred("disabled", true);

		if (_armaNasCostas != null && ArmaEquipada != null) _armaNasCostas.Visible = true;
		
		AtualizarDirecaoRays(moveDirection);
	}

	private void AoAcertarInimigo(Node2D body)
	{
		if (body.IsInGroup("Inimigo"))
		{
			GD.Print("VRAU! Espadada acertou o: " + body.Name);
		}
	}

	private void TentarEscalada(Node objetoAtingido, bool isPlataforma)
	{
		isClimbingLedge = true;
		Velocity = Vector2.Zero; 
		_impulsoTimer = 0.0f; 
		
		_posicaoAlvoTeleporte = GlobalPosition;

		float direcaoEscalada = isAttacking ? attackDirection : moveDirection;

		if (_armaNasCostas != null && ArmaEquipada != null)
		{
			Vector2 pos = ArmaEquipada.PosicaoGuardada;
			
			if (direcaoEscalada < 0)
			{
				_armaNasCostas.Position = new Vector2(-pos.X - AfastarArmaNaEscalada, pos.Y);
				_armaNasCostas.RotationDegrees = -ArmaEquipada.RotacaoGuardada; 
				_armaNasCostas.FlipH = true; 
			}
			else
			{
				_armaNasCostas.Position = new Vector2(pos.X + AfastarArmaNaEscalada, pos.Y);
				_armaNasCostas.RotationDegrees = ArmaEquipada.RotacaoGuardada;
				_armaNasCostas.FlipH = false;
			}
			
			_armaNasCostas.Visible = true; 
			if (isAttacking) _pivotArma.Visible = false;
		}

		if (isPlataforma)
		{
			_posicaoAlvoTeleporte.Y += OffsetTeleportQuina.Y; 
		}
		else
		{
			bool marcadorEncontrado = false;
			if (objetoAtingido != null)
			{
				Node noEncontrado = objetoAtingido.FindChild("PontoQuina", true, false);
				
				if (noEncontrado != null && noEncontrado is Marker2D ponto)
				{
					_posicaoAlvoTeleporte = ponto.GlobalPosition;
					marcadorEncontrado = true;
				}
			}

			if (!marcadorEncontrado)
			{
				_posicaoAlvoTeleporte.X += OffsetTeleportQuina.X * direcaoEscalada;
				_posicaoAlvoTeleporte.Y += OffsetTeleportQuina.Y;
			}
		}

		if (anim.SpriteFrames.HasAnimation("escalar_quina")) anim.Play("escalar_quina");
		
		if (SubidaSuave)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(this, "global_position", _posicaoAlvoTeleporte, TempoEscalada);
			tween.Finished += FinalizarEscalada;
		}
		else
		{
			GetTree().CreateTimer(TempoEscalada).Timeout += FinalizarEscalada; 
		}
	}

	private void FinalizarEscalada()
	{
		if (!isClimbingLedge) return;

		GlobalPosition = _posicaoAlvoTeleporte;
		
		isClimbingLedge = false; 
		Velocity = new Vector2(0, 10); 
		anim.Play("idle");
		
		if (isAttacking) AoTerminarAtaque();
	}

	private void UpdateAnimation()
	{
		if (isAttacking || isClimbingLedge) return; 

		anim.FlipH = moveDirection < 0;

		if (_pivotArma != null)
		{
			_pivotArma.Position = new Vector2(Mathf.Abs(_posicaoBasePivot.X) * (moveDirection > 0 ? 1 : -1), _posicaoBasePivot.Y);
		}

		if (ArmaEquipada != null && _armaNasCostas != null)
		{
			Vector2 posicaoGuardaAtual = ArmaEquipada.PosicaoGuardada;
			
			if (isRolling) posicaoGuardaAtual.Y += ArmaEquipada.DescerArmaAoDeslizar;

			if (moveDirection < 0) 
			{
				_armaNasCostas.Position = new Vector2(-posicaoGuardaAtual.X, posicaoGuardaAtual.Y);
				_armaNasCostas.RotationDegrees = -ArmaEquipada.RotacaoGuardada; 
				_armaNasCostas.FlipH = true; 
				_armaNasCostas.Scale = ArmaEquipada.EscalaGuardada; 
			}
			else 
			{
				_armaNasCostas.Position = posicaoGuardaAtual;
				_armaNasCostas.RotationDegrees = ArmaEquipada.RotacaoGuardada;
				_armaNasCostas.FlipH = false;
				_armaNasCostas.Scale = ArmaEquipada.EscalaGuardada;
			}
		}

		if (isImpacting) anim.Play("impact");
		else if (isSlamming) anim.Play("fall"); 
		else if (isRolling) anim.Play("roll");
		else if (isDashing) anim.Play("dash");
		else if (!IsOnFloor())
		{
			if (isHoldingWall && Velocity.Y > 0) anim.Play("wall");
			else if (Velocity.Y < 0) anim.Play("jump");
			else anim.Play("fall"); 
		}
		else
		{
			if (Velocity.X != 0) anim.Play("run");
			else anim.Play("idle");
		}
	} 
}
