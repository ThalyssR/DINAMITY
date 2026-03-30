extends TextureRect

@export_enum("qualquer", "capacete", "peitoral", "calca", "botas", "arma", "acessorio") var tipo_aceito: String = "qualquer"

# Variável mágica para linkar a Arma1 na Arma2 e vice-versa no Inspetor
@export var slot_parceiro: Node 

func _get_drag_data(_at_position):
	if get_child_count() == 0:
		return null
		
	var item_arrastado = get_child(0)
	item_arrastado.visible = false
	
	var preview_base = Control.new() 
	var preview_img = TextureRect.new()
	preview_img.texture = item_arrastado.texture
	preview_img.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	preview_img.size = item_arrastado.size
	preview_img.position = -preview_img.size / 2.0 
	
	preview_base.add_child(preview_img)
	
	var particulas = CPUParticles2D.new()
	particulas.emission_shape = CPUParticles2D.EMISSION_SHAPE_SPHERE
	particulas.emission_sphere_radius = 15.0
	particulas.gravity = Vector2(0, -90) 
	particulas.amount = 40
	particulas.lifetime = 0.6
	particulas.scale_amount_min = 2.0
	particulas.scale_amount_max = 4.0
	particulas.color = Color(1, 1, 1, 0.5) 
	
	preview_base.add_child(particulas)
	set_drag_preview(preview_base)
	
	return item_arrastado

func _can_drop_data(_at_position, data):
	if not (data is TextureRect and "categoria" in data):
		return false
		
	var aceita_item = (tipo_aceito == "qualquer" or tipo_aceito == data.categoria)
	if not aceita_item:
		return false
		
	# --- A MÁGICA DAS DUAS MÃOS ---
	if slot_parceiro != null:
		# Regra A: Eu sou a Arma2. A Arma1 já está segurando um espadão de duas mãos?
		if slot_parceiro.get_child_count() > 0:
			var item_vizinho = slot_parceiro.get_child(0)
			if "duas_maos" in item_vizinho and item_vizinho.duas_maos:
				return false # Bloqueado! O parceiro tá usando as duas mãos!
				
		# Regra B: Eu sou a Arma1. O jogador quer soltar um Espadão aqui, mas a Arma2 tá ocupada com um escudo/adaga?
		if "duas_maos" in data and data.duas_maos:
			if slot_parceiro.get_child_count() > 0:
				return false # Precisa esvaziar a mão secundária primeiro!
	# ------------------------------

	# Regra do Swap (Inverter itens)
	if get_child_count() > 0:
		var item_aqui = get_child(0)
		var slot_de_origem = data.get_parent()
		var origem_aceita = (slot_de_origem.tipo_aceito == "qualquer" or slot_de_origem.tipo_aceito == item_aqui.categoria)
		if not origem_aceita:
			return false
			
	return true

func _drop_data(_at_position, data):
	var slot_de_origem = data.get_parent()
	var borda = 20
	
	if get_child_count() > 0:
		var item_que_ja_tava_aqui = get_child(0)
		remove_child(item_que_ja_tava_aqui)
		slot_de_origem.add_child(item_que_ja_tava_aqui)
		
		item_que_ja_tava_aqui.set_anchors_preset(Control.PRESET_FULL_RECT)
		item_que_ja_tava_aqui.offset_left = borda
		item_que_ja_tava_aqui.offset_top = borda
		item_que_ja_tava_aqui.offset_right = -borda
		item_que_ja_tava_aqui.offset_bottom = -borda
		
	slot_de_origem.remove_child(data)
	add_child(data)
	
	data.set_anchors_preset(Control.PRESET_FULL_RECT)
	data.offset_left = borda
	data.offset_top = borda
	data.offset_right = -borda
	data.offset_bottom = -borda
	data.visible = true 

func _notification(what):
	if what == NOTIFICATION_DRAG_END:
		if get_child_count() > 0:
			get_child(0).visible = true
