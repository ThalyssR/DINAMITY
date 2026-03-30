extends CanvasLayer

func _ready():
	# O inventário começa escondido quando o jogo abre
	$InventarioUI.visible = false

func _process(_delta):
	# Checa se o jogador apertou o TAB
	if Input.is_action_just_pressed("abrir_inventario"):
		# Inverte a visibilidade (se tá falso, vira verdadeiro, e vice-versa)
		$InventarioUI.visible = not $InventarioUI.visible
		
		# (Opcional) Pausa o jogo quando o inventário abre, igual Minecraft!
		# get_tree().paused = $InventarioUI.visible
