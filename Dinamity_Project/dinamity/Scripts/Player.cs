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
	public const float FallGravityMultiplier = 1.9f; 

	// ---> CONFIGURAÇÃO DA QUINA <---
	[Export] public Vector2 OffsetTeleportQuina = new Vector2(30, -60); 
	[Export] public float TempoParaDeslizarParede = 0.5f; 
	private float _tempoEncostadoNaParede = 0.0f; 

	// --- CONTROLE DE ESTADOS ---
	private bool isSlamming = false; 
	private bool isImpacting = false; 
	private float impactTimer = 0.0f;
	private float wallJumpTimer = 0.0f;
	private float actionTimer = 0.0f;
	private bool isRolling = false;
	private bool isDashing = false;
	private float moveDirection = 1.0f; 
	private bool canAirDash = false; 
	private bool canWallInteract = true; 
	private bool isAttacking = false;
	
	private bool _inMenu = false;
	private bool isClimbingLedge = false;
	private int _jumpCount = 0;
	private float wallReattachTimer = 0.0f;

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// --- REFERÊNCIAS VISUAIS ---
	private AnimatedSprite2D anim;
	private Sprite2D _armaNasCostas;
	private AnimatedSprite2D _efeitoAtaque; 
	
	private RayCast2D _rayPeito;
	private RayCast2D _rayCabeca;

	[Export] public ItemData ArmaEquipada; 

	public override void _Ready()
	{
		AddToGroup("Player"); 

		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_armaNasCostas = GetNodeOrNull<Sprite2D>("ArmaNasCostas");
		_efeitoAtaque = GetNodeOrNull<AnimatedSprite2D>("EfeitoAtaque");
		
		_rayPeito = GetNode<RayCast2D>("RayPeito");
		_rayCabeca = GetNode<RayCast2D>("RayCabeca");

		if (_efeitoAtaque != null)
		{
			_efeitoAtaque.AnimationFinished += AoTerminarAtaque;
			_efeitoAtaque.Visible = false; 
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
			
			// ---> A MÁGICA DE LER O .TRES AQUI! <---
			if (_efeitoAtaque != null)
			{
				_efeitoAtaque.SpriteFrames = ArmaEquipada.AnimacaoAtaque;
				_efeitoAtaque.Position = ArmaEquipada.PosicaoAtaque;
				_efeitoAtaque.Scale = ArmaEquipada.EscalaAtaque;
			}
		}
		else if (_armaNasCostas != null) _armaNasCostas.Visible = false;
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

		if (isAttacking && IsOnFloor())
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		Vector2 velocity = Velocity;
		if (wallJumpTimer > 0) wallJumpTimer -= (float)delta;

		if (!canWallInteract)
		{
			wallReattachTimer -= (float)delta;
			if (wallReattachTimer <= 0) canWallInteract = true;
		}

		if (Input.IsActionJustPressed("ataque") && !isAttacking && !isRolling && !isSlamming && ArmaEquipada != null)
		{
			IniciarAtaque();
			return; 
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
			if (IsOnFloor()) { isSlamming = false; isImpacting = true; impactTimer = ImpactDuration; }
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
		if (direction != 0 && wallJumpTimer <= 0) 
		{
			moveDirection = direction > 0 ? 1.0f : -1.0f;
			
			float tamanhoRaio = Mathf.Abs(_rayPeito.TargetPosition.X);
			_rayPeito.TargetPosition = new Vector2(tamanhoRaio * moveDirection, _rayPeito.TargetPosition.Y);
			_rayCabeca.TargetPosition = new Vector2(tamanhoRaio * moveDirection, _rayCabeca.TargetPosition.Y);
		}
		else if (wallJumpTimer <= 0) 
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		if (!IsOnFloor() && Input.IsActionPressed("ui_down") && Input.IsActionJustPressed("ui_up")) isSlamming = true;
		if (Input.IsActionJustPressed("dash") && !isSlamming)
		{
			if (IsOnFloor()) { isRolling = true; actionTimer = ActionDuration; }
			else if (canAirDash) { isDashing = true; canAirDash = false; actionTimer = ActionDuration; }
		}

		if (!IsOnFloor())
		{
			float appliedGravity = velocity.Y > 0 ? gravity * FallGravityMultiplier : gravity;
			
			if (IsOnWall() && canWallInteract && direction != 0) 
			{
				_tempoEncostadoNaParede += (float)delta; 
				if (_tempoEncostadoNaParede >= TempoParaDeslizarParede && velocity.Y > 0)
					velocity.Y = Mathf.MoveToward(velocity.Y, WallSlideSpeed, appliedGravity * (float)delta);
				else
					velocity.Y += appliedGravity * (float)delta;
			}
			else
			{
				_tempoEncostadoNaParede = 0.0f;
				velocity.Y += appliedGravity * (float)delta;
			}
		}
		else 
		{ 
			canAirDash = false; canWallInteract = true; _jumpCount = 0; wallReattachTimer = 0.0f; _tempoEncostadoNaParede = 0.0f; 
		}

		if (Input.IsActionJustPressed("ui_up"))
		{
			if (Input.IsActionPressed("ui_down") && IsOnFloor())
			{
				Position = new Vector2(Position.X, Position.Y + 2);
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
				_tempoEncostadoNaParede = 0.0f; _jumpCount = 1; 
			}
			else if (_jumpCount < 2 && !Input.IsActionPressed("ui_down")) 
			{ 
				velocity.Y = DoubleJumpVelocity; _jumpCount = 2; 
			}
		}

		if (wallJumpTimer <= 0 && direction != 0) velocity.X = direction * Speed;

		// ==========================================================
		// ---> DETECÇÃO DA QUINA
		// ==========================================================
		if (!IsOnFloor() && velocity.Y >= 0) 
		{
			_rayPeito.ForceRaycastUpdate();
			_rayCabeca.ForceRaycastUpdate();

			if (_rayPeito.IsColliding() && !_rayCabeca.IsColliding())
			{
				IniciarEscaladaQuina();
				return; 
			}
		}

		Velocity = velocity;
		MoveAndSlide();
		UpdateAnimation();
	}

	private void IniciarEscaladaQuina()
	{
		isClimbingLedge = true;
		Velocity = Vector2.Zero; 
		
		if (anim.SpriteFrames.HasAnimation("escalar_quina")) anim.Play("escalar_quina");
		
		GetTree().CreateTimer(0.4f).Timeout += TerminarEscaladaQuina; 
	}

	private void TerminarEscaladaQuina()
	{
		if (!isClimbingLedge) return;

		Vector2 teleportPos = GlobalPosition;
		
		teleportPos.X += OffsetTeleportQuina.X * moveDirection; 
		teleportPos.Y += OffsetTeleportQuina.Y; 
		
		GlobalPosition = teleportPos;
		isClimbingLedge = false; 
		
		Velocity = new Vector2(0, 10);
		
		anim.Play("idle");
	}

	// ==========================================================
	// ---> MÁGICA DO ATAQUE (PUXANDO DO ARQUIVO .TRES) <---
	// ==========================================================
	private void IniciarAtaque()
	{
		isAttacking = true;
		if (_armaNasCostas != null) _armaNasCostas.Visible = false;
		
		// Verifica se o SpriteFrames que veio do .tres não é nulo
		if (_efeitoAtaque != null && _efeitoAtaque.SpriteFrames != null)
		{
			_efeitoAtaque.Visible = true;
			_efeitoAtaque.Frame = 0; 
			
			// Tenta tocar a animação "ataque" se você nomeou assim. Se não, toca a primeira que achar!
			if (_efeitoAtaque.SpriteFrames.HasAnimation("ataque"))
				_efeitoAtaque.Play("ataque");
			else if (_efeitoAtaque.SpriteFrames.GetAnimationNames().Length > 0)
				_efeitoAtaque.Play(_efeitoAtaque.SpriteFrames.GetAnimationNames()[0]);
		}
		else
		{
			// Se o arquivo .tres estiver sem a animação configurada, solta o boneco rápido
			GetTree().CreateTimer(0.3f).Timeout += AoTerminarAtaque;
		}

		// Se o corpo do player tiver a animação de ataque, toca! Senão, fica idle.
		if (anim.SpriteFrames.HasAnimation("ataque")) anim.Play("ataque");
		else anim.Play("idle"); 
	}

	private void AoTerminarAtaque()
	{
		isAttacking = false;
		if (_efeitoAtaque != null) _efeitoAtaque.Visible = false;
		if (_armaNasCostas != null && ArmaEquipada != null) _armaNasCostas.Visible = true;
	}

	private void UpdateAnimation()
	{
		if (isAttacking || isClimbingLedge) return; 

		if (moveDirection < 0) 
		{
			anim.FlipH = true;
			if (ArmaEquipada != null)
			{
				if (_armaNasCostas != null) _armaNasCostas.Scale = new Vector2(-Mathf.Abs(ArmaEquipada.EscalaGuardada.X), ArmaEquipada.EscalaGuardada.Y);
				if (_efeitoAtaque != null) _efeitoAtaque.Scale = new Vector2(-Mathf.Abs(ArmaEquipada.EscalaAtaque.X), ArmaEquipada.EscalaAtaque.Y);
			}
		}
		else if (moveDirection > 0) 
		{
			anim.FlipH = false;
			if (ArmaEquipada != null)
			{
				if (_armaNasCostas != null) _armaNasCostas.Scale = new Vector2(Mathf.Abs(ArmaEquipada.EscalaGuardada.X), ArmaEquipada.EscalaGuardada.Y);
				if (_efeitoAtaque != null) _efeitoAtaque.Scale = new Vector2(Mathf.Abs(ArmaEquipada.EscalaAtaque.X), ArmaEquipada.EscalaAtaque.Y);
			}
		}

		if (isImpacting) anim.Play("impact");
		else if (isSlamming) anim.Play("fall"); 
		else if (isRolling) anim.Play("roll");
		else if (isDashing) anim.Play("dash");
		else if (!IsOnFloor())
		{
			if (IsOnWall() && canWallInteract && Velocity.Y > 0 && _tempoEncostadoNaParede >= TempoParaDeslizarParede) 
				anim.Play("wall");
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
