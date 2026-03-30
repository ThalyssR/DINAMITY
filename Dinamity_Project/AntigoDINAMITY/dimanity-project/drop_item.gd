extends Resource
class_name DropItem

# Aqui você vai arrastar a sua "katana_de_fogo.tres" ou qualquer outro item
@export var item: DadosItem 
# A chance do item cair (De 0 a 100%)
@export_range(0.0, 100.0) var chance_drop: float = 20.0
