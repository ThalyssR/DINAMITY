extends Resource
class_name DadosItem

# AQUI ESTÃO AS SUAS LISTAS! (Pode adicionar os outros depois separando por vírgula)
enum Debuff { NENHUM, SANGRAMENTO, FOGO, FOGO_AZUL, OLEO, VENENO, CHOQUE, CONGELAMENTO, LENTIDAO, ATORDOAMENTO }
enum Passiva { NENHUM, DANO_EXTRA_QUEIMANDO, DANO_EXTRA_VENENO, DANO_EXTRA_SANGRAMENTO, APLICA_SANGRAMENTO, APLICA_FOGO, EXPLODE_AO_MORRER, DISPARA_FLECHAS, ROUBO_DE_VIDA, REDUZ_COOLDOWN_KILL, PARRY_APLICA_FOGO }

@export_group("Visual e Identidade")
@export var nome_do_item: String = "Nova Arma"
@export var icone: Texture2D
@export var sprites_de_ataque: Texture2D 
@export_enum("comum", "capacete", "peitoral", "calca", "botas", "arma", "acessorio") var categoria: String = "arma"
@export_enum("comum", "incomum", "raro", "epico", "lendario", "artefato") var raridade: String = "comum"
@export var level_romano: String = "I" 
@export var usa_duas_maos: bool = false

@export_group("Atributos de Combate")
@export var dano: int = 10
@export var velocidade_ataque: float = 1.0

@export_group("Efeitos e Passivas (Listas Selecionáveis)")
# O Array cria o botão de "+" no Inspetor para você adicionar várias opções!
@export var debuffs: Array[Debuff] = []
@export var passivas: Array[Passiva] = []

@export_group("Descrição (Tooltip)")
@export_multiline var descricao_extra: String = ""
