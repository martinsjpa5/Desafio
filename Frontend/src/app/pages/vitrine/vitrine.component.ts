import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProdutoService } from '../../domain/services/produto.service';
import { ProdutoListarResponse } from '../../domain/models/produto.model';
import { LoadingService } from '../../core/services/loading.service';
import { CarrinhoItem } from '../../domain/models/carrinho.model';
import { CarrinhoComponent } from '../../shared/components/carrinho/carrinho.component';
import { CarrinhoService } from '../../domain/services/carrinho.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
    selector: 'app-vitrine',
    standalone: true,
    imports: [CommonModule, CarrinhoComponent],
    templateUrl: './vitrine.component.html',
    styleUrls: ['./vitrine.component.scss']
})
export class VitrineComponent implements OnInit {

    produtos: ProdutoListarResponse[] = [];
    errorMessage = '';
    carrinho: CarrinhoItem[] = [];
    private destroyRef = inject(DestroyRef);

    constructor(
        private produtoService: ProdutoService,
        private loading: LoadingService,
        private carrinhoService: CarrinhoService
    ) { }

    ngOnInit(): void {
        this.buscarProdutos();
        this.subscribeCarrinho();
    }

    subscribeCarrinho() {
        this.carrinhoService.compraEfetuada$
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(() => {
                this.buscarProdutos();
            });
    }

    async buscarProdutos() {
        this.errorMessage = '';
        this.loading.show();

        try {
            this.produtos = await this.produtoService.listar();
        } catch (error: any) {
            if (error?.error?.erros) {
                this.errorMessage = error.error.erros.join('<br>');
            }
            else
                this.errorMessage = 'Erro ao carregar produtos.';
            console.error(error);
        } finally {
            this.loading.hide();
        }
    }

    adicionarAoCarrinho(produto: ProdutoListarResponse) {
        this.carrinhoService.adicionar(produto);
        this.carrinho = this.carrinhoService.itens;
    }

}
