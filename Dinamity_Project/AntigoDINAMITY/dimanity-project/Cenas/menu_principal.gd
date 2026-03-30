extends Control

# A função _ready é executada assim que o menu é aberto
func _ready():
	# Vamos ligar os botões a este script. 
	# O símbolo $ serve para aceder a um nó que está dentro da nossa cena.
	$VBoxContainer/BotaoComecar.pressed.connect(_on_comecar_pressed)
	$VBoxContainer/BotaoMultiplayer.pressed.connect(_on_multiplayer_pressed)
	$VBoxContainer/BotaoOpcoes.pressed.connect(_on_opcoes_pressed)
	$VBoxContainer/BotaoSair.pressed.connect(_on_sair_pressed)

# O NOME AQUI AGORA ESTÁ IGUAL AO LÁ DE CIMA!
func _on_comecar_pressed():
	print("Indo para a Criação de Personagem...")
	# O comando abaixo fecha o menu e abre a tela de criação!
	get_tree().change_scene_to_file("res://Cenas/criacao_personagem.tscn")
	
func _on_multiplayer_pressed():
	print("A abrir o lobby multiplayer...")

func _on_opcoes_pressed():
	print("Abrindo o menu de opções...")
	get_tree().change_scene_to_file("res://Cenas/menu_opcoes.tscn")

func _on_sair_pressed():
	print("A fechar o Dimanity. Até à próxima!")
	get_tree().quit() # Este é o comando oficial para fechar o jogo
