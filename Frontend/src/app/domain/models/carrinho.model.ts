import { ProdutoListarResponse } from "./produto.model";

export interface CarrinhoItem {
  produto: ProdutoListarResponse;
  quantidade: number;
}