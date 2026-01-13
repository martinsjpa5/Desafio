import { Component, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CarrinhoService } from '../../../domain/services/carrinho.service';
import { map } from 'rxjs';
import { LoadingService } from '../../../core/services/loading.service';
import { CompraService } from '../../../domain/services/compra.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ToastService } from '../../../core/services/toast.service';
import { ApiErrorHelper } from '../../../core/helpers/api-error.helper';

@Component({
    selector: 'app-carrinho',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './carrinho.component.html',
    styleUrl: './carrinho.component.scss'
})
export class CarrinhoComponent {

    carrinho$;
    carrinhoAberto = false;

    private destroyRef = inject(DestroyRef);

    constructor(private carrinhoService: CarrinhoService,
        private loadingService: LoadingService,
        private compraService: CompraService, private toastService: ToastService) {
        this.carrinho$ = this.carrinhoService.itens$;
        this.obterCarrinho();
    }

    async obterCarrinho() {
        this.loadingService.show();
        try {
            await this.carrinhoService.Obter();

        } catch (error: any) {
            this.toastService.error(ApiErrorHelper.getApiErrorMessage(error));
        } finally {
            this.loadingService.hide();
        }
    }

    async finalizarCompra() {
        this.loadingService.show();

        try {
            const itens = this.carrinhoService.itens;

            const request = itens.map(i => ({
                produtoId: i.produto.id,
                quantidade: i.quantidade
            }));

            await this.compraService.efetuar(request);
            this.carrinhoService.compraEfetuadaComSucesso();
            this.toastService.success("Compra finalizada com sucesso! Você pode verificar em Usuário/Compras.");

        } catch (error: any) {
            this.toastService.error(ApiErrorHelper.getApiErrorMessage(error));
        } finally {
            this.loadingService.hide();
        }
    }

    atualizarQuantidade(item: any, quantidade: number) {
        if (quantidade > item.produto.quantidadeEstoque) quantidade = item.produto.quantidadeEstoque;
        if (quantidade < 1) quantidade = 1;

        this.carrinhoService.definirQuantidade(item.produto.id, quantidade);
    }

    removerDoCarrinho(id: number) {
        this.carrinhoService.remover(id);
    }

    get totalCarrinho$() {
        return this.carrinho$.pipe(
            map(items => items.reduce((acc, i) => acc + i.produto.valor * i.quantidade, 0))
        );
    }


    toggleCarrinho() {
        this.carrinhoAberto = !this.carrinhoAberto;
    }
}
