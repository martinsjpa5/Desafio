import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ProdutoListarResponse } from '../../../domain/models/produto.model';
import { ProdutoService } from '../../../domain/services/produto.service';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-listar-produtos-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgbModule],
  templateUrl: './listar-produtos-admin.component.html',
  styleUrls: ['./listar-produtos-admin.component.scss']
})
export class ListarProdutosAdminComponent implements OnInit {

  produtos: ProdutoListarResponse[] = [];
  errorMessage = '';
  produtoForm: FormGroup;
  modalTitle = '';
  modalRef: any;

  constructor(
    private produtoService: ProdutoService,
    private modalService: NgbModal,
    private fb: FormBuilder,
    private loadingService: LoadingService
  ) {
    this.produtoForm = this.fb.group({
      id: [0],
      nome: ['', [Validators.required, Validators.maxLength(100)]],
      quantidadeEstoque: [0, [Validators.required, Validators.min(0.01)]],
      valor: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.buscarProdutos();
  }

  async buscarProdutos() {
    this.errorMessage = '';
    this.loadingService.show();
    try {
      this.produtos = await this.produtoService.listar();
    } catch (error) {
      this.errorMessage = 'Erro ao carregar produtos.';
    }
    finally{
      this.loadingService.hide();
    }
  }

  abrirModal(modal: any, produto?: ProdutoListarResponse) {
    if (produto) {
      this.produtoForm.patchValue(produto);
      this.modalTitle = 'Editar Produto';
    } else {
      this.produtoForm.reset({ id: 0, nome: '', quantidadeEstoque: 0, valor: 0 });
      this.modalTitle = 'Novo Produto';
    }
    this.modalRef = this.modalService.open(modal, { centered: true });
  }

  async salvarProduto() {
    if (this.produtoForm.invalid) {
      this.produtoForm.markAllAsTouched();
      return;
    }
    this.loadingService.show();
    const produto = this.produtoForm.value as ProdutoListarResponse;

    try {
      if (produto.id === 0) {
        await this.produtoService.cadastrar(produto);
      } else {
        await this.produtoService.editar(produto);
      }
      await this.buscarProdutos();
      this.modalRef.dismiss();
    } catch (error: any) {
      if (error?.error?.erros) {
        this.errorMessage = error.error.erros.join('<br>');
      }
      else {
        this.errorMessage = 'Erro ao salvar produto.';
      }
    }
    finally{
      this.loadingService.hide();
    }
  }

  async excluirProduto(produto: ProdutoListarResponse) {
    if (!confirm(`Deseja realmente excluir o produto "${produto.nome}"?`)) return;

    try {
      await this.produtoService.deletar(produto.id);
      this.buscarProdutos();
    } catch (error: any) {
      if (error?.error?.erros) {
        this.errorMessage = error.error.erros.join('<br>');
      }
      else {

        this.errorMessage = 'Erro ao excluir produto.';
      }
    }
  }
}
