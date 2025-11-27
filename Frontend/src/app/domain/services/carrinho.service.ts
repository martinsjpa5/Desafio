import { DestroyRef, inject, Injectable } from '@angular/core';
import { BehaviorSubject, firstValueFrom, interval, Observable, Subject } from 'rxjs';
import { CarrinhoItem } from '../models/carrinho.model';
import { ProdutoListarResponse } from '../models/produto.model';
import { environment } from '../../../environments/environment';
import { ApiGenericResponse, ApiResponse } from '../models/api-response.model';
import { HttpClient } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Injectable({ providedIn: 'root' })
export class CarrinhoService {

  private _itens = new BehaviorSubject<CarrinhoItem[]>([]);
  private _compraEfetuada = new Subject<void>();

  public itens$ = this._itens.asObservable();
  public compraEfetuada$ = this._compraEfetuada.asObservable();

  private ultimoSnapshot: string = '';

  private apiUrl = `${environment.apiUrl}/Carrinho`;

  constructor(private http: HttpClient) {
    interval(10000)
    .pipe(takeUntilDestroyed(inject(DestroyRef)))
    .subscribe(() => this.autoSalvar());
   }

  private autoSalvar() {
    const itens = this.itens;


    if (itens.length === 0) return;

    const snapshotAtual = JSON.stringify(itens);

    if (snapshotAtual === this.ultimoSnapshot) return;

    this.ultimoSnapshot = snapshotAtual;

    this.Salvar(itens);
  }


  Salvar(request: CarrinhoItem[]): Promise<ApiResponse> {
    return firstValueFrom(this.http.post<ApiResponse>(this.apiUrl, request));
    
  }

  async Obter(): Promise<ApiGenericResponse<CarrinhoItem[]>> {
    const resposta = await firstValueFrom(
      this.http.get<ApiGenericResponse<CarrinhoItem[]>>(this.apiUrl)
    );



    this._itens.next(resposta.data);

    this.ultimoSnapshot = JSON.stringify(resposta.data);

    return resposta;
  }

  get itens() {
    return this._itens.value;
  }

  private atualizar(itens: CarrinhoItem[]) {
    this._itens.next(itens);
  }

  adicionar(produto: ProdutoListarResponse) {
    const itens = [...this.itens];
    const item = itens.find(i => i.produto.id === produto.id);

    if (item) {
      item.quantidade++;
    } else {
      itens.push({ produto, quantidade: 1 });
    }

    this.atualizar(itens);
  }

  definirQuantidade(produtoId: number, quantidade: number) {
    const itens = [...this.itens];
    const item = itens.find(i => i.produto.id === produtoId);
    if (item) item.quantidade = quantidade;
    this.atualizar(itens);
  }

  remover(produtoId: number) {
    const itens = this.itens.filter(i => i.produto.id !== produtoId);
    this.atualizar(itens);
  }

  compraEfetuadaComSucesso() {
    this.limpar();
    this._compraEfetuada.next();
  }

  limpar() {
    this._itens.next([]);
  }


}
