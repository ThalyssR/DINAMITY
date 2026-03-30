extends Control

var cores_cabelo = [Color.WHITE, Color.RED, Color.BLUE, Color.GREEN, Color.YELLOW, Color.PURPLE]
var cor_atual = 0

func _ready():
	# Conecta os botões
	$VBoxContainer/BotaoCabelo.pressed.connect(_on_botao_cabelo_pressed)
	# Olha aqui: o nome dentro dos parênteses é exatamente igual ao da função lá embaixo
	$VBoxContainer/BotaoComecar.pressed.connect(_on_botao_comecar_pressed)

func _on_botao_cabelo_pressed():
	cor_atual += 1
	if cor_atual >= cores_cabelo.size():
		cor_atual = 0
	$Manequim/Cabelo.modulate = cores_cabelo[cor_atual]

# A função com o nome certinho!
func _on_botao_comecar_pressed():
	print("Criou o personagem! Entrando no Lobby...")
	# Verifica se o caminho e o nome do seu mundo estão exatamente assim:
	get_tree().change_scene_to_file("res://Cenas/mundo_1.tscn")
