extends CharacterBody2D

# Variáveis customizáveis no Inspector
@export var VELOCIDADE: float = 800.0
@export var FORCA_PULO: float = -500.0
@export var FORCA_PULO_DUPLO: float = -300.0
@export var MULTIPLICADOR_QUEDA: float = 3.0
@export var CORTE_PULO: float = 0.5 # Corta a força do pulo pela metade se soltar o botão!

var gravidade = ProjectSettings.get_setting("physics/2d/default_gravity")
var pode_pulo_duplo = false

func _ready():
	$ParticulaPulo.visible = false
	$ParticulaPulo.animation_finished.connect(_on_particula_terminou)

func _on_particula_terminou():
	$ParticulaPulo.visible = false
	$ParticulaPulo.stop()

func _physics_process(delta):
	# 1. GRAVIDADE
	if not is_on_floor():
		if velocity.y > 0:
			velocity.y += gravidade * MULTIPLICADOR_QUEDA * delta
		else:
			velocity.y += gravidade * delta

	# Recarrega o pulo duplo
	if is_on_floor():
		pode_pulo_duplo = true

	# 2. PULO E PULO DUPLO
	if Input.is_action_just_pressed("pular"):
		if is_on_floor():
			velocity.y = FORCA_PULO
		elif pode_pulo_duplo:
			velocity.y = FORCA_PULO_DUPLO
			pode_pulo_duplo = false
			
			$ParticulaPulo.visible = true
			$ParticulaPulo.frame = 0
			$ParticulaPulo.play("vento")

	# --------------------------------------------------------
	# A MÁGICA DO PULO VARIÁVEL (ESTILO DEAD CELLS)
	# Se a tecla pular for SOLTA (just_released) E o boneco ainda estiver subindo (y < 0)
	if Input.is_action_just_released("pular") and velocity.y < 0:
		velocity.y *= CORTE_PULO # Multiplica a velocidade de subida por 0.5 (corta pela metade)
	# --------------------------------------------------------

	# 3. ANDAR E VIRAR O ROSTO
	var direcao = Input.get_axis("mover_esquerda", "mover_direita")
	
	if direcao != 0:
		velocity.x = direcao * VELOCIDADE
		$AnimatedSprite2D.play("correr")
		
		if direcao < 0:
			$AnimatedSprite2D.flip_h = true
		elif direcao > 0:
			$AnimatedSprite2D.flip_h = false
	else:
		velocity.x = move_toward(velocity.x, 0, VELOCIDADE)
		
		if is_on_floor():
			$AnimatedSprite2D.play("parado")

	move_and_slide()
