export interface ProdutoListarResponse {
  id: number;
  nome: string;
  quantidadeEstoque: number;
  valor: number;
}

export interface ItemListarResponse {
    nome: string;
    quantidade: number;
    valor: number;
}


export interface ProdutoAdicionarRequest {
  nome: string;   
  quantidadeEstoque: number;  
  valor: number;             
}

export interface ProdutoEditarRequest {
  id: number;
  nome: string;   
  quantidadeEstoque: number;  
  valor: number;             
}