import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgFor, NgIf } from '@angular/common';
import { CompraService } from '../../../domain/services/compra.service';
import { CompraListarResponse } from '../../../domain/models/compra-listar.model';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-listar-compras',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './listar-compras-admin.component.html',
  styleUrls: ['./listar-compras-admin.component.scss']
})
export class ListarComprasAdminComponent implements OnInit {

  compras: CompraListarResponse[] = [];
  carregando = false;
  errorMessage = '';

  constructor(private compraService: CompraService, private loadingService: LoadingService) { }

  ngOnInit() {
    this.buscarCompras();
  }

  async buscarCompras() {
    this.loadingService.show();
    this.errorMessage = '';

    try {
      let response = await this.compraService.listarComprasAdmin();
      this.compras = response.data;
    } catch (error: any) {
      if (error?.error?.erros) {
        this.errorMessage = error.error.erros.join('<br>');
      }
      else {
        this.errorMessage = 'Erro ao carregar compras.';
      }
    } finally {
      this.loadingService.hide();
    }
  }

  async cancelarCompra(compraId: number) {
    if (!confirm('Deseja realmente cancelar esta compra?')) return;
    this.loadingService.show();
    try {
      const resposta = await this.compraService.cancelarCompra(compraId);
      this.buscarCompras();
    }
    catch (error: any) {
      if (error?.error?.erros) {
        this.errorMessage = error.error.erros.join('<br>');
      }
      else{
        this.errorMessage = "erro ao cancelar compra";
      }
      
    }
    finally {
      this.loadingService.hide();
    }
  }
}
