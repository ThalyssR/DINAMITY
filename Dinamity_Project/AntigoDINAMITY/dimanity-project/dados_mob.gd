extends Resource
class_name DadosMob

@export_group("Identidade")
@export var nome_monstro: String = "Novo Inimigo"
@export_enum("Melee", "Lançador", "Voador") var categoria: String = "Melee"

@export_group("Animações (Sprite Sheets)")
@export var sprite_parado: Texture2D
@export var sprite_correndo: Texture2D
@export var sprite_ataque: Texture2D
@export var sprite_queda: Texture2D

@export_group("Status de Combate")
@export var vida_maxima: int = 50
@export var dano_ataque: int = 10
# Deixei um espaço para você escrever o debuff do monstro (ex: "Veneno")
@export var efeito_de_ataque: String = "" 

@export_group("Sistema de Stun")
@export var hits_para_stun: int = 3 # Toma 3 porradas e fica tonto
@export var tempo_stun_segundos: float = 2.0 # Fica tonto por 2 segundos

@export_group("Economia (Garantido)")
@export var moedas_minimas: int = 8
@export var moedas_maximas: int = 10
@export var almas_minimas: int = 1
@export var almas_maximas: int = 3

@export_group("Drops de Itens (Sorte)")
# Isso cria uma lista onde você pode adicionar quantos itens quiser com suas % de chance!
@export var itens_dropaveis: Array[DropItem] = []
