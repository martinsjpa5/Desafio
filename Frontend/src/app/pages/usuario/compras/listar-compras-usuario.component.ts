import { Component, OnInit } from '@angular/core';
import { CompraListarResponse } from '../../../domain/models/compra-listar.model';
import { CompraService } from '../../../domain/services/compra.service';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../core/services/loading.service';
import { ToastService } from '../../../core/services/toast.service';
import { ApiErrorHelper } from '../../../core/helpers/api-error.helper';

@Component({
    standalone: true,
    imports: [CommonModule],
    selector: 'app-listar-compras-usuario',
    templateUrl: './listar-compras-usuario.component.html',
    styleUrls: ['./listar-compras-usuario.component.scss']
})
export class ListarComprasUsuarioComponent implements OnInit {
    compras: CompraListarResponse[] = [];

    constructor(private compraService: CompraService, private loadingService: LoadingService, private toastService: ToastService) { }

    ngOnInit() {
        this.buscarCompras();
    }

    async buscarCompras() {
        this.loadingService.show();
        try {
            let response = await this.compraService.listarComprasUsuario();
            this.compras = response.data;

        }
        catch (error: any) {
            this.toastService.error(ApiErrorHelper.getApiErrorMessage(error));
        }
        finally {
            this.loadingService.hide();
        }
    }
}
