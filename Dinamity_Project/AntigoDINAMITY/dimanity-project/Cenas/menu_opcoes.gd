extends Control

func _ready():
	# Conecta o botão de voltar
	$BotaoVoltar.pressed.connect(_on_botao_voltar_pressed)

func _on_botao_voltar_pressed():
	# Volta para o Menu Principal!
	get_tree().change_scene_to_file("res://Cenas/menu_principal.tscn")
