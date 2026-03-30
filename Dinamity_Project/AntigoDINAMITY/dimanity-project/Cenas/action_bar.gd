extends HBoxContainer

@export var slot_arma_1: TextureRect
@export var slot_arma_2: TextureRect

@export_group("Backgrounds (O Fundo por Raridade)")
@export var fundo_comum: Texture2D
@export var fundo_incomum: Texture2D
@export var fundo_raro: Texture2D
@export var fundo_epico: Texture2D
@export var fundo_lendario: Texture2D
@export var fundo_artefato: Texture2D

@export_group("Bordas (Estáticas e Animadas)")
@export var borda_comum: Texture2D
@export var borda_incomum: Texture2D
@export var borda_raro: Texture2D
@export var borda_epico: Texture2D
@export var borda_lendario: Texture2D 
@export var borda_artefato: Texture2D 

# Puxando o Fundo E a Borda pela NOVA estrutura (SlotBase)
@onready var fundo_1 = $SlotBase1/Background1
@onready var borda_1 = $SlotBase1/Borda1
@onready var icone_1 = $SlotBase1/IconeArma

@onready var fundo_2 = $SlotBase2/Background2
@onready var borda_2 = $SlotBase2/Borda2
@onready var icone_2 = $SlotBase2/IconeArma

func _ready():
	# Define a textura Comum (padrão) logo que o jogo abre!
	fundo_1.texture = fundo_comum
	borda_1.texture = borda_comum
	fundo_2.texture = fundo_comum
	borda_2.texture = borda_comum

func _process(_delta):
	_atualizar_slot(slot_arma_1, icone_1, borda_1, fundo_1)
	_atualizar_slot(slot_arma_2, icone_2, borda_2, fundo_2)

func _atualizar_slot(slot_origem, icone_ui, borda_ui, fundo_ui):
	if slot_origem != null and slot_origem.get_child_count() > 0:
		var item = slot_origem.get_child(0)
		icone_ui.texture = item.texture
		icone_ui.visible = true
		
		if item.dados != null:
			match item.dados.raridade:
				"comum": 
					borda_ui.texture = borda_comum
					fundo_ui.texture = fundo_comum
				"incomum": 
					borda_ui.texture = borda_incomum
					fundo_ui.texture = fundo_incomum
				"raro": 
					borda_ui.texture = borda_raro
					fundo_ui.texture = fundo_raro
				"epico": 
					borda_ui.texture = borda_epico
					fundo_ui.texture = fundo_epico
				"lendario": 
					borda_ui.texture = borda_lendario
					fundo_ui.texture = fundo_lendario
				"artefato": 
					borda_ui.texture = borda_artefato
					fundo_ui.texture = fundo_artefato
	else:
		# Se desequipar, volta a borda e o fundo para o Comum!
		icone_ui.visible = false
		borda_ui.texture = borda_comum
		fundo_ui.texture = fundo_comum
