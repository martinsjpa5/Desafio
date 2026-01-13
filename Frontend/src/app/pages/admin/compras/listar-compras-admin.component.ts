import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgFor, NgIf } from '@angular/common';
import { CompraService } from '../../../domain/services/compra.service';
import { CompraListarResponse } from '../../../domain/models/compra-listar.model';
import { LoadingService } from '../../../core/services/loading.service';
import { ToastService } from '../../../core/services/toast.service';
import { ApiErrorHelper } from '../../../core/helpers/api-error.helper';

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

  constructor(private compraService: CompraService, private loadingService: LoadingService, private toastService: ToastService) { }

  ngOnInit() {
    this.buscarCompras();
  }

  async buscarCompras() {
    this.loadingService.show();

    try {
      let response = await this.compraService.listarComprasAdmin();
      this.compras = response.data;
    } catch (error: any) {
      this.toastService.error(ApiErrorHelper.getApiErrorMessage(error));
    } finally {
      this.loadingService.hide();
    }
  }

  async cancelarCompra(compraId: number) {
    if (!confirm('Deseja realmente cancelar esta compra?')) return;
    this.loadingService.show();
    try {
      await this.compraService.cancelarCompra(compraId);
      this.toastService.success("Compra Cancelada com sucesso!")
      this.buscarCompras();
    }
    catch (error: any) {
     this.toastService.error(ApiErrorHelper.getApiErrorMessage(error));
    }
    finally {
      this.loadingService.hide();
    }
  }
}
