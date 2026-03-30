@tool
extends TextureRect

# Quando você arrasta o .tres para cá, ele roda a função _atualizar_item() na mesma hora!
@export var dados: DadosItem:
	set(novo_dado):
		dados = novo_dado
		_atualizar_item()

var categoria: String = ""
var duas_maos: bool = false

func _ready():
	_atualizar_item()

func _atualizar_item():
	if dados != null:
		texture = dados.icone # Puxa a imagem
		categoria = dados.categoria # Puxa as regras
		duas_maos = dados.usa_duas_maos
	else:
		texture = null # Fica vazio se não tiver nenhum dado
