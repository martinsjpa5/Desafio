export interface ItemListarResponse {
    nome: string;
    quantidade: number;
    valor: number;
}

export interface CompraListarResponse {
    id: number;
    cancelada: boolean;
    valorTotal: number;
    itens: ItemListarResponse[];
}
